using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BettingApi.Migrations
{
    /// <inheritdoc />
    public partial class UsernameChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "UserAccounts",
                newName: "UserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "UserAccounts",
                newName: "Username");
        }
    }
}
