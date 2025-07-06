using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace phonev2.Models
{
    [Table("DichVu")]
    public class DichVu
    {
        [Key]
        [Column("madichvu")]
        [Display(Name = "Mã Dịch Vụ")]
        public int MaDichVu { get; set; }

        [Column("tendichvu")]
        [Display(Name = "Tên Dịch Vụ")]
        [Required(ErrorMessage = "Tên dịch vụ là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên dịch vụ không được vượt quá 100 ký tự")]
        public string TenDichVu { get; set; } = string.Empty;

        [Column("mota")]
        [Display(Name = "Mô Tả")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? MoTa { get; set; }

        [Column("giadichvu")]
        [Display(Name = "Giá Dịch Vụ (VNĐ)")]
        [Required(ErrorMessage = "Giá dịch vụ là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá dịch vụ phải lớn hơn 0")]
        [DataType(DataType.Currency)]
        public decimal GiaDichVu { get; set; }

        [Column("thoigiansua")]
        [Display(Name = "Thời Gian Sửa (phút)")]
        [Range(1, int.MaxValue, ErrorMessage = "Thời gian sửa phải lớn hơn 0")]
        public int? ThoiGianSua { get; set; }

        [Column("ngaytao")]
        [Display(Name = "Ngày Tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Column("trangthai")]
        [Display(Name = "Trạng Thái")]
        public bool TrangThai { get; set; } = true;

        // Display properties
        [Display(Name = "Trạng Thái")]
        public string TrangThaiText => TrangThai ? "Hoạt động" : "Ngừng hoạt động";
        
        [Display(Name = "Giá Dịch Vụ")]
        public string GiaDichVuText => GiaDichVu.ToString("N0") + " VNĐ";
    }
}