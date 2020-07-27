using Microsoft.EntityFrameworkCore.Migrations;

namespace depot.Server.Data.Migrations
{
    public partial class AdjustingFolderCleanup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FolderAuthorizedUsers_AspNetUsers_ApplicationUserId",
                table: "FolderAuthorizedUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_FolderAuthorizedUsers_AspNetUsers_ApplicationUserId",
                table: "FolderAuthorizedUsers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FolderAuthorizedUsers_AspNetUsers_ApplicationUserId",
                table: "FolderAuthorizedUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_FolderAuthorizedUsers_AspNetUsers_ApplicationUserId",
                table: "FolderAuthorizedUsers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
