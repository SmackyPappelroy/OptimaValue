using FileLogger;
using Opc.Ua;
using OpcUaHm;
using OpcUaHm.Common;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OptimaValue;


public static class LoggerHandler
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

    private static List<LastValue> lastLogValues;

    private static Task logTask;
    private static CancellationTokenSource cancelTokenSource = new();
    private static System.Timers.Timer onlineTimer;

    public static bool startClosing = false;
    private static bool AppIsShuttingDown = false;

    public static void OnStartedEvent(EventArgs e)
    {
        StartedEvent?.Invoke(typeof(LoggerHandler), e);
    }

    public static void OnRestartEvent(EventArgs e)
    {
        RestartEvent?.Invoke(typeof(LoggerHandler), e);
    }

    public static void Start()
    {
        if (!AreActivePlcsAndTags())
        {
            Logger.LogError("Inga aktiva Plc eller aktiva taggar");
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
            if (!MyPlc.Active || !MyPlc.ActiveTagsInPlc)
                continue;

            MyPlc.LoggerIsStarted = true;
            StartStopButtonEvent.RaiseMessage(true);

            if (!AppIsShuttingDown)
            {
                OnStartedEvent(EventArgs.Empty);
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
            Logger.LogInfo("Loggningscykel avslutad");
            cancelTokenSource = new CancellationTokenSource();
        }, TaskContinuationOptions.OnlyOnCanceled);
    }

    public static void RequestDisconnect()
    {
        if (logTask == null || cancelTokenSource.IsCancellationRequested)
            return;

        cancelTokenSource.Cancel();
        Master.Stopping = true;
        Master.IsStarted = false;
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
        lastLogValues ??= new List<LastValue>();
    }

    private static async Task CheckConnectionsAsync()
    {
        foreach (ExtendedPlc plc in PlcConfig.PlcList)
        {
            if (!plc.Active || plc.IsConnected)
                continue;

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
    private static async Task CheckReconnectAsync(ExtendedPlc MyPlc)
    {
        if (MyPlc.ConnectionStatus == ConnectionStatus.Connected)
            return;

        var tiden = DateTime.UtcNow;

        if (tiden - MyPlc.LastReconnect < TimeSpan.FromSeconds(3))
            return;

        try
        {
            if (MyPlc.isOpc)
            {
                await ReconnectOpc(MyPlc);
            }
            else
            {
                await ReconnectPlc(MyPlc);
            }

            MyPlc.LastReconnect = tiden;
        }
        catch (Exception ex)
        {
            LogReconnectError(MyPlc, ex);
            MyPlc.LastReconnect = tiden;
        }
    }

    private static void LogConnectionError(ExtendedPlc plc, Exception ex = null)
    {
        Logger.LogError($"Lyckas ej ansluta till {plc.PlcName}", ex);
    }

    private static async Task ProcessAllPlcTagsAndHandleClosing()
    {
        foreach (ExtendedPlc MyPlc in PlcConfig.PlcList)
        {
            if (!MyPlc.Active)
                continue;
            await CheckPlcStatusAsync(MyPlc);
            await CheckReconnectAsync(MyPlc);
            await ProcessPlcTags(MyPlc);

            if (startClosing)
            {
                RequestDisconnect();
            }
        }
    }

    private static async Task CheckPlcStatusAsync(ExtendedPlc myPlc)
    {
        if (!myPlc.Plc.IsStreamConnected)
        {
            return;
        }
        // Check Plc status once a minute
        if (myPlc.Plc.LastPlcStatusCheck != default
                && DateTime.UtcNow - myPlc.Plc.LastPlcStatusCheck <= TimeSpan.FromMinutes(1))
        {
            return;
        }

        myPlc.Plc.LastPlcStatusCheck = DateTime.UtcNow;
        var isPlcRunning = await myPlc.Plc.IsCpuInRunAsync();
        if (!isPlcRunning)
        {
            Logger.LogWarning($"{myPlc.PlcName}-PLC är i STOPP");
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
        if (tag.LogType != LogType.Adaptive)
        {
            if (tag.LastLogTime.AddMilliseconds((double)logFreq) < DateTime.UtcNow.AddMilliseconds(100))
            {
                await ReadValue(plc, tag);
            }
        }
        else
        {
            if (tag.LastLogTime.AddMilliseconds((double)tag.CustomLogFrequency) < DateTime.UtcNow.AddMilliseconds(100))
            {
                await ReadValue(plc, tag);
            }
        }
    }



    private static async Task ReconnectOpc(ExtendedPlc MyPlc)
    {
        if (!MyPlc.IsConnected)
            await MyPlc.Plc.ConnectAsync();

        LogConnectionStatus(MyPlc);
    }

    private static async Task ReconnectPlc(ExtendedPlc MyPlc)
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
        Logger.Log($"{action} till {MyPlc.PlcName}\r\n{MyPlc.PlcConfiguration.Ip}", severity);
    }

    private static void LogReconnectError(ExtendedPlc MyPlc, Exception ex)
    {
        MyPlc.SendPlcStatusMessage($"Misslyckades att ansluta till {MyPlc.PlcName}\r\n{MyPlc.PlcConfiguration.Ip}", Status.Error);
        Logger.LogError($"Misslyckades att ansluta till {MyPlc.PlcName}\r\n{MyPlc.PlcConfiguration.Ip}", ex);
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
                if (!MyPlc.UnableToPing)
                    return;
                var errorString = $"Lyckas ej pinga {MyPlc.PlcName}\r\n{MyPlc.PlcConfiguration.Ip}";
                logTag.LastErrorMessage = errorString;
                MyPlc.SendPlcStatusMessage(errorString, Status.Error);
                return;
            }

            // Synkronisera klockan
            if (tiden > MyPlc.lastSyncTime + TimeSpan.FromDays(1) && MyPlc.SyncActive && !MyPlc.isOpc)
                await SyncPlc(MyPlc, tiden);

            ReadValue readValue = null;
            if (logTag.LogType != LogType.Calculated)
            {
                readValue = await ReadTagValueAsync(MyPlc, logTag, plcTag);
            }

            logTag.LastLogTime = tiden;
            logTag.NrSuccededReadAttempts++;

            if (!MyPlc.IsConnected)
                return;

            switch (logTag.LogType)
            {
                case LogType.Delta:
                    {
                        LastValue lastKnownLogValue = lastLogValues.FindLast(l => l.tag_id == logTag.Id);
                        if (lastKnownLogValue == null)
                        {
                            AddValueToSql(logTag, readValue);
                            return;
                        }

                        CheckDeadbandAndAddToSql(logTag, readValue, lastKnownLogValue);
                        break;
                    }
                case LogType.RateOfChange:
                    {
                        LastValue lastKnownLogValue = lastLogValues.FindLast(l => l.tag_id == logTag.Id);
                        if (lastKnownLogValue == null)
                        {
                            AddValueToSql(logTag, readValue);
                            return;
                        }

                        CheckRateOfChangeAndAddToSql(logTag, readValue, lastKnownLogValue);

                        void CheckRateOfChangeAndAddToSql(TagDefinitions logTag, ReadValue readValue, LastValue lastKnownLogValue)
                        {
                            double rateOfChangeThreshold = logTag.Deadband; // Define your rate of change threshold here
                            DateTime currentTime = DateTime.UtcNow;

                            double valueDifference = Math.Abs(readValue.ValueAsFloat - Convert.ToSingle(lastKnownLogValue.value));
                            double timeDifferenceInSeconds = (currentTime - lastKnownLogValue.last_updated).TotalSeconds;

                            if (timeDifferenceInSeconds == 0) // Prevent division by zero
                            {
                                return;
                            }

                            double rateOfChange = valueDifference / timeDifferenceInSeconds;

                            if (rateOfChange >= rateOfChangeThreshold)
                            {
                                AddValueToSql(logTag, readValue);
                            }
                        }

                        break;
                    }
                case LogType.Cyclic:
                    AddValueToSql(logTag, readValue);
                    break;
                case LogType.WriteWatchDogInt16 when !MyPlc.isOpc:
                    {
                        var oldWd = await MyPlc.Plc.ReadAsync(plcTag);
                        var intValue = Convert.ToInt16(oldWd.Value) + 1;
                        await MyPlc.Plc.WriteAsync(plcTag, (short)intValue);
                        break;
                    }
                case LogType.TimeOfDay:
                    {
                        var localTime = TimeZoneInfo.ConvertTimeFromUtc(tiden, TimeZoneInfo.Local);

                        bool timeMatches = localTime.Hour == logTag.TimeOfDay.Hours &&
                                           localTime.Minute == logTag.TimeOfDay.Minutes &&
                                           (logTag.TimeOfDay.Seconds == 0 || localTime.Second == logTag.TimeOfDay.Seconds);

                        if (timeMatches)
                        {
                            var allOccurrencesOfTagInList = lastLogValues.Find(n => n.tag_id == logTag.Id && n.ReadValue.LogTime.Day == tiden.Day);

                            if (allOccurrencesOfTagInList == null)
                            {
                                AddValueToSql(logTag, readValue);
                            }
                        }

                        break;
                    }
                case LogType.Calculated:
                    {
                        var tagIdsAndOperators = ExtractTagIdsAndOperators(logTag.Calculation);
                        var allTagsInCalculation = GetTagsFromIds(tagIdsAndOperators.TagIds);

                        var allValues = await ReadAllTagValuesAsync(allTagsInCalculation);
                        var calculatedValue = CalculateValue(allValues, tagIdsAndOperators.Operators);
                        var calculateReadValue = new ReadValue(MyPlc.Plc, calculatedValue);
                        AddValueToSql(logTag, calculateReadValue);

                        (List<string> TagIds, List<string> Operators) ExtractTagIdsAndOperators(string calculation)
                        {
                            var tagIds = new List<string>();
                            var operators = new List<string>();
                            var currentTagId = new StringBuilder();

                            foreach (char c in calculation)
                            {
                                if (char.IsDigit(c))
                                {
                                    currentTagId.Append(c);
                                }
                                else
                                {
                                    if (currentTagId.Length > 0)
                                    {
                                        tagIds.Add(currentTagId.ToString());
                                        currentTagId.Clear();
                                    }

                                    if (!char.IsWhiteSpace(c))
                                    {
                                        operators.Add(c.ToString());
                                    }
                                }
                            }

                            if (currentTagId.Length > 0)
                            {
                                tagIds.Add(currentTagId.ToString());
                            }

                            return (TagIds: tagIds, Operators: operators);
                        }



                        List<TagDefinitions> GetTagsFromIds(List<string> tagIds)
                        {
                            var tags = new List<TagDefinitions>();
                            foreach (var tagId in tagIds)
                            {
                                var tagIdInt = Convert.ToInt32(tagId);
                                var tagToAdd = TagHelpers.GetTagFromId(tagIdInt);
                                tags.Add(tagToAdd);
                            }
                            return tags;
                        }

                        async Task<List<ReadValue>> ReadAllTagValuesAsync(List<TagDefinitions> tags)
                        {
                            var values = new List<ReadValue>();
                            foreach (var tag in tags)
                            {
                                var plcTag = new PlcTag(tag);
                                var value = await MyPlc.Plc.ReadAsync(plcTag);
                                values.Add(value);
                            }
                            return values;
                        }

                        float CalculateValue(List<ReadValue> allValues, List<string> operators)
                        {
                            var infix = new StringBuilder();
                            var allValuesCount = allValues.Count;
                            var operatorIndex = 0;

                            for (int i = 0; i < allValuesCount; i++)
                            {
                                while (operatorIndex < operators.Count && operators[operatorIndex] == "(")
                                {
                                    infix.Append(operators[operatorIndex]);
                                    operatorIndex++;
                                }

                                var value = allValues[i];
                                var valueFloat = Convert.ToSingle(value.Value);
                                infix.Append(valueFloat);

                                while (operatorIndex < operators.Count && (operators[operatorIndex] == ")" || i == allValuesCount - 1))
                                {
                                    infix.Append(operators[operatorIndex]);
                                    operatorIndex++;
                                }

                                if (i < allValuesCount - 1)
                                {
                                    infix.Append(operators[operatorIndex]);
                                    operatorIndex++;
                                }
                            }

                            string postfix = ConvertToPostfix(infix.ToString());
                            return EvaluatePostfix(postfix);
                        }
                        break;
                    }
                case LogType.Adaptive:
                    {
                        LastValue lastKnownLogValue = lastLogValues.FindLast(l => l.tag_id == logTag.Id);
                        if (lastKnownLogValue == null)
                        {
                            AddValueToSql(logTag, readValue);
                            return;
                        }

                        CheckAdaptiveRateOfChangeAndAddToSql(logTag, readValue, lastKnownLogValue);
                        void CheckAdaptiveRateOfChangeAndAddToSql(TagDefinitions logTag, ReadValue readValue, LastValue lastKnownLogValue)
                        {
                            SetDefaultScale(logTag);
                            SetDefaultRaw(logTag);

                            double lowThreshold = logTag.scaleMin;
                            double highThreshold = logTag.scaleMax;
                            DateTime currentTime = DateTime.UtcNow;

                            double valueDifference = Math.Abs(readValue.ValueAsFloat - lastKnownLogValue.ReadValue.ValueAsFloat);
                            double timeDifferenceInSeconds = (currentTime - lastKnownLogValue.last_updated).TotalSeconds;

                            if (timeDifferenceInSeconds == 0) // Prevent division by zero
                            {
                                return;
                            }

                            double rateOfChange = valueDifference / timeDifferenceInSeconds;

                            double lowChangeInterval = logTag.rawMax;
                            double highChangeInterval = logTag.rawMin;

                            UpdateCustomLogFrequency(logTag, rateOfChange, lowThreshold, highThreshold, lowChangeInterval, highChangeInterval);
                            AddValueToSql(logTag, readValue);

                        }
                        void SetDefaultScale(TagDefinitions logTag)
                        {
                            if (logTag.scaleMin == 0) logTag.scaleMin = 0.1f;
                            if (logTag.scaleMax == 0) logTag.scaleMax = 1;
                            if (logTag.scaleMin > logTag.scaleMax)
                            {
                                logTag.scaleMin = 0.1f;
                                logTag.scaleMax = 1;
                            }
                        }

                        void SetDefaultRaw(TagDefinitions logTag)
                        {
                            if (logTag.rawMin == 0) logTag.rawMin = 1000;
                            if (logTag.rawMax == 0) logTag.rawMax = 10000;
                            if (logTag.rawMin > logTag.rawMax)
                            {
                                logTag.rawMin = 1000;
                                logTag.rawMax = 10000;
                            }
                        }
                        void UpdateCustomLogFrequency(TagDefinitions logTag, double rateOfChange, double lowThreshold, double highThreshold, double lowChangeInterval, double highChangeInterval)
                        {
                            if (rateOfChange <= lowThreshold)
                            {
                                logTag.CustomLogFrequency = (int)lowChangeInterval;
                            }
                            else if (rateOfChange >= highThreshold)
                            {
                                logTag.CustomLogFrequency = (int)highChangeInterval;
                            }
                            else
                            {
                                // Linear interpolation between lowChangeInterval and highChangeInterval
                                double slope = (highChangeInterval - lowChangeInterval) / (highThreshold - lowThreshold);
                                double intercept = lowChangeInterval - slope * lowThreshold;
                                logTag.CustomLogFrequency = (int)(slope * rateOfChange + intercept);
                            }
                        }
                        break;
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
                var lastValue = lastLogValues.Find(l => l.tag_id == logTag.Id);

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

            if (!logError)
                return;
            Logger.LogError(fullMessage, ex);
            logTag.NrFailedReadAttempts++;
            logTag.LastErrorMessage = ex.Message;
        }
    }

    /// <summary>
    /// Hämta företrädet för den angivna operatorn.
    /// </summary>
    /// <param name="operatorToUse">Operatorns tecken att kontrollera.</param>
    /// <returns>Ett heltal som representerar företrädet för operatorn</returns>
    private static int GetPrecedence(char operatorToUse)
    {
        switch (operatorToUse)
        {
            case '+':
            case '-':
                return 1;
            case '*':
            case '/':
                return 2;
            default:
                return 0;
        }
    }

    /// <summary>
    /// Konvertera infix-uttryck till postfix-uttryck.
    /// </summary>
    /// <param name="infix">Infix-uttrycket som ska konverteras.</param>
    /// <returns>En sträng som representerar det konverterade postfix-uttrycket.</returns>
    private static string ConvertToPostfix(string infix)
    {
        var output = new StringBuilder();
        var operatorStack = new Stack<char>();

        foreach (char token in infix)
        {
            if (char.IsWhiteSpace(token))
            {
                continue;
            }

            if (char.IsDigit(token) || token == '.')
            {
                output.Append(token);
            }
            else if (token == '(')
            {
                operatorStack.Push(token);
            }
            else if (token == ')')
            {
                while (operatorStack.Count > 0 && operatorStack.Peek() != '(')
                {
                    output.Append(' ');
                    output.Append(operatorStack.Pop());
                }
                operatorStack.Pop();
            }
            else // token is an operator
            {
                output.Append(' ');

                while (operatorStack.Count > 0 && GetPrecedence(token) <= GetPrecedence(operatorStack.Peek()))
                {
                    output.Append(operatorStack.Pop());
                }
                operatorStack.Push(token);
            }
        }

        while (operatorStack.Count > 0)
        {
            output.Append(' ');
            output.Append(operatorStack.Pop());
        }

        return output.ToString();
    }

    /// <summary>
    /// Utför operationen mellan två värden med hjälp av den angivna operatorn.
    /// </summary>
    /// <param name="currentValue">Det första värdet.</param>
    /// <param name="value">Det andra värdet.</param>
    /// <param name="operatorToUse">Operatorn som ska användas.</param>
    /// <returns>Ett flyttal som representerar resultatet av operationen.</returns>
    private static float EvaluatePostfix(string postfix)
    {
        var valueStack = new Stack<float>();

        string[] tokens = postfix.Split(' ');
        foreach (string token in tokens)
        {
            if (float.TryParse(token, out float value))
            {
                valueStack.Push(value);
            }
            else
            {
                if (valueStack.Count < 2)
                {
                    throw new InvalidOperationException($"Insufficient operands in the postfix expression: '{postfix}'");
                }

                float right = valueStack.Pop();
                float left = valueStack.Pop();
                valueStack.Push(PerformOperation(left, right, token));
            }
        }

        if (valueStack.Count != 1)
        {
            throw new InvalidOperationException($"Invalid postfix expression: '{postfix}'");
        }

        return valueStack.Pop();
    }

    /// <summary>
    /// Utför operationen mellan två värden med hjälp av den angivna operatorn.
    /// </summary>
    /// <param name="currentValue">Det första värdet.</param>
    /// <param name="value">Det andra värdet.</param>
    /// <param name="operatorToUse">Operatorn som ska användas.</param>
    /// <returns>Ett flyttal som representerar resultatet av operationen.</returns>
    private static float PerformOperation(float currentValue, float value, string operatorToUse)
    {
        switch (operatorToUse)
        {
            case "+":
                return currentValue + value;
            case "-":
                return currentValue - value;
            case "*":
                return currentValue * value;
            case "/":
                if (value == 0)
                {
                    Logger.LogError("Division by zero");
                    return float.NaN; // Return NaN to represent an invalid result
                }
                return currentValue / value;
            default:
                throw new ArgumentException($"Invalid operator: {operatorToUse}");
        }
    }




    private static async Task<ReadValue> ReadTagValueAsync(ExtendedPlc myPlc, TagDefinitions logTag, PlcTag plcTag)
    {
        switch (logTag.VarType)
        {
            case VarType.S7String when !myPlc.isOpc:
                return await ReadS7StringAsync(myPlc, plcTag);
            case VarType.String:
                return await ReadStringAsync(myPlc, plcTag);
            case VarType.DateTime:
                return await ReadDateTimeAsync(myPlc, plcTag);
            case VarType.DateTimeLong when !myPlc.isOpc:
                return await ReadDateTimeLongAsync(myPlc, plcTag);
            default:
                if (logTag.LogType != LogType.WriteWatchDogInt16)
                    return await myPlc.Plc.ReadAsync(plcTag);
                return null;
        }
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

        // Add new condition to check if the value has changed and Deadband is 0
        if (logTag.Deadband == 0 && !Equals(readValue.Value, lastKnownLogValue.value))
        {
            shouldAdd = true;
        }
        else if (logTag.Deadband > 0)
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
            Logger.LogError($"Misslyckades att synka {MyPlc.PlcName}");
            throw;
        }
        MyPlc.lastSyncTime = tid;

        DatabaseSql.SaveSyncTime(tid: tid, plcName: MyPlc.PlcName);

        Logger.LogSuccess($"Synkade {MyPlc.PlcName}");
    }

    private static void AbortLogThread(string message)
    {
        foreach (ExtendedPlc MyPlc in PlcConfig.PlcList)
        {
            if (!MyPlc.LoggerIsStarted)
                continue;

            MyPlc.Plc.Disconnect();

            if (message == string.Empty)
                MyPlc.SendPlcStatusMessage($"Kommunikationen till {MyPlc.PlcName} avbruten", Status.Warning);
            else
                Logger.LogError($"Kommunikationen upprättades ej till {MyPlc.PlcName}\n\r{message}");

            OnlineStatusEvent.RaiseMessage(MyPlc.ConnectionStatus, MyPlc.PlcName);
            MyPlc.LoggerIsStarted = false;
        }


        startClosing = false;
        lastLogValues?.Clear();

    }

    private static void AddValueToSql(TagDefinitions logTag, ReadValue readValue)
    {
        lock (sqlLock)
        {
            if (readValue.Value == null)
                return;

            logTag.TimesLogged++;
            lastLogValues.Add(new LastValue()
            {
                tag_id = logTag.Id,
                ReadValue = readValue,
                last_updated = DateTime.UtcNow
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
        var allOccurrencesOfTagInList = lastLogValues.FindAll(n => n.tag_id == tagId).OrderBy(dat => dat.ReadValue.LogTime).ToList();
        var nrOfItemsInLastLog = lastLogValues.FindAll(n => n.tag_id == tagId);

        // Garantera att det bara finns ett värde bakåt
        if (nrOfItemsInLastLog.Count > 2)
        {
            var removeDate = allOccurrencesOfTagInList[nrOfItemsInLastLog.Count - 3].ReadValue.LogTime;
            lastLogValues.RemoveAll(i => i.tag_id == tagId && i.ReadValue.LogTime <= removeDate);
        }
    }
}
