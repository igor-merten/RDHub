using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RDHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBankIdToCredential : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BankId",
                table: "Credentials",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankId",
                table: "Credentials");
        }
    }
}
