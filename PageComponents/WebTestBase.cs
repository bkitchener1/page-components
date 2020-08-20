using OpenQA.Selenium;
using System.Configuration;
using NUnit.Framework;
using System;
using System.IO;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium.Support.Extensions;
using NUnit.Framework.Internal;

namespace PageComponents
{

    /// <summary>
    /// Base test class inherited by all test classes.
    /// rovides logging, and automatic browser launching/closing
    /// </summary>
    public abstract class WebTestBase
    {
        protected IWebDriver driver
        {
            get
            {
                return DriverManager.WebDriver;
            }
            set
            {
                DriverManager.WebDriver = value;
            }
        }
        #region Test attributes

        //Use TestInitialize to run code before running each test 
        [SetUp]
        public void Setup()
        {
            // creates a test 
            // calling flush writes everything to the log file
            if (TestContext.CurrentContext.TestConfig.AutoLaunchBrowser)
            {
                DriverManager.StartDriver();
                driver.Manage().Window.Maximize();
            }
        }

        //Use TestCleanup to run code after each test has run
        [TearDown]
        public void Teardown()
        {
            try
            {
                if(NUnit.Framework.TestContext.CurrentContext.Result.Outcome != ResultState.Success)
                {
                    Logger.Error(NUnit.Framework.TestContext.CurrentContext.Result.Message);
                    Logger.Error(NUnit.Framework.TestContext.CurrentContext.Result.StackTrace);
                }
                if (TestContext.CurrentContext.TestConfig.AutoQuitBrowser)
                {
                    driver.Close();
                    driver.Quit();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }          
        }

        #endregion

    }
}
