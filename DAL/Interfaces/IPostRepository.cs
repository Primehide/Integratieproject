using System.Collections.Generic;
using Domain.Entiteit;
using Domain.Post;

namespace DAL.Interfaces
{
    public interface IPostRepository
    {
        void AddPosts(List<Post> posts);
        void AddPost(Post post);
        List<Post> GetAllPosts();

        IEnumerable<Grafiek> GetAllGrafieken();

        void AddGrafiek(Grafiek grafiek);
        List<Word> GetAllWords();
        List<Word> GetAllWordsFromPost(Post post);
        List<Grafiek> AlleGrafieken();
        Grafiek ReadGrafiek(int id);
        void UpdateGrafiek(Grafiek grafiekToUpdate);
        List<Entiteit> GetAlleEntiteiten();
    }
}
