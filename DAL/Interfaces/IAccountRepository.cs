using Domain.Account;
using Domain.Entiteit;
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
        Account ReadAccount(string ID);
        List<Account> readAccounts();
        void DeleteUser(string accountId);
        void FollowEntiteit(string accountId, int entiteitID);
        void UnFollowEntiteit(string accountId, int EntiteitID);
        void addFaq(Faq faq);
        void UpdateFaq(Faq faq);
        void DeleteFaq(int FaqID);

    }
}
