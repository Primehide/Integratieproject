using System.Collections.Generic;

namespace Domain.Entiteit
{
    public class Organisatie : Entiteit
    {
        //public string Naam { get; set; }
        public int AantalLeden { get; set; }
        public string Gemeente { get; set; }
        public List<Persoon> Leden { get; set; }
       // public byte[] Image { get; set; }
    }
}
