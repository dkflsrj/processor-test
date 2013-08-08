namespace Flavor.Common.Commands {
    internal abstract class AsyncErrorReply: ServicePacket {
        private byte[] cmdln;
        internal AsyncErrorReply(byte[] commandline) {
            cmdln = commandline;
            Config.logCrash(cmdln);
        }
        internal virtual string errorMessage {
            get { return "Async error reply"; }
        }

        internal class logInternalError: AsyncErrorReply {
            internal override string errorMessage {
                get { return "Internal error " + internalError.ToString(); }
            }

            private byte internalError;

            internal logInternalError(byte[] commandline)
                : base(commandline) {
                internalError = commandline[1];
            }
        }

        internal class logInvalidSystemState: AsyncErrorReply {
            internal override string errorMessage {
                get { return "Wrong system state"; }
            }
            internal logInvalidSystemState(byte[] commandline) : base(commandline) { }
        }

        internal class logVacuumCrash: AsyncErrorReply {
            internal override string errorMessage {
                get { return "Vacuum crash state " + vacState.ToString(); }
            }

            byte vacState;
            internal logVacuumCrash(byte[] commandline)
                : base(commandline) {
                vacState = commandline[1];
            }
        }

        internal class logTurboPumpFailure: AsyncErrorReply, IUpdateDevice {
            private ushort turboSpeed;
            private ushort turboCurrent;
            private ushort pwm;
            private ushort pumpTemp;
            private ushort driveTemp;
            private ushort operationTime;
            private byte v1;
            private byte v2;
            private byte v3;

            internal override string errorMessage {
                get { return "Turbopump failure"; }
            }
            internal logTurboPumpFailure(byte[] commandline)
                : base(commandline) {
                turboSpeed = (ushort)((ushort)commandline[1] + ((ushort)commandline[2] << 8));
                turboCurrent = (ushort)((ushort)commandline[3] + ((ushort)commandline[4] << 8));
                pwm = (ushort)((ushort)commandline[5] + ((ushort)commandline[6] << 8));
                pumpTemp = (ushort)((ushort)commandline[7] + ((ushort)commandline[8] << 8));
                driveTemp = (ushort)((ushort)commandline[9] + ((ushort)commandline[10] << 8));
                operationTime = (ushort)((ushort)commandline[11] + ((ushort)commandline[12] << 8));
                v1 = commandline[13];
                v2 = commandline[14];
                v3 = commandline[15];
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
        }

        internal class logPowerFail: AsyncErrorReply {
            internal override string errorMessage {
                get { return "Device power fail"; }
            }
            internal logPowerFail(byte[] commandline) : base(commandline) { }
        }

        internal class logInvalidVacuumState: AsyncErrorReply {
            internal override string errorMessage {
                get { return "Wrong vacuum state"; }
            }
            internal logInvalidVacuumState(byte[] commandline) : base(commandline) { }
        }

        internal class logAdcPlaceIonSrc: AsyncErrorReply {
            internal override string errorMessage {
                get { return "AdcPlaceIonSrc"; }
            }
            internal logAdcPlaceIonSrc(byte[] commandline) : base(commandline) { }
        }

        internal class logAdcPlaceScanv: AsyncErrorReply {
            internal override string errorMessage {
                get { return "AdcPlaceScanv"; }
            }
            internal logAdcPlaceScanv(byte[] commandline) : base(commandline) { }
        }

        internal class logAdcPlaceControlm: AsyncErrorReply {
            internal override string errorMessage {
                get { return "AdcPlaceControlm"; }
            }
            internal logAdcPlaceControlm(byte[] commandline) : base(commandline) { }
        }
    }
}
