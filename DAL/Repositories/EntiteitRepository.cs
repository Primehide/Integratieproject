using Domain.Entiteit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class EntiteitRepository : IEntiteitRepository
    {
        private EFContext ctx;

        public EntiteitRepository()
        {
            ctx = new EFContext();
        }

        public Thema CreateThema(Thema thema)
        {
            ctx.Themas.Add(thema);
            ctx.SaveChanges();
            return thema;
        }

        public void UpdateThema(Thema thema)
        {
            ctx.Entry(thema).State = System.Data.Entity.EntityState.Modified;
            ctx.SaveChanges();
        }

        public void DeleteThema(int entiteitsId)
        {
            Thema thema = ctx.Themas.Find(entiteitsId);
            ctx.Themas.Remove(thema);
            ctx.SaveChanges();
        }

        public IEnumerable<Thema> ReadThemas()
        {
            throw new NotImplementedException();
        }
    }
}
