using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Storage;

namespace TheWorld.Models
{
    public class WorldContext : IdentityDbContext<WorldUser>
    {
        public WorldContext()
        {
            Database.EnsureCreated(); // create the DB and execute any migrations needed
        }

        public DbSet<Trip> Trips { get; set; }
        public DbSet<Stop> Stops { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

#if RELEASE
            var connString = Startup.Configuration["Data:WorldContextConnection"];
#else
            var connString = Startup.Configuration["Data:WorldContextConnectionRelease"];
#endif

            optionsBuilder.UseSqlServer(connString);


            base.OnConfiguring(optionsBuilder);
        }
    }
}
