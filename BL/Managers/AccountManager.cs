
using System;
using System.Collections.Generic;
using DAL;

using Domain.Account;

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
