using Microsoft.EntityFrameworkCore.Migrations;

namespace glubfish.Server.Data.Migrations
{
    public partial class RemovingLinkedFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstanceLinks");

            migrationBuilder.DropColumn(
                name: "Primary",
                table: "Fields");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Primary",
                table: "Fields",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "InstanceLinks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GroupId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LinkId1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkId1_TypeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkId2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkId2_TypeId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstanceLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstanceLinks_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InstanceLinks_GroupId",
                table: "InstanceLinks",
                column: "GroupId");
        }
    }
}
