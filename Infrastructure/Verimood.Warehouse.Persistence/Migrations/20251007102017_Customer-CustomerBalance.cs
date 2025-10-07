using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Verimood.Warehouse.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CustomerCustomerBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Credit",
                table: "CustomerBalances");

            migrationBuilder.RenameColumn(
                name: "Debit",
                table: "CustomerBalances",
                newName: "Balance");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalBalance",
                table: "Customers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalBalance",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "Balance",
                table: "CustomerBalances",
                newName: "Debit");

            migrationBuilder.AddColumn<decimal>(
                name: "Credit",
                table: "CustomerBalances",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
