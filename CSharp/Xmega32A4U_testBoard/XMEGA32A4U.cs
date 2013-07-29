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
        public const byte classVersion = 6;
        //-------------------------------------ПЕРЕМЕННЫЕ-----------------------------------------
        static SerialPort   USART;
        static RichTextBox  tracer;
        static bool         tracer_defined = false;
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

            public const byte MC_get_status =           20;

            public const byte COA_set_timeInterval =    30;
            public const byte COA_start =               31;
            public const byte COA_get_count =           32;
            public const byte COA_stop =                33;

            public const byte DAC_set_voltage =         40;
            public const byte ADC_get_voltage =         41;
        };
        struct RTC
        {
            //СТРУКТУРА: Счётчик реального времени - хранилище констант
            public const double sourceFrequency = 32.768;//кГц
            //public const byte prescaler = 64;
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
            public ushort   getResult()
            {
                //ФУНКЦИЯ: Запрашиваем результат счёта у МК
                byte[] wDATA = { Command.COA_get_count };
                byte[] rDATA = { 0, 0, 0 };
                ushort count = 0;
                USART.Open();
                USART.Write(wDATA, 0, 1);
                try
                {
                    rDATA[0] = Convert.ToByte(USART.ReadByte());
                    rDATA[1] = Convert.ToByte(USART.ReadByte());
                    rDATA[2] = Convert.ToByte(USART.ReadByte());
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
                    trace("ОШИБКА ОТКЛИКА! Ожидалось: " + Response.COX_done + "|" + Response.COX_busy + "|" + Response.COX_stoped + ", получено: " + rDATA[0]);
                }
                count = Convert.ToUInt16(rDATA[1] * 256 + rDATA[2]);
                return count;
            }
            public void     setTimeInterval(ushort INTERVAL)
            {
                //ФУНКЦИЯ: Задаёт количество тиков для RTC через интервал в миллисекундах
                if ((INTERVAL < 64000) && (INTERVAL >= 0))
                {
                    ushort tiks = Convert.ToUInt16(Math.Round(Convert.ToDouble(INTERVAL) * RTC.sourceFrequency));
                    byte[] bytes = BitConverter.GetBytes(tiks);
                    byte[] wDATA = { Command.COA_set_timeInterval, bytes[1], bytes[0] };
                    USART.Open();
                    USART.Write(wDATA, 0, 3);
                    USART.Close();
                    trace("Задан временной интервал счёта: " + INTERVAL + "мс (" + tiks + " тиков)");
                }
                else
                {
                    trace("ОШИБКА! Неверный интервал! Ожидалось: 0 < INTERVAL < 64000; Получено: " + INTERVAL);
                }
            }
            public void     setTimeInterval(string INTERVAL)
            {
                setTimeInterval(Convert.ToUInt16(INTERVAL));
            }

        }
        //--------------------------------------ОБЪЕКТЫ-------------------------------------------
        public Counter COA = new Counter();
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
        }
        public void     setTracer(RichTextBox TRACER)
        {
            //ФУНКЦИЯ: Задаём трэйсер для отладки\ответов
            tracer = TRACER;
            tracer_defined = true;
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
        //ОТЛАДОЧНЫЕ
        public void     showMeByte(byte     BYTE)
        {
            //ФУНКЦИЯ: Выводит на светодиоды BYTE
            byte[] wDATA = { Command.showMeByte, BYTE };
            USART.Open();
            USART.Write(wDATA, 0, 2);
            USART.Close();
        }
        public void     showMeByte(string   BYTE)
        {
            showMeByte(Convert.ToByte(BYTE));
        }
        public void     showMeByte(int      BYTE)
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
            byte[] wDATA = { Command.MC_get_status };
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
            if (rDATA[0] != Response.status)
            {
                trace("ОШИБКА ОТКЛИКА! Ожидалось: 1, получено: " + rDATA[0]);
            }
            return rDATA[1];
        }
        public byte     getVersion()
        {
            //ФУНКЦИЯ: Получает статус у МК
            byte[] wDATA = { Command.MC_get_version };
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
            if (rDATA[0] != Response.version)
            {
                trace("ОШИБКА ОТКЛИКА! Ожидалось: " + Response.version + ", получено: " + rDATA[0]);
            }
            return rDATA[1];
        }
        public string   getBirthday()
        {
            //ФУНКЦИЯ: Получает статус у МК
            UInt32 birthday = 0;
            string answer = "00000000";
            byte[] wDATA = { Command.MC_get_birthday };
            byte[] rDATA = { 0, 0, 0, 0, 0 };
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
            if (rDATA[0] != Response.birthday)
            {
                trace("ОШИБКА ОТКЛИКА! Ожидалось: " + Response.birthday + ", получено: " + rDATA[0]);
            }

            birthday = Convert.ToUInt32(rDATA[4]) * 16777216 + Convert.ToUInt32(rDATA[3]) * 65536 + Convert.ToUInt32(rDATA[2]) * 256 + Convert.ToUInt32(rDATA[1]);

            answer = birthday.ToString();
            answer = answer[6] + "" + answer[7] + " " + answer[4] + answer[5] + " " + answer[0] + answer[1] + answer[2] + answer[3];

            return answer;
        }
        public string   getCPUfrequency()
        {
            UInt32 frequency = 0;
            byte[] wDATA = { Command.MC_get_CPUfreq };
            byte[] rDATA = { 0, 0, 0, 0, 0 };
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
            if (rDATA[0] != Response.CPU_frequency)
            {
                trace("ОШИБКА ОТКЛИКА! Ожидалось: " + Response.CPU_frequency + ", получено: " + rDATA[0]);
            }

            frequency = Convert.ToUInt32(rDATA[4]) * 16777216 + Convert.ToUInt32(rDATA[3]) * 65536 + Convert.ToUInt32(rDATA[2]) * 256 + Convert.ToUInt32(rDATA[1]);
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
