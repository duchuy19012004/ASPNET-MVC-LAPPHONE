using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace phonev2.Models
{
    [Table("ChiTietPhieuSua")]
    public class ChiTietPhieuSua
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("maphieusua")]
        [Display(Name = "Mã Phiếu Sửa")]
        public int MaPhieuSua { get; set; }

        [Column("madichvu")]
        [Display(Name = "Mã Dịch Vụ")]
        public int MaDichVu { get; set; }

        [Column("soluong")]
        [Display(Name = "Số Lượng")]
        public int SoLuong { get; set; } = 1;

        [Column("thanhtien")]
        [Display(Name = "Thành Tiền")]
        public decimal? ThanhTien { get; set; }

        // Navigation properties
        [ForeignKey("MaPhieuSua")]
        public virtual PhieuSua? PhieuSua { get; set; }

        [ForeignKey("MaDichVu")]
        public virtual DichVu? DichVu { get; set; }

        // Hiển thị tên khách hàng và nhân viên từ navigation property
        [NotMapped]
        public string TenKhachHang => PhieuSua?.KhachHang?.HoTen ?? string.Empty;
        [NotMapped]
        public string TenNhanVien => PhieuSua?.NhanVien?.HoTen ?? string.Empty;
    }
} 
 