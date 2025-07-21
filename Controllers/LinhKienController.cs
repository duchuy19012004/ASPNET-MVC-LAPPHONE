using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

        // GET: LinhKien/Deleted - Danh sách linh kiện đã xóa
        public async Task<IActionResult> Deleted()
        {
            ViewData["Title"] = "Linh Kiện Đã Xóa";
            var deletedItems = await _linhKienService.GetDeletedAsync();
            return View(deletedItems);
        }

        // GET: LinhKien/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Không tìm thấy linh kiện." });
            }

            var linhKien = await _linhKienService.GetByIdAsync(id.Value);
            
            if (linhKien == null)
            {
                return Json(new { success = false, message = "Không tìm thấy linh kiện." });
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_DetailsModal", linhKien);
            }

            ViewData["Title"] = "Chi Tiết Linh Kiện";
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
            if (id == null)
            {
                return Json(new { success = false, message = "Không tìm thấy linh kiện." });
            }

            var linhKien = await _linhKienService.GetByIdAsync(id.Value);
            if (linhKien == null)
            {
                return Json(new { success = false, message = "Không tìm thấy linh kiện." });
            }
            
            await LoadDropdownData();
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_EditModal", linhKien);
            }

            ViewData["Title"] = "Sửa Thông Tin Linh Kiện";
            return View(linhKien);
        }

        // POST: LinhKien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaLinhKien,TenLinhKien,MaLoaiLinhKien,HangSanXuat,GiaNhap,GiaBan,SoLuongTon,ThongSoKyThuat,NgayTao,TrangThai")] LinhKien linhKien)
        {
            try
            {
                if (id != linhKien.MaLinhKien)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
                }

                // Validation
                var (isValid, errors) = await _validationService.ValidateForUpdateAsync(linhKien);
                if (!isValid)
                {
                    return Json(new { success = false, message = string.Join(", ", errors) });
                }

                if (ModelState.IsValid)
                {
                    var success = await _linhKienService.UpdateAsync(linhKien);
                    if (success)
                    {
                        return Json(new { success = true, message = $"Đã cập nhật thành công linh kiện '{linhKien.TenLinhKien}'." });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật." });
                    }
                }
                
                var modelErrors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return Json(new { success = false, message = string.Join(", ", modelErrors) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // GET: LinhKien/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Không tìm thấy linh kiện." });
            }

            var linhKien = await _linhKienService.GetByIdAsync(id.Value);
                
            if (linhKien == null)
            {
                return Json(new { success = false, message = "Không tìm thấy linh kiện." });
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_DeleteModal", linhKien);
            }

            ViewData["Title"] = "Xóa Linh Kiện";
            return View(linhKien);
        }

        // POST: LinhKien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? lyDoXoa = "")
        {
            try
            {
                // Kiểm tra có thể xóa không
                var (canDelete, error) = await _validationService.CanDeleteAsync(id);
                if (!canDelete)
                {
                    return Json(new { success = false, message = error });
                }

                var success = await _linhKienService.DeleteAsync(id, lyDoXoa);
                if (success)
                {
                    var linhKien = await _linhKienService.GetByIdIncludeDeletedAsync(id);
                    return Json(new { success = true, message = $"Đã xóa thành công linh kiện '{linhKien?.TenLinhKien}'. Linh kiện vẫn được giữ lại trong các phiếu sửa để đảm bảo tính toàn vẹn dữ liệu." });
                }
                else
                {
                    return Json(new { success = false, message = "Có lỗi xảy ra khi xóa linh kiện." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa: " + ex.Message });
            }
        }

        // POST: Khôi phục linh kiện đã xóa
        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                var success = await _linhKienService.RestoreAsync(id);
                if (success)
                {
                    var linhKien = await _linhKienService.GetByIdAsync(id);
                    return Json(new { success = true, message = $"Đã khôi phục thành công linh kiện '{linhKien?.TenLinhKien}'." });
                }
                return Json(new { success = false, message = "Không thể khôi phục linh kiện này" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Xóa thực sự linh kiện
        [HttpPost]
        public async Task<IActionResult> HardDelete(int id)
        {
            try
            {
                var success = await _linhKienService.HardDeleteAsync(id);
                if (success)
                {
                    return Json(new { success = true, message = "Đã xóa vĩnh viễn linh kiện khỏi hệ thống." });
                }
                return Json(new { success = false, message = "Không thể xóa linh kiện này" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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
        public async Task<IActionResult> StockReport(string search, string category, string brand, string from, string to, string stock)
        {
            ViewData["Title"] = "Báo Cáo Tồn Kho";

            // Lấy danh sách loại linh kiện cho dropdown
            var loaiLinhKienList = await _linhKienService.GetLoaiLinhKienForDropdownAsync();
            ViewBag.LoaiLinhKienList = loaiLinhKienList.Select(l => new SelectListItem { Text = l.TenLoaiLinhKien, Value = l.MaLoaiLinhKien.ToString() }).ToList();

            // Lấy danh sách hãng sản xuất (distinct)
            var allLinhKien = await _linhKienService.GetAllAsync();
            ViewBag.BrandList = allLinhKien
                .Where(lk => !string.IsNullOrEmpty(lk.HangSanXuat))
                .Select(lk => lk.HangSanXuat)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            // Lấy danh sách tên linh kiện (distinct)
            ViewBag.NameList = allLinhKien
                .Select(lk => lk.TenLinhKien)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            DateTime? fromDate = null, toDate = null;
            if (DateTime.TryParse(from, out var f)) fromDate = f;
            if (DateTime.TryParse(to, out var t)) toDate = t;

            // Lấy dữ liệu báo cáo tồn kho cho chart
            var stockData = await _statisticsService.GetStockReportForChartsAsync(
                search, category, brand, fromDate, toDate, stock);

            // Nếu không có dữ liệu, trả về bản ghi mẫu để chart không lỗi
            if (stockData == null || !stockData.Any())
            {
                stockData = new List<object>
                {
                    new {
                        name = "Không có dữ liệu",
                        category = "Không có dữ liệu",
                        totalStock = 0,
                        stockHistory = new List<int> { 0, 0, 0, 0, 0, 0 },
                        totalImported = 0,
                        totalUsed = 0
                    }
                };
            }

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