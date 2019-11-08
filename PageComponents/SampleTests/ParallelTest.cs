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
            GoogleHomePage.OpenPage<GoogleHomePage>().SearchFor("selenium");
        }
        [Test]
        [Parallelizable]
        public void Test2()
        {
            GoogleHomePage.OpenPage<GoogleHomePage>().SearchFor("selenium");
        }
    }
}