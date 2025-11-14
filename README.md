# E-Commerce Backend API (Clean Architecture)

A comprehensive .NET 8 Web API solution for managing e-commerce operations including authentication, product and category administration, customer order workflows, and inventory management.

The architecture follows **Clean Architecture** principles with clear separation of concerns, dependency inversion, and testability in mind.

## Table of Contents

- [Architecture Overview](#architecture-overview)
- [Technology Stack](#technology-stack)
- [Solution Structure](#solution-structure)
- [Database Schema](#database-schema)
- [Authentication & Authorization](#authentication--authorization)
- [API Endpoints](#api-endpoints)
- [Features](#features)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Development](#development)

## Architecture Overview

The solution implements **Clean Architecture** with four main layers:

1. **Domain Layer** - Core business entities and enums (no dependencies)
2. **Application Layer** - Business logic, CQRS handlers, DTOs, validators
3. **Infrastructure Layer** - Data access, external services, implementations
4. **API Layer** - Controllers, middleware, request/response handling

### Architectural Patterns

- **CQRS (Command Query Responsibility Segregation)** - Separation of read and write operations using MediatR
- **Repository Pattern** - Generic repository with Unit of Work for data access
- **Dependency Injection** - Constructor injection throughout the application
- **Middleware Pipeline** - Custom exception handling and request logging
- **Soft Delete** - Entities are marked as deleted instead of being physically removed
- **Audit Trail** - Automatic tracking of CreatedDate, UpdatedDate, DeletedDate

## Technology Stack

### Framework & Runtime
- **.NET 8.0** - Target framework
- **ASP.NET Core 8.0** - Web API framework
- **C# 12** - Programming language with nullable reference types enabled

### Core Packages

#### API Layer (`ECommerce.API`)
- `Microsoft.AspNetCore.Authentication.JwtBearer` (8.0.0) - JWT authentication
- `Microsoft.EntityFrameworkCore.Tools` (8.0.2) - EF Core migrations
- `Serilog.AspNetCore` (8.0.1) - Structured logging
- `Swashbuckle.AspNetCore` (6.6.2) - Swagger/OpenAPI documentation

#### Application Layer (`ECommerce.Application`)
- `MediatR` (12.2.0) - CQRS pattern implementation
- `FluentValidation` (11.8.1) - Input validation
- `FluentValidation.DependencyInjectionExtensions` (11.8.1) - FluentValidation DI integration
- `AutoMapper` (12.0.1) - Object-to-object mapping
- `AutoMapper.Extensions.Microsoft.DependencyInjection` (12.0.1) - AutoMapper DI integration
- `Microsoft.Extensions.Logging.Abstractions` (8.0.0) - Logging abstractions
- `Microsoft.AspNetCore.Identity` (2.2.0) - Password hashing

#### Infrastructure Layer (`ECommerce.Infrastructure`)
- `Microsoft.EntityFrameworkCore` (8.0.2) - ORM framework
- `Microsoft.EntityFrameworkCore.SqlServer` (8.0.2) - SQL Server provider
- `Microsoft.EntityFrameworkCore.Design` (8.0.2) - EF Core design-time tools
- `Microsoft.AspNetCore.Identity` (2.3.1) - Password hashing utilities
- `Microsoft.Extensions.Caching.Memory` (8.0.1) - In-memory caching
- `Microsoft.IdentityModel.Tokens` (8.14.0) - JWT token validation
- `System.IdentityModel.Tokens.Jwt` (8.14.0) - JWT token generation
- `Serilog.Extensions.Logging` (7.0.0) - Serilog logging provider
- `Serilog.Sinks.File` (5.0.0) - File logging sink
- `Serilog.Settings.Configuration` (7.0.1) - Configuration-based Serilog setup
- `MailKit` (4.8.0) - Email sending capabilities
- `System.Text.Json` (8.0.6) - JSON serialization
- `Azure.Identity` (1.17.0) - Azure authentication (for future cloud integration)

### Database
- **SQL Server** - Primary database (supports SQL Server Express, LocalDB, or Azure SQL)

## Solution Structure

```
ECommerce/
 ├── ECommerce.Domain/
 │   └── ECommerce.Domain/
 │       ├── Entities/              # Domain entities
 │       │   ├── BaseEntity.cs      # Base class with audit fields
 │       │   ├── User.cs
 │       │   ├── Product.cs
 │       │   ├── Category.cs
 │       │   ├── Order.cs
 │       │   ├── OrderItem.cs
 │       │   └── RefreshToken.cs
 │       └── Enums/                 # Domain enums
 │           ├── UserRole.cs
 │           └── OrderStatus.cs
 │
 ├── ECommerce.Application/         # Application layer (business logic)
 │   ├── Common/
 │   │   ├── Behaviors/             # MediatR pipeline behaviors
 │   │   │   └── LoggingBehavior.cs # Request/response logging
 │   │   ├── DTOs/                  # Data transfer objects
 │   │   ├── Interfaces/            # Application interfaces
 │   │   │   ├── Authentication/
 │   │   │   ├── Persistence/
 │   │   │   ├── ICacheService.cs
 │   │   │   └── IEmailService.cs
 │   │   ├── Models/                # Application models
 │   │   └── Extensions/            # Extension methods
 │   ├── Features/                  # Feature-based organization
 │   │   ├── Authentication/        # Auth commands
 │   │   ├── Categories/            # Category commands & queries
 │   │   ├── Customers/             # Customer commands & queries
 │   │   ├── Orders/                # Order commands & queries
 │   │   └── Products/              # Product commands & queries
 │   ├── Mappings/                  # AutoMapper profiles
 │   └── DependencyInjection.cs     # DI registration
 │
 ├── ECommerce.Infrastructure/      # Infrastructure layer
 │   ├── Configurations/            # EF Core entity configurations
 │   ├── Data/
 │   │   ├── ECommerceDbContext.cs  # DbContext
 │   │   └── DatabaseSeeder.cs      # Database seeding
 │   ├── Migrations/                # EF Core migrations
 │   ├── Repositories/
 │   │   ├── Repository.cs          # Generic repository
 │   │   └── UnitOfWork.cs          # Unit of Work pattern
 │   ├── Services/
 │   │   ├── JwtTokenService.cs     # JWT token generation
 │   │   ├── MemoryCacheService.cs  # Caching implementation
 │   │   └── EmailService.cs        # Email service
 │   └── DependencyInjection.cs     # DI registration
 │
 └── ECommerce.API/                 # API layer (presentation)
     ├── Controllers/               # API controllers
     │   ├── Admin/                 # Admin-only controllers
     │   ├── ApiControllerBase.cs   # Base controller
     │   ├── AuthController.cs      # Authentication
     │   ├── ProductsController.cs  # Product endpoints
     │   ├── OrdersController.cs    # Order endpoints
     │   └── CustomersController.cs # Customer endpoints
     ├── Middlewares/
     │   └── ExceptionHandlingMiddleware.cs  # Global exception handler
     ├── Properties/
     │   └── launchSettings.json    # Launch configuration
     ├── appsettings.json           # Configuration
     ├── appsettings.Development.json
     └── Program.cs                 # Application entry point
```

### Layer Responsibilities

#### Domain Layer
- **Purpose**: Core business entities with no external dependencies
- **Entities**: 
  - `BaseEntity` - Abstract base class with `Id`, `CreatedDate`, `UpdatedDate`, `DeletedDate`, `IsDeleted`
  - `User` - User entity with role, email, password hash, orders, and refresh tokens
  - `Product` - Product entity with name, description, price, category, stock quantity, and active status
  - `Category` - Category entity with name and description
  - `Order` - Order entity with customer, status, total amount, order date, and items
  - `OrderItem` - Order item entity with product, quantity, and price at order time
  - `RefreshToken` - Refresh token entity for JWT token rotation
- **Enums**:
  - `UserRole` - Admin, Customer
  - `OrderStatus` - Pending, Completed, Cancelled

#### Application Layer
- **Purpose**: Business logic and use cases
- **Features**:
  - **CQRS Implementation**: Commands (write) and Queries (read) separated using MediatR
  - **Validation**: FluentValidation for request validation
  - **Mapping**: AutoMapper for entity-to-DTO mapping
  - **Logging**: MediatR pipeline behavior for request/response logging
  - **DTOs**: Data transfer objects for API responses
  - **Interfaces**: Abstractions for infrastructure services (repository, caching, JWT, email)

#### Infrastructure Layer
- **Purpose**: External concerns and data access
- **Features**:
  - **EF Core**: DbContext with entity configurations, migrations, and seeding
  - **Repository Pattern**: Generic repository with async operations, soft delete support, pagination, and includes
  - **Unit of Work**: Transaction management and repository coordination
  - **JWT Service**: Token generation and validation using HMAC SHA256
  - **Password Hashing**: ASP.NET Identity PasswordHasher for secure password storage
  - **Caching**: In-memory cache service for category listings
  - **Email Service**: MailKit-based email service (configurable)
  - **Logging**: Serilog integration with file sinks and rolling intervals

#### API Layer
- **Purpose**: HTTP API and request handling
- **Features**:
  - **Controllers**: RESTful API controllers with role-based authorization
  - **Middleware**: Exception handling, CORS, request logging
  - **Authentication**: JWT Bearer authentication with role-based authorization
  - **Swagger**: OpenAPI documentation with JWT support
  - **Health Checks**: Application health monitoring
  - **Database Seeding**: Automatic database seeding on startup

## Database Schema

### Entities

#### BaseEntity (Abstract)
- `Id` (Guid) - Primary key
- `CreatedDate` (DateTime) - Creation timestamp (UTC)
- `UpdatedDate` (DateTime?) - Last update timestamp (UTC)
- `DeletedDate` (DateTime?) - Deletion timestamp (UTC)
- `IsDeleted` (bool) - Soft delete flag

#### User
- `Id` (Guid) - Primary key
- `Name` (string, required) - User's full name
- `Email` (string, required, unique) - User's email address
- `PasswordHash` (string, required) - Hashed password (ASP.NET Identity)
- `Role` (UserRole) - User role (Admin, Customer)
- `Orders` (ICollection<Order>) - User's orders
- `RefreshTokens` (ICollection<RefreshToken>) - Refresh tokens

#### Category
- `Id` (Guid) - Primary key
- `Name` (string, required, unique) - Category name
- `Description` (string?) - Category description
- `Products` (ICollection<Product>) - Products in this category

#### Product
- `Id` (Guid) - Primary key
- `Name` (string, required) - Product name
- `Description` (string?) - Product description
- `Price` (decimal) - Product price
- `CategoryId` (Guid) - Foreign key to Category
- `Category` (Category?) - Navigation property
- `StockQuantity` (int) - Available stock quantity
- `IsActive` (bool) - Product active status
- `OrderItems` (ICollection<OrderItem>) - Order items

#### Order
- `Id` (Guid) - Primary key
- `CustomerId` (Guid) - Foreign key to User
- `Customer` (User?) - Navigation property
- `OrderDate` (DateTime) - Order date (UTC)
- `Status` (OrderStatus) - Order status (Pending, Completed, Cancelled)
- `TotalAmount` (decimal) - Total order amount
- `Items` (ICollection<OrderItem>) - Order items

#### OrderItem
- `Id` (Guid) - Primary key
- `OrderId` (Guid) - Foreign key to Order
- `Order` (Order?) - Navigation property
- `ProductId` (Guid) - Foreign key to Product
- `Product` (Product?) - Navigation property
- `Quantity` (int) - Item quantity
- `PriceAtOrder` (decimal) - Price at the time of order

#### RefreshToken
- `Id` (Guid) - Primary key
- `Token` (string, required) - Refresh token value
- `UserId` (Guid) - Foreign key to User
- `User` (User?) - Navigation property
- `ExpiresAt` (DateTime) - Token expiration date
- `IsRevoked` (bool) - Revocation flag

### Relationships

- **User** → **Order** (One-to-Many)
- **User** → **RefreshToken** (One-to-Many)
- **Category** → **Product** (One-to-Many)
- **Order** → **OrderItem** (One-to-Many)
- **Product** → **OrderItem** (One-to-Many)

### Database Features

- **Soft Delete**: All entities inherit from `BaseEntity` and support soft deletion
- **Audit Trail**: Automatic tracking of creation, update, and deletion timestamps
- **Entity Configurations**: Fluent API configurations in `Configurations/` folder
- **Migrations**: EF Core code-first migrations
- **Seeding**: Automatic database seeding on startup (users, categories, products, orders)

## Authentication & Authorization

### JWT Authentication

The API uses **JWT Bearer Authentication** with the following configuration:

- **Algorithm**: HMAC SHA256
- **Token Type**: Access Token + Refresh Token
- **Access Token Expiration**: 60 minutes (configurable)
- **Refresh Token Expiration**: 7 days (configurable)
- **Claims**: User ID, Email, Name, Role

### Token Structure

**Access Token Claims:**
- `sub` - User ID
- `email` - User email
- `name` - User name
- `role` - User role (Admin, Customer)
- `jti` - JWT ID

### Authorization Policies

- **AdminOnly**: Requires `Admin` role
- **CustomerOnly**: Requires `Customer` role

### Authentication Endpoints

- `POST /api/auth/register` - Register a new customer
- `POST /api/auth/login` - Login and receive tokens
- `POST /api/auth/refresh-token` - Refresh access token

### Password Security

- **Hashing**: ASP.NET Identity PasswordHasher
- **Algorithm**: PBKDF2 with HMAC-SHA256
- **Salt**: Automatically generated per password

## API Endpoints

### Authentication Endpoints

#### Register
```
POST /api/auth/register
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john.doe@example.com",
  "password": "Password123!"
}
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": {
    "token": "base64-encoded-token",
    "userId": "guid",
    "expiresAt": "2024-01-01T00:00:00Z"
  },
  "expiresAt": "2024-01-01T00:00:00Z",
  "user": {
    "id": "guid",
    "name": "John Doe",
    "email": "john.doe@example.com",
    "role": "Customer"
  }
}
```

#### Login
```
POST /api/auth/login
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "password": "Password123!"
}
```

#### Refresh Token
```
POST /api/auth/refresh-token
Content-Type: application/json

{
  "accessToken": "expired-access-token",
  "refreshToken": "valid-refresh-token"
}
```

### Product Endpoints

#### Get Products (Public)
```
GET /api/products?name=smartphone&categoryId={guid}&pageNumber=1&pageSize=10&sortBy=price&isAscending=true
```

#### Get Product by ID (Public)
```
GET /api/products/{id}
```

### Order Endpoints

#### Place Order (Customer)
```
POST /api/orders
Authorization: Bearer {token}
Content-Type: application/json

{
  "items": [
    { "productId": "guid", "quantity": 2 },
    { "productId": "guid", "quantity": 1 }
  ]
}
```

#### Get Order by ID
```
GET /api/orders/{id}
Authorization: Bearer {token}
```

#### Cancel Order
```
POST /api/orders/{id}/cancel
Authorization: Bearer {token}
```

### Customer Endpoints

#### Get Profile (Customer)
```
GET /api/customers/me
Authorization: Bearer {token}
```

#### Update Profile (Customer)
```
PUT /api/customers/me
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john.doe@example.com",
  "newPassword": "NewPassword123!"
}
```

#### Get My Orders (Customer)
```
GET /api/customers/me/orders?pageNumber=1&pageSize=10
Authorization: Bearer {token}
```

### Admin Endpoints

#### Categories

- `GET /api/admin/categories` - Get all categories
- `GET /api/admin/categories/{id}` - Get category by ID
- `POST /api/admin/categories` - Create category
- `PUT /api/admin/categories/{id}` - Update category
- `DELETE /api/admin/categories/{id}` - Delete category (soft delete)

#### Products

- `GET /api/admin/products` - Get all products (with pagination, filtering, sorting)
- `GET /api/admin/products/{id}` - Get product by ID
- `POST /api/admin/products` - Create product
- `PUT /api/admin/products/{id}` - Update product
- `DELETE /api/admin/products/{id}` - Delete product (soft delete)

#### Orders

- `GET /api/admin/orders` - Get all orders (with pagination, filtering)
- `GET /api/admin/orders/{id}` - Get order by ID
- `PUT /api/admin/orders/{id}/status` - Update order status

## Features

### Authentication & Authorization
- ✅ Customer registration with email validation
- ✅ User login with JWT token generation
- ✅ Refresh token rotation for secure token renewal
- ✅ Role-based authorization (Admin, Customer)
- ✅ Password hashing using ASP.NET Identity
- ✅ JWT token validation and expiration handling

### Product Management
- ✅ Product CRUD operations (Admin)
- ✅ Product search with filtering (name, category)
- ✅ Product pagination and sorting
- ✅ Stock quantity management
- ✅ Product active/inactive status
- ✅ Soft delete support

### Category Management
- ✅ Category CRUD operations (Admin)
- ✅ Category listing with caching
- ✅ Duplicate name protection
- ✅ Soft delete support

### Order Management
- ✅ Order placement with stock validation
- ✅ Automatic stock deduction on order placement
- ✅ Order status management (Pending, Completed, Cancelled)
- ✅ Stock restoration on order cancellation
- ✅ Order history retrieval
- ✅ Order total calculation
- ✅ Price snapshot at order time (PriceAtOrder)

### Customer Management
- ✅ Customer profile retrieval
- ✅ Profile update with email uniqueness check
- ✅ Password update
- ✅ Order history with pagination

### Cross-Cutting Concerns
- ✅ CQRS pattern implementation (MediatR)
- ✅ Repository pattern with Unit of Work
- ✅ FluentValidation for request validation
- ✅ AutoMapper for entity-to-DTO mapping
- ✅ Global exception handling middleware
- ✅ Structured logging with Serilog
- ✅ File-based logging with rolling intervals
- ✅ In-memory caching for categories
- ✅ Health checks endpoint
- ✅ CORS support
- ✅ Swagger/OpenAPI documentation
- ✅ Database seeding on startup
- ✅ Soft delete implementation
- ✅ Audit trail (CreatedDate, UpdatedDate, DeletedDate)

## Prerequisites

- [.NET SDK 8.0+](https://dotnet.microsoft.com/en-us/download)
- SQL Server or SQL Server Express / LocalDB
- Visual Studio 2022 or VS Code (optional)
- Postman or similar API testing tool (optional)

## Getting Started

### 1. Clone the Repository

```powershell
git clone <repository-url>
cd ECommerce
```

### 2. Restore Dependencies

```powershell
dotnet restore
```

### 3. Configure Connection String

Update the `ConnectionStrings:DefaultConnection` in:
- `ECommerce.API/appsettings.json`
- `ECommerce.API/appsettings.Development.json`

**Example:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost;Initial Catalog=ECommerce;Integrated Security=True;TrustServerCertificate=True"
  }
}
```

### 4. Configure JWT Settings

Update the `JwtSettings` section in `appsettings.json`:

```json
{
  "JwtSettings": {
    "Issuer": "ECommerce.Api",
    "Audience": "ECommerce.Client",
    "SigningKey": "Your-Secure-256-bit-Key-Here-Minimum-32-characters",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

**⚠️ Important:** Replace the signing key with a secure, randomly generated key before production use.

### 5. Apply Database Migrations

```powershell
dotnet ef migrations add InitialCreate `
     --project ECommerce.Infrastructure `
     --startup-project ECommerce.API

dotnet ef database update `
     --project ECommerce.Infrastructure `
     --startup-project ECommerce.API
```

### 6. Run the Application

```powershell
dotnet run --project ECommerce.API
```

### 7. Access Swagger UI

Navigate to `https://localhost:<port>/swagger` to access the Swagger UI for API documentation and testing.

### 8. Database Seeding

The application automatically seeds the database on startup with:
- **Users**: Admin user and sample customers (default password: `Password123!`)
- **Categories**: Electronics, Clothing, Books, Home & Garden, Sports & Outdoors
- **Products**: Sample products across all categories
- **Orders**: Sample orders with various statuses

**Default Admin Credentials:**
- Email: `admin@ecommerce.com`
- Password: `Password123!`

**Default Customer Credentials:**
- Email: `john.doe@example.com`
- Password: `Password123!`

## Configuration

### appsettings.json

The application uses `appsettings.json` and `appsettings.Development.json` for configuration.

#### Connection Strings

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost;Initial Catalog=ECommerce;Integrated Security=True;TrustServerCertificate=True"
  }
}
```

#### JWT Settings

```json
{
  "JwtSettings": {
    "Issuer": "ECommerce.Api",
    "Audience": "ECommerce.Client",
    "SigningKey": "Your-Secure-256-bit-Key-Here-Minimum-32-characters",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

#### Serilog Settings

```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.File" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information",
        "ECommerce.Application": "Information"
      }
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "Logs/log-.txt",
          "rollingInterval": "Day",
          "shared": true,
          "retainedFileCountLimit": 10
        }
      }
    ]
  }
}
```

#### Email Settings

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "FromEmail": "your-email@gmail.com",
    "FromName": "E-Commerce Team",
    "EnableSsl": true
  }
}
```

### Environment Variables

For production, consider using environment variables or Azure Key Vault for sensitive configuration:

- `ConnectionStrings:DefaultConnection`
- `JwtSettings:SigningKey`
- `EmailSettings:SmtpPassword`

## Development

### Project Structure

The solution follows Clean Architecture with clear separation of concerns:

1. **Domain Layer** - No dependencies, pure business entities
2. **Application Layer** - Depends on Domain, contains business logic
3. **Infrastructure Layer** - Depends on Application and Domain, contains implementations
4. **API Layer** - Depends on all layers, contains controllers and middleware

### Dependency Injection

Dependencies are registered in:
- `ECommerce.Application/DependencyInjection.cs` - Application services
- `ECommerce.Infrastructure/DependencyInjection.cs` - Infrastructure services
- `ECommerce.API/Program.cs` - API services

### Repository Pattern

The solution uses a generic repository pattern with Unit of Work:

```csharp
// Generic repository interface
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate = null, ...);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    // ... more methods
}

// Unit of Work
public interface IUnitOfWork
{
    IRepository<User> Users { get; }
    IRepository<Product> Products { get; }
    IRepository<Category> Categories { get; }
    IRepository<Order> Orders { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

### CQRS Pattern

Commands and Queries are separated using MediatR:

**Command Example:**
```csharp
public class CreateProductCommand : IRequest<ProductDto>
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    // ... more properties
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    // Handler implementation
}
```

**Query Example:**
```csharp
public class GetProductByIdQuery : IRequest<ProductDto>
{
    public Guid Id { get; set; }
}

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    // Handler implementation
}
```

### Validation

FluentValidation is used for request validation:

```csharp
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
        
        RuleFor(x => x.Price)
            .GreaterThan(0);
    }
}
```

### Mapping

AutoMapper is used for entity-to-DTO mapping:

```csharp
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<Order, OrderDto>();
        // ... more mappings
    }
}
```

### Logging

Serilog is used for structured logging:

- **Request Logging**: Automatic HTTP request/response logging
- **MediatR Logging**: Request/response logging for all MediatR handlers
- **File Logging**: Rolling file logs in `Logs/` directory
- **Log Levels**: Information, Warning, Error

### Exception Handling

Global exception handling middleware catches and handles:

- **ValidationException**: Returns 400 Bad Request with validation errors
- **UnauthorizedAccessException**: Returns 401 Unauthorized
- **Generic Exception**: Returns 500 Internal Server Error

### Caching

In-memory caching is used for:

- **Category Listings**: Cached to reduce database queries
- **Cache Invalidation**: Automatic invalidation on category create/update/delete

### Database Migrations

EF Core migrations are used for database schema management:

```powershell
# Create a new migration
dotnet ef migrations add MigrationName `
     --project ECommerce.Infrastructure `
     --startup-project ECommerce.API

# Apply migrations
dotnet ef database update `
     --project ECommerce.Infrastructure `
     --startup-project ECommerce.API

# Remove last migration
dotnet ef migrations remove `
     --project ECommerce.Infrastructure `
     --startup-project ECommerce.API
```

### Database Seeding

The `DatabaseSeeder` class automatically seeds the database on startup:

- **Users**: Admin and sample customers
- **Categories**: Sample categories
- **Products**: Sample products
- **Orders**: Sample orders

Seeding only occurs if the database is empty (no existing data).

### Testing the API

#### Using Swagger UI

1. Navigate to `https://localhost:<port>/swagger`
2. Click "Authorize" button
3. Enter your JWT token (format: `Bearer {token}`)
4. Test endpoints directly from Swagger UI

#### Using Postman

1. Register a new user: `POST /api/auth/register`
2. Login: `POST /api/auth/login`
3. Copy the `accessToken` from the response
4. Set Authorization header: `Bearer {accessToken}`
5. Test other endpoints

#### Using cURL

```bash
# Register
curl -X POST "https://localhost:5001/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{"name":"John Doe","email":"john@example.com","password":"Password123!"}'

# Login
curl -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"john@example.com","password":"Password123!"}'

# Get Products
curl -X GET "https://localhost:5001/api/products" \
  -H "Authorization: Bearer {token}"
```

### Health Checks

The application includes a health check endpoint:

```
GET /health
```

Returns `200 OK` if the application is healthy.

### CORS Configuration

CORS is configured to allow all origins, headers, and methods (for development). For production, restrict to specific origins:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
        policy.WithOrigins("https://your-frontend.com")
            .AllowAnyHeader()
            .AllowAnyMethod());
});
```

## License

This project is licensed under the MIT License.

