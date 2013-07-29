namespace Xmega32A4U_testBoard
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Log = new System.Windows.Forms.RichTextBox();
            this.BTN_COM_find = new System.Windows.Forms.Button();
            this.TABpanel = new System.Windows.Forms.TabControl();
            this.TABpanel_1 = new System.Windows.Forms.TabPage();
            this.cBox_COM = new System.Windows.Forms.ComboBox();
            this.CMB_COM_Handshake = new System.Windows.Forms.ComboBox();
            this.CMB_COM_Parity = new System.Windows.Forms.ComboBox();
            this.CMB_COM_BaudRate = new System.Windows.Forms.ComboBox();
            this.CMB_COM_StopBits = new System.Windows.Forms.ComboBox();
            this.CMB_COM_DataBits = new System.Windows.Forms.ComboBox();
            this.lable_COM_Handshake = new System.Windows.Forms.Label();
            this.lable_COM_StopBits = new System.Windows.Forms.Label();
            this.lable_COM_DataBits = new System.Windows.Forms.Label();
            this.lable_COM_Parity = new System.Windows.Forms.Label();
            this.lable_COM_BaudRate = new System.Windows.Forms.Label();
            this.lable_COM = new System.Windows.Forms.Label();
            this.BTN_COM_setParams = new System.Windows.Forms.Button();
            this.TABpanel_2 = new System.Windows.Forms.TabPage();
            this.BTN_reqCount = new System.Windows.Forms.Button();
            this.BTN_setInterval = new System.Windows.Forms.Button();
            this.BTN_stopCounter = new System.Windows.Forms.Button();
            this.BTN_startCounter = new System.Windows.Forms.Button();
            this.BTN_COM_setMCwait = new System.Windows.Forms.Button();
            this.BTN_SPI_ADC_request = new System.Windows.Forms.Button();
            this.BTN_SPI_DAC_send = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CHB_COM_DC_recieveDATA = new System.Windows.Forms.CheckBox();
            this.CHB_COM_DC_sendDATA = new System.Windows.Forms.CheckBox();
            this.CHB_COM_DC_recieveResponse = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.BTN_COM_DC_send = new System.Windows.Forms.Button();
            this.TXB_COM_DC_recieveDATA_count = new System.Windows.Forms.TextBox();
            this.TXB_COM_DC_sendDATA = new System.Windows.Forms.TextBox();
            this.TXB_COM_DC_command = new System.Windows.Forms.TextBox();
            this.TXB_interval = new System.Windows.Forms.TextBox();
            this.TXB_DAC_voltage = new System.Windows.Forms.TextBox();
            this.TXB_DAC_channel = new System.Windows.Forms.TextBox();
            this.TXB_ADC_channel = new System.Windows.Forms.TextBox();
            this.TXB_LEDbyte = new System.Windows.Forms.TextBox();
            this.BTN_LEDbyte = new System.Windows.Forms.Button();
            this.BTN_COM_getMCversion = new System.Windows.Forms.Button();
            this.BTN_COM_MC_CPUfreq = new System.Windows.Forms.Button();
            this.BTN_MCstatus = new System.Windows.Forms.Button();
            this.Hinter = new System.Windows.Forms.ToolTip(this.components);
            this.CHB_ADC_DoubleRange = new System.Windows.Forms.CheckBox();
            this.BTN_DAC_reset = new System.Windows.Forms.Button();
            this.GRB_MC = new System.Windows.Forms.GroupBox();
            this.GRB_SPI = new System.Windows.Forms.GroupBox();
            this.GRB_Counter = new System.Windows.Forms.GroupBox();
            this.TABpanel.SuspendLayout();
            this.TABpanel_1.SuspendLayout();
            this.TABpanel_2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.GRB_MC.SuspendLayout();
            this.GRB_SPI.SuspendLayout();
            this.GRB_Counter.SuspendLayout();
            this.SuspendLayout();
            // 
            // Log
            // 
            this.Log.Location = new System.Drawing.Point(12, 385);
            this.Log.Name = "Log";
            this.Log.Size = new System.Drawing.Size(560, 165);
            this.Log.TabIndex = 0;
            this.Log.Text = "";
            // 
            // BTN_COM_find
            // 
            this.BTN_COM_find.BackColor = System.Drawing.SystemColors.Control;
            this.BTN_COM_find.Location = new System.Drawing.Point(291, 27);
            this.BTN_COM_find.Name = "BTN_COM_find";
            this.BTN_COM_find.Size = new System.Drawing.Size(101, 26);
            this.BTN_COM_find.TabIndex = 0;
            this.BTN_COM_find.Text = "Найти COM-Port";
            this.BTN_COM_find.UseVisualStyleBackColor = false;
            this.BTN_COM_find.Click += new System.EventHandler(this.BTN_COM_find_Click);
            // 
            // TABpanel
            // 
            this.TABpanel.Controls.Add(this.TABpanel_1);
            this.TABpanel.Controls.Add(this.TABpanel_2);
            this.TABpanel.Location = new System.Drawing.Point(12, 12);
            this.TABpanel.Name = "TABpanel";
            this.TABpanel.SelectedIndex = 0;
            this.TABpanel.Size = new System.Drawing.Size(560, 346);
            this.TABpanel.TabIndex = 2;
            // 
            // TABpanel_1
            // 
            this.TABpanel_1.BackColor = System.Drawing.SystemColors.Control;
            this.TABpanel_1.Controls.Add(this.cBox_COM);
            this.TABpanel_1.Controls.Add(this.CMB_COM_Handshake);
            this.TABpanel_1.Controls.Add(this.CMB_COM_Parity);
            this.TABpanel_1.Controls.Add(this.CMB_COM_BaudRate);
            this.TABpanel_1.Controls.Add(this.CMB_COM_StopBits);
            this.TABpanel_1.Controls.Add(this.CMB_COM_DataBits);
            this.TABpanel_1.Controls.Add(this.lable_COM_Handshake);
            this.TABpanel_1.Controls.Add(this.lable_COM_StopBits);
            this.TABpanel_1.Controls.Add(this.lable_COM_DataBits);
            this.TABpanel_1.Controls.Add(this.lable_COM_Parity);
            this.TABpanel_1.Controls.Add(this.lable_COM_BaudRate);
            this.TABpanel_1.Controls.Add(this.lable_COM);
            this.TABpanel_1.Controls.Add(this.BTN_COM_setParams);
            this.TABpanel_1.Controls.Add(this.BTN_COM_find);
            this.TABpanel_1.Location = new System.Drawing.Point(4, 22);
            this.TABpanel_1.Name = "TABpanel_1";
            this.TABpanel_1.Padding = new System.Windows.Forms.Padding(3);
            this.TABpanel_1.Size = new System.Drawing.Size(552, 320);
            this.TABpanel_1.TabIndex = 0;
            this.TABpanel_1.Text = "COM-Port";
            // 
            // cBox_COM
            // 
            this.cBox_COM.FormattingEnabled = true;
            this.cBox_COM.Location = new System.Drawing.Point(144, 31);
            this.cBox_COM.Name = "cBox_COM";
            this.cBox_COM.Size = new System.Drawing.Size(121, 21);
            this.cBox_COM.TabIndex = 2;
            // 
            // CMB_COM_Handshake
            // 
            this.CMB_COM_Handshake.FormattingEnabled = true;
            this.CMB_COM_Handshake.Items.AddRange(new object[] {
            "Xon / Xoff",
            "Аппаратное",
            "Нет"});
            this.CMB_COM_Handshake.Location = new System.Drawing.Point(144, 166);
            this.CMB_COM_Handshake.Name = "CMB_COM_Handshake";
            this.CMB_COM_Handshake.Size = new System.Drawing.Size(121, 21);
            this.CMB_COM_Handshake.TabIndex = 2;
            this.CMB_COM_Handshake.Text = "Нет";
            // 
            // CMB_COM_Parity
            // 
            this.CMB_COM_Parity.FormattingEnabled = true;
            this.CMB_COM_Parity.Items.AddRange(new object[] {
            "Чёт",
            "Нечёт",
            "Нет",
            "Маркер",
            "Пробел"});
            this.CMB_COM_Parity.Location = new System.Drawing.Point(144, 112);
            this.CMB_COM_Parity.Name = "CMB_COM_Parity";
            this.CMB_COM_Parity.Size = new System.Drawing.Size(121, 21);
            this.CMB_COM_Parity.TabIndex = 2;
            this.CMB_COM_Parity.Text = "Нет";
            // 
            // CMB_COM_BaudRate
            // 
            this.CMB_COM_BaudRate.FormattingEnabled = true;
            this.CMB_COM_BaudRate.Items.AddRange(new object[] {
            "75",
            "110",
            "134",
            "150",
            "300",
            "600",
            "1200",
            "1800",
            "2400",
            "4800",
            "7200",
            "9600",
            "14400",
            "19200",
            "38400",
            "57600",
            "115200",
            "128000"});
            this.CMB_COM_BaudRate.Location = new System.Drawing.Point(144, 58);
            this.CMB_COM_BaudRate.Name = "CMB_COM_BaudRate";
            this.CMB_COM_BaudRate.Size = new System.Drawing.Size(121, 21);
            this.CMB_COM_BaudRate.TabIndex = 2;
            this.CMB_COM_BaudRate.Text = "128000";
            // 
            // CMB_COM_StopBits
            // 
            this.CMB_COM_StopBits.FormattingEnabled = true;
            this.CMB_COM_StopBits.Items.AddRange(new object[] {
            "1",
            "1.5",
            "2",
            "Нет"});
            this.CMB_COM_StopBits.Location = new System.Drawing.Point(144, 139);
            this.CMB_COM_StopBits.Name = "CMB_COM_StopBits";
            this.CMB_COM_StopBits.Size = new System.Drawing.Size(121, 21);
            this.CMB_COM_StopBits.TabIndex = 2;
            this.CMB_COM_StopBits.Text = "1";
            // 
            // CMB_COM_DataBits
            // 
            this.CMB_COM_DataBits.FormattingEnabled = true;
            this.CMB_COM_DataBits.Items.AddRange(new object[] {
            "4",
            "5",
            "6",
            "7",
            "8"});
            this.CMB_COM_DataBits.Location = new System.Drawing.Point(144, 85);
            this.CMB_COM_DataBits.Name = "CMB_COM_DataBits";
            this.CMB_COM_DataBits.Size = new System.Drawing.Size(121, 21);
            this.CMB_COM_DataBits.TabIndex = 2;
            this.CMB_COM_DataBits.Text = "8";
            // 
            // lable_COM_Handshake
            // 
            this.lable_COM_Handshake.AutoSize = true;
            this.lable_COM_Handshake.Location = new System.Drawing.Point(16, 169);
            this.lable_COM_Handshake.Name = "lable_COM_Handshake";
            this.lable_COM_Handshake.Size = new System.Drawing.Size(118, 13);
            this.lable_COM_Handshake.TabIndex = 1;
            this.lable_COM_Handshake.Text = "Управление потоком:";
            // 
            // lable_COM_StopBits
            // 
            this.lable_COM_StopBits.AutoSize = true;
            this.lable_COM_StopBits.Location = new System.Drawing.Point(46, 142);
            this.lable_COM_StopBits.Name = "lable_COM_StopBits";
            this.lable_COM_StopBits.Size = new System.Drawing.Size(88, 13);
            this.lable_COM_StopBits.TabIndex = 1;
            this.lable_COM_StopBits.Text = "Стоповые биты:";
            // 
            // lable_COM_DataBits
            // 
            this.lable_COM_DataBits.AutoSize = true;
            this.lable_COM_DataBits.Location = new System.Drawing.Point(58, 88);
            this.lable_COM_DataBits.Name = "lable_COM_DataBits";
            this.lable_COM_DataBits.Size = new System.Drawing.Size(76, 13);
            this.lable_COM_DataBits.TabIndex = 1;
            this.lable_COM_DataBits.Text = "Биты данных:";
            // 
            // lable_COM_Parity
            // 
            this.lable_COM_Parity.AutoSize = true;
            this.lable_COM_Parity.Location = new System.Drawing.Point(76, 115);
            this.lable_COM_Parity.Name = "lable_COM_Parity";
            this.lable_COM_Parity.Size = new System.Drawing.Size(58, 13);
            this.lable_COM_Parity.TabIndex = 1;
            this.lable_COM_Parity.Text = "Чётность:";
            // 
            // lable_COM_BaudRate
            // 
            this.lable_COM_BaudRate.AutoSize = true;
            this.lable_COM_BaudRate.Location = new System.Drawing.Point(54, 61);
            this.lable_COM_BaudRate.Name = "lable_COM_BaudRate";
            this.lable_COM_BaudRate.Size = new System.Drawing.Size(80, 13);
            this.lable_COM_BaudRate.TabIndex = 1;
            this.lable_COM_BaudRate.Text = "Бит в секунду:";
            // 
            // lable_COM
            // 
            this.lable_COM.AutoSize = true;
            this.lable_COM.Location = new System.Drawing.Point(74, 34);
            this.lable_COM.Name = "lable_COM";
            this.lable_COM.Size = new System.Drawing.Size(60, 13);
            this.lable_COM.TabIndex = 1;
            this.lable_COM.Text = "COM порт:";
            // 
            // BTN_COM_setParams
            // 
            this.BTN_COM_setParams.BackColor = System.Drawing.SystemColors.Control;
            this.BTN_COM_setParams.Location = new System.Drawing.Point(79, 193);
            this.BTN_COM_setParams.Name = "BTN_COM_setParams";
            this.BTN_COM_setParams.Size = new System.Drawing.Size(121, 35);
            this.BTN_COM_setParams.TabIndex = 0;
            this.BTN_COM_setParams.Text = "Задать параметры COM-Port";
            this.BTN_COM_setParams.UseVisualStyleBackColor = false;
            this.BTN_COM_setParams.Click += new System.EventHandler(this.BTN_COM_setParams_Click);
            // 
            // TABpanel_2
            // 
            this.TABpanel_2.BackColor = System.Drawing.SystemColors.Control;
            this.TABpanel_2.Controls.Add(this.GRB_Counter);
            this.TABpanel_2.Controls.Add(this.GRB_SPI);
            this.TABpanel_2.Controls.Add(this.GRB_MC);
            this.TABpanel_2.Controls.Add(this.groupBox1);
            this.TABpanel_2.Location = new System.Drawing.Point(4, 22);
            this.TABpanel_2.Name = "TABpanel_2";
            this.TABpanel_2.Padding = new System.Windows.Forms.Padding(3);
            this.TABpanel_2.Size = new System.Drawing.Size(552, 320);
            this.TABpanel_2.TabIndex = 1;
            this.TABpanel_2.Text = "XMega32A4U_testBoard";
            // 
            // BTN_reqCount
            // 
            this.BTN_reqCount.Location = new System.Drawing.Point(6, 77);
            this.BTN_reqCount.Name = "BTN_reqCount";
            this.BTN_reqCount.Size = new System.Drawing.Size(103, 23);
            this.BTN_reqCount.TabIndex = 7;
            this.BTN_reqCount.Text = "Проверить счёт";
            this.BTN_reqCount.UseVisualStyleBackColor = true;
            this.BTN_reqCount.Click += new System.EventHandler(this.BTN_reqCount_Click);
            // 
            // BTN_setInterval
            // 
            this.BTN_setInterval.Location = new System.Drawing.Point(6, 19);
            this.BTN_setInterval.Name = "BTN_setInterval";
            this.BTN_setInterval.Size = new System.Drawing.Size(103, 23);
            this.BTN_setInterval.TabIndex = 7;
            this.BTN_setInterval.Text = "Задать интервал";
            this.BTN_setInterval.UseVisualStyleBackColor = true;
            this.BTN_setInterval.Click += new System.EventHandler(this.BTN_setInterval_Click);
            // 
            // BTN_stopCounter
            // 
            this.BTN_stopCounter.Location = new System.Drawing.Point(115, 48);
            this.BTN_stopCounter.Name = "BTN_stopCounter";
            this.BTN_stopCounter.Size = new System.Drawing.Size(103, 23);
            this.BTN_stopCounter.TabIndex = 7;
            this.BTN_stopCounter.Text = "Остановить счёт";
            this.BTN_stopCounter.UseVisualStyleBackColor = true;
            this.BTN_stopCounter.Click += new System.EventHandler(this.BTN_stopCounter_Click);
            // 
            // BTN_startCounter
            // 
            this.BTN_startCounter.Location = new System.Drawing.Point(6, 48);
            this.BTN_startCounter.Name = "BTN_startCounter";
            this.BTN_startCounter.Size = new System.Drawing.Size(103, 23);
            this.BTN_startCounter.TabIndex = 7;
            this.BTN_startCounter.Text = "Начать счёт";
            this.BTN_startCounter.UseVisualStyleBackColor = true;
            this.BTN_startCounter.Click += new System.EventHandler(this.BTN_startCounter_Click);
            // 
            // BTN_COM_setMCwait
            // 
            this.BTN_COM_setMCwait.Location = new System.Drawing.Point(6, 48);
            this.BTN_COM_setMCwait.Name = "BTN_COM_setMCwait";
            this.BTN_COM_setMCwait.Size = new System.Drawing.Size(103, 23);
            this.BTN_COM_setMCwait.TabIndex = 7;
            this.BTN_COM_setMCwait.Text = "Ожидание";
            this.BTN_COM_setMCwait.UseVisualStyleBackColor = true;
            this.BTN_COM_setMCwait.Click += new System.EventHandler(this.BTN_COM_setMCwait_Click);
            // 
            // BTN_SPI_ADC_request
            // 
            this.BTN_SPI_ADC_request.Location = new System.Drawing.Point(6, 49);
            this.BTN_SPI_ADC_request.Name = "BTN_SPI_ADC_request";
            this.BTN_SPI_ADC_request.Size = new System.Drawing.Size(103, 23);
            this.BTN_SPI_ADC_request.TabIndex = 6;
            this.BTN_SPI_ADC_request.Text = "ADC";
            this.BTN_SPI_ADC_request.UseVisualStyleBackColor = true;
            this.BTN_SPI_ADC_request.Click += new System.EventHandler(this.BTN_SPI_ADC_request_Click);
            // 
            // BTN_SPI_DAC_send
            // 
            this.BTN_SPI_DAC_send.Location = new System.Drawing.Point(67, 19);
            this.BTN_SPI_DAC_send.Name = "BTN_SPI_DAC_send";
            this.BTN_SPI_DAC_send.Size = new System.Drawing.Size(42, 24);
            this.BTN_SPI_DAC_send.TabIndex = 5;
            this.BTN_SPI_DAC_send.Text = "DAC";
            this.BTN_SPI_DAC_send.UseVisualStyleBackColor = true;
            this.BTN_SPI_DAC_send.Click += new System.EventHandler(this.BTN_SPI_DAC_send_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CHB_COM_DC_recieveDATA);
            this.groupBox1.Controls.Add(this.CHB_COM_DC_sendDATA);
            this.groupBox1.Controls.Add(this.CHB_COM_DC_recieveResponse);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.BTN_COM_DC_send);
            this.groupBox1.Controls.Add(this.TXB_COM_DC_recieveDATA_count);
            this.groupBox1.Controls.Add(this.TXB_COM_DC_sendDATA);
            this.groupBox1.Controls.Add(this.TXB_COM_DC_command);
            this.groupBox1.Location = new System.Drawing.Point(340, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(206, 140);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Прямое управление";
            // 
            // CHB_COM_DC_recieveDATA
            // 
            this.CHB_COM_DC_recieveDATA.AutoSize = true;
            this.CHB_COM_DC_recieveDATA.Location = new System.Drawing.Point(6, 71);
            this.CHB_COM_DC_recieveDATA.Name = "CHB_COM_DC_recieveDATA";
            this.CHB_COM_DC_recieveDATA.Size = new System.Drawing.Size(113, 17);
            this.CHB_COM_DC_recieveDATA.TabIndex = 5;
            this.CHB_COM_DC_recieveDATA.Text = "Принять данные:";
            this.CHB_COM_DC_recieveDATA.UseVisualStyleBackColor = true;
            this.CHB_COM_DC_recieveDATA.CheckedChanged += new System.EventHandler(this.CHB_COM_DC_recieveDATA_CheckedChanged);
            // 
            // CHB_COM_DC_sendDATA
            // 
            this.CHB_COM_DC_sendDATA.AutoSize = true;
            this.CHB_COM_DC_sendDATA.Location = new System.Drawing.Point(6, 45);
            this.CHB_COM_DC_sendDATA.Name = "CHB_COM_DC_sendDATA";
            this.CHB_COM_DC_sendDATA.Size = new System.Drawing.Size(113, 17);
            this.CHB_COM_DC_sendDATA.TabIndex = 5;
            this.CHB_COM_DC_sendDATA.Text = "Послать данные:";
            this.CHB_COM_DC_sendDATA.UseVisualStyleBackColor = true;
            this.CHB_COM_DC_sendDATA.CheckedChanged += new System.EventHandler(this.CHB_COM_DC_sendDATA_CheckedChanged);
            // 
            // CHB_COM_DC_recieveResponse
            // 
            this.CHB_COM_DC_recieveResponse.AutoSize = true;
            this.CHB_COM_DC_recieveResponse.Location = new System.Drawing.Point(6, 95);
            this.CHB_COM_DC_recieveResponse.Name = "CHB_COM_DC_recieveResponse";
            this.CHB_COM_DC_recieveResponse.Size = new System.Drawing.Size(107, 17);
            this.CHB_COM_DC_recieveResponse.TabIndex = 5;
            this.CHB_COM_DC_recieveResponse.Text = "Принять отклик";
            this.CHB_COM_DC_recieveResponse.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Прямая команда:";
            // 
            // BTN_COM_DC_send
            // 
            this.BTN_COM_DC_send.Location = new System.Drawing.Point(127, 95);
            this.BTN_COM_DC_send.Name = "BTN_COM_DC_send";
            this.BTN_COM_DC_send.Size = new System.Drawing.Size(73, 39);
            this.BTN_COM_DC_send.TabIndex = 1;
            this.BTN_COM_DC_send.Text = "Послать";
            this.BTN_COM_DC_send.UseVisualStyleBackColor = true;
            this.BTN_COM_DC_send.Click += new System.EventHandler(this.BTN_DirectCommand_Click);
            // 
            // TXB_COM_DC_recieveDATA_count
            // 
            this.TXB_COM_DC_recieveDATA_count.Enabled = false;
            this.TXB_COM_DC_recieveDATA_count.Location = new System.Drawing.Point(127, 69);
            this.TXB_COM_DC_recieveDATA_count.Name = "TXB_COM_DC_recieveDATA_count";
            this.TXB_COM_DC_recieveDATA_count.Size = new System.Drawing.Size(73, 20);
            this.TXB_COM_DC_recieveDATA_count.TabIndex = 3;
            this.Hinter.SetToolTip(this.TXB_COM_DC_recieveDATA_count, "Количество принимаемых байт");
            // 
            // TXB_COM_DC_sendDATA
            // 
            this.TXB_COM_DC_sendDATA.Enabled = false;
            this.TXB_COM_DC_sendDATA.Location = new System.Drawing.Point(127, 43);
            this.TXB_COM_DC_sendDATA.Name = "TXB_COM_DC_sendDATA";
            this.TXB_COM_DC_sendDATA.Size = new System.Drawing.Size(73, 20);
            this.TXB_COM_DC_sendDATA.TabIndex = 3;
            this.Hinter.SetToolTip(this.TXB_COM_DC_sendDATA, "Данные задавать последовательно по байтам с пробелом после каждого (!) байта. При" +
                    "мер: \"2 147 \"");
            // 
            // TXB_COM_DC_command
            // 
            this.TXB_COM_DC_command.Location = new System.Drawing.Point(127, 17);
            this.TXB_COM_DC_command.Name = "TXB_COM_DC_command";
            this.TXB_COM_DC_command.Size = new System.Drawing.Size(73, 20);
            this.TXB_COM_DC_command.TabIndex = 2;
            // 
            // TXB_interval
            // 
            this.TXB_interval.Location = new System.Drawing.Point(115, 21);
            this.TXB_interval.Name = "TXB_interval";
            this.TXB_interval.Size = new System.Drawing.Size(73, 20);
            this.TXB_interval.TabIndex = 2;
            this.TXB_interval.Text = "1";
            // 
            // TXB_DAC_voltage
            // 
            this.TXB_DAC_voltage.Location = new System.Drawing.Point(152, 22);
            this.TXB_DAC_voltage.Name = "TXB_DAC_voltage";
            this.TXB_DAC_voltage.Size = new System.Drawing.Size(63, 20);
            this.TXB_DAC_voltage.TabIndex = 2;
            this.TXB_DAC_voltage.Text = "4000";
            // 
            // TXB_DAC_channel
            // 
            this.TXB_DAC_channel.Location = new System.Drawing.Point(115, 22);
            this.TXB_DAC_channel.Name = "TXB_DAC_channel";
            this.TXB_DAC_channel.Size = new System.Drawing.Size(31, 20);
            this.TXB_DAC_channel.TabIndex = 2;
            this.TXB_DAC_channel.Text = "1";
            // 
            // TXB_ADC_channel
            // 
            this.TXB_ADC_channel.Location = new System.Drawing.Point(115, 51);
            this.TXB_ADC_channel.Name = "TXB_ADC_channel";
            this.TXB_ADC_channel.Size = new System.Drawing.Size(31, 20);
            this.TXB_ADC_channel.TabIndex = 2;
            this.TXB_ADC_channel.Text = "1";
            // 
            // TXB_LEDbyte
            // 
            this.TXB_LEDbyte.Location = new System.Drawing.Point(115, 79);
            this.TXB_LEDbyte.Name = "TXB_LEDbyte";
            this.TXB_LEDbyte.Size = new System.Drawing.Size(100, 20);
            this.TXB_LEDbyte.TabIndex = 2;
            this.TXB_LEDbyte.Text = "0";
            // 
            // BTN_LEDbyte
            // 
            this.BTN_LEDbyte.Location = new System.Drawing.Point(6, 77);
            this.BTN_LEDbyte.Name = "BTN_LEDbyte";
            this.BTN_LEDbyte.Size = new System.Drawing.Size(103, 23);
            this.BTN_LEDbyte.TabIndex = 1;
            this.BTN_LEDbyte.Text = "Высветить байт";
            this.BTN_LEDbyte.UseVisualStyleBackColor = true;
            this.BTN_LEDbyte.Click += new System.EventHandler(this.BTN_LEDbyte_Click);
            // 
            // BTN_COM_getMCversion
            // 
            this.BTN_COM_getMCversion.Location = new System.Drawing.Point(6, 19);
            this.BTN_COM_getMCversion.Name = "BTN_COM_getMCversion";
            this.BTN_COM_getMCversion.Size = new System.Drawing.Size(103, 23);
            this.BTN_COM_getMCversion.TabIndex = 0;
            this.BTN_COM_getMCversion.Text = "Версия";
            this.BTN_COM_getMCversion.UseVisualStyleBackColor = true;
            this.BTN_COM_getMCversion.Click += new System.EventHandler(this.BTN_COM_getMCversion_Click);
            // 
            // BTN_COM_MC_CPUfreq
            // 
            this.BTN_COM_MC_CPUfreq.Location = new System.Drawing.Point(115, 48);
            this.BTN_COM_MC_CPUfreq.Name = "BTN_COM_MC_CPUfreq";
            this.BTN_COM_MC_CPUfreq.Size = new System.Drawing.Size(103, 23);
            this.BTN_COM_MC_CPUfreq.TabIndex = 0;
            this.BTN_COM_MC_CPUfreq.Text = "Частота CPU";
            this.BTN_COM_MC_CPUfreq.UseVisualStyleBackColor = true;
            this.BTN_COM_MC_CPUfreq.Click += new System.EventHandler(this.BTN_COM_getCPUfreq_Click);
            // 
            // BTN_MCstatus
            // 
            this.BTN_MCstatus.Location = new System.Drawing.Point(115, 19);
            this.BTN_MCstatus.Name = "BTN_MCstatus";
            this.BTN_MCstatus.Size = new System.Drawing.Size(103, 23);
            this.BTN_MCstatus.TabIndex = 0;
            this.BTN_MCstatus.Text = "Статус";
            this.BTN_MCstatus.UseVisualStyleBackColor = true;
            this.BTN_MCstatus.Click += new System.EventHandler(this.BTN_MCstatus_Click);
            // 
            // CHB_ADC_DoubleRange
            // 
            this.CHB_ADC_DoubleRange.AutoSize = true;
            this.CHB_ADC_DoubleRange.Location = new System.Drawing.Point(153, 53);
            this.CHB_ADC_DoubleRange.Name = "CHB_ADC_DoubleRange";
            this.CHB_ADC_DoubleRange.Size = new System.Drawing.Size(37, 17);
            this.CHB_ADC_DoubleRange.TabIndex = 8;
            this.CHB_ADC_DoubleRange.Text = "х2";
            this.Hinter.SetToolTip(this.CHB_ADC_DoubleRange, "Двойной диапазон напряжений");
            this.CHB_ADC_DoubleRange.UseVisualStyleBackColor = true;
            // 
            // BTN_DAC_reset
            // 
            this.BTN_DAC_reset.Location = new System.Drawing.Point(6, 19);
            this.BTN_DAC_reset.Name = "BTN_DAC_reset";
            this.BTN_DAC_reset.Size = new System.Drawing.Size(55, 24);
            this.BTN_DAC_reset.TabIndex = 9;
            this.BTN_DAC_reset.Text = "Сброс";
            this.BTN_DAC_reset.UseVisualStyleBackColor = true;
            this.BTN_DAC_reset.Click += new System.EventHandler(this.BTN_DAC_reset_Click);
            // 
            // GRB_MC
            // 
            this.GRB_MC.Controls.Add(this.BTN_COM_getMCversion);
            this.GRB_MC.Controls.Add(this.BTN_MCstatus);
            this.GRB_MC.Controls.Add(this.BTN_COM_MC_CPUfreq);
            this.GRB_MC.Controls.Add(this.BTN_LEDbyte);
            this.GRB_MC.Controls.Add(this.TXB_LEDbyte);
            this.GRB_MC.Controls.Add(this.BTN_COM_setMCwait);
            this.GRB_MC.Location = new System.Drawing.Point(6, 10);
            this.GRB_MC.Name = "GRB_MC";
            this.GRB_MC.Size = new System.Drawing.Size(223, 108);
            this.GRB_MC.TabIndex = 10;
            this.GRB_MC.TabStop = false;
            this.GRB_MC.Text = "Процессор";
            // 
            // GRB_SPI
            // 
            this.GRB_SPI.Controls.Add(this.BTN_DAC_reset);
            this.GRB_SPI.Controls.Add(this.TXB_ADC_channel);
            this.GRB_SPI.Controls.Add(this.TXB_DAC_channel);
            this.GRB_SPI.Controls.Add(this.CHB_ADC_DoubleRange);
            this.GRB_SPI.Controls.Add(this.TXB_DAC_voltage);
            this.GRB_SPI.Controls.Add(this.BTN_SPI_DAC_send);
            this.GRB_SPI.Controls.Add(this.BTN_SPI_ADC_request);
            this.GRB_SPI.Location = new System.Drawing.Point(6, 233);
            this.GRB_SPI.Name = "GRB_SPI";
            this.GRB_SPI.Size = new System.Drawing.Size(223, 81);
            this.GRB_SPI.TabIndex = 11;
            this.GRB_SPI.TabStop = false;
            this.GRB_SPI.Text = "SPI";
            // 
            // GRB_Counter
            // 
            this.GRB_Counter.Controls.Add(this.BTN_setInterval);
            this.GRB_Counter.Controls.Add(this.TXB_interval);
            this.GRB_Counter.Controls.Add(this.BTN_startCounter);
            this.GRB_Counter.Controls.Add(this.BTN_reqCount);
            this.GRB_Counter.Controls.Add(this.BTN_stopCounter);
            this.GRB_Counter.Location = new System.Drawing.Point(322, 206);
            this.GRB_Counter.Name = "GRB_Counter";
            this.GRB_Counter.Size = new System.Drawing.Size(224, 108);
            this.GRB_Counter.TabIndex = 12;
            this.GRB_Counter.TabStop = false;
            this.GRB_Counter.Text = "Счётчик";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 562);
            this.Controls.Add(this.TABpanel);
            this.Controls.Add(this.Log);
            this.MaximumSize = new System.Drawing.Size(600, 600);
            this.MinimumSize = new System.Drawing.Size(600, 600);
            this.Name = "Form1";
            this.Text = "XMega32A4U_testBoard v0.3";
            this.TABpanel.ResumeLayout(false);
            this.TABpanel_1.ResumeLayout(false);
            this.TABpanel_1.PerformLayout();
            this.TABpanel_2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.GRB_MC.ResumeLayout(false);
            this.GRB_MC.PerformLayout();
            this.GRB_SPI.ResumeLayout(false);
            this.GRB_SPI.PerformLayout();
            this.GRB_Counter.ResumeLayout(false);
            this.GRB_Counter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox Log;
        private System.Windows.Forms.Button BTN_COM_find;
        private System.Windows.Forms.TabControl TABpanel;
        private System.Windows.Forms.TabPage TABpanel_1;
        private System.Windows.Forms.Label lable_COM_BaudRate;
        private System.Windows.Forms.Label lable_COM;
        private System.Windows.Forms.TabPage TABpanel_2;
        private System.Windows.Forms.Label lable_COM_Handshake;
        private System.Windows.Forms.Label lable_COM_StopBits;
        private System.Windows.Forms.Label lable_COM_DataBits;
        private System.Windows.Forms.Label lable_COM_Parity;
        private System.Windows.Forms.ComboBox CMB_COM_Handshake;
        private System.Windows.Forms.ComboBox CMB_COM_Parity;
        private System.Windows.Forms.ComboBox CMB_COM_BaudRate;
        private System.Windows.Forms.ComboBox CMB_COM_StopBits;
        private System.Windows.Forms.ComboBox CMB_COM_DataBits;
        private System.Windows.Forms.ComboBox cBox_COM;
        private System.Windows.Forms.Button BTN_COM_setParams;
        private System.Windows.Forms.Button BTN_MCstatus;
        private System.Windows.Forms.TextBox TXB_LEDbyte;
        private System.Windows.Forms.Button BTN_LEDbyte;
        private System.Windows.Forms.TextBox TXB_COM_DC_command;
        private System.Windows.Forms.Button BTN_COM_DC_send;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox CHB_COM_DC_recieveDATA;
        private System.Windows.Forms.CheckBox CHB_COM_DC_recieveResponse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TXB_COM_DC_sendDATA;
        private System.Windows.Forms.Button BTN_SPI_DAC_send;
        private System.Windows.Forms.Button BTN_SPI_ADC_request;
        private System.Windows.Forms.TextBox TXB_ADC_channel;
        private System.Windows.Forms.TextBox TXB_DAC_voltage;
        private System.Windows.Forms.TextBox TXB_DAC_channel;
        private System.Windows.Forms.CheckBox CHB_COM_DC_sendDATA;
        private System.Windows.Forms.TextBox TXB_COM_DC_recieveDATA_count;
        private System.Windows.Forms.Button BTN_COM_getMCversion;
        private System.Windows.Forms.Button BTN_COM_setMCwait;
        private System.Windows.Forms.Button BTN_COM_MC_CPUfreq;
        private System.Windows.Forms.ToolTip Hinter;
        private System.Windows.Forms.Button BTN_reqCount;
        private System.Windows.Forms.Button BTN_startCounter;
        private System.Windows.Forms.Button BTN_setInterval;
        private System.Windows.Forms.TextBox TXB_interval;
        private System.Windows.Forms.Button BTN_stopCounter;
        private System.Windows.Forms.CheckBox CHB_ADC_DoubleRange;
        private System.Windows.Forms.GroupBox GRB_Counter;
        private System.Windows.Forms.GroupBox GRB_SPI;
        private System.Windows.Forms.Button BTN_DAC_reset;
        private System.Windows.Forms.GroupBox GRB_MC;
    }
}

