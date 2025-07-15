using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using phonev2.Models;
using phonev2.Services.LinhKien;

namespace phonev2.Controllers
{
    public class LinhKienController : Controller
    {
        private readonly ILinhKienService _linhKienService;
        private readonly ILinhKienValidationService _validationService;
        private readonly ILinhKienStatisticsService _statisticsService;

        public LinhKienController(
            ILinhKienService linhKienService,
            ILinhKienValidationService validationService,
            ILinhKienStatisticsService statisticsService)
        {
            _linhKienService = linhKienService;
            _validationService = validationService;
            _statisticsService = statisticsService;
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

            var (items, totalCount, totalPages) = await _linhKienService.GetPagedAsync(
                searchString, categoryFilter, stockFilter, sortOrder, page, pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;
            ViewBag.PageSize = pageSize;

            // Dropdown data
            var loaiLinhKienList = await _linhKienService.GetLoaiLinhKienForDropdownAsync();
            ViewBag.LoaiLinhKienList = new SelectList(loaiLinhKienList, "MaLoaiLinhKien", "TenLoaiLinhKien");

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

            var linhKien = await _linhKienService.GetByIdAsync(id.Value);
            
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

            // Validation
            var (isValid, errors) = await _validationService.ValidateForCreateAsync(linhKien);
            if (!isValid)
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError("", error);
                }
                await LoadDropdownData();
                return View(linhKien);
            }

            if (ModelState.IsValid)
            {
                var success = await _linhKienService.CreateAsync(linhKien);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Đã thêm thành công linh kiện '{linhKien.TenLinhKien}'.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi thêm linh kiện.");
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

            var linhKien = await _linhKienService.GetByIdAsync(id.Value);
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

            // Validation
            var (isValid, errors) = await _validationService.ValidateForUpdateAsync(linhKien);
            if (!isValid)
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError("", error);
                }
                await LoadDropdownData();
                return View(linhKien);
            }

            if (ModelState.IsValid)
            {
                var success = await _linhKienService.UpdateAsync(linhKien);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Đã cập nhật thành công linh kiện '{linhKien.TenLinhKien}'.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật.");
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

            var linhKien = await _linhKienService.GetByIdAsync(id.Value);
                
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
                // Kiểm tra có thể xóa không
                var (canDelete, error) = await _validationService.CanDeleteAsync(id);
                if (!canDelete)
                {
                    TempData["ErrorMessage"] = error;
                    return RedirectToAction(nameof(Index));
                }

                var success = await _linhKienService.DeleteAsync(id);
                if (success)
                {
                    var linhKien = await _linhKienService.GetByIdAsync(id);
                    TempData["SuccessMessage"] = $"Đã xóa thành công linh kiện '{linhKien?.TenLinhKien}'.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa linh kiện.";
                }
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
                var success = await _linhKienService.ToggleStatusAsync(id);
                if (success)
                {
                    var linhKien = await _linhKienService.GetByIdAsync(id);
                    return Json(new { success = true, status = linhKien?.TrangThai });
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
                var success = await _linhKienService.UpdateStockAsync(id, quantity, action);
                if (success)
                {
                    var linhKien = await _linhKienService.GetByIdAsync(id);
                    if (linhKien != null)
                    {
                        return Json(new { 
                            success = true, 
                            newStock = linhKien.SoLuongTon,
                            stockStatus = linhKien.TrangThaiTonKho,
                            stockClass = linhKien.TonKhoCssClass
                        });
                    }
                }
                return Json(new { success = false, message = "Không tìm thấy linh kiện" });
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

            var lowStockItems = await _statisticsService.GetLowStockItemsAsync(threshold);
            return View(lowStockItems);
        }

        // GET: Báo cáo tồn kho
        public async Task<IActionResult> StockReport()
        {
            ViewData["Title"] = "Báo Cáo Tồn Kho";

            var stockData = await _statisticsService.GetStockReportByCategoryAsync();
            return View(stockData);
        }

        // API: Tìm kiếm AJAX
        [HttpGet]
        public async Task<IActionResult> SearchAjax(string term, int? categoryId = null)
        {
            var linhKiens = await _linhKienService.SearchAjaxAsync(term, categoryId);
            return Json(linhKiens);
        }

        // API: Lấy theo loại
        [HttpGet]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var linhKiens = await _linhKienService.GetByCategoryAsync(categoryId);
            return Json(linhKiens);
        }

        // Báo cáo lợi nhuận
        public async Task<IActionResult> ProfitReport()
        {
            ViewData["Title"] = "Báo Cáo Lợi Nhuận";

            var profitData = await _statisticsService.GetProfitReportAsync();
            return View(profitData);
        }

        // Helper Methods
        private async Task LoadDropdownData()
        {
            var loaiLinhKienList = await _linhKienService.GetLoaiLinhKienForDropdownAsync();
            ViewBag.LoaiLinhKienList = new SelectList(loaiLinhKienList, "MaLoaiLinhKien", "TenLoaiLinhKien");
        }
    }
}