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
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Vui lòng không nhập ký tự đặc biệt")]
        public decimal GiaNhap { get; set; }

        [Column("giaban")]
        [Display(Name = "Giá Bán (VNĐ)")]
        [Required(ErrorMessage = "Giá bán là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn hoặc bằng 0")]
        [DataType(DataType.Currency)]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Vui lòng không nhập ký tự đặc biệt")]
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

        // Thêm các trường cho soft delete
        [Column("daxoa")]
        [Display(Name = "Đã Xóa")]
        public bool DaXoa { get; set; } = false;

        [Column("ngayxoa")]
        [Display(Name = "Ngày Xóa")]
        public DateTime? NgayXoa { get; set; }

        [Column("lydoxoa")]
        [Display(Name = "Lý Do Xóa")]
        [StringLength(500, ErrorMessage = "Lý do xóa không được vượt quá 500 ký tự")]
        public string? LyDoXoa { get; set; }

        // Navigation Properties
        [ForeignKey("MaLoaiLinhKien")]
        public virtual LoaiLinhKien? LoaiLinhKien { get; set; }

        // Display Properties
        [Display(Name = "Trạng Thái")]
        public string TrangThaiText 
        {
            get
            {
                if (DaXoa) return "Đã xóa";
                return TrangThai ? "Đang bán" : "Ngừng bán";
            }
        }

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
                if (DaXoa) return "Đã xóa";
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
                if (DaXoa) return "badge bg-secondary";
                if (SoLuongTon == 0) return "badge bg-danger";
                if (SoLuongTon <= 5) return "badge bg-warning";
                if (SoLuongTon <= 20) return "badge bg-info";
                return "badge bg-success";
            }
        }

        // Thêm thuộc tính để hiển thị thông tin xóa
        [Display(Name = "Ngày Xóa")]
        public string NgayXoaText => NgayXoa?.ToString("dd/MM/yyyy HH:mm") ?? "";

        [Display(Name = "Trạng Thái Hiển Thị")]
        public string TrangThaiHienThi
        {
            get
            {
                if (DaXoa) return "Đã xóa";
                if (!TrangThai) return "Ngừng bán";
                return "Đang bán";
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

        // Soft Delete Methods
        public void SoftDelete(string lyDoXoa = "")
        {
            DaXoa = true;
            NgayXoa = DateTime.Now;
            LyDoXoa = lyDoXoa;
            TrangThai = false; // Ngừng bán khi xóa
        }

        public void Restore()
        {
            DaXoa = false;
            NgayXoa = null;
            LyDoXoa = null;
            TrangThai = true; // Khôi phục trạng thái bán
        }

        public bool IsDeleted => DaXoa;
        public bool IsActive => !DaXoa && TrangThai;
    }
}