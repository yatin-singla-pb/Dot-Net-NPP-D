using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Scripts
{
    public class CheckTables
    {
        public static async Task Main(string[] args)
        {
            var connectionString = "Server=localhost;Database=NPPContractManagment;User=root;Password=;";
            
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Options;

            using var context = new ApplicationDbContext(options);

            Console.WriteLine("=== DATABASE TABLE VERIFICATION ===\n");

            try
            {
                // Check if database exists and is accessible
                var canConnect = await context.Database.CanConnectAsync();
                Console.WriteLine($"Database Connection: {(canConnect ? "✅ SUCCESS" : "❌ FAILED")}");

                if (!canConnect)
                {
                    Console.WriteLine("Cannot connect to database. Please check connection string.");
                    return;
                }

                // Check each table and count records
                Console.WriteLine("\n=== TABLE VERIFICATION ===");

                // OpCos
                try
                {
                    var opCoCount = await context.OpCos.CountAsync();
                    Console.WriteLine($"✅ OpCos: {opCoCount} records");
                    
                    if (opCoCount > 0)
                    {
                        var firstOpCo = await context.OpCos.FirstAsync();
                        Console.WriteLine($"   Sample: {firstOpCo.Name} (ID: {firstOpCo.Id})");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ OpCos: ERROR - {ex.Message}");
                }

                // MemberAccounts
                try
                {
                    var memberCount = await context.MemberAccounts.CountAsync();
                    Console.WriteLine($"✅ MemberAccounts: {memberCount} records");
                    
                    if (memberCount > 0)
                    {
                        var firstMember = await context.MemberAccounts.FirstAsync();
                        Console.WriteLine($"   Sample: {firstMember.FacilityName} (ID: {firstMember.Id})");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ MemberAccounts: ERROR - {ex.Message}");
                }

                // CustomerAccounts
                try
                {
                    var customerCount = await context.CustomerAccounts.CountAsync();
                    Console.WriteLine($"✅ CustomerAccounts: {customerCount} records");
                    
                    if (customerCount > 0)
                    {
                        var firstCustomer = await context.CustomerAccounts.FirstAsync();
                        Console.WriteLine($"   Sample: {firstCustomer.CustomerName} (ID: {firstCustomer.Id})");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ CustomerAccounts: ERROR - {ex.Message}");
                }

                // Products
                try
                {
                    var productCount = await context.Products.CountAsync();
                    Console.WriteLine($"✅ Products: {productCount} records");
                    
                    if (productCount > 0)
                    {
                        var firstProduct = await context.Products.FirstAsync();
                        Console.WriteLine($"   Sample: {firstProduct.Description} (SKU: {firstProduct.SKU})");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Products: ERROR - {ex.Message}");
                }

                // Manufacturers
                try
                {
                    var mfgCount = await context.Manufacturers.CountAsync();
                    Console.WriteLine($"✅ Manufacturers: {mfgCount} records");
                    
                    if (mfgCount > 0)
                    {
                        var firstMfg = await context.Manufacturers.FirstAsync();
                        Console.WriteLine($"   Sample: {firstMfg.Name} (ID: {firstMfg.Id})");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Manufacturers: ERROR - {ex.Message}");
                }

                // Distributors
                try
                {
                    var distCount = await context.Distributors.CountAsync();
                    Console.WriteLine($"✅ Distributors: {distCount} records");
                    
                    if (distCount > 0)
                    {
                        var firstDist = await context.Distributors.FirstAsync();
                        Console.WriteLine($"   Sample: {firstDist.Name} (ID: {firstDist.Id})");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Distributors: ERROR - {ex.Message}");
                }

                // Industries
                try
                {
                    var indCount = await context.Industries.CountAsync();
                    Console.WriteLine($"✅ Industries: {indCount} records");
                    
                    if (indCount > 0)
                    {
                        var firstInd = await context.Industries.FirstAsync();
                        Console.WriteLine($"   Sample: {firstInd.Name} (ID: {firstInd.Id})");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Industries: ERROR - {ex.Message}");
                }

                Console.WriteLine("\n=== VERIFICATION COMPLETE ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ CRITICAL ERROR: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }
    }
}
