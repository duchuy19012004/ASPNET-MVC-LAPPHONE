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
        public async Task<IActionResult> Index(string searchString, string deviceTypeFilter, string brandFilter, int? customerFilter, string sortOrder, int page = 1, int pageSize = 10)
        {
            ViewData["Title"] = "Quản Lý Thiết Bị";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentDeviceType"] = deviceTypeFilter;
            ViewData["CurrentBrand"] = brandFilter;
            ViewData["CurrentCustomer"] = customerFilter;
            ViewData["CurrentSort"] = sortOrder;
            
            // Sort parameters
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["TypeSortParm"] = sortOrder == "type" ? "type_desc" : "type";
            ViewData["BrandSortParm"] = sortOrder == "brand" ? "brand_desc" : "brand";
            ViewData["CustomerSortParm"] = sortOrder == "customer" ? "customer_desc" : "customer";

            var thietBis = from tb in _context.ThietBi.Include(t => t.KhachHang) select tb;

            // Tìm kiếm
            if (!String.IsNullOrEmpty(searchString))
            {
                thietBis = thietBis.Where(tb => tb.TenThietBi.Contains(searchString) 
                                              || tb.LoaiThietBi.Contains(searchString)
                                              || tb.HangSanXuat.Contains(searchString)
                                              || tb.Model.Contains(searchString)
                                              || tb.SoSerial.Contains(searchString)
                                              || tb.KhachHang.HoTen.Contains(searchString));
            }

            // Lọc theo loại thiết bị
            if (!String.IsNullOrEmpty(deviceTypeFilter))
            {
                thietBis = thietBis.Where(tb => tb.LoaiThietBi.Contains(deviceTypeFilter));
            }

            // Lọc theo hãng
            if (!String.IsNullOrEmpty(brandFilter))
            {
                thietBis = thietBis.Where(tb => tb.HangSanXuat.Contains(brandFilter));
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
                case "type":
                    thietBis = thietBis.OrderBy(tb => tb.LoaiThietBi);
                    break;
                case "type_desc":
                    thietBis = thietBis.OrderByDescending(tb => tb.LoaiThietBi);
                    break;
                case "brand":
                    thietBis = thietBis.OrderBy(tb => tb.HangSanXuat);
                    break;
                case "brand_desc":
                    thietBis = thietBis.OrderByDescending(tb => tb.HangSanXuat);
                    break;
                case "customer":
                    thietBis = thietBis.OrderBy(tb => tb.KhachHang.HoTen);
                    break;
                case "customer_desc":
                    thietBis = thietBis.OrderByDescending(tb => tb.KhachHang.HoTen);
                    break;
                default:
                    thietBis = thietBis.OrderBy(tb => tb.TenThietBi);
                    break;
            }

            // Pagination
            var totalItems = await thietBis.CountAsync();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            var result = await thietBis
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Dropdown data cho filters
            await LoadDropdownData();

            return View(result);
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
                .Include(t => t.KhachHang)
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
        public async Task<IActionResult> Create([Bind("MaKhachHang,TenThietBi,LoaiThietBi,HangSanXuat,Model,SoSerial")] ThietBi thietBi)
        {
            ViewData["Title"] = "Thêm Thiết Bị Mới";

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra trùng serial nếu có
                    if (!string.IsNullOrEmpty(thietBi.SoSerial))
                    {
                        var existingSerial = await _context.ThietBi
                            .AnyAsync(tb => tb.SoSerial == thietBi.SoSerial);
                        
                        if (existingSerial)
                        {
                            ModelState.AddModelError("SoSerial", "Số serial này đã tồn tại trong hệ thống.");
                            await LoadDropdownData();
                            return View(thietBi);
                        }
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
        public async Task<IActionResult> Edit(int id, [Bind("MaThietBi,MaKhachHang,TenThietBi,LoaiThietBi,HangSanXuat,Model,SoSerial")] ThietBi thietBi)
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
                    // Kiểm tra trùng serial (trừ chính nó)
                    if (!string.IsNullOrEmpty(thietBi.SoSerial))
                    {
                        var existingSerial = await _context.ThietBi
                            .AnyAsync(tb => tb.SoSerial == thietBi.SoSerial && tb.MaThietBi != thietBi.MaThietBi);
                        
                        if (existingSerial)
                        {
                            ModelState.AddModelError("SoSerial", "Số serial này đã tồn tại trong hệ thống.");
                            await LoadDropdownData();
                            return View(thietBi);
                        }
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
                .Include(t => t.KhachHang)
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
                TempData["ErrorMessage"] = "Không thể xóa thiết bị này vì đang có phiếu sửa chữa liên quan.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa: " + ex.Message;
            }
            
            return RedirectToAction(nameof(Index));
        }

        // AJAX: Get devices by customer
        [HttpGet]
        public async Task<IActionResult> GetDevicesByCustomer(int customerId)
        {
            try
            {
                var devices = await _context.ThietBi
                    .Where(tb => tb.MaKhachHang == customerId)
                    .Select(tb => new {
                        tb.MaThietBi,
                        tb.TenThietBi,
                        tb.LoaiThietBi,
                        tb.HangSanXuat,
                        tb.Model
                    })
                    .ToListAsync();

                return Json(new { success = true, data = devices });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Statistics
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var totalDevices = await _context.ThietBi.CountAsync();
                var devicesByType = await _context.ThietBi
                    .GroupBy(tb => tb.LoaiThietBi)
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .ToListAsync();
                var devicesByBrand = await _context.ThietBi
                    .Where(tb => !string.IsNullOrEmpty(tb.HangSanXuat))
                    .GroupBy(tb => tb.HangSanXuat)
                    .Select(g => new { Brand = g.Key, Count = g.Count() })
                    .ToListAsync();

                return Json(new {
                    success = true,
                    data = new {
                        totalDevices,
                        devicesByType,
                        devicesByBrand
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private async Task LoadDropdownData()
        {
            // Khách hàng
            ViewBag.KhachHangs = await _context.KhachHang
                .Where(kh => kh.TrangThai)
                .OrderBy(kh => kh.HoTen)
                .Select(kh => new SelectListItem
                {
                    Value = kh.MaKhachHang.ToString(),
                    Text = $"{kh.HoTen} - {kh.SoDienThoai}"
                })
                .ToListAsync();

            // Loại thiết bị (từ dữ liệu có sẵn)
            ViewBag.LoaiThietBis = await _context.ThietBi
                .Where(tb => !string.IsNullOrEmpty(tb.LoaiThietBi))
                .Select(tb => tb.LoaiThietBi)
                .Distinct()
                .OrderBy(lt => lt)
                .ToListAsync();

            // Hãng sản xuất (từ dữ liệu có sẵn)
            ViewBag.HangSanXuats = await _context.ThietBi
                .Where(tb => !string.IsNullOrEmpty(tb.HangSanXuat))
                .Select(tb => tb.HangSanXuat)
                .Distinct()
                .OrderBy(hsx => hsx)
                .ToListAsync();
        }

        private bool ThietBiExists(int id)
        {
            return _context.ThietBi.Any(e => e.MaThietBi == id);
        }
    }
}