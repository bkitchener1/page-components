using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using PageComponents;
using System.Drawing;

namespace PageComponentUnitTests
{
    [Parallelizable(ParallelScope.All)]
    public class ElementCommandTests : WebTestBase
    {
        Element AddElementButton => new Element("button[onclick*=addElement]");
        Element DeleteButton => new Element("button[onclick*=deleteElement]");


        [Test]
        public void TestElementClickWorks()
        {
            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/add_remove_elements/");

            AddElementButton.Click();
        }

        [Test]
        public void TestCannotClickElementNotPresent()
        {
            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/add_remove_elements/");

            Assert.Throws<WebDriverTimeoutException>(
                () => DeleteButton.Click()
                );
        }

        [Test]
        public void TestTypeAndGetValue()
        {
            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/inputs");
            var ele = new Element("input[type=number]");
            ele.SendKeys("123");
            Assert.AreEqual("123", ele.Value);

        }

        [Test]
        public void TestSetText()
        {
            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/inputs");
            var ele = new Element("input[type=number]");
            ele.SendKeys("123");
            ele.SetText("23232");
            Assert.AreEqual("23232", ele.Value);

        }

        [Test]
        public void TestElementWaitTimeDefaults()
        {
            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/add_remove_elements/");

            Assert.AreEqual(10000, DeleteButton.TimeoutMs);
        }

        [Test]
        public void TestElementWaitTimeOverride()
        {
            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/add_remove_elements/");

            var ele = new Element(".class", 1323);
            Assert.AreEqual(1323, ele.TimeoutMs);
        }

        [Test]
        public void TestInvalidFrame()
        {
            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/add_remove_elements/");

            Frame iframe = new Frame("#mce_0_ifr");
            Element editor = new Element("#tinymce", iframe);
            Assert.Throws<WebDriverTimeoutException>(() =>
            {
                editor.SendKeys("this is a test");
            });

        }

        [Test]
        public void TestSelectsFrame()
        {
            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/iframe");

            Frame iframe = new Frame("#mce_0_ifr");
            Element editor = new Element("#tinymce", iframe);
            Element example = new Element(".example");

            editor.SetText("this is a test");
            Assert.AreEqual("this is a test", editor.Text);

            Assert.AreEqual("An iFrame containing the TinyMCE WYSIWYG Editor\r\nFile Edit View Format\r\nFormats\r\np", example.Text);

            Assert.AreEqual("this is a test", editor.Text);

            Assert.AreEqual("An iFrame containing the TinyMCE WYSIWYG Editor\r\nFile Edit View Format\r\nFormats\r\np", example.Text);

        }



        [Test]
        public void TestContainer()
        {
            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/add_remove_elements/");

            Element container = new Element("#content");
            Element addElementButton = new Element(container, "button[onclick*=addElement]");
            addElementButton.Click();
            DeleteButton.Verify().IsDisplayed();
        }

        [Test]
        public void TestElementNotFoundOutsideContainer()
        {
            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/challenging_dom");

            Element button = new Element(".button");
            Element table = new Element("table");
            Element invalidButton = new Element(table, ".button");
            invalidButton.VerifyNot().IsPresent();
            button.Verify().IsPresent();



        }
    }
 }