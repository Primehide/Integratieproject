using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Post
{
    public class Profile
    {
        public int ProfileId { get; set; }
        public string gender { get; set; }
        public string age { get; set; }
        public string education { get; set; }
        public string language { get; set; }
        public string personality { get; set; }
    }
}
