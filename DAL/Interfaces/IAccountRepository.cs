using Domain.Account;
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
        void AddAlert(Alert alert);
        void UpdateAlert(Alert alert);
        void updateUser(Domain.Account.Account account);
        Account ReadAccount(string ID);
    }
}
