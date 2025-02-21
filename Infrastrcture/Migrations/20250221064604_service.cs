using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastrcture.Migrations
{
    public partial class service : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CatId",
                table: "Services",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_CatId",
                table: "Services",
                column: "CatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Categories_CatId",
                table: "Services",
                column: "CatId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Categories_CatId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_CatId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "CatId",
                table: "Services");
        }
    }
}
