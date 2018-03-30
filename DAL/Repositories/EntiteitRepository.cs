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
        #region
        public void CreatePerson(Persoon p)
        {
            ctx.Personen.Add(p);
            ctx.SaveChanges();

        }

        public void DeletePerson(int id)
        {
            ctx.Personen.Remove(ReadPerson(id));
            ctx.SaveChanges();
            }

        public List<Persoon> ReadlAllPeople()
        {
            return ctx.Personen.ToList();
        }

        public Persoon ReadPerson(int id)
        {
            return ctx.Personen.First(x => x.EntiteitId == id);
        }

        public Persoon UpdatePerson(Persoon UpdatedPerson)
        {
            Persoon toUpdated = ctx.Personen.First(x => x.EntiteitId == UpdatedPerson.PersonId);
            toUpdated.FirstName = UpdatedPerson.FirstName;
            toUpdated.LastName = UpdatedPerson.LastName;
            ctx.SaveChanges();
            return toUpdated;
        }
#endregion

    }
}
