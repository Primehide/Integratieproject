using Domain.Entiteit;
using System.Collections.Generic;
using System.Web;


namespace DAL
{
    public interface IEntiteitRepository
    {
        void CreatePersonWithPhoto(Persoon p, HttpPostedFileBase imageFile);
        void CreatePersonWithoutPhoto(Persoon p);
        Persoon ReadPerson(int id);
        Persoon UpdatePerson(Persoon updatedPerson);
        void UpdatePerson(Persoon changedPerson, IEnumerable<string> selectedOrganisations );
        void DeletePerson(int id);
        IEnumerable<Persoon> ReadAllPeople();
        byte[] GetPersonImageFromDataBase(int Id);

        void CreateOrganisatieWithPhoto(Organisatie o, HttpPostedFileBase imageFile);
        void CreateOrganisatieWithoutPhoto(Organisatie o);
        Organisatie UpdateOrganisatie(Organisatie updatedOrganisatie );
        IEnumerable<Organisatie> ReadAllOrganisaties();
        Organisatie ReadOrganisatie(int id);
        void DeleteOrganisatie(int id);
        Organisatie UpdateOrganisatie(Organisatie changedOrganisatie, IEnumerable<string> selectedPeople );
        byte[] GetOrganisationImageFromDataBase(int Id);

        void AddEntiteit(Entiteit entiteit);
        List<Entiteit> GetAlleEntiteiten();
        void UpdateEntiteit(Entiteit entiteit);
        Entiteit ReadEntiteit(int id);
        List<Entiteit> ReadEntiteiten(string naam);

        void CreateThema(Thema thema, HttpPostedFileBase imageFile);
        Thema UpdateThema(Thema thema);
        void DeleteThema(int entiteitsId);
        void DeleteSleutelwoord(int sleutelId);
        Thema ReadThema(int entiteitsId);
        Sleutelwoord ReadSleutelwoord(int sleutelId);
        IEnumerable<Thema> ReadThemas();
        IEnumerable<Entiteit> ReadEntiteitenVanDeelplatform(int id);
        void DeleteEntiteitenVanDeelplatform(int id);
        List<Entiteit> GetAlleEntiteiten(bool includePosts);
    }
}
