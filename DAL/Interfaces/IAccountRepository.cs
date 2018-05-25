using Domain.Account;
using Domain.Entiteit;
using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IAccountRepository
    {
        void addUser(Domain.Account.Account account);
        List<Alert> getAlleAlerts();
        List<Faq> getAlleFaqs();
        Alert ReadAlert(int alertID);
        void UpdateAlert(Alert alert);
        void AddAlert(Alert alert);
        void DeleteAlert(int alertID);
        void updateUser(Domain.Account.Account account);
        void DeleteGrafiekWaardes(int grafiekID);
        Account ReadAccount(string ID);
        Account ReadAccount(int ID);
        List<Account> readAccounts();
        void addDeviceId(string userId, string device);
        void DeleteUser(string accountId);
        void FollowEntiteit(string accountId, int entiteitID);
        void UnFollowEntiteit(string accountId, int EntiteitID);
        void addFaq(Faq faq);
        void UpdateFaq(Faq faq);
        void DeleteFaq(int FaqID);
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
