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
        public List<Organisatie> Organisaties { get; set; }
    }
}
