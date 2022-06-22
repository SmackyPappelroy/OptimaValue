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
        public static string ConnectionString => ($"Server={Settings.Server};Database={Settings.Databas};User Id={Settings.User};Password={Settings.Password};;TrustServerCertificate=true; ");

        public static string CreateConnectionString()
        {

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
