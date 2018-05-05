using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Domain.Account;
using Domain.Entiteit;

namespace BL
{
    public interface IAccountManager
    {
        void addUser(Account account);
        void genereerAlerts();
        List<Alert> getAlleAlerts();
        void UpdateAlert(Alert alert);
        Account getAccount(string ID);
        void updateUser(Account account);
        void grafiekAanGebruikerToevoegen(string IdentityId, Domain.Enum.GrafiekType TypeGrafiek, List<int> entiteitInts, List<string> CijferOpties, string vergelijkOptie, Domain.Enum.GrafiekSoort grafiekSoort);
        List<Account> GetAccounts();
        void UpdateUser(Account account);
        void DeleteUser(string accountId);
        void FollowEntity(string identityID, int entiteitID);
        void UnfollowEntity(string identityID, int entiteitID);
    }
}
