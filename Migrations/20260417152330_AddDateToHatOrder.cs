using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HattmakarenWebbAppGrupp03.Migrations
{
    /// <inheritdoc />
    public partial class AddDateToHatOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hats_Orders_OrderOId",
                table: "Hats");

            migrationBuilder.DropIndex(
                name: "IX_Hats_OrderOId",
                table: "Hats");

            migrationBuilder.DropColumn(
                name: "OrderOId",
                table: "Hats");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "HatOrders");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "HatOrders");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "HatOrders");

            migrationBuilder.DropColumn(
                name: "TakenTime",
                table: "HatOrders");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "HatOrders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "HatOrders");

            migrationBuilder.AddColumn<int>(
                name: "OrderOId",
                table: "Hats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "HatOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "HatOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "HatOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TakenTime",
                table: "HatOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hats_OrderOId",
                table: "Hats",
                column: "OrderOId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hats_Orders_OrderOId",
                table: "Hats",
                column: "OrderOId",
                principalTable: "Orders",
                principalColumn: "OId");
        }
    }
}
