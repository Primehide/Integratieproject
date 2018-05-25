using System;
using System.Collections.Generic;
using Domain.Account;
using Domain.Enum;
using Domain.Post;

namespace BL.Interfaces
{
    public interface IAccountManager
    {
        void AddUser(Account account);
        void GenereerAlerts();
        List<Alert> GetAlleAlerts();
        List<Alert> GetUserAlerts(string accountId);
        Alert GetAlert(int alertid);
        void UpdateAlert(Alert alert);
        void AddAlert(Alert alert, int entiteitId, bool web, bool android, bool mail);
        void DeleteAlert(int alertId);
        Account GetAccount(string id);
        Account GetAccount(int id);
        void DeleteGrafiekWaardes(int grafiekId);
        void GrafiekAanGebruikerToevoegen(string identityId, GrafiekType typeGrafiek, List<int> entiteitInts, List<string> cijferOpties, string vergelijkOptie, GrafiekSoort grafiekSoort);
        List<Account> GetAccounts();
        void UpdateUser(Account account);
        void DeleteUser(string accountId);
        void FollowEntity(string identityId, int entiteitId);
        void UnfollowEntity(string identityId, int entiteitId);
        void AddUserGrafiek(List<CijferOpties> opties, List<int> entiteitIds, GrafiekType grafiekType, int platId, string identityId, string naam, GrafiekSoort grafiekSoort);
        void UpdateGrafiek(int grafiekId);
        void DeleteDashboardBlok(Account account, int positie);
        void UpdateLocatie(int blokId, int locatie);
        void UpdateSize(int blokId, BlokGrootte blokGrootte);
        void UpdateTitel(int blokId, String titel);
        void UpdateSizeDimensions(int blokId, int x, int y);
        Dashboard GetPublicDashboard(int id);
        void UpdateConfiguratieTitle(int configuratieId, String titel);
        void SetPublic(int dashboardId, bool shared);

        //refactor sander
        List<CijferOpties> CreateCijferOpties(List<string> stringOpties);
        void CreateDomainUser(string identityId, string email, string voornaam, string achternaam,
            DateTime geboorteDatum);
    }
}
