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
            return IPAddress.TryParse(ip, out _);
        }

        public static bool IsValidOpcConnectionString(this string connectionString)
        {
            return connectionString.StartsWith("opc.tcp://", StringComparison.OrdinalIgnoreCase)
                || connectionString.StartsWith("opc.http://", StringComparison.OrdinalIgnoreCase);
        }



    }
}
