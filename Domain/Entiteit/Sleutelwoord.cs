using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entiteit
{
    public class Sleutelwoord
    {
        public Sleutelwoord(string Woord) {
            woord = Woord;
            }
        public Sleutelwoord()
        {

        }
        public int SleutelwoordId { get; set; }
        public string woord { get; set; }
    }
}
