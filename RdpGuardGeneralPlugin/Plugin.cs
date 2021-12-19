using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using rdpguard_plugin_api;

namespace RdpGuardGeneralPlugin
{
    public class Plugin : IExternalLogBasedEngine
    {
        public const string NotConfiguredText = "NOT_CONFIGURED";

        private static Configuration _configuration = null;

        public string Name
        {
            get
            {
                CheckConfiguration();
                return _configuration.Name;
            }
        }

        public string Protocol
        {
            get
            {
                CheckConfiguration();
                return _configuration.Protocol;
            }
        }

        public string Directory
        {
            get
            {
                CheckConfiguration();
                return _configuration.Directory;
            }
        }

        public string FileMask
        {
            get
            {
                CheckConfiguration();
                return _configuration.FileMask;
            }
        }

        public string RegularExpression
        {
            get
            {
                CheckConfiguration();
                return _configuration.RegularExpression;
            }
        }

        public bool IsFailedLoginAttempt(string line, out string ip, out string username)
        {
            CheckConfiguration();

            var match = Regex.Match(line, RegularExpression);
            if (!match.Success)
            {
                ip = null;
                username = null;
                return false;
            }

            ip = match.Groups["ip"].Value;
            username = match.Groups["username"].Value;

            if (string.IsNullOrWhiteSpace(ip))
                throw new Exception($"Cannot find ip in line {line}. Check configuration.");

            return true;
        }

        private void CheckConfiguration()
        {
            if (_configuration != null)
                return;

            string configurationFileName = GetConfigurationFileName();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));

            Configuration configuration;

            if (!File.Exists(configurationFileName))
            {
                CreateSampleConfiguration();
                throw new Exception($"Plugin not configured (missing '{GetConfigurationFileName()}')");
            }

            try
            {
                using (var fileStream = new StreamReader(configurationFileName))
                    configuration = (Configuration)xmlSerializer.Deserialize(fileStream);
            }
            catch (Exception e)
            {
                throw new Exception($"Error reading configuration file '{configurationFileName}' ({e.Message})");
            }

            if (configuration.Protocol == NotConfiguredText)
            {
                throw new Exception($"Plugin not configured (Protocol not configured in file '{configurationFileName}')");
            }

            _configuration = configuration;

        }

        public static void CreateSampleConfiguration()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));

            Configuration configuration = new Configuration()
            {
                Protocol = NotConfiguredText
            };

            using (var fileStream = new StreamWriter(GetConfigurationFileName()))
                xmlSerializer.Serialize(fileStream, configuration);
        }

        public static string GetConfigurationFileName()
        {
            var assemblyFileName = typeof(Configuration).Assembly.Location;
            var configurationFileName = Path.GetFileNameWithoutExtension(assemblyFileName) + ".config";
            return configurationFileName;
        }
    }
}
