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
            this.GRB_TotalControl = new System.Windows.Forms.GroupBox();
            this.LBL_TotalC_Status = new System.Windows.Forms.Label();
            this.LBL_error = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.CHB_TotalControl = new System.Windows.Forms.CheckBox();
            this.GRB_Counter = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.PGB_COA_progress = new System.Windows.Forms.ProgressBar();
            this.LBL_COA_ticks = new System.Windows.Forms.Label();
            this.LBL_COA_prescaler = new System.Windows.Forms.Label();
            this.LBL_COA_frequency = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.TXB_COA_delay = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TXB_COA_quantity = new System.Windows.Forms.TextBox();
            this.BTN_setInterval = new System.Windows.Forms.Button();
            this.TXB_COA_measureTime = new System.Windows.Forms.TextBox();
            this.BTN_startCounter = new System.Windows.Forms.Button();
            this.BTN_reqCount = new System.Windows.Forms.Button();
            this.BTN_stopCounter = new System.Windows.Forms.Button();
            this.GRB_SPI = new System.Windows.Forms.GroupBox();
            this.BTN_DAC_reset = new System.Windows.Forms.Button();
            this.TXB_ADC_channel = new System.Windows.Forms.TextBox();
            this.TXB_DAC_channel = new System.Windows.Forms.TextBox();
            this.CHB_ADC_DoubleRange = new System.Windows.Forms.CheckBox();
            this.TXB_DAC_voltage = new System.Windows.Forms.TextBox();
            this.BTN_SPI_DAC_send = new System.Windows.Forms.Button();
            this.BTN_SPI_ADC_request = new System.Windows.Forms.Button();
            this.GRB_MC = new System.Windows.Forms.GroupBox();
            this.BTN_COM_getMCversion = new System.Windows.Forms.Button();
            this.BTN_MCstatus = new System.Windows.Forms.Button();
            this.BTN_COM_MC_CPUfreq = new System.Windows.Forms.Button();
            this.BTN_MC_Reset = new System.Windows.Forms.Button();
            this.BTN_LEDbyte = new System.Windows.Forms.Button();
            this.TXB_LEDbyte = new System.Windows.Forms.TextBox();
            this.BTN_COM_setMCwait = new System.Windows.Forms.Button();
            this.Hinter = new System.Windows.Forms.ToolTip(this.components);
            this.CLK_timer = new System.Windows.Forms.Timer(this.components);
            this.CHB_enableSuperTracer = new System.Windows.Forms.CheckBox();
            this.CLK_COA = new System.Windows.Forms.Timer(this.components);
            this.BTN_openLog = new System.Windows.Forms.Button();
            this.CHB_traceLog = new System.Windows.Forms.CheckBox();
            this.TABpanel.SuspendLayout();
            this.TABpanel_1.SuspendLayout();
            this.TABpanel_2.SuspendLayout();
            this.GRB_TotalControl.SuspendLayout();
            this.GRB_Counter.SuspendLayout();
            this.GRB_SPI.SuspendLayout();
            this.GRB_MC.SuspendLayout();
            this.SuspendLayout();
            // 
            // Log
            // 
            this.Log.Location = new System.Drawing.Point(12, 385);
            this.Log.Name = "Log";
            this.Log.Size = new System.Drawing.Size(656, 165);
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
            this.TABpanel.Size = new System.Drawing.Size(660, 346);
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
            this.TABpanel_1.Size = new System.Drawing.Size(652, 320);
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
            this.TABpanel_2.Controls.Add(this.GRB_TotalControl);
            this.TABpanel_2.Controls.Add(this.GRB_Counter);
            this.TABpanel_2.Controls.Add(this.GRB_SPI);
            this.TABpanel_2.Controls.Add(this.GRB_MC);
            this.TABpanel_2.Location = new System.Drawing.Point(4, 22);
            this.TABpanel_2.Name = "TABpanel_2";
            this.TABpanel_2.Padding = new System.Windows.Forms.Padding(3);
            this.TABpanel_2.Size = new System.Drawing.Size(652, 320);
            this.TABpanel_2.TabIndex = 1;
            this.TABpanel_2.Text = "XMega32A4U_testBoard";
            // 
            // GRB_TotalControl
            // 
            this.GRB_TotalControl.Controls.Add(this.LBL_TotalC_Status);
            this.GRB_TotalControl.Controls.Add(this.LBL_error);
            this.GRB_TotalControl.Controls.Add(this.label2);
            this.GRB_TotalControl.Controls.Add(this.label1);
            this.GRB_TotalControl.Controls.Add(this.CHB_TotalControl);
            this.GRB_TotalControl.Location = new System.Drawing.Point(6, 145);
            this.GRB_TotalControl.Name = "GRB_TotalControl";
            this.GRB_TotalControl.Size = new System.Drawing.Size(223, 85);
            this.GRB_TotalControl.TabIndex = 14;
            this.GRB_TotalControl.TabStop = false;
            this.GRB_TotalControl.Text = "Контроль";
            // 
            // LBL_TotalC_Status
            // 
            this.LBL_TotalC_Status.AutoSize = true;
            this.LBL_TotalC_Status.ForeColor = System.Drawing.Color.Red;
            this.LBL_TotalC_Status.Location = new System.Drawing.Point(62, 39);
            this.LBL_TotalC_Status.Name = "LBL_TotalC_Status";
            this.LBL_TotalC_Status.Size = new System.Drawing.Size(68, 13);
            this.LBL_TotalC_Status.TabIndex = 14;
            this.LBL_TotalC_Status.Text = "Неизвестно";
            // 
            // LBL_error
            // 
            this.LBL_error.AutoSize = true;
            this.LBL_error.ForeColor = System.Drawing.Color.Red;
            this.LBL_error.Location = new System.Drawing.Point(62, 53);
            this.LBL_error.Name = "LBL_error";
            this.LBL_error.Size = new System.Drawing.Size(68, 13);
            this.LBL_error.TabIndex = 14;
            this.LBL_error.Text = "Неизвестно";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Ошибки:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Статус: ";
            // 
            // CHB_TotalControl
            // 
            this.CHB_TotalControl.AutoSize = true;
            this.CHB_TotalControl.ForeColor = System.Drawing.Color.Red;
            this.CHB_TotalControl.Location = new System.Drawing.Point(6, 19);
            this.CHB_TotalControl.Name = "CHB_TotalControl";
            this.CHB_TotalControl.Size = new System.Drawing.Size(78, 17);
            this.CHB_TotalControl.TabIndex = 13;
            this.CHB_TotalControl.Text = "Выключен";
            this.Hinter.SetToolTip(this.CHB_TotalControl, "Постоянный опрос состояния контроллера");
            this.CHB_TotalControl.UseVisualStyleBackColor = true;
            this.CHB_TotalControl.CheckedChanged += new System.EventHandler(this.CHB_TotalControl_CheckedChanged);
            // 
            // GRB_Counter
            // 
            this.GRB_Counter.Controls.Add(this.label11);
            this.GRB_Counter.Controls.Add(this.label9);
            this.GRB_Counter.Controls.Add(this.label10);
            this.GRB_Counter.Controls.Add(this.label8);
            this.GRB_Counter.Controls.Add(this.label6);
            this.GRB_Counter.Controls.Add(this.PGB_COA_progress);
            this.GRB_Counter.Controls.Add(this.LBL_COA_ticks);
            this.GRB_Counter.Controls.Add(this.LBL_COA_prescaler);
            this.GRB_Counter.Controls.Add(this.LBL_COA_frequency);
            this.GRB_Counter.Controls.Add(this.label5);
            this.GRB_Counter.Controls.Add(this.label4);
            this.GRB_Counter.Controls.Add(this.TXB_COA_delay);
            this.GRB_Counter.Controls.Add(this.label3);
            this.GRB_Counter.Controls.Add(this.TXB_COA_quantity);
            this.GRB_Counter.Controls.Add(this.BTN_setInterval);
            this.GRB_Counter.Controls.Add(this.TXB_COA_measureTime);
            this.GRB_Counter.Controls.Add(this.BTN_startCounter);
            this.GRB_Counter.Controls.Add(this.BTN_reqCount);
            this.GRB_Counter.Controls.Add(this.BTN_stopCounter);
            this.GRB_Counter.Location = new System.Drawing.Point(287, 124);
            this.GRB_Counter.Name = "GRB_Counter";
            this.GRB_Counter.Size = new System.Drawing.Size(359, 190);
            this.GRB_Counter.TabIndex = 12;
            this.GRB_Counter.TabStop = false;
            this.GRB_Counter.Text = "Счётчик";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(306, 24);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(21, 13);
            this.label11.TabIndex = 12;
            this.label11.Text = "мс";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Enabled = false;
            this.label9.Location = new System.Drawing.Point(306, 50);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(21, 13);
            this.label9.TabIndex = 12;
            this.label9.Text = "мс";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Enabled = false;
            this.label10.Location = new System.Drawing.Point(125, 50);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(77, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "Время паузы:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Enabled = false;
            this.label8.Location = new System.Drawing.Point(125, 77);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(131, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Количество интервалов:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(125, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Время измерения:";
            // 
            // PGB_COA_progress
            // 
            this.PGB_COA_progress.BackColor = System.Drawing.SystemColors.Control;
            this.PGB_COA_progress.ForeColor = System.Drawing.SystemColors.Desktop;
            this.PGB_COA_progress.Location = new System.Drawing.Point(6, 165);
            this.PGB_COA_progress.Maximum = 2000;
            this.PGB_COA_progress.Name = "PGB_COA_progress";
            this.PGB_COA_progress.Size = new System.Drawing.Size(337, 15);
            this.PGB_COA_progress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.PGB_COA_progress.TabIndex = 10;
            // 
            // LBL_COA_ticks
            // 
            this.LBL_COA_ticks.AutoSize = true;
            this.LBL_COA_ticks.Location = new System.Drawing.Point(229, 146);
            this.LBL_COA_ticks.Name = "LBL_COA_ticks";
            this.LBL_COA_ticks.Size = new System.Drawing.Size(13, 13);
            this.LBL_COA_ticks.TabIndex = 9;
            this.LBL_COA_ticks.Text = "0";
            // 
            // LBL_COA_prescaler
            // 
            this.LBL_COA_prescaler.AutoSize = true;
            this.LBL_COA_prescaler.Location = new System.Drawing.Point(229, 126);
            this.LBL_COA_prescaler.Name = "LBL_COA_prescaler";
            this.LBL_COA_prescaler.Size = new System.Drawing.Size(13, 13);
            this.LBL_COA_prescaler.TabIndex = 9;
            this.LBL_COA_prescaler.Text = "0";
            // 
            // LBL_COA_frequency
            // 
            this.LBL_COA_frequency.AutoSize = true;
            this.LBL_COA_frequency.Location = new System.Drawing.Point(229, 106);
            this.LBL_COA_frequency.Name = "LBL_COA_frequency";
            this.LBL_COA_frequency.Size = new System.Drawing.Size(13, 13);
            this.LBL_COA_frequency.TabIndex = 9;
            this.LBL_COA_frequency.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(125, 126);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Делитель:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(125, 146);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Количество тиков:";
            // 
            // TXB_COA_delay
            // 
            this.TXB_COA_delay.Enabled = false;
            this.TXB_COA_delay.Location = new System.Drawing.Point(260, 47);
            this.TXB_COA_delay.Name = "TXB_COA_delay";
            this.TXB_COA_delay.Size = new System.Drawing.Size(40, 20);
            this.TXB_COA_delay.TabIndex = 2;
            this.TXB_COA_delay.Text = "10";
            this.TXB_COA_delay.TextChanged += new System.EventHandler(this.TXB_interval_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(125, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Частота:";
            // 
            // TXB_COA_quantity
            // 
            this.TXB_COA_quantity.Enabled = false;
            this.TXB_COA_quantity.Location = new System.Drawing.Point(260, 74);
            this.TXB_COA_quantity.Name = "TXB_COA_quantity";
            this.TXB_COA_quantity.Size = new System.Drawing.Size(40, 20);
            this.TXB_COA_quantity.TabIndex = 2;
            this.TXB_COA_quantity.Text = "1";
            this.TXB_COA_quantity.TextChanged += new System.EventHandler(this.TXB_interval_TextChanged);
            // 
            // BTN_setInterval
            // 
            this.BTN_setInterval.Location = new System.Drawing.Point(6, 19);
            this.BTN_setInterval.Name = "BTN_setInterval";
            this.BTN_setInterval.Size = new System.Drawing.Size(113, 23);
            this.BTN_setInterval.TabIndex = 7;
            this.BTN_setInterval.Text = "Задать параметры";
            this.BTN_setInterval.UseVisualStyleBackColor = true;
            this.BTN_setInterval.Click += new System.EventHandler(this.BTN_setInterval_Click);
            // 
            // TXB_COA_measureTime
            // 
            this.TXB_COA_measureTime.Location = new System.Drawing.Point(237, 22);
            this.TXB_COA_measureTime.Name = "TXB_COA_measureTime";
            this.TXB_COA_measureTime.Size = new System.Drawing.Size(63, 20);
            this.TXB_COA_measureTime.TabIndex = 2;
            this.TXB_COA_measureTime.Text = "1000";
            this.TXB_COA_measureTime.TextChanged += new System.EventHandler(this.TXB_interval_TextChanged);
            // 
            // BTN_startCounter
            // 
            this.BTN_startCounter.Location = new System.Drawing.Point(6, 48);
            this.BTN_startCounter.Name = "BTN_startCounter";
            this.BTN_startCounter.Size = new System.Drawing.Size(113, 23);
            this.BTN_startCounter.TabIndex = 7;
            this.BTN_startCounter.Text = "Начать счёт";
            this.BTN_startCounter.UseVisualStyleBackColor = true;
            this.BTN_startCounter.Click += new System.EventHandler(this.BTN_startCounter_Click);
            // 
            // BTN_reqCount
            // 
            this.BTN_reqCount.Location = new System.Drawing.Point(6, 135);
            this.BTN_reqCount.Name = "BTN_reqCount";
            this.BTN_reqCount.Size = new System.Drawing.Size(113, 23);
            this.BTN_reqCount.TabIndex = 7;
            this.BTN_reqCount.Text = "Проверить счёт";
            this.BTN_reqCount.UseVisualStyleBackColor = true;
            this.BTN_reqCount.Click += new System.EventHandler(this.BTN_reqCount_Click);
            // 
            // BTN_stopCounter
            // 
            this.BTN_stopCounter.Location = new System.Drawing.Point(6, 106);
            this.BTN_stopCounter.Name = "BTN_stopCounter";
            this.BTN_stopCounter.Size = new System.Drawing.Size(113, 23);
            this.BTN_stopCounter.TabIndex = 7;
            this.BTN_stopCounter.Text = "Остановить счёт";
            this.BTN_stopCounter.UseVisualStyleBackColor = true;
            this.BTN_stopCounter.Click += new System.EventHandler(this.BTN_stopCounter_Click);
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
            // TXB_ADC_channel
            // 
            this.TXB_ADC_channel.Location = new System.Drawing.Point(115, 51);
            this.TXB_ADC_channel.Name = "TXB_ADC_channel";
            this.TXB_ADC_channel.Size = new System.Drawing.Size(31, 20);
            this.TXB_ADC_channel.TabIndex = 2;
            this.TXB_ADC_channel.Text = "1";
            // 
            // TXB_DAC_channel
            // 
            this.TXB_DAC_channel.Location = new System.Drawing.Point(115, 22);
            this.TXB_DAC_channel.Name = "TXB_DAC_channel";
            this.TXB_DAC_channel.Size = new System.Drawing.Size(31, 20);
            this.TXB_DAC_channel.TabIndex = 2;
            this.TXB_DAC_channel.Text = "1";
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
            // TXB_DAC_voltage
            // 
            this.TXB_DAC_voltage.Location = new System.Drawing.Point(152, 22);
            this.TXB_DAC_voltage.Name = "TXB_DAC_voltage";
            this.TXB_DAC_voltage.Size = new System.Drawing.Size(63, 20);
            this.TXB_DAC_voltage.TabIndex = 2;
            this.TXB_DAC_voltage.Text = "4000";
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
            // GRB_MC
            // 
            this.GRB_MC.Controls.Add(this.BTN_COM_getMCversion);
            this.GRB_MC.Controls.Add(this.BTN_MCstatus);
            this.GRB_MC.Controls.Add(this.BTN_COM_MC_CPUfreq);
            this.GRB_MC.Controls.Add(this.BTN_MC_Reset);
            this.GRB_MC.Controls.Add(this.BTN_LEDbyte);
            this.GRB_MC.Controls.Add(this.TXB_LEDbyte);
            this.GRB_MC.Controls.Add(this.BTN_COM_setMCwait);
            this.GRB_MC.Location = new System.Drawing.Point(6, 10);
            this.GRB_MC.Name = "GRB_MC";
            this.GRB_MC.Size = new System.Drawing.Size(223, 132);
            this.GRB_MC.TabIndex = 10;
            this.GRB_MC.TabStop = false;
            this.GRB_MC.Text = "Процессор";
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
            // BTN_MC_Reset
            // 
            this.BTN_MC_Reset.Enabled = false;
            this.BTN_MC_Reset.Location = new System.Drawing.Point(6, 103);
            this.BTN_MC_Reset.Name = "BTN_MC_Reset";
            this.BTN_MC_Reset.Size = new System.Drawing.Size(103, 23);
            this.BTN_MC_Reset.TabIndex = 1;
            this.BTN_MC_Reset.Text = "Перезагрузить";
            this.BTN_MC_Reset.UseVisualStyleBackColor = true;
            this.BTN_MC_Reset.Click += new System.EventHandler(this.BTN_MC_Reset_Click);
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
            // TXB_LEDbyte
            // 
            this.TXB_LEDbyte.Location = new System.Drawing.Point(115, 79);
            this.TXB_LEDbyte.Name = "TXB_LEDbyte";
            this.TXB_LEDbyte.Size = new System.Drawing.Size(100, 20);
            this.TXB_LEDbyte.TabIndex = 2;
            this.TXB_LEDbyte.Text = "0";
            // 
            // BTN_COM_setMCwait
            // 
            this.BTN_COM_setMCwait.Enabled = false;
            this.BTN_COM_setMCwait.Location = new System.Drawing.Point(6, 48);
            this.BTN_COM_setMCwait.Name = "BTN_COM_setMCwait";
            this.BTN_COM_setMCwait.Size = new System.Drawing.Size(103, 23);
            this.BTN_COM_setMCwait.TabIndex = 7;
            this.BTN_COM_setMCwait.Text = "Ожидание";
            this.BTN_COM_setMCwait.UseVisualStyleBackColor = true;
            this.BTN_COM_setMCwait.Click += new System.EventHandler(this.BTN_COM_setMCwait_Click);
            // 
            // CLK_timer
            // 
            this.CLK_timer.Interval = 2000;
            this.CLK_timer.Tick += new System.EventHandler(this.CLK_timer_Tick);
            // 
            // CHB_enableSuperTracer
            // 
            this.CHB_enableSuperTracer.AutoSize = true;
            this.CHB_enableSuperTracer.Checked = true;
            this.CHB_enableSuperTracer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHB_enableSuperTracer.Location = new System.Drawing.Point(217, 364);
            this.CHB_enableSuperTracer.Name = "CHB_enableSuperTracer";
            this.CHB_enableSuperTracer.Size = new System.Drawing.Size(114, 17);
            this.CHB_enableSuperTracer.TabIndex = 3;
            this.CHB_enableSuperTracer.Text = "Подробный отчёт";
            this.CHB_enableSuperTracer.UseVisualStyleBackColor = true;
            this.CHB_enableSuperTracer.CheckedChanged += new System.EventHandler(this.CHB_enableSuperTracer_CheckedChanged);
            // 
            // CLK_COA
            // 
            this.CLK_COA.Interval = 10;
            this.CLK_COA.Tick += new System.EventHandler(this.CLK_COA_Tick);
            // 
            // BTN_openLog
            // 
            this.BTN_openLog.Location = new System.Drawing.Point(12, 360);
            this.BTN_openLog.Name = "BTN_openLog";
            this.BTN_openLog.Size = new System.Drawing.Size(35, 23);
            this.BTN_openLog.TabIndex = 4;
            this.BTN_openLog.Text = "Log";
            this.BTN_openLog.UseVisualStyleBackColor = true;
            this.BTN_openLog.Click += new System.EventHandler(this.BTN_openLog_Click);
            // 
            // CHB_traceLog
            // 
            this.CHB_traceLog.AutoSize = true;
            this.CHB_traceLog.Location = new System.Drawing.Point(53, 364);
            this.CHB_traceLog.Name = "CHB_traceLog";
            this.CHB_traceLog.Size = new System.Drawing.Size(126, 17);
            this.CHB_traceLog.TabIndex = 3;
            this.CHB_traceLog.Text = "Записывать в файл";
            this.CHB_traceLog.UseVisualStyleBackColor = true;
            this.CHB_traceLog.CheckedChanged += new System.EventHandler(this.CHB_traceLog_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 562);
            this.Controls.Add(this.BTN_openLog);
            this.Controls.Add(this.CHB_traceLog);
            this.Controls.Add(this.CHB_enableSuperTracer);
            this.Controls.Add(this.TABpanel);
            this.Controls.Add(this.Log);
            this.MaximumSize = new System.Drawing.Size(700, 600);
            this.MinimumSize = new System.Drawing.Size(700, 600);
            this.Name = "Form1";
            this.Text = "XMega32A4U_testBoard v0.3";
            this.TABpanel.ResumeLayout(false);
            this.TABpanel_1.ResumeLayout(false);
            this.TABpanel_1.PerformLayout();
            this.TABpanel_2.ResumeLayout(false);
            this.GRB_TotalControl.ResumeLayout(false);
            this.GRB_TotalControl.PerformLayout();
            this.GRB_Counter.ResumeLayout(false);
            this.GRB_Counter.PerformLayout();
            this.GRB_SPI.ResumeLayout(false);
            this.GRB_SPI.PerformLayout();
            this.GRB_MC.ResumeLayout(false);
            this.GRB_MC.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.Button BTN_SPI_DAC_send;
        private System.Windows.Forms.Button BTN_SPI_ADC_request;
        private System.Windows.Forms.TextBox TXB_ADC_channel;
        private System.Windows.Forms.TextBox TXB_DAC_voltage;
        private System.Windows.Forms.TextBox TXB_DAC_channel;
        private System.Windows.Forms.Button BTN_COM_getMCversion;
        private System.Windows.Forms.Button BTN_COM_setMCwait;
        private System.Windows.Forms.Button BTN_COM_MC_CPUfreq;
        private System.Windows.Forms.ToolTip Hinter;
        private System.Windows.Forms.Button BTN_reqCount;
        private System.Windows.Forms.Button BTN_startCounter;
        private System.Windows.Forms.Button BTN_setInterval;
        private System.Windows.Forms.TextBox TXB_COA_measureTime;
        private System.Windows.Forms.Button BTN_stopCounter;
        private System.Windows.Forms.CheckBox CHB_ADC_DoubleRange;
        private System.Windows.Forms.GroupBox GRB_Counter;
        private System.Windows.Forms.GroupBox GRB_SPI;
        private System.Windows.Forms.Button BTN_DAC_reset;
        private System.Windows.Forms.GroupBox GRB_MC;
        private System.Windows.Forms.CheckBox CHB_TotalControl;
        private System.Windows.Forms.Timer CLK_timer;
        private System.Windows.Forms.GroupBox GRB_TotalControl;
        private System.Windows.Forms.Label LBL_TotalC_Status;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label LBL_COA_ticks;
        private System.Windows.Forms.Label LBL_COA_prescaler;
        private System.Windows.Forms.Label LBL_COA_frequency;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox CHB_enableSuperTracer;
        private System.Windows.Forms.ProgressBar PGB_COA_progress;
        private System.Windows.Forms.Timer CLK_COA;
        private System.Windows.Forms.Button BTN_MC_Reset;
        private System.Windows.Forms.Button BTN_openLog;
        private System.Windows.Forms.Label LBL_error;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox CHB_traceLog;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox TXB_COA_delay;
        private System.Windows.Forms.TextBox TXB_COA_quantity;
    }
}

