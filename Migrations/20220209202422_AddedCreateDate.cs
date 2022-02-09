using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControllerApi.Migrations
{
    public partial class AddedCreateDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "createDate",
                table: "AspNetUsers",
                type: "datetime(6)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "createDate",
                table: "AspNetUsers");
        }
    }
}
