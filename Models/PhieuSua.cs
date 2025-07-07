using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace phonev2.Models
{
    [Table("PhieuSua")]
    public class PhieuSua
    {
        [Key]
        [Column("maphieusua")]
        public int MaPhieuSua { get; set; }

        [Required]
        [Column("makhachhang")]
        public int MaKhachHang { get; set; }

        [Required]
        [Column("manhanvien")]
        public int MaNhanVien { get; set; }

        [Required]
        [Column("mathietbi")]
        public int MaThietBi { get; set; }

        [Required]
        [Column("ngaynhan")]
        public DateTime NgayNhan { get; set; }

        [Column("ngayhentra")]
        public DateTime? NgayHenTra { get; set; }

        [Column("ngaytrathucte")]
        public DateTime? NgayTraThucTe { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("tinhtrangnhan")]
        public string TinhTrangNhan { get; set; } = string.Empty;

        [MaxLength(500)]
        [Column("motaloi")]
        public string? MoTaLoi { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("trangthai")]
        public string TrangThai { get; set; } = "Tiếp nhận";

        [Column("tongtien", TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        [MaxLength(500)]
        [Column("ghichu")]
        public string? GhiChu { get; set; }

        // Navigation Properties
        [ForeignKey("MaKhachHang")]
        public virtual KhachHang? KhachHang { get; set; }

        [ForeignKey("MaNhanVien")]
        public virtual NhanVien? NhanVien { get; set; }

        [ForeignKey("MaThietBi")]
        public virtual ThietBi? ThietBi { get; set; }

        // Collections
        public virtual ICollection<ChiTiet_SuaChua> ChiTiet_SuaChuas { get; set; } = new List<ChiTiet_SuaChua>();
        public virtual ICollection<ChiTietLK> ChiTietLKs { get; set; } = new List<ChiTietLK>();
    }
}