using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HattmakarenWebbAppGrupp03.Migrations
{
    /// <inheritdoc />
    public partial class DatabasUpdateSmallFixesEverywhere : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignedOrders");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeEId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "HatOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "HatOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "TakenTime",
                table: "HatOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_EmployeeEId",
                table: "Orders",
                column: "EmployeeEId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Employees_EmployeeEId",
                table: "Orders",
                column: "EmployeeEId",
                principalTable: "Employees",
                principalColumn: "EId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Employees_EmployeeEId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_EmployeeEId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "EmployeeEId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "HatOrders");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "HatOrders");

            migrationBuilder.DropColumn(
                name: "TakenTime",
                table: "HatOrders");

            migrationBuilder.CreateTable(
                name: "AssignedOrders",
                columns: table => new
                {
                    EId = table.Column<int>(type: "int", nullable: false),
                    OId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WholeOrder = table.Column<bool>(type: "bit", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_AssignedOrders_OId",
                table: "AssignedOrders",
                column: "OId");
        }
    }
}
