using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHub.Migrations
{
    /// <inheritdoc />
    public partial class SyncDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Countries",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Countries", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Services",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ServiceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        BasePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        VehicleType = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        RequiresQuote = table.Column<bool>(type: "bit", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Services", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "SystemDictionaries",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_SystemDictionaries", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Users",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Email = table.Column<string>(type: "nvarchar(450)", nullable: true),
            //        PhoneNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
            //        PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        HouseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        StreetName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Ward = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        District = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        City = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        RankLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Users", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Brands",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CountryId = table.Column<int>(type: "int", nullable: false),
            //        IsVehicleBrand = table.Column<bool>(type: "bit", nullable: false),
            //        IsPartBrand = table.Column<bool>(type: "bit", nullable: false),
            //        IsToyBrand = table.Column<bool>(type: "bit", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Brands", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Brands_Countries_CountryId",
            //            column: x => x.CountryId,
            //            principalTable: "Countries",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Orders",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        UserId = table.Column<int>(type: "int", nullable: true),
            //        OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Orders", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Orders_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "Id");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "SpareParts",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        BrandId = table.Column<int>(type: "int", nullable: false),
            //        ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CostPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
            //        Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
            //        StockQuantity = table.Column<int>(type: "int", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_SpareParts", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_SpareParts_Brands_BrandId",
            //            column: x => x.BrandId,
            //            principalTable: "Brands",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Vehicles",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        BrandId = table.Column<int>(type: "int", nullable: false),
            //        ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        VehicleType = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        FuelType = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Transmission = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
            //        CurrentPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
            //        Quantity = table.Column<int>(type: "int", nullable: true),
            //        EngineType = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        EngineCapacity = table.Column<double>(type: "float", nullable: true),
            //        SeatingCapacity = table.Column<int>(type: "int", nullable: false),
            //        Weight = table.Column<double>(type: "float", nullable: true),
            //        BodyStyle = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Vehicles", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Vehicles_Brands_BrandId",
            //            column: x => x.BrandId,
            //            principalTable: "Brands",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "OrderDetails",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        OrderId = table.Column<int>(type: "int", nullable: false),
            //        ProductType = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ProductId = table.Column<int>(type: "int", nullable: false),
            //        Quantity = table.Column<int>(type: "int", nullable: false),
            //        Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_OrderDetails", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_OrderDetails_Orders_OrderId",
            //            column: x => x.OrderId,
            //            principalTable: "Orders",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "VehicleColors",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        VehicleId = table.Column<int>(type: "int", nullable: false),
            //        ColorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_VehicleColors", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_VehicleColors_Vehicles_VehicleId",
            //            column: x => x.VehicleId,
            //            principalTable: "Vehicles",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Brands_CountryId",
            //    table: "Brands",
            //    column: "CountryId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_OrderDetails_OrderId",
            //    table: "OrderDetails",
            //    column: "OrderId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Orders_UserId",
            //    table: "Orders",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_SpareParts_BrandId",
            //    table: "SpareParts",
            //    column: "BrandId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Users_Email",
            //    table: "Users",
            //    column: "Email",
            //    unique: true,
            //    filter: "[Email] IS NOT NULL");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Users_PhoneNumber",
            //    table: "Users",
            //    column: "PhoneNumber",
            //    unique: true,
            //    filter: "[PhoneNumber] IS NOT NULL");

            //migrationBuilder.CreateIndex(
            //    name: "IX_VehicleColors_VehicleId",
            //    table: "VehicleColors",
            //    column: "VehicleId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Vehicles_BrandId",
            //    table: "Vehicles",
            //    column: "BrandId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "SpareParts");

            migrationBuilder.DropTable(
                name: "SystemDictionaries");

            migrationBuilder.DropTable(
                name: "VehicleColors");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
