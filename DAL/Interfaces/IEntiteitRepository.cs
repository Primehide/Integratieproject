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
        IEnumerable<Persoon> ReadAllPeople();

        void CreateOrganisatie(Organisatie o);
        Organisatie UpdateOrganisatie(Organisatie UpdatedOrganisatie);
        IEnumerable<Organisatie> ReadAllOrganisaties();
        Organisatie ReadOrganisatie(int id);
        void DeleteOrganisatie(int id);
        Organisatie UpdateOrganisatie(Organisatie changedOrganisatie, IEnumerable<string> selectedPeople);
        void UpdatePerson(Persoon changedPerson, IEnumerable<string> selectedOrganisations);
    }
}
