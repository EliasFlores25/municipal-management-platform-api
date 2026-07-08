using Domain;
using Microsoft.EntityFrameworkCore;

namespace Data.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Municipality> Municipalities { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");

                entity.HasKey(r => r.Id);
                entity.Property(r => r.Id)
                      .UseIdentityColumn();

                entity.Property(r => r.Name)
                      .IsRequired()
                      .HasMaxLength(30)
                      .HasColumnType("varchar(30)");

                entity.HasIndex(r => r.Name)
                      .IsUnique()
                      .HasDatabaseName("UQ_Roles_Name");
            });
            modelBuilder.Entity<Municipality>(entity =>
            {
                entity.ToTable("Municipalities");

                entity.HasKey(m => m.Id);
                entity.Property(m => m.Id)
                      .UseIdentityColumn();

                entity.Property(m => m.Name)
                      .IsRequired()
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");

                entity.HasIndex(m => m.Name)
                      .IsUnique()
                      .HasDatabaseName("UQ_Municipalities_Name");
            });
            modelBuilder.Entity<Position>(entity =>
            {
                entity.ToTable("Positions");

                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id)
                      .UseIdentityColumn();

                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");

                entity.Property(p => p.Description)
                      .IsRequired()
                      .HasMaxLength(200)
                      .HasColumnType("nvarchar(200)");

                entity.HasIndex(p => p.Name)
                      .IsUnique()
                      .HasDatabaseName("UQ_Positions_Name");
            });
            modelBuilder.Entity<DocumentType>(entity =>
            {
                entity.ToTable("DocumentTypes");
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Id).UseIdentityColumn();

                entity.Property(d => d.Name)
                      .IsRequired()
                      .HasMaxLength(50)
                      .HasColumnType("nvarchar(50)");

                entity.HasIndex(d => d.Name)
                      .IsUnique()
                      .HasDatabaseName("UQ_DocumentTypes_Name");
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
