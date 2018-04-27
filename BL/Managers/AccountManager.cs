
using System;
using System.Collections.Generic;
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
            initNonExistingRepo(true);
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
        public void addAlert(Alert alert)
        {
            initNonExistingRepo(true);
            accountRepository.AddAlert(alert);
            uowManager.Save();
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
    }
}
