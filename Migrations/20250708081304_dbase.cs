using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace phonev2.Migrations
{
    /// <inheritdoc />
    public partial class dbase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhieuSua",
                columns: table => new
                {
                    maphieusua = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ngaysua = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    makhachhang = table.Column<int>(type: "int", nullable: true),
                    manhanvien = table.Column<int>(type: "int", nullable: true),
                    tongtien = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    ghichu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    trangthai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuSua", x => x.maphieusua);
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
                });

            migrationBuilder.CreateTable(
                name: "ChiTietLinhKien",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    maphieusua = table.Column<int>(type: "int", nullable: false),
                    malinhkien = table.Column<int>(type: "int", nullable: false),
                    soluong = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    thanhtien = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietLinhKien", x => x.id);
                    table.ForeignKey(
                        name: "FK_ChiTietLinhKien_LinhKien_malinhkien",
                        column: x => x.malinhkien,
                        principalTable: "LinhKien",
                        principalColumn: "malinhkien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietLinhKien_PhieuSua_maphieusua",
                        column: x => x.maphieusua,
                        principalTable: "PhieuSua",
                        principalColumn: "maphieusua",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietPhieuSua",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    maphieusua = table.Column<int>(type: "int", nullable: false),
                    madichvu = table.Column<int>(type: "int", nullable: false),
                    soluong = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    thanhtien = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietPhieuSua", x => x.id);
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuSua_DichVu_madichvu",
                        column: x => x.madichvu,
                        principalTable: "DichVu",
                        principalColumn: "madichvu",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuSua_PhieuSua_maphieusua",
                        column: x => x.maphieusua,
                        principalTable: "PhieuSua",
                        principalColumn: "maphieusua",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietLinhKien_malinhkien",
                table: "ChiTietLinhKien",
                column: "malinhkien");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietLinhKien_maphieusua",
                table: "ChiTietLinhKien",
                column: "maphieusua");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuSua_madichvu",
                table: "ChiTietPhieuSua",
                column: "madichvu");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuSua_maphieusua",
                table: "ChiTietPhieuSua",
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
                name: "IX_PhieuSua_ngaysua",
                table: "PhieuSua",
                column: "ngaysua");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuSua_trangthai",
                table: "PhieuSua",
                column: "trangthai");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietLinhKien");

            migrationBuilder.DropTable(
                name: "ChiTietPhieuSua");

            migrationBuilder.DropTable(
                name: "PhieuSua");
        }
    }
}
