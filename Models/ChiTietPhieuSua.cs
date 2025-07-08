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
        public int MaPhieuSua { get; set; }

        [Column("madichvu")]
        public int MaDichVu { get; set; }

        [Column("soluong")]
        public int SoLuong { get; set; } = 1;

        [Column("thanhtien")]
        public decimal? ThanhTien { get; set; }

        // Navigation properties
        [ForeignKey("MaPhieuSua")]
        public virtual PhieuSua? PhieuSua { get; set; }

        [ForeignKey("MaDichVu")]
        public virtual DichVu? DichVu { get; set; }
    }
} 
 