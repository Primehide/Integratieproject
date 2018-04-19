using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TextGain
{
    public class TextGainResponse
    {
        public Profile profile { get; set; }
        public List<string> Words { get; set; }
        public List<string> sentiment { get; set; }
        public string source { get; set; }
        public List<string> hashtags { get; set; }
        public List<string> mentions { get; set; }
        public List<string> themes { get; set; }
        public List<string> persons { get; set; }
        public List<string> urls { get; set; }
        public DateTime date { get; set; }
        //public string geo { get; set; }
        public string id { get; set; }
        public bool retweet { get; set; }
    }
}
