using phonev2.Models;

namespace phonev2.Services.LinhKien
{
    /// <summary>
    /// Interface cho các dịch vụ báo cáo và thống kê linh kiện
    /// </summary>
    public interface ILinhKienStatisticsService
    {
        // === BÁO CÁO TỒN KHO ===
        
        /// <summary>
        /// Lấy danh sách linh kiện tồn kho thấp
        /// </summary>
        Task<IEnumerable<phonev2.Models.LinhKien>> GetLowStockItemsAsync(int threshold = 10);
        
        /// <summary>
        /// Lấy báo cáo tồn kho theo loại
        /// </summary>
        Task<IEnumerable<object>> GetStockReportByCategoryAsync();
        
        /// <summary>
        /// Lấy tổng quan tồn kho
        /// </summary>
        Task<object> GetStockOverviewAsync();

        /// <summary>
        /// Lấy dữ liệu báo cáo tồn kho cho các biểu đồ chart (category, totalStock, stockHistory, totalImported, totalUsed)
        /// </summary>
        Task<IEnumerable<object>> GetStockReportForChartsAsync(
            string? search = null, string? category = null, string? brand = null,
            DateTime? from = null, DateTime? to = null, string? stock = null);

        // === BÁO CÁO LỢI NHUẬN ===
        
        /// <summary>
        /// Lấy báo cáo lợi nhuận theo linh kiện
        /// </summary>
        Task<IEnumerable<phonev2.Models.LinhKien>> GetProfitReportAsync();
        
        /// <summary>
        /// Lấy báo cáo lợi nhuận theo loại
        /// </summary>
        Task<IEnumerable<object>> GetProfitReportByCategoryAsync();

        // === THỐNG KÊ TỔNG QUAN ===
        
        /// <summary>
        /// Lấy thống kê tổng quan
        /// </summary>
        Task<object> GetOverviewStatisticsAsync();
        
        /// <summary>
        /// Lấy thống kê theo thời gian
        /// </summary>
        Task<IEnumerable<object>> GetTimeBasedStatisticsAsync(DateTime startDate, DateTime endDate);
    }
} 