using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HattmakarenWebbAppGrupp03.Migrations
{
    /// <inheritdoc />
    public partial class AddSentDateToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Employees_StartedById",
                table: "Orders");

            migrationBuilder.AddColumn<DateTime>(
                name: "SentDate",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Employees_StartedById",
                table: "Orders",
                column: "StartedById",
                principalTable: "Employees",
                principalColumn: "EId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Employees_StartedById",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SentDate",
                table: "Orders");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Employees_StartedById",
                table: "Orders",
                column: "StartedById",
                principalTable: "Employees",
                principalColumn: "EId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
