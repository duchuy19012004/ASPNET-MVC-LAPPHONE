using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace phonev2.Models
{
    [Table("LoaiLinhKien")]
    public class LoaiLinhKien
    {
        [Key]
        [Column("maloailinhkien")]
        [Display(Name = "Mã Loại Linh Kiện")]
        public int MaLoaiLinhKien { get; set; }

        [Column("tenloailinhkien")]
        [Display(Name = "Tên Loại Linh Kiện")]
        [Required(ErrorMessage = "Tên loại linh kiện là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên loại linh kiện không được vượt quá 100 ký tự")]
        public string TenLoaiLinhKien { get; set; } = string.Empty;

        [Column("mota")]
        [Display(Name = "Mô Tả")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? MoTa { get; set; }

        [Column("thoigianbaohanh")]
        [Display(Name = "Thời Gian Bảo Hành (tháng)")]
        [Range(0, 120, ErrorMessage = "Thời gian bảo hành từ 0-120 tháng")]
        public int? ThoiGianBaoHanh { get; set; }

        [Column("ngaytao")]
        [Display(Name = "Ngày Tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Column("trangthai")]
        [Display(Name = "Trạng Thái")]
        public bool TrangThai { get; set; } = true;

        // Display properties
        [Display(Name = "Trạng Thái")]
        public string TrangThaiText => TrangThai ? "Hoạt động" : "Ngừng hoạt động";
        
        [Display(Name = "Bảo Hành")]
        public string ThoiGianBaoHanhText => 
            ThoiGianBaoHanh.HasValue && ThoiGianBaoHanh > 0 
                ? $"{ThoiGianBaoHanh} tháng" 
                : "Không bảo hành";

        // Navigation Properties (sẽ được thêm sau khi có bảng LinhKien)
        // public virtual ICollection<LinhKien>? LinhKiens { get; set; }
    }
}