using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HattmakarenWebbAppGrupp03.Migrations
{
    /// <inheritdoc />
    public partial class HandledOrdersRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeEId",
                table: "Orders",
                type: "int",
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
    }
}
