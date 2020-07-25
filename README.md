# page-components

## Beyond Page Objects
Page objects are a great way to build robust reusable automated tests.  However many times we need a way to build smaller, more modular, and reusable components on a page.  This is how page-components was born!

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

### Creating Components
Smaller reusable components can be built using the same base PageComponent abstract class, and can be included as part of a larger page object
```cs
public class HeaderComponent : PageComponent {
  public Element LogOutLink => new Element(this, By.CssSelector(".logout"));
  
  public LoginPage LogOut() {
    LogOutLink.Click();
    return new LoginPage();
  }
}

public class HomePage : PageComponent {
   public Header = new HeaderComponent(By.CssSelector(".header"));
}
```
