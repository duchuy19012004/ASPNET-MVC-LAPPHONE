namespace phonev2.Services.LoaiLinhKien
{
    /// <summary>
    /// Interface chuyên về thống kê cho loại linh kiện
    /// Tách riêng logic thống kê để dễ mở rộng và tái sử dụng
    /// </summary>
    public interface ILoaiLinhKienStatisticsService
    {
        // === PHƯƠNG THỨC THỐNG KÊ CƠ BẢN ===
        
        /// <summary>
        /// Đếm tổng số loại linh kiện trong hệ thống
        /// </summary>
        /// <returns>Số lượng loại linh kiện</returns>
        Task<int> GetTotalCountAsync();
        
        /// <summary>
        /// Đếm số loại linh kiện đang hoạt động (TrangThai = true)
        /// </summary>
        /// <returns>Số lượng loại linh kiện đang hoạt động</returns>
        Task<int> GetActiveCountAsync();
        
        /// <summary>
        /// Đếm số loại linh kiện đã ngừng hoạt động (TrangThai = false)
        /// </summary>
        /// <returns>Số lượng loại linh kiện đã ngừng hoạt động</returns>
        Task<int> GetInactiveCountAsync();
        
        // === PHƯƠNG THỨC THỐNG KÊ TỔNG HỢP ===
        
        /// <summary>
        /// Lấy thống kê tổng hợp về loại linh kiện
        /// Bao gồm: tổng số, số đang hoạt động, số đã ngừng, tỷ lệ phần trăm
        /// </summary>
        /// <returns>Object chứa các thông tin thống kê</returns>
        Task<object> GetStatisticsAsync();
        
        /// <summary>
        /// Lấy thống kê theo thời gian bảo hành
        /// Nhóm các loại linh kiện theo thời gian bảo hành
        /// </summary>
        /// <returns>Danh sách thống kê theo thời gian bảo hành</returns>
        Task<IEnumerable<object>> GetWarrantyStatisticsAsync();
    }
} 