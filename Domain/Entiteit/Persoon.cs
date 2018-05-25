using System.Collections.Generic;

namespace Domain.Entiteit
{
    public class Persoon : Entiteit
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Organisatie> Organisations { get; set; }
      //  public byte[] Image { get; set; }
        public string Disctrict { get; set; }
        public string Level { get; set; }
        public string Gender { get; set; }
        public string Twitter { get; set; }
        public string Site { get; set; }
        public string DateOfBirth { get; set; }
        public string Facebook { get; set; }
        public string PostalCode { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string Organisation { get; set; }
        public string Town { get; set; }
    }
}
