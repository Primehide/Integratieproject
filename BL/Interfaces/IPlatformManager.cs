
using Domain.Platform;
﻿using Domain.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IPlatformManager
    {
        void AddDeelplatform(Deelplatform newPlatform);
        Deelplatform GetDeelplatform(int platformId);
        Deelplatform ChangeDeelplatform(Deelplatform changedDeelplatform);
        IEnumerable<Deelplatform> GetAllDeelplatformen();
        void RemoveDeelplatform(int platformId);
        StringBuilder ConvertToCSV(List<Account> accounts);
    }
}
