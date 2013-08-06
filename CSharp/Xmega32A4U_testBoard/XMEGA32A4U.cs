using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

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
        public const byte delay = 3;        //Задержка при приёме данных (см. transmit())
        //-------------------------------------ПЕРЕМЕННЫЕ-----------------------------------------
        static SerialPort   USART;
        static RichTextBox  tracer;
        static bool         tracer_defined = false;
        static bool         tracer_enabled = true;
        static bool         tracer_log_enabled = false;
        static bool         ERROR = false;
        
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
            public const byte MC_reset =                4;
            public const byte MC_wait =                 5;
            public const byte showTCD2_CNTl =           6;
            public const byte showTCD2_CNTh =           7;

            public const byte showMeByte =              10;

            public const byte LOCK =                    13;

            public const byte MC_get_status =           20;

            public const byte COA_set_MeasureTime =     30;
            public const byte COA_start =               31;
            public const byte COA_get_count =           32;
            public const byte COA_stop =                33;
            public const byte RTC_set_prescaler =       34;
            public const byte COA_getStatus =           35;
            public const byte COA_setMeasureDelay =     36;
            public const byte COA_setMeasureQuantity =  37;

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
            byte prescaler; //1,2,3(8),4(16),5(64),6(256),7(1024)
            ushort prescaler_long;


            public bool   setPrescaler(ushort PRESCALER)
            {
                switch (PRESCALER)
                {
                    case 1: prescaler = 1;
                        break;
                    case 2: prescaler = 2;
                        break;
                    case 8: prescaler = 3;
                        break;
                    case 16: prescaler = 4;
                        break;
                    case 64: prescaler = 5;
                        break;
                    case 256: prescaler = 6;
                        break;
                    case 1024: prescaler = 7;
                        break;
                    default: trace("ОШИБКА! Неверный предделитель!");
                        prescaler = 0;
                        return false;
                }
                return (transmit(Command.RTC_set_prescaler, prescaler)[0] == Command.RTC_set_prescaler);
            }
            public ushort get_Prescaler(uint MILLISECONDS)
            {
                if ((MILLISECONDS >= _RTC.min_ms_div1) && (MILLISECONDS < _RTC.min_ms_div2))
                {
                    prescaler_long = 1;
                }
                else if ((MILLISECONDS >= _RTC.min_ms_div2) && (MILLISECONDS < _RTC.min_ms_div8))
                {
                    prescaler_long = 2;
                }
                else if ((MILLISECONDS >= _RTC.min_ms_div8) && (MILLISECONDS < _RTC.min_ms_div16))
                {
                    prescaler_long = 8;
                }
                else if ((MILLISECONDS >= _RTC.min_ms_div16) && (MILLISECONDS < _RTC.min_ms_div64))
                {
                    prescaler_long = 16;
                }
                else if ((MILLISECONDS >= _RTC.min_ms_div64) && (MILLISECONDS < _RTC.min_ms_div256))
                {
                    prescaler_long = 64;
                }
                else if ((MILLISECONDS >= _RTC.min_ms_div256) && (MILLISECONDS < _RTC.min_ms_div1024))
                {
                    prescaler_long = 256;
                }
                else if ((MILLISECONDS >= _RTC.min_ms_div1024) && (MILLISECONDS < _RTC.max_ms_div1024))
                {
                    prescaler_long = 1024;
                }
                else
                {
                    trace("ОШИБКА! Неверный интервал! Ожидалось: 0 < MILLISECONDS < 2047967; Получено: " + MILLISECONDS);
                    prescaler_long = 0;
                    return 0;
                }
                return prescaler_long;
            }
                public ushort get_Prescaler(string MILLISECONDS)
            {
                return get_Prescaler(Convert.ToUInt32(MILLISECONDS));
            }
            public double get_Freqency()
            {
                return (sourceFrequency / prescaler_long);
            }
            public ushort get_Ticks(uint MILLISECONDS, ushort PRESCALER)
            {
                ushort tiks = Convert.ToUInt16(Math.Round(Convert.ToDouble(MILLISECONDS) * (sourceFrequency / PRESCALER)));
                return tiks;
            }
                public ushort get_Ticks(string MILLISECONDS, string PRESCALER)
            {
                if ((MILLISECONDS != "") && (PRESCALER != ""))
                {
                    return get_Ticks(Convert.ToUInt32(MILLISECONDS), Convert.ToUInt16(PRESCALER));
                }
                else
                {
                    return 0;
                }
            }
                public ushort get_Ticks(string MILLISECONDS, ushort PRESCALER)
            {
                if ((MILLISECONDS != ""))
                {
                    return get_Ticks(Convert.ToUInt32(MILLISECONDS), PRESCALER);
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
                byte Lbyte = 0;
                if (DoubleRange){Lbyte = Lbyte_DoubleRange;}else{Lbyte = Lbyte_NormalRange;}
                byte[] data = {Convert.ToByte(Hbyte + ChannelStep * CHANNEL), Lbyte };
                byte[] rDATA = transmit(Command.ADC_get_voltage, data);
                ushort voltage = 0;
                if (!ERROR)
                {
                    byte adress = 1;
                    adress += Convert.ToByte(rDATA[0] >> 4);
                    voltage = Convert.ToUInt16((Convert.ToUInt16(rDATA[0] & 0xf) << 8) + rDATA[1]);
                    trace("    Ответный адрес канала: " + adress);
                    trace("    Напряжение: " + voltage);
                }
                return voltage;
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

            public bool reset()
            {
                byte[] data = { Reset_Hbyte, Reset_Lbyte };
                if (transmit(Command.DAC_set_voltage, data)[0] == Command.DAC_set_voltage)
                {
                    trace("Напряжения DAC'a сброшены");
                    return true;
                } 
                trace("ОШИБКА ОТКЛИКА! Напряжения DAC'а вероятно не сброшены!");
                return false;
            }
            public bool setVoltage(byte     CHANNEL, ushort     VOLTAGE)
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
                byte[] bytes = BitConverter.GetBytes(VOLTAGE);
                byte[] data = { Convert.ToByte((CHANNEL - 1) * 16 + bytes[1]), bytes[0] };
                return (transmit(Command.DAC_set_voltage, data)[0] == Command.DAC_set_voltage);
            }
            public bool setVoltage(string CHANNEL, string VOLTAGE)
            {
                return setVoltage(Convert.ToByte(CHANNEL), Convert.ToUInt16(VOLTAGE));
            }
            public bool setVoltage(int CHANNEL, int VOLTAGE)
            {
                return setVoltage(Convert.ToByte(CHANNEL), Convert.ToUInt16(VOLTAGE));
            }
        }
        public struct Counter
        {
            //СТРУКТУРА: Счётчики, их три СОА, СОВ и СОС.
            _RTC RTC_;

            const byte state_ready = 0;
            const byte state_stopped = 1;
            const byte state_busy = 2;
            
            public byte state;

            public Counter(_RTC RealTimeCounter)
                : this()
            {
                RTC_ = RealTimeCounter;
            }
            public bool start()
            {
                //ФУНКЦИЯ: Запускаем счётчик, возвращаем состояние счётчика на момент запуска.
                byte[] answer = transmit(Command.COA_start);
                if (answer[0] != state_busy)
                {
                    trace("Счётчик начал счёт...");
                    state = state_busy;
                    return true;
                }
                else
                {
                    trace("Счётчик уже считает! Вы можите остановить счёт командой stop()");
                    return false;
                }
            }
            public bool stop()
            {
                //ФУНКЦИЯ: Останавливаем счётчик
                return (transmit(Command.COA_stop)[0] == Command.COA_stop);
            }
            public UInt32[] getResult()
            {
                //ФУНКЦИЯ: Запрашиваем результат счёта у МК
                UInt32 count = 0;
                
                byte[] rDATA = transmit(Command.COA_get_count);
                UInt32 state = rDATA[0];
                switch (state)
                {
                    case 0:
                        //Счётчик готов (ОН НЕ СЧИТАЛ!)
                        trace("Счётчик готов к работе.");
                        count = Convert.ToUInt32(rDATA[1] * 16777216 + rDATA[2] * 65536 + rDATA[3] * 256 + rDATA[4]);
                        break;
                    case 1:
                        //Счётчик был принудительно остановлен!
                        trace("Счётчик был принудительно остановлен!");
                        break;
                    case 2:
                        //Счётчик успешно завершил счёт, без переполнения
                        trace("Счётчик ещё считает!");
                        break;
                    default:
                        //Счётчик переполнился <state> раз
                        trace("Счётчик был переполнен! Количество переполнений: " + (state - 2).ToString());
                        count = Convert.ToUInt32(rDATA[1] * 16777216 + rDATA[2] * 65536 + rDATA[3] * 256 + rDATA[4]);
                        break;
                }
                return new UInt32[] { state, count };
            }
            public bool setMeasureTime(uint MILLISECONDS)
            {
                //ФУНКЦИЯ: Задаёт количество тиков для RTC через интервал в миллисекундах
                ushort RTC_prescaler = RTC_.get_Prescaler(MILLISECONDS);
                RTC_.setPrescaler(RTC_prescaler);
                ushort ticks = RTC_.get_Ticks(MILLISECONDS,RTC_prescaler);
                byte[] bytes_ticks = BitConverter.GetBytes(ticks);
                byte[] data = { bytes_ticks[1], bytes_ticks[0]};
                if (transmit(Command.COA_set_MeasureTime, data)[0] == Command.COA_set_MeasureTime)
                {
                    //trace("Задан временной интервал счёта: " + MILLISECONDS + "мс (" + ticks + " тиков)");
                    return true;
                }
                return false;
            }
                public bool setMeasureTime(string MILLISECONDS)
            {
                if (MILLISECONDS != "")
                {
                    return setMeasureTime(Convert.ToUInt32(MILLISECONDS));
                }
                return false;
            }
            public bool setMeasureDelay(byte MILLISECONDS)
            {
                //ФУНКЦИЯ: Задаёт задержку между измерениями
                return (transmit(Command.COA_setMeasureDelay, MILLISECONDS)[0] == Command.COA_setMeasureDelay);
            }
                public bool setMeasureDelay(string MILLISECONDS)
            {
                return setMeasureDelay(Convert.ToByte(MILLISECONDS));
            }
            public bool setMeasureQuantity(ushort QUANTITY)
            {
                //ФУНКЦИЯ: Задаёт количество измерений
                byte[] bytes = BitConverter.GetBytes(QUANTITY);
                byte[] data = { bytes[1], bytes[0] };
                return (transmit(Command.COA_setMeasureQuantity, data)[0] == Command.COA_setMeasureQuantity);
                 
            }
                public bool setMeasureQuantity(string QUANTITY)
            {
                return setMeasureQuantity(Convert.ToUInt16(QUANTITY));
            }
        }
        //--------------------------------------ОБЪЕКТЫ-------------------------------------------
        public _RTC RTC = new _RTC();
        public Counter COA;
        //public Counter COB = new Counter();
        //public Counter COC = new Counter();
        public SPI_ADC ADC = new SPI_ADC();
        public SPI_DAC DAC = new SPI_DAC();
        
        //--------------------------------------ФУНКЦИИ-------------------------------------------
        public void setTracer(RichTextBox TRACER)
        {
            //ФУНКЦИЯ: Задаём трэйсер для отладки\ответов
            tracer = TRACER;
            tracer_defined = true;
        }
        //ИНТЕРФЕЙСНЫЕ
        static public void trace(string text)
        {
            //ФУНКЦИЯ: Выводит text в RichTextBox и в файл
            string TEXT = Environment.NewLine + "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + text;
            if (tracer_defined && tracer_enabled)
            {
                tracer.AppendText(TEXT);
                tracer.ScrollToCaret();
            }
            if (tracer_log_enabled)
            {
                System.IO.File.AppendAllText("Log.txt", TEXT);
            }
        }
        static public void trace_attached(string text)
        {
            //ФУНКЦИЯ: Выводит text в RichTextBox
            string TEXT = text;
            if (tracer_defined && tracer_enabled)
            {
                tracer.AppendText(TEXT);
                tracer.ScrollToCaret();
            }
            if (tracer_log_enabled)
            {
                System.IO.File.AppendAllText("Log.txt", TEXT);
            }
        }
        public void tracer_enable(bool enable)
        {
            tracer_enabled = enable;
        }
        public void log_enable(bool enable)
        {
            tracer_log_enabled = enable;
        }
        public bool checkErrors()
        {
            return ERROR;
        }
        //USART
        public void setUSART(SerialPort COM_PORT)
        {
            //ФУНКЦИЯ: Задаём порт, припомози которого будем общаться с МК
            USART = COM_PORT;
            COA = new Counter(RTC);
            
            trace("Инициализация " + DateTime.Now.ToString("dd MMMM yyyy"));
        }
        static byte calcCheckSum(byte[] data)
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
        static byte[] transmit(List<byte> DATA)
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
            Thread.Sleep(delay);
            trace("     Передача завершена!");
            trace("     Приём...");                         //Приём-приём
            byte rBYTE;                                     //Принятый байт
            bool lock_recieved = false;                     //Затвор ещё небыл получен
            bool key_recived = false;                       //Ключ ещё не был получен
            byte BytesToReadQuantity = Convert.ToByte(USART.BytesToRead);
            trace("         Данные на приём:" + BytesToReadQuantity);
            if (BytesToReadQuantity == 0)
            {
                trace("ОШИБКА ПРИЁМА ДАННЫХ! Не было получено никаких данных!");
                USART.Close();
                ERROR = true;
                return new byte[] {0};
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
                    USART.Close();
                    ERROR = true;
                    return new byte[] {0};
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
                        ERROR = true;
                        return new byte[] { 0 };
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
                            ERROR = true;
                            return new byte[] { 0 };
                        }
                    }
                    return rDATA.ToArray();
                }
                trace("ОШИБКА ПРИЁМА ДАННЫХ! Не был получен затвор!");
                ERROR = true;
            }
            trace("ОШИБКА ПРИЁМА ДАННЫХ! Не был получен ключ!");
            ERROR = true;
            return new byte[] { 0 };
        }
            static byte[] transmit(byte[] DATA)
        {
            return transmit(DATA.ToList());
        }
            static byte[] transmit(byte COMMAND)
        {
            return transmit((new byte[] { COMMAND }).ToList());
        }
            static byte[] transmit(byte COMMAND, byte DATA)
        {
            return transmit((new byte[] { COMMAND, DATA }).ToList());
        }
            static byte[] transmit(byte COMMAND, byte[] DATA)
        {
            List<byte> data = new List<byte>();
            data.Add(COMMAND);
            data.AddRange(DATA);
            return transmit(data);
        }
            static byte[] transmit(byte COMMAND, List<byte> DATA)
        {
            //Проверить - не надо ли создавать новый список
            DATA.Insert(0, COMMAND);
            return transmit(DATA);
        }
        //ОТЛАДОЧНЫЕ
        public void     showMeByte(byte     BYTE)
        {
            transmit(Command.showMeByte, BYTE);
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
        void trace_error(byte[] DATA)
        {
            //ФУНКЦИЯ: Определение что за неверные данные такие пришли, и что нужно делать
            //сохраним данные в log
            trace("-----------------ОШИБКА!-----------------");
            trace("! Принятые данные:");
            foreach (byte b in DATA)
            {
                trace("!     ");
            }
            trace("--------------Конец ошибки---------------");
        }
        public bool     reset()
        {
            //ФУНКЦИЯ: Програмная перезагрузка микроконтроллера
            return (transmit(Command.MC_reset)[0] == Command.MC_reset);
        }
        public byte     getStatus()
        {
            //ФУНКЦИЯ: Получает статус у МК
            return transmit(Command.MC_get_status)[0];
        }
        public byte     getVersion()
        {
            //ФУНКЦИЯ: Получает статус у МК
            return transmit(Command.MC_get_version)[0];
        }
        public string   getBirthday()
        {
            //ФУНКЦИЯ: Получает статус у МК
            UInt32 birthday = 0;
            string answer = "00000000";
            byte[] recDATA = transmit(Command.MC_get_birthday);
            if (!ERROR)
            {
                birthday = Convert.ToUInt32(recDATA[3]) * 16777216 + Convert.ToUInt32(recDATA[2]) * 65536 + Convert.ToUInt32(recDATA[1]) * 256 + Convert.ToUInt32(recDATA[0]);

                answer = birthday.ToString();
                answer = answer[6] + "" + answer[7] + " " + answer[4] + answer[5] + " " + answer[0] + answer[1] + answer[2] + answer[3];

                
            }
            return answer;
        }
        public string   getCPUfrequency()
        {
            UInt32 frequency = 0;
            byte[] recDATA = transmit(Command.MC_get_CPUfreq);
            if (!ERROR)
            {
                frequency = Convert.ToUInt32(recDATA[3]) * 16777216 + Convert.ToUInt32(recDATA[2]) * 65536 + Convert.ToUInt32(recDATA[1]) * 256 + Convert.ToUInt32(recDATA[0]);
                return frequency.ToString() + " Гц";
            }
            return "0";
        }
        //--------------------------------------ЗАМЕТКИ-------------------------------------------
    }
    //---------------------------------------THE END------------------------------------------
}
