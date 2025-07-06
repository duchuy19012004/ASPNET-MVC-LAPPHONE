using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using phonev2.Models;
using phonev2.Repository;

namespace phonev2.Controllers
{
    public class LoaiLinhKienController : Controller
    {
        private readonly PhoneLapDbContext _context;

        public LoaiLinhKienController(PhoneLapDbContext context)
        {
            _context = context;
        }

        // GET: LoaiLinhKien
        public async Task<IActionResult> Index(string searchString, string sortOrder, int page = 1, int pageSize = 10)
        {
            ViewData["Title"] = "Quản Lý Loại Linh Kiện";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["WarrantySortParm"] = sortOrder == "warranty" ? "warranty_desc" : "warranty";
            ViewData["DateSortParm"] = sortOrder == "date" ? "date_desc" : "date";

            var loaiLinhKiens = from l in _context.LoaiLinhKien select l;

            // Tìm kiếm
            if (!String.IsNullOrEmpty(searchString))
            {
                loaiLinhKiens = loaiLinhKiens.Where(l => l.TenLoaiLinhKien.Contains(searchString) 
                                                     || (!string.IsNullOrEmpty(l.MoTa) && l.MoTa.Contains(searchString)));
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "name_desc":
                    loaiLinhKiens = loaiLinhKiens.OrderByDescending(l => l.TenLoaiLinhKien);
                    break;
                case "warranty":
                    loaiLinhKiens = loaiLinhKiens.OrderBy(l => l.ThoiGianBaoHanh ?? 0);
                    break;
                case "warranty_desc":
                    loaiLinhKiens = loaiLinhKiens.OrderByDescending(l => l.ThoiGianBaoHanh ?? 0);
                    break;
                case "date":
                    loaiLinhKiens = loaiLinhKiens.OrderBy(l => l.NgayTao);
                    break;
                case "date_desc":
                    loaiLinhKiens = loaiLinhKiens.OrderByDescending(l => l.NgayTao);
                    break;
                default:
                    loaiLinhKiens = loaiLinhKiens.OrderBy(l => l.TenLoaiLinhKien);
                    break;
            }

            // Phân trang
            var totalCount = await loaiLinhKiens.CountAsync();
            var items = await loaiLinhKiens.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.TotalCount = totalCount;
            ViewBag.PageSize = pageSize;

            return View(items);
        }

        // GET: LoaiLinhKien/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["Title"] = "Chi Tiết Loại Linh Kiện";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy loại linh kiện.";
                return RedirectToAction(nameof(Index));
            }

            var loaiLinhKien = await _context.LoaiLinhKien
                .FirstOrDefaultAsync(m => m.MaLoaiLinhKien == id);
            
            if (loaiLinhKien == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy loại linh kiện.";
                return RedirectToAction(nameof(Index));
            }

            return View(loaiLinhKien);
        }

        // GET: LoaiLinhKien/Create
        public IActionResult Create()
        {
            ViewData["Title"] = "Thêm Loại Linh Kiện Mới";
            return View();
        }

        // POST: LoaiLinhKien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenLoaiLinhKien,MoTa,ThoiGianBaoHanh,TrangThai")] LoaiLinhKien loaiLinhKien)
        {
            ViewData["Title"] = "Thêm Loại Linh Kiện Mới";

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra trùng tên
                    var existingType = await _context.LoaiLinhKien
                        .AnyAsync(l => l.TenLoaiLinhKien.ToLower() == loaiLinhKien.TenLoaiLinhKien.ToLower());
                    
                    if (existingType)
                    {
                        ModelState.AddModelError("TenLoaiLinhKien", "Tên loại linh kiện đã tồn tại.");
                        return View(loaiLinhKien);
                    }

                    loaiLinhKien.NgayTao = DateTime.Now;
                    _context.Add(loaiLinhKien);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Đã thêm thành công loại linh kiện '{loaiLinhKien.TenLoaiLinhKien}'.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi thêm loại linh kiện: " + ex.Message);
                }
            }
            
            return View(loaiLinhKien);
        }

        // GET: LoaiLinhKien/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Title"] = "Sửa Thông Tin Loại Linh Kiện";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy loại linh kiện.";
                return RedirectToAction(nameof(Index));
            }

            var loaiLinhKien = await _context.LoaiLinhKien.FindAsync(id);
            if (loaiLinhKien == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy loại linh kiện.";
                return RedirectToAction(nameof(Index));
            }
            
            return View(loaiLinhKien);
        }

        // POST: LoaiLinhKien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaLoaiLinhKien,TenLoaiLinhKien,MoTa,ThoiGianBaoHanh,NgayTao,TrangThai")] LoaiLinhKien loaiLinhKien)
        {
            ViewData["Title"] = "Sửa Thông Tin Loại Linh Kiện";

            if (id != loaiLinhKien.MaLoaiLinhKien)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra trùng tên (trừ chính nó)
                    var existingType = await _context.LoaiLinhKien
                        .AnyAsync(l => l.TenLoaiLinhKien.ToLower() == loaiLinhKien.TenLoaiLinhKien.ToLower() 
                                    && l.MaLoaiLinhKien != loaiLinhKien.MaLoaiLinhKien);
                    
                    if (existingType)
                    {
                        ModelState.AddModelError("TenLoaiLinhKien", "Tên loại linh kiện đã tồn tại.");
                        return View(loaiLinhKien);
                    }

                    _context.Update(loaiLinhKien);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Đã cập nhật thành công loại linh kiện '{loaiLinhKien.TenLoaiLinhKien}'.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiLinhKienExists(loaiLinhKien.MaLoaiLinhKien))
                    {
                        TempData["ErrorMessage"] = "Loại linh kiện không tồn tại.";
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
            
            return View(loaiLinhKien);
        }

        // GET: LoaiLinhKien/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["Title"] = "Xóa Loại Linh Kiện";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy loại linh kiện.";
                return RedirectToAction(nameof(Index));
            }

            var loaiLinhKien = await _context.LoaiLinhKien
                .FirstOrDefaultAsync(m => m.MaLoaiLinhKien == id);
                
            if (loaiLinhKien == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy loại linh kiện.";
                return RedirectToAction(nameof(Index));
            }

            return View(loaiLinhKien);
        }

        // POST: LoaiLinhKien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var loaiLinhKien = await _context.LoaiLinhKien.FindAsync(id);
                if (loaiLinhKien != null)
                {
                    _context.LoaiLinhKien.Remove(loaiLinhKien);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Đã xóa thành công loại linh kiện '{loaiLinhKien.TenLoaiLinhKien}'.";
                }
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Không thể xóa loại linh kiện này vì đang được sử dụng bởi các linh kiện khác.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // Toggle trạng thái
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var loaiLinhKien = await _context.LoaiLinhKien.FindAsync(id);
                if (loaiLinhKien != null)
                {
                    loaiLinhKien.TrangThai = !loaiLinhKien.TrangThai;
                    await _context.SaveChangesAsync();
                    
                    return Json(new { success = true, status = loaiLinhKien.TrangThai });
                }
                return Json(new { success = false, message = "Không tìm thấy loại linh kiện" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private bool LoaiLinhKienExists(int id)
        {
            return _context.LoaiLinhKien.Any(e => e.MaLoaiLinhKien == id);
        }

        // API cho AJAX search (tùy chọn)
        [HttpGet]
        public async Task<IActionResult> SearchAjax(string term)
        {
            var loaiLinhKiens = await _context.LoaiLinhKien
                .Where(l => l.TenLoaiLinhKien.Contains(term) && l.TrangThai)
                .Select(l => new { 
                    id = l.MaLoaiLinhKien, 
                    text = l.TenLoaiLinhKien,
                    warranty = l.ThoiGianBaoHanhText
                })
                .Take(10)
                .ToListAsync();

            return Json(loaiLinhKiens);
        }
    }
}