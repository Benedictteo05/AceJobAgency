using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AceJobAgency.Migrations
{
    public partial class AddOldPassword2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecondOldPassword",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecondOldPassword",
                table: "AspNetUsers");
        }
    }
}
