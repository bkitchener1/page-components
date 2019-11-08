using System.Linq;
using PageComponents;
using SampleTests.Components;

namespace SampleTests.Pages
{
    class GoogleSearchResultsPage : BasePageObject
    {
        public PageObjectList<GoogleSearchResult> SearchResults = new PageObjectList<GoogleSearchResult>(".g");


        public void ClickLinkWithText(string searchText)
        {
            SearchResults.First(x => x.ResultText.Text.Contains(searchText)).ResultLink.Click();
        }
    }
}
