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

        public void CreatePerson(Persoon p)
        {
            ctx.Entiteiten.Add(p);
            ctx.Personen.Add(p);
            ctx.SaveChanges();

        }

        public void DeletePerson(int id)
        {
            ctx.Personen.Remove(ReadPerson(id));
            ctx.SaveChanges();
            }

        public Persoon ReadPerson(int id)
        {
            return ctx.Personen.First(x => x.EntiteitId == id);
        }

        public Persoon UpdatePerson(Persoon UpdatedPerson)
        {
            Persoon toUpdated = ctx.Personen.First(x => x.PersonId == UpdatedPerson.PersonId);
            toUpdated = UpdatedPerson;
            ctx.SaveChanges();
            return UpdatedPerson;
        }
    }
}
