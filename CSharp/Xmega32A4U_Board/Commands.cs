namespace Xmega32A4U_testBoard
{
    public struct Command
    {
        //СТРУКТУРА: Хранилище констант - кодов команд
        #region Chip
        public struct Chip
        {
            //Коды команд микросхемы микроконтроллера
            public const byte getVersion = 1;
            public const byte getBirthday = 2;
            public const byte getCPUfrequency = 3;
            public const byte getStatus = 20;
            public const byte reset = 4;
            public const byte wait = 5;

        }
        #endregion
        #region RTC
        public struct RTC
        {
            //Коды команд счётчиков
            public const byte startMeasure = 30;
            public const byte stopMeasure = 31;
            public const byte receiveResults = 32;
        }
        #endregion
        #region TIC
        public struct TIC
        {
            public const byte retransmit = 50;
            public const byte set_Gauges = 51;
            public const byte restartMonitoring = 52;
            public const byte get_TIC_MEM = 53;
        }
        #endregion
        #region SPI
        public struct SPI
        {
            //Коды команд для SPI устройств
            public struct PSIS
            {
                //Коды команд Ионного Источника

                public const byte setVoltage = 40;
                public const byte getVoltage = 60;

            }
            public struct DPS
            {
                //Коды команд Детектора
                public const byte setVoltage = 41;
                public const byte getVoltage = 61;
            }
            public struct PSInl
            {
                //Коды команд Натекателя
                public const byte setVoltage = 44;
                public const byte getVoltage = 63;
            }
            public struct Scaner
            {
                //Коды команд Сканера
                public const byte setVoltage = 42;
                public const byte getVoltage = 62;

            }
            public struct Condensator
            {
                //Коды команд Конденсатора
                public const byte setVoltage = 43;
                public const byte getVoltage = 62;
            }
        }
        #endregion
        public const byte LOCK = 13;
        public const byte KEY = 58;
        #region Service
        public struct Service
        {
            //Коды команд отладки
            public const byte showMeByte = 21;
            public const byte checkCommandStack = 8;
        }
        #endregion
        #region Flags
        public struct Flags
        {
            public const byte setFlags = 70;
            public const byte HVE = 71;
            public const byte PRGE = 72;
            public const byte EDCD = 73;
            public const byte SEMV1 = 74;
            public const byte SEMV2 = 75;
            public const byte SEMV3 = 76;
            public const byte SPUMP = 77;
        }
        #endregion
    }
}