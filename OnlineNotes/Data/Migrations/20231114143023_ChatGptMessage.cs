using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineNotes.Data.Migrations
{
    public partial class ChatGptMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
            name: "ChatMessages",
            columns: table => new
        {
        Id = table.Column<int>(type: "int", nullable: false)
            .Annotation("SqlServer:Identity", "1, 1"),
        Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
        IsUser = table.Column<bool>(type: "bit", nullable: false),
        Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_ChatMessages", x => x.Id);
        });
            }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessages");
        }
    }
}
