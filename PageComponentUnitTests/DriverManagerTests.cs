using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using PageComponents;
using PageComponentUnitTests.Pages;
using System;
using WebDriverManager.DriverConfigs.Impl;

namespace PageComponentUnitTests
{
    public class DriverManagerTests 
    {
       [Test]
       public void TestDriverSet()
        {
            new WebDriverManager.DriverManager().SetUpDriver(new FirefoxConfig());
            DriverManager.WebDriver = new FirefoxDriver();
            Assert.AreEqual(typeof(FirefoxDriver), DriverManager.WebDriver.GetType());
            DriverManager.WebDriver.Quit();
        }

        [Test]
        public void TestDriver()
        {
            var driver = DriverManager.StartDriver();
            Assert.NotNull(driver);
            driver.Quit();
        }

        [Test]
        public void TestLocalDriver()
        {
            var driver = DriverManager.StartLocalDriver();
            Assert.NotNull(driver);
            driver.Quit();
        }

        [Test]
        public void TestRemoteDriver()
        {
            Assert.Throws<WebDriverException>(() =>
            {
                var driver = DriverManager.StartRemoteBrowser();
                Assert.NotNull(driver);
                driver.Quit();
            });
            
        }

        [Test]
        public void TestWiniumDriver()
        {
            Assert.Throws<WebDriverException>(() =>
            {
                var driver = DriverManager.StartWiniumSession();
                Assert.NotNull(driver);
                driver.Quit();
            });

        }

       

    }
}