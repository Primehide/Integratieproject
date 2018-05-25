using System;
using System.Collections.Generic;
using Domain.Account;
using Domain.Enum;

namespace DAL.Interfaces
{
    public interface IAccountRepository
    {
        void AddUser(Account account);
        List<Alert> GetAlleAlerts();
        Alert ReadAlert(int alertId);
        void UpdateAlert(Alert alert);
        void AddAlert(Alert alert);
        void DeleteAlert(int alertId);
        void UpdateUser(Account account);
        void DeleteGrafiekWaardes(int grafiekId);
        Account ReadAccount(string id);
        Account ReadAccount(int id);
        List<Account> ReadAccounts();
        void AddDeviceId(string userId, string device);
        void DeleteUser(string accountId);
        void FollowEntiteit(string accountId, int entiteitId);
        void UnFollowEntiteit(string accountId, int entiteitId);
        void DeleteDashboardBlok(Account account, int id);
        void UpdateLocatie(int blokId, int locatie);
        void UpdateSize(int blokId, BlokGrootte blokGrootte);
        void UpdateTitel(int blokId, String titel);
        void UpdateSizeDimensions(int blokId, int x, int y);
        Dashboard GetPublicDashboard(int id);
        void UpdateConfiguratieTitle(int configuratieId, String title);
        void SetPublic(int dashboardId, bool shared);

    }
}
