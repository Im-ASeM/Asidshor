using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastrcture.Migrations
{
    public partial class yo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMenu_Menus_MenuId",
                table: "UserMenu");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMenu_Users_UserId",
                table: "UserMenu");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserMenu",
                table: "UserMenu");

            migrationBuilder.RenameTable(
                name: "UserMenu",
                newName: "UserMenus");

            migrationBuilder.RenameIndex(
                name: "IX_UserMenu_UserId",
                table: "UserMenus",
                newName: "IX_UserMenus_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserMenu_MenuId",
                table: "UserMenus",
                newName: "IX_UserMenus_MenuId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserMenus",
                table: "UserMenus",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMenus_Menus_MenuId",
                table: "UserMenus",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMenus_Users_UserId",
                table: "UserMenus",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMenus_Menus_MenuId",
                table: "UserMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMenus_Users_UserId",
                table: "UserMenus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserMenus",
                table: "UserMenus");

            migrationBuilder.RenameTable(
                name: "UserMenus",
                newName: "UserMenu");

            migrationBuilder.RenameIndex(
                name: "IX_UserMenus_UserId",
                table: "UserMenu",
                newName: "IX_UserMenu_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserMenus_MenuId",
                table: "UserMenu",
                newName: "IX_UserMenu_MenuId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserMenu",
                table: "UserMenu",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMenu_Menus_MenuId",
                table: "UserMenu",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMenu_Users_UserId",
                table: "UserMenu",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
