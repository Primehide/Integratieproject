using Domain.Entiteit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IEntiteitManager
    {
        void AddPerson(Persoon p);
        Persoon GetPerson(int id);
        Persoon ChangePerson(Persoon ChangedPerson);
        void RemovePerson(int id);
    }
}
