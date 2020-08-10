using NUnit.Framework;
using PageComponents;
using System;
using System.Collections.Generic;
using System.Text;

namespace PageComponentUnitTests
{
    [NUnit.Framework.Parallelizable]
    public class ParallelTests : WebTestBase
    {
        [Test]
        [Parallelizable]
        public void Test1()
        {
            Element AddElementButton = new Element("button[onclick*=addElement]");
            Element DeleteButton = new Element("button[onclick*=deleteElement]");

            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/add_remove_elements/");

            AddElementButton.Click();
            DeleteButton.Click();
        }

        [Test]
        [Parallelizable]
        public void Test2()
        {
            driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/inputs");
            var ele = new Element("input[type=number]");
            ele.SendKeys("12345");
        }
    }
}
