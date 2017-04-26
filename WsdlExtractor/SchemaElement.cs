using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WsdlExtractor
{
    public class TypeDescription
    {
        public string Name { get; set; }
        public int MinOccurs { get; set; }
        public bool Nullable { get; set; }

        public string TypeLib { get; set; }
        public string Type { get; set; }
    }
}