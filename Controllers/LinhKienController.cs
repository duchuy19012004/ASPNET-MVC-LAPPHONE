using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using phonev2.Models;
using phonev2.Repository;

namespace phonev2.Controllers
{
    public class LinhKienController : Controller
    {
        private readonly PhoneLapDbContext _context;

        public LinhKienController(PhoneLapDbContext context)
        {
            _context = context;
        }

        // GET: LinhKien
        public async Task<IActionResult> Index(string searchString, string categoryFilter, string stockFilter, string sortOrder, int page = 1, int pageSize = 10)
        {
            ViewData["Title"] = "Quản Lý Linh Kiện";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCategory"] = categoryFilter;
            ViewData["CurrentStock"] = stockFilter;
            ViewData["CurrentSort"] = sortOrder;
            
            // Sort parameters
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "price" ? "price_desc" : "price";
            ViewData["StockSortParm"] = sortOrder == "stock" ? "stock_desc" : "stock";
            ViewData["CategorySortParm"] = sortOrder == "category" ? "category_desc" : "category";
            ViewData["DateSortParm"] = sortOrder == "date" ? "date_desc" : "date";

            var linhKiens = from l in _context.LinhKien.Include(l => l.LoaiLinhKien) select l;

            // Tìm kiếm
            if (!String.IsNullOrEmpty(searchString))
            {
                linhKiens = linhKiens.Where(l => l.TenLinhKien.Contains(searchString) 
                                             || (!string.IsNullOrEmpty(l.HangSanXuat) && l.HangSanXuat.Contains(searchString))
                                             || (!string.IsNullOrEmpty(l.ThongSoKyThuat) && l.ThongSoKyThuat.Contains(searchString)));
            }

            // Lọc theo loại
            if (!String.IsNullOrEmpty(categoryFilter) && int.TryParse(categoryFilter, out int categoryId))
            {
                linhKiens = linhKiens.Where(l => l.MaLoaiLinhKien == categoryId);
            }

            // Lọc theo tồn kho
            if (!String.IsNullOrEmpty(stockFilter))
            {
                switch (stockFilter)
                {
                    case "out":
                        linhKiens = linhKiens.Where(l => l.SoLuongTon == 0);
                        break;
                    case "low":
                        linhKiens = linhKiens.Where(l => l.SoLuongTon > 0 && l.SoLuongTon <= 5);
                        break;
                    case "normal":
                        linhKiens = linhKiens.Where(l => l.SoLuongTon > 5 && l.SoLuongTon <= 20);
                        break;
                    case "good":
                        linhKiens = linhKiens.Where(l => l.SoLuongTon > 20);
                        break;
                }
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "name_desc":
                    linhKiens = linhKiens.OrderByDescending(l => l.TenLinhKien);
                    break;
                case "price":
                    linhKiens = linhKiens.OrderBy(l => l.GiaBan);
                    break;
                case "price_desc":
                    linhKiens = linhKiens.OrderByDescending(l => l.GiaBan);
                    break;
                case "stock":
                    linhKiens = linhKiens.OrderBy(l => l.SoLuongTon);
                    break;
                case "stock_desc":
                    linhKiens = linhKiens.OrderByDescending(l => l.SoLuongTon);
                    break;
                case "category":
                    linhKiens = linhKiens.OrderBy(l => l.LoaiLinhKien!.TenLoaiLinhKien);
                    break;
                case "category_desc":
                    linhKiens = linhKiens.OrderByDescending(l => l.LoaiLinhKien!.TenLoaiLinhKien);
                    break;
                case "date":
                    linhKiens = linhKiens.OrderBy(l => l.NgayTao);
                    break;
                case "date_desc":
                    linhKiens = linhKiens.OrderByDescending(l => l.NgayTao);
                    break;
                default:
                    linhKiens = linhKiens.OrderBy(l => l.TenLinhKien);
                    break;
            }

            // Phân trang
            var totalCount = await linhKiens.CountAsync();
            var items = await linhKiens.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.TotalCount = totalCount;
            ViewBag.PageSize = pageSize;

            // Dropdown data
            ViewBag.LoaiLinhKienList = new SelectList(await _context.LoaiLinhKien
                .Where(l => l.TrangThai)
                .OrderBy(l => l.TenLoaiLinhKien)
                .ToListAsync(), "MaLoaiLinhKien", "TenLoaiLinhKien");

            return View(items);
        }

        // GET: LinhKien/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["Title"] = "Chi Tiết Linh Kiện";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy linh kiện.";
                return RedirectToAction(nameof(Index));
            }

            var linhKien = await _context.LinhKien
                .Include(l => l.LoaiLinhKien)
                .FirstOrDefaultAsync(m => m.MaLinhKien == id);
            
            if (linhKien == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy linh kiện.";
                return RedirectToAction(nameof(Index));
            }

            return View(linhKien);
        }

        // GET: LinhKien/Create
        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "Thêm Linh Kiện Mới";
            await LoadDropdownData();
            return View();
        }

        // POST: LinhKien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenLinhKien,MaLoaiLinhKien,HangSanXuat,GiaNhap,GiaBan,SoLuongTon,ThongSoKyThuat,TrangThai")] LinhKien linhKien)
        {
            ViewData["Title"] = "Thêm Linh Kiện Mới";

            // Custom validation
            if (linhKien.GiaBan < linhKien.GiaNhap)
            {
                ModelState.AddModelError("GiaBan", "Giá bán không được nhỏ hơn giá nhập.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra trùng tên trong cùng loại
                    var existingItem = await _context.LinhKien
                        .AnyAsync(l => l.TenLinhKien.ToLower() == linhKien.TenLinhKien.ToLower() 
                                    && l.MaLoaiLinhKien == linhKien.MaLoaiLinhKien
                                    && (!string.IsNullOrEmpty(l.HangSanXuat) && !string.IsNullOrEmpty(linhKien.HangSanXuat) 
                                        ? l.HangSanXuat.ToLower() == linhKien.HangSanXuat.ToLower() : true));
                    
                    if (existingItem)
                    {
                        ModelState.AddModelError("TenLinhKien", "Linh kiện này đã tồn tại trong hệ thống.");
                        await LoadDropdownData();
                        return View(linhKien);
                    }

                    linhKien.NgayTao = DateTime.Now;
                    _context.Add(linhKien);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Đã thêm thành công linh kiện '{linhKien.TenLinhKien}'.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi thêm linh kiện: " + ex.Message);
                }
            }
            
            await LoadDropdownData();
            return View(linhKien);
        }

        // GET: LinhKien/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Title"] = "Sửa Thông Tin Linh Kiện";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy linh kiện.";
                return RedirectToAction(nameof(Index));
            }

            var linhKien = await _context.LinhKien.FindAsync(id);
            if (linhKien == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy linh kiện.";
                return RedirectToAction(nameof(Index));
            }
            
            await LoadDropdownData();
            return View(linhKien);
        }

        // POST: LinhKien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaLinhKien,TenLinhKien,MaLoaiLinhKien,HangSanXuat,GiaNhap,GiaBan,SoLuongTon,ThongSoKyThuat,NgayTao,TrangThai")] LinhKien linhKien)
        {
            ViewData["Title"] = "Sửa Thông Tin Linh Kiện";

            if (id != linhKien.MaLinhKien)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            // Custom validation
            if (linhKien.GiaBan < linhKien.GiaNhap)
            {
                ModelState.AddModelError("GiaBan", "Giá bán không được nhỏ hơn giá nhập.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra trùng tên (trừ chính nó)
                    var existingItem = await _context.LinhKien
                        .AnyAsync(l => l.TenLinhKien.ToLower() == linhKien.TenLinhKien.ToLower() 
                                    && l.MaLoaiLinhKien == linhKien.MaLoaiLinhKien
                                    && l.MaLinhKien != linhKien.MaLinhKien
                                    && (!string.IsNullOrEmpty(l.HangSanXuat) && !string.IsNullOrEmpty(linhKien.HangSanXuat) 
                                        ? l.HangSanXuat.ToLower() == linhKien.HangSanXuat.ToLower() : true));
                    
                    if (existingItem)
                    {
                        ModelState.AddModelError("TenLinhKien", "Linh kiện này đã tồn tại trong hệ thống.");
                        await LoadDropdownData();
                        return View(linhKien);
                    }

                    _context.Update(linhKien);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Đã cập nhật thành công linh kiện '{linhKien.TenLinhKien}'.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LinhKienExists(linhKien.MaLinhKien))
                    {
                        TempData["ErrorMessage"] = "Linh kiện không tồn tại.";
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
            return View(linhKien);
        }

        // GET: LinhKien/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["Title"] = "Xóa Linh Kiện";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy linh kiện.";
                return RedirectToAction(nameof(Index));
            }

            var linhKien = await _context.LinhKien
                .Include(l => l.LoaiLinhKien)
                .FirstOrDefaultAsync(m => m.MaLinhKien == id);
                
            if (linhKien == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy linh kiện.";
                return RedirectToAction(nameof(Index));
            }

            return View(linhKien);
        }

        // POST: LinhKien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var linhKien = await _context.LinhKien.FindAsync(id);
                if (linhKien != null)
                {
                    _context.LinhKien.Remove(linhKien);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Đã xóa thành công linh kiện '{linhKien.TenLinhKien}'.";
                }
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Không thể xóa linh kiện này vì đang được sử dụng trong các giao dịch khác.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Toggle trạng thái
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var linhKien = await _context.LinhKien.FindAsync(id);
                if (linhKien != null)
                {
                    linhKien.TrangThai = !linhKien.TrangThai;
                    await _context.SaveChangesAsync();
                    
                    return Json(new { success = true, status = linhKien.TrangThai });
                }
                return Json(new { success = false, message = "Không tìm thấy linh kiện" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Cập nhật tồn kho
        [HttpPost]
        public async Task<IActionResult> UpdateStock(int id, int quantity, string action)
        {
            try
            {
                var linhKien = await _context.LinhKien.FindAsync(id);
                if (linhKien == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy linh kiện" });
                }

                switch (action.ToLower())
                {
                    case "set":
                        linhKien.SoLuongTon = quantity;
                        break;
                    case "add":
                        linhKien.SoLuongTon += quantity;
                        break;
                    case "subtract":
                        linhKien.SoLuongTon = Math.Max(0, linhKien.SoLuongTon - quantity);
                        break;
                    default:
                        return Json(new { success = false, message = "Hành động không hợp lệ" });
                }

                await _context.SaveChangesAsync();
                
                return Json(new { 
                    success = true, 
                    newStock = linhKien.SoLuongTon,
                    stockStatus = linhKien.TrangThaiTonKho,
                    stockClass = linhKien.TonKhoCssClass
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Báo cáo tồn kho thấp
        public async Task<IActionResult> LowStockAlert(int threshold = 10)
        {
            ViewData["Title"] = "Cảnh Báo Tồn Kho Thấp";
            ViewData["Threshold"] = threshold;

            var lowStockItems = await _context.LinhKien
                .Include(l => l.LoaiLinhKien)
                .Where(l => l.SoLuongTon <= threshold && l.TrangThai)
                .OrderBy(l => l.SoLuongTon)
                .ToListAsync();

            return View(lowStockItems);
        }

        // GET: Báo cáo tồn kho
        public async Task<IActionResult> StockReport()
        {
            ViewData["Title"] = "Báo Cáo Tồn Kho";

            var stockData = await _context.LinhKien
                .Include(l => l.LoaiLinhKien)
                .Where(l => l.TrangThai)
                .GroupBy(l => l.LoaiLinhKien!.TenLoaiLinhKien)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalItems = g.Count(),
                    TotalValue = g.Sum(l => l.SoLuongTon * l.GiaNhap),
                    OutOfStock = g.Count(l => l.SoLuongTon == 0),
                    LowStock = g.Count(l => l.SoLuongTon > 0 && l.SoLuongTon <= 5)
                })
                .ToListAsync();

            return View(stockData);
        }

        // API: Tìm kiếm AJAX
        [HttpGet]
        public async Task<IActionResult> SearchAjax(string term, int? categoryId = null)
        {
            var query = _context.LinhKien
                .Include(l => l.LoaiLinhKien)
                .Where(l => l.TrangThai && l.SoLuongTon > 0);

            if (!string.IsNullOrEmpty(term))
            {
                query = query.Where(l => l.TenLinhKien.Contains(term) || 
                                        (l.HangSanXuat != null && l.HangSanXuat.Contains(term)));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(l => l.MaLoaiLinhKien == categoryId.Value);
            }

            var linhKiens = await query
                .Select(l => new { 
                    id = l.MaLinhKien, 
                    text = l.TenLinhKien + (string.IsNullOrEmpty(l.HangSanXuat) ? "" : " - " + l.HangSanXuat),
                    price = l.GiaBan,
                    stock = l.SoLuongTon,
                    category = l.LoaiLinhKien!.TenLoaiLinhKien
                })
                .Take(10)
                .ToListAsync();

            return Json(linhKiens);
        }

        // API: Lấy theo loại
        [HttpGet]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var linhKiens = await _context.LinhKien
                .Include(l => l.LoaiLinhKien)
                .Where(l => l.MaLoaiLinhKien == categoryId && l.TrangThai && l.SoLuongTon > 0)
                .Select(l => new { 
                    id = l.MaLinhKien, 
                    name = l.TenLinhKien,
                    brand = l.HangSanXuat,
                    price = l.GiaBan,
                    stock = l.SoLuongTon
                })
                .OrderBy(l => l.name)
                .ToListAsync();

            return Json(linhKiens);
        }

        // Báo cáo lợi nhuận
        public async Task<IActionResult> ProfitReport()
        {
            ViewData["Title"] = "Báo Cáo Lợi Nhuận";

            var profitData = await _context.LinhKien
                .Include(l => l.LoaiLinhKien)
                .Where(l => l.TrangThai)
                .OrderByDescending(l => l.GiaBan - l.GiaNhap)
                .ToListAsync();

            return View(profitData);
        }

        // Helper Methods
        private bool LinhKienExists(int id)
        {
            return _context.LinhKien.Any(e => e.MaLinhKien == id);
        }

        private async Task LoadDropdownData()
        {
            ViewBag.LoaiLinhKienList = new SelectList(
                await _context.LoaiLinhKien
                    .Where(l => l.TrangThai)
                    .OrderBy(l => l.TenLoaiLinhKien)
                    .ToListAsync(), 
                "MaLoaiLinhKien", 
                "TenLoaiLinhKien"
            );
        }
    }
}