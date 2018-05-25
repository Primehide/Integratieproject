using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entiteit;
using Domain.Post;

namespace DAL
{
    public interface IPostRepository
    {
        void AddPosts(List<Post> posts);
        void AddPost(Post post);
        List<Post> getAllPosts();

        IEnumerable<Grafiek> GetAllGrafieken();

        void AddGrafiek(Grafiek grafiek);
        List<Word> GetAllWords();
        List<Word> GetAllWordsFromPost(Post post);
        List<Grafiek> AlleGrafieken();
        Grafiek ReadGrafiek(int id);
        void UpdateGrafiek(Grafiek grafiekToUpdate);
        List<Entiteit> getAlleEntiteiten();
    }
}
