using NUnit.Framework;
using PageComponents;

namespace SampleTests
{
    [Parallelizable]
    public class ParallelTest : WebTestBase
    {

        [Test]
        [Parallelizable]
        public void Test1()
        {
            GoogleHomePage.OpenPage<GoogleHomePage>().SearchFor("selenium").ClickLinkWithText("Selenium automates browsers. That's it! What you do with that power is entirely up to you.");
        }
        [Test]
        [Parallelizable]
        public void Test2()
        {
            GoogleHomePage.OpenPage<GoogleHomePage>().SearchFor("webdriver").ClickLinkWithText("WebDriver is a remote control interface that enables introspection and control of user agents.");
        }
    }
}