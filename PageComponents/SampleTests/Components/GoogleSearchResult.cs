using System;
using System.Collections.Generic;
using System.Text;
using PageComponents;

namespace SampleTests.Components
{
    class GoogleSearchResult : BasePageObject
    {
        public Element ResultLink => new Element(this, "a>div");
        public Element ResultText => new Element(this, ".st");
        public Element ResultTitle => new Element(this, "a>h3");

    }
}
