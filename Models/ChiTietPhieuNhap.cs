using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace phonev2.Models
{
    [Table("ChiTietPhieuNhap")]
    public class ChiTietPhieuNhap
    {
        [Key]
        [Column("maphieunhap")]
        public int MaPhieuNhap { get; set; }

        [Key]
        [Column("malinhkien")]
        public int MaLinhKien { get; set; }

        [Required]
        [Column("soluong")]
        public int SoLuong { get; set; }

        [Required]
        [Column("gianhap", TypeName = "decimal(18,2)")]
        public decimal GiaNhap { get; set; }

        [Required]
        [Column("thanhtien", TypeName = "decimal(18,2)")]
        public decimal ThanhTien { get; set; }

        [MaxLength(200)]
        [Column("ghichu")]
        public string? GhiChu { get; set; }

        // Navigation Properties
        [ForeignKey("MaPhieuNhap")]
        public virtual PhieuNhap? PhieuNhap { get; set; }

        [ForeignKey("MaLinhKien")]
        public virtual LinhKien? LinhKien { get; set; }
    }
}