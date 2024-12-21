using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityProvider.Infrastructure.Migrations
{
    public partial class addedCoreIdToEnjoyLifeUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CoreId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoreId",
                table: "AspNetUsers");
        }
    }
}
