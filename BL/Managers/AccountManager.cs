﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace BL
{
    public class AccountManager : IAccountManager
    {
        private IAccountRepository accountRepository;

        public AccountManager()
        {
            accountRepository = new AccountRepository();
        }
    }
}
