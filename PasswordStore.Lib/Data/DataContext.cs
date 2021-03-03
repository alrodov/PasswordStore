namespace PasswordStore.Lib.Data
{
    using Microsoft.EntityFrameworkCore;
    using PasswordStore.Lib.Entities;

    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        
        public DbSet<Credential> Credentials { get; set; }

        public DataContext() : base()
        {
        }
        
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasIndex(nameof(User.Login)).IsUnique();
            
            modelBuilder.Entity<Credential>()
                .HasIndex(nameof(Credential.ServiceName), nameof(Credential.Login), nameof(Credential.UserId))
                .IsUnique();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlite();
        }
    }
}