using S7.Net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace OptimaValue
{
    public static class InputValidation
    {
        public static (bool result, string message) ValidateAll(DataGridView grid)
        {
            if (grid == null)
                return (false, "Ingen kontakt med databas");
            if (grid.Rows.Count == 0 || grid.Columns.Count == 0)
                return (false, "Tabell tom, eller ingen kontakt med Databas");
            var result = true;
            var message = "";
            List<string> listan = new List<string>();

            for (int rowIndex = 0; rowIndex < grid.Rows.Count - 1; rowIndex++)
            {
                var row = grid.Rows[rowIndex];
                var ipString = row.Cells["ipAddress"].Value.ToString();
                listan.Add(ipString);
                if (!CheckIPValid(ipString))
                {
                    result = false;
                    if (!message.Contains("IP-"))
                    {
                        if (message.Length > 0)
                            message += " och ";
                        message += "Fel format på IP-adress";
                    }
                }
                var cpuType = row.Cells["cpuType"].Value.ToString();
                if (!CheckCpuValid(cpuType))
                {
                    result = false;
                    if (!message.Contains("CPU"))
                    {
                        if (message.Length > 0)
                            message += " och ";
                        message += "Fel format på CPU-typ";
                    }
                }
                var rack = row.Cells["Rack"].Value.ToString();
                var slot = row.Cells["Slot"].Value.ToString();
                var active = row.Cells["Active"].Value.ToString();

                if (!CheckShortValid(rack).result)
                {
                    result = false;
                    if (!message.Contains("Rack"))
                    {
                        if (message.Length > 0)
                            message += " och ";
                        message += "Fel format på Rack";
                    }
                }
                if (!CheckShortValid(slot).result)
                {
                    result = false;
                    if (!message.Contains("Slot"))
                    {
                        if (message.Length > 0)
                            message += " och ";
                        message += "Fel format på Slot";
                    }
                }
            }
            var query = listan.GroupBy(x => x)
               .Where(g => g.Count() > 1)
               .Select(y => y.Key)
               .ToList();
            //    if (query.Count > 0)
            //        return (false, "Det finns dubletter av IP-adress");

            return (result, message);
        }

        public static bool CheckIPValid(String strIP)
        {
            //  Split string by ".", check that array length is 4
            string[] arrOctets = strIP.Split('.');
            if (arrOctets.Length != 4)
                return false;

            //Check each substring checking that parses to byte
            byte obyte;
            foreach (string strOctet in arrOctets)
                if (!byte.TryParse(strOctet, out obyte))
                    return false;

            return true;
        }

        public static bool CheckCpuValid(string _cpu)
        {
            if (Enum.IsDefined(typeof(CpuType), _cpu))
                return true;
            else
                return false;
        }

        public static (bool result, string message) CheckShortValid(string input)
        {
            if (short.TryParse(input, out short result))
            {
                if (result < 0)
                    return (false, "Måste vara större än 0");
                return (true, string.Empty);
            }
            return (false, "Inte ett numeriskt värde");
        }

    }
}
