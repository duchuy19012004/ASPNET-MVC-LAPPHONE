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
        [Required(ErrorMessage = "Khách hàng là bắt buộc")]
        public int MaKhachHang { get; set; }

        [Column("tenthietbi")]
        [Display(Name = "Tên Thiết Bị")]
        [Required(ErrorMessage = "Tên thiết bị là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên thiết bị không được vượt quá 200 ký tự")]
        public string TenThietBi { get; set; } = string.Empty;

        [Column("loaithietbi")]
        [Display(Name = "Loại Thiết Bị")]
        [Required(ErrorMessage = "Loại thiết bị là bắt buộc")]
        [StringLength(50, ErrorMessage = "Loại thiết bị không được vượt quá 50 ký tự")]
        public string LoaiThietBi { get; set; } = string.Empty;

        [Column("hangsanxuat")]
        [Display(Name = "Hãng Sản Xuất")]
        [StringLength(100, ErrorMessage = "Hãng sản xuất không được vượt quá 100 ký tự")]
        public string? HangSanXuat { get; set; }

        [Column("model")]
        [Display(Name = "Model")]
        [StringLength(100, ErrorMessage = "Model không được vượt quá 100 ký tự")]
        public string? Model { get; set; }

        // Thuộc tính điều hướng
        [ForeignKey("MaKhachHang")]
        public virtual KhachHang? KhachHang { get; set; }

        // Thuộc tính điều hướng cho các bảng sẽ có sau
        // public virtual ICollection<PhieuSua>? PhieuSuas { get; set; }

        // Thuộc tính hiển thị
        [Display(Name = "Thông Tin Thiết Bị")]
        public string ThongTinThietBi
        {
            get
            {
                var parts = new List<string> { TenThietBi };
                if (!string.IsNullOrEmpty(HangSanXuat)) parts.Add(HangSanXuat);
                if (!string.IsNullOrEmpty(Model)) parts.Add(Model);
                return string.Join(" - ", parts);
            }
        }

        [Display(Name = "Mô Tả Ngắn")]
        public string MoTaNgan
        {
            get
            {
                var description = TenThietBi;
                if (!string.IsNullOrEmpty(HangSanXuat))
                {
                    description += $" ({HangSanXuat}";
                    if (!string.IsNullOrEmpty(Model))
                        description += $" {Model}";
                    description += ")";
                }
                return description.Length > 50 ? description.Substring(0, 47) + "..." : description;
            }
        }

        [Display(Name = "Loại Thiết Bị Badge")]
        public string LoaiThietBiBadge
        {
            get
            {
                return LoaiThietBi.ToLower() switch
                {
                    var x when x.Contains("điện thoại") || x.Contains("phone") => "badge bg-primary",
                    var x when x.Contains("laptop") || x.Contains("máy tính") => "badge bg-success",
                    var x when x.Contains("tablet") || x.Contains("máy tính bảng") => "badge bg-info",
                    var x when x.Contains("đồng hồ") || x.Contains("watch") => "badge bg-warning",
                    _ => "badge bg-secondary"
                };
            }
        }

        // Các phương thức logic nghiệp vụ
        public bool KiemTraThietBiHopLe()
        {
            return !string.IsNullOrEmpty(TenThietBi) && 
                   !string.IsNullOrEmpty(LoaiThietBi) && 
                   MaKhachHang > 0;
        }

        public string LayMaNhanDienThietBi()
        {
            return $"TB{MaThietBi:D6}"; // Định dạng: TB000001
        }

        public string LayMoTaDay()
        {
            var parts = new List<string> { TenThietBi };
            
            if (!string.IsNullOrEmpty(HangSanXuat)) parts.Add(HangSanXuat);
            if (!string.IsNullOrEmpty(Model)) parts.Add(Model);
            parts.Add($"(Loại: {LoaiThietBi})");
            
            return string.Join(" - ", parts);
        }

        public bool LaThietBiDienThoai()
        {
            return LoaiThietBi.ToLower().Contains("điện thoại") || 
                   LoaiThietBi.ToLower().Contains("phone") ||
                   LoaiThietBi.ToLower().Contains("smartphone");
        }

        public bool LaThietBiLaptop()
        {
            return LoaiThietBi.ToLower().Contains("laptop") || 
                   LoaiThietBi.ToLower().Contains("máy tính") ||
                   LoaiThietBi.ToLower().Contains("computer");
        }

        public bool LaThietBiTablet()
        {
            return LoaiThietBi.ToLower().Contains("tablet") || 
                   LoaiThietBi.ToLower().Contains("máy tính bảng") ||
                   LoaiThietBi.ToLower().Contains("ipad");
        }

        public string LayIconThietBi()
        {
            if (LaThietBiDienThoai()) return "📱";
            if (LaThietBiLaptop()) return "💻";
            if (LaThietBiTablet()) return "📊";
            return "🔧";
        }
    }
}