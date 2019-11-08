using System.Linq;
using PageComponents;
using SampleTests.Components;

namespace SampleTests.Pages
{
    class GoogleSearchResultsPage : BasePageObject
    {
        public PageObjectList<GoogleSearchResult> SearchResults = new PageObjectList<GoogleSearchResult>(".srg .g");


        public void ClickLinkWithText(string searchText)
        {
            var result = SearchResults.First(x => x.ResultText.Text.Contains(searchText));
                result.ResultLink.Click();
        }
    }
}
