namespace phonev2.Services.PhieuSua
{
    public interface IPhieuSuaStatisticsService
    {
        Task<PhieuSuaStatistics> GetStatisticsAsync();
        Task<List<object>> GetMonthlyStatisticsAsync();
        Task<List<object>> GetMonthlyRevenueStatisticsAsync();
    }

    public class PhieuSuaStatistics
    {
        public int TotalPhieuSua { get; set; }
        public int TiepNhan { get; set; }
        public int DangSua { get; set; }
        public int HoanThanh { get; set; }
        public int Huy { get; set; }
        public int MoiTrong30Ngay { get; set; }
        public decimal TongTienPhieuSua { get; set; }
    }
} 