using PageComponents;

namespace ExampleTests.Pages
{
    class GoogleHomePage : BasePageObject
    {
        public Element SearchField = new Element("input[name=q]");
        public Element SearchButton = new Element("input[name=btnK]");

        public GoogleHomePage()
        {
            this.Uri = "http://www.google.com/";
        }

        public GoogleSearchResultsPage SearchFor(string value)
        {
            SearchField.SendKeys(value).Submit();
            return new GoogleSearchResultsPage();
        }
    }
}
