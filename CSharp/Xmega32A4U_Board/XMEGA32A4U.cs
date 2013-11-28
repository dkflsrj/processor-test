using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace Xmega32A4U_testBoard
{
    public delegate void EventCallBack();
    /// <summary>
    /// Класс связи ПК с ATXmega32A4U
    /// </summary>
    class XMEGA32A4U
    {
        static EventCallBack _MeasureEnd;
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
        const uint TimeOut = 1625; //260000 вроде 1 сек
        //-------------------------------------СТРУКТУРЫ------------------------------------------
        struct Error
        {
            //СТРУКТУРА: Хранилище констант - кодовобычных ошибок, которые не ставят под угрозу работу системы.
            public const byte Token =           255;    //Есть ошибка
            //ErrorNums...
            public const byte Decoder =         1;       //Такое команды не существует
            public const byte CheckSum =        2;       //Неверная контрольная сумма
            //public const byte wrong_SPI_DEVICE_Number
        }
        struct LAM
        {
            //СТРУКТУРА: Хранилище констант - кодов асинхронных сообщений информирующие ПК о чём либо.
            public const byte Tocken =          254;      //Метка LAM сообщения
            //Номера асинхронных сообщений (обрати внимание)
            public const byte RTC_end =         1;      //RTC закончил измерение
        }
        struct Internal_Error
        {
            //СТРУКТУРА: Хранилище констант - кодов внутренних ошибок МК. Нежелательные и запрещённые состояния.
            public const byte Token =           253;      //Метка внутренней ошибки
            //Номера внутренних ошибок     
            public const byte USART_COMP =      1;       //Внутренняя ошибка приёма данных от ПК
            public const byte SPI =             2;       //SPI-устройства с таким номером нет  
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
                public const byte receiveResults =      32;
            }
            public struct TIC
            {
                //Коды команд для микроконтроллера насоса
                public const byte sendToTIC = 50;
            }
            public struct SPI
            {
                //Коды команд для SPI устройств
                public struct PSIS
                {
                    //Коды команд Ионного Источника
                    
                    public const byte setVoltage = 40;
                    public const byte getVoltage = 60;
                    
                }
                public struct DPS
                {
                    //Коды команд Детектора
                    public const byte setVoltage = 41;
                    public const byte getVoltage = 61;
                }
                public struct PSInl
                {
                    //Коды команд Натекателя
                    public const byte setVoltage = 44;
                    public const byte getVoltage = 63;
                }
                public struct Scaner
                {
                    //Коды команд Сканера
                    public const byte setVoltage = 42;
                    public const byte getVoltage = 62;
                    
                }
                public struct Condensator
                {
                    //Коды команд Конденсатора
                    public const byte setVoltage = 43;
                    public const byte getVoltage = 62;
                }
            }
            public const byte LOCK = 13;
            public const byte KEY = 58;
            public struct TEST
            {
                //Коды команд отладки
                public const byte checkCommandStack = 8;
            }
            public const byte setFlags = 70;
        }
        struct Incident
        {
            //СТРУКТУРА: Хранилище констант - кодов происшествий, которые передаются при опросе статуса МК
            public const byte LOCKisLost =      2;  //МК принимал пакет, но в последний байт пакета не затвор
            public const byte TooShortPacket =  4;  //байт длинны пакета меньше минимального (5)
            public const byte TooFast =         8;  //МК ещё не выполнил предыдущую команду, а уже приходит другая.
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
                    public const byte ready =               0;		//Счётчики готов к работе
                    public const string string_ready =      "ready";
                    public const byte stopped =             1;		//Счётчики был принудительно остановлен
                    public const string string_stopped =    "stopped";
                    public const byte busy =                2;		//Счётчики ещё считает
                    public const string string_busy =       "busy";
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
                public const byte LengthOfstartMeasurePacket =    6;
                public struct NextMeasure
                {
                    //Коды команд счётчиков
                    public const byte NotDo =               0;
                    public const byte Do =                  1;
                }

            }
            /// <summary>
            /// Событие: счётчики закончили счёт.
            /// </summary>
            public EventCallBack MeasureEnd
            {
                set { _MeasureEnd = value; }
                get { return _MeasureEnd; }
            }
            public class counter
            {
                //КЛАСС: Счётчик. Только хранит значения.
                /// <summary>
                /// Количество переполнений счётчика
                /// </summary>
                public byte Overflows = 0;  //Количество переполений счётчика
                /// <summary>
                /// Сосчитанный результат
                /// </summary>
                public uint Count = 0;   //Сосчитанный результат
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
            
            
            public string Status = "notSet";

            string convertStatus(byte Status_byte)
            {
                //ФУНКЦИЯ: Переводит числовой байт статуса в строку
                string Status_string = "";
                switch(Status_byte)
                {
                    case Constants.Status.ready:
                        Status_string = Constants.Status.string_ready;
                        break;
                    case Constants.Status.stopped:
                        Status_string = Constants.Status.string_stopped;
                        break;
                    case Constants.Status.busy:
                        Status_string = Constants.Status.string_busy;
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
            public byte calcRTCprescaler(uint MILLISECONDS)
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
                    trace("Counters.calcRTCprescaler(" + MILLISECONDS + "):Неверное значение! Ожидалось: 50...2047925 мс. Получено: " + MILLISECONDS);
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
            public ushort calcRTCprescaler_long(uint MILLISECONDS)
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
                    trace("Counters.calcRTCprescaler_long(" + MILLISECONDS + "):Неверное значение! Ожидалось: 50...2047925 мс. Получено: " + MILLISECONDS);
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
            public ushort calcRTCprescaler_long(string MILLISECONDS)
            {
                try { return calcRTCprescaler_long(Convert.ToUInt32(MILLISECONDS)); }
                catch { trace("Counters.calcRTCprescaler_long(" + MILLISECONDS + "):Неверное значение!"); return ushort.MaxValue; };
            }
            /// <summary>
            /// Возвращает частоту RTC в соответствии с предделителем в длинном формате
            /// <para>ПРИМЕЧАНИЕ: без участия МК</para>
            /// </summary>
            public double calcRTCfrequency(uint MILLISECONDS)
            {
                //ФУНКЦИЯ: Возвращает итоговую частоту RTC в соответствии с сохраннённым предделителем.
                return (Constants.sourceFrequency / calcRTCprescaler_long(MILLISECONDS));
            }
            /// <summary>
            /// Возвращает частоту RTC в соответствии с предделителем в длинном формате
            /// <para>ПРИМЕЧАНИЕ: без участия МК</para>
            /// </summary>
            public double calcRTCfrequency(string MILLISECONDS)
            {
                //ФУНКЦИЯ: Возвращает итоговую частоту RTC в соответствии с сохраннённым предделителем.
                return (Constants.sourceFrequency / calcRTCprescaler_long(MILLISECONDS));
            }
            /// <summary>
            /// Возвращает количество тиков RTC для отсчёта заданного времени с заданным предделителем 
            /// <para>ПРИМЕЧАНИЕ: без участия МК</para>
            /// </summary>
            /// <param name="MeasureTime_ms">Время измерения в миллисекундах от 0 до 2047925</param>
            /// <param name="PRESCALER_long">Предделитель. Возможные значения: 1,2,8,16,64,256,1024</param>
            /// <returns></returns>
            public ushort calcRTCticks(uint MILLISECONDS)
            {
                //ФУНКЦИЯ: Вычисляет количество тиков в соответствии с временем и предделителем. Возвращает количество тиков
                ushort tiks;
                if ((MILLISECONDS < 50)||(calcRTCprescaler(MILLISECONDS) == 0))
                { 
                    trace("Counters.calcRTCticks(" + MILLISECONDS + ")" + ": Отмена операции! Неверное значение! Ожидалось: 50...2047925 мс. Получено: " + MILLISECONDS); 
                    return ushort.MaxValue; 
                }
                tiks = Convert.ToUInt16(Math.Round(Convert.ToDouble(MILLISECONDS) * (Constants.sourceFrequency / calcRTCprescaler_long(MILLISECONDS))));
                return tiks;
            }
            /// <summary>
            /// Возвращает количество тиков RTC для отсчёта заданного времени с заданным предделителем 
            /// <para>ПРИМЕЧАНИЕ: без участия МК</para>
            /// </summary>
            /// <param name="MeasureTime_ms">Время измерения в миллисекундах от 0 до 2047925</param>
            /// <param name="PRESCALER_long">Предделитель. Возможные значения: 1,2,8,16,64,256,1024</param>
            /// <returns></returns>
            public ushort calcRTCticks(string MILLISECONDS)
            {
                try { return calcRTCticks(Convert.ToUInt32(MILLISECONDS)); }
                catch { trace("Counters.calcRTCticks(" + MILLISECONDS + "):Неверное значение!"); return ushort.MaxValue; };
            }
            /// <summary>
            /// Запускает измерение длительностью MILLISECONDS
            /// <para>По окончанию измерения МК пришлёт асинхронное LookAtMe сообщение </para>
            /// <para>, которое вызовет EventCallBack MeasureEnd.  </para>
            /// <para>Возвращает:</para>
            /// <para>true - счётчики начали счёт.</para>
            /// <para>false - операция отменена.</para>
            /// <param name="MeasureTime_ms">Время измерения в миллисекундах от 50 до 2047925</param>
            /// </summary>
            public bool startMeasure(uint MILLISECONDS)
            {
                //ФУНКЦИЯ: Запускаем счётчик, возвращает true если счёт начался, false - счётчик уже считает
                trace_attached(Environment.NewLine);
                string command = "Counters.startMeasure()";
                trace(command);
                //Проверка диапазона заданного интервала измерения
                if ((MILLISECONDS < 50)||(calcRTCprescaler(MILLISECONDS) == 0))
                {
                    trace(command + ": Отмена операции! Неверное значение! Ожидалось: 50...2047925 мс. Получено: " + MILLISECONDS); 
                    return false; 
                }
                //Очистка
                COA.Count = 0;
                COA.Overflows = 0;
                COB.Count = 0;
                COB.Overflows = 0;
                COC.Count = 0;
                COC.Overflows = 0;
                List<byte> wDATA = new List<byte>();    //Данные на передачу
                List<byte> rDATA;
                //Вычисление байт для периода RTC
                byte[]  MeasurePeriod = BitConverter.GetBytes(calcRTCticks(MILLISECONDS));
                //Набираем данные на отправку
                wDATA.Add(Command.RTC.startMeasure); //Команда
                wDATA.Add(calcRTCprescaler(MILLISECONDS));   //Предделитель
                wDATA.Add(MeasurePeriod[1]);
                wDATA.Add(MeasurePeriod[0]);
                //Посылаем команду
                rDATA = transmit(wDATA); 
                try 
                { 
                    if (rDATA[0] != Command.RTC.startMeasure)
                    {
                        addError(command + ": Ошибка отклика!", rDATA);
                        return false;
                    }
                    Status = convertStatus(rDATA[1]);
                }
                catch { addError(command + ": Ошибка данных!", rDATA); return false; }
                if ((Status == Constants.Status.string_ready) || (Status == Constants.Status.string_stopped))
                { trace(command + ": Счётчики начали счёт."); return true; }
                trace(command + ": Операция отменена! Счётчики уже считают!"); return false;
            }
            /// <summary>
            /// Запускает измерение длительностью MILLISECONDS
            /// <para>По окончанию измерения МК пришлёт асинхронное LookAtMe сообщение </para>
            /// <para>, которое вызовет EventCallBack MeasureEnd.  </para>
            /// <para>Возвращает:</para>
            /// <para>true - счётчики начали счёт.</para>
            /// <para>false - операция отменена.</para>
            /// <param name="MeasureTime_ms">Время измерения в миллисекундах от 50 до 2047925</param>
            /// </summary>
            public bool startMeasure(string MILLISECONDS)
            {
                try { return startMeasure(Convert.ToUInt32(MILLISECONDS)); }
                catch { trace("Counters.startMeasure(" + MILLISECONDS + "):Неверное значение!"); return false; };
            }
            /// <summary>
            /// Останавливает серию.
            ///<para>Возвращает:</para>
            ///<para>true - операция выполнена успешно.</para>
            ///<para>false - операция была отменена.</para>
            /// </summary>
            public bool stopMeasure()
            {
                //ФУНКЦИЯ: Останавливает счётчик. Возвращает true, если операция удалась. false - не удалась (счётчик не считает)
                string command = "Counters.stopMeasure()";
                trace_attached(Environment.NewLine);
                trace(command);
                List<byte> rDATA = transmit(Command.RTC.stopMeasure); 
                try 
                { 
                    if (rDATA[0] != Command.RTC.stopMeasure)
                    {
                        addError(command + ": Ошибка отклика!", rDATA);
                        return false;
                    }
                    Status = convertStatus(rDATA[1]);
                }
                catch { addError(command + ": Ошибка данных!", rDATA); return false; }
                switch (Status)
                {
                    case Constants.Status.string_busy:
                        trace(command + ": Операция выполнена успешно!");
                        return true;
                    case Constants.Status.string_ready:
                        trace(command + ": Операция отменена! Счётчики не считают!");
                        return false;
                    case Constants.Status.string_stopped:
                        trace(command + ": Операция отменена! Счётчики уже остановлены!");
                        return false;
                    default:
                        addError(command + ": ОШИБКА МК! НЕИЗВЕСТНОЕ СОСТОЯНИЕ СЧЁТЧИКОВ: " + Status);
                        return false;
                }
            }
            /// <summary>
            /// Запрашиваем результаты счёта у МК
            /// <para> Возвращает:</para>
            /// <para>  true - операция удалась. Результаты записаны в COA, COB и COC.</para>
            /// <para>  false - операция отменена. Вероятно счётчики всё ещё считают.</para>
            /// </summary>
            public bool receiveResults()
            {
                //ФУНКЦИЯ: Запрашиваем результаты измерения. Если измерение завершилось успешно нам пришлют результаты и мы выйдем с true, в противном случае пришлют статус (не ready) и мы вылетим с false'м
                //ДАННЫЕ: <Command><RTC_Status><COA_OVF><COA_M[3]><COA_M[2]><COA_M[1]><COA_M[0]><COВ_OVF><COВ_M[3]><COВ_M[2]><COВ_M[1]><COВ_M[0]><COС_OVF><COС_M[1]><COС_M[0]>
                trace_attached(Environment.NewLine);
                string command = "Counters.receiveResults()";
                trace(command);
                List<byte> rDATA;                       //Данные на приём
                rDATA = transmit(Command.RTC.receiveResults);
                try
                {
                    if (rDATA[0] != Command.RTC.receiveResults)
                    {
                        addError(command + ": Ошибка отклика!", rDATA);
                        return false;
                    }
                    Status = convertStatus(rDATA[1]);
                    if (Status == Constants.Status.string_ready)
                    {
                        COA.Overflows = rDATA[2];
                        COA.Count = (uint)(rDATA[3] * 16777216 + rDATA[4] * 65536 + rDATA[5] * 256 + rDATA[6]);
                        COB.Overflows = rDATA[7];
                        COB.Count = (uint)(rDATA[8] * 16777216 + rDATA[9] * 65536 + rDATA[10] * 256 + rDATA[11]);
                        COC.Overflows = rDATA[12];
                        COC.Count = (uint)(rDATA[13] * 256 + rDATA[14]);
                        trace(command + ":Результаты успешно получены!");
                        return true;
                    }
                }
                catch { addError(command + ": Ошибка данных!", rDATA); }
                return false;
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
                if (VOLTAGE >= 0 && VOLTAGE <= 4095)
                {
                    trace_attached(Environment.NewLine);
                    trace(_command);
                    byte[] bytes = BitConverter.GetBytes(VOLTAGE);
                    byte[] data = { Convert.ToByte((CHANNEL - 1) * 16 + bytes[1]), bytes[0] };
                    List<byte> rDATA = transmit(command, data);
                    try
                    {
                        if (rDATA[0] == DAC_command)
                        {
                            trace(_command + ": Операция выполнена успешно!");
                            return true;
                        } 
                    }
                    catch { addError(_command + ": Ошибка данных!", rDATA); return false; }
                    addError(_command + ": ОШИБКА ОТКЛИКА!");
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
                try { return setVoltage(Convert.ToUInt16(VOLTAGE)); }
                catch { trace("DAC_CHANNEL.setVoltage():Неверное значение!"); return false; }
                
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
                try { return setVoltage(Convert.ToUInt16(VOLTAGE)); }
                catch { trace("DAC_CHANNEL.setVoltage():Неверное значение!"); return false; }
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
            const bool DoubleRange = true;
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
                byte[] data = { Convert.ToByte(Hbyte + ChannelStep * CHANNEL), Lbyte_DoubleRange };
                List<byte> rDATA = transmit(command, data);
                ushort voltage = 0;
                byte adress = 1;
                try
                {
                    if (rDATA[0] != command)
                    {
                        addError(_command + ": Ошибка отклика!", rDATA);
                        return ushort.MaxValue;
                    }
                    adress += Convert.ToByte(rDATA[1] >> 4);
                    voltage = Convert.ToUInt16((Convert.ToUInt16(rDATA[1] & 0xf) << 8) + rDATA[2]);
                }
                catch { addError(_command + ": Ошибка данных!", rDATA); return ushort.MaxValue; }
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
                    List<byte> rDATA = transmit(Command.SPI.Condensator.setVoltage, data);
                    try
                    {
                        if (rDATA[0] == Command.SPI.Condensator.setVoltage)
                        {
                            trace(command + ": Операция выполнена успешно!");
                            return true;
                        }
                        addError(command + ": ОШИБКА ОТКЛИКА!");
                        return false;
                    }
                    catch { addError(command + ": Ошибка данных!", rDATA); return false; }
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
            const bool DoubleRange = true;
            ushort ADC_getVoltage(byte command, byte CHANNEL)
            {
                string _command = ".Condensator.getVoltage(" + command + ", " + CHANNEL + ")";
                trace_attached(Environment.NewLine);

                byte[] data = { Convert.ToByte(ADC_Hbyte + ChannelStep * CHANNEL), ADC_Lbyte_DoubleRange };
                List<byte> rDATA = transmit(command, data);
                ushort voltage = 0;
                byte adress = 0;
                try
                {
                    if (rDATA[0] != command)
                    {
                        addError(_command + ": Ошибка отклика!", rDATA);
                        return ushort.MaxValue;
                    }
                    adress += Convert.ToByte(rDATA[1] >> 4);
                    voltage = Convert.ToUInt16((Convert.ToUInt16(rDATA[1] & 0xf) << 8) + rDATA[2]);
                }
                catch { addError(command + ": Ошибка данных!", rDATA); return ushort.MaxValue; }
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
                return ADC_getVoltage(Command.SPI.Condensator.getVoltage, ADC_positive_channel);
            }
            /// <summary>
            /// Возвращает напряжение отрицательной обкладки конденсатора в диапазоне от 0 до 4095.
            /// </summary>
            public ushort getNegativeVoltage()
            {
                return ADC_getVoltage(Command.SPI.Condensator.getVoltage, ADC_negative_channel);
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
                try { return setVoltage(Convert.ToUInt16(VOLTAGE)); }
                catch { trace("DAC_CHANNEL.setVoltage():Неверное значение!"); return false; }
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
                try { return setVoltage(Convert.ToUInt16(VOLTAGE)); }
                catch { trace("DAC_CHANNEL.setVoltage():Неверное значение!"); return false; }
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
            public SPI_DEVICE_CHANNEL EmissionCurrent = new SPI_DEVICE_CHANNEL(EmissionCurrent_channel, Command.SPI.PSIS.setVoltage, EmissionCurrent_channel,Command.SPI.PSIS.getVoltage);
            /// <summary>
            /// Напряжение ионизации
            /// </summary>
            public SPI_DEVICE_CHANNEL Ionization = new SPI_DEVICE_CHANNEL(Ionization_channel, Command.SPI.PSIS.setVoltage, Ionization_channel, Command.SPI.PSIS.getVoltage);
            /// <summary>
            /// Фокусирующее напряжение 1 
            /// </summary>
            public SPI_DEVICE_CHANNEL F1 = new SPI_DEVICE_CHANNEL(F1_channel, Command.SPI.PSIS.setVoltage, F1_channel, Command.SPI.PSIS.getVoltage);
            /// <summary>
            /// Фокусирующее напряжение 2
            /// </summary>
            public SPI_DEVICE_CHANNEL F2 = new SPI_DEVICE_CHANNEL(F2_channel, Command.SPI.PSIS.setVoltage, F2_channel, Command.SPI.PSIS.getVoltage);
        }
        public class SPI_DETECTOR
        {
            //КЛАСС: Ионный источник - используется каналы А,B,C,D
            //DAC AD5328BR
            const byte DV1_channel = 1;
            const byte DV2_channel = 2;
            const byte DV3_channel = 3;
            //Каналы
            public SPI_DEVICE_CHANNEL DV1 = new SPI_DEVICE_CHANNEL(DV1_channel, Command.SPI.DPS.setVoltage, DV1_channel, Command.SPI.DPS.getVoltage);
            public SPI_DEVICE_CHANNEL DV2 = new SPI_DEVICE_CHANNEL(DV2_channel, Command.SPI.DPS.setVoltage, DV2_channel, Command.SPI.DPS.getVoltage);
            public SPI_DEVICE_CHANNEL DV3 = new SPI_DEVICE_CHANNEL(DV3_channel, Command.SPI.DPS.setVoltage, DV3_channel, Command.SPI.DPS.getVoltage);
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
                        List<byte> rDATA = transmit(DAC_command, data);
                        try
                        {
                            if (rDATA[0] == DAC_command)
                            {
                                trace(command + ": Операция выполнена успешно!");
                                return true;
                            }
                            addError(command + ": ОШИБКА ОТКЛИКА!");
                            return false;
                        }
                        catch { addError(command + ": Ошибка данных!", rDATA); return false; }
                    }
                    trace(command + ": Операция отменена! Значение VOLTAGE ожидалось от 0 до 16383!");
                    return false;
                }
                //ADC
                const byte ADC_Hbyte = 131;
                const byte ADC_Lbyte_DoubleRange = 16;
                const byte ADC_Lbyte_NormalRange = 48;
                const byte ChannelStep = 4;
                const bool DoubleRange = true;
                ushort ADC_getVoltage(byte command, byte CHANNEL)
                {
                    string _command = "Scaner.getVoltage(" + command + ", " + CHANNEL + ")";
                    trace_attached(Environment.NewLine);
                    trace(_command);

                    byte[] data = { Convert.ToByte(ADC_Hbyte + ChannelStep * CHANNEL), ADC_Lbyte_DoubleRange };
                    List<byte> rDATA = transmit(command, data);
                    ushort voltage = 0;
                    byte address = 0;
                    try
                    {
                        if (rDATA[0] != command)
                        {
                            addError(_command + ": Ошибка отклика!", rDATA);
                            return ushort.MaxValue;
                        }
                        address += Convert.ToByte(rDATA[1] >> 4);
                        voltage = Convert.ToUInt16((Convert.ToUInt16(rDATA[1] & 0xf) << 8) + rDATA[2]);
                    }
                    catch { addError(command + ": Ошибка данных!",rDATA); return ushort.MaxValue; }
                    trace(_command + ": Ответный адрес канала: " + address);
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
            public SPI_DEVICE_CHANNEL_withAD5643R ParentScan = new SPI_DEVICE_CHANNEL_withAD5643R(DAC_ParentScan_Channel, Command.SPI.Scaner.setVoltage, ADC_ParentScan_Channel, Command.SPI.Scaner.getVoltage);
            /// <summary>
            /// Сканирующее напряжение
            /// </summary>
            public SPI_DEVICE_CHANNEL_withAD5643R Scan = new SPI_DEVICE_CHANNEL_withAD5643R(DAC_Scan_Channel, Command.SPI.Scaner.setVoltage, ADC_Scan_Channel, Command.SPI.Scaner.getVoltage);
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
                if (!USART.IsOpen)
                {
                    USART.Open();
                }
                
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
                List<byte> rDATA;
                rDATA = transmit(Command.TEST.checkCommandStack);
                try
                {
                    if (rDATA[0] != Command.TEST.checkCommandStack)
                    {
                        addError(command + ": Ошибка отклика!", rDATA);
                        return false;
                    }
                    if (rDATA[1] == CommandStack)
                    {
                        trace(command = ": Команды идут синхронно. (" + CommandStack + ")");
                        return true;
                    }
                }
                catch { addError(command + ": Ошибка данных!", rDATA); return false; }
                trace(command + ": Команды идут НЕ синхронно! (" + CommandStack + ")");
                return false;
            }
            /// <summary>
            /// Возвращает код статуса МК (отладочное)
            /// </summary>
            public byte getStatus()
            {
                //ФУНКЦИЯ: Возвращает статус самого микроконтроллера и список проишествий
                string command = "Chip.getStatus()";
                trace_attached(Environment.NewLine);
                trace(command);
                List<byte> rDATA = transmit(Command.Chip.getStatus);
                byte incedent;
                try
                {
                    if (rDATA[0] != Command.Chip.getStatus)
                    {
                        addError(command + ": Ошибка отклика!", rDATA);
                        return byte.MaxValue;
                    }
                    trace(command + ": Статус МК: " + rDATA[1]); 
                    incedent = rDATA[2];
                }
                catch { addError(command + ": Ошибка данных!", rDATA); return byte.MaxValue; }
                if (incedent == 0)
                {
                    trace(command + ": За время работы проишествий не было. ");
                    return rDATA[0];
                }
                trace(command + ": Проишествия: ");
                if ((incedent & Incident.LOCKisLost) == Incident.LOCKisLost)
                {
                    trace("         LOCKisLost");
                }
                if ((incedent & Incident.TooShortPacket) == Incident.TooShortPacket)
                {
                    trace("         TooShortPacket");
                }
                if ((incedent & Incident.TooFast) == Incident.TooFast)
                {
                    trace("         TooFast");
                }
                return rDATA[0];
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
                List<byte> rDATA;
                rDATA = transmit(Command.Chip.getVersion);
                try 
                {
                    if (rDATA[0] != Command.Chip.getVersion)
                    {
                        addError(command + ": Ошибка отклика!", rDATA);
                        return byte.MaxValue;
                    }
                    trace(command + ": Версия прошивки МК: " + rDATA[1]);
                    return rDATA[1];
                }
                catch { addError(command + ": Ошибка данных!", rDATA); return byte.MaxValue; }
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
                List<byte> rDATA = transmit(Command.Chip.getBirthday);
                try
                {
                    if (rDATA[0] != Command.Chip.getBirthday)
                    {
                        addError(command + ": Ошибка отклика!", rDATA);
                        return string.Empty;
                    }
                    birthday = Convert.ToUInt32(rDATA[4]) * 16777216 + Convert.ToUInt32(rDATA[3]) * 65536 + Convert.ToUInt32(rDATA[2]) * 256 + Convert.ToUInt32(rDATA[1]);
                }
                catch (Exception)
                {
                    addError(" ! " + command + ": Ошибка при получении данных!");
                    return answer;
                }
                answer = birthday.ToString();
                answer = answer[6] + "" + answer[7] + "." + answer[4] + answer[5] + "." + answer[0] + answer[1] + answer[2] + answer[3];
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
                List<byte> rDATA = transmit(Command.Chip.getCPUfrequency);
                try
                {
                    if (rDATA[0] != Command.Chip.getCPUfrequency)
                    {
                        addError(command + ": Ошибка отклика!", rDATA);
                        return string.Empty;
                    }
                    frequency = Convert.ToUInt32(rDATA[4]) * 16777216 + Convert.ToUInt32(rDATA[3]) * 65536 + Convert.ToUInt32(rDATA[2]) * 256 + Convert.ToUInt32(rDATA[1]);
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
                trace("Tester.sendSomething(?)");
                //transmit(243);
                byte[] b = {58};
                USART.Write(b,0,b.Length);
            }
            /// <summary>
            /// Выводит сообщение в указанный Tracer
            /// </summary>
            /// <param name="text">Любой текст</param>
            public void traceIt(string text)
            {
                trace(text);
            }
        }
        public class TIC_PUMP
        {
            //КЛАСС: Микроконтроллер насоса
            /// <summary>
            /// Перендаём на МК данные DATA, которые он пересылает на TIC контроллер вакуумного насоса (В РАЗРАБОТКЕ...)
            /// </summary>
            List<byte> transmit_toTIC(byte[] DATA)
            {
                //ФУНКЦИЯ: Посылает данные TIC контроллеру
                //ПОЯСНЕНИЯ: ПК->МК: <key><Command.retransmitToTIC><lengthOfMessage><MessageToTIC><CS><lock>
                //           МК->TIC: <MessageToTIC>
                List<byte> formedDATA = new List<byte>();
                formedDATA.Add(Command.TIC.sendToTIC);
                formedDATA.Add((byte)(DATA.Length + 2)); //+2 для смещения относительно команды и байта длины
                formedDATA.AddRange(DATA);
                return transmit(formedDATA);
            }
            //Видимые функции
            /// <summary>
            /// Передаём на МК данные (внутри функции), которые он пересылает на TIC контроллер вакуумного насоса (В РАЗРАБОТКЕ...)
            /// </summary>
            public void send()
            {
                //ФУНКЦИЯ: Посылает данные TIC'у
                byte[] msg = { 58,23,13 };
                List<byte> data = new List<byte>();// = transmit_toTIC(msg);
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
        public SPI_DEVICE_CHANNEL Inlet = new SPI_DEVICE_CHANNEL(1, Command.SPI.PSInl.setVoltage, 1, Command.SPI.PSInl.getVoltage);
        /// <summary>
        /// Нагреватель
        /// </summary>
        public SPI_DEVICE_CHANNEL Heater = new SPI_DEVICE_CHANNEL(2, Command.SPI.PSInl.setVoltage, 2, Command.SPI.PSInl.getVoltage);
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
        /// <para>&lt;[Проверить\Установить][iHVE][PRGE][iEDCD][SEMV1][SEMV2][SEMV3][SPUMP]&gt;</para> 
        /// <para>,где [SPUM] - первый(нулевой) бит</para>
        /// <para>     [iHVE] - Разрешение высокого напряжения от TIC (readOnly)</para>
        /// <para>     [Проверить\Установить] - МК возвращает 1, если хотябы один из параметров был изменён, иначе 0</para>
        /// </summary>
        /// <param name="set_flags">true - установить следующие флаги, <para>false - не устанавливать, только проверить (последующие флаги не имеют значения)</para></param>
        /// <param name="PRGE">Разрешение высокого напряжения от оператора</param>
        /// <param name="EDCD">Разрешение дистанционного управления</param>
        /// <param name="SEMV1">Электромагнитный вентиль 1</param>
        /// <param name="SEMV2">Электромагнитный вентиль 2</param>
        /// <param name="SEMV3">Электромагнитный вентиль 3</param>
        /// <param name="SPUMP">Включение насоса?</param>
        /// <returns>Байт флагов в порядке &lt;[Изменено][iHVE][PRGE][iEDCD][SEMV1][SEMV2][SEMV3][SPUMP]&gt;</returns>
        public byte setFlags(bool set_flags, bool PRGE, bool EDCD, bool SEMV1, bool SEMV2, bool SEMV3, bool SPUMP)
        {
            //ФУНКЦИЯ: Выставляет флаги в соответствии с принятым байтом, если первый байт 1, и возвращает результат. Иначе просто возвращает флаги
            //ДАННЫЕ: <Command><[Проверить\Установить][iHVE][PRGE][iEDCD][SEMV1][SEMV2][SEMV3][SPUMP]>
            //				Если первый бит <Проверить\Установить> = 0, то МК тут же возвращает текущее состояние флагов
            //				Если первый бит <Проверить\Установить> = 1, то МК устанавливает флаги (кроме iHVE) и возвращает их.
            //				iHVE - только чтение
            //				Ответ МК битом [Проверить\Установить] -> 1 - параметры были изменены, 0 - нечего менять
            string command = "setFlags( set = " + Convert.ToInt16(set_flags) + " | iHVE(readOnly) | PRGE = " + Convert.ToInt16(PRGE) + " | EDCD = " + Convert.ToInt16(EDCD) + " | SEMV1 = " + Convert.ToInt16(SEMV1) + " | SEMV2 = " + Convert.ToInt16(SEMV2) + " | SEMV3 = " + Convert.ToInt16(SEMV3) + " | SPUMP = " + Convert.ToInt16(SPUMP) + ")";
            trace_attached(Environment.NewLine);
            trace(command);
            byte flags = Convert.ToByte(Convert.ToInt16(set_flags) * 128 + Convert.ToInt16(PRGE) * 32 + Convert.ToInt16(EDCD) * 16 + Convert.ToInt16(SEMV1) * 8 + Convert.ToInt16(SEMV2) * 4 + Convert.ToInt16(SEMV3) * 2 + Convert.ToInt16(SPUMP));
            List<byte> rDATA;
            rDATA = transmit(Command.setFlags, flags);
            try
            {
                if ((rDATA[1] & 128) == 0) { trace(command + ": Операция отменена! Нечего менять!"); }
                return rDATA[1];
            }
            catch { addError(command + ": Ошибка данных!", rDATA); return byte.MaxValue; }
        }
        //ИНТЕРФЕЙСНЫЕ
        delegate void delegate_trace(string text);
        static void trace(string text)
        {
            //ФУНКЦИЯ: Выводит text новой строкой с датой в RichTextBox и в файл, если эти действия разарешены
            string TEXT = Environment.NewLine + "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + text;
            if (tracer_defined && tracer_enabled)
            {
                //Трэйсер определён и разрешён
                if (tracer.InvokeRequired)
                {
                    delegate_trace a_delegate = new delegate_trace(trace);
                    tracer.Invoke(a_delegate, text);
                }
                else
                {
                    tracer.AppendText(TEXT);
                    tracer.ScrollToCaret();
                } 
                
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
                if (tracer.InvokeRequired)
                {
                    delegate_trace a_delegate = new delegate_trace(trace_attached);
                    tracer.Invoke(a_delegate, text);
                }
                else
                {
                    tracer.AppendText(TEXT);
                    tracer.ScrollToCaret();
                }
            }
            if (tracer_log_enabled)
            {
                //Вывод в файл разрешён
                System.IO.File.AppendAllText("Log.txt", TEXT);
            }
        }
        //USART
        public static void USART_DataReceived(object sender, EventArgs e)
        {
            //СОБЫТИЕ: Пришло асинхронное сообщение. Все асинхронные сообщения одной длины (7 байт)
            //ДАННЫЕ: <Tocken><DATA_1><DATA_2>
            bool tracer_enabled_before = tracer_enabled;    //сохраняем параметры трэйсера
            tracer_enabled = tracer_transmit_enabled;       //Если надо выключаем трэйс в трансмите
            List<byte> rDATA = decode(receive());
            tracer_enabled = tracer_enabled_before;
            Asynchr_decode(rDATA);
        }
        static void Asynchr_decode(List<byte> DATA)
        {
            //ФУНКЦИЯ: Декодируем асинхронное сообщение. Длина данных любого асинхронного сообщения 3 байта
            if(DATA.Count < 3)
            {
                if (DATA.Count == 0)
                {
                    trace("Скачёк на линии!");
                    return;
                }
                trace("НЕЧТО ТВОРИТЬСЯ НА ЛИНИИ:");
                foreach (byte b in DATA)
                {
                    trace("             " + b);
                }
                return;
            }
            switch (DATA[0])
            {
                case LAM.Tocken:
                    switch (DATA[1])
                    {
                        case LAM.RTC_end:
                            trace_attached(Environment.NewLine);
                            trace("LAM:Счётчики закончили счёт!");
                            _MeasureEnd.Invoke();
                            break;
                        default:
                            trace("МК хочет чтобы на него обратили внимание, но не понятно почему! "+DATA[1]);
                            break;
                    }
                    break;
                default:
                    trace("Неизвестная метка асинхронного сообщения: " + DATA[0]);
                    break;
            }
            
        }
        static List<byte> receive()
        {
            //ФУНКЦИЯ: Принимаем данные по СОМ-порту
            trace("         Приём...");                     //Приём-приём
            List<byte> rDATA = new List<byte>();
            if (!USART.IsOpen)
            {
                trace("ОШИБКА ПРИЁМА! Порт закрыт!");
                return rDATA;
            }
            trace("                 Принято:");
            //Входим в цикл приёма ждём байт до TimeOut
            for (uint time = 0; time < TimeOut; time++)
            {
                if(USART.BytesToRead > 0)
                {
                    time = 0;
                    rDATA.Add((byte)USART.ReadByte());
                    //trace("                     " + rDATA.Last<byte>());
                }
            }
            trace("         Приём завершён!");
            return rDATA;
        }
        static List<byte> decode(List<byte> DATA)
        {
            trace("         Анализ полученной команды...");
            if (DATA.Count < 3)
            {
                trace("Полученная команда слишком коротка!");
                return new List<byte>();
            }
            List<byte> rDATA = DATA;
            if (rDATA.First<byte>() == Command.KEY)
            {
                rDATA.RemoveAt(0);                              //Удаляем ключ
                if (rDATA.Last<byte>() == Command.LOCK)
                {
                    rDATA.RemoveAt(rDATA.Count - 1);        //Удаляем затвор
                    byte CheckSum = rDATA.Last<byte>();
                    rDATA.RemoveAt(rDATA.Count - 1);        //Удаляем контрольную сумму
                    byte calcedCheckSum = calcCheckSum(rDATA.ToArray());
                    if (CheckSum == calcedCheckSum)
                    {
                        //Пакет верный, возвращаем его
                        if (tracer_enabled)
                        {
                            trace("         Пакет принятых данных: ");
                            foreach (byte b in rDATA)
                            {
                                trace("             " + b);
                            }
                        }
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
                trace("ОШИБКА ПРИЁМА! Не был получен ключ. Получено:" + rDATA.First<byte>());
                return new List<byte>();
            }
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
        static void send(List<byte> DATA)
        {
            //ФУНКЦИЯ: Формируем пакет, передаём его МК.
            byte command = DATA[0];                         //Запоминаем передаваемую команду
            trace("     Начало исполнения команды:");
            trace("         Команда и байты данных:");
            foreach (byte b in DATA)
            {
                trace("             " + b);
            }
            trace("         Формирование пакета...");
            List<byte> Packet = new List<byte>();
            Packet.Add(Command.KEY);
            //Packet.Add((byte)(DATA.Count + 4));
            Packet.AddRange(DATA);
            Packet.Add(calcCheckSum(DATA.ToArray()));
            Packet.Add(Command.LOCK);
            trace("         Пакет:");
            foreach (byte b in Packet)
            {
                trace("             " + b);
            }
            //Выполняем передачу и приём
            trace("         Передача...");
            if (!USART_defined)
            {
                addError("СОМ-порт не определён!");
                return;
            }
            //Открытие порта, если он закрыт
            if (!USART.IsOpen)
            {
                USART.Open();
            }
            //Передача
            USART.Write(Packet.ToArray(), 0, Packet.Count);
            trace("         Передача завершена!");
            CommandStack++;                                 //Увеличиваем счётчик команд (команда ушла)
        }
        static List<byte> transmit(List<byte> wDATA)
        {
            //ФУНКЦИЯ: Формируем пакет, передаём его МК, слушаем ответ, возвращаем ответ.
            bool tracer_enabled_before = tracer_enabled;    //сохраняем параметры трэйсера
            tracer_enabled = tracer_transmit_enabled;       //Если надо выключаем трэйс в трансмите
            USART.DataReceived -= new SerialDataReceivedEventHandler(USART_DataReceived);
            send(wDATA);
            List<byte> rDATA = decode(receive());
            USART.DataReceived += new SerialDataReceivedEventHandler(USART_DataReceived);
            tracer_enabled = tracer_enabled_before;
            return rDATA;
        }
        static List<byte> transmit(byte[] wDATA)
        {
            return transmit(wDATA.ToList());
        }
        static List<byte> transmit(byte COMMAND)
        {
            return transmit((new byte[] { COMMAND }).ToList());
        }
        static List<byte> transmit(byte COMMAND, byte DATA)
        {
            return transmit((new byte[] { COMMAND, DATA }).ToList());
        }
        static List<byte> transmit(byte COMMAND, byte[] DATA)
        {
            List<byte> data = new List<byte>();
            data.Add(COMMAND);
            data.AddRange(DATA);
            return transmit(data);
        }
        static List<byte> transmit(byte COMMAND, List<byte> DATA)
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
                                    case Error.Decoder:
                                        addError(" !! МК СООБЩАЕТ ОБ ОШИБКЕ ДЕКОДЕРА! Неизвестная команда: " + DATA[2], DATA);
                                        break;
                                    case Error.CheckSum:
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
        static void addError(string TEXT, List<byte> rDATA)
        {
            string error = TEXT + Environment.NewLine + " Получено: ";
            foreach(byte b in rDATA)
            {
                error += b + " | ";
            }
            trace(error);
            ErrorList.Add(" -" + (ErrorList.Count + 1).ToString() + "- [" + DateTime.Now.ToString("HH:mm:ss") + "] " + error);
        }
        static void addError(string TEXT, byte[] rDATA)
        {
            string error = TEXT + Environment.NewLine + " Получено: ";
            foreach (byte b in rDATA)
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
        }
        bool reset()
        {
            //ФУНКЦИЯ: Програмная перезагрузка микроконтроллера
            return (transmit(Command.Chip.reset)[1] == Command.Chip.reset);
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
