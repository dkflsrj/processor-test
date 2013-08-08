namespace Flavor.Common.Commands {
    internal abstract class SyncErrorReply: ServicePacket.Sync {
        internal class logInvalidCommand: SyncErrorReply {
            //private byte[] command;

            internal logInvalidCommand(byte[] errorcommand) {
                //command = errorcommand;
            }

            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.InvalidCommand; }
            }
        }

        internal class logInvalidChecksum: SyncErrorReply {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.InvalidChecksum; }
            }
        }

        internal class logInvalidPacket: SyncErrorReply {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.InvalidPacket; }
            }
        }

        internal class logInvalidLength: SyncErrorReply {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.InvalidLength; }
            }
        }

        internal class logInvalidData: SyncErrorReply {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.InvalidData; }
            }
        }

        internal class logInvalidState: SyncErrorReply {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.InvalidState; }
            }
        }
    }
}