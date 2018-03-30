using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace BL
{
    public class EntiteitManager : IEntiteitManager
    {
        private IEntiteitRepository entiteitRepository;
        private UnitOfWorkManager uowManager;

        public EntiteitManager()
        {

        }

        public EntiteitManager(UnitOfWorkManager uofMgr)
        {
            uowManager = uofMgr;
            entiteitRepository = new EntiteitRepository();
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
    }
}
