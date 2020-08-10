using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using PageComponents;
using System;

namespace SampleTests
{
    public class ExternalDriverTest 
    {
        [SetUp]
        public void SetupDriver()
        {
            DriverManager.WebDriver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory);
        }

        [TearDown]
        public void Teardown()
        {
            DriverManager.WebDriver.Quit();
        }

        [Test]
        public void Test()
        {
            GoogleHomePage.OpenPage<GoogleHomePage>().SearchFor("selenium").ClickLinkWithText("Selenium automates browsers. That's it! What you do with that power is entirely up to you.");
        }
    }
}