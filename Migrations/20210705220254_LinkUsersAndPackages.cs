using Microsoft.EntityFrameworkCore.Migrations;

namespace DMPackageManager.Website.Migrations
{
    public partial class LinkUsersAndPackages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "owner",
                table: "packages");

            migrationBuilder.AddColumn<long>(
                name: "owneruserId",
                table: "packages",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_packages_owneruserId",
                table: "packages",
                column: "owneruserId");

            migrationBuilder.AddForeignKey(
                name: "FK_packages_users_owneruserId",
                table: "packages",
                column: "owneruserId",
                principalTable: "users",
                principalColumn: "userId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_packages_users_owneruserId",
                table: "packages");

            migrationBuilder.DropIndex(
                name: "IX_packages_owneruserId",
                table: "packages");

            migrationBuilder.DropColumn(
                name: "owneruserId",
                table: "packages");

            migrationBuilder.AddColumn<int>(
                name: "owner",
                table: "packages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
