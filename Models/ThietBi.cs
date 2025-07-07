using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace phonev2.Models
{
    [Table("ThietBi")]
    public class ThietBi
    {
        [Key]
        [Column("mathietbi")]
        [Display(Name = "Mã Thiết Bị")]
        public int MaThietBi { get; set; }

        [Column("makhachhang")]
        [Display(Name = "Khách Hàng")]
        [Required(ErrorMessage = "Vui lòng chọn khách hàng")]
        public int MaKhachHang { get; set; }

        [Column("tenthietbi")]
        [Display(Name = "Tên Thiết Bị")]
        [Required(ErrorMessage = "Tên thiết bị là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên thiết bị không được vượt quá 100 ký tự")]
        public string TenThietBi { get; set; } = string.Empty;

        [Column("loaithietbi")]
        [Display(Name = "Loại Thiết Bị")]
        [Required(ErrorMessage = "Loại thiết bị là bắt buộc")]
        [StringLength(50, ErrorMessage = "Loại thiết bị không được vượt quá 50 ký tự")]
        public string LoaiThietBi { get; set; } = string.Empty;

        [Column("hangsanxuat")]
        [Display(Name = "Hãng Sản Xuất")]
        [StringLength(50, ErrorMessage = "Hãng sản xuất không được vượt quá 50 ký tự")]
        public string? HangSanXuat { get; set; }

        [Column("model")]
        [Display(Name = "Model")]
        [StringLength(50, ErrorMessage = "Model không được vượt quá 50 ký tự")]
        public string? Model { get; set; }

        [Column("soserial")]
        [Display(Name = "Số Serial")]
        [StringLength(100, ErrorMessage = "Số serial không được vượt quá 100 ký tự")]
        public string? SoSerial { get; set; }

        // Navigation Properties
        [ForeignKey("MaKhachHang")]
        public virtual KhachHang? KhachHang { get; set; }

        // Display Properties
        [Display(Name = "Thông Tin Thiết Bị")]
        public string ThongTinThietBi => $"{TenThietBi} - {LoaiThietBi}";

        [Display(Name = "Thông Tin Đầy Đủ")]
        public string ThongTinDayDu
        {
            get
            {
                var info = TenThietBi;
                if (!string.IsNullOrEmpty(HangSanXuat))
                    info += $" ({HangSanXuat}";
                if (!string.IsNullOrEmpty(Model))
                    info += $" {Model}";
                if (!string.IsNullOrEmpty(HangSanXuat))
                    info += ")";
                return info;
            }
        }

        [Display(Name = "Mô Tả Ngắn")]
        public string MoTaNgan
        {
            get
            {
                var desc = $"{LoaiThietBi}";
                if (!string.IsNullOrEmpty(HangSanXuat))
                    desc += $" {HangSanXuat}";
                return desc;
            }
        }

        // Helper Methods
        public string GetDeviceIcon()
        {
            return LoaiThietBi?.ToLower() switch
            {
                var type when type.Contains("điện thoại") || type.Contains("phone") => "fas fa-mobile-alt",
                var type when type.Contains("laptop") || type.Contains("máy tính") => "fas fa-laptop",
                var type when type.Contains("tablet") || type.Contains("máy tính bảng") => "fas fa-tablet-alt",
                var type when type.Contains("đồng hồ") || type.Contains("watch") => "fas fa-clock",
                var type when type.Contains("tai nghe") || type.Contains("headphone") => "fas fa-headphones",
                var type when type.Contains("camera") || type.Contains("máy ảnh") => "fas fa-camera",
                var type when type.Contains("tivi") || type.Contains("tv") => "fas fa-tv",
                var type when type.Contains("máy in") || type.Contains("printer") => "fas fa-print",
                _ => "fas fa-microchip"
            };
        }

        public string GetDeviceColor()
        {
            return LoaiThietBi?.ToLower() switch
            {
                var type when type.Contains("điện thoại") || type.Contains("phone") => "text-primary",
                var type when type.Contains("laptop") || type.Contains("máy tính") => "text-success",
                var type when type.Contains("tablet") => "text-info",
                var type when type.Contains("đồng hồ") => "text-warning",
                var type when type.Contains("tai nghe") => "text-secondary",
                var type when type.Contains("camera") => "text-danger",
                _ => "text-muted"
            };
        }

        public bool HasCompleteInfo()
        {
            return !string.IsNullOrEmpty(HangSanXuat) && 
                   !string.IsNullOrEmpty(Model) && 
                   !string.IsNullOrEmpty(SoSerial);
        }

        public string GetCompletionStatus()
        {
            var completed = 0;
            var total = 3; // HangSanXuat, Model, SoSerial

            if (!string.IsNullOrEmpty(HangSanXuat)) completed++;
            if (!string.IsNullOrEmpty(Model)) completed++;
            if (!string.IsNullOrEmpty(SoSerial)) completed++;

            return $"{completed}/{total}";
        }

        // Navigation Properties (sẽ được thêm sau)
        // public virtual ICollection<PhieuSua>? PhieuSuas { get; set; }
    }
}