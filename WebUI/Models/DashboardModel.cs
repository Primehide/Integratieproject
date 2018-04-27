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
        public int PostFreqGrafiekCounter { get; set; }
        public int PostFreqLabelCounter { get; set; }
        public int PostFreqDataCounter { get; set; }
        public int PopulariteitLabelCounter { get; set; }
        public int PopulariteitDataCounter { get; set; }
    }
}