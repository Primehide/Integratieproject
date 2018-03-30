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
        void CreatePerson(Persoon p);
        Persoon ReadPerson(int id);
        Persoon UpdatePerson(Persoon UpdatedPerson);
        void DeletePerson(int id);
    }
}
