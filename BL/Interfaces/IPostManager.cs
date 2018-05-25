using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entiteit;
using Domain.Post;

namespace BL.Interfaces
{
    public interface IPostManager
    {
        Task SyncDataAsync(int platformid);
        void AddPost(Post post);
        List<Post> GetAllPosts();
        List<Grafiek> GetAllGrafieken();
        Dictionary<string, double> BerekenGrafiekWaarde(Domain.Enum.GrafiekType grafiekType, List<Entiteit> entiteiten);
        List<Post> GetRecentePosts();
        void MaakVasteGrafieken();
        void AddGrafiek(Grafiek grafiek);
        List<Grafiek> GetVasteGrafieken();
        void UpdateGrafiek(int id);
        Grafiek GetGrafiek(int id);
        void UpdateGrafiek(List<int> entiteitIds, Grafiek grafiek);
        List<GrafiekWaarde> BerekenGrafiekWaardes(List<CijferOpties> opties, List<Entiteit> entiteiten);
    }
}
