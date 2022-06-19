using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue.Config
{
    public static class SqlMethods
    {
        public static string Server { get; private set; } = "";
        public static string ConnectionString => ($"Server={Server};Database={Settings.Databas};User Id={Settings.User};Password={Settings.Password};;TrustServerCertificate=true; ");

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

            if (string.IsNullOrEmpty(Settings.Databas) || string.IsNullOrEmpty(Settings.User) || string.IsNullOrEmpty(Settings.Password))
            {
                Settings.Databas = "MCValuelog";
                Settings.User = "sa";
                Settings.Password = "sa";
            }


            return ConnectionString;
        }

        public static async Task<bool> TestSqlConnectionAsync()
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
