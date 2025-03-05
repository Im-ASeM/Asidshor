using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastrcture.Migrations
{
    public partial class admins : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminMenu");

            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "Menus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Menus_AdminId",
                table: "Menus",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_Admins_AdminId",
                table: "Menus",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Menus_Admins_AdminId",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_Menus_AdminId",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Menus");

            migrationBuilder.CreateTable(
                name: "AdminMenu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdminId = table.Column<int>(type: "int", nullable: false),
                    Menu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MenuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminMenu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminMenu_Admins_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Admins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdminMenu_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminMenu_AdminId",
                table: "AdminMenu",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminMenu_MenuId",
                table: "AdminMenu",
                column: "MenuId");
        }
    }
}
