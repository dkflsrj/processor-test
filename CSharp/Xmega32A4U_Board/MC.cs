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
    static class MC
    {
        //========================================================================================
        //=========================КЛАСС СВЯЗИ ПК С XMEGA32A4U ПО RS232===========================
        //========================================================================================
        #region-------------------------------------ПЕРЕМЕННЫЕ-----------------------------------------
        static SerialPort USART;                    //COM порт, его необходимо задать .Chip.setUSART(SerialPort)
        static bool USART_defined = false;          //СОМ-порт не определён.
        static RichTextBox tracer;                  //Трэйсер отладочных сообщений, задаётся .Service.setTracer(RichTextBox)
        static bool tracer_defined = false;         //Трэйсер не задан. 
        static bool tracer_enabled = true;          //Трэйсер включен. .Service.enableTracer(bool) - для вкл\выкл
        static bool tracer_transmit_enabled = true; //Трейсер в функции transmit включён\выключен
        static bool tracer_log_enabled = false;     //Ведение лога в Log.txt отключено. .Service.enableLog(bool) - для вкл\выкл
        static List<string> ErrorList = new List<string>();     //Лист всех ошибок. Получает при помощи .getErrorList()
        static byte CommandStack;                   //Счётчик выполненных команд (см. .Chip.checkCommandStack())
        const uint TimeOut = 1625;                  //260000 вроде 1 сек для данной машины
        const uint TIC_TimeOut = 7000;              //тайм аут для передачи и приёма для TIC'a
        #endregion
        #region-------------------------------------СТРУКТУРЫ------------------------------------------
        #region Коды ошибок
        struct Error
        {
            //СТРУКТУРА: Хранилище констант - кодовобычных ошибок, которые не ставят под угрозу работу системы.
            public const byte Token = 255;    //Есть ошибка
            //ErrorNums...
            public const byte Decoder = 1;       //Такое команды не существует
            public const byte CheckSum = 2;       //Неверная контрольная сумма
            //public const byte wrong_SPI_DEVICE_Number
        }
        #endregion
        #region Коды LAM сигналов
        struct LAM
        {
            //СТРУКТУРА: Хранилище констант - кодов асинхронных сообщений информирующие ПК о чём либо.
            public const byte Tocken = 254;      //Метка LAM сообщения
            //Номера асинхронных сообщений (обрати внимание)
            public const byte RTC_end = 1;      //RTC закончил измерение
            public const byte SPI_conf_done = 2;//После включения HVE все SPI устройства были настроены!
        }
        #endregion
        #region Коды внутренних ошибок
        struct Internal_Error
        {
            //СТРУКТУРА: Хранилище констант - кодов внутренних ошибок МК. Нежелательные и запрещённые состояния.
            public const byte Token = 253;      //Метка внутренней ошибки
            //Номера внутренних ошибок     
            public const byte USART_COMP = 1;       //Внутренняя ошибка приёма данных от ПК
            public const byte SPI = 2;       //SPI-устройства с таким номером нет  
        }
        #endregion
        #region Коды событий
        struct Incident
        {
            //СТРУКТУРА: Хранилище констант - кодов происшествий, которые передаются при опросе статуса МК
            public const byte LOCKisLost = 1;       //МК принимал пакет, но в последний байт пакета не затвор
            public const byte TooShortPacket = 2;   //байт длинны пакета меньше минимального (5)
            public const byte TooFast = 4;          //МК ещё не выполнил предыдущую команду, а уже приходит другая.
            public const byte Silence = 8;          //МК больше 60 секунд не связывался с ПК
            public const byte Noise = 16;           //На линии МК-ПК был замечен шум
        }
        #endregion
        #endregion
        #region---------------------------------------КЛАССЫ-------------------------------------------
        #region Таймер и счётчики
        public class Counters
        {
            #region Константы
            struct Constants
            {
                //СТРУКТУРА: Хранилище констант.
                public const double sourceFrequency = 32.768;//кГц - опорная частота таймера
                //Коды состояний
                public struct Status
                {
                    public const byte ready = 0;		//Счётчики готов к работе
                    public const string string_ready = "ready";
                    public const byte stopped = 1;		//Счётчики был принудительно остановлен
                    public const string string_stopped = "stopped";
                    public const byte busy = 2;		//Счётчики ещё считает
                    public const string string_busy = "busy";
                }
                //интервалы для предделителей
                public const int min_ms_div1 = 0;
                public const int min_ms_div2 = 2000;
                public const int min_ms_div8 = 4000;
                public const int min_ms_div16 = 16000;
                public const int min_ms_div64 = 32000;
                public const int min_ms_div256 = 127996;
                public const int min_ms_div1024 = 511981;
                public const int max_ms_div1024 = 2047925;
                //Проча
                public const byte LengthOfstartMeasurePacket = 6;
                public struct NextMeasure
                {
                    //Коды команд счётчиков
                    public const byte NotDo = 0;
                    public const byte Do = 1;
                }

            }
            #endregion
            /// <summary>
            /// Событие: счётчики закончили счёт.
            /// </summary>
            public static EventCallBack MeasureEnd;
            #region Счётчики
            /// <summary>
            /// Результат счётчика А (32-разрядный) 
            /// <para>Если значение счётчика равно максимальному для uint,</para>
            /// <para>то это означает, что счётчик переполнен.</para>
            /// </summary>
            public static uint COA = 0;
            /// <summary>
            /// Результат счётчика B (32-разрядный) 
            /// <para>Если значение счётчика равно максимальному для uint,</para>
            /// <para>то это означает, что счётчик переполнен.</para>
            /// </summary>
            public static uint COB = 0;
            /// <summary>
            /// Результат счётчика C (32-разрядный) 
            /// <para>Если значение счётчика равно максимальному для uint,</para>
            /// <para>то это означает, что счётчик переполнен.</para>
            /// </summary>
            public static uint COC = 0;
            #endregion
            public static string Status = "notSet";
            static string convertStatus(byte Status_byte)
            {
                //ФУНКЦИЯ: Переводит числовой байт статуса в строку
                string Status_string = "";
                switch (Status_byte)
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
            #region Расчётные функции
            /// <summary>
            /// Вычисляет и возвращает предделитель RTC 
            /// <para>ПРИМЕЧАНИЕ: без участия МК</para>
            /// </summary>
            /// <param name="MeasureTime_ms">Время измерения в миллисекундах от 0 до 2047925</param>
            /// <returns>?</returns>
            public static byte calcRTCprescaler(uint MILLISECONDS)
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
                    MC.Service.trace("Counters.calcRTCprescaler(" + MILLISECONDS + "):Неверное значение! Ожидалось: 50...2047925 мс. Получено: " + MILLISECONDS);
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
            public static ushort calcRTCprescaler_long(uint MILLISECONDS)
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
                    MC.Service.trace("Counters.calcRTCprescaler_long(" + MILLISECONDS + "):Неверное значение! Ожидалось: 50...2047925 мс. Получено: " + MILLISECONDS);
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
            public static ushort calcRTCprescaler_long(string MILLISECONDS)
            {
                try { return calcRTCprescaler_long(Convert.ToUInt32(MILLISECONDS)); }
                catch { MC.Service.trace("Counters.calcRTCprescaler_long(" + MILLISECONDS + "):Неверное значение!"); return ushort.MaxValue; };
            }
            /// <summary>
            /// Возвращает частоту RTC в соответствии с предделителем в длинном формате
            /// <para>ПРИМЕЧАНИЕ: без участия МК</para>
            /// </summary>
            public static double calcRTCfrequency(uint MILLISECONDS)
            {
                //ФУНКЦИЯ: Возвращает итоговую частоту RTC в соответствии с сохраннённым предделителем.
                return (Constants.sourceFrequency / calcRTCprescaler_long(MILLISECONDS));
            }
            /// <summary>
            /// Возвращает частоту RTC в соответствии с предделителем в длинном формате
            /// <para>ПРИМЕЧАНИЕ: без участия МК</para>
            /// </summary>
            public static double calcRTCfrequency(string MILLISECONDS)
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
            public static ushort calcRTCticks(uint MILLISECONDS)
            {
                //ФУНКЦИЯ: Вычисляет количество тиков в соответствии с временем и предделителем. Возвращает количество тиков
                ushort tiks;
                if ((MILLISECONDS < 50) || (calcRTCprescaler(MILLISECONDS) == 0))
                {
                    MC.Service.trace("Counters.calcRTCticks(" + MILLISECONDS + ")" + ": Отмена операции! Неверное значение! Ожидалось: 50...2047925 мс. Получено: " + MILLISECONDS);
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
            public static ushort calcRTCticks(string MILLISECONDS)
            {
                try { return calcRTCticks(Convert.ToUInt32(MILLISECONDS)); }
                catch { MC.Service.trace("Counters.calcRTCticks(" + MILLISECONDS + "):Неверное значение!"); return ushort.MaxValue; };
            }
            #endregion
            /// <summary>
            /// Запускает измерение длительностью MILLISECONDS
            /// <para>По окончанию измерения МК пришлёт асинхронное LookAtMe сообщение </para>
            /// <para>, которое вызовет EventCallBack MeasureEnd.  </para>
            /// <para>Возвращает:</para>
            /// <para>true - счётчики начали счёт.</para>
            /// <para>false - операция отменена.</para>
            /// <param name="MeasureTime_ms">Время измерения в миллисекундах от 50 до 2047925</param>
            /// </summary>
            public static bool startMeasure(uint MILLISECONDS)
            {
                //ФУНКЦИЯ: Запускаем счётчик, возвращает true если счёт начался, false - счётчик уже считает
                MC.Service.trace_attached(Environment.NewLine);
                string command = "Counters.startMeasure()";
                MC.Service.trace(command);
                //Проверка диапазона заданного интервала измерения
                if ((MILLISECONDS < 50) || (calcRTCprescaler(MILLISECONDS) == 0))
                {
                    MC.Service.trace(command + ": Отмена операции! Неверное значение! Ожидалось: 50...2047925 мс. Получено: " + MILLISECONDS);
                    return false;
                }
                //Очистка
                COA = 0;
                COB = 0;
                COC = 0;
                List<byte> wDATA = new List<byte>();    //Данные на передачу
                List<byte> rDATA;
                //Вычисление байт для периода RTC
                byte[] MeasurePeriod = BitConverter.GetBytes(calcRTCticks(MILLISECONDS));
                //Набираем данные на отправку
                wDATA.Add(Command.RTC.startMeasure); //Команда
                wDATA.Add(calcRTCprescaler(MILLISECONDS));   //Предделитель
                wDATA.Add(MeasurePeriod[1]);
                wDATA.Add(MeasurePeriod[0]);
                //Посылаем команду
                rDATA = MC.Service.transmit(wDATA);
                try
                {
                    if (rDATA[0] != Command.RTC.startMeasure)
                    {
                        MC.Service.addError(command + ": Ошибка отклика!", rDATA);
                        return false;
                    }
                    Status = convertStatus(rDATA[1]);
                }
                catch { MC.Service.addError(command + ": Ошибка данных!", rDATA); return false; }
                if ((Status == Constants.Status.string_ready) || (Status == Constants.Status.string_stopped))
                { MC.Service.trace(command + ": Счётчики начали счёт."); return true; }
                MC.Service.trace(command + ": Операция отменена! Счётчики уже считают!"); return false;
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
            public static bool startMeasure(string MILLISECONDS)
            {
                try { return startMeasure(Convert.ToUInt32(MILLISECONDS)); }
                catch { MC.Service.trace("Counters.startMeasure(" + MILLISECONDS + "):Неверное значение!"); return false; };
            }
            /// <summary>
            /// Останавливает серию.
            ///<para>Возвращает:</para>
            ///<para>true - операция выполнена успешно.</para>
            ///<para>false - операция была отменена.</para>
            /// </summary>
            public static bool stopMeasure()
            {
                //ФУНКЦИЯ: Останавливает счётчик. Возвращает true, если операция удалась. false - не удалась (счётчик не считает)
                string command = "Counters.stopMeasure()";
                MC.Service.trace_attached(Environment.NewLine);
                MC.Service.trace(command);
                List<byte> rDATA = MC.Service.transmit(Command.RTC.stopMeasure);
                try
                {
                    if (rDATA[0] != Command.RTC.stopMeasure)
                    {
                        MC.Service.addError(command + ": Ошибка отклика!", rDATA);
                        return false;
                    }
                    Status = convertStatus(rDATA[1]);
                }
                catch { MC.Service.addError(command + ": Ошибка данных!", rDATA); return false; }
                switch (Status)
                {
                    case Constants.Status.string_busy:
                        MC.Service.trace(command + ": Операция выполнена успешно!");
                        return true;
                    case Constants.Status.string_ready:
                        MC.Service.trace(command + ": Операция отменена! Счётчики не считают!");
                        return false;
                    case Constants.Status.string_stopped:
                        MC.Service.trace(command + ": Операция отменена! Счётчики уже остановлены!");
                        return false;
                    default:
                        MC.Service.addError(command + ": ОШИБКА МК! НЕИЗВЕСТНОЕ СОСТОЯНИЕ СЧЁТЧИКОВ: " + Status);
                        return false;
                }
            }
            /// <summary>
            /// Запрашиваем результаты счёта у МК
            /// <para> Возвращает:</para>
            /// <para>  true - операция удалась. Результаты записаны в COA, COB и COC.</para>
            /// <para>  false - операция отменена. Вероятно счётчики всё ещё считают.</para>
            /// </summary>
            public static bool receiveResults()
            {
                //ФУНКЦИЯ: Запрашиваем результаты измерения. Если измерение завершилось успешно нам пришлют результаты и мы выйдем с true, в противном случае пришлют статус (не ready) и мы вылетим с false'м
                //ДАННЫЕ: <Command><RTC_Status><COA_OVF[1]><COA_OVF[0]><COA_M[1]><COA_M[0]><COB_OVF[1]><COB_OVF[0]><COВ_M[1]><COВ_M[0]><COC_OVF[1]><COC_OVF[0]><COС_M[1]><COС_M[0]>
                MC.Service.trace_attached(Environment.NewLine);
                string command = "Counters.receiveResults()";
                MC.Service.trace(command);
                List<byte> rDATA;                       //Данные на приём
                rDATA = MC.Service.transmit(Command.RTC.receiveResults);
                try
                {
                    if (rDATA[0] != Command.RTC.receiveResults)
                    {
                        MC.Service.addError(command + ": Ошибка отклика!", rDATA);
                        return false;
                    }
                    Status = convertStatus(rDATA[1]);
                    if (Status == Constants.Status.string_ready)
                    {
                        COA = (uint)(rDATA[2] * 16777216 + rDATA[3] * 65536 + rDATA[4] * 256 + rDATA[5]);
                        COB = (uint)(rDATA[6] * 16777216 + rDATA[7] * 65536 + rDATA[8] * 256 + rDATA[9]);
                        COC = (uint)(rDATA[10] * 16777216 + rDATA[11] * 65536 + rDATA[12] * 256 + rDATA[13]);
                        MC.Service.trace(command + ":Результаты успешно получены!");
                        return true;
                    }
                }
                catch { MC.Service.addError(command + ": Ошибка данных!", rDATA); }
                return false;
            }
        }
        #endregion
        #region Микроконтроллер
        /// <summary>
        /// Микроконтроллер XMega32A4U
        /// </summary>
        public static class Chip
        {
            //КЛАСС: Микросхема, сам микроконтроллер.
            /// <summary>
            /// Проверка синхронности команд ПК и МК (отладочное)
            /// </summary>
            public static bool checkCommandStack()
            {
                //ФУНКЦИЯ: При общении ПК-МК, они считаю выполненные команды (byte). Это функция сверяет номер команды.
                string command = "Chip.checkCommandStack()";
                MC.Service.trace_attached(Environment.NewLine);
                MC.Service.trace(command);
                List<byte> rDATA;
                rDATA = MC.Service.transmit(Command.Service.checkCommandStack);
                try
                {
                    if (rDATA[0] != Command.Service.checkCommandStack)
                    {
                        MC.Service.addError(command + ": Ошибка отклика!", rDATA);
                        return false;
                    }
                    if (rDATA[1] == CommandStack)
                    {
                        MC.Service.trace(command = ": Команды идут синхронно. (" + CommandStack + ")");
                        return true;
                    }
                }
                catch { MC.Service.addError(command + ": Ошибка данных!", rDATA); return false; }
                MC.Service.trace(command + ": Команды идут НЕ синхронно! (" + CommandStack + ")");
                return false;
            }
            /// <summary>
            /// Возвращает код статуса МК (отладочное)
            /// </summary>
            public static byte getStatus()
            {
                //ФУНКЦИЯ: Возвращает статус самого микроконтроллера и список проишествий
                string command = "Chip.getStatus()";
                MC.Service.trace_attached(Environment.NewLine);
                MC.Service.trace(command);
                List<byte> rDATA = MC.Service.transmit(Command.Chip.getStatus);
                byte incedent;
                try
                {
                    if (rDATA[0] != Command.Chip.getStatus)
                    {
                        MC.Service.addError(command + ": Ошибка отклика!", rDATA);
                        return byte.MaxValue;
                    }
                    MC.Service.trace(command + ": Статус МК: " + rDATA[1]);
                    incedent = rDATA[2];
                }
                catch { MC.Service.addError(command + ": Ошибка данных!", rDATA); return byte.MaxValue; }
                if (incedent == 0)
                {
                    MC.Service.trace(command + ": За время работы проишествий не было. ");
                    return rDATA[0];
                }
                MC.Service.trace(command + ": Проишествия: ");
                if ((incedent & Incident.LOCKisLost) == Incident.LOCKisLost)
                {
                    MC.Service.trace("         LOCKisLost");
                }
                if ((incedent & Incident.TooShortPacket) == Incident.TooShortPacket)
                {
                    MC.Service.trace("         TooShortPacket");
                }
                if ((incedent & Incident.TooFast) == Incident.TooFast)
                {
                    MC.Service.trace("         TooFast");
                }
                if ((incedent & Incident.Silence) == Incident.Silence)
                {
                    MC.Service.trace("         Silence");
                }
                if ((incedent & Incident.Noise) == Incident.Noise)
                {
                    MC.Service.trace("         Noise");
                }
                return rDATA[0];
            }
            /// <summary>
            /// Возвращает версию прошивки МК
            /// </summary>
            public static byte getVersion()
            {
                //ФУНКЦИЯ: Возвращает версию прошивки микроконтроллера
                string command = "Chip.getVersion()";
                MC.Service.trace_attached(Environment.NewLine);
                MC.Service.trace(command);
                List<byte> rDATA;
                rDATA = MC.Service.transmit(Command.Chip.getVersion);
                try
                {
                    if (rDATA[0] != Command.Chip.getVersion)
                    {
                        MC.Service.addError(command + ": Ошибка отклика!", rDATA);
                        return byte.MaxValue;
                    }
                    MC.Service.trace(command + ": Версия прошивки МК: " + rDATA[1]);
                    return rDATA[1];
                }
                catch { MC.Service.addError(command + ": Ошибка данных!", rDATA); return byte.MaxValue; }
            }
            /// <summary>
            /// Возвращает дату создания прошивки МК 
            /// </summary>
            public static string getBirthday()
            {
                //ФУНКЦИЯ: Возвращает дату создания прошивки микроконтроллера
                string command = "Chip.getBirthday()";
                MC.Service.trace_attached(Environment.NewLine);
                MC.Service.trace(command);
                UInt32 birthday = 0;
                string answer = "00000000";
                List<byte> rDATA = MC.Service.transmit(Command.Chip.getBirthday);
                try
                {
                    if (rDATA[0] != Command.Chip.getBirthday)
                    {
                        MC.Service.addError(command + ": Ошибка отклика!", rDATA);
                        return string.Empty;
                    }
                    birthday = Convert.ToUInt32(rDATA[4]) * 16777216 + Convert.ToUInt32(rDATA[3]) * 65536 + Convert.ToUInt32(rDATA[2]) * 256 + Convert.ToUInt32(rDATA[1]);
                }
                catch (Exception)
                {
                    MC.Service.addError(" ! " + command + ": Ошибка при получении данных!");
                    return answer;
                }
                answer = birthday.ToString();
                answer = answer[6] + "" + answer[7] + "." + answer[4] + answer[5] + "." + answer[0] + answer[1] + answer[2] + answer[3];
                MC.Service.trace(command + ": Дата создания прошивки МК: " + answer);
                return answer;
            }
            /// <summary>
            /// Возвращает частоту процессора МК (не вычисляется) 
            /// </summary>
            public static string getCPUfrequency()
            {
                //ФУНКЦИЯ: Возвращает частоту процессора микроконтроллера (не вычисляется)
                string command = "Chip.getCPUfrequency()";
                MC.Service.trace_attached(Environment.NewLine);
                MC.Service.trace(command);
                UInt32 frequency = 0;
                List<byte> rDATA = MC.Service.transmit(Command.Chip.getCPUfrequency);
                try
                {
                    if (rDATA[0] != Command.Chip.getCPUfrequency)
                    {
                        MC.Service.addError(command + ": Ошибка отклика!", rDATA);
                        return string.Empty;
                    }
                    frequency = Convert.ToUInt32(rDATA[4]) * 16777216 + Convert.ToUInt32(rDATA[3]) * 65536 + Convert.ToUInt32(rDATA[2]) * 256 + Convert.ToUInt32(rDATA[1]);
                }
                catch (Exception)
                {
                    MC.Service.addError(" ! " + command + ": Ошибка при получении данных!");
                    return "!";
                }
                MC.Service.trace(command + ": Частота процессора: " + frequency.ToString() + " Гц");
                return frequency.ToString() + " Гц";
            }
        }
        #endregion
        #region Сервисные функции
        /// <summary>
        /// Сервисные функции для отладки и проча...
        /// </summary>
        public static class Service
        {
            //КЛАСС: Класс для сервисных функций, отладки и проча
            static List<byte> dummy = new List<byte>();
            /// <summary>
            /// Задаёт трейсер для вывода сообщений (отладочное)
            /// </summary>
            public static void setTracer(RichTextBox TRACER)
            {
                //ФУНКЦИЯ: Задаём трэйсер для отладки
                trace_attached(Environment.NewLine);
                trace("Service.setTracer(" + TRACER.Name + ")");
                tracer = TRACER;
                tracer_defined = true;
                //trace("Service.setTracer(" + TRACER.Name + "): Трэйсер задан.");
            }
            /// <summary>
            /// Разрешает вывод сообщений в трейсер, если он задан (отладочное) 
            /// </summary>
            public static void enableTracer(bool enable)
            {
                //ФУНКЦИЯ: Включает\выключает трэйсер
                if (enable)
                {
                    tracer_enabled = enable;
                    trace_attached(Environment.NewLine);
                    trace("Service.enableTracer(" + enable + ")");
                    return;
                }
                trace_attached(Environment.NewLine);
                trace("Service.enableTracer(" + enable + ")");
                tracer_enabled = enable;
            }
            /// <summary>
            /// Разрешает подробный вывод сообщений в трейсер, если он задан (отладочное) 
            /// </summary>
            public static void enableTracerInTransmit(bool enable)
            {
                //ФУНКЦИЯ: Включает\выключает трэйсер
                if (enable)
                {
                    tracer_transmit_enabled = enable;
                    trace_attached(Environment.NewLine);
                    trace("Service.enableTracerInTransmit(" + enable + ")");
                    return;
                }
                trace_attached(Environment.NewLine);
                trace("Service.enableTracerInTransmit(" + enable + ")");
                tracer_transmit_enabled = enable;
            }
            /// <summary>
            /// Разрешает вывод сообщений трейсера в текстовый файл Log.txt, находящийся в папке с exe-файлом
            /// </summary>
            public static void enableLog(bool enable)
            {
                //ФУНКЦИЯ: Включает\выключает ведение лога (сохранение отладочных сообщений в Log.txt)
                if (enable)
                {
                    tracer_log_enabled = enable;
                    trace_attached(Environment.NewLine);
                    trace("Service.enableLog(" + enable + ")");
                    return;
                }
                trace_attached(Environment.NewLine);
                trace("Service.enableLog(" + enable + ")");
                tracer_log_enabled = enable;
            }
            /// <summary>
            /// Посылает МК число 243. Получаем от него ругательства... (отладочное)
            /// </summary>
            public static void sendSomething()
            {
                //ФУНКЦИЯ: Просто посылает число микроконтроллеру.
                trace_attached(Environment.NewLine);
                trace("Service.sendSomething(?)");
                //transmit(243);
                byte[] b = { 58 };
                USART.Write(b, 0, b.Length);
            }
            /// <summary>
            /// Выводит сообщение в указанный Tracer
            /// </summary>
            /// <param name="text">Любой текст</param>
            public static void traceIt(string text)
            {
                trace(text);
            }
            //ИНТЕРФЕЙСНЫЕ
            delegate void delegate_trace(string text);
            public static void trace(string text)
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
            public static void trace_attached(string text)
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
            public static void Asynchr_decode(List<byte> DATA)
            {
                //ФУНКЦИЯ: Декодируем асинхронное сообщение. Длина данных любого асинхронного сообщения 3 байта
                if (DATA.Count < 3)
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
                                Counters.MeasureEnd.Invoke();
                                break;
                            case LAM.SPI_conf_done:
                                trace_attached(Environment.NewLine);
                                trace("LAM:Высокое напряжение разрешено, SPI устройства настроены!");
                                //Counters.MeasureEnd.Invoke(); //Здесь нужен делегат для этого события
                                break;
                            default:
                                trace("МК хочет чтобы на него обратили внимание, но не понятно почему! " + DATA[1]);
                                break;
                        }
                        break;
                    default:
                        trace("Неизвестная метка асинхронного сообщения: " + DATA[0]);
                        break;
                }

            }
            public static List<byte> receive()
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
                    if (USART.BytesToRead > 0)
                    {
                        time = 0;
                        rDATA.Add((byte)USART.ReadByte());
                        //trace("                     " + rDATA.Last<byte>());
                    }
                }
                trace("         Приём завершён!");
                return rDATA;
            }
            public static List<byte> decode(List<byte> DATA)
            {
                List<byte> rDATA = DATA;
                if (tracer_enabled)
                {
                    trace("         Принятый пакет: ");
                    foreach (byte b in rDATA)
                    {
                        trace("             " + b);
                    }
                }
                trace("         Анализ полученной команды...");
                if (DATA.Count < 3)
                {
                    trace("Полученная команда слишком коротка!");
                    return new List<byte>();
                }
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
                            //if (tracer_enabled)
                            //{
                            //    trace("         Пакет принятых данных: ");
                            //    foreach (byte b in rDATA)
                            //    {
                            //        trace("             " + b);
                            //    }
                            //}
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
            public static byte calcCheckSum(byte[] data)
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
            public static void send(List<byte> DATA)
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
            public static List<byte> transmit(List<byte> wDATA)
            {
                //ФУНКЦИЯ: Формируем пакет, передаём его МК, слушаем ответ, возвращаем ответ.
                bool tracer_enabled_before = tracer_enabled;    //сохраняем параметры трэйсера
                tracer_enabled = tracer_transmit_enabled;       //Если надо выключаем трэйс в трансмите
                try { USART.DataReceived -= new SerialDataReceivedEventHandler(USART_DataReceived); }
                catch { MC.Service.trace("MC.Service.transmit(...): Ошибка USART! COM-port не определён"); return dummy; }
                send(wDATA);
                List<byte> rDATA = decode(receive());
                USART.DataReceived += new SerialDataReceivedEventHandler(USART_DataReceived);
                tracer_enabled = tracer_enabled_before;
                return rDATA;
            }
            public static List<byte> transmit(byte[] wDATA)
            {
                return transmit(wDATA.ToList());
            }
            public static List<byte> transmit(byte COMMAND)
            {
                return transmit((new byte[] { COMMAND }).ToList());
            }
            public static List<byte> transmit(byte COMMAND, byte DATA)
            {
                return transmit((new byte[] { COMMAND, DATA }).ToList());
            }
            public static List<byte> transmit(byte COMMAND, byte[] DATA)
            {
                List<byte> data = new List<byte>();
                data.Add(COMMAND);
                data.AddRange(DATA);
                return transmit(data);
            }
            public static List<byte> transmit(byte COMMAND, List<byte> DATA)
            {
                //Проверить - не надо ли создавать новый список
                DATA.Insert(0, COMMAND);
                return transmit(DATA);
            }
            public static void addError(string TEXT, List<byte> rDATA)
            {
                string error = TEXT + Environment.NewLine + " Получено: ";
                foreach (byte b in rDATA)
                {
                    error += b + " | ";
                }
                MC.Service.trace(error);
                ErrorList.Add(" -" + (ErrorList.Count + 1).ToString() + "- [" + DateTime.Now.ToString("HH:mm:ss") + "] " + error);
            }
            public static void addError(string TEXT, byte[] rDATA)
            {
                string error = TEXT + Environment.NewLine + " Получено: ";
                foreach (byte b in rDATA)
                {
                    error += b + " | ";
                }
                MC.Service.trace(error);
                ErrorList.Add(" -" + (ErrorList.Count + 1).ToString() + "- [" + DateTime.Now.ToString("HH:mm:ss") + "] " + error);
            }
            public static void addError(string TEXT, int rDATA)
            {
                string error = " -" + (ErrorList.Count + 1).ToString() + "- [" + DateTime.Now.ToString("HH:mm:ss") + "] " + TEXT + Environment.NewLine + " Получено: " + rDATA;
                MC.Service.trace(TEXT + Environment.NewLine + "    Получено: " + rDATA);
                ErrorList.Add(error);
            }
            public static void addError(string TEXT)
            {
                string error = " -" + (ErrorList.Count + 1).ToString() + "- [" + DateTime.Now.ToString("HH:mm:ss") + "] " + TEXT;
                MC.Service.trace(TEXT);
                ErrorList.Add(error);
            }
        }
        #endregion
        #region Flags
        public static class Flags
        {
            /// <summary>
            /// Разрешение высокого напряжения от TIC'а. Возвращает "Enabled" или "Blocked"
            /// </summary>
            public static string HVE
            {
                get
                {
                    string command = "MC.HVE.get()";
                    MC.Service.trace_attached(Environment.NewLine);
                    MC.Service.trace(command);
                    List<byte> rDATA = MC.Service.transmit(Command.Flags.HVE);
                    try
                    {
                        if (rDATA[1] == rDATA[2]) { if (rDATA[1] == 0) { return "Enabled"; } else { return "Blocked"; } }
                        else { MC.Service.trace(command + ": Внутренняя ошибка! Не совпадение pin_iHVE и Flags.iHVE!"); }
                    }
                    catch { MC.Service.trace(command + ": Ошибка данных!"); }
                    return "?";
                }
            }
            /// <summary>
            /// Разрешение оператора на включение высокого напряжения.
            /// <para>Возвращает: "On" - включено\включить, </para>
            /// <para>"Off" - выключено\выключить,  </para>
            /// <para>"Blocked" - высокое напряжение запрещено TIC'ом</para>
            /// </summary>
            public static string PRGE
            {
                //ПАКЕТ: <Command><getOrSet>
                //					<0>\<1> - устанавливают
                //					<any_else> - запрашивает
                get
                {
                    string command = "MC.PRGE.get()";
                    MC.Service.trace_attached(Environment.NewLine);
                    MC.Service.trace(command);
                    List<byte> rDATA = MC.Service.transmit(Command.Flags.PRGE, 255);
                    try
                    {
                        switch (rDATA[1])
                        {
                            case 0: return "On";
                            case 1: return "Off";
                            case 254: return "Blocked";
                            default: return "?";
                        }
                    }
                    catch { MC.Service.trace(command + ": Ошибка данных!"); }
                    return "?";
                }
                set
                {
                    string command = "MC.PRGE.set(" + value + ")";
                    MC.Service.trace_attached(Environment.NewLine);
                    MC.Service.trace(command);
                    byte setupByte = 0;
                    if (value == "On")
                    {
                        setupByte = 1;
                    }
                    MC.Service.transmit(Command.Flags.PRGE, setupByte);
                }
            }
            /// <summary>
            /// Включает\выключает дистанционное управление
            /// <para>Возвращает: "On" - включено\включить, </para>
            /// <para>"Off" - выключено\выключить,  </para>
            /// </summary>
            public static string EDCD
            {
                get
                {
                    string command = "MC.EDCD.get()";
                    MC.Service.trace_attached(Environment.NewLine);
                    MC.Service.trace(command);
                    List<byte> rDATA = MC.Service.transmit(Command.Flags.EDCD, 255);
                    try
                    {
                        switch (rDATA[1])
                        {
                            case 0: return "Off";
                            case 1: return "On";
                            default: MC.Service.trace(command + ": Неверное состояние!");
                                break;
                        }
                    }
                    catch { MC.Service.trace(command + ": Ошибка данных!"); }
                    return "?";
                }
                set
                {
                    string command = "MC.EDCD.set(" + value + ")";
                    MC.Service.trace_attached(Environment.NewLine);
                    MC.Service.trace(command);
                    byte setupByte = 0;
                    if (value == "On")
                    {
                        setupByte = 1;
                    }
                    MC.Service.transmit(Command.Flags.EDCD, setupByte);
                }
            }
            /// <summary>
            /// Включает\выключает вентиль
            /// <para>Возвращает: "On" - включено\включить, </para>
            /// <para>"Off" - выключено\выключить,  </para>
            /// </summary>
            /// <param name="enable"></param>
            /// <returns></returns>
            public static string SEMV1
            {
                get
                {
                    string command = "MC.SEMV1.get()";
                    MC.Service.trace_attached(Environment.NewLine);
                    MC.Service.trace(command);
                    List<byte> rDATA = MC.Service.transmit(Command.Flags.SEMV1, 255);
                    try
                    {
                        switch (rDATA[1])
                        {
                            case 0: return "Off";
                            case 1: return "On";
                            default: MC.Service.trace(command + ": Неверное состояние!");
                                break;
                        }
                    }
                    catch { MC.Service.trace(command + ": Ошибка данных!"); }
                    return "?";
                }
                set
                {
                    string command = "MC.SEMV1.set(" + value + ")";
                    MC.Service.trace_attached(Environment.NewLine);
                    MC.Service.trace(command);
                    byte setupByte = 0;
                    if (value == "On")
                    {
                        setupByte = 1;
                    }
                    MC.Service.transmit(Command.Flags.SEMV1, setupByte);
                }
            }
            /// <summary>
            /// Включает\выключает вентиль
            /// <para>Возвращает: "On" - включено\включить, </para>
            /// <para>"Off" - выключено\выключить,  </para>
            /// </summary>
            /// <param name="enable"></param>
            /// <returns></returns>
            public static string SEMV2
            {
                get
                {
                    string command = "MC.SEMV2.get()";
                    MC.Service.trace_attached(Environment.NewLine);
                    MC.Service.trace(command);
                    List<byte> rDATA = MC.Service.transmit(Command.Flags.SEMV2, 255);
                    try
                    {
                        switch (rDATA[1])
                        {
                            case 0: return "Off";
                            case 1: return "On";
                            default: MC.Service.trace(command + ": Неверное состояние!");
                                break;
                        }
                    }
                    catch { MC.Service.trace(command + ": Ошибка данных!"); }
                    return "?";
                }
                set
                {
                    string command = "MC.SEMV2.set(" + value + ")";
                    MC.Service.trace_attached(Environment.NewLine);
                    MC.Service.trace(command);
                    byte setupByte = 0;
                    if (value == "On")
                    {
                        setupByte = 1;
                    }
                    MC.Service.transmit(Command.Flags.SEMV2, setupByte);
                }
            }
            /// <summary>
            /// Включает\выключает вентиль
            /// <para>Возвращает: "On" - включено\включить, </para>
            /// <para>"Off" - выключено\выключить,  </para>
            /// </summary>
            /// <param name="enable"></param>
            /// <returns></returns>
            public static string SEMV3
            {
                get
                {
                    string command = "MC.SEMV3.get()";
                    MC.Service.trace_attached(Environment.NewLine);
                    MC.Service.trace(command);
                    List<byte> rDATA = MC.Service.transmit(Command.Flags.SEMV3, 255);
                    try
                    {
                        switch (rDATA[1])
                        {
                            case 0: return "Off";
                            case 1: return "On";
                            default: MC.Service.trace(command + ": Неверное состояние!");
                                break;
                        }
                    }
                    catch { MC.Service.trace(command + ": Ошибка данных!"); }
                    return "?";
                }
                set
                {
                    string command = "MC.SEMV3.set(" + value + ")";
                    MC.Service.trace_attached(Environment.NewLine);
                    MC.Service.trace(command);
                    byte setupByte = 0;
                    if (value == "On")
                    {
                        setupByte = 1;
                    }
                    MC.Service.transmit(Command.Flags.SEMV3, setupByte);
                }
            }
            /// <summary>
            /// Включает\выключает насос
            /// <para>Возвращает: "On" - включено\включить, </para>
            /// <para>"Off" - выключено\выключить,  </para>
            /// </summary>
            /// <param name="enable"></param>
            /// <returns></returns>
            public static string SPUMP
            {
                get
                {
                    string command = "MC.SPUMP.get()";
                    MC.Service.trace_attached(Environment.NewLine);
                    MC.Service.trace(command);
                    List<byte> rDATA = MC.Service.transmit(Command.Flags.SPUMP, 255);
                    try
                    {
                        switch (rDATA[1])
                        {
                            case 0: return "Off";
                            case 1: return "On";
                            default: MC.Service.trace(command + ": Неверное состояние!");
                                break;
                        }
                    }
                    catch { MC.Service.trace(command + ": Ошибка данных!"); }
                    return "?";
                }
                set
                {
                    string command = "MC.SPUMP.set(" + value + ")";
                    MC.Service.trace_attached(Environment.NewLine);
                    MC.Service.trace(command);
                    byte setupByte = 0;
                    if (value == "On")
                    {
                        setupByte = 1;
                    }
                    MC.Service.transmit(Command.Flags.SPUMP, setupByte);
                }
            }
        }
        #endregion
        #endregion
        #region--------------------------------------ОБЪЕКТЫ-------------------------------------------
        /// <summary>
        /// Натекатель
        /// </summary>
        public static SPI_DEVICE_CHANNEL Inlet = new SPI_DEVICE_CHANNEL(1, Command.SPI.PSInl.setVoltage, 1, Command.SPI.PSInl.getVoltage);
        /// <summary>
        /// Нагреватель
        /// </summary>
        public static SPI_DEVICE_CHANNEL Heater = new SPI_DEVICE_CHANNEL(2, Command.SPI.PSInl.setVoltage, 2, Command.SPI.PSInl.getVoltage);
        /// <summary>
        /// Ионный Источник
        /// </summary>
        public static SPI_IonSOURCE IonSource = new SPI_IonSOURCE();
        /// <summary>
        /// Детектор
        /// </summary>
        public static SPI_DETECTOR Detector = new SPI_DETECTOR();
        /// <summary>
        /// Сканер
        /// </summary>
        public static SPI_SCANER Scaner = new SPI_SCANER();//У Сканера и Конденсатора DAC'и AD5643R и один общий ADC
        /// <summary>
        /// Конденсатор
        /// </summary>
        public static SPI_CONDENSATOR Condensator = new SPI_CONDENSATOR();
        #endregion
        #region----------------------------------ВИДИМЫЕ ФУНКЦИИ---------------------------------------
        /// <summary>
        /// Возвращает List(string) всех возникших ошибок (В РАЗРАБОТКЕ...)
        /// </summary>
        public static List<string> getErrorList()
        {
            //ФУНКЦИЯ: Возвращает лист ошибок, которые произошли во время сеанса.
            MC.Service.trace_attached(Environment.NewLine);
            MC.Service.trace(".getErrorList()");
            return ErrorList;
        }
        /// <summary>
        /// Задаёт СОМ-порт для связи ПК с МК
        /// </summary>
        public static void setUSART(SerialPort COM_PORT)
        {
            //ФУНКЦИЯ: Задаём порт, припомози которого будем общаться с МК
            MC.Service.trace_attached(Environment.NewLine);
            MC.Service.trace(".setUSART(" + COM_PORT.PortName + ")");
            USART = COM_PORT;
            USART_defined = true;
            if (!USART.IsOpen)
            {
                USART.Open();
            }

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
        public static byte setFlags(bool set_flags, bool PRGE, bool EDCD, bool SEMV1, bool SEMV2, bool SEMV3, bool SPUMP)
        {
            //ФУНКЦИЯ: Выставляет флаги в соответствии с принятым байтом, если первый байт 1, и возвращает результат. Иначе просто возвращает флаги
            //ДАННЫЕ: <Command><[Проверить\Установить][iHVE][PRGE][iEDCD][SEMV1][SEMV2][SEMV3][SPUMP]>
            //				Если первый бит <Проверить\Установить> = 0, то МК тут же возвращает текущее состояние флагов
            //				Если первый бит <Проверить\Установить> = 1, то МК устанавливает флаги (кроме iHVE) и возвращает их.
            //				iHVE - только чтение
            //				Ответ МК битом [Проверить\Установить] -> 1 - параметры были изменены, 0 - нечего менять
            string command = "setFlags( set = " + Convert.ToInt16(set_flags) + " | iHVE(readOnly) | PRGE = " + Convert.ToInt16(PRGE) + " | EDCD = " + Convert.ToInt16(EDCD) + " | SEMV1 = " + Convert.ToInt16(SEMV1) + " | SEMV2 = " + Convert.ToInt16(SEMV2) + " | SEMV3 = " + Convert.ToInt16(SEMV3) + " | SPUMP = " + Convert.ToInt16(SPUMP) + ")";
            MC.Service.trace_attached(Environment.NewLine);
            MC.Service.trace(command);
            byte flags = Convert.ToByte(Convert.ToInt16(set_flags) * 128 + Convert.ToInt16(PRGE) * 32 + Convert.ToInt16(EDCD) * 16 + Convert.ToInt16(SEMV1) * 8 + Convert.ToInt16(SEMV2) * 4 + Convert.ToInt16(SEMV3) * 2 + Convert.ToInt16(SPUMP));
            List<byte> rDATA;
            rDATA = MC.Service.transmit(Command.Flags.setFlags, flags);
            try
            {
                if ((rDATA[1] & 128) == 0) { MC.Service.trace(command + ": Операция отменена! Нечего менять!"); }
                return rDATA[1];
            }
            catch { MC.Service.addError(command + ": Ошибка данных!", rDATA); return byte.MaxValue; }
        }
        #endregion
        #region---------------------------------ВНУТРЕННИЕ ФУНКЦИИ-------------------------------------
        //ОТЛАДОЧНЫЕ
        static bool defineError(byte[] DATA)
        {
            //ФУНКЦИЯ: Дешефрирует ошибку
            MC.Service.trace("---------------ОШИБКА МК-------------");
            MC.Service.trace("Принятые данные:");
            foreach (byte b in DATA)
            {

                MC.Service.trace("     " + b);
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
                                        MC.Service.addError(" !! МК СООБЩАЕТ ОБ ОШИБКЕ ДЕКОДЕРА! Неизвестная команда: " + DATA[2], DATA);
                                        break;
                                    case Error.CheckSum:
                                        MC.Service.addError(" !! МК СООБЩАЕТ ОБ ОШИБКЕ! Неверная контрольная сумма: " + DATA[2], DATA);
                                        break;
                                    default:
                                        MC.Service.addError(" !!! МК сообщает о неизвестной ОШИБКЕ № " + DATA[1] + "!", DATA);
                                        MC.Service.trace("--------------Конец ошибки---------------");
                                        return false;
                                }
                                MC.Service.trace("--------------Конец ошибки---------------");
                                return true;
                            }
                            else
                            {
                                MC.Service.addError(" !!! НЕИЗВЕСТНАЯ ОШИБКА!", DATA);
                                MC.Service.trace("--------------Конец ошибки---------------");
                                return false;
                            }
                    }
                }
                else
                {
                    MC.Service.addError(" !!! Неверное сообщение об ошибке! Отсутствует метка ошибки!", DATA);
                    MC.Service.trace("--------------Конец ошибки---------------");
                    return false;
                }
            }
            else
            {
                MC.Service.addError(" !!! Слишком короткое сообщение об ошибке!", DATA);
                MC.Service.trace("--------------Конец ошибки---------------");
                return false;
            }
        }
        //   ! Р А З О Б Р А Т Ь ! Неиспользуемые функции
        static void trace_error(byte[] DATA)
        {
            //ФУНКЦИЯ: Определение что за неверные данные такие пришли, и что нужно делать
            //сохраним данные в log
            MC.Service.trace("-----------------ОШИБКА!-----------------");
            MC.Service.trace("! Принятые данные:");
            foreach (byte b in DATA)
            {
                MC.Service.trace("!     ");
            }
            MC.Service.trace("--------------Конец ошибки---------------");
        }
        static void wait()
        {
            //ФУНКЦИЯ: Устанавливает статус МК
            byte[] wDATA = { Command.Chip.wait };
            USART.Open();
            USART.Write(wDATA, 0, 1);
        }
        static bool reset()
        {
            //ФУНКЦИЯ: Програмная перезагрузка микроконтроллера
            return (MC.Service.transmit(Command.Chip.reset)[1] == Command.Chip.reset);
        }
        #endregion
        //--------------------------------------ЗАМЕТКИ-------------------------------------------
    }
    //---------------------------------------THE END------------------------------------------
}
