using Aptiverse.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aptiverse.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User>(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(b =>
            {
                b.ToTable("Users", "Identity");
                b.HasKey(u => u.Id);

                b.HasIndex(u => u.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();
                b.HasIndex(u => u.NormalizedEmail).HasDatabaseName("EmailIndex");

                b.Property(u => u.Id).HasMaxLength(255).IsUnicode(false);
                b.Property(u => u.UserName).HasMaxLength(256).IsUnicode(false);
                b.Property(u => u.NormalizedUserName).HasMaxLength(256).IsUnicode(false);
                b.Property(u => u.Email).HasMaxLength(256).IsUnicode(false);
                b.Property(u => u.NormalizedEmail).HasMaxLength(256).IsUnicode(false);
                b.Property(u => u.PasswordHash).HasMaxLength(255).IsUnicode(false);
                b.Property(u => u.SecurityStamp).HasMaxLength(255).IsUnicode(false);
                b.Property(u => u.ConcurrencyStamp).HasMaxLength(255).IsUnicode(false);
                b.Property(u => u.PhoneNumber).HasMaxLength(255).IsUnicode(false);
            });

            modelBuilder.Entity<IdentityRole>(b =>
            {
                b.ToTable("Roles", "Identity");
                b.HasKey(r => r.Id);

                b.HasIndex(r => r.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();

                b.Property(r => r.Id).HasMaxLength(255).IsUnicode(false);
                b.Property(r => r.Name).HasMaxLength(256).IsUnicode(false);
                b.Property(r => r.NormalizedName).HasMaxLength(256).IsUnicode(false);
                b.Property(r => r.ConcurrencyStamp).HasMaxLength(255).IsUnicode(false);
            });

            modelBuilder.Entity<IdentityUserRole<string>>(b =>
            {
                b.ToTable("UserRoles", "Identity");
                b.HasKey(r => new { r.UserId, r.RoleId });

                b.Property(r => r.UserId).HasMaxLength(255).IsUnicode(false);
                b.Property(r => r.RoleId).HasMaxLength(255).IsUnicode(false);
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(b =>
            {
                b.ToTable("UserClaims", "Identity");
                b.HasKey(uc => uc.Id);

                b.Property(uc => uc.UserId).HasMaxLength(255).IsUnicode(false);
                b.Property(uc => uc.ClaimType).HasMaxLength(255).IsUnicode(false);
                b.Property(uc => uc.ClaimValue).HasMaxLength(255).IsUnicode(false);
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.ToTable("UserLogins", "Identity");
                b.HasKey(l => new { l.LoginProvider, l.ProviderKey });

                b.Property(l => l.LoginProvider).HasMaxLength(255).IsUnicode(false);
                b.Property(l => l.ProviderKey).HasMaxLength(255).IsUnicode(false);
                b.Property(l => l.UserId).HasMaxLength(255).IsUnicode(false);
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.ToTable("RoleClaims", "Identity");
                b.HasKey(rc => rc.Id);

                b.Property(rc => rc.RoleId).HasMaxLength(255).IsUnicode(false);
                b.Property(rc => rc.ClaimType).HasMaxLength(255).IsUnicode(false);
                b.Property(rc => rc.ClaimValue).HasMaxLength(255).IsUnicode(false);
            });

            modelBuilder.Entity<IdentityUserToken<string>>(b =>
            {
                b.ToTable("UserTokens", "Identity");
                b.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

                b.Property(t => t.UserId).HasMaxLength(255).IsUnicode(false);
                b.Property(t => t.LoginProvider).HasMaxLength(255).IsUnicode(false);
                b.Property(t => t.Name).HasMaxLength(255).IsUnicode(false);
                b.Property(t => t.Value).HasMaxLength(255).IsUnicode(false);
            });

            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasOne<IdentityRole>()
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            modelBuilder.Entity<IdentityUserClaim<string>>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();

            modelBuilder.Entity<IdentityUserLogin<string>>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(ul => ul.UserId)
                .IsRequired();

            modelBuilder.Entity<IdentityRoleClaim<string>>()
                .HasOne<IdentityRole>()
                .WithMany()
                .HasForeignKey(rc => rc.RoleId)
                .IsRequired();

            modelBuilder.Entity<IdentityUserToken<string>>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(ut => ut.UserId)
                .IsRequired();

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