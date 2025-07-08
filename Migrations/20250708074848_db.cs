using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace phonev2.Migrations
{
    /// <inheritdoc />
    public partial class db : Migration
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

            migrationBuilder.CreateTable(
                name: "NhaCungCap",
                columns: table => new
                {
                    manhacungcap = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tennhacungcap = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    diachi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    sodienthoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ngaytao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    trangthai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhaCungCap", x => x.manhacungcap);
                });

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

            migrationBuilder.CreateTable(
                name: "ThietBi",
                columns: table => new
                {
                    mathietbi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    makhachhang = table.Column<int>(type: "int", nullable: false),
                    tenthietbi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    loaithietbi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    hangsanxuat = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThietBi", x => x.mathietbi);
                    table.ForeignKey(
                        name: "FK_ThietBi_KhachHang_makhachhang",
                        column: x => x.makhachhang,
                        principalTable: "KhachHang",
                        principalColumn: "makhachhang",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LinhKien",
                columns: table => new
                {
                    malinhkien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenlinhkien = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    maloailinhkien = table.Column<int>(type: "int", nullable: false),
                    hangsanxuat = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    gianhap = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    giaban = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    soluongton = table.Column<int>(type: "int", nullable: false),
                    thongsoktyhuat = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ngaytao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    trangthai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinhKien", x => x.malinhkien);
                    table.ForeignKey(
                        name: "FK_LinhKien_LoaiLinhKien_maloailinhkien",
                        column: x => x.maloailinhkien,
                        principalTable: "LoaiLinhKien",
                        principalColumn: "maloailinhkien",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "IX_DichVu_tendichvu",
                table: "DichVu",
                column: "tendichvu");

            migrationBuilder.CreateIndex(
                name: "IX_DichVu_trangthai",
                table: "DichVu",
                column: "trangthai");

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

            migrationBuilder.CreateIndex(
                name: "IX_LinhKien_giaban",
                table: "LinhKien",
                column: "giaban");

            migrationBuilder.CreateIndex(
                name: "IX_LinhKien_hangsanxuat",
                table: "LinhKien",
                column: "hangsanxuat");

            migrationBuilder.CreateIndex(
                name: "IX_LinhKien_maloailinhkien",
                table: "LinhKien",
                column: "maloailinhkien");

            migrationBuilder.CreateIndex(
                name: "IX_LinhKien_ngaytao",
                table: "LinhKien",
                column: "ngaytao");

            migrationBuilder.CreateIndex(
                name: "IX_LinhKien_soluongton",
                table: "LinhKien",
                column: "soluongton");

            migrationBuilder.CreateIndex(
                name: "IX_LinhKien_tenlinhkien",
                table: "LinhKien",
                column: "tenlinhkien");

            migrationBuilder.CreateIndex(
                name: "IX_LinhKien_trangthai",
                table: "LinhKien",
                column: "trangthai");

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

            migrationBuilder.CreateIndex(
                name: "IX_NhaCungCap_email",
                table: "NhaCungCap",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NhaCungCap_sodienthoai",
                table: "NhaCungCap",
                column: "sodienthoai");

            migrationBuilder.CreateIndex(
                name: "IX_NhaCungCap_tennhacungcap",
                table: "NhaCungCap",
                column: "tennhacungcap");

            migrationBuilder.CreateIndex(
                name: "IX_NhaCungCap_trangthai",
                table: "NhaCungCap",
                column: "trangthai");

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

            migrationBuilder.CreateIndex(
                name: "IX_ThietBi_hangsanxuat",
                table: "ThietBi",
                column: "hangsanxuat");

            migrationBuilder.CreateIndex(
                name: "IX_ThietBi_loaithietbi",
                table: "ThietBi",
                column: "loaithietbi");

            migrationBuilder.CreateIndex(
                name: "IX_ThietBi_makhachhang",
                table: "ThietBi",
                column: "makhachhang");

            migrationBuilder.CreateIndex(
                name: "IX_ThietBi_model",
                table: "ThietBi",
                column: "model");

            migrationBuilder.CreateIndex(
                name: "IX_ThietBi_tenthietbi",
                table: "ThietBi",
                column: "tenthietbi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietPhieuNhap");

            migrationBuilder.DropTable(
                name: "DichVu");

            migrationBuilder.DropTable(
                name: "ThietBi");

            migrationBuilder.DropTable(
                name: "LinhKien");

            migrationBuilder.DropTable(
                name: "PhieuNhap");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "LoaiLinhKien");

            migrationBuilder.DropTable(
                name: "NhaCungCap");

            migrationBuilder.DropTable(
                name: "NhanVien");
        }
    }
}
