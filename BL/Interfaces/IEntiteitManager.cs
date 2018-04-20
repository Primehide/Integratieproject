using Domain.Entiteit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BL
{
    public interface IEntiteitManager
    {
        void CreateTestData();
        List<Domain.Entiteit.Entiteit> getAlleEntiteiten();
        void updateEntiteit(Entiteit entiteit);
        void AddThema(string naam, List<Sleutelwoord> sleutelwoorden);
        void UpdateThema(Thema thema);
        void DeleteThema(int entiteitsId);
        IEnumerable<Thema> GetThemas();
        Thema GetThema(int entiteitsId);
    }
}
