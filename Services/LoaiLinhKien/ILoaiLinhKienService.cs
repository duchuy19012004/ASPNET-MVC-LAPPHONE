using phonev2.Models;

namespace phonev2.Services.LoaiLinhKien
{
    /// <summary>
    /// Interface chính cho việc quản lý loại linh kiện
    /// Chứa tất cả các phương thức CRUD và business logic cơ bản
    /// </summary>
    public interface ILoaiLinhKienService
    {
        // === PHƯƠNG THỨC CRUD CƠ BẢN ===
        
        /// <summary>
        /// Lấy tất cả loại linh kiện, sắp xếp theo tên
        /// </summary>
        Task<IEnumerable<phonev2.Models.LoaiLinhKien>> GetAllAsync();
        
        /// <summary>
        /// Lấy loại linh kiện theo ID
        /// </summary>
        Task<phonev2.Models.LoaiLinhKien?> GetByIdAsync(int id);
        
        /// <summary>
        /// Tạo mới loại linh kiện
        /// </summary>
        Task<bool> CreateAsync(phonev2.Models.LoaiLinhKien loaiLinhKien);
        
        /// <summary>
        /// Cập nhật thông tin loại linh kiện
        /// </summary>
        Task<bool> UpdateAsync(phonev2.Models.LoaiLinhKien loaiLinhKien);
        
        /// <summary>
        /// Xóa loại linh kiện
        /// </summary>
        Task<bool> DeleteAsync(int id);
        
        /// <summary>
        /// Bật/tắt trạng thái hoạt động của loại linh kiện
        /// </summary>
        Task<bool> ToggleStatusAsync(int id);

        // === PHƯƠNG THỨC TÌM KIẾM VÀ PHÂN TRANG ===
        
        /// <summary>
        /// Lấy danh sách loại linh kiện có phân trang, tìm kiếm và sắp xếp
        /// </summary>
        Task<(IEnumerable<phonev2.Models.LoaiLinhKien> Items, int TotalCount, int TotalPages)> GetPagedAsync(
            string searchString, string sortOrder, int page, int pageSize);

        // === PHƯƠNG THỨC VALIDATION ===
        
        /// <summary>
        /// Kiểm tra xem tên loại linh kiện đã tồn tại chưa
        /// </summary>
        Task<bool> IsNameExistsAsync(string tenLoaiLinhKien, int? excludeId = null);

        // === PHƯƠNG THỨC TÌM KIẾM AJAX ===
        
        /// <summary>
        /// Tìm kiếm loại linh kiện cho AJAX (dropdown, autocomplete)
        /// </summary>
        Task<IEnumerable<object>> SearchAjaxAsync(string term);
    }
} 