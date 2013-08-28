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
        XMEGA32A4U MC;
        decimal UI_PGB_COA_step = 0;
        decimal UI_PGB_COA_count = 0;
        const int CLK_COA_intreval = 10;

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

            findCOM();
            setCOMparams();
            TABpanel.SelectedIndex = 1;

            MC.setUSART(COM_Port);
            MC.setTracer(Log);

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
        void findCOM()
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
            }
            else
            {
                trace(true, "ОШИБКА: Ни один COM порт не найден!");
            }

        }
        //Функции МК
        private void BTN_MCstatus_Click(object sender, EventArgs e)
        {
            trace(true, "Статус: " + MC.getStatus().ToString());
        }
        private void BTN_COM_getMCversion_Click(object sender, EventArgs e)
        {
            trace(true, "Запрос информации о прошивке у МК...");
            trace(true, "   Дата создания: " + MC.getBirthday());
            trace(true, "   Версия: " + MC.getVersion().ToString());
        }
        private void BTN_COM_getCPUfreq_Click(object sender, EventArgs e)
        {
            trace(true, "Частота CPU: " + MC.getCPUfrequency());
        }
        private void BTN_COM_setMCwait_Click(object sender, EventArgs e)
        {
            //MC.setMCwait();
        }
        private void BTN_LEDbyte_Click(object sender, EventArgs e)
        {
            MC.showMeByte(TXB_LEDbyte.Text);
        }
        private void BTN_SPI_DAC_send_Click(object sender, EventArgs e)
        {
            if (MC.DAC.setVoltage(TXB_DAC_channel.Text, TXB_DAC_voltage.Text))
            {
                trace(true, "На канал " + TXB_DAC_channel.Text + " выставлено напряжение: " + TXB_DAC_voltage.Text);
            }
            else
            {
                trace(true, "ОШИБКА ОТКЛИКА! DAC возможно не выставил напряжение!");
            }
        }
        private void BTN_SPI_ADC_request_Click(object sender, EventArgs e)
        {
            MC.ADC.DoubleRange = CHB_ADC_DoubleRange.Checked;
            trace(true, "Напряжение на канале " + TXB_ADC_channel.Text + " : " + MC.ADC.getVoltage(TXB_ADC_channel.Text));

        }
        private void BTN_DAC_reset_Click(object sender, EventArgs e)
        {
            if (MC.DAC.reset())
            {
                trace(true, "DAC сброшен!");
            }
            else
            {
                trace(true, "ОШИБКА ОТКЛИКА! DAC возможно НЕ сброшен!");
            }
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
                trace(true, "ОШИБКА! Неверный интервал!");
                return;
            }
            if (MC.Counters.startMeasure())
            {
                trace(true, "COA начал счёт...");
                PGB_COA_progress.Value = 0;
                UI_PGB_COA_count = 0;
                UI_PGB_COA_step = (3000 * (decimal)CLK_COA_intreval) / Convert.ToInt32(TXB_COA_measureTime.Text);
                CLK_COA.Enabled = true;
                return;
            }
            trace(true, "Ошибка! Возможно COA не начал счёт!");

        }
        private void BTN_reqCount_Click(object sender, EventArgs e)
        {
            //ФУНКЦИЯ: Проверям счётчик МК, если сосчитал, то принимаем результат
            string Counters_status = MC.Counters.getResults();
            trace(true, "Состояние счётчиков: " + Counters_status);
            if (Counters_status == "Ready")
            {
                trace(true, "   Счёт: " + MC.Counters.COA_Result);
            }
        }
        private void BTN_setInterval_Click(object sender, EventArgs e)
        {
            //ФУНКЦИЯ: Задаёт временной интервал счёта в миллисекундах 
            try
            {
                Convert.ToInt32(TXB_COA_measureTime.Text);
            }
            catch (Exception)
            {
                trace(true, "ОШИБКА! Неверное время измерения!");
                return;
            }
            try
            {
                Convert.ToByte(TXB_COA_delay.Text);
            }
            catch (Exception)
            {
                trace(true, "ОШИБКА! Неверное время паузы!");
                return;
            }
            try
            {
                Convert.ToUInt16(TXB_COA_quantity.Text);
            }
            catch (Exception)
            {
                trace(true, "ОШИБКА! Неверное количество измерений!");
                return;
            }
            if (MC.Counters.setMeasureTime(TXB_COA_measureTime.Text))
            {
                trace(true, "Задан временной интервал счёта: " + TXB_COA_measureTime.Text + "мс (" + MC.Counters.getRTCticks(TXB_COA_measureTime.Text, MC.Counters.getRTCPrescaler(TXB_COA_measureTime.Text)) + " тиков)");
            }
            else
            {
                trace(true, "Счётчик ещё считает!");
            }
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
            if (MC.Counters.stopMeasure())
            {
                trace(true, "Счётчик был успешно остановлен!");
                CLK_COA.Enabled = false;
                PGB_COA_progress.Value = PGB_COA_progress.Minimum;
                return;
            }
            trace(true, "ОШИБКА ОТКЛИКА! Возможно Счётчик не был остановлен!");
        }


        private void CLK_timer_Tick(object sender, EventArgs e)
        {
            if (CHB_TotalControl.Checked)
            {
                switch (MC.getStatus())
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
            LBL_COA_ticks.Text += MC.Counters.getRTCticks(TXB_COA_measureTime.Text, MC.Counters.getRTCPrescaler(TXB_COA_measureTime.Text)).ToString();
            //LBL_COA_ticks.Text += " + " + MC.RTC.get_Ticks(TXB_COA_delay.Text, 1).ToString() + ")";
            //LBL_COA_ticks.Text += "*" + TXB_COA_quantity.Text;
            LBL_COA_frequency.Text = MC.Counters.getRTCfreqency().ToString();
            LBL_COA_prescaler.Text = MC.Counters.getRTCPrescaler(TXB_COA_measureTime.Text).ToString();
        }

        private void CHB_enableSuperTracer_CheckedChanged(object sender, EventArgs e)
        {
            if (CHB_enableSuperTracer.Checked)
            {
                MC.tracer_enable(true);
            }
            else
            {
                MC.tracer_enable(false);
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
            Process.Start("log.txt");
        }

        private void CHB_traceLog_CheckedChanged(object sender, EventArgs e)
        {
            if (CHB_traceLog.Checked)
            {
                MC.log_enable(true);
            }
            else
            {
                MC.log_enable(false);
            }
        }

        private void WATCHER_Counters_Tick(object sender, EventArgs e)
        {

            switch (MC.Counters.getStatus())
            {
                case "Ready": LBL_COA_status.Text = "Готов";
                    LBL_COA_status.ForeColor = System.Drawing.Color.Green;
                    break;
                case "Stopped": LBL_COA_status.Text = "Остановлен";
                    LBL_COA_status.ForeColor = System.Drawing.Color.Orange;
                    break;
                case "Busy": LBL_COA_status.Text = "Считает";
                    LBL_COA_status.ForeColor = System.Drawing.Color.Orange;
                    break;
                default: LBL_COA_status.Text = "Ошибка состояния!";
                    LBL_COA_status.ForeColor = System.Drawing.Color.Red;
                    break;
            }
        }

        private void BTN_sendSomething_Click(object sender, EventArgs e)
        {
            MC.sendSomething();
        }

        private void BTN_TIC_Click(object sender, EventArgs e)
        {
            MC.sendToTIC();
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
            MC.Inlet.DoubleRange = CHB_INLET_x2.Checked;
            LBL_INLET_getVoltage.Text = MC.Inlet.getVoltage().ToString();
        }

        private void BTN_INLET_reset_Click(object sender, EventArgs e)
        {
            MC.Inlet.reset();
        }


        //-------------------------------Нагреватель------------------------------
        private void BTN_HEATER_setVoltage_Click(object sender, EventArgs e)
        {
            MC.Heater.setVoltage(TXB_HEATER_setVoltage.Text);
        }

        private void _BTN_HEATER_getVoltage_Click(object sender, EventArgs e)
        {
            MC.Heater.DoubleRange = CHB_INLET_x2.Checked;
            LBL_HEATER_getVoltage.Text = MC.Heater.getVoltage().ToString();
        }

        private void BTN_HEATER_reset_Click(object sender, EventArgs e)
        {
            MC.Heater.reset();
        }

        
        //-------------------------------Ионный источник--------------------------
        private void BTN_IonSOURCE_setEmissionCurrentVoltage_Click(object sender, EventArgs e)
        {
            MC.IonSource.EC_setVoltage(TXB_IonSOURCE_setEmissionCurrentVoltage.Text);
            //MC.DAC.setVoltage((byte)1, Convert.ToUInt16(TXB_IonSOURCE_setEmissionCurrentVoltage.Text));
        }

        private void BTN_IonSOURCE_setIonizationVoltage_Click(object sender, EventArgs e)
        {
            MC.IonSource.Ion_setVoltage(TXB_IonSOURCE_setIonizationVoltage.Text);
        }

        private void BTN_IonSOURCE_setF1voltage_Click(object sender, EventArgs e)
        {
            MC.IonSource.F1_setVoltage(TXB_IonSOURCE_setF1voltage.Text);
        }

        private void BTN_IonSOURCE_setF2voltage_Click(object sender, EventArgs e)
        {
            MC.IonSource.F2_setVoltage(TXB_IonSOURCE_setF2voltage.Text);
        }

        private void BTN_IonSOURCE_getEmissionCurrentVoltage_Click(object sender, EventArgs e)
        {
            MC.IonSource.DoubleRange = CHB_IonSOURCE_x2.Checked;
            LBL_IonSOURCE_getEmissionCurrentVoltage.Text = MC.IonSource.EC_getVoltage().ToString();
        }

        private void BTN_IonSOURCE_getIonizationVoltage_Click(object sender, EventArgs e)
        {
            MC.IonSource.DoubleRange = CHB_IonSOURCE_x2.Checked;
            LBL_IonSOURCE_getIonizationVoltage.Text = MC.IonSource.Ion_getVoltage().ToString();
        }

        private void BTN_IonSOURCE_getF1voltage_Click(object sender, EventArgs e)
        {
            MC.IonSource.DoubleRange = CHB_IonSOURCE_x2.Checked;
            LBL_IonSOURCE_getF1voltage.Text = MC.IonSource.F1_getVoltage().ToString();
        }

        private void BTN_IonSOURCE_getF2voltage_Click(object sender, EventArgs e)
        {
            MC.IonSource.DoubleRange = CHB_IonSOURCE_x2.Checked;
            LBL_IonSOURCE_getF2voltage.Text = MC.IonSource.F2_getVoltage().ToString();
        }

        private void BTN_IonSOURCE_reset_Click(object sender, EventArgs e)
        {
            MC.IonSource.reset();
        }
        //------------------------------------Детектор------------------------------------------
        private void BTN_DETECTOR_setDV1voltage_Click(object sender, EventArgs e)
        {
            MC.Detector.DV1_setVoltage(TXB_DETECTOR_setDV1voltage.Text);
        }
        private void BTN_DETECTOR_setDV2voltage_Click(object sender, EventArgs e)
        {
            MC.Detector.DV2_setVoltage(TXB_DETECTOR_setDV2voltage.Text);
        }

        private void BTN_DETECTOR_setDV3voltage_Click(object sender, EventArgs e)
        {
            MC.Detector.DV3_setVoltage(TXB_DETECTOR_setDV3voltage.Text);
        }

        private void BTN_DETECTOR_getDV1voltage_Click(object sender, EventArgs e)
        {
            MC.Detector.DoubleRange = CHB_DETECTOR_x2.Checked;
            LBL_DETECTOR_getDV1voltage.Text = MC.Detector.DV1_getVoltage().ToString();
        }

        private void BTN_DETECTOR_getDV2voltage_Click(object sender, EventArgs e)
        {
            MC.Detector.DoubleRange = CHB_DETECTOR_x2.Checked;
            LBL_DETECTOR_getDV2voltage.Text = MC.Detector.DV2_getVoltage().ToString();
        }

        private void BTN_DETECTOR_getDV3voltage_Click(object sender, EventArgs e)
        {
            MC.Detector.DoubleRange = CHB_DETECTOR_x2.Checked;
            LBL_DETECTOR_getDV3voltage.Text = MC.Detector.DV3_getVoltage().ToString();
        }

        private void BTN_DETECTOR_reset_Click(object sender, EventArgs e)
        {
            MC.Detector.reset();
        }

        private void BTN_checkCommandStack_Click(object sender, EventArgs e)
        {
            MC.checkCommandStack();
        }

        private void BTN_SCANER_setParentScanVoltage_Click(object sender, EventArgs e)
        {
            MC.Scaner.setParentScanVoltage(TXB_SCANER_setParentScanVoltage.Text);
        }

        private void BTN_SCANER_setScanVoltage_Click(object sender, EventArgs e)
        {
            MC.Scaner.setScanVoltage(TXB_SCANER_setScanVoltage.Text);
        }

        private void BTN_SCANER_getParentScanVoltage_Click(object sender, EventArgs e)
        {
            MC.Scaner.DoubleRange = CHB_SCANER_x2.Checked;
            LBL_SCANER_getParentScanVoltage.Text = MC.Scaner.getParentScanVoltage().ToString();
        }

        private void BTN_SCANER_getScanVoltage_Click(object sender, EventArgs e)
        {
            MC.Scaner.DoubleRange = CHB_SCANER_x2.Checked;
            LBL_SCANER_getScanVoltage.Text = MC.Scaner.getScanVoltage().ToString();
        }

        private void BTN_SCANER_reset_Click(object sender, EventArgs e)
        {
            MC.Scaner.reset();
        }

        private void BTN_CONDENSATOR_setVoltage_Click(object sender, EventArgs e)
        {
            MC.Condensator.setVoltage(TXB_CONDENSATOR_setVoltage.Text);
        }

        private void BTN_CONDENSATOR_getPositiveVoltage_Click(object sender, EventArgs e)
        {
            MC.Condensator.DoubleRange = CHB_CONDENSATOR_x2.Checked;
            LBL_CONDENSATOR_getPositiveVoltage.Text = MC.Condensator.getPositiveVoltage().ToString();
        }

        private void BTN_CONDENSATOR_getNegativeVoltage_Click(object sender, EventArgs e)
        {
            MC.Condensator.DoubleRange = CHB_CONDENSATOR_x2.Checked;
            LBL_CONDENSATOR_getNegativeVoltage.Text = MC.Condensator.getNegativeVoltage().ToString();
        }

        private void BTN_CONDENSATOR_reset_Click(object sender, EventArgs e)
        {
            MC.Condensator.reset();
        }

    }
}
