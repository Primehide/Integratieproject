using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Post
{
    public class Sentiment
    {
        public int SentimentId { get; set; }
        public double polariteit { get; set; }
        public double subjectiviteit { get; set; }
    }
}
