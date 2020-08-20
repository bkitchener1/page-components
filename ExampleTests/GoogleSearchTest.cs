using NUnit.Framework;
using PageComponents;
using ExampleTests.Pages;

namespace ExampleTests
{
    [Parallelizable(ParallelScope.All)]
    public class GoogleSearchTests : WebTestBase
    {

        [Test]
        public void Test1()
        {
            GoogleHomePage.OpenPage<GoogleHomePage>().SearchFor("selenium").ClickLinkWithText("Selenium automates browsers. That's it! What you do with that power is entirely up to you.");
        }
    }
}