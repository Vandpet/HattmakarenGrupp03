using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HattmakarenWebbAppGrupp03.Migrations
{
    /// <inheritdoc />
    public partial class AddStartedByToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StartedById",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_StartedById",
                table: "Orders",
                column: "StartedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Employees_StartedById",
                table: "Orders",
                column: "StartedById",
                principalTable: "Employees",
                principalColumn: "EId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Employees_StartedById",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_StartedById",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "StartedById",
                table: "Orders");
        }
    }
}
