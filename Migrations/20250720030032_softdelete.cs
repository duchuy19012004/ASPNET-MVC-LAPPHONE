using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace phonev2.Migrations
{
    /// <inheritdoc />
    public partial class softdelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "daxoa",
                table: "LinhKien",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "lydoxoa",
                table: "LinhKien",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ngayxoa",
                table: "LinhKien",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "daxoa",
                table: "LinhKien");

            migrationBuilder.DropColumn(
                name: "lydoxoa",
                table: "LinhKien");

            migrationBuilder.DropColumn(
                name: "ngayxoa",
                table: "LinhKien");
        }
    }
}
