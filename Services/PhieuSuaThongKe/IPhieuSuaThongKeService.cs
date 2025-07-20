using System.Threading.Tasks;
using System.Collections.Generic;

namespace phonev2.Services.PhieuSuaThongKe
{
    public interface IPhieuSuaThongKeService
    {
        /// <summary>
        /// Lấy dữ liệu thống kê tổng tiền và số phiếu sửa theo tuần/tháng/năm
        /// </summary>
        /// <param name="type">"week" | "month" | "year"</param>
        /// <param name="year">Năm cần thống kê</param>
        /// <param name="month">Tháng cần thống kê (nếu có)</param>
        /// <param name="week">Tuần cần thống kê (nếu có)</param>
        /// <returns>Danh sách dữ liệu cho biểu đồ</returns>
        Task<IEnumerable<ThongKePhieuSuaDto>> GetThongKeAsync(string type, int? year, int? month, int? week);

        /// <summary>
        /// Lấy top N linh kiện sử dụng nhiều nhất trong phiếu sửa
        /// </summary>
        Task<IEnumerable<TopLinhKienDto>> GetTopLinhKienAsync(int top, string type, int? year, int? month, int? week);

        /// <summary>
        /// Thống kê số lượng sử dụng theo loại linh kiện
        /// </summary>
        Task<IEnumerable<TopLoaiLinhKienDto>> GetThongKeLoaiLinhKienAsync(string type, int? year, int? month, int? week);

        /// <summary>
        /// Lấy top N dịch vụ được sử dụng nhiều nhất trong phiếu sửa
        /// </summary>
        Task<IEnumerable<TopDichVuDto>> GetTopDichVuAsync(int top, string type, int? year, int? month, int? week);
    }

    public class ThongKePhieuSuaDto
    {
        public string Label { get; set; } = string.Empty; // Nhãn (ngày/tháng/tuần...)
        public int SoPhieu { get; set; } // Số phiếu sửa
        public decimal TongTien { get; set; } // Tổng tiền
    }

    public class TopLinhKienDto
    {
        public string TenLinhKien { get; set; } = string.Empty;
        public int SoLan { get; set; }
    }
    public class TopLoaiLinhKienDto
    {
        public string TenLoaiLinhKien { get; set; } = string.Empty;
        public int SoLan { get; set; }
    }

    public class TopDichVuDto
    {
        public string TenDichVu { get; set; } = string.Empty;
        public int SoLan { get; set; }
    }
} 