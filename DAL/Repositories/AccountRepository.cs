using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Account;
using Domain.Entiteit;

namespace DAL
{


    public class AccountRepository : IAccountRepository
    {

        private EFContext ctx;

        public AccountRepository()
        {
            ctx = new EFContext();
        }

        public AccountRepository(UnitOfWork uow)
        {
            ctx = uow.Context;
            ctx.SetUoWBool(true);
        }

        public void addUser(Account account)
        {
            ctx.Accounts.Add(account);
            ctx.SaveChanges();
        }

        public List<Alert> getAlleAlerts()
        {
            return ctx.Alerts.ToList();
        }

        public void UpdateAlert(Alert alert)
        {
            ctx.Entry(alert).State = System.Data.Entity.EntityState.Modified;
            ctx.SaveChanges();
        }

        public void updateUser(Account account)
        {
            Account updated = ctx.Accounts.Find(account.AccountId);
            updated.Voornaam = account.Voornaam;
            updated.Achternaam = account.Achternaam;
            updated.GeboorteDatum = account.GeboorteDatum;
            updated.Email = account.Email;
            ctx.SaveChanges();
        }

        public Account ReadAccount(string ID)
        {           
            Account account = ctx.Accounts.Include("Dashboard").Include("Alerts").Where(a => a.IdentityId == ID).First();
            return account;
        }

        public List<Account> readAccounts()
        {
            return ctx.Accounts.ToList();
        }

        public void DeleteUser(string accountId)
        {
            Account account = ReadAccount(accountId);
            ctx.Dashboards.Remove(account.Dashboard);
 
            if (account.Alerts != null)
            {
                foreach (Alert alert in account.Alerts.ToList())
                {
                    ctx.Alerts.Remove(alert);
                }
                account.Alerts = null;
            }
            ctx.SaveChanges();
            ctx.Accounts.Remove(account);
            ctx.SaveChanges();

        }

        public void FollowEntiteit(string accountId, Entiteit entiteit)
        {
            Account updated = ReadAccount(accountId);
            if (updated.Entiteiten == null) { 
            updated.Entiteiten = new List<Entiteit>();
             }
            updated.Entiteiten.Add(entiteit);            
            ctx.SaveChanges();
        }
    }
}
