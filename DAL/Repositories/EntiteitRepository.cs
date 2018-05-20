using Domain.Entiteit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;

namespace DAL
{
    public class EntiteitRepository : IEntiteitRepository
    {
        private EFContext ctx;

        public EntiteitRepository()
        {
            ctx = new EFContext();
        }

        public EntiteitRepository(UnitOfWork uow)
        {
            ctx = uow.Context;
            ctx.SetUoWBool(true);
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
            return ctx.Personen
                .Include(p => p.Organisations)
                .Include(p => p.Posts)
                .Include(p => p.Posts.Select(x => x.Mentions))
                .Include(p => p.Posts.Select(x => x.Sentiment))
                .Include(p => p.Posts.Select(x => x.Words))
                .Include(p => p.Posts.Select(x => x.HashTags))
                .Include(p => p.Posts.Select(x => x.Urls))
                .Include(p => p.Trends)
                .ToList();
        }

        public Persoon ReadPerson(int id)
        {
            return ctx.Personen.Where(obj => obj.EntiteitId == id)
                .Include(p => p.Organisations)
                .Include(p => p.Grafieken)
                .Include(p => p.Grafieken.Select(x => x.Waardes))
                .First();
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
            //ctx.Entry(UpdatedOrganisatie).State = EntityState.Modified;
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
            return ctx.Entiteiten
                .Include(x => x.Posts)
                .Include(x => x.Trends)
                .Include(x => x.Grafieken)
                .Include(x => x.Grafieken.Select(y => y.Waardes))
                .ToList();
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
            var result = ReadThema(thema.EntiteitId);
         
            if (result != null)
            {
                result.Naam = thema.Naam;
                ctx.SaveChanges();
            }
        }

        public void DeleteThema(int entiteitsId)
        {
            Thema thema = ReadThema(entiteitsId);
            //IList<Sleutelwoord> sleutelwoorden = thema.SleutenWoorden;
            foreach(Sleutelwoord sw in thema.SleutenWoorden.ToList())
            {
                ctx.SleutelWoorden.Remove(sw);
            }
            thema.SleutenWoorden = null;
            ctx.SaveChanges();
            // var thema = ctx.Themas.SingleOrDefault(b => b.EntiteitId == entiteitsId);
            ctx.Themas.Remove(thema);
            ctx.SaveChanges();
        }

        public Thema ReadThema(int entiteitsId)
        {
            Thema thema = ctx.Themas.Include(x => x.SleutenWoorden).SingleOrDefault(x => x.EntiteitId == entiteitsId);
            return thema;
        }

        public Sleutelwoord readSleutelwoord(int sleutelId)
        {
            Sleutelwoord sleutelwoord = ctx.SleutelWoorden.SingleOrDefault(x => x.SleutelwoordId == sleutelId);
            return sleutelwoord;
        }

        public IEnumerable<Thema> ReadThemas()
        {
            return ctx.Themas.Include(x => x.SleutenWoorden).ToList();
        }

        public void DeleteSleutelwoord(int sleutelId)
        {
            Sleutelwoord sleutelwoord = readSleutelwoord(sleutelId);
            ctx.SleutelWoorden.Remove(sleutelwoord);
            ctx.SaveChanges();
        }


        public Entiteit ReadEntiteit(int id)
        {
            return ctx.Entiteiten.SingleOrDefault(e => e.EntiteitId == id);
        }


       public IEnumerable<Entiteit> ReadEntiteitenVanDeelplatform(int id)
        {
            return ctx.Entiteiten.Where(x => x.PlatformId == id).ToList();
        }

        public void DeleteEntiteitenVanDeelplatform(int id)
        {
            ctx.Entiteiten.RemoveRange(ReadEntiteitenVanDeelplatform(id));
        }

        public void addEntiteit(Entiteit entiteit)
        {
            ctx.Entiteiten.Add(entiteit);
            ctx.SaveChanges();
        }



        List<Entiteit> IEntiteitRepository.ReadEntiteiten(string naam)
        {
            List<Entiteit> entiteiten = ctx.Entiteiten.Where(x => x.Naam.ToUpper().Contains(naam.ToUpper())).ToList();
            List<Thema> themas = ctx.Themas.Include(p => p.SleutenWoorden).ToList();
            foreach(Thema thema in themas)
            {
                foreach(Sleutelwoord sl in thema.SleutenWoorden)
                {
                    if (sl.woord.ToUpper().Contains(naam.ToUpper())) {
                        entiteiten.Add(ReadEntiteit(thema.EntiteitId));
                    }
                }
            }
            return entiteiten;
        }
    }
}
