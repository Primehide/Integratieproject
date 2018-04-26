
using System;
using System.Collections.Generic;
using System.Linq;
using DAL;

using Domain.Account;
using Domain.Entiteit;

namespace BL
{
    public class AccountManager : IAccountManager
    {
        private readonly IAccountRepository repo;

        private IAccountRepository accountRepository;
        private UnitOfWorkManager uowManager;

        public AccountManager()
        {
            repo = new AccountRepository();
        }

        public AccountManager(UnitOfWorkManager uofMgr)
        {
            uowManager = uofMgr;

        }

        public void addUser(Account account)
        {
            initNonExistingRepo(true);
            accountRepository.addUser(account);
            uowManager.Save();
        }
        public void updateUser(Account account)
        {
         
            initNonExistingRepo(true);
            Account oldaccount = new Account();

            oldaccount = accountRepository.ReadAccount(account.IdentityId);
            account.AccountId = oldaccount.AccountId;
         

            accountRepository.updateUser(account);
            uowManager.Save();
        }

        public Account getAccount(string ID)
        {
            return repo.ReadAccount(ID);
        }

        public void genereerAlerts()
        {
            initNonExistingRepo(true);
            EntiteitManager entiteitMgr = new EntiteitManager(uowManager);
            List<Alert> Alerts = getAlleAlerts();
            Entiteit e;
            //1 keer alle trends resetten om vandaag te kunnen kijken of er een trend aanwezig is
            entiteitMgr.ResetTrends();
            foreach (var alert in Alerts)
            {
                e = alert.Entiteit;
                if (entiteitMgr.berekenTrends(alert.MinWaarde, e, alert.TrendType, alert.Voorwaarde))
                {
                    alert.Triggered = true;
                    UpdateAlert(alert);
                }
            }
            throw new NotImplementedException();
        }

        public void UpdateAlert(Alert alert)
        {
            initNonExistingRepo();
            accountRepository.UpdateAlert(alert);
        }

        public List<Alert> getAlleAlerts()
        {
            initNonExistingRepo();
            return accountRepository.getAlleAlerts();
        }

        public void initNonExistingRepo(bool withUnitOfWork = false)
        {
            // Als we een repo met UoW willen gebruiken en als er nog geen uowManager bestaat:
            // Dan maken we de uowManager aan en gebruiken we de context daaruit om de repo aan te maken.

            if (withUnitOfWork)
            {
                if (uowManager == null)
                {
                    uowManager = new UnitOfWorkManager();
                    accountRepository = new AccountRepository(uowManager.UnitOfWork);
                }
            }
            // Als we niet met UoW willen werken, dan maken we een repo aan als die nog niet bestaat.
            else
            {
                accountRepository = (accountRepository == null) ? new AccountRepository() : accountRepository;
            }
        }

        public void grafiekAanGebruikerToevoegen(string IdentityId, Domain.Enum.GrafiekType TypeGrafiek, List<int> entiteitInts, List<string> CijferOpties, string VergelijkOptie, Domain.Enum.GrafiekSoort grafiekSoort)
        {
            initNonExistingRepo(true);
            //IPostManager postManager = new PostManager(uowManager);
            IEntiteitManager entiteitManager = new EntiteitManager(uowManager);
            Domain.Account.Account user = accountRepository.ReadAccount(IdentityId);
            Domain.Post.Grafiek grafiek = new Domain.Post.Grafiek();

            List<Entiteit> entiteiten = new List<Entiteit>();
            foreach (var i in entiteitInts)
            {
                var e = entiteitManager.getAlleEntiteiten().Single(x => x.EntiteitId == i);
                entiteiten.Add(e);
            }

            Dictionary<string, double> waardes = entiteitManager.BerekenGrafiekWaarde(TypeGrafiek,entiteiten,CijferOpties, VergelijkOptie);
            List<Domain.Post.GrafiekWaarde> grafiekWaardes = new List<Domain.Post.GrafiekWaarde>();
            
            foreach (var item in waardes)
            {
                Domain.Post.GrafiekWaarde w = new Domain.Post.GrafiekWaarde()
                {
                    Naam = item.Key,
                    Waarde = item.Value
                };
                grafiekWaardes.Add(w);
            }

            grafiek.Type = TypeGrafiek;
            grafiek.Waardes = grafiekWaardes;
            grafiek.GrafiekSoort = grafiekSoort;
            if(VergelijkOptie.ToLower() == "populariteit")
            {
                grafiek.soortGegevens = Domain.Enum.SoortGegevens.POPULARITEIT;
            } else if(VergelijkOptie.ToLower() == "postfrequentie")
            {
                grafiek.soortGegevens = Domain.Enum.SoortGegevens.POSTFREQUENTIE;
            }


            //cijfers
            switch (TypeGrafiek)
            {
                case Domain.Enum.GrafiekType.CIJFERS:
                    grafiek.Naam = entiteiten.First().Naam;
                    break;
            }

            Domain.Account.DashboardBlok dashboardBlok = new Domain.Account.DashboardBlok()
            {
                Grafiek = grafiek
            };

            if (user.Dashboard.Configuratie.DashboardBlokken == null)
                user.Dashboard.Configuratie.DashboardBlokken = new List<DashboardBlok>();
            user.Dashboard.Configuratie.DashboardBlokken.Add(dashboardBlok);
            accountRepository.updateUser(user);
            uowManager.Save();
        }
    }
}
