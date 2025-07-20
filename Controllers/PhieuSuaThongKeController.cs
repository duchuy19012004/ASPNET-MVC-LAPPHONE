using Microsoft.AspNetCore.Mvc;
using phonev2.Services.PhieuSuaThongKe;
using Microsoft.EntityFrameworkCore;

namespace phonev2.Controllers
{
    public class PhieuSuaThongKeController : Controller
    {
        private readonly IPhieuSuaThongKeService _thongKeService;
        private readonly phonev2.Repository.PhoneLapDbContext _context;
        public PhieuSuaThongKeController(IPhieuSuaThongKeService thongKeService, phonev2.Repository.PhoneLapDbContext context)
        {
            _thongKeService = thongKeService;
            _context = context;
        }

        // View báo cáo
        public IActionResult Index()
        {
            // Truyền SelectList khách hàng và nhân viên vào ViewBag
            ViewBag.KhachHangList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.KhachHang.ToList(), "MaKhachHang", "HoTen");
            ViewBag.NhanVienList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.NhanVien.ToList(), "MaNhanVien", "HoTen");
            return View();
        }

        // API trả về dữ liệu thống kê cho biểu đồ
        [HttpGet]
        public async Task<IActionResult> GetThongKe(string type = "month", int? year = null, int? month = null, int? week = null)
        {
            var data = await _thongKeService.GetThongKeAsync(type, year, month, week);
            return Json(data);
        }

        // API: Top 5 linh kiện sử dụng nhiều nhất
        [HttpGet]
        public async Task<IActionResult> GetTopLinhKien(string type = "month", int? year = null, int? month = null, int? week = null, bool today = false)
        {
            if (today)
            {
                var now = DateTime.Now;
                year = now.Year;
                month = now.Month;
                type = "month";
            }
            var data = await _thongKeService.GetTopLinhKienAsync(5, type, year, month, week);
            return Json(data);
        }

        // API: Thống kê theo loại linh kiện
        [HttpGet]
        public async Task<IActionResult> GetThongKeLoaiLinhKien(string type = "month", int? year = null, int? month = null, int? week = null, bool today = false)
        {
            if (today)
            {
                var now = DateTime.Now;
                year = now.Year;
                month = now.Month;
                type = "month";
            }
            var data = await _thongKeService.GetThongKeLoaiLinhKienAsync(type, year, month, week);
            return Json(data);
        }

        // API: Top 5 dịch vụ được sử dụng nhiều nhất
        [HttpGet]
        public async Task<IActionResult> GetTopDichVu(string type = "month", int? year = null, int? month = null, int? week = null)
        {
            var data = await _thongKeService.GetTopDichVuAsync(5, type, year, month, week);
            return Json(data);
        }

        // API: Lịch sử phiếu sửa đã hoàn thành (theo từng dịch vụ trong ChiTietPhieuSua)
        [HttpGet]
        public async Task<IActionResult> GetLichSuHoanThanh(int? year = null, int? month = null, int? nhanVienId = null, int? khachHangId = null)
        {
            var query = _context.ChiTietPhieuSua
                .Include(ct => ct.DichVu)
                .Include(ct => ct.PhieuSua)
                    .ThenInclude(p => p.KhachHang)
                .Include(ct => ct.PhieuSua)
                    .ThenInclude(p => p.NhanVien)
                .Include(ct => ct.PhieuSua)
                    .ThenInclude(p => p.ChiTietLinhKiens)
                        .ThenInclude(lk => lk.LinhKien)
                .Where(ct => ct.PhieuSua != null && ct.PhieuSua.TrangThai == phonev2.Models.TrangThaiPhieuSua.HoanThanh);

            if (year.HasValue) query = query.Where(ct => ct.PhieuSua.NgaySua.Year == year.Value);
            if (month.HasValue) query = query.Where(ct => ct.PhieuSua.NgaySua.Month == month.Value);
            if (nhanVienId.HasValue) query = query.Where(ct => ct.PhieuSua.MaNhanVien == nhanVienId.Value);
            if (khachHangId.HasValue) query = query.Where(ct => ct.PhieuSua.MaKhachHang == khachHangId.Value);

            var data = await query
                .OrderByDescending(ct => ct.PhieuSua.NgaySua)
                .Select(ct => new {
                    Ngay = ct.PhieuSua.NgaySua.ToString("dd/MM/yyyy HH:mm"),
                    NgayGioHoanThanh = ct.PhieuSua.NgayGioHoanThanh != null ? ct.PhieuSua.NgayGioHoanThanh.Value.ToString("dd/MM/yyyy HH:mm") : null,
                    KhachHang = ct.PhieuSua.KhachHang != null ? ct.PhieuSua.KhachHang.HoTen : "",
                    NhanVien = ct.PhieuSua.NhanVien != null ? ct.PhieuSua.NhanVien.HoTen : "",
                    MaKhachHang = ct.PhieuSua.MaKhachHang,
                    MaNhanVien = ct.PhieuSua.MaNhanVien,
                    DichVu = ct.DichVu != null ? ct.DichVu.TenDichVu : "",
                    LinhKien = string.Join(", ", ct.PhieuSua.ChiTietLinhKiens.Select(lk => lk.LinhKien != null ? lk.LinhKien.TenLinhKien : "")),
                    TongTien = ct.PhieuSua.TongTien ?? 0
                })
                .ToListAsync();

            return Json(data);
        }
    }
} 