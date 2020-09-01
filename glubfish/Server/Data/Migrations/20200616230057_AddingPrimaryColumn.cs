using Microsoft.EntityFrameworkCore.Migrations;

namespace glubfish.Server.Data.Migrations
{
    public partial class AddingPrimaryColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkedTypeId",
                table: "Fields");

            migrationBuilder.AddColumn<bool>(
                name: "Primary",
                table: "Fields",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Primary",
                table: "Fields");

            migrationBuilder.AddColumn<string>(
                name: "LinkedTypeId",
                table: "Fields",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
