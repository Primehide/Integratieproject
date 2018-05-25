
using Domain.Platform;
﻿using Domain.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BL
{
    public interface IPlatformManager
    {
        void AddDeelplatform(Deelplatform newPlatform);
        Deelplatform GetDeelplatform(int platformId);
        Deelplatform ChangeDeelplatform(Deelplatform changedDeelplatform, HttpPostedFileBase imgLogo);
        IEnumerable<Deelplatform> GetAllDeelplatformen();
        void RemoveDeelplatform(int platformId);
        StringBuilder ConvertToCsv(List<Account> accounts);
    }
}
