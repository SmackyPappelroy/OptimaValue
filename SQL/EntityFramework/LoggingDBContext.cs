using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using OptimaValue.Config;

namespace OptimaValue
{
    public class LoggingDBContext : DbContext
    {
        public DbSet<logValues> logValues { get; set; }
        public DbSet<plcConfig> plcConfig { get; set; }
        public DbSet<tagConfig> tagConfig { get; set; }

        public LoggingDBContext() : base(Settings.ConnectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<DbContext>(new CreateDatabaseIfNotExists<DbContext>());
            modelBuilder.Properties<decimal>().Configure(config => config.HasPrecision(18, 0));
            modelBuilder.Properties<TimeSpan>().Configure(config => config.HasPrecision(0));
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
