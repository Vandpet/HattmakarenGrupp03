using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HattmakarenWebbAppGrupp03.Migrations
{
    /// <inheritdoc />
    public partial class addHatMaterial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HatMaterial");

            migrationBuilder.AddColumn<int>(
                name: "MaterialMId",
                table: "Hats",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HatMaterials",
                columns: table => new
                {
                    HId = table.Column<int>(type: "int", nullable: false),
                    MId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HatMaterials", x => new { x.HId, x.MId });
                    table.ForeignKey(
                        name: "FK_HatMaterials_Hats_HId",
                        column: x => x.HId,
                        principalTable: "Hats",
                        principalColumn: "HId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HatMaterials_Materials_MId",
                        column: x => x.MId,
                        principalTable: "Materials",
                        principalColumn: "MId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Hats_MaterialMId",
                table: "Hats",
                column: "MaterialMId");

            migrationBuilder.CreateIndex(
                name: "IX_HatMaterials_MId",
                table: "HatMaterials",
                column: "MId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hats_Materials_MaterialMId",
                table: "Hats",
                column: "MaterialMId",
                principalTable: "Materials",
                principalColumn: "MId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hats_Materials_MaterialMId",
                table: "Hats");

            migrationBuilder.DropTable(
                name: "HatMaterials");

            migrationBuilder.DropIndex(
                name: "IX_Hats_MaterialMId",
                table: "Hats");

            migrationBuilder.DropColumn(
                name: "MaterialMId",
                table: "Hats");

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
                name: "IX_HatMaterial_MaterialsMId",
                table: "HatMaterial",
                column: "MaterialsMId");
        }
    }
}
