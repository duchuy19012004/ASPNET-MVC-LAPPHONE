using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace phonev2.Models
{
    [Table("LinhKien")]
    public class LinhKien
    {
        [Key]
        [Column("malinhkien")]
        [Display(Name = "Mã Linh Kiện")]
        public int MaLinhKien { get; set; }

        [Column("tenlinhkien")]
        [Display(Name = "Tên Linh Kiện")]
        [Required(ErrorMessage = "Tên linh kiện là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên linh kiện không được vượt quá 200 ký tự")]
        public string TenLinhKien { get; set; } = string.Empty;

        [Column("maloailinhkien")]
        [Display(Name = "Loại Linh Kiện")]
        [Required(ErrorMessage = "Loại linh kiện là bắt buộc")]
        public int MaLoaiLinhKien { get; set; }

        [Column("hangsanxuat")]
        [Display(Name = "Hãng Sản Xuất")]
        [StringLength(100, ErrorMessage = "Hãng sản xuất không được vượt quá 100 ký tự")]
        public string? HangSanXuat { get; set; }

        [Column("gianhap")]
        [Display(Name = "Giá Nhập (VNĐ)")]
        [Required(ErrorMessage = "Giá nhập là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá nhập phải lớn hơn hoặc bằng 0")]
        [DataType(DataType.Currency)]
        public decimal GiaNhap { get; set; }

        [Column("giaban")]
        [Display(Name = "Giá Bán (VNĐ)")]
        [Required(ErrorMessage = "Giá bán là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn hoặc bằng 0")]
        [DataType(DataType.Currency)]
        public decimal GiaBan { get; set; }

        [Column("soluongton")]
        [Display(Name = "Số Lượng Tồn")]
        [Required(ErrorMessage = "Số lượng tồn là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn phải lớn hơn hoặc bằng 0")]
        public int SoLuongTon { get; set; }

        [Column("thongsoktyhuat")]
        [Display(Name = "Thông Số Kỹ Thuật")]
        [StringLength(1000, ErrorMessage = "Thông số kỹ thuật không được vượt quá 1000 ký tự")]
        public string? ThongSoKyThuat { get; set; }

        [Column("ngaytao")]
        [Display(Name = "Ngày Tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Column("trangthai")]
        [Display(Name = "Trạng Thái")]
        public bool TrangThai { get; set; } = true;

        // Navigation Properties
        [ForeignKey("MaLoaiLinhKien")]
        public virtual LoaiLinhKien? LoaiLinhKien { get; set; }

        // Display Properties
        [Display(Name = "Trạng Thái")]
        public string TrangThaiText => TrangThai ? "Đang bán" : "Ngừng bán";

        [Display(Name = "Giá Nhập")]
        public string GiaNhapText => GiaNhap.ToString("N0") + " VNĐ";

        [Display(Name = "Giá Bán")]
        public string GiaBanText => GiaBan.ToString("N0") + " VNĐ";

        [Display(Name = "Lợi Nhuận")]
        public decimal LoiNhuan => GiaBan - GiaNhap;

        [Display(Name = "Lợi Nhuận")]
        public string LoiNhuanText => LoiNhuan.ToString("N0") + " VNĐ";

        [Display(Name = "Tỷ Lệ Lợi Nhuận")]
        public decimal TyLeLoiNhuan => GiaNhap > 0 ? Math.Round((LoiNhuan / GiaNhap) * 100, 2) : 0;

        [Display(Name = "Trạng Thái Tồn Kho")]
        public string TrangThaiTonKho
        {
            get
            {
                if (SoLuongTon == 0) return "Hết hàng";
                if (SoLuongTon <= 5) return "Sắp hết";
                if (SoLuongTon <= 20) return "Ít hàng";
                return "Còn hàng";
            }
        }

        [Display(Name = "CSS Class Tồn Kho")]
        public string TonKhoCssClass
        {
            get
            {
                if (SoLuongTon == 0) return "badge bg-danger";
                if (SoLuongTon <= 5) return "badge bg-warning";
                if (SoLuongTon <= 20) return "badge bg-info";
                return "badge bg-success";
            }
        }

        // Validation Methods
        public bool IsValidPrice()
        {
            return GiaBan >= GiaNhap;
        }

        public bool IsLowStock(int threshold = 10)
        {
            return SoLuongTon <= threshold;
        }

        public bool IsOutOfStock()
        {
            return SoLuongTon == 0;
        }
    }
}