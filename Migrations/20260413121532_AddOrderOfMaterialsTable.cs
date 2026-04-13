using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HattmakarenWebbAppGrupp03.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderOfMaterialsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderOfMaterials",
                columns: table => new
                {
                    OId = table.Column<int>(type: "int", nullable: false),
                    MoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderOfMaterials", x => new { x.OId, x.MoId });
                    table.ForeignKey(
                        name: "FK_OrderOfMaterials_MaterialOrders_MoId",
                        column: x => x.MoId,
                        principalTable: "MaterialOrders",
                        principalColumn: "MoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderOfMaterials_Orders_OId",
                        column: x => x.OId,
                        principalTable: "Orders",
                        principalColumn: "OId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderOfMaterials_MoId",
                table: "OrderOfMaterials",
                column: "MoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderOfMaterials");
        }
    }
}
