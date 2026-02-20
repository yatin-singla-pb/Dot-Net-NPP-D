# Video Script 1: Introduction & System Overview

## SLIDE 1: Welcome (30 seconds)

**[Show NPP Logo or Dashboard]**

Hello and welcome to the NPP Contract Management System overview.

In this video series, I'll walk you through the complete API architecture, all endpoints, and the core domain objects that power this enterprise contract management solution.

This system manages the entire lifecycle of contracts between manufacturers, distributors, and member accounts - from proposal creation to contract execution and velocity tracking.

---

## SLIDE 2: System Architecture (1 minute)

**[Show Architecture Diagram]**

The NPP Contract Management System is built on a modern tech stack:

**Backend:**
- ASP.NET Core 9.0 Web API
- Entity Framework Core for data access
- SQL Server database
- JWT-based authentication
- RESTful API design

**Frontend:**
- Angular 18 single-page application
- TypeScript for type safety
- Responsive Material Design UI

**Key Features:**
- Role-based access control
- Proposal workflow management
- Contract versioning
- Excel import/export
- Velocity data integration
- Comprehensive reporting

---

## SLIDE 3: Core Domain Areas (1 minute)

**[Show Domain Model Diagram]**

The system is organized into 6 major domain areas:

**1. Authentication & User Management**
- User accounts with role-based permissions
- JWT token authentication
- Password management

**2. Master Data Management**
- Manufacturers
- Products
- Distributors
- Industries
- Operating Companies (OpCos)

**3. Proposal Management**
- Create and submit pricing proposals
- Multi-step approval workflow
- Excel import for bulk pricing
- Product-level status tracking

**4. Contract Management**
- Convert approved proposals to contracts
- Contract versioning
- Pricing management
- Bulk renewal operations

**5. Velocity Integration**
- Import shipment data from CSV
- Match shipments to contracts
- Track actual vs contracted pricing
- Generate velocity reports

**6. Reporting & Analytics**
- Contract over-term reports
- Contract pricing reports
- Velocity analysis
- Export to Excel

---

## SLIDE 4: API Structure (45 seconds)

**[Show API Endpoint List]**

The API is organized into 12 main controllers:

1. **AuthController** - Authentication endpoints
2. **UsersController** - User management
3. **RolesController** - Role management
4. **ManufacturersController** - Manufacturer CRUD
5. **ProductsController** - Product catalog
6. **DistributorsController** - Distributor management
7. **ProposalsController** - Proposal workflow
8. **ContractsController** - Contract management
9. **VelocityController** - Velocity data ingestion
10. **ReportsController** - Reporting endpoints
11. **LookupController** - Reference data
12. **TestController** - Health checks

All endpoints follow RESTful conventions and return JSON responses.

---

## SLIDE 5: What We'll Cover (30 seconds)

**[Show Table of Contents]**

In the following videos, we'll dive deep into:

- **Video 2:** Authentication & User Management APIs
- **Video 3:** Master Data APIs (Manufacturers, Products, Distributors)
- **Video 4:** Proposal Management APIs
- **Video 5:** Contract Management APIs
- **Video 6:** Velocity Integration APIs
- **Video 7:** Reporting & Lookup APIs
- **Video 8:** Core Domain Objects & Data Models

Each video will show the endpoints, request/response formats, and explain the business logic.

Let's get started!

---

**[TOTAL TIME: ~4 minutes]**

