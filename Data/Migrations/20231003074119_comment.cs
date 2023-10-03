using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineNotes.Data.Migrations
{
    public partial class comment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoteId = table.Column<int>(type: "int", nullable: false),
                    Contents = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_Note_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Note",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_NoteId",
                table: "Comment",
                column: "NoteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment");
        }
    }
}
