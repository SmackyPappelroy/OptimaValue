using S7.Net;
using System;
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
