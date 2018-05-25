using System.Collections.Generic;

namespace Domain.Post
{
    public class Term
    {
        public int TermId { get; set; }
        public string Naam { get; set; }
        public List<Post> Posts { get; set; }

    }
}
