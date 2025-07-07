using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace phonev2.Migrations
{
    /// <inheritdoc />
    public partial class xoaphieusua : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTiet_SuaChua");

            migrationBuilder.DropTable(
                name: "PhieuSua");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhieuSua",
                columns: table => new
                {
                    maphieusua = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    makhachhang = table.Column<int>(type: "int", nullable: false),
                    manhanvien = table.Column<int>(type: "int", nullable: false),
                    mathietbi = table.Column<int>(type: "int", nullable: false),
                    motaloi = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ngayhentra = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ngaynhan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ngaytrathucte = table.Column<DateTime>(type: "datetime2", nullable: true),
                    tinhtrangnhan = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    tongtien = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    trangthai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Chờ xử lý")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuSua", x => x.maphieusua);
                    table.CheckConstraint("CK_PhieuSua_NgayHenTra", "NgayHenTra IS NULL OR NgayHenTra >= NgayNhan");
                    table.CheckConstraint("CK_PhieuSua_NgayTraThucTe", "NgayTraThucTe IS NULL OR NgayTraThucTe >= NgayNhan");
                    table.CheckConstraint("CK_PhieuSua_TongTien", "TongTien >= 0");
                    table.ForeignKey(
                        name: "FK_PhieuSua_KhachHang_makhachhang",
                        column: x => x.makhachhang,
                        principalTable: "KhachHang",
                        principalColumn: "makhachhang",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuSua_NhanVien_manhanvien",
                        column: x => x.manhanvien,
                        principalTable: "NhanVien",
                        principalColumn: "manhanvien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuSua_ThietBi_mathietbi",
                        column: x => x.mathietbi,
                        principalTable: "ThietBi",
                        principalColumn: "mathietbi",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTiet_SuaChua",
                columns: table => new
                {
                    maphieusua = table.Column<int>(type: "int", nullable: false),
                    madichvu = table.Column<int>(type: "int", nullable: false),
                    ghichu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    giadichvu = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTiet_SuaChua", x => new { x.maphieusua, x.madichvu });
                    table.ForeignKey(
                        name: "FK_ChiTiet_SuaChua_DichVu_madichvu",
                        column: x => x.madichvu,
                        principalTable: "DichVu",
                        principalColumn: "madichvu",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTiet_SuaChua_PhieuSua_maphieusua",
                        column: x => x.maphieusua,
                        principalTable: "PhieuSua",
                        principalColumn: "maphieusua",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTiet_SuaChua_madichvu",
                table: "ChiTiet_SuaChua",
                column: "madichvu");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTiet_SuaChua_maphieusua",
                table: "ChiTiet_SuaChua",
                column: "maphieusua");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuSua_makhachhang",
                table: "PhieuSua",
                column: "makhachhang");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuSua_manhanvien",
                table: "PhieuSua",
                column: "manhanvien");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuSua_mathietbi",
                table: "PhieuSua",
                column: "mathietbi");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuSua_ngayhentra",
                table: "PhieuSua",
                column: "ngayhentra");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuSua_ngaynhan",
                table: "PhieuSua",
                column: "ngaynhan");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuSua_ngaytrathucte",
                table: "PhieuSua",
                column: "ngaytrathucte");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuSua_tongtien",
                table: "PhieuSua",
                column: "tongtien");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuSua_trangthai",
                table: "PhieuSua",
                column: "trangthai");
        }
    }
}
