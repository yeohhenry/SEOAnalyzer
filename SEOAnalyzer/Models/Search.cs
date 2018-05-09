using System;
using System.Collections.Generic;
using SEOAnalyzer.ViewModels;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Text;
using System.Web.Mvc;

namespace SEOAnalyzer.Models
{
    public class Search
    {
        public Boolean IsCheckPageOccurrence { get; set; }
        public Boolean IsCheckMetaOccurrence { get; set; }
        public Boolean IsCheckLinkOccurrence { get; set; }
        public String SearchURL { get; set; }
        public String SearchText { get; set; }
        public Boolean IsCheckURL { get; set; }
        public List<StopWord> stopWords = new List<StopWord>();

        public void DoSearch()
        {
            stopWords.Add(new StopWord { Name = "or" });
            stopWords.Add(new StopWord { Name = "and" });
            stopWords.Add(new StopWord { Name = "the" });

            if ( this.IsCheckPageOccurrence || this.IsCheckMetaOccurrence)
            { 
                if (this.IsCheckURL)
                {
                    this.DoSearchURL();
                }
                else
                {
                    this.DoSearchText(this.SearchText);
                }
            }
        }

        public void DoSearchText( String searchText )
        {
            this.SearchHyperlink(searchText);

            if (this.IsCheckPageOccurrence)
            {
                this.SearchWord(searchText, true);
            }

            this.SearchStopWordInMeta(searchText);
        }
            

        public void DoSearchURL()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.SearchURL);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string tmp = reader.ReadToEnd();
                
                response.Close();

                this.DoSearchText(tmp);             
            }
            catch ( WebException ex )
            {
                Console.WriteLine( ex.Message );
            }
            catch ( Exception ex )
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SearchWord( String text, Boolean isPage )
        {
            // Filter out all stopwords
            foreach( StopWord word in stopWords)
            {
                text.Replace(word.Name, "");
            }

            foreach (string s in text.Split(new char[] { ' ', '.', ',', '?' }))
            {
                s.Trim();

                if( s.Length > 0 )
                {
                    int i = ResultViewModel.Occurrences.FindIndex(x => x.Word.ToLower() == s.ToLower());
                    Occurrence occurence = null;

                    // Create a new occurrence if word does not exists
                    if (i == -1)
                    {
                        occurence = new Occurrence { Word = s, PageOccurrence = 0, MetaOccurrence = 0 };
                        ResultViewModel.Occurrences.Add(occurence);
                    }
                    else
                    {
                        occurence = ResultViewModel.Occurrences[i];
                    }

                    // Increase occurrence nr
                    if (isPage)
                    {
                        occurence.PageOccurrence++;
                    }
                    else 
                    {
                        occurence.MetaOccurrence++;
                    }
                }
            }
        }

        public void SearchStopWordInMeta(String text)
        {
            if(this.IsCheckMetaOccurrence && this.IsCheckURL )
            {
                var webGet = new HtmlWeb();
                var document = webGet.Load(this.SearchURL);
                var metaTags = document.DocumentNode.SelectNodes("//meta");

                if (metaTags != null)
                {
                    foreach (var tag in metaTags)
                    {
                        var tagContent = tag.Attributes["content"];
                        if (tagContent != null)
                        {
                            this.SearchWord(tagContent.Value, false);
                        }
                    }
                }
            }
        }

        public void SearchHyperlink( String searchText)
        {
            Regex regx = new Regex("http(s)?://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase);
            MatchCollection matches = regx.Matches(searchText);

            foreach( Match url in matches)
            {
                searchText = searchText.Replace(url.Value, "");
            }

            if (this.IsCheckLinkOccurrence)
            {
                ResultViewModel.LinkOccurrence = matches.Count;
            }
        }
    }
}