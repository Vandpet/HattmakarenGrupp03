using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HattmakarenWebbAppGrupp03.Migrations
{
    /// <inheritdoc />
    public partial class HatSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HatSchedule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HatOrderId = table.Column<int>(type: "int", nullable: false),
                    HatOrderHId = table.Column<int>(type: "int", nullable: false),
                    HatOrderOId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HatSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HatSchedule_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HatSchedule_HatOrders_HatOrderHId_HatOrderOId",
                        columns: x => new { x.HatOrderHId, x.HatOrderOId },
                        principalTable: "HatOrders",
                        principalColumns: new[] { "HId", "OId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HatSchedule_EmployeeId",
                table: "HatSchedule",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HatSchedule_HatOrderHId_HatOrderOId",
                table: "HatSchedule",
                columns: new[] { "HatOrderHId", "HatOrderOId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HatSchedule");
        }
    }
}
