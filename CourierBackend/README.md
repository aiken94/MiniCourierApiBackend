# 📦 Courier Management API

A production-grade **courier/parcel delivery management REST API** built with **ASP.NET Core 10.0**, featuring JWT authentication, real-time package tracking, delivery history, admin management, role-based access control, and an analytics dashboard.

> Built as a portfolio project demonstrating clean architecture, enterprise design patterns, and modern C# development practices.

---

## ✨ Features

### 📋 Package Management
- **CRUD operations** for parcels with sender/receiver information
- **Unique tracking number** generation for every package
- **Status tracking** — Pending, In-Transit, Delivered, Failed
- **Public tracking endpoint** — anyone can track a package via its tracking number
- **File upload support** for package images (`multipart/form-data`)

### 🔐 Authentication & Authorization
- **JWT-based authentication** with access & refresh tokens
- **Role-based access control** — Admin vs. regular user permissions
- **Secure password hashing** via `PasswordHasher<T>`
- **Forgot / Reset password** flow powered by email
- Support for both **Bearer header** and **HttpOnly cookie** token delivery

### 📊 Analytics Dashboard
- **Summary statistics** — total packages, revenue, delivery histories, tracking views
- **Status breakdown** per package category
- **Revenue over time** — monthly revenue grouped by month/year (last 12 months)
- **Recent packages** — last 5 packages created
- **Caching** with `IMemoryCache` for dashboard performance
- **Date range filtering** for all dashboard metrics
- **Role-scoped data** — regular users see only their own packages

### 🚚 Delivery History
- Full **delivery event timeline** for each package
- Records remarks, location, and date for each checkpoint

### 🔎 Search, Filter, Sort & Pagination
- **Search** across packages by tracking number or description
- **Dynamic query filtering** — supports `eq`, `neq`, `gt`, `gte`, `lt`, `lte`, `like` operators
- **Field selection** — clients can request specific fields in responses
- **Sorting** by any entity property (ascending/descending)
- **Pagination** with configurable page size (max 100)

### 🛡️ Security & Middleware
- **Rate limiting** — per-IP and endpoint rules (e.g., 2 req/min on login)
- **Global exception middleware** — consistent error response format
- **CORS** configured for frontend origins
- **Input validation** via FluentValidation on all endpoints

### 📧 Email Service
- **SMTP email** integration (MailKit)
- **Password reset** email with templated HTML

---

## 🏗️ Architecture

```
CourierBackend/
├── Controllers/          # API endpoints (RESTful)
├── Services/             # Business logic layer
│   ├── Model/            # Domain services (Package, Admin, Dashboard, History)
│   ├── Auth/             # JWT, token management, current admin context
│   ├── Email/            # Email delivery (SMTP via MailKit)
│   └── ...               # Query, Pagination, Filter, Field Selection
├── Data/
│   ├── Repositories/     # Data access layer (interfaces + implementations)
│   ├── Resources/        # DTOs / response models
│   ├── Requests/         # Request models with validation
│   └── Enums/            # Shared enumerations (PackageStatus, etc.)
├── Models/               # Entity models (Package, Admin, Receiver, Sender, etc.)
├── Validators/           # FluentValidation validators
├── Middlewares/           # Exception handling pipeline
├── Configurations/       # EF Core entity type configurations
├── Helpers/              # Utilities (pagination, password, response helpers)
└── Migrations/           # EF Core database migrations
```

### Design Patterns Used

| Pattern | Implementation |
|---|---|
| **Repository Pattern** | `IPackageRepository`, `IAdminRepository`, `IDeliveryHistoryRepository` |
| **Service Layer** | `IPackageService`, `IAdminService`, `IDashboardService` |
| **Dependency Injection** | Built-in ASP.NET Core DI container |
| **Strategy Pattern** | Dynamic query filtering engine |
| **DTO Pattern** | `*Resource.cs` response models project entities to API contracts |
| **Middleware Pipeline** | Global exception handling middleware |
| **Caching** | `IMemoryCache` for dashboard and query results |

---

## 🛠️ Tech Stack

| Technology | Purpose |
|---|---|
| **.NET 10.0** | Framework |
| **ASP.NET Core Web API** | REST API |
| **Entity Framework Core 10.0** | ORM |
| **SQLite** | Database |
| **JWT Bearer Authentication** | Auth |
| **FluentValidation 12** | Request validation |
| **MailKit** | Email |
| **AspNetCoreRateLimit 5.0** | Rate limiting |
| **Swashbuckle / Swagger** | API documentation |
| **IMemoryCache** | Response caching |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- A code editor (VS Code, Rider, Visual Studio)

### 1. Clone the repository

```bash
git clone https://github.com/your-username/courier-backend.git
cd courier-backend
```

### 2. Configure the app

Copy the example settings file and customize:

```bash
cp appsettings.Example.json appsettings.json
```

Update the following in `appsettings.json`:

| Setting | Description |
|---|---|
| `ConnectionStrings:DefaultConnection` | SQLite connection string (default: `Data Source=Courier.db`) |
| `JwtSettings:SecretKey` | **Required** — change to a long, secure random key |
| `JwtSettings:Issuer` | JWT issuer name |
| `JwtSettings:Audience` | JWT audience |
| `EmailSettings` | SMTP credentials for password reset emails |
| `UserSettings` | App name, API version, description |

### 3. Apply database migrations

```bash
dotnet ef database update
```

Or at runtime (already configured in `Program.cs`), the database auto-migrates on first run.

### 4. Run the application

```bash
dotnet run
```

The API starts on:
- **HTTP:** `http://localhost:5052`
- **HTTPS:** `https://localhost:7011`

### 5. Explore the API

Open Swagger UI in development mode:

- **http://localhost:5052/swagger**

Or use the pre-configured Swagger playground to test all endpoints with JWT authorization.

---

## 📡 API Endpoints

### 🔐 Authentication

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/login` | ❌ | Login & get JWT tokens |
| POST | `/api/auth/refresh-token` | ❌ | Refresh access token |
| POST | `/api/auth/forgot-password` | ❌ | Request password reset email |
| POST | `/api/auth/reset-password` | ❌ | Reset password with token |

### 📦 Packages

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/package` | ✅ | List all packages (paginated) |
| GET | `/api/package/{id}` | ✅ | Get a single package |
| POST | `/api/package` | ✅ | Create a package (multipart) |
| PUT | `/api/package/{id}` | ✅ | Update a package (multipart) |
| DELETE | `/api/package/{id}` | ✅ | Delete a package |
| POST | `/api/package/track` | ❌ | Track a package by number |
| POST | `/api/package/{id}/update-status` | ✅ | Update package status |

### 👤 Admin Management

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/admin` | ✅ | List all admins (requires Admin role) |
| GET | `/api/admin/{id}` | ✅ | Get admin by ID (requires Admin role) |
| POST | `/api/admin` | ✅ | Create a new admin (requires Admin role) |
| PUT | `/api/admin/{id}/update` | ✅ | Update an admin |
| GET | `/api/admin/profile` | ✅ | Get current admin's profile |

### 📊 Dashboard

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/dashboard` | ✅ | Get analytics dashboard (with optional date filters) |

### 📜 Delivery History

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/history/package/{id}` | ✅ | Get all history for a package |
| POST | `/api/history/create` | ✅ | Add a delivery history event |
| DELETE | `/api/history/{id}/delete` | ✅ | Delete a history entry |

---

## 📁 Project Structure (Key Files)

```
CourierBackend/
├── Program.cs                          # App composition root (DI, middleware, config)
├── Controllers/
│   ├── PackageController.cs            # Package CRUD, tracking, status updates
│   ├── AuthController.cs               # Login, token refresh, password reset
│   ├── AdminController.cs              # Admin CRUD with role-based access
│   ├── DashboardController.cs          # Analytics dashboard
│   └── HistoryController.cs            # Delivery history management
├── Models/
│   ├── Package.cs                      # Core package entity
│   ├── Admin.cs                        # Admin user entity
│   ├── Receiver.cs / Sender.cs         # Party entities
│   └── PackageDeliveryHistory.cs       # Timeline event entity
├── Data/
│   ├── CourierContext.cs               # EF Core DbContext
│   ├── Repositories/                   # Data access implementations
│   ├── Resources/                      # Response DTOs
│   └── Requests/                       # Request DTOs
├── Services/
│   ├── Model/                          # Business logic services
│   ├── Auth/                           # JWT generation & validation
│   └── Email/                          # Email service
└── Validators/                         # FluentValidation rules
```

---

## 🧪 Testing

Tests are located in the `CourierBackend.Tests/` directory (sibling folder).

```bash
cd ../CourierBackend.Tests
dotnet test
```

---

## 📈 What This Project Demonstrates

| Skill | Evidence |
|---|---|
| **Clean Architecture** | Separation into Controllers → Services → Repositories with DI throughout |
| **RESTful API Design** | Proper HTTP verbs, status codes, resource-based URLs |
| **Authentication & Authorization** | JWT with refresh tokens, role-based access, cookie + header support |
| **Security Best Practices** | Rate limiting, input validation, password hashing, SQL injection prevention via EF |
| **Database Design** | EF Core with migrations, relationships, index configurations |
| **Caching Strategy** | Memory caching for dashboard and query responses |
| **Error Handling** | Global exception middleware returning consistent JSON errors |
| **API Documentation** | Swagger/OpenAPI with JWT bearer auth configured |
| **File Uploads** | Multipart form data for package images |
| **Email Integration** | SMTP via MailKit with template-based HTML emails |
| **Dynamic Querying** | Custom filter engine with operators, field selection, sorting |

---

## 📄 License

This project is available for educational and portfolio purposes.
