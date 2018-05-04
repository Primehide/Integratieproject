using Domain.Entiteit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace BL
{
    public interface IEntiteitManager
    {
        void AddPerson(Persoon p, HttpPostedFileBase ImageFile);
        Persoon GetPerson(int id);
        Persoon ChangePerson(Persoon ChangedPerson);
        void RemovePerson(int id);
        List<Persoon> GetAllPeople();
        byte[] GetPersonImageFromDataBase(int id);

        void AddOrganisatie(Organisatie o, HttpPostedFileBase ImageFile);
        Organisatie ChangeOrganisatie(Organisatie ChangedOrganisatie);
        List<Organisatie> GetAllOrganisaties();
        Organisatie GetOrganisatie(int id);
        void RemoveOrganisatie(int id);
        byte[] GetOrganisationImageFromDataBase(int id);

        void CreateTestData();
        List<Domain.Entiteit.Entiteit> getAlleEntiteiten();
        Entiteit getEntiteit(int entiteitId);
        void updateEntiteit(Entiteit entiteit);
        void AddThema(Thema nieuwThema, List<Sleutelwoord> sleutelwoorden);
        void UpdateThema(Thema thema);
        void DeleteThema(int entiteitsId);
        void DeleteSleutelwoord(int sleutelId);
        IEnumerable<Thema> GetThemas();
        Thema GetThema(int entiteitsId);
        Sleutelwoord GetSleutelwoord(int sleutelId);
        List<Entiteit> GetEntiteitenVanDeelplatform(int id);
        void DeleteEntiteitenVanDeelplatform(int id);

        Dictionary<string, double> BerekenGrafiekWaarde(Domain.Enum.GrafiekType grafiekType, List<Entiteit> entiteiten, List<string> CijferOpties, string VergelijkOptie);

    }
}
