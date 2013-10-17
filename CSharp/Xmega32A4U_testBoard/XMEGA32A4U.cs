using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace Xmega32A4U_testBoard
{
    /// <summary>
    /// Класс связи ПК с XMega32A4U
    /// </summary>
    class XMEGA32A4U
    {
        //========================================================================================
        //=========================КЛАСС СВЯЗИ ПК С XMEGA32A4U ПО RS232===========================
        //========================================================================================
        //
        //-------------------------------------ПОЯСНЕНИЯ------------------------------------------
        //
        //-------------------------------------ПЕРЕМЕННЫЕ-----------------------------------------
        static SerialPort USART;        //COM порт, его необходимо задать .Chip.setUSART(SerialPort)
        static bool USART_defined = false;  //СОМ-порт не определён.
        static RichTextBox tracer;      //Трэйсер отладочных сообщений, задаётся .Tester.setTracer(RichTextBox)
        static bool tracer_defined = false;  //Трэйсер не задан. 
        static bool tracer_enabled = true;   //Трэйсер включен. .Tester.enableTracer(bool) - для вкл\выкл
        static bool tracer_transmit_enabled = true; //Трейсер в функции transmit включён\выключен
        static bool tracer_log_enabled = false;     //Ведение лога в Log.txt отключено. .Tester.enableLog(bool) - для вкл\выкл
        static List<string> ErrorList = new List<string>();     //Лист всех ошибок. Получает при помощи .getErrorList()
        static byte CommandStack;       //Счётчик выполненных команд (см. .Chip.checkCommandStack())
        const byte delay = 3;           //Задержка при приёме данных (см. transmit())
        //-------------------------------------СТРУКТУРЫ------------------------------------------
        struct Error
        {
            //СТРУКТУРА: Хранилище констант - кодов ошибок
            //ПОЯСНЕНИЯ: Ошибка приходит в формате <key><Error.Token><ErrorNum><data[]><CS><lock>
            public const byte Token =           0;    //Есть ошибка
            //ErrorNums...
            public const byte DecoderError =    1;       //Такое команды не существует
            public const byte KeyError =        2;       //Не был получен ключ
            public const byte LockError =       3;       //Не был получен замок
            public const byte CheckSumError =   4;       //Неверная контрольная сумма
            //public const byte wrong_SPI_DEVICE_Number
        }
        struct Command
        {
            //СТРУКТУРА: Хранилище констант - кодов команд
            public struct Chip
            {
                //Коды команд микросхемы микроконтроллера
                public const byte getVersion =         1;
                public const byte getBirthday =        2;
                public const byte getCPUfrequency =    3;
                public const byte getStatus =          20;
                public const byte reset =              4;
                public const byte wait =               5;
                
            }
            public struct RTC
            {
                //Коды команд счётчиков
                public const byte setMeasureTime =     30;
                public const byte startMeasure =       31;
                public const byte getResult =          32;
                public const byte stopMeasure =        33;
                public const byte setPrescaler =       34;
                public const byte getStatus =          35;
            }
            public struct TIC
            {
                //Коды команд для микроконтроллера насоса
                public const byte sendToTIC = 11;
            }
            public struct SPI
            {
                //Коды команд для SPI устройств
                public const byte DAC_setVoltage =                      40;
                public const byte ADC_getVoltage =                      41;
                public struct IonSource
                {
                    //Коды команд Ионного Источника
                    public struct EmissionCurrent
                    {
                        public const byte setVoltage = 42;
                        public const byte getVoltage = 60;
                    }
                    public struct Ionization
                    {
                        public const byte setVoltage = 43;
                        public const byte getVoltage = 61;
                    }
                    public struct F1
                    {
                        public const byte setVoltage = 44;
                        public const byte getVoltage = 62;
                    }
                    public struct F2
                    {
                        public const byte setVoltage = 45;
                        public const byte getVoltage = 63;
                    }
                }
                public struct Detector
                {
                    //Коды команд Детектора
                    public struct DV1
                    {
                        public const byte setVoltage = 46;
                        public const byte getVoltage = 64;
                    }
                    public struct DV2
                    {
                        public const byte setVoltage = 47;
                        public const byte getVoltage = 65;
                    }
                    public struct DV3
                    {
                        public const byte setVoltage = 48;
                        public const byte getVoltage = 66;
                    }
                }
                public struct Inlet
                {
                    //Коды команд Натекателя
                    public const byte setVoltage = 49;
                    public const byte getVoltage = 67;
                }
                public struct Heater
                {
                    //Коды команд Нагревателя
                    public const byte setVoltage = 50;
                    public const byte getVoltage = 68;
                }
                public struct Scaner
                {
                    //Коды команд Сканера
                    public struct ParentScan
                    {
                        public const byte setVoltage = 51;
                        public const byte getVoltage = 70;
                    }
                    public struct Scan
                    {
                        public const byte setVoltage = 52;
                        public const byte getVoltage = 70;
                    }
                }
                public struct Condensator
                {
                    //Коды команд Конденсатора
                    public const byte setVoltage = 53;
                    public const byte getPositiveVoltage = 70;
                    public const byte getNegativeVoltage = 70;
                }
            }
            public const byte LOCK = 13;
            public const byte KEY = 58;
            public struct TEST
            {
                //Кооды команд отладки
                public const byte showMeByte = 10;
                public const byte checkCommandStack = 8;
            }
            public const byte setFlags = 80;
            //public const byte EMV1 = ;
            //public const byte EMV2 = ;
            //public const byte EMV3 = ;
            //public const byte HVE = ;
        }

        /*public struct SPI_ADC
        {
            //СТРУКТУРА: АЦП. Тестовая
            const byte Hbyte = 127;
            const byte Lbyte_DoubleRange = 16;
            const byte Lbyte_NormalRange = 48;
            const byte ChannelStep = 4;

            public bool DoubleRange;

            public ushort getVoltage(byte CHANNEL)
            {
                //Последовательность 11 битного слова, которое надо передать ADC'у: Или это просто регистр?
                //+-------+-----+-------+------+------+------+-----+-----+--------+-------+-------+--------+
                //| WRITE | SEQ | DONTC | ADD2 | ADD1 | ADD0 | PM1 | PM0 | SHADOW | DONTC | RANGE | CODING |
                //+-------+-----+-------+------+------+------+-----+-----+--------+-------+-------+--------+
                //
                //WRITE - 1 - тогда ADC запишет следующие 11 бит. Иначе пропустит мимо ушей
                //SEQ - ?
                //DONTC - не парься
                //         _______________________________________
                //         |____Адрес__|_________________|       |
                //         | 8 | 7 | 6 | Имя  | № вывода | Канал |  
                //         |---+---+---+------+----------+-------|
                //        | 0 | 0 | 0 | Vin0 |    16    |   1   |
                //         | 0 | 0 | 1 | Vin1 |    15    |   2   |
                //ADD2     | 0 | 1 | 0 | Vin2 |    14    |   3   |
                //ADD1     | 0 | 1 | 1 | Vin3 |    13    |   4   |
                //ADD0     | 1 | 0 | 0 | Vin4 |    12    |   5   |
                //         | 1 | 0 | 1 | Vin5 |    11    |   6   |
                //         | 1 | 1 | 0 | Vin6 |    10    |   7   |
                //         | 1 | 1 | 1 | Vin7 |    9     |   8   |
                //         |---+---+---+------+----------+-------| 
                //
                //PM1 и PM0 - управление питанием (1 1 - нормальный режим, самый быстрый)
                //SHADOW - ?
                //DONTC - не парься...
                //RANGE - 1 - стандартный диапазон 0...REF | 0 - удвоенный 0...2xREF
                //CODING - кодирует ответ ADC: 0 - the output coding for the part is twos complement | 
                //                             1 - the output coding from the part is straight binary (for the next conversion).
                //                             
                //
                //Сладкая парочка SEQ и SHADOW:
                // 0 0 - Каналы оцифровываются независимо. Ни какой "последовательной функцией" тут не пахнет
                // other - какая-то муть с "последовательными функциями" оцифровки и программированием shadow-регистра
                //
                //Составляем слово:
                // 1 0 0 ххх 11 0 0 1 1 '0000'
                // 131 + 16
                trace_attached(Environment.NewLine);
                trace("ADC.getVoltage(" + CHANNEL + ")");
                trace("ADC.getVoltage(" + CHANNEL + "): DoubleRange = "+DoubleRange);
                byte Lbyte = 0;
                if (DoubleRange) { Lbyte = Lbyte_DoubleRange; } else { Lbyte = Lbyte_NormalRange; }
                byte[] data = { Convert.ToByte(Hbyte + ChannelStep * CHANNEL), Lbyte };
                byte[] rDATA = transmit(Command.SPI.ADC_getVoltage, data);
                ushort voltage = 0;
                byte adress = 1;
                adress += Convert.ToByte(rDATA[0] >> 4);
                voltage = Convert.ToUInt16((Convert.ToUInt16(rDATA[0] & 0xf) << 8) + rDATA[1]);
                trace("ADC.getVoltage(" + CHANNEL + "): Ответный адрес канала: " + adress);
//Если адрес != Каналу - ошибка!
                trace("ADC.getVoltage(" + CHANNEL + "): Напряжение: " + voltage);
                return voltage;
            }
                public ushort getVoltage(string CHANNEL)
            {
                return getVoltage(Convert.ToByte(CHANNEL));
            }
                public ushort getVoltage(int CHANNEL)
            {
                return getVoltage(Convert.ToByte(CHANNEL));
            }
        }
        public struct SPI_DAC
        {
            //СТРУКТУРА: ЦАП. Тестовая
            const byte Reset_Hbyte = 255;
            const byte Reset_Lbyte = 255;

            public bool reset()
            {
                trace_attached(Environment.NewLine);
                trace("DAC.reset()");
                byte[] data = { Reset_Hbyte, Reset_Lbyte };
                if (transmit(Command.SPI.DAC_setVoltage, data)[0] == Command.SPI.DAC_setVoltage)
                {
                    trace("DAC.reset(): Операция выполнена успешно!");
                    return true;
                }
                trace("DAC.reset(): ОШИБКА ОТКЛИКА! Напряжения DAC'а вероятно не сброшены!");
                return false;
            }
            public bool setVoltage(byte CHANNEL, ushort VOLTAGE)
            {
                //ФУНКЦИЯ: Посылаем DAC'у адресс канала и напряжение на нём, получаем отклик
                //  ____________________________________________________________
                 //*  |_____Адрес____|__________________|       |    Диапазон    |
                 //*  | 14 | 13 | 12 |  Имя  | № вывода | Канал |ADRESS_and_Hbyte|  
                 //*  |----+----+----+-------+----------+-------+----------------|
                 //*  | 0  | 0  | 0  | DAC A |    4     |   1   |    0...15      |
                 //*  | 0  | 0  | 1  | DAC B |    5     |   2   |    16...31     |
                 //*  | 0  | 1  | 0  | DAC C |    6     |   3   |    32...47     |
                 //*  | 0  | 1  | 1  | DAC D |    7     |   4   |    48...63     |
                 //*  | 1  | 0  | 0  | DAC E |    10    |   5   |    64...79     |
                 //*  | 1  | 0  | 1  | DAC F |    11    |   6   |    80...95     |
                 //*  | 1  | 1  | 0  | DAC G |    12    |   7   |    96...111    |
                 //*  | 1  | 1  | 1  | DAC H |    13    |   8   |    112...127   |
                 //*  |----+----+----+-------+----------+-------+----------------| 
                 //* 
                 //*  ADRESS_and_Hbyte =  0      111         xxxx
                 //*                     D\C    адрес    Старший байт 
                 //*  Lbyte =   xxxx xxxx
                 //*          Младший байт
                 //* 
                 //* D\C -> 0 - адрес + напряжение | 1 - управляющий сигнал  (1111 1111 1111 1111 - полный сброс)
                 //* 
                 //* VOLTAGE = 0...4095 = 0_0 ... 15_255
                 //* 
                 //
                //Формируем данные на отправку
                trace_attached(Environment.NewLine);
                trace("DAC.setVoltage("+CHANNEL+", "+VOLTAGE+")");
                byte[] bytes = BitConverter.GetBytes(VOLTAGE);
                byte[] data = { Convert.ToByte((CHANNEL - 1) * 16 + bytes[1]), bytes[0] };
                if (transmit(Command.SPI.DAC_setVoltage, data)[0] == Command.SPI.DAC_setVoltage)
                {
                    trace("DAC.setVoltage(" + CHANNEL + ", " + VOLTAGE + "): Операция выполнена успешно!");
                    return true;
                }
                trace("DAC.setVoltage(" + CHANNEL + ", " + VOLTAGE + "): ОШИБКА ОТКЛИКА!");
//ОШИБКА ОТКЛИКА!
                return false;
            }
                public bool setVoltage(string CHANNEL, string VOLTAGE)
            {
                return setVoltage(Convert.ToByte(CHANNEL), Convert.ToUInt16(VOLTAGE));
            }
                public bool setVoltage(int CHANNEL, int VOLTAGE)
            {
                return setVoltage(Convert.ToByte(CHANNEL), Convert.ToUInt16(VOLTAGE));
            }
        }*/
        //---------------------------------------КЛАССЫ--------------------------------------------
        public class RTCounterAndCO
        {
            //КЛАСС: Счётчик реального времени и счётчики импульсов
            struct Constants
            {
                //СТРУКТУРА: Хранилище констант.
                public const double sourceFrequency = 32.768;//кГц - опорная частота таймера
                //Коды состояний
                public const byte Status_ready = 0;     //Готов
                public const byte Status_stopped = 1;   //Остановлен
                public const byte Status_busy = 2;      //Считает
                //интервалы для предделителей
                public const int min_ms_div1 = 0;
                public const int min_ms_div2 = 2000;
                public const int min_ms_div8 = 4000;
                public const int min_ms_div16 = 16000;
                public const int min_ms_div64 = 32000;
                public const int min_ms_div256 = 127996;
                public const int min_ms_div1024 = 511981;
                public const int max_ms_div1024 = 2047925;
            }
            public class counter
            {
                //КЛАСС: Счётчик. Только хранит значения.
                /// <summary>
                /// Количество переполнений счётчика
                /// </summary>
                public byte overflows;  //Количество переполений счётчика
                /// <summary>
                /// Сосчитанный результат
                /// </summary>
                public UInt32 Result;   //Сосчитанный результат
            }
            /// <summary>
            /// Счётчик А (32-разрядный)
            /// </summary>
            public counter COA = new counter();
            /// <summary>
            /// Счётчик В (32-разрядный)
            /// </summary>
            public counter COB = new counter();
            /// <summary>
            /// Счётчик С (16-разрядный)
            /// </summary>
            public counter COC = new counter();
            byte prescaler; //Предделитель в byte : 1,2,3(8),4(16),5(64),6(256),7(1024)
            ushort prescaler_long; //Предделитель в реальном коэффициенте деления (см. prescaler цифры в скобках)
            bool setPrescaler(ushort PRESCALER)
            {
                //ФУНКЦИЯ: Устанавливает предделитель (в МК)
                trace("Counters.setPrescaler("+PRESCALER+")");
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
                    default: trace("Counters.setPrescaler("+PRESCALER+"): ОШИБКА! Неверный предделитель!");
                        prescaler = 0;
                        return false;
                }
                if((transmit(Command.RTC.setPrescaler, prescaler)[0] == Command.RTC.setPrescaler))
                {
                    trace("Counters.setPrescaler(" + PRESCALER + "): Операция выполнена успешно!");
                    return true;
                }
                trace("Counters.setPrescaler(" + PRESCALER + "): ОШИБКА ОТКЛИКА!");
                return false;
            }
            //Видимые функции
            /// <summary>
            /// Вычисляет, устанавливает и возвращает предделитель RTC 
            /// <para>ПРИМЕЧАНИЕ: без участия МК</para>
            /// </summary>
            /// <param name="MILLISECONDS">Время измерения в миллисекундах от 0 до 2047925</param>
            /// <returns>?</returns>
            public ushort getRTCprescaler(uint MILLISECONDS)
            {
                //ФУНКЦИЯ: Вычисляет, сохраняет и возвращает предделитель.
                if ((MILLISECONDS >= Constants.min_ms_div1) && (MILLISECONDS < Constants.min_ms_div2))
                {
                    prescaler_long = 1;
                }
                else if ((MILLISECONDS >= Constants.min_ms_div2) && (MILLISECONDS < Constants.min_ms_div8))
                {
                    prescaler_long = 2;
                }
                else if ((MILLISECONDS >= Constants.min_ms_div8) && (MILLISECONDS < Constants.min_ms_div16))
                {
                    prescaler_long = 8;
                }
                else if ((MILLISECONDS >= Constants.min_ms_div16) && (MILLISECONDS < Constants.min_ms_div64))
                {
                    prescaler_long = 16;
                }
                else if ((MILLISECONDS >= Constants.min_ms_div64) && (MILLISECONDS < Constants.min_ms_div256))
                {
                    prescaler_long = 64;
                }
                else if ((MILLISECONDS >= Constants.min_ms_div256) && (MILLISECONDS < Constants.min_ms_div1024))
                {
                    prescaler_long = 256;
                }
                else if ((MILLISECONDS >= Constants.min_ms_div1024) && (MILLISECONDS < Constants.max_ms_div1024))
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
                /// <summary>
            /// Вычисляет, устанавливает и возвращает предделитель RTC 
            /// <para>ПРИМЕЧАНИЕ: без участия МК</para>
            /// </summary>
            /// <param name="MILLISECONDS">Время измерения в миллисекундах от 0 до 2047925</param>
            /// <returns>?</returns>
                public ushort getRTCprescaler(string MILLISECONDS)
            {
                return getRTCprescaler(Convert.ToUInt32(MILLISECONDS));
            }
            /// <summary>
            /// Возвращает частоту RTC в соответствии с предделителем
            /// <para>ПРИМЕЧАНИЕ: без участия МК</para>
            /// </summary>
            public double getRTCfrequency()
            {
                //ФУНКЦИЯ: Возвращает итоговую частоту RTC в соответствии с сохраннённым предделителем.
                return (Constants.sourceFrequency / prescaler_long);
            }
            /// <summary>
            /// Возвращает количество тиков RTC для отсчёта заданного времени с заданным предделителем 
            /// <para>ПРИМЕЧАНИЕ: без участия МК</para>
            /// </summary>
            /// <param name="MILLISECONDS">Время измерения в миллисекундах от 0 до 2047925</param>
            /// <param name="PRESCALER">Предделитель. Возможные значения: 1,2,8,16,64,256,1024</param>
            /// <returns></returns>
            public ushort getRTCticks(uint MILLISECONDS, ushort PRESCALER)
            {
                //ФУНКЦИЯ: Вычисляет количество тиков в соответствии с временем и предделителем. Возвращает количество тиков
                ushort tiks = Convert.ToUInt16(Math.Round(Convert.ToDouble(MILLISECONDS) * (Constants.sourceFrequency / PRESCALER)));
                return tiks;
            }
                /// <summary>
            /// Возвращает количество тиков RTC для отсчёта заданного времени с заданным предделителем 
            /// <para>ПРИМЕЧАНИЕ: без участия МК</para>
            /// </summary>
            /// <param name="MILLISECONDS">Время измерения в миллисекундах от 0 до 2047925</param>
            /// <param name="PRESCALER">Предделитель. Возможные значения: 1,2,8,16,64,256,1024</param>
            /// <returns></returns>
                public ushort getRTCticks(string MILLISECONDS, string PRESCALER)
            {
                if ((MILLISECONDS != "") && (PRESCALER != ""))
                {
                    return getRTCticks(Convert.ToUInt32(MILLISECONDS), Convert.ToUInt16(PRESCALER));
                }
                else
                {
                    return 0;
                }
            }
                /// <summary>
                /// Возвращает количество тиков RTC для отсчёта заданного времени с заданным предделителем 
                /// <para>ПРИМЕЧАНИЕ: без участия МК</para>
                /// </summary>
                /// <param name="MILLISECONDS">Время измерения в миллисекундах от 0 до 2047925</param>
                /// <param name="PRESCALER">Предделитель. Возможные значения: 1,2,8,16,64,256,1024</param>
                /// <returns></returns>
                public ushort getRTCticks(string MILLISECONDS, ushort PRESCALER)
            {
                if ((MILLISECONDS != ""))
                {
                    return getRTCticks(Convert.ToUInt32(MILLISECONDS), PRESCALER);
                }
                else
                {
                    return 0;
                }
            }
            /// <summary>
            /// Задаёт время измерения в миллисекундах. 
            /// <para>Возвращает:</para>
            /// <para>true - операция выполнена успешно</para>
            /// <para>false - операция отменена (счётчики уже считают, в этом случае их надо сначала остановить командой .stopMeasure();)</para>
            /// </summary>
            /// <param name="MILLISECONDS">Время измерения в миллисекундах от 0 до 2047925</param>
            public bool setMeasureTime(uint MILLISECONDS)
            {
                //ФУНКЦИЯ: Вычисляет, сохраняет и устанавливает предделитель, вычисляет и устанавливает количество тиков для RTC через интервал в миллисекундах
                trace_attached(Environment.NewLine);
                ushort RTC_prescaler = getRTCprescaler(MILLISECONDS);
                if (!setPrescaler(RTC_prescaler))
                {
                    trace("Counters.setMeasureTime(" + MILLISECONDS + "): Операция отменена! ОШИБКА ПРЕДДЕЛИТЕЛЯ!");
                    return false;
                }
                trace("Counters.setMeasureTime(" + MILLISECONDS + ")");
                ushort ticks = getRTCticks(MILLISECONDS, RTC_prescaler);
                byte[] bytes_ticks = BitConverter.GetBytes(ticks);
                byte[] data = { bytes_ticks[1], bytes_ticks[0] };
                if (transmit(Command.RTC.setMeasureTime, data)[0] == 1)
                {
                    trace("Counters.setMeasureTime(" + MILLISECONDS + "): Задан временной интервал счёта: " + MILLISECONDS + "мс (" + ticks + " тиков)");
                    trace("Counters.setMeasureTime(" + MILLISECONDS + "): Операция выполнена успешно!");
                    return true;
                }
                trace("Counters.setMeasureTime(" + MILLISECONDS + "): Операция не была выполнена! Счётчики считают!");
                return false;
            }
                /// <summary>
            /// Задаёт время измерения в миллисекундах. 
            /// <para>Возвращает:</para>
            /// <para>true - операция выполнена успешно</para>
            /// <para>false - операция отменена (счётчики уже считают, в этом случае их надо сначала остановить командой .stopMeasure();)</para>
            /// </summary>
            /// <param name="MILLISECONDS">Время измерения в миллисекундах от 0 до 2047925</param>
                public bool setMeasureTime(string MILLISECONDS)
            {
                if (MILLISECONDS != "")
                {
                    return setMeasureTime(Convert.ToUInt32(MILLISECONDS));
                }
                return false;
            }
            /// <summary>
            /// Запускает счётчики и RTC на заданный заранее промежуток времени (.setMeasureTime(MILLISECONDS)).
            /// <para>Возвращает:</para>
            /// <para>true - операция выполнена успешно. Счётчики начали счёт.</para>
            /// <para>false - операция отменена (счётчики уже считают, в этом случае их надо сначала остановить командой .stopMeasure();)</para>
            /// </summary>
            public bool startMeasure()
            {
                //ФУНКЦИЯ: Запускаем счётчик, возвращает true если счёт начался, false - счётчик уже считает
                trace_attached(Environment.NewLine);
                trace("Counters.startMeasure()");
                byte state = transmit(Command.RTC.startMeasure)[0];
                switch (state)
                {
                    case Constants.Status_busy:
                        trace("Counters.startMeasure(): Операция отменена! Счётчики уже считают!");
                        return false;
                    case Constants.Status_ready:
                    case Constants.Status_stopped:
                        trace("Counters.startMeasure(): Операция выполнена успешно!");
                        return true;
                    default:
                        trace("Counters.startMeasure(): ОШИБКА! НЕИЗВЕСТНОЕ СОСТОЯНИЕ СЧЁТЧИКОВ: " + state);
                        return false;
                }
            }
            /// <summary>
            /// Останавливает счётчики и RTC.
            ///<para>Возвращает:</para>
            ///<para>true - операция выполнена успешно. Счётчики принудительно остановлены (команда .recieveResults() будет проигнорирована)</para>
            ///<para>false - операция была отменена (счётчики не считают)</para>
            /// </summary>
            public bool stopMeasure()
            {
                //ФУНКЦИЯ: Останавливает счётчик. Возвращает true, если операция удалась. false - не удалась (счётчик не считает)
                trace_attached(Environment.NewLine);
                trace("Counters.stopMeasure()");
                byte state = transmit(Command.RTC.stopMeasure)[0];
                switch(state)
                {
                    case Constants.Status_busy:
                        trace("Counters.stopMeasure(): Операция выполнена успешно!");
                        return true;
                    case Constants.Status_ready:
                        trace("Counters.stopMeasure(): Операция отменена! Счётчики не считают!");
                        return false;
                    case Constants.Status_stopped:
                        trace("Counters.stopMeasure(): Операция отменена! Счётчики уже остановлены!");
                        return false;
                    default:
                        trace("Counters.stopMeasure(): ОШИБКА! НЕИЗВЕСТНОЕ СОСТОЯНИЕ СЧЁТЧИКОВ: " + state);
                        return false;
                }
            }
            /// <summary>
            /// Запрашивает у МК состояние счётчиков, если счётчики сосчитали (не считают сейчас и не были принудительно остановлены), то запрашивает результаты измерения и сохраняет их в СОХ.Result и СОХ.overflows. 
            /// <para>Возвращает состояние МК:</para>
            /// <para>"Ready" - счётчик готов к работе (команда .stopMeasure() будет проигнорирована)</para>
            /// <para>"Busy" - счётчик считает (команды .setMeasureTime(MILLISECONDS) и .startMeasure() будут проигнорированы, а команда .recieveResults() отменена)</para>
            /// <para>"Stopped" - счётчик был принудительно остановлен командой .stopMeasure(). (команда .recieveResults() будет отменена)</para>
            /// </summary>
            public string receiveResults()
            {
                //ФУНКЦИЯ: Запрашиваем результат счёта у МК и сохраняет по счётчикам. Возвращает состояние счётчика.
                //ПОЯСНЕНИЯ: <key><response_command><RTC_Status><COA_ovf><COA_Measurement_4bytes><COB_ovf><COB_Measurement_4bytes><COC_ovf><COC_Measurement_2bytes><checkSum><lock>
                trace_attached(Environment.NewLine);
                trace("Counters.receiveResults()");
                byte[] rDATA = transmit(Command.RTC.getResult);
                byte Status = rDATA[0];
                switch (Status)
                {
                    case Constants.Status_ready:
                        //Счётчик готов. Сохраняем результаты.
                        try
                        {
                            COA.overflows = rDATA[1];
                        }
                        catch (Exception)
                        {
                            trace("Counters.receiveResults(): Ошибка! Получено данных меньше, чем ожидалось!");
                            return "Ошибка!";
                        }
                        COA.Result = Convert.ToUInt32(rDATA[2] * 16777216 + rDATA[3] * 65536 + rDATA[4] * 256 + rDATA[5]);
                        COB.overflows = rDATA[6];
                        COB.Result = Convert.ToUInt32(rDATA[7] * 16777216 + rDATA[8] * 65536 + rDATA[9] * 256 + rDATA[10]);
                        COC.overflows = rDATA[11];
                        COC.Result = Convert.ToUInt32(rDATA[12] * 256 + rDATA[13]);
                        trace("Counters.receiveResults(): Статус счётчиков: Ready");
                        trace("Counters.receiveResults(): Операция выполнена успешно!");
                        return "Ready";
                    case Constants.Status_stopped:
                        trace("Counters.receiveResults(): Статус счётчиков: Stopped");
                        trace("Counters.receiveResults(): Операция отменена!");
                        return "Stopped";
                    case Constants.Status_busy:
                        trace("Counters.receiveResults(): Статус счётчиков: Busy");
                        trace("Counters.receiveResults(): Операция отменена!");
                        return "Busy";
                    default:
                        trace("ОШИБКА! Неизвестное состояние счётчиков: " + Status + " !!!");
                        return "ОШИБКА!";
                }
            }
            /// <summary>
            /// Возвращает состояние МК:
            /// <para>"Ready" - счётчик готов к работе (команда .stopMeasure() будет проигнорирована)</para>
            /// <para>"Busy" - счётчик считает (команды .setMeasureTime(MILLISECONDS) и .startMeasure() будут проигнорированы)</para>
            /// <para>"Stopped" - счётчик был принудительно остановлен командой .stopMeasure(). (команда .recieveResults(), будет проигнорирована)</para>
            /// </summary>
            public string getStatus()
            {
                //ФУНКЦИЯ: Возвращает статус счётчиков
                trace_attached(Environment.NewLine);
                trace("Counters.getStatus()");
                string answer = "";
                byte Status = transmit(Command.RTC.getStatus)[0];
                switch (Status)
                {
                    case Constants.Status_busy:
                        answer = "Busy";
                        break;
                    case Constants.Status_ready:
                        answer = "Ready";
                        break;
                    case Constants.Status_stopped:
                        answer = "Stopped";
                        break;
                    default:
                        trace("ОШИБКА СОСТОЯНИЯ СЧЁТЧИКА! Состояние:" + Status);
                        return "ОШИБКА!";
                }

                trace("Counters.getStatus(): Статус счётчиков: " + answer);
                trace("Counters.getStatus(): Операция выполнена успешно!");
                return answer;
            }
        }
        public class SPI_DEVICE_DAC_CHANNEL
        {
            //КЛАСС:Отдельнй канал отдельного DAC'а. 
            //Сам по себе не используется, используются только наследники
            protected byte DAC_channel; //Канал DAC'а
            protected byte DAC_command; //Команда обращения к конкретному DAC'у
            protected bool DAC_setVoltage(byte command, byte CHANNEL, ushort VOLTAGE)
            {
                //ФУНКЦИЯ: Задаёт на конкретный канал конкретного DAC'а конкретное напряжение
                if (VOLTAGE >= 0 && VOLTAGE <= 4095)
                {
                    trace_attached(Environment.NewLine);
                    trace("DAC_CHANNEL.setVoltage(" + command + ", " + CHANNEL + ", " + VOLTAGE + ")");
                    byte[] bytes = BitConverter.GetBytes(VOLTAGE);
                    byte[] data = { Convert.ToByte((CHANNEL - 1) * 16 + bytes[1]), bytes[0] };
                    if (transmit(command, data)[0] == DAC_command)
                    {
                        trace("DAC_CHANNEL.setVoltage(" + command + ", " + CHANNEL + ", " + VOLTAGE + "): Операция выполнена успешно!");
                        return true;
                    }
                    trace("DAC_CHANNEL.setVoltage(" + command + ", " + CHANNEL + ", " + VOLTAGE + "): ОШИБКА ОТКЛИКА!");
                }
                trace("DAC_CHANNEL.setVoltage(" + command + ", " + CHANNEL + ", " + VOLTAGE + "):ОШИБКА! Операция отменена! Значение VOLTAGE ожидалось от 0 до 4095!");
                return false;
            }
            //Видимые функции
            /// <summary>
            /// Задаёт напряжение на DAC
            /// <para>VOLTAGE - напряжение от 0 до 4095</para>
            /// <para>Возвращает:</para>
            /// <para>true - операция выполнена успешно</para>
            /// <para>false - операция отменена (значение VOLTAGE находтся не в диапазоне от 0 до 4095 или ошибка отклика)</para>
            /// </summary>
            /// <param name="VOLTAGE">Напряжение от 0 до 4095</param>
            /// <returns></returns>
            public bool setVoltage(ushort VOLTAGE)
            {
                return DAC_setVoltage(DAC_command, DAC_channel, VOLTAGE);
            }
                /// <summary>
            /// Задаёт напряжение на DAC
            /// <para>VOLTAGE - напряжение от 0 до 4095</para>
            /// <para>Возвращает:</para>
            /// <para>true - операция выполнена успешно</para>
            /// <para>false - операция отменена (значение VOLTAGE находтся не в диапазоне от 0 до 4095 или ошибка отклика)</para>
            /// </summary>
            /// <param name="VOLTAGE">Напряжение от 0 до 4095</param>
            /// <returns></returns>
                public bool setVoltage(string VOLTAGE)
            {
                return setVoltage(Convert.ToUInt16(VOLTAGE));
            }
                /// <summary>
                /// Задаёт напряжение на DAC
                /// <para>VOLTAGE - напряжение от 0 до 4095</para>
                /// <para>Возвращает:</para>
                /// <para>true - операция выполнена успешно</para>
                /// <para>false - операция отменена (значение VOLTAGE находтся не в диапазоне от 0 до 4095 или ошибка отклика)</para>
                /// </summary>
                /// <param name="VOLTAGE">Напряжение от 0 до 4095</param>
                /// <returns></returns>
                public bool setVoltage(int VOLTAGE)
            {
                return setVoltage(Convert.ToUInt16(VOLTAGE));
            }
        }
        public class SPI_DEVICE_CHANNEL : SPI_DEVICE_DAC_CHANNEL
        {
            //КЛАСС: Два канала: DAC и ADC. Используется. Кроме Натекателя, Нагревателя и Конденсатора.
            protected byte ADC_channel;     //Канал ADC
            protected byte ADC_command;     //Команда для канала ADC
            public SPI_DEVICE_CHANNEL(byte DAC_CHANNEL, byte DAC_COMMAND, byte ADC_CHANNEL, byte ADC_COMMAND)
            {
                //КОНСТРУКТОР: Запоминаем номера команд и каналов
                DAC_channel = DAC_CHANNEL;
                ADC_channel = ADC_CHANNEL;
                DAC_command = DAC_COMMAND;
                ADC_command = ADC_COMMAND;
            }
            public SPI_DEVICE_CHANNEL()
            {
                //КОНСТРУКТОР: Пустышка для наследника
            }
            //ADC
            const byte Hbyte = 127;
            const byte Lbyte_DoubleRange = 16;
            const byte Lbyte_NormalRange = 48;
            const byte ChannelStep = 4;
            bool DoubleRange = true;
            ushort ADC_getVoltage(byte command, byte CHANNEL)
            {
                trace_attached(Environment.NewLine);
                trace("ADC_CHANNEL.getVoltage(" + command + ", " + CHANNEL + ")");
                trace("ADC_CHANNEL.getVoltage(" + command + ", " + CHANNEL + "): DoubleRange = " + DoubleRange);
                byte Lbyte = 0;
                if (DoubleRange) { Lbyte = Lbyte_DoubleRange; } else { Lbyte = Lbyte_NormalRange; }
                byte[] data = { Convert.ToByte(Hbyte + ChannelStep * CHANNEL), Lbyte };
                byte[] rDATA = transmit(command, data);
                ushort voltage = 0;
                byte adress = 1;
                adress += Convert.ToByte(rDATA[0] >> 4);
                try
                {
                    voltage = Convert.ToUInt16((Convert.ToUInt16(rDATA[0] & 0xf) << 8) + rDATA[1]);
                }
                catch (Exception)
                {
                    trace("ADC_CHANNEL.getVoltage(" + command + ", " + CHANNEL + "):Ошибка! Получено данных меньше, чем ожидалось!");
                    return 0;
                }
                trace("ADC_CHANNEL.getVoltage(" + command + ", " + CHANNEL + "): Ответный адрес канала: " + adress);
                trace("ADC_CHANNEL.getVoltage(" + command + ", " + CHANNEL + "): Напряжение: " + voltage);
                return voltage;
            }
            //Видимые функции
            /// <summary>
            /// Возвращает напряжение на канале ADC.
            /// <para>Если DoubleRange = true, то диапазон напряжения увеличен в двое.</para>
            /// <para>Если DoubleRange = false, то диапазон напряжения соответствует выставляемому DAC'ом</para>
            /// <para>DoubleRange задаётся командой .enableDoubleRange(bool)</para>
            /// </summary>
            public ushort getVoltage()
            {
                return ADC_getVoltage(ADC_command, ADC_channel);
            }
            /// <summary>
            /// .enableDoubleRange(true) - увеличивает диапазон напряжения считываемого ADC в двое.
            /// <para>.enableDoubleRange(false) - диапазон напряжения соответствует выставляемому DAC'ом</para>
            /// <para>Задавать до .getVoltage()</para>
            /// </summary>
            public void enableDoubleRange(bool enable)
            {
                DoubleRange = enable;
            }
        }
        public class SPI_DEVICE_CHANNEL_withReset : SPI_DEVICE_CHANNEL
        {
            //КЛАСС: Класс для каналов с reset()'ом для Натекателя и Нагревателя
            const byte Reset_Hbyte = 255;
            const byte Reset_Lbyte = 255;

            public SPI_DEVICE_CHANNEL_withReset(byte DAC_CHANNEL, byte DAC_COMMAND, byte ADC_CHANNEL, byte ADC_COMMAND)
            {
                DAC_channel = DAC_CHANNEL;
                ADC_channel = ADC_CHANNEL;
                DAC_command = DAC_COMMAND;
                ADC_command = ADC_COMMAND;
            }
            /// <summary>
            /// Сбрасывает все настройки DAC'а и его напряжения 
            /// <para>ПРИМЕЧАНИЕ: у Натекателя и Нагревателя общий DAC, иными словами общий .reset()</para>
            /// </summary>
            public bool reset()
            {
                trace_attached(Environment.NewLine);
                trace("DAC_CHANNEL.reset(INLET)");
                byte[] data = { Reset_Hbyte, Reset_Lbyte };
                if (transmit(DAC_command, data)[0] == DAC_command)
                {
                    trace("DAC_CHANNEL.reset(INLET): Операция выполнена успешно! Напряжения DAC'а Натекателя и Нагревателя сброшены.");
                    return true;
                }
                trace("DAC_CHANNEL.reset(INLET): ОШИБКА ОТКЛИКА! Напряжения DAC'а Натекателя и Нагревателя вероятно не сброшены!");
                return false;
            }
        }
        public class SPI_CONDENSATOR
        {
            //КЛАСС: Каналы для конденсатора (+\-) с reset()'ом
            //DAC AD5643R
            //byte[] Reset_bytes = { 40, 0, 1 }; //старший, средний, младший
            byte[] ConfInnerRef_bytes = {56,0,1}; //старший, средний, младший
            protected bool Ref_is_inner = false;
            protected byte DAC_channel = 24; //Канал нулевой, но посылка (24+КАНАЛ)
            protected bool DAC_setVoltage(ushort VOLTAGE)
            {
                //ФУНКЦИЯ: Задаёт на конкретный канал конкретного DAC'а конкретное напряжение
                //ПОЯСНЕНИЯ:
                //      Пакет содержит три байта:
                //      [x|x|C2|C1|C0|A2|A1|A0] [D13|D12|D11|D10|D9|D8|D7|D6] [D5|D4|D3|D2|D1|D0|x|x]
                //
                //      Перед началом работы ставится внутренний референс (зашитов в МК при установке сигнала iHVE):
                //          [x|x|1|1|1|0|0|0] [0|0|0|0|0|0|0|0] [0|0|0|0|0|0|x| 1/0 ] - вкл/выкл внутренний референс
                //          или в байтах: [56][0][1/0]
                //
                //      Адресация: Каналов два: А - 000, и В - 001;
                //      Напряжение 14-бит -> 0...16383
                //
                //      Установка напряжения на канал:
                //          [x|x|0|1|1|0|0| 0/1 ] [V|V|V|V|V|V|V|V] [V|V|V|V|V|V|x|x]
                //                         канал
                //          или в байтах: [24/25][0..255][(0..63)<<2]
                //                         канал    напряжение
                if (VOLTAGE >= 0 && VOLTAGE <= 16383)
                {
                    trace(".Condensator.setVoltage(" + Command.SPI.Condensator.setVoltage + "," + DAC_channel + "," + VOLTAGE + "): AD5643R;");
                    //посылаем напряжение
                    int voltage = VOLTAGE;
                    voltage = voltage << 2;
                    byte[] bytes = BitConverter.GetBytes(voltage);
                    byte Hbyte = Convert.ToByte(DAC_channel);
                    byte Mbyte = bytes[1];
                    byte Lbyte = bytes[0];
                    byte[] data = new byte[] { Hbyte, Mbyte, Lbyte };
                    if (transmit(Command.SPI.Condensator.setVoltage, data)[0] == Command.SPI.Condensator.setVoltage)
                    {
                        trace(".Condensator.setVoltage(" + Command.SPI.Condensator.setVoltage + "," + DAC_channel + "," + VOLTAGE + "): Операция выполнена успешно!");
                        return true;
                    }
                    trace(".Condensator.setVoltage(" + Command.SPI.Condensator.setVoltage + "," + DAC_channel + "," + VOLTAGE + "): ОШИБКА ОТКЛИКА!");
                    return false;
                }
                trace(".Condensator.setVoltage(" + Command.SPI.Condensator.setVoltage + "," + DAC_channel + "," + VOLTAGE + "):ОШИБКА! Операция отменена! Значение VOLTAGE ожидалось от 0 до 16383!");
                return false;
            }
            //ADC
            byte ADC_positive_channel = 0;
            byte ADC_negative_channel = 1;
            const byte ADC_Hbyte = 131;
            const byte ADC_Lbyte_DoubleRange = 16;
            const byte ADC_Lbyte_NormalRange = 48;
            const byte ChannelStep = 4;
            bool DoubleRange = true;
            ushort ADC_getVoltage(byte command, byte CHANNEL)
            {
                trace_attached(Environment.NewLine);
                trace(".Condensator.getVoltage(" + command + ", " + CHANNEL + ")");
                trace(".Condensator.getVoltage(" + command + ", " + CHANNEL + "): DoubleRange = " + DoubleRange);
                byte Lbyte = 0;
                if (DoubleRange) { Lbyte = ADC_Lbyte_DoubleRange; } else { Lbyte = ADC_Lbyte_NormalRange; }
                byte[] data = { Convert.ToByte(ADC_Hbyte + ChannelStep * CHANNEL), Lbyte };
                byte[] rDATA = transmit(command, data);
                ushort voltage = 0;
                byte adress = 0;
                adress += Convert.ToByte(rDATA[0] >> 4);
                try
                {
                    voltage = Convert.ToUInt16((Convert.ToUInt16(rDATA[0] & 0xf) << 8) + rDATA[1]);
                }
                catch (Exception)
                {
                    trace(".Condensator.getVoltage(" + command + ", " + CHANNEL + "):Ошибка! Получено данных меньше, чем ожидалось!");
                    return 0;
                }
                trace(".Condensator.getVoltage(" + command + ", " + CHANNEL + "): Ответный адрес канала: " + adress);
                trace(".Condensator.getVoltage(" + command + ", " + CHANNEL + "): Напряжение: " + voltage);
                return voltage;
            }
            //Видимые функции
            /// <summary>
            /// Возвращает напряжение положительной обкладки конденсатора
            /// <para>Если DoubleRange = true, то диапазон напряжения увеличен в двое.</para>
            /// <para>Если DoubleRange = false, то диапазон напряжения соответствует выставляемому DAC'ом</para>
            /// <para>DoubleRange задаётся командой .enableDoubleRange(bool)</para>
            /// </summary>
            public ushort getPositiveVoltage()
            {
                return ADC_getVoltage(Command.SPI.Condensator.getPositiveVoltage, ADC_positive_channel);
            }
            /// <summary>
            /// Возвращает напряжение отрицательной обкладки конденсатора
            /// <para>Если DoubleRange = true, то диапазон напряжения увеличен в двое.</para>
            /// <para>Если DoubleRange = false, то диапазон напряжения соответствует выставляемому DAC'ом</para>
            /// <para>DoubleRange задаётся командой .enableDoubleRange(bool)</para>
            /// </summary>
            public ushort getNegativeVoltage()
            {
                return ADC_getVoltage(Command.SPI.Condensator.getNegativeVoltage, ADC_negative_channel);
            }
            /// <summary>
            /// .enableDoubleRange(true) - увеличивает диапазон напряжения считываемого ADC в двое.
            /// <para>.enableDoubleRange(false) - диапазон напряжения соответствует выставляемому DAC'ом</para>
            /// <para>Задавать до .getPositiveVoltage() и .getNegativeVoltage()</para>
            /// </summary>
            public void enableDoubleRange(bool enable)
            {
                DoubleRange = enable;
            }
            //Видимые функции
            /// <summary>
            /// Задаёт напряжение на DAC
            /// <para>VOLTAGE - напряжение от 0 до 4095</para>
            /// <para>Возвращает:</para>
            /// <para>true - операция выполнена успешно</para>
            /// <para>false - операция отменена (значение VOLTAGE находтся не в диапазоне от 0 до 4095 или ошибка отклика)</para>
            /// </summary>
            /// <param name="VOLTAGE">Напряжение от 0 до 4095</param>
            /// <returns></returns>
            public bool setVoltage(ushort VOLTAGE)
            {
                return DAC_setVoltage(VOLTAGE);
            }
            /// <summary>
            /// Задаёт напряжение на DAC
            /// <para>VOLTAGE - напряжение от 0 до 4095</para>
            /// <para>Возвращает:</para>
            /// <para>true - операция выполнена успешно</para>
            /// <para>false - операция отменена (значение VOLTAGE находтся не в диапазоне от 0 до 4095 или ошибка отклика)</para>
            /// </summary>
            /// <param name="VOLTAGE">Напряжение от 0 до 4095</param>
            /// <returns></returns>
            public bool setVoltage(string VOLTAGE)
            {
                return setVoltage(Convert.ToUInt16(VOLTAGE));
            }
            /// <summary>
            /// Задаёт напряжение на DAC
            /// <para>VOLTAGE - напряжение от 0 до 4095</para>
            /// <para>Возвращает:</para>
            /// <para>true - операция выполнена успешно</para>
            /// <para>false - операция отменена (значение VOLTAGE находтся не в диапазоне от 0 до 4095 или ошибка отклика)</para>
            /// </summary>
            /// <param name="VOLTAGE">Напряжение от 0 до 4095</param>
            /// <returns></returns>
            public bool setVoltage(int VOLTAGE)
            {
                return setVoltage(Convert.ToUInt16(VOLTAGE));
            }
        }
        public class SPI_IonSOURCE
        {
            //КЛАСС: Ионный источник - используется каналы А,B,C,D
            //DAC AD5328BR
            const byte Reset_Hbyte = 255;
            const byte Reset_Lbyte = 255;
            const byte EmissionCurrent_channel = 1;
            const byte Ionization_channel =2;
            const byte F1_channel = 3;
            const byte F2_channel = 4;
            /// <summary>
            /// Сбрасывает все настройки DAC'а и его напряжения 
            /// </summary>
            public bool reset()
            {
                trace_attached(Environment.NewLine);
                trace("DAC_CHANNEL.reset(IonSOURCE)");
                byte[] data = { Reset_Hbyte, Reset_Lbyte };
                if (transmit(Command.SPI.IonSource.EmissionCurrent.setVoltage, data)[0] == Command.SPI.IonSource.EmissionCurrent.setVoltage)
                {
                    trace("DAC_CHANNEL.reset(IonSOURCE): Операция выполнена успешно! Напряжения DAC'a Ионного Источника сброшены");
                    return true;
                }
                trace("DAC_CHANNEL.reset(IonSOURCE): ОШИБКА ОТКЛИКА! Напряжения DAC'а Ионного Источника вероятно не сброшены!");
                return false;
            }
            //Каналы
            /// <summary>
            /// Напряжение тока эмиссии
            /// </summary>
            public SPI_DEVICE_CHANNEL EmissionCurrent = new SPI_DEVICE_CHANNEL(EmissionCurrent_channel, Command.SPI.IonSource.EmissionCurrent.setVoltage, EmissionCurrent_channel,Command.SPI.IonSource.EmissionCurrent.getVoltage);
            /// <summary>
            /// Напряжение ионизации
            /// </summary>
            public SPI_DEVICE_CHANNEL Ionization = new SPI_DEVICE_CHANNEL(Ionization_channel, Command.SPI.IonSource.Ionization.setVoltage, Ionization_channel, Command.SPI.IonSource.Ionization.getVoltage);
            /// <summary>
            /// Фокусирующее напряжение 1 
            /// </summary>
            public SPI_DEVICE_CHANNEL F1 = new SPI_DEVICE_CHANNEL(F1_channel, Command.SPI.IonSource.F1.setVoltage, F1_channel, Command.SPI.IonSource.F1.getVoltage);
            /// <summary>
            /// Фокусирующее напряжение 2
            /// </summary>
            public SPI_DEVICE_CHANNEL F2 = new SPI_DEVICE_CHANNEL(F2_channel, Command.SPI.IonSource.F2.setVoltage, F2_channel, Command.SPI.IonSource.F2.getVoltage);
            /// <summary>
            /// .enableDoubleRange(true) - увеличивает диапазон напряжения в двое всем каналам ADC Ионного Источника.
            /// <para>.enableDoubleRange(false) - диапазон напряжения соответствует выставляемому DAC'ом всем каналам ADC Ионного Источника</para>
            /// <para>Задавать до .getVoltage()</para>
            /// </summary>
            public void enableDoubleRange(bool enable)
            {
                EmissionCurrent.enableDoubleRange(enable);
                Ionization.enableDoubleRange(enable);
                F1.enableDoubleRange(enable);
                F2.enableDoubleRange(enable);
            }
        }
        public class SPI_DETECTOR
        {
            //КЛАСС: Ионный источник - используется каналы А,B,C,D
            //DAC AD5328BR
            const byte Reset_Hbyte = 255;
            const byte Reset_Lbyte = 255;
            const byte DV1_channel = 1;
            const byte DV2_channel = 2;
            const byte DV3_channel = 3;
            //Каналы
            public SPI_DEVICE_CHANNEL DV1 = new SPI_DEVICE_CHANNEL(DV1_channel, Command.SPI.Detector.DV1.setVoltage, DV1_channel, Command.SPI.Detector.DV1.getVoltage);
            public SPI_DEVICE_CHANNEL DV2 = new SPI_DEVICE_CHANNEL(DV2_channel, Command.SPI.Detector.DV2.setVoltage, DV2_channel, Command.SPI.Detector.DV2.getVoltage);
            public SPI_DEVICE_CHANNEL DV3 = new SPI_DEVICE_CHANNEL(DV3_channel, Command.SPI.Detector.DV3.setVoltage, DV3_channel, Command.SPI.Detector.DV3.getVoltage);
            /// <summary>
            /// Сбрасывает все настройки DAC'а и его напряжения 
            /// </summary>
            public bool reset()
            {
                trace_attached(Environment.NewLine);
                trace("DAC_CHANNEL.reset(DETECTOR)");
                byte[] data = { Reset_Hbyte, Reset_Lbyte };
                if (transmit(Command.SPI.Detector.DV1.setVoltage, data)[0] == Command.SPI.Detector.DV1.setVoltage)
                {
                    trace("DAC_CHANNEL.reset(DETECTOR): Операция выполнена успешно! Напряжения DAC'a Детектора сброшены");
                    return true;
                }
                trace("DAC_CHANNEL.reset(DETECTOR): ОШИБКА ОТКЛИКА! Напряжения DAC'а Детектора вероятно не сброшены!");
                return false;
            }
            /// <summary>
            /// .enableDoubleRange(true) - увеличивает диапазон напряжения в двое всем каналам ADC Детектора.
            /// <para>.enableDoubleRange(false) - диапазон напряжения соответствует выставляемому DAC'ом всем каналам ADC Детектора</para>
            /// <para>Задавать до .getVoltage()</para>
            /// </summary>
            public void enableDoubleRange(bool enable)
            {
                DV1.enableDoubleRange(enable);
                DV2.enableDoubleRange(enable);
                DV3.enableDoubleRange(enable);
            }
        }
        public class SPI_SCANER
        {
            //КЛАСС: Сканер (Сканирющее напряжение).
            //DAC AD5643BR
            const byte DAC_ParentScan_Channel = 0;
            const byte DAC_Scan_Channel = 1;
            const byte ADC_ParentScan_Channel = 3;
            const byte ADC_Scan_Channel = 2;
            //Класс для двух каналов сканирующего напряжения
            public class SPI_DEVICE_CHANNEL_withAD5643R
            {
                //КЛАСС: Для канала с ЦАПом AD5643R и номральным АЦП
                //DAC AD5643R
                //byte[] Reset_bytes = { 40, 0, 1 }; //старший, средний, младший
                byte[] ConfInnerRef_bytes = { 56, 0, 1 }; //старший, средний, младший
                byte ADC_command;
                byte ADC_channel;
                byte DAC_command;
                byte DAC_channel = 24; //Канал нулевой, но посылка (24+КАНАЛ)
                //КОНСТРУКТОР
                public SPI_DEVICE_CHANNEL_withAD5643R(byte DAC_Channel, byte DAC_Command, byte ADC_Channel, byte ADC_Command)
                {
                    DAC_command = DAC_Command;
                    DAC_channel += DAC_Channel;
                    ADC_command = ADC_Command;
                    ADC_channel = ADC_Channel;
                }
                bool DAC_setVoltage(ushort VOLTAGE)
                {
                    //ФУНКЦИЯ: Задаёт на конкретный канал конкретного DAC'а конкретное напряжение
                    if (VOLTAGE >= 0 && VOLTAGE <= 16383)
                    {
                        trace(".Condensator.setVoltage(" + DAC_command + "," + DAC_channel + "," + VOLTAGE + "): AD5643R;");
                        //посылаем напряжение
                        int voltage = VOLTAGE;
                        voltage = voltage << 2;
                        byte[] bytes = BitConverter.GetBytes(voltage);
                        byte Hbyte = Convert.ToByte(DAC_channel);
                        byte Mbyte = bytes[1];
                        byte Lbyte = bytes[0];
                        byte[] data = new byte[] { Hbyte, Mbyte, Lbyte };
                        if (transmit(DAC_command, data)[0] == DAC_command)
                        {
                            trace(".Condensator.setVoltage(" + DAC_command + "," + DAC_channel + "," + VOLTAGE + "): Операция выполнена успешно!");
                            return true;
                        }
                        trace(".Condensator.setVoltage(" + DAC_command + "," + DAC_channel + "," + VOLTAGE + "): ОШИБКА ОТКЛИКА!");
                        return false;
                    }
                    trace(".Condensator.setVoltage(" + DAC_command + "," + DAC_channel + "," + VOLTAGE + "):ОШИБКА! Операция отменена! Значение VOLTAGE ожидалось от 0 до 16383!");
                    return false;
                }
                //ADC
                const byte ADC_Hbyte = 131;
                const byte ADC_Lbyte_DoubleRange = 16;
                const byte ADC_Lbyte_NormalRange = 48;
                const byte ChannelStep = 4;
                bool DoubleRange = true;
                ushort ADC_getVoltage(byte command, byte CHANNEL)
                {
                    trace_attached(Environment.NewLine);
                    trace(".Condensator.getVoltage(" + command + ", " + CHANNEL + ")");
                    trace(".Condensator.getVoltage(" + command + ", " + CHANNEL + "): DoubleRange = " + DoubleRange);
                    byte Lbyte = 0;
                    if (DoubleRange) { Lbyte = ADC_Lbyte_DoubleRange; } else { Lbyte = ADC_Lbyte_NormalRange; }
                    byte[] data = { Convert.ToByte(ADC_Hbyte + ChannelStep * CHANNEL), Lbyte };
                    byte[] rDATA = transmit(command, data);
                    ushort voltage = 0;
                    byte adress = 0;
                    adress += Convert.ToByte(rDATA[0] >> 4);
                    try
                    {
                        voltage = Convert.ToUInt16((Convert.ToUInt16(rDATA[0] & 0xf) << 8) + rDATA[1]);
                    }
                    catch (Exception)
                    {
                        trace(".Condensator.getVoltage(" + command + ", " + CHANNEL + "):Ошибка! Получено данных меньше, чем ожидалось!");
                        return 0;
                    }
                    trace(".Condensator.getVoltage(" + command + ", " + CHANNEL + "): Ответный адрес канала: " + adress);
                    trace(".Condensator.getVoltage(" + command + ", " + CHANNEL + "): Напряжение: " + voltage);
                    return voltage;
                }
                //Видимые функции
                /// <summary>
                /// Возвращает напряжение положительной обкладки конденсатора
                /// <para>Если DoubleRange = true, то диапазон напряжения увеличен в двое.</para>
                /// <para>Если DoubleRange = false, то диапазон напряжения соответствует выставляемому DAC'ом</para>
                /// <para>DoubleRange задаётся командой .enableDoubleRange(bool)</para>
                /// </summary>
                public ushort getVoltage()
                {
                    return ADC_getVoltage(ADC_command, ADC_channel);
                }
                /// <summary>
                /// .enableDoubleRange(true) - увеличивает диапазон напряжения считываемого ADC в двое.
                /// <para>.enableDoubleRange(false) - диапазон напряжения соответствует выставляемому DAC'ом</para>
                /// <para>Задавать до .getPositiveVoltage() и .getNegativeVoltage()</para>
                /// </summary>
                public void enableDoubleRange(bool enable)
                {
                    DoubleRange = enable;
                }
                //Видимые функции
                /// <summary>
                /// Задаёт напряжение на DAC
                /// <para>VOLTAGE - напряжение от 0 до 4095</para>
                /// <para>Возвращает:</para>
                /// <para>true - операция выполнена успешно</para>
                /// <para>false - операция отменена (значение VOLTAGE находтся не в диапазоне от 0 до 4095 или ошибка отклика)</para>
                /// </summary>
                /// <param name="VOLTAGE">Напряжение от 0 до 4095</param>
                /// <returns></returns>
                public bool setVoltage(ushort VOLTAGE)
                {
                    return DAC_setVoltage(VOLTAGE);
                }
                /// <summary>
                /// Задаёт напряжение на DAC
                /// <para>VOLTAGE - напряжение от 0 до 4095</para>
                /// <para>Возвращает:</para>
                /// <para>true - операция выполнена успешно</para>
                /// <para>false - операция отменена (значение VOLTAGE находтся не в диапазоне от 0 до 4095 или ошибка отклика)</para>
                /// </summary>
                /// <param name="VOLTAGE">Напряжение от 0 до 4095</param>
                /// <returns></returns>
                public bool setVoltage(string VOLTAGE)
                {
                    return setVoltage(Convert.ToUInt16(VOLTAGE));
                }
                /// <summary>
                /// Задаёт напряжение на DAC
                /// <para>VOLTAGE - напряжение от 0 до 4095</para>
                /// <para>Возвращает:</para>
                /// <para>true - операция выполнена успешно</para>
                /// <para>false - операция отменена (значение VOLTAGE находтся не в диапазоне от 0 до 4095 или ошибка отклика)</para>
                /// </summary>
                /// <param name="VOLTAGE">Напряжение от 0 до 4095</param>
                /// <returns></returns>
                public bool setVoltage(int VOLTAGE)
                {
                    return setVoltage(Convert.ToUInt16(VOLTAGE));
                }
            }
            //Каналы
            /// <summary>
            /// "Родительское" сканирующее напряжение
            /// </summary>
            public SPI_DEVICE_CHANNEL_withAD5643R ParentScan = new SPI_DEVICE_CHANNEL_withAD5643R(DAC_ParentScan_Channel, Command.SPI.Scaner.ParentScan.setVoltage, ADC_ParentScan_Channel, Command.SPI.Scaner.ParentScan.getVoltage);
            /// <summary>
            /// Сканирующее напряжение
            /// </summary>
            public SPI_DEVICE_CHANNEL_withAD5643R Scan = new SPI_DEVICE_CHANNEL_withAD5643R(DAC_Scan_Channel, Command.SPI.Scaner.Scan.setVoltage, ADC_Scan_Channel, Command.SPI.Scaner.Scan.getVoltage);
            /// <summary>
            /// .enableDoubleRange(true) - увеличивает диапазон напряжения в двое всем каналам ADC Сканера.
            /// <para>.enableDoubleRange(false) - диапазон напряжения соответствует выставляемому DAC'ом всем каналам ADC Сканера</para>
            /// <para>Задавать до .getVoltage()</para>
            /// </summary>
            public void enableDoubleRange(bool enable)
            {
                ParentScan.enableDoubleRange(enable);
                Scan.enableDoubleRange(enable);
            }
        }
        public class CHIP
        {
            //КЛАСС: Микросхема, сам микроконтроллер.
            /// <summary>
            /// Задаёт СОМ-порт для связи ПК с МК
            /// </summary>
            public void setUSART(SerialPort COM_PORT)
            {
                //ФУНКЦИЯ: Задаём порт, припомози которого будем общаться с МК
                trace_attached(Environment.NewLine);
                trace("Chip.setUSART(" + COM_PORT.PortName + ")");
                USART = COM_PORT;
                USART_defined = true;
                //trace("Chip.setUSART(" + COM_PORT.PortName + "): СОМ порт задан.");
            }
            /// <summary>
            /// Проверка синхронности команд ПК и МК (отладочное)
            /// </summary>
            public bool checkCommandStack()
            {
                //ФУНКЦИЯ: При общении ПК-МК, они считаю выполненные команды (byte). Это функция сверяет номер команды.
                trace_attached(Environment.NewLine);
                trace("Chip.checkCommandStack()");
                if (transmit(Command.TEST.checkCommandStack)[0] == CommandStack)
                {
                    trace("Chip.checkCommandStack(): Команды идут синхронно. (" + CommandStack + ")");
                    return true;
                }
                trace("Chip.checkCommandStack(): Команды идут НЕ синхронно! (" + CommandStack + ")");
                return false;
            }
            /// <summary>
            /// Возвращает код статуса МК (отладочное)
            /// </summary>
            public byte getStatus()
            {
                //ФУНКЦИЯ: Возвращает статус самого микроконтроллера
                trace_attached(Environment.NewLine);
                trace("Chip.getStatus()");
                byte answer = transmit(Command.Chip.getStatus)[0];
                trace("Chip.getStatus(): Статус системы: "+answer);
                return answer;
            }
            /// <summary>
            /// Возвращает версию прошивки МК
            /// </summary>
            public byte getVersion()
            {
                //ФУНКЦИЯ: Возвращает версию прошивки микроконтроллера
                trace_attached(Environment.NewLine);
                trace("Chip.getVersion()");
                byte answer = transmit(Command.Chip.getVersion)[0];
                trace("Chip.getVersion(): Версия прошивки МК: " + answer);
                return answer;
            }
            /// <summary>
            /// Возвращает дату создания прошивки МК 
            /// </summary>
            public string getBirthday()
            {
                //ФУНКЦИЯ: Возвращает дату создания прошивки микроконтроллера
                trace_attached(Environment.NewLine);
                trace("Chip.getBirthday()");
                UInt32 birthday = 0;
                string answer = "00000000";
                byte[] recDATA = transmit(Command.Chip.getBirthday);
                try
                {
                    birthday = Convert.ToUInt32(recDATA[3]) * 16777216 + Convert.ToUInt32(recDATA[2]) * 65536 + Convert.ToUInt32(recDATA[1]) * 256 + Convert.ToUInt32(recDATA[0]);
                }
                catch (Exception)
                {
                    trace("Chip.getBirthday(): Ошибка! Получено меньше данных чем ожидалось!");
                    return answer;
                }
                answer = birthday.ToString();
                answer = answer[6] + "" + answer[7] + " " + answer[4] + answer[5] + " " + answer[0] + answer[1] + answer[2] + answer[3];
                trace("Chip.getBirthday(): Дата создания прошивки МК: " + answer);
                return answer;
            }
            /// <summary>
            /// Возвращает частоту процессора МК (не вычисляется) 
            /// </summary>
            public string getCPUfrequency()
            {
                //ФУНКЦИЯ: Возвращает частоту процессора микроконтроллера (не вычисляется)
                trace_attached(Environment.NewLine);
                trace("Chip.getCPUfrequency()");
                UInt32 frequency = 0;
                byte[] recDATA = transmit(Command.Chip.getCPUfrequency);
                try
                {
                    frequency = Convert.ToUInt32(recDATA[3]) * 16777216 + Convert.ToUInt32(recDATA[2]) * 65536 + Convert.ToUInt32(recDATA[1]) * 256 + Convert.ToUInt32(recDATA[0]);
                }
                catch (Exception)
                {
                    trace("Chip.getBirthday(): Ошибка! Получено меньше данных чем ожидалось!");
                    return "Неизвестно";
                }
                trace("Chip.getCPUfrequency(): Частота процессора: "+frequency.ToString() + " Гц");
                return frequency.ToString() + " Гц";
            }
        }
        public class TEST
        {
            //КЛАСС: Класс для тестовых функций и отладки.
            /// <summary>
            /// Задаёт трейсер для вывода сообщений (отладочное)
            /// </summary>
            public void setTracer(RichTextBox TRACER)
            {
                //ФУНКЦИЯ: Задаём трэйсер для отладки
                trace_attached(Environment.NewLine);
                trace("Tester.setTracer(" + TRACER.Name + ")");
                tracer = TRACER;
                tracer_defined = true;
                //trace("Tester.setTracer(" + TRACER.Name + "): Трэйсер задан.");
            }
            /// <summary>
            /// Разрешает вывод сообщений в трейсер, если он задан (отладочное) 
            /// </summary>
            public void enableTracer(bool enable)
            {
                //ФУНКЦИЯ: Включает\выключает трэйсер
                if (enable)
                {
                    tracer_enabled = enable;
                    trace_attached(Environment.NewLine);
                    trace("Tester.enableTracer(" + enable + ")");
                    return;
                }
                trace_attached(Environment.NewLine);
                trace("Tester.enableTracer(" + enable + ")");
                tracer_enabled = enable;
            }
            /// <summary>
            /// Разрешает подробный вывод сообщений в трейсер, если он задан (отладочное) 
            /// </summary>
            public void enableTracerInTransmit(bool enable)
            {
                //ФУНКЦИЯ: Включает\выключает трэйсер
                if (enable)
                {
                    tracer_transmit_enabled = enable;
                    trace_attached(Environment.NewLine);
                    trace("Tester.enableTracerInTransmit(" + enable + ")");
                    return;
                }
                trace_attached(Environment.NewLine);
                trace("Tester.enableTracerInTransmit(" + enable + ")");
                tracer_transmit_enabled = enable;
            }
            /// <summary>
            /// Разрешает вывод сообщений трейсера в текстовый файл Log.txt, находящийся в папке с exe-файлом
            /// </summary>
            public void enableLog(bool enable)
            {
                //ФУНКЦИЯ: Включает\выключает ведение лога (сохранение отладочных сообщений в Log.txt)
                if (enable)
                {
                    tracer_log_enabled = enable;
                    trace_attached(Environment.NewLine);
                    trace("Tester.enableLog(" + enable + ")");
                    return;
                }
                trace_attached(Environment.NewLine);
                trace("Tester.enableLog(" + enable + ")");
                tracer_log_enabled = enable;
            }
            /// <summary>
            /// Посылает МК число 243. Получаем от него ругательства... (отладочное)
            /// </summary>
            public void sendSomething()
            {
                //ФУНКЦИЯ: Просто посылает число микроконтроллеру.
                trace_attached(Environment.NewLine);
                trace("Tester.sendSomething(243)");
                transmit(243);
            }
            /// <summary>
            /// Посылаем на МК BYTE, он высвечивает его на светодиодах (отладочное) 
            /// </summary>
            public void showMeByte(byte BYTE)
            {
                //ФУНКЦИЯ: МК высвечивает на светодиодах BYTE
                trace_attached(Environment.NewLine);
                trace("Tester.showMeByte(" + BYTE + ")");
                transmit(Command.TEST.showMeByte, BYTE);
            }
                public void showMeByte(string BYTE)
            {
                showMeByte(Convert.ToByte(BYTE));
            }
                public void showMeByte(uint BYTE)
            {
                showMeByte(Convert.ToByte(BYTE));
            }
        }
        public class TIC_PUMP
        {
            //КЛАСС: Микроконтроллер насоса
            /// <summary>
            /// Перендаём на МК данные DATA, которые он пересылает на TIC контроллер вакуумного насоса (В РАЗРАБОТКЕ...)
            /// </summary>
            byte[] transmit_toTIC(byte[] DATA)
            {
                //ФУНКЦИЯ: Посылает данные TIC контроллеру
                //ПОЯСНЕНИЯ: ПК->МК: <key><Command.retransmitToTIC><lengthOfMessage><MessageToTIC><CS><lock>
                //           МК->TIC: <MessageToTIC>
                List<byte> formedDATA = new List<byte>();
                formedDATA.Add(Command.TIC.sendToTIC);
                formedDATA.Add((byte)(DATA.Length + 2)); //+2 для смещения относительно команды и байта длины
                formedDATA.AddRange(DATA);
                return transmit(formedDATA.ToArray());
            }
            //Видимые функции
            /// <summary>
            /// Перендаём на МК данные (внутри функции), которые он пересылает на TIC контроллер вакуумного насоса (В РАЗРАБОТКЕ...)
            /// </summary>
            public void send()
            {
                //ФУНКЦИЯ: Посылает данные TIC'у
                byte[] msg = { 45, 76, 78, 34 };
                byte[] data = transmit_toTIC(msg);
                trace("Получено от TIC'а: (пока отражение)");
                foreach (byte b in data)
                {
                    trace("     " + b);
                }
            }
        }
        //--------------------------------------ОБЪЕКТЫ-------------------------------------------
        /// <summary>
        /// Счётчки импульсов
        /// </summary>
        public RTCounterAndCO Counters = new RTCounterAndCO();
        /// <summary>
        /// Натекатель (имеет общий reset() с нагревателем Heater)
        /// </summary>
        public SPI_DEVICE_CHANNEL_withReset Inlet = new SPI_DEVICE_CHANNEL_withReset(1, Command.SPI.Inlet.setVoltage, 1, Command.SPI.Inlet.getVoltage);
        /// <summary>
        /// Нагреватель (имеет общий reset() с натекателем Inlet)
        /// </summary>
        public SPI_DEVICE_CHANNEL_withReset Heater = new SPI_DEVICE_CHANNEL_withReset(2, Command.SPI.Heater.setVoltage, 2, Command.SPI.Heater.getVoltage);
        /// <summary>
        /// Ионный Источник
        /// </summary>
        public SPI_IonSOURCE IonSource = new SPI_IonSOURCE();
        /// <summary>
        /// Детектор
        /// </summary>
        public SPI_DETECTOR Detector = new SPI_DETECTOR();
        /// <summary>
        /// Сканер
        /// </summary>
        public SPI_SCANER Scaner = new SPI_SCANER();//У Сканера и Конденсатора DAC'и AD5643R и один общий ADC
        /// <summary>
        /// Конденсатор
        /// </summary>
        public SPI_CONDENSATOR Condensator = new SPI_CONDENSATOR();
        /// <summary>
        /// Микроконтроллер XMega32A4U
        /// </summary>
        public CHIP Chip = new CHIP();
        /// <summary>
        /// Тестовые функции для отладки.
        /// </summary>
        public TEST Tester = new TEST();
        /// <summary>
        /// Микроконтроллер вакуумного насоса
        /// </summary>
        public TIC_PUMP TIC = new TIC_PUMP();
        //--------------------------------------ВИДИМЫЕ ФУНКЦИИ-------------------------------------------
        /// <summary>
        /// Возвращает List(string) всех возникших ошибок (В РАЗРАБОТКЕ...)
        /// </summary>
        public List<string> getErrorList()
        {
            //ФУНКЦИЯ: Возвращает лист ошибок, которые произошли во время сеанса.
            trace_attached(Environment.NewLine);
            trace(".getErrorList()");
            return ErrorList;
        }
        //--------------------------------------ВНУТРЕННИЕ ФУНКЦИИ-------------------------------------------
        //КОНСТРУКТОР
        public XMEGA32A4U()
        {
            trace("Инициализация " + DateTime.Now.ToString("dd MMMM yyyy"));
        }
        //Флаги
        /// <summary>
        /// МК устанавливает и возвращает флаги в порядке |x|x|iHVE|iEDCD|SEMV1|SEMV2|SEMV3|SPUMP|, где SPUM - нулевой бит
        /// </summary>
        /// <param name="set_flags">true - установить следующие флаги, <para>false - не устанавливать, только проверить</para></param>
        /// <param name="HVE">Разрешение высокого напряжения</param>
        /// <param name="EDCD">Разрешение дистанционного управления</param>
        /// <param name="SEMV1">Электромагнитный вентиль 1</param>
        /// <param name="SEMV2">Электромагнитный вентиль 2</param>
        /// <param name="SEMV3">Электромагнитный вентиль 3</param>
        /// <param name="SPUMP">Включение насоса?</param>
        /// <returns>Байт флагов в порядке |x|x|iHVE|iEDCD|SEMV1|SEMV2|SEMV3|SPUMP| </returns>
        public byte setFlags(bool set_flags, bool HVE, bool EDCD, bool SEMV1, bool SEMV2, bool SEMV3, bool SPUMP)
        {
            //ФУНКЦИЯ: Выставляет флаги в соответствии с принятым байтом, если первый байт 1, и возвращает результат. Иначе просто возвращает флаги
            //ПОЯСНЕНИЯ: Формат байта: <Проверить\Установить><0><iHVE><iEDCD><SEMV1><SEMV2><SEMV3><SPUMP>
            trace_attached(Environment.NewLine);
            trace(".setFlags(" + Convert.ToInt16(set_flags) + 0 + Convert.ToInt16(HVE) + Convert.ToInt16(EDCD) + Convert.ToInt16(SEMV1) + Convert.ToInt16(SEMV2) + Convert.ToInt16(SEMV3) + Convert.ToInt16(SPUMP) + ")");
            byte flags = Convert.ToByte(Convert.ToInt16(set_flags) * 128 + Convert.ToInt16(HVE) * 32 + Convert.ToInt16(EDCD) * 16 + Convert.ToInt16(SEMV1) * 8 + Convert.ToInt16(SEMV2) * 4 + Convert.ToInt16(SEMV3) * 2 + Convert.ToInt16(SPUMP));
            //trace(flags.ToString());
            flags = transmit(Command.setFlags, flags)[0];
            if ((flags & 64) == 64) { trace(".setFlags(): Отмена операции. Флаги синхронизированы!"); }
            return flags;
        }
        //ИНТЕРФЕЙСНЫЕ
        static void trace(string text)
        {
            //ФУНКЦИЯ: Выводит text новой строкой с датой в RichTextBox и в файл, если эти действия разарешены
            string TEXT = Environment.NewLine + "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + text;
            if (tracer_defined && tracer_enabled)
            {
                //Трэйсер определён и разрешён
                tracer.AppendText(TEXT);
                tracer.ScrollToCaret();
            }
            if (tracer_log_enabled)
            {
                //Вывод в файл разрешён
                System.IO.File.AppendAllText("Log.txt", TEXT);
            }
        }
        static void trace_attached(string text)
        {
            //ФУНКЦИЯ: Выводит text, прикрепляя к старой строке, в RichTextBox и в файл, если эти действия разарешены
            string TEXT = text;
            if (tracer_defined && tracer_enabled)
            {
                //Трэйсер определён и разрешён
                tracer.AppendText(TEXT);
                tracer.ScrollToCaret();
            }
            if (tracer_log_enabled)
            {
                //Вывод в файл разрешён
                System.IO.File.AppendAllText("Log.txt", TEXT);
            }
        }
        //USART
        static byte calcCheckSum(byte[] data)
        {
            //ФУНКЦИЯ: Вычисление контрольной суммы для верификации данных
            byte CheckSum = 0;
            for (int i = 0; i < data.Length; i++)
            {
                CheckSum -= data[i];
            }
            trace("             Контрольная сумма: " + CheckSum);
            return CheckSum;
        }
        static byte[] transmit(List<byte> DATA)
        {
            //ФУНКЦИЯ: Формируем пакет, передаём его МК, слушаем ответ, возвращаем ответ.
            bool tracer_enabled_before = tracer_enabled;
            tracer_enabled = tracer_transmit_enabled;
            byte command = DATA[0];                         //Запоминаем передаваемую команду
            trace("     Начало исполнения команды:");
            trace("         Команда и байты данных:");
            foreach(byte b in DATA.ToArray())
            {
                trace("             " + b);
            }
            trace("         Формирование пакета...");
            List<byte> rDATA = new List<byte>();            //Список данных, которые будут приняты
            List<byte> Packet = new List<byte>();           //Список байтов, которые будут посланы
            //Формируем пакет по типу:  ':<response><data><CS>\r' 
            Packet.Add(Command.KEY);                        //':' - Начало данных           
            Packet.AddRange(DATA);                          //'<data>' - байты данных <<response><attached_data>>
            Packet.Add(calcCheckSum(DATA.ToArray()));       //'<CS>' - контрольная сумма
            Packet.Add(Command.LOCK);                       //'\r' - конец передачи
            trace("         Пакет:");
            foreach (byte b in Packet.ToArray())
            {
                trace("             " + b);
            }
            //Выполняем передачу и приём
            trace("         Передача...");
            if (!USART_defined)
            {
                trace("СОМ-порт не определён!");
                tracer_enabled = tracer_enabled_before;
                return new byte[] {0};
            }
            USART.Open();
            USART.Write(Packet.ToArray(), 0, Packet.ToArray().Length);
            trace("         Передача завершена!");
            CommandStack++;                                 //Увеличиваем счётчик команд (команда ушла)
            Thread.Sleep(delay);                            //Задерживаемся, потом принимаем данные            
            trace("         Приём...");                     //Приём-приём
            byte rBYTE;                                     //Принятый байт
            byte BytesToReadQuantity = Convert.ToByte(USART.BytesToRead);
            trace("             Данные на приём: " + BytesToReadQuantity);
            if (BytesToReadQuantity == 0)
            {
                trace("ОШИБКА ПРИЁМА ДАННЫХ! Не было получено никаких данных!");
                USART.Close();
                tracer_enabled = tracer_enabled_before;
                return new byte[] {0};
            }
            trace("                 Принято:");
            //Принимаем данные пока есть что принимать
            while (USART.BytesToRead > 0)
            {
                try
                {
                    rBYTE = (byte)USART.ReadByte();
                    trace("                     "+rBYTE);
                }
                catch
                {
                    trace("ОШИБКА ПРИЁМА ДАННЫХ! Приём не удался!");
                    USART.Close();
                    tracer_enabled = tracer_enabled_before;
                    return new byte[] {0};
                }
                rDATA.Add(rBYTE);
            }
            USART.Close();
            trace("         Приём завершён!");
            //Если последний байт затвор, то всё путём
            if (rDATA.First<byte>() == Command.KEY)
            {
                rDATA.RemoveAt(0);
                if (rDATA.Last<byte>() == Command.LOCK)
                {
                    rDATA.RemoveAt(rDATA.Count - 1);
                    //Анализируем полученные данные
                    trace("         Анализ полученной команды...");
                    byte rCheckSum = rDATA.Last();                          //Полученная КС
                    rDATA.RemoveAt(rDATA.Count - 1); //Убираем КС из списка полученных данных
                    byte CheckSum = calcCheckSum(rDATA.ToArray());          //Подсчитанная КС
                    if (CheckSum == rCheckSum)
                    {
                        trace("             Контрольная сумма совпадает!");
                    }
                    else
                    {
                        trace("             Несовпадает контрольная сумма!");
                        trace("                 Получено:" + rCheckSum);
                        trace("                 Подсчитано:" + CheckSum);
                        tracer_enabled = tracer_enabled_before;
                        return new byte[] { 0 };
                    }
                    //Проверяем данные на отклик
                    trace("             Отклик: " + rDATA[0]);
                    if (rDATA[0] == command)
                    {
                        trace("             Отклик совпадает!");
                        if (BytesToReadQuantity > 4)
                        {

                            rDATA.RemoveAt(0);
                            trace("         Пакет принятых данных: ");
                            foreach (byte b in rDATA)
                            {
                                trace("             " + b);
                            }
                        }
                    }
                    else
                    {
                        trace("ОШИБКА ОТКЛИКА! Отклик не совпадает!");
                        trace("     Ожидалось: " + command);
                        trace("     Получено: " + rDATA[0]);
                        defineError(rDATA.ToArray());
                        tracer_enabled = tracer_enabled_before;
                        return new byte[] { 0 };
                    }
                    tracer_enabled = tracer_enabled_before;
                    return rDATA.ToArray();
                }
                trace("ОШИБКА ПРИЁМА ДАННЫХ! Не был получен затвор!");
            }
            trace("ОШИБКА ПРИЁМА ДАННЫХ! Не был получен ключ!");
            tracer_enabled = tracer_enabled_before;
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
        static void defineError(byte[] DATA)
        {
            //ФУНКЦИЯ: Дешефрирует ошибку
            trace("---------------ОШИБКА-------------");
            trace("Принятые данные:");
            foreach (byte b in DATA)
            {
                
                trace("     " + b);
            }
            if (DATA.Length > 1)
            {
                if (DATA[0] == Error.Token)
                {
                    //это сообщение об ошибке
                    switch (DATA[1])
                    {
                        default:
                            if (DATA.Length > 2)
                            {
                                switch (DATA[1])
                                {
                                    case Error.DecoderError:
                                        trace("МК СООБЩАЕТ ОБ ОШИБКЕ ДЕКОДЕРА! Неизвестная команда: " + DATA[2]);
                                        break;
                                    case Error.KeyError:
                                        trace("МК СООБЩАЕТ ОБ ОШИБКЕ! НЕ БЫЛ ПОЛУЧЕН КЛЮЧ! Ключ: " + DATA[2]);
                                        break;
                                    case Error.LockError:
                                        trace("МК СООБЩАЕТ ОБ ОШИБКЕ! НЕ БЫЛ ПОЛУЧЕН ЗАМОК! Замок: " + DATA[2]);
                                        break;
                                    case Error.CheckSumError:
                                        trace("МК СООБЩАЕТ ОБ ОШИБКЕ! Неверная контрольная сумма: " + DATA[2]);
                                        break;
                                    default:
                                        trace("МК сообщает о неизвестной ОШИБКЕ № " + DATA[1] + "!");
                                        break;
                                }
                            }
                            else
                            {
                                trace("Неизвестная ошибка!");
                            }
                            break;
                    }
                }
                else
                {
                    trace("Неверное сообщение об ошибке! Отсутствует метка ошибки!");
                }
            }
            else
            {
                trace("Слишком короткое сообщение. Вероятно, это отклик.");
            }
        }
        //   ! Р А З О Б Р А Т Ь ! Неиспользуемые функции
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
        static void addError(string TEXT, byte[] rDATA)
        {
            string error = (ErrorList.Count + 1).ToString() + " [" + DateTime.Now.ToString("HH:mm:ss") + "] " + TEXT + " Получено: ";
            foreach(byte b in rDATA)
            {
                error += b + " | ";
            }
            trace(error);
            ErrorList.Add(error);
        }
        void wait()
        {
            //ФУНКЦИЯ: Устанавливает статус МК
            byte[] wDATA = { Command.Chip.wait };
            USART.Open();
            USART.Write(wDATA, 0, 1);
            USART.Close();
        }
        bool reset()
        {
            //ФУНКЦИЯ: Програмная перезагрузка микроконтроллера
            return (transmit(Command.Chip.reset)[0] == Command.Chip.reset);
        }
        //--------------------------------------ЗАМЕТКИ-------------------------------------------
        //public void AsyncEndable(bool enable)
        //{
        //    if (enable)
        //    {
        //        USART.DataReceived += new SerialDataReceivedEventHandler(AsyncHandler);
        //    }
        //    else
        //    {
        //        USART.DataReceived -= new SerialDataReceivedEventHandler(AsyncHandler);
        //    }
        //}
        //public void AsyncHandler(object sender,SerialDataReceivedEventArgs e)
        //{
        //    SerialPort usart = (SerialPort)sender;
        //    string data = usart.ReadExisting();
        //    recived_error = (byte)data.Length;
        //}
    }
    //---------------------------------------THE END------------------------------------------
}
