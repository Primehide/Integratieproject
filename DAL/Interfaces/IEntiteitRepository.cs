using Domain.Entiteit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IEntiteitRepository
    {
        void CreateThema(Thema thema);
        void UpdateThema(Thema thema);
        void DeleteThema(int entiteitsId);
        Thema ReadThema(int entiteitsId);
        IEnumerable<Thema> ReadThemas();
    }
}
