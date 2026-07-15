using Domain;
using Domain.Enums;
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
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Notice> Notices { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Document> Documents { get; set; }


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
            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.ToTable("Inventories");

                entity.HasKey(i => i.Id);
                entity.Property(i => i.Id)
                      .UseIdentityColumn();

                entity.Property(i => i.ItemName)
                      .IsRequired()
                      .HasMaxLength(50)
                      .HasColumnType("nvarchar(50)");

                entity.Property(i => i.Description)
                      .IsRequired()
                      .HasMaxLength(100)
                      .HasColumnType("nvarchar(100)");

                entity.Property(i => i.Quantity)
                      .IsRequired();

                entity.Property(i => i.EntryDate)
                      .IsRequired();

                entity.Property(i => i.ImageUrl)
                      .IsRequired()
                      .HasMaxLength(250)
                      .HasColumnType("varchar(250)");

                entity.Property(i => i.State)
                      .IsRequired()
                      .HasConversion(
                          s => s.ToString(),
                          s => (InventoryStatus)Enum.Parse(typeof(InventoryStatus), s))
                      .HasMaxLength(20)
                      .HasColumnType("varchar(20)");

                entity.HasOne(i => i.Municipality)
                      .WithMany()
                      .HasForeignKey(i => i.MunicipalityId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_Inventory_Municipalities_MunicipalityId");
            });
            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("Projects");

                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id)
                      .UseIdentityColumn();

                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(50)
                      .HasColumnType("nvarchar(50)");

                entity.Property(p => p.Description)
                      .IsRequired()
                      .HasMaxLength(200)
                      .HasColumnType("nvarchar(200)");

                entity.Property(p => p.StartDate)
                      .IsRequired()
                      .HasColumnType("datetime2");

                entity.Property(p => p.EndDate)
                      .IsRequired()
                      .HasColumnType("datetime2");

                entity.Property(p => p.Budget)
                      .IsRequired()
                      .HasPrecision(18, 2);

                entity.Property(p => p.State)
                      .IsRequired()
                      .HasConversion(
                          s => s.ToString(),
                          s => (ProjectStatus)Enum.Parse(typeof(ProjectStatus), s))
                      .HasMaxLength(20)
                      .HasColumnType("varchar(20)");

                entity.HasOne(p => p.Municipality)
                      .WithMany()
                      .HasForeignKey(p => p.MunicipalityId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_Projects_Municipalities_MunicipalityId");
            });
            modelBuilder.Entity<Notice>(entity =>
            {
                entity.ToTable("Notices");

                entity.HasKey(n => n.Id);
                entity.Property(n => n.Id)
                      .UseIdentityColumn();

                entity.Property(n => n.Title)
                      .IsRequired()
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");

                entity.Property(n => n.Description)
                      .IsRequired()
                      .HasMaxLength(200)
                      .HasColumnType("nvarchar(200)");

                entity.Property(n => n.RegistrationDate)
                      .IsRequired()
                      .HasColumnType("datetime2");

                entity.Property(n => n.IsArchived)
                      .IsRequired()
                      .HasDefaultValue(false);

                entity.Property(n => n.Category)
                      .IsRequired()
                      .HasConversion(
                          c => c.ToString(),
                          c => (NoticeCategory)Enum.Parse(typeof(NoticeCategory), c))
                      .HasMaxLength(30)
                      .HasColumnType("varchar(30)");

                entity.HasOne(n => n.Municipality)
                      .WithMany()
                      .HasForeignKey(n => n.MunicipalityId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_Notices_Municipalities_MunicipalityId");
            });
            modelBuilder.Entity<Problem>(entity =>
            {
                entity.ToTable("Problems");

                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id)
                      .UseIdentityColumn();

                entity.Property(p => p.Title)
                      .IsRequired()
                      .HasMaxLength(50)
                      .HasColumnType("nvarchar(50)");

                entity.Property(p => p.Description)
                      .IsRequired()
                      .HasMaxLength(200)
                      .HasColumnType("nvarchar(200)");

                entity.Property(p => p.RegistrationDate)
                      .IsRequired()
                      .HasColumnType("datetime2");

                entity.Property(p => p.Type)
                      .IsRequired()
                      .HasConversion(
                          t => t.ToString(),
                          t => (ProblemType)Enum.Parse(typeof(ProblemType), t))
                      .HasMaxLength(30)
                      .HasColumnType("varchar(30)");

                entity.Property(p => p.Severity)
                      .IsRequired()
                      .HasConversion(
                          s => s.ToString(),
                          s => (ProblemSeverity)Enum.Parse(typeof(ProblemSeverity), s))
                      .HasMaxLength(20)
                      .HasColumnType("varchar(20)");

                entity.Property(p => p.Status)
                      .IsRequired()
                      .HasConversion(
                          s => s.ToString(),
                          s => (ProblemStatus)Enum.Parse(typeof(ProblemStatus), s))
                      .HasMaxLength(20)
                      .HasColumnType("varchar(20)");

                entity.HasOne(p => p.Municipality)
                      .WithMany()
                      .HasForeignKey(p => p.MunicipalityId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_Problems_Municipalities_MunicipalityId");
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id)
                      .UseIdentityColumn();

                entity.Property(u => u.Name)
                      .IsRequired()
                      .HasMaxLength(50)
                      .HasColumnType("nvarchar(50)");

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(100)
                      .HasColumnType("varchar(100)");

                entity.Property(u => u.PasswordHash)
                      .IsRequired()
                      .HasMaxLength(255)
                      .HasColumnType("varchar(255)");

                entity.Property(u => u.IsActive)
                      .IsRequired()
                      .HasDefaultValue(true);

                entity.HasIndex(u => u.Email)
                      .IsUnique()
                      .HasDatabaseName("UQ_Users_Email");

                entity.HasOne(u => u.Role)
                      .WithMany()
                      .HasForeignKey(u => u.RoleId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_Users_Roles_RoleId");
            });
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employees");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                      .UseIdentityColumn();

                entity.Property(e => e.Code)
                      .IsRequired()
                      .HasMaxLength(20)
                      .HasColumnType("varchar(20)");

                entity.Property(e => e.FirstName)
                      .IsRequired()
                      .HasMaxLength(50)
                      .HasColumnType("nvarchar(50)");

                entity.Property(e => e.LastName)
                      .IsRequired()
                      .HasMaxLength(50)
                      .HasColumnType("nvarchar(50)");

                entity.Property(e => e.ContractDate)
                      .IsRequired()
                      .HasColumnType("datetime2");

                entity.Property(e => e.ExitDate)
                      .IsRequired(false)
                      .HasColumnType("datetime2");

                entity.Property(e => e.State)
                      .IsRequired()
                      .HasConversion(
                          s => s.ToString(),
                          s => (EmployeeStatus)Enum.Parse(typeof(EmployeeStatus), s))
                      .HasMaxLength(20)
                      .HasColumnType("varchar(20)");

                entity.Ignore(e => e.ContractDuration);
                entity.Ignore(e => e.FullName);

                entity.HasIndex(e => e.Code)
                      .IsUnique()
                      .HasDatabaseName("UQ_Employees_Code");

                entity.HasOne(e => e.Position)
                      .WithMany()
                      .HasForeignKey(e => e.PositionId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_Employees_Positions_PositionId");

                entity.HasOne(e => e.Municipality)
                      .WithMany()
                      .HasForeignKey(e => e.MunicipalityId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_Employees_Municipalities_MunicipalityId");
            });
            modelBuilder.Entity<Document>(entity =>
            {
                entity.ToTable("Documents");

                entity.HasKey(d => d.Id);
                entity.Property(d => d.Id)
                      .UseIdentityColumn();

                entity.Property(d => d.DocumentNumber)
                      .IsRequired()
                      .HasMaxLength(20)
                      .HasColumnType("varchar(20)");

                entity.Property(d => d.EmissionDate)
                      .IsRequired()
                      .HasColumnType("datetime2");

                entity.Property(d => d.Proprietary)
                      .IsRequired()
                      .HasMaxLength(50)
                      .HasColumnType("nvarchar(50)");

                entity.Property(d => d.Details)
                      .IsRequired()
                      .HasMaxLength(100)
                      .HasColumnType("nvarchar(100)");

                entity.Property(d => d.State)
                      .IsRequired()
                      .HasConversion(
                          s => s.ToString(),
                          s => (DocumentStatus)Enum.Parse(typeof(DocumentStatus), s))
                      .HasMaxLength(20)
                      .HasColumnType("varchar(20)");

                entity.HasIndex(d => d.DocumentNumber)
                      .IsUnique()
                      .HasDatabaseName("UQ_Documents_DocumentNumber");

                entity.HasOne(d => d.DocumentType)
                      .WithMany()
                      .HasForeignKey(d => d.DocumentTypeId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_Documents_DocumentTypes_DocumentTypeId");

                entity.HasOne(d => d.Municipality)
                      .WithMany()
                      .HasForeignKey(d => d.MunicipalityId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_Documents_Municipalities_MunicipalityId");
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
