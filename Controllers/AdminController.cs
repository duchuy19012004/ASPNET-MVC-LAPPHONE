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
            ViewData["Title"] = "Quản Lý Loại Linh Kiện";
            return View();
        }

        public IActionResult NhaCungCap()
        {
            ViewData["Title"] = "Quản Lý Nhà Cung Cấp";
            return View();
        }

        public IActionResult DichVu()
        {
            ViewData["Title"] = "Quản Lý Dịch Vụ";
            return View();
        }

        public IActionResult NhanVien()
        {
            ViewData["Title"] = "Quản Lý Nhân Viên";
            return View();
        }

        public IActionResult KhachHang()
        {
            ViewData["Title"] = "Quản Lý Khách Hàng";
            return View();
        }

        public IActionResult LinhKien()
        {
            ViewData["Title"] = "Quản Lý Linh Kiện";
            return View();
        }

        public IActionResult ThietBi()
        {
            ViewData["Title"] = "Quản Lý Thiết Bị";
            return View();
        }

        public IActionResult PhieuNhap()
        {
            ViewData["Title"] = "Quản Lý Phiếu Nhập";
            return View();
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