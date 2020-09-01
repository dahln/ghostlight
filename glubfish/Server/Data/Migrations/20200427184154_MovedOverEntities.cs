using Microsoft.EntityFrameworkCore.Migrations;

namespace glubfish.Server.Data.Migrations
{
    public partial class MovedOverEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupAuthorizedUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    IsGroupAdmin = table.Column<bool>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: true),
                    GroupId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupAuthorizedUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupAuthorizedUsers_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupAuthorizedUsers_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstanceTypes",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    GroupId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstanceTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstanceTypes_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fields",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Row = table.Column<int>(nullable: false),
                    Column = table.Column<int>(nullable: false),
                    ColumnSpan = table.Column<int>(nullable: false),
                    Options = table.Column<string>(nullable: true),
                    Optional = table.Column<bool>(nullable: false),
                    SearchShow = table.Column<bool>(nullable: false),
                    SearchOrder = table.Column<int>(nullable: false),
                    InstanceTypeId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fields_InstanceTypes_InstanceTypeId",
                        column: x => x.InstanceTypeId,
                        principalTable: "InstanceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstanceAuthorizedUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CanRead = table.Column<bool>(nullable: false),
                    CanWrite = table.Column<bool>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: true),
                    InstanceTypeId = table.Column<string>(nullable: true)
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
                name: "IX_Fields_InstanceTypeId",
                table: "Fields",
                column: "InstanceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAuthorizedUsers_ApplicationUserId",
                table: "GroupAuthorizedUsers",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAuthorizedUsers_GroupId",
                table: "GroupAuthorizedUsers",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_InstanceAuthorizedUsers_ApplicationUserId",
                table: "InstanceAuthorizedUsers",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InstanceAuthorizedUsers_InstanceTypeId",
                table: "InstanceAuthorizedUsers",
                column: "InstanceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_InstanceTypes_GroupId",
                table: "InstanceTypes",
                column: "GroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fields");

            migrationBuilder.DropTable(
                name: "GroupAuthorizedUsers");

            migrationBuilder.DropTable(
                name: "InstanceAuthorizedUsers");

            migrationBuilder.DropTable(
                name: "InstanceTypes");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
