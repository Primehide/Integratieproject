using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
