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
    }
}