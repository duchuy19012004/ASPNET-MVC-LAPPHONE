using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

            var dichVus = from d in _context.DichVu where !d.DaXoa select d;

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

            // Thống kê Top 5 Dịch Vụ Được Sử Dụng (chỉ lấy phiếu sửa hoàn thành)
            var top5DichVu = _context.PhieuSua
                .Where(ps => ps.TrangThai == TrangThaiPhieuSua.HoanThanh)
                .Join(_context.ChiTietPhieuSua, ps => ps.MaPhieuSua, ct => ct.MaPhieuSua, (ps, ct) => ct)
                .GroupBy(ct => ct.MaDichVu)
                .Select(g => new {
                    MaDichVu = g.Key,
                    SoLan = g.Count()
                })
                .OrderByDescending(x => x.SoLan)
                .Take(5)
                .Join(_context.DichVu, x => x.MaDichVu, dv => dv.MaDichVu, (x, dv) => new { dv.TenDichVu, x.SoLan })
                .ToList();
            ViewBag.Top5DichVuLabels = top5DichVu.Select(x => x.TenDichVu).ToList();
            ViewBag.Top5DichVuCounts = top5DichVu.Select(x => x.SoLan).ToList();

            return View(items);
        }

        // GET: DichVu/Details/5
        public async Task<IActionResult> Details(int? id, bool modal = false)
        {
            ViewData["Title"] = "Chi Tiết Dịch Vụ";
            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy dịch vụ.";
                return RedirectToAction(nameof(Index));
            }
            var dichVu = await _context.DichVu.FirstOrDefaultAsync(m => m.MaDichVu == id);
            if (dichVu == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy dịch vụ.";
                return RedirectToAction(nameof(Index));
            }
            if (Request.Query["modal"] == "1" || modal)
                return PartialView("_DetailsModal", dichVu);
            return View(dichVu);
        }

        // GET: DichVu/Create
        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "Thêm Dịch Vụ Mới";
            var loaiLinhKienList = await _context.LoaiLinhKien.Where(l => l.TrangThai).OrderBy(l => l.TenLoaiLinhKien).ToListAsync();
            var linhKienList = await _context.LinhKien.Where(lk => lk.TrangThai && !lk.DaXoa).OrderBy(lk => lk.TenLinhKien).ToListAsync();
            ViewBag.LoaiLinhKienList = new SelectList(loaiLinhKienList, "MaLoaiLinhKien", "TenLoaiLinhKien");
            ViewBag.LinhKienList = linhKienList;
            return View();
        }

        // POST: DichVu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenDichVu,MoTa,GiaDichVu,ThoiGianSua,TrangThai")] DichVu dichVu, string selectedLinhKienIds)
        {
            ViewData["Title"] = "Thêm Dịch Vụ Mới";
            if (string.IsNullOrWhiteSpace(selectedLinhKienIds))
            {
                ModelState.AddModelError("selectedLinhKienIds", "Vui lòng chọn ít nhất một linh kiện.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var existingService = await _context.DichVu.AnyAsync(d => d.TenDichVu.ToLower() == dichVu.TenDichVu.ToLower());
                    if (existingService)
                    {
                        ModelState.AddModelError("TenDichVu", "Tên dịch vụ đã tồn tại.");
                        goto ReturnView;
                    }
                    dichVu.NgayTao = DateTime.Now;
                    _context.Add(dichVu);
                    await _context.SaveChangesAsync();
                    // Lưu liên kết dịch vụ - linh kiện
                    if (!string.IsNullOrEmpty(selectedLinhKienIds))
                    {
                        var ids = selectedLinhKienIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                        foreach (var maLinhKien in ids)
                        {
                            _context.DichVuLinhKien.Add(new DichVuLinhKien
                            {
                                MaDichVu = dichVu.MaDichVu,
                                MaLinhKien = maLinhKien,
                                SoLuong = 1
                            });
                        }
                        await _context.SaveChangesAsync();
                    }
                    TempData["SuccessMessage"] = $"Đã thêm thành công dịch vụ '{dichVu.TenDichVu}'.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi thêm dịch vụ: " + ex.Message);
                }
            }
ReturnView:
            var loaiLinhKienList = await _context.LoaiLinhKien.Where(l => l.TrangThai).OrderBy(l => l.TenLoaiLinhKien).ToListAsync();
            var linhKienList = await _context.LinhKien.Where(lk => lk.TrangThai && !lk.DaXoa).OrderBy(lk => lk.TenLinhKien).ToListAsync();
            ViewBag.LoaiLinhKienList = new SelectList(loaiLinhKienList, "MaLoaiLinhKien", "TenLoaiLinhKien");
            ViewBag.LinhKienList = linhKienList;
            return View(dichVu);
        }

        // GET: DichVu/Edit/5
        public async Task<IActionResult> Edit(int? id, bool modal = false)
        {
            Console.WriteLine($"[Edit-GET] id: {id}");
            ViewData["Title"] = "Sửa Thông Tin Dịch Vụ";
            if (id == null)
            {
                Console.WriteLine("[Edit-GET] id null");
                TempData["ErrorMessage"] = "Không tìm thấy dịch vụ.";
                return RedirectToAction(nameof(Index));
            }
            var dichVu = await _context.DichVu.Include(dv => dv.DichVuLinhKiens).FirstOrDefaultAsync(dv => dv.MaDichVu == id);
            Console.WriteLine($"[Edit-GET] dichVu null: {dichVu == null}");
            if (dichVu == null)
            {
                Console.WriteLine("[Edit-GET] Không tìm thấy dịch vụ trong DB");
                TempData["ErrorMessage"] = "Không tìm thấy dịch vụ.";
                return RedirectToAction(nameof(Index));
            }
            // Lấy danh sách loại linh kiện và linh kiện
            var loaiLinhKienList = await _context.LoaiLinhKien.Where(l => l.TrangThai).OrderBy(l => l.TenLoaiLinhKien).ToListAsync();
            var linhKienList = await _context.LinhKien.Where(lk => lk.TrangThai && !lk.DaXoa).OrderBy(lk => lk.TenLinhKien).ToListAsync();
            Console.WriteLine($"[Edit-GET] So luong linh kien: {linhKienList.Count}, So luong loai linh kien: {loaiLinhKienList.Count}");
            var selectedLinhKienIds = dichVu.DichVuLinhKiens?.Select(x => x.MaLinhKien).ToList() ?? new List<int>();
            Console.WriteLine($"[Edit-GET] SelectedLinhKienIds: {string.Join(",", selectedLinhKienIds)}");
            ViewBag.LoaiLinhKienList = new SelectList(loaiLinhKienList, "MaLoaiLinhKien", "TenLoaiLinhKien");
            ViewBag.LinhKienList = linhKienList;
            ViewBag.SelectedLinhKienIds = selectedLinhKienIds;
            if (Request.Query["modal"] == "1" || modal)
                return PartialView("_EditModal", dichVu);
            return View(dichVu);
        }

        // POST: DichVu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaDichVu,TenDichVu,MoTa,GiaDichVu,ThoiGianSua,NgayTao,TrangThai")] DichVu dichVu, string selectedLinhKienIds)
        {
            ViewData["Title"] = "Sửa Thông Tin Dịch Vụ";
            if (id != dichVu.MaDichVu)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }
            if (string.IsNullOrWhiteSpace(selectedLinhKienIds))
            {
                ModelState.AddModelError("selectedLinhKienIds", "Vui lòng chọn ít nhất một linh kiện.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var existingService = await _context.DichVu.AnyAsync(d => d.TenDichVu.ToLower() == dichVu.TenDichVu.ToLower() && d.MaDichVu != dichVu.MaDichVu);
                    if (existingService)
                    {
                        ModelState.AddModelError("TenDichVu", "Tên dịch vụ đã tồn tại.");
                        return View(dichVu);
                    }
                    _context.Update(dichVu);
                    await _context.SaveChangesAsync();
                    // Xử lý cập nhật lại linh kiện nếu có selectedLinhKienIds
                    if (!string.IsNullOrWhiteSpace(selectedLinhKienIds))
                    {
                        // Xóa toàn bộ liên kết cũ
                        var oldLinks = _context.DichVuLinhKien.Where(x => x.MaDichVu == id);
                        _context.DichVuLinhKien.RemoveRange(oldLinks);
                        // Thêm lại liên kết mới
                        var ids = selectedLinhKienIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                        foreach (var maLinhKien in ids)
                        {
                            _context.DichVuLinhKien.Add(new DichVuLinhKien
                            {
                                MaDichVu = id,
                                MaLinhKien = maLinhKien,
                                SoLuong = 1
                            });
                        }
                        await _context.SaveChangesAsync();
                    }
                    TempData["SuccessMessage"] = $"Đã cập nhật dịch vụ '{dichVu.TenDichVu}'.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật: " + ex.Message);
                }
            }
            var loaiLinhKienList = await _context.LoaiLinhKien.Where(l => l.TrangThai).OrderBy(l => l.TenLoaiLinhKien).ToListAsync();
            var linhKienList = await _context.LinhKien.Where(lk => lk.TrangThai && !lk.DaXoa).OrderBy(lk => lk.TenLinhKien).ToListAsync();
            ViewBag.LoaiLinhKienList = new SelectList(loaiLinhKienList, "MaLoaiLinhKien", "TenLoaiLinhKien");
            ViewBag.LinhKienList = linhKienList;
            ViewBag.SelectedLinhKienIds = selectedLinhKienIds?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList() ?? new List<int>();
            return View(dichVu);
        }



        // GET: DichVu/Delete/5
        public async Task<IActionResult> Delete(int? id, bool modal = false)
        {
            ViewData["Title"] = "Xóa Dịch Vụ";
            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy dịch vụ.";
                return RedirectToAction(nameof(Index));
            }
            var dichVu = await _context.DichVu.FirstOrDefaultAsync(m => m.MaDichVu == id);
            if (dichVu == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy dịch vụ.";
                return RedirectToAction(nameof(Index));
            }
            if (Request.Query["modal"] == "1" || modal)
                return PartialView("_DeleteModal", dichVu);
            return View(dichVu);
        }

        // POST: DichVu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? lyDoXoa = "")
        {
            try
            {
                var dichVu = await _context.DichVu.FindAsync(id);
                if (dichVu != null)
                {
                    dichVu.DaXoa = true;
                    dichVu.NgayXoa = DateTime.Now;
                    dichVu.LyDoXoa = lyDoXoa;
                    dichVu.TrangThai = false;
                    await _context.SaveChangesAsync();
                    if (Request.Query["modal"] == "1" || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = true, message = $"Đã xóa (ẩn) thành công dịch vụ '{dichVu.TenDichVu}'. Dịch vụ vẫn được giữ lại trong các phiếu sửa để đảm bảo tính toàn vẹn dữ liệu." });
                    TempData["SuccessMessage"] = $"Đã xóa (ẩn) thành công dịch vụ '{dichVu.TenDichVu}'.";
                }
            }
            catch (DbUpdateException)
            {
                if (Request.Query["modal"] == "1" || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Không thể xóa dịch vụ này vì đang được sử dụng trong các phiếu sửa chữa." });
                TempData["ErrorMessage"] = "Không thể xóa dịch vụ này vì đang được sử dụng trong các phiếu sửa chữa.";
            }
            catch (Exception ex)
            {
                if (Request.Query["modal"] == "1" || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Có lỗi xảy ra khi xóa: " + ex.Message });
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa: " + ex.Message;
            }
            if (Request.Query["modal"] == "1" || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, message = "Không tìm thấy dịch vụ hoặc lỗi không xác định." });
            return RedirectToAction(nameof(Index));
        }

        // GET: DichVu/Deleted - Danh sách dịch vụ đã tạm ẩn
        public async Task<IActionResult> Deleted()
        {
            ViewData["Title"] = "Dịch Vụ Đã Ẩn";
            var deletedItems = await _context.DichVu.Where(d => d.DaXoa).OrderByDescending(d => d.NgayXoa).ToListAsync();
            return View(deletedItems);
        }

        // POST: Khôi phục dịch vụ đã ẩn
        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            var dichVu = await _context.DichVu.FindAsync(id);
            if (dichVu == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Không tìm thấy dịch vụ." });
                TempData["ErrorMessage"] = "Không tìm thấy dịch vụ.";
                return RedirectToAction("Deleted");
            }
            dichVu.DaXoa = false;
            dichVu.NgayXoa = null;
            dichVu.LyDoXoa = null;
            dichVu.TrangThai = true;
            await _context.SaveChangesAsync();
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true, message = $"Đã khôi phục dịch vụ '{dichVu.TenDichVu}'." });
            TempData["SuccessMessage"] = $"Đã khôi phục dịch vụ '{dichVu.TenDichVu}'.";
            return RedirectToAction("Index");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteForever(int id)
        {
            try
            {
                var dichVu = await _context.DichVu.FindAsync(id);
                if (dichVu == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy dịch vụ." });
                }
                // Xóa các liên kết với linh kiện (nếu có)
                var links = _context.DichVuLinhKien.Where(x => x.MaDichVu == id);
                _context.DichVuLinhKien.RemoveRange(links);
                _context.DichVu.Remove(dichVu);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Đã xóa vĩnh viễn dịch vụ." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa: " + ex.Message });
            }
        }

        // API: Lấy danh sách linh kiện gắn với dịch vụ
        [HttpGet]
        public async Task<IActionResult> GetLinhKienByDichVu(int maDichVu)
        {
            var linhKiens = await _context.DichVuLinhKien
                .Where(x => x.MaDichVu == maDichVu)
                .Include(x => x.LinhKien)
                .Select(x => new {
                    maLinhKien = x.MaLinhKien,
                    tenLinhKien = x.LinhKien != null ? x.LinhKien.TenLinhKien : "",
                    giaBan = x.LinhKien != null ? x.LinhKien.GiaBan : 0
                })
                .ToListAsync();
            return Json(linhKiens);
        }

        // API: Lấy thông tin dịch vụ (mô tả, thời gian sửa)
        [HttpGet]
        public async Task<IActionResult> GetDichVuInfo(int id)
        {
            var dv = await _context.DichVu.FirstOrDefaultAsync(x => x.MaDichVu == id);
            if (dv == null) return Json(null);
            return Json(new {
                moTa = dv.MoTa,
                thoiGian = dv.ThoiGianSua
            });
        }

        private bool DichVuExists(int id)
        {
            return _context.DichVu.Any(e => e.MaDichVu == id);
        }
    }
}