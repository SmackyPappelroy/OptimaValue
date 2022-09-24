using OpcUaHm;
using OpcUaHm.Common;
using S7.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        private static Task logTask;
        private static CancellationTokenSource cancelTokenSource = new CancellationTokenSource();


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
            if (!PlcConfig.PlcList.Any(x => x.Active && x.ActiveTagsInPlc))
            {
                Apps.Logger.Log("Inga aktiva Plc eller aktiva taggar", Severity.Error);
                return;
            }

            RestartTimer.Elapsed -= RestartTimer_Elapsed;
            RestartTimer.Elapsed += RestartTimer_Elapsed;


            foreach (ExtendedPlc MyPlc in PlcConfig.PlcList)
            {
                if (MyPlc.Active && MyPlc.ActiveTagsInPlc)
                {
                    MyPlc.LoggerIsStarted = true;
                    StartStopButtonEvent.RaiseMessage(true);
                    if (!AppIsShuttingDown)
                        OnStartedEvent(EventArgs.Empty);
                }
            }

            StartTask();

        }

        private static void StartTask()
        {
            logTask = Task.Run(async () =>
            {
                await Cycler(cancelTokenSource.Token).ConfigureAwait(false);
            }, cancelTokenSource.Token)
                .ContinueWith(t =>
                {
                    t.Exception?.Handle(e => true);
                    AbortLogThread(string.Empty);
                    Console.WriteLine("You have canceled the task");
                    cancelTokenSource = new CancellationTokenSource();
                }, TaskContinuationOptions.OnlyOnCanceled);
        }

        public static void RequestDisconnect()
        {
            if (logTask != null && !cancelTokenSource.IsCancellationRequested)
                cancelTokenSource.Cancel();
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

        private static async Task Cycler(CancellationToken ct)
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
                        if (!plc.isOpc)
                            plc.Open();
                        else
                            plc.OpcUaClient.Connect();
                        if ((plc.isOpc && plc.OpcUaClient.Status == OpcStatus.NotConnected) || (!plc.isOpc && !plc.IsConnected))
                        {
                            Apps.Logger.Log($"Får ej kontakt med {plc.PlcName}", Severity.Error);
                            RequestDisconnect();
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
                    RequestDisconnect();

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

                            if (tiden - MyPlc.LastReconnect > TimeSpan.FromSeconds(3))
                            {
                                try
                                {
                                    if (MyPlc.isOpc)
                                    {
                                        MyPlc.OpcUaClient = new UaClient(new Uri(MyPlc.ConnectionString));
                                        MyPlc.OpcUaClient.Connect();
                                        if (MyPlc.OpcUaClient.Status == OpcStatus.Connected)
                                        {
                                            MyPlc.SendPlcStatusMessage($"Lyckades återansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}", Status.Ok);
                                            Apps.Logger.Log($"Lyckades återansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}", Severity.Information);
                                            MyPlc.ConnectionStatus = ConnectionStatus.Connected;
                                        }
                                        else
                                        {
                                            MyPlc.SendPlcStatusMessage($"Misslyckades att återansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}", Status.Error);
                                            Apps.Logger.Log($"Misslyckades att återansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}", Severity.Error);
                                        }
                                    }
                                    else if (MyPlc.Ping())
                                    {
                                        MyPlc.Close();
                                        MyPlc.Open();
                                        if (MyPlc.IsConnected)
                                        {
                                            MyPlc.SendPlcStatusMessage($"Lyckades återansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}", Status.Ok);
                                            Apps.Logger.Log($"Lyckades återansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}", Severity.Information);
                                            MyPlc.ConnectionStatus = ConnectionStatus.Connected;
                                        }
                                        else
                                        {
                                            MyPlc.SendPlcStatusMessage($"Misslyckades att återansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}", Status.Error);
                                            Apps.Logger.Log($"Misslyckades att återansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}", Severity.Error);
                                        }
                                    }
                                    else
                                    {
                                        MyPlc.SendPlcStatusMessage($"Lyckas ej pinga {MyPlc.PlcName}\r\n{MyPlc.IP}", Status.Error);
                                        Apps.Logger.Log($"Lyckas ej pinga {MyPlc.PlcName}\r\n{MyPlc.IP}", Severity.Error);
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
                    {
                        foreach (TagDefinitions logValue in TagsToLog.AllLogValues)
                            if (logValue.PlcName.Equals(MyPlc.PlcName))
                                ReadValue(MyPlc, logValue);

                    }
                    if (startClosing)
                    {
                        RequestDisconnect();

                    }
                }
                await Task.Delay((int)((float)FastestLogTime * 0.125f));
                ct.ThrowIfCancellationRequested();

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
                switch ((CpuType)MyPlc.CPU)
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

            DatabaseSql.SaveSyncTime(tid: tid, plcName: MyPlc.PlcName);

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
                else
                    MyPlc.ConnectionStatus = ConnectionStatus.Disconnected;
            }


            startClosing = false;
            if (lastLogValue != null)
                lastLogValue.Clear();

        }

        private static void ReadValue(ExtendedPlc MyPlc, TagDefinitions logTag)
        {
            if (MyPlc.IsConnected)
            {
                if (logTag.Active == true && MyPlc.PlcName == logTag.PlcName)
                {
                    var tiden = DateTime.UtcNow;

                    int logdiff;
                    if ((int)logTag.LogFreq <= 250)
                        logdiff = 2;
                    else
                        logdiff = 0;

                    if ((tiden - logTag.LastLogTime) >= TimeSpan.FromMilliseconds((int)logTag.LogFreq - logdiff)) // Minskar med 2 millisekunder vid snabb loggning för att få en mer exakt loggning
                    {
                        object unknownTag = new object();
                        string opcTagType = "";
                        string opcTagName = "";

                        try
                        {
                            if (MyPlc.isOpc)
                            {
                                opcTagName = logTag.PlcName + "." + logTag.Name;
                                opcTagType = MyPlc.OpcUaClient.GetDataType(opcTagName).ToString();
                            }
                            if (MyPlc.ConnectionStatus == ConnectionStatus.Connected && MyPlc.IsConnected)
                            {
                                // Synkronisera klockan
                                if (tiden > MyPlc.lastSyncTime + TimeSpan.FromDays(1) && MyPlc.SyncActive && MyPlc.isOpc)
                                    SyncPlc(MyPlc, tiden);

                                if (logTag.VarType == VarType.S7String && !MyPlc.isOpc)
                                {
                                    var temp = MyPlc.ReadBytes(logTag.DataType, logTag.BlockNr, logTag.StartByte, logTag.NrOfElements + 2);
                                    unknownTag = temp.S7StringSwedish();
                                }
                                else if (logTag.VarType == VarType.String)
                                {
                                    if (!MyPlc.isOpc)
                                    {
                                        var temp = MyPlc.ReadBytes(logTag.DataType, logTag.BlockNr, logTag.StartByte, logTag.NrOfElements);
                                        unknownTag = temp.StringSwedish();
                                    }
                                    else
                                    {
                                        unknownTag = MyPlc.OpcUaClient.Read<string>(opcTagName);
                                    }
                                }
                                else if (logTag.VarType == VarType.DateTime)
                                {
                                    if (!MyPlc.isOpc)
                                    {
                                        var temp = MyPlc.ReadBytes(logTag.DataType, logTag.BlockNr, logTag.StartByte, 8);
                                        unknownTag = S7.Net.Types.DateTime.FromByteArray(temp);
                                    }
                                    else
                                    {
                                        unknownTag = MyPlc.OpcUaClient.Read<DateTime>(opcTagName);
                                    }

                                }
                                else if (logTag.VarType == VarType.DateTimeLong && !MyPlc.isOpc)
                                {
                                    var temp = MyPlc.ReadBytes(logTag.DataType, logTag.BlockNr, logTag.StartByte, 12);
                                    unknownTag = S7.Net.Types.DateTimeLong.FromByteArray(temp);
                                }
                                else if (!MyPlc.isOpc)
                                    unknownTag = MyPlc.Read(logTag.DataType, logTag.BlockNr, logTag.StartByte, (S7.Net.VarType)logTag.VarType, logTag.NrOfElements, logTag.BitAddress);
                                else if (MyPlc.isOpc)
                                {
                                    unknownTag = MyPlc.OpcUaClient.Read<object>(opcTagName);
                                }
                                logTag.LastLogTime = tiden;
                                logTag.NrSuccededReadAttempts++;


                                if (logTag.LogType == LogType.Delta && MyPlc.IsConnected)
                                {
                                    var lastKnownLogValue = lastLogValue.FindLast(l => l.tag_id == logTag.Id);
                                    if (lastKnownLogValue == null)
                                    {
                                        AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                        return;
                                    }

                                    if (logTag.Deadband > 0)
                                    {

                                        switch (logTag.VarType)
                                        {
                                            case VarType.Bit:
                                                if ((bool)unknownTag != (bool)lastKnownLogValue.value)
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                break;
                                            case VarType.Byte:
                                                if (unknownTag is byte unknownConverted)
                                                {
                                                    if (Math.Abs(unknownConverted - ((byte)lastKnownLogValue.value)) > (float)((byte)logTag.Deadband))
                                                        AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                }
                                                else
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                break;
                                            case VarType.Word:
                                                if (unknownTag is ushort unknownUShort)
                                                {
                                                    if (Math.Abs(unknownUShort - ((ushort)lastKnownLogValue.value)) > (float)((ushort)logTag.Deadband))
                                                        AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                }
                                                else
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                break;
                                            case VarType.DWord:
                                                if (unknownTag is uint uknonwnUint)
                                                {
                                                    if (Math.Abs(uknonwnUint - ((uint)lastKnownLogValue.value)) > (float)((uint)logTag.Deadband))
                                                        AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                }
                                                else
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                break;
                                            case VarType.Int:
                                                if (unknownTag is short unknownShort)
                                                {
                                                    if (Math.Abs(unknownShort - ((short)lastKnownLogValue.value)) > (float)((short)logTag.Deadband))
                                                        AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                }
                                                else
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                break;
                                            case VarType.DInt:
                                                if (unknownTag is int unknownInt)
                                                {
                                                    if (Math.Abs(unknownInt - ((int)lastKnownLogValue.value)) > (float)((int)logTag.Deadband))
                                                        AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                }
                                                else
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                break;
                                            case VarType.Real:
                                                if (unknownTag is float unknownFloat)
                                                {
                                                    if (Math.Abs(unknownFloat - (float)lastKnownLogValue.value) > (float)logTag.Deadband)
                                                        AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                }
                                                else
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                break;
                                            case VarType.String:
                                                if (!Equals(unknownTag.ToString(), lastKnownLogValue.value.ToString()))
                                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                                break;
                                            case VarType.S7String:
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
                                        switch (logTag.VarType)
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
                                            case VarType.S7String:
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
                                else if (logTag.LogType == LogType.Cyclic && MyPlc.IsConnected)
                                {
                                    AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                }
                                else if (logTag.LogType == LogType.TimeOfDay && MyPlc.IsConnected)
                                {
                                    var tid1 = TimeZoneInfo.ConvertTimeFromUtc(tiden, TimeZoneInfo.Local);
                                    if (logTag.TimeOfDay.Seconds != 0)
                                    {
                                        if (tid1.Hour == logTag.TimeOfDay.Hours &&
                                            tid1.Minute == logTag.TimeOfDay.Minutes &&
                                            tid1.Second == logTag.TimeOfDay.Seconds)
                                        {
                                            var allOccurencesOfTagInList = lastLogValue.Find(n => n.tag_id == logTag.Id && n.logDate.Day == tiden.Day);
                                            if (allOccurencesOfTagInList == null)
                                            {
                                                AddValueToSql(logTag, unknownTag, MyPlc.PlcName);
                                            }
                                        }
                                    }
                                    else if (tid1.Hour == logTag.TimeOfDay.Hours &&
                                            tid1.Minute == logTag.TimeOfDay.Minutes)
                                    {
                                        var allOccurencesOfTagInList = lastLogValue.Find(n => n.tag_id == logTag.Id && n.logDate.Day == tiden.Day);
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
                                        if (subbedTag.Active)
                                        {
                                            subbedTag.LastLogTime = tiden;
                                            var lastValue = lastLogValue.Find(l => l.tag_id == logTag.Id);
                                            if (subbedTag.IsBooleanTrigger && logTag.VarType == VarType.Bit)
                                            {
                                                switch (subbedTag.BoolTrigger)
                                                {
                                                    case BooleanTrigger.OnTrue:
                                                        if (!(bool)lastValue.value && (bool)unknownTag)
                                                        {
                                                            if (!MyPlc.isOpc)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            else
                                                            {
                                                                var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                        }
                                                        break;
                                                    case BooleanTrigger.WhileTrue:
                                                        if ((bool)unknownTag)
                                                        {
                                                            if (!MyPlc.isOpc)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            else
                                                            {
                                                                var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                        }
                                                        break;
                                                    case BooleanTrigger.OnFalse:
                                                        if ((bool)lastValue.value && !(bool)unknownTag)
                                                        {
                                                            if (!MyPlc.isOpc)
                                                            {
                                                                var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                            else
                                                            {
                                                                var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                            }
                                                        }
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                            else if (!subbedTag.IsBooleanTrigger)
                                            {
                                                switch (subbedTag.AnalogTrigger)
                                                {
                                                    case AnalogTrigger.LessThan:
                                                        switch (logTag.VarType)
                                                        {
                                                            case VarType.Bit:
                                                                break;
                                                            case VarType.Byte:
                                                                if ((byte)unknownTag < subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.Word:
                                                                if ((UInt16)unknownTag < subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.DWord:
                                                                if ((UInt32)unknownTag < subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.Int:
                                                                if ((short)unknownTag < subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.DInt:
                                                                if ((Int32)unknownTag < subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.Real:
                                                                if ((float)unknownTag < subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.String:
                                                                break;
                                                            case VarType.S7String:
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
                                                        switch (logTag.VarType)
                                                        {
                                                            case VarType.Bit:
                                                                break;
                                                            case VarType.Byte:
                                                                if ((byte)unknownTag > subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.Word:
                                                                if ((UInt16)unknownTag > subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.DWord:
                                                                if ((UInt32)unknownTag > subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.Int:
                                                                if ((short)unknownTag > subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.DInt:
                                                                if ((Int32)unknownTag > subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.Real:
                                                                if ((float)unknownTag > subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.String:
                                                                break;
                                                            case VarType.S7String:
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
                                                        switch (logTag.VarType)
                                                        {
                                                            case VarType.Bit:
                                                                break;
                                                            case VarType.Byte:
                                                                if ((byte)unknownTag == subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.Word:
                                                                if ((UInt16)unknownTag == subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.DWord:
                                                                if ((UInt32)unknownTag == subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.Int:
                                                                if ((short)unknownTag == subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.DInt:
                                                                if ((Int32)unknownTag == subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.Real:
                                                                if ((float)unknownTag == subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.String:
                                                                break;
                                                            case VarType.S7String:
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
                                                        switch (logTag.VarType)
                                                        {
                                                            case VarType.Bit:
                                                                break;
                                                            case VarType.Byte:
                                                                if ((byte)unknownTag <= subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.Word:
                                                                if ((UInt16)unknownTag <= subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.DWord:
                                                                if ((UInt32)unknownTag <= subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.Int:
                                                                if ((short)unknownTag <= subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.DInt:
                                                                if ((Int32)unknownTag <= subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.Real:
                                                                if ((float)unknownTag <= subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.String:
                                                                break;
                                                            case VarType.S7String:
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
                                                        switch (logTag.VarType)
                                                        {
                                                            case VarType.Bit:
                                                                break;
                                                            case VarType.Byte:
                                                                if ((byte)unknownTag >= subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.Word:
                                                                if ((UInt16)unknownTag >= subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.DWord:
                                                                if ((UInt32)unknownTag >= subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.Int:
                                                                if ((short)unknownTag >= subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.DInt:
                                                                if ((Int32)unknownTag >= subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.Real:
                                                                if ((float)unknownTag >= subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.String:
                                                                break;
                                                            case VarType.S7String:
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
                                                        switch (logTag.VarType)
                                                        {
                                                            case VarType.Bit:
                                                                break;
                                                            case VarType.Byte:
                                                                if ((byte)unknownTag != subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.Word:
                                                                if ((UInt16)unknownTag != subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.DWord:
                                                                if ((UInt32)unknownTag != subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.Int:
                                                                if ((short)unknownTag != subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.DInt:
                                                                if ((Int32)unknownTag != subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.Real:
                                                                if ((float)unknownTag != subbedTag.AnalogValue)
                                                                {
                                                                    if (!MyPlc.isOpc)
                                                                    {
                                                                        var subbedLog = MyPlc.Read(subbedTag.DataType, subbedTag.BlockNr, subbedTag.StartByte, (S7.Net.VarType)subbedTag.VarType, subbedTag.NrOfElements, subbedTag.BitAddress);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                    else
                                                                    {
                                                                        var subbedLog = MyPlc.OpcUaClient.Read<object>(subbedTag.OpcTagName);
                                                                        AddValueToSql(subbedTag, subbedLog, MyPlc.PlcName);
                                                                    }
                                                                }
                                                                break;
                                                            case VarType.String:
                                                                break;
                                                            case VarType.S7String:
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
                            MyPlc.SendPlcStatusMessage($"Misslyckades att läsa {logTag.Name} från {MyPlc.PlcName}\r\n{ex.Message}", Status.Error);
                            Apps.Logger.Log($"Misslyckades att läsa {logTag.Name} från {MyPlc.PlcName}\r\n{ex.Message}", Severity.Error, ex);
                            logTag.NrFailedReadAttempts++;
                            MyPlc.ConnectionStatus = ConnectionStatus.Disconnected;
                            logTag.LastErrorMessage = ex.Message;
                        }
                        catch (OpcException ex)
                        {
                            MyPlc.SendPlcStatusMessage($"Misslyckades att läsa {logTag.Name} från {MyPlc.PlcName}\r\n{ex.Message}", Status.Error);
                            Apps.Logger.Log($"Misslyckades att läsa {logTag.Name} från {MyPlc.PlcName}\r\n{ex.Message}", Severity.Error, ex);
                            logTag.NrFailedReadAttempts++;
                            MyPlc.ConnectionStatus = ConnectionStatus.Disconnected;
                            logTag.LastErrorMessage = ex.Message;
                        }
                        catch (OptimaValueException ex)
                        {
                            MyPlc.SendPlcStatusMessage($"Misslyckades att läsa {logTag.Name} från {MyPlc.PlcName}\r\n{ex.Message}", Status.Error);
                            Apps.Logger.Log($"Misslyckades att läsa {logTag.Name} från {MyPlc.PlcName}\r\n{ex.Message}", Severity.Error, ex);
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
                    tag_id = logTag.Id,
                    value = unknownTag,
                    logDate = logTag.LastLogTime,
                });

                var allOccurencesOfTagInList = lastLogValue.FindAll(n => n.tag_id == logTag.Id).OrderBy(dat => dat.logDate).ToList();
                var nrOfItemsInLastLog = lastLogValue.FindAll(n => n.tag_id == logTag.Id);


                // Garantera att det bara finns ett värde bakåt
                if (nrOfItemsInLastLog.Count > 2)
                {
                    var removeDate = allOccurencesOfTagInList[nrOfItemsInLastLog.Count - 3].logDate;
                    lastLogValue.RemoveAll(i => i.tag_id == logTag.Id && i.logDate <= removeDate);
                }

                var logValue = new TagDefinitions()
                {
                    Active = logTag.Active,
                    BitAddress = logTag.BitAddress,
                    BlockNr = logTag.BlockNr,
                    NrOfElements = logTag.NrOfElements,
                    DataType = logTag.DataType,
                    Id = logTag.Id,
                    LastLogTime = logTag.LastLogTime + UtcOffset,
                    LogFreq = logTag.LogFreq,
                    Name = logTag.Name,
                    PlcName = logTag.PlcName,
                    StartByte = logTag.StartByte,
                    VarType = logTag.VarType
                };

                SendValuesToSql.AddRawValue(unknownTag, logValue);
            }

        }


    }
}
