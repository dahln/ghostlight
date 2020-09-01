using Microsoft.EntityFrameworkCore.Migrations;

namespace glubfish.Server.Data.Migrations
{
    public partial class AddingTypesToLinks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkId1_TypeId",
                table: "InstanceLinks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkId2_TypeId",
                table: "InstanceLinks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkId1_TypeId",
                table: "InstanceLinks");

            migrationBuilder.DropColumn(
                name: "LinkId2_TypeId",
                table: "InstanceLinks");
        }
    }
}
