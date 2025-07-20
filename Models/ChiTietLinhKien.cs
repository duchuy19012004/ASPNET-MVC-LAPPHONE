using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace phonev2.Models
{
    [Table("ChiTietLinhKien")]
    public class ChiTietLinhKien
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("maphieusua")]
        [Display(Name = "Mã Phiếu Sửa")]
        public int MaPhieuSua { get; set; }

        [Column("malinhkien")]
        [Display(Name = "Mã Linh Kiện")]
        public int MaLinhKien { get; set; }

        [Column("soluong")]
        [Display(Name = "Số Lượng")]
        public int SoLuong { get; set; } = 1;

        [Column("thanhtien")]
        [Display(Name = "Thành Tiền")]
        public decimal? ThanhTien { get; set; }

        // Navigation properties
        [ForeignKey("MaPhieuSua")]
        public virtual PhieuSua? PhieuSua { get; set; }

        [ForeignKey("MaLinhKien")]
        public virtual LinhKien? LinhKien { get; set; }

        // Hiển thị tên khách hàng và nhân viên từ navigation property
        [NotMapped]
        public string TenKhachHang => PhieuSua?.KhachHang?.HoTen ?? string.Empty;
        [NotMapped]
        public string TenNhanVien => PhieuSua?.NhanVien?.HoTen ?? string.Empty;
    }
} 