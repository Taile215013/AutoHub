# AutoHub – Project Structure

## Top-Level Layout

```
AutoHub/
├── Areas/Admin/          # Admin area (MVC area)
├── Controllers/          # Customer-facing controllers
├── Data/                 # EF Core DbContext
├── Helpers/              # Static utility classes
├── Migrations/           # EF Core migration files
├── Models/
│   ├── Entities/         # Domain entity classes
│   ├── Infrastructure/   # EnvLoader (.env reader)
│   └── Settings/         # Strongly-typed config (e.g. CloudinarySettings)
├── Repositories/         # Data access layer (interfaces + EF implementations)
├── Services/             # Business logic layer (interfaces + implementations)
├── Views/                # Customer-facing Razor views
├── wwwroot/              # Static assets (css, js, images, lib)
├── docs/                 # UML diagrams (PlantUML), SQL scripts
├── .env                  # Local secrets (not committed)
├── appsettings.json
├── Program.cs            # App bootstrap, DI registration, middleware pipeline
└── SeedData.sql          # Initial data seed
```

## Areas – Admin

```
Areas/Admin/
├── Controllers/          # Admin controllers (decorated with [Area("Admin")])
├── ViewModels/           # Admin-specific view models (currently empty)
└── Views/                # Admin Razor views, _ViewImports, _ViewStart
```

Admin route pattern: `{area:exists}/{controller=Dashboard}/{action=Index}/{id?}`

## Architecture Patterns

### Repository Pattern
- Each aggregate has an interface (`IXxxRepository`) and an EF Core implementation (`EfXxxRepository`)
- Interfaces live alongside implementations in `Repositories/`
- Repositories are injected as `IXxxRepository` — never use `EfXxx` directly outside of DI registration

### Service Layer
- Business logic lives in `Services/`; some service interfaces are co-located in the same file as the implementation (e.g. `IAuthService` in `AuthService.cs`)
- Services are always resolved via their interface

### Dependency Injection
- All repositories and services registered as `Scoped` in `Program.cs`
- New repositories/services must be added to the DI container in `Program.cs`

## Entity Conventions

- All entities inherit from `BaseEntity` which provides: `Id` (int PK), `CreatedAt`, `UpdatedAt`, `IsDeleted`
- **Soft delete**: set `IsDeleted = true`, never use hard deletes. Global query filters in `AppDbContext` automatically exclude soft-deleted rows
- Always set `CreatedAt = DateTime.UtcNow` and `IsDeleted = false` when creating entities
- Set `UpdatedAt = DateTime.UtcNow` on every update
- Navigation properties default to `null!` or empty collections — never leave them uninitialized

## Naming Conventions

| Concept | Convention | Example |
|---|---|---|
| Entity | PascalCase, singular | `SparePart`, `VehicleColor` |
| Repository interface | `I` + entity + `Repository` | `IVehicleRepository` |
| Repository implementation | `Ef` + entity + `Repository` | `EfVehicleRepository` |
| Service interface | `I` + name + `Service` | `IAuthService` |
| Service implementation | name + `Service` | `AuthService` |
| Controller | entity/feature + `Controller` | `VehicleController` |
| Admin controller | same, with `[Area("Admin")]` | `VehicleController` in `Areas/Admin` |

## Authentication & Authorization

- No ASP.NET Identity — fully custom session-based auth
- Auth check: `HttpContext.Session.GetInt32("UserId") != null`
- Always redirect unauthenticated users to `AccountController.Login`
- Admin access is currently enforced manually (no `[Authorize]` attributes yet)

## ViewBag Usage

Dropdown/lookup data is passed to views via `ViewBag` (e.g. `ViewBag.Brands`, `ViewBag.VehicleTypes`). Lookups are sourced from the `SystemDictionaries` table via `ISystemDictionaryService`.

## Static Assets

- CSS/JS in `wwwroot/css/` and `wwwroot/js/`
- Third-party libraries in `wwwroot/lib/`
- Local images in `wwwroot/images/`; product images are hosted on Cloudinary
