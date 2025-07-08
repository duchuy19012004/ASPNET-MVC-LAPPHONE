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
        public int MaPhieuSua { get; set; }

        [Column("malinhkien")]
        public int MaLinhKien { get; set; }

        [Column("soluong")]
        public int SoLuong { get; set; } = 1;

        [Column("thanhtien")]
        public decimal? ThanhTien { get; set; }

        // Navigation properties
        [ForeignKey("MaPhieuSua")]
        public virtual PhieuSua? PhieuSua { get; set; }

        [ForeignKey("MaLinhKien")]
        public virtual LinhKien? LinhKien { get; set; }
    }
} 