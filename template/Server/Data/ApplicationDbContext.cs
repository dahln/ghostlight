using template.Server.Entities;
using template.Server.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace template.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        public DbSet<Folder> Folders { get; set; }
        public DbSet<FolderAuthorizedUser> FolderAuthorizedUsers { get; set; }
        public DbSet<DataType> DataTypes { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<Instance> Instances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<FolderAuthorizedUser>()
                .HasOne(c => c.Folder)
                .WithMany(c => c.AuthorizedUsers)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DataType>()
                .HasOne(c => c.Folder)
                .WithMany(c => c.DataTypes)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Field>()
                .HasOne(c => c.DataType)
                .WithMany(c => c.Fields)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Instance>()
                .HasOne(c => c.DataType)
                .WithMany(c => c.Instances)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
