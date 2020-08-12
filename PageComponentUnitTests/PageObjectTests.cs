using NUnit.Framework;
using PageComponents;
using System;
using System.Collections.Generic;
using System.Text;

namespace PageComponentUnitTests
{
    class PageObjectTests : WebTestBase
    {
        public class PageObject : BasePageObject
        {
            public PageObject()
            {
                this.Uri = "http://www.google.com/";
            }
        }

        [Test]
        public void TestOpenPage()
        {
            PageObject.OpenPage<PageObject>();
            Assert.IsTrue(driver.Url.Contains("www.google.com/"),"Page url not correct");
        }


    }
}
