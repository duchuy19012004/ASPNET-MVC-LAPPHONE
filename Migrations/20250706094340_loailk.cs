using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace phonev2.Migrations
{
    /// <inheritdoc />
    public partial class loailk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoaiLinhKien",
                columns: table => new
                {
                    maloailinhkien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenloailinhkien = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    mota = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    thoigianbaohanh = table.Column<int>(type: "int", nullable: true),
                    ngaytao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    trangthai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiLinhKien", x => x.maloailinhkien);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoaiLinhKien_tenloailinhkien",
                table: "LoaiLinhKien",
                column: "tenloailinhkien");

            migrationBuilder.CreateIndex(
                name: "IX_LoaiLinhKien_thoigianbaohanh",
                table: "LoaiLinhKien",
                column: "thoigianbaohanh");

            migrationBuilder.CreateIndex(
                name: "IX_LoaiLinhKien_trangthai",
                table: "LoaiLinhKien",
                column: "trangthai");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoaiLinhKien");
        }
    }
}
