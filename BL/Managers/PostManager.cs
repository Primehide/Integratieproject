using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace BL
{
    public class PostManager : IPostManager
    {
        private IPostRepository postRepository;

        public PostManager()
        {
            postRepository = new PostRepository();
        }
    }
}
