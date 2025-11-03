using Aptiverse.Domain.Models.Admins;
using Aptiverse.Domain.Models.Auth;
using Aptiverse.Domain.Models.Junctions;
using Aptiverse.Domain.Models.Parents;
using Aptiverse.Domain.Models.Students;
using Aptiverse.Domain.Models.Superusers;
using Aptiverse.Domain.Models.Teachers;
using Aptiverse.Domain.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aptiverse.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Superuser> Superusers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<BlacklistedToken> BlacklistedTokens { get; set; }

        // Junction tables
        public DbSet<StudentParent> StudentParents { get; set; }
        public DbSet<StudentTeacher> StudentTeachers { get; set; }
        public DbSet<StudentAdmin> StudentAdmins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<BlacklistedToken>(entity =>
            {
                entity.HasKey(e => e.TokenHash);
                entity.HasIndex(e => e.Expiry);
            });

            modelBuilder.Entity<Superuser>()
                .HasOne(a => a.User)
                .WithOne()
                .HasForeignKey<Superuser>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Admin>()
                .HasOne(a => a.User)
                .WithOne()
                .HasForeignKey<Admin>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Parent>()
                .HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<Parent>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.User)
                .WithOne()
                .HasForeignKey<Teacher>(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne()
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentParent>()
                .HasKey(sp => new { sp.StudentId, sp.ParentId });

            modelBuilder.Entity<StudentParent>()
                .HasOne(sp => sp.Student)
                .WithMany(s => s.StudentParents)
                .HasForeignKey(sp => sp.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentParent>()
                .HasOne(sp => sp.Parent)
                .WithMany(p => p.StudentParents)
                .HasForeignKey(sp => sp.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentTeacher>()
                .HasKey(st => new { st.StudentId, st.TeacherId });

            modelBuilder.Entity<StudentTeacher>()
                .HasOne(st => st.Student)
                .WithMany(s => s.StudentTeachers)
                .HasForeignKey(st => st.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentTeacher>()
                .HasOne(st => st.Teacher)
                .WithMany(t => t.StudentTeachers)
                .HasForeignKey(st => st.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentAdmin>()
                .HasKey(sa => new { sa.StudentId, sa.AdminId });

            modelBuilder.Entity<StudentAdmin>()
                .HasOne(sa => sa.Student)
                .WithMany(s => s.StudentAdmins)
                .HasForeignKey(sa => sa.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentAdmin>()
                .HasOne(sa => sa.Admin)
                .WithMany(a => a.StudentAdmins)
                .HasForeignKey(sa => sa.AdminId)
                .OnDelete(DeleteBehavior.Cascade);

            // PostgreSQL specific configuration
            if (Database.IsNpgsql())
            {
                modelBuilder.HasPostgresExtension("uuid-ossp");
                modelBuilder.HasPostgresExtension("pgcrypto");

                modelBuilder.Model.GetEntityTypes().ToList().ForEach(e =>
                {
                    modelBuilder.Entity(e.ClrType)
                        .Property<uint>("xmin")
                        .HasColumnName("xmin")
                        .HasColumnType("xid")
                        .ValueGeneratedOnAddOrUpdate()
                        .IsConcurrencyToken();
                });
            }
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<string>()
                .HaveMaxLength(255)
                .AreUnicode(false);
        }
    }
}