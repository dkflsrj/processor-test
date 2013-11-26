using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;

namespace Xmega32A4U_testBoard
{
    public partial class Form1 : Form
    {
        SerialPort COM_Port;
        XMEGA32A4U MC;
        decimal UI_PGB_COA_step = 0;
        decimal UI_PGB_COA_count = 0;
        const int CLK_COA_intreval = 10;
        bool MeasureDone = false;
        public Form1()
        {
            InitializeComponent();

            CMB_COM_BaudRate.Enabled = false;
            CMB_COM_DataBits.Enabled = false;
            CMB_COM_Handshake.Enabled = false;
            CMB_COM_Parity.Enabled = false;
            CMB_COM_StopBits.Enabled = false;
            BTN_COM_setParams.Enabled = false;

            MC = new XMEGA32A4U();

            trace(false, "Программа инициирована!");

            if (findCOM())
            {
                setCOMparams();
            
                TABpanel.SelectedIndex = 1;

                MC.Chip.setUSART(COM_Port);
            }
            MC.Tester.setTracer(Log);

            CLK_COA.Interval = CLK_COA_intreval;
            CLK_timer.Enabled = false;
        }
        //Функции интерфейса
        void trace(bool newLine, string text)
        {
            if (newLine)
            {
                Log.AppendText(Environment.NewLine + "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + text);
                //Log.AppendText(Environment.NewLine + text);

            }
            else
            {
                Log.AppendText(text);
            }
            Log.ScrollToCaret();
        }
        //Функции COM порта
        private void BTN_COM_find_Click(object sender, EventArgs e)
        {
            findCOM();
        }
        private void BTN_COM_setParams_Click(object sender, EventArgs e)
        {
            setCOMparams();
        }
        void setCOMparams()
        {
            Parity l_parity;
            switch (CMB_COM_Parity.Text)
            {
                case "Чёт":
                    l_parity = Parity.Even;
                    break;
                case "Нечёт":
                    l_parity = Parity.Odd;
                    break;
                case "Маркер":
                    l_parity = Parity.Mark;
                    break;
                case "Пробел":
                    l_parity = Parity.Space;
                    break;
                default:
                    //"Нет"
                    l_parity = Parity.None;
                    break;
            }
            StopBits l_stopBits;
            switch (CMB_COM_StopBits.Text)
            {
                case "1":
                    l_stopBits = StopBits.One;
                    break;
                case "1.5":
                    l_stopBits = StopBits.OnePointFive;
                    break;
                case "2":
                    l_stopBits = StopBits.Two;
                    break;
                default:
                    //"Нет"
                    l_stopBits = StopBits.None;
                    break;
            }
            /*Handshake l_handshake;
            switch (cBox_COM_Handshake.Text)
            {
                case "Xon / Xoff":
                    l_handshake = Handshake.XOnXOff;
                    break;
                case "Request Xon / Xoff":
                    l_handshake = Handshake.RequestToSendXOnXOff;
                    break;
                case "Аппаратное":
                    l_handshake = Handshake.RequestToSend;
                    break;
                default:
                    //"Нет"
                    l_handshake = Handshake.None;
                    break;
            }*/
            COM_Port = new SerialPort(cBox_COM.Text, Convert.ToInt32(CMB_COM_BaudRate.Text), l_parity, Convert.ToInt32(CMB_COM_DataBits.Text), l_stopBits);
            //COM_Port.Handshake = l_handshake;
            COM_Port.ReadTimeout = 2000;
            COM_Port.WriteTimeout = 2000;
            trace(true, "Установка параметров COM порта: " + COM_Port.PortName);
            trace(true, "   Бит в секунду: " + COM_Port.BaudRate.ToString());
            trace(true, "   Чётность: " + COM_Port.Parity.ToString());
            trace(true, "   Биты данных: " + COM_Port.DataBits.ToString());
            trace(true, "   Стоповые биты: " + COM_Port.StopBits.ToString());
        }
        bool findCOM()
        {
            trace(true, "Поиск COM портов...");
            string[] Ports = SerialPort.GetPortNames();
            if (Ports.Length > 0)
            {
                trace(true, "   Список обнаруженных COM портов:");
                for (int i = 0; i < Ports.Length; i++)
                {
                    cBox_COM.Items.Add(Ports[i]);
                    trace(true, "       - " + Ports[i]);
                }
                cBox_COM.Text = Ports[0];
                //Включаем настройки
                CMB_COM_BaudRate.Enabled = true;
                CMB_COM_DataBits.Enabled = true;
                CMB_COM_Handshake.Enabled = true;
                CMB_COM_Parity.Enabled = true;
                CMB_COM_StopBits.Enabled = true;
                BTN_COM_setParams.Enabled = true;
                return true;
            }
            else
            {
                trace(true, "ОШИБКА: Ни один COM порт не найден!");
                return false;
            }

        }
        //Функции МК
        private void BTN_MCstatus_Click(object sender, EventArgs e)
        {
            //trace(true, "Статус: " + 
            MC.Chip.getStatus().ToString();
        }
        private void BTN_COM_getMCversion_Click(object sender, EventArgs e)
        {
            //trace(true, "Запрос информации о прошивке у МК...");
            //trace(true, "   Дата создания: " + 
            MC.Chip.getBirthday();
            //trace(true, "   Версия: " + 
            MC.Chip.getVersion().ToString();
        }
        private void BTN_COM_getCPUfreq_Click(object sender, EventArgs e)
        {
            //trace(true, "Частота CPU: " + 
            MC.Chip.getCPUfrequency();
        }
        private void BTN_COM_setMCwait_Click(object sender, EventArgs e)
        {
            //MC.setMCwait();
        }
        private void BTN_LEDbyte_Click(object sender, EventArgs e)
        {
            //MC.Tester.showMeByte(TXB_LEDbyte.Text);
        }
        private void BTN_SPI_DAC_send_Click(object sender, EventArgs e)
        {
            //if (MC.DAC.setVoltage(TXB_DAC_channel.Text, TXB_DAC_voltage.Text))
            //{
            //    //trace(true, "На канал " + TXB_DAC_channel.Text + " выставлено напряжение: " + TXB_DAC_voltage.Text);
            //}
            //else
            //{
            //    //trace(true, "ОШИБКА ОТКЛИКА! DAC возможно не выставил напряжение!");
            //}
           // MC.IonSource.F8.setVoltage(TXB_DAC_voltage.Text);
        }
        private void BTN_SPI_ADC_request_Click(object sender, EventArgs e)
        {
           //MC.ADC.DoubleRange = CHB_ADC_DoubleRange.Checked;
           ////trace(true, "Напряжение на канале " + TXB_ADC_channel.Text + " : " + 
           //MC.ADC.getVoltage(TXB_ADC_channel.Text);

        }
        private void BTN_DAC_reset_Click(object sender, EventArgs e)
        {
            //if (MC.DAC.reset())
            //{
            //    //trace(true, "DAC сброшен!");
            //}
            //else
            //{
            //    //trace(true, "ОШИБКА ОТКЛИКА! DAC возможно НЕ сброшен!");
            //}
        }
        private void BTN_startCounter_Click(object sender, EventArgs e)
        {
            //ФУНКЦИЯ: Запускаем счётчик МК
            try
            {
                Convert.ToInt32(TXB_COA_measureTime.Text);
            }
            catch (Exception)
            {
                //trace(true, "ОШИБКА! Неверный интервал!");
                return;
            }
            //if (
                //MC.Counters.startSeries();
                //)
           //{
           //    //trace(true, "COA начал счёт...");
           //    PGB_COA_progress.Value = 0;
           //    UI_PGB_COA_count = 0;
           //    UI_PGB_COA_step = (3000 * (decimal)CLK_COA_intreval) / Convert.ToInt32(TXB_COA_measureTime.Text);
           //    CLK_COA.Enabled = true;
           //    return;
           //}
            //trace(true, "Ошибка! Возможно COA не начал счёт!");

        }
        private void BTN_reqCount_Click(object sender, EventArgs e)
        {
            //ФУНКЦИЯ: Проверям счётчик МК, если сосчитал, то принимаем результат
            //string Counters_status = MC.Counters.receiveResults();
            ////trace(true, "Состояние счётчиков: " + Counters_status);
            //if (Counters_status == "Ready")
            //{
            //    //trace(true, "   Счёт: " + MC.Counters.COA.Result);
            //}
        }
        private void BTN_setInterval_Click(object sender, EventArgs e)
        {
            //ФУНКЦИЯ: Задаёт временной интервал счёта в миллисекундах 
            //try
            //{
            //    Convert.ToInt32(TXB_COA_measureTime.Text);
            //}
            //catch (Exception)
            //{
            //    //trace(true, "ОШИБКА! Неверное время измерения!");
            //    return;
            //}
            //try
            //{
            //    Convert.ToByte(TXB_COA_delay.Text);
            //}
            //catch (Exception)
            //{
            //    //trace(true, "ОШИБКА! Неверное время паузы!");
            //    return;
            //}
            //try
            //{
            //    Convert.ToUInt16(TXB_COA_quantity.Text);
            //}
            //catch (Exception)
            //{
            //    //trace(true, "ОШИБКА! Неверное количество измерений!");
            //    return;
            //}
            //if (MC.Counters.setMeasureTime(TXB_COA_measureTime.Text))
            //{
            //    //trace(true, "Задан временной интервал счёта: " + TXB_COA_measureTime.Text + "мс (" + MC.Counters.getRTCticks(TXB_COA_measureTime.Text, MC.Counters.getRTCprescaler(TXB_COA_measureTime.Text)) + " тиков)");
            //}
            //else
            //{
            //    //trace(true, "Счётчик ещё считает!");
            //}
            /*if (MC.COA.setMeasureDelay(TXB_COA_delay.Text))
            {
                trace(true, "Задана пауза между измерениями: " + TXB_COA_delay.Text + "мс (" + MC.RTC.get_Ticks(TXB_COA_delay.Text,1) + " тиков)");
            }
            if(MC.COA.setMeasureQuantity(TXB_COA_quantity.Text))
            {
                trace(true, "Задано количество измерений: " + TXB_COA_quantity.Text);
            }*/
        }
        private void BTN_stopCounter_Click(object sender, EventArgs e)
        {
            //if (MC.Counters.stopMeasure())
            //{
            //    //trace(true, "Счётчик был успешно остановлен!");
            //    CLK_COA.Enabled = false;
            //    PGB_COA_progress.Value = PGB_COA_progress.Minimum;
            //    return;
            //}
            //trace(true, "ОШИБКА ОТКЛИКА! Возможно Счётчик не был остановлен!");
        }
        private void CLK_timer_Tick(object sender, EventArgs e)
        {
            if (CHB_TotalControl.Checked)
            {
                switch (MC.Chip.getStatus())
                {
                    case 0: LBL_TotalC_Status.Text = "Неинициализирован!";
                        LBL_TotalC_Status.ForeColor = System.Drawing.Color.Red;
                        break;
                    case 1: LBL_TotalC_Status.Text = "Ожидание";
                        LBL_TotalC_Status.ForeColor = System.Drawing.Color.Green;
                        break;
                    case 2: LBL_TotalC_Status.Text = "Состояние 2";
                        LBL_TotalC_Status.ForeColor = System.Drawing.Color.Yellow;
                        break;
                    case 3: LBL_TotalC_Status.Text = "Состояние 3";
                        LBL_TotalC_Status.ForeColor = System.Drawing.Color.Yellow;
                        break;
                    case 4: LBL_TotalC_Status.Text = "Отображение байтов";
                        LBL_TotalC_Status.ForeColor = System.Drawing.Color.Green;
                        break;
                    default: LBL_TotalC_Status.Text = "Неизвестно!";
                        LBL_TotalC_Status.ForeColor = System.Drawing.Color.Red;
                        break;
                }
                //if (MC.checkErrors())
                //{
                //    LBL_error.Text = "Есть ошибки!";
                //    LBL_TotalC_Status.ForeColor = System.Drawing.Color.Red;
                //}
                //else
                //{

                //    LBL_error.Text = "Ошибок нет";
                //    LBL_error.ForeColor = System.Drawing.Color.Green;
                //}
            }
        }
        private void CHB_TotalControl_CheckedChanged(object sender, EventArgs e)
        {
            if (CHB_TotalControl.Checked)
            {
                CHB_TotalControl.ForeColor = System.Drawing.Color.Green;
                CHB_TotalControl.Text = "Включен";
                CLK_timer.Enabled = true;
            }
            else
            {
                CLK_timer.Enabled = false;
                CHB_TotalControl.ForeColor = System.Drawing.Color.Red;
                CHB_TotalControl.Text = "Выключен";
                LBL_TotalC_Status.Text = "Неизвестно!";
                LBL_TotalC_Status.ForeColor = System.Drawing.Color.Red;
                LBL_error.Text = "Неизвестно!";
                LBL_error.ForeColor = System.Drawing.Color.Red;
            }
        }
        private void TXB_interval_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Convert.ToInt32(TXB_COA_measureTime.Text);
                Convert.ToByte(TXB_COA_delay.Text);
                Convert.ToUInt16(TXB_COA_quantity.Text);
            }
            catch (Exception)
            {
                return;
            }
            LBL_COA_ticks.Text = "";
            //LBL_COA_ticks.Text += "(";
            LBL_COA_ticks.Text += MC.Counters.getRTCticks(TXB_COA_measureTime.Text).ToString();
            //LBL_COA_ticks.Text += " + " + MC.RTC.get_Ticks(TXB_COA_delay.Text, 1).ToString() + ")";
            //LBL_COA_ticks.Text += "*" + TXB_COA_quantity.Text;
            LBL_COA_frequency.Text = MC.Counters.getRTCfrequency(TXB_COA_measureTime.Text).ToString();
            LBL_COA_prescaler.Text = MC.Counters.getRTCprescaler_long(TXB_COA_measureTime.Text).ToString();
        }
        private void CHB_enableSuperTracer_CheckedChanged(object sender, EventArgs e)
        {
            if (CHB_enableSuperTracer.Checked)
            {
                MC.Tester.enableTracerInTransmit(true);
            }
            else
            {
                MC.Tester.enableTracerInTransmit(false);
            }
        }
        private void CLK_COA_Tick(object sender, EventArgs e)
        {
            UI_PGB_COA_count += UI_PGB_COA_step;
            if (Convert.ToInt16(Math.Round(UI_PGB_COA_count)) >= PGB_COA_progress.Maximum)
            {
                PGB_COA_progress.Value = PGB_COA_progress.Maximum;
                CLK_COA.Enabled = false;
            }
            else
            {
                PGB_COA_progress.Value = Convert.ToInt16(Math.Round(UI_PGB_COA_count));
            }

        }
        private void BTN_MC_Reset_Click(object sender, EventArgs e)
        {
            /*CHB_TotalControl.Checked = false;
            if (MC.reset())
            {
                trace(true, "Производится перезагрузка микроконтроллера...");
                return;
            }
            trace(true, "ОШИБКА ОТКЛИКА! Вероятно перезагрузка не была выполнена!");*/
        }
        private void BTN_openLog_Click(object sender, EventArgs e)
        {
            Process.Start("Log.txt");
        }
        private void CHB_traceLog_CheckedChanged(object sender, EventArgs e)
        {
            if (CHB_traceLog.Checked)
            {
                MC.Tester.enableLog(true);
            }
            else
            {
                MC.Tester.enableLog(false);
            }
        }
        private void WATCHER_Counters_Tick(object sender, EventArgs e)
        {

            //switch (MC.Counters.getStatus())
            //{
            //    case "Ready": LBL_COA_status.Text = "Готов";
            //        LBL_COA_status.ForeColor = System.Drawing.Color.Green;
            //        break;
            //    case "Stopped": LBL_COA_status.Text = "Остановлен";
            //        LBL_COA_status.ForeColor = System.Drawing.Color.Orange;
            //        break;
            //    case "Busy": LBL_COA_status.Text = "Считает";
            //        LBL_COA_status.ForeColor = System.Drawing.Color.Orange;
            //        break;
            //    default: LBL_COA_status.Text = "Ошибка состояния!";
            //        LBL_COA_status.ForeColor = System.Drawing.Color.Red;
            //        break;
            //}
        }
        private void BTN_sendSomething_Click(object sender, EventArgs e)
        {
            MC.Tester.sendSomething();
        }
        private void BTN_TIC_Click(object sender, EventArgs e)
        {
            MC.TIC.send();
        }
        private void BTN_traceErrorList_Click(object sender, EventArgs e)
        {
            foreach (string error in MC.getErrorList().ToArray())
            {
                trace(true, error);
            }
        }
        //==================================REAL=================================
        //-------------------------------Натекатель------------------------------
        private void BTN_INLET_setVoltage_Click(object sender, EventArgs e)
        {
            MC.Inlet.setVoltage(TXB_INLET_setVoltage.Text);
        }
        private void BTN_INLET_getVoltage_Click(object sender, EventArgs e)
        {
            LBL_INLET_getVoltage.Text = MC.Inlet.getVoltage().ToString();
        }
        //-------------------------------Нагреватель------------------------------
        private void BTN_HEATER_setVoltage_Click(object sender, EventArgs e)
        {
            MC.Heater.setVoltage(TXB_HEATER_setVoltage.Text);
        }
        private void _BTN_HEATER_getVoltage_Click(object sender, EventArgs e)
        {
            LBL_HEATER_getVoltage.Text = MC.Heater.getVoltage().ToString();
        }
        private void BTN_HEATER_reset_Click(object sender, EventArgs e)
        {
            //MC.Inlet.reset();
        }
        //-------------------------------Ионный источник--------------------------
        private void BTN_IonSOURCE_setEmissionCurrentVoltage_Click(object sender, EventArgs e)
        {
            MC.IonSource.EmissionCurrent.setVoltage(TXB_IonSOURCE_setEmissionCurrentVoltage.Text);
        }
        private void BTN_IonSOURCE_setIonizationVoltage_Click(object sender, EventArgs e)
        {
            MC.IonSource.Ionization.setVoltage(TXB_IonSOURCE_setIonizationVoltage.Text);
        }
        private void BTN_IonSOURCE_setF1voltage_Click(object sender, EventArgs e)
        {
            MC.IonSource.F1.setVoltage(TXB_IonSOURCE_setF1voltage.Text);
        }
        private void BTN_IonSOURCE_setF2voltage_Click(object sender, EventArgs e)
        {
            MC.IonSource.F2.setVoltage(TXB_IonSOURCE_setF2voltage.Text);
        }
        private void BTN_IonSOURCE_getEmissionCurrentVoltage_Click(object sender, EventArgs e)
        {
            LBL_IonSOURCE_getEmissionCurrentVoltage.Text = MC.IonSource.EmissionCurrent.getVoltage().ToString();
        }
        private void BTN_IonSOURCE_getIonizationVoltage_Click(object sender, EventArgs e)
        {
            LBL_IonSOURCE_getIonizationVoltage.Text = MC.IonSource.Ionization.getVoltage().ToString();
        }
        private void BTN_IonSOURCE_getF1voltage_Click(object sender, EventArgs e)
        {
            LBL_IonSOURCE_getF1voltage.Text = MC.IonSource.F1.getVoltage().ToString();
        }
        private void BTN_IonSOURCE_getF2voltage_Click(object sender, EventArgs e)
        {
            LBL_IonSOURCE_getF2voltage.Text = MC.IonSource.F2.getVoltage().ToString();
        }
        //------------------------------------Детектор------------------------------------------
        private void BTN_DETECTOR_setDV1voltage_Click(object sender, EventArgs e)
        {
            MC.Detector.DV1.setVoltage(TXB_DETECTOR_setDV1voltage.Text);
        }
        private void BTN_DETECTOR_setDV2voltage_Click(object sender, EventArgs e)
        {
            MC.Detector.DV2.setVoltage(TXB_DETECTOR_setDV2voltage.Text);
        }
        private void BTN_DETECTOR_setDV3voltage_Click(object sender, EventArgs e)
        {
            MC.Detector.DV3.setVoltage(TXB_DETECTOR_setDV3voltage.Text);
        }
        private void BTN_DETECTOR_getDV1voltage_Click(object sender, EventArgs e)
        {
            LBL_DETECTOR_getDV1voltage.Text = MC.Detector.DV1.getVoltage().ToString();
        }
        private void BTN_DETECTOR_getDV2voltage_Click(object sender, EventArgs e)
        {
            LBL_DETECTOR_getDV2voltage.Text = MC.Detector.DV2.getVoltage().ToString();
        }
        private void BTN_DETECTOR_getDV3voltage_Click(object sender, EventArgs e)
        {
            LBL_DETECTOR_getDV3voltage.Text = MC.Detector.DV3.getVoltage().ToString();
        }
        private void BTN_DETECTOR_reset_Click(object sender, EventArgs e)
        {
            //MC.Detector.reset();
        }
        private void BTN_checkCommandStack_Click(object sender, EventArgs e)
        {
            MC.Chip.checkCommandStack();
        }
        //--------------------------------------Сканер----------------------------------------
        private void BTN_SCANER_setParentScanVoltage_Click(object sender, EventArgs e)
        {
            MC.Scaner.ParentScan.setVoltage(TXB_SCANER_setParentScanVoltage.Text);
        }
        private void BTN_SCANER_setScanVoltage_Click(object sender, EventArgs e)
        {
            MC.Scaner.Scan.setVoltage(TXB_SCANER_setScanVoltage.Text);
        }
        private void BTN_SCANER_getParentScanVoltage_Click(object sender, EventArgs e)
        {
            LBL_SCANER_getParentScanVoltage.Text = MC.Scaner.ParentScan.getVoltage().ToString();

        }
        private void BTN_SCANER_getScanVoltage_Click(object sender, EventArgs e)
        {
            LBL_SCANER_getScanVoltage.Text = MC.Scaner.Scan.getVoltage().ToString();
        }
        //------------------------------------Конденсатор------------------------------------------
        private void BTN_CONDENSATOR_setVoltage_Click(object sender, EventArgs e)
        {
            MC.Condensator.setVoltage(TXB_CONDENSATOR_setVoltage.Text);
        }
        private void BTN_CONDENSATOR_getPositiveVoltage_Click(object sender, EventArgs e)
        {
            LBL_CONDENSATOR_getPositiveVoltage.Text = MC.Condensator.getPositiveVoltage().ToString();
        }
        private void BTN_CONDENSATOR_getNegativeVoltage_Click(object sender, EventArgs e)
        {
            LBL_CONDENSATOR_getNegativeVoltage.Text = MC.Condensator.getNegativeVoltage().ToString();
        }
        //----------------------------------Real: Counters-------------------------------------
        private void TXB_realCOX_MeasureTime_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Convert.ToInt32(TXB_realCOX_MeasureTime.Text);
            }
            catch (Exception)
            {
                return;
            }
            LBL_realCOX_Tiks.Text = "";
            LBL_realCOX_Tiks.Text += MC.Counters.getRTCticks(TXB_realCOX_MeasureTime.Text).ToString();
            LBL_realCOX_frequency.Text = MC.Counters.getRTCfrequency(TXB_realCOX_MeasureTime.Text).ToString();
            LBL_realCOX_Devider.Text = MC.Counters.getRTCprescaler_long(TXB_realCOX_MeasureTime.Text).ToString();
        }
        private void BTN_realCOX_setParameters_Click(object sender, EventArgs e)
        {
            //ФУНКЦИЯ: Задаёт временной интервал счёта в миллисекундах 
            //if (
                //MC.Counters.setMeasureTime(TXB_realCOX_MeasureTime.Text);
            //   )
            //{
                //trace(true, "Задан временной интервал счёта: " + TXB_COA_measureTime.Text + "мс (" + MC.Counters.getRTCticks(TXB_COA_measureTime.Text, MC.Counters.getRTCprescaler(TXB_COA_measureTime.Text)) + " тиков)");
            //}
            //else
            //{
                //trace(true, "Счётчик ещё считает!");
            //}
        }
        delegate void Del(string text);
        delegate void Del_2(string text);
        private void fun()
        {
            
                MC.Counters.Series.Cycles = Convert.ToUInt16(TXB_realCOX_NumberOfMeasurments.Text);
                /*
                Thread myThread = new Thread(MC.Counters.startSeries); //Создаем новый объект потока (Thread)
                myThread.Priority = ThreadPriority.Highest;
                myThread.Start(); //запускаем поток*/

                //*
                MC.Counters.startSeries();
                //if (LBL_realCOX_COA_Result.InvokeRequired)
                //{
                //    LBL_realCOX_COA_Result.Invoke(new Del((s) => LBL_realCOX_COA_Result.Text = s), min + "..." + mid + "..." + max);
                //}
                //else
                //{
                //    LBL_realCOX_COA_Result.Text = min + "..." + mid + "..." + max;
                //}
                //if (answer)
                //{
                //    if (LBL_realCOX_COA_Result.InvokeRequired)
                //    {
                //        LBL_realCOX_COA_Result.Invoke(new Del((s) => trace(true, s)), "Серия завершена корректно!");
                //    }
                //    else
                //    {
                //        trace(true, "Наверное серия завершена корректно..."); ;
                //    }
                //}
                //else
                //{
                //    if (LBL_realCOX_COA_Result.InvokeRequired)
                //    {
                //        LBL_realCOX_COA_Result.Invoke(new Del((s) => trace(true, s)), "Серия завершена НЕ корректно!");
                //    }
                //    else
                //    {
                //        trace(true, "Наверное серия завершена НЕ корректно..."); ;
                //    }
                //}
                //myThread.Join();
                uint max = 0;
                uint min = 4294967295;
                uint mid = 0;
                uint Cycles = Convert.ToUInt16(TXB_realCOX_NumberOfMeasurments.Text);
                List<uint> Results = MC.Counters.COA.Count;
                List<byte> OVFs = new List<byte>();
                for (int i = 0; i < Cycles; i++)
                {
                    try
                    {
                        //trace("Измерение №" + (i + 1) + ": " + Results[i]);
                        if (Results[i] > max)
                        {
                            max = Results[i];
                        }
                        if (Results[i] < min)
                        {
                            min = Results[i];
                        }

                        mid += Results[i];
                    }
                    catch { }
                }
                mid = mid / Cycles;
                if (LBL_realCOX_COA_Result.InvokeRequired)
                {
                    LBL_realCOX_COA_Result.Invoke(new Del((s) => LBL_realCOX_COA_Result.Text = s), min + "..." + mid + "..." + max);
                }
                else
                {
                    LBL_realCOX_COA_Result.Text = min + "..." + mid + "..." + max;
                }
                    //LBL_realCOX_COA_Result.Text = min + "..." + mid + "..." + max;
                
                //LBL_realCOX_COA_Result.Text = min + "..." + mid + "..." + max;
                max = 0;
                min = 4294967295;
                mid = 0;
                OVFs = MC.Counters.COA.Overflows;
                for (int i = 0; i < Cycles; i++)
                {
                    try
                    {
                        //trace("Измерение №" + (i + 1) + ": " + Results[i]);
                        if (OVFs[i] > max)
                        {
                            max = OVFs[i];
                        }
                        if (OVFs[i] < min)
                        {
                            min = OVFs[i];
                        }

                        mid += OVFs[i];
                    }
                    catch { }
                }
                mid = mid / Cycles;
                if (LBL_realCOX_COA_Ovf.InvokeRequired)
                {
                    LBL_realCOX_COA_Ovf.Invoke(new Del((s) => LBL_realCOX_COA_Ovf.Text = s), min + "..." + mid + "..." + max);
                }
                else
                {
                    LBL_realCOX_COA_Ovf.Text = min + "..." + mid + "..." + max;
                }
                //LBL_realCOX_COA_Ovf.Text = min + "..." + mid + "..." + max;
                max = 0;
                min = 4294967295;
                mid = 0;
                Results = MC.Counters.COB.Count;
                for (int i = 0; i < Cycles; i++)
                {
                    try
                    {
                        //trace("Измерение №" + (i + 1) + ": " + Results[i]);
                        if (Results[i] > max)
                        {
                            max = Results[i];
                        }
                        if (Results[i] < min)
                        {
                            min = Results[i];
                        }

                        mid += Results[i];
                    }
                    catch { }
                }
                mid = mid / Cycles;
                if (LBL_realCOX_COB_Result.InvokeRequired)
                {
                    LBL_realCOX_COB_Result.Invoke(new Del((s) => LBL_realCOX_COB_Result.Text = s), min + "..." + mid + "..." + max);
                }
                else
                {
                    LBL_realCOX_COB_Result.Text = min + "..." + mid + "..." + max;
                }
                //LBL_realCOX_COB_Result.Text = min + "..." + mid + "..." + max;
                max = 0;
                min = 4294967295;
                mid = 0;
                OVFs = MC.Counters.COB.Overflows;
                for (int i = 0; i < Cycles; i++)
                {
                    try
                    {
                        //trace("Измерение №" + (i + 1) + ": " + Results[i]);
                        if (OVFs[i] > max)
                        {
                            max = OVFs[i];
                        }
                        if (OVFs[i] < min)
                        {
                            min = OVFs[i];
                        }

                        mid += OVFs[i];
                    }
                    catch { }
                }
                mid = mid / Cycles;
                if (LBL_realCOX_COB_Ovf.InvokeRequired)
                {
                    LBL_realCOX_COB_Ovf.Invoke(new Del((s) => LBL_realCOX_COB_Ovf.Text = s), min + "..." + mid + "..." + max);
                }
                else
                {
                    LBL_realCOX_COB_Ovf.Text = min + "..." + mid + "..." + max;
                }
                //LBL_realCOX_COB_Ovf.Text = min + "..." + mid + "..." + max;
                max = 0;
                min = 4294967295;
                mid = 0;
                Results = MC.Counters.COC.Count;
                for (int i = 0; i < Cycles; i++)
                {
                    try
                    {
                        //trace("Измерение №" + (i + 1) + ": " + Results[i]);
                        if (Results[i] > max)
                        {
                            max = Results[i];
                        }
                        if (Results[i] < min)
                        {
                            min = Results[i];
                        }

                        mid += Results[i];
                    }
                    catch { }
                }
                mid = mid / Cycles;
                if (LBL_realCOX_COC_Result.InvokeRequired)
                {
                    LBL_realCOX_COC_Result.Invoke(new Del((s) => LBL_realCOX_COC_Result.Text = s), min + "..." + mid + "..." + max);
                }
                else
                {
                    LBL_realCOX_COC_Result.Text = min + "..." + mid + "..." + max;
                }
                //LBL_realCOX_COC_Result.Text = min + "..." + mid + "..." + max;
                max = 0;
                min = 4294967295;
                mid = 0;
                OVFs = MC.Counters.COC.Overflows;
                for (int i = 0; i < Cycles; i++)
                {
                    try
                    {
                        //trace("Измерение №" + (i + 1) + ": " + Results[i]);
                        if (OVFs[i] > max)
                        {
                            max = OVFs[i];
                        }
                        if (OVFs[i] < min)
                        {
                            min = OVFs[i];
                        }

                        mid += OVFs[i];
                    }
                    catch { }
                }
                mid = mid / Cycles;
                if (LBL_realCOX_COC_Ovf.InvokeRequired)
                {
                    LBL_realCOX_COC_Ovf.Invoke(new Del((s) => LBL_realCOX_COC_Ovf.Text = s), min + "..." + mid + "..." + max);
                }
                else
                {
                    LBL_realCOX_COC_Ovf.Text = min + "..." + mid + "..." + max;
                }
                //LBL_realCOX_COC_Ovf.Text = min + "..." + mid + "..." + max;
                //*/
                return;
        }
        private void BTN_realCOX_start_Click(object sender, EventArgs e)
        {
            //MC.Counters.Series_Cycles = Convert.ToUInt16(TXB_realCOX_MeasureTime.Text);
            /*
            Thread myThread = new Thread(MC.Counters.startSeries); //Создаем новый объект потока (Thread)
            myThread.Priority = ThreadPriority.Highest;
            myThread.Start(); //запускаем поток*/
            //Thread updateForm_RTC_Status_thread = new Thread(updateForm_RTC_Status);
            //updateForm_RTC_Status_thread.Start();

            //timer1.Interval = 5;
            //timer1.Start();
            //Thread t = new Thread(fun);
            //t.Start();
            //t.Join();
            
            MC.Counters.Series.MeasureTimes[0] = 1000;
            MC.Counters.Series.DelayTimes[0] = 500;
            MC.Counters.Series.DAC.ParentScan[0] = 8000;
            MC.Counters.Series.DAC.Scan[0] = 8000;
            MC.Counters.Series.DAC.Condensator[0] = 8000;
                              
            MC.Counters.Series.MeasureTimes[1] = 100;
            MC.Counters.Series.DelayTimes[1] = 50;
            MC.Counters.Series.DAC.ParentScan[1] = 8000;
            MC.Counters.Series.DAC.Scan[1] = 8000;
            MC.Counters.Series.DAC.Condensator[1] = 8000;
                              
            MC.Counters.Series.MeasureTimes[2] = 50;
            MC.Counters.Series.DelayTimes[2] = 10;
            MC.Counters.Series.DAC.ParentScan[2] = 8000;
            MC.Counters.Series.DAC.Scan[2] = 8000;
            MC.Counters.Series.DAC.Condensator[2] = 8000;
                              
            MC.Counters.Series.MeasureTimes[3] = 50;
            MC.Counters.Series.DelayTimes[3] = 10;
            MC.Counters.Series.DAC.ParentScan[3] = 8000;
            MC.Counters.Series.DAC.Scan[3] = 8000;
            MC.Counters.Series.DAC.Condensator[3] = 8000;
                              
            MC.Counters.Series.MeasureTimes[4] = 50;
            MC.Counters.Series.DelayTimes[4] = 10;
            MC.Counters.Series.DAC.ParentScan[4] = 8500;
            MC.Counters.Series.DAC.Scan[4] = 8000;
            MC.Counters.Series.DAC.Condensator[4] = 8000;
                              
            MC.Counters.Series.MeasureTimes[5] = 50;
            MC.Counters.Series.DelayTimes[5] = 10;
            MC.Counters.Series.DAC.ParentScan[5] = 8000;
            MC.Counters.Series.DAC.Scan[5] = 8000;
            MC.Counters.Series.DAC.Condensator[5] = 8000;
                              
            MC.Counters.Series.MeasureTimes[6] = 75;
            MC.Counters.Series.DelayTimes[6] = 25;
            MC.Counters.Series.DAC.ParentScan[6] = 8000;
            MC.Counters.Series.DAC.Scan[6] = 8000;
            MC.Counters.Series.DAC.Condensator[6] = 8000;
                              
            MC.Counters.Series.MeasureTimes[7] = 100;
            MC.Counters.Series.DelayTimes[7] = 10;
            MC.Counters.Series.DAC.ParentScan[7] = 8000;
            MC.Counters.Series.DAC.Scan[7] = 8000;
            MC.Counters.Series.DAC.Condensator[7] = 8000;
                              
            MC.Counters.Series.MeasureTimes[8] = 1000;
            MC.Counters.Series.DelayTimes[8] = 10;
            MC.Counters.Series.DAC.ParentScan[8] = 8000;
            MC.Counters.Series.DAC.Scan[8] = 8000;
            MC.Counters.Series.DAC.Condensator[8] = 8000;
                              
            MC.Counters.Series.MeasureTimes[9] = 50;
            MC.Counters.Series.DelayTimes[9] = 500;
            MC.Counters.Series.DAC.ParentScan[9] = 8000;
            MC.Counters.Series.DAC.Scan[9] = 8000;
            MC.Counters.Series.DAC.Condensator[9] = 8000;

            fun();
            //for (int i = 0; i < Convert.ToInt32(TXB_realCOX_MeasureTime.Text); i++)
            //{
            //    trace(true,"[№"+i+"][MT:" + MC.Counters.Series_MeasureTimes[i] + "][DT:" + MC.Counters.Series_DelayTimes[i] + "]" + Environment.NewLine + "                       [DAC_PS:" + MC.Counters.Series_DAC_ParentScan[i] + "][DAC_S:" + MC.Counters.Series_DAC_Scan[i] + "][DAC_C:" + MC.Counters.Series_DAC_Condensator[i] + "]" + Environment.NewLine + "                       [ADC_PS:" + MC.Counters.Series_ADC_ParentScan[i] + "][ADC_S:" + MC.Counters.Series_ADC_Scan[i] + "][ADC_Cp:" + MC.Counters.Series_ADC_Condensator_pV[i] + "][ADC_Cn:" + MC.Counters.Series_ADC_Condensator_nV[i] + "]");
            //}
            
            /*
            MC.Counters.startSeries();
            //myThread.Join();
            uint max = 0;
            uint min = 4294967295;
            uint mid = 0;
            uint Series_Cycles = Convert.ToUInt16(TXB_realCOX_MeasureTime.Text);
            List<uint> Results = MC.Counters.COA.Count;
            List<byte> OVFs = new List<byte>();
            for (int i = 0; i < Series_Cycles; i++)
            {
                try
                {
                    //trace("Измерение №" + (i + 1) + ": " + Results[i]);
                    if (Results[i] > max)
                    {
                        max = Results[i];
                    }
                    if (Results[i] < min)
                    {
                        min = Results[i];
                    }
                
                    mid += Results[i];
                }
                catch { }
            }
            mid = mid / Series_Cycles;
            LBL_realCOX_COA_Result.Text = min + "..." + mid + "..." + max;
            max = 0;
            min = 4294967295;
            mid = 0;
            OVFs = MC.Counters.COA.Overflows;
            for (int i = 0; i < Series_Cycles; i++)
            {
                try
                {
                    //trace("Измерение №" + (i + 1) + ": " + Results[i]);
                    if (OVFs[i] > max)
                    {
                        max = OVFs[i];
                    }
                    if (OVFs[i] < min)
                    {
                        min = OVFs[i];
                    }

                    mid += OVFs[i];
                }
                catch { }
            }
            mid = mid / Series_Cycles;
            LBL_realCOX_COA_Ovf.Text = min + "..." + mid + "..." + max;
            max = 0;
            min = 4294967295;
            mid = 0;
            Results = MC.Counters.COB.Count;
            for (int i = 0; i < Series_Cycles; i++)
            {
                try
                {
                    //trace("Измерение №" + (i + 1) + ": " + Results[i]);
                    if (Results[i] > max)
                    {
                        max = Results[i];
                    }
                    if (Results[i] < min)
                    {
                        min = Results[i];
                    }

                    mid += Results[i];
                }
                catch { }
            }
            mid = mid / Series_Cycles;
            LBL_realCOX_COB_Result.Text = min + "..." + mid + "..." + max;
            max = 0;
            min = 4294967295;
            mid = 0;
            OVFs = MC.Counters.COB.Overflows;
            for (int i = 0; i < Series_Cycles; i++)
            {
                try
                {
                    //trace("Измерение №" + (i + 1) + ": " + Results[i]);
                    if (OVFs[i] > max)
                    {
                        max = OVFs[i];
                    }
                    if (OVFs[i] < min)
                    {
                        min = OVFs[i];
                    }

                    mid += OVFs[i];
                }
                catch { }
            }
            mid = mid / Series_Cycles;
            LBL_realCOX_COB_Ovf.Text = min + "..." + mid + "..." + max;
            max = 0;
            min = 4294967295;
            mid = 0;
            Results = MC.Counters.COC.Count;
            for (int i = 0; i < Series_Cycles; i++)
            {
                try
                {
                    //trace("Измерение №" + (i + 1) + ": " + Results[i]);
                    if (Results[i] > max)
                    {
                        max = Results[i];
                    }
                    if (Results[i] < min)
                    {
                        min = Results[i];
                    }

                    mid += Results[i];
                }
                catch { }
            }
            mid = mid / Series_Cycles;
            LBL_realCOX_COC_Result.Text = min + "..." + mid + "..." + max;
            max = 0;
            min = 4294967295;
            mid = 0;
            OVFs = MC.Counters.COC.Overflows;
            for (int i = 0; i < Series_Cycles; i++)
            {
                try
                {
                    //trace("Измерение №" + (i + 1) + ": " + Results[i]);
                    if (OVFs[i] > max)
                    {
                        max = OVFs[i];
                    }
                    if (OVFs[i] < min)
                    {
                        min = OVFs[i];
                    }

                    mid += OVFs[i];
                }
                catch { }
            }
            mid = mid / Series_Cycles;
            LBL_realCOX_COC_Ovf.Text = min + "..." + mid + "..." + max;
            //*/
        }
        private void BTN_realCOX_stop_Click(object sender, EventArgs e)
        {
            //MC.Counters.stopMeasure();
        }
        private void BTN_realCOX_check_Click(object sender, EventArgs e)
        {
           // LBL_realCOX_RTCstate.Text = MC.Counters.receiveResults();
            //LBL_realCOX_COA_Result.Text = MC.Counters.COA.Result.ToString();
            //LBL_realCOX_COB_Result.Text = MC.Counters.COB.Result.ToString();
            //LBL_realCOX_COC_Result.Text = MC.Counters.COC.Result.ToString();
            //LBL_realCOX_COA_Ovf.Text = MC.Counters.COA.overflows.ToString();
            //LBL_realCOX_COB_Ovf.Text = MC.Counters.COB.overflows.ToString();
            //LBL_realCOX_COC_Ovf.Text = MC.Counters.COC.overflows.ToString();
        }
        private void BTN_checkFlags_Click(object sender, EventArgs e)
        {
           
            byte flags = MC.setFlags(false, CHB_iHVE.Checked, CHB_iEDCD.Checked, CHB_SEMV1.Checked, CHB_SEMV2.Checked, CHB_SEMV3.Checked, CHB_SPUMP.Checked);
            if ((flags & 1) == 1){CHB_SPUMP.Checked = true;}else{CHB_SPUMP.Checked = false;}
            if ((flags & 2) == 2) { CHB_SEMV3.Checked = true; } else { CHB_SEMV3.Checked = false; }
            if ((flags & 4) == 4) { CHB_SEMV2.Checked = true; } else { CHB_SEMV2.Checked = false; }
            if ((flags & 8) == 8) { CHB_SEMV1.Checked = true; } else { CHB_SEMV1.Checked = false; }
            if ((flags & 16) == 16) { CHB_iEDCD.Checked = true; } else { CHB_iEDCD.Checked = false; }
            if ((flags & 32) == 32) { CHB_iHVE.Checked = true; } else { CHB_iHVE.Checked = false; }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            MC.setFlags(true, CHB_iHVE.Checked, CHB_iEDCD.Checked, CHB_SEMV1.Checked, CHB_SEMV2.Checked, CHB_SEMV3.Checked, CHB_SPUMP.Checked);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            LBL_realCOX_RTCstate.Text = MC.Counters.Status;
            
        }
    }
}
