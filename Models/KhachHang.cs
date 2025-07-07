using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace phonev2.Models
{
    [Table("KhachHang")]
    public class KhachHang
    {
        [Key]
        [Column("makhachhang")]
        [Display(Name = "Mã Khách Hàng")]
        public int MaKhachHang { get; set; }

        [Column("hoten")]
        [Display(Name = "Họ và Tên")]
        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự")]
        public string HoTen { get; set; } = string.Empty;

        [Column("sodienthoai")]
        [Display(Name = "Số Điện Thoại")]
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng Việt Nam")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Column("diachi")]
        [Display(Name = "Địa Chỉ")]
        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
        public string DiaChi { get; set; } = string.Empty;

        [Column("ngaytao")]
        [Display(Name = "Ngày Tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Column("trangthai")]
        [Display(Name = "Trạng Thái")]
        public bool TrangThai { get; set; } = true;

        [Column("tongchitieu")]
        [Display(Name = "Tổng Chi Tiêu")]
        [Range(0, double.MaxValue, ErrorMessage = "Tổng chi tiêu phải lớn hơn hoặc bằng 0")]
        [DataType(DataType.Currency)]
        public decimal TongChiTieu { get; set; } = 0;

        // Display Properties
        [Display(Name = "Trạng Thái")]
        public string TrangThaiText => TrangThai ? "Hoạt động" : "Tạm khóa";

        [Display(Name = "Tổng Chi Tiêu")]
        public string TongChiTieuText => TongChiTieu.ToString("N0") + " VNĐ";

        [Display(Name = "Ngày Tạo")]
        public string NgayTaoText => NgayTao.ToString("dd/MM/yyyy HH:mm");

        // Helper Methods
        public string GetCustomerLevel()
        {
            return TongChiTieu switch
            {
                >= 50000000 => "VIP",
                >= 20000000 => "Thân thiết",
                >= 10000000 => "Bạc",
                >= 5000000 => "Đồng",
                _ => "Thường"
            };
        }

        public string GetCustomerLevelColor()
        {
            return TongChiTieu switch
            {
                >= 50000000 => "text-warning", // Gold
                >= 20000000 => "text-info",    // Cyan
                >= 10000000 => "text-secondary", // Silver
                >= 5000000 => "text-warning", // Bronze
                _ => "text-muted"             // Normal
            };
        }

        public bool IsVipCustomer() => TongChiTieu >= 20000000;

        public bool IsNewCustomer() => (DateTime.Now - NgayTao).TotalDays <= 30;

        public string GetMembershipDuration()
        {
            var duration = DateTime.Now - NgayTao;
            var years = (int)(duration.TotalDays / 365);
            var months = (int)((duration.TotalDays % 365) / 30);

            if (years > 0)
                return $"{years} năm {(months > 0 ? months + " tháng" : "")}".Trim();
            else if (months > 0)
                return $"{months} tháng";
            else
                return "Dưới 1 tháng";
        }

        public string GetAvatarLetter() => string.IsNullOrEmpty(HoTen) ? "?" : HoTen.Trim().Substring(0, 1).ToUpper();

        // Navigation Properties (sẽ được thêm sau)
        // public virtual ICollection<PhieuSua>? PhieuSuas { get; set; }
        // public virtual ICollection<HoaDon>? HoaDons { get; set; }
        // public virtual ICollection<ThietBi>? ThietBis { get; set; }
    }
}