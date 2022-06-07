using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OptimaValue.Config
{
    public class SettingsClass
    {
        public string? Server { get; set; }
        public string? Database { get; set; }
        public string? User { get; set; }
        public string? Password { get; set; }
    }

    public static class SqlSettings
    {
        public static object lockObject = new();

        public static void Load()
        {
            LoadValuesFromFile();
        }

        private static readonly string filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\OptimaValue";
        private static readonly string fileName = "SqlSettings.json";
        private static string fullPathName => filePath + "\\" + fileName;

        public static string ConnectionString => ($"Server={@SqlSettings.Server};Database={SqlSettings.Databas};User Id={SqlSettings.User};Password={SqlSettings.Password}; ");

        private static string? server;
        public static string Server
        {
            get
            {
                return server;
            }
            set
            {
                server = value;
                Save();
            }
        }

        private static string? databas;
        public static string? Databas
        {
            get
            {
                return databas;
            }
            set
            {
                databas = value;
                Save();
            }
        }
        private static string? user;
        public static string? User
        {
            get
            {
                return user;
            }
            set
            {
                user = value;
                Save();
            }
        }
        private static string? password;
        public static string? Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                Save();
            }
        }

        private static void Save()
        {
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            SettingsClass settings = new()
            {
                Server = Server,
                Database = Databas,
                User = User,
                Password = Password
            };

            File.WriteAllText(fullPathName, JsonSerializer.Serialize(settings));
        }

        private static void LoadValuesFromFile()
        {
            lock (lockObject)
            {
                if (File.Exists(fullPathName))
                {

                    var json = File.ReadAllText(fullPathName);
                    SettingsClass settings = JsonSerializer.Deserialize<SettingsClass>(json);
                    Server = settings.Server;
                    Databas = settings.Database;
                    User = settings.User;
                    Password = settings.Password;
                }
                else
                {
                    SettingsClass settings = new()
                    {
                        Server = string.Empty,
                        Database = string.Empty,
                        User = string.Empty,
                        Password = string.Empty
                    };
                    File.WriteAllText(fullPathName, JsonSerializer.Serialize(settings));
                }
            }

        }
    }
}
