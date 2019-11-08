using NUnit.Framework;
using PageComponents;

namespace SampleTests
{
    public class Tests : WebTestBase
    {

        [Test]
        public void Test1()
        {
            GoogleHomePage.OpenPage<GoogleHomePage>().SearchFor("selenium").ClickLinkWithText("The biggest change in");
        }
    }
}