﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Domain.Account;
using Domain.Entiteit;
using Domain.Enum;
using Domain.Post;

namespace BL
{
    public interface IAccountManager
    {
        void addUser(Account account);
        void genereerAlerts();
        List<Alert> getAlleAlerts();
        List<Alert> GetUserAlerts(string accountID);
        Alert GetAlert(int alertid);
        void UpdateAlert(Alert alert);
        void AddAlert(Alert alert, int entiteitId, bool web, bool android, bool mail);
        void DeleteAlert(int alertID);
        Account getAccount(string ID);
        Account getAccount(int ID);
        void DeleteGrafiekWaardes(int grafiekID);
        void grafiekAanGebruikerToevoegen(string IdentityId, Domain.Enum.GrafiekType TypeGrafiek, List<int> entiteitInts, List<string> CijferOpties, string vergelijkOptie, Domain.Enum.GrafiekSoort grafiekSoort);
        List<Account> GetAccounts();
        void UpdateUser(Account account);
        void DeleteUser(string accountId);
        void FollowEntity(string identityID, int entiteitID);
        void UnfollowEntity(string identityID, int entiteitID);
        void AddUserGrafiek(List<CijferOpties> opties, List<int> entiteitIds, Domain.Enum.GrafiekType grafiekType, int platId, string IdentityId, string naam, GrafiekSoort grafiekSoort);
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
        void updateUser(Account account);
    }
}
