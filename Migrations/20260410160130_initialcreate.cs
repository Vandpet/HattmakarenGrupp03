using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HattmakarenWebbAppGrupp03.Migrations
{
    /// <inheritdoc />
    public partial class initialcreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CId = table.Column<int>(type: "int", nullable: false)
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
                    table.PrimaryKey("PK_Customers", x => x.CId);
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
                    MId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MeasuringUnits = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.MId);
                });

            migrationBuilder.CreateTable(
                name: "CustomerManagers",
                columns: table => new
                {
                    EId = table.Column<int>(type: "int", nullable: false),
                    CId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerManagers", x => new { x.EId, x.CId });
                    table.ForeignKey(
                        name: "FK_CustomerManagers_Customers_CId",
                        column: x => x.CId,
                        principalTable: "Customers",
                        principalColumn: "CId",
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
                    MoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Printed = table.Column<bool>(type: "bit", nullable: false),
                    EmployeeEId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialOrders", x => x.MoId);
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
                    OId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Express = table.Column<bool>(type: "bit", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrelDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    EmployeeEId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OId);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CId",
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
                    MaterialOrdersMoId = table.Column<int>(type: "int", nullable: false),
                    MaterialsMId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialMaterialOrder", x => new { x.MaterialOrdersMoId, x.MaterialsMId });
                    table.ForeignKey(
                        name: "FK_MaterialMaterialOrder_MaterialOrders_MaterialOrdersMoId",
                        column: x => x.MaterialOrdersMoId,
                        principalTable: "MaterialOrders",
                        principalColumn: "MoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialMaterialOrder_Materials_MaterialsMId",
                        column: x => x.MaterialsMId,
                        principalTable: "Materials",
                        principalColumn: "MId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssignedOrders",
                columns: table => new
                {
                    EId = table.Column<int>(type: "int", nullable: false),
                    OId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignedOrders", x => new { x.EId, x.OId });
                    table.ForeignKey(
                        name: "FK_AssignedOrders_Employees_EId",
                        column: x => x.EId,
                        principalTable: "Employees",
                        principalColumn: "EId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignedOrders_Orders_OId",
                        column: x => x.OId,
                        principalTable: "Orders",
                        principalColumn: "OId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hats",
                columns: table => new
                {
                    HId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StandardHat = table.Column<bool>(type: "bit", nullable: false),
                    PicturePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeEId = table.Column<int>(type: "int", nullable: true),
                    OrderOId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hats", x => x.HId);
                    table.ForeignKey(
                        name: "FK_Hats_Employees_EmployeeEId",
                        column: x => x.EmployeeEId,
                        principalTable: "Employees",
                        principalColumn: "EId");
                    table.ForeignKey(
                        name: "FK_Hats_Orders_OrderOId",
                        column: x => x.OrderOId,
                        principalTable: "Orders",
                        principalColumn: "OId");
                });

            migrationBuilder.CreateTable(
                name: "HatMaterial",
                columns: table => new
                {
                    HatsHId = table.Column<int>(type: "int", nullable: false),
                    MaterialsMId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HatMaterial", x => new { x.HatsHId, x.MaterialsMId });
                    table.ForeignKey(
                        name: "FK_HatMaterial_Hats_HatsHId",
                        column: x => x.HatsHId,
                        principalTable: "Hats",
                        principalColumn: "HId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HatMaterial_Materials_MaterialsMId",
                        column: x => x.MaterialsMId,
                        principalTable: "Materials",
                        principalColumn: "MId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignedOrders_OId",
                table: "AssignedOrders",
                column: "OId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerManagers_CId",
                table: "CustomerManagers",
                column: "CId");

            migrationBuilder.CreateIndex(
                name: "IX_HatMaterial_MaterialsMId",
                table: "HatMaterial",
                column: "MaterialsMId");

            migrationBuilder.CreateIndex(
                name: "IX_Hats_EmployeeEId",
                table: "Hats",
                column: "EmployeeEId");

            migrationBuilder.CreateIndex(
                name: "IX_Hats_OrderOId",
                table: "Hats",
                column: "OrderOId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialMaterialOrder_MaterialsMId",
                table: "MaterialMaterialOrder",
                column: "MaterialsMId");

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
                name: "AssignedOrders");

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
