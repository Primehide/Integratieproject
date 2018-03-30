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
        public EFContext() : base("DebugConn")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<EFContext, EFDbConfiguration>("DebugConn"));
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
    }
}
