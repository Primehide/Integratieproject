using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        public string Geo { get; set; }
        public string PostNummer { get; set; }
        public Sentiment Sentiment { get; set; }
        public bool Retweet { get; set; }
        public string Source { get; set; }
        public List<URL> Urls { get; set; }
        public List<Mention> Mentions { get; set; }
        public List<Entiteit.Entiteit> Entiteiten { get; set; }

    }
}
