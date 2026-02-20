using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace NPPContractManagement.API.Data
{
    // Design-time factory to allow EF Core tools to create the DbContext
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Read connection string from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");
            }

            // Set a fixed MySQL server version to avoid AutoDetect (which connects).
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));

            optionsBuilder.UseMySql(connectionString, serverVersion);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}

