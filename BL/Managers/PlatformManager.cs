using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace BL
{
    public class PlatformManager : IPlatformManager
    {
        private IPlatformRepository platformRepository;

        public PlatformManager()
        {
            platformRepository = new PlatformRepository();
        }
    }
}
