using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class AccountRepository : IAccountRepository
    {
        private EFContext ctx;

        public AccountRepository()
        {
            ctx = new EFContext();
        }
    }
}
