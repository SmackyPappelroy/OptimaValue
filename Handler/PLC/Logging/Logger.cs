using S7.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;


namespace OptimaValue
{

    public static class Logger
    {
        // Försök att få ner CPU-belastning
        private static Stopwatch cycleTime = new Stopwatch();
        public static int FastestLogTime = int.MaxValue;

        /// <summary>
        /// Lokal tid offset
        /// </summary>
        public static TimeSpan UtcOffset => TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);

        private static object sqlLock = new object();

        public static event EventHandler StartedEvent;
        public static event EventHandler RestartEvent;
        public static System.Timers.Timer RestartTimer = new System.Timers.Timer()
        {
            Interval = 5000,
        };

        private static List<LastValue> lastLogValue;

        private static Thread logThread;

        private static System.Timers.Timer onlineTimer;

        public static bool startClosing = false;
        private static bool AppIsShuttingDown = false;

        public static void OnStartedEvent(EventArgs e)
        {
            StartedEvent?.Invoke(typeof(Logger), e);
        }

        public static void OnRestartEvent(EventArgs e)
        {
            RestartEvent?.Invoke(typeof(Logger), e);
        }

        public static void Start()
        {
            if (!PlcConfig.PlcList.Any(x => x.Active))
                return;

            RestartTimer.Elapsed -= RestartTimer_Elapsed;
            RestartTimer.Elapsed += RestartTimer_Elapsed;

            if (logThread != null)
                logThread = null;

            logThread = new Thread(Cycler);
            logThread.Start();

            foreach (ExtendedPlc MyPlc in PlcConfig.PlcList)
            {
                if (MyPlc.Active)
                {
                    MyPlc.LoggerIsStarted = true;
                    StartStopButtonEvent.RaiseMessage(true);
                    if (!AppIsShuttingDown)
                        OnStartedEvent(EventArgs.Empty);
                }
            }

        }

        private static void RestartTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            OnRestartEvent(EventArgs.Empty);
            RestartTimer.Stop();
        }

        public static void Stop(bool applicationShutdown)
        {
            if (applicationShutdown)
                AppIsShuttingDown = true;
            startClosing = true;
        }

        private static void Cycler()
        {

            if (onlineTimer == null)
                onlineTimer = new System.Timers.Timer()
                {
                    Interval = 500
                };

            foreach (ExtendedPlc plc in PlcConfig.PlcList)
            {
                try
                {
                    if (plc.Active)
                    {
                        plc.Open();
                        if (!plc.IsConnected)
                        {
                            Apps.Logger.Log($"Får ej kontakt med {plc.PlcName}", Severity.Error);
                            AbortLogThread(string.Empty);
                        }
                        else
                        {
                            plc.ConnectionStatus = ConnectionStatus.Connected;
                            onlineTimer.Start();
                        }
                    }

                }

                catch (PlcException ex)
                {
                    Apps.Logger.Log($"Lyckas ej ansluta till {plc.PlcName}", Severity.Error, ex);
                    AbortLogThread(string.Empty);
                }
            }


            if (SendValuesToSql.rawValueBlock == null)
                SendValuesToSql.rawValueBlock = new ConcurrentBag<rawValueClass>();

            if (lastLogValue == null)
                lastLogValue = new List<LastValue>();


            while (PlcConfig.PlcList.Any(p => p.LoggerIsStarted))
            {
                // Reconnect om det ej går att pinga PLC
                foreach (ExtendedPlc MyPlc in PlcConfig.PlcList)
                {
                    if (MyPlc.LoggerIsStarted)
                    {
                        if (MyPlc.ConnectionStatus != ConnectionStatus.Connected)
                        {

                            var tiden = DateTime.UtcNow;

                            if (tiden - MyPlc.LastReconnect > TimeSpan.FromSeconds(5))
                            {
                                try
                                {
                                    MyPlc.Close();
                                    MyPlc.Open();
                                    if (MyPlc.IsConnected && MyPlc.Ping())
                                    {
                                        MyPlc.SendPlcStatusMessage($"Lyckades återansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}", Status.Ok);
                                        Apps.Logger.Log($"Lyckades återansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}", Severity.Normal);
                                        MyPlc.ConnectionStatus = ConnectionStatus.Connected;
                                    }
                                    else
                                    {
                                        MyPlc.SendPlcStatusMessage($"Misslyckades att återansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}", Status.Error);
                                        Apps.Logger.Log($"Misslyckades att återansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}", Severity.Error);
                                    }
                                    MyPlc.LastReconnect = tiden;
                                }
                                catch (Exception ex)
                                {
                                    MyPlc.SendPlcStatusMessage($"Misslyckades att ansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}", Status.Error);
                                    Apps.Logger.Log($"Misslyckades att ansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}", Severity.Error, ex);
                                    MyPlc.LastReconnect = tiden;
                                }
                            }



                        }
                    }
                }
                foreach (ExtendedPlc MyPlc in PlcConfig.PlcList)
                {
                    if (MyPlc.IsConnected && MyPlc.ConnectionStatus == ConnectionStatus.Connected && MyPlc.Active)
                        foreach (TagDefinitions logValue in TagsToLog.AllLogValues)
                            if (logValue.plcName.Equals(MyPlc.PlcName))
                                ReadValue(MyPlc, logValue);
                    if (startClosing)
                    {
                        AbortLogThread(string.Empty);
                    }
                }
                int sleepTime = (int)((float)FastestLogTime * 0.125f);
                Thread.Sleep(sleepTime);
            }
        }

        /// <summary>
        /// Synkronisera PLC-klockan en gång om dagen
        /// </summary>
        /// <param name="MyPlc"></param>
        /// <param name="tid"></param>
        private static void SyncPlc(ExtendedPlc MyPlc, DateTime tid)
        {
            var tid1 = TimeZoneInfo.ConvertTimeFromUtc(tid, TimeZoneInfo.Local);
            try
            {
                switch (MyPlc.CPU)
                {
                    case CpuType.S7200:
                        break;
                    case CpuType.Logo0BA8:
                        break;
                    case CpuType.S7300:
                        // Write Time
                        var tidBytes = S7.Net.Types.DateTime.ToByteArray(tid1);
                        MyPlc.WriteBytes(DataType.DataBlock, MyPlc.SyncTimeDbNr, MyPlc.SyncTimeOffset, tidBytes);
                        MyPlc.Write(MyPlc.SyncBoolAddress, 1);
                        break;
                    case CpuType.S7400:
                        var tidByte = S7.Net.Types.DateTime.ToByteArray(tid1);
                        MyPlc.WriteBytes(DataType.DataBlock, MyPlc.SyncTimeDbNr, MyPlc.SyncTimeOffset, tidByte);
                        MyPlc.Write(MyPlc.SyncBoolAddress, 1);
                        break;
                    case CpuType.S71200:
                        break;
                    case CpuType.S71500:
                        var tidByt = S7.Net.Types.DateTimeLong.ToByteArray(tid1);
                        MyPlc.WriteBytes(DataType.DataBlock, MyPlc.SyncTimeDbNr, MyPlc.SyncTimeOffset, tidByt);
                        MyPlc.Write(MyPlc.SyncBoolAddress, 1);
                        break;
                    default:
                        break;
                }
            }
            catch (PlcException)
            {
                Apps.Logger.Log($"Misslyckades att synka {MyPlc.PlcName}", Severity.Error);
                throw;
            }
            MyPlc.lastSyncTime = tid;

            var connectionString = PlcConfig.ConnectionString();
            var query = $"UPDATE {SqlSettings.Default.Databas}.dbo.plcConfig SET lastSyncTime = '{tid}' WHERE name = '{MyPlc.PlcName}'";
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                Apps.Logger.Log($"Lyckades ej skriva till databas vid tids-synkning: {MyPlc.PlcName}", Severity.Error, ex);
            }

            Apps.Logger.Log($"Synkade {MyPlc.PlcName}", Severity.Success);
        }

        private static void AbortLogThread(string message)
        {


            foreach (ExtendedPlc MyPlc in PlcConfig.PlcList)
            {
                if (MyPlc.LoggerIsStarted)
                {
                    MyPlc.Close();

                    if (message == string.Empty)
                        MyPlc.SendPlcStatusMessage($"Kommunikationen till {MyPlc.PlcName} avbruten", Status.Warning);
                    else
                        Apps.Logger.Log($"Kommunikationen upprättades ej till {MyPlc.PlcName}\n\r{message}", Severity.Error);

                    MyPlc.ConnectionStatus = ConnectionStatus.Disconnected;
                    OnlineStatusEvent.RaiseMessage(MyPlc.ConnectionStatus, MyPlc.PlcName);
                    MyPlc.LoggerIsStarted = false;
                }
            }


            startClosing = false;
            if (lastLogValue != null)
                lastLogValue.Clear();
            logThread.Abort();
        }

        private static void ReadValue(ExtendedPlc MyPlc, TagDefinitions logTag)
        {
            if (MyPlc.IsConnected)
            {
                if (logTag.active == true && MyPlc.PlcName == logTag.plcName)
                {
                    var tiden = DateTime.UtcNow;

                    int logdiff;
                    if ((int)logTag.logFreq <= 250)
                        logdiff = 2;
                    else
                        logdiff = 0;

                    if ((tiden - logTag.LastLogTime) >= TimeSpan.FromMilliseconds((int)logTag.logFreq - logdiff)) // Minskar med 2 millisekunder vid snabb loggning för att få en mer exakt loggning
                    {
                        object unknownTag = new object();
                        try
                        {
                            if (MyPlc.ConnectionStatus == ConnectionStatus.Connected && MyPlc.IsConnected)
                            {
                                // Synkronisera klockan
                                if (tiden > MyPlc.lastSyncTime + TimeSpan.FromDays(1) && MyPlc.SyncActive)
                                    SyncPlc(MyPlc, tiden);

                                if (logTag.varType == VarType.StringEx)
                                {
                                    var temp = MyPlc.ReadBytes(logTag.dataType, logTag.blockNr, logTag.startByte, logTag.nrOfElements + 2);
                                    unknownTag = S7.Net.Types.StringEx.FromByteArray(temp);
                                }
                                else if (logTag.varType == VarType.String)
                                {
                                    var temp = MyPlc.ReadBytes(logTag.dataType, logTag.blockNr, logTag.startByte, logTag.nrOfElements);
                                    unknownTag = S7.Net.Types.String.FromByteArray(temp);
                                }
                                else if (logTag.varType == VarType.DateTime)
                                {
                                    var temp = MyPlc.ReadBytes(logTag.dataType, logTag.blockNr, logTag.startByte, 8);
                                    unknownTag = S7.Net.Types.DateTime.FromByteArray(temp);
                                }
                                else if (logTag.varType == VarType.DateTimeLong)
                                {
                                    var temp = MyPlc.ReadBytes(logTag.dataType, logTag.blockNr, logTag.startByte, 12);
                                    unknownTag = S7.Net.Types.DateTimeLong.FromByteArray(temp);
                                }
                                else
                                    unknownTag = MyPlc.Read(logTag.dataType, logTag.blockNr, logTag.startByte, logTag.varType, logTag.nrOfElements, logTag.bitAddress);

                                logTag.LastLogTime = tiden;
                                logTag.NrSuccededReadAttempts++;


                                if (logTag.logType == LogType.Delta && MyPlc.IsConnected)
                                {
                                    var lastKnownLogValue = lastLogValue.FindLast(l => l.name == logTag.name && l.PlcName == logTag.plcName);
                                    if (lastKnownLogValue == null)
                                    {
                                        AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                        return;
                                    }

                                    if (logTag.deadband > 0)
                                    {

                                        switch (logTag.varType)
                                        {
                                            case VarType.Bit:
                                                if ((bool)unknownTag != (bool)lastKnownLogValue.value)
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                break;
                                            case VarType.Byte:
                                                if (unknownTag is byte unknownConverted)
                                                {
                                                    if (Math.Abs(unknownConverted - ((byte)lastKnownLogValue.value)) > (float)((byte)logTag.deadband))
                                                        AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                }
                                                else
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                break;
                                            case VarType.Word:
                                                if (unknownTag is ushort unknownUShort)
                                                {
                                                    if (Math.Abs(unknownUShort - ((ushort)lastKnownLogValue.value)) > (float)((ushort)logTag.deadband))
                                                        AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                }
                                                else
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                break;
                                            case VarType.DWord:
                                                if (unknownTag is uint uknonwnUint)
                                                {
                                                    if (Math.Abs(uknonwnUint - ((uint)lastKnownLogValue.value)) > (float)((uint)logTag.deadband))
                                                        AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                }
                                                else
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                break;
                                            case VarType.Int:
                                                if (unknownTag is short unknownShort)
                                                {
                                                    if (Math.Abs(unknownShort - ((short)lastKnownLogValue.value)) > (float)((short)logTag.deadband))
                                                        AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                }
                                                else
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                break;
                                            case VarType.DInt:
                                                if (unknownTag is int unknownInt)
                                                {
                                                    if (Math.Abs(unknownInt - ((int)lastKnownLogValue.value)) > (float)((int)logTag.deadband))
                                                        AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                }
                                                else
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                break;
                                            case VarType.Real:
                                                if (unknownTag is float unknownFloat)
                                                {
                                                    if (Math.Abs(unknownFloat - (float)lastKnownLogValue.value) > (float)logTag.deadband)
                                                        AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                }
                                                else
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                break;
                                            case VarType.String:
                                                if (!Equals(unknownTag.ToString(), lastKnownLogValue.value.ToString()))
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                break;
                                            case VarType.StringEx:
                                                if (!Equals(unknownTag.ToString(), lastKnownLogValue.value.ToString()))
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                break;
                                            case VarType.Timer:
                                                break;
                                            case VarType.Counter:
                                                break;
                                            case VarType.DateTime:
                                                break;
                                            case VarType.DateTimeLong:
                                                break;
                                            default:
                                                break;

                                        }
                                    }
                                    else
                                    {
                                        switch (logTag.varType)
                                        {
                                            case VarType.Bit:
                                                if ((bool)unknownTag != (bool)lastKnownLogValue.value)
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                break;
                                            case VarType.Byte:
                                                if (unknownTag is byte unknownByte)
                                                {
                                                    if (unknownByte > (byte)lastKnownLogValue.value ||
                                                        unknownByte < (byte)lastKnownLogValue.value)
                                                    {
                                                        AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                    }
                                                }
                                                else
                                                {
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                }
                                                break;
                                            case VarType.Word:
                                                if (unknownTag is ushort unknownUShort)
                                                {
                                                    if (unknownUShort > (ushort)lastKnownLogValue.value ||
                                                        unknownUShort < (ushort)lastKnownLogValue.value)
                                                    {
                                                        AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                    }
                                                }
                                                else
                                                {
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                }
                                                break;
                                            case VarType.DWord:
                                                if (unknownTag is uint unknownUInt)
                                                {
                                                    if (unknownUInt > (uint)lastKnownLogValue.value ||
                                                        unknownUInt < (uint)lastKnownLogValue.value)
                                                    {
                                                        AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                    }
                                                }
                                                else
                                                {
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                }
                                                break;
                                            case VarType.Int:
                                                if (unknownTag is short unknownShort)
                                                {
                                                    if (unknownShort > (short)lastKnownLogValue.value ||
                                                        unknownShort < (short)lastKnownLogValue.value)
                                                    {
                                                        AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                    }
                                                }
                                                else
                                                {
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                }
                                                break;
                                            case VarType.DInt:
                                                if (unknownTag is int unknownInt)
                                                {
                                                    if (unknownInt > (int)lastKnownLogValue.value ||
                                                        unknownInt < (int)lastKnownLogValue.value)
                                                    {
                                                        AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                    }
                                                }
                                                else
                                                {
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                }
                                                break;
                                            case VarType.Real:
                                                if (unknownTag is float unknownFloat)
                                                {
                                                    if (unknownFloat > (float)lastKnownLogValue.value ||
                                                        unknownFloat < (float)lastKnownLogValue.value)
                                                    {
                                                        AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                    }
                                                }
                                                else
                                                {
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                }
                                                break;
                                            case VarType.String:
                                                if (!Equals(unknownTag.ToString(), lastKnownLogValue.value.ToString()))
                                                {
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                }
                                                break;
                                            case VarType.StringEx:
                                                if (!Equals(unknownTag.ToString(), lastKnownLogValue.value.ToString()))
                                                {
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                }
                                                break;
                                            case VarType.Timer:
                                                break;
                                            case VarType.Counter:
                                                break;
                                            case VarType.DateTime:
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                                else if (logTag.logType == LogType.Cyclic && MyPlc.IsConnected)
                                {
                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                }
                                else if (logTag.logType == LogType.TimeOfDay && MyPlc.IsConnected)
                                {
                                    var tid1 = TimeZoneInfo.ConvertTimeFromUtc(tiden, TimeZoneInfo.Local);
                                    if (logTag.timeOfDay.Seconds != 0)
                                    {
                                        if (tid1.Hour == logTag.timeOfDay.Hours &&
                                            tid1.Minute == logTag.timeOfDay.Minutes &&
                                            tid1.Second == logTag.timeOfDay.Seconds)
                                        {
                                            var allOccurencesOfTagInList = lastLogValue.Find(n => n.name == logTag.name && n.logDate.Day == tiden.Day);
                                            if (allOccurencesOfTagInList == null)
                                            {
                                                AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                            }
                                        }
                                    }
                                    else if (tid1.Hour == logTag.timeOfDay.Hours &&
                                            tid1.Minute == logTag.timeOfDay.Minutes)
                                    {
                                        var allOccurencesOfTagInList = lastLogValue.Find(n => n.name == logTag.name && n.logDate.Day == tiden.Day);
                                        if (allOccurencesOfTagInList == null)
                                        {
                                            AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                        }
                                    }
                                }

                                // Check if tag has any subscribed event tags
                                if (logTag.SubscribedTags.Count > 0)
                                {
                                    foreach (int id in logTag.SubscribedTags)
                                    {
                                        var subbedTag = TagHelpers.GetTagFromId(id);
                                        var lastValue = lastLogValue.Find(l => l.name == logTag.name);
                                        if (subbedTag.IsBooleanTrigger && logTag.varType == VarType.Bit)
                                        {
                                            switch (subbedTag.boolTrigger)
                                            {
                                                case BooleanTrigger.OnTrue:
                                                    if (!(bool)lastValue.value && (bool)unknownTag)
                                                    {
                                                        var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                    }
                                                    break;
                                                case BooleanTrigger.WhileTrue:
                                                    if ((bool)unknownTag)
                                                    {
                                                        var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                    }
                                                    break;
                                                case BooleanTrigger.OnFalse:
                                                    if ((bool)lastValue.value && !(bool)unknownTag)
                                                    {
                                                        var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                    }
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        else if (!subbedTag.IsBooleanTrigger)
                                        {
                                            switch (subbedTag.analogTrigger)
                                            {
                                                case AnalogTrigger.LessThan:
                                                    switch (logTag.varType)
                                                    {
                                                        case VarType.Bit:
                                                            break;
                                                        case VarType.Byte:
                                                            if ((byte)unknownTag < subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.Word:
                                                            if ((UInt16)unknownTag < subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.DWord:
                                                            if ((UInt32)unknownTag < subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.Int:
                                                            if ((short)unknownTag < subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.DInt:
                                                            if ((Int32)unknownTag < subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.Real:
                                                            if ((float)unknownTag < subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.String:
                                                            break;
                                                        case VarType.StringEx:
                                                            break;
                                                        case VarType.Timer:
                                                            break;
                                                        case VarType.Counter:
                                                            break;
                                                        case VarType.DateTime:
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                    break;
                                                case AnalogTrigger.MoreThan:
                                                    switch (logTag.varType)
                                                    {
                                                        case VarType.Bit:
                                                            break;
                                                        case VarType.Byte:
                                                            if ((byte)unknownTag > subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.Word:
                                                            if ((UInt16)unknownTag > subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.DWord:
                                                            if ((UInt32)unknownTag > subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.Int:
                                                            if ((short)unknownTag > subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.DInt:
                                                            if ((Int32)unknownTag > subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.Real:
                                                            if ((float)unknownTag > subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.String:
                                                            break;
                                                        case VarType.StringEx:
                                                            break;
                                                        case VarType.Timer:
                                                            break;
                                                        case VarType.Counter:
                                                            break;
                                                        case VarType.DateTime:
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                    break;
                                                case AnalogTrigger.Equal:
                                                    switch (logTag.varType)
                                                    {
                                                        case VarType.Bit:
                                                            break;
                                                        case VarType.Byte:
                                                            if ((byte)unknownTag == subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.Word:
                                                            if ((UInt16)unknownTag == subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.DWord:
                                                            if ((UInt32)unknownTag == subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.Int:
                                                            if ((short)unknownTag == subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.DInt:
                                                            if ((Int32)unknownTag == subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.Real:
                                                            if ((float)unknownTag == subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.String:
                                                            break;
                                                        case VarType.StringEx:
                                                            break;
                                                        case VarType.Timer:
                                                            break;
                                                        case VarType.Counter:
                                                            break;
                                                        case VarType.DateTime:
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                    break;
                                                case AnalogTrigger.LessOrEqual:
                                                    switch (logTag.varType)
                                                    {
                                                        case VarType.Bit:
                                                            break;
                                                        case VarType.Byte:
                                                            if ((byte)unknownTag <= subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.Word:
                                                            if ((UInt16)unknownTag <= subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.DWord:
                                                            if ((UInt32)unknownTag <= subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.Int:
                                                            if ((short)unknownTag <= subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.DInt:
                                                            if ((Int32)unknownTag <= subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.Real:
                                                            if ((float)unknownTag <= subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.String:
                                                            break;
                                                        case VarType.StringEx:
                                                            break;
                                                        case VarType.Timer:
                                                            break;
                                                        case VarType.Counter:
                                                            break;
                                                        case VarType.DateTime:
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                    break;
                                                case AnalogTrigger.MoreOrEqual:
                                                    switch (logTag.varType)
                                                    {
                                                        case VarType.Bit:
                                                            break;
                                                        case VarType.Byte:
                                                            if ((byte)unknownTag >= subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.Word:
                                                            if ((UInt16)unknownTag >= subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.DWord:
                                                            if ((UInt32)unknownTag >= subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.Int:
                                                            if ((short)unknownTag >= subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.DInt:
                                                            if ((Int32)unknownTag >= subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.Real:
                                                            if ((float)unknownTag >= subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.String:
                                                            break;
                                                        case VarType.StringEx:
                                                            break;
                                                        case VarType.Timer:
                                                            break;
                                                        case VarType.Counter:
                                                            break;
                                                        case VarType.DateTime:
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                    break;
                                                case AnalogTrigger.NotEqual:
                                                    switch (logTag.varType)
                                                    {
                                                        case VarType.Bit:
                                                            break;
                                                        case VarType.Byte:
                                                            if ((byte)unknownTag != subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.Word:
                                                            if ((UInt16)unknownTag != subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.DWord:
                                                            if ((UInt32)unknownTag != subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.Int:
                                                            if ((short)unknownTag != subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.DInt:
                                                            if ((Int32)unknownTag != subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.Real:
                                                            if ((float)unknownTag != subbedTag.analogValue)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            break;
                                                        case VarType.String:
                                                            break;
                                                        case VarType.StringEx:
                                                            break;
                                                        case VarType.Timer:
                                                            break;
                                                        case VarType.Counter:
                                                            break;
                                                        case VarType.DateTime:
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }

                                    }
                                }

                            }
                            else
                            {
                                if (MyPlc.UnableToPing)
                                {
                                    var errorString = $"Lyckas ej pinga {MyPlc.PlcName}\r\n{MyPlc.IP}";
                                    logTag.LastErrorMessage = errorString;
                                    MyPlc.SendPlcStatusMessage(errorString, Status.Error);
                                }
                            }

                        }
                        catch (PlcException ex)
                        {
                            MyPlc.SendPlcStatusMessage($"Misslyckades att läsa {logTag.name} från {MyPlc.PlcName}\r\n{ex.Message}", Status.Error);
                            Apps.Logger.Log(ex.Message, Severity.Error, ex);
                            logTag.NrFailedReadAttempts++;
                            MyPlc.ConnectionStatus = ConnectionStatus.Disconnected;
                            logTag.LastErrorMessage = ex.Message;
                        }
                    }
                }
            }
        }


        private static void AddValueToSql(TagDefinitions logTag, object unknownTag, string plcName)
        {
            lock (sqlLock)
            {
                if (unknownTag == null)
                    return;

                logTag.TimesLogged++;
                lastLogValue.Add(new LastValue()
                {
                    name = logTag.name,
                    value = unknownTag,
                    logDate = logTag.LastLogTime,
                    PlcName = plcName,
                });

                var allOccurencesOfTagInList = lastLogValue.FindAll(n => n.name == logTag.name && n.PlcName == logTag.plcName).OrderBy(dat => dat.logDate).ToList();
                var nrOfItemsInLastLog = lastLogValue.FindAll(n => n.name == logTag.name && n.PlcName == logTag.plcName);


                // Garantera att det bara finns ett värde bakåt
                if (nrOfItemsInLastLog.Count > 2)
                {
                    var removeDate = allOccurencesOfTagInList[nrOfItemsInLastLog.Count - 3].logDate;
                    lastLogValue.RemoveAll(i => i.name == logTag.name && i.logDate <= removeDate && i.PlcName == logTag.plcName);
                }

                var logValue = new TagDefinitions()
                {
                    active = logTag.active,
                    bitAddress = logTag.bitAddress,
                    blockNr = logTag.blockNr,
                    nrOfElements = logTag.nrOfElements,
                    dataType = logTag.dataType,
                    id = logTag.id,
                    LastLogTime = logTag.LastLogTime + UtcOffset,
                    logFreq = logTag.logFreq,
                    name = logTag.name,
                    plcName = logTag.plcName,
                    startByte = logTag.startByte,
                    varType = logTag.varType
                };

                SendValuesToSql.AddRawValue(unknownTag, logValue);
            }

        }


    }
}
