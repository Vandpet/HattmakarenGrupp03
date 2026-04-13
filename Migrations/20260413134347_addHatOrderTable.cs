using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HattmakarenWebbAppGrupp03.Migrations
{
    /// <inheritdoc />
    public partial class addHatOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HatOrder",
                columns: table => new
                {
                    HId = table.Column<int>(type: "int", nullable: false),
                    OId = table.Column<int>(type: "int", nullable: false),
                    EId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HatOrder", x => new { x.HId, x.OId });
                    table.ForeignKey(
                        name: "FK_HatOrder_Employees_EId",
                        column: x => x.EId,
                        principalTable: "Employees",
                        principalColumn: "EId");
                    table.ForeignKey(
                        name: "FK_HatOrder_Hats_HId",
                        column: x => x.HId,
                        principalTable: "Hats",
                        principalColumn: "HId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HatOrder_Orders_OId",
                        column: x => x.OId,
                        principalTable: "Orders",
                        principalColumn: "OId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HatOrder_EId",
                table: "HatOrder",
                column: "EId");

            migrationBuilder.CreateIndex(
                name: "IX_HatOrder_OId",
                table: "HatOrder",
                column: "OId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HatOrder");
        }
    }
}
