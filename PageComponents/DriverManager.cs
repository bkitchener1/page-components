using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace PageComponents
{
    /// <summary>
    /// Manages launching and managing the webdriver instance based upon the app.config setting currently set.
    /// By default the webdriver instance is stored in a dictionary using the nunit test name as a key
    /// This allows an Element class to be instantiated in class headers without hard-coding a webdriver instance 
    /// </summary>
    public static class DriverManager
    {
        /// <summary>
        /// The list of webdriver instances currently stored.  Each Test has it's own instance of webdriver.
        /// This supports parallel executions of tests and multiple instances of webdriver
        /// If additional instances are needed for a test they must bne added manually
        /// </summary>
        private static ThreadLocal<IWebDriver> _webDrivers = new ThreadLocal<IWebDriver>();

        public static IWebDriver WebDriver
        {
            get
            {
                return _webDrivers.Value;
            }
            set
            {
                _webDrivers.Value = value;
            }
        }

        /// <summary>
        /// Starts a new webdriver browser based upon the setting stored in the app.config,
        /// and stores it based upon the testname passed into the function.
        /// If no test name isspecified it will use the Nunit test name as a key
        /// </summary>
        /// <param name="testName"></param>
        /// <returns></returns>
        public static IWebDriver StartDriver(string testName = null)
        {
            if (TestConfig.RemoteSession)
            {
                WebDriver = StartRemoteBrowser();
            }
            else if (TestConfig.WiniumSession)
            {
                WebDriver = StartWiniumSession();
            }
            else
            {
                WebDriver = StartLocalDriver();
            }

            if (testName == null)
            {
                testName = TestContext.CurrentContext.Test.Name;
            }
            return WebDriver;

        }

        public static IWebDriver StartWiniumSession()
        {
            IWebDriver driver = null;
            Uri remoteUri = new Uri(TestConfig.RemoteServer);
            DesiredCapabilities caps = new DesiredCapabilities();
            caps.SetCapability("app", TestConfig.WiniumApp);
            driver = new RemoteWebDriver(remoteUri, caps);
            return driver;
        }

        public static IWebDriver StartRemoteBrowser()
        {
            IWebDriver driver = null;
            Uri remoteUri = new Uri(TestConfig.RemoteServer + "/wd/hub/");
            var options = GetRemoteBrowserOptions();
            options = AddRemoteCapibilitiesFromConfig(options);
            driver = new RemoteWebDriver(remoteUri, options);
            return driver;
        }

        private static DriverOptions AddRemoteCapibilitiesFromConfig(DriverOptions options)
        {
            var caps = TestConfig.RemoteCapabilities.Split(';');
            if (caps[0] == "")
            {
                return options;
            }
            foreach (var cap in caps)
            {
                var capsSplit = cap.Split(',');
                var capibilityName = capsSplit[0];
                var capibilityValue = capsSplit[1];
                options.AddAdditionalCapability(capibilityName, capibilityValue);

            }
            return options;

        }

        private static DriverOptions GetRemoteBrowserOptions()
        {
            string browserName = TestConfig.BrowserName;
            IWebDriver driver = null;
            Uri remoteUri = new Uri(TestConfig.RemoteServer);
            switch (browserName.ToLower())
            {
                case WebBrowsers.InternetExplorer:
                    var ieOptions = new InternetExplorerOptions();
                    //ieOptions.IntroduceInstabilityByIgnoringProtectedModeSettings = true;
                   // ieOptions.AcceptInsecureCertificates = true; 
                    return ieOptions;
                case WebBrowsers.Firefox:
                    var ffOptions = new FirefoxOptions();
                    return ffOptions;
                case WebBrowsers.Chrome:
                    var chromeOptions = new ChromeOptions();
                    return chromeOptions;
                default:
                    throw new Exception($"Could not launch browser of name {browserName} please use : ie, firefox, or chrome");
            }
        }


        public static IWebDriver StartLocalDriver()
        {
            string browserName = TestConfig.BrowserName;
            IWebDriver driver = null;
            var path = GetDriverPath(browserName.ToLower());
            switch (browserName.ToLower())
            {
                case WebBrowsers.InternetExplorer:
                    InternetExplorerOptions options = new InternetExplorerOptions();
                    options.IntroduceInstabilityByIgnoringProtectedModeSettings = true;
                    options.AcceptInsecureCertificates = true;
                    driver = new InternetExplorerDriver(path, options);
                    break;
                case WebBrowsers.Firefox:
                    driver = new FirefoxDriver(path);
                    break;
                case WebBrowsers.Chrome:
                    driver = new ChromeDriver(path);
                    break;
                default:
                    throw new Exception($"Could not launch browser of name {browserName} please use : ie, firefox, or chrome");
                    break;
            }
            return driver;
        }

        private static object locker = new object();

        private static string GetDriverPath(string browserName)
        {
            try
            {
                string fileName = null;
                switch (browserName)
                {
                    case WebBrowsers.InternetExplorer:
                        fileName = "IEDriverServer.exe";
                        break;
                    case WebBrowsers.Firefox:
                        fileName = "GeckoDriver.exe";
                        break;
                    case WebBrowsers.Chrome:
                        fileName = "ChromeDriver.exe";
                        break;
                    default:
                        throw new Exception("Browser not supported : " + browserName);
                        break;
                }
                var dir = AppDomain.CurrentDomain.BaseDirectory;
                if (File.Exists(Path.Combine(dir, fileName)))
                {
                    return dir;
                }
                //if we're running from Bamboo the directory is different and we need to find the dll
                var paths = Directory.GetFiles(Path.Combine(Path.Combine(dir, ".."), ".."), fileName, SearchOption.AllDirectories);

                var path = Path.GetDirectoryName(paths[0]);
                if (File.Exists(path))
                {
                    return path;
                }
                else
                {
                    throw new Exception($"Could not find WebDriver Browser executable {fileName}.  It will need to be copied to path {dir} or {path} ");
                }
            }
            catch(Exception e)
            {
                throw new Exception($"Could not find WebDriver Browser executable {browserName}.  It will need to be copied to path {AppDomain.CurrentDomain.BaseDirectory} or installed via Nuget");

            }
            

        }

    }
}
