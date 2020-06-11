using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WordCloud.Models;

namespace WordCloud.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Admin()
        {
            SiteWordsViewModel siteWordsViewModel = new SiteWordsViewModel();
            siteWordsViewModel.Words = getSiteWords();
            return View(siteWordsViewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public ActionResult WordCloud(string url, bool verbOnly)
        { 
            WordCloudViewModel wordCloudView = new WordCloudViewModel();
            wordCloudView.url = url;

            List<SiteWord> siteWords = generateWordsForSite(url, 100, verbOnly);
            wordCloudView.Words = siteWords;

            storeSiteWords(siteWords);

            return View(wordCloudView);
        }

        /// <summary>
        /// Takes a web site url and returns a list of words on that page (Only words in a or p tags). The words are sorted by the number of times they appear on the page.
        /// </summary>
        /// <param name="url">The web site</param>
        /// <param name="limit">Limits the number of words returned</param>
        /// <param name="verbOnly">Restrict the site words to only verbs or nouns</param>
        /// <returns></returns>
        private List<SiteWord> generateWordsForSite(string url, int limit, bool verbOnly)
        {
            _logger.LogInformation(verbOnly ? "only verb" : "anything");
            WebClient w = new WebClient();
            string html = w.DownloadString(url);
            _logger.LogInformation(html);

            string pattern = @"<a|p[^>]*?>([^<]*)</a|p>";
            MatchCollection matches = Regex.Matches(html, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            VerbAndNounChecker verbAndNounChecker = new VerbAndNounChecker();

            List<SiteWord> siteWords = new List<SiteWord>();
            foreach (Match match in matches)
            {
                string[] words = match.Groups[1].Value.Trim().Split();
                foreach (string word in words)
                {
                    if (string.IsNullOrEmpty(word)) continue;
                    if (verbOnly && verbAndNounChecker.NotVerbOrNoun(word)) continue;

                    SiteWord siteWord = siteWords.Find(siteWord => siteWord.Word == word);
                    if (siteWord != null)
                    {
                        siteWord.Count++;
                        continue;
                    }

                    siteWord = new SiteWord(word);
                    siteWords.Add(siteWord);
                }

            }

            return siteWords.OrderByDescending(siteWord => siteWord.Count).Take(limit).ToList();
        }

        /// <summary>
        /// Stores the passed site words, when passed sie word already exists in storage the record will be updated
        /// </summary>
        /// <param name="words">List of site words</param>
        private void storeSiteWords(List<SiteWord> words)
        {
            using (SiteWordContext context = new SiteWordContext())
            {
                
                var wordIds = words.Select(word => word.Id);
                var existingWordIds = new HashSet<string>(
                    context.SiteWords.Where(x => wordIds.Contains(x.Id)).Select(x => x.Id));

                foreach(SiteWord word in words)
                {
                    if (existingWordIds.Contains(word.Id))
                        context.Update(word);
                    else
                        context.SiteWords.Add(word);
                }

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Returns all site words in storage
        /// </summary>
        /// <returns>List of site words</returns>
        private List<SiteWord> getSiteWords()
        {
            List<SiteWord> words = new List<SiteWord>();
            using (SiteWordContext context = new SiteWordContext())
            {
                words = context.SiteWords.OrderByDescending(siteWord => siteWord.Count).ToList();   
            }
            return words;
        }
    }

    public class VerbAndNounChecker {

        private List<string> knownVerbsAndNouns { get; set; }

        public VerbAndNounChecker()
        {
            using (StreamReader r = new StreamReader("./verbs.json"))
            {
                string json = r.ReadToEnd();
                List<string> knownVerbs = JsonConvert.DeserializeObject<List<string>>(json);
                knownVerbsAndNouns = knownVerbs;
            }

            using (StreamReader r = new StreamReader("./nouns.json"))
            {
                string json = r.ReadToEnd();
                List<string> knownNouns = JsonConvert.DeserializeObject<List<string>>(json);
                knownVerbsAndNouns = knownVerbsAndNouns.Concat(knownNouns).ToList();
            }
        }

        /// <summary>
        /// Returns true when the passed word is not a verb or noun
        /// </summary>
        /// <param name="word">The word to be checked</param>
        /// <returns>bool</returns>
        public bool NotVerbOrNoun(string word) => !knownVerbsAndNouns.Contains(word);
    }
}
