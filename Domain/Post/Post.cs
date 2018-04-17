using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Post
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        public Profile Profile { get; set; }
        public List<HashTag> HashTags { get; set; }
        public List<Word> Words { get; set; }
        public DateTime Date { get; set; }
        public List<Person> Persons { get; set; }
        public string geo { get; set; }
        public string postNummer { get; set; }
        public Sentiment Sentiment { get; set; }
        public bool retweet { get; set; }
        public string source { get; set; }
        public List<URL> Urls { get; set; }
        public List<Mention> Mentions { get; set; }
    }
}
