using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;
using Xunit;

namespace NPPContractManagement.API.Tests
{
    public class UserManufacturerRepositoryTests
    {
        private static ApplicationDbContext CreateInMemoryContext(string dbName)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;

            var ctx = new ApplicationDbContext(options);
            ctx.Database.EnsureCreated();

            // Ensure the junction table exists (provider-agnostic)
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS UserManufacturers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    ManufacturerId INTEGER NOT NULL,
                    AssignedDate TEXT NULL,
                    AssignedBy TEXT NULL
                );";
                cmd.ExecuteNonQuery();
            }

            // Seed minimal required references
            if (!ctx.Users.Any())
            {
                ctx.Users.AddRange(
                    new User { Id = 1, UserId = "u1", FirstName = "A", LastName = "B", Email = "a@b.com", PasswordHash = "x" },
                    new User { Id = 2, UserId = "u2", FirstName = "C", LastName = "D", Email = "c@d.com", PasswordHash = "x" }
                );
            }
            if (!ctx.Manufacturers.Any())
            {
                ctx.Manufacturers.AddRange(
                    new Manufacturer { Id = 10, Name = "M1" },
                    new Manufacturer { Id = 20, Name = "M2" },
                    new Manufacturer { Id = 30, Name = "M3" }
                );
            }
            ctx.SaveChanges();
            return ctx;
        }

        private static List<(int UserId, int ManufacturerId)> ReadAssignments(ApplicationDbContext ctx)
        {
            using var conn = ctx.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT UserId, ManufacturerId FROM UserManufacturers ORDER BY UserId, ManufacturerId";
            var list = new List<(int, int)>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add((reader.GetInt32(0), reader.GetInt32(1)));
            return list;
        }

        [Fact]
        public async Task Assign_Single_Manufacturer()
        {
            using var ctx = CreateInMemoryContext(nameof(Assign_Single_Manufacturer));
            var repo = new UserManufacturerRepository(ctx);

            await repo.SyncUserManufacturersAsync(1, new[] { 10 }, "tester");

            var rows = ReadAssignments(ctx);
            rows.Should().BeEquivalentTo(new[] { (1, 10) });
        }

        [Fact]
        public async Task Assign_Multiple_Manufacturers()
        {
            using var ctx = CreateInMemoryContext(nameof(Assign_Multiple_Manufacturers));
            var repo = new UserManufacturerRepository(ctx);

            await repo.SyncUserManufacturersAsync(1, new[] { 10, 20, 30 }, "tester");

            var rows = ReadAssignments(ctx);
            rows.Should().BeEquivalentTo(new[] { (1, 10), (1, 20), (1, 30) });
        }

        [Fact]
        public async Task Remove_One_Keep_Others()
        {
            using var ctx = CreateInMemoryContext(nameof(Remove_One_Keep_Others));
            var repo = new UserManufacturerRepository(ctx);

            await repo.SyncUserManufacturersAsync(1, new[] { 10, 20, 30 }, "seed");
            await repo.SyncUserManufacturersAsync(1, new[] { 10, 30 }, "tester");

            var rows = ReadAssignments(ctx);
            rows.Should().BeEquivalentTo(new[] { (1, 10), (1, 30) });
        }

        [Fact]
        public async Task Replace_Full_List()
        {
            using var ctx = CreateInMemoryContext(nameof(Replace_Full_List));
            var repo = new UserManufacturerRepository(ctx);

            await repo.SyncUserManufacturersAsync(1, new[] { 10 }, "seed");
            await repo.SyncUserManufacturersAsync(1, new[] { 20, 30 }, "tester");

            var rows = ReadAssignments(ctx);
            rows.Should().BeEquivalentTo(new[] { (1, 20), (1, 30) });
        }

        [Fact]
        public async Task Idempotent_No_Changes_When_Same_List()
        {
            using var ctx = CreateInMemoryContext(nameof(Idempotent_No_Changes_When_Same_List));
            var repo = new UserManufacturerRepository(ctx);

            await repo.SyncUserManufacturersAsync(1, new[] { 10, 20 }, "seed");
            var before = ReadAssignments(ctx);
            await repo.SyncUserManufacturersAsync(1, new[] { 10, 20 }, "seed");
            var after = ReadAssignments(ctx);

            after.Should().BeEquivalentTo(before);
        }

        [Fact]
        public async Task Removing_All_Clears_Assignments()
        {
            using var ctx = CreateInMemoryContext(nameof(Removing_All_Clears_Assignments));
            var repo = new UserManufacturerRepository(ctx);

            await repo.SyncUserManufacturersAsync(1, new[] { 10, 20 }, "seed");
            await repo.SyncUserManufacturersAsync(1, Array.Empty<int>(), "tester");

            var rows = ReadAssignments(ctx);
            rows.Should().BeEmpty();
        }
    }
}

