using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SEOAnalyzer.Models;

namespace SEOAnalyzer.ViewModels
{
    public class ResultViewModel
    {
        public static List<Occurrence> Occurrences;
        public static int LinkOccurrence;
        public static Search SearchData;

        public ResultViewModel(List<Occurrence> occurrences, int linkOccurrence, Search searchData)
        {
            Occurrences = occurrences;
            LinkOccurrence = linkOccurrence;
            SearchData = searchData;
        }

        public static ResultViewModel Get()
        {
            if( Occurrences == null )
            {
                Occurrences = new List<Occurrence>();
            }

            if( SearchData == null )
            {
                SearchData = new Search();
            }

            var rvm = new ResultViewModel(Occurrences, LinkOccurrence, SearchData);
            return rvm;
        }

        public static void Set( List<Occurrence> occurrences)
        {
            Occurrences = occurrences;
        }
    }
}