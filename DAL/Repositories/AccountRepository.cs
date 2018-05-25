using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DAL.Interfaces;
using Domain.Account;
using Domain.Entiteit;
using Domain.Enum;
using Domain.Post;

namespace DAL.Repositories
{


    public class AccountRepository : IAccountRepository
    {

        private readonly EFContext _ctx;

        public AccountRepository()
        {
            _ctx = new EFContext();
            _ctx.Database.CommandTimeout = 180;
        }

        public AccountRepository(UnitOfWork uow)
        {
            _ctx = uow.Context;
            _ctx.SetUoWBool(true);
            _ctx.Database.CommandTimeout = 180;
        }

        public void AddUser(Account account)
        {
            _ctx.Accounts.Add(account);
            _ctx.SaveChanges();
        }

        public void AddDeviceId(string userId,string device)
        {
            Account acc = _ctx.Accounts.FirstOrDefault(m => m.IdentityId == userId);
            if (acc != null) acc.DeviceId = device;
            _ctx.SaveChanges();
        }

        public Alert ReadAlert(int alertId)
        {
            Alert alert = _ctx.Alerts.Where(x => x.AlertId == alertId)
                .Include(x => x.Entiteit)
                .FirstOrDefault();
            
            return alert;
        }
        public List<Alert> GetAlleAlerts()
        {
            return _ctx.Alerts.Include(x => x.Account)
                               .Include(x => x.Entiteit)
                
                .ToList();
        }

        public void AddAlert(Alert alert)
        {
            _ctx.Alerts.Add(alert);
            _ctx.Entry(alert.Entiteit).State = EntityState.Unchanged;
            _ctx.SaveChanges();
        }
    


        public void UpdateAlert(Alert alert)
        {

            _ctx.Entry(alert.Entiteit).State = EntityState.Modified;
            _ctx.Entry(alert).State = EntityState.Modified;
            _ctx.SaveChanges();
            
        }
        public void DeleteAlert(int alertId)
        {
            Alert alert = ReadAlert(alertId);
         
   
                    _ctx.Alerts.Remove(alert);
            
            _ctx.SaveChanges();
    

        }

        public void UpdateUser(Account account)
        {
            _ctx.Entry(account).State = EntityState.Modified;
            //Account updated = ctx.Accounts.Find(account.AccountId);
            _ctx.SaveChanges();

            /*
            updated.Dashboard = account.Dashboard;
            foreach (DashboardBlok b in updated.Dashboard.Configuratie.DashboardBlokken)
            {
                if (b.Grafiek.Entiteiten != null)
                {
                    foreach (Entiteit e in b.Grafiek.Entiteiten)
                    {
                        ctx.Entry(e).State = EntityState.Modified;
                    }
                }
            }
                ctx.SaveChanges();
                */
        }

        public Account ReadAccount(string id)
        {           
            //Account account = ctx.Accounts.Include("Dashboard").Include("Alerts").Include("Items").Where(a => a.IdentityId == ID).First();
            Account account = _ctx.Accounts
                .Include(x => x.Dashboard)
                .Include(x => x.ReviewEntiteiten.Select(y => y.Posts))
                .Include(x => x.ReviewEntiteiten.Select(y => y.Posts.Select(z => z.Urls)))
                .Include(x => x.ReviewEntiteiten.Select(y => y.Trends))
                .Include("Alerts")
                .Include("Items")
                .Include(x => x.Dashboard.Configuratie)
                .Include(x => x.Dashboard.Configuratie.DashboardBlokken)
                .Include(x => x.Dashboard.Configuratie.DashboardBlokken.Select(y => y.Grafiek))
                .Include(x => x.Dashboard.Configuratie.DashboardBlokken.Select(y => y.Grafiek).Select(z => z.Waardes))
                .Single(a => a.IdentityId == id);
            return account;
        }

        public Account ReadAccount(int id)
        {
            //Account account = ctx.Accounts.Include("Dashboard").Include("Alerts").Include("Items").Where(a => a.IdentityId == ID).First();
            Account account = _ctx.Accounts
                .Include(x => x.Dashboard)
                .Include(x => x.Dashboard.Configuratie)
                .Include(x => x.Dashboard.Configuratie.DashboardBlokken)
                .Include(x => x.Dashboard.Configuratie.DashboardBlokken.Select(y => y.Grafiek))
                .Include(x => x.Dashboard.Configuratie.DashboardBlokken.Select(y => y.Grafiek).Select(z => z.Waardes))
                .Single(a => a.AccountId == id);
            return account;
        }

        public List<Account> ReadAccounts()
        {
           
            return _ctx.Accounts
                .Include(x => x.Alerts)
                .Include(x => x.Items)
                .Include(x => x.ReviewEntiteiten)
                .Include(x => x.Dashboard.Configuratie)
                .Include(x => x.Dashboard)
                .Include(x => x.Dashboard.Configuratie.DashboardBlokken)
                .Include(x => x.Dashboard.Configuratie.DashboardBlokken.Select(y => y.Grafiek))
                .Include(x => x.Dashboard.Configuratie.DashboardBlokken.Select(y => y.Grafiek).Select(z => z.Entiteiten))
                .Include(x => x.Dashboard.Configuratie.DashboardBlokken.Select(y => y.Grafiek).Select(z => z.CijferOpties))
                .ToList();
        }
        
        public void DeleteUser(string accountId)
        {
            Account account = ReadAccount(accountId);
            _ctx.Dashboards.Remove(account.Dashboard);
 
            if (account.Alerts != null)
            {
                foreach (Alert alert in account.Alerts.ToList())
                {
                    _ctx.Alerts.Remove(alert);
                }
                account.Alerts = null;
            }
            _ctx.SaveChanges();
            _ctx.Accounts.Remove(account);
            _ctx.SaveChanges();

        }

        public void FollowEntiteit(string accountId, int entiteitId)
        {
            Account updated = ReadAccount(accountId);
            Item item = new Item(entiteitId);
            updated.Items.Add(item);
            _ctx.Items.Add(item);
            _ctx.SaveChanges();
        }

        public void UnFollowEntiteit(string accountId, int entiteitId)
        {
            int itemId = 0;
            Account updated = ReadAccount(accountId);
            List<Item> items = updated.Items;
            foreach(Item item in items)
            {
               if(item.EntiteitId == entiteitId)
                {
                    itemId = item.ItemId;
                }
            }
            Item itemToUnfollow = _ctx.Items.SingleOrDefault(p => p.ItemId == itemId);          
            updated.Items.Remove(itemToUnfollow);
            _ctx.Items.Remove(itemToUnfollow ?? throw new InvalidOperationException());
            _ctx.SaveChanges();
        }

        public void DeleteGrafiekWaardes(int grafiekId)
        {
            Grafiek teverwijderenwaardes = _ctx.Grafieken.First(o => o.GrafiekId == grafiekId);
            teverwijderenwaardes.Waardes = null;
        }

        public void DeleteDashboardBlok(Account account, int id)
        {
            var blok = _ctx.DashboardBloks.Single(dashBlok => dashBlok.DashboardBlokId == id);

            if (blok != null)
            {
                _ctx.DashboardBloks.Remove(blok);
            }

            _ctx.SaveChanges();
        }

        public void UpdateLocatie(int blokId, int locatie)
        {
            var dashBlok = _ctx.DashboardBloks.Find(blokId);
            if(dashBlok != null) dashBlok.DashboardLocatie = locatie;
            _ctx.SaveChanges();
        }

        public void UpdateSize(int blokId, BlokGrootte blokGrootte)
        {
            var dashBlok = _ctx.DashboardBloks.Find(blokId);
            if (dashBlok != null) dashBlok.BlokGrootte = blokGrootte;
            _ctx.SaveChanges();
        }

        public void UpdateTitel(int blokId, String titel)
        {
            var dashBlok = _ctx.DashboardBloks.Find(blokId);
            if (dashBlok != null) dashBlok.Titel = titel;
            _ctx.SaveChanges();
        }

        public void UpdateSizeDimensions(int blokId, int x, int y)
        {
            var dashBlok = _ctx.DashboardBloks.Find(blokId);
            if (dashBlok != null)
            {
                dashBlok.sizeX = x;
                dashBlok.sizeY = y;
            }

            _ctx.SaveChanges();
        }

        public void UpdateConfiguratieTitle(int configuratieId, String title)
        {
            var config = _ctx.DashboardConfiguraties.Find(configuratieId);
            if (config != null) config.ConfiguratieNaam = title;
            _ctx.SaveChanges();
        }

        public void SetPublic(int dashboardId, bool shared)
        {
            var dashboard = _ctx.Dashboards.Find(dashboardId);
            if (dashboard != null) dashboard.IsPublic = shared;
            _ctx.SaveChanges();
        }

        public Dashboard GetPublicDashboard(int id)
        {
            Dashboard dashboard = _ctx.Dashboards
                .Include(x => x.Configuratie)
                .Include(x => x.Configuratie.DashboardBlokken)
                .Include(x => x.Configuratie.DashboardBlokken.Select(y => y.Grafiek))
                .Include(x => x.Configuratie.DashboardBlokken.Select(y => y.Grafiek).Select(z => z.Waardes)).First(x => x.DashboardId == id);

            if (dashboard.IsPublic)
            {
                return dashboard;
            }
            else
            {
                return null;
            }
        }
    }
}
