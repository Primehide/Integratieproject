using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models
{
    public class EditPersonViewModel
    {
        public Domain.Entiteit.Persoon Persoon { get; set; }
        public List<Domain.Entiteit.Organisatie> Organisaties { get; set; }
    }
}