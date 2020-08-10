using NUnit.Framework;
using OpenQA.Selenium;
using PageComponents;
using System.Drawing;
using System.Linq;

namespace PageComponentUnitTests
{
    [Parallelizable(ParallelScope.All)]
    public class ElementsTests : WebTestBase
    {
        Element AddElementButton => new Element("button[onclick*=addElement]");
        Elements DeleteButtons => new Elements("button[onclick*=deleteElement]");

        [SetUp]
        public void Setup()
        {
            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/add_remove_elements/");
        }

        [Test]
        public void TestElementClickWorks()
        {
            AddElementButton.Click();
            AddElementButton.Click();
            AddElementButton.Click();
            foreach (var ele in DeleteButtons)
            {
                ele.Click();
            }
        }

        [Test]
        public void TestElementsGoStale()
        {
            AddElementButton.Click();
            AddElementButton.Click();
            AddElementButton.Click();
            DeleteButtons.Verify().IsPresent();
            Assert.AreEqual(3, DeleteButtons.Count());
            driver.Navigate().Refresh();
            var newCount = DeleteButtons.Count();

            Assert.AreEqual(0, DeleteButtons.Count());
        }

        [Test]
        public void TestElementsAssertCount()
        {
            AddElementButton.Click();
            AddElementButton.Click();
            AddElementButton.Click();
            DeleteButtons.Verify().Count(3);
        }


        [Test]
        public void TestElementsAssertCountGreaterThan()
        {
            AddElementButton.Click();
            AddElementButton.Click();
            AddElementButton.Click();
            DeleteButtons.Verify().CountGreaterThan(2);
        }

        [Test]
        public void TestElementsAssertCountNotGreaterThan()
        {
            AddElementButton.Click();
            AddElementButton.Click();
            AddElementButton.Click();
            DeleteButtons.VerifyNot().CountGreaterThan(4);
        }

        [Test]
        public void TestElementsAssertCountNot()
        {
            AddElementButton.Click();
            AddElementButton.Click();
            AddElementButton.Click();
            DeleteButtons.VerifyNot().Count(2);
        }

        [Test]
        public void TestElementsIsPresent()
        {
            AddElementButton.Click();
            DeleteButtons.Verify().IsPresent();
        }

        [Test]
        public void TestElementsIsNotPresent()
        {
            DeleteButtons.VerifyNot().IsPresent();
        }


    }
}