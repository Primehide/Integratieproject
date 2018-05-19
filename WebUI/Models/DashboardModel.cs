using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models
{
    public class DashboardModel
    {
        public Domain.Account.DashboardConfiguratie Configuratie { get; set; }
        public int CanvasCounter { get; set; }
        public int ScriptCounter { get; set; }

        public Dictionary<string,string> GrafiekLabels { get; set; }
        public Dictionary<string, string> GrafiekDataSets { get; set; }
        public int DataSetTeller { get; set; }
        public List<string> ColorCodes { get; set; }

        public List<Domain.Entiteit.Item> GevolgdeItems { get; set; }
        public List<Domain.Entiteit.Entiteit> AlleEntiteiten { get; set; }
        public Domain.Entiteit.Entiteit Entiteit { get; set; }

    }
}