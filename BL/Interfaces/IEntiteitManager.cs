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
        void AddThema(string naam, List<Trend> trends, List<Domain.Post.Post> posts, List<Sleutelwoord> sleutelwoorden);
        void UpdateThema(Thema thema);
        void DeleteThema(int entiteitsId);
        IEnumerable<Thema> GetThemas();
        Thema GetThema(int entiteitsId);
    }
}
