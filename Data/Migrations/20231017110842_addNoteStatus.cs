using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineNotes.Data.Migrations
{
    public partial class addNoteStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Note",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Note");
        }
    }
}
