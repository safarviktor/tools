using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace ConfigMonitor
{
    public class MainWindowVM : BindableBase
    {
        private Dictionary<string, string> _configs;
        public Dictionary<string, string> Configs => _configs ?? (_configs = GetConfigs());

        private Dictionary<string, string> GetConfigs()
        {
            var result = new Dictionary<string, string>();
            foreach (ConnectionStringSettings conn in ConfigurationManager.ConnectionStrings)
            {
                var filePath = conn.ConnectionString;
                if (filePath.StartsWith("C:"))
                {
                    result.Add(conn.ConnectionString, GetKvpFromConnectionString(conn.ConnectionString));
                }
            }

            return result;
        }

        private string GetKvpFromConnectionString(string filePath)
        {
            if (filePath.EndsWith("json"))
            {
                return GetFromJson(filePath);
            }

            ExeConfigurationFileMap configMap = new ExeConfigurationFileMap { ExeConfigFilename = filePath };
            Configuration applicationConfig = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            var sqlConnectionString =  applicationConfig.ConnectionStrings.ConnectionStrings[1].ConnectionString;
            return GetFromActualConnectionString(sqlConnectionString);
        }

        private static string GetFromJson(string filePath)
        {
            dynamic jsonContent = JsonConvert.DeserializeObject(File.ReadAllText(filePath));

            return GetFromActualConnectionString((string)jsonContent.ConnectionStrings.Sidra);
        }

        private static string GetFromActualConnectionString(string connectionString)
        {
            var result = string.Empty;

            if (connectionString != null)
            {
                foreach (var part in connectionString.Replace("provider connection string=\"", "").Split(';'))
                {
                    if (part.StartsWith("data source", StringComparison.InvariantCultureIgnoreCase)
                        || part.StartsWith("initial catalog", StringComparison.InvariantCultureIgnoreCase))
                    {
                        result += part + ';';
                    }
                }
            }

            return result;
        }
    }
}