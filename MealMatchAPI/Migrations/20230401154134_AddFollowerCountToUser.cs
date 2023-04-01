using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealMatchAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddFollowerCountToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Followers",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Following",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Followers",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Following",
                table: "Users");
        }
    }
}
