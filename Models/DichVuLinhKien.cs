using System.ComponentModel.DataAnnotations; 
using System.ComponentModel.DataAnnotations.Schema;

namespace phonev2.Models
{
    [Table("DichVuLinhKien")]
    public class DichVuLinhKien
    {
        [Key, Column(Order = 0)]
        public int MaDichVu { get; set; }
        [Key, Column(Order = 1)]
        public int MaLinhKien { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int SoLuong { get; set; } = 1;
        [ForeignKey("MaDichVu")]
        public virtual DichVu? DichVu { get; set; }
        [ForeignKey("MaLinhKien")]
        public virtual LinhKien? LinhKien { get; set; }
    }
} 