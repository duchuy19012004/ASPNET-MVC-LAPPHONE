using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace phonev2.Migrations
{
    /// <inheritdoc />
    public partial class nhanvien : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NhanVien",
                columns: table => new
                {
                    manhanvien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    hoten = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    sodienthoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    diachi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ngaysinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ngayvaolam = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ngaynghiviec = table.Column<DateTime>(type: "datetime2", nullable: true),
                    chucvu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    luong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    trangthai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVien", x => x.manhanvien);
                    table.CheckConstraint("CK_NhanVien_Luong", "Luong >= 0");
                    table.CheckConstraint("CK_NhanVien_NgayNghiViec", "NgayNghiViec IS NULL OR NgayNghiViec >= NgayVaoLam");
                    table.CheckConstraint("CK_NhanVien_NgaySinh", "NgaySinh <= GETDATE()");
                    table.CheckConstraint("CK_NhanVien_NgayVaoLam", "NgayVaoLam >= NgaySinh");
                });

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_chucvu",
                table: "NhanVien",
                column: "chucvu");

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_email",
                table: "NhanVien",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_hoten",
                table: "NhanVien",
                column: "hoten");

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_ngaynghiviec",
                table: "NhanVien",
                column: "ngaynghiviec");

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_ngayvaolam",
                table: "NhanVien",
                column: "ngayvaolam");

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_sodienthoai",
                table: "NhanVien",
                column: "sodienthoai");

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_trangthai",
                table: "NhanVien",
                column: "trangthai");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NhanVien");
        }
    }
}
