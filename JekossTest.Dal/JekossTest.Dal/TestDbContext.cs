using System;
using JekossTest.Dal.Common;
using JekossTest.Dal.Configurations;
using JekossTest.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


namespace JekossTest.Dal
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> contextOptions) : base(contextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.AddConfiguration(new UserConfiguration());
            modelBuilder.AddConfiguration(new RoleConfiguration());
            
            modelBuilder.Entity<Role>().HasData(new Role() {Id = 1, RoleName = "User"},
                new Role() {Id = 2, RoleName = "Admin"});
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<AccountRefreshToken> AccountRefreshTokens { get; set; }
    }
    
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TestDbContext>
    {
        public TestDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Settings/appsettings.development.json")
                .Build();

            var builder = new DbContextOptionsBuilder<TestDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseSqlServer(connectionString);

            return new TestDbContext(builder.Options);
        }
    }
}