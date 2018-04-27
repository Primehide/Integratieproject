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
        //STAAF
        public int PostFreqGrafiekCounter { get; set; }
        public int PostFreqLabelCounter { get; set; }
        public int PostFreqDataCounter { get; set; }

        public int PopulariteitLabelCounter { get; set; }
        public int PopulariteitDataCounter { get; set; }

        //LIJN
        public int PostFreqLijnCounter { get; set; }
        public int PostFreqLabelLijnCounter { get; set; }
        public int PostFreqDataLijnCounter { get; set; }

        public int PopulariteitLabelLijnCounter { get; set; }
        public int PopulariteitDataLijnCounter { get; set; }

        //TAART
        public int PopulariteitGrafiekTaartCounter { get; set; }
        public int PopulariteitLabelTaartCounter { get; set; }
        public int PopulariteitDataTaartCounter { get; set; }
    }
}