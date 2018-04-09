using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Post;

namespace DAL
{
    public class PostRepository : IPostRepository
    {
        private EFContext ctx;

        public PostRepository()
        {
            ctx = new EFContext();
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
            return ctx.Posts.ToList();
        }

        public PostRepository(UnitOfWork uow)
        {
            ctx = uow.Context;
            ctx.SetUoWBool(true);
        }
    }
}
