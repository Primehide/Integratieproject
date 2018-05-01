using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Account;
<<<<<<< HEAD
using Domain.Entiteit;
=======
using System.Data.Entity;
>>>>>>> upstream/master

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
<<<<<<< HEAD
        {           
            Account account = ctx.Accounts.Include("Dashboard").Include("Alerts").Include("Items").Where(a => a.IdentityId == ID).First();
=======
        {
            Account account = ctx.Accounts
                .Include(x => x.Dashboard)
                .Include(x => x.Dashboard.Configuratie)
                .Include(x => x.Dashboard.Configuratie.DashboardBlokken)
                .Include(x => x.Dashboard.Configuratie.DashboardBlokken.Select(y => y.Grafiek))
                .Include(x => x.Dashboard.Configuratie.DashboardBlokken.Select(y => y.Grafiek).Select(z => z.Waardes))
                .Where(a => a.IdentityId == ID).First();
>>>>>>> upstream/master
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

        public void FollowEntiteit(string accountId, int entiteitID)
        {
            Account updated = ReadAccount(accountId);
            Item item = new Item(entiteitID);
            updated.Items.Add(item);
            ctx.Items.Add(item);
            ctx.SaveChanges();
        }
    }
}
