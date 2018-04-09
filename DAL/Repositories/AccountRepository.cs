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
    }
}
