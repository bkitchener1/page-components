using System.Linq;
using PageComponents;
using SampleTests.Components;

namespace SampleTests.Pages
{
    class GoogleSearchResultsPage : BasePageObject
    {
        public PageComponentList<GoogleSearchResult> SearchResults = new PageComponentList<GoogleSearchResult>(".g");

        public void ClickLinkWithText(string searchText)
        {
            var result = SearchResults.First(x => x.ResultText.Text.Contains(searchText));
                result.ResultLink.Click();
        }
    }
}
