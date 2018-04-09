using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IEntiteitRepository
    {
        void AddEntiteit(Domain.Entiteit.Entiteit entiteit);
    }
}
