
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

        //refactor sander
        void AddFaq(Domain.Platform.Faq faq, int platId);
        void DeleteFaq(int id);
        List<Faq> GetAlleFaqs(int PlatId);
    }
}
