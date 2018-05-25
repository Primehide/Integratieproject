using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DAL;
using Domain.Platform;
using Domain.Account;

namespace BL
{
    public class PlatformManager : IPlatformManager
    {
        private PlatformRepository _platformRepository;
        private UnitOfWorkManager _uowManager;

        public PlatformManager()
        {
            _platformRepository = new PlatformRepository();
        }

        public PlatformManager(UnitOfWorkManager uofMgr)
        {
            _platformRepository = new PlatformRepository();

            _uowManager = uofMgr;
        }

        public void AddDeelplatform(Deelplatform newPlatform, HttpPostedFileBase imgLogo)
        {
            InitNonExistingRepo();

            if (imgLogo != null)
            {
                BinaryReader reader = new BinaryReader(imgLogo.InputStream);
                var imageBytes = reader.ReadBytes(imgLogo.ContentLength);
                newPlatform.Logo = imageBytes;
            }
            else
            {
                byte[] imageBytes = System.IO.File.ReadAllBytes("C:/Users/WaffleDealer/Desktop/IP/Integratieproject/WebUI/Controllers/default.png");
                newPlatform.Logo = imageBytes;
            }


            _platformRepository.CreateDeelplatform(newPlatform);
        }

        public Deelplatform ChangeDeelplatform(Deelplatform changedDeelplatform, HttpPostedFileBase imgLogo)
        {
            InitNonExistingRepo();
            Deelplatform deelplatformToUpdate = GetDeelplatform(changedDeelplatform.DeelplatformId);
            if (imgLogo != null)
            {
                BinaryReader reader = new BinaryReader(imgLogo.InputStream);
                var imageBytes = reader.ReadBytes(imgLogo.ContentLength);
                deelplatformToUpdate.Logo = imageBytes;
            }
            deelplatformToUpdate.Naam = changedDeelplatform.Naam;
            deelplatformToUpdate.Tagline = changedDeelplatform.Tagline;
            deelplatformToUpdate.ColorCode1 = changedDeelplatform.ColorCode1;
            deelplatformToUpdate.ColorCode2 = changedDeelplatform.ColorCode2;
            return _platformRepository.UpdateDeelplatform(changedDeelplatform);
        }

        public Deelplatform GetDeelplatform(int platformId)
        {
            InitNonExistingRepo();
            return _platformRepository.ReadDeelplatform(platformId);
        }

        public void RemoveDeelplatform(int platformId)
        {
            InitNonExistingRepo();
            _platformRepository.DeleteDeelplatform(platformId);
        }

        public IEnumerable<Deelplatform> GetAllDeelplatformen()
        {
            InitNonExistingRepo();
            return _platformRepository.ReadAllDeelplatformen();
        }

        #region
        public void InitNonExistingRepo(bool withUnitOfWork = false)
        {
            // Als we een repo met UoW willen gebruiken en als er nog geen uowManager bestaat:
            // Dan maken we de uowManager aan en gebruiken we de context daaruit om de repo aan te maken.

            if (withUnitOfWork)
            {
                if (_uowManager == null)
                {
                    _uowManager = new UnitOfWorkManager();
                    _platformRepository = new PlatformRepository(_uowManager.UnitOfWork);
                }
            }
            // Als we niet met UoW willen werken, dan maken we een repo aan als die nog niet bestaat.
            else
            {
                _platformRepository = (_platformRepository == null) ? new PlatformRepository() : _platformRepository;
            }
        }


        #endregion


      
        public StringBuilder ConvertToCsv(List<Account> accounts)
        {
            InitNonExistingRepo();
            var lstData = accounts;
            var sb = new StringBuilder();
            foreach (var data in lstData)
            {
                sb.AppendLine(data.AccountId + "," + data.Voornaam + "," + data.Achternaam + ", " + data.Email + ", " + data.GeboorteDatum);
            }
            return sb;
        }
    }
}
