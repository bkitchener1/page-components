using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
        private static bool EnvFileLoaded = false;
        public int ElementTimeoutMs { get; set; }
        public bool AutoLaunchBrowser { get; set; }
        public bool AutoQuitBrowser { get; set; }
        public string DefaultUrl { get; set; }
        public string BrowserName { get; set; }
        public string Browser2Name { get; set; }
        public string Browser3Name { get; set; }
        public int BrowserHeight { get; set; }
        public int BrowserWidth { get; set; }
        public bool HighlightElements { get; set; }
        public bool HeadlessBrowser { get; set; }
        public bool RemoteSession { get; set; }
        public string RemoteServer { get; set; }
        public string RemoteCapabilities { get; set; }
        public bool WiniumSession { get; set; }
        public string WiniumApp { get; set; }

        public static string EnvFilePath
        {
            get
            {
                var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                for (var i = 0; i < 5 && dir.FullName.Contains("\\bin"); i++)
                    dir = Directory.GetParent(dir.FullName);
                return Path.Combine(dir.ToString(), ".env");
            }
        }

        public static void LoadEnvFile()
        {
            if (File.Exists(EnvFilePath) && !EnvFileLoaded)
            {
                DotNetEnv.Env.Load(EnvFilePath, new DotNetEnv.Env.LoadOptions(clobberExistingVars: false));
                EnvFileLoaded = true;
            }
        }


        public TestConfig()
        {
            LoadEnvFile();

            ElementTimeoutMs = DotNetEnv.Env.GetInt("ElementTimeoutMs", 10000);
            AutoLaunchBrowser = DotNetEnv.Env.GetBool("AutoLaunchBrowser", true);
            AutoQuitBrowser = DotNetEnv.Env.GetBool("AutoQuitBrowser", true);
            DefaultUrl = DotNetEnv.Env.GetString("DefaultUrl", "");
            BrowserName = DotNetEnv.Env.GetString("BrowserName", "chrome");
            Browser2Name = DotNetEnv.Env.GetString("Browser2Name", null);
            Browser3Name = DotNetEnv.Env.GetString("Browser3Name", null);
            HighlightElements = DotNetEnv.Env.GetBool("HighlightElements", true);
            BrowserWidth = DotNetEnv.Env.GetInt("BrowserWidth", 1680);
            BrowserHeight = DotNetEnv.Env.GetInt("BrowserHeight", 1020);
            HeadlessBrowser = DotNetEnv.Env.GetBool("HeadlessBrowser", false);
            RemoteSession = DotNetEnv.Env.GetBool("RemoteSession", false);
            RemoteServer = DotNetEnv.Env.GetString("RemoteServer", "http://127.0.0.1:4444");
            RemoteCapabilities = DotNetEnv.Env.GetString("RemoteCapabilities", "");
            WiniumSession = DotNetEnv.Env.GetBool("WiniumSession", false);
            WiniumApp = DotNetEnv.Env.GetString("WiniumApp", "");
    }

    }
}
