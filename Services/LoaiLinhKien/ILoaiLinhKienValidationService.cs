using phonev2.Models;

namespace phonev2.Services.LoaiLinhKien
{
    /// <summary>
    /// Interface chuyên về validation (kiểm tra tính hợp lệ) cho loại linh kiện
    /// Tách riêng logic validation để dễ test và maintain
    /// </summary>
    public interface ILoaiLinhKienValidationService
    {
        // === PHƯƠNG THỨC KIỂM TRA DỮ LIỆU ===
        
        /// <summary>
        /// Kiểm tra xem tên loại linh kiện đã tồn tại trong database chưa
        /// </summary>
        /// <param name="tenLoaiLinhKien">Tên cần kiểm tra</param>
        /// <param name="excludeId">ID loại linh kiện cần loại trừ (dùng khi edit)</param>
        /// <returns>True nếu tên đã tồn tại, False nếu chưa</returns>
        Task<bool> IsNameExistsAsync(string tenLoaiLinhKien, int? excludeId = null);
        
        /// <summary>
        /// Kiểm tra xem có thể xóa loại linh kiện này không
        /// (Kiểm tra xem có linh kiện nào đang sử dụng loại này không)
        /// </summary>
        /// <param name="id">ID loại linh kiện cần kiểm tra</param>
        /// <returns>True nếu có thể xóa, False nếu không thể xóa</returns>
        Task<bool> CanDeleteAsync(int id);
        
        /// <summary>
        /// Kiểm tra thời gian bảo hành có hợp lệ không
        /// </summary>
        /// <param name="thoiGianBaoHanh">Thời gian bảo hành (tháng)</param>
        /// <returns>True nếu hợp lệ, False nếu không hợp lệ</returns>
        bool IsValidWarrantyPeriod(int? thoiGianBaoHanh);
        
        /// <summary>
        /// Kiểm tra tên loại linh kiện có hợp lệ không
        /// </summary>
        /// <param name="tenLoaiLinhKien">Tên cần kiểm tra</param>
        /// <returns>True nếu hợp lệ, False nếu không hợp lệ</returns>
        bool IsValidName(string tenLoaiLinhKien);
    }
} 