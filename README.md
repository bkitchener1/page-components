# page-components

## Beyond Page Objects
Page objects are a great way to build robust reusable automated tests.  However many times we need a way to build smaller, more modular, and reusable components on a page.  This is how page-components was born!  Page Components are a hybrid between a page object and an element...a page object that has an element as a root container.  All elements in  page component use the root element for searches, allowing us to use simple locators.  

## Overview

The page components library is intended to simplify the process of building reliable, scalable, and complex Selenium-WebDriver tests.  Page-Components takes the concepts of the page object model and extends it, allowing users to build page objects as well as small, modular, reusable sub-components on pages (page-components).  This helps build page objects which contain components that are reused across multiple pages, and also helps break page objects up into smaller reusable components that can be more easily identified and manipulated.  It is primarily composed of a few classes that help make the process of building selenium page objects easier

* Element - A class that acts as a wrapper around the IWebElement interface.  It can be easily instantiated in a class header, or in a method.  It handles implicit and explit waiting, automatically caches found IWebElement objects to avoid duplicate lookups, and automatically re-finds new references to IWebElements when they go stale.  Essentially it handles the hard parts of Selenium-WebDriver, so you don't have to!

* Elements - A class that acts as a List of Element objects.  This allows you to iterate through the collection, to find a specific item, manipulate each one in turn, or do anything else you might need.  

* PageComponent - An abstract base class that is inherited by all the page objects or subcomponents in our tests.  Implementing classes can be used as full page objects, including a URL and a Visit() function to open and instantiate them.  Or they can be components, acting as a smaller section of a larger page object. This class inherits from Element, so it can be instantiated with a locator just like an Element, which acts as a root element or container for all further element queries. 

* PageComponentList - A class that acts as a List of PageComponents.  Allows the user to instnatiate a list of page objects using one locator, and iterate through the collection.  This allows us to build page components that are duplicated multiple times on the page, and iterate through them.  It also allows us to quickly query them using LINQ methods to quickly select the appropriate component in the collection. 

## Usage
To start using, install via nuget : page-components.
### Using the built in webdriver manager
You can simply inherit the WebTestBase class from our test classes, and webdriver instantiation will happen automatically before each test, and quit after.  An extent html report will be generated including log messages and screenshots on test failure.
#### Configuration
The WebConfig class contains static values for all the configurable options, such as selecting the appropriate browser, running a remote session, and specifying the default timeout.  Default values can be overridden by using the appropriate key in the app.config.

### Creating Page Objects and Elements
To create a normal page object, create a new class and inherit the PageComponent class.  You can specify a URL that can be used for navigation purposes.
```cs
public class LoginPage : PageComponent{
  public Element LoginField = new Element(By.CssSelector(".user"));
  public Element PasswordField = new Element(By.CssSelector(".password"));
  public Element SignInButton = new Element(By.CssSelector(".login"));
  public Elements ErrorMessages = new Elements(By.CssSelector(".error"));
  
  public HomePage Login(string username, string password)
  {
    LoginField.SetText(username);
    PasswordField.SetText(password);
    SignInButton.Click();
    return new HomePage();
  }
  
  public LoginPage AssertNoErrors(){
    ErrorMessages.Verify().Count(0);
  }
}
```

### Creating a Component
Smaller reusable components can be built using the same base PageComponent abstract class, and can be included as part of a larger page object
Components act as a page object with a root element.  They are instantiated with a locator, and elements use the component as the root node for all searches.
This helps to minimize search context when multiple elements are present that match a locator

When instantiating an Element inside a component, the component must be passed into the element as a root element.  The component can be passed into the Element constructor using the "this" keyword (new Element(this, By.CssSelector(".class")), or the Element can be instantiated using a function in the component "this.Element(By.CssSelector(".class")"
```cs
public class HeaderComponent : PageComponent {
  public Element LogOutLink => new Element(this, By.CssSelector(".logout"));
  public Element ProfileLink => this.Element(By.CssSelector(".profile"));
  
  public LoginPage LogOut() {
    LogOutLink.Click();
    return new LoginPage();
  }
}

public class HomePage : PageComponent {
   public Header = new HeaderComponent(By.CssSelector(".header"));
}
```

### PageComponentLists
Many times components are repeated on a page numerous times.  This can be a row of a table, or a panel displayed in a grid.  
In this case the PageComponentList class can be used to build a List of components.  The PageComponentList is instantiated with
a selector that returns the multiple elements on a page.  Each iteration of the component uses one of those elements as the root element,
and all searches will be inside of that element.  

For example, the google results page.  When a user searches for some text, the results page is displayed.  A GoogleSearchResult is built representing a single google result
in the list, containing the search link, title, and text.  

The list of google results are represented by building PageComponentList, with a locator that returns the root element of each result in the list.  We can now iterate through
the collection, or use LINQ to query the collection.  This allows me to avoid having to use static locators using an index (such as an xpath expression), or build complicated 
locators that reference multiple elements on the page.  I can now easily click the link of a search result containing specific text.
```cs
//The GoogleSearchResult component represents a single google search result item on the google results page
public class GoogleSearchResult : BaseComponent
{
    //Each Element is instantiated using "this" to show that these elements are descendents of the component
    public Element ResultLink => this.Element("a>div");
    public Element ResultText => this.Element(".st");
    public Element ResultTitle => this.Element("a>h3");

}

//The GoogleSearchResultsPage represents the entire google search results page
public class GoogleSearchResultsPage : BasePageObject
{
    //The List of google search results.  Each item in the list will return one GoogleSearchResult found using the selector
    public PageComponentList<GoogleSearchResult> SearchResults = new PageComponentList<GoogleSearchResult>(".g");

    //We can easily query the list of search results using LINQ to find relative elements.  IN this case I want to click on the 
    //search results link for a result that contains specific text.  
    public void ClickLinkWithText(string searchText)
    {
        var result = SearchResults.First(x => x.ResultText.Text.Contains(searchText));
            result.ResultLink.Click();
    }
}
```


