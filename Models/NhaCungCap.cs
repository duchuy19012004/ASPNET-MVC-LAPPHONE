using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace phonev2.Models
{
    [Table("NhaCungCap")]
    public class NhaCungCap
    {
        [Key]
        [Column("manhacungcap")]
        [Display(Name = "Mã Nhà Cung Cấp")]
        public int MaNhaCungCap { get; set; }

        [Column("tennhacungcap")]
        [Display(Name = "Tên Nhà Cung Cấp")]
        [Required(ErrorMessage = "Tên nhà cung cấp là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên nhà cung cấp không được vượt quá 100 ký tự")]
        public string TenNhaCungCap { get; set; } = string.Empty;

        [Column("diachi")]
        [Display(Name = "Địa Chỉ")]
        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
        public string DiaChi { get; set; } = string.Empty;

        [Column("sodienthoai")]
        [Display(Name = "Số Điện Thoại")]
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng Việt Nam")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Column("email")]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string Email { get; set; } = string.Empty;

        [Column("ngaytao")]
        [Display(Name = "Ngày Tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Column("trangthai")]
        [Display(Name = "Trạng Thái")]
        public bool TrangThai { get; set; } = true;

        // Display Properties
        [Display(Name = "Trạng Thái")]
        public string TrangThaiText => TrangThai ? "Đang hợp tác" : "Ngừng hợp tác";

        [Display(Name = "Thông Tin Liên Hệ")]
        public string ThongTinLienHe => $"{SoDienThoai} - {Email}";

        [Display(Name = "Địa Chỉ Ngắn")]
        public string DiaChiNgan => DiaChi.Length > 50 ? DiaChi.Substring(0, 50) + "..." : DiaChi;

        // Navigation Properties (sẽ được thêm sau khi có PhieuNhap)
        // public virtual ICollection<PhieuNhap>? PhieuNhaps { get; set; }

        // Business Logic Methods
        public bool IsActiveSupplier()
        {
            return TrangThai;
        }

        public string GetContactSummary()
        {
            return $"{TenNhaCungCap} - {SoDienThoai}";
        }

        public bool IsValidContact()
        {
            return !string.IsNullOrEmpty(SoDienThoai) && !string.IsNullOrEmpty(Email);
        }
    }
}