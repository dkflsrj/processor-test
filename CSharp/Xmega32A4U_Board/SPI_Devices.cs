using System;
using System.Collections.Generic;

namespace Xmega32A4U_testBoard
{
    #region SPI каналы
    #region DAC
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
                MC.Service.trace_attached(Environment.NewLine);
                MC.Service.trace(_command);
                byte[] bytes = BitConverter.GetBytes(VOLTAGE);
                byte[] data = { Convert.ToByte((CHANNEL - 1) * 16 + bytes[1]), bytes[0] };
                List<byte> rDATA = MC.Service.transmit(command, data);
                try
                {
                    if (rDATA[0] == DAC_command)
                    {
                        MC.Service.trace(_command + ": Операция выполнена успешно!");
                        return true;
                    }
                }
                catch { MC.Service.addError(_command + ": Ошибка данных!", rDATA); return false; }
                MC.Service.addError(_command + ": ОШИБКА ОТКЛИКА!");
            }
            MC.Service.trace(_command + ": Операция отменена! Значение VOLTAGE ожидалось от 0 до 4095!");
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
            catch { MC.Service.trace("DAC_CHANNEL.setVoltage():Неверное значение!"); return false; }

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
            catch { MC.Service.trace("DAC_CHANNEL.setVoltage():Неверное значение!"); return false; }
        }
    }
    #endregion
    #region ADC
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
            MC.Service.trace_attached(Environment.NewLine);
            byte[] data = { Convert.ToByte(Hbyte + ChannelStep * CHANNEL), Lbyte_DoubleRange };
            List<byte> rDATA = MC.Service.transmit(command, data);
            ushort voltage = 0;
            byte adress = 1;
            try
            {
                if (rDATA[0] != command)
                {
                    MC.Service.addError(_command + ": Ошибка отклика!", rDATA);
                    return ushort.MaxValue;
                }
                adress += Convert.ToByte(rDATA[1] >> 4);
                voltage = Convert.ToUInt16((Convert.ToUInt16(rDATA[1] & 0xf) << 8) + rDATA[2]);
            }
            catch { MC.Service.addError(_command + ": Ошибка данных!", rDATA); return ushort.MaxValue; }
            MC.Service.trace(_command + ": Ответный адрес канала: " + adress);
            MC.Service.trace(_command + ": Напряжение: " + voltage);
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
    #endregion
    #endregion
    #region Конденсатор
    public class SPI_CONDENSATOR
    {
        //КЛАСС: Каналы для конденсатора (+\-)
        //DAC AD5643R
        byte[] ConfInnerRef_bytes = { 56, 0, 1 }; //старший, средний, младший
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
                MC.Service.trace(command + ": AD5643R;");
                //посылаем напряжение
                int voltage = VOLTAGE;
                voltage = voltage << 2;
                byte[] bytes = BitConverter.GetBytes(voltage);
                byte Hbyte = Convert.ToByte(DAC_channel);
                byte Mbyte = bytes[1];
                byte Lbyte = bytes[0];
                byte[] data = new byte[] { Hbyte, Mbyte, Lbyte };
                List<byte> rDATA = MC.Service.transmit(Command.SPI.Condensator.setVoltage, data);
                try
                {
                    if (rDATA[0] == Command.SPI.Condensator.setVoltage)
                    {
                        MC.Service.trace(command + ": Операция выполнена успешно!");
                        return true;
                    }
                    MC.Service.addError(command + ": ОШИБКА ОТКЛИКА!");
                    return false;
                }
                catch { MC.Service.addError(command + ": Ошибка данных!", rDATA); return false; }
            }
            MC.Service.trace(command + ": Операция отменена! Значение VOLTAGE ожидалось от 0 до 16383!");
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
            MC.Service.trace_attached(Environment.NewLine);

            byte[] data = { Convert.ToByte(ADC_Hbyte + ChannelStep * CHANNEL), ADC_Lbyte_DoubleRange };
            List<byte> rDATA = MC.Service.transmit(command, data);
            ushort voltage = 0;
            byte adress = 0;
            try
            {
                if (rDATA[0] != command)
                {
                    MC.Service.addError(_command + ": Ошибка отклика!", rDATA);
                    return ushort.MaxValue;
                }
                adress += Convert.ToByte(rDATA[1] >> 4);
                voltage = Convert.ToUInt16((Convert.ToUInt16(rDATA[1] & 0xf) << 8) + rDATA[2]);
            }
            catch { MC.Service.addError(command + ": Ошибка данных!", rDATA); return ushort.MaxValue; }
            MC.Service.trace(_command + ": Ответный адрес канала: " + adress);
            MC.Service.trace(_command + ": Напряжение: " + voltage);
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
            catch { MC.Service.trace("DAC_CHANNEL.setVoltage():Неверное значение!"); return false; }
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
            catch { MC.Service.trace("DAC_CHANNEL.setVoltage():Неверное значение!"); return false; }
        }
    }
    #endregion
    #region Ионный Источник
    public class SPI_IonSOURCE
    {
        //КЛАСС: Ионный источник - используется каналы А,B,C,D
        //DAC AD5328BR
        const byte EmissionCurrent_channel = 1;
        const byte Ionization_channel = 2;
        const byte F1_channel = 3;
        const byte F2_channel = 4;
        //Каналы
        /// <summary>
        /// Напряжение тока эмиссии
        /// </summary>
        public SPI_DEVICE_CHANNEL EmissionCurrent = new SPI_DEVICE_CHANNEL(EmissionCurrent_channel, Command.SPI.PSIS.setVoltage, EmissionCurrent_channel, Command.SPI.PSIS.getVoltage);
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
    #endregion
    #region Детектор
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
    #endregion
    #region Сканер
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
                    MC.Service.trace_attached(Environment.NewLine);
                    MC.Service.trace(command + ": AD5643R;");
                    //посылаем напряжение
                    int voltage = VOLTAGE;
                    voltage = voltage << 2;
                    byte[] bytes = BitConverter.GetBytes(voltage);
                    byte Hbyte = Convert.ToByte(DAC_channel);
                    byte Mbyte = bytes[1];
                    byte Lbyte = bytes[0];
                    byte[] data = new byte[] { Hbyte, Mbyte, Lbyte };
                    List<byte> rDATA = MC.Service.transmit(DAC_command, data);
                    try
                    {
                        if (rDATA[0] == DAC_command)
                        {
                            MC.Service.trace(command + ": Операция выполнена успешно!");
                            return true;
                        }
                        MC.Service.addError(command + ": ОШИБКА ОТКЛИКА!");
                        return false;
                    }
                    catch { MC.Service.addError(command + ": Ошибка данных!", rDATA); return false; }
                }
                MC.Service.trace(command + ": Операция отменена! Значение VOLTAGE ожидалось от 0 до 16383!");
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
                MC.Service.trace_attached(Environment.NewLine);
                MC.Service.trace(_command);

                byte[] data = { Convert.ToByte(ADC_Hbyte + ChannelStep * CHANNEL), ADC_Lbyte_DoubleRange };
                List<byte> rDATA = MC.Service.transmit(command, data);
                ushort voltage = 0;
                byte address = 0;
                try
                {
                    if (rDATA[0] != command)
                    {
                        MC.Service.addError(_command + ": Ошибка отклика!", rDATA);
                        return ushort.MaxValue;
                    }
                    address += Convert.ToByte(rDATA[1] >> 4);
                    voltage = Convert.ToUInt16((Convert.ToUInt16(rDATA[1] & 0xf) << 8) + rDATA[2]);
                }
                catch { MC.Service.addError(command + ": Ошибка данных!", rDATA); return ushort.MaxValue; }
                MC.Service.trace(_command + ": Ответный адрес канала: " + address);
                MC.Service.trace(_command + ": Напряжение: " + voltage);
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
    #endregion
}