using Domain.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Platform;

namespace DAL
{
    public class PlatformRepository : IPlatformRepository
    {
        private EFContext ctx;

        public PlatformRepository()
        {
            ctx = new EFContext();
        }

        public PlatformRepository(UnitOfWork uow)
        {
            ctx = uow.Context;
            ctx.SetUoWBool(true);
        }

        public void CreateDeelplatform(Deelplatform d)
        {
            ctx.DeelPlatformen.Add(d);
            ctx.SaveChanges();
        }

        public void DeleteDeelplatform(int id)
        {
            ctx.DeelPlatformen.Remove(ReadDeelplatform(id)); ;
        }

        public Deelplatform ReadDeelplatform(int id)
        {
            return ctx.DeelPlatformen.Include("Entiteiten").Where(x => x.DeelplatformId == id).FirstOrDefault();
        }

        public IEnumerable<Deelplatform> ReadAllDeelplatformen()
        {
            return ctx.DeelPlatformen.Include("Entiteiten").ToList();

        }

        public Deelplatform UpdateDeelplatform(Deelplatform uDeelplatform)
        {
            Deelplatform toChange = ReadDeelplatform(uDeelplatform.DeelplatformId);

            if (toChange != null)
            {
                toChange.Naam = uDeelplatform.Naam;
                toChange.Entiteiten = uDeelplatform.Entiteiten;
            }
            ctx.Entry(toChange).State = System.Data.Entity.EntityState.Modified;
            ctx.SaveChanges();
            return toChange;
        }
    }
}
