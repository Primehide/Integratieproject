using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class PlatformRepository : IPlatformRepository
    {
        private EFContext ctx;

        public PlatformRepository()
        {
            ctx = new EFContext();
        }
    }
}
