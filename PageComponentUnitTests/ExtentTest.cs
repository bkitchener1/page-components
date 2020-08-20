using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using PageComponents;
using PageComponentUnitTests.Pages;
using System;

namespace PageComponentUnitTests
{
    public class ExtentTest : ExtentTestBase
    {
        [Test]
        public void Test()
        {
            GoogleHomePage.OpenPage<GoogleHomePage>().SearchFor("selenium").ClickLinkWithText("Selenium automates browsers. That's it! What you do with that power is entirely up to you.");
        }
    }
}