using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastrcture.Migrations
{
    public partial class serviceMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MenuId",
                table: "Services",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_MenuId",
                table: "Services",
                column: "MenuId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Menus_MenuId",
                table: "Services",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Menus_MenuId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_MenuId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "MenuId",
                table: "Services");
        }
    }
}
