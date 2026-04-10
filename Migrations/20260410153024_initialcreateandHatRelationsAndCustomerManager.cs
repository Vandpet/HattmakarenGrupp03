using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HattmakarenWebbAppGrupp03.Migrations
{
    /// <inheritdoc />
    public partial class initialcreateandHatRelationsAndCustomerManager : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Cid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Cid);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNr = table.Column<int>(type: "int", nullable: false),
                    accesslevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EId);
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    MID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MeasuringUnits = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.MID);
                });

            migrationBuilder.CreateTable(
                name: "CustomerManagers",
                columns: table => new
                {
                    EId = table.Column<int>(type: "int", nullable: false),
                    Cid = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerManagers", x => new { x.EId, x.Cid });
                    table.ForeignKey(
                        name: "FK_CustomerManagers_Customers_Cid",
                        column: x => x.Cid,
                        principalTable: "Customers",
                        principalColumn: "Cid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerManagers_Employees_EId",
                        column: x => x.EId,
                        principalTable: "Employees",
                        principalColumn: "EId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialOrders",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Utskriven = table.Column<bool>(type: "bit", nullable: false),
                    EmployeeEId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialOrders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MaterialOrders_Employees_EmployeeEId",
                        column: x => x.EmployeeEId,
                        principalTable: "Employees",
                        principalColumn: "EId");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Express = table.Column<bool>(type: "bit", nullable: false),
                    Rabatt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RabattBeskrivning = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BeställningsDatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PreliminärtLeveransDatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Anteckning = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    EmployeeEId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Cid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Employees_EmployeeEId",
                        column: x => x.EmployeeEId,
                        principalTable: "Employees",
                        principalColumn: "EId");
                });

            migrationBuilder.CreateTable(
                name: "MaterialMaterialOrder",
                columns: table => new
                {
                    MaterialOrdersID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaterialsMID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialMaterialOrder", x => new { x.MaterialOrdersID, x.MaterialsMID });
                    table.ForeignKey(
                        name: "FK_MaterialMaterialOrder_MaterialOrders_MaterialOrdersID",
                        column: x => x.MaterialOrdersID,
                        principalTable: "MaterialOrders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialMaterialOrder_Materials_MaterialsMID",
                        column: x => x.MaterialsMID,
                        principalTable: "Materials",
                        principalColumn: "MID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hats",
                columns: table => new
                {
                    HatID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StandardHat = table.Column<bool>(type: "bit", nullable: false),
                    PicturePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeEId = table.Column<int>(type: "int", nullable: true),
                    OrderId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hats", x => x.HatID);
                    table.ForeignKey(
                        name: "FK_Hats_Employees_EmployeeEId",
                        column: x => x.EmployeeEId,
                        principalTable: "Employees",
                        principalColumn: "EId");
                    table.ForeignKey(
                        name: "FK_Hats_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId");
                });

            migrationBuilder.CreateTable(
                name: "HatMaterial",
                columns: table => new
                {
                    HatsHatID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaterialsMID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HatMaterial", x => new { x.HatsHatID, x.MaterialsMID });
                    table.ForeignKey(
                        name: "FK_HatMaterial_Hats_HatsHatID",
                        column: x => x.HatsHatID,
                        principalTable: "Hats",
                        principalColumn: "HatID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HatMaterial_Materials_MaterialsMID",
                        column: x => x.MaterialsMID,
                        principalTable: "Materials",
                        principalColumn: "MID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerManagers_Cid",
                table: "CustomerManagers",
                column: "Cid");

            migrationBuilder.CreateIndex(
                name: "IX_HatMaterial_MaterialsMID",
                table: "HatMaterial",
                column: "MaterialsMID");

            migrationBuilder.CreateIndex(
                name: "IX_Hats_EmployeeEId",
                table: "Hats",
                column: "EmployeeEId");

            migrationBuilder.CreateIndex(
                name: "IX_Hats_OrderId",
                table: "Hats",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialMaterialOrder_MaterialsMID",
                table: "MaterialMaterialOrder",
                column: "MaterialsMID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialOrders_EmployeeEId",
                table: "MaterialOrders",
                column: "EmployeeEId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_EmployeeEId",
                table: "Orders",
                column: "EmployeeEId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerManagers");

            migrationBuilder.DropTable(
                name: "HatMaterial");

            migrationBuilder.DropTable(
                name: "MaterialMaterialOrder");

            migrationBuilder.DropTable(
                name: "Hats");

            migrationBuilder.DropTable(
                name: "MaterialOrders");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
