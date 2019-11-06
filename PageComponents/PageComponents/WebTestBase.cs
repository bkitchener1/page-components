using OpenQA.Selenium;
using System.Configuration;
using NUnit.Framework;
using System;
using System.IO;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium.Support.Extensions;

namespace PageComponents
{

    /// <summary>
    /// Base test class inherited by all test classes.
    /// rovides logging, and automatic browser launching/closing
    /// </summary>
    public abstract class WebTestBase
    {
        protected IWebDriver driver;
        #region Test attributes

        private ExtentTest test => ExtentManager.GetTest();


        [OneTimeTearDown]
        public void TeardownAll()
        {
            //calling flush generates the report 
            ExtentManager.Flush();
           // DriverManager.CloseAllDrivers();
        }

      
        //Use TestInitialize to run code before running each test 
        [SetUp]
        public void Setup()
        {
            // creates a test 
            ExtentManager.CreateTest(this.GetType().Name + "." + TestContext.CurrentContext.Test.Name, TestContext.CurrentContext.Test.FullName);
            // calling flush writes everything to the log file
            if (WebConfig.AutoLaunchBrowser)
            {
                driver = DriverManager.StartDriver();
                driver.Manage().Window.Maximize();
            }
        }

        //Use TestCleanup to run code after each test has run
        [TearDown]
        public void Teardown()
        {
            try
            {
                if (TestContext.CurrentContext.Result.Outcome != ResultState.Success)
                {
                    test.Fail(TestContext.CurrentContext.Result.Message,
                        MediaEntityBuilder
                            .CreateScreenCaptureFromBase64String(driver.TakeScreenshot().AsBase64EncodedString)
                            .Build());
                    test.Error(TestContext.CurrentContext.Result.StackTrace);
                }

                if (WebConfig.AutoQuitBrowser)
                {
                    driver.Close();
                    driver.Quit();
                }
            }
            catch (Exception e)
            {
                test.Fatal(e);
            }          
        }

        #endregion

    }
}
