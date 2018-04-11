using Domain.Entiteit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
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

        public IEnumerable<Persoon> ReadAllPeople()
        {
            return ctx.Personen.Include(p => p.Organisations).ToList();
        }

        public Persoon ReadPerson(int id)
        {
            return ctx.Personen.Where(obj => obj.EntiteitId == id).Include(p => p.Organisations).First();
        }

        public Persoon UpdatePerson(Persoon UpdatedPerson)
        {
            Persoon toUpdated = ctx.Personen.Include(org => org.Organisations).Where(x => x.EntiteitId == UpdatedPerson.EntiteitId).FirstOrDefault();

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
            Organisatie toUpdate = ctx.Organisaties.Where(x => x.EntiteitId == UpdatedOrganisatie.EntiteitId).FirstOrDefault();
            toUpdate.Naam = UpdatedOrganisatie.Naam;
            toUpdate.Gemeente = UpdatedOrganisatie.Gemeente;
            toUpdate.Posts = UpdatedOrganisatie.Posts;
            toUpdate.Trends = UpdatedOrganisatie.Trends;
            ctx.SaveChanges();
            return toUpdate;

        }


        public Organisatie ReadOrganisatie(int id)
        {
            return ctx.Organisaties.Where(obj => obj.EntiteitId == id).Include(o => o.Leden).First();
        }


        public IEnumerable<Organisatie> ReadAllOrganisaties()
        {
            return ctx.Organisaties.Include(o => o.Leden).ToList();
        }

        public void DeleteOrganisatie(int id)
        {
            ctx.Organisaties.Remove(ReadOrganisatie(id));
            ctx.SaveChanges();
        }

        public Organisatie UpdateOrganisatie(Organisatie UpdatedOrganisatie, IEnumerable<string> SelectedPeople)
        {
            Organisatie toUpdate = ctx.Organisaties.Include(l => l.Leden).Where(x => x.EntiteitId == UpdatedOrganisatie.EntiteitId).FirstOrDefault();


            List<Persoon> NewlyAppointedPeople = new List<Persoon>();
            //Bestaande referenties verwijderen
            if (toUpdate.Leden != null)
            {
                toUpdate.Leden = new List<Persoon>();
                foreach (Persoon p in toUpdate.Leden)
                {
                    p.Organisations.Remove(toUpdate);
                }
            }
            //Nieuwe referenties toevoegen
            foreach (string pId in SelectedPeople)
            {
                Persoon person = ReadPerson(Int32.Parse(pId));
                person.Organisations.Add(UpdatedOrganisatie);
                toUpdate.Leden.Add(person);
            }

            toUpdate.Naam = UpdatedOrganisatie.Naam;
            toUpdate.Gemeente = UpdatedOrganisatie.Gemeente;
            toUpdate.Posts = UpdatedOrganisatie.Posts;
            toUpdate.Trends = UpdatedOrganisatie.Trends;

            toUpdate.AantalLeden = toUpdate.Leden.Count();

            ctx.Entry(toUpdate).State = EntityState.Modified;

            ctx.SaveChanges();
            return toUpdate;
        }

        public void UpdatePerson(Persoon UpdatedPerson, IEnumerable<string> selectedOrganisations)
        {
            Persoon toUpdated = ctx.Personen.Include(org => org.Organisations).Where(x => x.EntiteitId == UpdatedPerson.EntiteitId).FirstOrDefault();

            //Remove all references
            toUpdated.Organisations = new List<Organisatie>();

            //Add new References
            foreach (string oId in selectedOrganisations)
            {
                toUpdated.Organisations.Add(ReadOrganisatie(Int32.Parse(oId)));
            }

            toUpdated.FirstName = UpdatedPerson.FirstName;
            toUpdated.LastName = UpdatedPerson.LastName;

            ctx.SaveChanges();
        }
        #endregion
    }
}
