using Microsoft.EntityFrameworkCore;
using OutReachToursAPI.Models;
using System.Text.Json;

namespace OutReachToursAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<CustomRole> Roles { get; set; }
        public DbSet<PipelineStage> Stages { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientActivity> ClientActivities { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<POSTransaction> Transactions { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Npgsql array mapping for List<string>
            modelBuilder.Entity<CustomRole>()
                .Property(r => r.Permissions)
                .HasColumnType("text[]");

            modelBuilder.Entity<Client>()
                .HasMany(c => c.Activities)
                .WithOne()
                .HasForeignKey(a => a.ClientId);
        }
    }
}
