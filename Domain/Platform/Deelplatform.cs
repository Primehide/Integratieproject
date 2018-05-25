using System.Collections.Generic;

namespace Domain.Platform
{
    public class Deelplatform
    {
        public int DeelplatformId { get; set; }
        public string Naam { get; set; }
        public string Tagline { get; set; }
        public List<Domain.Entiteit.Entiteit> Entiteiten { get; set; }
        public byte[] Logo { get; set; }
        public string ColorCode1 { get; set; }
        public string ColorCode2 { get; set; }
    }
}
