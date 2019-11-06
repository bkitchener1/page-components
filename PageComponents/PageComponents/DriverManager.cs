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

namespace PageComponents
{
    /// <summary>
    /// Manages launching and managing the webdriver instance based upon the app.config setting currently set.
    /// By default the webdriver instance is stored in a dictionary using the nunit test name as a key
    /// This allows an Element class to be instantiated in class headers without hard-coding a webdriver instance 
    /// </summary>
    public static class DriverManager
    {
        private static IDictionary<string, IWebDriver> _webDrivers;

        /// <summary>
        /// The list of webdriver instances currently stored.  Each Test has it's own instance of webdriver.
        /// This supports parallel executions of tests and multiple instances of webdriver
        /// If additional instances are needed for a test they must bne added manually
        /// </summary>
        public static IDictionary<string, IWebDriver> WebDrivers
        {
            get { return _webDrivers ?? (_webDrivers = new Dictionary<string, IWebDriver>()); }
            set { _webDrivers = value; }
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
            IWebDriver driver;

            if (WebConfig.RemoteSession)
            {
                driver = StartRemoteBrowser();
            }
            else if (WebConfig.WiniumSession)
            {
                driver = StartWiniumSession();
            }
            else
            {
                driver = StartLocalDriver();
            }

            if (testName == null)
            {
                testName = TestContext.CurrentContext.Test.Name;
            }
            WebDrivers[testName] = driver;
            return driver;

        }

        public static IWebDriver StartWiniumSession()
        {
            IWebDriver driver = null;
            Uri remoteUri = new Uri(WebConfig.RemoteServer);
            DesiredCapabilities caps = new DesiredCapabilities();
            caps.SetCapability("app", WebConfig.WiniumApp);
            driver = new RemoteWebDriver(remoteUri, caps);
            return driver;
        }

        public static IWebDriver StartRemoteBrowser()
        {
            IWebDriver driver = null;
            Uri remoteUri = new Uri(WebConfig.RemoteServer + "/wd/hub/");
            var options = GetRemoteBrowserOptions();
            options = AddRemoteCapibilitiesFromConfig(options);
            driver = new RemoteWebDriver(remoteUri, options);
            return driver;
        }

        private static DriverOptions AddRemoteCapibilitiesFromConfig(DriverOptions options)
        {
            var caps = WebConfig.RemoteCapabilities.Split(';');
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
            string browserName = WebConfig.BrowserName;
            IWebDriver driver = null;
            Uri remoteUri = new Uri(WebConfig.RemoteServer);
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
            string browserName = WebConfig.BrowserName;
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

        /// <summary>
        /// Gets the current instance of webdriver based upon the current test name.
        /// IF a new browser is needed use StartDriver() instead
        /// </summary>
        /// <returns></returns>
        public static IWebDriver GetDriver(string testName="")
        {
            if (testName == "") testName = TestContext.CurrentContext.Test.Name;
            if (!WebDrivers.ContainsKey(testName))
            {
                throw new Exception($"Could not find WebDriver instance for test {testName}");
            }
            return WebDrivers[testName];
        }

        private static string GetDriverPath(string browserName)
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
        public static void CloseAllDrivers()
        {
            foreach (var driver in _webDrivers)
            {
                driver.Value.Close();
                driver.Value.Quit();
            }
        }
    }
}
