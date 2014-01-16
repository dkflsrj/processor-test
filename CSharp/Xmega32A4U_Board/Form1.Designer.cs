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
            this.BTN_traceErrorList = new System.Windows.Forms.Button();
            this.BTN_sendSomething = new System.Windows.Forms.Button();
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
            this.LBL_COA_status = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.PGB_COA_progress = new System.Windows.Forms.ProgressBar();
            this.CHB_Control_COA = new System.Windows.Forms.CheckBox();
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
            this.BTN_SPI_ADC_request = new System.Windows.Forms.Button();
            this.BTN_SPI_DAC_send = new System.Windows.Forms.Button();
            this.GRB_MC = new System.Windows.Forms.GroupBox();
            this.BTN_checkCommandStack = new System.Windows.Forms.Button();
            this.BTN_COM_getMCversion = new System.Windows.Forms.Button();
            this.BTN_MCstatus = new System.Windows.Forms.Button();
            this.BTN_COM_MC_CPUfreq = new System.Windows.Forms.Button();
            this.BTN_MC_Reset = new System.Windows.Forms.Button();
            this.BTN_LEDbyte = new System.Windows.Forms.Button();
            this.TXB_LEDbyte = new System.Windows.Forms.TextBox();
            this.BTN_COM_setMCwait = new System.Windows.Forms.Button();
            this.TABpanel_3 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.BTN_TIC_HVEconf_check = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.LBL_TIC_HVE_ElapsedTime = new System.Windows.Forms.Label();
            this.LBL_TIC_HVE_Errors = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.TXB_TIC_HVE_Period = new System.Windows.Forms.TextBox();
            this.CHB_TIC_HVE = new System.Windows.Forms.CheckBox();
            this.GRB_TIC_DisplayContrast = new System.Windows.Forms.GroupBox();
            this.label19 = new System.Windows.Forms.Label();
            this.TXB_TIC_DisplayContrast = new System.Windows.Forms.TextBox();
            this.BTN_TIC_DisplayContrast_get = new System.Windows.Forms.Button();
            this.BTN_TIC_DisplayContrast_set = new System.Windows.Forms.Button();
            this.TABpanel_4 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.BTN_DETECTOR_getDV3voltage = new System.Windows.Forms.Button();
            this.BTN_DETECTOR_getDV2voltage = new System.Windows.Forms.Button();
            this.BTN_DETECTOR_getDV1voltage = new System.Windows.Forms.Button();
            this.TXB_DETECTOR_setDV3voltage = new System.Windows.Forms.TextBox();
            this.TXB_DETECTOR_setDV2voltage = new System.Windows.Forms.TextBox();
            this.LBL_DETECTOR_getDV3voltage = new System.Windows.Forms.Label();
            this.LBL_DETECTOR_getDV2voltage = new System.Windows.Forms.Label();
            this.LBL_DETECTOR_getDV1voltage = new System.Windows.Forms.Label();
            this.TXB_DETECTOR_setDV1voltage = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.BTN_DETECTOR_setDV3voltage = new System.Windows.Forms.Button();
            this.BTN_DETECTOR_setDV2voltage = new System.Windows.Forms.Button();
            this.BTN_DETECTOR_setDV1voltage = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.GPB_Flags = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.BTN_checkFlags = new System.Windows.Forms.Button();
            this.CHB_iEDCD = new System.Windows.Forms.CheckBox();
            this.CHB_SEMV3 = new System.Windows.Forms.CheckBox();
            this.CHB_iHVE = new System.Windows.Forms.CheckBox();
            this.CHB_PRGE = new System.Windows.Forms.CheckBox();
            this.CHB_SEMV2 = new System.Windows.Forms.CheckBox();
            this.CHB_SPUMP = new System.Windows.Forms.CheckBox();
            this.CHB_SEMV1 = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.LBL_IonSOURCE_getF2voltage = new System.Windows.Forms.Label();
            this.LBL_IonSOURCE_getF1voltage = new System.Windows.Forms.Label();
            this.LBL_IonSOURCE_getIonizationVoltage = new System.Windows.Forms.Label();
            this.LBL_IonSOURCE_getEmissionCurrentVoltage = new System.Windows.Forms.Label();
            this.TXB_IonSOURCE_setF2voltage = new System.Windows.Forms.TextBox();
            this.TXB_IonSOURCE_setF1voltage = new System.Windows.Forms.TextBox();
            this.TXB_IonSOURCE_setIonizationVoltage = new System.Windows.Forms.TextBox();
            this.TXB_IonSOURCE_setEmissionCurrentVoltage = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.BTN_IonSOURCE_setF2voltage = new System.Windows.Forms.Button();
            this.BTN_IonSOURCE_setF1voltage = new System.Windows.Forms.Button();
            this.BTN_IonSOURCE_setIonizationVoltage = new System.Windows.Forms.Button();
            this.BTN_IonSOURCE_getF2voltage = new System.Windows.Forms.Button();
            this.BTN_IonSOURCE_getF1voltage = new System.Windows.Forms.Button();
            this.BTN_IonSOURCE_getIonizationVoltage = new System.Windows.Forms.Button();
            this.BTN_IonSOURCE_getEmissionCurrentVoltage = new System.Windows.Forms.Button();
            this.BTN_IonSOURCE_setEmissionCurrentVoltage = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BTN_SCANER_getScanVoltage = new System.Windows.Forms.Button();
            this.BTN_SCANER_getParentScanVoltage = new System.Windows.Forms.Button();
            this.LBL_SCANER_getScanVoltage = new System.Windows.Forms.Label();
            this.TXB_SCANER_setScanVoltage = new System.Windows.Forms.TextBox();
            this.TXB_SCANER_setParentScanVoltage = new System.Windows.Forms.TextBox();
            this.BTN_SCANER_setParentScanVoltage = new System.Windows.Forms.Button();
            this.LBL_SCANER_getParentScanVoltage = new System.Windows.Forms.Label();
            this.BTN_SCANER_setScanVoltage = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.GPB_CONDER = new System.Windows.Forms.GroupBox();
            this.BTN_CONDENSATOR_getNegativeVoltage = new System.Windows.Forms.Button();
            this.BTN_CONDENSATOR_getPositiveVoltage = new System.Windows.Forms.Button();
            this.BTN_CONDENSATOR_setVoltage = new System.Windows.Forms.Button();
            this.LBL_CONDENSATOR_getPositiveVoltage = new System.Windows.Forms.Label();
            this.LBL_CONDENSATOR_getNegativeVoltage = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.TXB_CONDENSATOR_setVoltage = new System.Windows.Forms.TextBox();
            this.GPB_Heater = new System.Windows.Forms.GroupBox();
            this.BTN_HEATER_setVoltage = new System.Windows.Forms.Button();
            this.LBL_HEATER_getVoltage = new System.Windows.Forms.Label();
            this.BTN_INLET_setVoltage = new System.Windows.Forms.Button();
            this.TXB_HEATER_setVoltage = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.LBL_INLET_getVoltage = new System.Windows.Forms.Label();
            this._BTN_HEATER_getVoltage = new System.Windows.Forms.Button();
            this.TXB_INLET_setVoltage = new System.Windows.Forms.TextBox();
            this.BTN_INLET_getVoltage = new System.Windows.Forms.Button();
            this.TABpanel_5 = new System.Windows.Forms.TabPage();
            this.GPB_realCOX = new System.Windows.Forms.GroupBox();
            this.LBL_realCOX_RTC_OverTime = new System.Windows.Forms.Label();
            this.LBL_realCOX_COC_Result = new System.Windows.Forms.Label();
            this.LBL_realCOX_COB_Result = new System.Windows.Forms.Label();
            this.LBL_realCOX_RTCstate = new System.Windows.Forms.Label();
            this.LBL_realCOX_COA_Result = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.labelN = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.LBL_realCOX_Tiks = new System.Windows.Forms.Label();
            this.LBL_realCOX_Devider = new System.Windows.Forms.Label();
            this.LBL_realCOX_frequency = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.BTN_realCOX_setParameters = new System.Windows.Forms.Button();
            this.TXB_realCOX_NumberOfMeasurments = new System.Windows.Forms.TextBox();
            this.TXB_realCOX_MeasureTime = new System.Windows.Forms.TextBox();
            this.BTN_realCOX_start = new System.Windows.Forms.Button();
            this.BTN_realCOX_check = new System.Windows.Forms.Button();
            this.BTN_realCOX_stop = new System.Windows.Forms.Button();
            this.Hinter = new System.Windows.Forms.ToolTip(this.components);
            this.CLK_timer = new System.Windows.Forms.Timer(this.components);
            this.CHB_enableSuperTracer = new System.Windows.Forms.CheckBox();
            this.BTN_openLog = new System.Windows.Forms.Button();
            this.CHB_traceLog = new System.Windows.Forms.CheckBox();
            this.CLK_COA = new System.Windows.Forms.Timer(this.components);
            this.TIM_TIC_HVE = new System.Windows.Forms.Timer(this.components);
            this.TABpanel.SuspendLayout();
            this.TABpanel_1.SuspendLayout();
            this.TABpanel_2.SuspendLayout();
            this.GRB_TotalControl.SuspendLayout();
            this.GRB_Counter.SuspendLayout();
            this.GRB_SPI.SuspendLayout();
            this.GRB_MC.SuspendLayout();
            this.TABpanel_3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.GRB_TIC_DisplayContrast.SuspendLayout();
            this.TABpanel_4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.GPB_Flags.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.GPB_CONDER.SuspendLayout();
            this.GPB_Heater.SuspendLayout();
            this.TABpanel_5.SuspendLayout();
            this.GPB_realCOX.SuspendLayout();
            this.SuspendLayout();
            // 
            // Log
            // 
            this.Log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Log.Location = new System.Drawing.Point(12, 385);
            this.Log.Name = "Log";
            this.Log.Size = new System.Drawing.Size(656, 257);
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
            this.TABpanel.Controls.Add(this.TABpanel_3);
            this.TABpanel.Controls.Add(this.TABpanel_4);
            this.TABpanel.Controls.Add(this.TABpanel_5);
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
            this.CMB_COM_Handshake.Enabled = false;
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
            this.CMB_COM_Parity.Enabled = false;
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
            this.CMB_COM_BaudRate.Enabled = false;
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
            this.CMB_COM_StopBits.Enabled = false;
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
            this.CMB_COM_DataBits.Enabled = false;
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
            this.BTN_COM_setParams.Enabled = false;
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
            this.TABpanel_2.Controls.Add(this.BTN_traceErrorList);
            this.TABpanel_2.Controls.Add(this.BTN_sendSomething);
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
            // BTN_traceErrorList
            // 
            this.BTN_traceErrorList.Location = new System.Drawing.Point(6, 171);
            this.BTN_traceErrorList.Name = "BTN_traceErrorList";
            this.BTN_traceErrorList.Size = new System.Drawing.Size(95, 37);
            this.BTN_traceErrorList.TabIndex = 15;
            this.BTN_traceErrorList.Text = "Вывести список ошибок";
            this.BTN_traceErrorList.UseVisualStyleBackColor = true;
            this.BTN_traceErrorList.Click += new System.EventHandler(this.BTN_traceErrorList_Click);
            // 
            // BTN_sendSomething
            // 
            this.BTN_sendSomething.Location = new System.Drawing.Point(6, 144);
            this.BTN_sendSomething.Name = "BTN_sendSomething";
            this.BTN_sendSomething.Size = new System.Drawing.Size(95, 23);
            this.BTN_sendSomething.TabIndex = 15;
            this.BTN_sendSomething.Text = "Послать что-то";
            this.BTN_sendSomething.UseVisualStyleBackColor = true;
            this.BTN_sendSomething.Click += new System.EventHandler(this.BTN_sendSomething_Click);
            // 
            // GRB_TotalControl
            // 
            this.GRB_TotalControl.Controls.Add(this.LBL_TotalC_Status);
            this.GRB_TotalControl.Controls.Add(this.LBL_error);
            this.GRB_TotalControl.Controls.Add(this.label2);
            this.GRB_TotalControl.Controls.Add(this.label1);
            this.GRB_TotalControl.Controls.Add(this.CHB_TotalControl);
            this.GRB_TotalControl.Location = new System.Drawing.Point(235, 10);
            this.GRB_TotalControl.Name = "GRB_TotalControl";
            this.GRB_TotalControl.Size = new System.Drawing.Size(411, 108);
            this.GRB_TotalControl.TabIndex = 14;
            this.GRB_TotalControl.TabStop = false;
            this.GRB_TotalControl.Text = "Контроль";
            // 
            // LBL_TotalC_Status
            // 
            this.LBL_TotalC_Status.AutoSize = true;
            this.LBL_TotalC_Status.Enabled = false;
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
            this.LBL_error.Enabled = false;
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
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(6, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Ошибки:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Enabled = false;
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
            this.GRB_Counter.Controls.Add(this.LBL_COA_status);
            this.GRB_Counter.Controls.Add(this.label8);
            this.GRB_Counter.Controls.Add(this.label7);
            this.GRB_Counter.Controls.Add(this.label6);
            this.GRB_Counter.Controls.Add(this.PGB_COA_progress);
            this.GRB_Counter.Controls.Add(this.CHB_Control_COA);
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
            this.GRB_Counter.Enabled = false;
            this.GRB_Counter.Location = new System.Drawing.Point(235, 124);
            this.GRB_Counter.Name = "GRB_Counter";
            this.GRB_Counter.Size = new System.Drawing.Size(411, 190);
            this.GRB_Counter.TabIndex = 12;
            this.GRB_Counter.TabStop = false;
            this.GRB_Counter.Text = "Счётчик";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(373, 24);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(21, 13);
            this.label11.TabIndex = 12;
            this.label11.Text = "мс";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Enabled = false;
            this.label9.Location = new System.Drawing.Point(373, 50);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(21, 13);
            this.label9.TabIndex = 12;
            this.label9.Text = "мс";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Enabled = false;
            this.label10.Location = new System.Drawing.Point(192, 50);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(77, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "Время паузы:";
            // 
            // LBL_COA_status
            // 
            this.LBL_COA_status.AutoSize = true;
            this.LBL_COA_status.Enabled = false;
            this.LBL_COA_status.ForeColor = System.Drawing.Color.Red;
            this.LBL_COA_status.Location = new System.Drawing.Point(65, 80);
            this.LBL_COA_status.Name = "LBL_COA_status";
            this.LBL_COA_status.Size = new System.Drawing.Size(68, 13);
            this.LBL_COA_status.TabIndex = 14;
            this.LBL_COA_status.Text = "Неизвестно";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Enabled = false;
            this.label8.Location = new System.Drawing.Point(192, 80);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(128, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Количество измерений:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Enabled = false;
            this.label7.Location = new System.Drawing.Point(27, 80);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "COA:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(192, 24);
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
            // CHB_Control_COA
            // 
            this.CHB_Control_COA.AutoSize = true;
            this.CHB_Control_COA.Enabled = false;
            this.CHB_Control_COA.ForeColor = System.Drawing.Color.Red;
            this.CHB_Control_COA.Location = new System.Drawing.Point(6, 80);
            this.CHB_Control_COA.Name = "CHB_Control_COA";
            this.CHB_Control_COA.Size = new System.Drawing.Size(15, 14);
            this.CHB_Control_COA.TabIndex = 13;
            this.CHB_Control_COA.UseVisualStyleBackColor = true;
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
            this.TXB_COA_delay.Location = new System.Drawing.Point(327, 47);
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
            this.TXB_COA_quantity.Location = new System.Drawing.Point(327, 77);
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
            this.TXB_COA_measureTime.Location = new System.Drawing.Point(304, 22);
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
            this.GRB_SPI.Controls.Add(this.BTN_SPI_ADC_request);
            this.GRB_SPI.Controls.Add(this.BTN_SPI_DAC_send);
            this.GRB_SPI.Enabled = false;
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
            this.TXB_DAC_voltage.Location = new System.Drawing.Point(152, 23);
            this.TXB_DAC_voltage.Name = "TXB_DAC_voltage";
            this.TXB_DAC_voltage.Size = new System.Drawing.Size(57, 20);
            this.TXB_DAC_voltage.TabIndex = 2;
            this.TXB_DAC_voltage.Text = "40";
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
            // GRB_MC
            // 
            this.GRB_MC.Controls.Add(this.BTN_checkCommandStack);
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
            // BTN_checkCommandStack
            // 
            this.BTN_checkCommandStack.Location = new System.Drawing.Point(115, 103);
            this.BTN_checkCommandStack.Name = "BTN_checkCommandStack";
            this.BTN_checkCommandStack.Size = new System.Drawing.Size(100, 23);
            this.BTN_checkCommandStack.TabIndex = 16;
            this.BTN_checkCommandStack.Text = "Синхронно?";
            this.BTN_checkCommandStack.UseVisualStyleBackColor = true;
            this.BTN_checkCommandStack.Click += new System.EventHandler(this.BTN_checkCommandStack_Click);
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
            this.BTN_LEDbyte.Enabled = false;
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
            this.TXB_LEDbyte.Enabled = false;
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
            // TABpanel_3
            // 
            this.TABpanel_3.BackColor = System.Drawing.SystemColors.Control;
            this.TABpanel_3.Controls.Add(this.groupBox5);
            this.TABpanel_3.Controls.Add(this.groupBox4);
            this.TABpanel_3.Controls.Add(this.GRB_TIC_DisplayContrast);
            this.TABpanel_3.Location = new System.Drawing.Point(4, 22);
            this.TABpanel_3.Name = "TABpanel_3";
            this.TABpanel_3.Size = new System.Drawing.Size(652, 320);
            this.TABpanel_3.TabIndex = 2;
            this.TABpanel_3.Text = "TIC";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.comboBox2);
            this.groupBox5.Controls.Add(this.comboBox1);
            this.groupBox5.Controls.Add(this.BTN_TIC_HVEconf_check);
            this.groupBox5.Controls.Add(this.button3);
            this.groupBox5.Controls.Add(this.button2);
            this.groupBox5.Location = new System.Drawing.Point(3, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(288, 108);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Настройки HVE";
            // 
            // comboBox2
            // 
            this.comboBox2.Enabled = false;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(87, 49);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(73, 21);
            this.comboBox2.TabIndex = 1;
            // 
            // comboBox1
            // 
            this.comboBox1.Enabled = false;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(87, 19);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(73, 21);
            this.comboBox1.TabIndex = 1;
            // 
            // BTN_TIC_HVEconf_check
            // 
            this.BTN_TIC_HVEconf_check.Location = new System.Drawing.Point(6, 77);
            this.BTN_TIC_HVEconf_check.Name = "BTN_TIC_HVEconf_check";
            this.BTN_TIC_HVEconf_check.Size = new System.Drawing.Size(75, 23);
            this.BTN_TIC_HVEconf_check.TabIndex = 0;
            this.BTN_TIC_HVEconf_check.Text = "check";
            this.BTN_TIC_HVEconf_check.UseVisualStyleBackColor = true;
            this.BTN_TIC_HVEconf_check.Click += new System.EventHandler(this.BTN_TIC_HVEconf_check_Click);
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(6, 48);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 0;
            this.button3.Text = "set";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(6, 19);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 0;
            this.button2.Text = "get";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.LBL_TIC_HVE_ElapsedTime);
            this.groupBox4.Controls.Add(this.LBL_TIC_HVE_Errors);
            this.groupBox4.Controls.Add(this.label25);
            this.groupBox4.Controls.Add(this.label20);
            this.groupBox4.Controls.Add(this.TXB_TIC_HVE_Period);
            this.groupBox4.Controls.Add(this.CHB_TIC_HVE);
            this.groupBox4.Location = new System.Drawing.Point(351, 226);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(298, 88);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Тест HVE мониторинга";
            // 
            // LBL_TIC_HVE_ElapsedTime
            // 
            this.LBL_TIC_HVE_ElapsedTime.AutoSize = true;
            this.LBL_TIC_HVE_ElapsedTime.Location = new System.Drawing.Point(6, 42);
            this.LBL_TIC_HVE_ElapsedTime.Name = "LBL_TIC_HVE_ElapsedTime";
            this.LBL_TIC_HVE_ElapsedTime.Size = new System.Drawing.Size(123, 13);
            this.LBL_TIC_HVE_ElapsedTime.TabIndex = 3;
            this.LBL_TIC_HVE_ElapsedTime.Text = "Прошедшее время: 0 c";
            // 
            // LBL_TIC_HVE_Errors
            // 
            this.LBL_TIC_HVE_Errors.AutoSize = true;
            this.LBL_TIC_HVE_Errors.Location = new System.Drawing.Point(7, 63);
            this.LBL_TIC_HVE_Errors.Name = "LBL_TIC_HVE_Errors";
            this.LBL_TIC_HVE_Errors.Size = new System.Drawing.Size(119, 13);
            this.LBL_TIC_HVE_Errors.TabIndex = 3;
            this.LBL_TIC_HVE_Errors.Text = "Количество ошибок: 0";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(262, 20);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(21, 13);
            this.label25.TabIndex = 2;
            this.label25.Text = "мс";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(97, 20);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(87, 13);
            this.label20.TabIndex = 2;
            this.label20.Text = "Период опроса:";
            // 
            // TXB_TIC_HVE_Period
            // 
            this.TXB_TIC_HVE_Period.Location = new System.Drawing.Point(190, 17);
            this.TXB_TIC_HVE_Period.Name = "TXB_TIC_HVE_Period";
            this.TXB_TIC_HVE_Period.Size = new System.Drawing.Size(66, 20);
            this.TXB_TIC_HVE_Period.TabIndex = 1;
            this.TXB_TIC_HVE_Period.Text = "200";
            this.TXB_TIC_HVE_Period.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CHB_TIC_HVE
            // 
            this.CHB_TIC_HVE.AutoSize = true;
            this.CHB_TIC_HVE.ForeColor = System.Drawing.Color.Red;
            this.CHB_TIC_HVE.Location = new System.Drawing.Point(7, 20);
            this.CHB_TIC_HVE.Name = "CHB_TIC_HVE";
            this.CHB_TIC_HVE.Size = new System.Drawing.Size(84, 17);
            this.CHB_TIC_HVE.TabIndex = 0;
            this.CHB_TIC_HVE.Text = "Выключено";
            this.CHB_TIC_HVE.UseVisualStyleBackColor = true;
            this.CHB_TIC_HVE.CheckedChanged += new System.EventHandler(this.CHB_TIC_HVE_CheckedChanged);
            // 
            // GRB_TIC_DisplayContrast
            // 
            this.GRB_TIC_DisplayContrast.Controls.Add(this.label19);
            this.GRB_TIC_DisplayContrast.Controls.Add(this.TXB_TIC_DisplayContrast);
            this.GRB_TIC_DisplayContrast.Controls.Add(this.BTN_TIC_DisplayContrast_get);
            this.GRB_TIC_DisplayContrast.Controls.Add(this.BTN_TIC_DisplayContrast_set);
            this.GRB_TIC_DisplayContrast.Location = new System.Drawing.Point(3, 255);
            this.GRB_TIC_DisplayContrast.Name = "GRB_TIC_DisplayContrast";
            this.GRB_TIC_DisplayContrast.Size = new System.Drawing.Size(160, 62);
            this.GRB_TIC_DisplayContrast.TabIndex = 1;
            this.GRB_TIC_DisplayContrast.TabStop = false;
            this.GRB_TIC_DisplayContrast.Text = "Контрастность дисплея";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(6, 46);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(134, 13);
            this.label19.TabIndex = 2;
            this.label19.Text = "Значения: от \"-5\" до \"15\"";
            // 
            // TXB_TIC_DisplayContrast
            // 
            this.TXB_TIC_DisplayContrast.Location = new System.Drawing.Point(110, 18);
            this.TXB_TIC_DisplayContrast.Name = "TXB_TIC_DisplayContrast";
            this.TXB_TIC_DisplayContrast.Size = new System.Drawing.Size(44, 20);
            this.TXB_TIC_DisplayContrast.TabIndex = 1;
            this.TXB_TIC_DisplayContrast.Text = "5";
            this.TXB_TIC_DisplayContrast.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // BTN_TIC_DisplayContrast_get
            // 
            this.BTN_TIC_DisplayContrast_get.Location = new System.Drawing.Point(58, 18);
            this.BTN_TIC_DisplayContrast_get.Name = "BTN_TIC_DisplayContrast_get";
            this.BTN_TIC_DisplayContrast_get.Size = new System.Drawing.Size(46, 22);
            this.BTN_TIC_DisplayContrast_get.TabIndex = 0;
            this.BTN_TIC_DisplayContrast_get.Text = "get";
            this.BTN_TIC_DisplayContrast_get.UseVisualStyleBackColor = true;
            this.BTN_TIC_DisplayContrast_get.Click += new System.EventHandler(this.BTN_TIC_DisplayContrast_get_Click);
            // 
            // BTN_TIC_DisplayContrast_set
            // 
            this.BTN_TIC_DisplayContrast_set.Location = new System.Drawing.Point(6, 18);
            this.BTN_TIC_DisplayContrast_set.Name = "BTN_TIC_DisplayContrast_set";
            this.BTN_TIC_DisplayContrast_set.Size = new System.Drawing.Size(46, 22);
            this.BTN_TIC_DisplayContrast_set.TabIndex = 0;
            this.BTN_TIC_DisplayContrast_set.Text = "set";
            this.BTN_TIC_DisplayContrast_set.UseVisualStyleBackColor = true;
            this.BTN_TIC_DisplayContrast_set.Click += new System.EventHandler(this.BTN_TIC_DisplayContrast_set_Click);
            // 
            // TABpanel_4
            // 
            this.TABpanel_4.BackColor = System.Drawing.SystemColors.Control;
            this.TABpanel_4.Controls.Add(this.groupBox2);
            this.TABpanel_4.Controls.Add(this.GPB_Flags);
            this.TABpanel_4.Controls.Add(this.groupBox3);
            this.TABpanel_4.Controls.Add(this.groupBox1);
            this.TABpanel_4.Controls.Add(this.GPB_CONDER);
            this.TABpanel_4.Controls.Add(this.GPB_Heater);
            this.TABpanel_4.Location = new System.Drawing.Point(4, 22);
            this.TABpanel_4.Name = "TABpanel_4";
            this.TABpanel_4.Size = new System.Drawing.Size(652, 320);
            this.TABpanel_4.TabIndex = 3;
            this.TABpanel_4.Text = "Real: SPI";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.BTN_DETECTOR_getDV3voltage);
            this.groupBox2.Controls.Add(this.BTN_DETECTOR_getDV2voltage);
            this.groupBox2.Controls.Add(this.BTN_DETECTOR_getDV1voltage);
            this.groupBox2.Controls.Add(this.TXB_DETECTOR_setDV3voltage);
            this.groupBox2.Controls.Add(this.TXB_DETECTOR_setDV2voltage);
            this.groupBox2.Controls.Add(this.LBL_DETECTOR_getDV3voltage);
            this.groupBox2.Controls.Add(this.LBL_DETECTOR_getDV2voltage);
            this.groupBox2.Controls.Add(this.LBL_DETECTOR_getDV1voltage);
            this.groupBox2.Controls.Add(this.TXB_DETECTOR_setDV1voltage);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.BTN_DETECTOR_setDV3voltage);
            this.groupBox2.Controls.Add(this.BTN_DETECTOR_setDV2voltage);
            this.groupBox2.Controls.Add(this.BTN_DETECTOR_setDV1voltage);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Location = new System.Drawing.Point(249, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(209, 150);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Детектор";
            // 
            // BTN_DETECTOR_getDV3voltage
            // 
            this.BTN_DETECTOR_getDV3voltage.Location = new System.Drawing.Point(103, 93);
            this.BTN_DETECTOR_getDV3voltage.Name = "BTN_DETECTOR_getDV3voltage";
            this.BTN_DETECTOR_getDV3voltage.Size = new System.Drawing.Size(29, 23);
            this.BTN_DETECTOR_getDV3voltage.TabIndex = 3;
            this.BTN_DETECTOR_getDV3voltage.Text = "->";
            this.BTN_DETECTOR_getDV3voltage.UseVisualStyleBackColor = true;
            this.BTN_DETECTOR_getDV3voltage.Click += new System.EventHandler(this.BTN_DETECTOR_getDV3voltage_Click);
            // 
            // BTN_DETECTOR_getDV2voltage
            // 
            this.BTN_DETECTOR_getDV2voltage.Location = new System.Drawing.Point(103, 64);
            this.BTN_DETECTOR_getDV2voltage.Name = "BTN_DETECTOR_getDV2voltage";
            this.BTN_DETECTOR_getDV2voltage.Size = new System.Drawing.Size(29, 23);
            this.BTN_DETECTOR_getDV2voltage.TabIndex = 3;
            this.BTN_DETECTOR_getDV2voltage.Text = "->";
            this.BTN_DETECTOR_getDV2voltage.UseVisualStyleBackColor = true;
            this.BTN_DETECTOR_getDV2voltage.Click += new System.EventHandler(this.BTN_DETECTOR_getDV2voltage_Click);
            // 
            // BTN_DETECTOR_getDV1voltage
            // 
            this.BTN_DETECTOR_getDV1voltage.Location = new System.Drawing.Point(103, 35);
            this.BTN_DETECTOR_getDV1voltage.Name = "BTN_DETECTOR_getDV1voltage";
            this.BTN_DETECTOR_getDV1voltage.Size = new System.Drawing.Size(29, 23);
            this.BTN_DETECTOR_getDV1voltage.TabIndex = 3;
            this.BTN_DETECTOR_getDV1voltage.Text = "->";
            this.BTN_DETECTOR_getDV1voltage.UseVisualStyleBackColor = true;
            this.BTN_DETECTOR_getDV1voltage.Click += new System.EventHandler(this.BTN_DETECTOR_getDV1voltage_Click);
            // 
            // TXB_DETECTOR_setDV3voltage
            // 
            this.TXB_DETECTOR_setDV3voltage.Location = new System.Drawing.Point(56, 95);
            this.TXB_DETECTOR_setDV3voltage.Name = "TXB_DETECTOR_setDV3voltage";
            this.TXB_DETECTOR_setDV3voltage.Size = new System.Drawing.Size(41, 20);
            this.TXB_DETECTOR_setDV3voltage.TabIndex = 2;
            this.TXB_DETECTOR_setDV3voltage.Text = "100";
            // 
            // TXB_DETECTOR_setDV2voltage
            // 
            this.TXB_DETECTOR_setDV2voltage.Location = new System.Drawing.Point(56, 66);
            this.TXB_DETECTOR_setDV2voltage.Name = "TXB_DETECTOR_setDV2voltage";
            this.TXB_DETECTOR_setDV2voltage.Size = new System.Drawing.Size(41, 20);
            this.TXB_DETECTOR_setDV2voltage.TabIndex = 2;
            this.TXB_DETECTOR_setDV2voltage.Text = "250";
            // 
            // LBL_DETECTOR_getDV3voltage
            // 
            this.LBL_DETECTOR_getDV3voltage.AutoSize = true;
            this.LBL_DETECTOR_getDV3voltage.ForeColor = System.Drawing.Color.Red;
            this.LBL_DETECTOR_getDV3voltage.Location = new System.Drawing.Point(138, 98);
            this.LBL_DETECTOR_getDV3voltage.Name = "LBL_DETECTOR_getDV3voltage";
            this.LBL_DETECTOR_getDV3voltage.Size = new System.Drawing.Size(13, 13);
            this.LBL_DETECTOR_getDV3voltage.TabIndex = 3;
            this.LBL_DETECTOR_getDV3voltage.Text = "?";
            // 
            // LBL_DETECTOR_getDV2voltage
            // 
            this.LBL_DETECTOR_getDV2voltage.AutoSize = true;
            this.LBL_DETECTOR_getDV2voltage.ForeColor = System.Drawing.Color.Red;
            this.LBL_DETECTOR_getDV2voltage.Location = new System.Drawing.Point(138, 69);
            this.LBL_DETECTOR_getDV2voltage.Name = "LBL_DETECTOR_getDV2voltage";
            this.LBL_DETECTOR_getDV2voltage.Size = new System.Drawing.Size(13, 13);
            this.LBL_DETECTOR_getDV2voltage.TabIndex = 3;
            this.LBL_DETECTOR_getDV2voltage.Text = "?";
            // 
            // LBL_DETECTOR_getDV1voltage
            // 
            this.LBL_DETECTOR_getDV1voltage.AutoSize = true;
            this.LBL_DETECTOR_getDV1voltage.ForeColor = System.Drawing.Color.Red;
            this.LBL_DETECTOR_getDV1voltage.Location = new System.Drawing.Point(138, 40);
            this.LBL_DETECTOR_getDV1voltage.Name = "LBL_DETECTOR_getDV1voltage";
            this.LBL_DETECTOR_getDV1voltage.Size = new System.Drawing.Size(13, 13);
            this.LBL_DETECTOR_getDV1voltage.TabIndex = 3;
            this.LBL_DETECTOR_getDV1voltage.Text = "?";
            // 
            // TXB_DETECTOR_setDV1voltage
            // 
            this.TXB_DETECTOR_setDV1voltage.Location = new System.Drawing.Point(56, 37);
            this.TXB_DETECTOR_setDV1voltage.Name = "TXB_DETECTOR_setDV1voltage";
            this.TXB_DETECTOR_setDV1voltage.Size = new System.Drawing.Size(41, 20);
            this.TXB_DETECTOR_setDV1voltage.TabIndex = 2;
            this.TXB_DETECTOR_setDV1voltage.Text = "380";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(56, 37);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(41, 20);
            this.textBox1.TabIndex = 2;
            // 
            // BTN_DETECTOR_setDV3voltage
            // 
            this.BTN_DETECTOR_setDV3voltage.Location = new System.Drawing.Point(6, 93);
            this.BTN_DETECTOR_setDV3voltage.Name = "BTN_DETECTOR_setDV3voltage";
            this.BTN_DETECTOR_setDV3voltage.Size = new System.Drawing.Size(44, 23);
            this.BTN_DETECTOR_setDV3voltage.TabIndex = 0;
            this.BTN_DETECTOR_setDV3voltage.Text = "DV3";
            this.BTN_DETECTOR_setDV3voltage.UseVisualStyleBackColor = true;
            this.BTN_DETECTOR_setDV3voltage.Click += new System.EventHandler(this.BTN_DETECTOR_setDV3voltage_Click);
            // 
            // BTN_DETECTOR_setDV2voltage
            // 
            this.BTN_DETECTOR_setDV2voltage.Location = new System.Drawing.Point(6, 64);
            this.BTN_DETECTOR_setDV2voltage.Name = "BTN_DETECTOR_setDV2voltage";
            this.BTN_DETECTOR_setDV2voltage.Size = new System.Drawing.Size(44, 23);
            this.BTN_DETECTOR_setDV2voltage.TabIndex = 0;
            this.BTN_DETECTOR_setDV2voltage.Text = "DV2";
            this.BTN_DETECTOR_setDV2voltage.UseVisualStyleBackColor = true;
            this.BTN_DETECTOR_setDV2voltage.Click += new System.EventHandler(this.BTN_DETECTOR_setDV2voltage_Click);
            // 
            // BTN_DETECTOR_setDV1voltage
            // 
            this.BTN_DETECTOR_setDV1voltage.Location = new System.Drawing.Point(6, 35);
            this.BTN_DETECTOR_setDV1voltage.Name = "BTN_DETECTOR_setDV1voltage";
            this.BTN_DETECTOR_setDV1voltage.Size = new System.Drawing.Size(44, 23);
            this.BTN_DETECTOR_setDV1voltage.TabIndex = 0;
            this.BTN_DETECTOR_setDV1voltage.Text = "DV1";
            this.BTN_DETECTOR_setDV1voltage.UseVisualStyleBackColor = true;
            this.BTN_DETECTOR_setDV1voltage.Click += new System.EventHandler(this.BTN_DETECTOR_setDV1voltage_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 16);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(108, 13);
            this.label13.TabIndex = 1;
            this.label13.Text = "Задать напряжения";
            // 
            // GPB_Flags
            // 
            this.GPB_Flags.Controls.Add(this.button1);
            this.GPB_Flags.Controls.Add(this.BTN_checkFlags);
            this.GPB_Flags.Controls.Add(this.CHB_iEDCD);
            this.GPB_Flags.Controls.Add(this.CHB_SEMV3);
            this.GPB_Flags.Controls.Add(this.CHB_iHVE);
            this.GPB_Flags.Controls.Add(this.CHB_PRGE);
            this.GPB_Flags.Controls.Add(this.CHB_SEMV2);
            this.GPB_Flags.Controls.Add(this.CHB_SPUMP);
            this.GPB_Flags.Controls.Add(this.CHB_SEMV1);
            this.GPB_Flags.Location = new System.Drawing.Point(500, 141);
            this.GPB_Flags.Name = "GPB_Flags";
            this.GPB_Flags.Size = new System.Drawing.Size(143, 171);
            this.GPB_Flags.TabIndex = 5;
            this.GPB_Flags.TabStop = false;
            this.GPB_Flags.Text = "Флаги";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 139);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(130, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Установить";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // BTN_checkFlags
            // 
            this.BTN_checkFlags.Location = new System.Drawing.Point(7, 110);
            this.BTN_checkFlags.Name = "BTN_checkFlags";
            this.BTN_checkFlags.Size = new System.Drawing.Size(130, 23);
            this.BTN_checkFlags.TabIndex = 6;
            this.BTN_checkFlags.Text = "Проверить";
            this.BTN_checkFlags.UseVisualStyleBackColor = true;
            this.BTN_checkFlags.Click += new System.EventHandler(this.BTN_checkFlags_Click);
            // 
            // CHB_iEDCD
            // 
            this.CHB_iEDCD.AutoSize = true;
            this.CHB_iEDCD.Checked = true;
            this.CHB_iEDCD.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHB_iEDCD.Location = new System.Drawing.Point(7, 85);
            this.CHB_iEDCD.Name = "CHB_iEDCD";
            this.CHB_iEDCD.Size = new System.Drawing.Size(56, 17);
            this.CHB_iEDCD.TabIndex = 1;
            this.CHB_iEDCD.Text = "EDCD";
            this.CHB_iEDCD.UseVisualStyleBackColor = true;
            // 
            // CHB_SEMV3
            // 
            this.CHB_SEMV3.AutoSize = true;
            this.CHB_SEMV3.Location = new System.Drawing.Point(6, 65);
            this.CHB_SEMV3.Name = "CHB_SEMV3";
            this.CHB_SEMV3.Size = new System.Drawing.Size(62, 17);
            this.CHB_SEMV3.TabIndex = 1;
            this.CHB_SEMV3.Text = "SEMV3";
            this.CHB_SEMV3.UseVisualStyleBackColor = true;
            // 
            // CHB_iHVE
            // 
            this.CHB_iHVE.AutoSize = true;
            this.CHB_iHVE.Checked = true;
            this.CHB_iHVE.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.CHB_iHVE.Enabled = false;
            this.CHB_iHVE.Location = new System.Drawing.Point(74, 65);
            this.CHB_iHVE.Name = "CHB_iHVE";
            this.CHB_iHVE.Size = new System.Drawing.Size(48, 17);
            this.CHB_iHVE.TabIndex = 1;
            this.CHB_iHVE.Text = "HVE";
            this.CHB_iHVE.UseVisualStyleBackColor = true;
            // 
            // CHB_PRGE
            // 
            this.CHB_PRGE.AutoSize = true;
            this.CHB_PRGE.Location = new System.Drawing.Point(74, 42);
            this.CHB_PRGE.Name = "CHB_PRGE";
            this.CHB_PRGE.Size = new System.Drawing.Size(56, 17);
            this.CHB_PRGE.TabIndex = 1;
            this.CHB_PRGE.Text = "PRGE";
            this.CHB_PRGE.UseVisualStyleBackColor = true;
            // 
            // CHB_SEMV2
            // 
            this.CHB_SEMV2.AutoSize = true;
            this.CHB_SEMV2.Location = new System.Drawing.Point(6, 42);
            this.CHB_SEMV2.Name = "CHB_SEMV2";
            this.CHB_SEMV2.Size = new System.Drawing.Size(62, 17);
            this.CHB_SEMV2.TabIndex = 1;
            this.CHB_SEMV2.Text = "SEMV2";
            this.CHB_SEMV2.UseVisualStyleBackColor = true;
            // 
            // CHB_SPUMP
            // 
            this.CHB_SPUMP.AutoSize = true;
            this.CHB_SPUMP.Location = new System.Drawing.Point(74, 19);
            this.CHB_SPUMP.Name = "CHB_SPUMP";
            this.CHB_SPUMP.Size = new System.Drawing.Size(64, 17);
            this.CHB_SPUMP.TabIndex = 1;
            this.CHB_SPUMP.Text = "SPUMP";
            this.CHB_SPUMP.UseVisualStyleBackColor = true;
            // 
            // CHB_SEMV1
            // 
            this.CHB_SEMV1.AutoSize = true;
            this.CHB_SEMV1.Location = new System.Drawing.Point(6, 19);
            this.CHB_SEMV1.Name = "CHB_SEMV1";
            this.CHB_SEMV1.Size = new System.Drawing.Size(62, 17);
            this.CHB_SEMV1.TabIndex = 1;
            this.CHB_SEMV1.Text = "SEMV1";
            this.CHB_SEMV1.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.LBL_IonSOURCE_getF2voltage);
            this.groupBox3.Controls.Add(this.LBL_IonSOURCE_getF1voltage);
            this.groupBox3.Controls.Add(this.LBL_IonSOURCE_getIonizationVoltage);
            this.groupBox3.Controls.Add(this.LBL_IonSOURCE_getEmissionCurrentVoltage);
            this.groupBox3.Controls.Add(this.TXB_IonSOURCE_setF2voltage);
            this.groupBox3.Controls.Add(this.TXB_IonSOURCE_setF1voltage);
            this.groupBox3.Controls.Add(this.TXB_IonSOURCE_setIonizationVoltage);
            this.groupBox3.Controls.Add(this.TXB_IonSOURCE_setEmissionCurrentVoltage);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.BTN_IonSOURCE_setF2voltage);
            this.groupBox3.Controls.Add(this.BTN_IonSOURCE_setF1voltage);
            this.groupBox3.Controls.Add(this.BTN_IonSOURCE_setIonizationVoltage);
            this.groupBox3.Controls.Add(this.BTN_IonSOURCE_getF2voltage);
            this.groupBox3.Controls.Add(this.BTN_IonSOURCE_getF1voltage);
            this.groupBox3.Controls.Add(this.BTN_IonSOURCE_getIonizationVoltage);
            this.groupBox3.Controls.Add(this.BTN_IonSOURCE_getEmissionCurrentVoltage);
            this.groupBox3.Controls.Add(this.BTN_IonSOURCE_setEmissionCurrentVoltage);
            this.groupBox3.Location = new System.Drawing.Point(9, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(234, 150);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Ионный источник";
            // 
            // LBL_IonSOURCE_getF2voltage
            // 
            this.LBL_IonSOURCE_getF2voltage.AutoSize = true;
            this.LBL_IonSOURCE_getF2voltage.ForeColor = System.Drawing.Color.Red;
            this.LBL_IonSOURCE_getF2voltage.Location = new System.Drawing.Point(182, 126);
            this.LBL_IonSOURCE_getF2voltage.Name = "LBL_IonSOURCE_getF2voltage";
            this.LBL_IonSOURCE_getF2voltage.Size = new System.Drawing.Size(13, 13);
            this.LBL_IonSOURCE_getF2voltage.TabIndex = 3;
            this.LBL_IonSOURCE_getF2voltage.Text = "?";
            // 
            // LBL_IonSOURCE_getF1voltage
            // 
            this.LBL_IonSOURCE_getF1voltage.AutoSize = true;
            this.LBL_IonSOURCE_getF1voltage.ForeColor = System.Drawing.Color.Red;
            this.LBL_IonSOURCE_getF1voltage.Location = new System.Drawing.Point(182, 98);
            this.LBL_IonSOURCE_getF1voltage.Name = "LBL_IonSOURCE_getF1voltage";
            this.LBL_IonSOURCE_getF1voltage.Size = new System.Drawing.Size(13, 13);
            this.LBL_IonSOURCE_getF1voltage.TabIndex = 3;
            this.LBL_IonSOURCE_getF1voltage.Text = "?";
            // 
            // LBL_IonSOURCE_getIonizationVoltage
            // 
            this.LBL_IonSOURCE_getIonizationVoltage.AutoSize = true;
            this.LBL_IonSOURCE_getIonizationVoltage.ForeColor = System.Drawing.Color.Red;
            this.LBL_IonSOURCE_getIonizationVoltage.Location = new System.Drawing.Point(182, 69);
            this.LBL_IonSOURCE_getIonizationVoltage.Name = "LBL_IonSOURCE_getIonizationVoltage";
            this.LBL_IonSOURCE_getIonizationVoltage.Size = new System.Drawing.Size(13, 13);
            this.LBL_IonSOURCE_getIonizationVoltage.TabIndex = 3;
            this.LBL_IonSOURCE_getIonizationVoltage.Text = "?";
            // 
            // LBL_IonSOURCE_getEmissionCurrentVoltage
            // 
            this.LBL_IonSOURCE_getEmissionCurrentVoltage.AutoSize = true;
            this.LBL_IonSOURCE_getEmissionCurrentVoltage.ForeColor = System.Drawing.Color.Red;
            this.LBL_IonSOURCE_getEmissionCurrentVoltage.Location = new System.Drawing.Point(182, 40);
            this.LBL_IonSOURCE_getEmissionCurrentVoltage.Name = "LBL_IonSOURCE_getEmissionCurrentVoltage";
            this.LBL_IonSOURCE_getEmissionCurrentVoltage.Size = new System.Drawing.Size(13, 13);
            this.LBL_IonSOURCE_getEmissionCurrentVoltage.TabIndex = 3;
            this.LBL_IonSOURCE_getEmissionCurrentVoltage.Text = "?";
            // 
            // TXB_IonSOURCE_setF2voltage
            // 
            this.TXB_IonSOURCE_setF2voltage.Location = new System.Drawing.Point(93, 123);
            this.TXB_IonSOURCE_setF2voltage.Name = "TXB_IonSOURCE_setF2voltage";
            this.TXB_IonSOURCE_setF2voltage.Size = new System.Drawing.Size(44, 20);
            this.TXB_IonSOURCE_setF2voltage.TabIndex = 2;
            this.TXB_IonSOURCE_setF2voltage.Text = "1500";
            // 
            // TXB_IonSOURCE_setF1voltage
            // 
            this.TXB_IonSOURCE_setF1voltage.Location = new System.Drawing.Point(93, 95);
            this.TXB_IonSOURCE_setF1voltage.Name = "TXB_IonSOURCE_setF1voltage";
            this.TXB_IonSOURCE_setF1voltage.Size = new System.Drawing.Size(44, 20);
            this.TXB_IonSOURCE_setF1voltage.TabIndex = 2;
            this.TXB_IonSOURCE_setF1voltage.Text = "2500";
            // 
            // TXB_IonSOURCE_setIonizationVoltage
            // 
            this.TXB_IonSOURCE_setIonizationVoltage.Location = new System.Drawing.Point(93, 66);
            this.TXB_IonSOURCE_setIonizationVoltage.Name = "TXB_IonSOURCE_setIonizationVoltage";
            this.TXB_IonSOURCE_setIonizationVoltage.Size = new System.Drawing.Size(44, 20);
            this.TXB_IonSOURCE_setIonizationVoltage.TabIndex = 2;
            this.TXB_IonSOURCE_setIonizationVoltage.Text = "3000";
            // 
            // TXB_IonSOURCE_setEmissionCurrentVoltage
            // 
            this.TXB_IonSOURCE_setEmissionCurrentVoltage.Location = new System.Drawing.Point(93, 37);
            this.TXB_IonSOURCE_setEmissionCurrentVoltage.Name = "TXB_IonSOURCE_setEmissionCurrentVoltage";
            this.TXB_IonSOURCE_setEmissionCurrentVoltage.Size = new System.Drawing.Size(44, 20);
            this.TXB_IonSOURCE_setEmissionCurrentVoltage.TabIndex = 2;
            this.TXB_IonSOURCE_setEmissionCurrentVoltage.Text = "3500";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 16);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(108, 13);
            this.label12.TabIndex = 1;
            this.label12.Text = "Задать напряжения";
            // 
            // BTN_IonSOURCE_setF2voltage
            // 
            this.BTN_IonSOURCE_setF2voltage.Location = new System.Drawing.Point(6, 121);
            this.BTN_IonSOURCE_setF2voltage.Name = "BTN_IonSOURCE_setF2voltage";
            this.BTN_IonSOURCE_setF2voltage.Size = new System.Drawing.Size(82, 23);
            this.BTN_IonSOURCE_setF2voltage.TabIndex = 0;
            this.BTN_IonSOURCE_setF2voltage.Text = "Фокусное 2";
            this.BTN_IonSOURCE_setF2voltage.UseVisualStyleBackColor = true;
            this.BTN_IonSOURCE_setF2voltage.Click += new System.EventHandler(this.BTN_IonSOURCE_setF2voltage_Click);
            // 
            // BTN_IonSOURCE_setF1voltage
            // 
            this.BTN_IonSOURCE_setF1voltage.Location = new System.Drawing.Point(6, 93);
            this.BTN_IonSOURCE_setF1voltage.Name = "BTN_IonSOURCE_setF1voltage";
            this.BTN_IonSOURCE_setF1voltage.Size = new System.Drawing.Size(82, 23);
            this.BTN_IonSOURCE_setF1voltage.TabIndex = 0;
            this.BTN_IonSOURCE_setF1voltage.Text = "Фокусное 1";
            this.BTN_IonSOURCE_setF1voltage.UseVisualStyleBackColor = true;
            this.BTN_IonSOURCE_setF1voltage.Click += new System.EventHandler(this.BTN_IonSOURCE_setF1voltage_Click);
            // 
            // BTN_IonSOURCE_setIonizationVoltage
            // 
            this.BTN_IonSOURCE_setIonizationVoltage.Location = new System.Drawing.Point(6, 64);
            this.BTN_IonSOURCE_setIonizationVoltage.Name = "BTN_IonSOURCE_setIonizationVoltage";
            this.BTN_IonSOURCE_setIonizationVoltage.Size = new System.Drawing.Size(82, 23);
            this.BTN_IonSOURCE_setIonizationVoltage.TabIndex = 0;
            this.BTN_IonSOURCE_setIonizationVoltage.Text = "Ионизации";
            this.BTN_IonSOURCE_setIonizationVoltage.UseVisualStyleBackColor = true;
            this.BTN_IonSOURCE_setIonizationVoltage.Click += new System.EventHandler(this.BTN_IonSOURCE_setIonizationVoltage_Click);
            // 
            // BTN_IonSOURCE_getF2voltage
            // 
            this.BTN_IonSOURCE_getF2voltage.Location = new System.Drawing.Point(143, 121);
            this.BTN_IonSOURCE_getF2voltage.Name = "BTN_IonSOURCE_getF2voltage";
            this.BTN_IonSOURCE_getF2voltage.Size = new System.Drawing.Size(33, 23);
            this.BTN_IonSOURCE_getF2voltage.TabIndex = 0;
            this.BTN_IonSOURCE_getF2voltage.Text = "->";
            this.BTN_IonSOURCE_getF2voltage.UseVisualStyleBackColor = true;
            this.BTN_IonSOURCE_getF2voltage.Click += new System.EventHandler(this.BTN_IonSOURCE_getF2voltage_Click);
            // 
            // BTN_IonSOURCE_getF1voltage
            // 
            this.BTN_IonSOURCE_getF1voltage.Location = new System.Drawing.Point(143, 93);
            this.BTN_IonSOURCE_getF1voltage.Name = "BTN_IonSOURCE_getF1voltage";
            this.BTN_IonSOURCE_getF1voltage.Size = new System.Drawing.Size(33, 23);
            this.BTN_IonSOURCE_getF1voltage.TabIndex = 0;
            this.BTN_IonSOURCE_getF1voltage.Text = "->";
            this.BTN_IonSOURCE_getF1voltage.UseVisualStyleBackColor = true;
            this.BTN_IonSOURCE_getF1voltage.Click += new System.EventHandler(this.BTN_IonSOURCE_getF1voltage_Click);
            // 
            // BTN_IonSOURCE_getIonizationVoltage
            // 
            this.BTN_IonSOURCE_getIonizationVoltage.Location = new System.Drawing.Point(143, 64);
            this.BTN_IonSOURCE_getIonizationVoltage.Name = "BTN_IonSOURCE_getIonizationVoltage";
            this.BTN_IonSOURCE_getIonizationVoltage.Size = new System.Drawing.Size(33, 23);
            this.BTN_IonSOURCE_getIonizationVoltage.TabIndex = 0;
            this.BTN_IonSOURCE_getIonizationVoltage.Text = "->";
            this.BTN_IonSOURCE_getIonizationVoltage.UseVisualStyleBackColor = true;
            this.BTN_IonSOURCE_getIonizationVoltage.Click += new System.EventHandler(this.BTN_IonSOURCE_getIonizationVoltage_Click);
            // 
            // BTN_IonSOURCE_getEmissionCurrentVoltage
            // 
            this.BTN_IonSOURCE_getEmissionCurrentVoltage.Location = new System.Drawing.Point(143, 35);
            this.BTN_IonSOURCE_getEmissionCurrentVoltage.Name = "BTN_IonSOURCE_getEmissionCurrentVoltage";
            this.BTN_IonSOURCE_getEmissionCurrentVoltage.Size = new System.Drawing.Size(33, 23);
            this.BTN_IonSOURCE_getEmissionCurrentVoltage.TabIndex = 0;
            this.BTN_IonSOURCE_getEmissionCurrentVoltage.Text = "->";
            this.BTN_IonSOURCE_getEmissionCurrentVoltage.UseVisualStyleBackColor = true;
            this.BTN_IonSOURCE_getEmissionCurrentVoltage.Click += new System.EventHandler(this.BTN_IonSOURCE_getEmissionCurrentVoltage_Click);
            // 
            // BTN_IonSOURCE_setEmissionCurrentVoltage
            // 
            this.BTN_IonSOURCE_setEmissionCurrentVoltage.Location = new System.Drawing.Point(6, 35);
            this.BTN_IonSOURCE_setEmissionCurrentVoltage.Name = "BTN_IonSOURCE_setEmissionCurrentVoltage";
            this.BTN_IonSOURCE_setEmissionCurrentVoltage.Size = new System.Drawing.Size(82, 23);
            this.BTN_IonSOURCE_setEmissionCurrentVoltage.TabIndex = 0;
            this.BTN_IonSOURCE_setEmissionCurrentVoltage.Text = "Эмиссии";
            this.BTN_IonSOURCE_setEmissionCurrentVoltage.UseVisualStyleBackColor = true;
            this.BTN_IonSOURCE_setEmissionCurrentVoltage.Click += new System.EventHandler(this.BTN_IonSOURCE_setEmissionCurrentVoltage_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BTN_SCANER_getScanVoltage);
            this.groupBox1.Controls.Add(this.BTN_SCANER_getParentScanVoltage);
            this.groupBox1.Controls.Add(this.LBL_SCANER_getScanVoltage);
            this.groupBox1.Controls.Add(this.TXB_SCANER_setScanVoltage);
            this.groupBox1.Controls.Add(this.TXB_SCANER_setParentScanVoltage);
            this.groupBox1.Controls.Add(this.BTN_SCANER_setParentScanVoltage);
            this.groupBox1.Controls.Add(this.LBL_SCANER_getParentScanVoltage);
            this.groupBox1.Controls.Add(this.BTN_SCANER_setScanVoltage);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Location = new System.Drawing.Point(249, 159);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(245, 95);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Сканер";
            // 
            // BTN_SCANER_getScanVoltage
            // 
            this.BTN_SCANER_getScanVoltage.Location = new System.Drawing.Point(165, 65);
            this.BTN_SCANER_getScanVoltage.Name = "BTN_SCANER_getScanVoltage";
            this.BTN_SCANER_getScanVoltage.Size = new System.Drawing.Size(33, 23);
            this.BTN_SCANER_getScanVoltage.TabIndex = 7;
            this.BTN_SCANER_getScanVoltage.Text = "->";
            this.BTN_SCANER_getScanVoltage.UseVisualStyleBackColor = true;
            this.BTN_SCANER_getScanVoltage.Click += new System.EventHandler(this.BTN_SCANER_getScanVoltage_Click);
            // 
            // BTN_SCANER_getParentScanVoltage
            // 
            this.BTN_SCANER_getParentScanVoltage.Location = new System.Drawing.Point(165, 36);
            this.BTN_SCANER_getParentScanVoltage.Name = "BTN_SCANER_getParentScanVoltage";
            this.BTN_SCANER_getParentScanVoltage.Size = new System.Drawing.Size(33, 23);
            this.BTN_SCANER_getParentScanVoltage.TabIndex = 7;
            this.BTN_SCANER_getParentScanVoltage.Text = "->";
            this.BTN_SCANER_getParentScanVoltage.UseVisualStyleBackColor = true;
            this.BTN_SCANER_getParentScanVoltage.Click += new System.EventHandler(this.BTN_SCANER_getParentScanVoltage_Click);
            // 
            // LBL_SCANER_getScanVoltage
            // 
            this.LBL_SCANER_getScanVoltage.AutoSize = true;
            this.LBL_SCANER_getScanVoltage.ForeColor = System.Drawing.Color.Red;
            this.LBL_SCANER_getScanVoltage.Location = new System.Drawing.Point(204, 71);
            this.LBL_SCANER_getScanVoltage.Name = "LBL_SCANER_getScanVoltage";
            this.LBL_SCANER_getScanVoltage.Size = new System.Drawing.Size(13, 13);
            this.LBL_SCANER_getScanVoltage.TabIndex = 3;
            this.LBL_SCANER_getScanVoltage.Text = "?";
            // 
            // TXB_SCANER_setScanVoltage
            // 
            this.TXB_SCANER_setScanVoltage.Location = new System.Drawing.Point(115, 67);
            this.TXB_SCANER_setScanVoltage.Name = "TXB_SCANER_setScanVoltage";
            this.TXB_SCANER_setScanVoltage.Size = new System.Drawing.Size(44, 20);
            this.TXB_SCANER_setScanVoltage.TabIndex = 6;
            this.TXB_SCANER_setScanVoltage.Text = "500";
            // 
            // TXB_SCANER_setParentScanVoltage
            // 
            this.TXB_SCANER_setParentScanVoltage.Location = new System.Drawing.Point(115, 38);
            this.TXB_SCANER_setParentScanVoltage.Name = "TXB_SCANER_setParentScanVoltage";
            this.TXB_SCANER_setParentScanVoltage.Size = new System.Drawing.Size(44, 20);
            this.TXB_SCANER_setParentScanVoltage.TabIndex = 6;
            this.TXB_SCANER_setParentScanVoltage.Text = "750";
            // 
            // BTN_SCANER_setParentScanVoltage
            // 
            this.BTN_SCANER_setParentScanVoltage.Location = new System.Drawing.Point(5, 36);
            this.BTN_SCANER_setParentScanVoltage.Name = "BTN_SCANER_setParentScanVoltage";
            this.BTN_SCANER_setParentScanVoltage.Size = new System.Drawing.Size(104, 23);
            this.BTN_SCANER_setParentScanVoltage.TabIndex = 0;
            this.BTN_SCANER_setParentScanVoltage.Text = "Дополнительное";
            this.BTN_SCANER_setParentScanVoltage.UseVisualStyleBackColor = true;
            this.BTN_SCANER_setParentScanVoltage.Click += new System.EventHandler(this.BTN_SCANER_setParentScanVoltage_Click);
            // 
            // LBL_SCANER_getParentScanVoltage
            // 
            this.LBL_SCANER_getParentScanVoltage.AutoSize = true;
            this.LBL_SCANER_getParentScanVoltage.ForeColor = System.Drawing.Color.Red;
            this.LBL_SCANER_getParentScanVoltage.Location = new System.Drawing.Point(204, 41);
            this.LBL_SCANER_getParentScanVoltage.Name = "LBL_SCANER_getParentScanVoltage";
            this.LBL_SCANER_getParentScanVoltage.Size = new System.Drawing.Size(13, 13);
            this.LBL_SCANER_getParentScanVoltage.TabIndex = 3;
            this.LBL_SCANER_getParentScanVoltage.Text = "?";
            // 
            // BTN_SCANER_setScanVoltage
            // 
            this.BTN_SCANER_setScanVoltage.Location = new System.Drawing.Point(5, 65);
            this.BTN_SCANER_setScanVoltage.Name = "BTN_SCANER_setScanVoltage";
            this.BTN_SCANER_setScanVoltage.Size = new System.Drawing.Size(104, 23);
            this.BTN_SCANER_setScanVoltage.TabIndex = 0;
            this.BTN_SCANER_setScanVoltage.Text = "Сканирующее";
            this.BTN_SCANER_setScanVoltage.UseVisualStyleBackColor = true;
            this.BTN_SCANER_setScanVoltage.Click += new System.EventHandler(this.BTN_SCANER_setScanVoltage_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 17);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(108, 13);
            this.label14.TabIndex = 1;
            this.label14.Text = "Задать напряжения";
            // 
            // GPB_CONDER
            // 
            this.GPB_CONDER.Controls.Add(this.BTN_CONDENSATOR_getNegativeVoltage);
            this.GPB_CONDER.Controls.Add(this.BTN_CONDENSATOR_getPositiveVoltage);
            this.GPB_CONDER.Controls.Add(this.BTN_CONDENSATOR_setVoltage);
            this.GPB_CONDER.Controls.Add(this.LBL_CONDENSATOR_getPositiveVoltage);
            this.GPB_CONDER.Controls.Add(this.LBL_CONDENSATOR_getNegativeVoltage);
            this.GPB_CONDER.Controls.Add(this.label18);
            this.GPB_CONDER.Controls.Add(this.TXB_CONDENSATOR_setVoltage);
            this.GPB_CONDER.Location = new System.Drawing.Point(9, 260);
            this.GPB_CONDER.Name = "GPB_CONDER";
            this.GPB_CONDER.Size = new System.Drawing.Size(340, 57);
            this.GPB_CONDER.TabIndex = 5;
            this.GPB_CONDER.TabStop = false;
            this.GPB_CONDER.Text = "Конденсатор";
            // 
            // BTN_CONDENSATOR_getNegativeVoltage
            // 
            this.BTN_CONDENSATOR_getNegativeVoltage.Location = new System.Drawing.Point(240, 29);
            this.BTN_CONDENSATOR_getNegativeVoltage.Name = "BTN_CONDENSATOR_getNegativeVoltage";
            this.BTN_CONDENSATOR_getNegativeVoltage.Size = new System.Drawing.Size(37, 23);
            this.BTN_CONDENSATOR_getNegativeVoltage.TabIndex = 5;
            this.BTN_CONDENSATOR_getNegativeVoltage.Text = "-> -";
            this.BTN_CONDENSATOR_getNegativeVoltage.UseVisualStyleBackColor = true;
            this.BTN_CONDENSATOR_getNegativeVoltage.Click += new System.EventHandler(this.BTN_CONDENSATOR_getNegativeVoltage_Click);
            // 
            // BTN_CONDENSATOR_getPositiveVoltage
            // 
            this.BTN_CONDENSATOR_getPositiveVoltage.Location = new System.Drawing.Point(143, 30);
            this.BTN_CONDENSATOR_getPositiveVoltage.Name = "BTN_CONDENSATOR_getPositiveVoltage";
            this.BTN_CONDENSATOR_getPositiveVoltage.Size = new System.Drawing.Size(37, 23);
            this.BTN_CONDENSATOR_getPositiveVoltage.TabIndex = 5;
            this.BTN_CONDENSATOR_getPositiveVoltage.Text = "-> +";
            this.BTN_CONDENSATOR_getPositiveVoltage.UseVisualStyleBackColor = true;
            this.BTN_CONDENSATOR_getPositiveVoltage.Click += new System.EventHandler(this.BTN_CONDENSATOR_getPositiveVoltage_Click);
            // 
            // BTN_CONDENSATOR_setVoltage
            // 
            this.BTN_CONDENSATOR_setVoltage.Location = new System.Drawing.Point(6, 30);
            this.BTN_CONDENSATOR_setVoltage.Name = "BTN_CONDENSATOR_setVoltage";
            this.BTN_CONDENSATOR_setVoltage.Size = new System.Drawing.Size(82, 23);
            this.BTN_CONDENSATOR_setVoltage.TabIndex = 4;
            this.BTN_CONDENSATOR_setVoltage.Text = "Конденсатор";
            this.BTN_CONDENSATOR_setVoltage.UseVisualStyleBackColor = true;
            this.BTN_CONDENSATOR_setVoltage.Click += new System.EventHandler(this.BTN_CONDENSATOR_setVoltage_Click);
            // 
            // LBL_CONDENSATOR_getPositiveVoltage
            // 
            this.LBL_CONDENSATOR_getPositiveVoltage.AutoSize = true;
            this.LBL_CONDENSATOR_getPositiveVoltage.ForeColor = System.Drawing.Color.Red;
            this.LBL_CONDENSATOR_getPositiveVoltage.Location = new System.Drawing.Point(186, 35);
            this.LBL_CONDENSATOR_getPositiveVoltage.Name = "LBL_CONDENSATOR_getPositiveVoltage";
            this.LBL_CONDENSATOR_getPositiveVoltage.Size = new System.Drawing.Size(13, 13);
            this.LBL_CONDENSATOR_getPositiveVoltage.TabIndex = 3;
            this.LBL_CONDENSATOR_getPositiveVoltage.Text = "?";
            // 
            // LBL_CONDENSATOR_getNegativeVoltage
            // 
            this.LBL_CONDENSATOR_getNegativeVoltage.AutoSize = true;
            this.LBL_CONDENSATOR_getNegativeVoltage.ForeColor = System.Drawing.Color.Red;
            this.LBL_CONDENSATOR_getNegativeVoltage.Location = new System.Drawing.Point(283, 34);
            this.LBL_CONDENSATOR_getNegativeVoltage.Name = "LBL_CONDENSATOR_getNegativeVoltage";
            this.LBL_CONDENSATOR_getNegativeVoltage.Size = new System.Drawing.Size(13, 13);
            this.LBL_CONDENSATOR_getNegativeVoltage.TabIndex = 3;
            this.LBL_CONDENSATOR_getNegativeVoltage.Text = "?";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 14);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(108, 13);
            this.label18.TabIndex = 1;
            this.label18.Text = "Задать напряжение";
            // 
            // TXB_CONDENSATOR_setVoltage
            // 
            this.TXB_CONDENSATOR_setVoltage.Location = new System.Drawing.Point(93, 32);
            this.TXB_CONDENSATOR_setVoltage.Name = "TXB_CONDENSATOR_setVoltage";
            this.TXB_CONDENSATOR_setVoltage.Size = new System.Drawing.Size(44, 20);
            this.TXB_CONDENSATOR_setVoltage.TabIndex = 2;
            this.TXB_CONDENSATOR_setVoltage.Text = "1250";
            // 
            // GPB_Heater
            // 
            this.GPB_Heater.Controls.Add(this.BTN_HEATER_setVoltage);
            this.GPB_Heater.Controls.Add(this.LBL_HEATER_getVoltage);
            this.GPB_Heater.Controls.Add(this.BTN_INLET_setVoltage);
            this.GPB_Heater.Controls.Add(this.TXB_HEATER_setVoltage);
            this.GPB_Heater.Controls.Add(this.label15);
            this.GPB_Heater.Controls.Add(this.LBL_INLET_getVoltage);
            this.GPB_Heater.Controls.Add(this._BTN_HEATER_getVoltage);
            this.GPB_Heater.Controls.Add(this.TXB_INLET_setVoltage);
            this.GPB_Heater.Controls.Add(this.BTN_INLET_getVoltage);
            this.GPB_Heater.Location = new System.Drawing.Point(9, 159);
            this.GPB_Heater.Name = "GPB_Heater";
            this.GPB_Heater.Size = new System.Drawing.Size(234, 95);
            this.GPB_Heater.TabIndex = 4;
            this.GPB_Heater.TabStop = false;
            this.GPB_Heater.Text = "Натекатель и нагреватель";
            // 
            // BTN_HEATER_setVoltage
            // 
            this.BTN_HEATER_setVoltage.Location = new System.Drawing.Point(6, 66);
            this.BTN_HEATER_setVoltage.Name = "BTN_HEATER_setVoltage";
            this.BTN_HEATER_setVoltage.Size = new System.Drawing.Size(82, 23);
            this.BTN_HEATER_setVoltage.TabIndex = 0;
            this.BTN_HEATER_setVoltage.Text = "Нагреватель";
            this.BTN_HEATER_setVoltage.UseVisualStyleBackColor = true;
            this.BTN_HEATER_setVoltage.Click += new System.EventHandler(this.BTN_HEATER_setVoltage_Click);
            // 
            // LBL_HEATER_getVoltage
            // 
            this.LBL_HEATER_getVoltage.AutoSize = true;
            this.LBL_HEATER_getVoltage.ForeColor = System.Drawing.Color.Red;
            this.LBL_HEATER_getVoltage.Location = new System.Drawing.Point(182, 71);
            this.LBL_HEATER_getVoltage.Name = "LBL_HEATER_getVoltage";
            this.LBL_HEATER_getVoltage.Size = new System.Drawing.Size(13, 13);
            this.LBL_HEATER_getVoltage.TabIndex = 3;
            this.LBL_HEATER_getVoltage.Text = "?";
            // 
            // BTN_INLET_setVoltage
            // 
            this.BTN_INLET_setVoltage.Location = new System.Drawing.Point(6, 36);
            this.BTN_INLET_setVoltage.Name = "BTN_INLET_setVoltage";
            this.BTN_INLET_setVoltage.Size = new System.Drawing.Size(82, 23);
            this.BTN_INLET_setVoltage.TabIndex = 0;
            this.BTN_INLET_setVoltage.Text = "Натекатель";
            this.BTN_INLET_setVoltage.UseVisualStyleBackColor = true;
            this.BTN_INLET_setVoltage.Click += new System.EventHandler(this.BTN_INLET_setVoltage_Click);
            // 
            // TXB_HEATER_setVoltage
            // 
            this.TXB_HEATER_setVoltage.Location = new System.Drawing.Point(93, 68);
            this.TXB_HEATER_setVoltage.Name = "TXB_HEATER_setVoltage";
            this.TXB_HEATER_setVoltage.Size = new System.Drawing.Size(44, 20);
            this.TXB_HEATER_setVoltage.TabIndex = 2;
            this.TXB_HEATER_setVoltage.Text = "2000";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 17);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(108, 13);
            this.label15.TabIndex = 1;
            this.label15.Text = "Задать напряжение";
            // 
            // LBL_INLET_getVoltage
            // 
            this.LBL_INLET_getVoltage.AutoSize = true;
            this.LBL_INLET_getVoltage.ForeColor = System.Drawing.Color.Red;
            this.LBL_INLET_getVoltage.Location = new System.Drawing.Point(182, 42);
            this.LBL_INLET_getVoltage.Name = "LBL_INLET_getVoltage";
            this.LBL_INLET_getVoltage.Size = new System.Drawing.Size(13, 13);
            this.LBL_INLET_getVoltage.TabIndex = 3;
            this.LBL_INLET_getVoltage.Text = "?";
            // 
            // _BTN_HEATER_getVoltage
            // 
            this._BTN_HEATER_getVoltage.Location = new System.Drawing.Point(143, 66);
            this._BTN_HEATER_getVoltage.Name = "_BTN_HEATER_getVoltage";
            this._BTN_HEATER_getVoltage.Size = new System.Drawing.Size(33, 23);
            this._BTN_HEATER_getVoltage.TabIndex = 0;
            this._BTN_HEATER_getVoltage.Text = "->";
            this._BTN_HEATER_getVoltage.UseVisualStyleBackColor = true;
            this._BTN_HEATER_getVoltage.Click += new System.EventHandler(this._BTN_HEATER_getVoltage_Click);
            // 
            // TXB_INLET_setVoltage
            // 
            this.TXB_INLET_setVoltage.Location = new System.Drawing.Point(93, 39);
            this.TXB_INLET_setVoltage.Name = "TXB_INLET_setVoltage";
            this.TXB_INLET_setVoltage.Size = new System.Drawing.Size(44, 20);
            this.TXB_INLET_setVoltage.TabIndex = 2;
            this.TXB_INLET_setVoltage.Text = "1000";
            // 
            // BTN_INLET_getVoltage
            // 
            this.BTN_INLET_getVoltage.Location = new System.Drawing.Point(143, 36);
            this.BTN_INLET_getVoltage.Name = "BTN_INLET_getVoltage";
            this.BTN_INLET_getVoltage.Size = new System.Drawing.Size(33, 23);
            this.BTN_INLET_getVoltage.TabIndex = 0;
            this.BTN_INLET_getVoltage.Text = "->";
            this.BTN_INLET_getVoltage.UseVisualStyleBackColor = true;
            this.BTN_INLET_getVoltage.Click += new System.EventHandler(this.BTN_INLET_getVoltage_Click);
            // 
            // TABpanel_5
            // 
            this.TABpanel_5.BackColor = System.Drawing.SystemColors.Control;
            this.TABpanel_5.Controls.Add(this.GPB_realCOX);
            this.TABpanel_5.Location = new System.Drawing.Point(4, 22);
            this.TABpanel_5.Name = "TABpanel_5";
            this.TABpanel_5.Size = new System.Drawing.Size(652, 320);
            this.TABpanel_5.TabIndex = 4;
            this.TABpanel_5.Text = "Real: Countrers";
            // 
            // GPB_realCOX
            // 
            this.GPB_realCOX.Controls.Add(this.LBL_realCOX_RTC_OverTime);
            this.GPB_realCOX.Controls.Add(this.LBL_realCOX_COC_Result);
            this.GPB_realCOX.Controls.Add(this.LBL_realCOX_COB_Result);
            this.GPB_realCOX.Controls.Add(this.LBL_realCOX_RTCstate);
            this.GPB_realCOX.Controls.Add(this.LBL_realCOX_COA_Result);
            this.GPB_realCOX.Controls.Add(this.label21);
            this.GPB_realCOX.Controls.Add(this.label17);
            this.GPB_realCOX.Controls.Add(this.label22);
            this.GPB_realCOX.Controls.Add(this.labelN);
            this.GPB_realCOX.Controls.Add(this.label30);
            this.GPB_realCOX.Controls.Add(this.label16);
            this.GPB_realCOX.Controls.Add(this.label24);
            this.GPB_realCOX.Controls.Add(this.label23);
            this.GPB_realCOX.Controls.Add(this.progressBar1);
            this.GPB_realCOX.Controls.Add(this.LBL_realCOX_Tiks);
            this.GPB_realCOX.Controls.Add(this.LBL_realCOX_Devider);
            this.GPB_realCOX.Controls.Add(this.LBL_realCOX_frequency);
            this.GPB_realCOX.Controls.Add(this.label27);
            this.GPB_realCOX.Controls.Add(this.label28);
            this.GPB_realCOX.Controls.Add(this.label29);
            this.GPB_realCOX.Controls.Add(this.BTN_realCOX_setParameters);
            this.GPB_realCOX.Controls.Add(this.TXB_realCOX_NumberOfMeasurments);
            this.GPB_realCOX.Controls.Add(this.TXB_realCOX_MeasureTime);
            this.GPB_realCOX.Controls.Add(this.BTN_realCOX_start);
            this.GPB_realCOX.Controls.Add(this.BTN_realCOX_check);
            this.GPB_realCOX.Controls.Add(this.BTN_realCOX_stop);
            this.GPB_realCOX.Location = new System.Drawing.Point(3, 3);
            this.GPB_realCOX.Name = "GPB_realCOX";
            this.GPB_realCOX.Size = new System.Drawing.Size(646, 314);
            this.GPB_realCOX.TabIndex = 13;
            this.GPB_realCOX.TabStop = false;
            this.GPB_realCOX.Text = "Счётчики";
            // 
            // LBL_realCOX_RTC_OverTime
            // 
            this.LBL_realCOX_RTC_OverTime.AutoSize = true;
            this.LBL_realCOX_RTC_OverTime.Location = new System.Drawing.Point(79, 234);
            this.LBL_realCOX_RTC_OverTime.Name = "LBL_realCOX_RTC_OverTime";
            this.LBL_realCOX_RTC_OverTime.Size = new System.Drawing.Size(13, 13);
            this.LBL_realCOX_RTC_OverTime.TabIndex = 16;
            this.LBL_realCOX_RTC_OverTime.Text = "0";
            // 
            // LBL_realCOX_COC_Result
            // 
            this.LBL_realCOX_COC_Result.AutoSize = true;
            this.LBL_realCOX_COC_Result.Location = new System.Drawing.Point(44, 214);
            this.LBL_realCOX_COC_Result.Name = "LBL_realCOX_COC_Result";
            this.LBL_realCOX_COC_Result.Size = new System.Drawing.Size(13, 13);
            this.LBL_realCOX_COC_Result.TabIndex = 16;
            this.LBL_realCOX_COC_Result.Text = "0";
            // 
            // LBL_realCOX_COB_Result
            // 
            this.LBL_realCOX_COB_Result.AutoSize = true;
            this.LBL_realCOX_COB_Result.Location = new System.Drawing.Point(44, 194);
            this.LBL_realCOX_COB_Result.Name = "LBL_realCOX_COB_Result";
            this.LBL_realCOX_COB_Result.Size = new System.Drawing.Size(13, 13);
            this.LBL_realCOX_COB_Result.TabIndex = 17;
            this.LBL_realCOX_COB_Result.Text = "0";
            // 
            // LBL_realCOX_RTCstate
            // 
            this.LBL_realCOX_RTCstate.AutoSize = true;
            this.LBL_realCOX_RTCstate.Location = new System.Drawing.Point(44, 154);
            this.LBL_realCOX_RTCstate.Name = "LBL_realCOX_RTCstate";
            this.LBL_realCOX_RTCstate.Size = new System.Drawing.Size(68, 13);
            this.LBL_realCOX_RTCstate.TabIndex = 18;
            this.LBL_realCOX_RTCstate.Text = "Неизвестно";
            // 
            // LBL_realCOX_COA_Result
            // 
            this.LBL_realCOX_COA_Result.AutoSize = true;
            this.LBL_realCOX_COA_Result.Location = new System.Drawing.Point(44, 174);
            this.LBL_realCOX_COA_Result.Name = "LBL_realCOX_COA_Result";
            this.LBL_realCOX_COA_Result.Size = new System.Drawing.Size(13, 13);
            this.LBL_realCOX_COA_Result.TabIndex = 18;
            this.LBL_realCOX_COA_Result.Text = "0";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(6, 194);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(32, 13);
            this.label21.TabIndex = 14;
            this.label21.Text = "СОВ:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 234);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(52, 13);
            this.label17.TabIndex = 13;
            this.label17.Text = "Overtime:";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(6, 214);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(32, 13);
            this.label22.TabIndex = 13;
            this.label22.Text = "СОС:";
            // 
            // labelN
            // 
            this.labelN.AutoSize = true;
            this.labelN.Location = new System.Drawing.Point(6, 154);
            this.labelN.Name = "labelN";
            this.labelN.Size = new System.Drawing.Size(32, 13);
            this.labelN.TabIndex = 15;
            this.labelN.Text = "RTC:";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(6, 174);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(32, 13);
            this.label30.TabIndex = 15;
            this.label30.Text = "СОА:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(306, 24);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(21, 13);
            this.label16.TabIndex = 12;
            this.label16.Text = "мс";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(344, 24);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(128, 13);
            this.label24.TabIndex = 11;
            this.label24.Text = "Количество измерений:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(125, 24);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(102, 13);
            this.label23.TabIndex = 11;
            this.label23.Text = "Время измерения:";
            // 
            // progressBar1
            // 
            this.progressBar1.BackColor = System.Drawing.SystemColors.Control;
            this.progressBar1.Enabled = false;
            this.progressBar1.ForeColor = System.Drawing.SystemColors.Desktop;
            this.progressBar1.Location = new System.Drawing.Point(6, 107);
            this.progressBar1.Maximum = 2000;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(321, 15);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 10;
            // 
            // LBL_realCOX_Tiks
            // 
            this.LBL_realCOX_Tiks.AutoSize = true;
            this.LBL_realCOX_Tiks.Location = new System.Drawing.Point(232, 88);
            this.LBL_realCOX_Tiks.Name = "LBL_realCOX_Tiks";
            this.LBL_realCOX_Tiks.Size = new System.Drawing.Size(13, 13);
            this.LBL_realCOX_Tiks.TabIndex = 9;
            this.LBL_realCOX_Tiks.Text = "0";
            // 
            // LBL_realCOX_Devider
            // 
            this.LBL_realCOX_Devider.AutoSize = true;
            this.LBL_realCOX_Devider.Location = new System.Drawing.Point(232, 68);
            this.LBL_realCOX_Devider.Name = "LBL_realCOX_Devider";
            this.LBL_realCOX_Devider.Size = new System.Drawing.Size(13, 13);
            this.LBL_realCOX_Devider.TabIndex = 9;
            this.LBL_realCOX_Devider.Text = "0";
            // 
            // LBL_realCOX_frequency
            // 
            this.LBL_realCOX_frequency.AutoSize = true;
            this.LBL_realCOX_frequency.Location = new System.Drawing.Point(232, 48);
            this.LBL_realCOX_frequency.Name = "LBL_realCOX_frequency";
            this.LBL_realCOX_frequency.Size = new System.Drawing.Size(13, 13);
            this.LBL_realCOX_frequency.TabIndex = 9;
            this.LBL_realCOX_frequency.Text = "0";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(128, 68);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(60, 13);
            this.label27.TabIndex = 8;
            this.label27.Text = "Делитель:";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(128, 88);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(101, 13);
            this.label28.TabIndex = 8;
            this.label28.Text = "Количество тиков:";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(128, 48);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(52, 13);
            this.label29.TabIndex = 8;
            this.label29.Text = "Частота:";
            // 
            // BTN_realCOX_setParameters
            // 
            this.BTN_realCOX_setParameters.Enabled = false;
            this.BTN_realCOX_setParameters.Location = new System.Drawing.Point(6, 19);
            this.BTN_realCOX_setParameters.Name = "BTN_realCOX_setParameters";
            this.BTN_realCOX_setParameters.Size = new System.Drawing.Size(113, 23);
            this.BTN_realCOX_setParameters.TabIndex = 7;
            this.BTN_realCOX_setParameters.Text = "Задать время";
            this.BTN_realCOX_setParameters.UseVisualStyleBackColor = true;
            this.BTN_realCOX_setParameters.Click += new System.EventHandler(this.BTN_realCOX_setParameters_Click);
            // 
            // TXB_realCOX_NumberOfMeasurments
            // 
            this.TXB_realCOX_NumberOfMeasurments.Location = new System.Drawing.Point(478, 21);
            this.TXB_realCOX_NumberOfMeasurments.Name = "TXB_realCOX_NumberOfMeasurments";
            this.TXB_realCOX_NumberOfMeasurments.Size = new System.Drawing.Size(63, 20);
            this.TXB_realCOX_NumberOfMeasurments.TabIndex = 2;
            this.TXB_realCOX_NumberOfMeasurments.Text = "10";
            // 
            // TXB_realCOX_MeasureTime
            // 
            this.TXB_realCOX_MeasureTime.Location = new System.Drawing.Point(237, 22);
            this.TXB_realCOX_MeasureTime.Name = "TXB_realCOX_MeasureTime";
            this.TXB_realCOX_MeasureTime.Size = new System.Drawing.Size(63, 20);
            this.TXB_realCOX_MeasureTime.TabIndex = 2;
            this.TXB_realCOX_MeasureTime.Text = "1000";
            this.TXB_realCOX_MeasureTime.TextChanged += new System.EventHandler(this.TXB_realCOX_MeasureTime_TextChanged);
            // 
            // BTN_realCOX_start
            // 
            this.BTN_realCOX_start.Location = new System.Drawing.Point(6, 48);
            this.BTN_realCOX_start.Name = "BTN_realCOX_start";
            this.BTN_realCOX_start.Size = new System.Drawing.Size(113, 23);
            this.BTN_realCOX_start.TabIndex = 7;
            this.BTN_realCOX_start.Text = "Начать счёт";
            this.BTN_realCOX_start.UseVisualStyleBackColor = true;
            this.BTN_realCOX_start.Click += new System.EventHandler(this.BTN_realCOX_start_Click);
            // 
            // BTN_realCOX_check
            // 
            this.BTN_realCOX_check.Location = new System.Drawing.Point(6, 128);
            this.BTN_realCOX_check.Name = "BTN_realCOX_check";
            this.BTN_realCOX_check.Size = new System.Drawing.Size(113, 23);
            this.BTN_realCOX_check.TabIndex = 7;
            this.BTN_realCOX_check.Text = "Проверить счёт";
            this.BTN_realCOX_check.UseVisualStyleBackColor = true;
            this.BTN_realCOX_check.Click += new System.EventHandler(this.BTN_realCOX_check_Click);
            // 
            // BTN_realCOX_stop
            // 
            this.BTN_realCOX_stop.Location = new System.Drawing.Point(6, 78);
            this.BTN_realCOX_stop.Name = "BTN_realCOX_stop";
            this.BTN_realCOX_stop.Size = new System.Drawing.Size(113, 23);
            this.BTN_realCOX_stop.TabIndex = 7;
            this.BTN_realCOX_stop.Text = "Остановить счёт";
            this.BTN_realCOX_stop.UseVisualStyleBackColor = true;
            this.BTN_realCOX_stop.Click += new System.EventHandler(this.BTN_realCOX_stop_Click);
            // 
            // CLK_timer
            // 
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
            // CLK_COA
            // 
            this.CLK_COA.Interval = 10;
            this.CLK_COA.Tick += new System.EventHandler(this.CLK_COA_Tick);
            // 
            // TIM_TIC_HVE
            // 
            this.TIM_TIC_HVE.Interval = 200;
            this.TIM_TIC_HVE.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 654);
            this.Controls.Add(this.BTN_openLog);
            this.Controls.Add(this.CHB_traceLog);
            this.Controls.Add(this.CHB_enableSuperTracer);
            this.Controls.Add(this.TABpanel);
            this.Controls.Add(this.Log);
            this.MaximumSize = new System.Drawing.Size(700, 900);
            this.MinimumSize = new System.Drawing.Size(700, 600);
            this.Name = "Form1";
            this.Text = "XMega32A4U_testBoard v0.112";
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
            this.TABpanel_3.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.GRB_TIC_DisplayContrast.ResumeLayout(false);
            this.GRB_TIC_DisplayContrast.PerformLayout();
            this.TABpanel_4.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.GPB_Flags.ResumeLayout(false);
            this.GPB_Flags.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.GPB_CONDER.ResumeLayout(false);
            this.GPB_CONDER.PerformLayout();
            this.GPB_Heater.ResumeLayout(false);
            this.GPB_Heater.PerformLayout();
            this.TABpanel_5.ResumeLayout(false);
            this.GPB_realCOX.ResumeLayout(false);
            this.GPB_realCOX.PerformLayout();
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
        private System.Windows.Forms.Label LBL_COA_status;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox CHB_Control_COA;
        private System.Windows.Forms.Timer CLK_COA;
        private System.Windows.Forms.Button BTN_sendSomething;
        private System.Windows.Forms.TabPage TABpanel_3;
        private System.Windows.Forms.Button BTN_TIC_DisplayContrast_set;
        private System.Windows.Forms.Button BTN_traceErrorList;
        private System.Windows.Forms.TabPage TABpanel_4;
        private System.Windows.Forms.Button BTN_INLET_setVoltage;
        private System.Windows.Forms.Label LBL_INLET_getVoltage;
        private System.Windows.Forms.TextBox TXB_INLET_setVoltage;
        private System.Windows.Forms.Button BTN_INLET_getVoltage;
        private System.Windows.Forms.GroupBox GPB_Heater;
        private System.Windows.Forms.Button BTN_HEATER_setVoltage;
        private System.Windows.Forms.Label LBL_HEATER_getVoltage;
        private System.Windows.Forms.TextBox TXB_HEATER_setVoltage;
        private System.Windows.Forms.Button _BTN_HEATER_getVoltage;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox GPB_CONDER;
        private System.Windows.Forms.TextBox TXB_IonSOURCE_setEmissionCurrentVoltage;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button BTN_IonSOURCE_setF2voltage;
        private System.Windows.Forms.Button BTN_IonSOURCE_setF1voltage;
        private System.Windows.Forms.Button BTN_IonSOURCE_setIonizationVoltage;
        private System.Windows.Forms.Button BTN_IonSOURCE_setEmissionCurrentVoltage;
        private System.Windows.Forms.Label LBL_IonSOURCE_getF2voltage;
        private System.Windows.Forms.Label LBL_IonSOURCE_getF1voltage;
        private System.Windows.Forms.Label LBL_IonSOURCE_getIonizationVoltage;
        private System.Windows.Forms.Label LBL_IonSOURCE_getEmissionCurrentVoltage;
        private System.Windows.Forms.TextBox TXB_IonSOURCE_setF2voltage;
        private System.Windows.Forms.TextBox TXB_IonSOURCE_setF1voltage;
        private System.Windows.Forms.TextBox TXB_IonSOURCE_setIonizationVoltage;
        private System.Windows.Forms.Button BTN_IonSOURCE_getF2voltage;
        private System.Windows.Forms.Button BTN_IonSOURCE_getF1voltage;
        private System.Windows.Forms.Button BTN_IonSOURCE_getIonizationVoltage;
        private System.Windows.Forms.Button BTN_IonSOURCE_getEmissionCurrentVoltage;
        private System.Windows.Forms.Button BTN_DETECTOR_getDV3voltage;
        private System.Windows.Forms.Button BTN_DETECTOR_getDV2voltage;
        private System.Windows.Forms.Button BTN_DETECTOR_getDV1voltage;
        private System.Windows.Forms.TextBox TXB_DETECTOR_setDV3voltage;
        private System.Windows.Forms.TextBox TXB_DETECTOR_setDV2voltage;
        private System.Windows.Forms.Label LBL_DETECTOR_getDV3voltage;
        private System.Windows.Forms.Label LBL_DETECTOR_getDV2voltage;
        private System.Windows.Forms.Label LBL_DETECTOR_getDV1voltage;
        private System.Windows.Forms.TextBox TXB_DETECTOR_setDV1voltage;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button BTN_DETECTOR_setDV3voltage;
        private System.Windows.Forms.Button BTN_DETECTOR_setDV2voltage;
        private System.Windows.Forms.Button BTN_DETECTOR_setDV1voltage;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button BTN_checkCommandStack;
        private System.Windows.Forms.Button BTN_SCANER_setParentScanVoltage;
        private System.Windows.Forms.Button BTN_SCANER_setScanVoltage;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button BTN_SCANER_getScanVoltage;
        private System.Windows.Forms.Button BTN_SCANER_getParentScanVoltage;
        private System.Windows.Forms.Label LBL_SCANER_getScanVoltage;
        private System.Windows.Forms.TextBox TXB_SCANER_setScanVoltage;
        private System.Windows.Forms.TextBox TXB_SCANER_setParentScanVoltage;
        private System.Windows.Forms.Label LBL_SCANER_getParentScanVoltage;
        private System.Windows.Forms.Label LBL_CONDENSATOR_getPositiveVoltage;
        private System.Windows.Forms.Label LBL_CONDENSATOR_getNegativeVoltage;
        private System.Windows.Forms.Button BTN_CONDENSATOR_getNegativeVoltage;
        private System.Windows.Forms.Button BTN_CONDENSATOR_getPositiveVoltage;
        private System.Windows.Forms.Button BTN_CONDENSATOR_setVoltage;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox TXB_CONDENSATOR_setVoltage;
        private System.Windows.Forms.GroupBox GPB_Flags;
        private System.Windows.Forms.TabPage TABpanel_5;
        private System.Windows.Forms.GroupBox GPB_realCOX;
        private System.Windows.Forms.Label LBL_realCOX_COC_Result;
        private System.Windows.Forms.Label LBL_realCOX_COB_Result;
        private System.Windows.Forms.Label LBL_realCOX_COA_Result;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label LBL_realCOX_Tiks;
        private System.Windows.Forms.Label LBL_realCOX_Devider;
        private System.Windows.Forms.Label LBL_realCOX_frequency;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Button BTN_realCOX_setParameters;
        private System.Windows.Forms.TextBox TXB_realCOX_MeasureTime;
        private System.Windows.Forms.Button BTN_realCOX_start;
        private System.Windows.Forms.Button BTN_realCOX_check;
        private System.Windows.Forms.Button BTN_realCOX_stop;
        private System.Windows.Forms.Label LBL_realCOX_RTCstate;
        private System.Windows.Forms.Label labelN;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button BTN_checkFlags;
        private System.Windows.Forms.CheckBox CHB_iEDCD;
        private System.Windows.Forms.CheckBox CHB_SEMV3;
        private System.Windows.Forms.CheckBox CHB_PRGE;
        private System.Windows.Forms.CheckBox CHB_SEMV2;
        private System.Windows.Forms.CheckBox CHB_SPUMP;
        private System.Windows.Forms.CheckBox CHB_SEMV1;
        private System.Windows.Forms.Button BTN_DAC_reset;
        private System.Windows.Forms.Timer TIM_TIC_HVE;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox TXB_realCOX_NumberOfMeasurments;
        private System.Windows.Forms.CheckBox CHB_iHVE;
        private System.Windows.Forms.Label LBL_realCOX_RTC_OverTime;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.GroupBox GRB_TIC_DisplayContrast;
        private System.Windows.Forms.Button BTN_TIC_DisplayContrast_get;
        private System.Windows.Forms.TextBox TXB_TIC_DisplayContrast;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox CHB_TIC_HVE;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox TXB_TIC_HVE_Period;
        private System.Windows.Forms.Label LBL_TIC_HVE_ElapsedTime;
        private System.Windows.Forms.Label LBL_TIC_HVE_Errors;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button BTN_TIC_HVEconf_check;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
    }
}

