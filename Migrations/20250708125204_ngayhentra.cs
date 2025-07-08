using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace phonev2.Migrations
{
    /// <inheritdoc />
    public partial class ngayhentra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ngayhentra",
                table: "PhieuSua",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ngaytrathucte",
                table: "PhieuSua",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhieuSua_ngayhentra",
                table: "PhieuSua",
                column: "ngayhentra");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuSua_ngaytrathucte",
                table: "PhieuSua",
                column: "ngaytrathucte");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PhieuSua_ngayhentra",
                table: "PhieuSua");

            migrationBuilder.DropIndex(
                name: "IX_PhieuSua_ngaytrathucte",
                table: "PhieuSua");

            migrationBuilder.DropColumn(
                name: "ngayhentra",
                table: "PhieuSua");

            migrationBuilder.DropColumn(
                name: "ngaytrathucte",
                table: "PhieuSua");
        }
    }
}
