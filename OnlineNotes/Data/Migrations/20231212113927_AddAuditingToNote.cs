using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineNotes.Data.Migrations
{
    public partial class AddAuditingToNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "Note",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "Note");
        }
    }
}
