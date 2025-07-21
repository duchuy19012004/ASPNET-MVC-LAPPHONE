using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace phonev2.Migrations
{
    /// <inheritdoc />
    public partial class dichvusortdelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "daxoa",
                table: "DichVu",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "lydoxoa",
                table: "DichVu",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ngayxoa",
                table: "DichVu",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "daxoa",
                table: "DichVu");

            migrationBuilder.DropColumn(
                name: "lydoxoa",
                table: "DichVu");

            migrationBuilder.DropColumn(
                name: "ngayxoa",
                table: "DichVu");
        }
    }
}
