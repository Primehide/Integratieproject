using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace BL
{
    public class PlatformManager
    {
        private IPlatformRepository platformRepository;

        public PlatformManager()
        {
            platformRepository = new PlatformRepository();
        }
    }
}
