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

            modelBuilder.Entity<Credential>().HasOne(e => e.User).WithMany(u => u.Credentials)
                .HasForeignKey(e => e.UserId);
            
            modelBuilder.Entity<SecretQuestion>().HasOne(e => e.Credential).WithMany(e => e.SecretQuestions)
                .HasForeignKey(e => e.CredentialId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlite();
        }
    }
}