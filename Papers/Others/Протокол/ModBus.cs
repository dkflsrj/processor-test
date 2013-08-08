using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Timers;
using Flavor.Common.Commands;

namespace Flavor.Common {
    internal static class ModBus {
        internal class ModBusException: Exception { }

        internal enum CommandCode: byte {
            None = 0x00,
            //sync
            GetState = 0x01,
            GetStatus = 0x02,
            Shutdown = 0x03,
            Init = 0x04,
            SetHeatCurrent = 0x05,
            SetEmissionCurrent = 0x06,
            SetIonizationVoltage = 0x07,
            SetFocusVoltage1 = 0x08,
            SetFocusVoltage2 = 0x09,
            SetScanVoltage = 0x0A,
            SetCapacitorVoltage = 0x0B,
            Measure = 0x0C,
            GetCounts = 0x0D,
            //heatCurrentEnable = 0x0E,// отсюда нужно согласовывать
            //emissionCurrentEnable = 0x0F,
            heatCurrentEnable = 0x11,
            EnableHighVoltage = 0x14,
            GetTurboPumpStatus = 0x15,
            SetForvacuumLevel = 0x16,
            //syncerr
            InvalidCommand = 0x40,
            InvalidChecksum = 0x80,
            InvalidPacket = 0x81,
            InvalidLength = 0x82,
            InvalidData = 0x83,
            InvalidState = 0x84,
            //asyncerr
            InternalError = 0xC0,
            InvalidSystemState = 0xC1,
            VacuumCrash = 0xC2,//+ еще что-то..
            TurboPumpFailure = 0xC3,
            PowerFail = 0xC4,
            InvalidVacuumState = 0xC5,
            AdcPlaceIonSrc = 0xC8,
            AdcPlaceScanv = 0xC9,
            AdcPlaceControlm = 0xCA,
            //async
            Measured = 0xE0,
            VacuumReady = 0xE1,
            SystemShutdowned = 0xE2,
            SystemReseted = 0xE3,//+ еще что-то..
            HighVoltageOff = 0xE5,
            HighVoltageOn = 0xE6
        }

        private enum PacketingState {
            Idle,
            WaitUpper,
            WaitLower
        }

        internal enum PortStates {
            Closed,
            Opened,
            Closing,
            Opening,
            ErrorClosing,
            ErrorOpening
        }

        private static PacketingState PackState = PacketingState.Idle;

        private static List<byte> PacketBuffer = new List<byte>();

        private static byte UpperNibble;

        private static SerialPort _serialPort = null;

        private static List<byte[]> PacketReceived = new List<byte[]>();

        internal static string[] AvailablePorts {
            get { return SerialPort.GetPortNames(); }
        }
        
        internal static PortStates Open() {
            if (_serialPort != null) {
                if (_serialPort.IsOpen){
                    return PortStates.Opened;
                    //В лог: Уже открыт
                }
                _serialPort.Dispose();
            }
            _serialPort = new SerialPort();
            _serialPort.PortName = Config.Port;
            _serialPort.BaudRate = Config.BaudRate;
            _serialPort.DataBits = 8;
            _serialPort.Parity = System.IO.Ports.Parity.None;
            _serialPort.StopBits = System.IO.Ports.StopBits.One;
            _serialPort.ReadTimeout = 1000;
            _serialPort.WriteTimeout = 1000;

            try {
                _serialPort.Open();
            } catch (Exception Error) {
                System.Windows.Forms.MessageBox.Show(Error.Message, "Ошибка обращения к последовательному порту");
                return PortStates.ErrorOpening;
            }
            Receiving();
            return PortStates.Opening;
        }

        internal static PortStates Close() {
            if (_serialPort == null) {
                System.Windows.Forms.MessageBox.Show("Порт не инициализирован", "Ошибка обращения к последовательному порту");
                return PortStates.ErrorClosing;
            }
            if (!_serialPort.IsOpen) {
                return PortStates.Closed;
                //В лог Уже закрыт
            }
            try {
                StopReceiving();
                _serialPort.Close();
                _serialPort.Dispose();
                _serialPort = null;
            } catch (Exception Error) {
                System.Windows.Forms.MessageBox.Show(Error.Message, "Ошибка обращения к последовательному порту");
                return PortStates.ErrorClosing;
            }
            return PortStates.Closing;
        }

        internal static void Send(byte[] message) {
            try {
                _serialPort.Write(message, 0, message.Length);
            } catch {
                // BAD! consider revising
                ConsoleWriter.WriteLine("Error writing this command to serial port:");
                //throw new ModBusException();
            } finally {
                ConsoleWriter.Write("[out]");
                foreach (byte b in message) {
                    ConsoleWriter.Write((char)b);
                }
                ConsoleWriter.WriteLine();
            }
        }

        private static void Receiving() {
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(_serialPort_DataReceived);
        }

        private static void StopReceiving() {
            _serialPort.DataReceived -= new SerialDataReceivedEventHandler(_serialPort_DataReceived);
        }

        private static void _serialPort_DataReceived(object sender, EventArgs e) {
            SerialPort port = sender as SerialPort;
            while (port.IsOpen && port.BytesToRead > 0) {
                byte ch;
                try {
                    ch = (byte)port.ReadByte();
                } catch {
                    ConsoleWriter.WriteLine("Error(reading byte)");
                    continue;
                    // не получилось;
                }
                ConsoleWriter.Write((char)ch);
                DispatchByte(ch);
            }
            foreach (byte[] raw_command in PacketReceived) {
                //if (Commander.pState != Commander.programStates.Start)
                Commander.Realize(ModBus.Parse(raw_command));
            }
            PacketReceived.Clear();
        }

        private static void DispatchByte(byte data) {
            switch (ModBus.PackState) {
                case PacketingState.Idle: {
                        if (data == (byte)':') {
                            ModBus.PacketBuffer.Clear();
                            ModBus.PackState = PacketingState.WaitUpper;
                        } else {
                            ConsoleWriter.WriteLine("Error({0})", data);
                            //Symbol outside packet
                        }
                        break;
                    }
                case PacketingState.WaitUpper: {
                        if (data == 0x0d) {
                            ModBus.PacketReceived.Add((ModBus.PacketBuffer).ToArray());
                            ModBus.PacketBuffer.Clear();
                            ConsoleWriter.WriteLine();
                            ModBus.PackState = PacketingState.Idle;
                        } else {
                            ModBus.UpperNibble = GetInt(data);
                            ModBus.PackState = PacketingState.WaitLower;
                        }
                        break;
                    }
                case PacketingState.WaitLower: {
                        byte LowerNibble = GetInt(data);
                        LowerNibble |= (byte)(ModBus.UpperNibble << 4);
                        PacketBuffer.Add(LowerNibble);
                        ModBus.PackState = PacketingState.WaitUpper;
                        break;
                    }
            }
        }

        internal static ServicePacket Parse(byte[] raw_command) {
            ///<summary> CS проверка <summary>
            if (raw_command.Length >= 2) {
                if (checkCS(raw_command)) {
                    //ConsoleWriter.WriteLine("Контрольная сумма в порядке");
                    ///<summary> Отделяем функциональный код команды,
                    ///отталкиваясь от него принимаем решение о создании команды
                    ///<summary>
                    switch ((CommandCode)raw_command[0]) {
                        case CommandCode.GetState:
                            if (raw_command.Length == 3) {
                                return new SyncReply.updateState(raw_command[1]);
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.GetStatus:
                            if (raw_command.Length == 29) {
                                return new SyncReply.updateStatus(raw_command[1],
                                                        raw_command[2],
                                                        (ushort)((ushort)raw_command[3] + ((ushort)raw_command[4] << 8)),
                                                        (ushort)((ushort)raw_command[5] + ((ushort)raw_command[6] << 8)),
                                                        (ushort)((ushort)raw_command[7] + ((ushort)raw_command[8] << 8)),
                                                        (ushort)((ushort)raw_command[9] + ((ushort)raw_command[10] << 8)),
                                                        (ushort)((ushort)raw_command[11] + ((ushort)raw_command[12] << 8)),
                                                        (ushort)((ushort)raw_command[13] + ((ushort)raw_command[14] << 8)),
                                                        (ushort)((ushort)raw_command[15] + ((ushort)raw_command[16] << 8)),
                                                        (ushort)((ushort)raw_command[17] + ((ushort)raw_command[18] << 8)),
                                                        (ushort)((ushort)raw_command[19] + ((ushort)raw_command[20] << 8)),
                                                        (ushort)((ushort)raw_command[21] + ((ushort)raw_command[22] << 8)),
                                                        (ushort)((ushort)raw_command[23] + ((ushort)raw_command[24] << 8)),
                                                        raw_command[25],
                                    //raw_command[26], 
                                                        (ushort)((ushort)raw_command[26] + ((ushort)raw_command[27] << 8)));
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.Shutdown:
                            if (raw_command.Length == 2) {
                                return new SyncReply.confirmShutdown();
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.Init:
                            if (raw_command.Length == 2) {
                                return new SyncReply.confirmInit();
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.SetHeatCurrent:
                            if (raw_command.Length == 2) {
                                return new SyncReply.confirmHCurrent();
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.SetEmissionCurrent:
                            if (raw_command.Length == 2) {
                                return new SyncReply.confirmECurrent();
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.SetIonizationVoltage:
                            if (raw_command.Length == 2) {
                                return new SyncReply.confirmIVoltage();
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.SetFocusVoltage1:
                            if (raw_command.Length == 2) {
                                return new SyncReply.confirmF1Voltage();
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.SetFocusVoltage2:
                            if (raw_command.Length == 2) {
                                return new SyncReply.confirmF2Voltage();
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.SetScanVoltage:
                            if (raw_command.Length == 2) {
                                return new SyncReply.confirmSVoltage();
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.SetCapacitorVoltage:
                            if (raw_command.Length == 2) {
                                return new SyncReply.confirmCP();
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.Measure:
                            if (raw_command.Length == 2) {
                                return new SyncReply.confirmMeasure();
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.GetCounts:
                            if (raw_command.Length == 8) {
                                return new SyncReply.updateCounts((int)raw_command[1] + ((int)raw_command[2] << 8) + ((int)raw_command[3] << 16),
                                                        (int)raw_command[4] + ((int)raw_command[5] << 8) + ((int)raw_command[6] << 16),
                                                        delegate {
                                                            if (Commander.CurrentMeasureMode == null) {
                                                                // fake packet. BAD solution
                                                                return;
                                                            }
                                                            // Not the best place for automatic refresh!
                                                            // move further to Commander
                                                            Commander.CurrentMeasureMode.updateGraph();});
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.heatCurrentEnable:
                            if (raw_command.Length == 2) {
                                return new SyncReply.confirmHECurrent();
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.EnableHighVoltage:
                            if (raw_command.Length == 2) {
                                return new SyncReply.confirmHighVoltage();
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.GetTurboPumpStatus:
                            if (raw_command.Length == 17) {
                                return new SyncReply.updateTurboPumpStatus((ushort)((ushort)raw_command[1] + ((ushort)raw_command[2] << 8)),
                                                        (ushort)((ushort)raw_command[3] + ((ushort)raw_command[4] << 8)),
                                                        (ushort)((ushort)raw_command[5] + ((ushort)raw_command[6] << 8)),
                                                        (ushort)((ushort)raw_command[7] + ((ushort)raw_command[8] << 8)),
                                                        (ushort)((ushort)raw_command[9] + ((ushort)raw_command[10] << 8)),
                                                        (ushort)((ushort)raw_command[11] + ((ushort)raw_command[12] << 8)),
                                                        raw_command[13],
                                                        raw_command[14],
                                                        raw_command[15]);
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.SetForvacuumLevel:
                            if (raw_command.Length == 2) {
                                return new SyncReply.confirmForvacuumLevel();
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.InvalidCommand:
                            byte[] tempArray = new byte[raw_command.Length - 1];
                            raw_command.CopyTo(tempArray, 1);
                            return new SyncErrorReply.logInvalidCommand(tempArray);
                        case CommandCode.InvalidChecksum:
                            if (raw_command.Length == 2) {
                                return new SyncErrorReply.logInvalidChecksum();
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.InvalidPacket:
                            if (raw_command.Length == 2) {
                                return new SyncErrorReply.logInvalidPacket();
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.InvalidLength:
                            if (raw_command.Length == 2) {
                                return new SyncErrorReply.logInvalidLength();
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.InvalidData:
                            if (raw_command.Length == 2) {
                                return new SyncErrorReply.logInvalidData();
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.InvalidState:
                            if (raw_command.Length == 2) {
                                return new SyncErrorReply.logInvalidState();
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.InternalError:
                            if (raw_command.Length == 3) {
                                return new AsyncErrorReply.logInternalError(raw_command);
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.InvalidSystemState:
                            if (raw_command.Length == 2) {
                                return new AsyncErrorReply.logInvalidSystemState(raw_command);
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.VacuumCrash:
                            if (raw_command.Length == 3) {
                                return new AsyncErrorReply.logVacuumCrash(raw_command);
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.TurboPumpFailure:
                            if (raw_command.Length == 17) {
                                return new AsyncErrorReply.logTurboPumpFailure(raw_command);
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.PowerFail:
                            if (raw_command.Length == 2) {
                                return new AsyncErrorReply.logPowerFail(raw_command);
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.InvalidVacuumState:
                            if (raw_command.Length == 2) {
                                return new AsyncErrorReply.logInvalidVacuumState(raw_command);
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.AdcPlaceIonSrc:
                            //!!!
                            if (raw_command.Length >= 2) {
                                return new AsyncErrorReply.logAdcPlaceIonSrc(raw_command);
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.AdcPlaceScanv:
                            //!!!
                            if (raw_command.Length >= 2) {
                                return new AsyncErrorReply.logAdcPlaceScanv(raw_command);
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.AdcPlaceControlm:
                            //!!!
                            if (raw_command.Length >= 2) {
                                return new AsyncErrorReply.logAdcPlaceControlm(raw_command);
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.Measured:
                            if (raw_command.Length == 2) {
                                return new AsyncReply.requestCounts();
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.VacuumReady:
                            if (raw_command.Length == 2) {
                                return new AsyncReply.confirmVacuumReady();
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.SystemShutdowned:
                            if (raw_command.Length == 2) {
                                return new AsyncReply.confirmShutdowned();
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.SystemReseted:
                            if (raw_command.Length == 2) {
                                return new AsyncReply.SystemReseted();
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.HighVoltageOff:
                            if (raw_command.Length == 2) {
                                return new AsyncReply.confirmHighVoltageOff();
                            }
                            return ServicePacket.ZERO;
                        case CommandCode.HighVoltageOn:
                            if (raw_command.Length == 2) {
                                return new AsyncReply.confirmHighVoltageOn();
                            }
                            return ServicePacket.ZERO;
                        default:
                            ConsoleWriter.WriteLine("Неверная команда");
                            return ServicePacket.ZERO;
                    }
                } else {
                    ConsoleWriter.WriteLine("Неверная контрольная сумма");
                    return ServicePacket.ZERO;
                }
            } else {
                ConsoleWriter.WriteLine("Короткий пакет");
                return ServicePacket.ZERO;
            }
        }

        private static byte ComputeChecksum(byte[] data) {
            byte checkSum = 0;
            for (int i = 0; i < data.Length; i++) {
                checkSum -= data[i];
            }
            return checkSum;
        }

        private static bool checkCS(byte[] data) {
            return true ^ Convert.ToBoolean(ComputeChecksum(data));
        }

        internal static byte[] buildPack(byte[] data) {
            List<byte> pack = new List<byte>();
            pack.Add((byte)':');
            buildPackBody(pack, data);
            pack.Add((byte)'\r');
            return pack.ToArray();
        }
        internal static void buildPackBody(List<byte> pack, byte[] data) {
            for (int i = 0; i < data.Length; i++) {
                pack.Add(GetNibble(data[i] >> 4));
                pack.Add(GetNibble(data[i]));
            }
            byte cs = ComputeChecksum(data);
            pack.Add(GetNibble(cs >> 4));
            pack.Add(GetNibble(cs));
        }

        internal static byte[] collectData(byte functCode) {
            return new byte[] { functCode };
        }
        internal static byte[] collectData(byte functCode, byte value) {
            return new byte[] { functCode, value };
        }
        internal static byte[] collectData(byte functCode, ushort value) {
            byte[] data = ushort2ByteArray(value);
            return new byte[] { functCode, data[0], data[1] };
        }
        internal static byte[] collectData(byte functCode, ushort value1, ushort value2) {
            byte[] data1 = ushort2ByteArray(value1);
            byte[] data2 = ushort2ByteArray(value2);
            return new byte[] { functCode, data1[0], data1[1], data2[0], data2[1] };
        }
        internal static byte[] collectData(byte functCode, int value1, int value2) {
            List<byte> Data = new List<byte>();
            Data.Add(functCode);
            Data.AddRange(int2ByteArray(value1));
            Data.AddRange(int2ByteArray(value2));
            return Data.ToArray();
        }

        internal static byte[] ushort2ByteArray(ushort value) {
            if (value < 0) value = 0;
            if (value > 4095) value = 4095;
            return new byte[] { (byte)(value), (byte)(value >> 8) };
        }

        internal static byte[] int2ByteArray(int value) {
            if (value < 0) value = 0;
            if (value > 16777215) value = 16777215;
            return new byte[] { (byte)(value), (byte)(value >> 8), (byte)(value >> 16) };
        }

        private static byte GetNibble(int data) {
            data &= 0x0F;
            if (data < 10) {
                return (byte)(data + (int)'0');
            } else {
                return (byte)(data + (int)'a' - 10);
            }
        }

        private static byte GetInt(int data) {
            if (data >= (byte)'0' && data <= (int)'9') {
                return (byte)(data - (int)'0');
            }
            if (data >= (byte)'a' && data <= (int)'f') {
                return (byte)(data - (int)'a' + 10);
            }
            if (data >= (byte)'A' && data <= (int)'F') {
                return (byte)(data - (int)'A' + 10);
            }
            return 0;
        }
    }
}
