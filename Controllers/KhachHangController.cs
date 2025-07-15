using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using phonev2.Models;
using phonev2.Services.KhachHang;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;

namespace phonev2.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly IKhachHangService _khachHangService;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        public KhachHangController(IKhachHangService khachHangService, ICompositeViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
        {
            _khachHangService = khachHangService;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        // GET: KhachHang
        public async Task<IActionResult> Index(string searchString, string customerLevelFilter, string statusFilter, string sortOrder, int page = 1, int pageSize = 10)
        {
            ViewData["Title"] = "Quản Lý Khách Hàng";
            var (result, totalItems, totalPages) = await _khachHangService.GetPagedAsync(searchString, customerLevelFilter, statusFilter, sortOrder, page, pageSize);
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentLevel = customerLevelFilter;
            ViewBag.CurrentStatus = statusFilter;
            ViewBag.CurrentSort = sortOrder;
            return View(result);
        }

        // GET: KhachHang/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["Title"] = "Chi Tiết Khách Hàng";
            if (id == null) return RedirectToAction(nameof(Index));
            var khachHang = await _khachHangService.GetByIdAsync(id.Value);
            if (khachHang == null) return RedirectToAction(nameof(Index));
            return View(khachHang);
        }

        // GET: KhachHang/Create
        public IActionResult Create()
        {
            ViewData["Title"] = "Thêm Khách Hàng Mới";
            var khachHang = new KhachHang { NgayTao = DateTime.Now, TrangThai = true, TongChiTieu = 0 };
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("Create", khachHang);
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
                var success = await _khachHangService.CreateAsync(khachHang);
                if (success)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = true, id = khachHang.MaKhachHang });
                    TempData["SuccessMessage"] = $"Đã thêm thành công khách hàng '{khachHang.HoTen}'.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng bởi khách hàng khác.");
            }
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var html = await this.RenderViewAsync("Create", khachHang, true);
                return Json(new { success = false, html });
            }
            return View(khachHang);
        }

        // GET: KhachHang/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Title"] = "Sửa Thông Tin Khách Hàng";
            if (id == null) return RedirectToAction(nameof(Index));
            var khachHang = await _khachHangService.GetByIdAsync(id.Value);
            if (khachHang == null) return RedirectToAction(nameof(Index));
            return View(khachHang);
        }

        // POST: KhachHang/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaKhachHang,HoTen,SoDienThoai,DiaChi,NgayTao,TongChiTieu,TrangThai")] KhachHang khachHang)
        {
            ViewData["Title"] = "Sửa Thông Tin Khách Hàng";
            if (id != khachHang.MaKhachHang) return RedirectToAction(nameof(Index));
            if (ModelState.IsValid)
            {
                var success = await _khachHangService.UpdateAsync(khachHang);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Đã cập nhật thành công khách hàng '{khachHang.HoTen}'.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng bởi khách hàng khác.");
            }
            return View(khachHang);
        }

        // GET: KhachHang/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["Title"] = "Xóa Khách Hàng";
            if (id == null) return RedirectToAction(nameof(Index));
            var khachHang = await _khachHangService.GetByIdAsync(id.Value);
            if (khachHang == null) return RedirectToAction(nameof(Index));
            return View(khachHang);
        }

        // POST: KhachHang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _khachHangService.DeleteAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Đã xóa thành công khách hàng.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa khách hàng này vì đang có giao dịch liên quan.";
            }
            return RedirectToAction(nameof(Index));
        }

        // AJAX: Toggle Status
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var success = await _khachHangService.ToggleStatusAsync(id);
            if (success)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Không tìm thấy khách hàng" });
        }

        // AJAX: Update Spending
        [HttpPost]
        public async Task<IActionResult> UpdateSpending(int id, decimal amount, string action = "add")
        {
            var success = await _khachHangService.UpdateSpendingAsync(id, amount, action);
            if (success)
            {
                var khachHang = await _khachHangService.GetByIdAsync(id);
                return Json(new { success = true, newAmount = khachHang?.TongChiTieu });
            }
            return Json(new { success = false, message = "Không tìm thấy khách hàng" });
        }

        // Statistics
        public async Task<IActionResult> GetStatistics()
        {
            var data = await _khachHangService.GetStatisticsAsync();
            return Json(new { success = true, data });
        }

        // API: Lấy danh sách khách hàng đang hoạt động cho dropdown
        [HttpGet]
        public async Task<IActionResult> GetActiveCustomers()
        {
            var khs = await _khachHangService.GetActiveCustomersForDropdownAsync();
            return Json(khs);
        }

        // Helper để render view thành HTML string cho AJAX
        private async Task<string> RenderViewAsync(string viewName, object model, bool partial = false)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, !partial);
                if (!viewResult.Success)
                {
                    throw new InvalidOperationException($"View {viewName} not found");
                }
                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    sw,
                    new HtmlHelperOptions()
                );
                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }
    }
}