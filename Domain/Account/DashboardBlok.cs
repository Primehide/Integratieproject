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
        public Grafiek Grafiek { get; set; }
    }
}
