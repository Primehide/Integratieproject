using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Domain.Entiteit;

namespace DAL
{
    public class EntiteitRepository : IEntiteitRepository
    {
        private EFContext ctx;
        private DbModelBuilder modelBuilder;
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
            return ctx.Entiteiten.Include(x => x.Posts).ToList();
        }

        public void updateEntiteit(Entiteit entiteit)
        {
            ctx.Entry(entiteit).State = EntityState.Modified;
            ctx.SaveChanges();
        }

        public void CreateThema(Thema thema)
        {
            ctx.Themas.Add(thema);
            ctx.SaveChanges();
        }

        public void UpdateThema(Thema thema)
        {
            var result = ctx.Themas.SingleOrDefault(b => b.EntiteitId == thema.EntiteitId);
            if (result != null)
            {
                result.Naam = thema.Naam;
                ctx.SaveChanges();
            }
        }

        public void DeleteThema(int entiteitsId)
        {
            //Thema thema = ctx.Themas.Find(entiteitsId);

            var thema = ctx.Themas.Include(b => b.SleutenWoorden).FirstOrDefault(b => b.EntiteitId == entiteitsId);
            var woorden = ctx.SleutelWoorden.All(b => b.SleutelwoordId == entiteitsId);
            ctx.Themas.Remove(thema);
            ctx.SaveChanges();
        }

        public Thema ReadThema(int entiteitsId)
        {
            Thema thema = ctx.Themas.Find(entiteitsId);
            return thema;
        }

        public IEnumerable<Thema> ReadThemas()
        {
            return ctx.Themas.ToList();
        }

        public EntiteitRepository(UnitOfWork uow)
        {
            ctx = uow.Context;
            ctx.SetUoWBool(true);
        }

       
    }
}
