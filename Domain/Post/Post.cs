using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Post
{
    public class Post
    {
        public int PostId { get; set; }
        public List<HashTag> HashTags { get; set; }
        public List<Word> Words { get; set; }
        public DateTime Date { get; set; }
        public Naam Naam { get; set; }
        public string geo { get; set; }
        public string id { get; set; }
        public Sentiment Sentiment { get; set; }
        public bool retweet { get; set; }
        public string source { get; set; }
        public List<URL> Urls { get; set; }
        public List<Mention> Mentions { get; set; }
    }
}
