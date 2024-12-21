using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileStorage.API.Migrations
{
    public partial class removedServiceEntityAddedEnumAndPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MetaDatas_OriginServices_OriginServiceId",
                table: "MetaDatas");

            migrationBuilder.DropTable(
                name: "OriginServices");

            migrationBuilder.DropIndex(
                name: "IX_MetaDatas_OriginServiceId",
                table: "MetaDatas");

            migrationBuilder.RenameColumn(
                name: "OriginServiceId",
                table: "MetaDatas",
                newName: "Service");

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "MetaDatas",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "MetaDatas");

            migrationBuilder.RenameColumn(
                name: "Service",
                table: "MetaDatas",
                newName: "OriginServiceId");

            migrationBuilder.CreateTable(
                name: "OriginServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OriginServices", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MetaDatas_OriginServiceId",
                table: "MetaDatas",
                column: "OriginServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_MetaDatas_OriginServices_OriginServiceId",
                table: "MetaDatas",
                column: "OriginServiceId",
                principalTable: "OriginServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
