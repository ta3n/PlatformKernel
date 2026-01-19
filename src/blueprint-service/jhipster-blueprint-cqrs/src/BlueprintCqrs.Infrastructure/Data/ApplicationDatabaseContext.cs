using BlueprintCqrs.Domain.Entities;
using BlueprintCqrs.Domain.Entities.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlueprintCqrs.Infrastructure.Data;

public class ApplicationDatabaseContext(
    DbContextOptions<ApplicationDatabaseContext> options,
    IHttpContextAccessor httpContextAccessor
) : IdentityDbContext<
    User, Role, string,
    IdentityUserClaim<string>,
    UserRole,
    IdentityUserLogin<string>,
    IdentityRoleClaim<string>,
    IdentityUserToken<string>
>(options)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    protected override void OnModelCreating(
        ModelBuilder builder
    )
    {
        base.OnModelCreating(builder);

        // Rename AspNet default tables
        builder.Entity<User>().ToTable("Users");
        builder.Entity<Role>().ToTable("Roles");
        builder.Entity<UserRole>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

        builder.Entity<UserRole>(
            userRole =>
            {
                userRole.HasKey(
                    ur => new
                    {
                        ur.UserId,
                        ur.RoleId
                    }
                );

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            }
        );

        builder.Entity<User>()
            .HasMany(e => e.UserRoles)
            .WithOne()
            .HasForeignKey(e => e.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }

    /// <summary>
    /// SaveChangesAsync with entities audit
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default
    )
    {
        var entries = ChangeTracker
            .Entries()
            .Where(
                e => e is { Entity: IAuditedEntityBase, State: EntityState.Added or EntityState.Modified }
            );

        var modifiedOrCreatedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                ((IAuditedEntityBase)entityEntry.Entity).CreatedDate = DateTime.Now;
                ((IAuditedEntityBase)entityEntry.Entity).CreatedBy = modifiedOrCreatedBy;
            }
            else
            {
                Entry((IAuditedEntityBase)entityEntry.Entity).Property(p => p.CreatedDate).IsModified = false;
                Entry((IAuditedEntityBase)entityEntry.Entity).Property(p => p.CreatedBy).IsModified = false;
            }

            ((IAuditedEntityBase)entityEntry.Entity).LastModifiedDate = DateTime.Now;
            ((IAuditedEntityBase)entityEntry.Entity).LastModifiedBy = modifiedOrCreatedBy;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
