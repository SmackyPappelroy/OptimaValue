using S7.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace OptimaValue
{

    public class Logger
    {
        private readonly ExtendedPlc MyPlc = null;
        public event EventHandler StartedEvent;

        public Logger(ExtendedPlc myPlc)
        {
            MyPlc = myPlc;
        }
        private List<LastValue> lastLogValue;

        private Thread logThread;

        private bool isStarted = false;

        private System.Timers.Timer onlineTimer;

        public bool startClosing = false;
        private bool AppIsShuttingDown = false;

        public bool isSubscribed = false;

        protected virtual void OnStartedEvent(EventArgs e)
        {
            StartedEvent?.Invoke(this, e);
        }

        public bool IsStarted
        {
            get => isStarted;
            set
            {
                StartStopButtonEvent.RaiseMessage(value);
                isStarted = value;
                if (!AppIsShuttingDown)
                    OnStartedEvent(EventArgs.Empty);
            }
        }

        public void Start()
        {
            if (IsStarted)
                return;

            if (logThread != null)
                logThread = null;

            logThread = new Thread(Cycler);
            logThread.Start();

            IsStarted = true;
        }

        public void Stop(bool applicationShutdown)
        {
            if (applicationShutdown)
                AppIsShuttingDown = true;
            startClosing = true;
        }

        private void Cycler()
        {
            if (onlineTimer == null)
                onlineTimer = new System.Timers.Timer()
                {
                    Interval = 500
                };
            if (!isSubscribed)
            {
                onlineTimer.Elapsed += OnlineTimer_Elapsed;
                isSubscribed = true;
            }



            try
            {
                MyPlc.Open();
                if (!MyPlc.IsConnected)
                {
                    Apps.Logger.Log($"Får ej kontakt med {MyPlc.PlcName}", Severity.Error);
                    AbortLogThread(string.Empty);
                }
                else
                {
                    MyPlc.ConnectionStatus = ConnectionStatus.Connected;
                    onlineTimer.Start();
                }

            }
            catch (PlcException ex)
            {
                Apps.Logger.Log($"Lyckas ej ansluta till {MyPlc.PlcName}", Severity.Error, ex);
                AbortLogThread(string.Empty);
            }



            if (SendValuesToSql.rawValueBlock == null)
                SendValuesToSql.rawValueBlock = new BlockingCollection<rawValueClass>();

            if (lastLogValue == null)
                lastLogValue = new List<LastValue>();


            while (IsStarted)
            {
                if (MyPlc == null)
                    AbortLogThread("Hittade ingen PLC");

                // Reconnect om det ej går att pinga PLC
                if (MyPlc.ConnectionStatus != ConnectionStatus.Connected)
                {
                    if (MyPlc.ReconnectRetries < MyPlc.MaxReconnectRetries)
                    {
                        if (DateTime.Now - MyPlc.LastReconnect > TimeSpan.FromSeconds(30))
                        {
                            try
                            {
                                MyPlc.ReconnectRetries++;

                                MyPlc.Close();
                                MyPlc.Open();
                                if (MyPlc.IsConnected)
                                {
                                    MyPlc.SendPlcStatusMessage($"Lyckades återansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}\r\nFörsök nummer: {MyPlc.ReconnectRetries}", Status.Ok);
                                    Apps.Logger.Log($"Lyckades återansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}\r\nFörsök nummer: {MyPlc.ReconnectRetries}", Severity.Normal);
                                    MyPlc.ConnectionStatus = ConnectionStatus.Connected;
                                    MyPlc.ReconnectRetries = 0;
                                }
                                else
                                {
                                    MyPlc.SendPlcStatusMessage($"Misslyckades att återansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}\r\nFörsök nummer: {MyPlc.ReconnectRetries}", Status.Error);
                                    Apps.Logger.Log($"Misslyckades att återansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}\r\nFörsök nummer: {MyPlc.ReconnectRetries}", Severity.Error);
                                }
                                MyPlc.LastReconnect = DateTime.Now;
                            }
                            catch (Exception ex)
                            {
                                MyPlc.SendPlcStatusMessage($"Misslyckades att ansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}\r\nFörsök nummer: {MyPlc.ReconnectRetries}", Status.Error);
                                Apps.Logger.Log($"Misslyckades att ansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}\r\nFörsök nummer: {MyPlc.ReconnectRetries}", Severity.Error, ex);
                                MyPlc.LastReconnect = DateTime.Now;
                            }
                        }


                    }
                    else
                    {
                        MyPlc.SendPlcStatusMessage($"Max reconnect försök för {MyPlc.PlcName}\r\nAvbryter!", Status.Error);
                        Apps.Logger.Log($"Max reconnect försök för {MyPlc.PlcName}\r\nAvbryter!", Severity.Error);
                        if (PlcPicker.NrActiveEnabledPlc == 1)
                            Master.StopLog(true);
                        else
                            AbortLogThread("");
                    }
                }

                if (MyPlc.IsConnected)
                    foreach (TagDefinitions logValue in TagsToLog.AllLogValues)
                        if (logValue.plcName.Equals(MyPlc.PlcName))
                            ReadValue(logValue);
                if (startClosing)
                {
                    AbortLogThread(string.Empty);
                }
            }
        }

        private void OnlineTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (MyPlc != null)
                MyPlc.SendPlcOnlineMessage(MyPlc.ConnectionStatus, MyPlc.UpTimeString);
            else
                MyPlc.SendPlcOnlineMessage(ConnectionStatus.Disconnected, string.Empty);
        }

        private void AbortLogThread(string message)
        {
            MyPlc.Close();

            if (message == string.Empty)
                MyPlc.SendPlcStatusMessage($"Kommunikationen till {MyPlc.PlcName} avbruten", Status.Warning);
            else
                Apps.Logger.Log($"Kommunikationen upprättades ej till {MyPlc.PlcName}\n\r{message}", Severity.Error);
            MyPlc.ReconnectRetries = 0;


            MyPlc.ConnectionStatus = ConnectionStatus.Disconnected;
            OnlineStatusEvent.RaiseMessage(MyPlc.ConnectionStatus, MyPlc.PlcName);
            onlineTimer.Elapsed -= OnlineTimer_Elapsed;
            isSubscribed = false;
            IsStarted = false;
            startClosing = false;
            if (lastLogValue != null)
                lastLogValue.Clear();
            logThread.Abort();
        }




        private void ReadValue(TagDefinitions logTag)
        {
            if (logTag.active == true && MyPlc.PlcName == logTag.plcName)
            {
                int logdiff;
                if ((int)logTag.logFreq <= 250)
                    logdiff = 2;
                else
                    logdiff = 0;

                if ((DateTime.Now - logTag.LastLogTime) >= TimeSpan.FromMilliseconds((int)logTag.logFreq - logdiff)) // Minskar med 2 millisekunder vid snabb loggning för att få en mer exakt loggning
                {
                    object unknownTag = new object();
                    try
                    {
                        if (MyPlc.ConnectionStatus == ConnectionStatus.Connected)
                        {
                            if (logTag.varType == VarType.String || logTag.varType == VarType.StringEx)
                                unknownTag = MyPlc.ReadS7stringToString(logTag.dataType, logTag.blockNr, logTag.startByte, logTag.nrOfElements);
                            else
                                unknownTag = MyPlc.Read(logTag.dataType, logTag.blockNr, logTag.startByte, logTag.varType, logTag.nrOfElements, logTag.bitAddress);

                            logTag.LastLogTime = DateTime.Now;
                            logTag.NrSuccededReadAttempts++;


                            if (logTag.logType == LogType.Delta)
                            {
                                var lastKnownLogValue = lastLogValue.FindLast(l => l.name == logTag.name);
                                if (lastKnownLogValue == null)
                                {
                                    AddValueToSql(logTag, unknownTag);
                                    return;
                                }

                                if (logTag.deadband > 0)
                                {

                                    switch (logTag.varType)
                                    {
                                        case VarType.Bit:
                                            if ((bool)unknownTag != (bool)lastKnownLogValue.value)
                                                AddValueToSql(logTag, unknownTag);
                                            break;
                                        case VarType.Byte:
                                            if (unknownTag is byte unknownConverted)
                                            {
                                                if (Math.Abs(unknownConverted - ((byte)lastKnownLogValue.value)) > (float)((byte)logTag.deadband))
                                                    AddValueToSql(logTag, unknownTag);
                                            }
                                            else
                                                AddValueToSql(logTag, unknownTag);
                                            break;
                                        case VarType.Word:
                                            if (unknownTag is ushort unknownUShort)
                                            {
                                                if (Math.Abs(unknownUShort - ((ushort)lastKnownLogValue.value)) > (float)((ushort)logTag.deadband))
                                                    AddValueToSql(logTag, unknownTag);
                                            }
                                            else
                                                AddValueToSql(logTag, unknownTag);
                                            break;
                                        case VarType.DWord:
                                            if (unknownTag is uint uknonwnUint)
                                            {
                                                if (Math.Abs(uknonwnUint - ((uint)lastKnownLogValue.value)) > (float)((uint)logTag.deadband))
                                                    AddValueToSql(logTag, unknownTag);
                                            }
                                            else
                                                AddValueToSql(logTag, unknownTag);
                                            break;
                                        case VarType.Int:
                                            if (unknownTag is short unknownShort)
                                            {
                                                if (Math.Abs(unknownShort - ((short)lastKnownLogValue.value)) > (float)((short)logTag.deadband))
                                                    AddValueToSql(logTag, unknownTag);
                                            }
                                            else
                                                AddValueToSql(logTag, unknownTag);
                                            break;
                                        case VarType.DInt:
                                            if (unknownTag is int unknownInt)
                                            {
                                                if (Math.Abs(unknownInt - ((int)lastKnownLogValue.value)) > (float)((int)logTag.deadband))
                                                    AddValueToSql(logTag, unknownTag);
                                            }
                                            else
                                                AddValueToSql(logTag, unknownTag);
                                            break;
                                        case VarType.Real:
                                            if (unknownTag is float unknownFloat)
                                            {
                                                if (Math.Abs(unknownFloat - (float)lastKnownLogValue.value) > (float)logTag.deadband)
                                                    AddValueToSql(logTag, unknownTag);
                                            }
                                            else
                                                AddValueToSql(logTag, unknownTag);
                                            break;
                                        case VarType.String:
                                            if (!Equals(unknownTag.ToString(), lastKnownLogValue.value.ToString()))
                                                AddValueToSql(logTag, unknownTag);
                                            break;
                                        case VarType.StringEx:
                                            if (!Equals(unknownTag.ToString(), lastKnownLogValue.value.ToString()))
                                                AddValueToSql(logTag, unknownTag);
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
                                else
                                {
                                    switch (logTag.varType)
                                    {
                                        case VarType.Bit:
                                            if ((bool)unknownTag != (bool)lastKnownLogValue.value)
                                                AddValueToSql(logTag, unknownTag);
                                            break;
                                        case VarType.Byte:
                                            if (unknownTag is byte unknownByte)
                                            {
                                                if (unknownByte > (byte)lastKnownLogValue.value ||
                                                    unknownByte < (byte)lastKnownLogValue.value)
                                                {
                                                    AddValueToSql(logTag, unknownTag);
                                                }
                                            }
                                            else
                                            {
                                                AddValueToSql(logTag, unknownTag);
                                            }
                                            break;
                                        case VarType.Word:
                                            if (unknownTag is ushort unknownUShort)
                                            {
                                                if (unknownUShort > (ushort)lastKnownLogValue.value ||
                                                    unknownUShort < (ushort)lastKnownLogValue.value)
                                                {
                                                    AddValueToSql(logTag, unknownTag);
                                                }
                                            }
                                            else
                                            {
                                                AddValueToSql(logTag, unknownTag);
                                            }
                                            break;
                                        case VarType.DWord:
                                            if (unknownTag is uint unknownUInt)
                                            {
                                                if (unknownUInt > (uint)lastKnownLogValue.value ||
                                                    unknownUInt < (uint)lastKnownLogValue.value)
                                                {
                                                    AddValueToSql(logTag, unknownTag);
                                                }
                                            }
                                            else
                                            {
                                                AddValueToSql(logTag, unknownTag);
                                            }
                                            break;
                                        case VarType.Int:
                                            if (unknownTag is short unknownShort)
                                            {
                                                if (unknownShort > (short)lastKnownLogValue.value ||
                                                    unknownShort < (short)lastKnownLogValue.value)
                                                {
                                                    AddValueToSql(logTag, unknownTag);
                                                }
                                            }
                                            else
                                            {
                                                AddValueToSql(logTag, unknownTag);
                                            }
                                            break;
                                        case VarType.DInt:
                                            if (unknownTag is int unknownInt)
                                            {
                                                if (unknownInt > (int)lastKnownLogValue.value ||
                                                    unknownInt < (int)lastKnownLogValue.value)
                                                {
                                                    AddValueToSql(logTag, unknownTag);
                                                }
                                            }
                                            else
                                            {
                                                AddValueToSql(logTag, unknownTag);
                                            }
                                            break;
                                        case VarType.Real:
                                            if (unknownTag is float unknownFloat)
                                            {
                                                if (unknownFloat > (float)lastKnownLogValue.value ||
                                                    unknownFloat < (float)lastKnownLogValue.value)
                                                {
                                                    AddValueToSql(logTag, unknownTag);
                                                }
                                            }
                                            else
                                            {
                                                AddValueToSql(logTag, unknownTag);
                                            }
                                            break;
                                        case VarType.String:
                                            if (!Equals(unknownTag.ToString(), lastKnownLogValue.value.ToString()))
                                            {
                                                AddValueToSql(logTag, unknownTag);
                                            }
                                            break;
                                        case VarType.StringEx:
                                            if (!Equals(unknownTag.ToString(), lastKnownLogValue.value.ToString()))
                                            {
                                                AddValueToSql(logTag, unknownTag);
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
                            else if (logTag.logType == LogType.Cyclic)
                            {
                                AddValueToSql(logTag, unknownTag);
                            }
                            else if (logTag.logType == LogType.TimeOfDay)
                            {
                                if (logTag.timeOfDay.Seconds != 0)
                                {
                                    if (DateTime.Now.Hour == logTag.timeOfDay.Hours &&
                                        DateTime.Now.Minute == logTag.timeOfDay.Minutes &&
                                        DateTime.Now.Second == logTag.timeOfDay.Seconds)
                                    {
                                        var allOccurencesOfTagInList = lastLogValue.Find(n => n.name == logTag.name && n.logDate.Day == DateTime.Now.Day);
                                        if (allOccurencesOfTagInList == null)
                                        {
                                            AddValueToSql(logTag, unknownTag);
                                        }
                                    }
                                }
                                else if (DateTime.Now.Hour == logTag.timeOfDay.Hours &&
                                        DateTime.Now.Minute == logTag.timeOfDay.Minutes)
                                {
                                    var allOccurencesOfTagInList = lastLogValue.Find(n => n.name == logTag.name && n.logDate.Day == DateTime.Now.Day);
                                    if (allOccurencesOfTagInList == null)
                                    {
                                        AddValueToSql(logTag, unknownTag);
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
                                                    AddValueToSql(subbedTag, subbedLog);
                                                }
                                                break;
                                            case BooleanTrigger.WhileTrue:
                                                if ((bool)unknownTag)
                                                {
                                                    var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                    AddValueToSql(subbedTag, subbedLog);
                                                }
                                                break;
                                            case BooleanTrigger.OnFalse:
                                                if ((bool)lastValue.value && !(bool)unknownTag)
                                                {
                                                    var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                    AddValueToSql(subbedTag, subbedLog);
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
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.Word:
                                                        if ((UInt16)unknownTag < subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.DWord:
                                                        if ((UInt32)unknownTag < subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.Int:
                                                        if ((short)unknownTag < subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.DInt:
                                                        if ((Int32)unknownTag < subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.Real:
                                                        if ((float)unknownTag < subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
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
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.Word:
                                                        if ((UInt16)unknownTag > subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.DWord:
                                                        if ((UInt32)unknownTag > subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.Int:
                                                        if ((short)unknownTag > subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.DInt:
                                                        if ((Int32)unknownTag > subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.Real:
                                                        if ((float)unknownTag > subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
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
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.Word:
                                                        if ((UInt16)unknownTag == subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.DWord:
                                                        if ((UInt32)unknownTag == subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.Int:
                                                        if ((short)unknownTag == subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.DInt:
                                                        if ((Int32)unknownTag == subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.Real:
                                                        if ((float)unknownTag == subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
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
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.Word:
                                                        if ((UInt16)unknownTag <= subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.DWord:
                                                        if ((UInt32)unknownTag <= subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.Int:
                                                        if ((short)unknownTag <= subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.DInt:
                                                        if ((Int32)unknownTag <= subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.Real:
                                                        if ((float)unknownTag <= subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
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
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.Word:
                                                        if ((UInt16)unknownTag >= subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.DWord:
                                                        if ((UInt32)unknownTag >= subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.Int:
                                                        if ((short)unknownTag >= subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.DInt:
                                                        if ((Int32)unknownTag >= subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.Real:
                                                        if ((float)unknownTag >= subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
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
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.Word:
                                                        if ((UInt16)unknownTag != subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.DWord:
                                                        if ((UInt32)unknownTag != subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.Int:
                                                        if ((short)unknownTag != subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.DInt:
                                                        if ((Int32)unknownTag != subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
                                                        }
                                                        break;
                                                    case VarType.Real:
                                                        if ((float)unknownTag != subbedTag.analogValue)
                                                        {
                                                            var subbedLog = MyPlc.Read(subbedTag.dataType, subbedTag.blockNr, subbedTag.startByte, subbedTag.varType, subbedTag.nrOfElements, subbedTag.bitAddress);
                                                            AddValueToSql(subbedTag, subbedLog);
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
                        logTag.NrFailedReadAttempts++;
                        logTag.LastErrorMessage = ex.Message;
                    }
                }
            }
        }

        private void AddValueToSql(TagDefinitions logTag, object unknownTag)
        {
            logTag.TimesLogged++;
            lastLogValue.Add(new LastValue()
            {
                name = logTag.name,
                value = unknownTag,
                logDate = logTag.LastLogTime,
            });

            var allOccurencesOfTagInList = lastLogValue.FindAll(n => n.name == logTag.name).OrderBy(dat => dat.logDate).ToList();
            var nrOfItemsInLastLog = lastLogValue.FindAll(n => n.name == logTag.name);


            // Garantera att det bara finns ett värde bakåt
            if (nrOfItemsInLastLog.Count > 2)
            {
                var removeDate = allOccurencesOfTagInList[nrOfItemsInLastLog.Count - 3].logDate;
                lastLogValue.RemoveAll(i => i.name == logTag.name && i.logDate <= removeDate);
            }

            var logValue = new TagDefinitions()
            {
                active = logTag.active,
                bitAddress = logTag.bitAddress,
                blockNr = logTag.blockNr,
                nrOfElements = logTag.nrOfElements,
                dataType = logTag.dataType,
                id = logTag.id,
                LastLogTime = logTag.LastLogTime,
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
