using Microsoft.EntityFrameworkCore;
using phonev2.Models;
using phonev2.Repository;

namespace phonev2.Services.LinhKien
{
    /// <summary>
    /// Service cho các báo cáo và thống kê linh kiện
    /// </summary>
    public class LinhKienStatisticsService : ILinhKienStatisticsService
    {
        private readonly PhoneLapDbContext _context;

        public LinhKienStatisticsService(PhoneLapDbContext context)
        {
            _context = context;
        }

        // === BÁO CÁO TỒN KHO ===

        public async Task<IEnumerable<phonev2.Models.LinhKien>> GetLowStockItemsAsync(int threshold = 10)
        {
            return await _context.LinhKien
                .Include(l => l.LoaiLinhKien)
                .Where(l => l.SoLuongTon <= threshold && l.TrangThai)
                .OrderBy(l => l.SoLuongTon)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetStockReportByCategoryAsync()
        {
            return await _context.LinhKien
                .Include(l => l.LoaiLinhKien)
                .Where(l => l.TrangThai)
                .GroupBy(l => l.LoaiLinhKien!.TenLoaiLinhKien)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalItems = g.Count(),
                    TotalValue = g.Sum(l => l.SoLuongTon * l.GiaNhap),
                    OutOfStock = g.Count(l => l.SoLuongTon == 0),
                    LowStock = g.Count(l => l.SoLuongTon > 0 && l.SoLuongTon <= 5),
                    NormalStock = g.Count(l => l.SoLuongTon > 5 && l.SoLuongTon <= 20),
                    GoodStock = g.Count(l => l.SoLuongTon > 20)
                })
                .ToListAsync();
        }

        public async Task<object> GetStockOverviewAsync()
        {
            var totalItems = await _context.LinhKien.Where(l => l.TrangThai).CountAsync();
            var outOfStock = await _context.LinhKien.Where(l => l.TrangThai && l.SoLuongTon == 0).CountAsync();
            var lowStock = await _context.LinhKien.Where(l => l.TrangThai && l.SoLuongTon > 0 && l.SoLuongTon <= 5).CountAsync();
            var totalValue = await _context.LinhKien.Where(l => l.TrangThai).SumAsync(l => l.SoLuongTon * l.GiaNhap);

            return new
            {
                TotalItems = totalItems,
                OutOfStock = outOfStock,
                LowStock = lowStock,
                TotalValue = totalValue,
                StockHealth = totalItems > 0 ? ((totalItems - outOfStock - lowStock) * 100.0 / totalItems) : 0
            };
        }

        // === BÁO CÁO LỢI NHUẬN ===

        public async Task<IEnumerable<phonev2.Models.LinhKien>> GetProfitReportAsync()
        {
            return await _context.LinhKien
                .Include(l => l.LoaiLinhKien)
                .Where(l => l.TrangThai)
                .OrderByDescending(l => l.GiaBan - l.GiaNhap)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetProfitReportByCategoryAsync()
        {
            return await _context.LinhKien
                .Include(l => l.LoaiLinhKien)
                .Where(l => l.TrangThai)
                .GroupBy(l => l.LoaiLinhKien!.TenLoaiLinhKien)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalItems = g.Count(),
                    TotalProfit = g.Sum(l => (l.GiaBan - l.GiaNhap) * l.SoLuongTon),
                    AverageProfit = g.Average(l => l.GiaBan - l.GiaNhap),
                    TotalRevenue = g.Sum(l => l.GiaBan * l.SoLuongTon),
                    TotalCost = g.Sum(l => l.GiaNhap * l.SoLuongTon)
                })
                .OrderByDescending(x => x.TotalProfit)
                .ToListAsync();
        }

        // === THỐNG KÊ TỔNG QUAN ===

        public async Task<object> GetOverviewStatisticsAsync()
        {
            var totalItems = await _context.LinhKien.Where(l => l.TrangThai).CountAsync();
            var activeItems = await _context.LinhKien.Where(l => l.TrangThai && l.SoLuongTon > 0).CountAsync();
            var totalValue = await _context.LinhKien.Where(l => l.TrangThai).SumAsync(l => l.SoLuongTon * l.GiaNhap);
            var totalProfit = await _context.LinhKien.Where(l => l.TrangThai).SumAsync(l => (l.GiaBan - l.GiaNhap) * l.SoLuongTon);
            var categories = await _context.LinhKien
                .Include(l => l.LoaiLinhKien)
                .Where(l => l.TrangThai)
                .GroupBy(l => l.LoaiLinhKien!.TenLoaiLinhKien)
                .CountAsync();

            return new
            {
                TotalItems = totalItems,
                ActiveItems = activeItems,
                TotalValue = totalValue,
                TotalProfit = totalProfit,
                Categories = categories,
                AveragePrice = totalItems > 0 ? await _context.LinhKien.Where(l => l.TrangThai).AverageAsync(l => l.GiaBan) : 0
            };
        }

        public async Task<IEnumerable<object>> GetTimeBasedStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.LinhKien
                .Where(l => l.TrangThai && l.NgayTao >= startDate && l.NgayTao <= endDate)
                .GroupBy(l => l.NgayTao.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    ItemsAdded = g.Count(),
                    TotalValue = g.Sum(l => l.SoLuongTon * l.GiaNhap),
                    AveragePrice = g.Average(l => l.GiaBan)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();
        }
    }
} 