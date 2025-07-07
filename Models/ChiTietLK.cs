using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace phonev2.Models
{
    [Table("ChiTietLK")]
    public class ChiTietLK
    {
        [Required]
        [Column("maphieusua")]
        public int MaPhieuSua { get; set; }

        [Required]
        [Column("malinhkien")]
        public int MaLinhKien { get; set; }

        [Required]
        [Column("soluong")]
        public int SoLuong { get; set; }

        [Required]
        [Column("giaban", TypeName = "decimal(18,2)")]
        public decimal GiaBan { get; set; }

        [Required]
        [Column("thanhtien", TypeName = "decimal(18,2)")]
        public decimal ThanhTien { get; set; }

        [MaxLength(200)]
        [Column("ghichu")]
        public string? GhiChu { get; set; }

        // Navigation Properties
        [ForeignKey("MaPhieuSua")]
        public virtual PhieuSua? PhieuSua { get; set; }

        [ForeignKey("MaLinhKien")]
        public virtual LinhKien? LinhKien { get; set; }
    }
}