using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileStorage.API.Migrations
{
    public partial class changedModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "MetaDatas");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "MetaDatas");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "MetaDatas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "MetaDatas",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
