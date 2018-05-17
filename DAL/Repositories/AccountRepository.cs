using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Account;
using Domain.Entiteit;
using System.Data.Entity;

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
        public Alert ReadAlert(int alertID)
        {
            Alert alert = ctx.Alerts.Find(alertID);
            return alert;
        }
        public List<Alert> getAlleAlerts()
        {
            return ctx.Alerts.Include(x => x.Account)
                               .Include(x => x.Entiteit)
                
                .ToList();
        }

        public void AddAlert(Alert alert)
        {
            ctx.Alerts.Add(alert);
            ctx.Entry(alert.Entiteit).State = EntityState.Unchanged;
            ctx.SaveChanges();
        }

        public void UpdateAlert(Alert alert)
        {

            ctx.Entry(alert.Entiteit).State = System.Data.Entity.EntityState.Modified;
            ctx.Entry(alert).State = System.Data.Entity.EntityState.Modified;
            ctx.SaveChanges();
            
        }
        public void DeleteAlert(int alertID)
        {
            Alert alert = ReadAlert(alertID);
         
   
                    ctx.Alerts.Remove(alert);
            
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
            //Account account = ctx.Accounts.Include("Dashboard").Include("Alerts").Include("Items").Where(a => a.IdentityId == ID).First();
            Account account = ctx.Accounts
                .Include(x => x.Dashboard)
                .Include("Alerts")
                .Include("Items")
                .Include(x => x.Dashboard.Configuratie)
                .Include(x => x.Dashboard.Configuratie.DashboardBlokken)
                .Include(x => x.Dashboard.Configuratie.DashboardBlokken.Select(y => y.Grafiek))
                .Include(x => x.Dashboard.Configuratie.DashboardBlokken.Select(y => y.Grafiek).Select(z => z.Waardes))
                .Where(a => a.IdentityId == ID).First();
            return account;
        }

        public List<Account> readAccounts()
        {
           
            return ctx.Accounts
                .Include(x => x.Alerts)
                .Include(x => x.Items)
                .ToList();
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

        public void UnFollowEntiteit(string accountId, int EntiteitID)
        {
            int ItemId = 0;
            Account updated = ReadAccount(accountId);
            List<Item> items = updated.Items;
            foreach(Item item in items)
            {
               if(item.EntiteitId == EntiteitID)
                {
                    ItemId = item.ItemId;
                }
            }
            Item ItemToUnfollow = ctx.Items.SingleOrDefault(p => p.ItemId == ItemId);          
            updated.Items.Remove(ItemToUnfollow);
            ctx.Items.Remove(ItemToUnfollow);
            ctx.SaveChanges();
        }
    }
}
