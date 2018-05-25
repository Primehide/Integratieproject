using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Account
{
    public class DashboardConfiguratie
    {
        public int DashboardConfiguratieId { get; set; }
        public String ConfiguratieNaam { get; set; }
        public List<DashboardBlok> DashboardBlokken { get; set; }
    }
}
