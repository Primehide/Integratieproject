using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entiteit;
using Domain.Post;
using Domain.Account;
using Domain.Platform;

namespace DAL
{
    internal class EFContext : DbContext
    {
        private readonly bool delaySave;

        public EFContext(bool unitOfWorkPresent = false)
            :base("DebugConn")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<EFContext, EFDbConfiguration>("DebugConn"));
            delaySave = unitOfWorkPresent;
        }

        //ENTITEITEN//
        public DbSet<Entiteit> Entiteiten { get; set; }
        public DbSet<Organisatie> Organisaties { get; set; }
        public DbSet<Persoon> Personen { get; set; }
        public DbSet<Thema> Themas { get; set; }
        public DbSet<Sleutelwoord> SleutelWoorden { get; set; }
        public DbSet<Trend> Trends { get; set; }

        //POST//
        public DbSet<Post> Posts { get; set; }
        public DbSet<Verhaal> Verhalen { get; set; }
        public DbSet<Grafiek> Grafieken { get; set; }
        public DbSet<Term> Termen { get; set; }

        //ACCOUNT//
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<Dashboard> Dashboards { get; set; }

        //PLATFORM//
        public DbSet<Deelplatform> DeelPlatformen { get; set; }
        public DbSet<Pagina> Paginas { get; set; }

        public override int SaveChanges()
        {
            if (delaySave) return -1;
            return base.SaveChanges();
        }

        internal int CommitChanges()
        {
            if (delaySave)
            {
                return base.SaveChanges();
            }
            throw new InvalidOperationException("No UnitOfWork present, use SaveChanges instead");
        }
    }
}
