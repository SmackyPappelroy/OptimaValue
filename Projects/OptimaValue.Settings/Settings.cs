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
        public string? OptimaValueFilePath { get; set; }
        public string? OptimaValueWpfFilePath { get; set; }
        public bool IsTrendRunning { get; set; }
        public bool IsOptimaValueRunning { get; set; }
        public bool UseMqtt { get; set; }

    }

    public static class Settings
    {
        public static object lockObject = new();

        public static void Load()
        {
            LoadValuesFromFile();
        }

        private static readonly string filePath = @"c:\OptimaSettings\OptimaValue";
        private static readonly string fileName = "SqlSettings.json";
        public static string FullPathName => filePath + "\\" + fileName;

        public static string ConnectionString => ($"Server={Settings.Server};Database={Settings.Databas};User Id={Settings.User};Password={Settings.Password}; ");

        private static bool isOptimaValueRunning;
        public static bool IsOptimaValueRunning
        {
            get
            {
                return isOptimaValueRunning;
            }
            set
            {
                isOptimaValueRunning = value;
                Save();
            }
        }

        private static bool isTrendRunning;
        public static bool IsTrendRunning
        {
            get
            {
                return isTrendRunning;
            }
            set
            {
                isTrendRunning = value;
                Save();
            }
        }

        private static string? optimaValueFilePath;
        public static string OptimaValueFilePath
        {
            get
            {
                return optimaValueFilePath;
            }
            set
            {
                optimaValueFilePath = value;
                Save();
            }
        }

        private static string? optimaValueWpfFilePath;
        public static string OptimaValueWpfFilePath
        {
            get
            {
                return optimaValueWpfFilePath;
            }
            set
            {
                optimaValueWpfFilePath = value;
                Save();
            }
        }

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

        private static bool useMqtt;
        public static bool UseMqtt
        {
            get
            {
                return useMqtt;
            }
            set
            {
                useMqtt = value;
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
                Password = Password,
                OptimaValueFilePath = OptimaValueFilePath,
                OptimaValueWpfFilePath = OptimaValueWpfFilePath,
                IsTrendRunning = IsTrendRunning,
                IsOptimaValueRunning = IsOptimaValueRunning,
                UseMqtt = UseMqtt
            };

            File.WriteAllText(FullPathName, JsonSerializer.Serialize(settings));
        }

        private static void LoadValuesFromFile()
        {
            lock (lockObject)
            {
                if (File.Exists(FullPathName))
                {
                    var json = File.ReadAllText(FullPathName);
                    SettingsClass settings = JsonSerializer.Deserialize<SettingsClass>(json);
                    Server = settings.Server;
                    Databas = settings.Database;
                    User = settings.User;
                    Password = settings.Password;
                    OptimaValueFilePath = settings.OptimaValueFilePath;
                    OptimaValueWpfFilePath = settings.OptimaValueWpfFilePath;
                    IsTrendRunning = settings.IsTrendRunning;
                    IsOptimaValueRunning = settings.IsOptimaValueRunning;
                    UseMqtt = settings.UseMqtt;
                }
                else
                {
                    SettingsClass settings = new()
                    {
                        Server = string.Empty,
                        Database = string.Empty,
                        User = string.Empty,
                        Password = string.Empty,
                        OptimaValueFilePath = string.Empty,
                        OptimaValueWpfFilePath = string.Empty,
                        IsTrendRunning = false,
                        IsOptimaValueRunning = false,
                        UseMqtt = false
                    };
                    Directory.CreateDirectory(filePath);
                    File.WriteAllText(FullPathName, JsonSerializer.Serialize(settings));
                }
            }

        }
    }
}
