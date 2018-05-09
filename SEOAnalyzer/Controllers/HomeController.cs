using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SEOAnalyzer.ViewModels;
using SEOAnalyzer.Models;

namespace SEOAnalyzer.Controllers
{
    public class HomeController : Controller
    {
        public List<StopWord> model = new List<StopWord>();

        public ActionResult Index()
        {
            model.Add(new StopWord { Name = "or" });
            model.Add(new StopWord { Name = "and" });
            model.Add(new StopWord { Name = "the" });

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult StopWord()
        {
            ViewBag.Message = "Your stopword page.";
 
            return View(model);
        }

        [HttpPost]
        public ActionResult StopWord(String name)
        {
            //check for model.Name property value now
            //to do : Return something
            model.Add(new StopWord { Name = name });
            return RedirectToAction("StopWord");
        }

        public ActionResult Search(String sortOrder = "")
        {
            ViewBag.Message = "Your search page.";

            var rvm = ResultViewModel.Get();

            var occurrences = from s in ResultViewModel.Occurrences
                              select s;

            ViewBag.WordSortParm = String.IsNullOrEmpty(sortOrder) ? "word_desc" : "";
            ViewBag.PageSortParm = sortOrder == "page" ? "page_desc" : "page";
            ViewBag.MetaSortParm = sortOrder == "meta" ? "meta_desc" : "meta";

            switch (sortOrder)
            {
                case "word_desc":
                    occurrences = occurrences.OrderByDescending(s => s.Word);
                    break;
                case "page":
                    occurrences = occurrences.OrderBy(s => s.PageOccurrence);
                    break;
                case "page_desc":
                    occurrences = occurrences.OrderByDescending(s => s.PageOccurrence);
                    break;
                case "meta":
                    occurrences = occurrences.OrderBy(s => s.MetaOccurrence);
                    break;
                case "meta_desc":
                    occurrences = occurrences.OrderByDescending(s => s.MetaOccurrence);
                    break;
                default:
                    occurrences = occurrences.OrderBy(s => s.Word);
                    break;
            }

            ResultViewModel.Occurrences = new List<Occurrence>(occurrences);

            return View(rvm);
        }
        
        [HttpPost]
        public ActionResult Search(String urltext = "", String searchtext = "", Boolean pageOccurrence = false, Boolean metaOccurrence = false, Boolean linkOccurrence = false, String sortOrder = "")
        {
            var search = new Search()
            {
                SearchURL = urltext,
                SearchText = searchtext,
                IsCheckPageOccurrence = pageOccurrence,
                IsCheckMetaOccurrence = metaOccurrence,
                IsCheckLinkOccurrence = linkOccurrence,
                IsCheckURL = urltext.Length > 0
            };

            var rvm = ResultViewModel.Get();
            search.DoSearch();
            ResultViewModel.SearchData = search;

            ViewBag.WordSortParm = String.IsNullOrEmpty(sortOrder) ? "word_desc" : "";
            ViewBag.PageSortParm = sortOrder == "Page" ? "page_desc" : "page";
            ViewBag.MetaSortParm = sortOrder == "Meta" ? "meta_desc" : "meta";

            return View(rvm);
        }
    }
}