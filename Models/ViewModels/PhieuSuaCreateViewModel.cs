using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using phonev2.Models;

namespace phonev2.Models.ViewModels
{
    public class DichVuChonVM
    {
        public int MaDichVu { get; set; }
        public string TenDichVu { get; set; } = string.Empty;
        // Không cần số lượng cho dịch vụ
    }
    public class LinhKienChonVM
    {
        public int MaLinhKien { get; set; }
        public string TenLinhKien { get; set; } = string.Empty;
        public int SoLuong { get; set; } = 1;
        public int MaDichVu { get; set; } // Bổ sung để tick lại đúng linh kiện theo dịch vụ
    }
    public class DichVuOptionVM
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public decimal GiaDichVu { get; set; }
    }
    public class PhieuSuaCreateViewModel
    {
        public PhieuSua PhieuSua { get; set; } = new PhieuSua();
        public List<DichVuChonVM> DichVus { get; set; } = new List<DichVuChonVM>();
        public List<LinhKienChonVM> LinhKiens { get; set; } = new List<LinhKienChonVM>();
        public IEnumerable<SelectListItem> KhachHangList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> NhanVienList { get; set; } = new List<SelectListItem>();
        public List<DichVuOptionVM> DichVuList { get; set; } = new List<DichVuOptionVM>();
        public IEnumerable<SelectListItem> LinhKienList { get; set; } = new List<SelectListItem>();
        public KhachHang? KhachHang { get; set; }
    }
} 