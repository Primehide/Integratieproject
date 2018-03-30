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
        private IEntiteitRepository entiteitRepository;

        public EntiteitManager()
        {
            entiteitRepository = new EntiteitRepository();
        }
        #region
        public void AddPerson(Persoon p)
        {
            entiteitRepository.CreatePerson(p);
        }

        public Persoon ChangePerson(Persoon ChangedPerson)
        {
            return entiteitRepository.UpdatePerson(ChangedPerson);
        }

        public List<Persoon> GetAllPeople()
        {
            return entiteitRepository.ReadlAllPeople();
        }

        public Persoon GetPerson(int id)
        {
            return entiteitRepository.ReadPerson(id);
        }

        public void RemovePerson(int id)
        {
            entiteitRepository.DeletePerson(id);
        }
        #endregion
    }
}
