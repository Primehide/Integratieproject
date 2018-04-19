using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entiteit;
using System.Data.Entity;

namespace DAL
{
    public class EntiteitRepository : IEntiteitRepository
    {
        private EFContext ctx;

        public EntiteitRepository()
        {
            ctx = new EFContext();
        }

        public void AddEntiteit(Domain.Entiteit.Entiteit entiteit)
        {
            ctx.Entiteiten.Add(entiteit);
            ctx.SaveChanges();
        }

        public List<Entiteit> getAlleEntiteiten()
        {
            return ctx.Entiteiten.Include(x => x.Posts).Include(x => x.Trends).ToList();
        }

        public void updateEntiteit(Entiteit entiteit)
        {
            ctx.Entry(entiteit).State = EntityState.Modified;
            ctx.SaveChanges();
        }

        public EntiteitRepository(UnitOfWork uow)
        {
            ctx = uow.Context;
            ctx.SetUoWBool(true);
        }
    }
}
