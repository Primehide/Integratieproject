using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Post;
using System.Data.Entity;
using Domain.Entiteit;

namespace DAL
{
    public class PostRepository : IPostRepository
    {
        private EFContext ctx;

        public PostRepository()
        {
            ctx = new EFContext();
        }

        public PostRepository(UnitOfWork uow)
        {
            ctx = uow.Context;
            ctx.SetUoWBool(true);
        }

        public void AddPosts(List<Post> posts)
        {
            foreach (var post in posts)
            {
                ctx.Posts.Add(post);
            }
            ctx.SaveChanges();
        }

        public void AddPost(Post post)
        {
            ctx.Posts.Add(post);
            ctx.SaveChanges();
        }

        public List<Post> getAllPosts()
        {
            return ctx.Posts
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


        public IEnumerable<Domain.Post.Grafiek> GetAllGrafieken()
        {
            return ctx.Grafieken.Include(ctx => ctx.Entiteiten).Include(mbox => mbox.Waardes).ToList();
        }
        public void AddGrafiek(Grafiek grafiek)
        {
            ctx.Grafieken.Add(grafiek);
            ctx.SaveChanges();
        }

        public List<Word> GetAllWords()
        {
            return ctx.Words.ToList();
        }

        public List<Grafiek> AlleGrafieken()
        {
            return ctx.Grafieken.Include(x => x.Waardes).Include(x => x.Entiteiten).ToList();

        }

        public Grafiek ReadGrafiek(int id)
        {
            return ctx.Grafieken
                .Include(x => x.Waardes)
                .Include(x => x.Entiteiten)
                .Include(x => x.CijferOpties)
                .Single(x => x.GrafiekId == id);
        }

        public void UpdateGrafiek(Grafiek grafiekToUpdate)
        {
            //ctx.Entry(grafiekToUpdate.Entiteiten).State = EntityState.Unchanged;
            ctx.Entry(grafiekToUpdate).State = EntityState.Modified;
            ctx.SaveChanges();
        }

        public List<Entiteit> getAlleEntiteiten()
        {
            return ctx.Entiteiten.Include(x => x.Posts).ToList();
        }
    }
}
