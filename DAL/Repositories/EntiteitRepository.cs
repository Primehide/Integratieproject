using Domain.Entiteit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
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

        public byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;

        }

        #region

        public void CreatePersonWithoutPhoto(Persoon p)
        {
            ctx.Personen.Add(p);
            ctx.SaveChanges();

        }

        public void CreatePersonWithPhoto(Persoon p, HttpPostedFileBase ImageFile)
        {
            p.Image = ConvertToBytes(ImageFile);
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
            ctx.Entry(UpdatedPerson).State = EntityState.Modified;
            ctx.SaveChanges();
            return UpdatedPerson;
        }

        public byte[] GetPersonImageFromDataBase(int Id)

        {

            var q = from temp in ctx.Personen where temp.EntiteitId == Id select temp.Image;

            byte[] cover = q.First();

            return cover;

        }
        #endregion

        #region

        public void CreateOrganisatieWithoutPhoto(Organisatie o)
        {
            ctx.Organisaties.Add(o);
            ctx.SaveChanges();
        }

        public void CreateOrganisatieWithPhoto(Organisatie o, HttpPostedFileBase ImageFile)
        {
            o.Image = ConvertToBytes(ImageFile);
            ctx.Organisaties.Add(o);
            ctx.SaveChanges();
        }


        public Organisatie UpdateOrganisatie(Organisatie UpdatedOrganisatie)
        {
            ctx.Entry(UpdatedOrganisatie).State = EntityState.Modified;
            ctx.SaveChanges();
            return UpdatedOrganisatie;
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

        public byte[] GetOrganisationImageFromDataBase(int Id)
        {
            var q = from temp in ctx.Organisaties where temp.EntiteitId == Id select temp.Image;

            byte[] cover = q.First();

            return cover;
        }

        public Organisatie UpdateOrganisatie(Organisatie changedOrganisatie, IEnumerable<string> selectedPeople)
        {
            throw new NotImplementedException();
        }

        public void UpdatePerson(Persoon changedPerson, IEnumerable<string> selectedOrganisations)
        {
            throw new NotImplementedException();
        }
        #endregion
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

        public EntiteitRepository(UnitOfWork uow)
        {
            ctx = uow.Context;
            ctx.SetUoWBool(true);
        }
    }
}
