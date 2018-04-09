using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class EntiteitRepository : IEntiteitRepository
    {
        private EFContext ctx;

        public EntiteitRepository()
        {
            ctx = new EFContext();
        }

        public void AddEntiteit(Domain.Entiteit.Entiteit entiteit)
        {
            ctx.Entiteiten.Add(entiteit);
            ctx.SaveChanges();
        }
    }
}
