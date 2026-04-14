using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HattmakarenWebbAppGrupp03.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hats_Materials_MaterialMId",
                table: "Hats");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Employees_EmployeeEId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_EmployeeEId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Hats_MaterialMId",
                table: "Hats");

            migrationBuilder.DropColumn(
                name: "EmployeeEId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "MaterialMId",
                table: "Hats");

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "Materials",
                type: "float(18)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CreatedById",
                table: "Orders",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Employees_CreatedById",
                table: "Orders",
                column: "CreatedById",
                principalTable: "Employees",
                principalColumn: "EId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Employees_CreatedById",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CreatedById",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeEId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Materials",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(18)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<int>(
                name: "MaterialMId",
                table: "Hats",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_EmployeeEId",
                table: "Orders",
                column: "EmployeeEId");

            migrationBuilder.CreateIndex(
                name: "IX_Hats_MaterialMId",
                table: "Hats",
                column: "MaterialMId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hats_Materials_MaterialMId",
                table: "Hats",
                column: "MaterialMId",
                principalTable: "Materials",
                principalColumn: "MId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Employees_EmployeeEId",
                table: "Orders",
                column: "EmployeeEId",
                principalTable: "Employees",
                principalColumn: "EId");
        }
    }
}
