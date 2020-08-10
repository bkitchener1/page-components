using NUnit.Framework;
using OpenQA.Selenium;
using PageComponents;
using System.Drawing;

namespace PageComponentUnitTests
{
    [Parallelizable(ParallelScope.All)]
    public class ElementPropertyTests : WebTestBase
    {
        Element AddElementButton => new Element("button[onclick*=addElement]");
        Element DeleteButton => new Element("button[onclick*=deleteElement]");

        [SetUp]
        public void Setup()
        {
            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/add_remove_elements/");
        }



        [Test]
        public void TestElementIsPresent()
        {
            Assert.IsTrue(AddElementButton.Present, "Could not find Add Element Button");
        }

        [Test]
        public void TestElementIsNotPresent()
        {
            Assert.IsFalse(DeleteButton.Present, "Delete button should not be present");
        }

        [Test]
        public void TestElementTextIsPresent()
        {
            Assert.AreEqual("Add Element", AddElementButton.Text, "Add Element Button Text not correct");
        }

        [Test]
        public void TestElementAttributeIsPresent()
        {
            Assert.AreEqual("addElement()", AddElementButton.GetAttribute("onclick"), "Add Element Button attribute 'onclick' not correct");
        }

        [Test]
        public void TestElementLocation()
        {
            Assert.NotNull(AddElementButton.Location, "Add Element Button Location Not correct");
        }

        [Test]
        public void TestElementSize()
        {
            Assert.AreEqual(new Size(148, 45), AddElementButton.Size, "Add Element Button size Not correct");
        }

        [Test]
        public void TestElementTag()
        {
            Assert.AreEqual("button", AddElementButton.TagName, "Add Element Button tag correct");
        }

        [Test]
        public void TestElementEnabled()
        {
            Assert.AreEqual(true, AddElementButton.Enabled, "Add Element Button enabled Not correct");
        }

        [Test]
        public void TestElementDisplayed()
        {
            Assert.AreEqual(true, AddElementButton.Displayed, "Add Element Button displayed Not correct");
        }

        [Test]
        public void TestElementSelected()
        {
            Assert.AreEqual(false, AddElementButton.Selected, "Add Element Button selected Not correct");
        }
    }
}