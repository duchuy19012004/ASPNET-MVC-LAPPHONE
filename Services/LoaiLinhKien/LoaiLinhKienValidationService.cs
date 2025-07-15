using Microsoft.EntityFrameworkCore;
using phonev2.Models;
using phonev2.Repository;

namespace phonev2.Services.LoaiLinhKien
{
    /// <summary>
    /// Service kiểm tra tính hợp lệ của loại linh kiện
    /// </summary>
    public class LoaiLinhKienValidationService : ILoaiLinhKienValidationService
    {
        private readonly PhoneLapDbContext _context;

        public LoaiLinhKienValidationService(PhoneLapDbContext context)
        {
            _context = context;
        }

        // Kiểm tra tên đã tồn tại chưa
        public async Task<bool> IsNameExistsAsync(string tenLoaiLinhKien, int? excludeId = null)
        {
            var query = _context.LoaiLinhKien
                .Where(l => l.TenLoaiLinhKien.ToLower() == tenLoaiLinhKien.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(l => l.MaLoaiLinhKien != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        // Kiểm tra có thể xóa không (không có linh kiện nào sử dụng)
        public async Task<bool> CanDeleteAsync(int id)
        {
            return !await _context.LinhKien.AnyAsync(lk => lk.MaLoaiLinhKien == id);
        }

        // Kiểm tra thời gian bảo hành hợp lệ
        public bool IsValidWarrantyPeriod(int? thoiGianBaoHanh)
        {
            return !thoiGianBaoHanh.HasValue || thoiGianBaoHanh.Value >= 0;
        }

        // Kiểm tra tên hợp lệ
        public bool IsValidName(string tenLoaiLinhKien)
        {
            return !string.IsNullOrWhiteSpace(tenLoaiLinhKien) && tenLoaiLinhKien.Trim().Length >= 2;
        }
    }
} 