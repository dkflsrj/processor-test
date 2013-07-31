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

namespace Xmega32A4U_testBoard
{
    public partial class Form1 : Form
    {
        SerialPort COM_Port;
        XMEGA32A4U MC;

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
        /*private void COM_Port_send(byte DATA)
        {
            byte[] BYTE = {DATA};
            COM_Port.Open();
            COM_Port.Write(BYTE,0,1);
            COM_Port.Close();
        }*/
        /*private byte COM_Port_read()
        {
            byte[] DATA = { 0 };
            COM_Port.Open();
            try
            {
                COM_Port.Read(DATA, 0, 1);
                COM_Port.Close();
                return DATA[0];
            }
            catch (Exception)
            {
                trace(true, "ОШИБКА ПРИЁМА ДАННЫХ! ");
                COM_Port.Close();
                return DATA[0];
            }

        }*/
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
            MC.setMCwait();
        }
        private void BTN_LEDbyte_Click(object sender, EventArgs e)
        {
            MC.showMeByte(TXB_LEDbyte.Text);
        }
        private void BTN_SPI_DAC_send_Click(object sender, EventArgs e)
        {
            MC.DAC.setVoltage(TXB_DAC_channel.Text, TXB_DAC_voltage.Text);
        }
        private void BTN_SPI_ADC_request_Click(object sender, EventArgs e)
        {
            MC.ADC.DoubleRange = CHB_ADC_DoubleRange.Checked;
            MC.ADC.getVoltage(TXB_ADC_channel.Text);
        }
        private void BTN_DAC_reset_Click(object sender, EventArgs e)
        {
            MC.DAC.reset();
        }

        private void CHB_COM_DC_sendDATA_CheckedChanged(object sender, EventArgs e)
        {
            if (CHB_COM_DC_sendDATA.Checked)
            {
                TXB_COM_DC_sendDATA.Enabled = true;
            }
            else
            {
                TXB_COM_DC_sendDATA.Enabled = false;
            }
        }
        private void CHB_COM_DC_recieveDATA_CheckedChanged(object sender, EventArgs e)
        {
            if (CHB_COM_DC_recieveDATA.Checked)
            {
                TXB_COM_DC_recieveDATA_count.Enabled = true;
            }
            else
            {
                TXB_COM_DC_recieveDATA_count.Enabled = false;
            }
        }
        private void BTN_DirectCommand_Click(object sender, EventArgs e)
        {
            if (TXB_COM_DC_command.Text != "")
            {
                if (CHB_COM_DC_sendDATA.Checked && CHB_COM_DC_recieveDATA.Checked && CHB_COM_DC_recieveResponse.Checked)
                {
                    trace(true, "Прямая команда с данными, откликом и ответом");
                    //MC.directCommand(TXB_COM_DC_command, DataSet, TXB_COM_DC_recieveDATA_count, true);
                }
                else if (CHB_COM_DC_sendDATA.Checked && CHB_COM_DC_recieveDATA.Checked)
                {
                    trace(true, "Прямая команда с данными и ответом");
                }
                else if (CHB_COM_DC_recieveDATA.Checked && CHB_COM_DC_recieveResponse.Checked)
                {
                    trace(true, "Прямая команда с откликом и ответом");
                }
                else if (CHB_COM_DC_sendDATA.Checked && CHB_COM_DC_recieveResponse.Checked)
                {
                    trace(true, "Прямая команда с данными и откликом");
                }
                else if (CHB_COM_DC_sendDATA.Checked)
                {
                    trace(true, "Прямая команда с данными");
                    string wDATAs = TXB_COM_DC_sendDATA.Text;

                    string wdata = "";
                    byte i = 0;
                    foreach (char ch in wDATAs)
                    {
                        if (ch == ' ')
                        {
                            i++;
                        }
                    }
                    byte[] wDATA = new byte[i];
                    i = 0;
                    foreach (char ch in wDATAs)
                    {
                        if (ch != ' ')
                        {
                            wdata += ch;
                        }
                        else
                        {
                            wDATA[i] = Convert.ToByte(wdata);
                            i++;
                            wdata = "";
                        }
                    }
                    MC.directCommand(Convert.ToByte(TXB_COM_DC_command.Text), wDATA);
                }
                else if (CHB_COM_DC_recieveResponse.Checked)
                {
                    trace(true, "Приказ");
                }
                else if (CHB_COM_DC_recieveDATA.Checked)
                {
                    trace(true, "Запрос");
                    trace(true, MC.directCommand(10, 4).ToString());
                }
                else
                {
                    MC.directCommand(Convert.ToByte(TXB_COM_DC_command.Text));
                    trace(true, "Директива");
                }

            }
            else
            {
                trace(true, "Прямая команда не выполнена! Введите номер команды!");
            }
        }

        private void BTN_startCounter_Click(object sender, EventArgs e)
        {
            //ФУНКЦИЯ: Запускаем счётчик МК
            MC.COA.start();
        }
        private void BTN_reqCount_Click(object sender, EventArgs e)
        {
            //ФУНКЦИЯ: Проверям счётчик МК, если сосчитал, то принимаем результат
            trace(true, "   Счёт: " + MC.COA.getResult().ToString());
        }
        private void BTN_setInterval_Click(object sender, EventArgs e)
        {
            //ФУНКЦИЯ: Задаёт временной интервал счёта в миллисекундах [1...130]
            MC.COA.setTimeInterval(TXB_interval.Text);
        }
        private void BTN_stopCounter_Click(object sender, EventArgs e)
        {
            MC.COA.stop();
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
                    default:
                        break;
                }
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
                CHB_TotalControl.ForeColor = System.Drawing.Color.Red;
                CHB_TotalControl.Text = "Выключен";
                LBL_TotalC_Status.Text = "Неизвестно!";
                LBL_TotalC_Status.ForeColor = System.Drawing.Color.Red;
                CLK_timer.Enabled = false;
            }
        }
        private void TXB_interval_TextChanged(object sender, EventArgs e)
        {
            LBL_COA_ticks.Text = MC.RTC.getTicks(TXB_interval.Text).ToString();
            LBL_COA_frequency.Text = MC.RTC.getFreqency().ToString();
            LBL_COA_prescaler.Text = MC.RTC.getPrescaler().ToString();
        }
    }
}
