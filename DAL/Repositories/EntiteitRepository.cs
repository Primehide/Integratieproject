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

        public IEnumerable<Persoon> ReadlAllPeople()
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

        #region
        public void CreateOrganisatie(Organisatie o)
        {
            ctx.Organisaties.Add(o);
            ctx.SaveChanges();
        }


        public Organisatie UpdateOrganisatie(Organisatie UpdatedOrganisatie)
        {
            Organisatie toUpdate = ctx.Organisaties.First(x => x.EntiteitId == UpdatedOrganisatie.EntiteitId);
            toUpdate.Naam = UpdatedOrganisatie.Naam;
            //toUpdate.Leden = UpdatedOrganisatie.Leden;
            toUpdate.Gemeente = UpdatedOrganisatie.Gemeente;
            toUpdate.Posts = UpdatedOrganisatie.Posts;
            toUpdate.Trends = UpdatedOrganisatie.Trends;
            toUpdate.AantalLeden = UpdatedOrganisatie.Leden.Count();
            ctx.SaveChanges();
            return toUpdate;

        }


        public Organisatie ReadOrganisatie(int id)
        {
            return ctx.Organisaties.Where(obj => obj.EntiteitId == id).First();
        }


        public IEnumerable<Organisatie> ReadAllOrganisaties()
        {
            return ctx.Organisaties.ToList();
        }

        public void DeleteOrganisatie(int id)
        {
            ctx.Organisaties.Remove(ReadOrganisatie(id));
            ctx.SaveChanges();
        }
        #endregion
    }
}
