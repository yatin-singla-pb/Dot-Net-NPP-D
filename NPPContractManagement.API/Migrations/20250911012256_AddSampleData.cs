using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSampleData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Distributors",
                columns: new[] { "Id", "Address", "City", "ContactPerson", "Country", "CreatedBy", "CreatedDate", "Description", "Email", "IsActive", "ModifiedBy", "ModifiedDate", "Name", "PhoneNumber", "ReceiveContractProposal", "State", "Status", "ZipCode" },
                values: new object[,]
                {
                    { 1, "123 Distribution Way, Chicago, IL 60601", null, null, null, "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, null, null, "Regional Food Services", "(312) 555-0100", true, null, 1, null },
                    { 2, "456 Supply Chain Blvd, Atlanta, GA 30309", null, null, null, "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, null, null, "Metro Food Distribution", "(404) 555-0200", true, null, 1, null },
                    { 3, "789 Logistics Ave, Dallas, TX 75201", null, null, null, "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, null, null, "National Food Partners", "(214) 555-0300", false, null, 1, null }
                });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.InsertData(
                table: "Manufacturers",
                columns: new[] { "Id", "AKA", "Address", "City", "ContactPerson", "Country", "CreatedBy", "CreatedDate", "Description", "Email", "IsActive", "ModifiedBy", "ModifiedDate", "Name", "PhoneNumber", "PrimaryBroker", "State", "Status", "ZipCode" },
                values: new object[,]
                {
                    { 1, "Sysco", "1390 Enclave Pkwy, Houston, TX 77077", null, null, null, "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, null, null, "Sysco Corporation", "(281) 584-1390", "John Smith", null, 1, null },
                    { 2, "USF", "9399 W Higgins Rd, Rosemont, IL 60018", null, null, null, "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, null, null, "US Foods", "(847) 720-8000", "Jane Doe", null, 1, null },
                    { 3, "PFG", "12500 West Creek Pkwy, Richmond, VA 23238", null, null, null, "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, null, null, "Performance Food Group", "(804) 484-7700", "Mike Johnson", null, 1, null }
                });

            migrationBuilder.InsertData(
                table: "MemberAccounts",
                columns: new[] { "Id", "Address", "BusinessType", "City", "ContactPerson", "Country", "CreatedBy", "CreatedDate", "Email", "FacilityName", "IndustryId", "IsActive", "MemberNumber", "ModifiedBy", "ModifiedDate", "PhoneNumber", "State", "Status", "TaxId", "W9", "ZipCode" },
                values: new object[,]
                {
                    { 1, "5801 S Ellis Ave, Chicago, IL 60637", "Educational Institution", "Chicago", "Jennifer Adams", "USA", "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "dining@uchicago.edu", "University of Chicago Dining", 1, true, "MEM001", null, null, "(773) 702-1234", "IL", 1, "36-1234567", true, "60637" },
                    { 2, "123 School St, Atlanta, GA 30309", "Public School", "Atlanta", "Mary Johnson", "USA", "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "cafeteria@lincoln.edu", "Lincoln Elementary School", 2, true, "MEM002", null, null, "(404) 555-5000", "GA", 1, "58-9876543", true, "30309" },
                    { 3, "456 Fast Food Blvd, Dallas, TX 75201", "Restaurant", "Dallas", "Carlos Rodriguez", "USA", "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manager@quickbite.com", "Quick Bite Restaurant", 3, true, "MEM003", null, null, "(214) 555-6000", "TX", 1, "75-1122334", false, "75201" },
                    { 4, "789 Health Way, Chicago, IL 60611", "Healthcare Facility", "Chicago", "Dr. Patricia Lee", "USA", "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "food@generalhospital.org", "General Hospital Cafeteria", 4, true, "MEM004", null, null, "(312) 555-7000", "IL", 1, "36-5566778", true, "60611" }
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.InsertData(
                table: "OpCos",
                columns: new[] { "Id", "Address", "City", "ContactPerson", "Country", "CreatedBy", "CreatedDate", "DistributorId", "Email", "IsActive", "ModifiedBy", "ModifiedDate", "Name", "PhoneNumber", "RemoteReferenceCode", "State", "Status", "ZipCode" },
                values: new object[,]
                {
                    { 1, "100 N Michigan Ave, Chicago, IL 60601", "Chicago", "Sarah Wilson", "USA", "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "chicago@regionalfood.com", true, null, null, "Chicago Operations", "(312) 555-1001", "CHI001", "IL", 1, "60601" },
                    { 2, "200 Peachtree St, Atlanta, GA 30303", "Atlanta", "Robert Brown", "USA", "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "atlanta@metrofood.com", true, null, null, "Atlanta Hub", "(404) 555-2001", "ATL001", "GA", 1, "30303" },
                    { 3, "300 Main St, Dallas, TX 75202", "Dallas", "Lisa Garcia", "USA", "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "dallas@nationalfood.com", true, null, null, "Dallas Center", "(214) 555-3001", "DAL001", "TX", 1, "75202" },
                    { 4, "400 W Lake St, Chicago, IL 60606", "Chicago", "David Miller", "USA", "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "west@regionalfood.com", true, null, null, "Chicago West", "(312) 555-1002", "CHI002", "IL", 1, "60606" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CostPrice", "CreatedBy", "CreatedDate", "Description", "GTIN", "IsActive", "ListPrice", "ManufacturerId", "ManufacturerProductCode", "ModifiedBy", "ModifiedDate", "Name", "PackSize", "SKU", "Status", "SubCategory", "UPC", "UnitOfMeasure", "UnitPrice" },
                values: new object[,]
                {
                    { 1, "Meat", 38.50m, "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Premium Ground Beef 80/20", "1234567890123", true, 45.99m, 1, "SYSCO-BEEF-001", null, null, "", "10 lb", "SYS001", 1, "Ground Beef", "123456789012", null, null },
                    { 2, "Meat", 27.50m, "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Boneless Chicken Breast", "2345678901234", true, 32.99m, 2, "USF-CHICKEN-001", null, null, "", "5 lb", "USF001", 1, "Chicken", "234567890123", null, null },
                    { 3, "Produce", 15.25m, "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fresh Romaine Lettuce", "3456789012345", true, 18.99m, 3, "PFG-PRODUCE-001", null, null, "", "24 count", "PFG001", 1, "Lettuce", "345678901234", null, null },
                    { 4, "Dairy", 10.75m, "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Whole Milk Gallon", "4567890123456", true, 12.99m, 1, "SYSCO-DAIRY-001", null, null, "", "4 gallons", "SYS002", 1, "Milk", "456789012345", null, null },
                    { 5, "Frozen", 20.50m, "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Frozen French Fries", "5678901234567", true, 24.99m, 2, "USF-FROZEN-001", null, null, "", "6/5 lb", "USF002", 1, "Potato Products", "567890123456", null, null }
                });

            migrationBuilder.InsertData(
                table: "CustomerAccounts",
                columns: new[] { "Id", "Address", "City", "ContactPerson", "Country", "CreatedBy", "CreatedDate", "CreditLimit", "CurrentBalance", "CustomerAccountNumber", "CustomerName", "DistributorId", "Email", "IsActive", "MemberAccountId", "ModifiedBy", "ModifiedDate", "OpCoId", "PhoneNumber", "State", "Status", "ZipCode" },
                values: new object[,]
                {
                    { 1, "5801 S Ellis Ave, Chicago, IL 60637", "Chicago", "Jennifer Adams", "USA", "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 50000.00m, 12500.00m, "CUST001", "UChicago Main Dining", 1, "dining@uchicago.edu", true, 1, null, null, 1, "(773) 702-1234", "IL", 1, "60637" },
                    { 2, "123 School St, Atlanta, GA 30309", "Atlanta", "Mary Johnson", "USA", "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 25000.00m, 5000.00m, "CUST002", "Lincoln School Cafeteria", 2, "cafeteria@lincoln.edu", true, 2, null, null, 2, "(404) 555-5000", "GA", 1, "30309" },
                    { 3, "456 Fast Food Blvd, Dallas, TX 75201", "Dallas", "Carlos Rodriguez", "USA", "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 15000.00m, 3200.00m, "CUST003", "Quick Bite Main Location", 3, "manager@quickbite.com", true, 3, null, null, 3, "(214) 555-6000", "TX", 1, "75201" },
                    { 4, "789 Health Way, Chicago, IL 60611", "Chicago", "Dr. Patricia Lee", "USA", "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 75000.00m, 18750.00m, "CUST004", "General Hospital Food Service", 1, "food@generalhospital.org", true, 4, null, null, 4, "(312) 555-7000", "IL", 1, "60611" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CustomerAccounts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CustomerAccounts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CustomerAccounts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CustomerAccounts",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "OpCos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OpCos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OpCos",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "OpCos",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Distributors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Distributors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Distributors",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 1, 17, 56, 433, DateTimeKind.Utc).AddTicks(4511));

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 1, 17, 56, 433, DateTimeKind.Utc).AddTicks(5978));

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 1, 17, 56, 433, DateTimeKind.Utc).AddTicks(5983));

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 1, 17, 56, 433, DateTimeKind.Utc).AddTicks(5985));

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 1, 17, 56, 433, DateTimeKind.Utc).AddTicks(5986));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 1, 17, 56, 432, DateTimeKind.Utc).AddTicks(4428));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 1, 17, 56, 432, DateTimeKind.Utc).AddTicks(5934));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 1, 17, 56, 432, DateTimeKind.Utc).AddTicks(5938));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 1, 17, 56, 432, DateTimeKind.Utc).AddTicks(5939));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 1, 17, 56, 432, DateTimeKind.Utc).AddTicks(5940));
        }
    }
}
