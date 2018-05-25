using System.Collections.Generic;

namespace WebUI.Models
{
    public class EditPersonViewModel
    {
        public Domain.Entiteit.Persoon Persoon { get; set; }
        public List<Domain.Entiteit.Organisatie> Organisaties { get; set; }
    }
}