using System;
using System.Collections.Generic;

namespace WordCloud.Models
{
    public class WordCloudViewModel
    {
        public string url { get; set; }
        public List<SiteWord> Words = new List<SiteWord>();
    }
}
