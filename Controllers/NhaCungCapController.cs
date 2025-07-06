using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using phonev2.Models;
using phonev2.Repository;

namespace phonev2.Controllers
{
    public class NhaCungCapController : Controller
    {
        private readonly PhoneLapDbContext _context;

        public NhaCungCapController(PhoneLapDbContext context)
        {
            _context = context;
        }

        // GET: NhaCungCap
        public async Task<IActionResult> Index(string searchString, string sortOrder, int page = 1, int pageSize = 10)
        {
            ViewData["Title"] = "Quản Lý Nhà Cung Cấp";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["EmailSortParm"] = sortOrder == "email" ? "email_desc" : "email";
            ViewData["PhoneSortParm"] = sortOrder == "phone" ? "phone_desc" : "phone";
            ViewData["DateSortParm"] = sortOrder == "date" ? "date_desc" : "date";

            var nhaCungCaps = from n in _context.NhaCungCap select n;

            // Tìm kiếm
            if (!String.IsNullOrEmpty(searchString))
            {
                nhaCungCaps = nhaCungCaps.Where(n => n.TenNhaCungCap.Contains(searchString) 
                                                 || n.DiaChi.Contains(searchString)
                                                 || n.SoDienThoai.Contains(searchString)
                                                 || n.Email.Contains(searchString));
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "name_desc":
                    nhaCungCaps = nhaCungCaps.OrderByDescending(n => n.TenNhaCungCap);
                    break;
                case "email":
                    nhaCungCaps = nhaCungCaps.OrderBy(n => n.Email);
                    break;
                case "email_desc":
                    nhaCungCaps = nhaCungCaps.OrderByDescending(n => n.Email);
                    break;
                case "phone":
                    nhaCungCaps = nhaCungCaps.OrderBy(n => n.SoDienThoai);
                    break;
                case "phone_desc":
                    nhaCungCaps = nhaCungCaps.OrderByDescending(n => n.SoDienThoai);
                    break;
                case "date":
                    nhaCungCaps = nhaCungCaps.OrderBy(n => n.NgayTao);
                    break;
                case "date_desc":
                    nhaCungCaps = nhaCungCaps.OrderByDescending(n => n.NgayTao);
                    break;
                default:
                    nhaCungCaps = nhaCungCaps.OrderBy(n => n.TenNhaCungCap);
                    break;
            }

            // Phân trang
            var totalCount = await nhaCungCaps.CountAsync();
            var items = await nhaCungCaps.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.TotalCount = totalCount;
            ViewBag.PageSize = pageSize;

            return View(items);
        }

        // GET: NhaCungCap/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["Title"] = "Chi Tiết Nhà Cung Cấp";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhà cung cấp.";
                return RedirectToAction(nameof(Index));
            }

            var nhaCungCap = await _context.NhaCungCap
                .FirstOrDefaultAsync(m => m.MaNhaCungCap == id);
            
            if (nhaCungCap == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhà cung cấp.";
                return RedirectToAction(nameof(Index));
            }

            return View(nhaCungCap);
        }

        // GET: NhaCungCap/Create
        public IActionResult Create()
        {
            ViewData["Title"] = "Thêm Nhà Cung Cấp Mới";
            return View();
        }

        // POST: NhaCungCap/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenNhaCungCap,DiaChi,SoDienThoai,Email,TrangThai")] NhaCungCap nhaCungCap)
        {
            ViewData["Title"] = "Thêm Nhà Cung Cấp Mới";

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra trùng email
                    var existingByEmail = await _context.NhaCungCap
                        .AnyAsync(n => n.Email.ToLower() == nhaCungCap.Email.ToLower());
                    
                    if (existingByEmail)
                    {
                        ModelState.AddModelError("Email", "Email này đã được sử dụng bởi nhà cung cấp khác.");
                        return View(nhaCungCap);
                    }

                    // Kiểm tra trùng số điện thoại
                    var existingByPhone = await _context.NhaCungCap
                        .AnyAsync(n => n.SoDienThoai == nhaCungCap.SoDienThoai);
                    
                    if (existingByPhone)
                    {
                        ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng bởi nhà cung cấp khác.");
                        return View(nhaCungCap);
                    }

                    // Kiểm tra trùng tên
                    var existingByName = await _context.NhaCungCap
                        .AnyAsync(n => n.TenNhaCungCap.ToLower() == nhaCungCap.TenNhaCungCap.ToLower());
                    
                    if (existingByName)
                    {
                        ModelState.AddModelError("TenNhaCungCap", "Tên nhà cung cấp đã tồn tại.");
                        return View(nhaCungCap);
                    }

                    nhaCungCap.NgayTao = DateTime.Now;
                    _context.Add(nhaCungCap);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Đã thêm thành công nhà cung cấp '{nhaCungCap.TenNhaCungCap}'.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi thêm nhà cung cấp: " + ex.Message);
                }
            }
            
            return View(nhaCungCap);
        }

        // GET: NhaCungCap/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Title"] = "Sửa Thông Tin Nhà Cung Cấp";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhà cung cấp.";
                return RedirectToAction(nameof(Index));
            }

            var nhaCungCap = await _context.NhaCungCap.FindAsync(id);
            if (nhaCungCap == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhà cung cấp.";
                return RedirectToAction(nameof(Index));
            }
            
            return View(nhaCungCap);
        }

        // POST: NhaCungCap/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaNhaCungCap,TenNhaCungCap,DiaChi,SoDienThoai,Email,NgayTao,TrangThai")] NhaCungCap nhaCungCap)
        {
            ViewData["Title"] = "Sửa Thông Tin Nhà Cung Cấp";

            if (id != nhaCungCap.MaNhaCungCap)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra trùng email (trừ chính nó)
                    var existingByEmail = await _context.NhaCungCap
                        .AnyAsync(n => n.Email.ToLower() == nhaCungCap.Email.ToLower() 
                                    && n.MaNhaCungCap != nhaCungCap.MaNhaCungCap);
                    
                    if (existingByEmail)
                    {
                        ModelState.AddModelError("Email", "Email này đã được sử dụng bởi nhà cung cấp khác.");
                        return View(nhaCungCap);
                    }

                    // Kiểm tra trùng số điện thoại (trừ chính nó)
                    var existingByPhone = await _context.NhaCungCap
                        .AnyAsync(n => n.SoDienThoai == nhaCungCap.SoDienThoai 
                                    && n.MaNhaCungCap != nhaCungCap.MaNhaCungCap);
                    
                    if (existingByPhone)
                    {
                        ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng bởi nhà cung cấp khác.");
                        return View(nhaCungCap);
                    }

                    // Kiểm tra trùng tên (trừ chính nó)
                    var existingByName = await _context.NhaCungCap
                        .AnyAsync(n => n.TenNhaCungCap.ToLower() == nhaCungCap.TenNhaCungCap.ToLower() 
                                    && n.MaNhaCungCap != nhaCungCap.MaNhaCungCap);
                    
                    if (existingByName)
                    {
                        ModelState.AddModelError("TenNhaCungCap", "Tên nhà cung cấp đã tồn tại.");
                        return View(nhaCungCap);
                    }

                    _context.Update(nhaCungCap);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Đã cập nhật thành công nhà cung cấp '{nhaCungCap.TenNhaCungCap}'.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhaCungCapExists(nhaCungCap.MaNhaCungCap))
                    {
                        TempData["ErrorMessage"] = "Nhà cung cấp không tồn tại.";
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
            
            return View(nhaCungCap);
        }

        // GET: NhaCungCap/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["Title"] = "Xóa Nhà Cung Cấp";

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhà cung cấp.";
                return RedirectToAction(nameof(Index));
            }

            var nhaCungCap = await _context.NhaCungCap
                .FirstOrDefaultAsync(m => m.MaNhaCungCap == id);
                
            if (nhaCungCap == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhà cung cấp.";
                return RedirectToAction(nameof(Index));
            }

            return View(nhaCungCap);
        }

        // POST: NhaCungCap/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var nhaCungCap = await _context.NhaCungCap.FindAsync(id);
                if (nhaCungCap != null)
                {
                    _context.NhaCungCap.Remove(nhaCungCap);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Đã xóa thành công nhà cung cấp '{nhaCungCap.TenNhaCungCap}'.";
                }
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Không thể xóa nhà cung cấp này vì đang được sử dụng trong các phiếu nhập.";
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
                var nhaCungCap = await _context.NhaCungCap.FindAsync(id);
                if (nhaCungCap != null)
                {
                    nhaCungCap.TrangThai = !nhaCungCap.TrangThai;
                    await _context.SaveChangesAsync();
                    
                    return Json(new { success = true, status = nhaCungCap.TrangThai });
                }
                return Json(new { success = false, message = "Không tìm thấy nhà cung cấp" });
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
            var nhaCungCaps = await _context.NhaCungCap
                .Where(n => n.TenNhaCungCap.Contains(term) && n.TrangThai)
                .Select(n => new { 
                    id = n.MaNhaCungCap, 
                    text = n.TenNhaCungCap,
                    contact = n.ThongTinLienHe,
                    address = n.DiaChiNgan
                })
                .Take(10)
                .ToListAsync();

            return Json(nhaCungCaps);
        }

        // Lấy thông tin liên hệ
        [HttpGet]
        public async Task<IActionResult> GetContactInfo(int id)
        {
            var nhaCungCap = await _context.NhaCungCap.FindAsync(id);
            if (nhaCungCap == null)
            {
                return Json(new { success = false, message = "Không tìm thấy nhà cung cấp" });
            }

            return Json(new { 
                success = true, 
                data = new {
                    id = nhaCungCap.MaNhaCungCap,
                    name = nhaCungCap.TenNhaCungCap,
                    phone = nhaCungCap.SoDienThoai,
                    email = nhaCungCap.Email,
                    address = nhaCungCap.DiaChi,
                    status = nhaCungCap.TrangThaiText
                }
            });
        }

        // Lấy danh sách nhà cung cấp đang hoạt động
        [HttpGet]
        public async Task<IActionResult> GetActiveSuppliers()
        {
            var suppliers = await _context.NhaCungCap
                .Where(n => n.TrangThai)
                .Select(n => new {
                    id = n.MaNhaCungCap,
                    name = n.TenNhaCungCap,
                    contact = n.ThongTinLienHe
                })
                .OrderBy(n => n.name)
                .ToListAsync();

            return Json(suppliers);
        }

        private bool NhaCungCapExists(int id)
        {
            return _context.NhaCungCap.Any(e => e.MaNhaCungCap == id);
        }
    }
}