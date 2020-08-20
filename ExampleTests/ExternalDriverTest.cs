using ExampleTests.Pages;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using PageComponents;
using System;
using WebDriverManager.DriverConfigs.Impl;

namespace ExampleTests
{
    public class ExternalDriverTest 
    {
        [SetUp]
        public void SetupDriver()
        {
            new WebDriverManager.DriverManager().SetUpDriver(new ChromeConfig());
            DriverManager.WebDriver = new ChromeDriver();
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