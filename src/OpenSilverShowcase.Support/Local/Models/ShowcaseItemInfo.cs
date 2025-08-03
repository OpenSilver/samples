using System;
using System.Collections.Generic;
using System.Text;

namespace OpenSilverShowcase.Support.Local.Models
{
    public class ShowcaseItemInfo
    {
        public string Name { get; set; }
        public Type Type { get; set; }

        public override string ToString() => Name;
    }
}
