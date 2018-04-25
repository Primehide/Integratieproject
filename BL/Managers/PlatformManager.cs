using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Domain.Platform;

namespace BL
{
    public class PlatformManager : IPlatformManager
    {
        private PlatformRepository platformRepository;
        private UnitOfWorkManager uowManager;

        public PlatformManager()
        {
            platformRepository = new PlatformRepository();
        }

        public PlatformManager(UnitOfWorkManager uofMgr)
        {
            platformRepository = new PlatformRepository();

            uowManager = uofMgr;
        }

        public void AddDeelplatform(Deelplatform newPlatform)
        {
            platformRepository.CreateDeelplatform(newPlatform);
        }

        public Deelplatform ChangeDeelplatform(Deelplatform changedDeelplatform)
        {
            return platformRepository.UpdateDeelplatform(changedDeelplatform);
        }

        public Deelplatform GetDeelplatform(int platformId)
        {
            return platformRepository.ReadDeelplatform(platformId);
        }

        public void RemoveDeelplatform(int platformId)
        {
            platformRepository.DeleteDeelplatform(platformId);
        }

        public IEnumerable<Deelplatform> GetAllDeelplatformen()
        {
            return platformRepository.ReadAllDeelplatformen();
        }

        #region
        public void initNonExistingRepo(bool withUnitOfWork = false)
        {
            // Als we een repo met UoW willen gebruiken en als er nog geen uowManager bestaat:
            // Dan maken we de uowManager aan en gebruiken we de context daaruit om de repo aan te maken.

            if (withUnitOfWork)
            {
                if (uowManager == null)
                {
                    uowManager = new UnitOfWorkManager();
                    platformRepository = new PlatformRepository(uowManager.UnitOfWork);
                }
            }
            // Als we niet met UoW willen werken, dan maken we een repo aan als die nog niet bestaat.
            else
            {
                platformRepository = (platformRepository == null) ? new PlatformRepository() : platformRepository;
            }
        }


        #endregion


    }
}
