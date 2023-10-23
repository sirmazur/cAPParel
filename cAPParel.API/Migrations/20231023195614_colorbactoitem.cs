using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cAPParel.API.Migrations
{
    public partial class colorbactoitem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Piece");

            migrationBuilder.AddColumn<int>(
                name: "Color",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Items");

            migrationBuilder.AddColumn<int>(
                name: "Color",
                table: "Piece",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
