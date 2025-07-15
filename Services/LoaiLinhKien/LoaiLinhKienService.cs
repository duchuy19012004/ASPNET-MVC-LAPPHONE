using Microsoft.EntityFrameworkCore;
using phonev2.Models;
using phonev2.Repository;

namespace phonev2.Services.LoaiLinhKien
{
    /// <summary>
    /// Service chính quản lý loại linh kiện
    /// </summary>
    public class LoaiLinhKienService : ILoaiLinhKienService
    {
        private readonly PhoneLapDbContext _context;
        private readonly ILoaiLinhKienValidationService _validationService;

        public LoaiLinhKienService(
            PhoneLapDbContext context,
            ILoaiLinhKienValidationService validationService)
        {
            _context = context;
            _validationService = validationService;
        }

        // Lấy tất cả loại linh kiện
        public async Task<IEnumerable<phonev2.Models.LoaiLinhKien>> GetAllAsync()
        {
            return await _context.LoaiLinhKien
                .OrderBy(l => l.TenLoaiLinhKien)
                .ToListAsync();
        }

        // Lấy theo ID
        public async Task<phonev2.Models.LoaiLinhKien?> GetByIdAsync(int id)
        {
            return await _context.LoaiLinhKien.FindAsync(id);
        }

        // Tạo mới
        public async Task<bool> CreateAsync(phonev2.Models.LoaiLinhKien loaiLinhKien)
        {
            try
            {
                // Kiểm tra validation
                if (!_validationService.IsValidName(loaiLinhKien.TenLoaiLinhKien))
                {
                    return false;
                }

                if (await _validationService.IsNameExistsAsync(loaiLinhKien.TenLoaiLinhKien))
                {
                    return false;
                }

                if (!_validationService.IsValidWarrantyPeriod(loaiLinhKien.ThoiGianBaoHanh))
                {
                    return false;
                }

                // Set ngày tạo
                loaiLinhKien.NgayTao = DateTime.Now;

                _context.Add(loaiLinhKien);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Cập nhật
        public async Task<bool> UpdateAsync(phonev2.Models.LoaiLinhKien loaiLinhKien)
        {
            try
            {
                // Kiểm tra validation
                if (!_validationService.IsValidName(loaiLinhKien.TenLoaiLinhKien))
                {
                    return false;
                }

                if (await _validationService.IsNameExistsAsync(loaiLinhKien.TenLoaiLinhKien, loaiLinhKien.MaLoaiLinhKien))
                {
                    return false;
                }

                if (!_validationService.IsValidWarrantyPeriod(loaiLinhKien.ThoiGianBaoHanh))
                {
                    return false;
                }

                _context.Update(loaiLinhKien);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Xóa
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                if (!await _validationService.CanDeleteAsync(id))
                {
                    return false;
                }

                var loaiLinhKien = await _context.LoaiLinhKien.FindAsync(id);
                if (loaiLinhKien == null)
                {
                    return false;
                }

                _context.LoaiLinhKien.Remove(loaiLinhKien);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Bật/tắt trạng thái
        public async Task<bool> ToggleStatusAsync(int id)
        {
            try
            {
                var loaiLinhKien = await _context.LoaiLinhKien.FindAsync(id);
                if (loaiLinhKien == null)
                {
                    return false;
                }

                loaiLinhKien.TrangThai = !loaiLinhKien.TrangThai;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Lấy danh sách có phân trang, tìm kiếm, sắp xếp
        public async Task<(IEnumerable<phonev2.Models.LoaiLinhKien> Items, int TotalCount, int TotalPages)> GetPagedAsync(
            string searchString, string sortOrder, int page, int pageSize)
        {
            var query = _context.LoaiLinhKien.AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(l => l.TenLoaiLinhKien.Contains(searchString) ||
                                       (!string.IsNullOrEmpty(l.MoTa) && l.MoTa.Contains(searchString)));
            }

            // Sắp xếp
            query = sortOrder switch
            {
                "name_desc" => query.OrderByDescending(l => l.TenLoaiLinhKien),
                "warranty" => query.OrderBy(l => l.ThoiGianBaoHanh ?? 0),
                "warranty_desc" => query.OrderByDescending(l => l.ThoiGianBaoHanh ?? 0),
                "date" => query.OrderBy(l => l.NgayTao),
                "date_desc" => query.OrderByDescending(l => l.NgayTao),
                _ => query.OrderBy(l => l.TenLoaiLinhKien)
            };

            // Đếm tổng số
            var totalCount = await query.CountAsync();

            // Phân trang
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return (items, totalCount, totalPages);
        }

        // Kiểm tra tên tồn tại
        public async Task<bool> IsNameExistsAsync(string tenLoaiLinhKien, int? excludeId = null)
        {
            return await _validationService.IsNameExistsAsync(tenLoaiLinhKien, excludeId);
        }

        // Tìm kiếm cho AJAX
        public async Task<IEnumerable<object>> SearchAjaxAsync(string term)
        {
            return await _context.LoaiLinhKien
                .Where(l => l.TenLoaiLinhKien.Contains(term) && l.TrangThai)
                .Select(l => new
                {
                    id = l.MaLoaiLinhKien,
                    text = l.TenLoaiLinhKien,
                    warranty = l.ThoiGianBaoHanhText
                })
                .Take(10)
                .ToListAsync();
        }
    }
} 