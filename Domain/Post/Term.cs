using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Post
{
    public class Term
    {
        public int TermId { get; set; }
        public string Naam { get; set; }
        public List<Post> Posts { get; set; }

    }
}
