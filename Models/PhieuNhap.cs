using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace phonev2.Models
{
    [Table("PhieuNhap")]
    public class PhieuNhap
    {
        [Key]
        [Column("maphieunhap")]
        public int MaPhieuNhap { get; set; }

        [Required]
        [Column("manhacungcap")]
        public int MaNhaCungCap { get; set; }

        [Required]
        [Column("manhanvien")]
        public int MaNhanVien { get; set; }

        [Required]
        [Column("ngaynhap")]
        public DateTime NgayNhap { get; set; }

        [Column("tongtien", TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        [MaxLength(500)]
        [Column("ghichu")]
        public string? GhiChu { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("trangthai")]
        public string TrangThai { get; set; } = "Nháp"; // Nháp, Đã xác nhận

        [Column("ngaytao")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Column("ngaycapnhat")]
        public DateTime? NgayCapNhat { get; set; }

        // Navigation Properties
        [ForeignKey("MaNhaCungCap")]
        public virtual NhaCungCap? NhaCungCap { get; set; }

        [ForeignKey("MaNhanVien")]
        public virtual NhanVien? NhanVien { get; set; }

        public virtual ICollection<ChiTietPhieuNhap>? ChiTietPhieuNhaps { get; set; }
    }
}