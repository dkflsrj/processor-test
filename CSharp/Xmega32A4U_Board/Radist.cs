using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace Xmega32A4U_testBoard
{
    class Radist
    {
        #region ------------------------------ОПИСАНИЕ-----------------------------
        //КЛАСС: Класс преобразующий данные в пакет по протоколу Радист-1 и обратно
        #region ---------------------------ПРОТОКОЛ "РАДИСТ"-----------------------
        //Байты <250>, <251> и <252> запрещены.
        //      <key>  - Ключ   - открывает передачу - <250>
        //      <lock> - Затвор - завершает передачу - <251>
        //      <door> - Дверь  - командный байт     - <252>
        //          Модифицирует следующий за дверью байт
        //          Дешифровка модификаций:
        //              <door><0> = <250>
        //              <door><1> = <251>
        //              <door><2> = <252>
        #endregion
        #endregion
        static string version = "Radist v2 Simpler";
        static string birthday = "29.04.2014";
        #region -------------------------------ПАМЯТЬ------------------------------
        #region ДЕЛЕГАЦИЯ
        public delegate void Error(string text);
        public static Error Error_messager;
        static class ErrorMessage
        {
            //КЛАСС: Содержит тексты сообщений об ошибках
            public static string NotInitialized = "Ошибка!\r\nRadist: Радист не инициализован!";
            public static string ByteToRyte = "Ошибка!\r\nRadist: Неверные входные данные! Ожидалось: 0...249. Получено: ";
            public static string WrongInputParameterWhileInitialization = "Ошибка!\r\nRadist: Неверный входной параметр!\r\n[BTN_setParameters_Click_function()]";
        }
        #endregion
        #region ФОРМА
        static ToolStripMenuItem MainButton = new ToolStripMenuItem("Radist", null, showForm);  //Встраиваемая кнопка для вызова меню Радиста
        static Form RadistForm;
        static Label LBL_State;
        static Label LBL_Name;
        static Label LBL_baudRate;
        static Label LBL_DataBits;
        static Label LBL_Parity;
        static Label LBL_StopBits;
        static Label LBL_Handshake;
        static ComboBox CMB_Name;
        static ComboBox CMB_BaudRate;
        static ComboBox CMB_DataBits;
        static ComboBox CMB_Parity;
        static ComboBox CMB_StopBits;
        static ComboBox CMB_Handshake;
        static Button BTN_setParameters;
        static Button BTN_look_around;
        #endregion
        public static SerialPort COM_port;                  //СОМ-порт
        static System.Timers.Timer Synchro = new System.Timers.Timer();                 //Таймер синхронного приёма
        static System.Timers.Timer Receiver = new System.Timers.Timer();                //Таймер ожидания следующего байта
        volatile static bool time_is_out = false;           //Булка таймера - время ожидания вышло
        volatile static bool data_received = false;         //Булка приёма данных - данные получены
        volatile static bool packet_receiving = false;      //Булка приёма данных - принимаем пакет
        volatile static bool door_exists = false;           //Булка приёма данных - получена дверь
        static List<byte> bufPacket = new List<byte>();     //Буферный пакет на приёме
        static List<List<byte>> rPackets = new List<List<byte>>(); //Принятые пакеты во время одной передачи
        static List<byte> synPacket = new List<byte>();     //Ответный пакет синхронной передачи
        //static List<byte> rPacket = new List<byte>();       //Принятый пакет
        static List<byte> Dummy = new List<byte>();         //Пустой пакет
        static byte initialized = 0;                        //0 - Радист не инициализован; 1 - инициализован; 2 - ошибка инизиализации; 3 - нет портов
        const byte Key = 250;                               //Байт-ключ протокола Радист
        const byte Lock = 251;                              //Байт-затвор протокола Радист
        const byte Door = 252;                              //Байт-дверь протокола Радист
        const byte Door_Key = 0;
        const byte Door_Lock = 1;
        const byte Door_Door = 2;

        const byte TOCKEN_Asynchro = 0;                     //Метка асинхронного сообщения
        #endregion
        #region ---------------------------ИНИЦИАЛИЗАЦИЯ---------------------------
        public static void initialize(string COM_port_Name, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            try
            {
                if (COM_port != null)
                {
                    if (COM_port.IsOpen)
                    {
                        COM_port.Close();
                    }
                }
                COM_port = new SerialPort(COM_port_Name, baudRate, parity, dataBits, stopBits);
                COM_port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
                if (!COM_port.IsOpen)
                {
                    COM_port.Open();
                }
                initialized = 1;
                Receiver.Interval = 10;
                Receiver.Elapsed += new System.Timers.ElapsedEventHandler(Receiver_time_out);
                Receiver.AutoReset = false;
                Synchro.Interval = 50;
                Synchro.Elapsed += new System.Timers.ElapsedEventHandler(Synchro_time_out);
                Synchro.AutoReset = false;
            }
            catch (Exception exception)
            {
                Error_messager.Invoke("Ошибка инициализации!\r\nRadist: \r\n" + exception.Message);
                initialized = 2;
            }
        }
        #endregion
        #region ------------------------ВНУТРЕННИЕ ФУНКЦИИ-------------------------
        #region ФОРМА
        static void showForm(object sender, EventArgs e)
        {
            //ФУНКЦИЯ: Создаёт и отображает форму
            #region Форма
            RadistForm = new Form();
            RadistForm.Text = version;
            RadistForm.Size = new System.Drawing.Size(270, 300);
            RadistForm.MinimumSize = RadistForm.Size;
            RadistForm.MaximumSize = RadistForm.Size;
            #endregion
            #region Надписи
            LBL_State = new Label();
            LBL_Name = new Label();
            LBL_baudRate = new Label();
            LBL_DataBits = new Label();
            LBL_Parity = new Label();
            LBL_StopBits = new Label();
            LBL_Handshake = new Label();
            LBL_Name.Text = "COM-порт:";
            LBL_baudRate.Text = "Бод в секунду:";
            LBL_DataBits.Text = "Бит данных:";
            LBL_Parity.Text = "Чётность:";
            LBL_StopBits.Text = "Стоповые биты:";
            LBL_Handshake.Text = "Управление потоком:";
            Label[] LBLs = new Label[6] { LBL_Name, LBL_baudRate, LBL_DataBits, LBL_Parity, LBL_StopBits, LBL_Handshake };
            int y = 10;
            foreach (Label lbl in LBLs)
            {
                lbl.Width = 130;
                lbl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                lbl.Location = new System.Drawing.Point(0, y);
                y += 25;
                RadistForm.Controls.Add(lbl);
            }
            RadistForm_update_LBL_State();
            LBL_State.TextAlign = ContentAlignment.MiddleCenter;
            LBL_State.Width = 250;
            LBL_State.Location = new System.Drawing.Point(0, 210);
            RadistForm.Controls.Add(LBL_State);
            #endregion
            #region Поля выбора
            CMB_Name = new ComboBox();
            CMB_BaudRate = new ComboBox();
            CMB_DataBits = new ComboBox();
            CMB_Parity = new ComboBox();
            CMB_StopBits = new ComboBox();
            CMB_Handshake = new ComboBox();
            CMB_Name.Text = "";
            CMB_BaudRate.Text = "9600";
            CMB_BaudRate.Items.AddRange(new string[] { "4800", "9600", "14400", "19200", "38400", "57600", "115200", "128000" });
            CMB_DataBits.Text = "8";
            CMB_DataBits.Items.AddRange(new string[] { "4", "5", "6", "7", "8" });
            CMB_Parity.Text = "Нет";
            CMB_Parity.Items.AddRange(new string[] { "Чётный", "Нечётный", "Нет", "Маркер", "Пробел" });
            CMB_StopBits.Text = "1";
            CMB_StopBits.Items.AddRange(new string[] { "1", "1.5", "2", "Нет" });
            CMB_Handshake.Text = "Нет";
            CMB_Handshake.Enabled = false;
            CMB_Handshake.Items.AddRange(new string[] { "Xon / Xoff", "Аппаратное", "Нет" });
            ComboBox[] CMBs = new ComboBox[6] { CMB_Name, CMB_BaudRate, CMB_DataBits, CMB_Parity, CMB_StopBits, CMB_Handshake };
            y = 12;
            foreach (ComboBox cmb in CMBs)
            {
                cmb.Width = 100;
                cmb.Location = new System.Drawing.Point(140, y);
                y += 25;
                RadistForm.Controls.Add(cmb);
            }
            #endregion
            #region Кнопки
            BTN_look_around = new Button();
            BTN_setParameters = new Button();
            BTN_look_around.Text = "Найти COM-порты";
            BTN_look_around.Location = new System.Drawing.Point(10, 170);
            BTN_look_around.Size = new System.Drawing.Size(110, 25);
            BTN_look_around.Click += new EventHandler(BTN_look_around_Click);
            RadistForm.Controls.Add(BTN_look_around);
            BTN_setParameters.Text = "Задать параметры";
            BTN_setParameters.Location = new System.Drawing.Point(125, 170);
            BTN_setParameters.Size = new System.Drawing.Size(115, 25);
            BTN_setParameters.Click += new EventHandler(BTN_setParameters_Click);
            BTN_setParameters.Enabled = false;
            RadistForm.Controls.Add(BTN_setParameters);
            #endregion
            RadistForm.Show();
            BTN_look_around_Click_function();
        }
        static void BTN_look_around_Click(object sender, EventArgs e)
        {
            //СОБЫТИЕ: Нажата кнопка "Найти COM-порты"
            BTN_look_around_Click_function();
        }
        static void BTN_look_around_Click_function()
        {
            //ФУНКЦИЯ: Все найденные порты засовываем в поле списка портов и выбираем первый
            CMB_Name.Items.Clear();
            string[] PortNames = SerialPort.GetPortNames();
            if (PortNames.Length > 0)
            {
                CMB_Name.Items.AddRange(PortNames);
                CMB_Name.Text = PortNames.First();
                BTN_setParameters.Enabled = true;
                if (initialized != 1)
                {
                    initialized = 0;
                }
            }
            else
            {
                initialized = 3;
                BTN_setParameters.Enabled = false;
            }
            RadistForm_update_LBL_State();
        }
        static void BTN_setParameters_Click(object sender, EventArgs e)
        {
            //СОБЫТИЕ: Нажата кнопка "Задать параметры"
            BTN_setParameters_Click_function();
        }
        static void BTN_setParameters_Click_function()
        {
            //ФУНКЦИЯ: Конфигурируем порт в соответствии с предпочтениями
            try
            {
                string PortName = CMB_Name.Text;
                int BaudRate = Convert.ToInt32(CMB_BaudRate.Text);
                int DataBits = Convert.ToInt32(CMB_DataBits.Text);
                StopBits stopBits = StopBits.One;
                switch (CMB_StopBits.Text)
                {
                    case "1": stopBits = StopBits.One;
                        break;
                    case "1.5": stopBits = StopBits.OnePointFive;
                        break;
                    case "2": stopBits = StopBits.Two;
                        break;
                    case "Нет": stopBits = StopBits.None;
                        break;
                }
                Parity parity = Parity.None;
                switch (CMB_Parity.Text)
                {
                    case "Чётный": parity = Parity.Even;
                        break;
                    case "Нечётный": parity = Parity.Odd;
                        break;
                    case "Нет": parity = Parity.None;
                        break;
                    case "Маркер": parity = Parity.Mark;
                        break;
                    case "Пробел": parity = Parity.Space;
                        break;
                }
                //Handshake handshake = Handshake.None;
                //switch (CMB_Handshake.Text)
                //{
                //    case "Xon / Xoff": handshake = Handshake.XOnXOff;
                //        break;
                //    case "Аппаратное": handshake = Handshake.RequestToSend;
                //        break;
                //    case "Нет": handshake = Handshake.None;
                //        break;
                //}
                initialize(PortName, BaudRate, parity, DataBits, stopBits);
            }
            catch
            {
                Error_messager.Invoke(ErrorMessage.WrongInputParameterWhileInitialization);
                initialized = 2;
            }
            RadistForm_update_LBL_State();
        }
        static void RadistForm_update_LBL_State()
        {
            switch(initialized)
            {
                case 0:LBL_State.Text = "COM-порт не задан!";
                    LBL_State.ForeColor = Color.Red;
                    break;
                case 1:LBL_State.Text = "COM-порт задан";
                    LBL_State.ForeColor = Color.DarkGreen;
                    break;
                case 2:LBL_State.Text = "Ошибка при попытке задать COM-порт!";
                    LBL_State.ForeColor = Color.Red;
                    break;
                case 3:LBL_State.Text = "Ни один СОМ-порт не найден!";
                    LBL_State.ForeColor = Color.Red;
                    break;
            }
        }
        #endregion
        static byte calcCheckSum(List<byte> Data)
        {
            //ФУНКЦИЯ: Принимает лист райтов, возвращает контрольную сумму пакета.
            byte check_sum = 0;
            foreach (byte b in Data)
            {
                check_sum -= b;
            }
            return check_sum;
        }
        static int ByteAfterDoor(byte received_byte_after_door)
        {
            switch (received_byte_after_door)
            {
                case Door_Key: return Key;
                case Door_Lock: return Lock;
                case Door_Door: return Door;
                default: return -1;
            }
        }
        static string a = "";
        static void DataReceived(object sender, EventArgs e)
        {
            //СОБЫТИЕ: Есть байты в буфере порта
            //ФУНКЦИЯ: Прочесать буфер и если есть байт ключ - начать приём до получения байта затвора.
            //          Если в буфере, всё ещё есть байты принять их в другой массив.
            //Принимаем всё что есть
            bool traced = false;
            Receiver.Stop();
            while (COM_port.BytesToRead > 0)
            {
                int buffer = COM_port.ReadByte();
                a += buffer + " | ";
                switch(buffer)
                {
                    case Key: packet_receiving = true;
                        break;
                    case Lock: data_received = true;
                        break;
                    case Door: door_exists = true;
                        break;
                }
                //Если мы в режиме приёма - записываем байт в буфер
                if (packet_receiving) 
                {
                    if (door_exists)
                    {
                        buffer = ByteAfterDoor((byte)buffer);
                        if (buffer < 0)
                        {
                            MC.Service.trace("Ахтунг! Байт после двери неверный!");
                            packet_receiving = false;           //выходим из режима приёма
                            bufPacket.Clear();                  //Очищаем буфер
                            data_received = false;              //Ждём следующий пакет
                            return;
                        }
                        door_exists = false;
                    }
                    bufPacket.Add((byte)buffer); 
                }
                //Если мы получили затвор, то
                if (data_received)
                {
                    packet_receiving = false;           //выходим из режима приёма
                    //копируем буферный пакет в список принятых пакетов
                    rPackets.Add(new List<byte>());
                    foreach (byte b in bufPacket)
                    {
                        rPackets.Last().Add(b);
                    }
                    bufPacket.Clear();                  //Очищаем буфер
                    data_received = false;              //Ждём следующий пакет
                }
            }
            //Если буферный пакет не завершён (то есть не чист), то 
            if (bufPacket.Count > 0)
            {
                Receiver.Start();                       //запускаем таймаут по приёму байтов
            }
            //Если мы в режиме синхронной передачи и получен хотябы один завершённый пакет
            if (Synchro.Enabled)
            {
                if (rPackets.Count > 0)
                {
                    //Необходимо установить есть ли среди принятых пакетов хотябы один синхронный
                    int number_of_packet = 0;
                    int i = 0;
                    foreach (List<byte> packet in rPackets)
                    {
                        if (packet[1] != 0)
                        {
                            number_of_packet = i;
                            synPacket.Clear();
                            foreach (byte b in packet)
                            {
                                synPacket.Add(b);
                            }
                            break;
                        }
                        i++;
                    }
                    rPackets.RemoveAt(number_of_packet);
                    Synchro.Stop();                         //Останавливаем таймер - передача завершена
                }
            }
            //Если пакеты ещё остались - обрабатываем их как асинхронные
            foreach (List<byte> packet in rPackets)
            {
                if (packet.Count > 0)
                {
                    Asynchro_decode(packet);
                }
            }
            rPackets.Clear();
            MC.Service.trace("->[" + a.Substring(0, a.Length - 3) + "]");
            a = "";
        }
        static void Asynchro_decode(List<byte> packet)
        {
            if (packet[0] == TOCKEN_Asynchro)
            {
                switch (packet[1])
                {
                    case 1: MC.Service.trace("МК сообщает о неверной контрольной сумме!");
                        break;
                    case 2: MC.Service.trace("МК сообщает о неверных комплиментах пакета!");
                        break;
                    case 3: MC.Service.trace("МК сообщает о слишком коротком пакете!");
                        break;
                    case 10: MC.Service.trace("МК сообщает о том, что запрашиваемая команда не существует!");
                        break;
                }
            }
            else
            {
                MC.Service.trace("Синхронное сообщение было получено асинхронным образом:");
            }
        }
        static void Receiver_time_out(object sender, System.Timers.ElapsedEventArgs e)
        {
            //СОБЫТИЕ: Таймер по приёму байтов не дождался следующего байта.
            bufPacket.Clear();                  //Очищаем буфер.
        }
        static void Synchro_time_out(object sender, System.Timers.ElapsedEventArgs e)
        {
            time_is_out = true;
            packet_receiving = false;
        }
        #endregion
        #region --------------------------ВИДИМЫЕ ФУНКЦИИ--------------------------
        public static string[] look_around()
        {
            return SerialPort.GetPortNames();
        }
        public static List<byte> send(List<byte> Data)
        {
            //Составление байтов комплиментов и райтов
            if (initialized == 1)
            {
                synPacket.Clear();
                synPacket.Add(Key);
                Data.Add(calcCheckSum(Data));
                for (int i = 0; i < Data.Count; i++ )
                {
                    switch (Data[i])
                    {
                        case Key: synPacket.Add(Door);
                            synPacket.Add(Door_Key);
                            break;
                        case Lock: synPacket.Add(Door);
                            synPacket.Add(Door_Lock);
                            break;
                        case Door: synPacket.Add(Door);
                            synPacket.Add(Door_Door);
                            break;
                        default:
                            synPacket.Add(Data[i]);
                            break;
                    }
                }
                synPacket.Add(Lock);
                MC.Service.trace("<-[" + MC.Service.tracer_toString(synPacket) + "]");
                //Подготовка к передаче
                time_is_out = false;
                if (!COM_port.IsOpen)
                {
                    COM_port.Open();
                }
                foreach (byte r in synPacket)
                {
                    COM_port.Write(new byte[1] { r }, 0, 1);
                }
                synPacket.Clear();
                //Приём
                Synchro.Start();
                while (Synchro.Enabled) { }
                if ((!time_is_out)&&(synPacket.Count > 0))
                {
                    //string text = "->[" + MC.Service.tracer_toString(synPacket) + "]";
                    //MC.Service.trace(text);
                    //Tracer.next();
                    synPacket.RemoveAt(0);
                    synPacket.RemoveAt(synPacket.Count - 1);
                    if (calcCheckSum(synPacket) != 0)
                    {
                        MC.Service.trace("Ошибка контрольной суммы!");
                    }
                    return synPacket;
                }
                MC.Service.trace("Ошибка! Radist: время приёма вышло!");
                return Dummy;
            }
            Error_messager(ErrorMessage.NotInitialized);
            return Dummy;
        }
        public static class Service
        {
            public static ToolStripMenuItem Button
            {
                get { return MainButton; }
            }
            #region Таблица символов
            //          Цифры
            //           <0> = '0'
            //           <1> = '1'
            //           <2> = '2'
            //           <3> = '3'
            //           <4> = '4'
            //           <5> = '5'
            //           <6> = '6'
            //           <7> = '7'
            //           <8> = '8'
            //           <9> = '9'
            //          Сервисные символы
            //           <10> = / - символ команды, следующий байт будет расценен как командный (то есть + ещё 250 значений), не может быть последним байтом файла

            //          Символы
            //           <30> = /TAB - табуляция (многократный пробел)
            //           <31> = /NEW_LINE - новая строка 
            //           <32> = ' '
            //           <33> = '!'
            //           <34> = '"' - \"
            //           <35> = '#'
            //           <36> = '$'
            //           <37> = '%'
            //           <38> = '&' - & - но не отображается в Label'ах, нужно дубликатить
            //           <39> = '''
            //           <40> = ')'
            //           <41> = '('
            //           <42> = '*'
            //           <43> = '+'
            //           <44> = ','
            //           <45> = '-'
            //           <46> = '.'
            //           <47> = '/'

            //           <58> = ':'
            //           <59> = ';'
            //           <60> = '<'
            //           <61> = '='
            //           <62> = '>'
            //           <63> = '?'
            //           <64> = '@'
            //           <65> = '№'

            //           <91> = '['
            //           <92> = '\' - \\
            //           <93> = ']'
            //           <94> = '^'
            //           <95> = '_'

            //           <123> = '{'
            //           <124> = '|'
            //           <125> = '}'
            //           <126> = '~'
            //          Русский алфавит
            //           <128> = 'А'
            //           <129> = 'Б'
            //           <130> = 'В'
            //           <131> = 'Г'
            //           <132> = 'Д'
            //           <133> = 'Е'
            //           <134> = 'Ё'
            //           <135> = 'Ж'
            //           <136> = 'З'
            //           <137> = 'И'
            //           <138> = 'Й'
            //           <139> = 'К'
            //           <140> = 'Л'
            //           <141> = 'М'
            //           <142> = 'Н'
            //           <143> = 'О'
            //           <144> = 'П'
            //           <145> = 'Р'
            //           <146> = 'С'
            //           <147> = 'Т'
            //           <148> = 'У'
            //           <149> = 'Ф'
            //           <150> = 'Х'
            //           <151> = 'Ц'
            //           <152> = 'Ч'
            //           <153> = 'Ш'
            //           <154> = 'Щ'
            //           <155> = 'Ъ'
            //           <156> = 'Ы'
            //           <157> = 'Ь'
            //           <158> = 'Э'
            //           <159> = 'Ю'
            //           <160> = 'Я'
            //           <161> = 'а'
            //           <162> = 'б'
            //           <163> = 'в'
            //           <164> = 'г'
            //           <165> = 'д'
            //           <166> = 'е'
            //           <167> = 'ё'
            //           <168> = 'ж'
            //           <169> = 'з'
            //           <170> = 'и'
            //           <171> = 'й'
            //           <172> = 'к'
            //           <173> = 'л'
            //           <174> = 'м'
            //           <175> = 'н'
            //           <176> = 'о'
            //           <177> = 'п'
            //           <178> = 'р'
            //           <179> = 'с'
            //           <180> = 'т'
            //           <181> = 'у'
            //           <182> = 'ф'
            //           <183> = 'х'
            //           <184> = 'ц'
            //           <185> = 'ч'
            //           <186> = 'ш'
            //           <187> = 'щ'
            //           <188> = 'ъ'
            //           <189> = 'ы'
            //           <190> = 'ь'
            //           <191> = 'э'
            //           <192> = 'ю'
            //           <193> = 'я'
            //          Английский алфавит
            //           <194> = 'A'
            //           <195> = 'B'
            //           <196> = 'C'
            //           <197> = 'D'
            //           <198> = 'E'
            //           <199> = 'F'
            //           <200> = 'G'
            //           <201> = 'H'
            //           <202> = 'I'
            //           <203> = 'J'
            //           <204> = 'K'
            //           <205> = 'L'
            //           <206> = 'M'
            //           <207> = 'N'
            //           <208> = 'O'
            //           <209> = 'P'
            //           <210> = 'Q'
            //           <211> = 'R'
            //           <212> = 'S'
            //           <213> = 'T'
            //           <214> = 'U'
            //           <215> = 'V'
            //           <216> = 'W'
            //           <217> = 'X'
            //           <218> = 'Y'
            //           <219> = 'Z'
            //           <220> = 'a'
            //           <221> = 'b'
            //           <222> = 'c'
            //           <223> = 'd'
            //           <224> = 'e'
            //           <225> = 'f'
            //           <226> = 'g'
            //           <227> = 'h'
            //           <228> = 'i'
            //           <229> = 'j'
            //           <230> = 'k'
            //           <231> = 'l'
            //           <232> = 'm'
            //           <233> = 'n'
            //           <234> = 'o'
            //           <235> = 'p'
            //           <236> = 'q'
            //           <237> = 'r'
            //           <238> = 's'
            //           <239> = 't'
            //           <240> = 'u'
            //           <241> = 'v'
            //           <242> = 'w'
            //           <243> = 'x'
            //           <244> = 'y'
            //           <245> = 'z'

            //Отсутствующие символы заменяются сочетанием '??'
            #endregion
            static string Table_in_string = "0123456789????????????????????\t\n !\"#$%&')(*+,-./??????????:;<=>?@№?????????????????????????[\\]^_???????????????????????????{|}~?АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюяABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz????";
            
            public static string BytesToString(List<byte> Bytes)
            {
                string answer = "";
                foreach (byte b in Bytes)
                {
                    if (b > 249)
                    {
                        return "";
                    }
                    answer += Table_in_string[b];
                }
                return answer;
            }
            public static List<byte> StringToBytes(string Text)
            {
                List<byte> answer = new List<byte>();
                foreach(char ch in Text)
                {
                    answer.Add((byte)Table_in_string.IndexOf(ch));
                }
                return answer;
            }
        }
        #endregion
    }
}
