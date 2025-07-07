// Controllers/PhieuNhapController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using phonev2.Models;
using phonev2.Repository;
using System.Text.Json;

namespace phonev2.Controllers
{
    public class PhieuNhapController : Controller
    {
        private readonly PhoneLapDbContext _context;

        public PhieuNhapController(PhoneLapDbContext context)
        {
            _context = context;
        }

        // GET: PhieuNhap - Danh sách phiếu nhập
        public async Task<IActionResult> Index(string searchString, string statusFilter, int? supplierFilter, string sortOrder, int page = 1, int pageSize = 10)
        {
            ViewData["Title"] = "Quản Lý Phiếu Nhập";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentStatus"] = statusFilter;
            ViewData["CurrentSupplier"] = supplierFilter;
            ViewData["CurrentSort"] = sortOrder;

            // Sort parameters
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["SupplierSortParm"] = sortOrder == "supplier" ? "supplier_desc" : "supplier";
            ViewData["StatusSortParm"] = sortOrder == "status" ? "status_desc" : "status";
            ViewData["TotalSortParm"] = sortOrder == "total" ? "total_desc" : "total";

            var phieuNhaps = _context.PhieuNhap
                .Include(pn => pn.NhaCungCap)
                .Include(pn => pn.NhanVien)
                .AsQueryable();

            // Tìm kiếm
            if (!String.IsNullOrEmpty(searchString))
            {
                phieuNhaps = phieuNhaps.Where(pn => pn.MaPhieuNhap.ToString().Contains(searchString)
                                                  || pn.NhaCungCap!.TenNhaCungCap.Contains(searchString)
                                                  || pn.NhanVien!.HoTen.Contains(searchString)
                                                  || (pn.GhiChu != null && pn.GhiChu.Contains(searchString)));
            }

            // Lọc theo trạng thái
            if (!String.IsNullOrEmpty(statusFilter))
            {
                phieuNhaps = phieuNhaps.Where(pn => pn.TrangThai == statusFilter);
            }

            // Lọc theo nhà cung cấp
            if (supplierFilter.HasValue)
            {
                phieuNhaps = phieuNhaps.Where(pn => pn.MaNhaCungCap == supplierFilter.Value);
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "date_desc":
                    phieuNhaps = phieuNhaps.OrderByDescending(pn => pn.NgayNhap);
                    break;
                case "supplier":
                    phieuNhaps = phieuNhaps.OrderBy(pn => pn.NhaCungCap!.TenNhaCungCap);
                    break;
                case "supplier_desc":
                    phieuNhaps = phieuNhaps.OrderByDescending(pn => pn.NhaCungCap!.TenNhaCungCap);
                    break;
                case "status":
                    phieuNhaps = phieuNhaps.OrderBy(pn => pn.TrangThai);
                    break;
                case "status_desc":
                    phieuNhaps = phieuNhaps.OrderByDescending(pn => pn.TrangThai);
                    break;
                case "total":
                    phieuNhaps = phieuNhaps.OrderBy(pn => pn.TongTien);
                    break;
                case "total_desc":
                    phieuNhaps = phieuNhaps.OrderByDescending(pn => pn.TongTien);
                    break;
                default:
                    phieuNhaps = phieuNhaps.OrderByDescending(pn => pn.NgayTao);
                    break;
            }

            // Phân trang
            var totalRecords = await phieuNhaps.CountAsync();
            var items = await phieuNhaps
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["TotalPages"] = (int)Math.Ceiling(totalRecords / (double)pageSize);
            ViewData["CurrentPage"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalRecords"] = totalRecords;

            // Data cho dropdown filters
            ViewData["Suppliers"] = await _context.NhaCungCap
                .Where(ncc => ncc.TrangThai == true)
                .Select(ncc => new SelectListItem
                {
                    Value = ncc.MaNhaCungCap.ToString(),
                    Text = ncc.TenNhaCungCap
                }).ToListAsync();

            ViewData["StatusList"] = new List<SelectListItem>
            {
                new SelectListItem { Value = "Nháp", Text = "Nháp" },
                new SelectListItem { Value = "Đã xác nhận", Text = "Đã xác nhận" }
            };

            return View(items);
        }

        // GET: PhieuNhap/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var phieuNhap = await _context.PhieuNhap
                .Include(pn => pn.NhaCungCap)
                .Include(pn => pn.NhanVien)
                .Include(pn => pn.ChiTietPhieuNhaps!)
                    .ThenInclude(ct => ct.LinhKien!)
                    .ThenInclude(lk => lk.LoaiLinhKien)
                .FirstOrDefaultAsync(pn => pn.MaPhieuNhap == id);

            if (phieuNhap == null) return NotFound();

            ViewData["Title"] = $"Chi Tiết Phiếu Nhập #{phieuNhap.MaPhieuNhap}";
            return View(phieuNhap);
        }

        // GET: PhieuNhap/Create
        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "Tạo Phiếu Nhập Mới";
            
            // Load data cho dropdown
            ViewData["NhaCungCaps"] = await _context.NhaCungCap
                .Where(ncc => ncc.TrangThai == true)
                .Select(ncc => new SelectListItem
                {
                    Value = ncc.MaNhaCungCap.ToString(),
                    Text = ncc.TenNhaCungCap
                }).ToListAsync();

            ViewData["NhanViens"] = await _context.NhanVien
                .Where(nv => nv.TrangThai == true)
                .Select(nv => new SelectListItem
                {
                    Value = nv.MaNhanVien.ToString(),
                    Text = nv.HoTen
                }).ToListAsync();

            ViewData["LinhKiens"] = await _context.LinhKien
                .Include(lk => lk.LoaiLinhKien)
                .Where(lk => lk.TrangThai == true)
                .Select(lk => new {
                    MaLinhKien = lk.MaLinhKien,
                    TenLinhKien = lk.TenLinhKien,
                    LoaiLinhKien = lk.LoaiLinhKien!.TenLoaiLinhKien,
                    GiaNhap = lk.GiaNhap,
                    SoLuongTon = lk.SoLuongTon
                }).ToListAsync();

            var phieuNhap = new PhieuNhap
            {
                NgayNhap = DateTime.Now,
                TrangThai = "Nháp"
            };

            return View(phieuNhap);
        }

        // POST: PhieuNhap/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNhaCungCap,MaNhanVien,NgayNhap,GhiChu")] PhieuNhap phieuNhap, string chiTietJson)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    phieuNhap.NgayTao = DateTime.Now;
                    phieuNhap.TrangThai = "Nháp";
                    phieuNhap.TongTien = 0;

                    _context.PhieuNhap.Add(phieuNhap);
                    await _context.SaveChangesAsync();

                    // Xử lý chi tiết phiếu nhập
                    if (!string.IsNullOrEmpty(chiTietJson))
                    {
                        var chiTietList = JsonSerializer.Deserialize<List<ChiTietPhieuNhapDto>>(chiTietJson);
                        decimal tongTien = 0;

                        foreach (var item in chiTietList!)
                        {
                            var chiTiet = new ChiTietPhieuNhap
                            {
                                MaPhieuNhap = phieuNhap.MaPhieuNhap,
                                MaLinhKien = item.MaLinhKien,
                                SoLuong = item.SoLuong,
                                GiaNhap = item.GiaNhap,
                                ThanhTien = item.SoLuong * item.GiaNhap,
                                GhiChu = item.GhiChu
                            };

                            _context.ChiTietPhieuNhap.Add(chiTiet);
                            tongTien += chiTiet.ThanhTien;
                        }

                        // Cập nhật tổng tiền
                        phieuNhap.TongTien = tongTien;
                        _context.PhieuNhap.Update(phieuNhap);
                    }

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Tạo phiếu nhập thành công!";
                    return RedirectToAction(nameof(Details), new { id = phieuNhap.MaPhieuNhap });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi tạo phiếu nhập: " + ex.Message);
                }
            }

            // Reload data nếu có lỗi
            await LoadCreateViewData();
            return View(phieuNhap);
        }

        // GET: PhieuNhap/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var phieuNhap = await _context.PhieuNhap
                .Include(pn => pn.ChiTietPhieuNhaps!)
                    .ThenInclude(ct => ct.LinhKien)
                .FirstOrDefaultAsync(pn => pn.MaPhieuNhap == id);

            if (phieuNhap == null) return NotFound();

            // Chỉ cho phép sửa phiếu nhập ở trạng thái "Nháp"
            if (phieuNhap.TrangThai != "Nháp")
            {
                TempData["ErrorMessage"] = "Chỉ có thể sửa phiếu nhập ở trạng thái Nháp!";
                return RedirectToAction(nameof(Details), new { id });
            }

            ViewData["Title"] = $"Sửa Phiếu Nhập #{phieuNhap.MaPhieuNhap}";
            await LoadCreateViewData();

            return View(phieuNhap);
        }

        // POST: PhieuNhap/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaPhieuNhap,MaNhaCungCap,MaNhanVien,NgayNhap,GhiChu")] PhieuNhap phieuNhap, string chiTietJson)
        {
            if (id != phieuNhap.MaPhieuNhap) return NotFound();

            var existingPhieuNhap = await _context.PhieuNhap.FindAsync(id);
            if (existingPhieuNhap == null) return NotFound();

            if (existingPhieuNhap.TrangThai != "Nháp")
            {
                TempData["ErrorMessage"] = "Chỉ có thể sửa phiếu nhập ở trạng thái Nháp!";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Cập nhật thông tin phiếu nhập
                    existingPhieuNhap.MaNhaCungCap = phieuNhap.MaNhaCungCap;
                    existingPhieuNhap.MaNhanVien = phieuNhap.MaNhanVien;
                    existingPhieuNhap.NgayNhap = phieuNhap.NgayNhap;
                    existingPhieuNhap.GhiChu = phieuNhap.GhiChu;
                    existingPhieuNhap.NgayCapNhat = DateTime.Now;

                    // Xóa chi tiết cũ
                    var oldChiTiet = _context.ChiTietPhieuNhap.Where(ct => ct.MaPhieuNhap == id);
                    _context.ChiTietPhieuNhap.RemoveRange(oldChiTiet);

                    // Thêm chi tiết mới
                    decimal tongTien = 0;
                    if (!string.IsNullOrEmpty(chiTietJson))
                    {
                        var chiTietList = JsonSerializer.Deserialize<List<ChiTietPhieuNhapDto>>(chiTietJson);

                        foreach (var item in chiTietList!)
                        {
                            var chiTiet = new ChiTietPhieuNhap
                            {
                                MaPhieuNhap = id,
                                MaLinhKien = item.MaLinhKien,
                                SoLuong = item.SoLuong,
                                GiaNhap = item.GiaNhap,
                                ThanhTien = item.SoLuong * item.GiaNhap,
                                GhiChu = item.GhiChu
                            };

                            _context.ChiTietPhieuNhap.Add(chiTiet);
                            tongTien += chiTiet.ThanhTien;
                        }
                    }

                    existingPhieuNhap.TongTien = tongTien;
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cập nhật phiếu nhập thành công!";
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật: " + ex.Message);
                }
            }

            await LoadCreateViewData();
            return View(phieuNhap);
        }

        // POST: PhieuNhap/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var phieuNhap = await _context.PhieuNhap.FindAsync(id);
                if (phieuNhap == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy phiếu nhập!" });
                }

                // Chỉ cho phép xóa phiếu nhập ở trạng thái "Nháp"
                if (phieuNhap.TrangThai != "Nháp")
                {
                    return Json(new { success = false, message = "Chỉ có thể xóa phiếu nhập ở trạng thái Nháp!" });
                }

                // Xóa chi tiết trước
                var chiTietList = _context.ChiTietPhieuNhap.Where(ct => ct.MaPhieuNhap == id);
                _context.ChiTietPhieuNhap.RemoveRange(chiTietList);

                // Xóa phiếu nhập
                _context.PhieuNhap.Remove(phieuNhap);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Xóa phiếu nhập thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // POST: PhieuNhap/Confirm/5 - Xác nhận phiếu nhập và cập nhật tồn kho
        [HttpPost]
        public async Task<IActionResult> Confirm(int id)
        {
            try
            {
                var phieuNhap = await _context.PhieuNhap
                    .Include(pn => pn.ChiTietPhieuNhaps!)
                        .ThenInclude(ct => ct.LinhKien)
                    .FirstOrDefaultAsync(pn => pn.MaPhieuNhap == id);

                if (phieuNhap == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy phiếu nhập!" });
                }

                if (phieuNhap.TrangThai != "Nháp")
                {
                    return Json(new { success = false, message = "Phiếu nhập đã được xác nhận trước đó!" });
                }

                if (phieuNhap.ChiTietPhieuNhaps == null || !phieuNhap.ChiTietPhieuNhaps.Any())
                {
                    return Json(new { success = false, message = "Phiếu nhập chưa có chi tiết!" });
                }

                // Cập nhật tồn kho cho từng linh kiện
                foreach (var chiTiet in phieuNhap.ChiTietPhieuNhaps)
                {
                    var linhKien = chiTiet.LinhKien;
                    if (linhKien != null)
                    {
                        linhKien.SoLuongTon += chiTiet.SoLuong;
                        linhKien.GiaNhap = chiTiet.GiaNhap; // Cập nhật giá nhập mới nhất
                        _context.LinhKien.Update(linhKien);
                    }
                }

                // Cập nhật trạng thái phiếu nhập
                phieuNhap.TrangThai = "Đã xác nhận";
                phieuNhap.NgayCapNhat = DateTime.Now;
                _context.PhieuNhap.Update(phieuNhap);

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Xác nhận phiếu nhập thành công! Tồn kho đã được cập nhật." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // GET: API để lấy thông tin linh kiện
        [HttpGet]
        public async Task<JsonResult> GetLinhKienInfo(int linhKienId)
        {
            try
            {
                var linhKien = await _context.LinhKien
                    .Include(lk => lk.LoaiLinhKien)
                    .FirstOrDefaultAsync(lk => lk.MaLinhKien == linhKienId);

                if (linhKien == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy linh kiện!" });
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        MaLinhKien = linhKien.MaLinhKien,
                        TenLinhKien = linhKien.TenLinhKien,
                        LoaiLinhKien = linhKien.LoaiLinhKien?.TenLoaiLinhKien,
                        GiaNhap = linhKien.GiaNhap,
                        GiaBan = linhKien.GiaBan,
                        SoLuongTon = linhKien.SoLuongTon,
                        ThongSoKyThuat = linhKien.ThongSoKyThuat
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // Private methods
        private async Task LoadCreateViewData()
        {
            ViewData["NhaCungCaps"] = await _context.NhaCungCap
                .Where(ncc => ncc.TrangThai == true)
                .Select(ncc => new SelectListItem
                {
                    Value = ncc.MaNhaCungCap.ToString(),
                    Text = ncc.TenNhaCungCap
                }).ToListAsync();

            ViewData["NhanViens"] = await _context.NhanVien
                .Where(nv => nv.TrangThai == true)
                .Select(nv => new SelectListItem
                {
                    Value = nv.MaNhanVien.ToString(),
                    Text = nv.HoTen
                }).ToListAsync();

            ViewData["LinhKiens"] = await _context.LinhKien
                .Include(lk => lk.LoaiLinhKien)
                .Where(lk => lk.TrangThai == true)
                .Select(lk => new {
                    MaLinhKien = lk.MaLinhKien,
                    TenLinhKien = lk.TenLinhKien,
                    LoaiLinhKien = lk.LoaiLinhKien!.TenLoaiLinhKien,
                    GiaNhap = lk.GiaNhap,
                    GiaBan = lk.GiaBan,
                    SoLuongTon = lk.SoLuongTon
                }).ToListAsync();
        }
    }

    // DTO cho ChiTietPhieuNhap
    public class ChiTietPhieuNhapDto
    {
        public int MaLinhKien { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaNhap { get; set; }
        public string? GhiChu { get; set; }
    }
}