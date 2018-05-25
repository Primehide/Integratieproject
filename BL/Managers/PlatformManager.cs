using System.Collections.Generic;
using System.Text;
using BL.Interfaces;
using DAL.Repositories;
using Domain.Account;
using Domain.Platform;

namespace BL.Managers
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

        public void AddDeelplatform(Deelplatform newPlatform)
        {
            InitNonExistingRepo();
            _platformRepository.CreateDeelplatform(newPlatform);
        }

        public Deelplatform ChangeDeelplatform(Deelplatform changedDeelplatform)
        {
            InitNonExistingRepo();
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


      
        public StringBuilder ConvertToCSV(List<Account> accounts)
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

        //refactor sander
        public void AddFaq(Faq faq, int platId)
        {
            InitNonExistingRepo();
            faq.PlatformId = platId;
            _platformRepository.AddFaq(faq);
        }

        public void UpdateFaq(Faq faq)
        {
            InitNonExistingRepo();
            _platformRepository.UpdateFaq(faq);

        }
        public void DeleteFaq(int faqId)
        {
            InitNonExistingRepo();
            _platformRepository.DeleteFaq(faqId);

        }
        public List<Faq> GetAlleFaqs(int platId)
        {
            InitNonExistingRepo();
            return _platformRepository.GetAlleFaqs(platId);
        }
    }
}
