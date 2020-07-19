using S7.Net;
using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace OptimaValue
{
    /// <summary>
    /// Created by Hans-Martin Nilsson<para></para>
    /// 2020-07-01
    /// </summary>
    public static class OptimaExtensions
    {

        /// <summary>
        /// Returns True if pingable<para></para>Timeout of 2 seconds <para></para>
        /// Throws a <see cref="NullReferenceException"/> if <paramref name="plc"/> is null<para></para>
        /// Throws a <see cref="PingException"/> if not valid IP address format
        /// </summary>
        /// <returns></returns>
        public static bool Ping(this Plc plc)
        {
            if (plc == null)
                throw new NullReferenceException("Plc kan ej vara null");
            if (!plc.IP.CheckValidIpAddress())
                throw new PingException("Inte giltig IP-adress");
            Ping pinger = null;
            var Pingable = false;
            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(plc.IP, 2000); // Timeout-tid 2 sekunder
                Pingable = reply.Status == IPStatus.Success;

                if (!Pingable)
                {
                    return false;
                }
            }
            catch (PingException)
            {
                if (!Pingable)
                    return false;
            }
            finally
            {
                if (pinger != null)
                    pinger.Dispose();
            }
            return Pingable;
        }

        /// <summary>
        /// Returns a <see cref="string"/> from an S7-string<para></para>
        /// Throws a <see cref="NullReferenceException"/> if <paramref name="plc"/> is null
        /// </summary>
        /// <param name="dataType">The <see cref="DataType"/> of the <see cref="Plc"/></param>
        /// <param name="db">The DB-number</param>
        /// <param name="startByteAdr">The start byte address</param>
        /// <param name="count">How many bytes forward</param>
        /// <returns>Null if unable to convert</returns>
        public static string ReadS7stringToString(this Plc plc, DataType dataType, int db, int startByteAdr, int count)
        {
            if (plc == null)
                throw new NullReferenceException("Plc får ej va null");

            byte[] byteArray;

            try
            {
                byteArray = plc.ReadBytes(dataType, db, startByteAdr, count);
            }
            catch { throw new PlcException(ErrorCode.ReadData, "Lyckas ej läsa strängen"); }

            if (byteArray.Length <= 3)
                return null;

            byte currentLength = (byte)byteArray[1];

            var convText = new byte[currentLength];
            for (int i = 2; i < (int)currentLength + 2; i++)
            {
                convText[i - 2] = byteArray[i];
            }

            string resultText;
            try
            {
                resultText = System.Text.Encoding.ASCII.GetString(convText);
            }
            catch { throw new PlcException(ErrorCode.ReadData, "Lyckas ej omvandla strängen"); }

            return resultText;
        }

        /// <summary>
        /// Writes a string to a S7 compatible string <para></para>ExampleString[<paramref name="s7StringLength"/>]<para></para>
        /// Throws a <see cref="NullReferenceException"/> if <paramref name="plc"/> is null
        /// </summary>
        /// <param name="plc"></param>
        /// <param name="dataType"></param>
        /// <param name="db"></param>
        /// <param name="startByteAdr"></param>
        /// <param name="s7StringLength"></param>
        /// <param name="writeString"></param>
        /// <returns></returns>
        public static bool WriteStringToPlc(this Plc plc, DataType dataType, int db, int startByteAdr, int s7StringLength, string writeString)
        {
            if (plc == null)
                throw new NullReferenceException("Plc får ej va null");

            string theString;
            int stringLength;
            if (writeString.Length > s7StringLength)
            {
                theString = writeString.Substring(0, s7StringLength);
                stringLength = s7StringLength;
            }
            else
            {
                theString = writeString;
                stringLength = writeString.Length;
            }

            byte[] byteArray = new byte[stringLength + 2];

            byteArray[0] = (byte)stringLength;
            byteArray[1] = byteArray[0];


            for (int i = 2; i < stringLength + 1; i++)
                byteArray[i] = Convert.ToByte(theString[i - 2]);

            byteArray[byteArray.Length - 1] = Convert.ToByte(theString[theString.Length - 1]);

            try
            {
                plc.WriteBytes(dataType, db, startByteAdr, byteArray);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Reads a DTL from <see cref="Plc"/><para></para>
        /// Returns <see cref="DateTime"/><para></para>
        /// Throws an <see cref="Exception"/> if failed
        /// </summary>
        /// <param name="dtl">The <see cref="DTL"/> class</param>
        /// <param name="dbNr">The DB number</param>
        /// <param name="startByteAddr">The start of the byte address</param>
        /// <returns></returns>
        public static DateTime DTLtoDateTime(this Plc plc, DTL dtl, int dbNr, int startByteAddr)
        {
            if (plc == null)
                throw new NullReferenceException("Plc får ej va null");
            try
            {
                plc.ReadClass(dtl, dbNr, startByteAddr);
            }
            catch (Exception)
            {
                throw new Exception("Misslyckades att läsa från plc, när DTL lästes från PLC");
            }


            var temp = dtl.Year.ToString();
            temp += "-" + (dtl.Month.ToString().Count() == 1 ? ("0" + dtl.Month.ToString()) : dtl.Month.ToString());
            temp += "-" + (dtl.Day.ToString().Count() == 1 ? ("0" + dtl.Day.ToString()) : dtl.Day.ToString());
            temp += " " + (dtl.Hour.ToString().Count() == 1 ? ("0" + dtl.Hour.ToString()) : dtl.Hour.ToString());
            temp += ":" + (dtl.Minute.ToString().Count() == 1 ? ("0" + dtl.Minute.ToString()) : dtl.Minute.ToString());
            temp += ":" + (dtl.Second.ToString().Count() == 1 ? ("0" + dtl.Second.ToString()) : dtl.Second.ToString());
            var temp2 = dtl.NanoSecond / 1000000;
            if (temp2.ToString().Length > 2)
                temp += "." + (temp2.ToString()).Substring(0, 3);
            else
                temp += "." + temp2.ToString();
            return Convert.ToDateTime(temp);
        }

        /// <summary>
        /// Writes a DTL from a <see cref="DateTime"/> to Plc<para></para>
        /// Returns <see cref="DTL"/><para></para>
        /// Throws an <see cref="Exception"/> if failed
        /// </summary>
        /// <param name="dt">The <see cref="DateTime"/> to be converted</param>
        /// <param name="dbNr">The DB <see cref="Int32"/> number</param>
        /// <param name="startByteAddress">The start byte of the <see cref="DTL"/></param>
        /// <returns></returns>
        public static DTL DateTimeToDTL(this Plc plc, DateTime dt, int dbNr, int startByteAddress)
        {
            if (plc == null)
                throw new NullReferenceException("Plc får ej va null");

            var dtl = new DTL()
            {
                Year = (short)dt.Year,
                Month = (byte)dt.Month,
                Day = (byte)dt.Day,
                Hour = (byte)dt.Hour,
                Minute = (byte)dt.Minute,
                Second = (byte)dt.Second,
                NanoSecond = dt.Millisecond * 1000000,
                WeekDay = (byte)(dt.DayOfWeek + 1),
            };

            try
            {
                plc.WriteClass(dtl, dbNr, startByteAddress);
                return dtl;
            }
            catch (Exception ex)
            {
                throw new Exception($"Misslyckades skriva DTL till Plc. {ex.Message}");
            }

        }

        /// <summary>
        /// Returns True if valid <see cref="CpuType"/>
        /// </summary>
        /// <param name="cpuType"></param>
        /// <returns></returns>
        public static bool CheckValidCpuType(this string cpuType)
        {
            if (Enum.IsDefined(typeof(CpuType), cpuType))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns True if valid IP address
        /// </summary>
        /// <param name="ipString"></param>
        /// <returns></returns>
        public static bool CheckValidIpAddress(this string ipString)
        {
            //  Split string by ".", check that array length is 4
            string[] arrOctets = ipString.Split('.');
            if (arrOctets.Length != 4)
                return false;

            //Check each substring checking that parses to byte
            foreach (string strOctet in arrOctets)
                if (!byte.TryParse(strOctet, out _))
                    return false;

            return true;
        }

        /// <summary>
        /// Returns true if able to Ping<para></para>Timeout of 2 seconds<para></para>
        /// Throws a <see cref="PingException"/> if not valid IP address format
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static bool Ping(this string ipAddress)
        {

            if (!ipAddress.CheckValidIpAddress())
                throw new PingException("Inte giltig IP-adress");
            Ping pinger = null;
            var Pingable = false;
            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(ipAddress, 2000); // Timeout-tid 2 sekunder
                Pingable = reply.Status == IPStatus.Success;

                if (!Pingable)
                {
                    return false;
                }
            }
            catch (PingException)
            {
                if (!Pingable)
                    return false;
            }
            finally
            {
                if (pinger != null)
                    pinger.Dispose();
            }
            return Pingable;
        }

        public static void SendPlcStatusMessage(this ExtendedPlc plc, string message, Status status)
        {
            PlcStatusEvent.RaiseMessage(message, plc.PlcName, status);
        }
        public static void SendPlcOnlineMessage(this ExtendedPlc plc, ConnectionStatus connectionStatus, string elapsedTime = "")
        {
            OnlineStatusEvent.RaiseMessage(connectionStatus, plc.PlcName, elapsedTime);
        }

    }
}
