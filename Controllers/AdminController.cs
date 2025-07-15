using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using phonev2.Repository;
using phonev2.Models;
using System.Linq;

namespace phonev2.Controllers
{
    public class AdminController : Controller
    {
        private readonly PhoneLapDbContext _context;

        public AdminController(PhoneLapDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            ViewData["Title"] = "Dashboard";
            
            // Thống kê tổng quan
            var totalCustomers = await _context.KhachHang.CountAsync();
            var totalRepairs = await _context.PhieuSua.CountAsync();
            var totalRevenue = await _context.PhieuSua
                .Where(p => p.TrangThai == TrangThaiPhieuSua.HoanThanh)
                .SumAsync(p => p.TongTien ?? 0);
            var pendingRepairs = await _context.PhieuSua
                .Where(p => p.TrangThai == TrangThaiPhieuSua.DangSua)
                .CountAsync();

            // Thống kê theo trạng thái phiếu sửa
            var thongKeTrangThai = await _context.PhieuSua
                .GroupBy(p => p.TrangThai)
                .Select(g => new { 
                    TrangThai = g.Key, 
                    SoLuong = g.Count() 
                })
                .ToListAsync();

            // Thống kê theo nhân viên (top 5)
            var topNhanVien = await _context.PhieuSua
                .Where(p => p.MaNhanVien.HasValue && p.TrangThai == TrangThaiPhieuSua.HoanThanh)
                .GroupBy(p => p.MaNhanVien)
                .Select(g => new
                {
                    MaNhanVien = g.Key,
                    HoTen = _context.NhanVien.Where(nv => nv.MaNhanVien == g.Key).Select(nv => nv.HoTen).FirstOrDefault(),
                    SoPhieuSua = g.Count(),
                    TongDoanhThu = g.Sum(p => p.TongTien ?? 0)
                })
                .OrderByDescending(x => x.TongDoanhThu)
                .Take(5)
                .ToListAsync();

            // Phiếu sửa gần đây (10 phiếu mới nhất)
            var recentRepairs = await _context.PhieuSua
                .Include(p => p.ChiTietLinhKiens).ThenInclude(ct => ct.LinhKien)
                .OrderByDescending(p => p.NgaySua)
                .Take(10)
                .Select(p => new
                {
                    p.MaPhieuSua,
                    p.NgaySua,
                    p.TrangThai,
                    p.TongTien,
                    KhachHang = _context.KhachHang.Where(kh => kh.MaKhachHang == p.MaKhachHang).Select(kh => kh.HoTen).FirstOrDefault(),
                    NhanVien = _context.NhanVien.Where(nv => nv.MaNhanVien == p.MaNhanVien).Select(nv => nv.HoTen).FirstOrDefault(),
                    ThietBi = p.ChiTietLinhKiens.FirstOrDefault() != null ? 
                             p.ChiTietLinhKiens.First().LinhKien.TenLinhKien : "N/A"
                })
                .ToListAsync();

            // Thống kê theo tháng (6 tháng gần nhất)
            var today = DateTime.Today;
            var thongKeTheoThang = new List<object>();
            for (int i = 5; i >= 0; i--)
            {
                var thang = today.AddMonths(-i);
                var startOfMonth = new DateTime(thang.Year, thang.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                var soPhieuSua = await _context.PhieuSua
                    .CountAsync(p => p.NgaySua >= startOfMonth && p.NgaySua <= endOfMonth);
                var doanhThu = await _context.PhieuSua
                    .Where(p => p.TrangThai == TrangThaiPhieuSua.HoanThanh && 
                               p.NgaySua >= startOfMonth && p.NgaySua <= endOfMonth)
                    .SumAsync(p => p.TongTien ?? 0);

                thongKeTheoThang.Add(new
                {
                    Thang = thang.ToString("MM/yyyy"),
                    SoPhieuSua = soPhieuSua,
                    DoanhThu = doanhThu
                });
            }

            var dashboardData = new
            {
                TotalCustomers = totalCustomers,
                TotalRepairs = totalRepairs,
                TotalRevenue = totalRevenue,
                PendingRepairs = pendingRepairs,
                ThongKeTrangThai = thongKeTrangThai,
                TopNhanVien = topNhanVien,
                RecentRepairs = recentRepairs,
                ThongKeTheoThang = thongKeTheoThang
            };

            return View(dashboardData);
        }

        // Các action methods cho từng module sẽ được thêm sau
        public IActionResult LoaiLinhKien()
        {
            return RedirectToAction("Index", "LoaiLinhKien");
        }

        public IActionResult NhaCungCap()
        {
            return RedirectToAction("Index", "NhaCungCap");
        }

        public IActionResult DichVu()
        {
            return RedirectToAction("Index", "DichVu");
        }

        public IActionResult NhanVien()
        {
            return RedirectToAction("Index", "NhanVien");
        }

        public IActionResult KhachHang()
        {
            return RedirectToAction("Index", "KhachHang");
        }

        public IActionResult LinhKien()
        {
            return RedirectToAction("Index", "LinhKien");
        }

        public IActionResult ThietBi()
        {
            return RedirectToAction("Index", "ThietBi");
        }

        public IActionResult PhieuNhap()
        {
            return RedirectToAction("Index", "PhieuNhap");
        }

        public IActionResult PhieuSua()
        {
            return RedirectToAction("Index", "PhieuSua");
        }

        public IActionResult HoaDon()
        {
            ViewData["Title"] = "Quản Lý Hóa Đơn";
            return View();
        }

        public IActionResult Reports()
        {
            ViewData["Title"] = "Báo Cáo Doanh Thu";
            return View();
        }

        public IActionResult Statistics()
        {
            ViewData["Title"] = "Thống Kê";
            return View();
        }

        public IActionResult ThongKePhieuSua()
        {
            return RedirectToAction("Index", "PhieuSuaThongKe");
        }
    }
}