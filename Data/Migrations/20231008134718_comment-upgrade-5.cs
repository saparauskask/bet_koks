using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineNotes.Data.Migrations
{
    public partial class commentupgrade5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Note_NoteId",
                table: "Comment");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Note_NoteId",
                table: "Comment",
                column: "NoteId",
                principalTable: "Note",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Note_NoteId",
                table: "Comment");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Note_NoteId",
                table: "Comment",
                column: "NoteId",
                principalTable: "Note",
                principalColumn: "Id");
        }
    }
}
