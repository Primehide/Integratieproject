using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Account;

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
        public void updateUser(Account account  )
        {
            Account updated = ctx.Accounts.Find(account.AccountId);
            updated.Voornaam = account.Voornaam;
            updated.Achternaam = account.Achternaam;
            updated.GeboorteDatum = account.GeboorteDatum;
            //    updated.Email = account.Email;




            ctx.SaveChanges();


        }

        public Account ReadAccount(string ID)
        {
            string test = ID;
            Account account = ctx.Accounts.Where(a => a.IdentityId == ID).First();
            return account;
        }
    }
}
