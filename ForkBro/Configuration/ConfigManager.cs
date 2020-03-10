using System;
using System.IO;
using System.Text.Json;
using Newtonsoft.Json;

namespace ForkBro.Configuration
{
    internal class ConfigManager
    {
        Config config;
        static ConfigManager manager;
        string path;

        public ConfigManager() { 
            config = Config.Empty;
            manager = this;
            path = System.Reflection.Assembly.GetEntryAssembly().Location;
            path = path.Substring(0, path.LastIndexOf('.')) + ".json";
        }

        public ConfigManager GetInstance() => manager;
        public ref Config CurrConfig => ref config;
        public Config CreateConfig() => config = Config.Empty;
        public bool FileExists { get => File.Exists(path); }

        public bool ReadConfig()
        {
            bool result = false;
            try
            {
                string configFile = File.ReadAllText(path);
                config = JsonConvert.DeserializeObject<Config>(configFile);
                result = true;
            }
            catch { }
            return result;
        }
        public bool WriteConfig()
        {
            bool result = false;
                try
                {
                    var jsonString = JsonConvert.SerializeObject(config);
                    File.WriteAllText(path, jsonString);
                    result = true;
                }
                catch { }
            return result;
        }


    }
}