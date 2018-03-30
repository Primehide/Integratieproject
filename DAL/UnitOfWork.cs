using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class UnitOfWork
    {
        private EFContext ctx;

        internal EFContext Context
        {
            get
            {
                if (ctx == null) ctx = new EFContext();
                return ctx;
            }
        }

        /// <summary>
        /// Deze methode zorgt ervoor dat alle tot hier toe aangepaste domein objecten
        /// worden gepersisteert naar de databank
        /// </summary>
        public void CommitChanges()
        {
            ctx.CommitChanges();
        }
    }
}
