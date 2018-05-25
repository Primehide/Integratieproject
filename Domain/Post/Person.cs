using System.Collections.Generic;

namespace Domain.Post
{
    public class Person
    {
        public int PersonId { get; set; }
        public string Naam { get; set; }
        public List<Word> Words { get; set; }
    }
}
