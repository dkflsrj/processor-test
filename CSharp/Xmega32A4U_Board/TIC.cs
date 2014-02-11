using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;

namespace Xmega32A4U_testBoard
{
    public delegate void TIC_CallBack(string error, string errorID);
    #region DataBase class
    public class DataBase
    {
        string Name;
        public string[] Value;
        public string[] Meaning;
        public DataBase(string name, string[] values, string[] meanings)
        {
            Name = name;
            Value = values;
            Meaning = meanings;
        }
        public string findValue(string meaning)
        {
            for (byte i = 0; i < Value.Length; i++)
            {
                if (meaning == Meaning[i])
                {
                    return Value[i];
                }
            }
            return Name + ":Совпадений с " + meaning + " не найдено!";
        }
        public string findMeaning(string value)
        {
            for (byte i = 0; i < Value.Length; i++)
            {
                if (value == Value[i])
                {
                    return Meaning[i];
                }
            }
            return Name + ":Совпадений со значением " + value + " не найдено!";
        }
        public string find(string meaningORvalue)
        {
            //ФУНКЦИЯ: Ищет в массиве Meaning, если нахоит возвращает число
            //, если не находит, ищет в массиве чисел, если находит возвращает входящий параметр
            //, если и там не находит сообщает об ошибке возвращает ""
            for (byte i = 0; i < Value.Length; i++)
            {
                if (meaningORvalue == Meaning[i])
                {
                    return Value[i];
                }
            }
            for (byte i = 0; i < Value.Length; i++)
            {
                if (meaningORvalue == Value[i])
                {
                    return meaningORvalue;
                }
            }
            MC.Service.trace(Name + ":Совпадений со значением " + meaningORvalue + " не найдено!");
            return "?";
        }
    }
    #endregion
    public static class TIC
    {
        /// <summary>
        /// Вызывается, если TIC на запрос значения (?V) присылает приоритет "Alert_LOW"
        /// </summary>
        public static TIC_CallBack Alert_LOW_Call;
        /// <summary>
        /// Вызывается, если TIC на запрос значения (?V) присылает приоритет "Alert_HIGH"
        /// </summary>
        public static TIC_CallBack Alert_HIGH_Call;
        /// <summary>
        /// Вызывается, если TIC на запрос значения (?V) присылает приоритет "warning"
        /// </summary>
        public static TIC_CallBack WarningCall;
        /// <summary>
        /// Вызывается, если TIC сообщает о какой-либо ошибке связанной с СОМ-портом.
        /// </summary>
        public static TIC_CallBack ErrorCall;
        static string[] dummy = new string[] { };
        static string[] response;
        static string AlertID = CodeLists.AlertID.No_Alert;
        static string ErrorID = CodeLists.SerialCommsResponseCode.no_error.ToString();
        static string PriorityID = CodeLists.Priority.OK;
        static string Alert = CodeBase.AlertID.findMeaning(AlertID);
        static string Error = CodeBase.SerialCommsResponseCode.findMeaning(ErrorID);
        static string Priority = CodeBase.Priority.findMeaning(PriorityID);
        #region База кодов
        /// <summary>
        /// Класс для поиска значений по кодам и наоборот
        /// </summary>
        public static class CodeBase
        {
            //КЛАСС: Хранилище кодов TIC контроллера
            #region Addresses
            /// <summary>
            /// Адреса модулей
            /// </summary>
            public static DataBase Addresses = new DataBase
            (
                "Addresses",
                new string[] 
                { 
                    "850",
                    "851",
                    "852",
                    "853",
                    "854",
                    "855",
                    "856",
                    "857",
                    "859",
                    "860",
                    "867",
                    "868",
                    "869",
                    "870",
                    "871",
                    "872",
                    "875",
                    "901",
                    "902",
                    "904",
                    "905",
                    "906",
                    "907",
                    "908",
                    "909",
                    "910",
                    "911",
                    "912",
                    "913",
                    "914",
                    "915",
                    "916",
                    "917",
                    "918",
                    "919",
                    "920",
                    "921",
                    "922",
                    "923",
                    "924",
                    "925",
                    "926",
                    "928",
                    "929",
                    "930",
                    "931",
                    "932",
                    "933",
                    "934",
                    "935",
                    "936",
                    "937",
                    "938",
                    "939",
                    "940"
                },
                new string[] 
                { 
                     "EXT75DX.Node",
                     "EXT75DX.Pump_type",
                     "EXT75DX.Pump_control",
                     "EXT75DX.Vent_options",
                     "EXT75DX.Timer_setting",
                     "EXT75DX.Power_limit_setting",
                     "EXT75DX.Normal_speed_setting",
                     "EXT75DX.Standby_speed_setting",
                     "EXT75DX.Temperature_readings",
                     "EXT75DX.Link_parameter_readings",
                     "EXT75DX.Factory_settings",
                     "EXT75DX.PIC_software_version",
                     "EXT75DX.Speed_control",
                     "EXT75DX.Timer_options",
                     "EXT75DX.Analogue_signal_options",
                     "EXT75DX.Electronic_braking_option",
                     "EXT75DX.Close_vent_valve",
                     "Node",
                     "TIC_Status",
                     "Turbo_Pump",
                     "Turbo_Speed",
                     "Turbo_Power",
                     "Turbo_normal",
                     "Turbo_standby",
                     "Turbo_cycle_time",
                     "Backing_Pump",
                     "Backing_speed",
                     "Backing_power",
                     "Gauge_1",
                     "Gauge_2",
                     "Gauge_3",
                     "Relay_1",
                     "Relay_2",
                     "Relay_3",
                     "PS_Temperature",
                     "InternalTemperature",
                     "Analogue_out",
                     "External_vent_valve",
                     "Heater_band",
                     "External_Air_Cooler",
                     "Display_contrast ",
                     "Configuration_Operations",
                     "Lock",
                     "Pressure_Units ",
                     "PCcomms",
                     "DefaultScreen",
                     "Fixed_or_Float_ASG",
                     "System",
                     "Gauge_4",
                     "Gauge_5",
                     "Gauge_6",
                     "Relay_4",
                     "Relay_5",
                     "Relay_6",
                     "GaugeValues"
                }
            );
            #endregion
            #region SerialCommsResponseCode
            /// <summary>
            /// Отклики на команды
            /// </summary>
            public static DataBase SerialCommsResponseCode = new DataBase
            (
                "SerialCommsResponseCode",
                new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" },
                new string[] 
                {
                    "no error",
                    "Invalid command for object ID",
                    "Invalid query/command",
                    "Missing parameter",
                    "Parameter out of range",
                    "Invalid command in current state",
                    "Data checksum error",
                    "EEPROM read or write error",
                    "Operation took too long",
                    "Invalid config ID"
                }
            );
            #endregion
            #region Priority
            /// <summary>
            /// Приоритеты (присылаются вместе с запросом любого значения (?V))
            /// </summary>
            public static DataBase Priority = new DataBase
            (
                "Priority",
                new string[] { "0", "1", "2", "3" },
                new string[] { "OK", "warning", "alert_LOW", "alert_HIGH" }
            );
            #endregion
            #region AlertID
            /// <summary>
            /// AlertIDs
            /// </summary>
            public static DataBase AlertID = new DataBase
            (
                "AlertID",
                new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47" },
                new string[] 
                {
                    "No Alert",
                    "ADC Fault",
                    "ADC Not Ready",
                    "Over Range",
                    "Under Range",
                    "ADC Invalid",
                    "No Gauge",
                    "Unknown",
                    "Not Supported",
                    "New ID",
                    "Over Range",
                    "Under Range",
                    "Over Range",
                    "Ion Em Timeout",
                    "Not Struck",
                    "Filament Fail",
                    "Mag Fail",
                    "Striker Fail",
                    "Not Struck",
                    "Filament Fail",
                    "Cal Error",
                    "Initialising",
                    "Emission Error",
                    "Over Pressure",
                    "ASGCant Zero",
                    "RampUp Timeout",
                    "Droop Timeout",
                    "Run Hours High",
                    "SC Interlock",
                    "ID Volts Error",
                    "Serial ID Fail",
                    "Upload Active",
                    "DX Fault",
                    "Temp Alert",
                    "SYSI Inhibit",
                    "Ext Inxibit",
                    "Temp Inhibit",
                    "No Reading",
                    "No Message",
                    "NOV Failure",
                    "Upload Timeout",
                    "Download Failed",
                    "No Tube",
                    "Use Gauges 4-6",
                    "Degas Inhibited",
                    "IGC Inhibited",
                    "Brownout/Short",
                    "Service due"
                }
            );
            #endregion
            #region SNTvalues
            /// <summary>
            /// Единицы измерения (V,P,%)
            /// </summary>
            public static DataBase SNVTvalues = new DataBase
            (
                "SNVTvalues",
                new string[] { "66", "59", "81" },
                new string[] { "VOLTAGE", "PRESSURE", "PERCENT" }
            );
            #endregion
            #region CommandList
            /// <summary>
            /// CommandList
            /// </summary>
            public static DataBase CommandList_Device = new DataBase
            (
                "CommandList_Device",
                new string[] { "0", "1" },
                new string[] { "Device Off", "Device On" }
            );
            public static DataBase CommandList_Gauge = new DataBase
            (
                "CommandList_Gauge",
                new string[] { "0", "1", "2", "3", "4", "5" },
                new string[] 
                { 
                    "Gauge Off", 
                    "Gauge On",
                    "Gauge New_Id",
                    "Gauge Zero",
                    "Gauge Cal",
                    "Gauge Degas" 
                }
            );
            public static DataBase CommandList_TIC = new DataBase
            (
                "CommandList_Gauge",
                new string[] { "576", "0", "1" },
                new string[] { "Load Defaults", "Upload", "Download" }
            );
            #endregion
            #region State
            /// <summary>
            /// Состояния
            /// </summary>
            public static DataBase State = new DataBase
            (
                "State",
                new string[] { "0", "1", "2", "3", "4" },
                new string[] 
                {
                    "Off State",
                    "Off Going On State",
                    "On Going Off Shutdown State",
                    "On Going Off Normal State",
                    "On State"
                }
            );
            #endregion
            #region ActiveGaugeStates
            /// <summary>
            /// Состояния датчиков
            /// </summary>
            public static DataBase ActiveGaugeStates = new DataBase
            (
                "ActiveGaugeStates",
                new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" },
                new string[] 
                {
                    "Gauge Not connected",
                    "Gauge Connected",
                    "New Gauge Id",
                    "Gauge Change",
                    "Gauge In Alert",
                    "Off",
                    "Striking",
                    "Initialising",
                    "Calibrating",
                    "Zeroing",
                    "Degassing",
                    "On",
                    "Inhibited"
                }
            );
            #endregion
            #region FullPumpStates
            /// <summary>
            /// Состояния насосов
            /// </summary>
            public static DataBase FullPumpStates = new DataBase
            (
                "FullPumpStates",
                new string[] { "0", "1", "5", "4", "2", "3", "6", "7" },
                new string[] 
                {
                    "Stopped",
                    "Starting Delay",
                    "Accelerating",
                    "Running",
                    "Stopping Short Delay",
                    "Stoppenig Normal Delay",
                    "Fault Braking",
                    "Braking"
                }
            );
            #endregion
            #region GasTypes
            /// <summary>
            /// Типы газа
            /// </summary>
            public static DataBase GasTypes = new DataBase
            (
                "GasTypes",
                new string[] { "0", "1", "2", "3", "4", "5", "6" },
                new string[] 
                {
                    "Nitrogen",
                    "Helium",
                    "Argon",
                    "Carbon Dioxide",
                    "Neon",
                    "Krypton",
                    "Voltage"
                }
            );
            #endregion
            #region GaugeTypes
            /// <summary>
            /// Типы датчиков
            /// </summary>
            public static DataBase GaugeTypes = new DataBase
            (
                "GaugeTypes",
                new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25" },
                new string[] 
                {
                    "Unckown Device",
                    "No Device",
                    "EXP_CM",
                    "EXP_STD",
                    "CMAN_S",
                    "CMAN_D",
                    "TURBO",
                    "APGM",
                    "APGL",
                    "APGXM",
                    "APGXH",
                    "APGXL",
                    "ATCA",
                    "ATCD",
                    "ATCM",
                    "WRG",
                    "AIMC",
                    "AIMN",
                    "AIMS",
                    "AIMX",
                    "AIGC_I2R",
                    "AIGC_2FIL",
                    "ION_EB",
                    "AIGXS",
                    "USER",
                    "ASG"
                }
            );
            #endregion
            #region PumpTypes
            /// <summary>
            /// Типы насосов
            /// </summary>
            public static DataBase PumpTypes = new DataBase
            (
                "PumpTypes",
                new string[] { "0", "1", "3", "4", "8", "9", "10", "11", "12", "99" },
                new string[] 
                {
                    "No Pump",
                    "EXDC Pump",
                    "EXT75DX Pump",
                    "EXT255DX",
                    "Mains Backing Pump",
                    "Serial Pump",
                    "nEXT - 485",
                    "nEXT - 232",
                    "nXDS",
                    "Not yet identified"
                }
            );
            #endregion
            #region ASG_range
            /// <summary>
            /// ASG range
            /// </summary>
            public static DataBase ASG_range = new DataBase
            (
                "ASG_range",
                new string[] { "0", "1" },
                new string[] 
                    {
                        "1000",
                        "2000"
                    }
            );
            #endregion
            // Проча
            #region Turn
            public static DataBase Turn = new DataBase
            (
                "Turn",
                new string[] { "1", "0" },
                new string[] { "On", "Off" }
            );
            #endregion
            //EXT75DX
            #region EXT75DX
            public static DataBase EXT75DX_Error_Codes = new DataBase
            (
                "EXT75DX_Error_Codes",
                new string[] { "0", "1", "2", "3", "4", "5" },
                new string[] 
                    { 
                        "No error", 
                        "Invalid command for object ID",
                        "Invalid Query/Command",
                        "Missing parameter",
                        "Parameter out of range",
                        "Invalid command in current state - e.g. serial command to start/stop when in parallel control mode"
                    }
            );
            #endregion
        }
        /// <summary>
        /// Списки возможных вариантов
        /// </summary>
        public static class CodeLists
        {
            //КЛАСС: Хранилище списков возможных вариантов
            #region Addresses
            /// <summary>
            /// Адреса модулей
            /// </summary>
            public struct Address
            {
                //СТРУКТУРА: Хранилище кодов адресов объектов, к которым можно обратиться
                public struct EXT75DX
                {
                    public const ushort Node = 850;
                    public const ushort Pump_type = 851;
                    public const ushort Pump_control = 852;
                    public const ushort Vent_options = 853;
                    public const ushort Timer_setting = 854;
                    public const ushort Power_limit_setting = 855;
                    public const ushort Normal_speed_setting = 856;
                    public const ushort Standby_speed_setting = 857;
                    public const ushort Temperature_readings = 859;
                    public const ushort Link_parameter_readings = 860;
                    public const ushort Factory_settings = 867;
                    public const ushort PIC_software_version = 868;
                    public const ushort Speed_control = 869;
                    public const ushort Timer_options = 870;
                    public const ushort Analogue_signal_options = 871;
                    public const ushort Electronic_braking_option = 872;
                    public const ushort Close_vent_valve = 875;
                }
                public const ushort Node = 901;
                public const ushort TIC_Status = 902;
                public const ushort Turbo_Pump = 904;
                public const ushort Turbo_Speed = 905;
                public const ushort Turbo_Power = 906;
                public const ushort Turbo_normal = 907;
                public const ushort Turbo_standby = 908;
                public const ushort Turbo_cycle_time = 909;
                public const ushort Backing_Pump = 910;
                public const ushort Backing_speed = 911;
                public const ushort Backing_power = 912;
                public const ushort Gauge_1 = 913;
                public const ushort Gauge_2 = 914;
                public const ushort Gauge_3 = 915;
                public const ushort Relay_1 = 916;
                public const ushort Relay_2 = 917;
                public const ushort Relay_3 = 918;
                public const ushort PS_Temperature = 919;
                public const ushort InternalTemperature = 920;
                public const ushort Analogue_out = 921;
                public const ushort External_vent_valve = 922;
                public const ushort Heater_band = 923;
                public const ushort External_Air_Cooler = 924;
                public const ushort Display_contrast = 925;
                public const ushort Configuration_Operations = 926;
                public const ushort Lock = 928;
                public const ushort Pressure_Units = 929;
                public const ushort PCcomms = 930;
                public const ushort DefaultScreen = 931;
                public const ushort Fixed_or_Float_ASG = 932;
                public const ushort System = 933;
                public const ushort Gauge_4 = 934;
                public const ushort Gauge_5 = 935;
                public const ushort Gauge_6 = 936;
                public const ushort Relay_4 = 937;
                public const ushort Relay_5 = 938;
                public const ushort Relay_6 = 939;
                public const ushort GaugeValues = 940;

            }
            #endregion
            #region SNTvalues
            /// <summary>
            /// Единицы измерения (V,P,%)
            /// </summary>
            public struct SNVTvalues
            {
                public const string VOLTAGE = "66";
                public const string PRESSURE = "59";
                public const string PERCENT = "81";
            }
            #endregion
            #region GasTypes
            /// <summary>
            /// Типы газа
            /// </summary>
            public struct GasTypes
            {
                public const string Nitrogen = "0";
                public const string Helium = "1";
                public const string Argon = "2";
                public const string Carbon_Dioxide = "3";
                public const string Neon = "4";
                public const string Krypton = "5";
                public const string Voltage = "6";
            }
            #endregion
            #region CommandLists
            public struct CommandList_Device
            {
                public const string Device_Off = "0";
                public const string Device_On = "1";
            }
            public struct CommandList_Gauge
            {
                public const string Gauge_Off = "0";
                public const string Gauge_On = "1";
                public const string Gauge_New_Id = "2";
                public const string Gauge_Zero = "3";
                public const string Gauge_Cal = "4";
                public const string Gauge_Degas = "5";
            }
            public struct CommandList_TIC
            {
                public const string Load_Defaults = "576";
                public const string Upload = "0";
                public const string Download = "1";
            }
            #endregion
            #region ASG_range
            public struct ASG_range
            {
                public const string _1000_mbar = "0";
                public const string _2000_mbar = "1";
            }
            #endregion
            #region Turn
            public struct Turn
            {
                public const string On = "1";
                public const string Off = "0";
            }
            #endregion
            #region SerialCommsResponseCode
            public struct SerialCommsResponseCode
            {
                public const byte no_error = 0;
                public const byte Invalid_command_for_object_ID = 1;
                public const byte Invalid_query_or_command = 2;
                public const byte Missing_parameter = 3;
                public const byte Parameter_out_of_range = 4;
                public const byte Invalid_command_in_current_state = 5;
                public const byte Data_checksum_error = 6;
                public const byte EEPROM_read_or_write_error = 7;
                public const byte Operation_took_too_long = 8;
                public const byte Invalid_config_ID = 9;
            }
            #endregion
            #region Priority
            /// <summary>
            /// Приоритеты (присылаются вместе с запросом любого значения (?V))
            /// </summary>
            public struct Priority
            {
                public const string OK = "0";
                public const string warning = "1";
                public const string alert_LOW = "2";
                public const string alert_HIGH = "3";
            }
            #endregion
            #region AlertID
            /// <summary>
            /// AlertIDs
            /// </summary>
            public struct AlertID
            {
                public const string No_Alert = "0";
                public const string ADC_Fault = "1";
                public const string ADC_Not_Ready = "2";
                public const string Over_Range_1 = "3";
                public const string Under_Range_1 = "4";
                public const string ADC_Invalid = "5";
                public const string No_Gauge = "6";
                public const string Unknown = "7";
                public const string Not_Supported = "8";
                public const string New_ID = "9";
                public const string Over_Range_2 = "10";
                public const string Under_Range_2 = "11";
                public const string Over_Range_3 = "12";
                public const string Ion_Em_Timeout = "13";
                public const string Not_Struck_1 = "14";
                public const string Filament_Fail_1 = "15";
                public const string Mag_Fail = "16";
                public const string Striker_Fail = "17";
                public const string Not_Struck_2 = "18";
                public const string Filament_Fail_2 = "19";
                public const string Cal_Error = "20";
                public const string Initialising = "21";
                public const string Emission_Error = "22";
                public const string Over_Pressure = "23";
                public const string ASGCant_Zero = "24";
                public const string RampUp_Timeout = "25";
                public const string Droop_Timeout = "26";
                public const string Run_Hours_High = "27";
                public const string SC_Interlock = "28";
                public const string ID_Volts_Error = "29";
                public const string Serial_ID_Fail = "30";
                public const string Upload_Active = "31";
                public const string DX_Fault = "32";
                public const string Temp_Alert = "33";
                public const string SYSI_Inhibit = "34";
                public const string Ext_Inxibit = "35";
                public const string Temp_Inhibit = "36";
                public const string No_Reading = "37";
                public const string No_Message = "38";
                public const string NOV_Failure = "39";
                public const string Upload_Timeout = "40";
                public const string Download_Failed = "41";
                public const string No_Tube = "42";
                public const string Use_Gauges_4_6 = "43";
                public const string Degas_Inhibited = "44";
                public const string IGC_Inhibited = "45";
                public const string Brownout_or_Short = "46";
                public const string Service_due = "47";
            }
            #endregion
            //EXT75DX
            #region EXT75DX
            public struct EXT75DX_Error_Codes
            {
                public const string No_error = "0";
                public const string Invalid_command_for_object_ID = "1";
                public const string Invalid_Query_or_Command = "2";
                public const string Missing_parameter = "3";
                public const string Parameter_out_of_range = "4";
                public const string Invalid_command_in_current_state = "5";
            }
            #endregion
        }
        #endregion
        //Модули
        #region Node
        /// <summary>
        /// Node. Get/set multi-drop 0...98
        /// </summary>
        public static string Node
        {
            get
            {
                response = toTIC_QS(CodeLists.Address.Node, dummy);
                try { return response[0]; }
                catch { MC.Service.trace("TIC.Node.get(): Ошибка данных!"); }
                return "?";
            }
            set { toTIC_SS(CodeLists.Address.Node, new string[] { value }); }
        }
        #endregion
        #region TIC_Status
        /// <summary>
        /// TIC Status
        /// </summary>
        public static class TIC_Status
        {
            /// <summary>
            /// Возвращает состояния вакуумной системы:
            /// </summary>
            /// <param name="T">Turbo state</param>
            /// <param name="B">Backing state</param>
            /// <param name="G1">Gauge_1 state</param>
            /// <param name="G2">Gauge_2 state</param>
            /// <param name="G3">Gauge_3 state</param>
            /// <param name="R1">Relay_1 state</param>
            /// <param name="R2">Relay_2 state</param>
            /// <param name="R3">Relay_3 state</param>
            public static void value(ref string T, ref string B, ref string G1, ref string G2, ref string G3, ref string R1, ref string R2, ref string R3)
            {
                response = toTIC_QV(CodeLists.Address.TIC_Status);
                try
                {
                    T = CodeBase.FullPumpStates.findMeaning(response[0]);
                    B = CodeBase.FullPumpStates.findMeaning(response[1]);     //Однако, чётко не сказано откуда брать состояния
                    G1 = CodeBase.ActiveGaugeStates.findMeaning(response[2]);  //Однако, чётко не сказано откуда брать состояния
                    G2 = CodeBase.ActiveGaugeStates.findMeaning(response[3]);  //Однако, чётко не сказано откуда брать состояния
                    G3 = CodeBase.ActiveGaugeStates.findMeaning(response[4]);  //Однако, чётко не сказано откуда брать состояния
                    R1 = CodeBase.CommandList_Device.findMeaning(response[5]); //Однако, чётко не сказано откуда брать состояния
                    R2 = CodeBase.CommandList_Device.findMeaning(response[6]); //Однако, чётко не сказано откуда брать состояния
                    R3 = CodeBase.CommandList_Device.findMeaning(response[7]); //Однако, чётко не сказано откуда брать состояния
                    //response[8] = CodeBase.AlertID.findMeaning(response[8]);
                    //response[10] = CodeBase.Priority.findMeaning(response[9]);
                }
                catch { MC.Service.trace("TIC.TIC_Status.value(): Ошибка данных!"); }

            }
            /// <summary>
            /// Возвращает сведения о системе
            /// </summary>
            /// <param name="TICxxx">fixed string for this pump</param>
            /// <param name="SW_Ver">software version number fixed by the software</param>
            /// <param name="SerNum">serial number</param>
            /// <param name="PIC_SW_Ver">PIC software version</param>
            public static void setup(ref string TICxxx, ref string SW_Ver, ref string SerNum, ref string PIC_SW_Ver)
            {
                toTIC_QS(CodeLists.Address.TIC_Status, dummy);
                TICxxx = response[0];
                SW_Ver = response[1];
                SerNum = response[2];
                PIC_SW_Ver = response[3];
            }
        }
        #endregion
        #region Turbo
        /// <summary>
        /// Turbo Pump, Speed, Power ...
        /// </summary>
        public static class Turbo
        {
            #region Base
            public struct ListOfSetups
            {
                public const string set_in_standby = "1";
                public const string set_not_standby = "0";
            }
            #endregion
            //---
            #region Pump
            public static class Pump
            {
                /// <summary>
                /// Включает турбомолекулярный насос
                /// </summary>
                public static void turnOn() 
                {
                    toTIC_C(CodeLists.Address.Turbo_Pump, "1"); 
                }
                /// <summary>
                /// Выключает турбомолекулярный насос.
                /// <para>ПРИМЕЧАНИЕ: При этом выключаются высокие напряжения (PRGE = Off)</para>
                /// </summary>
                public static void turnOff()
                {
                    MC.Flags.PRGE = "Off";
                    toTIC_C(CodeLists.Address.Turbo_Pump, "0");
                }
                /// <summary>
                /// Возвращает состояние Turbo Pump
                /// </summary>
                public static string state
                {
                    get
                    {
                        response = toTIC_QV(CodeLists.Address.Turbo_Pump);
                        try
                        {
                            response[0] = CodeBase.FullPumpStates.findMeaning(response[0]);
                            //response[1] = CodeBase.AlertID.findMeaning(response[1]);
                            //response[2] = CodeBase.Priority.findMeaning(response[2]);
                            return response[0];
                        }
                        catch { MC.Service.trace("TIC.Turbo.Pump.state.get(): Ошибка данных!"); }
                        return "?";
                    }
                }
                /// <summary>
                /// Возвращает тип насоса
                /// </summary>
                public static string type
                {
                    get
                    {
                        string configType = "3";
                        response = toTIC_QS(CodeLists.Address.Turbo_Pump, new string[] { configType });
                        try
                        {
                            if (response[0] == configType)
                            {
                                return CodeBase.PumpTypes.findMeaning(response[1]);
                            }
                            MC.Service.trace("TIC.Turbo.Pump.type.get(): Ошибка config type! Ожидалось: " + configType + " Получено: " + response[0]);
                        }
                        catch { MC.Service.trace("TIC.Turbo.Pump.type.get(): Ошибка данных!"); }
                        return "";
                    }
                }
                /// <summary>
                /// Возвращает установки Turbo Pump
                /// </summary>
                /// <param name="Master">object ID ("Gauge_1", "Gauge_2" или "Gauge_3")</param>
                /// <param name="units_type">"VOLTAGE" или "PRESSURE"</param>
                /// <param name="on_setpoint">on setpoint</param>
                /// <param name="off_setpoint">off setpoint</param>
                /// <param name="enable">"On" или "Off"</param>
                public static void setup(ref string Master, ref string units_type, ref string on_setpoint, ref string off_setpoint, ref string enable)
                {
                    string configType = "4";
                    response = toTIC_QS(CodeLists.Address.Turbo_Pump, new string[] { configType });
                    try
                    {
                        if (response[0] == configType)
                        {
                            Master = CodeBase.Addresses.findMeaning(response[1]);
                            units_type = CodeBase.SNVTvalues.findMeaning(response[2]);
                            on_setpoint = response[3];
                            off_setpoint = response[4];
                            enable = CodeBase.Turn.findMeaning(response[5]);
                            return;
                        }
                        MC.Service.trace("TIC.Turbo.Pump.setup(): Ошибка config type! Ожидалось: " + configType + "Получено: " + response[0]);
                    }
                    catch { MC.Service.trace("TIC.Turbo.Pump.setup(): Ошибка данных!"); }
                }
                /// <summary>
                /// Настраивает установки Turbo_Pump:
                /// </summary>
                /// <param name="Master">object ID ("Gauge_1", "Gauge_2" или "Gauge_3")</param>
                /// <param name="units_type">"VOLTAGE" или "PRESSURE" (CodeList.SNVTvalues)</param>
                /// <param name="on_setpoint">on setpoint
                /// <para>Напряжение - 0.000 - 9.999</para>
                /// <para>Давление - float</para></param>
                /// <param name="off_setpoint">off setpoint
                /// <para>Напряжение 0.000 - 9.999</para>
                /// <para>Давление - float</para></param>
                /// <param name="enable">"On" или "Off" (CodeList.Turn)</param>
                public static void setup(string Master, string units_type, string on_setpoint, string off_setpoint, string enable)
                {
                    string configType = "4";
                    string[] wDATA = new string[6];
                    wDATA[0] = configType;
                    wDATA[1] = CodeBase.Addresses.find(Master);
                    wDATA[2] = CodeBase.SNVTvalues.find(units_type);
                    wDATA[3] = on_setpoint;
                    wDATA[4] = off_setpoint;
                    wDATA[5] = CodeBase.Turn.find(enable);
                    response = toTIC_SS(CodeLists.Address.Turbo_Pump, wDATA);
                }
                /// <summary>
                /// Get/set start delay 0...99 mins
                /// </summary>
                public static string delay
                {
                    get
                    {
                        try
                        {
                            string configType = "21";
                            response = toTIC_QS(CodeLists.Address.Turbo_Pump, new string[] { configType });
                            if (response[0] == configType)
                            {
                                return response[1];
                            }
                            else
                            { MC.Service.trace("TIC.Turbo.Pump.delay.get(): Ошибка config type! Ожидалось: " + configType + "Получено: " + response[0]); }
                        }
                        catch { MC.Service.trace("TIC.Turbo.Pump.delay.get(): Ошибка данных!"); }
                        return "?";
                    }
                    set
                    {
                        string configType = "21";
                        toTIC_SS(CodeLists.Address.Turbo_Pump, new string[] { configType, value });
                    }
                }
            }
            #endregion
            #region speed
            public static class speed
            {
                /// <summary>
                /// Возвращает скорость в процентах (0.0...110.0%)
                /// </summary>
                public static string value
                {
                    get
                    {
                        response = toTIC_QV(CodeLists.Address.Turbo_Speed);
                        try
                        {
                            //response[1] = CodeBase.AlertID.findMeaning(response[1]);
                            //response[2] = CodeBase.Priority.findMeaning(response[2]);
                            return response[0];
                        }
                        catch { MC.Service.trace("TIC.Turbo.speed.value.get(): Ошибка данных!"); }
                        return "?";
                    }
                }
                /// <summary>
                /// Возвращает start fail time и droop fail time.
                /// <para>Times 1-30;</para>
                /// <para>0-30 mins</para>
                /// </summary>
                /// <param name="start_fail_time">start fail time</param>
                /// <param name="droop_fail_time">droop fail time</param>
                public static void setup(ref string start_fail_time, ref string droop_fail_time)
                {
                    response = toTIC_QS(CodeLists.Address.Turbo_Speed, dummy);
                    try
                    {
                        start_fail_time = response[0];
                        droop_fail_time = response[1];
                    }
                    catch { MC.Service.trace("TIC.Turbo.speed.setup(): Ошибка данных!"); }
                }
                /// <summary>
                /// Устанавливает start fail time и droop fail time
                /// </summary>
                /// <param name="start_fail_time">start fail time
                /// <para>Times 1 - 30;</para>
                /// <para>0 - 30 mins</para></param>
                /// <param name="droop_fail_time">droop fail time
                /// <para>Times 1 - 30;</para>
                /// <para>0 - 30 mins</para></param>
                /// <returns></returns>
                public static void setup(string start_fail_time, string droop_fail_time)
                {
                    toTIC_SS(CodeLists.Address.Turbo_Speed, new string[] { start_fail_time, droop_fail_time });
                }
            }
            #endregion
            #region power
            /// <summary>
            /// Возвращает Turbo Power в Ваттах
            /// </summary>
            public static string power
            {
                get
                {
                    response = toTIC_QV(CodeLists.Address.Turbo_Power);
                    try
                    {
                        //response[1] = CodeBase.AlertID.findMeaning(response[1]);
                        //response[2] = CodeBase.Priority.findMeaning(response[2]);
                        return response[0];
                    }
                    catch { MC.Service.trace("TIC.Turbo.power.get(): Ошибка данных!"); }
                    return "?";
                }
            }
            #endregion
            #region normal
            /// <summary>
            /// Возвращает "yes" или "no"
            /// </summary>
            public static string normal
            {
                get
                {
                    response = toTIC_QV(CodeLists.Address.Turbo_normal);
                    try
                    {
                        //response[1] = CodeBase.AlertID.findMeaning(response[1]);
                        //response[2] = CodeBase.Priority.findMeaning(response[2]);
                        switch (response[0])
                        {
                            case "0": return "no";
                            case "4": return "yes";
                            default: MC.Service.trace("TIC.Turbo.normal.get(): Ошибка! Неверное состояние: " + response[0]);
                                return "?";
                        }
                    }
                    catch { MC.Service.trace("TIC.Turbo.normal.get(): Ошибка данных!"); }
                    return "?";
                }
            }
            #endregion
            #region standby
            /// <summary>
            /// Возвращает "in standby" или "not in standby" 
            /// <para>При установке использовать Turbo.ListOfSetups</para>
            /// </summary>
            public static string standby
            {
                get
                {
                    response = toTIC_QV(CodeLists.Address.Turbo_standby);
                    try
                    {
                        //response[1] = CodeBase.AlertID.findMeaning(response[1]);
                        //response[2] = CodeBase.Priority.findMeaning(response[2]);
                        switch (response[0])
                        {
                            case "4": return "in standby";
                            case "0": return "not in standby";
                            default: MC.Service.trace("TIC.Turbo_standby.get(): Ошибка! Неверное состояние: " + response[0]);
                                return "?";
                        }
                    }
                    catch { MC.Service.trace("TIC.Turbo_standby.get(): Ошибка данных!"); }
                    return "?";
                }
                set { toTIC_C(CodeLists.Address.Turbo_standby, value); }
            }
            #endregion
            #region cycle_time
            /// <summary>
            /// Возвращает время работы 
            /// </summary>
            /// <param name="time">Time period turbo has been on (0...65535 hours)</param>
            /// <param name="state">Состояние</param>
            public static string cycle_time(ref string state)
            {
                response = toTIC_QV(CodeLists.Address.Turbo_cycle_time);
                try
                {
                    state = CodeBase.State.findMeaning(response[1]);
                    return response[0];
                    //response[2] = CodeBase.AlertID.findMeaning(response[2]);
                    //response[3] = CodeBase.Priority.findMeaning(response[3]);
                }
                catch { MC.Service.trace("TIC.Turbo.cycle_time.get(): Ошибка данных!"); }
                return "?";
            }
            #endregion
        }
        #endregion
        #region Backing
        /// <summary>
        /// Backing Pump, speed, power...
        /// </summary>
        public static class Backing
        {
            #region Pump
            public static class Pump
            {
                #region Base
                static DataBase CodeBase_local = new DataBase
                (
                    "CodeBase_local",
                    new string[] { "0", "1", "2" },
                    new string[] { "None", "On stop", "On 50%" }
                );
                public struct ListOfSetups
                {
                    public const string None = "0";
                    public const string On_stop = "1";
                    public const string On_50 = "2";
                }
                #endregion
                /// <summary>
                /// Включает или выключает насос
                /// </summary>
                /// <param name="OnOrOff">"On" или "Off" (CodeList.Turn)</param>
                public static void turn(string OnOrOff) { toTIC_C(CodeLists.Address.Backing_Pump, CodeBase.Turn.find(OnOrOff)); }
                /// <summary>
                /// Возвращает статус Backing Pump
                /// </summary>
                public static string state
                {
                    get
                    {
                        response = toTIC_QV(CodeLists.Address.Backing_Pump);
                        try
                        {
                            //response[1] = CodeBase.AlertID.findMeaning(response[1]);
                            //response[2] = CodeBase.Priority.findMeaning(response[2]);
                            return CodeBase.State.findMeaning(response[0]);
                        }
                        catch { MC.Service.trace("TIC.Backing.Pump.state.get(): Ошибка данных!"); }
                        return "?";
                    }
                }
                /// <summary>
                /// Возвращает тип насоса
                /// </summary>
                public static string type
                {
                    get
                    {
                        string configType = "3";
                        response = toTIC_QS(CodeLists.Address.Backing_Pump, new string[] { configType });
                        try
                        {
                            if (response[0] == configType)
                            {
                                return CodeBase.PumpTypes.findMeaning(response[1]);
                            }
                            MC.Service.trace("TIC.Backing.Pump.type.get(): Ошибка config type! Ожидалось: " + configType + " Получено: " + response[0]);
                        }
                        catch { MC.Service.trace("TIC.Backing.Pump.type.get(): Ошибка данных!"); }
                        return "";
                    }
                }
                /// <summary>
                /// Возвращет или устанавливает настройки насоса ("None", "On stop","On 50%")
                /// <para>При установке выбирать из списка Backing.Pump.ListOfSetups</para>
                /// </summary>
                public static string setup
                {
                    get
                    {
                        string configType = "70";
                        response = toTIC_QS(CodeLists.Address.Backing_Pump, new string[] { configType });
                        try
                        {
                            if (response[0] == configType)
                            {
                                return CodeBase_local.findMeaning(response[1]);
                            }
                            MC.Service.trace("TIC.Backing.Pump.setup(): Ошибка config type! Ожидалось: " + configType + "Получено: " + response[0]);
                        }
                        catch { MC.Service.trace("TIC.Backing.Pump.setup(): Ошибка данных!"); }
                        return "";
                    }
                    set
                    {
                        string configType = "70";
                        toTIC_SS(CodeLists.Address.Backing_Pump, new string[] { configType, value });
                    }
                }
            }
            #endregion
            #region speed
            public static string speed
            {
                get
                {
                    response = toTIC_QV(CodeLists.Address.Backing_speed);
                    try
                    {
                        //response[1] = CodeBase.AlertID.findMeaning(response[1]);
                        //response[2] = CodeBase.Priority.findMeaning(response[2]);
                        return response[0];
                    }
                    catch { MC.Service.trace("TIC.Backing.speed.get(): Ошибка данных!"); }
                    return "?";
                }
            }
            #endregion
            #region power
            public static string power
            {
                get
                {
                    response = toTIC_QV(CodeLists.Address.Backing_power);
                    try
                    {
                        //response[1] = CodeBase.AlertID.findMeaning(response[1]);
                        //response[2] = CodeBase.Priority.findMeaning(response[2]);
                        return response[0];
                    }
                    catch { MC.Service.trace("TIC.Backing.power.get(): Ошибка данных!"); }
                    return "?";
                }
            }
            #endregion
        }
        #endregion
        #region Gauges
        public class Gauge
        {
            ushort SelfAddress;
            public Gauge(ushort address) { SelfAddress = address; }
            /// <summary>
            /// Возвращает численное значение давления или напряжения (0.000...11.000 V), единицы измерения и состояние датчика 
            /// </summary>
            /// <param name="units_type">Единицы измерения</param>
            /// <param name="state">Состояние</param>
            public string value(ref string units_type, ref string state)
            {
                response = toTIC_QV(SelfAddress);
                try
                {
                    units_type = CodeBase.SNVTvalues.findMeaning(response[1]);
                    state = CodeBase.State.findMeaning(response[2]);
                    //response[3] = CodeBase.AlertID.findMeaning(response[3]);
                    //response[4] = CodeBase.Priority.findMeaning(response[4]);
                    return response[0];
                }
                catch { MC.Service.trace("TIC." + CodeBase.Addresses.findMeaning(SelfAddress.ToString()) + ".value.get(): Ошибка данных!"); }
                return "?";
            }
            /// <summary>
            /// Возвращает установки
            /// </summary>
            /// <param name="Master">object ID ("Gauge_1", "Gauge_2", "Gauge_3" или "Turbo_speed")</param>
            /// <param name="units_type">"VOLTAGE" или "PRESSURE"</param>
            /// <param name="on_setpoint">on setpoint</param>
            /// <param name="off_setpoint">off setpoint</param>
            /// <param name="enable">"On" или "Off"</param>
            public void setup(ref string Master, ref string units_type, ref string on_setpoint, ref string off_setpoint, ref string enable)
            {
                string configType = "4";
                response = toTIC_QS(SelfAddress, new string[] { configType });
                try
                {
                    if (response[0] == configType)
                    {
                        Master = CodeBase.Addresses.findMeaning(response[1]);
                        units_type = CodeBase.SNVTvalues.findMeaning(response[2]);
                        on_setpoint = response[3];
                        off_setpoint = response[4];
                        enable = CodeBase.Turn.findMeaning(response[5]);
                        return;
                    }
                    MC.Service.trace("TIC." + CodeBase.Addresses.findMeaning(SelfAddress.ToString()) + ".setup(): Ошибка config type! Ожидалось: " + configType + "Получено: " + response[0]);
                }
                catch { MC.Service.trace("TIC." + CodeBase.Addresses.findMeaning(SelfAddress.ToString()) + ".setup(): Ошибка данных!"); }

            }
            /// <summary>
            /// Настраивает установки датчика
            /// </summary>
            /// <param name="Master">object ID ("Gauge_1", "Gauge_2", "Gauge_3" или "Turbo_speed")</param>
            /// <param name="units_type">"VOLTAGE" или "PRESSURE" (CodeList.SNTvalues)</param>
            /// <param name="on_setpoint">on setpoint
            /// <para>Напряжение - 0.000 - 9.999</para>
            /// <para>Давление - float</para></param>
            /// <param name="off_setpoint">off setpoint
            /// <para>Напряжение 0.000 - 9.999</para>
            /// <para>Давление - float</para></param>
            /// <param name="enable">"On" или "Off" (CodeList.Turn)</param>
            public void setup(string Master, string units_type, string on_setpoint, string off_setpoint, string enable)
            {
                string configType = "4";
                string[] wDATA = new string[6];
                wDATA[0] = configType;
                wDATA[1] = CodeBase.Addresses.find(Master);
                wDATA[2] = CodeBase.SNVTvalues.find(units_type);
                wDATA[3] = on_setpoint;
                wDATA[4] = off_setpoint;
                wDATA[5] = CodeBase.Turn.find(enable);
                response = toTIC_SS(SelfAddress, wDATA);
            }
            /// <summary>
            /// Возвращает тип датчика
            /// </summary>
            public string type
            {
                get
                {
                    string configType = "5";
                    response = toTIC_QS(SelfAddress, new string[] { configType });
                    try
                    {
                        if (response[0] == configType)
                        {
                            return CodeBase.GaugeTypes.findMeaning(response[1]);
                        }
                        MC.Service.trace("TIC." + CodeBase.Addresses.findMeaning(SelfAddress.ToString()) + ".type.get(): Ошибка config type! Ожидалось: " + configType + "Получено: " + response[0]);
                    }
                    catch { MC.Service.trace("TIC." + CodeBase.Addresses.findMeaning(SelfAddress.ToString()) + ".type.get(): Ошибка данных!"); }
                    return "?";
                }
            }
            /// <summary>
            /// Возвращает тип газа и filter on/off
            /// </summary>
            /// <param name="gas_type">Тип газа</param>
            /// <param name="filter">"On" или "Off"</param>
            public void Gas_type(ref string gas_type, ref string filter)
            {
                string configType = "7";
                response = toTIC_QS(SelfAddress, new string[] { configType });
                try
                {
                    if (response[0] == configType)
                    {
                        gas_type = CodeBase.GasTypes.findMeaning(response[1]);
                        filter = CodeBase.Turn.findMeaning(response[2]);
                        return;
                    }
                    MC.Service.trace("TIC." + CodeBase.Addresses.findMeaning(SelfAddress.ToString()) + ".Gas_type.get(): Ошибка config type! Ожидалось: " + configType + "Получено: " + response[0]);
                }
                catch { MC.Service.trace("TIC." + CodeBase.Addresses.findMeaning(SelfAddress.ToString()) + ".Gas_type.get(): Ошибка данных!"); }
                return;
            }
            /// <summary>
            /// Устанавливает тип газа и filter on/off
            /// </summary>
            /// <param name="gas_type">Тип газа выбирается из CodeLists.GasTypes</param>
            /// <param name="filter">"On" или "Off" (CodeLists.Turn)</param>
            /// <returns></returns>
            public void Gas_type(string gas_type, string filter)
            {
                string configType = "7";
                string[] wDATA = new string[3];
                wDATA[0] = configType;
                wDATA[1] = CodeBase.GasTypes.findValue(gas_type);
                wDATA[2] = CodeBase.Turn.findValue(filter);
                response = toTIC_SS(SelfAddress, wDATA);
            }
            /// <summary>
            /// Возвращает и устанавливает ASG_range
            /// <para>При установке использовать CodeList.ASG_range</para>
            /// </summary>
            public string ASG_range
            {
                get
                {
                    string configType = "6";
                    response = toTIC_QS(SelfAddress, new string[] { configType });
                    try
                    {
                        if (response[0] == configType) { return CodeBase.ASG_range.findMeaning(response[1]); }
                        MC.Service.trace("TIC." + CodeBase.Addresses.findMeaning(SelfAddress.ToString()) + ".ASG_range.get(): Ошибка config type! Ожидалось: " + configType + "Получено: " + response[0]);
                    }
                    catch { MC.Service.trace("TIC." + CodeBase.Addresses.findMeaning(SelfAddress.ToString()) + ".ASG_range.get(): Ошибка данных!"); }
                    return "?";
                }
                set
                {
                    string configType = "6";
                    string[] wDATA = new string[2];
                    wDATA[0] = configType;
                    wDATA[1] = value;
                    toTIC_SS(SelfAddress, wDATA);
                }
            }
            /// <summary>
            /// Возвращает или задаёт имя датчику.
            /// <para>Длинна имени: 4 символа</para>
            /// <para>Допустимые символы: 0...9 и A...Z (только заглавные)</para>
            /// </summary>
            public string name
            {
                get
                {
                    string configType = "68";
                    response = toTIC_QS(SelfAddress, new string[] { configType });
                    try
                    {
                        if (response[0] == configType) { return response[1]; }
                        MC.Service.trace("TIC." + CodeBase.Addresses.findMeaning(SelfAddress.ToString()) + ".name.get(): Ошибка config type! Ожидалось: " + configType + "Получено: " + response[0]);
                    }
                    catch { MC.Service.trace("TIC." + CodeBase.Addresses.findMeaning(SelfAddress.ToString()) + ".name.get(): Ошибка данных!"); }
                    return "?";
                }
                set
                {
                    string configType = "68";
                    string[] wDATA = new string[2];
                    wDATA[0] = configType;
                    wDATA[1] = value;
                    toTIC_SS(SelfAddress, wDATA);
                }
            }
            /// <summary>
            /// Команда датчику
            /// </summary>
            /// <param name="Command">Команда выбирается из списка CodeList.CommandList_Gauge</param>
            public void command(string Command) { toTIC_C(SelfAddress, Command); }
            #region Gauge_Values
            /// <summary>
            /// Возвращает позиции и значения датчиков
            /// <para>Пример: Подключён один датчик к позиции 2.</para>
            /// <para>[0] - "2" - позиция,</para>
            /// <para>[1] - "3.9441e+02" - значение.</para>
            /// <para>Пример: Подключёны три датчика к позициям 2,3 и 5.</para>
            /// <para>Датчик в позиции 2 в режиме напряжения, а датчик в позиции 5 не включён</para>
            /// <para>[0] - "2" - позиция,</para>
            /// <para>[1] - "6.546" - значение,</para>
            /// <para>[2] - "3" - позиция,</para>
            /// <para>[3] - "2.7245e-04" - значение,</para>
            /// <para>[4] - "5" - позиция,</para>
            /// <para>[5] - "9.9000e+09" - значение.</para>
            /// </summary>
            public static string[] values
            {
                get
                {
                    response = toTIC_QV(CodeLists.Address.GaugeValues);
                    try { return response; }
                    catch { MC.Service.trace("TIC.GaugeValues.get(): Ошибка данных!"); }
                    return dummy;
                }
            }
            #endregion
        }
        public static Gauge Gauge_1 = new Gauge(CodeLists.Address.Gauge_1);
        public static Gauge Gauge_2 = new Gauge(CodeLists.Address.Gauge_2);
        public static Gauge Gauge_3 = new Gauge(CodeLists.Address.Gauge_3);
        //public string
        #endregion
        #region Relays
        public class Relay
        {
            ushort SelfAddress;
            public Relay(ushort address) { SelfAddress = address; }
            /// <summary>
            /// Возвращает состояние реле
            /// </summary>
            public string value
            {
                get
                {
                    response = toTIC_QV(SelfAddress);
                    try
                    {
                        //response[1] = CodeBase.AlertID.findMeaning(response[1]);
                        //response[2] = CodeBase.Priority.findMeaning(response[2]);
                        return CodeBase.State.findMeaning(response[0]);
                    }
                    catch { MC.Service.trace("TIC." + CodeBase.Addresses.findMeaning(SelfAddress.ToString()) + ".value.get(): Ошибка данных!"); }
                    return "?";
                }
            }
            /// <summary>
            /// Возвращает установки
            /// </summary>
            /// <param name="Master">object ID ("Gauge_1", "Gauge_2", "Gauge_3")</param>
            /// <param name="units_type">"VOLTAGE", "PRESSURE" или "PERCENT"</param>
            /// <param name="on_setpoint">on setpoint</param>
            /// <param name="off_setpoint">off setpoint</param>
            /// <param name="enable">"On" или "Off"</param>
            public void setup(ref string Master, ref string units_type, ref string on_setpoint, ref string off_setpoint, ref string enable)
            {
                response = toTIC_QS(SelfAddress, new string[] { });
                try
                {
                    Master = CodeBase.Addresses.findMeaning(response[0]);
                    units_type = CodeBase.SNVTvalues.findMeaning(response[1]);
                    on_setpoint = response[2];
                    off_setpoint = response[3];
                    enable = CodeBase.Turn.findMeaning(response[4]);
                }
                catch { MC.Service.trace("TIC." + CodeBase.Addresses.findMeaning(SelfAddress.ToString()) + ".setup(): Ошибка данных!"); }
            }
            /// <summary>
            /// Настраивает установки
            /// </summary>
            /// <param name="Master">object ID ("Gauge_1", "Gauge_2", "Gauge_3")</param>
            /// <param name="units_type">"VOLTAGE", "PRESSURE" или "PERCENT"(CodeList.SNTvalues)</param>
            /// <param name="on_setpoint">on setpoint
            /// <para>Напряжение - 0.000 - 9.999</para>
            /// <para>Давление - float</para></param>
            /// <param name="off_setpoint">off setpoint
            /// <para>Напряжение 0.000 - 9.999</para>
            /// <para>Давление - float</para></param>
            /// <param name="enable">"On" или "Off" (CodeList.Turn)</param>
            public void setup(string Master, string units_type, string on_setpoint, string off_setpoint, string enable)
            {
                string[] wDATA = new string[5];
                wDATA[0] = CodeBase.Addresses.find(Master);
                wDATA[1] = CodeBase.SNVTvalues.find(units_type);
                wDATA[2] = on_setpoint;
                wDATA[3] = off_setpoint;
                wDATA[4] = CodeBase.Turn.find(enable);
                toTIC_SS(SelfAddress, wDATA);
            }
            /// <summary>
            /// Включает или выключает реле
            /// </summary>
            /// <param name="OnOrOff">"On" или "Off" (CodeList.Turn)</param>
            public void turn(string OnOrOff) { toTIC_C(SelfAddress, CodeBase.Turn.find(OnOrOff)); }
        }
        public static Relay Relay_1 = new Relay(CodeLists.Address.Relay_1);
        public static Relay Relay_2 = new Relay(CodeLists.Address.Relay_2);
        public static Relay Relay_3 = new Relay(CodeLists.Address.Relay_3);
        #endregion
        #region PS_Temperature
        /// <summary>
        /// Возвращает температуру источника питания
        /// </summary>
        public static string PS_Temperature
        {
            get
            {
                response = toTIC_QV(CodeLists.Address.PS_Temperature);
                try
                {
                    //response[1] = CodeBase.AlertID.findMeaning(response[1]);
                    //response[2] = CodeBase.Priority.findMeaning(response[2]);
                    return response[0];
                }
                catch { MC.Service.trace("TIC.PS_Temperature.get(): Ошибка данных!"); }
                return "?";
            }
        }
        #endregion
        #region Internal_Temperature
        /// <summary>
        /// Возвращает температуру внутри TIC'а
        /// </summary>
        public static string Internal_Temperature
        {
            get
            {
                response = toTIC_QV(CodeLists.Address.InternalTemperature);
                try
                {
                    //response[1] = CodeBase.AlertID.findMeaning(response[1]);
                    //response[2] = CodeBase.Priority.findMeaning(response[2]);
                    return response[0];
                }
                catch { MC.Service.trace("TIC.Internal_Temperature.get(): Ошибка данных!"); }
                return "?";
            }
        }
        #endregion
        #region Analogue_out
        public static class Analogue_out
        {
            /// <summary>
            /// Возвращает значение 0...255
            /// </summary>
            public static string value
            {
                get
                {
                    response = toTIC_QV(CodeLists.Address.Analogue_out);
                    try
                    {
                        //response[1] = CodeBase.AlertID.findMeaning(response[1]);
                        //response[2] = CodeBase.Priority.findMeaning(response[2]);
                        return response[0];
                    }
                    catch { MC.Service.trace("TIC.Analogue_out.value.get(): Ошибка данных!"); }
                    return "?";
                }
            }
            /// <summary>
            /// Возвращает и устанавливает analogue out source ("Gauge_1","Gauge_2","Gauge_3" или "Turbo_speed")
            /// </summary>
            public static string setup
            {
                get
                {
                    response = toTIC_QS(CodeLists.Address.Analogue_out, dummy);
                    try { return response[0]; }
                    catch { MC.Service.trace("TIC.Analogue_out.setup(): Ошибка данных!"); }
                    return "?";
                }
                set { toTIC_SS(CodeLists.Address.Analogue_out, new string[] { CodeBase.Addresses.find(value) }); }
            }
        }
        #endregion
        #region External_vent_valve
        public static class External_vent_valve
        {
            #region Base
            public struct ListOfSetups
            {
                public const string On_stop = "0";
                public const string On_50 = "1";
            }
            #endregion
            //--
            /// <summary>
            /// Возвращает состояние "On" или "Off"
            /// </summary>
            public static string value
            {
                get
                {
                    response = toTIC_QV(CodeLists.Address.External_vent_valve);
                    try
                    {
                        //response[1] = CodeBase.AlertID.findMeaning(response[1]);
                        //response[2] = CodeBase.Priority.findMeaning(response[2]);
                        return CodeBase.State.findMeaning(response[0]);
                    }
                    catch { MC.Service.trace("TIC.External_vent_valve.value.get(): Ошибка данных!"); }
                    return "?";
                }
            }
            /// <summary>
            /// Возвращает "On stop" или "On 50%"
            /// <para>Устанавливать значение при помощи External_vent_valve.ListOfSetups</para>
            /// </summary>
            public static string setup
            {
                get
                {
                    response = toTIC_QS(CodeLists.Address.External_vent_valve, dummy);
                    try
                    {
                        switch (response[0])
                        {
                            case "0": return "On stop";
                            case "1": return "On 50%";
                            default: MC.Service.trace("TIC.External_vent_valve.setup(): Ошибка! Неверное состояние: " + response[0]);
                                return "?";
                        }
                    }
                    catch { MC.Service.trace("TIC.External_vent_valve.setup(): Ошибка данных!"); }
                    return "?";
                }
                set { toTIC_SS(CodeLists.Address.External_vent_valve, new string[] { value }); }
            }
        }
        #endregion
        #region Heater_band
        public static class Heater_band
        {
            /// <summary>
            /// Возвращает значение 0...2100 mins
            /// </summary>
            /// <param name="state">"On" или "Off"</param>
            /// <returns></returns>
            public static string value(ref string state)
            {
                response = toTIC_QV(CodeLists.Address.Heater_band);
                try
                {
                    state = CodeBase.State.findMeaning(response[1]);
                    //response[2] = CodeBase.AlertID.findMeaning(response[2]);
                    //response[3] = CodeBase.Priority.findMeaning(response[3]);
                    return response[0];
                }
                catch { MC.Service.trace("TIC.Heater_band.value.get(): Ошибка данных!"); }
                return "?";
            }
            /// <summary>
            /// Возвращает или устанавливает on time в часах 0...35
            /// </summary>
            public static string setup
            {
                get
                {
                    try { return toTIC_QS(CodeLists.Address.Heater_band, dummy)[0]; }
                    catch { MC.Service.trace("TIC.Heater_band.setup(): Ошибка данных!"); }
                    return "?";
                }
                set
                {
                    toTIC_SS(CodeLists.Address.Heater_band, new string[] { value });
                }
            }
            /// <summary>
            /// Включает или выключает Heater_band
            /// </summary>
            /// <param name="OnOrOff">"On" или "Off" (CodeList.Turn)</param>
            public static void turn(string OnOrOff)
            {
                toTIC_C(CodeLists.Address.Heater_band, CodeBase.Turn.find(OnOrOff));
            }
        }
        #endregion
        #region External_Air_Cooler
        public static class External_Air_Cooler
        {
            public struct ListOfSetups
            {
                public const string Permanent = "0";
                public const string Turbo_slaved = "1";
            }
            //--
            /// <summary>
            /// Возвращает состояние External_Air_Cooler
            /// </summary>
            public static string state
            {
                get
                {
                    response = toTIC_QV(CodeLists.Address.External_Air_Cooler);
                    try
                    {
                        //response[1] = CodeBase.AlertID.findMeaning(response[1]);
                        //response[2] = CodeBase.Priority.findMeaning(response[2]);
                        return CodeBase.State.findMeaning(response[0]);
                    }
                    catch { MC.Service.trace("TIC.External_Air_Cooler.value.get(): Ошибка данных!"); }
                    return "?";
                }
            }
            /// <summary>
            /// Возвращает "Permanent" или "Turbo slaved".
            /// <para>Устанавливать при помощи External_Air_Cooler.ListOfSetups</para>
            /// </summary>
            public static string setup
            {
                get
                {
                    response = toTIC_QS(CodeLists.Address.External_Air_Cooler, dummy);
                    try
                    {
                        switch (response[0])
                        {
                            case ListOfSetups.Turbo_slaved: return "Turbo slaved";
                            case ListOfSetups.Permanent: return "Permanent";
                            default: MC.Service.trace("TIC.External_Air_Cooler.setup(): Ошибка! Неверный ответ: " + response[0]);
                                return "?";
                        }
                    }
                    catch { MC.Service.trace("TIC.External_Air_Cooler.setup(): Ошибка данных!"); }
                    return "?";
                }
                set
                {
                    toTIC_SS(CodeLists.Address.External_Air_Cooler, new string[] { value });
                }
            }
        }
        #endregion
        #region Display_contrast
        /// <summary>
        /// Возвращает и устанавливает контраст диспея от "-5" до "15"
        /// </summary>
        public static string Display_contrast
        {
            get
            {
                response = toTIC_QS(CodeLists.Address.Display_contrast, dummy);
                try { return response[0]; }
                catch { MC.Service.trace("TIC.Display_contrast.get(): Ошибка данных!"); }
                return "?";
            }
            set { toTIC_SS(CodeLists.Address.Display_contrast, new string[] { value }); }
        }
        #endregion
        #region Configuration_Operations
        /// <summary>
        /// Default TIC. 
        /// <para>Default Turbo send to DX or nEXT pump.</para>
        /// <para>Load config to\from Turbo (DX or nEXT)</para>
        /// </summary>
        /// <param name="command">смотри CodeList.CommandList</param>
        /// <returns></returns>
        public static void Configuration_Operations(string command) { toTIC_C(CodeLists.Address.Configuration_Operations, command); }
        #endregion
        #region Pressure_Units
        /// <summary>
        /// Отображаемые единицы измерения давления на дисплее
        /// </summary>
        public static class Pressure_Units
        {
            public struct ListOfSetups
            {
                public const string kPa = "1";
                public const string mbar = "2";
                public const string Torr = "3";
            }
            /// <summary>
            /// Возвращает "kPa", "mbar" или "Torr".
            /// <para>Устанавливать при помощи Pressure_Units.ListOfSetups</para>
            /// </summary>
            public static string setup
            {
                get
                {
                    response = toTIC_QS(CodeLists.Address.Pressure_Units, dummy);
                    try
                    {
                        switch (response[0])
                        {
                            case ListOfSetups.kPa: return "kPa";
                            case ListOfSetups.mbar: return "mbar";
                            case ListOfSetups.Torr: return "Torr";
                            default: MC.Service.trace("TIC.Pressure_Units.setup(): Ошибка! Неверный ответ: " + response[0]);
                                return "?";
                        }
                    }
                    catch { MC.Service.trace("TIC.Pressure_Units.setup(): Ошибка данных!"); }
                    return "?";
                }
                set { toTIC_SS(CodeLists.Address.Pressure_Units, new string[] { value }); }
            }
        }
        #endregion
        #region Fixed_or_Float_ASG
        public static class Fixed_or_Float_ASG
        {
            public struct ListOfSetups
            {
                public const string Float = "1";
                public const string Fixed = "0";
            }
            /// <summary>
            /// Возвращает "Fixed" или "Float".
            /// <para>Устанавливать при помощи Fixed_or_Float_ASG.ListOfSetups</para>
            /// </summary>
            public static string setup
            {
                get
                {
                    response = toTIC_QS(CodeLists.Address.Fixed_or_Float_ASG, dummy);
                    try
                    {
                        switch (response[0])
                        {
                            case "0": return "Fixed";
                            case "1": return "Float";
                            default: MC.Service.trace("TIC.Fixed_or_Float_ASG.setup(): Ошибка! Неверный ответ: " + response[0]);
                                return "?";
                        }
                    }
                    catch { MC.Service.trace("TIC.Fixed_or_Float_ASG.setup(): Ошибка данных!"); }
                    return "?";
                }
                set { toTIC_SS(CodeLists.Address.Fixed_or_Float_ASG, new string[] { value }); }
            }
        }
        #endregion
        #region System
        public static class System
        {
            public struct ListOfMarks
            {
                public const byte notMarked = 1;
                public const byte markOn = 2;
                public const byte markOff = 3;
                public const byte markOnAndOff = 4;
            }
            static void getBytesFromMark(byte Mark, ref string On, ref string Off)
            {
                switch (Mark)
                {
                    case 1:
                        On = "0";
                        Off = "0";
                        break;
                    case 2:
                        On = "1";
                        Off = "0";
                        break;
                    case 3:
                        On = "0";
                        Off = "1";
                        break;
                    case 4:
                        On = "1";
                        Off = "1";
                        break;
                    default: MC.Service.trace("TIC.System_getBytesFromMark(...): Ошибка! Неправильное входное значение: " + Mark);
                        break;
                }
            }
            static string getMarkFromBytes(string On, string Off)
            {
                switch (On)
                {
                    case "0":
                        switch (Off)
                        {
                            case "0":
                                return "not marked";
                            case "1":
                                return "marked to turn off";
                            default: MC.Service.trace("TIC.System_getMarkFromBytes(...): Ошибка! Неправильное входное значение Off: " + Off);
                                break;
                        }
                        break;
                    case "1":
                        switch (Off)
                        {
                            case "0":
                                return "marked to turn on";
                            case "1":
                                return "marked to turn on and off";
                            default: MC.Service.trace("TIC.System_getMarkFromBytes(...): Ошибка! Неправильное входное значение Off: " + Off);
                                break;
                        }
                        break;
                    default: MC.Service.trace("TIC.System_getMarkFromBytes(...): Ошибка! Неправильное входное значение On: " + On);
                        break;
                }
                return "?";
            }
            /// <summary>
            /// Устанавливает метки на включение и на выключение.
            /// <para>Метки выбирать из System.ListOfMarks</para>
            /// </summary>
            public static void setup(byte Turbo, byte Back, byte Gauge_1, byte Gauge_2, byte Gauge_3, byte Relay_1, byte Relay_2, byte Relay_3)
            {
                string[] wDATA = new string[24];
                wDATA[0] = CodeLists.Address.Turbo_Pump.ToString();
                getBytesFromMark(Turbo, ref wDATA[1], ref wDATA[2]);
                wDATA[3] = CodeLists.Address.Backing_Pump.ToString();
                getBytesFromMark(Back, ref wDATA[4], ref wDATA[5]);
                wDATA[6] = CodeLists.Address.Gauge_1.ToString();
                getBytesFromMark(Gauge_1, ref wDATA[7], ref wDATA[8]);
                wDATA[9] = CodeLists.Address.Gauge_2.ToString();
                getBytesFromMark(Gauge_2, ref wDATA[10], ref wDATA[11]);
                wDATA[12] = CodeLists.Address.Gauge_3.ToString();
                getBytesFromMark(Gauge_3, ref wDATA[13], ref wDATA[14]);
                wDATA[15] = CodeLists.Address.Relay_1.ToString();
                getBytesFromMark(Relay_1, ref wDATA[16], ref wDATA[17]);
                wDATA[18] = CodeLists.Address.Relay_2.ToString();
                getBytesFromMark(Relay_2, ref wDATA[19], ref wDATA[20]);
                wDATA[21] = CodeLists.Address.Relay_3.ToString();
                getBytesFromMark(Relay_3, ref wDATA[22], ref wDATA[23]);
                response = toTIC_SS(CodeLists.Address.System, wDATA);
            }
            /// <summary>
            /// Возвращает метки на включение и выключение в виде "Объект метка"
            /// <para>Метки: "not marked", "marked to turn off", "marked to turn on", "marked to turn on and off" </para>
            /// </summary>
            public static void setup(ref string Turbo, ref string Back, ref string Gauge_1, ref string Gauge_2, ref string Gauge_3, ref string Relay_1, ref string Relay_2, ref string Relay_3)
            {
                response = toTIC_QS(CodeLists.Address.System, dummy);
                try
                {
                    string[] answer = new string[8];
                    for (byte i = 0; i < 8; i++)
                    {
                        answer[i] = CodeBase.Addresses.findMeaning(response[i * 3]) + " "
                            + getMarkFromBytes(response[i * 3 + 1], response[i * 3 + 2]);
                    }
                }
                catch { MC.Service.trace("TIC.System.setup(): Ошибка данных!"); }
            }
            /// <summary>
            /// Включает или выключает все помеченные объекты
            /// </summary>
            /// <param name="OnOrOff">"On" или "Off" (CodeList.Turn)</param>
            public static void turn(string OnOrOff) { toTIC_C(CodeLists.Address.System, CodeBase.Turn.find(OnOrOff)); }
            /// <summary>
            /// Возвращает состояние
            /// </summary>
            public static string state
            {
                get
                {
                    response = toTIC_QV(CodeLists.Address.System);
                    try
                    {
                        //response[1] = CodeBase.AlertID.findMeaning(response[1]);
                        //response[2] = CodeBase.Priority.findMeaning(response[2]);
                        return CodeBase.State.findMeaning(response[0]);
                    }
                    catch { MC.Service.trace("TIC.System.state.get(): Ошибка данных!"); }
                    return "?";
                }
            }
        }
        #endregion
        //--EXT75DX
        #region EXT75DX
        public static class EXT75DX
        {
            #region Node
            /// <summary>
            /// Node. Get/set multi-drop 0...98
            /// </summary>
            public static string Node
            {
                get
                {
                    response = toTIC_QS(CodeLists.Address.EXT75DX.Node, dummy);
                    try { return response[0]; }
                    catch { MC.Service.trace("TIC.EXT75DX.Node.get(): Ошибка данных!"); }
                    return "?";
                }
                set { toTIC_SS(CodeLists.Address.EXT75DX.Node, new string[] { value }); }
            }
            #endregion
            #region Pump
            public static class Pump
            {
                static string[] decryptSystemStatusWord(string SystemStatusWord)
                {
                    string[] answer = new string[16];
                    string binaryWord = "";
                    foreach (char ch in SystemStatusWord)
                    {
                        binaryWord += Pifagor.Hex.toBin.String(ch);
                    }
                    // Bit [0] - Fail (<0> - The pump has not failed, <1> - Fail status condition active)
                    if (binaryWord[31] == '1') { answer[0] = "The pump has failed!"; }
                    else { answer[0] = "The pump has not failed."; }
                    // Bit [1] - Stopped speed (<0> - Above stopped speed, <1> - Below stopped speed)
                    if (binaryWord[30] == '1') { answer[1] = "Speed is below stopped speed."; }
                    else { answer[1] = "Speed is above stopped speed."; }
                    // Bit [2] - Normal speed (<0> - Below normal speed, <1> - Above normal speed)
                    if (binaryWord[29] == '1') { answer[2] = "Speed is above normal speed."; }
                    else { answer[2] = "Speed is below normal speed."; }
                    // Bit [3] - Vent valve closed (<0> - Vent valve is open, <1> - Vent valve is close)
                    if (binaryWord[28] == '1') { answer[3] = "Vent valve is close."; }
                    else { answer[3] = "Vent valve is open."; }
                    // Bit [4] - Start (<0> - There is no active Start command, <1> - Start command is active)
                    if (binaryWord[27] == '1') { answer[4] = "Start command is active."; }
                    else { answer[4] = "There is no active Start command."; }
                    // Bit [5] - Serial enable (<0> - Serial enable is not active, <1> - Serial enable is active)
                    if (binaryWord[26] == '1') { answer[5] = "Serial enable is active."; }
                    else { answer[5] = "Serial enable is not active."; }
                    // Bit [6] - Standby (<0> - Standby is not active, <1> - Standby is active)
                    if (binaryWord[25] == '1') { answer[6] = "Standby is active."; }
                    else { answer[6] = "Standby is not active."; }
                    // Bit [7] - Half full speed (<0> - Speed is below 50% of full rotation speed, <1> - Speed is above 50% of full rotation speed)
                    if (binaryWord[24] == '1') { answer[7] = "Speed is above 50% of full rotation speed."; }
                    else { answer[7] = "Speed is below 50% of full rotation speed."; }
                    // Bit [8] - Parallel control mode (<0> - The pump is not in parallel control mode, <1> -  The pump is in parallel control mode)
                    if (binaryWord[23] == '1') { answer[8] = "The pump is in parallel control mode."; }
                    else { answer[8] = "The pump is not in parallel control mode."; }
                    // Bit [9] - Serial control mode (<0> - The pump is not in serial control mode, <1> - The pump is in serial control mode)
                    if (binaryWord[22] == '1') { answer[9] = "The pump is in serial control mode."; }
                    else { answer[9] = "The pump is not in serial control mode."; }
                    // Bit [10] - Invalid Podule software (<0> - There is no Podule internal software mismatch, <1> -  Podule internal software mismatch)
                    if (binaryWord[21] == '1') { answer[10] = "Podule internal software mismatch."; }
                    else { answer[10] = "There is no Podule internal software mismatch."; }
                    // Bit [11] - Podule upload incomplete (<0> - Podule passed internal configuration and calibration operation, <1> -  Podule failed internal configuration and calibration operation)
                    if (binaryWord[20] == '1') { answer[11] = "Podule failed internal configuration and calibration operation."; }
                    else { answer[11] = "Podule passed internal configuration and calibration operation."; }
                    // Bit [12] - Timer expired (<0> -  The timer has not timed out, <1> - The timer has timed out)
                    if (binaryWord[19] == '1') { answer[12] = "The timer has timed out."; }
                    else { answer[12] = "The timer has not timed out."; }
                    // Bit [13] - Hardware trip (<0> - Overspeed and Overcurrent trip not activated, <1> - Overspeed or Overcurrent trip activated)
                    if (binaryWord[18] == '1') { answer[13] = "Overspeed or Overcurrent trip activated."; }
                    else { answer[13] = "Overspeed and Overcurrent trip not activated."; }
                    // Bit [14] - Thermistor error (<0> - Pump internal temperature measurement system is fine, <1> -  Pump internal temperature measurement system disconnected or damaged)
                    if (binaryWord[17] == '1') { answer[14] = "Pump internal temperature measurement system disconnected or damaged."; }
                    else { answer[14] = "Pump internal temperature measurement system is fine."; }
                    // Bit [15] - Serial control mode interlock (<0> - Serial enable has not become inactive during serial control, <1> -  Serial enable has become inactive following a serial Start command.)
                    if (binaryWord[16] == '1') { answer[15] = "Serial enable has become inactive following a serial Start command."; }
                    else { answer[15] = "Serial enable has not become inactive during serial control."; }
                    return answer;
                }
                //----
                /// <summary>
                /// Возвращает тип, версию и полную скорость
                /// </summary>
                /// <param name="DSP_sw_ver">DSP software version number (D39647631x for EXT75DX)</param>
                /// <param name="Full_speed_RPS">1500 for EXT75DX</param>
                /// <returns></returns>
                public static string type(ref string DSP_sw_ver, ref string Full_speed_RPS)
                {
                    response = toTIC_QS(CodeLists.Address.EXT75DX.Pump_type, dummy);
                    try
                    {
                        DSP_sw_ver = response[1];
                        Full_speed_RPS = response[2];
                        return response[0];
                    }
                    catch { MC.Service.trace("TIC.EXT75DX.Pump.type.get(): Ошибка данных!"); }
                    return "?";
                }
                /// <summary>
                /// Включает или выключает насос.
                /// <para>"On": The pump will then accelerate to full operating speed. </para>
                /// <para>The green indicator LED will illuminate when the pump reaches Normal speed.</para> 
                /// <para>(This is 80% of full rotational speed by default but you may have selected</para> 
                /// <para>a different value to suit your application).</para>
                /// </summary>
                /// <param name="OnOrOff">"On" или "Off" (CodeList.Turn)</param>
                public static void turn(string OnOrOff) { toTIC_C(CodeLists.Address.EXT75DX.Pump_control, CodeBase.Turn.find(OnOrOff)); }
                /// <summary>
                /// Returns system status word and (ref) measured motor speed.
                /// The System Status Word is a 32-bit Word (set of 8 hexadecimal characters) which is useful
                /// for fault-finding. Refer to Section 4.5.2 for advice on decoding the System Status Word.
                /// </summary>
                /// <param name="Measured_motor_speed">RPS</param>
                /// <returns>System status word</returns>
                public static string[] control(ref string Measured_motor_speed)
                {
                    response = toTIC_QV(CodeLists.Address.EXT75DX.Pump_type);
                    try
                    {
                        Measured_motor_speed = response[0];

                        return decryptSystemStatusWord(response[1]);
                    }
                    catch { MC.Service.trace("TIC.EXT75DX.Pump.control.get(): Ошибка данных!"); }
                    return dummy;
                }
            }
            #endregion
            #region Vent_options
            static DataBase CodeBase_Local_Vent_options = new DataBase
            (
                "Vent_options.CodeBase_Local",
                new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8" },
                new string[]
                        {
                            "Hard vent only when <50% speed",
                            "Controlled vent if >50% speed or hard vent if <50% speed",
                            "Hard vent if stop or hard vent if fail and <50% speed",
                            "Hard vent if stop or controlled vent if fail and >50% speed or hard vent if fail and <50% speed",
                            "Hard vent if fail or hard vent if stop and <50% speed",
                            "Hard vent if fail or controlled vent if stop and >50% speed or hard vent if stop and <50% speed",
                            "Hard vent if stop or fail",
                            "Same as option 6",
                            "Vent = Permanently Enabled (Fan)"
                        }
            );
            /// <summary>
            /// Возвращает описание настройки вентилятора.
            /// <para>При установке использовать значение value из таблицы ниже</para>
            /// <para>value | Available vent options</para>
            /// <para>------|-------------------------------------------------</para>
            /// <para>&#160;&#160;"0"&#160;&#160;| Hard vent only when &lt;50% speed (default),</para>
            /// <para>&#160;&#160;"1"&#160;&#160;| Controlled vent if &gt;50% speed or hard vent if &lt;50% speed,</para>
            /// <para>&#160;&#160;"2"&#160;&#160;| Hard vent if stop or hard vent if fail and &lt;50% speed,</para>
            /// <para>&#160;&#160;"3"&#160;&#160;| Hard vent if stop or controlled vent if fail and &gt;50% speed or hard vent if fail and &lt;50% speed,</para>
            /// <para>&#160;&#160;"4"&#160;&#160;| Hard vent if fail or hard vent if stop and &lt;50% speed,</para>
            /// <para>&#160;&#160;"5"&#160;&#160;| Hard vent if fail or controlled vent if stop and &gt;50% speed or hard vent if stop and &lt;50% speed,</para>
            /// <para>&#160;&#160;"6"&#160;&#160;| Hard vent if stop or fail,</para>
            /// <para>&#160;&#160;"7"&#160;&#160;| Same as option 6,</para>
            /// <para>&#160;&#160;"8"&#160;&#160;| Vent = Permanently Enabled (Fan)</para>
            /// </summary>
            public static string Vent_options
            {
                get
                {
                    response = toTIC_QS(TIC.CodeLists.Address.EXT75DX.Vent_options, dummy);
                    try { return CodeBase_Local_Vent_options.findMeaning(response[0]); }
                    catch { MC.Service.trace("TIC.EXT75DX.Vent_options: Ошибка данных!"); }
                    return "?";
                }
                set { toTIC_SS(TIC.CodeLists.Address.EXT75DX.Vent_options, new string[] { value }); }
            }
            #endregion
            #region Timer
            public static class Timer
            {
                /// <summary>
                /// Timeout period (minutes) for both initial ramp up and if speed drops below 50% 
                /// <para> value = 1...30</para>
                /// </summary>
                public static string setting
                {
                    get
                    {
                        response = toTIC_QS(TIC.CodeLists.Address.EXT75DX.Timer_setting, dummy);
                        try { return response[0]; }
                        catch { MC.Service.trace("TIC.EXT75DX.Timer.setting: Ошибка данных!"); }
                        return "?";
                    }
                    set { toTIC_SS(TIC.CodeLists.Address.EXT75DX.Timer_setting, new string[] { value }); }
                }
                /// <summary>
                /// Возвращает состояние таймера (вкл\выкл) (по умолчанию включен)
                /// <para>При установке "1" - включает таймер, "0" - выключает</para>
                /// <para>Note that the timer is permanently enabled on ramp-up.</para>
                /// </summary>
                public static string options
                {
                    get
                    {
                        response = toTIC_QS(TIC.CodeLists.Address.EXT75DX.Timer_options, dummy);
                        try
                        {
                            if (response[0] == "1")
                            {
                                return "Timer enabled";
                            }
                            else
                            {
                                return "Timer disabled";
                            }
                        }
                        catch { MC.Service.trace("TIC.EXT75DX.Timer.options: Ошибка данных!"); }
                        return "?";
                    }
                    set { toTIC_SS(TIC.CodeLists.Address.EXT75DX.Timer_options, new string[] { value }); }
                }
            }
            #endregion
            #region Power_limit_setting
            /// <summary>
            /// Link power maximum EXT75DX
            /// <para>Возвращает значение максимальной мощности в Ваттах (по умолчанию 80)</para>
            /// <para>При установке value = 50...120</para>
            /// </summary>
            public static string Power_limit_setting
            {
                get
                {
                    response = toTIC_QS(TIC.CodeLists.Address.EXT75DX.Power_limit_setting, dummy);
                    try { return response[0]; }
                    catch { MC.Service.trace("TIC.EXT75DX.Power_limit_setting: Ошибка данных!"); }
                    return "?";
                }
                set { toTIC_SS(TIC.CodeLists.Address.EXT75DX.Power_limit_setting, new string[] { value }); }
            }
            #endregion
            #region Normal_speed_setting
            /// <summary>
            /// Normal speed as a percentage of full speed
            /// <para>Возвращает значение нормальной скорости в процентах от максимальной (по умолчанию 80%)</para>
            /// <para>При установке value = 50...100</para>
            /// </summary>
            public static string Normal_speed_setting
            {
                get
                {
                    response = toTIC_QS(TIC.CodeLists.Address.EXT75DX.Normal_speed_setting, dummy);
                    try { return response[0]; }
                    catch { MC.Service.trace("TIC.EXT75DX.Normal_speed_setting: Ошибка данных!"); }
                    return "?";
                }
                set { toTIC_SS(TIC.CodeLists.Address.EXT75DX.Normal_speed_setting, new string[] { value }); }
            }
            #endregion
            #region Standby_speed_setting
            /// <summary>
            /// Standby speed as a percentage of full speed
            /// <para>Возвращает значение нормальной скорости в процентах от максимальной (по умолчанию 70%)</para>
            /// <para>При установке value = 55...100</para>
            /// </summary>
            public static string Standby_speed_setting
            {
                get
                {
                    response = toTIC_QS(TIC.CodeLists.Address.EXT75DX.Standby_speed_setting, dummy);
                    try { return response[0]; }
                    catch { MC.Service.trace("TIC.EXT75DX.Standby_speed_setting: Ошибка данных!"); }
                    return "?";
                }
                set { toTIC_SS(TIC.CodeLists.Address.EXT75DX.Standby_speed_setting, new string[] { value }); }
            }
            #endregion
            #region Temperature_readings
            /// <summary>
            /// Возвращает температуры от 0&#x2070;С до 100&#x2070;С 
            /// </summary>
            /// <param name="motor_t">Measured motor temperature</param>
            /// <param name="controller_t">Measured controller temperature</param>
            public static void Temperature_readings(ref string motor_t, ref string controller_t)
            {
                response = toTIC_QV(TIC.CodeLists.Address.EXT75DX.Temperature_readings);
                try
                {
                    motor_t = response[0];
                    controller_t = response[1];
                }
                catch { MC.Service.trace("TIC.EXT75DX.Temperature_readings: Ошибка данных!"); }

            }
            #endregion
            #region Link_parameter_readings
            /// <summary>
            /// Возвращает параметры.
            /// </summary>
            /// <param name="voltage">Measured link voltage от 0 до 500 в единицах: 0.1В</param>
            /// <param name="current">Measured link current от 0 до 300 в единицах: 0.1А</param>
            /// <param name="power">Measured link power от 0 до 15000 в единицах: 0.1Вт</param>
            public static void Link_parameter_readings(ref string voltage, ref string current, ref string power)
            {
                response = toTIC_QV(TIC.CodeLists.Address.EXT75DX.Link_parameter_readings);
                try
                {
                    voltage = response[0];
                    current = response[1];
                    power = response[2];
                }
                catch { MC.Service.trace("TIC.EXT75DX.Link_parameter_readings: Ошибка данных!"); }

            }
            #endregion
            #region Factory_settings
            /// <summary>
            /// Reset all configuration options and parameters to the factory settings
            /// </summary>
            public static void Factory_settings() { toTIC_SS(TIC.CodeLists.Address.EXT75DX.Factory_settings, new string[] { "1" }); }
            #endregion
            #region PIC_software_version
            /// <summary>
            /// PIC software version number (D39647620x) - 10 chars
            /// </summary>
            public static string PIC_software_version
            {
                get
                {
                    response = toTIC_QS(TIC.CodeLists.Address.EXT75DX.PIC_software_version, dummy);
                    try { return response[0]; }
                    catch { MC.Service.trace("TIC.EXT75DX.Standby_speed_setting: Ошибка данных!"); }
                    return "?";
                }
            }
            #endregion
            #region Speed_control
            /// <summary>
            /// Устанавливает целевую скорость
            /// <para>value | target speed:</para>
            /// <para>------|-------------------------------------------------</para>
            /// <para>&#160;&#160;"0"&#160;&#160;| Set target speed to full speed,</para>
            /// <para>&#160;&#160;"1"&#160;&#160;| Set target speed to standby speed</para>
            /// </summary>
            public static string Speed_control
            {
                set { toTIC_C(TIC.CodeLists.Address.EXT75DX.Speed_control, value); }
            }
            #endregion
            #region Analogue_signal_options
            /// <summary>
            /// Возвращает analogue output.
            /// <para>При установке использовать значение value из таблицы ниже</para>
            /// <para>value | analogue output</para>
            /// <para>------|-------------------------------------------------</para>
            /// <para>&#160;&#160;"0"&#160;&#160;| Measured speed (default),</para>
            /// <para>&#160;&#160;"1"&#160;&#160;| Measured power,</para>
            /// <para>&#160;&#160;"2"&#160;&#160;| Measured motor temperature,</para>
            /// <para>&#160;&#160;"3"&#160;&#160;| measured control temperature</para>
            /// </summary>
            public static string Analogue_signal_options
            {
                get
                {
                    response = toTIC_QS(TIC.CodeLists.Address.EXT75DX.Analogue_signal_options, dummy);
                    try { return response[0]; }
                    catch { MC.Service.trace("TIC.EXT75DX.Analogue_signal_options: Ошибка данных!"); }
                    return "?";
                }
                set { toTIC_SS(TIC.CodeLists.Address.EXT75DX.Analogue_signal_options, new string[] { value }); }
            }
            #endregion
            #region Electronic_braking_option
            /// <summary>
            /// Возвращает Electronic_braking_option
            /// <para>При установке "1" - включает, "0" - выключает</para>
            /// </summary>
            public static string Electronic_braking_option
            {
                get
                {
                    response = toTIC_QS(TIC.CodeLists.Address.EXT75DX.Electronic_braking_option, dummy);
                    try
                    {
                        if (response[0] == "1")
                        {
                            return "Electronic braking enabled";
                        }
                        else
                        {
                            return "Electronic braking disabled (default)";
                        }
                    }
                    catch { MC.Service.trace("TIC.EXT75DX.Electronic_braking_option: Ошибка данных!"); }
                    return "?";
                }
                set { toTIC_SS(TIC.CodeLists.Address.EXT75DX.Electronic_braking_option, new string[] { value }); }
            }
            #endregion
            #region Close_vent_valve
            /// <summary>
            /// Closes the vent valve for delayed start and overrides the current vent option.
            /// <para>There is no open vent valve command,</para>
            /// <para>but the stop command (EXT75DX.Pump.turn("Off");) will clear the override.</para>
            /// </summary>
            public static void Close_vent_valve() { toTIC_C(TIC.CodeLists.Address.EXT75DX.Close_vent_valve, "1"); }
            #endregion
        }
        #endregion
        //--
        #region Приём-передача
        //команда
        static void toTIC_C(ushort address, string m)
        {
            transmit_toTIC("!C", address.ToString(), new string[] { m });
        }
        //установка установки
        static string[] toTIC_SS(ushort address, string[] wDATA)
        {
            return transmit_toTIC("!S", address.ToString(), wDATA);
        }
        //запрос установки
        static string[] toTIC_QS(ushort address, string[] wDATA)
        {
            return transmit_toTIC("?S", address.ToString(), wDATA);
        }
        //запрос значения
        static string[] toTIC_QV(ushort address)
        {
            return transmit_toTIC("?V", address.ToString(), new string[] { });
        }

        static string[] transmit_toTIC(string prefix, string address, string[] wDATA)
        {
            //uint TimeOut = 7000;
            //формируем пакет на передачу
            string Packet = prefix + address;       //Добваляем префикс (что это, команда или запрос...) и добавляем адрес
            //Добавляем всяческие данные, если надо
            if (wDATA.Length > 0)
            {
                Packet += ' ';
                foreach (string str in wDATA)
                {
                    Packet += str + ';';
                }
                Packet = Packet.Substring(0, Packet.Length - 1);
            }
            Packet += '\r';                         //Завершаем пакет
            return Decode(Encoding.ASCII.GetString(MC.Service.retransmit_toTIC(Encoding.ASCII.GetBytes(Packet)).ToArray()), address, Packet);
        }
        static string[] Decode(string response, string address, string command)
        {
            //Проверяем на то, что мы получили
            bool ResponseWithData = false;
            bool ResponseIsValue = false;
            try
            {
                switch (response[0])
                {
                    case '*':
                        //Это отклик (нормальный или с ошибкой)
                        switch (response[1])
                        {
                            case 'C':
                                //Это отклик на команду
                                break;
                            case 'S':
                                //Это отклик на установку
                                break;
                            case 'V':
                                //Это сообщение об ошибке на запрос значения
                                break;
                            default: MC.Service.trace("TIC: " + command + " Ошибка! Неверный отклик: " + response);
                                return new string[] { };
                        }
                        break;
                    case '=':
                        //Это отклик на запрос значения или установки
                        switch (response[1])
                        {
                            case 'S':
                                //Это отклик на запрос установки
                                ResponseWithData = true;
                                break;
                            case 'V':
                                //Это отклик на запрос значения
                                ResponseIsValue = true;
                                ResponseWithData = true;
                                break;
                            default: MC.Service.trace("TIC: " + command + " Ошибка! Неверный отклик: " + response);
                                return new string[] { };
                        }
                        break;
                    default: MC.Service.trace("TIC: " + command + " Ошибка! Неверный отклик: " + response);
                        return new string[] { };
                }

                //Проверка адреса
                string received_address = "";
                byte i = 2;
                while (response[i] != ' ')
                {
                    received_address += response[i];
                    i++;
                }
                if (received_address != address)
                {
                    MC.Service.trace("TIC: " + command + " Ошибка! Неверный отклик: " + response + "\rАдреса не совпадают! Ожидалось:" + address + " Получено: " + received_address);
                    return new string[] { };
                }
                i++; //Уходим с пробела
                //Если отклик с данными ('=')
                if (ResponseWithData)
                {
                    //вычисляем количество данных по ';'
                    byte k = 1;
                    foreach (char ch in response)
                    {
                        if (ch == ';')
                        {
                            k++;
                        }
                    }
                    string[] rDATA = new string[k];
                    //Если мы добрались до сюда, то собираем данные
                    byte j = 0;
                    while (response[i] != '\r')
                    {
                        switch (response[i])
                        {
                            case ';':
                                //Предыдущее данное закончилось, сейчас начнётся следующее
                                j++;
                                break;
                            case '.':
                                //Это точка, заменяем её на запятую
                                rDATA[j] += ',';
                                break;
                            default:
                                //Это либо число либо точка, сохраняем в текущее данное
                                rDATA[j] += response[i];
                                break;
                        }
                        i++;
                    }
                    if (ResponseIsValue)
                    {
                        //Последние два данных - AlertID и PriorityID
                        AlertID = rDATA[rDATA.Length - 2].ToString();
                        PriorityID = rDATA[rDATA.Length - 1].ToString();
                        Alert = CodeBase.AlertID.findMeaning(AlertID);
                        Priority = CodeBase.Priority.findMeaning(PriorityID);
                        switch (PriorityID)
                        {
                            case CodeLists.Priority.warning:
                                WarningCall.Invoke(Alert, AlertID);
                                break;
                            case CodeLists.Priority.alert_LOW:
                                Alert_LOW_Call.Invoke(Alert, AlertID);
                                break;
                            case CodeLists.Priority.alert_HIGH:
                                Alert_HIGH_Call.Invoke(Alert, AlertID);
                                break;
                            default:
                                break;
                        }
                    }
                    return rDATA;
                }
                else
                {
                    //Ответ будет в виде rr
                    MC.Service.trace("TIC: Отклик: " + CodeBase.SerialCommsResponseCode.findMeaning(response[i].ToString()));
                    ErrorID = response[i].ToString();
                    Error = CodeBase.SerialCommsResponseCode.findMeaning(ErrorID);
                    if (ErrorID != CodeLists.SerialCommsResponseCode.no_error.ToString())
                    { ErrorCall.Invoke(Error, ErrorID); }
                    return new string[] { };
                }
            }
            catch
            {
                MC.Service.trace("TIC: Ошибка дешифровки данных! " + response);
            }
            return new string[] { };
        }
        #endregion
        //Видимые функции
        /// <summary>
        /// Настраивает МК на опрос датчиков и порогов.
        /// <para>По умолчанию: Включение выского напряжения по датчику "Gauge_1" c нижним порогом 2.000V,</para>
        /// <para>Выключение выского напряжения по датчику "Gauge_2" c верхним порогом 6.700V</para>
        /// </summary>
        /// <param name="onGauge">Датчик включения высокого напряжения.
        /// <para>Варианты: "Gauge_1" (по умолчанию),"Gauge_2","Gauge_3".</para></param>
        /// <param name="onThreshold">Порог (меньше или равно) включения высокого напряжения.</para>
        /// <para>Пример: "2.000" = 2.000V (по умолчанию).</para></param>
        /// <param name="offGauge">Датчик выключения высокого напряжения.
        /// <para>Варианты: "Gauge_1","Gauge_2" (по умолчанию),"Gauge_3".</para></param>
        /// <param name="offThreshold">Порог (больше или равно) выключения высокого напряжения.</para>
        /// <para>Пример: "6.700" = 6.700V (по умолчанию).</para></param>
        public static void setup_HVE_conditions(string onGauge, string onThreshold, string offGauge, string offThreshold)
        {
            string command = "TIC.setup_HVE_conditions.set(" + onGauge + "," + onThreshold + "," + offGauge + "," + offThreshold + ")";
            MC.Service.trace_attached(Environment.NewLine);
            MC.Service.trace(command);
            byte onLevel_1 = (byte)((Pifagor.Hex.toBin.Byte(onThreshold[0]) << 4) + Pifagor.Hex.toBin.Byte(onThreshold[2]));
            byte onLevel_0 = (byte)((Pifagor.Hex.toBin.Byte(onThreshold[3]) << 4) + Pifagor.Hex.toBin.Byte(onThreshold[4]));
            byte offLevel_1 = (byte)((Pifagor.Hex.toBin.Byte(offThreshold[0]) << 4) + Pifagor.Hex.toBin.Byte(offThreshold[2]));
            byte offLevel_0 = (byte)((Pifagor.Hex.toBin.Byte(offThreshold[3]) << 4) + Pifagor.Hex.toBin.Byte(offThreshold[4]));
            byte[] DATA =  
            {
                Encoding.ASCII.GetBytes(CodeBase.Addresses.findValue(onGauge))[2], onLevel_1, onLevel_0,
                Encoding.ASCII.GetBytes(CodeBase.Addresses.findValue(offGauge))[2], offLevel_1, offLevel_0
            };
            MC.Service.transmit(Command.TIC.set_Gauges, DATA);
        }
        /// <summary>
        /// Запрашивает у МК данные по опросу датчиков.
        /// </summary>
        /// <param name="onGauge">Датчик включения высокого напряжения.
        /// <para>Варианты: "Gauge_1" (по умолчанию),"Gauge_2","Gauge_3".</para></param>
        /// <param name="onThreshold">Порог (меньше или равно) включения высокого напряжения.</para>
        /// <para>Пример: "2.000" = 2.000V (по умолчанию).</para></param>
        /// <param name="offGauge">Датчик выключения высокого напряжения.
        /// <para>Варианты: "Gauge_1","Gauge_2" (по умолчанию),"Gauge_3".</para></param>
        /// <param name="offThreshold">Порог (больше или равно) выключения высокого напряжения.</para>
        /// <para>Пример: "6.700" = 6.700V (по умолчанию).</para></param>
        public static void setup_HVE_conditions(ref string onGauge, ref string onThreshold, ref string offGauge, ref string offThreshold)
        {
            //ПАКЕТ: <Command><pin_iHVE><Flags.HVE><onGauge><onLevel[1]><onLevel[0]><offGauge><offLevel[1]><offLevel[1]><monitoringEnabled>
            string command = "TIC.setup_HVE_conditions.get()";
            MC.Service.trace_attached(Environment.NewLine);
            MC.Service.trace(command);
            List<byte> rDATA = MC.Service.transmit(Command.Flags.HVE);
            try
            {
                onGauge = CodeBase.Addresses.findMeaning("91" + Encoding.ASCII.GetString(new byte[] { rDATA[3] }));
                onThreshold = ((byte)(rDATA[4] >> 4) + "." + (byte)(rDATA[4] & 0x0F) + (byte)(rDATA[5] >> 4) + (byte)(rDATA[5] & 0x0F)).ToString();
                offGauge = CodeBase.Addresses.findMeaning("91" + Encoding.ASCII.GetString(new byte[] { rDATA[6] }));
                offThreshold = ((byte)(rDATA[7] >> 4) + "." + (byte)(rDATA[7] & 0x0F) + (byte)(rDATA[8] >> 4) + (byte)(rDATA[8] & 0x0F)).ToString();
            }
            catch { MC.Service.trace("TIC.setup_HVE_conditions.get(): Ошибка данных!"); }
        }
        /// <summary>
        /// Возвращает последнее сообщение, которое прислал TIC на МК (либо опрос HVE, либо ответ на ретрансмит)
        /// </summary>
        /// <returns></returns>
        public static string getTIC_MEM()
        {
            string command = "TIC.getTIC_MEM()";
            MC.Service.trace_attached(Environment.NewLine);
            MC.Service.trace(command);
            List<byte> rDATA = MC.Service.transmit(Command.TIC.get_TIC_MEM);
            char[] answer = Encoding.ASCII.GetChars(rDATA.ToArray<byte>());
            return new string(answer);
        }
    }
}