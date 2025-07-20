using Microsoft.EntityFrameworkCore;
using phonev2.Models;
using phonev2.Repository;

namespace phonev2.Services.LinhKien
{
    /// <summary>
    /// Service chính cho việc quản lý linh kiện
    /// </summary>
    public class LinhKienService : ILinhKienService
    {
        private readonly PhoneLapDbContext _context;

        public LinhKienService(PhoneLapDbContext context)
        {
            _context = context;
        }

        // === PHƯƠNG THỨC CRUD CƠ BẢN ===

        public async Task<IEnumerable<phonev2.Models.LinhKien>> GetAllAsync()
        {
            return await _context.LinhKien
                .Include(l => l.LoaiLinhKien)
                .Where(l => !l.DaXoa) // Chỉ lấy linh kiện chưa bị xóa
                .OrderBy(l => l.TenLinhKien)
                .ToListAsync();
        }

        public async Task<phonev2.Models.LinhKien?> GetByIdAsync(int id)
        {
            return await _context.LinhKien
                .Include(l => l.LoaiLinhKien)
                .FirstOrDefaultAsync(l => l.MaLinhKien == id);
        }

        // Thêm phương thức để lấy cả linh kiện đã xóa
        public async Task<phonev2.Models.LinhKien?> GetByIdIncludeDeletedAsync(int id)
        {
            return await _context.LinhKien
                .Include(l => l.LoaiLinhKien)
                .FirstOrDefaultAsync(l => l.MaLinhKien == id);
        }

        public async Task<bool> CreateAsync(phonev2.Models.LinhKien linhKien)
        {
            try
            {
                linhKien.NgayTao = DateTime.Now;
                linhKien.DaXoa = false; // Đảm bảo linh kiện mới không bị đánh dấu xóa
                _context.Add(linhKien);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(phonev2.Models.LinhKien linhKien)
        {
            try
            {
                _context.Update(linhKien);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Cập nhật phương thức Delete để sử dụng soft delete
        public async Task<bool> DeleteAsync(int id, string? lyDoXoa = "")
        {
            try
            {
                var linhKien = await _context.LinhKien.FindAsync(id);
                if (linhKien != null)
                {
                    // Sử dụng soft delete thay vì xóa thực sự
                    linhKien.SoftDelete(string.IsNullOrEmpty(lyDoXoa) ? "Xóa bởi người dùng" : lyDoXoa);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Thêm phương thức để xóa thực sự (hard delete) - chỉ dùng khi cần thiết
        public async Task<bool> HardDeleteAsync(int id)
        {
            try
            {
                var linhKien = await _context.LinhKien.FindAsync(id);
                if (linhKien != null)
                {
                    _context.LinhKien.Remove(linhKien);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Thêm phương thức để khôi phục linh kiện đã xóa
        public async Task<bool> RestoreAsync(int id)
        {
            try
            {
                var linhKien = await _context.LinhKien.FindAsync(id);
                if (linhKien != null && linhKien.DaXoa)
                {
                    linhKien.Restore();
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Lấy danh sách linh kiện đã xóa
        public async Task<IEnumerable<phonev2.Models.LinhKien>> GetDeletedAsync()
        {
            return await _context.LinhKien
                .Include(l => l.LoaiLinhKien)
                .Where(l => l.DaXoa)
                .OrderByDescending(l => l.NgayXoa)
                .ToListAsync();
        }

        public async Task<bool> ToggleStatusAsync(int id)
        {
            try
            {
                var linhKien = await _context.LinhKien.FindAsync(id);
                if (linhKien != null && !linhKien.DaXoa) // Chỉ toggle status cho linh kiện chưa bị xóa
                {
                    linhKien.TrangThai = !linhKien.TrangThai;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // === PHƯƠNG THỨC TÌM KIẾM VÀ PHÂN TRANG ===

        public async Task<(IEnumerable<phonev2.Models.LinhKien> Items, int TotalCount, int TotalPages)> GetPagedAsync(
            string searchString, string categoryFilter, string stockFilter, string sortOrder, int page, int pageSize)
        {
            var query = _context.LinhKien.Include(l => l.LoaiLinhKien)
                .Where(l => !l.DaXoa); // Chỉ lấy linh kiện chưa bị xóa

            // Tìm kiếm
            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(l => l.TenLinhKien.Contains(searchString) 
                                     || (!string.IsNullOrEmpty(l.HangSanXuat) && l.HangSanXuat.Contains(searchString))
                                     || (!string.IsNullOrEmpty(l.ThongSoKyThuat) && l.ThongSoKyThuat.Contains(searchString)));
            }

            // Lọc theo loại
            if (!String.IsNullOrEmpty(categoryFilter) && int.TryParse(categoryFilter, out int categoryId))
            {
                query = query.Where(l => l.MaLoaiLinhKien == categoryId);
            }

            // Lọc theo tồn kho
            if (!String.IsNullOrEmpty(stockFilter))
            {
                switch (stockFilter)
                {
                    case "out":
                        query = query.Where(l => l.SoLuongTon == 0);
                        break;
                    case "low":
                        query = query.Where(l => l.SoLuongTon > 0 && l.SoLuongTon <= 5);
                        break;
                    case "normal":
                        query = query.Where(l => l.SoLuongTon > 5 && l.SoLuongTon <= 20);
                        break;
                    case "good":
                        query = query.Where(l => l.SoLuongTon > 20);
                        break;
                }
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "name_desc":
                    query = query.OrderByDescending(l => l.TenLinhKien);
                    break;
                case "price":
                    query = query.OrderBy(l => l.GiaBan);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(l => l.GiaBan);
                    break;
                case "stock":
                    query = query.OrderBy(l => l.SoLuongTon);
                    break;
                case "stock_desc":
                    query = query.OrderByDescending(l => l.SoLuongTon);
                    break;
                case "category":
                    query = query.OrderBy(l => l.LoaiLinhKien!.TenLoaiLinhKien);
                    break;
                case "category_desc":
                    query = query.OrderByDescending(l => l.LoaiLinhKien!.TenLoaiLinhKien);
                    break;
                case "date":
                    query = query.OrderBy(l => l.NgayTao);
                    break;
                case "date_desc":
                    query = query.OrderByDescending(l => l.NgayTao);
                    break;
                default:
                    query = query.OrderBy(l => l.TenLinhKien);
                    break;
            }

            var totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return (items, totalCount, totalPages);
        }

        // === PHƯƠNG THỨC VALIDATION ===

        public async Task<bool> IsExistsAsync(string tenLinhKien, int maLoaiLinhKien, string? hangSanXuat, int? excludeId = null)
        {
            var query = _context.LinhKien.Where(l => l.TenLinhKien.ToLower() == tenLinhKien.ToLower() 
                                                 && l.MaLoaiLinhKien == maLoaiLinhKien
                                                 && !l.DaXoa); // Chỉ kiểm tra linh kiện chưa bị xóa

            if (!string.IsNullOrEmpty(hangSanXuat))
            {
                query = query.Where(l => l.HangSanXuat != null && l.HangSanXuat.ToLower() == hangSanXuat.ToLower());
            }

            if (excludeId.HasValue)
            {
                query = query.Where(l => l.MaLinhKien != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        // === PHƯƠNG THỨC QUẢN LÝ TỒN KHO ===

        public async Task<bool> UpdateStockAsync(int id, int quantity, string action)
        {
            try
            {
                var linhKien = await _context.LinhKien.FindAsync(id);
                if (linhKien == null || linhKien.DaXoa) return false; // Không cho phép cập nhật linh kiện đã xóa

                switch (action.ToLower())
                {
                    case "set":
                        linhKien.SoLuongTon = quantity;
                        break;
                    case "add":
                        linhKien.SoLuongTon += quantity;
                        break;
                    case "subtract":
                        linhKien.SoLuongTon = Math.Max(0, linhKien.SoLuongTon - quantity);
                        break;
                    default:
                        return false;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // === PHƯƠNG THỨC TÌM KIẾM AJAX ===

        public async Task<IEnumerable<object>> SearchAjaxAsync(string term, int? categoryId = null)
        {
            var query = _context.LinhKien
                .Include(l => l.LoaiLinhKien)
                .Where(l => !l.DaXoa && l.TrangThai && l.SoLuongTon > 0); // Chỉ tìm linh kiện chưa xóa, đang bán và còn hàng

            if (!string.IsNullOrEmpty(term))
            {
                query = query.Where(l => l.TenLinhKien.Contains(term) || 
                                        (l.HangSanXuat != null && l.HangSanXuat.Contains(term)));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(l => l.MaLoaiLinhKien == categoryId.Value);
            }

            return await query
                .Select(l => new { 
                    id = l.MaLinhKien, 
                    text = l.TenLinhKien + (string.IsNullOrEmpty(l.HangSanXuat) ? "" : " - " + l.HangSanXuat),
                    price = l.GiaBan,
                    stock = l.SoLuongTon,
                    category = l.LoaiLinhKien!.TenLoaiLinhKien
                })
                .Take(10)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetByCategoryAsync(int categoryId)
        {
            return await _context.LinhKien
                .Include(l => l.LoaiLinhKien)
                .Where(l => l.MaLoaiLinhKien == categoryId && !l.DaXoa && l.TrangThai && l.SoLuongTon > 0)
                .Select(l => new { 
                    id = l.MaLinhKien, 
                    name = l.TenLinhKien,
                    brand = l.HangSanXuat,
                    price = l.GiaBan,
                    stock = l.SoLuongTon
                })
                .OrderBy(l => l.name)
                .ToListAsync();
        }

        // === PHƯƠNG THỨC DROPDOWN DATA ===

        public async Task<IEnumerable<phonev2.Models.LoaiLinhKien>> GetLoaiLinhKienForDropdownAsync()
        {
            return await _context.LoaiLinhKien
                .Where(l => l.TrangThai)
                .OrderBy(l => l.TenLoaiLinhKien)
                .ToListAsync();
        }
    }
} 