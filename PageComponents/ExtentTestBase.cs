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
    /// Base class that hooks into extent for reporting
    /// </summary>
    public abstract class ExtentTestBase : WebTestBase
    {

        #region Test attributes

        private ExtentTest test => ExtentManager.GetTest();

        private void Log(string message)
        {
            test.Info(message);
            Console.WriteLine(message);
        }

        [OneTimeSetUp]
        public void SetupLogger()
        {
            Logger.SetLogger(Log);
        }

        [OneTimeTearDown]
        public void TeardownAll()
        {
            //calling flush generates the report 
            ExtentManager.Flush();
        }

      
        //Use TestInitialize to run code before running each test 
        [SetUp]
        public void Setup()
        {
            
            // creates a test 
            ExtentManager.CreateTest(this.GetType().Name + "." + NUnit.Framework.TestContext.CurrentContext.Test.Name, NUnit.Framework.TestContext.CurrentContext.Test.FullName);
        }

        //Use TestCleanup to run code after each test has run
        [TearDown]
        public void Teardown()
        {
            try
            {
                if (NUnit.Framework.TestContext.CurrentContext.Result.Outcome != ResultState.Success)
                {
                    test.Fail(NUnit.Framework.TestContext.CurrentContext.Result.Message,
                        MediaEntityBuilder
                            .CreateScreenCaptureFromBase64String(driver.TakeScreenshot().AsBase64EncodedString)
                            .Build());
                    test.Error(NUnit.Framework.TestContext.CurrentContext.Result.StackTrace);
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
