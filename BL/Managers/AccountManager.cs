using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Domain.Account;

namespace BL
{
    public class AccountManager : IAccountManager
    {
        private IAccountRepository accountRepository;
        private UnitOfWorkManager uowManager;

        public AccountManager()
        {

        }

        public AccountManager(UnitOfWorkManager uofMgr)
        {
            uowManager = uofMgr;
            accountRepository = new AccountRepository();
        }

        public void addUser(Account account)
        {
            initNonExistingRepo(false);
            accountRepository.addUser(account);
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
