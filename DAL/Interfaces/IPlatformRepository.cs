using System.Collections.Generic;
using Domain.Platform;

namespace DAL.Interfaces
{
    public interface IPlatformRepository
    {
        void CreateDeelplatform(Deelplatform d);
        Deelplatform ReadDeelplatform(int id);
        IEnumerable<Deelplatform> ReadAllDeelplatformen();
        Deelplatform UpdateDeelplatform(Deelplatform uDeelplatform);
        void DeleteDeelplatform(int id);

        //refactor sander
        void AddFaq(Faq faq);
        void UpdateFaq(Faq faq);
        void DeleteFaq(int faqId);
        List<Faq> GetAlleFaqs(int platId);
    }
}
