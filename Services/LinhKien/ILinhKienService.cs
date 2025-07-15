using phonev2.Models;

namespace phonev2.Services.LinhKien
{
    /// <summary>
    /// Interface chính cho việc quản lý linh kiện
    /// Chứa tất cả các phương thức CRUD và business logic cơ bản
    /// </summary>
    public interface ILinhKienService
    {
        // === PHƯƠNG THỨC CRUD CƠ BẢN ===
        
        /// <summary>
        /// Lấy tất cả linh kiện với thông tin loại linh kiện
        /// </summary>
        Task<IEnumerable<phonev2.Models.LinhKien>> GetAllAsync();
        
        /// <summary>
        /// Lấy linh kiện theo ID với thông tin loại linh kiện
        /// </summary>
        Task<phonev2.Models.LinhKien?> GetByIdAsync(int id);
        
        /// <summary>
        /// Tạo mới linh kiện
        /// </summary>
        Task<bool> CreateAsync(phonev2.Models.LinhKien linhKien);
        
        /// <summary>
        /// Cập nhật thông tin linh kiện
        /// </summary>
        Task<bool> UpdateAsync(phonev2.Models.LinhKien linhKien);
        
        /// <summary>
        /// Xóa linh kiện
        /// </summary>
        Task<bool> DeleteAsync(int id);
        
        /// <summary>
        /// Bật/tắt trạng thái hoạt động của linh kiện
        /// </summary>
        Task<bool> ToggleStatusAsync(int id);

        // === PHƯƠNG THỨC TÌM KIẾM VÀ PHÂN TRANG ===
        
        /// <summary>
        /// Lấy danh sách linh kiện có phân trang, tìm kiếm, lọc và sắp xếp
        /// </summary>
        Task<(IEnumerable<phonev2.Models.LinhKien> Items, int TotalCount, int TotalPages)> GetPagedAsync(
            string searchString, string categoryFilter, string stockFilter, string sortOrder, int page, int pageSize);

        // === PHƯƠNG THỨC VALIDATION ===
        
        /// <summary>
        /// Kiểm tra xem linh kiện đã tồn tại chưa (tên + loại + hãng sản xuất)
        /// </summary>
        Task<bool> IsExistsAsync(string tenLinhKien, int maLoaiLinhKien, string? hangSanXuat, int? excludeId = null);

        // === PHƯƠNG THỨC QUẢN LÝ TỒN KHO ===
        
        /// <summary>
        /// Cập nhật số lượng tồn kho
        /// </summary>
        Task<bool> UpdateStockAsync(int id, int quantity, string action);

        // === PHƯƠNG THỨC TÌM KIẾM AJAX ===
        
        /// <summary>
        /// Tìm kiếm linh kiện cho AJAX (dropdown, autocomplete)
        /// </summary>
        Task<IEnumerable<object>> SearchAjaxAsync(string term, int? categoryId = null);
        
        /// <summary>
        /// Lấy linh kiện theo loại
        /// </summary>
        Task<IEnumerable<object>> GetByCategoryAsync(int categoryId);

        // === PHƯƠNG THỨC DROPDOWN DATA ===
        
        /// <summary>
        /// Lấy danh sách loại linh kiện cho dropdown
        /// </summary>
        Task<IEnumerable<phonev2.Models.LoaiLinhKien>> GetLoaiLinhKienForDropdownAsync();
    }
} 