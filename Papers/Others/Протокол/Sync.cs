namespace Flavor.Common.Commands {
    abstract class SyncReply: ServicePacket.Sync {
        public class updateState: SyncReply, IUpdateDevice, IAutomatedReply {
            private byte sysState;

            public updateState(byte value) {
                sysState = value;
            }

            #region IUpdateDevice Members
            public void UpdateDevice() {
                Device.sysState = sysState;
            }
            #endregion

            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.GetState; }
            }

            #region IAutomatedReply Members
            public void AutomatedReply() {
                Commander.AddToSend(new UserRequest.requestStatus());
            }
            #endregion
        }
        public class updateStatus: SyncReply, IUpdateDevice {

            private byte sysState;
            private byte vacState;
            private ushort fVacuum;
            private ushort hVacuum;
            private ushort hCurrent;
            private ushort eCurrent;
            private ushort iVoltage;
            private ushort fV1;
            private ushort fV2;
            private ushort sVoltage;
            private ushort cVPlus;
            private ushort cVMin;
            private ushort dVoltage;
            private byte relaysStates;
            //private byte relaysStates2;
            private ushort turboSpeed;

            internal updateStatus(byte value1, byte value2, ushort value3, ushort value4, ushort value5, ushort value6, ushort value7, ushort value8, ushort value9, ushort value10, ushort value11, ushort value12, ushort value13, byte value14, /*byte value15,*/ ushort value16) {
                sysState = value1;
                vacState = value2;
                fVacuum = value3;
                hVacuum = value4;
                hCurrent = value5;
                eCurrent = value6;
                iVoltage = value7;
                fV1 = value8;
                fV2 = value9;
                sVoltage = value10;
                cVPlus = value11;
                cVMin = value12;
                dVoltage = value13;
                relaysStates = value14;
                //relaysStates2 = value15;
                turboSpeed = value16;
            }

            #region IUpdateDevice Members

            public void UpdateDevice() {
                Device.sysState = sysState;
                Device.vacState = vacState;
                Device.fVacuum = fVacuum;
                Device.hVacuum = hVacuum;
                Device.DeviceCommonData.hCurrent = hCurrent;
                Device.DeviceCommonData.eCurrent = eCurrent;
                Device.DeviceCommonData.iVoltage = iVoltage;
                Device.DeviceCommonData.fV1 = fV1;
                Device.DeviceCommonData.fV2 = fV2;
                Device.DeviceCommonData.sVoltage = sVoltage;
                Device.DeviceCommonData.cVPlus = cVPlus;
                Device.DeviceCommonData.cVMin = cVMin;
                Device.DeviceCommonData.dVoltage = dVoltage;
                Device.TurboPump.Speed = turboSpeed;
                Device.relaysState(relaysStates/*, relaysStates2*/);
            }

            #endregion

            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.GetStatus; }
            }
        }
        public class confirmShutdown: SyncReply {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.Shutdown; }
            }
        }
        public class confirmInit: SyncReply, IAutomatedReply {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.Init; }
            }

            #region IReply Members

            public void AutomatedReply() {
                Commander.AddToSend(new UserRequest.requestStatus());
            }

            #endregion
        }
        public class confirmHCurrent: SyncReply, IAutomatedReply {
            #region IReply Members
            public void AutomatedReply() {
                Commander.AddToSend(new UserRequest.sendF1Voltage());
            }
            #endregion

            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.SetHeatCurrent; }
            }
        }
        public class confirmECurrent: SyncReply, IAutomatedReply {
            #region IReply Members
            public void AutomatedReply() {
                Commander.AddToSend(new UserRequest.sendHCurrent());
            }
            #endregion

            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.SetEmissionCurrent; }
            }
        }
        public class confirmIVoltage: SyncReply, IAutomatedReply {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.SetIonizationVoltage; }
            }

            #region IReply Members
            public void AutomatedReply() {
                Commander.AddToSend(new UserRequest.sendCapacitorVoltage());
                ;
            }
            #endregion
        }
        public class confirmF1Voltage: SyncReply, IAutomatedReply {
            #region IReply Members
            public void AutomatedReply() {
                Commander.AddToSend(new UserRequest.sendF2Voltage());
            }
            #endregion

            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.SetFocusVoltage1; }
            }
        }
        public class confirmF2Voltage: SyncReply {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.SetFocusVoltage2; }
            }
        }
        public class confirmSVoltage: SyncReply, IAutomatedReply {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.SetScanVoltage; }
            }

            #region IAutomatedReply Members
            public void AutomatedReply() {
                if (Commander.CurrentMeasureMode != null && Commander.CurrentMeasureMode.isOperating) {
                    Commander.CurrentMeasureMode.autoNextMeasure();
                }
            }
            #endregion
        }
        public class confirmCP: SyncReply, IAutomatedReply {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.SetCapacitorVoltage; }
            }

            #region IReply Members
            public void AutomatedReply() {
                Commander.AddToSend(new UserRequest.enableHCurrent());
            }
            #endregion
        }
        public class confirmMeasure: SyncReply {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.Measure; }
            }
        }
        public class updateCounts: SyncReply, IUpdateDevice, IUpdateGraph {
            private int Detector1;
            private int Detector2;

            internal updateCounts(int value1, int value2, Action graphupdate) {
                Detector1 = value1;
                Detector2 = value2;
                updateGraph = graphupdate;
            }

            #region IUpdateDevice Members

            public void UpdateDevice() {
                Device.Detector1 = Detector1;
                Device.Detector2 = Detector2;
            }

            #endregion

            #region IUpdateGraph Members

            public delegate void Action();
            private readonly Action updateGraph;
            public void UpdateGraph() {
                updateGraph();
                /*if (Commander.CurrentMeasureMode == null) {
                    // fake packet. BAD solution
                    return;
                }
                // Not the best place for automatic refresh!
                Commander.CurrentMeasureMode.updateGraph();*/
            }

            #endregion

            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.GetCounts; }
            }
        }
        public class confirmHECurrent: SyncReply, IAutomatedReply {
            #region IReply Members
            public void AutomatedReply() {
                Commander.AddToSend(new UserRequest.sendECurrent());
            }
            #endregion

            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.heatCurrentEnable; }
            }
        }
        public class confirmHighVoltage: SyncReply {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.EnableHighVoltage; }
            }
        }
        public class updateTurboPumpStatus: SyncReply, IUpdateDevice {
            private ushort turboSpeed;
            private ushort turboCurrent;
            private ushort pwm;
            private ushort pumpTemp;
            private ushort driveTemp;
            private ushort operationTime;
            private byte v1;
            private byte v2;
            private byte v3;

            internal updateTurboPumpStatus(ushort value1, ushort value2, ushort value3, ushort value4, ushort value5, ushort value6, byte value7, byte value8, byte value9) {
                turboSpeed = value1;
                turboCurrent = value2;
                pwm = value3;
                pumpTemp = value4;
                driveTemp = value5;
                operationTime = value6;
                v1 = value7;
                v2 = value8;
                v3 = value9;
            }

            #region IUpdateDevice Members

            public void UpdateDevice() {
                Device.TurboPump.Speed = turboSpeed;
                Device.TurboPump.Current = turboCurrent;
                Device.TurboPump.pwm = pwm;
                Device.TurboPump.PumpTemperature = pumpTemp;
                Device.TurboPump.DriveTemperature = driveTemp;
                Device.TurboPump.OperationTime = operationTime;
                Device.TurboPump.relaysState(v1, v2, v3);
            }

            #endregion

            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.GetTurboPumpStatus; }
            }
        }
        public class confirmForvacuumLevel: SyncReply {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.SetForvacuumLevel; }
            }
        }
    }

}