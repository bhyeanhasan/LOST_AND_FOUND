﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOST_AND_FOUND.Migrations
{
    public partial class newfound : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PictureName",
                table: "LostItem",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureName",
                table: "LostItem");
        }
    }
}
