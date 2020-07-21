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
        private ExtendedPlc MyPlc = null;
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
            return;
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
                    StatusEvent.RaiseMessage($"Får ej kontakt med {MyPlc.PlcName}", Status.Error);
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
                StatusEvent.RaiseMessage($"Lyckas ej ansluta till {MyPlc.PlcName}\r\n{ex.Message}", Status.Error);
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
                        if (DateTime.Now - MyPlc.LastReconnect > TimeSpan.FromSeconds(15))
                        {
                            try
                            {
                                MyPlc.ReconnectRetries++;

                                MyPlc.Close();
                                MyPlc.Open();
                                if (MyPlc.IsConnected)
                                {
                                    MyPlc.SendPlcStatusMessage($"Lyckades återansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}\r\nFörsök nummer: {MyPlc.ReconnectRetries}", Status.Ok);
                                    StatusEvent.RaiseMessage($"Lyckades återansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}\r\nFörsök nummer: {MyPlc.ReconnectRetries}", Status.Ok);
                                    MyPlc.ConnectionStatus = ConnectionStatus.Connected;
                                    MyPlc.ReconnectRetries = 0;
                                }
                                else
                                {
                                    MyPlc.SendPlcStatusMessage($"Misslyckades att återansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}\r\nFörsök nummer: {MyPlc.ReconnectRetries}", Status.Error);
                                    StatusEvent.RaiseMessage($"Misslyckades att återansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}\r\nFörsök nummer: {MyPlc.ReconnectRetries}", Status.Error);
                                }
                                MyPlc.LastReconnect = DateTime.Now;
                            }
                            catch
                            {
                                MyPlc.SendPlcStatusMessage($"Misslyckades att ansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}\r\nFörsök nummer: {MyPlc.ReconnectRetries}", Status.Error);
                                StatusEvent.RaiseMessage($"Misslyckades att ansluta till {MyPlc.PlcName}\r\n{MyPlc.IP}\r\nFörsök nummer: {MyPlc.ReconnectRetries}", Status.Error);
                                MyPlc.LastReconnect = DateTime.Now;
                            }
                        }


                    }
                    else
                    {
                        MyPlc.SendPlcStatusMessage($"Max reconnect försök för {MyPlc.PlcName}\r\nAvbryter!", Status.Error);
                        StatusEvent.RaiseMessage($"Max reconnect försök för {MyPlc.PlcName}\r\nAvbryter!", Status.Error);
                        if (PlcPicker.NrActiveEnabledPlc == 1)
                            Master.StopLog(true);
                        else
                            AbortLogThread("");
                    }
                }

                if (MyPlc.IsConnected)
                    foreach (TagDefinitions logValue in TagsToLog.AllLogValues)
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
                StatusEvent.RaiseMessage($"Kommunikationen upprättades ej till {MyPlc.PlcName}\n\r{message}", Status.Error);
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
                    var shouldLog = false;
                    object unknownTag = new object();
                    try
                    {
                        if (MyPlc.ConnectionStatus == ConnectionStatus.Connected)
                        {
                            if (logTag.varType == VarType.String || logTag.varType == VarType.StringEx)
                            {
                                unknownTag = MyPlc.ReadS7stringToString(logTag.dataType, logTag.blockNr, logTag.startByte, logTag.nrOfElements);
                            }
                            else
                                unknownTag = MyPlc.Read(logTag.dataType, logTag.blockNr, logTag.startByte, logTag.varType, logTag.nrOfElements, logTag.bitAddress);

                            logTag.LastLogTime = DateTime.Now;
                            logTag.NrSuccededReadAttempts++;


                            if (logTag.logType == LogType.Delta)
                            {
                                var lastKnowLogValue = lastLogValue.FindLast(l => l.name == logTag.name);
                                if (lastKnowLogValue == null)
                                {
                                    shouldLog = true;
                                    goto store;
                                }

                                if (logTag.deadband > 0)
                                {

                                    switch (logTag.varType)
                                    {
                                        case VarType.Bit:
                                            if ((bool)unknownTag != (bool)lastKnowLogValue.value)
                                                shouldLog = true;
                                            break;
                                        case VarType.Byte:
                                            if (unknownTag is byte unknownConverted)
                                            {
                                                if (Math.Abs(unknownConverted - ((byte)lastKnowLogValue.value)) > (float)((byte)logTag.deadband))
                                                    shouldLog = true;
                                            }
                                            else
                                                shouldLog = true;
                                            break;
                                        case VarType.Word:
                                            if (unknownTag is ushort unknownUShort)
                                            {
                                                if (Math.Abs(unknownUShort - ((ushort)lastKnowLogValue.value)) > (float)((ushort)logTag.deadband))
                                                    shouldLog = true;
                                            }
                                            else
                                                shouldLog = true;
                                            break;
                                        case VarType.DWord:
                                            if (unknownTag is uint uknonwnUint)
                                            {
                                                if (Math.Abs(uknonwnUint - ((uint)lastKnowLogValue.value)) > (float)((uint)logTag.deadband))
                                                    shouldLog = true;
                                            }
                                            else
                                                shouldLog = true;
                                            break;
                                        case VarType.Int:
                                            if (unknownTag is short unknownShort)
                                            {
                                                if (Math.Abs(unknownShort - ((short)lastKnowLogValue.value)) > (float)((short)logTag.deadband))
                                                    shouldLog = true;
                                            }
                                            else
                                                shouldLog = true;
                                            break;
                                        case VarType.DInt:
                                            if (unknownTag is int unknownInt)
                                            {
                                                if (Math.Abs(unknownInt - ((int)lastKnowLogValue.value)) > (float)((int)logTag.deadband))
                                                    shouldLog = true;
                                            }
                                            else
                                                shouldLog = true;
                                            break;
                                        case VarType.Real:
                                            if (unknownTag is float unknownFloat)
                                            {
                                                if (Math.Abs(unknownFloat - (float)lastKnowLogValue.value) > (float)logTag.deadband)
                                                    shouldLog = true;
                                            }
                                            else
                                                shouldLog = true;
                                            break;
                                        case VarType.String:
                                            if (!Equals(unknownTag.ToString(), lastKnowLogValue.value.ToString()))
                                                shouldLog = true;
                                            break;
                                        case VarType.StringEx:
                                            if (!Equals(unknownTag.ToString(), lastKnowLogValue.value.ToString()))
                                                shouldLog = true;
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
                                            if ((bool)unknownTag != (bool)lastKnowLogValue.value)
                                                shouldLog = true;
                                            break;
                                        case VarType.Byte:
                                            if (unknownTag is byte unknownByte)
                                            {
                                                if (unknownByte > (byte)lastKnowLogValue.value ||
                                                    unknownByte < (byte)lastKnowLogValue.value)
                                                    shouldLog = true;
                                            }
                                            else
                                                shouldLog = true;
                                            break;
                                        case VarType.Word:
                                            if (unknownTag is ushort unknownUShort)
                                            {
                                                if (unknownUShort > (ushort)lastKnowLogValue.value ||
                                                    unknownUShort < (ushort)lastKnowLogValue.value)
                                                    shouldLog = true;
                                            }
                                            else
                                                shouldLog = true;
                                            break;
                                        case VarType.DWord:
                                            if (unknownTag is uint unknownUInt)
                                            {
                                                if (unknownUInt > (uint)lastKnowLogValue.value ||
                                                    unknownUInt < (uint)lastKnowLogValue.value)
                                                    shouldLog = true;
                                            }
                                            else
                                                shouldLog = true;
                                            break;
                                        case VarType.Int:
                                            if (unknownTag is short unknownShort)
                                            {
                                                if (unknownShort > (short)lastKnowLogValue.value ||
                                                    unknownShort < (short)lastKnowLogValue.value)
                                                    shouldLog = true;
                                            }
                                            else
                                                shouldLog = true;
                                            break;
                                        case VarType.DInt:
                                            if (unknownTag is int unknownInt)
                                            {
                                                if (unknownInt > (int)lastKnowLogValue.value ||
                                                    unknownInt < (int)lastKnowLogValue.value)
                                                    shouldLog = true;
                                            }
                                            else
                                                shouldLog = true;
                                            break;
                                        case VarType.Real:
                                            if (unknownTag is float unknownFloat)
                                            {
                                                if (unknownFloat > (float)lastKnowLogValue.value ||
                                                    unknownFloat < (float)lastKnowLogValue.value)
                                                    shouldLog = true;
                                            }
                                            else
                                                shouldLog = true;
                                            break;
                                        case VarType.String:
                                            if (!Equals(unknownTag.ToString(), lastKnowLogValue.value.ToString()))
                                                shouldLog = true;
                                            break;
                                        case VarType.StringEx:
                                            if (!Equals(unknownTag.ToString(), lastKnowLogValue.value.ToString()))
                                                shouldLog = true;
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
                                shouldLog = true;
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
                                            shouldLog = true;
                                        }
                                    }
                                }
                                else if (DateTime.Now.Hour == logTag.timeOfDay.Hours &&
                                        DateTime.Now.Minute == logTag.timeOfDay.Minutes)
                                {
                                    var allOccurencesOfTagInList = lastLogValue.Find(n => n.name == logTag.name && n.logDate.Day == DateTime.Now.Day);
                                    if (allOccurencesOfTagInList == null)
                                    {
                                        shouldLog = true;
                                    }
                                }
                            }


                        }
                        else
                        {
                            if (MyPlc.UnableToPing)
                            {
                                MyPlc.SendPlcStatusMessage($"Lyckas ej pinga {MyPlc.PlcName}\r\n{MyPlc.IP}", Status.Error);
                            }
                            return;
                        }

                    }
                    catch (PlcException ex)
                    {
                        MyPlc.SendPlcStatusMessage($"Misslyckades att läsa {logTag.name} från {MyPlc.PlcName}\r\n{ex.Message}\r\n{ex.TargetSite}\r\n{ex.Source}", Status.Error);
                        logTag.NrFailedReadAttempts++;
                        logTag.LastErrorMessage = ex.Message;
                        return;
                    }

                    store:
                    if (shouldLog)
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
        }




    }
}
