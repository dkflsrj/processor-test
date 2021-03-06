﻿using System;
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
        decimal UI_PGB_COA_step = 0;
        decimal UI_PGB_COA_count = 0;
        const int CLK_COA_intreval = 10;
        public Form1()
        {
            InitializeComponent();
            MC.Service.setTracer(Log);
            MC.Service.trace("Программа инициирована!");
            CMB_COM_BaudRate.Enabled = false;
            CMB_COM_DataBits.Enabled = false;
            CMB_COM_Handshake.Enabled = false;
            CMB_COM_Parity.Enabled = false;
            CMB_COM_StopBits.Enabled = false;
            BTN_COM_setParams.Enabled = false;

            if (findCOM())
            {
                setCOMparams();
                TABpanel.SelectedIndex = 1;
            }
            CLK_COA.Interval = CLK_COA_intreval;
            CLK_timer.Enabled = false;

            MC.Flags.PRGE_blocked += new EventCallBack(PRGE_blocked);
            MC.Counters.MeasureEnd += new EventCallBack(EventHandler);
            MC.CriticalError_HVE_decoder += new EventCallBack(Event_TIC_HVE_decoder_error);
            MC.CriticalError_HVE_TIC_noResponse += new EventCallBack(Event_TIC_noResponse);
            MC.TIC_approve_HVE += new EventCallBack(Event_TIC_approve_HVE);
            MC.TIC_disapprove_HVE += new EventCallBack(Event_TIC_disapprove_HVE);
        }
        //Функции интерфейса
        delegate void delegate_trace(string text);
        void trace(string text)
        {
            if (Log.InvokeRequired)
            {
                delegate_trace a_delegate = new delegate_trace(trace);
                Log.Invoke(a_delegate, text);
            }
            else
            {
                Log.AppendText(Environment.NewLine + "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + text);
                Log.ScrollToCaret();
            }
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
            COM_Port = new SerialPort(cBox_COM.Text);
            MC.setUSART(COM_Port);
        }
        bool findCOM()
        {
            trace("Поиск COM портов...");
            string[] Ports = SerialPort.GetPortNames();
            if (Ports.Length > 0)
            {
                trace("   Список обнаруженных COM портов:");
                for (int i = 0; i < Ports.Length; i++)
                {
                    cBox_COM.Items.Add(Ports[i]);
                    trace("       - " + Ports[i]);
                }
                cBox_COM.Text = Ports[0];
                //Включаем настройки
                //CMB_COM_BaudRate.Enabled = true;
                //CMB_COM_DataBits.Enabled = true;
                //CMB_COM_Handshake.Enabled = true;
                //CMB_COM_Parity.Enabled = true;
                //CMB_COM_StopBits.Enabled = true;
                BTN_COM_setParams.Enabled = true;
                return true;
            }
            else
            {
                trace("ОШИБКА: Ни один COM порт не найден!");
                return false;
            }

        }
        //Функции МК
        private void BTN_MCstatus_Click(object sender, EventArgs e)
        {
            //trace(true, "Статус: " + 
            MC.Chip.getStatus();
        }
        private void BTN_COM_getMCversion_Click(object sender, EventArgs e)
        {
            //trace(true, "Запрос информации о прошивке у МК...");
            MC.Chip.getVersion();
            //trace(true, "   Дата создания: " + 
            //trace(true, "   Версия: " + 
        }
        private void BTN_COM_getCPUfreq_Click(object sender, EventArgs e)
        {
            //trace(true, "Частота CPU: " + 
            MC.Chip.getCPUfrequency();
        }
        private void BTN_COM_setMCwait_Click(object sender, EventArgs e)
        {
            //MC.setMCwait();
            MC.Chip.getBirthday();
        }
        private void BTN_LEDbyte_Click(object sender, EventArgs e)
        {
            //MC.Service.showMeByte(Convert.ToByte(TXB_LEDbyte.Text));
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
            // MC.PSIS.F8.setVoltage(TXB_DAC_voltage.Text);
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
            //MC.Counters.startMeasure();
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
            //    //trace(true, "Задан временной интервал счёта: " + TXB_COA_measureTime.Text + "мс (" + MC.Counters.calcRTCticks(TXB_COA_measureTime.Text, MC.Counters.calcRTCprescaler(TXB_COA_measureTime.Text)) + " тиков)");
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
            LBL_COA_ticks.Text += MC.Counters.calcRTCticks(TXB_COA_measureTime.Text).ToString();
            //LBL_COA_ticks.Text += " + " + MC.RTC.get_Ticks(TXB_COA_delay.Text, 1).ToString() + ")";
            //LBL_COA_ticks.Text += "*" + TXB_COA_quantity.Text;
            LBL_COA_frequency.Text = MC.Counters.calcRTCfrequency(TXB_COA_measureTime.Text).ToString();
            LBL_COA_prescaler.Text = MC.Counters.calcRTCprescaler_long(TXB_COA_measureTime.Text).ToString();
        }
        private void CHB_enableSuperTracer_CheckedChanged(object sender, EventArgs e)
        {
            if (CHB_enableSuperTracer.Checked)
            {
                MC.Service.enableTracerInTransmit(true);
            }
            else
            {
                MC.Service.enableTracerInTransmit(false);
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
                MC.Service.enableLog(true);
            }
            else
            {
                MC.Service.enableLog(false);
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
            //TIC.External_Air_Cooler.setup = TIC.External_Air_Cooler.ListOfSetups.Permanent;
            //TIC.External_Air_Cooler.setup = TIC.External_Air_Cooler.ListOfSetups.Turbo_slaved;
            //trace(TIC.External_Air_Cooler.state);
            //MC.Service.test();
            string a = MC.Flags.HVE;
        }
        private void BTN_TIC_DisplayContrast_set_Click(object sender, EventArgs e)
        {
            TIC.Display_contrast = TXB_TIC_DisplayContrast.Text;
        }
        private void BTN_TIC_DisplayContrast_get_Click(object sender, EventArgs e)
        {
            TXB_TIC_DisplayContrast.Text = TIC.Display_contrast;
        }
        private void BTN_traceErrorList_Click(object sender, EventArgs e)
        {
            //foreach (string error in MC.getErrorList().ToArray())
            //{
            //    trace(error);
            //}
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
            //MC.PSInl.reset();
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
            //MC.DPS.reset();
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
            //try
            //{
            //    Convert.ToInt32(TXB_realCOX_MeasureTime.Text);
            //}
            //catch (Exception)
            //{
            //    return;
            //}
            LBL_realCOX_Tiks.Text = "";
            LBL_realCOX_Tiks.Text += MC.Counters.calcRTCticks(TXB_realCOX_MeasureTime.Text).ToString();
            LBL_realCOX_frequency.Text = MC.Counters.calcRTCfrequency(TXB_realCOX_MeasureTime.Text).ToString();
            LBL_realCOX_Devider.Text = MC.Counters.calcRTCprescaler_long(TXB_realCOX_MeasureTime.Text).ToString();
        }
        private void BTN_realCOX_setParameters_Click(object sender, EventArgs e)
        {
            //ФУНКЦИЯ: Задаёт временной интервал счёта в миллисекундах 
            //if (
            //MC.Counters.setMeasureTime(TXB_realCOX_MeasureTime.Text);
            //   )
            //{
            //trace(true, "Задан временной интервал счёта: " + TXB_COA_measureTime.Text + "мс (" + MC.Counters.calcRTCticks(TXB_COA_measureTime.Text, MC.Counters.calcRTCprescaler(TXB_COA_measureTime.Text)) + " тиков)");
            //}
            //else
            //{
            //trace(true, "Счётчик ещё считает!");
            //}
        }
        uint Cycles;
        private void BTN_realCOX_start_Click(object sender, EventArgs e)
        {
            //if (Convert.ToUInt16(TXB_realCOX_NumberOfMeasurments.Text) > 0)
            //{
                //Cycles = Convert.ToUInt16(TXB_realCOX_NumberOfMeasurments.Text);
                //Cycles_performed = 0;
                MC.Counters.startMeasure(TXB_realCOX_MeasureTime.Text);
            //}
        }
        uint COA_min = uint.MaxValue;
        uint COA_mid = 0;
        uint COA_max = uint.MinValue;
        uint COB_min = uint.MaxValue;
        uint COB_mid = 0;
        uint COB_max = uint.MinValue;
        uint COC_min = uint.MaxValue;
        uint COC_mid = 0;
        uint COC_max = uint.MinValue;
        double RTC_min = double.MaxValue;
        double RTC_mid = 0;
        double RTC_max = double.MinValue;
        uint COX_buf;
        delegate void delegate_void();
        uint Cycles_performed;
        public void EventHandler()
        {
            
            //*
            if (GPB_realCOX.InvokeRequired)
            {
                delegate_void a_delegate_2 = new delegate_void(EventHandler);
                GPB_realCOX.Invoke(a_delegate_2);
            }
            else
            {
                MC.Counters.receiveResults();
                LBL_realCOX_RTCstate.Text = MC.Counters.Status;
                LBL_realCOX_COA_Result.Text = MC.Counters.COA.ToString();
                LBL_realCOX_COB_Result.Text = MC.Counters.COB.ToString();
                LBL_realCOX_COC_Result.Text = MC.Counters.COC.ToString();
                LBL_realCOX_RTC_OverTime.Text = MC.Counters.OverTime.ToString();
                //Cycles_performed++;
                ////TXB_realCOX_NumberOfMeasurments.Text = (Cycles - Cycles_performed).ToString();
                //
                //COX_buf = MC.Counters.COA;
                //if (COA_min > COX_buf) { COA_min = COX_buf; }
                //COA_mid = (COA_mid * (Cycles_performed - 1) + COX_buf) / Cycles_performed;
                //if (COA_max < COX_buf) { COA_max = COX_buf; }
                //
                //COX_buf = MC.Counters.COB;
                //if (COB_min > COX_buf) { COB_min = COX_buf; }
                //COB_mid = (COB_mid * (Cycles_performed - 1) + COX_buf) / Cycles_performed;
                //if (COB_max < COX_buf) { COB_max = COX_buf; }
                //
                //COX_buf = MC.Counters.COC;
                //if (COC_min > COX_buf) { COC_min = COX_buf; }
                //COC_mid = (COC_mid * (Cycles_performed - 1) + COX_buf) / Cycles_performed;
                //if (COC_max < COX_buf) { COC_max = COX_buf; }
                //
                //double RTC_buf = MC.Counters.OverTime;
                //if (RTC_min > RTC_buf) { RTC_min = RTC_buf; }
                //RTC_mid = (RTC_mid * (Cycles_performed - 1) + RTC_buf) / Cycles_performed;
                //if (RTC_max < RTC_buf) { RTC_max = RTC_buf; }
                //
                //if (Cycles_performed < Cycles)
                //{
                //    //Тут настройки SPI
                //    Thread.Sleep(20);
                //    MC.Counters.startMeasure(TXB_realCOX_MeasureTime.Text);
                //    LBL_realCOX_RTCstate.Text = MC.Counters.Status;
                //}
                //else
                //{
                //    LBL_realCOX_COA_Result.Text = COA_min + "..." + COA_mid + "..." + COA_max;
                //    LBL_realCOX_COB_Result.Text = COB_min + "..." + COB_mid + "..." + COB_max;
                //    LBL_realCOX_COC_Result.Text = COC_min + "..." + COC_mid + "..." + COC_max;
                //    LBL_realCOX_RTC_OverTime.Text = RTC_min + "..." + RTC_mid + "..." + RTC_max;
                //}
            }
            //*/
        }
        private void BTN_realCOX_stop_Click(object sender, EventArgs e)
        {
            MC.Counters.stopMeasure();
        }
        private void BTN_realCOX_check_Click(object sender, EventArgs e)
        {
            //EventHandler();
            MC.Counters.receiveResults();
        }
        private void BTN_checkFlags_Click(object sender, EventArgs e)
        {
            //byte flags = MC.setFlags(false, CHB_PRGE.Checked, CHB_iEDCD.Checked, CHB_SEMV1.Checked, CHB_SEMV2.Checked, CHB_SEMV3.Checked, CHB_SPUMP.Checked);
            //if ((flags & 1) == 1) { CHB_SPUMP.Checked = true; } else { CHB_SPUMP.Checked = false; }
            //if ((flags & 2) == 2) { CHB_SEMV3.Checked = true; } else { CHB_SEMV3.Checked = false; }
            //if ((flags & 4) == 4) { CHB_SEMV2.Checked = true; } else { CHB_SEMV2.Checked = false; }
            //if ((flags & 8) == 8) { CHB_SEMV1.Checked = true; } else { CHB_SEMV1.Checked = false; }
            //if ((flags & 16) == 16) { CHB_iEDCD.Checked = true; } else { CHB_iEDCD.Checked = false; }
            //if ((flags & 32) == 32) { CHB_PRGE.Checked = true; } else { CHB_PRGE.Checked = false; }
            //if ((flags & 64) == 64) { CHB_iHVE.CheckState = CheckState.Checked; } else { CHB_iHVE.CheckState = CheckState.Unchecked; }
            switch (MC.Flags.HVE)
            {
                case "Enabled": CHB_iHVE.CheckState = CheckState.Checked;
                    break;
                case "Blocked": CHB_iHVE.CheckState = CheckState.Unchecked;
                    break;
                default: CHB_iHVE.CheckState = CheckState.Indeterminate;
                    break;
            }
            switch (MC.Flags.PRGE)
            {
                case "On": CHB_PRGE.CheckState = CheckState.Checked;
                    break;
                case "Off": CHB_PRGE.CheckState = CheckState.Unchecked;
                    break;
                case "Blocked": CHB_PRGE.CheckState = CheckState.Unchecked;
                    break;
                default: CHB_PRGE.CheckState = CheckState.Indeterminate;
                    break;
            }
            switch (MC.Flags.EDCD)
            {
                case "On": CHB_iEDCD.CheckState = CheckState.Checked;
                    break;
                case "Off": CHB_iEDCD.CheckState = CheckState.Unchecked;
                    break;
                default: CHB_iEDCD.CheckState = CheckState.Indeterminate;
                    break;
            }
            switch (MC.Flags.SEMV1)
            {
                case "On": CHB_SEMV1.CheckState = CheckState.Checked;
                    break;
                case "Off": CHB_SEMV1.CheckState = CheckState.Unchecked;
                    break;
                default: CHB_SEMV1.CheckState = CheckState.Indeterminate;
                    break;
            }
            switch (MC.Flags.SEMV2)
            {
                case "On": CHB_SEMV2.CheckState = CheckState.Checked;
                    break;
                case "Off": CHB_SEMV2.CheckState = CheckState.Unchecked;
                    break;
                default: CHB_SEMV2.CheckState = CheckState.Indeterminate;
                    break;
            }
            switch (MC.Flags.SEMV3)
            {
                case "On": CHB_SEMV3.CheckState = CheckState.Checked;
                    break;
                case "Off": CHB_SEMV3.CheckState = CheckState.Unchecked;
                    break;
                default: CHB_SEMV3.CheckState = CheckState.Indeterminate;
                    break;
            }
            switch (MC.Flags.SPUMP)
            {
                case "On": CHB_SPUMP.CheckState = CheckState.Checked;
                    break;
                case "Off": CHB_SPUMP.CheckState = CheckState.Unchecked;
                    break;
                default: CHB_SPUMP.CheckState = CheckState.Indeterminate;
                    break;
            }
        }
        void PRGE_blocked()
        {
            CHB_PRGE.CheckState = CheckState.Indeterminate;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //MC.setFlags(true, CHB_PRGE.Checked, CHB_iEDCD.Checked, CHB_SEMV1.Checked, CHB_SEMV2.Checked, CHB_SEMV3.Checked, CHB_SPUMP.Checked);
            if (CHB_PRGE.Checked) { MC.Flags.PRGE = "On"; } else { MC.Flags.PRGE = "Off"; }
            //if (CHB_iEDCD.Checked) { MC.Flags.EDCD = "On"; } else { MC.Flags.EDCD = "Off"; }
            //if (CHB_SEMV1.Checked) { MC.Flags.SEMV1 = "On"; } else { MC.Flags.SEMV1 = "Off"; }
            //if (CHB_SEMV2.Checked) { MC.Flags.SEMV2 = "On"; } else { MC.Flags.SEMV2 = "Off"; }
            //if (CHB_SEMV3.Checked) { MC.Flags.SEMV3 = "On"; } else { MC.Flags.SEMV3 = "Off"; }
            //if (CHB_SPUMP.Checked) { MC.Flags.SPUMP = "On"; } else { MC.Flags.SPUMP = "Off"; }
        }
        private void BTN_TIC_FOR_Update_Click(object sender, EventArgs e)
        {
           // LBL_TIC_FOR_State.Text = TIC.Backing.Pump.state;
           // LBL_TIC_FOR_Speed.Text = TIC.Backing.speed + "%";
           // LBL_TIC_FOR_Power.Text = TIC.Backing.power + "Вт";
           // LBL_TIC_FOR_Setup.Text = TIC.Backing.Pump.setup;
           // LBL_TIC_FOR_Type.Text = TIC.Backing.Pump.type;
            string T = "";
            string B = "";
            string G1 ="";
            string G2 ="";
            string G3 ="";
            string R1 ="";
            string R2 ="";
            string R3 ="";
            TIC.TIC_Status.value(ref T, ref B, ref G1, ref G2, ref G3, ref R1, ref R2, ref R3);
        }
        private void BTN_TIC_FOR_ON_Click(object sender, EventArgs e)
        {
            TIC.Backing.Pump.turn("On");
            LBL_TIC_FOR_State.Text = TIC.Backing.Pump.state;
        }
        private void BTN_TIC_FOR_OFF_Click(object sender, EventArgs e)
        {
            TIC.Backing.Pump.turn("Off");
            LBL_TIC_FOR_State.Text = TIC.Backing.Pump.state;
        }
        private void BTN_TIC_TRB_Update_Click(object sender, EventArgs e)
        {
            LBL_TIC_TRB_State.Text = TIC.Turbo.Pump.state;
            //LBL_TIC_TRB_Speed.Text = TIC.Turbo.speed.value + "%";
            //LBL_TIC_TRB_Power.Text = TIC.Turbo.power + "Вт";
            //string t1 = "";
            //string t2 = "";
            //TIC.EXT75DX.Temperature_readings(ref t1, ref t2);
            //LBL_TIC_TRB_Temperatures.Text = "D:" + t1 +"c | P:" + t2 + "C";//TIC.Turbo.Pump.delay + " минут";
            //LBL_TIC_TRB_Type.Text = TIC.Turbo.Pump.type;
        }
        private void BTN_TIC_TRB_ON_Click(object sender, EventArgs e)
        {
            TIC.Turbo.Pump.turnOn();
            LBL_TIC_FOR_State.Text = TIC.Turbo.Pump.state;
        }
        private void BTN_TIC_TRB_OFF_Click(object sender, EventArgs e)
        {
            TIC.Turbo.Pump.turnOff();
            LBL_TIC_FOR_State.Text = TIC.Turbo.Pump.state;
        }
        private void BTN_TIC_Gauge1_Update_Click(object sender, EventArgs e)
        {
            //LBL_TIC_Gauge1_type.Text = TIC.Gauge_1.type;
            string buf_1 = "";
            string buf_2 = "";
            //TIC.Gauge_1.Gas_type(ref buf_1, ref buf_2);
            //LBL_TIC_Gauge1_gasType.Text = buf_1;
            //LBL_TIC_Gauge1_Filter.Text = buf_2;
            //LBL_TIC_Gauge1_name.Text = TIC.Gauge_1.name;
            LBL_TIC_Gauge1_value.Text = TIC.Gauge_1.value(ref buf_1, ref buf_2);
            LBL_TIC_Gauge1_value.Text += " " + buf_1;
            LBL_TIC_Gauge1_state.Text = buf_2;
        }
        private void BTN_TIC_Gauge2_Update_Click(object sender, EventArgs e)
        {
            //LBL_TIC_Gauge2_type.Text = TIC.Gauge_2.type;
            string buf_1 = "";
            string buf_2 = "";
            LBL_TIC_TRB_Speed.Text = TIC.Turbo.speed.value + "%";
            LBL_TIC_TRB_Power.Text = TIC.Turbo.power + "Вт";
            //TIC.Gauge_2.Gas_type(ref buf_1, ref buf_2);
            //LBL_TIC_Gauge2_gasType.Text = buf_1;
            //LBL_TIC_Gauge2_Filter.Text = buf_2;
            //LBL_TIC_Gauge2_name.Text = TIC.Gauge_2.name;
            LBL_TIC_Gauge2_value.Text = TIC.Gauge_2.value(ref buf_1, ref buf_2);
            LBL_TIC_Gauge2_value.Text += " " + buf_1;
            //LBL_TIC_Gauge2_state.Text = buf_2;
            LBL_TIC_Gauge1_value.Text = TIC.Gauge_1.value(ref buf_1, ref buf_2);
            LBL_TIC_Gauge1_value.Text += " " + buf_1;
            //LBL_TIC_Gauge1_state.Text = buf_2;
            //string t1 = "";
            //string t2 = "";
            //TIC.EXT75DX.Temperature_readings(ref t1, ref t2);
            //LBL_TIC_TRB_Temperatures.Text = "D:" + t1 + "c | P:" + t2 + "C";
            LBL_TIC_FOR_Speed.Text = TIC.Backing.speed + "%";
            LBL_TIC_FOR_Power.Text = TIC.Backing.power + "Вт";
            switch (MC.Flags.HVE)
            {
                case "Enabled": CHB_iHVE.CheckState = CheckState.Checked;
                    break;
                case "Blocked": CHB_iHVE.CheckState = CheckState.Unchecked;
                    break;
                default: CHB_iHVE.CheckState = CheckState.Indeterminate;
                    break;
            }
        }
        //-----
        void Event_TIC_approve_HVE()
        {
            MessageBox.Show("TIC сообщает о том, что\r\nможно включить высокое напряжение.","Сообщение от TIC'a",MessageBoxButtons.OK,MessageBoxIcon.Information);
            if (CHB_iHVE.InvokeRequired)
            {
                CHB_iHVE.BeginInvoke(new MethodInvoker(delegate { CHB_iHVE.CheckState = CheckState.Checked; }));
            }
            else
            {
                CHB_iHVE.CheckState = CheckState.Checked;
            }
        }
        void Event_TIC_disapprove_HVE()
        {
            MessageBox.Show("TIC сообщает о том, что\r\nвысокое напряжение заблокировано!", "Сообщение от TIC'a", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            if (CHB_iHVE.InvokeRequired)
            {
                CHB_iHVE.BeginInvoke(new MethodInvoker(delegate { CHB_iHVE.CheckState = CheckState.Unchecked; }));
            }
            else
            {
                CHB_iHVE.CheckState = CheckState.Unchecked;
            }
        }
        void Event_TIC_HVE_decoder_error()
        {
            MessageBox.Show("MC сообщает о том, что\r\nему три раза подряд не удалось\r\nрасшифровать сообщение от TIC'а!\r\nВысокое напряжение выключено!", "Сообщение от MC'a", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (CHB_iHVE.InvokeRequired)
            {
                CHB_iHVE.BeginInvoke(new MethodInvoker(delegate { CHB_iHVE.CheckState = CheckState.Unchecked; }));
            }
            else
            {
                CHB_iHVE.CheckState = CheckState.Unchecked;
            }
        }
        void Event_TIC_noResponse()
        {
            MessageBox.Show("MC сообщает о том, что\r\nему три раза подряд не удалось\r\nсвязаться с TIC'ом!\r\nВысокое напряжение выключено!", "Сообщение от MC'a", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (CHB_iHVE.InvokeRequired)
            {
                CHB_iHVE.BeginInvoke(new MethodInvoker(delegate { CHB_iHVE.CheckState = CheckState.Unchecked; })); 
            }
            else
            {
                CHB_iHVE.CheckState = CheckState.Unchecked;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            switch (MC.Flags.HVE)
            {
                case "Enabled": CHB_iHVE.CheckState = CheckState.Checked;
                    break;
                case "Blocked": CHB_iHVE.CheckState = CheckState.Unchecked;
                    break;
                default: CHB_iHVE.CheckState = CheckState.Indeterminate;
                    break;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MC.Flags.SEMV1 = "On";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            MC.Flags.SEMV1 = "Off";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            switch (MC.Flags.SEMV1)
            {
                case "On": CHB_SEMV1.CheckState = CheckState.Checked;
                    break;
                case "Off": CHB_SEMV1.CheckState = CheckState.Unchecked;
                    break;
                default: CHB_SEMV1.CheckState = CheckState.Indeterminate;
                    break;
            }
        }
    }
}
