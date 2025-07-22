using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace phonev2.Migrations
{
    /// <inheritdoc />
    public partial class dbcontext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhieuSua_KhachHang_KhachHangMaKhachHang",
                table: "PhieuSua");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuSua_NhanVien_NhanVienMaNhanVien",
                table: "PhieuSua");

            migrationBuilder.DropIndex(
                name: "IX_PhieuSua_KhachHangMaKhachHang",
                table: "PhieuSua");

            migrationBuilder.DropIndex(
                name: "IX_PhieuSua_NhanVienMaNhanVien",
                table: "PhieuSua");

            migrationBuilder.DropColumn(
                name: "KhachHangMaKhachHang",
                table: "PhieuSua");

            migrationBuilder.DropColumn(
                name: "NhanVienMaNhanVien",
                table: "PhieuSua");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KhachHangMaKhachHang",
                table: "PhieuSua",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NhanVienMaNhanVien",
                table: "PhieuSua",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhieuSua_KhachHangMaKhachHang",
                table: "PhieuSua",
                column: "KhachHangMaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuSua_NhanVienMaNhanVien",
                table: "PhieuSua",
                column: "NhanVienMaNhanVien");

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuSua_KhachHang_KhachHangMaKhachHang",
                table: "PhieuSua",
                column: "KhachHangMaKhachHang",
                principalTable: "KhachHang",
                principalColumn: "makhachhang");

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuSua_NhanVien_NhanVienMaNhanVien",
                table: "PhieuSua",
                column: "NhanVienMaNhanVien",
                principalTable: "NhanVien",
                principalColumn: "manhanvien");
        }
    }
}
