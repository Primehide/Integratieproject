using System.Collections.Generic;
using System.Text;
using Domain.Account;
using Domain.Platform;

namespace BL.Interfaces
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
        void AddFaq(Faq faq, int platId);
        void DeleteFaq(int id);
        List<Faq> GetAlleFaqs(int platId);
    }
}
