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

var builder = WebApplication.CreateBuilder(args);

EnvLoader.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<IVehicleRepository, EfVehicleRepository>();
builder.Services.AddScoped<IServiceRepository, EfServiceRepository>();
builder.Services.AddScoped<IOrderRepository, EfOrderRepository>();
builder.Services.AddScoped<IBrandRepository, EfBrandRepository>();
builder.Services.AddScoped<ISparePartRepository, EfSparePartRepository>();

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<IFileService, CloudinaryService>();
builder.Services.AddHttpContextAccessor();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

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
        catch 
        {
        }

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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
