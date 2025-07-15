using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using phonev2.Models;
using phonev2.Repository;

namespace phonev2.Controllers
{
    public class DichVuController : Controller
    {
        private readonly PhoneLapDbContext _context;

        public DichVuController(PhoneLapDbContext context)
        {
            _context = context;
        }

        // GET: DichVu
        public async Task<IActionResult> Index(string searchString, string sortOrder, int page = 1, int pageSize = 10)
        {
            ViewData["Title"] = "Quản Lý Dịch Vụ";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "price" ? "price_desc" : "price";
            ViewData["DateSortParm"] = sortOrder == "date" ? "date_desc" : "date";

            var dichVus = from d in _context.DichVu select d;

            // Tìm kiếm
            if (!String.IsNullOrEmpty(searchString))
            {
                dichVus = dichVus.Where(d => d.TenDichVu.Contains(searchString) 
                                         || (!string.IsNullOrEmpty(d.MoTa) && d.MoTa.Contains(searchString)));
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "name_desc":
                    dichVus = dichVus.OrderByDescending(d => d.TenDichVu);
                    break;
                case "price":
                    dichVus = dichVus.OrderBy(d => d.GiaDichVu);
                    break;
                case "price_desc":
                    dichVus = dichVus.OrderByDescending(d => d.GiaDichVu);
                    break;
                case "date":
                    dichVus = dichVus.OrderBy(d => d.NgayTao);
                    break;
                case "date_desc":
                    dichVus = dichVus.OrderByDescending(d => d.NgayTao);
                    break;
                default:
                    dichVus = dichVus.OrderBy(d => d.TenDichVu);
                    break;
            }

            // Phân trang
            var totalCount = await dichVus.CountAsync();
            var items = await dichVus.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.TotalCount = totalCount;
            ViewBag.PageSize = pageSize;

            return View(items);
        }

        // GET: DichVu/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["Title"] = "Chi Tiết Dịch Vụ";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy dịch vụ.";
                return RedirectToAction(nameof(Index));
            }

            var dichVu = await _context.DichVu
                .FirstOrDefaultAsync(m => m.MaDichVu == id);
            
            if (dichVu == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy dịch vụ.";
                return RedirectToAction(nameof(Index));
            }

            return View(dichVu);
        }

        // GET: DichVu/Create
        public IActionResult Create()
        {
            ViewData["Title"] = "Thêm Dịch Vụ Mới";
            return View();
        }

        // POST: DichVu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenDichVu,MoTa,GiaDichVu,ThoiGianSua,TrangThai")] DichVu dichVu)
        {
            ViewData["Title"] = "Thêm Dịch Vụ Mới";

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra trùng tên
                    var existingService = await _context.DichVu
                        .AnyAsync(d => d.TenDichVu.ToLower() == dichVu.TenDichVu.ToLower());
                    
                    if (existingService)
                    {
                        ModelState.AddModelError("TenDichVu", "Tên dịch vụ đã tồn tại.");
                        return View(dichVu);
                    }

                    dichVu.NgayTao = DateTime.Now;
                    _context.Add(dichVu);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Đã thêm thành công dịch vụ '{dichVu.TenDichVu}'.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi thêm dịch vụ: " + ex.Message);
                }
            }
            
            return View(dichVu);
        }

        // GET: DichVu/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Title"] = "Sửa Thông Tin Dịch Vụ";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy dịch vụ.";
                return RedirectToAction(nameof(Index));
            }

            var dichVu = await _context.DichVu.FindAsync(id);
            if (dichVu == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy dịch vụ.";
                return RedirectToAction(nameof(Index));
            }
            
            return View(dichVu);
        }

        // POST: DichVu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaDichVu,TenDichVu,MoTa,GiaDichVu,ThoiGianSua,NgayTao,TrangThai")] DichVu dichVu)
        {
            ViewData["Title"] = "Sửa Thông Tin Dịch Vụ";

            if (id != dichVu.MaDichVu)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra trùng tên (trừ chính nó)
                    var existingService = await _context.DichVu
                        .AnyAsync(d => d.TenDichVu.ToLower() == dichVu.TenDichVu.ToLower() 
                                    && d.MaDichVu != dichVu.MaDichVu);
                    
                    if (existingService)
                    {
                        ModelState.AddModelError("TenDichVu", "Tên dịch vụ đã tồn tại.");
                        return View(dichVu);
                    }

                    _context.Update(dichVu);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Đã cập nhật thành công dịch vụ '{dichVu.TenDichVu}'.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DichVuExists(dichVu.MaDichVu))
                    {
                        TempData["ErrorMessage"] = "Dịch vụ không tồn tại.";
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
            
            return View(dichVu);
        }



        // GET: DichVu/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["Title"] = "Xóa Dịch Vụ";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy dịch vụ.";
                return RedirectToAction(nameof(Index));
            }

            var dichVu = await _context.DichVu
                .FirstOrDefaultAsync(m => m.MaDichVu == id);
                
            if (dichVu == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy dịch vụ.";
                return RedirectToAction(nameof(Index));
            }

            return View(dichVu);
        }

        // POST: DichVu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var dichVu = await _context.DichVu.FindAsync(id);
                if (dichVu != null)
                {
                    _context.DichVu.Remove(dichVu);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Đã xóa thành công dịch vụ '{dichVu.TenDichVu}'.";
                }
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Không thể xóa dịch vụ này vì đang được sử dụng trong các phiếu sửa chữa.";
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
                var dichVu = await _context.DichVu.FindAsync(id);
                if (dichVu != null)
                {
                    dichVu.TrangThai = !dichVu.TrangThai;
                    await _context.SaveChangesAsync();
                    
                    return Json(new { success = true, status = dichVu.TrangThai });
                }
                return Json(new { success = false, message = "Không tìm thấy dịch vụ" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private bool DichVuExists(int id)
        {
            return _context.DichVu.Any(e => e.MaDichVu == id);
        }
    }
}