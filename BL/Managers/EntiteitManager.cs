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
            return entiteitRepository.ReadAllPeople().ToList();
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


        #region
        
        public void AddOrganisatie(Organisatie o)
        {
            entiteitRepository.CreateOrganisatie(o);
        }

        public Organisatie ChangeOrganisatie(Organisatie ChangedOrganisatie)
        {
            return entiteitRepository.UpdateOrganisatie(ChangedOrganisatie);
        }

        public List<Organisatie> GetAllOrganisaties()
        {
            return entiteitRepository.ReadAllOrganisaties().ToList();
        }

        public Organisatie GetOrganisatie(int id)
        {
            return entiteitRepository.ReadOrganisatie(id);
        }

        public void RemoveOrganisatie(int id)
        {
            entiteitRepository.DeleteOrganisatie(id);
        }

        public Organisatie ChangeOrganisatie(Organisatie ChangedOrganisatie, IEnumerable<string> selectedPeople)
        {
            return entiteitRepository.UpdateOrganisatie(ChangedOrganisatie,selectedPeople);
        }

        public void ChangePerson(Persoon changedPerson, IEnumerable<string> selectedOrganisations)
        {
             entiteitRepository.UpdatePerson(changedPerson, selectedOrganisations);
        }
        #endregion
    }
}
