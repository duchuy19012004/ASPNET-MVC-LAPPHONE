using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace phonev2.Migrations
{
    /// <inheritdoc />
    public partial class phieunhap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhieuNhap",
                columns: table => new
                {
                    maphieunhap = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    manhacungcap = table.Column<int>(type: "int", nullable: false),
                    manhanvien = table.Column<int>(type: "int", nullable: false),
                    ngaynhap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    tongtien = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    ghichu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    trangthai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Nháp"),
                    ngaytao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    ngaycapnhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuNhap", x => x.maphieunhap);
                    table.CheckConstraint("CK_PhieuNhap_NgayNhap", "NgayNhap <= GETDATE()");
                    table.CheckConstraint("CK_PhieuNhap_TongTien", "TongTien >= 0");
                    table.CheckConstraint("CK_PhieuNhap_TrangThai", "TrangThai IN ('Nháp', 'Đã xác nhận')");
                    table.ForeignKey(
                        name: "FK_PhieuNhap_NhaCungCap_manhacungcap",
                        column: x => x.manhacungcap,
                        principalTable: "NhaCungCap",
                        principalColumn: "manhacungcap",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuNhap_NhanVien_manhanvien",
                        column: x => x.manhanvien,
                        principalTable: "NhanVien",
                        principalColumn: "manhanvien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietPhieuNhap",
                columns: table => new
                {
                    maphieunhap = table.Column<int>(type: "int", nullable: false),
                    malinhkien = table.Column<int>(type: "int", nullable: false),
                    soluong = table.Column<int>(type: "int", nullable: false),
                    gianhap = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    thanhtien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ghichu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietPhieuNhap", x => new { x.maphieunhap, x.malinhkien });
                    table.CheckConstraint("CK_ChiTietPhieuNhap_GiaNhap", "GiaNhap >= 0");
                    table.CheckConstraint("CK_ChiTietPhieuNhap_SoLuong", "SoLuong > 0");
                    table.CheckConstraint("CK_ChiTietPhieuNhap_ThanhTien", "ThanhTien >= 0");
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuNhap_LinhKien_malinhkien",
                        column: x => x.malinhkien,
                        principalTable: "LinhKien",
                        principalColumn: "malinhkien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuNhap_PhieuNhap_maphieunhap",
                        column: x => x.maphieunhap,
                        principalTable: "PhieuNhap",
                        principalColumn: "maphieunhap",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuNhap_malinhkien",
                table: "ChiTietPhieuNhap",
                column: "malinhkien");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuNhap_maphieunhap",
                table: "ChiTietPhieuNhap",
                column: "maphieunhap");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhap_manhacungcap",
                table: "PhieuNhap",
                column: "manhacungcap");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhap_manhanvien",
                table: "PhieuNhap",
                column: "manhanvien");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhap_ngaynhap",
                table: "PhieuNhap",
                column: "ngaynhap");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhap_ngaytao",
                table: "PhieuNhap",
                column: "ngaytao");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhap_trangthai",
                table: "PhieuNhap",
                column: "trangthai");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietPhieuNhap");

            migrationBuilder.DropTable(
                name: "PhieuNhap");
        }
    }
}
