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
        Persoon ChangePerson(Persoon ChangedPerson, HttpPostedFileBase ImageFile);
        void RemovePerson(int id);
        List<Persoon> GetAllPeople(int platId);
        byte[] GetPersonImageFromDataBase(int id);

        void AddOrganisatie(Organisatie o, HttpPostedFileBase ImageFile);
        Organisatie ChangeOrganisatie(Organisatie ChangedOrganisatie, HttpPostedFileBase ImageFile);
        List<Organisatie> GetAllOrganisaties(int platId);
        Organisatie GetOrganisatie(int id);
        void RemoveOrganisatie(int id);
        byte[] GetOrganisationImageFromDataBase(int id);

        void CreateTestData();
        List<Domain.Entiteit.Entiteit> getAlleEntiteiten();
        Entiteit getEntiteit(int entiteitId);
        void updateEntiteit(Entiteit entiteit);
        Entiteit GetEntiteit(int id);
        List<Entiteit> GetEntiteiten(string naam);

        void UpdateGrafieken();

     
        void AddThema(Thema nieuwThema, List<Sleutelwoord> sleutelwoorden, HttpPostedFileBase imageFile);
        void UpdateThema(Thema thema, HttpPostedFileBase ImageFile);
        void DeleteThema(int entiteitsId);
        void DeleteSleutelwoord(int sleutelId);
        IEnumerable<Thema> GetThemas(int platId);
        Thema GetThema(int entiteitsId);
        Sleutelwoord GetSleutelwoord(int sleutelId);
        List<Entiteit> GetEntiteitenVanDeelplatform(int id);
        void DeleteEntiteitenVanDeelplatform(int id);

        Dictionary<string, double> BerekenGrafiekWaarde(Domain.Enum.GrafiekType grafiekType, List<Entiteit> entiteiten, List<string> CijferOpties, string VergelijkOptie);

        List<Entiteit> ZoekEntiteiten(string zoek);
        void ConvertJsonToEntiteit(List<Persoon> jsonEntiteiten);

        void BerekenVasteGrafiekenAlleEntiteiten();
        List<Entiteit> getAlleEntiteiten(bool IncludePosts);
        Dictionary<Entiteit, string> FillEntiteiten();

    }
}
