using Domain.Enum;
using Domain.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Account
{
    public class DashboardBlok
    {
        public int DashboardBlokId { get; set; }
        public int DashboardLocatie { get; set; }
        public String Titel { get; set; }
        public Grafiek Grafiek { get; set; }
        public BlokGrootte BlokGrootte { get; set; }
        public int sizeX { get; set; }
        public int sizeY { get; set; }
    }
}
