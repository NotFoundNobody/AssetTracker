using AssetTracker.Core.Entities;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssetTracker.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<EquipmentStatus> EquipmentStatuses { get; set; }
        public DbSet<TransactionMode> TransactionModes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigurePerson(modelBuilder);
            ConfigureEquipment(modelBuilder);
            ConfigureTransaction(modelBuilder);

            ConfigureLookups(modelBuilder);
            ConfigureUser(modelBuilder);

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(x => new { x.UserID, x.RoleID });

                entity.HasOne(x => x.User)
                      .WithMany(x => x.UserRoles)
                      .HasForeignKey(x => x.UserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Role)
                      .WithMany(x => x.UserRoles)
                      .HasForeignKey(x => x.RoleID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(x => new { x.RoleID, x.PermissionID });

                entity.HasOne(x => x.Role)
                      .WithMany(x => x.RolePermissions)
                      .HasForeignKey(x => x.RoleID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Permission)
                      .WithMany(x => x.RolePermissions)
                      .HasForeignKey(x => x.PermissionID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

           

            modelBuilder.Entity<Role>()
                .HasIndex(x => x.Name)
                .IsUnique();

            modelBuilder.Entity<Permission>()
                .HasIndex(x => x.Name)
                .IsUnique();

        }
        private void ConfigureUser(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<User>();

            entity.HasKey(u => u.UserID);

            entity.Property(u => u.Username)
                .IsRequired();
            entity.HasIndex(u => u.Username)
.IsUnique();
            entity.HasQueryFilter(u => !u.IsDeleted);
            entity.HasOne(u => u.Person)
              .WithMany()
              .HasForeignKey(u => u.PersonID)
              .OnDelete(DeleteBehavior.Restrict);
        }
        private void ConfigurePerson(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Person>();

            entity.HasKey(p => p.PersonID);

            entity.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(p => p.NationaleCode)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(p => p.CellPhone)
                .IsRequired()
                .HasMaxLength(11);

            entity.HasIndex(p => p.NationaleCode)
                .IsUnique();

            entity.HasIndex(p => p.CellPhone)
                .IsUnique();

            entity.HasQueryFilter(p => !p.IsDeleted);
        }

        private void ConfigureEquipment(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Equipment>();

            entity.HasKey(e => e.EquipmentID);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.HasIndex(e => e.EquipmentAssetNumber)
                .IsUnique();

            entity.HasOne(e => e.Type)
                .WithMany()
                .HasForeignKey(e => e.EquipmentTypeID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Status)
                .WithMany()
                .HasForeignKey(e => e.EquipmentStatusID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(e => !e.IsDeleted);
        }

        private void ConfigureTransaction(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Transaction>();

            entity.HasKey(t => t.TransactionID);

            entity.Property(t => t.Description)
                .HasMaxLength(2000);

            entity.HasOne(t => t.Person)
                .WithMany()
                .HasForeignKey(t => t.PersonID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Equipment)
                .WithMany()
                .HasForeignKey(t => t.EquipmentID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.TransactionMode)
                .WithMany()
                .HasForeignKey(t => t.TransactionModeID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Status)
                .WithMany()
                .HasForeignKey(t => t.EquipmentStatusID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(t => !t.IsDeleted);
        }

        private void ConfigureLookups(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EquipmentType>()
                .HasKey(e => e.EquipmentTypeID);

            modelBuilder.Entity<EquipmentStatus>()
                .HasKey(e => e.EquipmentStatusID);

            modelBuilder.Entity<TransactionMode>()
                .HasKey(e => e.TransactionModeID);
        }

        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker
                .Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.DateModified = DateTime.Now;
                }

                if (entry.State == EntityState.Added)
                {
                    entry.Entity.DateCreate = DateTime.Now;
                }
            }
        }
    }
}
