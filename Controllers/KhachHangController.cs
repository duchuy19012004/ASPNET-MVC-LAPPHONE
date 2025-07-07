using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using phonev2.Models;
using phonev2.Repository;

namespace phonev2.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly PhoneLapDbContext _context;

        public KhachHangController(PhoneLapDbContext context)
        {
            _context = context;
        }

        // GET: KhachHang
        public async Task<IActionResult> Index(string searchString, string customerLevelFilter, string statusFilter, string sortOrder, int page = 1, int pageSize = 10)
        {
            ViewData["Title"] = "Quản Lý Khách Hàng";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentLevel"] = customerLevelFilter;
            ViewData["CurrentStatus"] = statusFilter;
            ViewData["CurrentSort"] = sortOrder;
            
            // Sort parameters
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["SpendingSortParm"] = sortOrder == "spending" ? "spending_desc" : "spending";
            ViewData["DateSortParm"] = sortOrder == "date" ? "date_desc" : "date";
            ViewData["LevelSortParm"] = sortOrder == "level" ? "level_desc" : "level";

            var khachHangs = from kh in _context.KhachHang select kh;

            // Tìm kiếm
            if (!String.IsNullOrEmpty(searchString))
            {
                khachHangs = khachHangs.Where(kh => kh.HoTen.Contains(searchString) 
                                                || kh.SoDienThoai.Contains(searchString)
                                                || kh.DiaChi.Contains(searchString));
            }

            // Lọc theo mức khách hàng
            if (!String.IsNullOrEmpty(customerLevelFilter))
            {
                switch (customerLevelFilter)
                {
                    case "vip":
                        khachHangs = khachHangs.Where(kh => kh.TongChiTieu >= 50000000);
                        break;
                    case "loyal":
                        khachHangs = khachHangs.Where(kh => kh.TongChiTieu >= 20000000 && kh.TongChiTieu < 50000000);
                        break;
                    case "silver":
                        khachHangs = khachHangs.Where(kh => kh.TongChiTieu >= 10000000 && kh.TongChiTieu < 20000000);
                        break;
                    case "bronze":
                        khachHangs = khachHangs.Where(kh => kh.TongChiTieu >= 5000000 && kh.TongChiTieu < 10000000);
                        break;
                    case "normal":
                        khachHangs = khachHangs.Where(kh => kh.TongChiTieu < 5000000);
                        break;
                }
            }

            // Lọc theo trạng thái
            if (!String.IsNullOrEmpty(statusFilter))
            {
                switch (statusFilter)
                {
                    case "active":
                        khachHangs = khachHangs.Where(kh => kh.TrangThai);
                        break;
                    case "blocked":
                        khachHangs = khachHangs.Where(kh => !kh.TrangThai);
                        break;
                    case "new":
                        var thirtyDaysAgo = DateTime.Now.AddDays(-30);
                        khachHangs = khachHangs.Where(kh => kh.NgayTao >= thirtyDaysAgo);
                        break;
                }
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "name_desc":
                    khachHangs = khachHangs.OrderByDescending(kh => kh.HoTen);
                    break;
                case "spending":
                    khachHangs = khachHangs.OrderBy(kh => kh.TongChiTieu);
                    break;
                case "spending_desc":
                    khachHangs = khachHangs.OrderByDescending(kh => kh.TongChiTieu);
                    break;
                case "date":
                    khachHangs = khachHangs.OrderBy(kh => kh.NgayTao);
                    break;
                case "date_desc":
                    khachHangs = khachHangs.OrderByDescending(kh => kh.NgayTao);
                    break;
                case "level":
                    khachHangs = khachHangs.OrderBy(kh => kh.TongChiTieu);
                    break;
                case "level_desc":
                    khachHangs = khachHangs.OrderByDescending(kh => kh.TongChiTieu);
                    break;
                default:
                    khachHangs = khachHangs.OrderBy(kh => kh.HoTen);
                    break;
            }

            // Pagination
            var totalItems = await khachHangs.CountAsync();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            var result = await khachHangs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return View(result);
        }

        // GET: KhachHang/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["Title"] = "Chi Tiết Khách Hàng";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khách hàng.";
                return RedirectToAction(nameof(Index));
            }

            var khachHang = await _context.KhachHang
                .FirstOrDefaultAsync(m => m.MaKhachHang == id);
            
            if (khachHang == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khách hàng.";
                return RedirectToAction(nameof(Index));
            }

            return View(khachHang);
        }

        // GET: KhachHang/Create
        public IActionResult Create()
        {
            ViewData["Title"] = "Thêm Khách Hàng Mới";
            
            var khachHang = new KhachHang
            {
                NgayTao = DateTime.Now,
                TrangThai = true,
                TongChiTieu = 0
            };
            
            return View(khachHang);
        }

        // POST: KhachHang/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HoTen,SoDienThoai,DiaChi,TrangThai")] KhachHang khachHang)
        {
            ViewData["Title"] = "Thêm Khách Hàng Mới";

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra trùng số điện thoại
                    var existingByPhone = await _context.KhachHang
                        .AnyAsync(kh => kh.SoDienThoai == khachHang.SoDienThoai);
                    
                    if (existingByPhone)
                    {
                        ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng bởi khách hàng khác.");
                        return View(khachHang);
                    }

                    // Set default values
                    khachHang.NgayTao = DateTime.Now;
                    khachHang.TongChiTieu = 0;

                    _context.Add(khachHang);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Đã thêm thành công khách hàng '{khachHang.HoTen}'.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi thêm khách hàng: " + ex.Message);
                }
            }
            
            return View(khachHang);
        }

        // GET: KhachHang/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Title"] = "Sửa Thông Tin Khách Hàng";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khách hàng.";
                return RedirectToAction(nameof(Index));
            }

            var khachHang = await _context.KhachHang.FindAsync(id);
            if (khachHang == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khách hàng.";
                return RedirectToAction(nameof(Index));
            }
            
            return View(khachHang);
        }

        // POST: KhachHang/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaKhachHang,HoTen,SoDienThoai,DiaChi,NgayTao,TongChiTieu,TrangThai")] KhachHang khachHang)
        {
            ViewData["Title"] = "Sửa Thông Tin Khách Hàng";

            if (id != khachHang.MaKhachHang)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra trùng số điện thoại (trừ chính nó)
                    var existingByPhone = await _context.KhachHang
                        .AnyAsync(kh => kh.SoDienThoai == khachHang.SoDienThoai 
                                    && kh.MaKhachHang != khachHang.MaKhachHang);
                    
                    if (existingByPhone)
                    {
                        ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng bởi khách hàng khác.");
                        return View(khachHang);
                    }

                    _context.Update(khachHang);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Đã cập nhật thành công khách hàng '{khachHang.HoTen}'.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhachHangExists(khachHang.MaKhachHang))
                    {
                        TempData["ErrorMessage"] = "Khách hàng không tồn tại.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật: " + ex.Message);
                }
            }
            
            return View(khachHang);
        }

        // GET: KhachHang/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["Title"] = "Xóa Khách Hàng";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khách hàng.";
                return RedirectToAction(nameof(Index));
            }

            var khachHang = await _context.KhachHang
                .FirstOrDefaultAsync(m => m.MaKhachHang == id);
                
            if (khachHang == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khách hàng.";
                return RedirectToAction(nameof(Index));
            }

            return View(khachHang);
        }

        // POST: KhachHang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var khachHang = await _context.KhachHang.FindAsync(id);
                if (khachHang != null)
                {
                    _context.KhachHang.Remove(khachHang);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Đã xóa thành công khách hàng '{khachHang.HoTen}'.";
                }
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Không thể xóa khách hàng này vì đang có giao dịch liên quan.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa: " + ex.Message;
            }
            
            return RedirectToAction(nameof(Index));
        }

        // AJAX: Toggle Status
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var khachHang = await _context.KhachHang.FindAsync(id);
                if (khachHang == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy khách hàng" });
                }

                khachHang.TrangThai = !khachHang.TrangThai;
                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    message = khachHang.TrangThai ? "Đã kích hoạt khách hàng" : "Đã khóa khách hàng",
                    newStatus = khachHang.TrangThai
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // AJAX: Update Spending
        [HttpPost]
        public async Task<IActionResult> UpdateSpending(int id, decimal amount, string action = "add")
        {
            try
            {
                var khachHang = await _context.KhachHang.FindAsync(id);
                if (khachHang == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy khách hàng" });
                }

                if (action == "add")
                {
                    khachHang.TongChiTieu += amount;
                }
                else if (action == "subtract")
                {
                    khachHang.TongChiTieu = Math.Max(0, khachHang.TongChiTieu - amount);
                }
                else
                {
                    khachHang.TongChiTieu = amount;
                }

                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    newAmount = khachHang.TongChiTieu,
                    newLevel = khachHang.GetCustomerLevel(),
                    formattedAmount = khachHang.TongChiTieuText
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // Statistics
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var totalCustomers = await _context.KhachHang.CountAsync();
                var activeCustomers = await _context.KhachHang.CountAsync(kh => kh.TrangThai);
                var vipCustomers = await _context.KhachHang.CountAsync(kh => kh.TongChiTieu >= 50000000);
                var newCustomers = await _context.KhachHang.CountAsync(kh => kh.NgayTao >= DateTime.Now.AddDays(-30));
                var totalSpending = await _context.KhachHang.SumAsync(kh => kh.TongChiTieu);
                var averageSpending = totalCustomers > 0 ? totalSpending / totalCustomers : 0;

                return Json(new {
                    success = true,
                    data = new {
                        totalCustomers,
                        activeCustomers,
                        vipCustomers,
                        newCustomers,
                        totalSpending,
                        averageSpending
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private bool KhachHangExists(int id)
        {
            return _context.KhachHang.Any(e => e.MaKhachHang == id);
        }
    }
}