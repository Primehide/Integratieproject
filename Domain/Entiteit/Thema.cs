using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entiteit
{
    public class Thema : Entiteit
    {
        public List<Sleutelwoord> SleutenWoorden { get; set; }
    }
}
