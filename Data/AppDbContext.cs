using Microsoft.EntityFrameworkCore;
using AutoHub.Models.Entities;

namespace AutoHub.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<Brand> Brands { get; set; } = null!;
        public DbSet<Vehicle> Vehicles { get; set; } = null!;
        public DbSet<VehicleColor> VehicleColors { get; set; } = null!;
        public DbSet<SparePart> SpareParts { get; set; } = null!;
        public DbSet<Service> Services { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Showroom> Showrooms { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public DbSet<SystemDictionary> SystemDictionaries { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<ProductCategory> ProductCategories { get; set; } = null!;
        public DbSet<VehicleName> VehicleNames { get; set; } = null!;
        public DbSet<VehicleVariant> VehicleVariants { get; set; } = null!;
        public DbSet<VehicleModelYear> VehicleModelYears { get; set; } = null!;
        public DbSet<SparePartCompatibility> SparePartCompatibilities { get; set; } = null!;

        // Địa chỉ hành chính Việt Nam
        public DbSet<Province> Provinces { get; set; } = null!;
        public DbSet<District> Districts { get; set; } = null!;
        public DbSet<Ward> Wards { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Brand>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Vehicle>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<VehicleColor>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<SparePart>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Service>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Showroom>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Order>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<OrderDetail>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<SystemDictionary>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Cart>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<CartItem>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<ProductCategory>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<VehicleName>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<VehicleVariant>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<VehicleModelYear>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<SparePartCompatibility>().HasQueryFilter(e => !e.IsDeleted);

            // Địa chỉ hành chính
            modelBuilder.Entity<Province>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<District>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Ward>().HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<Brand>()
                .HasOne(b => b.Country)
                .WithMany(c => c.Brands)
                .HasForeignKey(b => b.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Brand)
                .WithMany(b => b.Vehicles)
                .HasForeignKey(v => v.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VehicleName>()
                .HasOne(vn => vn.Brand)
                .WithMany(b => b.VehicleNames)
                .HasForeignKey(vn => vn.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VehicleName>()
                .HasIndex(vn => new { vn.BrandId, vn.NormalizedName })
                .IsUnique();

            modelBuilder.Entity<VehicleVariant>()
                .HasOne(vv => vv.VehicleName)
                .WithMany(vn => vn.Variants)
                .HasForeignKey(vv => vv.VehicleNameId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VehicleVariant>()
                .HasIndex(vv => new { vv.VehicleNameId, vv.Name })
                .IsUnique();

            modelBuilder.Entity<VehicleModelYear>()
                .HasOne(vy => vy.VehicleVariant)
                .WithMany(vv => vv.ModelYears)
                .HasForeignKey(vy => vy.VehicleVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VehicleModelYear>()
                .HasIndex(vy => new { vy.VehicleVariantId, vy.Year })
                .IsUnique();

            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.VehicleNameMaster)
                .WithMany(vn => vn.Vehicles)
                .HasForeignKey(v => v.VehicleNameId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.VehicleVariant)
                .WithMany(vv => vv.Vehicles)
                .HasForeignKey(v => v.VehicleVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.VehicleModelYear)
                .WithMany(vy => vy.Vehicles)
                .HasForeignKey(v => v.VehicleModelYearId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VehicleColor>()
                .HasOne(vc => vc.Vehicle)
                .WithMany(v => v.Colors)
                .HasForeignKey(vc => vc.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SparePart>()
                .HasOne(sp => sp.Brand)
                .WithMany(b => b.SpareParts)
                .HasForeignKey(sp => sp.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductCategory>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.ChildCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductCategory>()
                .HasIndex(c => new { c.CategoryType, c.Code })
                .IsUnique();

            modelBuilder.Entity<SparePart>()
                .HasOne(sp => sp.CategoryMaster)
                .WithMany(c => c.SpareParts)
                .HasForeignKey(sp => sp.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Service>()
                .HasOne(s => s.Category)
                .WithMany(c => c.Services)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SparePartCompatibility>()
                .HasOne(c => c.SparePart)
                .WithMany(sp => sp.Compatibilities)
                .HasForeignKey(c => c.SparePartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SparePartCompatibility>()
                .HasOne(c => c.VehicleName)
                .WithMany(vn => vn.SparePartCompatibilities)
                .HasForeignKey(c => c.VehicleNameId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SparePartCompatibility>()
                .HasOne(c => c.VehicleVariant)
                .WithMany(vv => vv.SparePartCompatibilities)
                .HasForeignKey(c => c.VehicleVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SparePartCompatibility>()
                .HasOne(c => c.VehicleModelYear)
                .WithMany(vy => vy.SparePartCompatibilities)
                .HasForeignKey(c => c.VehicleModelYearId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<Vehicle>()
                .Property(v => v.PurchasePrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Vehicle>()
                .Property(v => v.CurrentPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SparePart>()
                .Property(sp => sp.CostPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SparePart>()
                .Property(sp => sp.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Service>()
                .Property(s => s.BasePrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Employee>()
                .Property(e => e.BaseSalary)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Employee>()
                .Property(e => e.WeightKg)
                .HasPrecision(5, 1);

            // Địa chỉ hành chính — dùng Code string làm FK thay vì Id
            // để giữ đúng mã số chính thống của Nhà nước
            modelBuilder.Entity<Province>()
                .HasIndex(p => p.Code).IsUnique();

            modelBuilder.Entity<District>()
                .HasIndex(d => d.Code).IsUnique();

            modelBuilder.Entity<District>()
                .HasOne(d => d.Province)
                .WithMany(p => p.Districts)
                .HasForeignKey(d => d.ProvinceCode)
                .HasPrincipalKey(p => p.Code)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ward>()
                .HasIndex(w => w.Code).IsUnique();

            modelBuilder.Entity<Ward>()
                .HasOne(w => w.District)
                .WithMany(d => d.Wards)
                .HasForeignKey(w => w.DistrictCode)
                .HasPrincipalKey(d => d.Code)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
