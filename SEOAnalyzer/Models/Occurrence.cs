using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEOAnalyzer.Models
{
    public class Occurrence
    {
        public String Word { get; set; }
        public int PageOccurrence { get; set; }
        public int MetaOccurrence { get; set; }
    }
}