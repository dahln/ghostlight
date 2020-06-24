using Microsoft.EntityFrameworkCore.Migrations;

namespace depot.Server.Data.Migrations
{
    public partial class AddingInstanceLinkTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InstanceLinks",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    LinkId1 = table.Column<string>(nullable: true),
                    LinkId2 = table.Column<string>(nullable: true),
                    GroupId = table.Column<string>(nullable: true)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstanceLinks");
        }
    }
}
