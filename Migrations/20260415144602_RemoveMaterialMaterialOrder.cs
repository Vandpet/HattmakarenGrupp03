using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HattmakarenWebbAppGrupp03.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMaterialMaterialOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaterialMaterialOrder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_MaterialMaterialOrder_MaterialsMId",
                table: "MaterialMaterialOrder",
                column: "MaterialsMId");
        }
    }
}
