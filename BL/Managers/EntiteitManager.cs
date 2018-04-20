using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Domain.Entiteit;

namespace BL
{
    public class EntiteitManager : IEntiteitManager
    {
        private IEntiteitRepository entiteitRepository = new EntiteitRepository();
        private UnitOfWorkManager uowManager;

        public EntiteitManager()
        {

        }

        public EntiteitManager(UnitOfWorkManager uofMgr)
        {
            uowManager = uofMgr;
        }

        public void initNonExistingRepo(bool withUnitOfWork = false)
        {
            // Als we een repo met UoW willen gebruiken en als er nog geen uowManager bestaat:
            // Dan maken we de uowManager aan en gebruiken we de context daaruit om de repo aan te maken.

            if (withUnitOfWork)
            {
                if (uowManager == null)
                {
                    uowManager = new UnitOfWorkManager();
                    entiteitRepository = new EntiteitRepository(uowManager.UnitOfWork);
                }
            }
            // Als we niet met UoW willen werken, dan maken we een repo aan als die nog niet bestaat.
            else
            {
                entiteitRepository = (entiteitRepository == null) ? new EntiteitRepository() : entiteitRepository;
            }
        }

        public void CreateTestData()
        {
            initNonExistingRepo(false);
            Domain.Entiteit.Organisatie NVA = new Domain.Entiteit.Organisatie()
            {
                Leden = new List<Domain.Entiteit.Persoon>(),
                Naam = "N-VA"
            };

            Domain.Entiteit.Persoon BenWeyts = new Domain.Entiteit.Persoon()
            {
                Naam = "Ben Weyts",
                Organisaties = new List<Domain.Entiteit.Organisatie>()
            };

            Domain.Entiteit.Persoon Bartje = new Domain.Entiteit.Persoon()
            {
                Naam = "Bart De Wever",
                Organisaties = new List<Domain.Entiteit.Organisatie>()
            };

            //legt eveneens relatie van organisatie -> lid (Ben Weyts) en van Ben Weyts kunnen we zijn orginasaties opvragen (in dit geval N-VA)
            BenWeyts.Organisaties.Add(NVA);
            Bartje.Organisaties.Add(NVA);


            entiteitRepository.AddEntiteit(NVA);
            entiteitRepository.AddEntiteit(BenWeyts);
            entiteitRepository.AddEntiteit(Bartje);
        }

        public List<Entiteit> getAlleEntiteiten()
        {
            initNonExistingRepo(false);
            return entiteitRepository.getAlleEntiteiten();
        }

        public void updateEntiteit(Entiteit entiteit)
        {
            initNonExistingRepo(false);
            entiteitRepository.updateEntiteit(entiteit);
        }

        public void AddThema(string naam, List<Sleutelwoord> sleutelwoorden)
        {
            Thema thema = new Thema()
            {
                Naam = naam,
                SleutenWoorden = sleutelwoorden
            };
            entiteitRepository.CreateThema(thema);
        }


        public void UpdateThema(Thema thema)
        {
            entiteitRepository.UpdateThema(thema);
        }

        public void DeleteThema(int entiteitsId)
        {
            entiteitRepository.DeleteThema(entiteitsId);
        }

        public IEnumerable<Thema> GetThemas()
        {
            return entiteitRepository.ReadThemas();
        }

        public Thema GetThema(int entiteitsId)
        {
            return entiteitRepository.ReadThema(entiteitsId);
        }
    }
}
