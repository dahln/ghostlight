using depot.Server.Entities;
using depot.Server.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace depot.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupAuthorizedUser> GroupAuthorizedUsers { get; set; }
        public DbSet<InstanceType> InstanceTypes { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<InstanceLink> InstanceLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GroupAuthorizedUser>()
                .HasOne(c => c.Group)
                .WithMany(c => c.AuthorizedUsers)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InstanceLink>()
                .HasOne(c => c.Group)
                .WithMany(c => c.InstanceLinks)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InstanceType>()
                .HasOne(c => c.Group)
                .WithMany(c => c.InstanceTypes)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Field>()
                .HasOne(c => c.InstanceType)
                .WithMany(c => c.Fields)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
