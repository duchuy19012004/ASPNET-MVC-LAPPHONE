using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace phonev2.Models
{
    public enum TrangThaiPhieuSua
    {
        [Display(Name = "Tiếp nhận")]
        TiepNhan = 0,
        [Display(Name = "Đang sửa")]
        DangSua = 1,
        [Display(Name = "Hoàn thành")]
        HoanThanh = 2,
        [Display(Name = "Hủy")]
        Huy = 3
    }

    [Table("PhieuSua")]
    public class PhieuSua
    {
        [Key]
        [Column("maphieusua")]
        [Display(Name = "Mã Phiếu Sửa")]
        public int MaPhieuSua { get; set; }

        [Column("ngaysua")]
        [Display(Name = "Ngày Sửa")]
        public DateTime NgaySua { get; set; } = DateTime.Now;

        [Column("makhachhang")]
        [Display(Name = "Khách Hàng")]
        [Required(ErrorMessage = "Vui lòng chọn khách hàng")]
        public int? MaKhachHang { get; set; }

        [Column("manhanvien")]
        [Display(Name = "Nhân Viên")]
        [Required(ErrorMessage = "Vui lòng chọn nhân viên")]
        public int? MaNhanVien { get; set; }

        [Column("tongtien")]
        [Display(Name = "Tổng Tiền")]
        public decimal? TongTien { get; set; }

        [Column("ghichu")]
        [Display(Name = "Ghi Chú")]
        [StringLength(500)]
        public string? GhiChu { get; set; }

        [Column("ngayhentra")]
        [Display(Name = "Ngày Hẹn Trả")]
        public DateTime? NgayHenTra { get; set; }

        [Column("ngaytrathucte")]
        [Display(Name = "Ngày Trả Thực Tế")]
        public DateTime? NgayTraThucTe { get; set; }

        [Column("trangthai")]
        [Display(Name = "Trạng Thái")]
        public TrangThaiPhieuSua TrangThai { get; set; } = TrangThaiPhieuSua.TiepNhan;

        [Column("giokhoanthanhtacvu")]
        [Display(Name = "Giờ Hoàn Thành")]
        public TimeSpan? GioHoanThanh { get; set; }

        // Nếu muốn lưu cả ngày và giờ hoàn thành:
        [Column("ngaygiohoanthanhtacvu")]
        [Display(Name = "Ngày Giờ Hoàn Thành")]
        public DateTime? NgayGioHoanThanh { get; set; }

        // Navigation properties
        public virtual ICollection<ChiTietPhieuSua>? ChiTietPhieuSuas { get; set; }
        public virtual ICollection<ChiTietLinhKien>? ChiTietLinhKiens { get; set; }
        [ForeignKey("MaKhachHang")]
        public virtual KhachHang? KhachHang { get; set; }
        [ForeignKey("MaNhanVien")]
        public virtual NhanVien? NhanVien { get; set; }

        // Hiển thị tên khách hàng và nhân viên từ navigation property
        [NotMapped]
        public string TenKhachHang => KhachHang?.HoTen ?? string.Empty;
        [NotMapped]
        public string TenNhanVien => NhanVien?.HoTen ?? string.Empty;
    }
} 