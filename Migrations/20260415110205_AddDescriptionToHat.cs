using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HattmakarenWebbAppGrupp03.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToHat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HatOrder_Employees_EId",
                table: "HatOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_HatOrder_Hats_HId",
                table: "HatOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_HatOrder_Orders_OId",
                table: "HatOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HatOrder",
                table: "HatOrder");

            migrationBuilder.RenameTable(
                name: "HatOrder",
                newName: "HatOrders");

            migrationBuilder.RenameIndex(
                name: "IX_HatOrder_OId",
                table: "HatOrders",
                newName: "IX_HatOrders_OId");

            migrationBuilder.RenameIndex(
                name: "IX_HatOrder_EId",
                table: "HatOrders",
                newName: "IX_HatOrders_EId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Hats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HatOrders",
                table: "HatOrders",
                columns: new[] { "HId", "OId" });

            migrationBuilder.AddForeignKey(
                name: "FK_HatOrders_Employees_EId",
                table: "HatOrders",
                column: "EId",
                principalTable: "Employees",
                principalColumn: "EId");

            migrationBuilder.AddForeignKey(
                name: "FK_HatOrders_Hats_HId",
                table: "HatOrders",
                column: "HId",
                principalTable: "Hats",
                principalColumn: "HId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HatOrders_Orders_OId",
                table: "HatOrders",
                column: "OId",
                principalTable: "Orders",
                principalColumn: "OId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HatOrders_Employees_EId",
                table: "HatOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_HatOrders_Hats_HId",
                table: "HatOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_HatOrders_Orders_OId",
                table: "HatOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HatOrders",
                table: "HatOrders");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Hats");

            migrationBuilder.RenameTable(
                name: "HatOrders",
                newName: "HatOrder");

            migrationBuilder.RenameIndex(
                name: "IX_HatOrders_OId",
                table: "HatOrder",
                newName: "IX_HatOrder_OId");

            migrationBuilder.RenameIndex(
                name: "IX_HatOrders_EId",
                table: "HatOrder",
                newName: "IX_HatOrder_EId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HatOrder",
                table: "HatOrder",
                columns: new[] { "HId", "OId" });

            migrationBuilder.AddForeignKey(
                name: "FK_HatOrder_Employees_EId",
                table: "HatOrder",
                column: "EId",
                principalTable: "Employees",
                principalColumn: "EId");

            migrationBuilder.AddForeignKey(
                name: "FK_HatOrder_Hats_HId",
                table: "HatOrder",
                column: "HId",
                principalTable: "Hats",
                principalColumn: "HId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HatOrder_Orders_OId",
                table: "HatOrder",
                column: "OId",
                principalTable: "Orders",
                principalColumn: "OId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
