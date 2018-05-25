using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DAL.Interfaces;
using Domain.Entiteit;
using Domain.Post;

namespace DAL.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly EFContext _ctx;

        public PostRepository()
        {
            _ctx = new EFContext();
        }

        public PostRepository(UnitOfWork uow)
        {
            _ctx = uow.Context;
            _ctx.SetUoWBool(true);
        }

        public void AddPosts(List<Post> posts)
        {
            foreach (var post in posts)
            {
                _ctx.Posts.Add(post);
            }
            _ctx.SaveChanges();
        }

        public void AddPost(Post post)
        {
            _ctx.Posts.Add(post);
            _ctx.SaveChanges();
        }

        public List<Post> GetAllPosts()
        {
            return _ctx.Posts
                .Include(x => x.Entiteiten)
                .Include(x => x.HashTags)
                .Include(x => x.Mentions)
                .Include(x => x.Persons)
                .Include(x => x.Profile)
                .Include(x => x.Sentiment)
                .Include(x => x.Urls)
                .Include(x => x.Words)
                .ToList();
        }


        public IEnumerable<Grafiek> GetAllGrafieken()
        {
            return _ctx.Grafieken.Include(ctx => ctx.Entiteiten).Include(mbox => mbox.Waardes).ToList();
        }
        public void AddGrafiek(Grafiek grafiek)
        {
            _ctx.Grafieken.Add(grafiek);
            _ctx.SaveChanges();
        }

        public List<Word> GetAllWords()
        {
            return _ctx.Words.ToList();
        }
        public List<Word> GetAllWordsFromPost(Post post)
        {
            return _ctx.Words.Where(x => x.PostId == post.PostId).ToList();
        }

        public List<Grafiek> AlleGrafieken()
        {
            return _ctx.Grafieken.Include(x => x.Waardes).Include(x => x.Entiteiten).ToList();

        }

        public Grafiek ReadGrafiek(int id)
        {
            return _ctx.Grafieken
                .Include(x => x.Waardes)
                .Include(x => x.Entiteiten)
                .Include(x => x.CijferOpties)
                .Single(x => x.GrafiekId == id);
        }

        public void UpdateGrafiek(Grafiek grafiekToUpdate)
        {
            //ctx.Entry(grafiekToUpdate.Entiteiten).State = EntityState.Unchanged;
            _ctx.Entry(grafiekToUpdate).State = EntityState.Modified;
            _ctx.SaveChanges();
        }

        public List<Entiteit> GetAlleEntiteiten()
        {
            return _ctx.Entiteiten.Include(x => x.Posts).ToList();
        }
    }
}
