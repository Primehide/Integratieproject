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
        void UpdateAlert(Alert alert);
        void updateUser(Domain.Account.Account account);
        Account ReadAccount(string ID);
        List<Account> readAccounts();
        void DeleteUser(string accountId);
        void FollowEntiteit(string accountId, int entiteitID);
        void UnFollowEntiteit(string accountId, int EntiteitID);
    }
}
