using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using phonev2.Models;
using phonev2.Repository;

namespace phonev2.Controllers
{
    public class NhanVienController : Controller
    {
        private readonly PhoneLapDbContext _context;

        public NhanVienController(PhoneLapDbContext context)
        {
            _context = context;
        }

        // GET: NhanVien
        public async Task<IActionResult> Index(string searchString, string chucVuFilter, string trangThaiFilter, string sortOrder, int page = 1, int pageSize = 10)
        {
            ViewData["Title"] = "Quản Lý Nhân Viên";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentChucVu"] = chucVuFilter;
            ViewData["CurrentTrangThai"] = trangThaiFilter;
            ViewData["CurrentSort"] = sortOrder;
            
            // Sort parameters
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["ChucVuSortParm"] = sortOrder == "chucvu" ? "chucvu_desc" : "chucvu";
            ViewData["LuongSortParm"] = sortOrder == "luong" ? "luong_desc" : "luong";
            ViewData["NgayVaoLamSortParm"] = sortOrder == "ngayvaolam" ? "ngayvaolam_desc" : "ngayvaolam";
            ViewData["TuoiSortParm"] = sortOrder == "tuoi" ? "tuoi_desc" : "tuoi";

            var nhanViens = from nv in _context.NhanVien select nv;

            // Tìm kiếm
            if (!String.IsNullOrEmpty(searchString))
            {
                nhanViens = nhanViens.Where(nv => nv.HoTen.Contains(searchString) 
                                              || nv.Email.Contains(searchString)
                                              || nv.SoDienThoai.Contains(searchString)
                                              || nv.ChucVu.Contains(searchString));
            }

            // Lọc theo chức vụ
            if (!String.IsNullOrEmpty(chucVuFilter))
            {
                nhanViens = nhanViens.Where(nv => nv.ChucVu.Contains(chucVuFilter));
            }

            // Lọc theo trạng thái
            if (!String.IsNullOrEmpty(trangThaiFilter))
            {
                switch (trangThaiFilter)
                {
                    case "active":
                        nhanViens = nhanViens.Where(nv => nv.TrangThai && !nv.NgayNghiViec.HasValue);
                        break;
                    case "inactive":
                        nhanViens = nhanViens.Where(nv => !nv.TrangThai);
                        break;
                    case "retired":
                        nhanViens = nhanViens.Where(nv => nv.NgayNghiViec.HasValue);
                        break;
                }
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "name_desc":
                    nhanViens = nhanViens.OrderByDescending(nv => nv.HoTen);
                    break;
                case "chucvu":
                    nhanViens = nhanViens.OrderBy(nv => nv.ChucVu);
                    break;
                case "chucvu_desc":
                    nhanViens = nhanViens.OrderByDescending(nv => nv.ChucVu);
                    break;
                case "luong":
                    nhanViens = nhanViens.OrderBy(nv => nv.Luong);
                    break;
                case "luong_desc":
                    nhanViens = nhanViens.OrderByDescending(nv => nv.Luong);
                    break;
                case "ngayvaolam":
                    nhanViens = nhanViens.OrderBy(nv => nv.NgayVaoLam);
                    break;
                case "ngayvaolam_desc":
                    nhanViens = nhanViens.OrderByDescending(nv => nv.NgayVaoLam);
                    break;
                case "tuoi":
                    nhanViens = nhanViens.OrderBy(nv => nv.NgaySinh);
                    break;
                case "tuoi_desc":
                    nhanViens = nhanViens.OrderByDescending(nv => nv.NgaySinh);
                    break;
                default:
                    nhanViens = nhanViens.OrderBy(nv => nv.HoTen);
                    break;
            }

            // Pagination
            var totalItems = await nhanViens.CountAsync();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            var result = await nhanViens
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Dropdown data cho filters
            ViewBag.ChucVuList = await _context.NhanVien
                .Where(nv => !string.IsNullOrEmpty(nv.ChucVu))
                .Select(nv => nv.ChucVu)
                .Distinct()
                .OrderBy(cv => cv)
                .ToListAsync();

            return View(result);
        }

        // GET: NhanVien/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["Title"] = "Chi Tiết Nhân Viên";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhân viên.";
                return RedirectToAction(nameof(Index));
            }

            var nhanVien = await _context.NhanVien
                .FirstOrDefaultAsync(m => m.MaNhanVien == id);
            
            if (nhanVien == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhân viên.";
                return RedirectToAction(nameof(Index));
            }

            return View(nhanVien);
        }

        // GET: NhanVien/Create
        public IActionResult Create()
        {
            ViewData["Title"] = "Thêm Nhân Viên Mới";
            
            // Tạo model với giá trị mặc định
            var nhanVien = new NhanVien
            {
                NgayVaoLam = DateTime.Now.Date,
                TrangThai = true
            };
            
            return View(nhanVien);
        }

        // POST: NhanVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HoTen,SoDienThoai,Email,DiaChi,NgaySinh,NgayVaoLam,ChucVu,Luong,TrangThai")] NhanVien nhanVien)
        {
            ViewData["Title"] = "Thêm Nhân Viên Mới";

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra tuổi tối thiểu (18 tuổi)
                    var age = DateTime.Now.Year - nhanVien.NgaySinh.Year;
                    if (DateTime.Now.DayOfYear < nhanVien.NgaySinh.DayOfYear) age--;
                    
                    if (age < 18)
                    {
                        ModelState.AddModelError("NgaySinh", "Nhân viên phải từ 18 tuổi trở lên.");
                        return View(nhanVien);
                    }

                    // Kiểm tra ngày vào làm không được trước ngày sinh
                    if (nhanVien.NgayVaoLam < nhanVien.NgaySinh)
                    {
                        ModelState.AddModelError("NgayVaoLam", "Ngày vào làm không thể trước ngày sinh.");
                        return View(nhanVien);
                    }

                    // Kiểm tra trùng email
                    var existingByEmail = await _context.NhanVien
                        .AnyAsync(nv => nv.Email.ToLower() == nhanVien.Email.ToLower());
                    
                    if (existingByEmail)
                    {
                        ModelState.AddModelError("Email", "Email này đã được sử dụng bởi nhân viên khác.");
                        return View(nhanVien);
                    }

                    // Kiểm tra trùng số điện thoại
                    var existingByPhone = await _context.NhanVien
                        .AnyAsync(nv => nv.SoDienThoai == nhanVien.SoDienThoai);
                    
                    if (existingByPhone)
                    {
                        ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng bởi nhân viên khác.");
                        return View(nhanVien);
                    }

                    _context.Add(nhanVien);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Đã thêm thành công nhân viên '{nhanVien.HoTen}'.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi thêm nhân viên: " + ex.Message);
                }
            }
            
            return View(nhanVien);
        }

        // GET: NhanVien/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Title"] = "Sửa Thông Tin Nhân Viên";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhân viên.";
                return RedirectToAction(nameof(Index));
            }

            var nhanVien = await _context.NhanVien.FindAsync(id);
            if (nhanVien == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhân viên.";
                return RedirectToAction(nameof(Index));
            }
            
            return View(nhanVien);
        }

        // POST: NhanVien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaNhanVien,HoTen,SoDienThoai,Email,DiaChi,NgaySinh,NgayVaoLam,NgayNghiViec,ChucVu,Luong,TrangThai")] NhanVien nhanVien)
        {
            ViewData["Title"] = "Sửa Thông Tin Nhân Viên";

            if (id != nhanVien.MaNhanVien)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra tuổi tối thiểu (18 tuổi)
                    var age = DateTime.Now.Year - nhanVien.NgaySinh.Year;
                    if (DateTime.Now.DayOfYear < nhanVien.NgaySinh.DayOfYear) age--;
                    
                    if (age < 18)
                    {
                        ModelState.AddModelError("NgaySinh", "Nhân viên phải từ 18 tuổi trở lên.");
                        return View(nhanVien);
                    }

                    // Kiểm tra ngày nghỉ việc
                    if (nhanVien.NgayNghiViec.HasValue && nhanVien.NgayNghiViec < nhanVien.NgayVaoLam)
                    {
                        ModelState.AddModelError("NgayNghiViec", "Ngày nghỉ việc không thể trước ngày vào làm.");
                        return View(nhanVien);
                    }

                    // Kiểm tra trùng email (trừ chính nó)
                    var existingByEmail = await _context.NhanVien
                        .AnyAsync(nv => nv.Email.ToLower() == nhanVien.Email.ToLower() 
                                    && nv.MaNhanVien != nhanVien.MaNhanVien);
                    
                    if (existingByEmail)
                    {
                        ModelState.AddModelError("Email", "Email này đã được sử dụng bởi nhân viên khác.");
                        return View(nhanVien);
                    }

                    // Kiểm tra trùng số điện thoại (trừ chính nó)
                    var existingByPhone = await _context.NhanVien
                        .AnyAsync(nv => nv.SoDienThoai == nhanVien.SoDienThoai 
                                    && nv.MaNhanVien != nhanVien.MaNhanVien);
                    
                    if (existingByPhone)
                    {
                        ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng bởi nhân viên khác.");
                        return View(nhanVien);
                    }

                    _context.Update(nhanVien);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Đã cập nhật thành công nhân viên '{nhanVien.HoTen}'.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhanVienExists(nhanVien.MaNhanVien))
                    {
                        TempData["ErrorMessage"] = "Nhân viên không tồn tại.";
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
            
            return View(nhanVien);
        }

        // GET: NhanVien/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["Title"] = "Xóa Nhân Viên";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhân viên.";
                return RedirectToAction(nameof(Index));
            }

            var nhanVien = await _context.NhanVien
                .FirstOrDefaultAsync(m => m.MaNhanVien == id);
                
            if (nhanVien == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhân viên.";
                return RedirectToAction(nameof(Index));
            }

            return View(nhanVien);
        }

        // POST: NhanVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var nhanVien = await _context.NhanVien.FindAsync(id);
                if (nhanVien != null)
                {
                    _context.NhanVien.Remove(nhanVien);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Đã xóa thành công nhân viên '{nhanVien.HoTen}'.";
                }
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Không thể xóa nhân viên này vì đang được sử dụng trong các phiếu sửa chữa hoặc hóa đơn.";
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
                var nhanVien = await _context.NhanVien.FindAsync(id);
                if (nhanVien != null)
                {
                    nhanVien.TrangThai = !nhanVien.TrangThai;
                    await _context.SaveChangesAsync();
                    
                    return Json(new { success = true, status = nhanVien.TrangThai });
                }
                return Json(new { success = false, message = "Không tìm thấy nhân viên" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Nghỉ việc
        [HttpPost]
        public async Task<IActionResult> Retire(int id, DateTime? ngayNghiViec = null)
        {
            try
            {
                var nhanVien = await _context.NhanVien.FindAsync(id);
                if (nhanVien != null)
                {
                    nhanVien.NgayNghiViec = ngayNghiViec ?? DateTime.Now;
                    nhanVien.TrangThai = false;
                    await _context.SaveChangesAsync();
                    
                    return Json(new { success = true, message = $"Nhân viên {nhanVien.HoTen} đã nghỉ việc" });
                }
                return Json(new { success = false, message = "Không tìm thấy nhân viên" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Quay lại làm việc
        [HttpPost]
        public async Task<IActionResult> Reactivate(int id)
        {
            try
            {
                var nhanVien = await _context.NhanVien.FindAsync(id);
                if (nhanVien != null)
                {
                    nhanVien.NgayNghiViec = null;
                    nhanVien.TrangThai = true;
                    await _context.SaveChangesAsync();
                    
                    return Json(new { success = true, message = $"Nhân viên {nhanVien.HoTen} đã quay lại làm việc" });
                }
                return Json(new { success = false, message = "Không tìm thấy nhân viên" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API cho AJAX search (tùy chọn)
        [HttpGet]
        public async Task<IActionResult> SearchAjax(string term)
        {
            var nhanViens = await _context.NhanVien
                .Where(nv => nv.HoTen.Contains(term) && nv.TrangThai && !nv.NgayNghiViec.HasValue)
                .Select(nv => new { 
                    id = nv.MaNhanVien, 
                    text = nv.HoTen,
                    chucvu = nv.ChucVu,
                    contact = nv.ThongTinLienHe
                })
                .Take(10)
                .ToListAsync();

            return Json(nhanViens);
        }

        // Lấy thông tin nhân viên
        [HttpGet]
        public async Task<IActionResult> GetEmployeeInfo(int id)
        {
            var nhanVien = await _context.NhanVien.FindAsync(id);
            if (nhanVien == null)
            {
                return Json(new { success = false, message = "Không tìm thấy nhân viên" });
            }

            return Json(new { 
                success = true, 
                data = new {
                    id = nhanVien.MaNhanVien,
                    name = nhanVien.HoTen,
                    chucvu = nhanVien.ChucVu,
                    phone = nhanVien.SoDienThoai,
                    email = nhanVien.Email,
                    address = nhanVien.DiaChi,
                    status = nhanVien.TrangThaiText,
                    age = nhanVien.Tuoi,
                    experience = nhanVien.GetWorkExperience()
                }
            });
        }

        // Lấy danh sách nhân viên đang hoạt động theo chức vụ
        [HttpGet]
        public async Task<IActionResult> GetActiveEmployeesByRole(string chucVu = "")
        {
            var query = _context.NhanVien
                .Where(nv => nv.TrangThai && !nv.NgayNghiViec.HasValue);

            if (!string.IsNullOrEmpty(chucVu))
            {
                query = query.Where(nv => nv.ChucVu.Contains(chucVu));
            }

            var employees = await query
                .Select(nv => new {
                    id = nv.MaNhanVien,
                    name = nv.HoTen,
                    chucvu = nv.ChucVu,
                    contact = nv.ThongTinLienHe
                })
                .OrderBy(nv => nv.name)
                .ToListAsync();

            return Json(employees);
        }

        // Báo cáo nhân viên
        [HttpGet]
        public async Task<IActionResult> GetEmployeeStats()
        {
            var totalEmployees = await _context.NhanVien.CountAsync();
            var activeEmployees = await _context.NhanVien.CountAsync(nv => nv.TrangThai && !nv.NgayNghiViec.HasValue);
            var retiredEmployees = await _context.NhanVien.CountAsync(nv => nv.NgayNghiViec.HasValue);
            var avgAge = await _context.NhanVien.AverageAsync(nv => DateTime.Now.Year - nv.NgaySinh.Year);
            var avgSalary = await _context.NhanVien.Where(nv => nv.TrangThai).AverageAsync(nv => nv.Luong);

            var positionStats = await _context.NhanVien
                .Where(nv => nv.TrangThai)
                .GroupBy(nv => nv.ChucVu)
                .Select(g => new { position = g.Key, count = g.Count() })
                .ToListAsync();

            return Json(new {
                total = totalEmployees,
                active = activeEmployees,
                retired = retiredEmployees,
                avgAge = Math.Round(avgAge, 1),
                avgSalary = Math.Round(avgSalary, 0),
                positions = positionStats
            });
        }

        private bool NhanVienExists(int id)
        {
            return _context.NhanVien.Any(e => e.MaNhanVien == id);
        }
    }
}