using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechBlog.Infrastructure.Identity.Configuration.Migrations
{
    public partial class RefreshToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdateAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 2, 18, 19, 18, 12, 286, DateTimeKind.Local).AddTicks(9074),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 2, 16, 21, 43, 37, 242, DateTimeKind.Local).AddTicks(9667));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 2, 18, 19, 18, 12, 286, DateTimeKind.Local).AddTicks(8936),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 2, 16, 21, 43, 37, 242, DateTimeKind.Local).AddTicks(9578));

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "varchar(300)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpires",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpires",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdateAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 2, 16, 21, 43, 37, 242, DateTimeKind.Local).AddTicks(9667),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 2, 18, 19, 18, 12, 286, DateTimeKind.Local).AddTicks(9074));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 2, 16, 21, 43, 37, 242, DateTimeKind.Local).AddTicks(9578),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 2, 18, 19, 18, 12, 286, DateTimeKind.Local).AddTicks(8936));
        }
    }
}
