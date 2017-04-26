using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WsdlExtractor
{
    public class SchemaRequest
    {
        public SchemaRequest()
        {
            this.XsdUrls = new List<string>();
            this.Types = new Dictionary<string, List<TypeDescription>>();
        }

        public List<string> XsdUrls { get; set; }
        public Dictionary<string, List<TypeDescription>> Types { get; set; }
    }
}