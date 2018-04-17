using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Domain.Entiteit;
using System.Web;

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
        public void AddPerson(Persoon p, HttpPostedFileBase ImageFile)
        {
            if (ImageFile != null)
            {
                entiteitRepository.CreatePersonWithPhoto(p, ImageFile);
            } else
            {
                entiteitRepository.CreatePersonWithoutPhoto(p);
            }
        }

        public Persoon ChangePerson(Persoon ChangedPerson)
        {
            Persoon toUpdated = GetPerson(ChangedPerson.EntiteitId);

            toUpdated.FirstName = ChangedPerson.FirstName;
            toUpdated.LastName = ChangedPerson.LastName;
            foreach (Organisatie o in toUpdated.Organisations)
            {
                ChangeOrganisatie(o);
            }
            return entiteitRepository.UpdatePerson(toUpdated);
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
        
        public void AddOrganisatie(Organisatie o, HttpPostedFileBase ImageFile)
        {
            if (ImageFile != null)
            {
                entiteitRepository.CreateOrganisatieWithPhoto(o, ImageFile);
            } else
            {
                entiteitRepository.CreateOrganisatieWithoutPhoto(o);
            }
        }

        public Organisatie ChangeOrganisatie(Organisatie ChangedOrganisatie)
        {
            Organisatie toUpdate = GetOrganisatie(ChangedOrganisatie.EntiteitId);
            toUpdate.Naam = ChangedOrganisatie.Naam;
            toUpdate.Gemeente = ChangedOrganisatie.Gemeente;
            toUpdate.Posts = ChangedOrganisatie.Posts;
            toUpdate.Trends = ChangedOrganisatie.Trends;

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
            Organisatie toUpdate = GetOrganisatie(ChangedOrganisatie.EntiteitId);


            List<Persoon> NewlyAppointedPeople = new List<Persoon>();
            //Bestaande referenties verwijderen
            if (toUpdate.Leden != null)
            {

                foreach (Persoon p in toUpdate.Leden)
                {
                    p.Organisations.Remove(toUpdate);
                }
                toUpdate.Leden = new List<Persoon>();
            }

            //Nieuwe referenties toevoegen
            foreach (string pId in selectedPeople)
            {
                Persoon person = GetPerson(Int32.Parse(pId));
                //person.Organisations.Add(UpdatedOrganisatie);
                toUpdate.Leden.Add(person);
            }

            toUpdate.Naam = ChangedOrganisatie.Naam;
            toUpdate.Gemeente = ChangedOrganisatie.Gemeente;
            toUpdate.Posts = ChangedOrganisatie.Posts;
            toUpdate.Trends = ChangedOrganisatie.Trends;

            toUpdate.AantalLeden = toUpdate.Leden.Count();

            return entiteitRepository.UpdateOrganisatie(toUpdate);
        }

        public void ChangePerson(Persoon changedPerson, IEnumerable<string> selectedOrganisations)
        {
            Persoon toUpdated = GetPerson(changedPerson.EntiteitId);

            //Remove all references
            toUpdated.Organisations = new List<Organisatie>();

            //Add new References
            foreach (string oId in selectedOrganisations)
            {
                toUpdated.Organisations.Add(GetOrganisatie(Int32.Parse(oId)));
            }

            toUpdated.FirstName = changedPerson.FirstName;
            toUpdated.LastName = changedPerson.LastName;

            entiteitRepository.UpdatePerson(toUpdated);
        }

        public byte[] GetPersonImageFromDataBase(int id)
        {
            return entiteitRepository.GetPersonImageFromDataBase(id);
        }

        public byte[] GetOrganisationImageFromDataBase(int id)
        {
            return entiteitRepository.GetOrganisationImageFromDataBase(id);
        }
        #endregion
    }
}
