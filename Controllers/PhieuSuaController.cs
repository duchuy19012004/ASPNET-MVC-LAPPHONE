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

namespace phonev2.Controllers
{
    public class PhieuSuaController : Controller
    {
        private readonly PhoneLapDbContext _context;

        public PhieuSuaController(PhoneLapDbContext context)
        {
            _context = context;
        }

        // GET: PhieuSua
        public async Task<IActionResult> Index(string search = "", int page = 1, int pageSize = 10, string sort = "")
        {
            var query = _context.PhieuSua
                .Include(p => p.ChiTietPhieuSuas).ThenInclude(ct => ct.DichVu)
                .Include(p => p.ChiTietLinhKiens).ThenInclude(ct => ct.LinhKien)
                .AsQueryable();

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

            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.ToLower();
                query = query.Where(p =>
                    _context.KhachHang.Any(kh => kh.MaKhachHang == p.MaKhachHang && kh.HoTen.ToLower().Contains(s))
                    || _context.NhanVien.Any(nv => nv.MaNhanVien == p.MaNhanVien && nv.HoTen.ToLower().Contains(s))
                    || p.ChiTietPhieuSuas.Any(ct => ct.DichVu != null && ct.DichVu.TenDichVu.ToLower().Contains(s))
                );
            }
            
            // Xử lý sắp xếp theo giá riêng vì cần client evaluation
            if (sort == "price_asc" || sort == "price_desc")
            {
                return await HandlePriceSorting(query, search, page, pageSize, sort);
            }
            
            // Sắp xếp theo sort (các trường hợp khác)
            switch (sort)
            {
                case "name_asc":
                    query = query.OrderBy(p => p.MaPhieuSua); // Tăng dần mã phiếu sửa
                    break;
                case "name_desc":
                    query = query.OrderByDescending(p => p.MaPhieuSua); // Giảm dần mã phiếu sửa
                    break;
                case "oldest":
                    query = query.OrderBy(p => p.NgaySua); // Cũ nhất
                    break;
                case "newest":
                    query = query.OrderByDescending(p => p.NgaySua); // Mới nhất
                    break;
                default:
                    query = query.OrderByDescending(p => p.MaPhieuSua);
                    break;
            }
            int totalCount = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var list = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            ViewBag.KhachHangList = _context.KhachHang.Select(kh => new SelectListItem { Value = kh.MaKhachHang.ToString(), Text = kh.HoTen }).ToList();
            ViewBag.NhanVienList = _context.NhanVien.Select(nv => new SelectListItem { Value = nv.MaNhanVien.ToString(), Text = nv.HoTen }).ToList();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.Search = search;
            ViewBag.Sort = sort;
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // Đảm bảo PartialView cũng có đủ ViewBag
                ViewBag.KhachHangList = _context.KhachHang.Select(kh => new SelectListItem { Value = kh.MaKhachHang.ToString(), Text = kh.HoTen }).ToList();
                ViewBag.NhanVienList = _context.NhanVien.Select(nv => new SelectListItem { Value = nv.MaNhanVien.ToString(), Text = nv.HoTen }).ToList();
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalCount;
                ViewBag.Search = search;
                ViewBag.Sort = sort;
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

        // GET: PhieuSua/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var phieuSua = await _context.PhieuSua
                .Include(p => p.ChiTietPhieuSuas).ThenInclude(ct => ct.DichVu)
                .Include(p => p.ChiTietLinhKiens).ThenInclude(ct => ct.LinhKien)
                .FirstOrDefaultAsync(m => m.MaPhieuSua == id);
            if (phieuSua == null) return NotFound();
            ViewBag.KhachHangList = _context.KhachHang.Select(kh => new SelectListItem { Value = kh.MaKhachHang.ToString(), Text = kh.HoTen }).ToList();
            ViewBag.NhanVienList = _context.NhanVien.Select(nv => new SelectListItem { Value = nv.MaNhanVien.ToString(), Text = nv.HoTen }).ToList();
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("Details", phieuSua);
            }
            return View(phieuSua);
        }

        // GET: PhieuSua/Create
        public IActionResult Create()
        {
            // Kiểm tra xem có khách hàng và nhân viên không
            var khachHangCount = _context.KhachHang.Where(kh => kh.TrangThai).Count();
            var nhanVienCount = _context.NhanVien.Where(nv => nv.TrangThai).Count();
            
            System.Diagnostics.Debug.WriteLine($"Khách hàng: {khachHangCount}, Nhân viên: {nhanVienCount}");
            
            var vm = new PhieuSuaCreateViewModel
            {
                PhieuSua = new PhieuSua
                {
                    NgaySua = DateTime.Now,
                    TrangThai = TrangThaiPhieuSua.TiepNhan
                },
                DichVus = new List<DichVuChonVM>(),
                LinhKiens = new List<LinhKienChonVM>(),
                KhachHangList = _context.KhachHang.Where(kh => kh.TrangThai)
                    .Select(kh => new SelectListItem { Value = kh.MaKhachHang.ToString(), Text = kh.HoTen }).ToList(),
                NhanVienList = _context.NhanVien.Where(nv => nv.TrangThai)
                    .Select(nv => new SelectListItem { Value = nv.MaNhanVien.ToString(), Text = nv.HoTen }).ToList(),
                DichVuList = _context.DichVu.Where(dv => dv.TrangThai)
                    .Select(dv => new SelectListItem { Value = dv.MaDichVu.ToString(), Text = dv.TenDichVu }).ToList(),
                LinhKienList = _context.LinhKien.Where(lk => lk.TrangThai)
                    .Select(lk => new SelectListItem { Value = lk.MaLinhKien.ToString(), Text = lk.TenLinhKien }).ToList()
            };
            return View(vm);
        }

        // POST: PhieuSua/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhieuSuaCreateViewModel vm)
        {
            try
            {
                // Validation cho ngày hẹn trả
                if (vm.PhieuSua.NgayHenTra.HasValue && vm.PhieuSua.NgayHenTra.Value < vm.PhieuSua.NgaySua)
                {
                    ModelState.AddModelError("PhieuSua.NgayHenTra", "Ngày hẹn trả phải lớn hơn hoặc bằng ngày sửa");
                }

                if (ModelState.IsValid)
                {
                    // Đảm bảo ngày sửa được set
                    if (vm.PhieuSua.NgaySua == default)
                    {
                        vm.PhieuSua.NgaySua = DateTime.Now;
                    }

                    // Đảm bảo trạng thái được set
                    vm.PhieuSua.TrangThai = TrangThaiPhieuSua.TiepNhan;

                    _context.PhieuSua.Add(vm.PhieuSua);
                    await _context.SaveChangesAsync();

                    // Lấy MaPhieuSua sau khi đã lưu
                    var maPhieuSua = vm.PhieuSua.MaPhieuSua;

                    // Thêm dịch vụ
                    if (vm.DichVus != null)
                    {
                        foreach (var dv in vm.DichVus)
                        {
                            if (dv.MaDichVu > 0)
                            {
                                _context.ChiTietPhieuSua.Add(new ChiTietPhieuSua
                                {
                                    MaPhieuSua = maPhieuSua,
                                    MaDichVu = dv.MaDichVu,
                                    SoLuong = 1 // Mặc định 1, vì không có trường SoLuong
                                });
                            }
                        }
                    }

                    // Thêm linh kiện
                    if (vm.LinhKiens != null)
                    {
                        foreach (var lk in vm.LinhKiens)
                        {
                            if (lk.MaLinhKien > 0 && lk.SoLuong > 0)
                            {
                                _context.ChiTietLinhKien.Add(new ChiTietLinhKien
                                {
                                    MaPhieuSua = maPhieuSua,
                                    MaLinhKien = lk.MaLinhKien,
                                    SoLuong = lk.SoLuong
                                });
                            }
                        }
                    }

                    await _context.SaveChangesAsync();

                    // Tính toán và cập nhật tổng tiền
                    var phieuSua = await _context.PhieuSua
                        .Include(p => p.ChiTietPhieuSuas).ThenInclude(ct => ct.DichVu)
                        .Include(p => p.ChiTietLinhKiens).ThenInclude(ct => ct.LinhKien)
                        .FirstOrDefaultAsync(p => p.MaPhieuSua == maPhieuSua);

                    if (phieuSua != null)
                    {
                        decimal tongTienDichVu = phieuSua.ChiTietPhieuSuas?.Sum(ct => ct.DichVu?.GiaDichVu ?? 0) ?? 0;
                        decimal tongTienLinhKien = phieuSua.ChiTietLinhKiens?.Sum(lk => (lk.LinhKien?.GiaBan ?? 0) * lk.SoLuong) ?? 0;
                        phieuSua.TongTien = tongTienDichVu + tongTienLinhKien;
                        await _context.SaveChangesAsync();
                    }
                    
                    // Log để debug
                    System.Diagnostics.Debug.WriteLine($"Đã tạo phiếu sửa thành công: {maPhieuSua}");
                    
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Log lỗi validation
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    System.Diagnostics.Debug.WriteLine($"Validation errors: {string.Join(", ", errors)}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi tạo phiếu sửa: {ex.Message}");
                ModelState.AddModelError("", $"Có lỗi xảy ra: {ex.Message}");
            }

            // Debug: Log ModelState errors
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                System.Diagnostics.Debug.WriteLine($"ModelState errors: {string.Join(", ", errors)}");
            }

            // Nạp lại dropdown nếu lỗi
            vm.KhachHangList = _context.KhachHang.Where(kh => kh.TrangThai)
                .Select(kh => new SelectListItem { Value = kh.MaKhachHang.ToString(), Text = kh.HoTen }).ToList();
            vm.NhanVienList = _context.NhanVien.Where(nv => nv.TrangThai)
                .Select(nv => new SelectListItem { Value = nv.MaNhanVien.ToString(), Text = nv.HoTen }).ToList();
            vm.DichVuList = _context.DichVu.Where(dv => dv.TrangThai)
                .Select(dv => new SelectListItem { Value = dv.MaDichVu.ToString(), Text = dv.TenDichVu }).ToList();
            vm.LinhKienList = _context.LinhKien.Where(lk => lk.TrangThai)
                .Select(lk => new SelectListItem { Value = lk.MaLinhKien.ToString(), Text = lk.TenLinhKien }).ToList();
            
            // Đảm bảo lists không null
            if (vm.DichVus == null) vm.DichVus = new List<DichVuChonVM>();
            if (vm.LinhKiens == null) vm.LinhKiens = new List<LinhKienChonVM>();
            
            return View(vm);
        }

        // GET: PhieuSua/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var phieuSua = await _context.PhieuSua
                .Include(p => p.ChiTietPhieuSuas).ThenInclude(ct => ct.DichVu)
                .Include(p => p.ChiTietLinhKiens).ThenInclude(ct => ct.LinhKien)
                .FirstOrDefaultAsync(p => p.MaPhieuSua == id);
            if (phieuSua == null) return NotFound();
            
            // Set ViewBag cho dropdown
            ViewBag.KhachHangList = _context.KhachHang.Where(kh => kh.TrangThai)
                .Select(kh => new SelectListItem { Value = kh.MaKhachHang.ToString(), Text = kh.HoTen }).ToList();
            ViewBag.NhanVienList = _context.NhanVien.Where(nv => nv.TrangThai)
                .Select(nv => new SelectListItem { Value = nv.MaNhanVien.ToString(), Text = nv.HoTen }).ToList();
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("Edit", phieuSua);
            }
            return View(phieuSua);
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
                try
                {
                    // Clear any existing tracked entities to avoid conflicts
                    _context.ChangeTracker.Clear();
                    
                    // Lấy phiếu sửa hiện tại từ DB để so sánh trạng thái
                    var currentPhieuSua = await _context.PhieuSua.AsNoTracking().FirstOrDefaultAsync(p => p.MaPhieuSua == id);
                    if (currentPhieuSua == null)
                    {
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return Json(new { success = false, message = "Không tìm thấy phiếu sửa!" });
                        }
                        return NotFound();
                    }

                    // Tự động set NgayTraThucTe khi chuyển sang trạng thái Hoàn thành
                    if (phieuSua.TrangThai == TrangThaiPhieuSua.HoanThanh && currentPhieuSua.TrangThai != TrangThaiPhieuSua.HoanThanh)
                    {
                        phieuSua.NgayTraThucTe = DateTime.Now;
                    }
                    // Clear NgayTraThucTe nếu chuyển về trạng thái khác
                    else if (phieuSua.TrangThai != TrangThaiPhieuSua.HoanThanh && currentPhieuSua.TrangThai == TrangThaiPhieuSua.HoanThanh)
                    {
                        phieuSua.NgayTraThucTe = null;
                    }
                    // Giữ nguyên NgayTraThucTe nếu không thay đổi trạng thái hoặc vẫn là Hoàn thành
                    else if (phieuSua.TrangThai == TrangThaiPhieuSua.HoanThanh && currentPhieuSua.TrangThai == TrangThaiPhieuSua.HoanThanh)
                    {
                        phieuSua.NgayTraThucTe = currentPhieuSua.NgayTraThucTe;
                    }

                    // Tính toán và cập nhật tổng tiền
                    var phieuSuaWithDetails = await _context.PhieuSua
                        .Include(p => p.ChiTietPhieuSuas).ThenInclude(ct => ct.DichVu)
                        .Include(p => p.ChiTietLinhKiens).ThenInclude(ct => ct.LinhKien)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.MaPhieuSua == id);

                    if (phieuSuaWithDetails != null)
                    {
                        decimal tongTienDichVu = phieuSuaWithDetails.ChiTietPhieuSuas?.Sum(ct => ct.DichVu?.GiaDichVu ?? 0) ?? 0;
                        decimal tongTienLinhKien = phieuSuaWithDetails.ChiTietLinhKiens?.Sum(lk => (lk.LinhKien?.GiaBan ?? 0) * lk.SoLuong) ?? 0;
                        phieuSua.TongTien = tongTienDichVu + tongTienLinhKien;
                    }

                    // Attach the entity and mark it as modified
                    _context.Attach(phieuSua);
                    _context.Entry(phieuSua).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Cập nhật phiếu sửa thành công!" });
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.PhieuSua.Any(e => e.MaPhieuSua == id))
                    {
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return Json(new { success = false, message = "Không tìm thấy phiếu sửa!" });
                        }
                        return NotFound();
                    }
                    else
                    {
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật!" });
                        }
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
                    }
                    throw;
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
            ViewBag.KhachHangList = _context.KhachHang.Where(kh => kh.TrangThai)
                .Select(kh => new SelectListItem { Value = kh.MaKhachHang.ToString(), Text = kh.HoTen }).ToList();
            ViewBag.NhanVienList = _context.NhanVien.Where(nv => nv.TrangThai)
                .Select(nv => new SelectListItem { Value = nv.MaNhanVien.ToString(), Text = nv.HoTen }).ToList();
            return View(phieuSua);
        }

        // GET: PhieuSua/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var phieuSua = await _context.PhieuSua.FindAsync(id);
            if (phieuSua == null) return NotFound();
            return View(phieuSua);
        }

        // POST: PhieuSua/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phieuSua = await _context.PhieuSua.FindAsync(id);
            if (phieuSua != null)
            {
                _context.PhieuSua.Remove(phieuSua);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
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
                var allPhieuSua = await _context.PhieuSua
                    .Include(p => p.ChiTietPhieuSuas).ThenInclude(ct => ct.DichVu)
                    .Include(p => p.ChiTietLinhKiens).ThenInclude(ct => ct.LinhKien)
                    .ToListAsync();

                int updatedCount = 0;
                foreach (var phieuSua in allPhieuSua)
                {
                    decimal tongTienDichVu = phieuSua.ChiTietPhieuSuas?.Sum(ct => ct.DichVu?.GiaDichVu ?? 0) ?? 0;
                    decimal tongTienLinhKien = phieuSua.ChiTietLinhKiens?.Sum(lk => (lk.LinhKien?.GiaBan ?? 0) * lk.SoLuong) ?? 0;
                    decimal newTongTien = tongTienDichVu + tongTienLinhKien;
                    
                    if (phieuSua.TongTien != newTongTien)
                    {
                        phieuSua.TongTien = newTongTien;
                        updatedCount++;
                    }
                }

                if (updatedCount > 0)
                {
                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true, message = $"Đã cập nhật tổng tiền cho {updatedCount} phiếu sửa." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
    }
}
