using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HattmakarenWebbAppGrupp03.Migrations
{
    /// <inheritdoc />
    public partial class initcreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedOrders_Employees_EId",
                table: "AssignedOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_AssignedOrders_Orders_OId",
                table: "AssignedOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerManagers_Customers_CId",
                table: "CustomerManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerManagers_Employees_EId",
                table: "CustomerManagers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerManagers",
                table: "CustomerManagers");

            migrationBuilder.DropIndex(
                name: "IX_CustomerManagers_CId",
                table: "CustomerManagers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssignedOrders",
                table: "AssignedOrders");

            migrationBuilder.DropIndex(
                name: "IX_AssignedOrders_OId",
                table: "AssignedOrders");

            migrationBuilder.DropColumn(
                name: "Rabatt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RabattBeskrivning",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "EId",
                table: "CustomerManagers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "CustomerCId",
                table: "CustomerManagers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeEId",
                table: "CustomerManagers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "EId",
                table: "AssignedOrders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeEId",
                table: "AssignedOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrderOId",
                table: "AssignedOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerManagers",
                table: "CustomerManagers",
                column: "EId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssignedOrders",
                table: "AssignedOrders",
                column: "EId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerManagers_CustomerCId",
                table: "CustomerManagers",
                column: "CustomerCId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerManagers_EmployeeEId",
                table: "CustomerManagers",
                column: "EmployeeEId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignedOrders_EmployeeEId",
                table: "AssignedOrders",
                column: "EmployeeEId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignedOrders_OrderOId",
                table: "AssignedOrders",
                column: "OrderOId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedOrders_Employees_EmployeeEId",
                table: "AssignedOrders",
                column: "EmployeeEId",
                principalTable: "Employees",
                principalColumn: "EId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedOrders_Orders_OrderOId",
                table: "AssignedOrders",
                column: "OrderOId",
                principalTable: "Orders",
                principalColumn: "OId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerManagers_Customers_CustomerCId",
                table: "CustomerManagers",
                column: "CustomerCId",
                principalTable: "Customers",
                principalColumn: "CId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerManagers_Employees_EmployeeEId",
                table: "CustomerManagers",
                column: "EmployeeEId",
                principalTable: "Employees",
                principalColumn: "EId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedOrders_Employees_EmployeeEId",
                table: "AssignedOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_AssignedOrders_Orders_OrderOId",
                table: "AssignedOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerManagers_Customers_CustomerCId",
                table: "CustomerManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerManagers_Employees_EmployeeEId",
                table: "CustomerManagers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerManagers",
                table: "CustomerManagers");

            migrationBuilder.DropIndex(
                name: "IX_CustomerManagers_CustomerCId",
                table: "CustomerManagers");

            migrationBuilder.DropIndex(
                name: "IX_CustomerManagers_EmployeeEId",
                table: "CustomerManagers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssignedOrders",
                table: "AssignedOrders");

            migrationBuilder.DropIndex(
                name: "IX_AssignedOrders_EmployeeEId",
                table: "AssignedOrders");

            migrationBuilder.DropIndex(
                name: "IX_AssignedOrders_OrderOId",
                table: "AssignedOrders");

            migrationBuilder.DropColumn(
                name: "CustomerCId",
                table: "CustomerManagers");

            migrationBuilder.DropColumn(
                name: "EmployeeEId",
                table: "CustomerManagers");

            migrationBuilder.DropColumn(
                name: "EmployeeEId",
                table: "AssignedOrders");

            migrationBuilder.DropColumn(
                name: "OrderOId",
                table: "AssignedOrders");

            migrationBuilder.AddColumn<decimal>(
                name: "Rabatt",
                table: "Orders",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "RabattBeskrivning",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "EId",
                table: "CustomerManagers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "EId",
                table: "AssignedOrders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerManagers",
                table: "CustomerManagers",
                columns: new[] { "EId", "CId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssignedOrders",
                table: "AssignedOrders",
                columns: new[] { "EId", "OId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerManagers_CId",
                table: "CustomerManagers",
                column: "CId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignedOrders_OId",
                table: "AssignedOrders",
                column: "OId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedOrders_Employees_EId",
                table: "AssignedOrders",
                column: "EId",
                principalTable: "Employees",
                principalColumn: "EId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedOrders_Orders_OId",
                table: "AssignedOrders",
                column: "OId",
                principalTable: "Orders",
                principalColumn: "OId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerManagers_Customers_CId",
                table: "CustomerManagers",
                column: "CId",
                principalTable: "Customers",
                principalColumn: "CId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerManagers_Employees_EId",
                table: "CustomerManagers",
                column: "EId",
                principalTable: "Employees",
                principalColumn: "EId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
