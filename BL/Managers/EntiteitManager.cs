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
<<<<<<< Updated upstream
=======
        }
       
        public void AddThema(string naam, List<Trend> trends, List<Domain.Post.Post> posts, 
            List<Sleutelwoord> sleutelwoorden)
        {
            Thema thema = new Thema()
            {
                Naam = naam,
                Trends = trends,
                Posts = posts,
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
>>>>>>> Stashed changes
        }
       
    }
}
