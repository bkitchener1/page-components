using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageComponents
{
    /// <summary>
    /// WebConfig stores config values from the app.config in the appropriate format.
    /// If no value has been set in the app.config uses the default setting provided here
    /// </summary>
    public class TestConfig
    {
        public int ElementTimeoutMs = GetConfigInt("ElementTimeoutMs", 10000);
        public bool AutoLaunchBrowser = GetConfigBool("AutoLaunchBrowser", true);
        public bool AutoQuitBrowser = GetConfigBool("AutoQuitBrowser", true);
        public string DefaultUrl = GetConfigString("DefaultUrl", "");
        public string BrowserName = GetConfigString("BrowserName", "chrome");
        public bool RemoteSession = GetConfigBool("RemoteSession", false);
        public string RemoteServer = GetConfigString("RemoteServer", "http://127.0.0.1:4444");
        public string RemoteCapabilities = GetConfigString("RemoteCapabilities", "");
        public bool WiniumSession = GetConfigBool("WiniumSession", false);
        public string WiniumApp = GetConfigString("WiniumApp", "");

        private static string GetConfigString(string key, string defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];
            if (value != null)
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }

        private static bool GetConfigBool(string key, bool defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];
            if (value != null)
            {
                var result = Boolean.Parse(value.ToLower());
                return result;
            }
            else
            {
                return defaultValue;
            }
        }


        private static int GetConfigInt(string key, int defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];
            if (value != null)
            {
                return Int32.Parse(value);
            }
            else
            {
                return defaultValue;
            }  
        }
    }
}
