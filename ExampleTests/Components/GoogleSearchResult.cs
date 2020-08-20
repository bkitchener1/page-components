using System;
using System.Collections.Generic;
using System.Text;
using PageComponents;

namespace ExampleTests.Components
{
    class GoogleSearchResult : BaseComponent
    {
        public Element ResultLink => this.Element("a>div");
        public Element ResultText => this.Element(".st");
        public Element ResultTitle => this.Element("a>h3");

    }
}
