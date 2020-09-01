using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace glubfish.Server.Data.Migrations
{
    public partial class ChangeInstanceUpdateDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedOn",
                table: "Instances",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedOn",
                table: "Instances",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset));
        }
    }
}
