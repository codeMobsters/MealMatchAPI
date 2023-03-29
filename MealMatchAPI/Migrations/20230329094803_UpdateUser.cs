using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMatchAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Favorites",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Favorites",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
