using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using phonev2.Models;
using phonev2.Repository;

namespace phonev2.Controllers
{
    public class ThietBiController : Controller
    {
        private readonly PhoneLapDbContext _context;

        public ThietBiController(PhoneLapDbContext context)
        {
            _context = context;
        }

        // GET: ThietBi
        public async Task<IActionResult> Index(string searchString, string categoryFilter, int? customerFilter, string sortOrder, int page = 1, int pageSize = 10)
        {
            ViewData["Title"] = "Quản Lý Thiết Bị";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCategory"] = categoryFilter;
            ViewData["CurrentCustomer"] = customerFilter;
            ViewData["CurrentSort"] = sortOrder;

            // Sort parameters
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CategorySortParm"] = sortOrder == "category" ? "category_desc" : "category";
            ViewData["CustomerSortParm"] = sortOrder == "customer" ? "customer_desc" : "customer";
            ViewData["BrandSortParm"] = sortOrder == "brand" ? "brand_desc" : "brand";

            var thietBis = from tb in _context.ThietBi.Include(tb => tb.KhachHang) select tb;

            // Tìm kiếm
            if (!String.IsNullOrEmpty(searchString))
            {
                thietBis = thietBis.Where(tb => tb.TenThietBi.Contains(searchString)
                                            || (!string.IsNullOrEmpty(tb.HangSanXuat) && tb.HangSanXuat.Contains(searchString))
                                            || (!string.IsNullOrEmpty(tb.Model) && tb.Model.Contains(searchString))
                                            || tb.LoaiThietBi.Contains(searchString)
                                            || tb.KhachHang!.HoTen.Contains(searchString));
            }

            // Lọc theo loại thiết bị
            if (!String.IsNullOrEmpty(categoryFilter))
            {
                thietBis = thietBis.Where(tb => tb.LoaiThietBi.Contains(categoryFilter));
            }

            // Lọc theo khách hàng
            if (customerFilter.HasValue)
            {
                thietBis = thietBis.Where(tb => tb.MaKhachHang == customerFilter.Value);
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "name_desc":
                    thietBis = thietBis.OrderByDescending(tb => tb.TenThietBi);
                    break;
                case "category":
                    thietBis = thietBis.OrderBy(tb => tb.LoaiThietBi);
                    break;
                case "category_desc":
                    thietBis = thietBis.OrderByDescending(tb => tb.LoaiThietBi);
                    break;
                case "customer":
                    thietBis = thietBis.OrderBy(tb => tb.KhachHang!.HoTen);
                    break;
                case "customer_desc":
                    thietBis = thietBis.OrderByDescending(tb => tb.KhachHang!.HoTen);
                    break;
                case "brand":
                    thietBis = thietBis.OrderBy(tb => tb.HangSanXuat);
                    break;
                case "brand_desc":
                    thietBis = thietBis.OrderByDescending(tb => tb.HangSanXuat);
                    break;
                default:
                    thietBis = thietBis.OrderBy(tb => tb.TenThietBi);
                    break;
            }

            // Phân trang
            var totalCount = await thietBis.CountAsync();
            var items = await thietBis.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.TotalCount = totalCount;
            ViewBag.PageSize = pageSize;

            // Dropdown data cho filters
            ViewBag.KhachHangList = new SelectList(await _context.KhachHang
                .Where(kh => kh.TrangThai)
                .OrderBy(kh => kh.HoTen)
                .ToListAsync(), "MaKhachHang", "HoTen");

            ViewBag.CategoryList = await _context.ThietBi
                .Where(tb => !string.IsNullOrEmpty(tb.LoaiThietBi))
                .Select(tb => tb.LoaiThietBi)
                .Distinct()
                .OrderBy(category => category)
                .ToListAsync();

            return View(items);
        }

        // GET: ThietBi/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["Title"] = "Chi Tiết Thiết Bị";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thiết bị.";
                return RedirectToAction(nameof(Index));
            }

            var thietBi = await _context.ThietBi
                .Include(tb => tb.KhachHang)
                .FirstOrDefaultAsync(m => m.MaThietBi == id);

            if (thietBi == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thiết bị.";
                return RedirectToAction(nameof(Index));
            }

            return View(thietBi);
        }

        // GET: ThietBi/Create
        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "Thêm Thiết Bị Mới";
            await LoadDropdownData();
            return View();
        }

        // POST: ThietBi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaKhachHang,TenThietBi,LoaiThietBi,HangSanXuat,Model")] ThietBi thietBi)
        {
            ViewData["Title"] = "Thêm Thiết Bị Mới";

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra thiết bị đã tồn tại chưa (cùng khách hàng, tên, loại)
                    var existingDevice = await _context.ThietBi
                        .AnyAsync(tb => tb.MaKhachHang == thietBi.MaKhachHang
                                    && tb.TenThietBi.ToLower() == thietBi.TenThietBi.ToLower()
                                    && tb.LoaiThietBi.ToLower() == thietBi.LoaiThietBi.ToLower()
                                    && (string.IsNullOrEmpty(thietBi.HangSanXuat) ||
                                        (tb.HangSanXuat != null && tb.HangSanXuat.ToLower() == thietBi.HangSanXuat.ToLower()))
                                    && (string.IsNullOrEmpty(thietBi.Model) ||
                                        (tb.Model != null && tb.Model.ToLower() == thietBi.Model.ToLower())));

                    if (existingDevice)
                    {
                        ModelState.AddModelError("TenThietBi", "Thiết bị này đã tồn tại cho khách hàng này.");
                        await LoadDropdownData();
                        return View(thietBi);
                    }

                    _context.Add(thietBi);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Đã thêm thành công thiết bị '{thietBi.TenThietBi}'.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi thêm thiết bị: " + ex.Message);
                }
            }

            await LoadDropdownData();
            return View(thietBi);
        }

        // GET: ThietBi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Title"] = "Sửa Thông Tin Thiết Bị";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thiết bị.";
                return RedirectToAction(nameof(Index));
            }

            var thietBi = await _context.ThietBi.FindAsync(id);
            if (thietBi == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thiết bị.";
                return RedirectToAction(nameof(Index));
            }

            await LoadDropdownData();
            return View(thietBi);
        }

        // POST: ThietBi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaThietBi,MaKhachHang,TenThietBi,LoaiThietBi,HangSanXuat,Model")] ThietBi thietBi)
        {
            ViewData["Title"] = "Sửa Thông Tin Thiết Bị";

            if (id != thietBi.MaThietBi)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra thiết bị đã tồn tại chưa (trừ chính nó)
                    var existingDevice = await _context.ThietBi
                        .AnyAsync(tb => tb.MaKhachHang == thietBi.MaKhachHang
                                    && tb.TenThietBi.ToLower() == thietBi.TenThietBi.ToLower()
                                    && tb.LoaiThietBi.ToLower() == thietBi.LoaiThietBi.ToLower()
                                    && tb.MaThietBi != thietBi.MaThietBi
                                    && (string.IsNullOrEmpty(thietBi.HangSanXuat) ||
                                        (tb.HangSanXuat != null && tb.HangSanXuat.ToLower() == thietBi.HangSanXuat.ToLower()))
                                    && (string.IsNullOrEmpty(thietBi.Model) ||
                                        (tb.Model != null && tb.Model.ToLower() == thietBi.Model.ToLower())));

                    if (existingDevice)
                    {
                        ModelState.AddModelError("TenThietBi", "Thiết bị này đã tồn tại cho khách hàng này.");
                        await LoadDropdownData();
                        return View(thietBi);
                    }

                    _context.Update(thietBi);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Đã cập nhật thành công thiết bị '{thietBi.TenThietBi}'.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThietBiExists(thietBi.MaThietBi))
                    {
                        TempData["ErrorMessage"] = "Thiết bị không tồn tại.";
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

            await LoadDropdownData();
            return View(thietBi);
        }

        // GET: ThietBi/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["Title"] = "Xóa Thiết Bị";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thiết bị.";
                return RedirectToAction(nameof(Index));
            }

            var thietBi = await _context.ThietBi
                .Include(tb => tb.KhachHang)
                .FirstOrDefaultAsync(m => m.MaThietBi == id);

            if (thietBi == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thiết bị.";
                return RedirectToAction(nameof(Index));
            }

            return View(thietBi);
        }

        // POST: ThietBi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var thietBi = await _context.ThietBi.FindAsync(id);
                if (thietBi != null)
                {
                    _context.ThietBi.Remove(thietBi);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Đã xóa thành công thiết bị '{thietBi.TenThietBi}'.";
                }
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Không thể xóa thiết bị này vì đang được sử dụng trong các phiếu sửa chữa.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // Helper Methods
        private bool ThietBiExists(int id)
        {
            return _context.ThietBi.Any(e => e.MaThietBi == id);
        }

        private async Task LoadDropdownData()
        {
            ViewBag.KhachHangList = new SelectList(
                await _context.KhachHang
                    .Where(kh => kh.TrangThai)
                    .OrderBy(kh => kh.HoTen)
                    .ToListAsync(),
                "MaKhachHang",
                "HoTen"
            );
        }

        // API: Lấy thiết bị theo khách hàng
        [HttpGet]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            try
            {
                var thietBis = await _context.ThietBi
                    .Where(tb => tb.MaKhachHang == customerId)
                    .Select(tb => new
                    {
                        id = tb.MaThietBi,
                        name = tb.TenThietBi,
                        type = tb.LoaiThietBi,
                        brand = tb.HangSanXuat,
                        model = tb.Model,
                        description = tb.ThongTinThietBi
                    })
                    .OrderBy(tb => tb.name)
                    .ToListAsync();

                return Json(new { success = true, data = thietBis });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Tìm kiếm AJAX
        [HttpGet]
        public async Task<IActionResult> SearchAjax(string term, int? customerId = null)
        {
            try
            {
                var query = _context.ThietBi
                    .Include(tb => tb.KhachHang)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(term))
                {
                    query = query.Where(tb => tb.TenThietBi.Contains(term) ||
                                            (tb.HangSanXuat != null && tb.HangSanXuat.Contains(term)) ||
                                            (tb.Model != null && tb.Model.Contains(term)) ||
                                            tb.LoaiThietBi.Contains(term));
                }

                if (customerId.HasValue)
                {
                    query = query.Where(tb => tb.MaKhachHang == customerId.Value);
                }

                var thietBis = await query
                    .Select(tb => new
                    {
                        id = tb.MaThietBi,
                        text = tb.ThongTinThietBi,
                        customer = tb.KhachHang!.HoTen,
                        type = tb.LoaiThietBi,
                        identifier = tb.LayMaNhanDienThietBi()
                    })
                    .Take(10)
                    .ToListAsync();

                return Json(thietBis);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Lấy thông tin thiết bị
        [HttpGet]
        public async Task<IActionResult> GetDeviceInfo(int id)
        {
            try
            {
                var thietBi = await _context.ThietBi
                    .Include(tb => tb.KhachHang)
                    .FirstOrDefaultAsync(tb => tb.MaThietBi == id);

                if (thietBi == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thiết bị" });
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        id = thietBi.MaThietBi,
                        name = thietBi.TenThietBi,
                        type = thietBi.LoaiThietBi,
                        brand = thietBi.HangSanXuat,
                        model = thietBi.Model,
                        customer = thietBi.KhachHang?.HoTen,
                        customerId = thietBi.MaKhachHang,
                        description = thietBi.LayMoTaDay(),
                        identifier = thietBi.LayMaNhanDienThietBi(),
                        isPhone = thietBi.LaThietBiDienThoai(),
                        isLaptop = thietBi.LaThietBiLaptop(),
                        icon = thietBi.LayIconThietBi()
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}