using NUnit.Framework;
using OpenQA.Selenium;
using PageComponents;
using System;
using System.Collections.Generic;
using System.Text;

namespace PageComponentUnitTests
{
    [Parallelizable(ParallelScope.All)]
    class WebElementCaching : WebTestBase
    {
        Element AddElementButton => new Element("button[onclick*=addElement]");
        Elements DeleteButtons => new Elements("button[onclick*=deleteElement]");

        [Test]
        public void SaveElementToCache()
        {
            Element button = new Element("button[onclick*=addElement]");

            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/add_remove_elements/");

            button.Click();

            WebElementCache.SaveElementToCache(button.WrappedElement, button.ToString());

            var ele = WebElementCache.GetCachedElement(button.ToString());
            Assert.AreEqual(button.WrappedElement, ele);
        }

        [Test]
        public void VerifyElementCaches()
        {

            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/add_remove_elements/");

            AddElementButton.Click();

            var ele = WebElementCache.GetCachedElement(AddElementButton.ToString());
            Assert.NotNull(ele);
        }

        [Test]
        public void VerifyElementCacheClearsWhenStale()
        {

            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/add_remove_elements/");

            AddElementButton.Click();
            driver.Navigate().Refresh();
            var ele = WebElementCache.GetCachedElement(AddElementButton.ToString());
            Assert.IsNull(ele);
        }

        [Test]
        public void VerifyElementsCache()
        {

            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/add_remove_elements/");

            AddElementButton.Click();
            AddElementButton.Click();
            AddElementButton.Click();

            DeleteButtons.Verify().IsPresent();
            var eles = WebElementsCache.GetCachedElements(DeleteButtons.ToString());
            Assert.AreEqual(3, eles.Count);
        }
    }
}
