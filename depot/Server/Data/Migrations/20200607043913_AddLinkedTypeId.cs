using Microsoft.EntityFrameworkCore.Migrations;

namespace depot.Server.Data.Migrations
{
    public partial class AddLinkedTypeId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkedTypeId",
                table: "Fields",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkedTypeId",
                table: "Fields");
        }
    }
}
