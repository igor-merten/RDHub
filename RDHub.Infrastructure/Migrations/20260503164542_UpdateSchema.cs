using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RDHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Banks");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "PixCharges");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "Secrets");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Secrets");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Audits");

            migrationBuilder.RenameColumn(
                name: "Details",
                table: "Audits",
                newName: "Detail");

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "Audits",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Audits",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BankId",
                table: "Audits",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Audits",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PaidAmount",
                table: "Audits",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "Audits",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Audits",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TxId",
                table: "Audits",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    BankId = table.Column<Guid>(type: "uuid", nullable: false),
                    Agency = table.Column<string>(type: "text", nullable: false),
                    AccountNumber = table.Column<string>(type: "text", nullable: false),
                    Cnpj = table.Column<string>(type: "text", nullable: false),
                    PixKey = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "TxId",
                table: "Audits");

            migrationBuilder.RenameColumn(
                name: "Detail",
                table: "Audits",
                newName: "Details");

            migrationBuilder.AddColumn<Guid>(
                name: "BankId",
                table: "Secrets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Secrets",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Audits",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Banks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BankId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    TxId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PixCharges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BankId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Emv = table.Column<string>(type: "text", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    TxId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PixCharges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Account = table.Column<string>(type: "text", nullable: false),
                    Agency = table.Column<string>(type: "text", nullable: false),
                    BankId = table.Column<string>(type: "text", nullable: false),
                    Cnpj = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PixKey = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }
    }
}
