using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using phonev2.Models;
using phonev2.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace phonev2.Controllers
{
    public class BaoCaoController : Controller
    {
        private readonly PhoneLapDbContext _context;

        public BaoCaoController(PhoneLapDbContext context)
        {
            _context = context;
        }

        // GET: BaoCao/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var today = DateTime.Today;
                var startOfMonth = new DateTime(today.Year, today.Month, 1);
                var startOfYear = new DateTime(today.Year, 1, 1);

                // Thống kê tổng quan từ database PhieuSua
                var tongPhieuSua = await _context.PhieuSua.CountAsync();
                var tongDoanhThu = await _context.PhieuSua
                    .Where(p => p.TrangThai == TrangThaiPhieuSua.HoanThanh)
                    .SumAsync(p => p.TongTien ?? 0);
                var phieuSuaThangNay = await _context.PhieuSua
                    .CountAsync(p => p.NgaySua >= startOfMonth);
                var doanhThuThangNay = await _context.PhieuSua
                    .Where(p => p.TrangThai == TrangThaiPhieuSua.HoanThanh && p.NgaySua >= startOfMonth)
                    .SumAsync(p => p.TongTien ?? 0);

                // Thống kê theo trạng thái từ database
                var thongKeTrangThai = await _context.PhieuSua
                    .GroupBy(p => p.TrangThai)
                    .Select(g => new { 
                        TrangThai = g.Key, 
                        SoLuong = g.Count() 
                    })
                    .ToListAsync();

                // Thêm TrangThaiText sau khi query
                var thongKeTrangThaiWithText = thongKeTrangThai.Select(x => new {
                    x.TrangThai,
                    TrangThaiText = GetDisplayName(x.TrangThai),
                    x.SoLuong
                }).Cast<object>().ToList();

                // Thống kê theo nhân viên từ database
                var thongKeNhanVien = await _context.PhieuSua
                    .Where(p => p.MaNhanVien.HasValue && p.TrangThai == TrangThaiPhieuSua.HoanThanh)
                    .GroupBy(p => p.MaNhanVien)
                    .Select(g => new
                    {
                        MaNhanVien = g.Key,
                        HoTen = _context.NhanVien.Where(nv => nv.MaNhanVien == g.Key).Select(nv => nv.HoTen).FirstOrDefault(),
                        SoPhieuSua = g.Count(),
                        TongDoanhThu = g.Sum(p => p.TongTien ?? 0)
                    })
                    .OrderByDescending(x => x.TongDoanhThu)
                    .Take(10)
                    .Cast<object>()
                    .ToListAsync();

                // Thống kê theo tháng (12 tháng gần nhất) từ database
                var thongKeTheoThang = new List<object>();
                for (int i = 11; i >= 0; i--)
                {
                    var thang = today.AddMonths(-i);
                    var startOfMonthStat = new DateTime(thang.Year, thang.Month, 1);
                    var endOfMonthStat = startOfMonthStat.AddMonths(1).AddDays(-1);

                    // Lấy tất cả phiếu sửa trong tháng (không phân biệt trạng thái)
                    var soPhieuSua = await _context.PhieuSua
                        .CountAsync(p => p.NgaySua >= startOfMonthStat && p.NgaySua <= endOfMonthStat);
                    
                    // Lấy doanh thu từ tất cả phiếu (không chỉ hoàn thành)
                    var doanhThu = await _context.PhieuSua
                        .Where(p => p.NgaySua >= startOfMonthStat && p.NgaySua <= endOfMonthStat)
                        .SumAsync(p => p.TongTien ?? 0);

                    thongKeTheoThang.Add(new
                    {
                        Thang = thang.ToString("MM/yyyy"),
                        SoPhieuSua = soPhieuSua,
                        DoanhThu = doanhThu
                    });
                }

                // Thống kê theo ngày (30 ngày gần nhất) từ database
                var thongKeTheoNgay = new List<object>();
                for (int i = 29; i >= 0; i--)
                {
                    var ngay = today.AddDays(-i);
                    var soPhieuSua = await _context.PhieuSua
                        .CountAsync(p => p.NgaySua.Date == ngay);
                    var doanhThu = await _context.PhieuSua
                        .Where(p => p.NgaySua.Date == ngay)
                        .SumAsync(p => p.TongTien ?? 0);

                    thongKeTheoNgay.Add(new
                    {
                        Ngay = ngay.ToString("dd/MM"),
                        SoPhieuSua = soPhieuSua,
                        DoanhThu = doanhThu
                    });
                }

                // Debug: Log data to console
                System.Diagnostics.Debug.WriteLine($"=== BAO CAO DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"TongPhieuSua: {tongPhieuSua}");
                System.Diagnostics.Debug.WriteLine($"TongDoanhThu: {tongDoanhThu}");
                System.Diagnostics.Debug.WriteLine($"PhieuSuaThangNay: {phieuSuaThangNay}");
                System.Diagnostics.Debug.WriteLine($"DoanhThuThangNay: {doanhThuThangNay}");
                System.Diagnostics.Debug.WriteLine($"ThongKeTheoThang Count: {thongKeTheoThang.Count}");
                System.Diagnostics.Debug.WriteLine($"ThongKeTrangThai Count: {thongKeTrangThaiWithText.Count}");
                System.Diagnostics.Debug.WriteLine($"ThongKeNhanVien Count: {thongKeNhanVien.Count}");
                System.Diagnostics.Debug.WriteLine($"ThongKeTheoNgay Count: {thongKeTheoNgay.Count}");

                // Log chi tiết dữ liệu thống kê theo tháng
                foreach (var item in thongKeTheoThang)
                {
                    dynamic data = item;
                    System.Diagnostics.Debug.WriteLine($"Thang: {data.Thang}, SoPhieuSua: {data.SoPhieuSua}, DoanhThu: {data.DoanhThu}");
                }

                ViewBag.TongPhieuSua = tongPhieuSua;
                ViewBag.TongDoanhThu = tongDoanhThu;
                ViewBag.PhieuSuaThangNay = phieuSuaThangNay;
                ViewBag.DoanhThuThangNay = doanhThuThangNay;
                ViewBag.ThongKeTrangThai = thongKeTrangThaiWithText;
                ViewBag.ThongKeNhanVien = thongKeNhanVien;
                ViewBag.ThongKeTheoThang = thongKeTheoThang;
                ViewBag.ThongKeTheoNgay = thongKeTheoNgay;

                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR in BaoCao/Index: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                
                // Trả về dữ liệu rỗng nếu có lỗi
                ViewBag.TongPhieuSua = 0;
                ViewBag.TongDoanhThu = 0;
                ViewBag.PhieuSuaThangNay = 0;
                ViewBag.DoanhThuThangNay = 0;
                ViewBag.ThongKeTrangThai = new List<object>();
                ViewBag.ThongKeNhanVien = new List<object>();
                ViewBag.ThongKeTheoThang = new List<object>();
                ViewBag.ThongKeTheoNgay = new List<object>();
                
                return View();
            }
        }

        // GET: BaoCao/ChiTiet
        public async Task<IActionResult> ChiTiet(DateTime? tuNgay = null, DateTime? denNgay = null, int? maNhanVien = null, string trangThai = "")
        {
            var query = _context.PhieuSua
                .Include(p => p.ChiTietPhieuSuas).ThenInclude(ct => ct.DichVu)
                .Include(p => p.ChiTietLinhKiens).ThenInclude(ct => ct.LinhKien)
                .AsQueryable();

            // Filter theo ngày
            if (tuNgay.HasValue)
            {
                query = query.Where(p => p.NgaySua >= tuNgay.Value);
            }
            if (denNgay.HasValue)
            {
                query = query.Where(p => p.NgaySua <= denNgay.Value.AddDays(1).AddSeconds(-1));
            }

            // Filter theo nhân viên
            if (maNhanVien.HasValue)
            {
                query = query.Where(p => p.MaNhanVien == maNhanVien.Value);
            }

            // Filter theo trạng thái
            if (!string.IsNullOrEmpty(trangThai) && Enum.TryParse<TrangThaiPhieuSua>(trangThai, out var trangThaiEnum))
            {
                query = query.Where(p => p.TrangThai == trangThaiEnum);
            }

            var phieuSuas = await query
                .OrderByDescending(p => p.NgaySua)
                .ToListAsync();

            // Thống kê tổng hợp
            var tongPhieuSua = phieuSuas.Count;
            var tongDoanhThu = phieuSuas.Where(p => p.TrangThai == TrangThaiPhieuSua.HoanThanh).Sum(p => p.TongTien ?? 0);
            var phieuHoanThanh = phieuSuas.Count(p => p.TrangThai == TrangThaiPhieuSua.HoanThanh);
            var phieuDangSua = phieuSuas.Count(p => p.TrangThai == TrangThaiPhieuSua.DangSua);

            // Danh sách nhân viên cho filter
            var nhanVienList = await _context.NhanVien
                .Where(nv => nv.TrangThai)
                .Select(nv => new { nv.MaNhanVien, nv.HoTen })
                .ToListAsync();

            // Danh sách khách hàng cho filter
            var khachHangList = await _context.KhachHang
                .Where(kh => kh.TrangThai)
                .Select(kh => new { kh.MaKhachHang, kh.HoTen })
                .ToListAsync();

            ViewBag.TuNgay = tuNgay;
            ViewBag.DenNgay = denNgay;
            ViewBag.MaNhanVien = maNhanVien;
            ViewBag.TrangThai = trangThai;
            ViewBag.TongPhieuSua = tongPhieuSua;
            ViewBag.TongDoanhThu = tongDoanhThu;
            ViewBag.PhieuHoanThanh = phieuHoanThanh;
            ViewBag.PhieuDangSua = phieuDangSua;
            ViewBag.NhanVienList = nhanVienList;
            ViewBag.KhachHangList = khachHangList;
            ViewBag.TrangThaiList = Enum.GetValues<TrangThaiPhieuSua>();

            return View(phieuSuas);
        }

        // GET: BaoCao/DoanhThuTheoThoiGian
        public async Task<IActionResult> DoanhThuTheoThoiGian(string period = "month")
        {
            var today = DateTime.Today;
            var data = new List<object>();

            if (period == "month")
            {
                // Thống kê theo tháng (12 tháng gần nhất)
                for (int i = 11; i >= 0; i--)
                {
                    var thang = today.AddMonths(-i);
                    var startOfMonth = new DateTime(thang.Year, thang.Month, 1);
                    var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                    var soPhieuSua = await _context.PhieuSua
                        .CountAsync(p => p.NgaySua >= startOfMonth && p.NgaySua <= endOfMonth);
                    var doanhThu = await _context.PhieuSua
                        .Where(p => p.TrangThai == TrangThaiPhieuSua.HoanThanh && 
                                   p.NgaySua >= startOfMonth && p.NgaySua <= endOfMonth)
                        .SumAsync(p => p.TongTien ?? 0);

                    data.Add(new
                    {
                        Period = thang.ToString("MM/yyyy"),
                        SoPhieuSua = soPhieuSua,
                        DoanhThu = doanhThu
                    });
                }
            }
            else if (period == "week")
            {
                // Thống kê theo tuần (12 tuần gần nhất)
                for (int i = 11; i >= 0; i--)
                {
                    var startOfWeek = today.AddDays(-(i * 7)).AddDays(-(int)today.DayOfWeek);
                    var endOfWeek = startOfWeek.AddDays(6);

                    var soPhieuSua = await _context.PhieuSua
                        .CountAsync(p => p.NgaySua >= startOfWeek && p.NgaySua <= endOfWeek);
                    var doanhThu = await _context.PhieuSua
                        .Where(p => p.TrangThai == TrangThaiPhieuSua.HoanThanh && 
                                   p.NgaySua >= startOfWeek && p.NgaySua <= endOfWeek)
                        .SumAsync(p => p.TongTien ?? 0);

                    data.Add(new
                    {
                        Period = $"Tuần {startOfWeek.ToString("dd/MM")} - {endOfWeek.ToString("dd/MM")}",
                        SoPhieuSua = soPhieuSua,
                        DoanhThu = doanhThu
                    });
                }
            }
            else // day
            {
                // Thống kê theo ngày (30 ngày gần nhất)
                for (int i = 29; i >= 0; i--)
                {
                    var ngay = today.AddDays(-i);
                    var soPhieuSua = await _context.PhieuSua
                        .CountAsync(p => p.NgaySua.Date == ngay);
                    var doanhThu = await _context.PhieuSua
                        .Where(p => p.TrangThai == TrangThaiPhieuSua.HoanThanh && p.NgaySua.Date == ngay)
                        .SumAsync(p => p.TongTien ?? 0);

                    data.Add(new
                    {
                        Period = ngay.ToString("dd/MM"),
                        SoPhieuSua = soPhieuSua,
                        DoanhThu = doanhThu
                    });
                }
            }

            return Json(data);
        }

        // GET: BaoCao/DoanhThuTheoNhanVien
        public async Task<IActionResult> DoanhThuTheoNhanVien()
        {
            var thongKeNhanVien = await _context.PhieuSua
                .Where(p => p.MaNhanVien.HasValue && p.TrangThai == TrangThaiPhieuSua.HoanThanh)
                .GroupBy(p => p.MaNhanVien)
                .Select(g => new
                {
                    MaNhanVien = g.Key,
                    HoTen = _context.NhanVien.Where(nv => nv.MaNhanVien == g.Key).Select(nv => nv.HoTen).FirstOrDefault(),
                    SoPhieuSua = g.Count(),
                    TongDoanhThu = g.Sum(p => p.TongTien ?? 0),
                    TrungBinhDoanhThu = g.Average(p => p.TongTien ?? 0)
                })
                .OrderByDescending(x => x.TongDoanhThu)
                .ToListAsync();

            return Json(thongKeNhanVien);
        }

        // GET: BaoCao/ChiTietPhieuSua
        public async Task<IActionResult> ChiTietPhieuSua(int? id)
        {
            if (id == null) return NotFound();

            var phieuSua = await _context.PhieuSua
                .Include(p => p.ChiTietPhieuSuas).ThenInclude(ct => ct.DichVu)
                .Include(p => p.ChiTietLinhKiens).ThenInclude(ct => ct.LinhKien)
                .Include(p => p.ChiTietLinhKiens).ThenInclude(ct => ct.LinhKien).ThenInclude(lk => lk.LoaiLinhKien)
                .FirstOrDefaultAsync(m => m.MaPhieuSua == id);

            if (phieuSua == null) return NotFound();

            // Load danh sách khách hàng và nhân viên cho view
            ViewBag.KhachHangList = await _context.KhachHang
                .Select(kh => new { kh.MaKhachHang, kh.HoTen })
                .ToListAsync();
            ViewBag.NhanVienList = await _context.NhanVien
                .Select(nv => new { nv.MaNhanVien, nv.HoTen })
                .ToListAsync();

            return PartialView("_ChiTietPhieuSua", phieuSua);
        }



        // GET: BaoCao/TestData - Action để test dữ liệu
        public async Task<IActionResult> TestData()
        {
            try
            {
                var today = DateTime.Today;
                var startOfMonth = new DateTime(today.Year, today.Month, 1);

                // Kiểm tra tổng số phiếu sửa
                var tongPhieuSua = await _context.PhieuSua.CountAsync();
                
                // Kiểm tra phiếu sửa trong tháng này
                var phieuSuaThangNay = await _context.PhieuSua
                    .CountAsync(p => p.NgaySua >= startOfMonth);

                // Lấy 5 phiếu sửa gần nhất để kiểm tra
                var phieuSuaGanNhat = await _context.PhieuSua
                    .OrderByDescending(p => p.NgaySua)
                    .Take(5)
                    .Select(p => new
                    {
                        p.MaPhieuSua,
                        p.NgaySua,
                        p.TongTien,
                        p.TrangThai
                    })
                    .ToListAsync();

                // Thống kê theo trạng thái
                var thongKeTrangThai = await _context.PhieuSua
                    .GroupBy(p => p.TrangThai)
                    .Select(g => new
                    {
                        TrangThai = g.Key.ToString(),
                        SoLuong = g.Count()
                    })
                    .ToListAsync();

                var result = new
                {
                    TongPhieuSua = tongPhieuSua,
                    PhieuSuaThangNay = phieuSuaThangNay,
                    PhieuSuaGanNhat = phieuSuaGanNhat,
                    ThongKeTrangThai = thongKeTrangThai
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }

        private string GetDisplayName(TrangThaiPhieuSua trangThai)
        {
            return trangThai switch
            {
                TrangThaiPhieuSua.TiepNhan => "Tiếp nhận",
                TrangThaiPhieuSua.DangSua => "Đang sửa",
                TrangThaiPhieuSua.HoanThanh => "Hoàn thành",
                TrangThaiPhieuSua.Huy => "Hủy",
                _ => trangThai.ToString()
            };
        }
    }
} 