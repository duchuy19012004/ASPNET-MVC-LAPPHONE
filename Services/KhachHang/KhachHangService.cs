using phonev2.Models;
using phonev2.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace phonev2.Services.KhachHang
{
    public class KhachHangService : IKhachHangService
    {
        private readonly PhoneLapDbContext _context;
        public KhachHangService(PhoneLapDbContext context)
        {
            _context = context;
        }

        // CRUD
        public async Task<(IEnumerable<phonev2.Models.KhachHang> Items, int TotalCount, int TotalPages)> GetPagedAsync(string searchString, string customerLevelFilter, string statusFilter, string sortOrder, int page, int pageSize)
        {
            var khachHangs = _context.KhachHang.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                khachHangs = khachHangs.Where(kh => kh.HoTen.Contains(searchString) || kh.SoDienThoai.Contains(searchString) || kh.DiaChi.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(customerLevelFilter))
            {
                switch (customerLevelFilter)
                {
                    case "vip": khachHangs = khachHangs.Where(kh => kh.TongChiTieu >= 50000000); break;
                    case "loyal": khachHangs = khachHangs.Where(kh => kh.TongChiTieu >= 20000000 && kh.TongChiTieu < 50000000); break;
                    case "silver": khachHangs = khachHangs.Where(kh => kh.TongChiTieu >= 10000000 && kh.TongChiTieu < 20000000); break;
                    case "bronze": khachHangs = khachHangs.Where(kh => kh.TongChiTieu >= 5000000 && kh.TongChiTieu < 10000000); break;
                    case "normal": khachHangs = khachHangs.Where(kh => kh.TongChiTieu < 5000000); break;
                }
            }
            if (!string.IsNullOrEmpty(statusFilter))
            {
                switch (statusFilter)
                {
                    case "active": khachHangs = khachHangs.Where(kh => kh.TrangThai); break;
                    case "blocked": khachHangs = khachHangs.Where(kh => !kh.TrangThai); break;
                    case "new": var thirtyDaysAgo = DateTime.Now.AddDays(-30); khachHangs = khachHangs.Where(kh => kh.NgayTao >= thirtyDaysAgo); break;
                }
            }
            switch (sortOrder)
            {
                case "name_desc": khachHangs = khachHangs.OrderByDescending(kh => kh.HoTen); break;
                case "spending": khachHangs = khachHangs.OrderBy(kh => kh.TongChiTieu); break;
                case "spending_desc": khachHangs = khachHangs.OrderByDescending(kh => kh.TongChiTieu); break;
                case "date": khachHangs = khachHangs.OrderBy(kh => kh.NgayTao); break;
                case "date_desc": khachHangs = khachHangs.OrderByDescending(kh => kh.NgayTao); break;
                case "level": khachHangs = khachHangs.OrderBy(kh => kh.TongChiTieu); break;
                case "level_desc": khachHangs = khachHangs.OrderByDescending(kh => kh.TongChiTieu); break;
                default: khachHangs = khachHangs.OrderBy(kh => kh.HoTen); break;
            }
            var totalItems = await khachHangs.CountAsync();
            var items = await khachHangs.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            return (items, totalItems, totalPages);
        }
        public async Task<phonev2.Models.KhachHang?> GetByIdAsync(int id)
        {
            return await _context.KhachHang.FirstOrDefaultAsync(kh => kh.MaKhachHang == id);
        }
        public async Task<bool> CreateAsync(phonev2.Models.KhachHang khachHang)
        {
            // Kiểm tra trùng số điện thoại
            var existingByPhone = await _context.KhachHang.AnyAsync(kh => kh.SoDienThoai == khachHang.SoDienThoai);
            if (existingByPhone) return false;
            khachHang.NgayTao = DateTime.Now;
            khachHang.TongChiTieu = 0;
            _context.Add(khachHang);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateAsync(phonev2.Models.KhachHang khachHang)
        {
            // Kiểm tra trùng số điện thoại (trừ chính nó)
            var existingByPhone = await _context.KhachHang.AnyAsync(kh => kh.SoDienThoai == khachHang.SoDienThoai && kh.MaKhachHang != khachHang.MaKhachHang);
            if (existingByPhone) return false;
            _context.Update(khachHang);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var khachHang = await _context.KhachHang.FindAsync(id);
            if (khachHang == null) return false;
            _context.KhachHang.Remove(khachHang);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ToggleStatusAsync(int id)
        {
            var khachHang = await _context.KhachHang.FindAsync(id);
            if (khachHang == null) return false;
            khachHang.TrangThai = !khachHang.TrangThai;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateSpendingAsync(int id, decimal amount, string action = "add")
        {
            var khachHang = await _context.KhachHang.FindAsync(id);
            if (khachHang == null) return false;
            if (action == "add") khachHang.TongChiTieu += amount;
            else if (action == "subtract") khachHang.TongChiTieu = Math.Max(0, khachHang.TongChiTieu - amount);
            else khachHang.TongChiTieu = amount;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<object> GetStatisticsAsync()
        {
            var totalCustomers = await _context.KhachHang.CountAsync();
            var activeCustomers = await _context.KhachHang.CountAsync(kh => kh.TrangThai);
            var vipCustomers = await _context.KhachHang.CountAsync(kh => kh.TongChiTieu >= 50000000);
            var newCustomers = await _context.KhachHang.CountAsync(kh => kh.NgayTao >= DateTime.Now.AddDays(-30));
            var totalSpending = await _context.KhachHang.SumAsync(kh => kh.TongChiTieu);
            var averageSpending = totalCustomers > 0 ? totalSpending / totalCustomers : 0;
            return new {
                totalCustomers,
                activeCustomers,
                vipCustomers,
                newCustomers,
                totalSpending,
                averageSpending
            };
        }
        public async Task<IEnumerable<SelectListItem>> GetActiveCustomersForDropdownAsync()
        {
            return await _context.KhachHang
                .Where(kh => kh.TrangThai)
                .OrderBy(kh => kh.HoTen)
                .Select(kh => new SelectListItem { Value = kh.MaKhachHang.ToString(), Text = kh.HoTen })
                .ToListAsync();
        }
        public async Task<phonev2.Models.KhachHang?> SearchByPhoneAsync(string phone)
        {
            return await _context.KhachHang.FirstOrDefaultAsync(kh => kh.SoDienThoai == phone);
        }
    }
} 