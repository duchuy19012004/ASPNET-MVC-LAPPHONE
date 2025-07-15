using Microsoft.EntityFrameworkCore;
using phonev2.Repository;
using phonev2.Models;

namespace phonev2.Services.PhieuSua
{
    public class PhieuSuaStatisticsService : IPhieuSuaStatisticsService
    {
        private readonly PhoneLapDbContext _context;

        public PhieuSuaStatisticsService(PhoneLapDbContext context)
        {
            _context = context;
        }

        public async Task<PhieuSuaStatistics> GetStatisticsAsync()
        {
            var allPhieuSua = _context.PhieuSua.AsQueryable();

            var statistics = new PhieuSuaStatistics
            {
                TotalPhieuSua = await allPhieuSua.CountAsync(),
                TiepNhan = await allPhieuSua.CountAsync(p => p.TrangThai == TrangThaiPhieuSua.TiepNhan),
                DangSua = await allPhieuSua.CountAsync(p => p.TrangThai == TrangThaiPhieuSua.DangSua),
                HoanThanh = await allPhieuSua.CountAsync(p => p.TrangThai == TrangThaiPhieuSua.HoanThanh),
                Huy = await allPhieuSua.CountAsync(p => p.TrangThai == TrangThaiPhieuSua.Huy),
                MoiTrong30Ngay = await allPhieuSua.CountAsync(p => p.NgaySua >= DateTime.Now.AddDays(-30)),
                TongTienPhieuSua = await allPhieuSua
                    .Where(p => p.TrangThai == TrangThaiPhieuSua.HoanThanh)
                    .SumAsync(p => p.TongTien ?? 0)
            };

            return statistics;
        }

        public async Task<List<object>> GetMonthlyStatisticsAsync()
        {
            var allPhieuSua = _context.PhieuSua.AsQueryable();
            var thongKeTheoThang = new List<object>();
            
            for (int i = 11; i >= 0; i--)
            {
                var thang = DateTime.Now.AddMonths(-i);
                var startOfMonth = new DateTime(thang.Year, thang.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
                
                var soPhieuTrongThang = await allPhieuSua.CountAsync(p => 
                    p.NgaySua >= startOfMonth && p.NgaySua <= endOfMonth);
                
                thongKeTheoThang.Add(new { 
                    thang = thang.ToString("MM/yyyy"), 
                    soPhieu = soPhieuTrongThang 
                });
            }

            return thongKeTheoThang;
        }

        public async Task<List<object>> GetMonthlyRevenueStatisticsAsync()
        {
            var allPhieuSua = _context.PhieuSua.AsQueryable();
            var thongKeTienTheoThang = new List<object>();
            
            for (int i = 11; i >= 0; i--)
            {
                var thang = DateTime.Now.AddMonths(-i);
                var startOfMonth = new DateTime(thang.Year, thang.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
                
                var tongTienTrongThang = await allPhieuSua
                    .Where(p => p.TrangThai == TrangThaiPhieuSua.HoanThanh && 
                               p.NgaySua >= startOfMonth && p.NgaySua <= endOfMonth)
                    .SumAsync(p => p.TongTien ?? 0);
                
                thongKeTienTheoThang.Add(new { 
                    thang = thang.ToString("MM/yyyy"), 
                    tongTien = tongTienTrongThang 
                });
            }

            return thongKeTienTheoThang;
        }
    }
} 