using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERManager.Migrations
{
    /// <inheritdoc />
    public partial class @new : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "TreasuryMoneyTransfers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "MoneyTransactions",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "TreasuryMoneyTransfers");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "MoneyTransactions");
        }
    }
}
