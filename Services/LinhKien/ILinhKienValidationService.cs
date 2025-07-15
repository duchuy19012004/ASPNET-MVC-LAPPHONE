using phonev2.Models;

namespace phonev2.Services.LinhKien
{
    /// <summary>
    /// Interface cho các dịch vụ validation linh kiện
    /// </summary>
    public interface ILinhKienValidationService
    {
        // === VALIDATION CƠ BẢN ===
        
        /// <summary>
        /// Kiểm tra tính hợp lệ của dữ liệu linh kiện
        /// </summary>
        Task<(bool IsValid, List<string> Errors)> ValidateAsync(phonev2.Models.LinhKien linhKien);
        
        /// <summary>
        /// Kiểm tra tính hợp lệ khi tạo mới
        /// </summary>
        Task<(bool IsValid, List<string> Errors)> ValidateForCreateAsync(phonev2.Models.LinhKien linhKien);
        
        /// <summary>
        /// Kiểm tra tính hợp lệ khi cập nhật
        /// </summary>
        Task<(bool IsValid, List<string> Errors)> ValidateForUpdateAsync(phonev2.Models.LinhKien linhKien);

        // === VALIDATION CHI TIẾT ===
        
        /// <summary>
        /// Kiểm tra tên linh kiện
        /// </summary>
        Task<(bool IsValid, string Error)> ValidateTenLinhKienAsync(string tenLinhKien, int maLoaiLinhKien, string? hangSanXuat, int? excludeId = null);
        
        /// <summary>
        /// Kiểm tra giá
        /// </summary>
        (bool IsValid, string Error) ValidateGia(decimal giaNhap, decimal giaBan);
        
        /// <summary>
        /// Kiểm tra số lượng tồn kho
        /// </summary>
        (bool IsValid, string Error) ValidateSoLuongTon(int soLuongTon);
        
        /// <summary>
        /// Kiểm tra loại linh kiện
        /// </summary>
        Task<(bool IsValid, string Error)> ValidateLoaiLinhKienAsync(int maLoaiLinhKien);

        // === VALIDATION BUSINESS RULES ===
        
        /// <summary>
        /// Kiểm tra quy tắc kinh doanh
        /// </summary>
        Task<(bool IsValid, List<string> Errors)> ValidateBusinessRulesAsync(phonev2.Models.LinhKien linhKien);
        
        /// <summary>
        /// Kiểm tra xem có thể xóa linh kiện không
        /// </summary>
        Task<(bool CanDelete, string Error)> CanDeleteAsync(int id);
    }
} 