using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DAL.Interfaces;
using Domain.Platform;

namespace DAL.Repositories
{
    public class PlatformRepository : IPlatformRepository
    {
        private readonly EFContext _ctx;

        public PlatformRepository()
        {
            _ctx = new EFContext();
        }

        public PlatformRepository(UnitOfWork uow)
        {
            _ctx = uow.Context;
            _ctx.SetUoWBool(true);
        }

        public void CreateDeelplatform(Deelplatform d)
        {
            _ctx.DeelPlatformen.Add(d);
            _ctx.SaveChanges();
        }

        public void DeleteDeelplatform(int id)
        {
            _ctx.DeelPlatformen.Remove(ReadDeelplatform(id));
        }

        public Deelplatform ReadDeelplatform(int id)
        {
            return _ctx.DeelPlatformen.Include("Entiteiten").FirstOrDefault(x => x.DeelplatformId == id);
        }

        public IEnumerable<Deelplatform> ReadAllDeelplatformen()
        {
            return _ctx.DeelPlatformen.Include("Entiteiten").ToList();

        }

        public Deelplatform UpdateDeelplatform(Deelplatform uDeelplatform)
        {
            Deelplatform toChange = ReadDeelplatform(uDeelplatform.DeelplatformId);

            if (toChange != null)
            {
                toChange.Naam = uDeelplatform.Naam;
                toChange.Tagline = uDeelplatform.Tagline;
                toChange.Entiteiten = uDeelplatform.Entiteiten;
                _ctx.Entry(toChange).State = EntityState.Modified;
            }
            _ctx.SaveChanges();
            return toChange;
        }

        public void AddFaq(Faq faq)
        {
            _ctx.Faqs.Add(faq);
            _ctx.SaveChanges();
        }

        public void UpdateFaq(Faq faq)
        {
            _ctx.Entry(faq).State = EntityState.Modified;
            _ctx.SaveChanges();
        }

        public void DeleteFaq(int faqId)
        {
            _ctx.Faqs.Remove(_ctx.Faqs.Find(faqId) ?? throw new InvalidOperationException());
            _ctx.SaveChanges();
        }

        public List<Faq> GetAlleFaqs(int platId)
        {
            return _ctx.Faqs.Where(x => x.PlatformId == platId).ToList();
        }
    }
}
