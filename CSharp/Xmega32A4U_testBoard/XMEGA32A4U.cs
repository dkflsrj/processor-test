using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace Xmega32A4U_testBoard
{
    class XMEGA32A4U
    {
        //========================================================================================
        //=========================КЛАСС СВЯЗИ ПК С XMEGA32A4U ПО RS232===========================
        //========================================================================================
        //
        //-------------------------------------ПОЯСНЕНИЯ------------------------------------------
        //
        public const byte classVersion = 9;
        //-------------------------------------ПЕРЕМЕННЫЕ-----------------------------------------
        static SerialPort   USART;
        static RichTextBox  tracer;
        static bool         tracer_defined = false;
        List<byte> USART_buffer = new List<byte>();
        //-------------------------------------СТРУКТУРЫ------------------------------------------
        struct Response
        {
            //СТРУКТУРА: Хранилище констант - кодов откликов
            public const byte status =                  1;
            public const byte error =                   2;

            public const byte version =                 4;
            public const byte birthday =                5;
            public const byte CPU_frequency =           6;

            public const byte COX_done =                7;
            public const byte COX_busy =                8;
            public const byte COX_stoped =              9;
        };
        struct Command
        {
            //СТРУКТУРА: Хранилище констант - кодов команд
            public const byte MC_get_version =          1;
            public const byte MC_get_birthday =         2;
            public const byte MC_get_CPUfreq =          3;

            public const byte MC_wait =                 5;
            public const byte showTCD2_CNTl =           6;
            public const byte showTCD2_CNTh =           7;

            public const byte showMeByte =              10;

            public const byte LOCK =                    13;

            public const byte MC_get_status =           20;

            public const byte COA_set_timeInterval =    30;
            public const byte COA_start =               31;
            public const byte COA_get_count =           32;
            public const byte COA_stop =                33;
            public const byte RTC_set_prescaler =       34;

            public const byte DAC_set_voltage =         40;
            public const byte ADC_get_voltage =         41;

            public const byte KEY =                     58;
        };
        public struct _RTC
        {
            //СТРУКТУРА: Счётчик реального времени - хранилище констант
            const double sourceFrequency     = 32.768;//кГц - опорная частота таймера
            const ushort maxCount = 65535;
            //интервалы для делителей
            const int    min_ms_div1         = 0;
            const int    min_ms_div2         = 2000;
            const int    min_ms_div8         = 4000;
            const int    min_ms_div16        = 16000;
            const int    min_ms_div64        = 32000;
            const int    min_ms_div256       = 127996;
            const int    min_ms_div1024      = 511981;
            const int    max_ms_div1024      = 2047925;
            //предделители частоты
            ushort prescaler;

            public void   setPrescaler(ushort DIV_1_2_8_16_64_256_1024)
            {
                prescaler = DIV_1_2_8_16_64_256_1024;
                byte[] wDATA = { Command.RTC_set_prescaler, 0};
                switch (DIV_1_2_8_16_64_256_1024)
                {

                    case 1:     wDATA[1] = 1;
                        break;
                    case 2:     wDATA[1] = 2;
                        break;
                    case 8:     wDATA[1] = 3;
                        break;
                    case 16:    wDATA[1] = 4;
                        break;
                    case 64:    wDATA[1] = 5;
                        break;
                    case 256:   wDATA[1] = 6;
                        break;
                    case 1024:  wDATA[1] = 7;
                        break;
                    default: trace("ОШИБКА! Неверный предделитель!");
                        prescaler = 0;
                        return;
                }
                USART.Open();
                USART.Write(wDATA, 0, 2);
                USART.Close();
            }
            public ushort getPrescaler()
            {
                return prescaler;
            }
            public double getFreqency()
            {
                return (sourceFrequency / prescaler);
            }
            public ushort getTicks(int MILLISECONDS)
            {
                if ((MILLISECONDS >= _RTC.min_ms_div1) && (MILLISECONDS < _RTC.min_ms_div2))
                {
                    prescaler = 1;
                }
                else if ((MILLISECONDS >= _RTC.min_ms_div2) && (MILLISECONDS < _RTC.min_ms_div8))
                {
                    prescaler = 2;
                }
                else if ((MILLISECONDS >= _RTC.min_ms_div8) && (MILLISECONDS < _RTC.min_ms_div16))
                {
                    prescaler = 8;
                }
                else if ((MILLISECONDS >= _RTC.min_ms_div16) && (MILLISECONDS < _RTC.min_ms_div64))
                {
                    prescaler = 16;
                }
                else if ((MILLISECONDS >= _RTC.min_ms_div64) && (MILLISECONDS < _RTC.min_ms_div256))
                {
                    prescaler = 64;
                }
                else if ((MILLISECONDS >= _RTC.min_ms_div256) && (MILLISECONDS < _RTC.min_ms_div1024))
                {
                    prescaler = 256;
                }
                else if ((MILLISECONDS >= _RTC.min_ms_div1024) && (MILLISECONDS < _RTC.max_ms_div1024))
                {
                    prescaler = 1024;
                }
                else
                {
                    trace("ОШИБКА! Неверный интервал! Ожидалось: 0 < MILLISECONDS < 2047967; Получено: " + MILLISECONDS);
                    prescaler = 0;
                    return 0;
                }
                ushort tiks = Convert.ToUInt16(Math.Round(Convert.ToDouble(MILLISECONDS) * getFreqency()));
                return tiks;
            }
            public ushort getTicks(string MILLISECONDS)
            {
                if (MILLISECONDS != "")
                {
                    return getTicks(Convert.ToInt32(MILLISECONDS));
                }
                else
                {
                    return 0;
                }
            }
        }
        public struct SPI_ADC
        {
            //СТРУКТУРА: АЦП
            const byte Hbyte = 127;
            const byte Lbyte_DoubleRange = 16;
            const byte Lbyte_NormalRange = 48;
            const byte ChannelStep = 4;

            public bool DoubleRange;

            public ushort getVoltage(byte     CHANNEL)
            {
                /*  Последовательность 11 битного слова, которое надо передать ADC'у: Или это просто регистр?
                 * +-------+-----+-------+------+------+------+-----+-----+--------+-------+-------+--------+
                 * | WRITE | SEQ | DONTC | ADD2 | ADD1 | ADD0 | PM1 | PM0 | SHADOW | DONTC | RANGE | CODING |
                 * +-------+-----+-------+------+------+------+-----+-----+--------+-------+-------+--------+
                 * 
                 * WRITE - 1 - тогда ADC запишет следующие 11 бит. Иначе пропустит мимо ушей
                 * SEQ - ?
                 * DONTC - не парься
                 *          _______________________________________
                 *          |____Адрес__|_________________|       |
                 *          | 8 | 7 | 6 | Имя  | № вывода | Канал |  
                 *          |---+---+---+------+----------+-------|
                 *          | 0 | 0 | 0 | Vin0 |    16    |   1   |
                 *          | 0 | 0 | 1 | Vin1 |    15    |   2   |
                 * ADD2     | 0 | 1 | 0 | Vin2 |    14    |   3   |
                 * ADD1     | 0 | 1 | 1 | Vin3 |    13    |   4   |
                 * ADD0     | 1 | 0 | 0 | Vin4 |    12    |   5   |
                 *          | 1 | 0 | 1 | Vin5 |    11    |   6   |
                 *          | 1 | 1 | 0 | Vin6 |    10    |   7   |
                 *          | 1 | 1 | 1 | Vin7 |    9     |   8   |
                 *          |---+---+---+------+----------+-------| 
                 * 
                 * PM1 и PM0 - управление питанием (1 1 - нормальный режим, самый быстрый)
                 * SHADOW - ?
                 * DONTC - не парься...
                 * RANGE - 1 - стандартный диапазон 0...REF | 0 - удвоенный 0...2xREF
                 * CODING - кодирует ответ ADC: 0 - the output coding for the part is twos complement | 
                 *                              1 - the output coding from the part is straight binary (for the next conversion).
                 *                              
                 * 
                 * Сладкая парочка SEQ и SHADOW:
                 *  0 0 - Каналы оцифровываются независимо. Ни какой "последовательной функцией" тут не пахнет
                 *  othe - какая-то муть с "последовательными функциями" оцифровки и программированием shadow-регистра
                 *  
                 * Составляем слово:
                 *  1 0 0 ххх 11 0 0 1 1 '0000'
                 *  131 + 16
                 */
                if (CHANNEL > 0 && CHANNEL < 9)
                {
                    trace("Запрос у ADC напряжения на канале " + CHANNEL);
                    byte Lbyte = 0;
                    if (DoubleRange)
                    {
                        Lbyte = Lbyte_DoubleRange;
                    }
                    else
                    {
                        Lbyte = Lbyte_NormalRange;
                    }
                    byte[] wDATA = { Command.ADC_get_voltage, Convert.ToByte(Hbyte + ChannelStep * CHANNEL), Lbyte };
                    byte[] rDATA = { 0, 0, 0, 0 };
                    byte adress = 1;
                    ushort voltage = 0;
                    USART.Open();
                    USART.Write(wDATA, 0, 3);
                    try
                    {
                        for (byte i = 0; i < 4; i++)
                        {
                            rDATA[i] = Convert.ToByte(USART.ReadByte());
                            //0 - адрес + старшее напряжение; 1 - младшее напряжение; 2 - отклик; 3 - данные отклика
                        }
                    }
                    catch (Exception)
                    {
                        trace("ОШИБКА ПРИЁМА ДАННЫХ!");
                    }
                    USART.Close();
                    if (rDATA[2] != Response.error)
                    {
                        trace("ОШИБКА ОТКЛИКА! Ожидалось: " + Response.error + ", получено: " + rDATA[2]);
                    }
                    adress += Convert.ToByte(rDATA[0] >> 4);
                    voltage = Convert.ToUInt16((Convert.ToUInt16(rDATA[0] & 0xf)<<8)+ rDATA[1]);
                    trace("    Ответный адрес канала: " + adress);
                    trace("    Напряжение: " + voltage);
                    return rDATA[3];
                }
                else
                {
                    trace("ОШИБКА SPI_ADC! Ввод неправильного значения!");
                    trace("    Ожидалось: CHANNEL = 1...8");
                    trace("    Получено:  CHANNEL = " + CHANNEL);
                    return 0;
                }
            }
            public ushort getVoltage(string   CHANNEL)
            {
                return getVoltage(Convert.ToByte(CHANNEL));
            }
            public ushort getVoltage(int      CHANNEL)
            {
                return getVoltage(Convert.ToByte(CHANNEL));
            }
        }
        public struct SPI_DAC
        {
            //СТРУКТУРА: ЦАП
            const byte Reset_Hbyte = 255;
            const byte Reset_Lbyte = 255;

            public void reset()
            {
                byte[] wDATA = { Command.DAC_set_voltage, Reset_Hbyte, Reset_Lbyte };
                USART.Open();
                USART.Write(wDATA, 0, 3);
                USART.Close();
                trace("Напряжения DAC'a сброшены");
            }
            public byte setVoltage(byte     CHANNEL, ushort     VOLTAGE)
            {
                //ФУНКЦИЯ: Посылаем DAC'у адресс канала и напряжение на нём, получаем отклик
                /*  ____________________________________________________________
                 *  |_____Адрес____|__________________|       |    Диапазон    |
                 *  | 14 | 13 | 12 |  Имя  | № вывода | Канал |ADRESS_and_Hbyte|  
                 *  |----+----+----+-------+----------+-------+----------------|
                 *  | 0  | 0  | 0  | DAC A |    4     |   1   |    0...15      |
                 *  | 0  | 0  | 1  | DAC B |    5     |   2   |    16...31     |
                 *  | 0  | 1  | 0  | DAC C |    6     |   3   |    32...47     |
                 *  | 0  | 1  | 1  | DAC D |    7     |   4   |    48...63     |
                 *  | 1  | 0  | 0  | DAC E |    10    |   5   |    64...79     |
                 *  | 1  | 0  | 1  | DAC F |    11    |   6   |    80...95     |
                 *  | 1  | 1  | 0  | DAC G |    12    |   7   |    96...111    |
                 *  | 1  | 1  | 1  | DAC H |    13    |   8   |    112...127   |
                 *  |----+----+----+-------+----------+-------+----------------| 
                 * 
                 *  ADRESS_and_Hbyte =  0      111         xxxx
                 *                     D\C    адрес    Старший байт 
                 *  Lbyte =   xxxx xxxx
                 *          Младший байт
                 * 
                 * D\C -> 0 - адрес + напряжение | 1 - управляющий сигнал  (1111 1111 1111 1111 - полный сброс)
                 * 
                 * VOLTAGE = 0...4095 = 0_0 ... 15_255
                 * 
                 */
                //Формируем данные на отправку
                if (CHANNEL > 0 && CHANNEL < 9 && VOLTAGE >= 0 && VOLTAGE < 4096)
                {
                    byte[] bytes = BitConverter.GetBytes(VOLTAGE);
                    byte[] wDATA = { Command.DAC_set_voltage, Convert.ToByte((CHANNEL - 1) * 16 + bytes[1]), bytes[0] };
                    byte[] rDATA = { 0, 0 };
                    USART.Open();
                    USART.Write(wDATA, 0, 3);
                    try
                    {
                        rDATA[0] = Convert.ToByte(USART.ReadByte());
                        rDATA[1] = Convert.ToByte(USART.ReadByte());
                    }
                    catch (Exception)
                    {
                        trace("ОШИБКА ПРИЁМА ДАННЫХ!");
                    }
                    USART.Close();
                    if (rDATA[0] != Response.error)
                    {
                        trace("ОШИБКА ОТКЛИКА! Ожидалось: 2, получено: " + rDATA[0]);
                    }
                    trace("Команда DAC'у: установить напряжение " + VOLTAGE.ToString() + " на канале " + CHANNEL.ToString());
                    return rDATA[1];
                }
                else
                {
                    trace("ОШИБКА SPI_DAC! Ввод неправильного значения!");
                    trace("    Ожидалось: CHANNEL = 1...8 ; VOLTAGE = 0...4095.");
                    trace("    Получено:  CHANNEL = " + CHANNEL + ";  VOLTAGE = " + VOLTAGE);
                    return 0;
                }
            }
            public byte setVoltage(string   CHANNEL, string     VOLTAGE)
            {
                return setVoltage(Convert.ToByte(CHANNEL), Convert.ToUInt16(VOLTAGE));
            }
            public byte setVoltage(int      CHANNEL, int        VOLTAGE)
            {
                return setVoltage(Convert.ToByte(CHANNEL), Convert.ToUInt16(VOLTAGE));
            }
        }
        public struct Counter
        {
            //СТРУКТУРА: Счётчики, их три СОА, СОВ и СОС.
            _RTC RTC_;
            public Counter(_RTC RealTimeCounter)
                : this()
            {
                RTC_ = RealTimeCounter;
            }
            public void     start()
            {
                //ФУНКЦИЯ: Запускаем счётчик
                //ПРИМЕЧАНИЕ: Надо сделать проверку состояния, вдруг он уже считает
                byte[] wDATA = { Command.COA_start };
                USART.Open();
                USART.Write(wDATA, 0, 1);
                USART.Close();
            }
            public void     stop()
            {
                //ФУНКЦИЯ: Останавливаем счётчик
                //ПРИМЕЧАНИЕ: Надо сделать проверку состояния, вдруг он уже стоит, откликнуться
                byte[] wDATA = { Command.COA_stop };
                USART.Open();
                USART.Write(wDATA, 0, 1);
                USART.Close();
            }
            public UInt32   getResult()
            {
                //ФУНКЦИЯ: Запрашиваем результат счёта у МК
                byte[] wDATA = { Command.COA_get_count };
                byte[] rDATA = { 0, 0, 0, 0, 0 };
                UInt32 count = 0;
                USART.Open();
                USART.Write(wDATA, 0, 1);
                try
                {
                    rDATA[0] = Convert.ToByte(USART.ReadByte());
                    rDATA[1] = Convert.ToByte(USART.ReadByte());
                    rDATA[2] = Convert.ToByte(USART.ReadByte());
                    rDATA[3] = Convert.ToByte(USART.ReadByte());
                    rDATA[4] = Convert.ToByte(USART.ReadByte());
                }
                catch (Exception)
                {
                    trace("ОШИБКА ПРИЁМА ДАННЫХ!");
                }
                USART.Close();
                if (rDATA[0] == Response.COX_done)
                {
                    trace("Счётчик завершил счёт.");
                }
                else if (rDATA[0] == Response.COX_busy)
                {
                    trace("Счётчик всё ещё считает!");
                }
                else if (rDATA[0] == Response.COX_stoped)
                {
                    trace("Счётчик был принудительно остановлен!");
                }
                else
                {
                    trace("ОШИБКА ОТКЛИКА! Ожидалось: " + Response.COX_done + "|" + Response.COX_busy + "|" + Response.COX_stoped + ", получено: " + rDATA[0] + " " + rDATA[1] + " " + rDATA[2]);
                }
                count = Convert.ToUInt32(rDATA[1] * 16777216 + rDATA[2]*65536 + rDATA[3]*256 + rDATA[4]);
                return count;
            }
            public void     setTimeInterval(int MILLISECONDS)
            {
                //ФУНКЦИЯ: Задаёт количество тиков для RTC через интервал в миллисекундах
                ushort ticks = RTC_.getTicks(MILLISECONDS);
                RTC_.setPrescaler(RTC_.getPrescaler());
                byte[] bytes_ticks = BitConverter.GetBytes(ticks);
                byte[] wDATA = { Command.COA_set_timeInterval, bytes_ticks[1], bytes_ticks[0] };
                USART.Open();
                USART.Write(wDATA, 0, 3);
                USART.Close();
                trace("Задан временной интервал счёта: " + MILLISECONDS + "мс (" + ticks + " тиков)");
            }
            public void     setTimeInterval(string MILLISECONDS)
            {
                if (MILLISECONDS != "")
                {
                    setTimeInterval(Convert.ToInt32(MILLISECONDS));
                }
            }
            //public DateTime MILLISECONDS { get; set; }
        }
        //--------------------------------------ОБЪЕКТЫ-------------------------------------------
        public _RTC RTC = new _RTC();
        public Counter COA;
        //public Counter COB = new Counter();
        //public Counter COC = new Counter();
        public SPI_ADC ADC = new SPI_ADC();
        public SPI_DAC DAC = new SPI_DAC();
        
        //--------------------------------------ФУНКЦИИ-------------------------------------------
        //ИНИЦИАЛИЗАЦИОННЫЕ
        public void     setUSART(SerialPort COM_PORT)
        {
            //ФУНКЦИЯ: Задаём порт, припомози которого будем общаться с МК
            USART = COM_PORT;
            COA = new Counter(RTC);
        }
        public void     setTracer(RichTextBox TRACER)
        {
            //ФУНКЦИЯ: Задаём трэйсер для отладки\ответов
            tracer = TRACER;
            tracer_defined = true;
        }
        //USART

        byte calcCheckSum(byte[] data)
        {
            //ФУНКЦИЯ: Вычисление контрольной суммы для верификации данных
            byte CheckSum = 0;
            //trace("Составление контрольной суммы: ");
            for (int i = 0; i < data.Length; i++)
            {
                //trace("    " + data[i]);
                CheckSum -= data[i];
            }
            trace("         Контрольная сумма: " + CheckSum);
            return CheckSum;
        }
        byte[] wDATA(List<byte> DATA)
        {
            byte CheckSum = calcCheckSum(DATA.ToArray());
            List<byte> data = new List<byte>();
            data.Add(Command.KEY);
            data.AddRange(DATA);
            data.Add(CheckSum);
            data.Add(Command.LOCK);
            return data.ToArray();
        }
        byte[] wDATA(byte[] DATA)
        {
            return wDATA(DATA.ToList());
        }
        byte[] wDATA(byte DATA)
        {
            return wDATA(new byte[] { DATA });
        }
        byte[] wDATA(ushort DATA)
        {
            return wDATA(BitConverter.GetBytes(DATA));
        }
        byte[] wDATA(uint DATA)
        {
            return wDATA(BitConverter.GetBytes(DATA));
        }
        byte[] wDATA(byte COMMAND, byte DATA)
        {
            return wDATA(new byte[] { COMMAND, DATA });
        }
        byte[] wDATA(byte COMMAND, ushort DATA)
        {
            return wDATA(new byte[] { COMMAND, BitConverter.GetBytes(DATA)[0], BitConverter.GetBytes(DATA)[1] });
        }
        byte[] wDATA(byte COMMAND, uint DATA)
        {
            return wDATA(new byte[] { COMMAND, BitConverter.GetBytes(DATA)[0], BitConverter.GetBytes(DATA)[1], BitConverter.GetBytes(DATA)[2], BitConverter.GetBytes(DATA)[3] });
        }
        /*byte[] rDATA(byte rDATAquantity)
        {
            if (rDATAquantity != 0)
            {
                rDATAquantity += 3;                     //+3 для ключа, КС и затвора
                byte[] data = new byte[rDATAquantity]; 
                List<byte> recDATA = new List<byte>();
                byte j = 0;
                bool dataBody = false;
                try
                {
                    for (byte i = 0; i < rDATAquantity; i++)
                    {
                        data[i] = Convert.ToByte(USART.ReadByte());
                    }
                }
                catch (Exception)
                {
                    trace("ОШИБКА ПРИЁМА ДАННЫХ!");
                    return new byte[] { 0 };
                }
                trace("Получены данные:");
                foreach (byte b in data)
                {
                    
                    if (dataBody)
                    {
                        if (b == Command.LOCK)
                        {
                            trace("    Был получен затвор!");
                            byte CheckSum = recDATA.Last();
                            recDATA.RemoveAt(recDATA.ToArray().Length - 1);
                            if (calcCheckSum(recDATA.ToArray()) == CheckSum)
                            {
                                trace("    Контрольная сумма: " + CheckSum);
                                return recDATA.ToArray();
                            }
                            trace("Несовпадает контрольная сумма!");
                            return new byte[] { 0 };
                        }
                        else
                        {
                            trace("         " + b);
                            recDATA.Add(b);
                            j++;
                        }
                    }
                    else if (b == Command.KEY)
                    {
                        dataBody = true;
                        trace("    Был получен ключ!");
                    }
                }
                trace("ОШИБКА ШИФРА! Пакет был неправильно сформирован!");
                return new byte[] { 0 };
            }
            return new byte[] {0};
        }
        byte[] rDATA(int  rDATAquantity)
        {
            return rDATA(Convert.ToByte(rDATAquantity));
        }
        */
        void Receiving_ON()
        {
            //ФУНКЦИЯ: Включает постоянное получение данных по USART
            //USART.DataReceived += new SerialDataReceivedEventHandler(receive_Handler);
            trace("Обработчик включён!");
        }
        void Receiving_OFF()
        {
            //ФУНКЦИЯ: Выключает постоянное получение данных по USART
            //USART.DataReceived -= new SerialDataReceivedEventHandler(receive_Handler);
            trace("Обработчик выключен!");
        }
        void receive_Handler(object sender, EventArgs e)
        {
            //ОБРАБОТЧИК: Происходит при получении данных по USART
            receive();
            trace("!");
        }
        byte[] receive()
        {
            //ФУНКЦИЯ: Получение данных по USART, если есть что получать
            byte rBYTE;
            trace("Обнаружены данные! Количество:" + USART.BytesToRead);
            while (USART.BytesToRead > 0)
            {
                try
                {
                    rBYTE = (byte)USART.ReadByte();
                }
                catch
                {
                    trace("ОШИБКА ПРИЁМА ДАННЫХ! (Receiving)");
                    return new byte[] {};
                }
                switch (rBYTE)
                {
                    case Command.KEY:
                        trace("     Получен ключ!");
                        USART_buffer.Clear();
                        break;
                    case Command.LOCK:
                        trace("     Получен затвор!");
                        return USART_buffer.ToArray();
                    default:
                        USART_buffer.Add(rBYTE);
                        trace("         " + rBYTE);
                        break;
                }
            }
            return new byte[] {};
        }
        byte[] Analizer(List<byte> DATA)
        {
            trace("     Анализ полученной команды...");
            byte CheckSum = DATA.Last();
            DATA.RemoveAt(DATA.ToArray().Length - 1);
            if (calcCheckSum(DATA.ToArray()) == CheckSum)
            {
                trace("         Контрольная сумма: " + CheckSum);
                //return DATA.ToArray();
            }
            else
            {
                trace("         Несовпадает контрольная сумма! Получено:" + CheckSum);
            }
            trace("     Получены данные: ");
            foreach (byte b in DATA)
            {
                trace(b.ToString());
            }
            return DATA.ToArray();
        }

        byte[] transmit(List<byte> DATA)
        {
            //ФУНКЦИЯ: Формируем пакет, передаём его МК, слушаем ответ, возвращаем ответ.
            byte command = DATA[0];                         //Запоминаем передаваемую команду
            trace("Начало исполнения команды:");
            trace("     Команда:");
            foreach(byte b in DATA.ToArray())
            {
                trace("         " + b);
            }
            trace("     Формирование пакета...");
            List<byte> rDATA = new List<byte>();            //Список данных, которые будут приняты
            List<byte> Packet = new List<byte>();           //Список байтов, которые будут посланы
            //Формируем пакет по типу:  ':<response><data><CS>\r' 
            Packet.Add(Command.KEY);                        //':' - Начало данных           
            Packet.AddRange(DATA);                          //'<data>' - байты данных <<response><attached_data>>
            Packet.Add(calcCheckSum(DATA.ToArray()));       //'<CS>' - контрольная сумма
            Packet.Add(Command.LOCK);                       //'\r' - конец передачи
            trace("     Пакет:");
            foreach (byte b in Packet.ToArray())
            {
                trace("         " + b);
            }
            //Выполняем передачу и приём
            USART.Open();
            USART.Write(Packet.ToArray(), 0, Packet.ToArray().Length);
            trace("     Передача завершена!");
            trace("     Приём...");                         //Приём-приём
            byte rBYTE;                                       //Принятый байт
            bool lock_recieved = false;                       //Затвор ещё небыл получен
            bool key_recived = false;                       //Ключ ещё не был получен
            byte BytesToReadQuantity = (byte)USART.BytesToRead;
            trace("         Данные на приём:" + BytesToReadQuantity);
            if (BytesToReadQuantity == 0)
            {
                trace("ОШИБКА ПРИЁМА ДАННЫХ! Не было получено никаких данных!");
                return new byte[] { 255 };
            }
            trace("             Принято:");
            //Принимаем данные пока есть что принимать и пока не пришёл затвор
            while ((USART.BytesToRead > 0) && (!lock_recieved))
            {
                try
                {
                    rBYTE = (byte)USART.ReadByte();
                    trace("             "+rBYTE);
                }
                catch
                {
                    trace("ОШИБКА ПРИЁМА ДАННЫХ! Приём не удался!");
                    return new byte[] { };
                }
                switch (rBYTE)
                {
                    case Command.KEY:
                        trace_attached(" - ключ!");
                        key_recived = true;
                        break;
                    case Command.LOCK:
                        trace_attached(" - затвор!");
                        lock_recieved = true;
                        break;
                    default:
                        rDATA.Add(rBYTE);
                        break;
                }
            }
            USART.Close();
            if (key_recived)
            {
                if (lock_recieved)
                {
                    //Анализируем полученные данные
                    trace("     Анализ полученной команды...");
                    byte rCheckSum = rDATA.Last();                          //Полученная КС
                    rDATA.RemoveAt(rDATA.ToArray().Length - 1); //Убираем КС из списка полученных данных
                    byte CheckSum = calcCheckSum(rDATA.ToArray());          //Подсчитанная КС
                    if (CheckSum == rCheckSum)
                    {
                        trace("         Контрольная сумма совпадает!");
                    }
                    else
                    {
                        trace("         Несовпадает контрольная сумма!");
                        trace("             Получено:" + rCheckSum);
                        trace("             Подсчитано:" + CheckSum);
                        return new byte[] { 255 };
                    }
                    //Проверяем данные на отклик
                    trace("     Отклик: " + rDATA[0]);
                    if (BytesToReadQuantity > 4)
                    {
                        if (rDATA[0] == command)
                        {
                            rDATA.RemoveAt(0);
                            trace("     Принятые данные: ");
                            foreach (byte b in rDATA)
                            {
                                trace("         " + b);
                            }
                        }
                        else
                        {
                            trace("ОШИБКА ОТКЛИКА!");
                            trace("     Ожидалось: " + command);
                            trace("     Получено: " + rDATA[0]);
                            return new byte[] { 255 };
                        }
                    }
                    return rDATA.ToArray();
                }
                trace("ОШИБКА ПРИЁМА ДАННЫХ! Не был получен затвор!");
            }
            trace("ОШИБКА ПРИЁМА ДАННЫХ! Не был получен ключ!");
            return new byte[] { 255 };
        }
        byte[] transmit(byte[] DATA)
        {
            return transmit(DATA.ToList());
        }
        byte[] transmit(byte COMMAND)
        {
            return transmit((new byte[] { COMMAND }).ToList());
        }
        byte[] transmit(byte COMMAND, byte DATA)
        {
            return transmit((new byte[] { COMMAND, DATA }).ToList());
        }
        byte[] transmit(byte COMMAND, byte[] DATA)
        {
            List<byte> data = new List<byte>();
            data.Add(COMMAND);
            data.AddRange(DATA);
            return transmit(data);
        }
        byte[] transmit(byte COMMAND, List<byte> DATA)
        {
            //Проверить - не надо ли создавать новый список
            DATA.Insert(0, COMMAND);
            return transmit(DATA);
        }
        //ИНТЕРФЕЙСНЫЕ
        static public void trace(string text)
        {
            //ФУНКЦИЯ: Выводит text в RichTextBox
            if (tracer_defined)
            {
                tracer.AppendText(Environment.NewLine + "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + text);
                tracer.ScrollToCaret();
            }
        }
        static public void trace_attached(string text)
        {
            //ФУНКЦИЯ: Выводит text в RichTextBox
            if (tracer_defined)
            {
                tracer.AppendText(text);
                tracer.ScrollToCaret();
            }
        }
        //ОТЛАДОЧНЫЕ
        public void     showMeByte(byte     BYTE)
        {
            //transmit(Command.showMeByte, BYTE);
            List<byte> data = new List<byte>();
            data.Add(Command.showMeByte);
            data.Add(BYTE);
            transmit(data);
        }
        public void     showMeByte(string   BYTE)
        {
            showMeByte(Convert.ToByte(BYTE));
        }
        public void     showMeByte(uint     BYTE)
        {
            showMeByte(Convert.ToByte(BYTE));
        }
        public void     setMCwait()
        {
            //ФУНКЦИЯ: Устанавливает статус МК
            byte[] wDATA = { Command.MC_wait };
            USART.Open();
            USART.Write(wDATA, 0, 1);
            USART.Close();
        }
        //ФУНКЦИИ МИКРОКОНТРОЛЛЕРА
        public byte     getStatus()
        {
            //ФУНКЦИЯ: Получает статус у МК
            byte[] rStatus = transmit(Command.MC_get_status);
            if (rStatus[0] == Command.MC_get_status)
            {
                return rStatus[1];
            }
            trace("ОШИБКА ОТКЛИКА! Ожидалось: " + Command.MC_get_status + " Получено:");
            foreach(byte b in rStatus)
            {
                trace("     " + b);
            }
            return rStatus[0];
        }
        public byte     getVersion()
        {
            //ФУНКЦИЯ: Получает статус у МК
            return 0;// transmit(wDATA(Command.MC_get_version), 1)[0];
        }
        public string   getBirthday()
        {
            //ФУНКЦИЯ: Получает статус у МК
            UInt32 birthday = 0;
            string answer = "00000000";
            byte[] recDATA = {0};//(wDATA(Command.MC_get_birthday), 4);
            birthday = Convert.ToUInt32(recDATA[3]) * 16777216 + Convert.ToUInt32(recDATA[2]) * 65536 + Convert.ToUInt32(recDATA[1]) * 256 + Convert.ToUInt32(recDATA[0]);

            answer = birthday.ToString();
            answer = answer[6] + "" + answer[7] + " " + answer[4] + answer[5] + " " + answer[0] + answer[1] + answer[2] + answer[3];

            return answer;
        }
        public string   getCPUfrequency()
        {
            UInt32 frequency = 0;
            byte[] recDATA = { 0 };// transmit(wDATA(Command.MC_get_CPUfreq), 4);
            frequency = Convert.ToUInt32(recDATA[3]) * 16777216 + Convert.ToUInt32(recDATA[2]) * 65536 + Convert.ToUInt32(recDATA[1]) * 256 + Convert.ToUInt32(recDATA[0]);
            return frequency.ToString() + " Гц";
        }
        //Функции прямого управления
        public byte[]   directCommand(byte COMMAND)
        {
            trace("Прямая команда!");
            trace("    Комманда: " + COMMAND);
            byte[] wDATA = { COMMAND };
            byte[] rDATA = { 0 };

            USART.Open();
            USART.Write(wDATA, 0, 1);
            USART.Close();

            return rDATA;
        }
        public byte[]   directCommand(byte COMMAND, bool RESPONSE)
        {
            trace("Прямая команда!");
            trace("    Комманда: " + COMMAND);
            trace("    Ожидается отклик");

            byte[] wDATA = { COMMAND };
            byte[] rDATA = { 0, 0 };

            USART.Open();
            USART.Write(wDATA, 0, 1);
            try
            {
                rDATA[0] = Convert.ToByte(USART.ReadByte());
                rDATA[1] = Convert.ToByte(USART.ReadByte());
            }
            catch (Exception)
            {
                trace("ОШИБКА ПРИЁМА ДАННЫХ!");
            }
            USART.Close();

            trace("    Отклик: " + rDATA[0]);
            trace("    Код отклика: " + rDATA[0]);

            return rDATA;
        }
        public byte[]   directCommand(byte COMMAND, byte[] sendDATA)
        {
            trace("Прямая команда!");
            trace("    Комманда: " + COMMAND);
            trace("    Переданные данные: " + sendDATA);

            byte[] wDATA = { COMMAND };
            byte[] rDATA = { 0, 0 };

            USART.Open();
            USART.Write(wDATA, 0, 1);
            USART.Write(sendDATA, 0, sendDATA.Length);
            USART.Close();

            return rDATA;
        }
        public byte[]   directCommand(byte COMMAND, byte recieveDATA_count)
        {
            trace("Прямая команда!");
            trace("    Комманда: " + COMMAND);
            trace("    Ожидаемое количество ответных данных: " + recieveDATA_count);

            byte[] wDATA = { COMMAND };
            byte[] rDATA = new byte[recieveDATA_count];

            USART.Open();
            USART.Write(wDATA, 0, 1);
            try
            {
                for (byte i = 0; i < recieveDATA_count; i++)
                {
                    rDATA[i] = Convert.ToByte(USART.ReadByte());
                }
            }
            catch (Exception)
            {
                trace("ОШИБКА ПРИЁМА ДАННЫХ!");
            }
            USART.Close();

            trace("    Полученные данные:");
            for (byte i = 0; i < recieveDATA_count; i++)
            {
                trace("        " + (i + 1) + ": " + rDATA[i]);
            }

            return rDATA;
        }
        public byte[]   directCommand(byte COMMAND, byte recieveDATA_count, bool RESPONSE)
        {
            trace("Прямая команда!");
            trace("    Комманда: " + COMMAND);
            trace("    Ожидаемое количество ответных данных: " + recieveDATA_count);
            trace("    Ожидается отклик");

            byte[] wDATA = { COMMAND };
            byte[] rDATA = new byte[recieveDATA_count + 2];

            USART.Open();
            USART.Write(wDATA, 0, 1);
            try
            {
                for (byte i = 0; i < (recieveDATA_count + 2); i++)
                {
                    rDATA[i] = Convert.ToByte(USART.ReadByte());
                }
            }
            catch (Exception)
            {
                trace("ОШИБКА ПРИЁМА ДАННЫХ!");
            }
            USART.Close();

            trace("    Полученные данные:");
            for (byte i = 0; i < recieveDATA_count; i++)
            {
                trace("        " + (i + 1) + ": " + rDATA[i]);
            }
            trace("    Отклик: " + rDATA[recieveDATA_count]);
            trace("    Код отклика: " + rDATA[recieveDATA_count + 1]);

            return rDATA;
        }
        public byte[]   directCommand(byte COMMAND, byte[] sendDATA,        bool RESPONSE)
        {
            trace("Прямая команда!");
            trace("    Комманда: " + COMMAND);
            trace("    Переданные данные: " + sendDATA);
            trace("    Ожидается отклик");

            byte[] wDATA = { COMMAND };
            byte[] rDATA = { 0, 0 };

            USART.Open();
            USART.Write(wDATA, 0, 1);
            USART.Write(sendDATA, 0, sendDATA.Length);
            try
            {
                rDATA[0] = Convert.ToByte(USART.ReadByte());
                rDATA[1] = Convert.ToByte(USART.ReadByte());
            }
            catch (Exception)
            {
                trace("ОШИБКА ПРИЁМА ДАННЫХ!");
            }
            USART.Close();

            trace("    Отклик: " + rDATA[0]);
            trace("    Код отклика: " + rDATA[0]);

            return rDATA;
        }
        public byte[]   directCommand(byte COMMAND, byte[] sendDATA,        byte recieveDATA_count)
        {
            trace("Прямая команда!");
            trace("    Комманда: " + COMMAND);
            trace("    Переданные данные: " + sendDATA);
            trace("    Ожидаемое количество ответных данных: " + recieveDATA_count);

            byte[] wDATA = { COMMAND };
            byte[] rDATA = new byte[recieveDATA_count];

            USART.Open();
            USART.Write(wDATA, 0, 1);
            USART.Write(sendDATA, 0, sendDATA.Length);
            try
            {
                for (byte i = 0; i < recieveDATA_count; i++)
                {
                    rDATA[i] = Convert.ToByte(USART.ReadByte());
                }
            }
            catch (Exception)
            {
                trace("ОШИБКА ПРИЁМА ДАННЫХ!");
            }
            USART.Close();

            trace("    Полученные данные:");
            for (byte i = 0; i < recieveDATA_count; i++)
            {
                trace("        " + (i + 1) + ": " + rDATA[i]);
            }

            return rDATA;
        }
        public byte[]   directCommand(byte COMMAND, byte[] sendDATA,        byte recieveDATA_count, bool RESPONSE)
        {
            trace("Прямая команда!");
            trace("    Комманда: " + COMMAND);
            trace("    Переданные данные: " + sendDATA);
            trace("    Ожидаемое количество ответных данных: " + recieveDATA_count);
            trace("    Ожидается отклик");

            byte[] wDATA = { COMMAND };
            byte[] rDATA = new byte[recieveDATA_count+2];

            USART.Open();
            USART.Write(wDATA, 0, 1);
            USART.Write(sendDATA, 0, sendDATA.Length);
            try
            {
                for (byte i = 0; i < (recieveDATA_count + 2); i++)
                {
                    rDATA[i] = Convert.ToByte(USART.ReadByte());
                }
            }
            catch (Exception)
            {
                trace("ОШИБКА ПРИЁМА ДАННЫХ!");
            }
            USART.Close();

            trace("    Полученные данные:");
            for (byte i = 0; i < recieveDATA_count; i++)
            {
                trace("        "+(i+1)+": " + rDATA[i]);
            }
            trace("    Отклик: " + rDATA[recieveDATA_count]);
            trace("    Код отклика: " + rDATA[recieveDATA_count+1]);

            return rDATA;
        }
        //ПРОЧИЕ ФУНКЦИИ
        //
        //--------------------------------------ЗАМЕТКИ-------------------------------------------
        /*
         *
         */
        //public byte SPI_DAC(byte CHANNEL, ushort VOLTAGE)
        //{
        //    //ФУНКЦИЯ: Посылаем DAC'у адресс канала и напряжение на нём, получаем отклик
        //    /*  ____________________________________________________________
        //     *  |_____Адрес____|__________________|       |    Диапазон    |
        //     *  | 14 | 13 | 12 |  Имя  | № вывода | Канал |ADRESS_and_Hbyte|  
        //     *  |----+----+----+-------+----------+-------+----------------|
        //     *  | 0  | 0  | 0  | DAC A |    4     |   1   |    0...15      |
        //     *  | 0  | 0  | 1  | DAC B |    5     |   2   |    16...31     |
        //     *  | 0  | 1  | 0  | DAC C |    6     |   3   |    32...47     |
        //     *  | 0  | 1  | 1  | DAC D |    7     |   4   |    48...63     |
        //     *  | 1  | 0  | 0  | DAC E |    10    |   5   |    64...79     |
        //     *  | 1  | 0  | 1  | DAC F |    11    |   6   |    80...95     |
        //     *  | 1  | 1  | 0  | DAC G |    12    |   7   |    96...111    |
        //     *  | 1  | 1  | 1  | DAC H |    13    |   8   |    112...127   |
        //     *  |----+----+----+-------+----------+-------+----------------| 
        //     * 
        //     *  ADRESS_and_Hbyte =  0      111         xxxx
        //     *                     D\C    адрес    Старший байт 
        //     *  Lbyte =   xxxx xxxx
        //     *          Младший байт
        //     * 
        //     * D\C -> 0 - адрес + напряжение | 1 - управляющий сигнал  (1111 1111 1111 1111 - полный сброс)
        //     * 
        //     * VOLTAGE = 0...4095 = 0_0 ... 15_255
        //     * 
        //     */
        //    //Формируем данные на отправку
        //    if (CHANNEL > 0 && CHANNEL < 9 && VOLTAGE >= 0 && VOLTAGE < 4096)
        //    {
        //        byte[] bytes = BitConverter.GetBytes(VOLTAGE);
        //        byte[] wDATA = { Command.DAC_set_voltage, Convert.ToByte((CHANNEL - 1) * 16 + bytes[1]), bytes[0] };
        //        byte[] rDATA = { 0, 0 };
        //        USART.Open();
        //        USART.Write(wDATA, 0, 3);
        //        try
        //        {
        //            rDATA[0] = Convert.ToByte(USART.ReadByte());
        //            rDATA[1] = Convert.ToByte(USART.ReadByte());
        //        }
        //        catch (Exception)
        //        {
        //            trace("ОШИБКА ПРИЁМА ДАННЫХ!");
        //        }
        //        USART.Close();
        //        if (rDATA[0] != Response.error)
        //        {
        //            trace("ОШИБКА ОТКЛИКА! Ожидалось: 2, получено: " + rDATA[0]);
        //        }
        //        trace("Команда DAC'у: установить напряжение " + VOLTAGE.ToString() + " на канале " + CHANNEL.ToString());
        //        return rDATA[1];
        //    }
        //    else
        //    {
        //        trace("ОШИБКА SPI_DAC! Ввод неправильного значения!");
        //        trace("    Ожидалось: CHANNEL = 1...8 ; VOLTAGE = 0...4095.");
        //        trace("    Получено:  CHANNEL = " + CHANNEL + ";  VOLTAGE = " + VOLTAGE);
        //        return 0;
        //    }
        //}

        //public byte _ADC(byte CHANNEL)
        //{
        //    /*  Последовательность 11 битного слова, которое надо передать ADC'у: Или это просто регистр?
        //     * +-------+-----+-------+------+------+------+-----+-----+--------+-------+-------+--------+
        //     * | WRITE | SEQ | DONTC | ADD2 | ADD1 | ADD0 | PM1 | PM0 | SHADOW | DONTC | RANGE | CODING |
        //     * +-------+-----+-------+------+------+------+-----+-----+--------+-------+-------+--------+
        //     * 
        //     * WRITE - 1 - тогда ADC запишет следующие 11 бит. Иначе пропустит мимо ушей
        //     * SEQ - ?
        //     * DONTC - не парься
        //     *          _______________________________________
        //     *          |____Адрес__|_________________|       |
        //     *          | 8 | 7 | 6 | Имя  | № вывода | Канал |  
        //     *          |---+---+---+------+----------+-------|
        //     *          | 0 | 0 | 0 | Vin0 |    16    |   1   |
        //     *          | 0 | 0 | 1 | Vin1 |    15    |   2   |
        //     * ADD2     | 0 | 1 | 0 | Vin2 |    14    |   3   |
        //     * ADD1     | 0 | 1 | 1 | Vin3 |    13    |   4   |
        //     * ADD0     | 1 | 0 | 0 | Vin4 |    12    |   5   |
        //     *          | 1 | 0 | 1 | Vin5 |    11    |   6   |
        //     *          | 1 | 1 | 0 | Vin6 |    10    |   7   |
        //     *          | 1 | 1 | 1 | Vin7 |    9     |   8   |
        //     *          |---+---+---+------+----------+-------| 
        //     * 
        //     * PM1 и PM0 - управление питанием (1 1 - нормальный режим, самый быстрый)
        //     * SHADOW - ?
        //     * DONTC - не парься...
        //     * RANGE - 1 - стандартный диапазон 0...REF | 0 - удвоенный 0...2xREF
        //     * CODING - кодирует ответ ADC: 0 - the output coding for the part is twos complement | 
        //     *                              1 - the output coding from the part is straight binary (for the next conversion).
        //     *                              
        //     * 
        //     * Сладкая парочка SEQ и SHADOW:
        //     *  0 0 - Каналы оцифровываются независимо. Ни какой "последовательной функцией" тут не пахнет
        //     *  othe - какая-то муть с "последовательными функциями" оцифровки и программированием shadow-регистра
        //     *  
        //     * Составляем слово:
        //     *  1 0 0 ххх 11 0 0 1 1 '0000'
        //     *  131 + 16
        //     */
        //    if (CHANNEL > 0 && CHANNEL < 9)
        //    {
        //        trace("Запрос у ADC напряжения на канале " + CHANNEL);
        //        byte[] wDATA = { Command.ADC_get_voltage, Convert.ToByte(127 + 4 * CHANNEL), 16 };
        //        byte[] rDATA = { 0, 0, 0, 0 };

        //        USART.Open();
        //        USART.Write(wDATA, 0, 3);
        //        try
        //        {
        //            for (byte i = 0; i < 4; i++)
        //            {
        //                rDATA[i] = Convert.ToByte(USART.ReadByte());
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            trace("ОШИБКА ПРИЁМА ДАННЫХ!");
        //        }
        //        USART.Close();
        //        trace("    Ответный адрес канала: " + rDATA[0]);
        //        trace("    Напряжение: " + rDATA[1]);
        //        if (rDATA[2] == 2)
        //        {
        //            return rDATA[3];
        //        }
        //        else
        //        {
        //            trace("ОШИБКА ОТКЛИКА! Ожидалось: 2, получено: " + rDATA[0]);
        //            return 0;
        //        }
        //    }
        //    else
        //    {
        //        trace("ОШИБКА SPI_ADC! Ввод неправильного значения!");
        //        trace("    Ожидалось: CHANNEL = 1...8");
        //        trace("    Получено:  CHANNEL = " + CHANNEL);
        //        return 0;
        //    }
        //}

        //public void COA_start()
        //{
        //    //ФУНКЦИЯ: Запускаем счётчик
        //    //ПРИМЕЧАНИЕ: Надо сделать проверку состояния, вдруг он уже считает
        //    byte[] wDATA = { Command.COA_start };
        //    USART.Open();
        //    USART.Write(wDATA, 0, 1);
        //    USART.Close();
        //}
        //public void COA_stop()
        //{
        //    //ФУНКЦИЯ: Останавливаем счётчик
        //    //ПРИМЕЧАНИЕ: Надо сделать проверку состояния, вдруг он уже стоит, откликнуться
        //    byte[] wDATA = { Command.COA_stop };
        //    USART.Open();
        //    USART.Write(wDATA, 0, 1);
        //    USART.Close();
        //}
        //public ushort COA_getResult()
        //{
        //    //ФУНКЦИЯ: Запрашиваем результат счёта у МК
        //    byte[] wDATA = { Command.COA_get_count };
        //    byte[] rDATA = { 0, 0, 0 };
        //    ushort count = 0;
        //    USART.Open();
        //    USART.Write(wDATA, 0, 1);
        //    try
        //    {
        //        rDATA[0] = Convert.ToByte(USART.ReadByte());
        //        rDATA[1] = Convert.ToByte(USART.ReadByte());
        //        rDATA[2] = Convert.ToByte(USART.ReadByte());
        //    }
        //    catch (Exception)
        //    {
        //        trace("ОШИБКА ПРИЁМА ДАННЫХ!");
        //    }
        //    USART.Close();
        //    if (rDATA[0] == Response.COX_done)
        //    {
        //        trace("Счётчик завершил счёт.");
        //    }
        //    else if (rDATA[0] == Response.COX_busy)
        //    {
        //        trace("Счётчик всё ещё считает!");
        //    }
        //    else if (rDATA[0] == Response.COX_stoped)
        //    {
        //        trace("Счётчик был принудительно остановлен!");
        //    }
        //    else
        //    {
        //        trace("ОШИБКА ОТКЛИКА! Ожидалось: " +Response.COX_done + "|" +Response.COX_busy + "|" +Response.COX_stoped + ", получено: " + rDATA[0]);
        //    }
        //    count = Convert.ToUInt16(rDATA[1]*256 + rDATA[2]);
        //    return count;
        //}
        //public void COA_setTimeInterval(ushort INTERVAL)
        //{
        //    //ФУНКЦИЯ: Задаёт количество тиков для RTC через интервал в миллисекундах
        //    if ((INTERVAL < 64000) && (INTERVAL > 0))
        //    {
        //        ushort tiks = Convert.ToUInt16(Math.Round(Convert.ToDouble(INTERVAL) * RTC.sourceFrequency));
        //        byte[] bytes = BitConverter.GetBytes(tiks);
        //        byte[] wDATA = { Command.COA_set_timeInterval, bytes[1], bytes[0] };
        //        USART.Open();
        //        USART.Write(wDATA, 0, 3);
        //        USART.Close();
        //        trace("Задан временной интервал счёта: " + INTERVAL + "мс (" + tiks + " тиков)");
        //    }
        //    else
        //    {
        //        trace("ОШИБКА! Неверный интервал! Ожидалось: 0 < INTERVAL < 64000; Получено: "+ INTERVAL);
        //    }
        //}
        //
       
        //public byte SPI_DAC(string CHANNEL, string VOLTAGE)
        //{
        //    return SPI_DAC(Convert.ToByte(CHANNEL),Convert.ToUInt16(VOLTAGE));
        //}
        //public byte SPI_DAC(int CHANNEL, int VOLTAGE)
        //{
        //    return SPI_DAC(Convert.ToByte(CHANNEL), Convert.ToUInt16(VOLTAGE));
        //}

        //public byte SPI_ADC(string CHANNEL)
        //{
        //    return SPI_ADC(Convert.ToByte(CHANNEL));
        //}
        //public byte SPI_ADC(int CHANNEL)
        //{
        //    return SPI_ADC(Convert.ToByte(CHANNEL));
        //}

        //public void COA_setTimeInterval(string INTERVAL)
        //{
        //    //COA_setTimeInterval(Convert.ToUInt16(INTERVAL));
        //}
    }
    //---------------------------------------THE END------------------------------------------
}
