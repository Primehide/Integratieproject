using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entiteit
{
    public class Persoon : Entiteit
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Organisatie> Organisations { get; set; }
        public byte[] Image { get; set; }
        public string Disctrict { get; set; }
        public string Level { get; set; }
        public string Gender { get; set; }
        public string Twitter { get; set; }
        public string Site { get; set; }
        public string DateOfBirth { get; set; }
        public string Facebook { get; set; }
        public string Postal_Code { get; set; }
        public string Full_name { get; set; }
        public string position { get; set; }
        public string Organisation { get; set; }
        public string Town { get; set; }
    }
}
