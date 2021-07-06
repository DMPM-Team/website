using Microsoft.EntityFrameworkCore.Migrations;

namespace DMPackageManager.Website.Migrations
{
    public partial class ExtraURLs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "documentation_url",
                table: "packages",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "source_url",
                table: "packages",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "documentation_url",
                table: "packages");

            migrationBuilder.DropColumn(
                name: "source_url",
                table: "packages");
        }
    }
}
