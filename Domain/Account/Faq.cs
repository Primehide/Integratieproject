using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Account
{
    public class Faq
    {
        public int FaqId { get; set; }
        public string Vraag { get; set; }
        public string Antwoord { get; set; }
        public int PlatformId { get; set; }
    }
}
