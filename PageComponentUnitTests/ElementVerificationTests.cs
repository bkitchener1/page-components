using NUnit.Framework;
using OpenQA.Selenium;
using PageComponents;
using System.Drawing;

namespace PageComponentUnitTests
{
    [Parallelizable(ParallelScope.All)]
    public class ElementVerificationTests : WebTestBase
    {
        Element StartButton => new Element("#start>button");
        Element LoadingImage => new Element("#loading");
        Element HelloWorldText => new Element("#finish");

        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void VerifyElementDisplayed()
        {
            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/dynamic_loading/1");
            driver.WaitForAjax(30000);
            StartButton.Verify().IsDisplayed().Click();
            HelloWorldText.Verify().IsDisplayed();
        }

        [Test]
        public void VerifyElementNotDisplayed()
        {
            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/dynamic_loading/1");
            driver.WaitForAjax(30000);
            HelloWorldText.VerifyNot().IsDisplayed();
        }

        [Test]
        public void VerifyElementPresent()
        {
            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/dynamic_loading/2");
            driver.WaitForAjax(30000);
            StartButton.Verify().IsPresent().Click();
            HelloWorldText.Verify().IsPresent();
        }

        [Test]
        public void VerifyElementNotPresent()
        {
            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/dynamic_loading/2");
            driver.WaitForAjax(30000);
            HelloWorldText.VerifyNot().IsPresent();
        }

        [Test]
        public void VerifyElementText()
        {
            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/dynamic_loading/2");
            driver.WaitForAjax(30000);
            StartButton.Verify().TextIs("Start");
        }

        [Test]
        public void VerifyElementNotText()
        {
            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/dynamic_loading/2");
            driver.WaitForAjax(30000);
            StartButton.VerifyNot().TextIs("False");
        }

        [Test]
        public void VerifyElementAttribute()
        {
            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/dynamic_loading/2");
            driver.WaitForAjax(30000);
            StartButton.Click();
            LoadingImage.Verify().AttributeIs("id","loading");
        }

        [Test]
        public void VerifyElementAttributeNot()
        {
            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/dynamic_loading/2");
            driver.WaitForAjax(30000);
            StartButton.VerifyNot().AttributeIs("id","False");
        }
    }
}
