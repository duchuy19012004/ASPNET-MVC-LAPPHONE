using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace phonev2.Migrations
{
    /// <inheritdoc />
    public partial class khachhang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    makhachhang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    hoten = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    sodienthoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    diachi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ngaytao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    trangthai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    tongchitieu = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHang", x => x.makhachhang);
                    table.CheckConstraint("CK_KhachHang_NgayTao", "NgayTao <= GETDATE()");
                    table.CheckConstraint("CK_KhachHang_TongChiTieu", "TongChiTieu >= 0");
                });

            migrationBuilder.CreateIndex(
                name: "IX_KhachHang_hoten",
                table: "KhachHang",
                column: "hoten");

            migrationBuilder.CreateIndex(
                name: "IX_KhachHang_ngaytao",
                table: "KhachHang",
                column: "ngaytao");

            migrationBuilder.CreateIndex(
                name: "IX_KhachHang_sodienthoai",
                table: "KhachHang",
                column: "sodienthoai",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KhachHang_tongchitieu",
                table: "KhachHang",
                column: "tongchitieu");

            migrationBuilder.CreateIndex(
                name: "IX_KhachHang_trangthai",
                table: "KhachHang",
                column: "trangthai");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KhachHang");
        }
    }
}
