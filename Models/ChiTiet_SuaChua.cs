using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace phonev2.Models
{
    [Table("ChiTiet_SuaChua")]
    public class ChiTiet_SuaChua
    {
        [Required]
        [Column("maphieusua")]
        public int MaPhieuSua { get; set; }

        [Required]
        [Column("madichvu")]
        public int MaDichVu { get; set; }

        [Required]
        [MaxLength(500)]
        [Column("ghichu")]
        public string GhiChu { get; set; } = string.Empty;

        [Column("giadichvu", TypeName = "decimal(18,2)")]
        public decimal GiaDichVu { get; set; }

        // Navigation Properties
        [ForeignKey("MaPhieuSua")]
        public virtual PhieuSua? PhieuSua { get; set; }

        [ForeignKey("MaDichVu")]
        public virtual DichVu? DichVu { get; set; }
    }
}