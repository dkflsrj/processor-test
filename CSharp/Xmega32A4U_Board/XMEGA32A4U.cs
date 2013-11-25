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
    /// Класс связи ПК с ATXmega32A4U
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
        static bool RTC_is_busy = false;
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
                public const byte startMeasure =        30;
                public const byte stopMeasure =         31;
                public const byte setAll =              32;
                public const byte LAM =                 33;
            }
            public struct TIC
            {
                //Коды команд для микроконтроллера насоса
                public const byte sendToTIC = 11;
            }
            public struct SPI
            {
                //Коды команд для SPI устройств
                public struct IonSource
                {
                    //Коды команд Ионного Источника
                    public struct EmissionCurrent
                    {
                        public const byte setVoltage = 40;
                        public const byte getVoltage = 60;
                    }
                    public struct Ionization
                    {
                        public const byte setVoltage = 40;
                        public const byte getVoltage = 60;
                    }
                    public struct F1
                    {
                        public const byte setVoltage = 40;
                        public const byte getVoltage = 60;
                    }
                    public struct F2
                    {
                        public const byte setVoltage = 40;
                        public const byte getVoltage = 60;
                    }
                }
                public struct Detector
                {
                    //Коды команд Детектора
                    public struct DV1
                    {
                        public const byte setVoltage = 41;
                        public const byte getVoltage = 64;
                    }
                    public struct DV2
                    {
                        public const byte setVoltage = 41;
                        public const byte getVoltage = 65;
                    }
                    public struct DV3
                    {
                        public const byte setVoltage = 41;
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
                        public const byte setVoltage = 42;
                        public const byte getVoltage = 70;
                    }
                    public struct Scan
                    {
                        public const byte setVoltage = 42;
                        public const byte getVoltage = 70;
                    }
                }
                public struct Condensator
                {
                    //Коды команд Конденсатора
                    public const byte setVoltage = 43;
                    public const byte getPositiveVoltage = 70;
                    public const byte getNegativeVoltage = 70;
                }
            }
            public const byte LOCK = 13;
            public const byte KEY = 58;
            public struct TEST
            {
                //Коды команд отладки
                public const byte checkCommandStack = 8;
            }
            public const byte setFlags = 80;
        }

        //---------------------------------------КЛАССЫ--------------------------------------------
        public class RTCounterAndCO
        {
            //КЛАСС: Счётчик реального времени и счётчики импульсов
            struct Constants
            {
                //СТРУКТУРА: Хранилище констант.
                public const double sourceFrequency = 32.768;//кГц - опорная частота таймера
                //Коды состояний
                public struct Status
                {
                    public const byte notSet =              0;		//Счётчики не настроен
                    public const string string_notSet =     "notSet";
                    public const byte ready =               1;		//Счётчики готов к работе
                    public const string string_ready =      "ready";
                    public const byte stopped =             2;		//Счётчики был принудительно остановлен
                    public const string string_stopped =    "stopped";
                    public const byte busy =                3;		//Счётчики ещё считает
                    public const string string_busy =       "busy";
                    public const byte delayed =             4;		//RTC считает задержку
                    public const string string_delayed =    "delayed";
                }
                //интервалы для предделителей
                public const int min_ms_div1 =              0;
                public const int min_ms_div2 =              2000;
                public const int min_ms_div8 =              4000;
                public const int min_ms_div16 =             16000;
                public const int min_ms_div64 =             32000;
                public const int min_ms_div256 =            127996;
                public const int min_ms_div1024 =           511981;
                public const int max_ms_div1024 =           2047925;
                //Проча
                public const byte LengthOfSetAllPacket =    28;
                public const byte LengthOfLAMPacket =       6;
                public struct NextMeasure
                {
                    //Коды команд счётчиков
                    public const byte NotDo =               0;
                    public const byte Do =                  1;
                }

            }
            
            public class counter
            {
                //КЛАСС: Счётчик. Только хранит значения.
                /// <summary>
                /// Количество переполнений счётчика
                /// </summary>
                public List<byte> Overflows = new List<byte>();  //Количество переполений счётчика
                /// <summary>
                /// Сосчитанный результат
                /// </summary>
                public List<uint> Count = new List<uint>();   //Сосчитанный результат
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
            //Настройки для измерений
            public class _measure
            {
                /// <summary>
                /// Массив времени измерений в миллисекундах. (4096 измерений)
                /// </summary>
                public uint[] MeasureTimes = new uint[4096];
                /// <summary>
                /// Массив времени задержек между измерениями в миллисекундах. (4096 задержек)
                /// </summary>
                public uint[] DelayTimes = new uint[4096];
                public class _DAC
                {
                    /// <summary>
                    /// Массив Дополнительных Сканирующих напряжений (4096 напряжений)
                    /// </summary>
                    public ushort[] ParentScan = new ushort[4096];
                    /// <summary>
                    /// Массив Сканирующих напряжений (4096 напряжений)
                    /// </summary>
                    public ushort[] Scan = new ushort[4096];
                    /// <summary>
                    /// Массив напряжений Конденсатора (4096 напряжений)
                    /// </summary>
                    public ushort[] Condensator = new ushort[4096];
                }
                /// <summary>
                /// Массив напряжений, которые будут выставлены МК во время задержки (4096 напряжений)
                /// </summary>
                public _DAC DAC = new _DAC();
                public class _ADC
                {
                    public ushort[] ParentScan = new ushort[4096];
                    public ushort[] Scan = new ushort[4096];
                    public ushort[] Condensator_pV = new ushort[4096];
                    public ushort[] Condensator_nV = new ushort[4096];
                }
                /// <summary>
                /// Массив напряжений, которые будут считаны МК-ом во время измерния с АЦП (4096 напряжений)
                /// </summary>
                public _ADC ADC = new _ADC();
                /// <summary>
                /// Количество ступеней измерений в серии. От 1 до 4096.
                /// <para>По умолчанию: 1</para>
                /// </summary>
                public ushort Cycles = 1;
            }
            /// <summary>
            /// Параметры серии измерений
            /// </summary>
            public _measure Series = new _measure();
            /// <summary>
            /// Состояние счётчиков
            /// <para>  notSet: С момента запуска МК не было установлено параметров серии измерений.</para>
            /// <para>  ready: Счётчики готовы к работе.</para>
            /// <para>  busy: Счётчики считают импульсы.</para>
            /// <para>  delayed: Счётчики отключены. МК настраивает DAC MSV для следующего измерения.</para>
            /// <para>  stopped: Серия была прервана.</para>
            /// </summary>
            public string Status = "notSet";

            string convertStatus(byte Status_byte)
            {
                //ФУНКЦИЯ: Переводит числовой байт статуса в строку
                string Status_string = "";
                switch(Status_byte)
                {
                    case Constants.Status.notSet:
                        Status_string = Constants.Status.string_notSet;
                        break;
                    case Constants.Status.ready:
                        Status_string = Constants.Status.string_ready;
                        break;
                    case Constants.Status.stopped:
                        Status_string = Constants.Status.string_stopped;
                        break;
                    case Constants.Status.busy:
                        Status_string = Constants.Status.string_busy;
                        break;
                    case Constants.Status.delayed:
                        Status_string = Constants.Status.string_delayed;
                        break;
                    default:
                        Status_string = "НЕИЗВЕСТНОЕ СОСТОЯНИЕ!";
                        break;
                }
                return Status_string;
            }
            //Видимые функции
            /// <summary>
            /// Вычисляет и возвращает предделитель RTC 
            /// <para>ПРИМЕЧАНИЕ: без участия МК</para>
            /// </summary>
            /// <param name="MeasureTime_ms">Время измерения в миллисекундах от 0 до 2047925</param>
            /// <returns>?</returns>
            public byte getRTCprescaler(uint MILLISECONDS)
            {
                byte prescaler; //Предделитель
                if ((MILLISECONDS >= Constants.min_ms_div1) && (MILLISECONDS < Constants.min_ms_div2))
                {
                    prescaler = 1;
                }
                else if ((MILLISECONDS >= Constants.min_ms_div2) && (MILLISECONDS < Constants.min_ms_div8))
                {
                    prescaler = 2;
                }
                else if ((MILLISECONDS >= Constants.min_ms_div8) && (MILLISECONDS < Constants.min_ms_div16))
                {
                    prescaler = 3;
                }
                else if ((MILLISECONDS >= Constants.min_ms_div16) && (MILLISECONDS < Constants.min_ms_div64))
                {
                    prescaler = 4;
                }
                else if ((MILLISECONDS >= Constants.min_ms_div64) && (MILLISECONDS < Constants.min_ms_div256))
                {
                    prescaler = 5;
                }
                else if ((MILLISECONDS >= Constants.min_ms_div256) && (MILLISECONDS < Constants.min_ms_div1024))
                {
                    prescaler = 6;
                }
                else if ((MILLISECONDS >= Constants.min_ms_div1024) && (MILLISECONDS < Constants.max_ms_div1024))
                {
                    prescaler = 7;
                }
                else
                {
                    trace("ОШИБКА! Неверный интервал! Ожидалось: 0 < MeasureTime_ms < 2047967; Получено: " + MILLISECONDS);
                    prescaler = 0;
                    return 0;
                }
                return prescaler;
            }
            /// <summary>
            /// Вычисляет и возвращает предделитель RTC в длинном формате
            /// <para>ПРИМЕЧАНИЕ: без участия МК</para>
            /// </summary>
            /// <param name="MeasureTime_ms">Время измерения в миллисекундах от 0 до 2047925</param>
            /// <returns>?</returns>
            public ushort getRTCprescaler_long(uint MILLISECONDS)
            {
                //ФУНКЦИЯ: Вычисляет, сохраняет и возвращает предделитель.
                ushort prescaler_long; //Предделитель в реальном коэффициенте деления (см. prescaler цифры в скобках)
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
                    trace("ОШИБКА! Неверный интервал! Ожидалось: 0 < MeasureTime_ms < 2047967; Получено: " + MILLISECONDS);
                    prescaler_long = 0;
                    return 0;
                }
                return prescaler_long;
            }
            /// <summary>
            /// Вычисляет и возвращает предделитель RTC (в длинном формате) 
            /// <para>ПРИМЕЧАНИЕ: без участия МК</para>
            /// </summary>
            /// <param name="MeasureTime_ms">Время измерения в миллисекундах от 0 до 2047925</param>
            /// <returns>?</returns>
            public ushort getRTCprescaler_long(string MILLISECONDS)
            {
                return getRTCprescaler_long(Convert.ToUInt32(MILLISECONDS));
            }
            /// <summary>
            /// Возвращает частоту RTC в соответствии с предделителем в длинном формате
            /// <para>ПРИМЕЧАНИЕ: без участия МК</para>
            /// </summary>
            public double getRTCfrequency(uint MILLISECONDS)
            {
                //ФУНКЦИЯ: Возвращает итоговую частоту RTC в соответствии с сохраннённым предделителем.
                return (Constants.sourceFrequency / getRTCprescaler_long(MILLISECONDS));
            }
            /// <summary>
            /// Возвращает частоту RTC в соответствии с предделителем в длинном формате
            /// <para>ПРИМЕЧАНИЕ: без участия МК</para>
            /// </summary>
            public double getRTCfrequency(string MILLISECONDS)
            {
                //ФУНКЦИЯ: Возвращает итоговую частоту RTC в соответствии с сохраннённым предделителем.
                return (Constants.sourceFrequency / getRTCprescaler_long(Convert.ToUInt32(MILLISECONDS)));
            }
            /// <summary>
            /// Возвращает количество тиков RTC для отсчёта заданного времени с заданным предделителем 
            /// <para>ПРИМЕЧАНИЕ: без участия МК</para>
            /// </summary>
            /// <param name="MeasureTime_ms">Время измерения в миллисекундах от 0 до 2047925</param>
            /// <param name="PRESCALER_long">Предделитель. Возможные значения: 1,2,8,16,64,256,1024</param>
            /// <returns></returns>
            public ushort getRTCticks(uint MILLISECONDS)
            {
                //ФУНКЦИЯ: Вычисляет количество тиков в соответствии с временем и предделителем. Возвращает количество тиков
                ushort tiks = Convert.ToUInt16(Math.Round(Convert.ToDouble(MILLISECONDS) * (Constants.sourceFrequency / getRTCprescaler_long(MILLISECONDS))));
                return tiks;
            }
            /// <summary>
            /// Возвращает количество тиков RTC для отсчёта заданного времени с заданным предделителем 
            /// <para>ПРИМЕЧАНИЕ: без участия МК</para>
            /// </summary>
            /// <param name="MeasureTime_ms">Время измерения в миллисекундах от 0 до 2047925</param>
            /// <param name="PRESCALER_long">Предделитель. Возможные значения: 1,2,8,16,64,256,1024</param>
            /// <returns></returns>
            public ushort getRTCticks(string MILLISECONDS)
            {
                if (MILLISECONDS != "")
                {
                    return getRTCticks(Convert.ToUInt32(MILLISECONDS));
                }
                else
                {
                    return 0;
                }
            }
            /// <summary>
            /// Запускает серию измерений. Количество ступеней в серии определяется параметром Series_Cycles
            /// <para>Параметры для каждой ступени измерений беруться из соответствующих массивов.</para>
            /// <para>Возвращает:</para>
            /// <para>true - серия успешно завершена.</para>
            /// <para>false - ошибка при выполнении операции.</para>
            /// </summary>
            public bool startMeasure()
            {
                //ФУНКЦИЯ: Запускаем счётчик, возвращает true если счёт начался, false - счётчик уже считает

                const byte A = 2;//Отладочное (порядок байтов при обращении к ЦАПам)
                const byte B = 1;//Отладочное (порядок байтов при обращении к ЦАПам)

                ushort buf = 0; //Отладочный буффер

                string command = "Counters.startMeasure()";
                trace_attached(Environment.NewLine);
                trace(command);
                if (RTC_is_busy)
                {
                    trace("Отмена операции! Запрещено во время счёта!");
                    return false;
                }
                RTC_is_busy = true;
                COA.Count.Clear();
                COA.Overflows.Clear();
                COB.Count.Clear();
                COB.Overflows.Clear();
                COC.Count.Clear();
                COC.Overflows.Clear();
                //Отладочные настройки
                //Объявления
                List<byte> wDATA = new List<byte>();
                List<byte> rDATA = new List<byte>();
                byte[] BYTES_buf = new byte[4];
                byte MeasurePrescaler = 0;
                byte[] MeasurePeriod = new byte[2];
                byte DelayPrescaler = 0;
                byte[] DelayPeriod = new byte[2];
                byte[] ParentScanerBytes = new byte[3];
                byte[] ScanerBytes = new byte[3];
                byte[] CondensatorBytes = new byte[3];
                ushort N = 0; //Номер текущего измерения
                //Вычисление байт для RTC
                MeasurePrescaler = getRTCprescaler(Series.MeasureTimes[N]);
                BYTES_buf = BitConverter.GetBytes(getRTCticks(Series.MeasureTimes[N]));
                MeasurePeriod = new byte[] { BYTES_buf[0], BYTES_buf[1] };
                DelayPrescaler = getRTCprescaler(Series.DelayTimes[N]);
                BYTES_buf = BitConverter.GetBytes(getRTCticks(Series.DelayTimes[N]));
                DelayPeriod = new byte[] { BYTES_buf[0], BYTES_buf[1] };
                //Вычисление байт для DAC'a Родительского Напряжения (канал = 24)
                BYTES_buf = BitConverter.GetBytes(Series.DAC.ParentScan[N] << 2);
                ParentScanerBytes = new byte[] { Convert.ToByte(24), BYTES_buf[0], BYTES_buf[1] };
                //Вычисление байт для DAC'a Сканирующего Напряжения (канал = 25)
                BYTES_buf = BitConverter.GetBytes(Series.DAC.Scan[N] << 2);
                ScanerBytes = new byte[] { Convert.ToByte(25), BYTES_buf[0], BYTES_buf[1] };
                //Вычисление байт для DAC'a Конденсатора (канал = 24)
                BYTES_buf = BitConverter.GetBytes(Series.DAC.Condensator[N] << 2);
                CondensatorBytes = new byte[] { Convert.ToByte(24), BYTES_buf[0], BYTES_buf[1] };
                //Набираем данные на отправку
                wDATA.Add(Command.RTC.setAll); //Команда
                wDATA.Add(Constants.NextMeasure.NotDo);    //Не делать сделующе измерение
                wDATA.Add(MeasurePrescaler);
                wDATA.Add(MeasurePeriod[1]);
                wDATA.Add(MeasurePeriod[0]);
                wDATA.Add(DelayPrescaler);
                wDATA.Add(DelayPeriod[1]);
                wDATA.Add(DelayPeriod[0]);
                wDATA.Add(ParentScanerBytes[0]);
                wDATA.Add(ParentScanerBytes[A]);
                wDATA.Add(ParentScanerBytes[B]);
                wDATA.Add(ScanerBytes[0]);
                wDATA.Add(ScanerBytes[A]);
                wDATA.Add(ScanerBytes[B]);
                wDATA.Add(CondensatorBytes[0]);
                wDATA.Add(CondensatorBytes[A]);
                wDATA.Add(CondensatorBytes[B]);
                rDATA = transmit_2(wDATA, Constants.LengthOfSetAllPacket, false);                    //Посылаем настройки для измерения №0, но ответ забываем
                Status = convertStatus(rDATA[2]);
                rDATA.Clear();
                //Подготовка команды начала измерений
                wDATA.Clear();
                wDATA.Add(Command.RTC.startMeasure);

                send_2(wDATA, false);                           //Посылаем команду начала измерения (не слушаем ответ), а надо бы

                //N = 0;//
                N++;
                //Подготовка новых данных для следующего измерения
                MeasurePrescaler = getRTCprescaler(Series.MeasureTimes[N]);
                BYTES_buf = BitConverter.GetBytes(getRTCticks(Series.MeasureTimes[N]));
                MeasurePeriod = new byte[] { BYTES_buf[0], BYTES_buf[1] };
                DelayPrescaler = getRTCprescaler(Series.DelayTimes[N]);
                BYTES_buf = BitConverter.GetBytes(getRTCticks(Series.DelayTimes[N]));
                DelayPeriod = new byte[] { BYTES_buf[0], BYTES_buf[1] };
                //Вычисление байт для Родительского Напряжения (канал = 24)
                BYTES_buf = BitConverter.GetBytes(Series.DAC.ParentScan[N] << 2);
                ParentScanerBytes = new byte[] { Convert.ToByte(24), BYTES_buf[0], BYTES_buf[1] };
                //Вычисление байт для Сканирующего Напряжения (канал = 25)
                BYTES_buf = BitConverter.GetBytes(Series.DAC.Scan[N] << 2);
                ScanerBytes = new byte[] { Convert.ToByte(25), BYTES_buf[0], BYTES_buf[1] };
                //Вычисление байт для Конденсатора (канал = 24)
                BYTES_buf = BitConverter.GetBytes(Series.DAC.Condensator[N] << 2);
                CondensatorBytes = new byte[] { Convert.ToByte(24), BYTES_buf[0], BYTES_buf[1] };
                //Формирование данных
                wDATA.Clear();
                wDATA.Add(Command.RTC.setAll); //Команда
                if (Series.Cycles == 1)
                {
                    wDATA.Add(Constants.NextMeasure.NotDo);    //Это последнее измерение! Не делать следующее
                }
                else
                {
                    wDATA.Add(Constants.NextMeasure.Do);    //Делать следующее измерение
                }
                wDATA.Add(MeasurePrescaler);
                wDATA.Add(MeasurePeriod[1]);
                wDATA.Add(MeasurePeriod[0]);
                wDATA.Add(DelayPrescaler);
                wDATA.Add(DelayPeriod[1]);
                wDATA.Add(DelayPeriod[0]);
                wDATA.Add(ParentScanerBytes[0]);
                wDATA.Add(ParentScanerBytes[A]);
                wDATA.Add(ParentScanerBytes[B]);
                wDATA.Add(ScanerBytes[0]);
                wDATA.Add(ScanerBytes[A]);
                wDATA.Add(ScanerBytes[B]);
                wDATA.Add(CondensatorBytes[0]);
                wDATA.Add(CondensatorBytes[A]);
                wDATA.Add(CondensatorBytes[B]);
                rDATA = transmit_2(wDATA, Constants.LengthOfSetAllPacket, false);   //Посылаем настройки для измерения №1, но ответ забываем
                Status = convertStatus(rDATA[2]);
                rDATA.Clear();
                //Цикл...
                for (int i = 0; i < Series.Cycles; i++)
                {
                    rDATA = decode_2(receive_2(Constants.LengthOfLAMPacket, false));
                    Status = convertStatus(rDATA[1]);
                    rDATA.Clear();

                    //N = 0;//
                    N++;
                    //Подготовка новых данных для следующего измерения
                    MeasurePrescaler = getRTCprescaler(Series.MeasureTimes[N]);
                    BYTES_buf = BitConverter.GetBytes(getRTCticks(Series.MeasureTimes[N]));
                    MeasurePeriod = new byte[] { BYTES_buf[0], BYTES_buf[1] };
                    DelayPrescaler = getRTCprescaler(Series.DelayTimes[N]);
                    BYTES_buf = BitConverter.GetBytes(getRTCticks(Series.DelayTimes[N]));
                    DelayPeriod = new byte[] { BYTES_buf[0], BYTES_buf[1] };
                    //Постараемся не выйти за диапозон массива измерений
                    buf = N;
                    if (N > Series.Cycles - 1)
                    {
                        N = (ushort)(Series.Cycles - 1);
                    }
                    //Вычисление байт для Родительского Напряжения (канал = 24)
                    BYTES_buf = BitConverter.GetBytes(Series.DAC.ParentScan[N] << 2);
                    ParentScanerBytes = new byte[] { Convert.ToByte(24), BYTES_buf[0], BYTES_buf[1] };
                    //Вычисление байт для Сканирующего Напряжения (канал = 25)
                    BYTES_buf = BitConverter.GetBytes(Series.DAC.Scan[N] << 2);
                    ScanerBytes = new byte[] { Convert.ToByte(25), BYTES_buf[0], BYTES_buf[1] };
                    //Вычисление байт для Конденсатора (канал = 24)
                    BYTES_buf = BitConverter.GetBytes(Series.DAC.Condensator[N] << 2);
                    CondensatorBytes = new byte[] { Convert.ToByte(24), BYTES_buf[0], BYTES_buf[1] };
                    N = buf;
                    //Формирования данных
                    wDATA.Clear();
                    wDATA.Add(Command.RTC.setAll); //Команда
                    if(i == Series.Cycles - 2)
                    {
                        wDATA.Add(Constants.NextMeasure.NotDo);    //Это последнее измерение! Не делать следующее
                    }
                    else
                    {
                        wDATA.Add(Constants.NextMeasure.Do);    //Делать следующее измерение
                    }
                    wDATA.Add(MeasurePrescaler);
                    wDATA.Add(MeasurePeriod[1]);
                    wDATA.Add(MeasurePeriod[0]);
                    wDATA.Add(DelayPrescaler);
                    wDATA.Add(DelayPeriod[1]);
                    wDATA.Add(DelayPeriod[0]);
                    wDATA.Add(ParentScanerBytes[0]);
                    wDATA.Add(ParentScanerBytes[A]);
                    wDATA.Add(ParentScanerBytes[B]);
                    wDATA.Add(ScanerBytes[0]);
                    wDATA.Add(ScanerBytes[A]);
                    wDATA.Add(ScanerBytes[B]);
                    wDATA.Add(CondensatorBytes[0]);
                    wDATA.Add(CondensatorBytes[A]);
                    wDATA.Add(CondensatorBytes[B]);
                    rDATA = transmit_2(wDATA, Constants.LengthOfSetAllPacket, false);
                    Status = convertStatus(rDATA[2]);
                    //Сохраняем данные
                    COA.Overflows.Add(rDATA[3]);
                    COA.Count.Add((uint)(rDATA[4] * 16777216 + rDATA[5] * 65536 + rDATA[6] * 256 + rDATA[7]));
                    COB.Overflows.Add(rDATA[8]);
                    COB.Count.Add((uint)(rDATA[9] * 16777216 + rDATA[10] * 65536 + rDATA[11] * 256 + rDATA[12]));
                    COC.Overflows.Add(rDATA[13]);
                    COC.Count.Add((uint)(rDATA[14] * 256 + rDATA[15]));
                    Series.ADC.ParentScan[i] = Convert.ToUInt16((Convert.ToUInt16(rDATA[16] & 0xf) << 8) + rDATA[17]);
                    Series.ADC.Scan[i] = Convert.ToUInt16((Convert.ToUInt16(rDATA[18] & 0xf) << 8) + rDATA[19]);
                    Series.ADC.Condensator_pV[i] = Convert.ToUInt16((Convert.ToUInt16(rDATA[20] & 0xf) << 8) + rDATA[21]);
                    Series.ADC.Condensator_nV[i] = Convert.ToUInt16((Convert.ToUInt16(rDATA[22] & 0xf) << 8) + rDATA[23]);
                    //сервис
                    rDATA.Clear();
                    trace("["+N+"№" + (i + 1) + "][MT:" + Series.MeasureTimes[i] + "][DT:" + Series.DelayTimes[i] + "]" + Environment.NewLine + "        [DAC_PS:" + Series.DAC.ParentScan[i] + "][DAC_S:" + Series.DAC.Scan[i] + "][DAC_C:" + Series.DAC.Condensator[i] + "]" + Environment.NewLine + "        [ADC_PS:" + Series.ADC.ParentScan[i] + "][ADC_S:" + Series.ADC.Scan[i] + "][ADC_Cp:" + Series.ADC.Condensator_pV[i] + "][ADC_Cn:" + Series.ADC.Condensator_nV[i] + "]");
                }
                //конец
                if (USART.IsOpen)
                {
                    USART.Close();
                }
                //for (int i = 0; i < Series_Cycles; i++)
                //{
                //    trace("[№"+i+"][MT:"+Series_MeasureTimes[i]+"][DT:"+Series_DelayTimes[i]+"]"+Environment.NewLine+"        [DAC_PS:"+Series_DAC_ParentScan[i]+"][DAC_S:"+Series_DAC_Scan[i]+"][DAC_C:"+Series_DAC_Condensator[i]+"]"+Environment.NewLine+"        [ADC_PS:"+Series_ADC_ParentScan[i]+"][ADC_S:"+Series_ADC_Scan[i]+"][ADC_Cp:"+Series_ADC_Condensator_pV[i]+"][ADC_Cn:"+Series_ADC_Condensator_nV[i]+"]");
                //}
                RTC_is_busy = false;
                if (Status == Constants.Status.string_ready)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// Останавливает серию.
            ///<para>Возвращает:</para>
            ///<para>true - операция выполнена успешно.</para>
            ///<para>false - операция была отменена.</para>
            /// </summary>
            bool stopMeasure()
            {
                //ФУНКЦИЯ: Останавливает счётчик. Возвращает true, если операция удалась. false - не удалась (счётчик не считает)
                string command = "Counters.stopMeasure()";
                trace_attached(Environment.NewLine);
                trace(command);
                byte state = transmit(Command.RTC.stopMeasure)[0];
                switch(state)
                {
                    case Constants.Status.busy:
                        trace(command + ": Операция выполнена успешно!");
                        return true;
                    case Constants.Status.ready:
                        trace(command + ": Операция отменена! Счётчики не считают!");
                        return false;
                    case Constants.Status.stopped:
                        trace(command + ": Операция отменена! Счётчики уже остановлены!");
                        return false;
                    default:
                        addError(" ! " + command + ": ОШИБКА МК! НЕИЗВЕСТНОЕ СОСТОЯНИЕ СЧЁТЧИКОВ: " + state);
                        return false;
                }
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
                //* Настройка на двойной референс:
                //*                      1      00         xxxxx xx     GG2 GG1 GB2 GB1 GR2 GR1
                //*                     D\C   control      не парься       управляющие биты
                //* GG - умножение до 2хRef выходного напряжения на канале 1/0 (вкл/выкл)
                //* GB - включение буфферизации
                //* GR - установка в качестве референса Vdd
                //* В итоге нужно установить двойной референс и буфферизацию
                //*                      100х хххх хх11 1100
                //*                        128        60
                string _command = "DAC_CHANNEL.setVoltage(" + command + ", " + CHANNEL + ", " + VOLTAGE + ")";
                if (RTC_is_busy)
                {
                    trace("Отмена операции! Запрещено во время счёта!");
                    return false;
                }
                if (VOLTAGE >= 0 && VOLTAGE <= 4095)
                {
                    trace_attached(Environment.NewLine);
                    trace(_command);
                    byte[] bytes = BitConverter.GetBytes(VOLTAGE);
                    byte[] data = { Convert.ToByte((CHANNEL - 1) * 16 + bytes[1]), bytes[0] };
                    if (transmit(command, data)[0] == DAC_command)
                    {
                        trace(_command + ": Операция выполнена успешно!");
                        return true;
                    }
                    addError(" ! " + _command + ": ОШИБКА ОТКЛИКА!");
                }
                trace(_command + ": Операция отменена! Значение VOLTAGE ожидалось от 0 до 4095!");
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
                //         | 0 | 0 | 0 | Vin0 |    16    |   1   |
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
                string _command = "ADC_CHANNEL.getVoltage(" + command + ", " + CHANNEL + ")";
                trace_attached(Environment.NewLine);
                trace(_command + ": DoubleRange = " + DoubleRange);
                if (RTC_is_busy)
                {
                    trace("Отмена операции! Запрещено во время счёта!");
                    return 6666;
                }
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
                    addError(" ! " + _command + ":Ошибка при получении данных!");
                    return 4095;
                }
                trace(_command + ": Ответный адрес канала: " + adress);
                trace(_command + ": Напряжение: " + voltage);
                return voltage;
            }
            //Видимые функции
            /// <summary>
            /// Возвращает напряжение на канале ADC в диапазоне от 0 до 4095.
            /// </summary>
            public ushort getVoltage()
            {
                return ADC_getVoltage(ADC_command, ADC_channel);
            }
        }
        public class SPI_CONDENSATOR
        {
            //КЛАСС: Каналы для конденсатора (+\-)
            //DAC AD5643R
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
                string command = ".Condensator.setVoltage(" + Command.SPI.Condensator.setVoltage + "," + DAC_channel + "," + VOLTAGE + ")";
                if (RTC_is_busy)
                {
                    trace("Отмена операции! Запрещено во время счёта!");
                    return false;
                }
                if (VOLTAGE >= 0 && VOLTAGE <= 16383)
                {
                    trace(command + ": AD5643R;");
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
                        trace(command + ": Операция выполнена успешно!");
                        return true;
                    }
                    addError(command + ": ОШИБКА ОТКЛИКА!");
                    return false;
                }
                trace(command + ": Операция отменена! Значение VOLTAGE ожидалось от 0 до 16383!");
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
                string _command = ".Condensator.getVoltage(" + command + ", " + CHANNEL + ")";
                trace_attached(Environment.NewLine);
                trace(_command + ": DoubleRange = " + DoubleRange);
                if (RTC_is_busy)
                {
                    trace("Отмена операции! Запрещено во время счёта!");
                    return 6666;
                }
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
                    addError(" ! " + _command + ":Ошибка при получении данных!");
                    return 16383;
                }
                trace(_command + ": Ответный адрес канала: " + adress);
                trace(_command + ": Напряжение: " + voltage);
                return voltage;
            }
            //Видимые функции
            /// <summary>
            /// Возвращает напряжение положительной обкладки конденсатора в диапазоне от 0 до 4095.
            /// </summary>
            public ushort getPositiveVoltage()
            {
                return ADC_getVoltage(Command.SPI.Condensator.getPositiveVoltage, ADC_positive_channel);
            }
            /// <summary>
            /// Возвращает напряжение отрицательной обкладки конденсатора в диапазоне от 0 до 4095.
            /// </summary>
            public ushort getNegativeVoltage()
            {
                return ADC_getVoltage(Command.SPI.Condensator.getNegativeVoltage, ADC_negative_channel);
            }
            //Видимые функции
            /// <summary>
            /// Задаёт напряжение на DAC
            /// <para>VOLTAGE - напряжение от 0 до 16383</para>
            /// <para>Возвращает:</para>
            /// <para>true - операция выполнена успешно</para>
            /// <para>false - операция отменена (значение VOLTAGE находтся не в диапазоне от 0 до 16383 или ошибка отклика)</para>
            /// </summary>
            /// <param name="VOLTAGE">Напряжение от 0 до 16383</param>
            /// <returns></returns>
            public bool setVoltage(ushort VOLTAGE)
            {
                return DAC_setVoltage(VOLTAGE);
            }
            /// <summary>
            /// Задаёт напряжение на DAC
            /// <para>VOLTAGE - напряжение от 0 до 16383</para>
            /// <para>Возвращает:</para>
            /// <para>true - операция выполнена успешно</para>
            /// <para>false - операция отменена (значение VOLTAGE находтся не в диапазоне от 0 до 16383 или ошибка отклика)</para>
            /// </summary>
            /// <param name="VOLTAGE">Напряжение от 0 до 16383</param>
            /// <returns></returns>
            public bool setVoltage(string VOLTAGE)
            {
                return setVoltage(Convert.ToUInt16(VOLTAGE));
            }
            /// <summary>
            /// Задаёт напряжение на DAC
            /// <para>VOLTAGE - напряжение от 0 до 16383</para>
            /// <para>Возвращает:</para>
            /// <para>true - операция выполнена успешно</para>
            /// <para>false - операция отменена (значение VOLTAGE находтся не в диапазоне от 0 до 16383 или ошибка отклика)</para>
            /// </summary>
            /// <param name="VOLTAGE">Напряжение от 0 до 16383</param>
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
            const byte EmissionCurrent_channel =        1;
            const byte Ionization_channel =             2;
            const byte F1_channel =                     3;
            const byte F2_channel =                     4;
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
        }
        public class SPI_DETECTOR
        {
            //КЛАСС: Ионный источник - используется каналы А,B,C,D
            //DAC AD5328BR
            const byte DV1_channel = 1;
            const byte DV2_channel = 2;
            const byte DV3_channel = 3;
            //Каналы
            public SPI_DEVICE_CHANNEL DV1 = new SPI_DEVICE_CHANNEL(DV1_channel, Command.SPI.Detector.DV1.setVoltage, DV1_channel, Command.SPI.Detector.DV1.getVoltage);
            public SPI_DEVICE_CHANNEL DV2 = new SPI_DEVICE_CHANNEL(DV2_channel, Command.SPI.Detector.DV2.setVoltage, DV2_channel, Command.SPI.Detector.DV2.getVoltage);
            public SPI_DEVICE_CHANNEL DV3 = new SPI_DEVICE_CHANNEL(DV3_channel, Command.SPI.Detector.DV3.setVoltage, DV3_channel, Command.SPI.Detector.DV3.getVoltage);
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
                    string command = "Condensator.setVoltage(" + DAC_command + "," + DAC_channel + "," + VOLTAGE + ")";
                    if (RTC_is_busy)
                    {
                        trace("Отмена операции! Запрещено во время счёта!");
                        return false;
                    }
                    if (VOLTAGE >= 0 && VOLTAGE <= 16383)
                    {
                        trace_attached(Environment.NewLine);
                        trace(command + ": AD5643R;");
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
                            trace(command + ": Операция выполнена успешно!");
                            return true;
                        }
                        addError(" ! " + command + ": ОШИБКА ОТКЛИКА!");
                        return false;
                    }
                    trace(command + ": Операция отменена! Значение VOLTAGE ожидалось от 0 до 16383!");
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
                    string _command = "Scaner.getVoltage(" + command + ", " + CHANNEL + ")";
                    trace_attached(Environment.NewLine);
                    trace(_command);
                    trace(_command + ": DoubleRange = " + DoubleRange);
                    if (RTC_is_busy)
                    {
                        trace("Отмена операции! Запрещено во время счёта!");
                        return 6666;
                    }
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
                        addError(" ! " + _command + ":Ошибка при получении данных!");
                        return 16383;
                    }
                    trace(_command + ": Ответный адрес канала: " + adress);
                    trace(_command + ": Напряжение: " + voltage);
                    return voltage;
                }
                //Видимые функции
                /// <summary>
                /// Возвращает напряжение положительной обкладки конденсатора в диапазоне от 0 до 4095
                /// </summary>
                public ushort getVoltage()
                {
                    return ADC_getVoltage(ADC_command, ADC_channel);
                }
                //Видимые функции
                /// <summary>
                /// Задаёт напряжение на DAC
                /// <para>VOLTAGE - напряжение от 0 до 16383</para>
                /// <para>Возвращает:</para>
                /// <para>true - операция выполнена успешно</para>
                /// <para>false - операция отменена (значение VOLTAGE находтся не в диапазоне от 0 до 16383 или ошибка отклика)</para>
                /// </summary>
                /// <param name="VOLTAGE">Напряжение от 0 до 16383</param>
                /// <returns></returns>
                public bool setVoltage(ushort VOLTAGE)
                {
                    return DAC_setVoltage(VOLTAGE);
                }
                /// <summary>
                /// Задаёт напряжение на DAC
                /// <para>VOLTAGE - напряжение от 0 до 16383</para>
                /// <para>Возвращает:</para>
                /// <para>true - операция выполнена успешно</para>
                /// <para>false - операция отменена (значение VOLTAGE находтся не в диапазоне от 0 до 16383 или ошибка отклика)</para>
                /// </summary>
                /// <param name="VOLTAGE">Напряжение от 0 до 16383</param>
                /// <returns></returns>
                public bool setVoltage(string VOLTAGE)
                {
                    return setVoltage(Convert.ToUInt16(VOLTAGE));
                }
                /// <summary>
                /// Задаёт напряжение на DAC
                /// <para>VOLTAGE - напряжение от 0 до 16383</para>
                /// <para>Возвращает:</para>
                /// <para>true - операция выполнена успешно</para>
                /// <para>false - операция отменена (значение VOLTAGE находтся не в диапазоне от 0 до 16383 или ошибка отклика)</para>
                /// </summary>
                /// <param name="VOLTAGE">Напряжение от 0 до 16383</param>
                /// <returns></returns>
                public bool setVoltage(int VOLTAGE)
                {
                    return setVoltage(Convert.ToUInt16(VOLTAGE));
                }
            }
            //Каналы
            /// <summary>
            /// Дополнительное сканирующее напряжение
            /// </summary>
            public SPI_DEVICE_CHANNEL_withAD5643R ParentScan = new SPI_DEVICE_CHANNEL_withAD5643R(DAC_ParentScan_Channel, Command.SPI.Scaner.ParentScan.setVoltage, ADC_ParentScan_Channel, Command.SPI.Scaner.ParentScan.getVoltage);
            /// <summary>
            /// Сканирующее напряжение
            /// </summary>
            public SPI_DEVICE_CHANNEL_withAD5643R Scan = new SPI_DEVICE_CHANNEL_withAD5643R(DAC_Scan_Channel, Command.SPI.Scaner.Scan.setVoltage, ADC_Scan_Channel, Command.SPI.Scaner.Scan.getVoltage);
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
                string command = "Chip.checkCommandStack()";
                trace_attached(Environment.NewLine);
                trace(command);
                if (transmit(Command.TEST.checkCommandStack)[0] == CommandStack)
                {
                    trace(command = ": Команды идут синхронно. (" + CommandStack + ")");
                    return true;
                }
                trace(command + ": Команды идут НЕ синхронно! (" + CommandStack + ")");
                return false;
            }
            /// <summary>
            /// Возвращает код статуса МК (отладочное)
            /// </summary>
            public byte getStatus()
            {
                //ФУНКЦИЯ: Возвращает статус самого микроконтроллера
                string command = "Chip.getStatus()";
                trace_attached(Environment.NewLine);
                trace(command);
                byte answer = transmit(Command.Chip.getStatus)[0];
                trace(command + ": Статус МК: " + answer);
                return answer;
            }
            /// <summary>
            /// Возвращает версию прошивки МК
            /// </summary>
            public byte getVersion()
            {
                //ФУНКЦИЯ: Возвращает версию прошивки микроконтроллера
                string command = "Chip.getVersion()";
                trace_attached(Environment.NewLine);
                trace(command);
                byte answer = transmit(Command.Chip.getVersion)[0];
                trace(command + ": Версия прошивки МК: " + answer);
                return answer;
            }
            /// <summary>
            /// Возвращает дату создания прошивки МК 
            /// </summary>
            public string getBirthday()
            {
                //ФУНКЦИЯ: Возвращает дату создания прошивки микроконтроллера
                string command = "Chip.getBirthday()";
                trace_attached(Environment.NewLine);
                trace(command);
                UInt32 birthday = 0;
                string answer = "00000000";
                byte[] recDATA = transmit(Command.Chip.getBirthday);
                try
                {
                    birthday = Convert.ToUInt32(recDATA[3]) * 16777216 + Convert.ToUInt32(recDATA[2]) * 65536 + Convert.ToUInt32(recDATA[1]) * 256 + Convert.ToUInt32(recDATA[0]);
                }
                catch (Exception)
                {
                    addError(" ! " + command + ": Ошибка при получении данных!");
                    return answer;
                }
                answer = birthday.ToString();
                answer = answer[6] + "" + answer[7] + " " + answer[4] + answer[5] + " " + answer[0] + answer[1] + answer[2] + answer[3];
                trace(command + ": Дата создания прошивки МК: " + answer);
                return answer;
            }
            /// <summary>
            /// Возвращает частоту процессора МК (не вычисляется) 
            /// </summary>
            public string getCPUfrequency()
            {
                //ФУНКЦИЯ: Возвращает частоту процессора микроконтроллера (не вычисляется)
                string command = "Chip.getCPUfrequency()";
                trace_attached(Environment.NewLine);
                trace(command);
                UInt32 frequency = 0;
                byte[] recDATA = transmit(Command.Chip.getCPUfrequency);
                try
                {
                    frequency = Convert.ToUInt32(recDATA[3]) * 16777216 + Convert.ToUInt32(recDATA[2]) * 65536 + Convert.ToUInt32(recDATA[1]) * 256 + Convert.ToUInt32(recDATA[0]);
                }
                catch (Exception)
                {
                    addError(" ! " + command + ": Ошибка при получении данных!");
                    return "!";
                }
                trace(command + ": Частота процессора: " + frequency.ToString() + " Гц");
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
            /// Передаём на МК данные (внутри функции), которые он пересылает на TIC контроллер вакуумного насоса (В РАЗРАБОТКЕ...)
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
        /// Натекатель
        /// </summary>
        public SPI_DEVICE_CHANNEL Inlet = new SPI_DEVICE_CHANNEL(1, Command.SPI.Inlet.setVoltage, 1, Command.SPI.Inlet.getVoltage);
        /// <summary>
        /// Нагреватель
        /// </summary>
        public SPI_DEVICE_CHANNEL Heater = new SPI_DEVICE_CHANNEL(2, Command.SPI.Heater.setVoltage, 2, Command.SPI.Heater.getVoltage);
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
        /// МК устанавливает и возвращает флаги в порядке:
        /// <para>|x|x|iHVE|iEDCD|SEMV1|SEMV2|SEMV3|SPUMP|</para> 
        /// <para>,где SPUM - первый(нулевой) бит</para>
        /// <para>     x - не имеет значения</para>
        /// </summary>
        /// <param name="set_flags">true - установить следующие флаги, <para>false - не устанавливать, только проверить (последующие флаги не имеют значения)</para></param>
        /// <param name="HVE">Разрешение высокого напряжения (впоследствии будет заменено на PRGE)</param>
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
            string command = "setFlags( set = " + Convert.ToInt16(set_flags) + " | " + 0 + " | " + Convert.ToInt16(HVE) + " | EDCD = " + Convert.ToInt16(EDCD) + " | SEMV1 = " + Convert.ToInt16(SEMV1) + " | SEMV2 = " + Convert.ToInt16(SEMV2) + " | SEMV3 = " + Convert.ToInt16(SEMV3) + " | SPUMP = " + Convert.ToInt16(SPUMP) + ")";
            trace_attached(Environment.NewLine);
            trace(command);
            byte flags = Convert.ToByte(Convert.ToInt16(set_flags) * 128 + Convert.ToInt16(HVE) * 32 + Convert.ToInt16(EDCD) * 16 + Convert.ToInt16(SEMV1) * 8 + Convert.ToInt16(SEMV2) * 4 + Convert.ToInt16(SEMV3) * 2 + Convert.ToInt16(SPUMP));
            byte received_flags = transmit(Command.setFlags, flags)[0];
            if ((received_flags & 64) == 64) { trace(command + ": Операция отменена! Нечего менять!"); }
            return received_flags;
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
        static List<byte> receive_2(byte rPacketLength, bool closePort)
        {
            List<byte> rDATA = new List<byte>();
            if (!USART.IsOpen)
            {
                USART.Open();
            }
            for (byte i = 0; i < rPacketLength; i++)
            {
                while (USART.BytesToRead == 0) { }
                rDATA.Add((byte)USART.ReadByte());
            }
            if (closePort && USART.IsOpen)
            {
                USART.Close();
            }
            return rDATA;
        }
        static List<byte> decode_2(List<byte> DATA)
        {
            List<byte> rDATA = DATA;
            if (rDATA.First<byte>() == Command.KEY)
            {
                rDATA.RemoveAt(0);                              //Удаляем ключ
                if (rDATA.First<byte>() == rDATA.Count + 1)
                {
                    rDATA.RemoveAt(0);                          //Удаляем длинну пакета
                    if (rDATA.Last<byte>() == Command.LOCK)
                    {
                        rDATA.RemoveAt(rDATA.Count - 1);        //Удаляем затвор
                        byte CheckSum = rDATA.Last<byte>();
                        rDATA.RemoveAt(rDATA.Count - 1);        //Удаляем контрольную сумму
                        byte calcedCheckSum = calcCheckSum(rDATA.ToArray());
                        if (CheckSum == calcedCheckSum)
                        {
                            //Пакет верный, возвращаем его
                            return rDATA;
                        }
                        else
                        {
                            trace("ОШИБКА ПРИЁМА! Неверная контрольная сумма. Получено:" + CheckSum + " Подсчитано: " + calcedCheckSum);
                            return new List<byte>();
                        }
                    }
                    else
                    {
                        trace("ОШИБКА ПРИЁМА! Не был получен затвор. Получено:" + rDATA.Last<byte>());
                        return new List<byte>();
                    }
                }
                else
                {
                    trace("ОШИБКА ПРИЁМА! Неверная длина пакета. Получено:" + rDATA.First<byte>());
                    return new List<byte>();
                }
            }
            else
            {
                trace("ОШИБКА ПРИЁМА! Не был получен ключ. Получено:" + rDATA.First<byte>());
                return new List<byte>();
            }
        }
        static void send_2(List<byte> DATA, bool closePort)
        {
            List<byte> Packet = new List<byte>();
            Packet.Add(Command.KEY);
            Packet.Add((byte)(DATA.Count + 4));
            Packet.AddRange(DATA);
            Packet.Add(calcCheckSum(DATA.ToArray()));
            Packet.Add(Command.LOCK);
            //Открытие порта, если он закрыт
            if (!USART.IsOpen)
            {
                USART.Open();
            }
            //Передача
            USART.Write(Packet.ToArray(), 0, Packet.Count);
            if (closePort && USART.IsOpen)
            {
                USART.Close();
            }
        }
        static List<byte> transmit_2(List<byte> wDATA, byte rPacketLength, bool closePort)
        {
            send_2(wDATA, false);
            return decode_2(receive_2(rPacketLength, closePort));
            //Декодируем
        }
        static byte calcCheckSum(byte[] data)
        {
            //ФУНКЦИЯ: Вычисление контрольной суммы для верификации данных
            byte CheckSum = 0;
            for (int i = 0; i < data.Length; i++)
            {
                CheckSum -= data[i];
            }
            //trace("             Контрольная сумма: " + CheckSum);
            return CheckSum;
        }
        static byte[] transmit(List<byte> DATA)
        {
            //ФУНКЦИЯ: Формируем пакет, передаём его МК, слушаем ответ, возвращаем ответ.
            bool tracer_enabled_before = tracer_enabled;    //сохраняем параметры трэйсера
            tracer_enabled = tracer_transmit_enabled;       //Если надо выключаем трэйс в трансмите
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
            //Формируем пакет по типу:  ':<Length><data><CS>\r' 
            Packet.Add(Command.KEY);                        //':' - Начало данных 
            Packet.Add((byte)(DATA.Count + 4));             //'Length' - длинна пакета
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
                addError("СОМ-порт не определён!");
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
                addError(" ! ОШИБКА ПРИЁМА ДАННЫХ! Нет данных на приём!");
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
                    addError(" ! ОШИБКА ПРИЁМА ДАННЫХ! Приём не удался!");
                    USART.Close();
                    tracer_enabled = tracer_enabled_before;
                    return new byte[] {0};
                }
                rDATA.Add(rBYTE);
            }
            USART.Close();
            trace("         Приём завершён!");
            trace("         Анализ полученной команды...");
            //Если последний байт затвор, то всё путём
            if (rDATA.First<byte>() == Command.KEY)
            {
                rDATA.RemoveAt(0);                                              //Удаляем ключ
                if (rDATA.First<byte>() == rDATA.Count + 1)
                {
                    rDATA.RemoveAt(0);                                          //Удаляем длину пакета
                    if (rDATA.Last<byte>() == Command.LOCK)
                    {
                        rDATA.RemoveAt(rDATA.Count - 1);                        //Удаляем затвор
                        //Анализируем полученные данные
                        byte rCheckSum = rDATA.Last();                          //Полученная КС
                        rDATA.RemoveAt(rDATA.Count - 1);                        //Убираем КС
                        byte CheckSum = calcCheckSum(rDATA.ToArray());          //Подсчитанная КС
                        if (CheckSum != rCheckSum)
                        {
                            addError("             Несовпадает контрольная сумма! Получено: " + rCheckSum + " Подсчитано: " + CheckSum);
                            tracer_enabled = tracer_enabled_before;
                            return new byte[] { 0 };
                        }
                        //Проверяем данные на отклик
                        if (rDATA[0] == command)
                        {
                            trace("             Отклик: " + rDATA[0]);
                            if (BytesToReadQuantity > 5)
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
                            //addError(" ! ОШИБКА ОТКЛИКА! Отклик не совпадает! Ожидался отклик: " + command, rDATA.ToArray());
                            trace("Отклик не совпадает! Значит МК сообщает об ошибке!");
                            trace("    Расшифровка сообщения об ошибке...");
                            defineError(rDATA.ToArray());
                            tracer_enabled = tracer_enabled_before;
                            return new byte[] { 0 };
                        }
                        tracer_enabled = tracer_enabled_before;
                        return rDATA.ToArray();
                    }
                    addError(" ! ОШИБКА ПРИЁМА ДАННЫХ! Не был получен затвор!", rDATA.ToArray());
                    tracer_enabled = tracer_enabled_before;
                    return new byte[] { 0 };
                }
                addError(" ! ОШИБКА ПРИЁМА ДАННЫХ! Не верная длина пакета!", rDATA.ToArray());
                tracer_enabled = tracer_enabled_before;
                return new byte[] { 0 };
            }
            addError(" ! ОШИБКА ПРИЁМА ДАННЫХ! Не был получен ключ!", rDATA.ToArray());
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
        static bool defineError(byte[] DATA)
        {
            //ФУНКЦИЯ: Дешефрирует ошибку
            trace("---------------ОШИБКА МК-------------");
            trace("Принятые данные:");
            foreach (byte b in DATA)
            {
                
                trace("     " + b);
            }
            if (DATA.Length > 2)
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
                                        addError(" !! МК СООБЩАЕТ ОБ ОШИБКЕ ДЕКОДЕРА! Неизвестная команда: " + DATA[2], DATA);
                                        break;
                                    case Error.KeyError:
                                        addError(" !! МК СООБЩАЕТ ОБ ОШИБКЕ! НЕ БЫЛ ПОЛУЧЕН КЛЮЧ! Ключ: " + DATA[2], DATA);
                                        break;
                                    case Error.LockError:
                                        addError(" !! МК СООБЩАЕТ ОБ ОШИБКЕ! НЕ БЫЛ ПОЛУЧЕН ЗАМОК! Замок: " + DATA[2], DATA);
                                        break;
                                    case Error.CheckSumError:
                                        addError(" !! МК СООБЩАЕТ ОБ ОШИБКЕ! Неверная контрольная сумма: " + DATA[2], DATA);
                                        break;
                                    default:
                                        addError(" !!! МК сообщает о неизвестной ОШИБКЕ № " + DATA[1] + "!", DATA);
                                        trace("--------------Конец ошибки---------------");
                                        return false;
                                }
                                trace("--------------Конец ошибки---------------");
                                return true;
                            }
                            else
                            {
                                addError(" !!! НЕИЗВЕСТНАЯ ОШИБКА!", DATA);
                                trace("--------------Конец ошибки---------------");
                                return false;
                            }
                    }
                }
                else
                {
                    addError(" !!! Неверное сообщение об ошибке! Отсутствует метка ошибки!", DATA);
                    trace("--------------Конец ошибки---------------");
                    return false;
                }
            }
            else
            {
                addError(" !!! Слишком короткое сообщение об ошибке!", DATA);
                trace("--------------Конец ошибки---------------");
                return false;
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
            string error = TEXT + Environment.NewLine + " Получено: ";
            foreach(byte b in rDATA)
            {
                error += b + " | ";
            }
            trace(error);
            ErrorList.Add(" -" + (ErrorList.Count + 1).ToString() + "- [" + DateTime.Now.ToString("HH:mm:ss") + "] " + error);
        }
        static void addError(string TEXT, int rDATA)
        {
            string error = " -" + (ErrorList.Count + 1).ToString() + "- [" + DateTime.Now.ToString("HH:mm:ss") + "] " + TEXT + Environment.NewLine + " Получено: " + rDATA;
            trace(TEXT + Environment.NewLine + "    Получено: " + rDATA);
            ErrorList.Add(error);
        }
        static void addError(string TEXT)
        {
            string error = " -" + (ErrorList.Count + 1).ToString() + "- [" + DateTime.Now.ToString("HH:mm:ss") + "] " + TEXT;
            trace(TEXT);
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
