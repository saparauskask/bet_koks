using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineNotes.Data.Migrations
{
    public partial class UpdateNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Note",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Note");
        }
    }
}
