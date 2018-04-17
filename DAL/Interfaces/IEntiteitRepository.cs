using Domain.Entiteit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Domain.Entiteit;

namespace DAL
{
    public interface IEntiteitRepository
    {
      //chuck
        void CreatePersonWithPhoto(Persoon p, HttpPostedFileBase ImageFile);
        void CreatePersonWithoutPhoto(Persoon p);
        Persoon ReadPerson(int id);
        Persoon UpdatePerson(Persoon UpdatedPerson);
        void UpdatePerson(Persoon changedPerson, IEnumerable<string> selectedOrganisations);
        void DeletePerson(int id);
        IEnumerable<Persoon> ReadAllPeople();
        byte[] GetPersonImageFromDataBase(int Id);
        void CreateOrganisatieWithPhoto(Organisatie o, HttpPostedFileBase ImageFile);
        void CreateOrganisatieWithoutPhoto(Organisatie o);
        Organisatie UpdateOrganisatie(Organisatie UpdatedOrganisatie);
        IEnumerable<Organisatie> ReadAllOrganisaties();
        Organisatie ReadOrganisatie(int id);
        void DeleteOrganisatie(int id);
        Organisatie UpdateOrganisatie(Organisatie changedOrganisatie, IEnumerable<string> selectedPeople);
        byte[] GetOrganisationImageFromDataBase(int Id);
      
        //sander
        void AddEntiteit(Entiteit entiteit);
        List<Entiteit> getAlleEntiteiten();
        void updateEntiteit(Entiteit entiteit);
    }
}
