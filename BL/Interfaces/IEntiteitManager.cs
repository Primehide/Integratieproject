using System;
using System.Collections.Generic;
using System.Web;
using Domain.Entiteit;
using Domain.Post;

namespace BL.Interfaces
{
    public interface IEntiteitManager
    {
        void AddPerson(Persoon p, HttpPostedFileBase imageFile);
        Persoon GetPerson(int id);
        Persoon ChangePerson(Persoon changedPerson, HttpPostedFileBase imageFile);
        void RemovePerson(int id);
        List<Persoon> GetAllPeople(int platId);
        byte[] GetPersonImageFromDataBase(int id);
        void AddOrganisatie(Organisatie o, HttpPostedFileBase imageFile);
        Organisatie ChangeOrganisatie(Organisatie changedOrganisatie, HttpPostedFileBase imageFile);
        List<Organisatie> GetAllOrganisaties(int platId);
        Organisatie GetOrganisatie(int id);
        void RemoveOrganisatie(int id);
        byte[] GetOrganisationImageFromDataBase(int id);
        void CreateTestData();
        List<Entiteit> GetAlleEntiteiten();
        void UpdateEntiteit(Entiteit entiteit);
        Entiteit GetEntiteit(int id);
        List<Entiteit> GetEntiteiten(string naam);
        void UpdateGrafieken();
        void AddThema(Thema nieuwThema, List<Sleutelwoord> sleutelwoorden, HttpPostedFileBase imageFile);
        void UpdateThema(Thema thema, HttpPostedFileBase imageFile);
        void DeleteThema(int entiteitsId);
        void DeleteSleutelwoord(int sleutelId);
        IEnumerable<Thema> GetThemas(int platId);
        Thema GetThema(int entiteitsId);
        Sleutelwoord GetSleutelwoord(int sleutelId);
        List<Entiteit> GetEntiteitenVanDeelplatform(int id);
        void DeleteEntiteitenVanDeelplatform(int id);
        Dictionary<string, double> BerekenGrafiekWaarde(Domain.Enum.GrafiekType grafiekType, List<Entiteit> entiteiten, List<string> cijferOpties, string vergelijkOptie);
        List<Entiteit> ZoekEntiteiten(string zoek);
        void ConvertJsonToEntiteit(List<Persoon> jsonEntiteiten);
        void BerekenVasteGrafiekenAlleEntiteiten();
        List<Entiteit> GetAlleEntiteiten(bool includePosts);

        Thema AddSleutelWoordenToThema(Thema thema, List<Sleutelwoord> sleutelwoorden);
        List<Sleutelwoord> AddSleutelWoordenToLijst(List<Sleutelwoord> sleutelWoorden);
        Dictionary<String, int> GetSentimenten(List<Post> posts);
        List<Sleutelwoord> GetSleutelwoorden(string woorden);
        void Upload(HttpPostedFileBase file);
    }
}
