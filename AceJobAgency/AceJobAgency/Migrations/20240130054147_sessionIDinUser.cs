using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AceJobAgency.Migrations
{
    public partial class sessionIDinUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "AspNetUsers");
        }
    }
}
