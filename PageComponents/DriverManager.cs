using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using WebDriverManager.DriverConfigs.Impl;

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
            if (TestContext.CurrentContext.TestConfig.RemoteSession)
            {
                WebDriver = StartRemoteBrowser();
            }
            else if (TestContext.CurrentContext.TestConfig.WiniumSession)
            {
                WebDriver = StartWiniumSession();
            }
            else
            {
                WebDriver = StartLocalDriver();
            }

            if (testName == null)
            {
                testName = NUnit.Framework.TestContext.CurrentContext.Test.Name;
            }
            return WebDriver;

        }

        public static IWebDriver StartWiniumSession()
        {
            IWebDriver driver = null;
            Uri remoteUri = new Uri(TestContext.CurrentContext.TestConfig.RemoteServer);
            DesiredCapabilities caps = new DesiredCapabilities();
            caps.SetCapability("app", TestContext.CurrentContext.TestConfig.WiniumApp);
            driver = new RemoteWebDriver(remoteUri, caps);
            return driver;
        }

        public static IWebDriver StartRemoteBrowser()
        {
            IWebDriver driver = null;
            Uri remoteUri = new Uri(TestContext.CurrentContext.TestConfig.RemoteServer + "/wd/hub/");
            var options = GetRemoteBrowserOptions();
            options = AddRemoteCapibilitiesFromConfig(options);
            driver = new RemoteWebDriver(remoteUri, options);
            return driver;
        }

        private static DriverOptions AddRemoteCapibilitiesFromConfig(DriverOptions options)
        {
            var caps = TestContext.CurrentContext.TestConfig.RemoteCapabilities.Split(';');
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
            string browserName = TestContext.CurrentContext.TestConfig.BrowserName;
            IWebDriver driver = null;
            Uri remoteUri = new Uri(TestContext.CurrentContext.TestConfig.RemoteServer);
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
            string browserName = TestContext.CurrentContext.TestConfig.BrowserName;
            IWebDriver driver = null;
            switch (browserName.ToLower())
            {
                case WebBrowsers.InternetExplorer:
                    new WebDriverManager.DriverManager().SetUpDriver(new InternetExplorerConfig());
                    InternetExplorerOptions options = new InternetExplorerOptions();
                    options.IntroduceInstabilityByIgnoringProtectedModeSettings = true;
                    options.AcceptInsecureCertificates = true;
                    driver = new InternetExplorerDriver(options);
                    break;
                case WebBrowsers.Firefox:
                    new WebDriverManager.DriverManager().SetUpDriver(new FirefoxConfig());
                    driver = new FirefoxDriver();
                    break;
                case WebBrowsers.Chrome:
                    new WebDriverManager.DriverManager().SetUpDriver(new ChromeConfig());
                    driver = new ChromeDriver();
                    break;
                case WebBrowsers.Edge:
                    new WebDriverManager.DriverManager().SetUpDriver(new LegacyEdgeConfig());
                    new WebDriverManager.DriverManager().SetUpDriver(new EdgeConfig());
                    var pathVariable = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
                    driver = new EdgeDriver();
                    break;
                default:
                    throw new Exception($"Could not launch browser of name {browserName} please use : ie, firefox, or chrome");
                    break;
            }
            return driver;
        }

        private static object locker = new object();

    }
}
