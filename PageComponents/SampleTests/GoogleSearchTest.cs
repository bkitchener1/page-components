using NUnit.Framework;
using PageComponents;

namespace SampleTests
{
    public class Tests : WebTestBase
    {

        [Test]
        public void Test1()
        {
            GoogleHomePage.OpenPage<GoogleHomePage>().SearchFor("selenium").ClickLinkWithText("Selenium is a portable framework for testing web applications");
        }
    }
}