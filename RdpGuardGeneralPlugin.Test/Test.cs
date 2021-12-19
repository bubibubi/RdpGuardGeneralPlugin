using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RdpGuardGeneralPlugin.Test
{
    [TestClass]
    public class Test
    {
        [TestMethod]
        public void IfNotExistsConfiguration_CreatesSampleConfiguration_ThrowsException()
        {
            DeleteConfiguration();
            var plugin = new Plugin();
            try
            {
                string unused = plugin.Name;
                Assert.Fail("Expected exception");
            }
            catch (Exception)
            {
                // Ok
            }

            using (var fileStream = new StreamReader(Plugin.GetConfigurationFileName()))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));
                var configuration = (Configuration)xmlSerializer.Deserialize(fileStream);
                Assert.AreEqual(Plugin.NotConfiguredText, configuration.Protocol);
            }


        }


        [TestMethod]
        public void ExistingSampleConfiguration_ThrowsException()
        {
            DeleteConfiguration();
            Plugin.CreateSampleConfiguration();

            var plugin = new Plugin();
            try
            {
                string unused = plugin.Name;
                Assert.Fail("Expected exception");
            }
            catch (Exception)
            {
                // Ok
            }
        }


        private void DeleteConfiguration()
        {
            if (File.Exists(Plugin.GetConfigurationFileName()))
                File.Delete(Plugin.GetConfigurationFileName());
        }


        [TestMethod]
        public void ConnectionOkTest()
        {
            DeleteConfiguration();
            WriteConfiguration();
            var plugin = new Plugin();
            Assert.IsFalse(plugin.IsFailedLoginAttempt("1.2.3.4 ok", out string _, out string _));
        }


        [TestMethod]
        public void MissingIpThrowsException()
        {
            DeleteConfiguration();
            WriteConfiguration();
            var plugin = new Plugin();
            try
            {
                plugin.IsFailedLoginAttempt(" failed", out string _, out string _);
                Assert.Fail("Expected exception");
            }
            catch (Exception)
            {
                // Ok
            }
        }

        [TestMethod]
        public void ConnectionFailedTest()
        {
            DeleteConfiguration();
            WriteConfiguration();
            string ip;
            var plugin = new Plugin();
            Assert.IsTrue(plugin.IsFailedLoginAttempt("1.2.3.4 fail", out ip, out string _));

            Assert.AreEqual("1.2.3.4", ip);
        }


        private static void WriteConfiguration()
        {
            using (var fileStream = new StreamWriter(Plugin.GetConfigurationFileName()))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));
                var configuration = new Configuration()
                {
                    Protocol = "TEST",
                    RegularExpression = "(?<ip>.*) fail"
                };
                xmlSerializer.Serialize(fileStream, configuration);
            }
        }

    }
}
