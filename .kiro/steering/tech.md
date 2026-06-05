# AutoHub – Tech Stack & Build System

## Runtime & Framework

- **Platform**: .NET 10 / ASP.NET Core MVC
- **Language**: C# with nullable reference types and implicit usings enabled
- **Project file**: `AutoHub.csproj` (SDK-style, `Microsoft.NET.Sdk.Web`)

## Key Libraries

| Library | Version | Purpose |
|---|---|---|
| `Microsoft.EntityFrameworkCore.SqlServer` | 10.0.7 | ORM + SQL Server provider |
| `Microsoft.EntityFrameworkCore.Tools` | 10.0.7 | EF Core CLI (migrations) |
| `CloudinaryDotNet` | 1.29.1 | Cloud image hosting |
| `SixLabors.ImageSharp` | 2.1.9 | Server-side image resize + WebP conversion |
| `BCrypt.Net-Next` | 4.2.0 | Password hashing (note: currently SHA-256 is used in AuthService — BCrypt is available but not yet wired up) |

## Database

- **SQL Server** (local dev: `localhost`, database `AutoHubDB`, trusted connection)
- EF Core Code-First with migrations in `Migrations/`
- Global query filters on `IsDeleted` field — all entities use soft delete
- Seed data loaded from `SeedData.sql` on first run if `SystemDictionaries` table is empty

## Configuration & Secrets

- Secrets are stored in `.env` at the project root and loaded by `EnvLoader` before the builder runs
- `.env` takes priority over `appsettings.json`
- Required env vars: `CONNECTION_STRING`, `CLOUDINARY_CLOUD_NAME`, `CLOUDINARY_API_KEY`, `CLOUDINARY_API_SECRET`
- `appsettings.json` holds non-secret defaults; `appsettings.Development.json` for dev overrides

## Session

- ASP.NET Core distributed memory session
- Idle timeout: 30 minutes, HttpOnly + Essential cookie
- Session keys: `UserId` (int), `UserName` (string)

## Image Handling

- All uploaded images are resized and converted to WebP before upload
- Main images: max 1920px wide, quality 80
- Thumbnails: max 480px wide, quality 60, stored under `thumbnails/{folder}` in Cloudinary
- Additional images stored as a JSON array of URLs in a single string column

## Common Commands

```bash
# Run the application
dotnet run

# Build
dotnet build

# Add a new EF Core migration
dotnet ef migrations add <MigrationName>

# Apply migrations to the database
dotnet ef database update

# Remove the last migration (if not yet applied)
dotnet ef migrations remove
```

> There is no test project at this time. No `dotnet test` target exists.
