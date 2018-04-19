using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TextGain
{
    public class PostRequest
    {
        public string name { get; set; }
        public DateTime since { get; set; }
        public DateTime until { get; set; }
    }
}
