using phonev2.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace phonev2.Services.KhachHang
{
    public interface IKhachHangService
    {
        // CRUD
        Task<(IEnumerable<phonev2.Models.KhachHang> Items, int TotalCount, int TotalPages)> GetPagedAsync(string searchString, string customerLevelFilter, string statusFilter, string sortOrder, int page, int pageSize);
        Task<phonev2.Models.KhachHang?> GetByIdAsync(int id);
        Task<bool> CreateAsync(phonev2.Models.KhachHang khachHang);
        Task<bool> UpdateAsync(phonev2.Models.KhachHang khachHang);
        Task<bool> DeleteAsync(int id);

        // Toggle status
        Task<bool> ToggleStatusAsync(int id);

        // Update spending
        Task<bool> UpdateSpendingAsync(int id, decimal amount, string action = "add");

        // Statistics
        Task<object> GetStatisticsAsync();

        // Dropdown data
        Task<IEnumerable<SelectListItem>> GetActiveCustomersForDropdownAsync();
        Task<phonev2.Models.KhachHang?> SearchByPhoneAsync(string phone);
    }
} 