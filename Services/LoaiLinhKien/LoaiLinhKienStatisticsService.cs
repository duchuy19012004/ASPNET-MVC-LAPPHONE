using Microsoft.EntityFrameworkCore;
using phonev2.Repository;

namespace phonev2.Services.LoaiLinhKien
{
    /// <summary>
    /// Service thống kê loại linh kiện
    /// </summary>
    public class LoaiLinhKienStatisticsService : ILoaiLinhKienStatisticsService
    {
        private readonly PhoneLapDbContext _context;

        public LoaiLinhKienStatisticsService(PhoneLapDbContext context)
        {
            _context = context;
        }

        // Đếm tổng số loại linh kiện
        public async Task<int> GetTotalCountAsync()
        {
            return await _context.LoaiLinhKien.CountAsync();
        }

        // Đếm số loại đang hoạt động
        public async Task<int> GetActiveCountAsync()
        {
            return await _context.LoaiLinhKien.CountAsync(l => l.TrangThai);
        }

        // Đếm số loại đã ngừng hoạt động
        public async Task<int> GetInactiveCountAsync()
        {
            return await _context.LoaiLinhKien.CountAsync(l => !l.TrangThai);
        }

        // Lấy thống kê tổng hợp
        public async Task<object> GetStatisticsAsync()
        {
            var total = await GetTotalCountAsync();
            var active = await GetActiveCountAsync();
            var inactive = await GetInactiveCountAsync();

            return new
            {
                Total = total,
                Active = active,
                Inactive = inactive,
                ActivePercentage = total > 0 ? Math.Round((double)active / total * 100, 2) : 0
            };
        }

        // Thống kê theo thời gian bảo hành
        public async Task<IEnumerable<object>> GetWarrantyStatisticsAsync()
        {
            var warrantyStats = await _context.LoaiLinhKien
                .Where(l => l.ThoiGianBaoHanh.HasValue)
                .GroupBy(l => l.ThoiGianBaoHanh)
                .Select(g => new
                {
                    WarrantyPeriod = g.Key,
                    Count = g.Count(),
                    Types = g.Select(l => l.TenLoaiLinhKien).ToList()
                })
                .OrderBy(x => x.WarrantyPeriod)
                .ToListAsync();

            return warrantyStats;
        }
    }
} 