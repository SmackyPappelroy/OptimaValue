using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using OptimaValue.Config;


namespace OptimaValue
{
    public class LoggingDBContext : DbContext
    {
        public DbSet<logValues> logValues { get; set; }
        public DbSet<plcConfig> plcConfig { get; set; }
        public DbSet<tagConfig> tagConfig { get; set; }

        public LoggingDBContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Ange anslutningssträngen direkt här
                optionsBuilder.UseSqlServer($"Server={Config.Settings.Server};Database={Config.Settings.Databas};User={Config.Settings.User};Password={Config.Settings.Password};TrustServerCertificate=True;");

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Konfigurera precision för decimalvärden
            modelBuilder.Entity<logValues>()
                .Property(l => l.index)
                .HasColumnType("decimal(18, 0)");

            // Konfigurera TimeSpan precision (om tillämpligt)
            // EF Core hanterar dock TimeSpan automatiskt bättre än EF6.
            modelBuilder.Entity<tagConfig>()
                .Property(t => t.timeOfDay)
                .HasColumnType("time");

            // Lägg till index för snabbare sökning efter loggar baserat på tid
            modelBuilder.Entity<logValues>()
                .HasIndex(l => l.logTime)
                .HasDatabaseName("IX_LogValues_LogTime");

            // Lägg till index för relationer via tagConfig
            modelBuilder.Entity<logValues>()
                .HasIndex(l => l.tag_id) // Foreign Key till tagConfig
                .HasDatabaseName("IX_LogValues_TagId");

            // Kombinerat index för logTime och tag_id
            modelBuilder.Entity<logValues>()
                .HasIndex(l => new { l.tag_id, l.logTime })
                .HasDatabaseName("IX_LogValues_TagId_LogTime");

            // Definiera relation mellan logValues och tagConfig
            modelBuilder.Entity<logValues>()
                .HasOne(l => l.tag) // Navigation property
                .WithMany() // Omvänd relation
                .HasForeignKey(l => l.tag_id) // FK i logValues
                .OnDelete(DeleteBehavior.Restrict); // Ingen cascading delete

            // Explicit mappning av float till SQL float
            modelBuilder.Entity<tagConfig>()
                .Property(t => t.deadband)
                .HasColumnType("float");

            modelBuilder.Entity<tagConfig>()
                .Property(t => t.analogValue)
                .HasColumnType("float");

            modelBuilder.Entity<tagConfig>()
                .Property(t => t.scaleMin)
                .HasColumnType("float");

            modelBuilder.Entity<tagConfig>()
                .Property(t => t.scaleMax)
                .HasColumnType("float");

            modelBuilder.Entity<tagConfig>()
                .Property(t => t.rawMin)
                .HasColumnType("float");

            modelBuilder.Entity<tagConfig>()
                .Property(t => t.rawMax)
                .HasColumnType("float");

            modelBuilder.Entity<tagConfig>()
                .Property(t => t.scaleOffset)
                .HasColumnType("float");

            // Explicit mappning för numericValue till SQL float
            modelBuilder.Entity<logValues>()
                .Property(l => l.numericValue)
                .HasColumnType("float");

            base.OnModelCreating(modelBuilder);
        }
    }
}
