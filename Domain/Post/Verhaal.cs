using System.Collections.Generic;

namespace Domain.Post
{
    public class Verhaal
    {
        public int VerhaalId { get; set; }
        public URL Url { get; set; }
        public List<Post> Posts { get; set; }
    }
}
