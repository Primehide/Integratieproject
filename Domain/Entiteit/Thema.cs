using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entiteit
{
    public class Thema
    {
        [Key]
        public int ThemaId { get; set; }
        public List<Sleutelwoord> SleutenWoorden { get; set; }
    }
}
