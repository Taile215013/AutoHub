using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AutoHub.Infrastructure;
using AutoHub.Data;
using AutoHub.Repositories;
using AutoHub.Models.Settings;
using AutoHub.Services;

// ── 1. Nạp biến môi trường từ .env (phải chạy trước mọi thứ) ──────────────
EnvLoader.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

var builder = WebApplication.CreateBuilder(args);

// ── 2. Database ────────────────────────────────────────────────────────────
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connectionString));

// ── 3. Repositories ────────────────────────────────────────────────────────
builder.Services.AddScoped<IUserRepository,        EfUserRepository>();
builder.Services.AddScoped<IVehicleRepository,     EfVehicleRepository>();
builder.Services.AddScoped<IServiceRepository,     EfServiceRepository>();
builder.Services.AddScoped<IOrderRepository,       EfOrderRepository>();
builder.Services.AddScoped<IBrandRepository,       EfBrandRepository>();
builder.Services.AddScoped<ISparePartRepository,   EfSparePartRepository>();
builder.Services.AddScoped<IMasterDataRepository,  EfMasterDataRepository>();
builder.Services.AddScoped<IEmployeeRepository,    EfEmployeeRepository>();
builder.Services.AddScoped<ICartRepository,        EfCartRepository>();
builder.Services.AddScoped<IShowroomRepository,    EfShowroomRepository>();

// ── 4. Services ────────────────────────────────────────────────────────────
builder.Services.AddScoped<IAuthService,              AuthService>();
builder.Services.AddScoped<IDashboardService,         DashboardService>();
builder.Services.AddScoped<ILocationService,          LocationService>();
builder.Services.AddScoped<ISystemDictionaryService,  SystemDictionaryService>();
builder.Services.AddScoped<IEmployeeService,          EmployeeService>();

// ── 5. Cloudinary — đọc từ .env, không để secret trong appsettings ─────────
builder.Services.Configure<CloudinarySettings>(opt =>
{
    opt.CloudName = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME")
                    ?? builder.Configuration["CloudinarySettings:CloudName"];
    opt.ApiKey    = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY")
                    ?? builder.Configuration["CloudinarySettings:ApiKey"];
    opt.ApiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET")
                    ?? builder.Configuration["CloudinarySettings:ApiSecret"];
});
builder.Services.AddScoped<IFileService, CloudinaryService>();

// ── 6. HTTP infrastructure ─────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();

// Nén response (gzip/brotli) — giảm ~70% kích thước HTML/JSON gửi xuống client
builder.Services.AddResponseCompression(opt =>
{
    opt.EnableForHttps = true;
    opt.Providers.Add<BrotliCompressionProvider>();
    opt.Providers.Add<GzipCompressionProvider>();
});

builder.Services.AddSession(opt =>
{
    opt.IdleTimeout        = TimeSpan.FromMinutes(30);
    opt.Cookie.HttpOnly    = true;
    opt.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

// ── 7. Build app ───────────────────────────────────────────────────────────
var app = builder.Build();

// ── 8. Seed dữ liệu lần đầu nếu SystemDictionaries chưa có ────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.EnsureCreated();

        var needSeed = !await db.SystemDictionaries.AnyAsync(d => d.Type == "VehicleColor");
        if (needSeed)
        {
            var sqlPath = Path.Combine(Directory.GetCurrentDirectory(), "SeedData.sql");
            if (File.Exists(sqlPath))
                db.Database.ExecuteSqlRaw(await File.ReadAllTextAsync(sqlPath));
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Seed] Lỗi khởi tạo DB: {ex.Message}");
    }
}

// ── 9. Middleware pipeline ─────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseResponseCompression(); // phải đứng trước UseStaticFiles và UseRouting
app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapStaticAssets();

// ── 10. Routes ─────────────────────────────────────────────────────────────
app.MapControllerRoute(
    name:    "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name:    "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
