using Microsoft.AspNetCore.Mvc;
using phonev2.Models;
using phonev2.Services.LoaiLinhKien;

namespace phonev2.Controllers
{
    /// <summary>
    /// Controller quản lý loại linh kiện
    /// Sử dụng services để tách biệt logic
    /// </summary>
    public class LoaiLinhKienController : Controller
    {
        private readonly ILoaiLinhKienService _loaiLinhKienService;
        private readonly ILoaiLinhKienStatisticsService _statisticsService;

        public LoaiLinhKienController(
            ILoaiLinhKienService loaiLinhKienService,
            ILoaiLinhKienStatisticsService statisticsService)
        {
            _loaiLinhKienService = loaiLinhKienService;
            _statisticsService = statisticsService;
        }

        // Hiển thị danh sách loại linh kiện
        public async Task<IActionResult> Index(string searchString, string sortOrder, int page = 1, int pageSize = 10)
        {
            ViewData["Title"] = "Quản Lý Loại Linh Kiện";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["WarrantySortParm"] = sortOrder == "warranty" ? "warranty_desc" : "warranty";
            ViewData["DateSortParm"] = sortOrder == "date" ? "date_desc" : "date";

            // Lấy dữ liệu qua service
            var (items, totalCount, totalPages) = await _loaiLinhKienService.GetPagedAsync(searchString, sortOrder, page, pageSize);

            // Lấy thống kê
            var statistics = await _statisticsService.GetStatisticsAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;
            ViewBag.PageSize = pageSize;
            ViewBag.Statistics = statistics;

            return View(items);
        }

        // Hiển thị chi tiết loại linh kiện
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["Title"] = "Chi Tiết Loại Linh Kiện";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy loại linh kiện.";
                return RedirectToAction(nameof(Index));
            }

            var loaiLinhKien = await _loaiLinhKienService.GetByIdAsync(id.Value);
            
            if (loaiLinhKien == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy loại linh kiện.";
                return RedirectToAction(nameof(Index));
            }

            return View(loaiLinhKien);
        }

        // Form tạo mới
        public IActionResult Create()
        {
            ViewData["Title"] = "Thêm Loại Linh Kiện Mới";
            return View();
        }

        // Xử lý tạo mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenLoaiLinhKien,MoTa,ThoiGianBaoHanh,TrangThai")] phonev2.Models.LoaiLinhKien loaiLinhKien)
        {
            ViewData["Title"] = "Thêm Loại Linh Kiện Mới";

            if (ModelState.IsValid)
            {
                var success = await _loaiLinhKienService.CreateAsync(loaiLinhKien);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Đã thêm thành công loại linh kiện '{loaiLinhKien.TenLoaiLinhKien}'.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Kiểm tra lỗi cụ thể
                    if (await _loaiLinhKienService.IsNameExistsAsync(loaiLinhKien.TenLoaiLinhKien))
                    {
                        ModelState.AddModelError("TenLoaiLinhKien", "Tên loại linh kiện đã tồn tại.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Có lỗi xảy ra khi thêm loại linh kiện.");
                    }
                }
            }
            
            return View(loaiLinhKien);
        }

        // Form chỉnh sửa
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Title"] = "Sửa Thông Tin Loại Linh Kiện";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy loại linh kiện.";
                return RedirectToAction(nameof(Index));
            }

            var loaiLinhKien = await _loaiLinhKienService.GetByIdAsync(id.Value);
            if (loaiLinhKien == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy loại linh kiện.";
                return RedirectToAction(nameof(Index));
            }
            
            return View(loaiLinhKien);
        }

        // Xử lý chỉnh sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaLoaiLinhKien,TenLoaiLinhKien,MoTa,ThoiGianBaoHanh,NgayTao,TrangThai")] phonev2.Models.LoaiLinhKien loaiLinhKien)
        {
            ViewData["Title"] = "Sửa Thông Tin Loại Linh Kiện";

            if (id != loaiLinhKien.MaLoaiLinhKien)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                var success = await _loaiLinhKienService.UpdateAsync(loaiLinhKien);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Đã cập nhật thành công loại linh kiện '{loaiLinhKien.TenLoaiLinhKien}'.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Kiểm tra lỗi cụ thể
                    if (await _loaiLinhKienService.IsNameExistsAsync(loaiLinhKien.TenLoaiLinhKien, loaiLinhKien.MaLoaiLinhKien))
                    {
                        ModelState.AddModelError("TenLoaiLinhKien", "Tên loại linh kiện đã tồn tại.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật.");
                    }
                }
            }
            
            return View(loaiLinhKien);
        }

        // Form xác nhận xóa
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["Title"] = "Xóa Loại Linh Kiện";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy loại linh kiện.";
                return RedirectToAction(nameof(Index));
            }

            var loaiLinhKien = await _loaiLinhKienService.GetByIdAsync(id.Value);
                
            if (loaiLinhKien == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy loại linh kiện.";
                return RedirectToAction(nameof(Index));
            }

            return View(loaiLinhKien);
        }

        // Xử lý xóa
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _loaiLinhKienService.DeleteAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Đã xóa thành công loại linh kiện.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa loại linh kiện này vì đang được sử dụng bởi các linh kiện khác.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Bật/tắt trạng thái (AJAX)
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var success = await _loaiLinhKienService.ToggleStatusAsync(id);
            if (success)
            {
                var loaiLinhKien = await _loaiLinhKienService.GetByIdAsync(id);
                return Json(new { success = true, status = loaiLinhKien?.TrangThai });
            }
            return Json(new { success = false, message = "Không tìm thấy loại linh kiện" });
        }

        // Tìm kiếm AJAX
        [HttpGet]
        public async Task<IActionResult> SearchAjax(string term)
        {
            var results = await _loaiLinhKienService.SearchAjaxAsync(term);
            return Json(results);
        }
    }
}