//using System;
//using System.IO;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.EntityFrameworkCore;
//using AutoHub.Infrastructure;
//using AutoHub.Data;
//using AutoHub.Repositories;
//using AutoHub.Models.Settings;
//using AutoHub.Services;
//using AutoHub.Models.Settings; // Thay đổi namespace này cho đúng với class CloudinarySettings của bạn

//// =========================================================================
//// BƯỚC 1: NẠP FILE .ENV LÊN BỘ NHỚ TRƯỚC TIÊN (QUAN TRỌNG NHẤT)
//// =========================================================================
//EnvLoader.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

//// =========================================================================
//// BƯỚC 2: KHỞI TẠO WEB APPLICATION BUILDER
//// =========================================================================
//var builder = WebApplication.CreateBuilder(args);

//// =========================================================================
//// BƯỚC 3: TRUY XUẤT CHUỖI KẾT NỐI DATABASE (ƯU TIÊN .ENV, SƠ CUA APPSETTINGS)
//// =========================================================================

//var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") 
//    ?? builder.Configuration.GetConnectionString("DefaultConnection");

//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(connectionString));

//builder.Services.AddScoped<IUserRepository, EfUserRepository>();
//builder.Services.AddScoped<IVehicleRepository, EfVehicleRepository>();
//builder.Services.AddScoped<IServiceRepository, EfServiceRepository>();
//builder.Services.AddScoped<IOrderRepository, EfOrderRepository>();
//builder.Services.AddScoped<IBrandRepository, EfBrandRepository>();
//builder.Services.AddScoped<ISparePartRepository, EfSparePartRepository>();

//builder.Services.AddScoped<ILocationService, LocationService>();
//builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<IDashboardService, DashboardService>();
//builder.Services.AddScoped<ISystemDictionaryService, SystemDictionaryService>();

//builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
//builder.Services.AddScoped<IFileService, CloudinaryService>();
//builder.Services.AddHttpContextAccessor();


//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromMinutes(30);
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//});

//builder.Services.AddControllersWithViews();

//var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    try
//    {
//        context.Database.EnsureCreated();

//        var tableExists = false;
//        try
//        {
//            var conn = context.Database.GetDbConnection();
//            if (conn.State != System.Data.ConnectionState.Open) conn.Open();
//            using (var cmd = conn.CreateCommand())
//            {
//                cmd.CommandText = "SELECT COUNT(*) FROM sys.tables WHERE name = 'SystemDictionaries'";
//                var count = (int)(cmd.ExecuteScalar() ?? 0);
//                tableExists = count > 0;
//            }
//        }
//        catch 
//        {
//        }

//        var needSeed = !tableExists;
//        if (tableExists)
//        {
//            try
//            {
//                needSeed = !context.SystemDictionaries.Any(d => d.Type == "VehicleColor");
//            }
//            catch
//            {
//                needSeed = true;
//            }
//        }

//        if (needSeed)
//        {
//            var sqlPath = Path.Combine(Directory.GetCurrentDirectory(), "SeedData.sql");
//            if (File.Exists(sqlPath))
//            {
//                var sql = File.ReadAllText(sqlPath);
//                context.Database.ExecuteSqlRaw(sql);
//            }
//        }
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"Error during DB initialization: {ex.Message}");
//    }
//}

//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseRouting();

//app.UseSession();

//app.UseAuthorization();

//app.MapStaticAssets();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}")
//    .WithStaticAssets();

//app.Run();

using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using AutoHub.Infrastructure;
using AutoHub.Data;
using AutoHub.Repositories;
using AutoHub.Models.Settings;
using AutoHub.Services;

// =========================================================================
// BƯỚC 1: NẠP FILE .ENV LÊN BỘ NHỚ TRƯỚC TIÊN (QUAN TRỌNG NHẤT)
// =========================================================================
EnvLoader.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

// =========================================================================
// BƯỚC 2: KHỞI TẠO WEB APPLICATION BUILDER
// =========================================================================
var builder = WebApplication.CreateBuilder(args);

// =========================================================================
// BƯỚC 3: TRUY XUẤT CHUỖI KẾT NỐI DATABASE (ƯU TIÊN .ENV, SƠ CUA APPSETTINGS)
// =========================================================================
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Đăng ký các Repositories (DI)
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<IVehicleRepository, EfVehicleRepository>();
builder.Services.AddScoped<IServiceRepository, EfServiceRepository>();
builder.Services.AddScoped<IOrderRepository, EfOrderRepository>();
builder.Services.AddScoped<IBrandRepository, EfBrandRepository>();
builder.Services.AddScoped<ISparePartRepository, EfSparePartRepository>();
builder.Services.AddScoped<IMasterDataRepository, EfMasterDataRepository>();

// Đăng ký các Services (DI)
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ISystemDictionaryService, SystemDictionaryService>();

// =========================================================================
// SỬA TẠI ĐÂY: ĐỌC ĐỒNG THỜI TỪ FILE .ENV ĐỂ TRANH LỘ SECRET KEY
// =========================================================================
builder.Services.Configure<CloudinarySettings>(options =>
{
    options.CloudName = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME")
                        ?? builder.Configuration["CloudinarySettings:CloudName"];

    options.ApiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY")
                     ?? builder.Configuration["CloudinarySettings:ApiKey"];

    options.ApiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET")
                        ?? builder.Configuration["CloudinarySettings:ApiSecret"];
});

builder.Services.AddScoped<IFileService, CloudinaryService>();
builder.Services.AddHttpContextAccessor();

// Cấu hình Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Khởi tạo Database và Seed Data tự động từ file SQL mẫu nếu cần
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        context.Database.EnsureCreated();

        var tableExists = false;
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open) conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM sys.tables WHERE name = 'SystemDictionaries'";
                var count = (int)(cmd.ExecuteScalar() ?? 0);
                tableExists = count > 0;
            }
        }
        catch { }

        var needSeed = !tableExists;
        if (tableExists)
        {
            try
            {
                needSeed = !context.SystemDictionaries.Any(d => d.Type == "VehicleColor");
            }
            catch
            {
                needSeed = true;
            }
        }

        if (needSeed)
        {
            var sqlPath = Path.Combine(Directory.GetCurrentDirectory(), "SeedData.sql");
            if (File.Exists(sqlPath))
            {
                var sql = File.ReadAllText(sqlPath);
                context.Database.ExecuteSqlRaw(sql);
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during DB initialization: {ex.Message}");
    }
}

// Cấu hình HTTP Request Pipeline (Middleware)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
