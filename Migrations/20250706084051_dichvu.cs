using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace phonev2.Migrations
{
    /// <inheritdoc />
    public partial class dichvu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DichVu",
                columns: table => new
                {
                    madichvu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tendichvu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    mota = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    giadichvu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    thoigiansua = table.Column<int>(type: "int", nullable: true),
                    ngaytao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    trangthai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DichVu", x => x.madichvu);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DichVu_tendichvu",
                table: "DichVu",
                column: "tendichvu");

            migrationBuilder.CreateIndex(
                name: "IX_DichVu_trangthai",
                table: "DichVu",
                column: "trangthai");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DichVu");
        }
    }
}
