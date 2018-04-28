using Domain.Entiteit;
using Domain.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IPlatformRepository
    {
        void CreateDeelplatform(Deelplatform d);
        Deelplatform ReadDeelplatform(int id);
        IEnumerable<Deelplatform> ReadAllDeelplatformen();
        Deelplatform UpdateDeelplatform(Deelplatform uDeelplatform);
        void DeleteDeelplatform(int id);


    }
}
