using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using phonev2.Models;
using phonev2.Models.ViewModels;
using phonev2.Repository;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using phonev2.Services.PhieuSua;

namespace phonev2.Controllers
{
    public class PhieuSuaController : Controller
    {
        private readonly PhoneLapDbContext _context;
        private readonly IPhieuSuaService _phieuSuaService;
        private readonly IPhieuSuaStatisticsService _phieuSuaStatisticsService;

        public PhieuSuaController(
            PhoneLapDbContext context,
            IPhieuSuaService phieuSuaService,
            IPhieuSuaStatisticsService phieuSuaStatisticsService)
        {
            _context = context;
            _phieuSuaService = phieuSuaService;
            _phieuSuaStatisticsService = phieuSuaStatisticsService;
        }

        // GET: PhieuSua
        public async Task<IActionResult> Index(string search = "", int page = 1, int pageSize = 10, string sort = "")
        {
            // Lấy danh sách phiếu sửa và phân trang qua service
            var (list, totalCount, totalPages) = await _phieuSuaService.GetPhieuSuasAsync(search, page, pageSize, sort);

            // Lấy thống kê qua service
            var statistics = await _phieuSuaStatisticsService.GetStatisticsAsync();
            var thongKeTheoThang = await _phieuSuaStatisticsService.GetMonthlyStatisticsAsync();
            var thongKeTienTheoThang = await _phieuSuaStatisticsService.GetMonthlyRevenueStatisticsAsync();

            // Set ViewBag
            ViewBag.TotalPhieuSua = statistics.TotalPhieuSua;
            ViewBag.TiepNhan = statistics.TiepNhan;
            ViewBag.DangSua = statistics.DangSua;
            ViewBag.HoanThanh = statistics.HoanThanh;
            ViewBag.Huy = statistics.Huy;
            ViewBag.MoiTrong30Ngay = statistics.MoiTrong30Ngay;
            ViewBag.TongTienPhieuSua = statistics.TongTienPhieuSua;
            ViewBag.ThongKeTheoThang = thongKeTheoThang;
            ViewBag.ThongKeTienTheoThang = thongKeTienTheoThang;

            ViewBag.KhachHangList = _phieuSuaService.GetKhachHangList();
            ViewBag.NhanVienList = _phieuSuaService.GetNhanVienList();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.Search = search;
            ViewBag.Sort = sort;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_PhieuSuaTable", list);
            }
            return View(list);
        }

        // Method riêng để xử lý sắp xếp theo giá
        private async Task<IActionResult> HandlePriceSorting(IQueryable<PhieuSua> query, string search, int page, int pageSize, string sort)
        {
            // Tính thống kê cho toàn bộ dữ liệu (không áp dụng filter)
            var allPhieuSua = _context.PhieuSua.AsQueryable();
            var totalPhieuSua = await allPhieuSua.CountAsync();
            var tiepNhan = await allPhieuSua.CountAsync(p => p.TrangThai == TrangThaiPhieuSua.TiepNhan);
            var dangSua = await allPhieuSua.CountAsync(p => p.TrangThai == TrangThaiPhieuSua.DangSua);
            var hoanThanh = await allPhieuSua.CountAsync(p => p.TrangThai == TrangThaiPhieuSua.HoanThanh);
            var huy = await allPhieuSua.CountAsync(p => p.TrangThai == TrangThaiPhieuSua.Huy);
            var moiTrong30Ngay = await allPhieuSua.CountAsync(p => p.NgaySua >= DateTime.Now.AddDays(-30));

            // Tính tổng tiền từ phiếu sửa (chỉ tính khi hoàn thành)
            var tongTienPhieuSua = await allPhieuSua
                .Where(p => p.TrangThai == TrangThaiPhieuSua.HoanThanh)
                .SumAsync(p => p.TongTien ?? 0);

            // Thống kê theo tháng trong 12 tháng gần nhất
            var thongKeTheoThang = new List<object>();
            var thongKeTienTheoThang = new List<object>();
            
            for (int i = 11; i >= 0; i--)
            {
                var thang = DateTime.Now.AddMonths(-i);
                var startOfMonth = new DateTime(thang.Year, thang.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
                
                var soPhieuTrongThang = await allPhieuSua.CountAsync(p => 
                    p.NgaySua >= startOfMonth && p.NgaySua <= endOfMonth);
                
                var tongTienTrongThang = await allPhieuSua
                    .Where(p => p.TrangThai == TrangThaiPhieuSua.HoanThanh && 
                               p.NgaySua >= startOfMonth && p.NgaySua <= endOfMonth)
                    .SumAsync(p => p.TongTien ?? 0);
                
                thongKeTheoThang.Add(new { 
                    thang = thang.ToString("MM/yyyy"), 
                    soPhieu = soPhieuTrongThang 
                });
                
                thongKeTienTheoThang.Add(new { 
                    thang = thang.ToString("MM/yyyy"), 
                    tongTien = tongTienTrongThang 
                });
            }

            // Set thống kê vào ViewBag
            ViewBag.TotalPhieuSua = totalPhieuSua;
            ViewBag.TiepNhan = tiepNhan;
            ViewBag.DangSua = dangSua;
            ViewBag.HoanThanh = hoanThanh;
            ViewBag.Huy = huy;
            ViewBag.MoiTrong30Ngay = moiTrong30Ngay;
            ViewBag.TongTienPhieuSua = tongTienPhieuSua;
            ViewBag.ThongKeTheoThang = thongKeTheoThang;
            ViewBag.ThongKeTienTheoThang = thongKeTienTheoThang;

            // Lấy dữ liệu trước, sau đó sắp xếp trong memory
            int count = await query.CountAsync();
            int pages = (int)Math.Ceiling(count / (double)pageSize);
            var allData = await query.ToListAsync();
            
            IEnumerable<PhieuSua> sortedData;
            if (sort == "price_asc")
            {
                sortedData = allData.OrderBy(p => 
                    (p.ChiTietPhieuSuas?.Sum(ct => ct.DichVu?.GiaDichVu ?? 0) ?? 0) +
                    (p.ChiTietLinhKiens?.Sum(lk => (lk.LinhKien?.GiaBan ?? 0) * lk.SoLuong) ?? 0)
                );
            }
            else // price_desc
            {
                sortedData = allData.OrderByDescending(p => 
                    (p.ChiTietPhieuSuas?.Sum(ct => ct.DichVu?.GiaDichVu ?? 0) ?? 0) +
                    (p.ChiTietLinhKiens?.Sum(lk => (lk.LinhKien?.GiaBan ?? 0) * lk.SoLuong) ?? 0)
                );
            }
            
            var pagedData = sortedData.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            // Set ViewBag
            ViewBag.KhachHangList = _context.KhachHang.Select(kh => new SelectListItem { Value = kh.MaKhachHang.ToString(), Text = kh.HoTen }).ToList();
            ViewBag.NhanVienList = _context.NhanVien.Select(nv => new SelectListItem { Value = nv.MaNhanVien.ToString(), Text = nv.HoTen }).ToList();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = pages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = count;
            ViewBag.Search = search;
            ViewBag.Sort = sort;
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_PhieuSuaTable", pagedData);
            }
            return View(pagedData);
        }

        // GET: PhieuSua/Create
        public async Task<IActionResult> Create()
        {
            var vm = await _phieuSuaService.GetCreateViewModelAsync();
            return View(vm);
        }

        // POST: PhieuSua/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhieuSuaCreateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var success = await _phieuSuaService.CreatePhieuSuaAsync(vm);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            // Nếu lỗi, nạp lại ViewModel
            vm = await _phieuSuaService.GetCreateViewModelAsync();
            return View(vm);
        }

        // GET: PhieuSua/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var vm = await _phieuSuaService.GetPhieuSuaDetailsViewModelAsync(id.Value);
            if (vm == null) return NotFound();
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("Details", vm);
            }
            return View(vm);
        }

        // GET: PhieuSua/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var vm = await _phieuSuaService.GetPhieuSuaEditViewModelAsync(id.Value);
            if (vm == null) return NotFound();
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("Edit", vm);
            }
            return View(vm);
        }

        // POST: PhieuSua/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PhieuSua phieuSua)
        {
            if (id != phieuSua.MaPhieuSua) 
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "ID không hợp lệ!" });
                }
                return NotFound();
            }
            
            // Validation cho ngày hẹn trả
            if (phieuSua.NgayHenTra.HasValue && phieuSua.NgayHenTra.Value < phieuSua.NgaySua)
            {
                ModelState.AddModelError("NgayHenTra", "Ngày hẹn trả phải lớn hơn hoặc bằng ngày sửa");
            }
            
            if (ModelState.IsValid)
            {
                var success = await _phieuSuaService.UpdatePhieuSuaAsync(phieuSua);
                if (success)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Cập nhật phiếu sửa thành công!" });
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật!" });
                    }
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật phiếu sửa!");
                }
            }
            
            // Validation errors
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            var errorMessage = "Dữ liệu không hợp lệ: " + string.Join(", ", errors);
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = errorMessage });
            }
            
            // For non-AJAX requests, reload ViewBag and return view
            ViewBag.KhachHangList = _phieuSuaService.GetKhachHangList();
            ViewBag.NhanVienList = _phieuSuaService.GetNhanVienList();
            return View(phieuSua);
        }

        // GET: PhieuSua/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var phieuSua = await _context.PhieuSua.FindAsync(id);
            if (phieuSua == null) return NotFound();
            
            // Nếu là request AJAX (modal), trả về PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("Delete", phieuSua);
            
            // Nếu truy cập trực tiếp, trả về View đầy đủ
            return View(phieuSua);
        }

        // POST: PhieuSua/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _phieuSuaService.DeletePhieuSuaAsync(id);
            if (success)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi xóa phiếu sửa!");
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: PhieuSua/AddDichVu/5
        public async Task<IActionResult> AddDichVu(int? id)
        {
            if (id == null) return NotFound();
            ViewBag.DichVus = await _context.DichVu.Where(d => d.TrangThai).ToListAsync();
            ViewBag.MaPhieuSua = id;
            return View();
        }

        // POST: PhieuSua/AddDichVu/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDichVu(int id, int madichvu, int soluong)
        {
            var chitiet = new ChiTietPhieuSua
            {
                MaPhieuSua = id,
                MaDichVu = madichvu,
                SoLuong = soluong
            };
            _context.ChiTietPhieuSua.Add(chitiet);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id });
        }

        // GET: PhieuSua/AddLinhKien/5
        public async Task<IActionResult> AddLinhKien(int? id)
        {
            if (id == null) return NotFound();
            ViewBag.LinhKiens = await _context.LinhKien.Where(lk => lk.TrangThai).ToListAsync();
            ViewBag.MaPhieuSua = id;
            return View();
        }

        // POST: PhieuSua/AddLinhKien/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLinhKien(int id, int malinhkien, int soluong)
        {
            var chitiet = new ChiTietLinhKien
            {
                MaPhieuSua = id,
                MaLinhKien = malinhkien,
                SoLuong = soluong
            };
            _context.ChiTietLinhKien.Add(chitiet);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id });
        }

        // POST: PhieuSua/UpdateAllTongTien
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAllTongTien()
        {
            try
            {
                var updatedCount = await _phieuSuaService.UpdateAllTongTienAsync();
                return Json(new { success = true, message = $"Đã cập nhật tổng tiền cho {updatedCount} phiếu sửa." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
    }
}
