using Microsoft.AspNetCore.Mvc;
using phonev2.Repository;

namespace phonev2.Controllers
{
    public class AdminController : Controller
    {
        private readonly PhoneLapDbContext _context;

        public AdminController(PhoneLapDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Dashboard";
            
            // TODO: Thêm logic thống kê khi có dữ liệu
            var dashboardData = new
            {
                TotalCustomers = 0, // _context.KhachHangs.Count(),
                TotalRepairs = 0,   // _context.PhieuSuas.Count(),
                TotalRevenue = 0,   // _context.HoaDons.Sum(h => h.TongTien),
                PendingRepairs = 0  // _context.PhieuSuas.Where(p => p.TrangThai == "Pending").Count()
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
            ViewData["Title"] = "Quản Lý Phiếu Sửa Chữa";
            return View();
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
    }
}