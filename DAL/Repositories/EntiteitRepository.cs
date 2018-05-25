using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using DAL.Interfaces;
using Domain.Entiteit;

namespace DAL.Repositories
{
    public class EntiteitRepository : IEntiteitRepository
    {
        private readonly EFContext _ctx;

        public EntiteitRepository()
        {
            _ctx = new EFContext();
        }

        public EntiteitRepository(UnitOfWork uow)
        {
            _ctx = uow.Context;
            _ctx.SetUoWBool(true);
        }

        public byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            BinaryReader reader = new BinaryReader(image.InputStream);
            var imageBytes = reader.ReadBytes(image.ContentLength);
            return imageBytes;

        }

        #region

        public void CreatePersonWithoutPhoto(Persoon p)
        {
            _ctx.Personen.Add(p);
            _ctx.SaveChanges();

        }

        public void CreatePersonWithPhoto(Persoon p, HttpPostedFileBase imageFile)
        {
            p.Image = ConvertToBytes(imageFile);
            _ctx.Personen.Add(p);
            _ctx.SaveChanges();

        }

        public void DeletePerson(int id)
        {
            _ctx.Personen.Remove(ReadPerson(id));
            _ctx.SaveChanges();
        }

        public IEnumerable<Persoon> ReadAllPeople()
        {
            return _ctx.Personen
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
            return _ctx.Personen.Where(obj => obj.EntiteitId == id).Include(p => p.Organisations).Include(j => j.Posts.Select(s => s.Sentiment)).First();
        }

        public Persoon UpdatePerson(Persoon updatedPerson )
        {
            _ctx.Entry(updatedPerson).State = EntityState.Modified;
            _ctx.SaveChanges();
            return updatedPerson;
        }

        public byte[] GetPersonImageFromDataBase(int id)
        {
            var q = from temp in _ctx.Personen where temp.EntiteitId == id select temp.Image;
            byte[] cover = q.First();
            return cover;

        }
        #endregion

        #region

        public void CreateOrganisatieWithoutPhoto(Organisatie o)
        {
            foreach (Persoon p in o.Leden ?? new List<Persoon>())
            {
                _ctx.Entry(p).State = EntityState.Modified;
            }
            _ctx.Organisaties.Add(o);
            _ctx.SaveChanges();
        }

        public void CreateOrganisatieWithPhoto(Organisatie o, HttpPostedFileBase imageFile)
        {
            foreach (Persoon p in o.Leden ?? new List<Persoon>())
            {
                _ctx.Entry(p).State = EntityState.Modified;
            }
            o.Image = ConvertToBytes(imageFile);
            _ctx.Organisaties.Add(o);
            _ctx.SaveChanges();
        }


        public Organisatie UpdateOrganisatie(Organisatie updatedOrganisatie)
        {

            _ctx.SaveChanges();
            return updatedOrganisatie;
        }


        public Organisatie ReadOrganisatie(int id)
        {
            return _ctx.Organisaties.Where(obj => obj.EntiteitId == id).Include(o => o.Leden).First();
        }


        public IEnumerable<Organisatie> ReadAllOrganisaties()
        {
            return _ctx.Organisaties.Include(o => o.Leden).ToList();
        }

        public void DeleteOrganisatie(int id)
        {
            _ctx.Organisaties.Remove(ReadOrganisatie(id));
            _ctx.SaveChanges();
        }

        public byte[] GetOrganisationImageFromDataBase(int id)
        {
            var q = from temp in _ctx.Organisaties where temp.EntiteitId == id select temp.Image;

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
        public void AddEntiteit(Entiteit entiteit)
        {
            _ctx.Entiteiten.Add(entiteit);
            _ctx.SaveChanges();
        }
     
        public List<Entiteit> GetAlleEntiteiten()
        {
            return _ctx.Entiteiten
                .Include(x => x.Posts)
                .Include(x => x.Trends)
                .Include(x => x.Grafieken)
                .Include(x => x.Grafieken.Select(y => y.Waardes))
                .ToList();
        }

        public List<Entiteit> GetAlleEntiteiten(bool includePosts)
        {
            if (includePosts)
            {
                return _ctx.Entiteiten
                .Include(x => x.Posts)
                .Include(x => x.Trends)
                .Include(x => x.Grafieken)
                .Include(x => x.Grafieken.Select(y => y.Waardes))
                .ToList();
            } else
            {
                return _ctx.Entiteiten
                .Include(x => x.Trends)
                .Include(x => x.Grafieken)
                .Include(x => x.Grafieken.Select(y => y.Waardes))
                .ToList();
            }
        }

        public void UpdateEntiteit(Entiteit entiteit)
        {
            _ctx.Entry(entiteit).State = EntityState.Modified;
            _ctx.SaveChanges();
        }

        public void CreateThema(Thema thema, HttpPostedFileBase imageFile)
        {
            if(imageFile != null)
            {
                thema.Image = ConvertToBytes(imageFile);
            }
      
            _ctx.Themas.Add(thema);
            _ctx.SaveChanges();
        }


        public Thema UpdateThema(Thema updatedThema)
        {
          _ctx.Entry(updatedThema).State = EntityState.Modified;
            _ctx.SaveChanges();
            return updatedThema;
        }

        public void DeleteThema(int entiteitsId)
        {
            Thema thema = ReadThema(entiteitsId);
            foreach(Sleutelwoord sw in thema.SleutenWoorden.ToList())
            {
                _ctx.SleutelWoorden.Remove(sw);
            }
            thema.SleutenWoorden = null;
            _ctx.SaveChanges();
            _ctx.Themas.Remove(thema);
            _ctx.SaveChanges();
        }

        public Thema ReadThema(int entiteitsId)
        {
            Thema thema = _ctx.Themas.Include(x => x.SleutenWoorden).SingleOrDefault(x => x.EntiteitId == entiteitsId);
            return thema;
        }

        public Sleutelwoord ReadSleutelwoord(int sleutelId)
        {
            Sleutelwoord sleutelwoord = _ctx.SleutelWoorden.SingleOrDefault(x => x.SleutelwoordId == sleutelId);
            return sleutelwoord;
        }

        public IEnumerable<Thema> ReadThemas()
        {
            return _ctx.Themas.Include(x => x.SleutenWoorden).ToList();
        }

        public void DeleteSleutelwoord(int sleutelId)
        {
            Sleutelwoord sleutelwoord = ReadSleutelwoord(sleutelId);
            _ctx.SleutelWoorden.Remove(sleutelwoord);
            _ctx.SaveChanges();
        }


        public Entiteit ReadEntiteit(int id)
        {
            return _ctx.Entiteiten.Include(e => e.Posts.Select(y => y.Urls)).Include(e => e.Trends).SingleOrDefault(e => e.EntiteitId == id);
        }


       public IEnumerable<Entiteit> ReadEntiteitenVanDeelplatform(int id)
        {
            return _ctx.Entiteiten.Where(x => x.PlatformId == id).ToList();
        }

        public void DeleteEntiteitenVanDeelplatform(int id)
        {
            _ctx.Entiteiten.RemoveRange(ReadEntiteitenVanDeelplatform(id));
        }

        List<Entiteit> IEntiteitRepository.ReadEntiteiten(string naam)
        {
            List<Entiteit> entiteiten = _ctx.Entiteiten.Where(x => x.Naam.ToUpper().Contains(naam.ToUpper())).ToList();
            List<Thema> themas = _ctx.Themas.Include(p => p.SleutenWoorden).ToList();
            foreach(Thema thema in themas)
            {
                foreach(Sleutelwoord sl in thema.SleutenWoorden)
                {
                    if (sl.Woord.ToUpper().Contains(naam.ToUpper())) {
                        entiteiten.Add(ReadEntiteit(thema.EntiteitId));
                    }
                }
            }
            return entiteiten;
        }
    }
}
