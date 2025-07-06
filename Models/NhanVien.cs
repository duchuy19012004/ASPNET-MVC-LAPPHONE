using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace phonev2.Models
{
    [Table("NhanVien")]
    public class NhanVien
    {
        [Key]
        [Column("manhanvien")]
        [Display(Name = "Mã Nhân Viên")]
        public int MaNhanVien { get; set; }

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

        [Column("email")]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string Email { get; set; } = string.Empty;

        [Column("diachi")]
        [Display(Name = "Địa Chỉ")]
        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
        public string DiaChi { get; set; } = string.Empty;

        [Column("ngaysinh")]
        [Display(Name = "Ngày Sinh")]
        [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
        [DataType(DataType.Date)]
        public DateTime NgaySinh { get; set; }

        [Column("ngayvaolam")]
        [Display(Name = "Ngày Vào Làm")]
        [Required(ErrorMessage = "Ngày vào làm là bắt buộc")]
        [DataType(DataType.Date)]
        public DateTime NgayVaoLam { get; set; } = DateTime.Now;

        [Column("ngaynghiviec")]
        [Display(Name = "Ngày Nghỉ Việc")]
        [DataType(DataType.Date)]
        public DateTime? NgayNghiViec { get; set; }

        [Column("chucvu")]
        [Display(Name = "Chức Vụ")]
        [Required(ErrorMessage = "Chức vụ là bắt buộc")]
        [StringLength(50, ErrorMessage = "Chức vụ không được vượt quá 50 ký tự")]
        public string ChucVu { get; set; } = string.Empty;

        [Column("luong")]
        [Display(Name = "Lương")]
        [Required(ErrorMessage = "Lương là bắt buộc")]
        [Range(0, 999999999, ErrorMessage = "Lương phải lớn hơn 0")]
        public decimal Luong { get; set; }

        [Column("trangthai")]
        [Display(Name = "Trạng Thái")]
        public bool TrangThai { get; set; } = true;

        // Display Properties
        [Display(Name = "Trạng Thái")]
        public string TrangThaiText => TrangThai ? "Đang làm việc" : "Đã nghỉ việc";

        [Display(Name = "Tuổi")]
        public int Tuoi => DateTime.Now.Year - NgaySinh.Year - (DateTime.Now.DayOfYear < NgaySinh.DayOfYear ? 1 : 0);

        [Display(Name = "Số Năm Làm Việc")]
        public int SoNamLamViec
        {
            get
            {
                var endDate = NgayNghiViec ?? DateTime.Now;
                return endDate.Year - NgayVaoLam.Year;
            }
        }

        [Display(Name = "Lương Text")]
        public string LuongText => Luong.ToString("N0") + " VNĐ";

        [Display(Name = "Thông Tin Liên Hệ")]
        public string ThongTinLienHe => $"{SoDienThoai} - {Email}";

        [Display(Name = "Họ Tên Ngắn")]
        public string HoTenNgan
        {
            get
            {
                var parts = HoTen.Trim().Split(' ');
                if (parts.Length >= 2)
                {
                    return $"{parts[0]} {parts[parts.Length - 1]}";
                }
                return HoTen;
            }
        }

        [Display(Name = "Địa Chỉ Ngắn")]
        public string DiaChiNgan => DiaChi.Length > 50 ? DiaChi.Substring(0, 50) + "..." : DiaChi;

        // Navigation Properties (sẽ được thêm sau)
        // public virtual ICollection<PhieuSua>? PhieuSuas { get; set; }
        // public virtual ICollection<PhieuNhap>? PhieuNhaps { get; set; }
        // public virtual ICollection<HoaDon>? HoaDons { get; set; }

        // Business Logic Methods
        public bool IsActiveEmployee()
        {
            return TrangThai && !NgayNghiViec.HasValue;
        }

        public bool IsRetired()
        {
            return NgayNghiViec.HasValue;
        }

        public string GetDisplayName()
        {
            return $"{HoTen} - {ChucVu}";
        }

        public bool CanWork()
        {
            return TrangThai && !NgayNghiViec.HasValue && Tuoi >= 18;
        }

        public string GetWorkExperience()
        {
            var years = SoNamLamViec;
            if (years == 0) return "Dưới 1 năm";
            if (years == 1) return "1 năm";
            return $"{years} năm";
        }

        public string GetContactSummary()
        {
            return $"{HoTen} ({ChucVu}) - {SoDienThoai}";
        }

        public bool IsValidContact()
        {
            return !string.IsNullOrEmpty(SoDienThoai) && !string.IsNullOrEmpty(Email);
        }

        public bool IsSeniorEmployee()
        {
            return SoNamLamViec >= 5;
        }

        public string GetAgeGroup()
        {
            if (Tuoi < 25) return "Trẻ";
            if (Tuoi < 40) return "Trung niên";
            return "Cao tuổi";
        }
    }
}