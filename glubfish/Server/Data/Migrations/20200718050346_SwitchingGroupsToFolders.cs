using Microsoft.EntityFrameworkCore.Migrations;

namespace glubfish.Server.Data.Migrations
{
    public partial class SwitchingGroupsToFolders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fields_InstanceTypes_InstanceTypeId",
                table: "Fields");

            migrationBuilder.DropTable(
                name: "GroupAuthorizedUsers");

            migrationBuilder.DropTable(
                name: "InstanceTypes");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Fields_InstanceTypeId",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "InstanceTypeId",
                table: "Fields");

            migrationBuilder.AddColumn<string>(
                name: "DataTypeId",
                table: "Fields",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Folders",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataTypes",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    FolderId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataTypes_Folders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "Folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FolderAuthorizedUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    IsFolderAdmin = table.Column<bool>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: true),
                    FolderId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderAuthorizedUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FolderAuthorizedUsers_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FolderAuthorizedUsers_Folders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "Folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fields_DataTypeId",
                table: "Fields",
                column: "DataTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DataTypes_FolderId",
                table: "DataTypes",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_FolderAuthorizedUsers_ApplicationUserId",
                table: "FolderAuthorizedUsers",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FolderAuthorizedUsers_FolderId",
                table: "FolderAuthorizedUsers",
                column: "FolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_DataTypes_DataTypeId",
                table: "Fields",
                column: "DataTypeId",
                principalTable: "DataTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fields_DataTypes_DataTypeId",
                table: "Fields");

            migrationBuilder.DropTable(
                name: "DataTypes");

            migrationBuilder.DropTable(
                name: "FolderAuthorizedUsers");

            migrationBuilder.DropTable(
                name: "Folders");

            migrationBuilder.DropIndex(
                name: "IX_Fields_DataTypeId",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "DataTypeId",
                table: "Fields");

            migrationBuilder.AddColumn<string>(
                name: "InstanceTypeId",
                table: "Fields",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupAuthorizedUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    GroupId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsGroupAdmin = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GroupId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "IX_InstanceTypes_GroupId",
                table: "InstanceTypes",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_InstanceTypes_InstanceTypeId",
                table: "Fields",
                column: "InstanceTypeId",
                principalTable: "InstanceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
