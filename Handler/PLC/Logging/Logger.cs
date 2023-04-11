using Opc.Ua;
using OpcUaHm;
using OpcUaHm.Common;
using S7.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OptimaValue
{

    public static class Logger
    {
        public static int FastestLogTime = int.MaxValue;

        /// <summary>
        /// Lokal tid offset
        /// </summary>
        public static TimeSpan UtcOffset => TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);

        private static object sqlLock = new();

        public static event EventHandler StartedEvent;
        public static event EventHandler RestartEvent;
        public static System.Timers.Timer RestartTimer = new()
        {
            Interval = 5000,
        };

        private static List<LastValue> lastLogValue;

        private static Task logTask;
        private static CancellationTokenSource cancelTokenSource = new();
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
            if (!AreActivePlcsAndTags())
            {
                Apps.Logger.Log("Inga aktiva Plc eller aktiva taggar", Severity.Error);
                return;
            }

            ConfigureRestartTimer();
            InitializeActivePlcs();

            StartLoggingTask();
        }

        private static bool AreActivePlcsAndTags()
            => PlcConfig.PlcList.Any(x => x.Active && x.ActiveTagsInPlc);

        private static void ConfigureRestartTimer()
        {
            RestartTimer.Elapsed -= RestartTimer_Elapsed;
            RestartTimer.Elapsed += RestartTimer_Elapsed;
        }

        private static void InitializeActivePlcs()
        {
            foreach (ExtendedPlc MyPlc in PlcConfig.PlcList)
            {
                if (MyPlc.Active && MyPlc.ActiveTagsInPlc)
                {
                    MyPlc.LoggerIsStarted = true;
                    StartStopButtonEvent.RaiseMessage(true);

                    if (!AppIsShuttingDown)
                    {
                        OnStartedEvent(EventArgs.Empty);
                    }
                }
            }
        }

        private static void StartLoggingTask()
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
                Apps.Logger.Log("Loggningscykel avslutad", Severity.Error);
                cancelTokenSource = new CancellationTokenSource();
            }, TaskContinuationOptions.OnlyOnCanceled);
        }

        public static void RequestDisconnect()
        {
            if (logTask != null && !cancelTokenSource.IsCancellationRequested)
            {
                cancelTokenSource.Cancel();
                Master.Stopping = true;
                Master.IsStarted = false;
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

        private static async Task Cycler(CancellationToken ct)
        {
            InitializeOnlineTimer();
            //    InitializeSendValuesToSql();
            InitializeLastLogValue();
            await CheckConnectionsAsync();

            while (PlcConfig.PlcList.Any(p => p.LoggerIsStarted))
            {
                var sw = Stopwatch.StartNew();

                long minReadTime = TagsToLog.AllLogValues.Min(x => (long)x.LogFreq);
                await ProcessAllPlcTagsAndHandleClosing();

                sw.Stop();
                long executionTime = sw.ElapsedMilliseconds;
                var delay = executionTime > minReadTime ? 10 : minReadTime - executionTime;
                await Task.Delay((int)delay, ct);

                ct.ThrowIfCancellationRequested();
            }
        }

        private static void InitializeOnlineTimer()
        {
            onlineTimer ??= new System.Timers.Timer()
                {
                    Interval = 500
                };
        }

        private static void InitializeLastLogValue()
        {
            lastLogValue ??= new List<LastValue>();
        }

        private static async Task CheckConnectionsAsync()
        {
            foreach (ExtendedPlc plc in PlcConfig.PlcList)
            {
                if (plc.Active && !plc.IsConnected)
                {
                    try
                    {
                        await plc.Plc.ConnectAsync();

                        if (!plc.Plc.IsConnected)
                        {
                            LogConnectionError(plc);
                            RequestDisconnect();
                        }
                        else
                        {
                            onlineTimer.Start();
                        }
                    }
                    catch (TimeoutException ex)
                    {
                        LogConnectionError(plc, ex);
                        RequestDisconnect();
                    }
                    catch (PlcException ex)
                    {
                        LogConnectionError(plc, ex);
                        RequestDisconnect();
                    }
                    catch (ServiceResultException ex)
                    {
                        LogConnectionError(plc, ex);
                        RequestDisconnect();
                    }
                }
            }
        }
        private static async Task CheckReconnectAsync(ExtendedPlc MyPlc)
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
                            await ReconnectOpc(MyPlc);
                        }
                        else
                        {
                            ReconnectPlc(MyPlc);
                        }

                        MyPlc.LastReconnect = tiden;
                    }
                    catch (Exception ex)
                    {
                        LogReconnectError(MyPlc, ex);
                        MyPlc.LastReconnect = tiden;
                    }
                }
            }
        }

        private static void LogConnectionError(ExtendedPlc plc, Exception ex = null)
        {
            Apps.Logger.Log($"Lyckas ej ansluta till {plc.PlcName}", Severity.Error, ex);
        }

        private static async Task ProcessAllPlcTagsAndHandleClosing()
        {
            foreach (ExtendedPlc MyPlc in PlcConfig.PlcList)
            {
                if (!MyPlc.Active)
                    continue;
                await CheckReconnectAsync(MyPlc);
                await ProcessPlcTags(MyPlc);

                if (startClosing)
                {
                    RequestDisconnect();
                }
            }
        }

        private static async Task ProcessPlcTags(ExtendedPlc plc)
        {
            if (!IsPlcConnectedAndActive(plc))
                return;

            var tagsGroupedByLogFreq = GetGroupedActiveTags(plc.PlcName);

            foreach (var group in tagsGroupedByLogFreq)
            {
                await ProcessTagGroup(plc, group);
            }
        }

        private static bool IsPlcConnectedAndActive(ExtendedPlc plc) =>
            plc.IsConnected &&
            plc.ConnectionStatus == ConnectionStatus.Connected &&
            plc.Active;

        private static List<IGrouping<LogFrequency, TagDefinitions>> GetGroupedActiveTags(string plcName)
        {
            return TagsToLog.AllLogValues
                .Where(p => p.PlcName.Equals(plcName) && p.Active)
                .GroupBy(p => p.LogFreq)
                .ToList();
        }

        private static async Task ProcessTagGroup(ExtendedPlc plc, IGrouping<LogFrequency, TagDefinitions> tagGroup)
        {
            foreach (var tag in tagGroup)
            {
                UpdateLastLogTimeIfNeeded(tag);
                await ReadValueIfReady(plc, tag, (int)tagGroup.Key);
            }
        }

        private static void UpdateLastLogTimeIfNeeded(TagDefinitions tag)
        {
            if (tag.LastLogTime == DateTime.MinValue)
                tag.LastLogTime = DateTime.UtcNow;
        }

        private static async Task ReadValueIfReady(ExtendedPlc plc, TagDefinitions tag, int logFreq)
        {
            if (tag.LastLogTime.AddMilliseconds((double)logFreq) < DateTime.UtcNow.AddMilliseconds(100))
            {
                await ReadValue(plc, tag);
            }
        }



        private static async Task ReconnectOpc(ExtendedPlc MyPlc)
        {
            if (!MyPlc.IsConnected)
                await MyPlc.Plc.ConnectAsync();

            LogConnectionStatus(MyPlc);
        }

        private static void ReconnectPlc(ExtendedPlc MyPlc)
        {
            if (MyPlc.Plc.Ping())
            {
                MyPlc.Plc.Disconnect();
                MyPlc.Plc.Connect();
            }

            LogConnectionStatus(MyPlc);
        }

        private static void LogConnectionStatus(ExtendedPlc MyPlc)
        {
            var status = MyPlc.IsConnected ? Status.Ok : Status.Error;
            var action = MyPlc.IsConnected ? "Lyckades återansluta" : "Misslyckades att återansluta";
            var severity = MyPlc.IsConnected ? Severity.Information : Severity.Error;

            MyPlc.SendPlcStatusMessage($"{action} till {MyPlc.PlcName}\r\n{MyPlc.PlcConfiguration.Ip}", status);
            Apps.Logger.Log($"{action} till {MyPlc.PlcName}\r\n{MyPlc.PlcConfiguration.Ip}", severity);
        }

        private static void LogReconnectError(ExtendedPlc MyPlc, Exception ex)
        {
            MyPlc.SendPlcStatusMessage($"Misslyckades att ansluta till {MyPlc.PlcName}\r\n{MyPlc.PlcConfiguration.Ip}", Status.Error);
            Apps.Logger.Log($"Misslyckades att ansluta till {MyPlc.PlcName}\r\n{MyPlc.PlcConfiguration.Ip}", Severity.Error, ex);
        }

        private static async Task ReadValue(ExtendedPlc MyPlc, TagDefinitions logTag)
        {
            if (!MyPlc.IsConnected || logTag.Active != true || MyPlc.PlcName != logTag.PlcName
                || MyPlc.ConnectionStatus != ConnectionStatus.Connected)
                return;

            var tiden = DateTime.UtcNow;
            var tag = (ITagDefinition)logTag;
            var plcTag = new PlcTag(tag);

            try
            {

                if (MyPlc.ConnectionStatus != ConnectionStatus.Connected || !MyPlc.IsConnected)
                {
                    if (MyPlc.UnableToPing)
                    {
                        var errorString = $"Lyckas ej pinga {MyPlc.PlcName}\r\n{MyPlc.PlcConfiguration.Ip}";
                        logTag.LastErrorMessage = errorString;
                        MyPlc.SendPlcStatusMessage(errorString, Status.Error);
                    }
                    return;
                }

                // Synkronisera klockan
                if (tiden > MyPlc.lastSyncTime + TimeSpan.FromDays(1) && MyPlc.SyncActive && !MyPlc.isOpc)
                    await SyncPlc(MyPlc, tiden);

                ReadValue readValue = await ReadTagValueAsync(MyPlc, logTag, plcTag);

                logTag.LastLogTime = tiden;
                logTag.NrSuccededReadAttempts++;

                if (!MyPlc.IsConnected)
                    return;

                if (logTag.LogType == LogType.Delta)
                {
                    LastValue lastKnownLogValue = lastLogValue.FindLast(l => l.tag_id == logTag.Id);
                    if (lastKnownLogValue == null)
                    {
                        AddValueToSql(logTag, readValue);
                        return;
                    }

                    CheckDeadbandAndAddToSql(logTag, readValue, lastKnownLogValue);
                }
                else if (logTag.LogType == LogType.Cyclic)
                {
                    AddValueToSql(logTag, readValue);
                }
                else if (logTag.LogType == LogType.WriteWatchDogInt16 && !MyPlc.isOpc)
                {
                    var oldWd = await MyPlc.Plc.ReadAsync(plcTag);
                    var intValue = Convert.ToInt16(oldWd.Value) + 1;
                    await MyPlc.Plc.WriteAsync(plcTag, (short)intValue);
                }
                else if (logTag.LogType == LogType.TimeOfDay)
                {
                    var localTime = TimeZoneInfo.ConvertTimeFromUtc(tiden, TimeZoneInfo.Local);

                    bool timeMatches = localTime.Hour == logTag.TimeOfDay.Hours &&
                                       localTime.Minute == logTag.TimeOfDay.Minutes &&
                                       (logTag.TimeOfDay.Seconds == 0 || localTime.Second == logTag.TimeOfDay.Seconds);

                    if (timeMatches)
                    {
                        var allOccurrencesOfTagInList = lastLogValue.Find(n => n.tag_id == logTag.Id && n.ReadValue.LogTime.Day == tiden.Day);

                        if (allOccurrencesOfTagInList == null)
                        {
                            AddValueToSql(logTag, readValue);
                        }
                    }
                }


                // Check if tag has any subscribed event tags
                if (logTag.SubscribedTags.Count <= 0)
                    return;

                foreach (int id in logTag.SubscribedTags)
                {
                    var subbedTag = TagHelpers.GetTagFromId(id);
                    var tagg = (ITagDefinition)subbedTag;

                    plcTag = new PlcTag(tagg);

                    if (!subbedTag.Active)
                        continue;

                    subbedTag.LastLogTime = tiden;
                    var lastValue = lastLogValue.Find(l => l.tag_id == logTag.Id);

                    async Task LogSubscribedTagAsync()
                    {
                        var subbedLog = await MyPlc.Plc.ReadAsync(plcTag);
                        AddValueToSql(subbedTag, subbedLog);
                    }

                    if (subbedTag.IsBooleanTrigger && logTag.VarType == VarType.Bit)
                    {
                        switch (subbedTag.BoolTrigger)
                        {
                            case BooleanTrigger.OnTrue:
                                if (!(bool)lastValue.value && (bool)readValue.Value) await LogSubscribedTagAsync();
                                break;
                            case BooleanTrigger.WhileTrue:
                                if ((bool)readValue.Value) await LogSubscribedTagAsync();
                                break;
                            case BooleanTrigger.OnFalse:
                                if ((bool)lastValue.value && !(bool)readValue.Value) await LogSubscribedTagAsync();
                                break;
                            default:
                                break;
                        }
                        return;
                    }
                    else
                    {
                        if (subbedTag.AnalogTrigger == AnalogTrigger.LessThan && Convert.ToDouble(readValue.Value) < subbedTag.AnalogValue)
                        {
                            await LogSubscribedTagAsync();
                        }
                        else if (subbedTag.AnalogTrigger == AnalogTrigger.MoreThan && Convert.ToDouble(readValue.Value) > subbedTag.AnalogValue)
                        {
                            await LogSubscribedTagAsync();
                        }
                        else if (subbedTag.AnalogTrigger == AnalogTrigger.Equal && Convert.ToDouble(readValue.Value) == subbedTag.AnalogValue)
                        {
                            await LogSubscribedTagAsync();
                        }
                        else if (subbedTag.AnalogTrigger == AnalogTrigger.LessOrEqual && Convert.ToDouble(readValue.Value) <= subbedTag.AnalogValue)
                        {
                            await LogSubscribedTagAsync();
                        }
                        else if (subbedTag.AnalogTrigger == AnalogTrigger.MoreOrEqual && Convert.ToDouble(readValue.Value) >= subbedTag.AnalogValue)
                        {
                            await LogSubscribedTagAsync();
                        }
                        else if (subbedTag.AnalogTrigger == AnalogTrigger.NotEqual && Convert.ToDouble(readValue.Value) != subbedTag.AnalogValue)
                        {
                            await LogSubscribedTagAsync();
                        }
                    }
                }
            }
            catch (PlcException ex)
            {
                HandleException(ex, "Misslyckades att läsa");
            }
            catch (IOException ex)
            {
                HandleException(ex, "Misslyckades att läsa", "Ingen licens?");
            }
            catch (OpcUnableToReadTagException ex)
            {
                HandleException(ex, "Misslyckades att läsa", "Ingen licens?");
            }
            catch (OpcException ex)
            {
                HandleException(ex, "Misslyckades att läsa");
            }
            catch (OpcUaException ex)
            {
                HandleException(ex, "Misslyckades att läsa", logError: false);
            }
            catch (ServiceResultException ex)
            {
                HandleException(ex, "Misslyckades att läsa", logError: false);
            }

            void HandleException(Exception ex, string errorMessage, string customMessage = null, bool logError = true)
            {
                logTag.LastLogTime = tiden;
                string fullMessage = $"{errorMessage} {logTag.Name} från {MyPlc.PlcName}\r\n{(customMessage ?? ex.Message)}";
                MyPlc.SendPlcStatusMessage(fullMessage, Status.Error);

                if (logError)
                {
                    Apps.Logger.Log(fullMessage, Severity.Error, ex);
                    logTag.NrFailedReadAttempts++;
                    logTag.LastErrorMessage = ex.Message;
                }
            }
        }

        private static async Task<ReadValue> ReadTagValueAsync(ExtendedPlc myPlc, TagDefinitions logTag, PlcTag plcTag)
        {
            if (logTag.VarType == VarType.S7String && !myPlc.isOpc)
            {
                return await ReadS7StringAsync(myPlc, plcTag);
            }
            else if (logTag.VarType == VarType.String)
            {
                return await ReadStringAsync(myPlc, plcTag);
            }
            else if (logTag.VarType == VarType.DateTime)
            {
                return await ReadDateTimeAsync(myPlc, plcTag);
            }
            else if (logTag.VarType == VarType.DateTimeLong && !myPlc.isOpc)
            {
                return await ReadDateTimeLongAsync(myPlc, plcTag);
            }
            else if (logTag.LogType != LogType.WriteWatchDogInt16)
            {
                return await myPlc.Plc.ReadAsync(plcTag);
            }
            return null;
        }

        private static async Task<ReadValue> ReadS7StringAsync(ExtendedPlc MyPlc, PlcTag plcTag)
        {
            var temp = await MyPlc.Plc.ReadBytesAsync(plcTag, 2);
            var rdVal = new ReadValue(MyPlc.Plc, temp);
            var rs = (byte[])rdVal.Value;
            return new ReadValue(MyPlc.Plc, rs.S7StringSwedish());
        }

        private static async Task<ReadValue> ReadStringAsync(ExtendedPlc MyPlc, PlcTag plcTag)
        {
            if (!MyPlc.isOpc)
            {
                var temp = MyPlc.Plc.ReadBytes(plcTag, 2);
                return new ReadValue(MyPlc, temp.S7StringSwedish());
            }
            else
            {
                return await MyPlc.Plc.ReadAsync(plcTag);
            }
        }

        private static async Task<ReadValue> ReadDateTimeAsync(ExtendedPlc MyPlc, PlcTag plcTag)
        {
            if (!MyPlc.isOpc)
            {
                var temp = await MyPlc.Plc.ReadBytesAsync(plcTag, 8);
                return new ReadValue(MyPlc, S7.Net.Types.DateTime.FromByteArray(temp));
            }
            else
            {
                return await MyPlc.Plc.ReadAsync(plcTag);
            }
        }

        private static async Task<ReadValue> ReadDateTimeLongAsync(ExtendedPlc MyPlc, PlcTag plcTag)
        {
            var temp = await MyPlc.Plc.ReadBytesAsync(plcTag, 12);
            return new ReadValue(MyPlc, S7.Net.Types.DateTimeLong.FromByteArray(temp));
        }

        private static void CheckDeadbandAndAddToSql(TagDefinitions logTag, ReadValue readValue, LastValue lastKnownLogValue)
        {
            bool shouldAdd = false;

            if (logTag.Deadband <= 0)
            {
                shouldAdd = true;
            }
            else
            {
                switch (logTag.VarType)
                {
                    case VarType.Bit:
                        shouldAdd = (bool)readValue.Value != (bool)lastKnownLogValue.value;
                        break;
                    case VarType.Byte:
                        shouldAdd = Math.Abs((byte)readValue.Value - (byte)lastKnownLogValue.value) >= (byte)logTag.Deadband;
                        break;
                    case VarType.Word:
                        shouldAdd = Math.Abs((ushort)readValue.Value - (ushort)lastKnownLogValue.value) >= (ushort)logTag.Deadband;
                        break;
                    case VarType.DWord:
                        shouldAdd = Math.Abs((uint)readValue.Value - (uint)lastKnownLogValue.value) >= (uint)logTag.Deadband;
                        break;
                    case VarType.Int:
                        shouldAdd = Math.Abs((short)readValue.Value - (short)lastKnownLogValue.value) >= (short)logTag.Deadband;
                        break;
                    case VarType.DInt:
                        shouldAdd = Math.Abs((int)readValue.Value - (int)lastKnownLogValue.value) >= (int)logTag.Deadband;
                        break;
                    case VarType.Real:
                        shouldAdd = Math.Abs((float)readValue.Value - (float)lastKnownLogValue.value) >= (float)logTag.Deadband;
                        break;
                    case VarType.String:
                        if (!Equals(readValue.ToString(), lastKnownLogValue.value.ToString()))
                            shouldAdd = true;
                        break;
                    case VarType.S7String:
                        if (!Equals(readValue.ToString(), lastKnownLogValue.value.ToString()))
                            shouldAdd = true;
                        break;
                    default:
                        break;
                }
            }
            if (shouldAdd)
            {
                AddValueToSql(logTag, readValue);
            }
        }

        /// <summary>
        /// Synkronisera PLC-klockan en gång om dagen
        /// </summary>
        /// <param name="MyPlc"></param>
        /// <param name="tid"></param>
        private static async Task SyncPlc(ExtendedPlc MyPlc, DateTime tid)
        {
            var tid1 = TimeZoneInfo.ConvertTimeFromUtc(tid, TimeZoneInfo.Local);
            var plcTag = new PlcTag(DataType.DataBlock, MyPlc.SyncTimeDbNr, MyPlc.SyncTimeOffset);

            try
            {
                switch ((CpuType)MyPlc.CpuType)
                {
                    case CpuType.S7200:
                        break;
                    case CpuType.Logo0BA8:
                        break;
                    case CpuType.S7300:
                        // Write Time
                        var tidBytes = S7.Net.Types.DateTime.ToByteArray(tid1);
                        await MyPlc.Plc.WriteBytesAsync(plcTag, tidBytes);
                        await MyPlc.Plc.WriteAsync(MyPlc.SyncBoolAddress, 1);
                        break;
                    case CpuType.S7400:
                        var tidByte = S7.Net.Types.DateTime.ToByteArray(tid1);
                        await MyPlc.Plc.WriteBytesAsync(plcTag, tidByte);
                        await MyPlc.Plc.WriteAsync(MyPlc.SyncBoolAddress, 1);
                        break;
                    case CpuType.S71200:
                        break;
                    case CpuType.S71500:
                        var tidByt = S7.Net.Types.DateTimeLong.ToByteArray(tid1);
                        await MyPlc.Plc.WriteBytesAsync(plcTag, tidByt);
                        await MyPlc.Plc.WriteAsync(MyPlc.SyncBoolAddress, 1);
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
                    MyPlc.Plc.Disconnect();

                    if (message == string.Empty)
                        MyPlc.SendPlcStatusMessage($"Kommunikationen till {MyPlc.PlcName} avbruten", Status.Warning);
                    else
                        Apps.Logger.Log($"Kommunikationen upprättades ej till {MyPlc.PlcName}\n\r{message}", Severity.Error);

                    OnlineStatusEvent.RaiseMessage(MyPlc.ConnectionStatus, MyPlc.PlcName);
                    MyPlc.LoggerIsStarted = false;
                }
            }


            startClosing = false;
            lastLogValue?.Clear();

        }

        private static void AddValueToSql(TagDefinitions logTag, ReadValue readValue)
        {
            lock (sqlLock)
            {
                if (readValue.Value == null)
                    return;

                logTag.TimesLogged++;
                lastLogValue.Add(new LastValue()
                {
                    tag_id = logTag.Id,
                    ReadValue = readValue,
                });

                RemoveOldLogValues(logTag.Id);

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

                SendValuesToSql.AddRawValue(readValue, logValue);
            }
        }

        private static void RemoveOldLogValues(int tagId)
        {
            var allOccurrencesOfTagInList = lastLogValue.FindAll(n => n.tag_id == tagId).OrderBy(dat => dat.ReadValue.LogTime).ToList();
            var nrOfItemsInLastLog = lastLogValue.FindAll(n => n.tag_id == tagId);

            // Garantera att det bara finns ett värde bakåt
            if (nrOfItemsInLastLog.Count > 2)
            {
                var removeDate = allOccurrencesOfTagInList[nrOfItemsInLastLog.Count - 3].ReadValue.LogTime;
                lastLogValue.RemoveAll(i => i.tag_id == tagId && i.ReadValue.LogTime <= removeDate);
            }
        }
    }
}
