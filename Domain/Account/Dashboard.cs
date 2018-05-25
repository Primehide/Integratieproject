using System;

namespace Domain.Account
{
    public class Dashboard
    {
        public int DashboardId { get; set; }
        public String Naam { get; set; }
        public DashboardConfiguratie Configuratie { get; set; }
        public Boolean IsPublic { get; set; }
    }
}
