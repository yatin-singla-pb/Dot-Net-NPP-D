using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Distributor> Distributors { get; set; }
        public DbSet<Industry> Industries { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractProduct> ContractProducts { get; set; }
        public DbSet<ContractVersion> ContractVersions { get; set; }
        public DbSet<ContractVersionPrice> ContractVersionPrices { get; set; }
        public DbSet<ContractPrice> ContractPrices { get; set; }
        public DbSet<OpCo> OpCos { get; set; }
        public DbSet<MemberAccount> MemberAccounts { get; set; }
        public DbSet<CustomerAccount> CustomerAccounts { get; set; }
        public DbSet<DistributorProductCode> DistributorProductCodes { get; set; }
        public DbSet<ContractDistributor> ContractDistributors { get; set; }
        public DbSet<ContractOpCo> ContractOpCos { get; set; }
        public DbSet<ContractIndustry> ContractIndustries { get; set; }
        public DbSet<ContractManufacturer> ContractManufacturers { get; set; }

        public DbSet<ContractIndustryVersion> ContractIndustriesVersion { get; set; }
        public DbSet<ContractManufacturerVersion> ContractManufacturersVersion { get; set; }
        public DbSet<ContractOpCoVersion> ContractOpCosVersion { get; set; }
        public DbSet<ContractDistributorVersion> ContractDistributorsVersion { get; set; }
        public DbSet<ContractVersionProduct> ContractVersionProducts { get; set; }


        // Proposals module
        public DbSet<NPPContractManagement.API.Domain.Proposals.Entities.Proposal> Proposals { get; set; }
        public DbSet<NPPContractManagement.API.Domain.Proposals.Entities.ProposalProduct> ProposalProducts { get; set; }
        public DbSet<NPPContractManagement.API.Domain.Proposals.Entities.ProposalDistributor> ProposalDistributors { get; set; }
        public DbSet<NPPContractManagement.API.Domain.Proposals.Entities.ProposalIndustry> ProposalIndustries { get; set; }
        public DbSet<NPPContractManagement.API.Domain.Proposals.Entities.ProposalOpco> ProposalOpcos { get; set; }
        public DbSet<NPPContractManagement.API.Domain.Proposals.Entities.ProposalStatus> ProposalStatuses { get; set; }
        public DbSet<NPPContractManagement.API.Domain.Proposals.Entities.ProposalType> ProposalTypes { get; set; }
        public DbSet<NPPContractManagement.API.Domain.Proposals.Entities.PriceType> PriceTypes { get; set; }
        public DbSet<NPPContractManagement.API.Domain.Proposals.Entities.ProductProposalStatus> ProductProposalStatuses { get; set; }
        public DbSet<NPPContractManagement.API.Domain.Proposals.Entities.AmendmentAction> AmendmentActions { get; set; }
        public DbSet<NPPContractManagement.API.Domain.Proposals.Entities.ProposalStatusHistory> ProposalStatusHistories { get; set; }
        public DbSet<NPPContractManagement.API.Domain.Proposals.Entities.ProposalProductHistory> ProposalProductHistories { get; set; }
        public DbSet<NPPContractManagement.API.Domain.Proposals.Entities.ProposalBatchJob> ProposalBatchJobs { get; set; }

        // Registration module
        public DbSet<RegistrationVerification> RegistrationVerifications { get; set; }
        public DbSet<InvitationEmail> InvitationEmails { get; set; }

        // Velocity module
        public DbSet<IngestedFile> IngestedFiles { get; set; }
        public DbSet<VelocityJob> VelocityJobs { get; set; }
        public DbSet<VelocityJobData> VelocityJobData { get; set; }
        public DbSet<VelocityShipment> VelocityShipments { get; set; }
        public DbSet<VelocityJobRow> VelocityJobRows { get; set; }
        public DbSet<VelocityError> VelocityErrors { get; set; }
        public DbSet<SftpProbeConfig> SftpProbeConfigs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure unique constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserId)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            // Align Contract mapping to latest schema: ignore deprecated columns
            modelBuilder.Entity<Contract>().Ignore(c => c.Title);
            modelBuilder.Entity<Contract>().Ignore(c => c.Status);
            modelBuilder.Entity<Contract>().Ignore(c => c.TotalValue);
            modelBuilder.Entity<Contract>().Ignore(c => c.Notes);
            modelBuilder.Entity<Contract>().Ignore(c => c.ManufacturerId);
            modelBuilder.Entity<Contract>().Ignore(c => c.Manufacturer);


            // Optional relationship: Contract -> Proposal (source of creation)
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Proposal)
                .WithMany()
                .HasForeignKey(c => c.ProposalId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure composite unique constraint for UserRole
            modelBuilder.Entity<UserRole>()
                .HasIndex(ur => new { ur.UserId, ur.RoleId })
                .IsUnique();

            // Configure unique constraints for new entities
            modelBuilder.Entity<MemberAccount>()
                .HasIndex(ma => ma.MemberNumber)
                .IsUnique();

            modelBuilder.Entity<CustomerAccount>()
                .HasIndex(ca => new { ca.DistributorId, ca.CustomerAccountNumber })
                .IsUnique();

            modelBuilder.Entity<DistributorProductCode>()
                .HasIndex(dpc => new { dpc.DistributorId, dpc.DistributorCode })
                .IsUnique();

            // Optional business rule: single product mapping per distributor
            modelBuilder.Entity<DistributorProductCode>()
                .HasIndex(dpc => new { dpc.DistributorId, dpc.ProductId })
                .IsUnique();

            // Configure composite unique constraints for junction tables
            modelBuilder.Entity<ContractDistributor>()
                .HasIndex(cd => new { cd.ContractId, cd.DistributorId })
                .IsUnique();

            modelBuilder.Entity<ContractOpCo>()
                .HasIndex(co => new { co.ContractId, co.OpCoId })
                .IsUnique();

            modelBuilder.Entity<ContractIndustry>()
                .HasIndex(ci => new { ci.ContractId, ci.IndustryId })
                .IsUnique();

            modelBuilder.Entity<ContractManufacturer>()
                .HasIndex(cm => new { cm.ContractId, cm.ManufacturerId })
                .IsUnique();


            // Manufacturer -> PrimaryBroker (User) optional relationship
            modelBuilder.Entity<Manufacturer>()
                .HasOne(m => m.PrimaryBroker)
                .WithMany()
                .HasForeignKey(m => m.PrimaryBrokerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ContractVersion>()
                .HasIndex(cv => new { cv.ContractId, cv.VersionNumber })
                .IsUnique();
            // Explicit table mapping to match existing database table names
            modelBuilder.Entity<ContractVersionPrice>().ToTable("ContractVersionPrice");



            // ContractVersionPrice mapping to existing table structure
            modelBuilder.Entity<ContractVersionPrice>().ToTable("ContractVersionPrice");
            modelBuilder.Entity<ContractVersionPrice>()
                .HasIndex(x => new { x.ContractId, x.PriceId, x.VersionNumber });
            modelBuilder.Entity<ContractVersionPrice>()
                .HasIndex(x => x.PriceId);
            modelBuilder.Entity<ContractVersionPrice>()
                .HasOne(x => x.Contract)
                .WithMany()
                .HasForeignKey(x => x.ContractId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ContractVersionPrice>()
                .HasOne(x => x.ContractPrice)
                .WithMany()
                .HasForeignKey(x => x.PriceId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ContractVersionPrice>()
                .HasIndex(x => new { x.ContractId, x.ProductId, x.VersionNumber });
            modelBuilder.Entity<ContractVersionPrice>()
                .HasIndex(x => x.ProductId);
            modelBuilder.Entity<ContractVersionPrice>()
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);


            // ContractPrice config
            modelBuilder.Entity<ContractPrice>()
                .HasIndex(cp => new { cp.ProductId, cp.VersionNumber });

            modelBuilder.Entity<ContractPrice>()
                .HasOne(cp => cp.Product)
                .WithMany()
                .HasForeignKey(cp => cp.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContractPrice>()
                .HasOne(cp => cp.Contract)
                .WithMany(c => c.ContractPrices)
                .HasForeignKey(cp => cp.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContractPrice>()
                .HasIndex(cp => new { cp.ContractId, cp.VersionNumber });

            // Contract Version Relationship Tables
            modelBuilder.Entity<ContractIndustryVersion>()
                .HasIndex(x => new { x.ContractId, x.IndustryId, x.VersionNumber });
            modelBuilder.Entity<ContractIndustryVersion>()
                .HasOne(x => x.Contract)
                .WithMany()
                .HasForeignKey(x => x.ContractId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ContractIndustryVersion>()
                .HasOne(x => x.Industry)
                .WithMany()
                .HasForeignKey(x => x.IndustryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContractManufacturerVersion>()
                .HasIndex(x => new { x.ContractId, x.ManufacturerId, x.VersionNumber });
            modelBuilder.Entity<ContractManufacturerVersion>()
                .HasOne(x => x.Contract)
                .WithMany()
                .HasForeignKey(x => x.ContractId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ContractManufacturerVersion>()
                .HasOne(x => x.Manufacturer)
                .WithMany()
                .HasForeignKey(x => x.ManufacturerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContractOpCoVersion>()
                .HasIndex(x => new { x.ContractId, x.OpCoId, x.VersionNumber });
            modelBuilder.Entity<ContractOpCoVersion>()
                .HasOne(x => x.Contract)
                .WithMany()
                .HasForeignKey(x => x.ContractId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ContractOpCoVersion>()
                .HasOne(x => x.OpCo)
                .WithMany()
                .HasForeignKey(x => x.OpCoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContractDistributorVersion>()
                .HasIndex(x => new { x.ContractId, x.DistributorId, x.VersionNumber });
            modelBuilder.Entity<ContractDistributorVersion>()
                .HasOne(x => x.Contract)
                .WithMany()
                .HasForeignKey(x => x.ContractId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ContractDistributorVersion>()
                .HasOne(x => x.Distributor)
                .WithMany()
                .HasForeignKey(x => x.DistributorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContractVersionProduct>()
                .HasIndex(x => new { x.ContractId, x.ProductId, x.VersionNumber });
            modelBuilder.Entity<ContractVersionProduct>()
                .HasOne(x => x.Contract)
                .WithMany()
                .HasForeignKey(x => x.ContractId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ContractVersionProduct>()
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);


            // Seed default roles
            var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "System Administrator", Description = "Full system access", CreatedBy = "System", CreatedDate = seedDate },
                new Role { Id = 2, Name = "Contract Manager", Description = "Manage contracts and related data", CreatedBy = "System", CreatedDate = seedDate },
                new Role { Id = 3, Name = "Manufacturer", Description = "Manufacturer user access", CreatedBy = "System", CreatedDate = seedDate },
                new Role { Id = 4, Name = "Headless", Description = "Headless contact with no login access", CreatedBy = "System", CreatedDate = seedDate },
                new Role { Id = 5, Name = "Contract Viewer", Description = "View contracts and run reports", CreatedBy = "System", CreatedDate = seedDate }
            );

            // Seed default industries
            modelBuilder.Entity<Industry>().HasData(
                new Industry { Id = 1, Name = "College & University", Description = "Higher education institutions", CreatedBy = "System", CreatedDate = seedDate, Status = IndustryStatus.Active, IsActive = true },
                new Industry { Id = 2, Name = "K-12", Description = "Primary and secondary education", CreatedBy = "System", CreatedDate = seedDate, Status = IndustryStatus.Active, IsActive = true },
                new Industry { Id = 3, Name = "Quick Serve Restaurant", Description = "Fast food and quick service restaurants", CreatedBy = "System", CreatedDate = seedDate, Status = IndustryStatus.Active, IsActive = true },
                new Industry { Id = 4, Name = "Healthcare", Description = "Hospitals and healthcare facilities", CreatedBy = "System", CreatedDate = seedDate, Status = IndustryStatus.Active, IsActive = true },
                new Industry { Id = 5, Name = "Corporate", Description = "Corporate dining and cafeterias", CreatedBy = "System", CreatedDate = seedDate, Status = IndustryStatus.Active, IsActive = true }
            );

            // Seed sample manufacturers
            modelBuilder.Entity<Manufacturer>().HasData(
                new Manufacturer { Id = 1, Name = "Sysco Corporation", AKA = "Sysco", Status = ManufacturerStatus.Active, Address = "1390 Enclave Pkwy, Houston, TX 77077", PhoneNumber = "(281) 584-1390", CreatedBy = "System", CreatedDate = seedDate, IsActive = true },
                new Manufacturer { Id = 2, Name = "US Foods", AKA = "USF", Status = ManufacturerStatus.Active, Address = "9399 W Higgins Rd, Rosemont, IL 60018", PhoneNumber = "(847) 720-8000", CreatedBy = "System", CreatedDate = seedDate, IsActive = true },
                new Manufacturer { Id = 3, Name = "Performance Food Group", AKA = "PFG", Status = ManufacturerStatus.Active, Address = "12500 West Creek Pkwy, Richmond, VA 23238", PhoneNumber = "(804) 484-7700", CreatedBy = "System", CreatedDate = seedDate, IsActive = true }
            );

            // Seed sample distributors
            modelBuilder.Entity<Distributor>().HasData(
                new Distributor { Id = 1, Name = "Regional Food Services", Status = DistributorStatus.Active, Address = "123 Distribution Way, Chicago, IL 60601", PhoneNumber = "(312) 555-0100", ReceiveContractProposal = true, CreatedBy = "System", CreatedDate = seedDate, IsActive = true },
                new Distributor { Id = 2, Name = "Metro Food Distribution", Status = DistributorStatus.Active, Address = "456 Supply Chain Blvd, Atlanta, GA 30309", PhoneNumber = "(404) 555-0200", ReceiveContractProposal = true, CreatedBy = "System", CreatedDate = seedDate, IsActive = true },
                new Distributor { Id = 3, Name = "National Food Partners", Status = DistributorStatus.Active, Address = "789 Logistics Ave, Dallas, TX 75201", PhoneNumber = "(214) 555-0300", ReceiveContractProposal = false, CreatedBy = "System", CreatedDate = seedDate, IsActive = true }
            );

            // Seed sample OpCos
            modelBuilder.Entity<OpCo>().HasData(
                new OpCo { Id = 1, Name = "Chicago Operations", RemoteReferenceCode = "CHI001", DistributorId = 1, Address = "100 N Michigan Ave, Chicago, IL 60601", City = "Chicago", State = "IL", ZipCode = "60601", Country = "USA", PhoneNumber = "(312) 555-1001", Email = "chicago@regionalfood.com", ContactPerson = "Sarah Wilson", Status = OpCoStatus.Active, IsActive = true, CreatedBy = "System", CreatedDate = seedDate },
                new OpCo { Id = 2, Name = "Atlanta Hub", RemoteReferenceCode = "ATL001", DistributorId = 2, Address = "200 Peachtree St, Atlanta, GA 30303", City = "Atlanta", State = "GA", ZipCode = "30303", Country = "USA", PhoneNumber = "(404) 555-2001", Email = "atlanta@metrofood.com", ContactPerson = "Robert Brown", Status = OpCoStatus.Active, IsActive = true, CreatedBy = "System", CreatedDate = seedDate },
                new OpCo { Id = 3, Name = "Dallas Center", RemoteReferenceCode = "DAL001", DistributorId = 3, Address = "300 Main St, Dallas, TX 75202", City = "Dallas", State = "TX", ZipCode = "75202", Country = "USA", PhoneNumber = "(214) 555-3001", Email = "dallas@nationalfood.com", ContactPerson = "Lisa Garcia", Status = OpCoStatus.Active, IsActive = true, CreatedBy = "System", CreatedDate = seedDate },
                new OpCo { Id = 4, Name = "Chicago West", RemoteReferenceCode = "CHI002", DistributorId = 1, Address = "400 W Lake St, Chicago, IL 60606", City = "Chicago", State = "IL", ZipCode = "60606", Country = "USA", PhoneNumber = "(312) 555-1002", Email = "west@regionalfood.com", ContactPerson = "David Miller", Status = OpCoStatus.Active, IsActive = true, CreatedBy = "System", CreatedDate = seedDate }
            );

            // Seed sample Member Accounts
            modelBuilder.Entity<MemberAccount>().HasData(
                new MemberAccount { Id = 1, MemberNumber = "MEM001", FacilityName = "University of Chicago Dining", Address = "5801 S Ellis Ave, Chicago, IL 60637", City = "Chicago", State = "IL", ZipCode = "60637", Country = "USA", PhoneNumber = "(773) 702-1234", IndustryId = 1, W9 = "Submitted", TaxId = "36-1234567", BusinessType = "Educational Institution", Status = MemberAccountStatus.Active, IsActive = true, CreatedBy = "System", CreatedDate = seedDate },
                new MemberAccount { Id = 2, MemberNumber = "MEM002", FacilityName = "Lincoln Elementary School", Address = "123 School St, Atlanta, GA 30309", City = "Atlanta", State = "GA", ZipCode = "30309", Country = "USA", PhoneNumber = "(404) 555-5000", IndustryId = 2, W9 = "Submitted", TaxId = "58-9876543", BusinessType = "Public School", Status = MemberAccountStatus.Active, IsActive = true, CreatedBy = "System", CreatedDate = seedDate },
                new MemberAccount { Id = 3, MemberNumber = "MEM003", FacilityName = "Quick Bite Restaurant", Address = "456 Fast Food Blvd, Dallas, TX 75201", City = "Dallas", State = "TX", ZipCode = "75201", Country = "USA", PhoneNumber = "(214) 555-6000", IndustryId = 3, W9 = "Not Submitted", TaxId = "75-1122334", BusinessType = "Restaurant", Status = MemberAccountStatus.Active, IsActive = true, CreatedBy = "System", CreatedDate = seedDate },
                new MemberAccount { Id = 4, MemberNumber = "MEM004", FacilityName = "General Hospital Cafeteria", Address = "789 Health Way, Chicago, IL 60611", City = "Chicago", State = "IL", ZipCode = "60611", Country = "USA", PhoneNumber = "(312) 555-7000", IndustryId = 4, W9 = "Submitted", TaxId = "36-5566778", BusinessType = "Healthcare Facility", Status = MemberAccountStatus.Active, IsActive = true, CreatedBy = "System", CreatedDate = seedDate }
            );

            // Seed sample Customer Accounts
            modelBuilder.Entity<CustomerAccount>().HasData(
                new CustomerAccount { Id = 1, MemberAccountId = 1, DistributorId = 1, OpCoId = 1, CustomerName = "UChicago Main Dining", CustomerAccountNumber = "CUST001", Address = "5801 S Ellis Ave, Chicago, IL 60637", City = "Chicago", State = "IL", ZipCode = "60637", Country = "USA", PhoneNumber = "(773) 702-1234", Email = "dining@uchicago.edu", Status = CustomerAccountStatus.Active, IsActive = true, CreatedBy = "System", CreatedDate = seedDate },
                new CustomerAccount { Id = 2, MemberAccountId = 2, DistributorId = 2, OpCoId = 2, CustomerName = "Lincoln School Cafeteria", CustomerAccountNumber = "CUST002", Address = "123 School St, Atlanta, GA 30309", City = "Atlanta", State = "GA", ZipCode = "30309", Country = "USA", PhoneNumber = "(404) 555-5000", Email = "cafeteria@lincoln.edu", Status = CustomerAccountStatus.Active, IsActive = true, CreatedBy = "System", CreatedDate = seedDate },
                new CustomerAccount { Id = 3, MemberAccountId = 3, DistributorId = 3, OpCoId = 3, CustomerName = "Quick Bite Main Location", CustomerAccountNumber = "CUST003", Address = "456 Fast Food Blvd, Dallas, TX 75201", City = "Dallas", State = "TX", ZipCode = "75201", Country = "USA", PhoneNumber = "(214) 555-6000", Email = "manager@quickbite.com", Status = CustomerAccountStatus.Active, IsActive = true, CreatedBy = "System", CreatedDate = seedDate },
                new CustomerAccount { Id = 4, MemberAccountId = 4, DistributorId = 1, OpCoId = 4, CustomerName = "General Hospital Food Service", CustomerAccountNumber = "CUST004", Address = "789 Health Way, Chicago, IL 60611", City = "Chicago", State = "IL", ZipCode = "60611", Country = "USA", PhoneNumber = "(312) 555-7000", Email = "food@generalhospital.org", Status = CustomerAccountStatus.Active, IsActive = true, CreatedBy = "System", CreatedDate = seedDate }
            );

            // Seed sample Products
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, SKU = "SYS001", ManufacturerProductCode = "SYSCO-BEEF-001", GTIN = "1234567890123", UPC = "123456789012", Description = "Premium Ground Beef 80/20", PackSize = "10 lb", ManufacturerId = 1, Category = "Meat", SubCategory = "Ground Beef", Status = ProductStatus.Active, IsActive = true, CreatedBy = "System", CreatedDate = seedDate },
                new Product { Id = 2, SKU = "USF001", ManufacturerProductCode = "USF-CHICKEN-001", GTIN = "2345678901234", UPC = "234567890123", Description = "Boneless Chicken Breast", PackSize = "5 lb", ManufacturerId = 2, Category = "Meat", SubCategory = "Chicken", Status = ProductStatus.Active, IsActive = true, CreatedBy = "System", CreatedDate = seedDate },
                new Product { Id = 3, SKU = "PFG001", ManufacturerProductCode = "PFG-PRODUCE-001", GTIN = "3456789012345", UPC = "345678901234", Description = "Fresh Romaine Lettuce", PackSize = "24 count", ManufacturerId = 3, Category = "Produce", SubCategory = "Lettuce", Status = ProductStatus.Active, IsActive = true, CreatedBy = "System", CreatedDate = seedDate },
                new Product { Id = 4, SKU = "SYS002", ManufacturerProductCode = "SYSCO-DAIRY-001", GTIN = "4567890123456", UPC = "456789012345", Description = "Whole Milk Gallon", PackSize = "4 gallons", ManufacturerId = 1, Category = "Dairy", SubCategory = "Milk", Status = ProductStatus.Active, IsActive = true, CreatedBy = "System", CreatedDate = seedDate },
                new Product { Id = 5, SKU = "USF002", ManufacturerProductCode = "USF-FROZEN-001", GTIN = "5678901234567", UPC = "567890123456", Description = "Frozen French Fries", PackSize = "6/5 lb", ManufacturerId = 2, Category = "Frozen", SubCategory = "Potato Products", Status = ProductStatus.Active, IsActive = true, CreatedBy = "System", CreatedDate = seedDate }
            );

            // Seed sample Contracts
            modelBuilder.Entity<Contract>().HasData(
                new Contract { Id = 1, Title = "University Food Service Contract 2025", StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 12, 31), ManufacturerId = 1, Status = ContractStatus.Active, TotalValue = 500000.00m, IsSuspended = false, SuspendedDate = null, ForeignContractID = null, SendToPerformance = true, CreatedBy = "System", CreatedDate = seedDate },
                new Contract { Id = 2, Title = "K-12 School District Agreement", StartDate = new DateTime(2025, 2, 1), EndDate = new DateTime(2026, 1, 31), ManufacturerId = 2, Status = ContractStatus.Active, TotalValue = 750000.00m, IsSuspended = false, SuspendedDate = null, ForeignContractID = null, SendToPerformance = true, CreatedBy = "System", CreatedDate = seedDate },
                new Contract { Id = 3, Title = "Healthcare Facilities Contract", StartDate = new DateTime(2025, 3, 1), EndDate = new DateTime(2025, 8, 31), ManufacturerId = 3, Status = ContractStatus.Active, TotalValue = 300000.00m, IsSuspended = false, SuspendedDate = null, ForeignContractID = null, SendToPerformance = false, CreatedBy = "System", CreatedDate = seedDate }
            );

            // Seed Contract-Distributor relationships
            modelBuilder.Entity<ContractDistributor>().HasData(
                new ContractDistributor { Id = 1, ContractId = 1, DistributorId = 1, CreatedDate = seedDate, CreatedBy = "System", IsActive = true },
                new ContractDistributor { Id = 2, ContractId = 1, DistributorId = 2, CreatedDate = seedDate, CreatedBy = "System", IsActive = true },
                new ContractDistributor { Id = 3, ContractId = 2, DistributorId = 2, CreatedDate = seedDate, CreatedBy = "System", IsActive = true },
                new ContractDistributor { Id = 4, ContractId = 3, DistributorId = 3, CreatedDate = seedDate, CreatedBy = "System", IsActive = true }
            );

            // Seed Contract-OpCo relationships
            modelBuilder.Entity<ContractOpCo>().HasData(
                new ContractOpCo { Id = 1, ContractId = 1, OpCoId = 1, CreatedDate = seedDate, CreatedBy = "System", IsActive = true },
                new ContractOpCo { Id = 2, ContractId = 1, OpCoId = 4, CreatedDate = seedDate, CreatedBy = "System", IsActive = true },
                new ContractOpCo { Id = 3, ContractId = 2, OpCoId = 2, CreatedDate = seedDate, CreatedBy = "System", IsActive = true },
                new ContractOpCo { Id = 4, ContractId = 3, OpCoId = 3, CreatedDate = seedDate, CreatedBy = "System", IsActive = true }
            );

            // Seed Contract-Industry relationships
            modelBuilder.Entity<ContractIndustry>().HasData(
                new ContractIndustry { Id = 1, ContractId = 1, IndustryId = 1, CreatedDate = seedDate, CreatedBy = "System", IsActive = true },
                new ContractIndustry { Id = 2, ContractId = 2, IndustryId = 2, CreatedDate = seedDate, CreatedBy = "System", IsActive = true },
                new ContractIndustry { Id = 3, ContractId = 3, IndustryId = 4, CreatedDate = seedDate, CreatedBy = "System", IsActive = true }
            );


            // Proposals module constraints and relationships
            modelBuilder.Entity<NPPContractManagement.API.Domain.Proposals.Entities.Proposal>()
                .HasIndex(p => new { p.ProposalTypeId, p.ProposalStatusId });

            modelBuilder.Entity<NPPContractManagement.API.Domain.Proposals.Entities.ProposalProduct>()
                .HasIndex(pp => new { pp.ProposalId, pp.ProductId })
                .IsUnique();
            modelBuilder.Entity<NPPContractManagement.API.Domain.Proposals.Entities.ProposalProduct>()
                .HasOne(pp => pp.Proposal)
                .WithMany(p => p.Products)
                .HasForeignKey(pp => pp.ProposalId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<NPPContractManagement.API.Domain.Proposals.Entities.ProposalProduct>()
                .HasOne(pp => pp.Product)
                .WithMany()
                .HasForeignKey(pp => pp.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NPPContractManagement.API.Domain.Proposals.Entities.ProposalDistributor>()
                .HasIndex(x => new { x.ProposalId, x.DistributorId })
                .IsUnique();
            modelBuilder.Entity<NPPContractManagement.API.Domain.Proposals.Entities.ProposalDistributor>()
                .HasOne(x => x.Proposal)
                .WithMany(p => p.Distributors)
                .HasForeignKey(x => x.ProposalId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<NPPContractManagement.API.Domain.Proposals.Entities.ProposalDistributor>()
                .HasOne(x => x.Distributor)
                .WithMany()
                .HasForeignKey(x => x.DistributorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NPPContractManagement.API.Domain.Proposals.Entities.ProposalIndustry>()
                .HasIndex(x => new { x.ProposalId, x.IndustryId })
                .IsUnique();
            modelBuilder.Entity<NPPContractManagement.API.Domain.Proposals.Entities.ProposalIndustry>()
                .HasOne(x => x.Proposal)
                .WithMany(p => p.Industries)
                .HasForeignKey(x => x.ProposalId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<NPPContractManagement.API.Domain.Proposals.Entities.ProposalIndustry>()
                .HasOne(x => x.Industry)
                .WithMany()
                .HasForeignKey(x => x.IndustryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NPPContractManagement.API.Domain.Proposals.Entities.ProposalOpco>()
                .HasIndex(x => new { x.ProposalId, x.OpCoId })
                .IsUnique();
            modelBuilder.Entity<NPPContractManagement.API.Domain.Proposals.Entities.ProposalOpco>()
                .HasOne(x => x.Proposal)
                .WithMany(p => p.Opcos)
                .HasForeignKey(x => x.ProposalId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<NPPContractManagement.API.Domain.Proposals.Entities.ProposalOpco>()
                .HasOne(x => x.OpCo)
                .WithMany()
                .HasForeignKey(x => x.OpCoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NPPContractManagement.API.Domain.Proposals.Entities.ProposalStatusHistory>()
                .HasIndex(x => x.ProposalId);
            modelBuilder.Entity<NPPContractManagement.API.Domain.Proposals.Entities.ProposalStatusHistory>()
                .HasOne(x => x.Proposal)
                .WithMany(p => p.StatusHistory)
                .HasForeignKey(x => x.ProposalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NPPContractManagement.API.Domain.Proposals.Entities.ProposalProductHistory>()
                .HasIndex(x => x.ProposalProductId);
            modelBuilder.Entity<NPPContractManagement.API.Domain.Proposals.Entities.ProposalProductHistory>()
                .HasOne(x => x.ProposalProduct)
                .WithMany(p => p.History)
                .HasForeignKey(x => x.ProposalProductId)
                .OnDelete(DeleteBehavior.Cascade);


            // Registration module indexes
            modelBuilder.Entity<RegistrationVerification>()
                .HasIndex(rv => rv.UserId);
            modelBuilder.Entity<RegistrationVerification>()
                .HasOne(rv => rv.User)
                .WithMany()
                .HasForeignKey(rv => rv.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InvitationEmail>()
                .HasIndex(ie => ie.UserId);
            modelBuilder.Entity<InvitationEmail>()
                .HasOne(ie => ie.User)
                .WithMany()
                .HasForeignKey(ie => ie.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Shared-type entity for UserManufacturers (for EF migration)
            modelBuilder.SharedTypeEntity<Dictionary<string, object>>("UserManufacturers", b =>
            {
                b.ToTable("UserManufacturers");
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<int>("UserId").IsRequired();
                b.Property<int>("ManufacturerId").IsRequired();
                b.Property<DateTime>("AssignedDate").HasColumnType("datetime(6)").HasDefaultValueSql("NOW(6)");
                b.Property<string>("AssignedBy").HasMaxLength(100);
                b.HasKey("Id");
                b.HasIndex("UserId");
                b.HasIndex("ManufacturerId");
                b.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade);
                b.HasOne<Manufacturer>().WithMany().HasForeignKey("ManufacturerId").OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}
