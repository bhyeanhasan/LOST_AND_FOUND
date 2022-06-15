using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOST_AND_FOUND.Data.Migrations
{
    public partial class recent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Posted_by",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Posted_by",
                table: "User");
        }
    }
}
