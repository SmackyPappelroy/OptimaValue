using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue.Config
{
    public static class SqlMethods
    {
        public static string Server { get; private set; } = "";
        public static string ConnectionString => ($"Server={Server};Database={SqlSettings.Default.Databas};User Id={SqlSettings.Default.User};Password={SqlSettings.Default.Password};;TrustServerCertificate=true; ");

        public static string CreateConnectionString()
        {
            if (OperatingSystem.IsWindows())
            {
                List<string> list = new();
                string ServerName = Environment.MachineName;
                RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
                using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
                {
                    RegistryKey instanceKey = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL", false);
                    if (instanceKey != null)
                    {
                        foreach (var instanceName in instanceKey.GetValueNames())
                        {
                            list.Add(ServerName + "\\" + instanceName);
                        }
                    }
                }
                if (list.Count > 0)
                {
                    Server = list[0];
                }
            }

            if (string.IsNullOrEmpty(SqlSettings.Default.Databas) || string.IsNullOrEmpty(SqlSettings.Default.User) || string.IsNullOrEmpty(SqlSettings.Default.Password))
            {
                SqlSettings.Default.Databas = "MCValulog";
                SqlSettings.Default.User = "sa";
                SqlSettings.Default.Password = "sa";
                SqlSettings.Default.Save();
            }


            return ConnectionString;
        }
    }
}
