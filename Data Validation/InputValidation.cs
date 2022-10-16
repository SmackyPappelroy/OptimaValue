using S7.Net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace OptimaValue
{
    public static class InputValidation
    {

        public static bool IsValidIpAddress(this string ip)
        {
            var ipChars = ip.ToCharArray();
            return IPAddress.TryParse(ipChars, out var _);
        }

        public static bool IsValidOpcConnectionString(this string connectionString)
        {
            var normalizedString = connectionString.ToLower();
            return normalizedString.StartsWith("opc.tcp://")
                || normalizedString.StartsWith("opc.http://");
        }


    }
}
