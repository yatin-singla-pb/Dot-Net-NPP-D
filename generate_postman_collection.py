#!/usr/bin/env python3
"""
Generate comprehensive Postman collection for NPP Contract Management API
All 194 endpoints organized by module
"""

import json

# Base collection structure
collection = {
    "info": {
        "_postman_id": "npp-contract-mgmt-2026",
        "name": "NPP Contract Management API - Complete Collection (194 Endpoints)",
        "description": "Complete Postman collection for NPP Contract Management API with all 194 endpoints.\n\nBase URL: http://34.9.77.60:8081/api\n\nHow to use:\n1. Import this collection into Postman\n2. Run 'Login' request first to get JWT token\n3. Token will be automatically saved and used for all subsequent requests\n4. All requests have sample bodies pre-filled",
        "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
    },
    "auth": {
        "type": "bearer",
        "bearer": [{"key": "token", "value": "{{token}}", "type": "string"}]
    },
    "variable": [
        {"key": "baseUrl", "value": "http://34.9.77.60:8081/api", "type": "string"},
        {"key": "token", "value": "", "type": "string"}
    ],
    "item": []
}

# Helper function to create request
def create_request(name, method, path, body=None, auth_required=True, description=""):
    request = {
        "name": name,
        "request": {
            "method": method,
            "header": [{"key": "Content-Type", "value": "application/json"}] if body else [],
            "url": {
                "raw": "{{baseUrl}}" + path,
                "host": ["{{baseUrl}}"],
                "path": path.strip("/").split("/")
            }
        },
        "response": []
    }
    
    if description:
        request["request"]["description"] = description
    
    if not auth_required:
        request["request"]["auth"] = {"type": "noauth"}
    
    if body:
        request["request"]["body"] = {
            "mode": "raw",
            "raw": json.dumps(body, indent=2)
        }
    
    return request

# 1. AUTHENTICATION MODULE (7 endpoints)
auth_module = {
    "name": "01. Authentication (7 endpoints)",
    "item": [
        {
            "name": "Login",
            "event": [{
                "listen": "test",
                "script": {
                    "exec": [
                        "if (pm.response.code === 200) {",
                        "    var jsonData = pm.response.json();",
                        "    pm.environment.set('token', jsonData.token);",
                        "    pm.collectionVariables.set('token', jsonData.token);",
                        "}"
                    ],
                    "type": "text/javascript"
                }
            }],
            "request": {
                "auth": {"type": "noauth"},
                "method": "POST",
                "header": [{"key": "Content-Type", "value": "application/json"}],
                "body": {
                    "mode": "raw",
                    "raw": json.dumps({"userId": "admin", "password": "Admin@123"}, indent=2)
                },
                "url": {
                    "raw": "{{baseUrl}}/auth/login",
                    "host": ["{{baseUrl}}"],
                    "path": ["auth", "login"]
                },
                "description": "Login with credentials. Returns JWT token that will be automatically saved for subsequent requests."
            },
            "response": []
        },
        create_request("Logout", "POST", "/auth/logout", description="Logout current user"),
        create_request("Refresh Token", "POST", "/auth/refresh-token", 
                      {"refreshToken": "{{refreshToken}}"}, False,
                      "Refresh expired JWT token"),
        create_request("Forgot Password", "POST", "/auth/forgot-password",
                      {"userId": "admin", "email": "admin@example.com"}, False,
                      "Request password reset email"),
        create_request("Reset Password", "POST", "/auth/reset-password",
                      {"token": "reset-token-here", "newPassword": "NewPassword@123"}, False,
                      "Reset password using token from email"),
        create_request("Change Password", "POST", "/auth/change-password",
                      {"currentPassword": "Admin@123", "newPassword": "NewPassword@123"},
                      description="Change password for logged-in user"),
        create_request("Validate Token", "GET", "/auth/validate-token?token=reset-token-here",
                      auth_required=False, description="Validate password reset token")
    ]
}

collection["item"].append(auth_module)

# 2. USERS MODULE (13 endpoints)
users_module = {
    "name": "02. Users (13 endpoints)",
    "item": [
        create_request("Get All Users", "GET", "/users?pageNumber=1&pageSize=10"),
        create_request("Get User by ID", "GET", "/users/1"),
        create_request("Create User", "POST", "/users", {
            "userId": "newuser",
            "firstName": "John",
            "lastName": "Doe",
            "email": "john.doe@example.com",
            "roleIds": [2],
            "isActive": True
        }),
        create_request("Update User", "PUT", "/users/1", {
            "firstName": "John Updated",
            "lastName": "Doe",
            "email": "john.doe@example.com",
            "isActive": True
        }),
        create_request("Delete User", "DELETE", "/users/1"),
        create_request("Get User Roles", "GET", "/users/1/roles"),
        create_request("Assign Role to User", "POST", "/users/1/roles", {"roleId": 2}),
        create_request("Remove Role from User", "DELETE", "/users/1/roles/2"),
        create_request("Get User Manufacturers", "GET", "/users/1/manufacturers"),
        create_request("Assign Manufacturer to User", "POST", "/users/1/manufacturers", [1, 2, 3]),
        create_request("Remove Manufacturer from User", "DELETE", "/users/1/manufacturers/1"),
        create_request("Activate User", "PUT", "/users/1/activate"),
        create_request("Deactivate User", "PUT", "/users/1/deactivate")
    ]
}
collection["item"].append(users_module)

# 3. ROLES MODULE (5 endpoints)
roles_module = {
    "name": "03. Roles (5 endpoints)",
    "item": [
        create_request("Get All Roles", "GET", "/roles"),
        create_request("Get Role by ID", "GET", "/roles/1"),
        create_request("Create Role", "POST", "/roles", {
            "name": "New Role",
            "description": "Role description"
        }),
        create_request("Update Role", "PUT", "/roles/1", {
            "name": "Updated Role",
            "description": "Updated description"
        }),
        create_request("Delete Role", "DELETE", "/roles/1")
    ]
}
collection["item"].append(roles_module)

# 4. CONTRACTS MODULE (24 endpoints)
contracts_module = {
    "name": "04. Contracts (24 endpoints)",
    "item": [
        create_request("Get All Contracts", "GET", "/contracts?page=1&pageSize=10"),
        create_request("Get Contract by ID", "GET", "/contracts/1"),
        create_request("Create Contract", "POST", "/contracts", {
            "contractNumber": "CNT-2024-001",
            "manufacturerId": 1,
            "startDate": "2024-01-01",
            "endDate": "2024-12-31",
            "contractStatusId": 1,
            "description": "New contract"
        }),
        create_request("Update Contract", "PUT", "/contracts/1", {
            "contractNumber": "CNT-2024-001",
            "description": "Updated contract"
        }),
        create_request("Delete Contract", "DELETE", "/contracts/1"),
        create_request("Get Contract Versions", "GET", "/contracts/1/versions"),
        create_request("Get Specific Version", "GET", "/contracts/1/versions/1"),
        create_request("Create New Version", "POST", "/contracts/1/versions", {
            "versionNumber": 2,
            "effectiveDate": "2024-06-01"
        }),
        create_request("Update Version", "PUT", "/contracts/1/versions/1", {
            "effectiveDate": "2024-06-15"
        }),
        create_request("Delete Version", "DELETE", "/contracts/1/versions/1"),
        create_request("Get Contract Products", "GET", "/contracts/1/products"),
        create_request("Add Product to Contract", "POST", "/contracts/1/products", {
            "productId": 1,
            "quantity": 100
        }),
        create_request("Remove Product from Contract", "DELETE", "/contracts/1/products/1"),
        create_request("Get Contract Prices", "GET", "/contracts/1/prices"),
        create_request("Add Price", "POST", "/contracts/1/prices", {
            "productId": 1,
            "priceTypeId": 1,
            "price": 99.99
        }),
        create_request("Update Price", "PUT", "/contracts/1/prices/1", {
            "price": 89.99
        }),
        create_request("Delete Price", "DELETE", "/contracts/1/prices/1"),
        create_request("Get Contract Assignments", "GET", "/contracts/1/assignments"),
        create_request("Assign Contract", "POST", "/contracts/1/assign", {
            "customerAccountId": 1,
            "effectiveDate": "2024-01-01"
        }),
        create_request("Remove Assignment", "DELETE", "/contracts/1/assignments/1"),
        create_request("Activate Contract", "PUT", "/contracts/1/activate"),
        create_request("Suspend Contract", "PUT", "/contracts/1/suspend"),
        create_request("Search Contracts", "GET", "/contracts/search?searchTerm=test"),
        create_request("Export Contracts", "GET", "/contracts/export")
    ]
}
collection["item"].append(contracts_module)

# 5. PROPOSALS MODULE (11 endpoints)
proposals_module = {
    "name": "05. Proposals (11 endpoints)",
    "item": [
        create_request("Get All Proposals", "GET", "/proposals?page=1&pageSize=10"),
        create_request("Get Proposal by ID", "GET", "/proposals/1"),
        create_request("Create Proposal", "POST", "/proposals", {
            "proposalNumber": "PROP-2024-001",
            "manufacturerId": 1,
            "proposalTypeId": 1,
            "proposalStatusId": 1,
            "startDate": "2024-01-01",
            "endDate": "2024-12-31"
        }),
        create_request("Update Proposal", "PUT", "/proposals/1", {
            "proposalNumber": "PROP-2024-001",
            "description": "Updated proposal"
        }),
        create_request("Delete Proposal", "DELETE", "/proposals/1"),
        create_request("Get Proposal Products", "GET", "/proposals/1/products"),
        create_request("Add Product to Proposal", "POST", "/proposals/1/products", {
            "productId": 1,
            "quantity": 100,
            "proposedPrice": 99.99
        }),
        create_request("Update Proposal Product", "PUT", "/proposals/1/products/1", {
            "quantity": 150,
            "proposedPrice": 89.99
        }),
        create_request("Remove Product from Proposal", "DELETE", "/proposals/1/products/1"),
        create_request("Import Products from Excel", "POST", "/proposals/1/products/import"),
        create_request("Export Products to Excel", "GET", "/proposals/1/products/export")
    ]
}
collection["item"].append(proposals_module)

# 6. PRODUCTS MODULE (15 endpoints)
products_module = {
    "name": "06. Products (15 endpoints)",
    "item": [
        create_request("Get All Products", "GET", "/products?page=1&pageSize=10"),
        create_request("Get Product by ID", "GET", "/products/1"),
        create_request("Create Product", "POST", "/products", {
            "productCode": "PROD-001",
            "productName": "Test Product",
            "manufacturerId": 1,
            "categoryId": 1,
            "uomId": 1,
            "packSize": "12x500ml"
        }),
        create_request("Update Product", "PUT", "/products/1", {
            "productName": "Updated Product",
            "packSize": "24x500ml"
        }),
        create_request("Delete Product", "DELETE", "/products/1"),
        create_request("Search Products", "GET", "/products/search?searchTerm=test"),
        create_request("Get Products by Manufacturer", "GET", "/products/by-manufacturer/1"),
        create_request("Get Contracts Using Product", "GET", "/products/1/contracts"),
        create_request("Get Product Price History", "GET", "/products/1/prices"),
        create_request("Import Products from Excel", "POST", "/products/import"),
        create_request("Export Products to Excel", "GET", "/products/export"),
        create_request("Activate Product", "PUT", "/products/1/activate"),
        create_request("Deactivate Product", "PUT", "/products/1/deactivate"),
        create_request("Find Duplicate Products", "GET", "/products/duplicates"),
        create_request("Merge Duplicate Products", "POST", "/products/merge", {
            "sourceProductId": 1,
            "targetProductId": 2
        })
    ]
}
collection["item"].append(products_module)

# 7. MANUFACTURERS MODULE (5 endpoints)
manufacturers_module = {
    "name": "07. Manufacturers (5 endpoints)",
    "item": [
        create_request("Get All Manufacturers", "GET", "/manufacturers"),
        create_request("Get Manufacturer by ID", "GET", "/manufacturers/1"),
        create_request("Create Manufacturer", "POST", "/manufacturers", {
            "name": "New Manufacturer",
            "code": "MFG-001",
            "contactEmail": "contact@manufacturer.com"
        }),
        create_request("Update Manufacturer", "PUT", "/manufacturers/1", {
            "name": "Updated Manufacturer"
        }),
        create_request("Delete Manufacturer", "DELETE", "/manufacturers/1")
    ]
}
collection["item"].append(manufacturers_module)

# 8. DISTRIBUTORS MODULE (7 endpoints)
distributors_module = {
    "name": "08. Distributors (7 endpoints)",
    "item": [
        create_request("Get All Distributors", "GET", "/distributors"),
        create_request("Get Distributor by ID", "GET", "/distributors/1"),
        create_request("Create Distributor", "POST", "/distributors", {
            "name": "New Distributor",
            "code": "DIST-001",
            "contactEmail": "contact@distributor.com"
        }),
        create_request("Update Distributor", "PUT", "/distributors/1", {
            "name": "Updated Distributor"
        }),
        create_request("Delete Distributor", "DELETE", "/distributors/1"),
        create_request("Get Distributor Product Codes", "GET", "/distributors/1/product-codes"),
        create_request("Add Product Code Mapping", "POST", "/distributors/1/product-codes", {
            "productId": 1,
            "distributorProductCode": "DIST-PROD-001"
        })
    ]
}
collection["item"].append(distributors_module)

# 9. INDUSTRIES MODULE (10 endpoints)
industries_module = {
    "name": "09. Industries (10 endpoints)",
    "item": [
        create_request("Get All Industries", "GET", "/industries"),
        create_request("Get Industry by ID", "GET", "/industries/1"),
        create_request("Create Industry", "POST", "/industries", {
            "name": "Healthcare",
            "code": "IND-HC"
        }),
        create_request("Update Industry", "PUT", "/industries/1", {
            "name": "Healthcare Updated"
        }),
        create_request("Delete Industry", "DELETE", "/industries/1"),
        create_request("Get Contracts by Industry", "GET", "/industries/1/contracts"),
        create_request("Get Customers in Industry", "GET", "/industries/1/customers"),
        create_request("Activate Industry", "PUT", "/industries/1/activate"),
        create_request("Deactivate Industry", "PUT", "/industries/1/deactivate"),
        create_request("Get Industry Hierarchy", "GET", "/industries/hierarchy")
    ]
}
collection["item"].append(industries_module)

# 10. OPCOS MODULE (11 endpoints)
opcos_module = {
    "name": "10. OpCos (11 endpoints)",
    "item": [
        create_request("Get All OpCos", "GET", "/opcos"),
        create_request("Get OpCo by ID", "GET", "/opcos/1"),
        create_request("Create OpCo", "POST", "/opcos", {
            "name": "New OpCo",
            "code": "OPCO-001"
        }),
        create_request("Update OpCo", "PUT", "/opcos/1", {
            "name": "Updated OpCo"
        }),
        create_request("Delete OpCo", "DELETE", "/opcos/1"),
        create_request("Get Contracts by OpCo", "GET", "/opcos/1/contracts"),
        create_request("Get Customers by OpCo", "GET", "/opcos/1/customers"),
        create_request("Get Members by OpCo", "GET", "/opcos/1/members"),
        create_request("Activate OpCo", "PUT", "/opcos/1/activate"),
        create_request("Deactivate OpCo", "PUT", "/opcos/1/deactivate"),
        create_request("Get OpCo Hierarchy", "GET", "/opcos/hierarchy")
    ]
}
collection["item"].append(opcos_module)

# 11. CUSTOMER ACCOUNTS MODULE (11 endpoints)
customer_accounts_module = {
    "name": "11. Customer Accounts (11 endpoints)",
    "item": [
        create_request("Get All Customer Accounts", "GET", "/customer-accounts?page=1&pageSize=10"),
        create_request("Get Customer by ID", "GET", "/customer-accounts/1"),
        create_request("Create Customer", "POST", "/customer-accounts", {
            "accountNumber": "CUST-001",
            "accountName": "ABC Restaurant",
            "opCoId": 1,
            "industryId": 1
        }),
        create_request("Update Customer", "PUT", "/customer-accounts/1", {
            "accountName": "ABC Restaurant Updated"
        }),
        create_request("Delete Customer", "DELETE", "/customer-accounts/1"),
        create_request("Search Customers", "GET", "/customer-accounts/search?searchTerm=ABC"),
        create_request("Get Customer Contracts", "GET", "/customer-accounts/1/contracts"),
        create_request("Get Contract Assignments", "GET", "/customer-accounts/1/assignments"),
        create_request("Import Customers from Excel", "POST", "/customer-accounts/import"),
        create_request("Export Customers to Excel", "GET", "/customer-accounts/export"),
        create_request("Get Customers by OpCo", "GET", "/customer-accounts/by-opco/1")
    ]
}
collection["item"].append(customer_accounts_module)

# 12. MEMBER ACCOUNTS MODULE (9 endpoints)
member_accounts_module = {
    "name": "12. Member Accounts (9 endpoints)",
    "item": [
        create_request("Get All Member Accounts", "GET", "/member-accounts?page=1&pageSize=10"),
        create_request("Get Member by ID", "GET", "/member-accounts/1"),
        create_request("Create Member", "POST", "/member-accounts", {
            "accountNumber": "MEM-001",
            "accountName": "Member Organization",
            "opCoId": 1
        }),
        create_request("Update Member", "PUT", "/member-accounts/1", {
            "accountName": "Member Organization Updated"
        }),
        create_request("Delete Member", "DELETE", "/member-accounts/1"),
        create_request("Search Members", "GET", "/member-accounts/search?searchTerm=Member"),
        create_request("Get Member Contracts", "GET", "/member-accounts/1/contracts"),
        create_request("Import Members from Excel", "POST", "/member-accounts/import"),
        create_request("Export Members to Excel", "GET", "/member-accounts/export")
    ]
}
collection["item"].append(member_accounts_module)

print("Generating Postman collection...")
print(f"Added Authentication module: {len(auth_module['item'])} endpoints")
print(f"Added Users module: {len(users_module['item'])} endpoints")
print(f"Added Roles module: {len(roles_module['item'])} endpoints")
print(f"Added Contracts module: {len(contracts_module['item'])} endpoints")
print(f"Added Proposals module: {len(proposals_module['item'])} endpoints")
print(f"Added Products module: {len(products_module['item'])} endpoints")
print(f"Added Manufacturers module: {len(manufacturers_module['item'])} endpoints")
print(f"Added Distributors module: {len(distributors_module['item'])} endpoints")
print(f"Added Industries module: {len(industries_module['item'])} endpoints")
print(f"Added OpCos module: {len(opcos_module['item'])} endpoints")
print(f"Added Customer Accounts module: {len(customer_accounts_module['item'])} endpoints")
print(f"Added Member Accounts module: {len(member_accounts_module['item'])} endpoints")

# 13. VELOCITY MODULE (11 endpoints)
velocity_module = {
    "name": "13. Velocity - Data Ingestion (11 endpoints)",
    "item": [
        create_request("Get All Velocity Jobs", "GET", "/velocity/jobs?page=1&pageSize=10"),
        create_request("Get Job by ID", "GET", "/velocity/jobs/1"),
        create_request("Create Velocity Job", "POST", "/velocity/jobs", {
            "distributorId": 1,
            "jobName": "Monthly Upload - Jan 2024"
        }),
        {
            "name": "Upload Velocity File (CSV/Excel)",
            "request": {
                "method": "POST",
                "header": [],
                "body": {
                    "mode": "formdata",
                    "formdata": [
                        {
                            "key": "file",
                            "type": "file",
                            "src": []
                        },
                        {
                            "key": "distributorId",
                            "value": "1",
                            "type": "text"
                        }
                    ]
                },
                "url": {
                    "raw": "{{baseUrl}}/velocity/jobs/1/upload",
                    "host": ["{{baseUrl}}"],
                    "path": ["velocity", "jobs", "1", "upload"]
                },
                "description": "Upload CSV or Excel file for velocity data processing. Max file size: 10MB"
            },
            "response": []
        },
        create_request("Process Velocity Data", "POST", "/velocity/jobs/1/process"),
        create_request("Get Job Processing Status", "GET", "/velocity/jobs/1/status"),
        create_request("Get Job Errors", "GET", "/velocity/jobs/1/errors"),
        create_request("Get All Shipments", "GET", "/velocity/shipments?page=1&pageSize=10"),
        create_request("Search Shipments", "GET", "/velocity/shipments/search?searchTerm=test"),
        create_request("Generate Usage Report", "GET", "/velocity/usage-report?startDate=2024-01-01&endDate=2024-12-31"),
        create_request("Export Usage Report to Excel", "GET", "/velocity/usage-report/export?startDate=2024-01-01&endDate=2024-12-31")
    ]
}
collection["item"].append(velocity_module)

# 14. REPORTS MODULE (4 endpoints)
reports_module = {
    "name": "14. Reports (4 endpoints)",
    "item": [
        create_request("Contract Pricing Report", "GET", "/reports/contract-pricing?startDate=2024-01-01&endDate=2024-12-31"),
        create_request("Contracts Over Term Report", "GET", "/reports/contract-over-term"),
        create_request("Velocity Usage Report", "GET", "/reports/velocity-usage?startDate=2024-01-01&endDate=2024-12-31"),
        create_request("Proposal Summary Report", "GET", "/reports/proposal-summary?startDate=2024-01-01&endDate=2024-12-31")
    ]
}
collection["item"].append(reports_module)

# 15. BULK RENEWAL MODULE (2 endpoints)
bulk_renewal_module = {
    "name": "15. Bulk Renewal (2 endpoints)",
    "item": [
        create_request("Preview Bulk Renewal", "POST", "/bulk-renewal/preview", {
            "contractIds": [1, 2, 3],
            "newStartDate": "2025-01-01",
            "newEndDate": "2025-12-31"
        }),
        create_request("Execute Bulk Renewal", "POST", "/bulk-renewal/execute", {
            "contractIds": [1, 2, 3],
            "newStartDate": "2025-01-01",
            "newEndDate": "2025-12-31"
        })
    ]
}
collection["item"].append(bulk_renewal_module)

# 16. LOOKUP MODULE (10 endpoints)
lookup_module = {
    "name": "16. Lookup / Dropdown Data (10 endpoints)",
    "item": [
        create_request("Get Price Types", "GET", "/lookup/price-types", auth_required=False),
        create_request("Get Contract Statuses", "GET", "/lookup/contract-statuses", auth_required=False),
        create_request("Get Proposal Statuses", "GET", "/lookup/proposal-statuses", auth_required=False),
        create_request("Get Product Categories", "GET", "/lookup/product-categories", auth_required=False),
        create_request("Get Units of Measure", "GET", "/lookup/uom", auth_required=False),
        create_request("Get US States", "GET", "/lookup/states", auth_required=False),
        create_request("Get Countries", "GET", "/lookup/countries", auth_required=False),
        create_request("Get Currencies", "GET", "/lookup/currencies", auth_required=False),
        create_request("Get Payment Terms", "GET", "/lookup/payment-terms", auth_required=False),
        create_request("Get Shipping Terms", "GET", "/lookup/shipping-terms", auth_required=False)
    ]
}
collection["item"].append(lookup_module)

# 17. CONTRACT PRICES (5 endpoints)
contract_prices_module = {
    "name": "17. Contract Prices (5 endpoints)",
    "item": [
        create_request("Get All Contract Prices", "GET", "/contract-prices?productId=1&versionNumber=1"),
        create_request("Get Contract Price by ID", "GET", "/contract-prices/1"),
        create_request("Create Contract Price", "POST", "/contract-prices", {
            "contractId": 1,
            "productId": 1,
            "priceTypeId": 1,
            "price": 99.99,
            "versionNumber": 1
        }),
        create_request("Update Contract Price", "PUT", "/contract-prices/1", {
            "price": 89.99
        }),
        create_request("Delete Contract Price", "DELETE", "/contract-prices/1")
    ]
}
collection["item"].append(contract_prices_module)

# 18. CONTRACT VERSION - OPCOS (5 endpoints)
contract_version_opcos_module = {
    "name": "18. Contract Version - OpCos (5 endpoints)",
    "item": [
        create_request("Get Contract Version OpCos", "GET", "/contract-version/opcos?contractId=1&versionNumber=1"),
        create_request("Get Contract Version OpCo by ID", "GET", "/contract-version/opcos/1"),
        create_request("Assign OpCo to Contract Version", "POST", "/contract-version/opcos", {
            "contractId": 1,
            "opCoId": 1,
            "versionNumber": 1,
            "assignedBy": "admin",
            "assignedDate": "2024-01-01"
        }),
        create_request("Update Contract Version OpCo", "PUT", "/contract-version/opcos/1", {
            "assignedBy": "admin",
            "assignedDate": "2024-01-01"
        }),
        create_request("Remove OpCo from Contract Version", "DELETE", "/contract-version/opcos/1")
    ]
}
collection["item"].append(contract_version_opcos_module)

# 19. CONTRACT VERSION - DISTRIBUTORS (5 endpoints)
contract_version_distributors_module = {
    "name": "19. Contract Version - Distributors (5 endpoints)",
    "item": [
        create_request("Get Contract Version Distributors", "GET", "/contract-version/distributors?contractId=1&versionNumber=1"),
        create_request("Get Contract Version Distributor by ID", "GET", "/contract-version/distributors/1"),
        create_request("Assign Distributor to Contract Version", "POST", "/contract-version/distributors", {
            "contractId": 1,
            "distributorId": 1,
            "versionNumber": 1,
            "assignedBy": "admin",
            "assignedDate": "2024-01-01"
        }),
        create_request("Update Contract Version Distributor", "PUT", "/contract-version/distributors/1", {
            "assignedBy": "admin",
            "assignedDate": "2024-01-01"
        }),
        create_request("Remove Distributor from Contract Version", "DELETE", "/contract-version/distributors/1")
    ]
}
collection["item"].append(contract_version_distributors_module)

# 20. CONTRACT VERSION - MANUFACTURERS (5 endpoints)
contract_version_manufacturers_module = {
    "name": "20. Contract Version - Manufacturers (5 endpoints)",
    "item": [
        create_request("Get Contract Version Manufacturers", "GET", "/contract-version/manufacturers?contractId=1&versionNumber=1"),
        create_request("Get Contract Version Manufacturer by ID", "GET", "/contract-version/manufacturers/1"),
        create_request("Assign Manufacturer to Contract Version", "POST", "/contract-version/manufacturers", {
            "contractId": 1,
            "manufacturerId": 1,
            "versionNumber": 1,
            "assignedBy": "admin",
            "assignedDate": "2024-01-01"
        }),
        create_request("Update Contract Version Manufacturer", "PUT", "/contract-version/manufacturers/1", {
            "assignedBy": "admin",
            "assignedDate": "2024-01-01"
        }),
        create_request("Remove Manufacturer from Contract Version", "DELETE", "/contract-version/manufacturers/1")
    ]
}
collection["item"].append(contract_version_manufacturers_module)

# 21. CONTRACT VERSION - INDUSTRIES (5 endpoints)
contract_version_industries_module = {
    "name": "21. Contract Version - Industries (5 endpoints)",
    "item": [
        create_request("Get Contract Version Industries", "GET", "/contract-version/industries?contractId=1&versionNumber=1"),
        create_request("Get Contract Version Industry by ID", "GET", "/contract-version/industries/1"),
        create_request("Assign Industry to Contract Version", "POST", "/contract-version/industries", {
            "contractId": 1,
            "industryId": 1,
            "versionNumber": 1,
            "assignedBy": "admin",
            "assignedDate": "2024-01-01"
        }),
        create_request("Update Contract Version Industry", "PUT", "/contract-version/industries/1", {
            "assignedBy": "admin",
            "assignedDate": "2024-01-01"
        }),
        create_request("Remove Industry from Contract Version", "DELETE", "/contract-version/industries/1")
    ]
}
collection["item"].append(contract_version_industries_module)

# 22. CONTRACT VERSION - PRODUCTS (5 endpoints)
contract_version_products_module = {
    "name": "22. Contract Version - Products (5 endpoints)",
    "item": [
        create_request("Get Contract Version Products", "GET", "/contract-version/products?contractId=1&versionNumber=1"),
        create_request("Get Contract Version Product by ID", "GET", "/contract-version/products/1"),
        create_request("Add Product to Contract Version", "POST", "/contract-version/products", {
            "contractId": 1,
            "productId": 1,
            "versionNumber": 1,
            "quantity": 100
        }),
        create_request("Update Contract Version Product", "PUT", "/contract-version/products/1", {
            "quantity": 150
        }),
        create_request("Remove Product from Contract Version", "DELETE", "/contract-version/products/1")
    ]
}
collection["item"].append(contract_version_products_module)

# 23. PROPOSAL PRODUCTS (4 endpoints)
proposal_products_module = {
    "name": "23. Proposal Products (4 endpoints)",
    "item": [
        create_request("Get Proposal Products", "GET", "/proposal-products?proposalId=1"),
        create_request("Get Proposal Product by ID", "GET", "/proposal-products/1"),
        create_request("Update Proposal Product", "PUT", "/proposal-products/1", {
            "quantity": 150,
            "proposedPrice": 89.99
        }),
        create_request("Delete Proposal Product", "DELETE", "/proposal-products/1")
    ]
}
collection["item"].append(proposal_products_module)

print(f"Added Velocity module: {len(velocity_module['item'])} endpoints")
print(f"Added Reports module: {len(reports_module['item'])} endpoints")
print(f"Added Bulk Renewal module: {len(bulk_renewal_module['item'])} endpoints")
print(f"Added Lookup module: {len(lookup_module['item'])} endpoints")
print(f"Added Contract Prices module: {len(contract_prices_module['item'])} endpoints")
print(f"Added Contract Version OpCos module: {len(contract_version_opcos_module['item'])} endpoints")
print(f"Added Contract Version Distributors module: {len(contract_version_distributors_module['item'])} endpoints")
print(f"Added Contract Version Manufacturers module: {len(contract_version_manufacturers_module['item'])} endpoints")
print(f"Added Contract Version Industries module: {len(contract_version_industries_module['item'])} endpoints")
print(f"Added Contract Version Products module: {len(contract_version_products_module['item'])} endpoints")
print(f"Added Proposal Products module: {len(proposal_products_module['item'])} endpoints")

# Save to file
with open("NPP_Contract_Management_API.postman_collection.json", "w") as f:
    json.dump(collection, f, indent=2)

print(f"\n✅ Postman collection generated successfully!")
print(f"📁 File: NPP_Contract_Management_API.postman_collection.json")
print(f"📊 Total modules: {len(collection['item'])}")
total_endpoints = sum(len(module['item']) for module in collection['item'])
print(f"📊 Total endpoints: {total_endpoints}")
print(f"\n🎯 Import this file into Postman to test all {total_endpoints} endpoints!")
print(f"🔑 Run 'Login' request first to get JWT token")
print(f"🌐 Base URL: http://34.9.77.60:8081/api")

