using Microsoft.EntityFrameworkCore.Migrations;

namespace depot.Server.Data.Migrations
{
    public partial class RemovedInstanceAuthorization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstanceAuthorizedUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InstanceAuthorizedUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CanRead = table.Column<bool>(type: "bit", nullable: false),
                    CanWrite = table.Column<bool>(type: "bit", nullable: false),
                    InstanceTypeId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstanceAuthorizedUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstanceAuthorizedUsers_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InstanceAuthorizedUsers_InstanceTypes_InstanceTypeId",
                        column: x => x.InstanceTypeId,
                        principalTable: "InstanceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InstanceAuthorizedUsers_ApplicationUserId",
                table: "InstanceAuthorizedUsers",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InstanceAuthorizedUsers_InstanceTypeId",
                table: "InstanceAuthorizedUsers",
                column: "InstanceTypeId");
        }
    }
}
