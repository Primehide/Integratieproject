using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Post
{
    public class Verhaal
    {
        public int VerhaalId { get; set; }
        public URL Url { get; set; }
        public List<Post> Posts { get; set; }
    }
}
