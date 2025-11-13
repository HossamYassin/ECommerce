# E-Commerce Backend API (Clean Architecture)

This solution delivers a layered .NET 8 Web API for managing e-commerce operations such as authentication, product and category administration, customer order workflows, and reporting. The architecture mirrors the `TestAndScore` structure and follows Clean Architecture principles.

## Solution Layout

```text
ECommerce/
 ├── ECommerce.Domain/          # Domain entities and enums
 ├── ECommerce.Application/     # CQRS handlers, DTOs, validators, interfaces
 ├── ECommerce.Infrastructure/  # EF Core DbContext, repositories, JWT + caching services
 ├── ECommerce.API/             # ASP.NET Core Web API, controllers, middleware
 └── ECommerce.sln              # Solution referencing all projects
```

### Layer Responsibilities

- **Domain**: Entity definitions for `User`, `Product`, `Category`, `Order`, `OrderItem`, and `RefreshToken`, plus shared enums (`UserRole`, `OrderStatus`) and auditing fields (Created/Updated/Deleted timestamps, soft-delete flags).
- **Application**: 
  - CQRS handlers implemented with MediatR.
  - DTOs, pagination helpers, and AutoMapper profile.
  - Validation rules via FluentValidation.
  - Abstractions for repositories, caching, and JWT token generation.
- **Infrastructure**:
  - EF Core `ECommerceDbContext` with entity configurations (constraints, soft-delete filters, precision).
  - Generic repository and unit of work implementation.
  - JWT token service, password hashing (ASP.NET Identity), and in-memory caching facade.
  - Serilog integration.
- **API**:
  - REST controllers for authentication, customer-facing endpoints, and admin management areas.
  - JWT authentication/authorization, global exception handling middleware, Swagger with JWT support, and health checks.

## Prerequisites

- [.NET SDK 8.0+](https://dotnet.microsoft.com/en-us/download)
- SQL Server or SQL Server Express / LocalDB

## Getting Started

1. **Restore dependencies**

   ```powershell
   cd ECommerce
   dotnet restore
   ```

2. **Configure connection string and JWT**

   Update the `ConnectionStrings:DefaultConnection` and `JwtSettings` values in:

   - `ECommerce.API/appsettings.Development.json`
   - `ECommerce.API/appsettings.json`

   Replace the sample signing key with a secure value before production use.

3. **Apply database migrations**

   ```powershell
   dotnet ef migrations add InitialCreate `
        --project ECommerce.Infrastructure `
        --startup-project ECommerce.API

   dotnet ef database update `
        --project ECommerce.Infrastructure `
        --startup-project ECommerce.API
   ```

4. **Seed initial admin user *(optional)***

   The solution does not create default users. Use SQL or add a migration/seed routine to insert an Admin record with a hashed password if needed.

5. **Run the API**

   ```powershell
    dotnet run --project ECommerce.API
   ```

   Swagger UI is available at `https://localhost:<port>/swagger`.

## Key Features

- **Authentication**
  - Customer registration, login, and refresh-token rotation.
  - Role-based authorization (`Admin`, `Customer`).
- **Admin APIs**
  - CRUD for categories and products (including duplicate-name protection and soft delete).
  - Pagination, sorting, and search for product catalog.
  - Order review with status updates (Pending, Completed, Cancelled).
- **Customer APIs**
  - Profile management with email uniqueness checks and password updates.
  - Product browsing with search and pagination.
  - Order placement, stock validation, automatic stock adjustments, and cancellation while restoring inventory.
  - Order history retrieval.
- **Cross-cutting**
  - Async EF Core operations.
  - In-memory caching for category listings (invalidate on change).
  - Centralized exception handling with standardized problem responses.
  - Serilog request logging and rolling file sinks.

## Sample Requests

```http
POST /api/auth/register
Content-Type: application/json

{
  "name": "Jane Customer",
  "email": "jane@example.com",
  "password": "StrongPass123"
}
```

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "jane@example.com",
  "password": "StrongPass123"
}
```

```http
POST /api/orders
Authorization: Bearer <access-token>
Content-Type: application/json

{
  "items": [
    { "productId": "GUID-HERE", "quantity": 2 },
    { "productId": "GUID-HERE", "quantity": 1 }
  ]
}
```

```http
PUT /api/admin/orders/{orderId}/status
Authorization: Bearer <admin-access-token>
Content-Type: application/json

{ "status": "Completed" }
```

## Next Steps

- Add full integration tests for critical flows (auth, ordering, admin management).
- Implement background services (e.g., email notifications) using the existing architecture patterns.
- Expand caching strategy (e.g., product search results) or introduce distributed caching for scaling scenarios.

## Troubleshooting

- **JWT invalid / unauthorized**: confirm that the same issuer/audience/signing key values are used for token generation (Infrastructure) and validation (API).
- **EF Core migrations**: ensure the `--startup-project` is set to `ECommerce.API` so configuration is loaded correctly.
- **Soft-deleted products not visible**: Admin endpoints can include inactive/soft-deleted records by passing `includeInactive=true`. Customer endpoints always filter them out.

This project establishes the foundational backend for the machine test requirements. Extend the Application layer with additional commands/queries as domain behavior evolves while keeping the Clean Architecture boundaries intact.

