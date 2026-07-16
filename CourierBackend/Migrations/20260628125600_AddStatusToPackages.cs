using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourierBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToPackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Packages",
                type: "TEXT",
                nullable: false,
                defaultValue: "pending");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Packages");
        }
    }
}
