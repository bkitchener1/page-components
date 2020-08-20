using NUnit.Framework;
using PageComponents;
using System;
using System.Collections.Generic;
using System.Text;

namespace PageComponentUnitTests
{
    class BrowserTests
    {
        [Test]
        public void TestChrome()
        {
            PageComponents.TestContext.CurrentContext.TestConfig.BrowserName = "chrome";
           var driver = DriverManager.StartDriver();
            Assert.NotNull(driver.CurrentWindowHandle);
            driver.Quit();
        }

        [Test]
        public void TestFF()
        {
            PageComponents.TestContext.CurrentContext.TestConfig.BrowserName = "firefox";
            var driver = DriverManager.StartDriver();
            Assert.NotNull(driver.CurrentWindowHandle);
            driver.Quit();
        }

        [Test]
        public void TestEdge()
        {
            PageComponents.TestContext.CurrentContext.TestConfig.BrowserName = "edge";
            var driver = DriverManager.StartDriver();
            Assert.NotNull(driver.CurrentWindowHandle);
            driver.Quit();
        }

        [Test, Ignore("This test only works on a mac")]
        public void TestSafari()
        {
            PageComponents.TestContext.CurrentContext.TestConfig.BrowserName = "safari";
            var driver = DriverManager.StartDriver();
            Assert.NotNull(driver.CurrentWindowHandle);
            driver.Quit();
        }
    }
}
