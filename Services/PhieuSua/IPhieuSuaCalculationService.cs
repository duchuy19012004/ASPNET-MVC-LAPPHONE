using phonev2.Models;

namespace phonev2.Services.PhieuSua
{
    public interface IPhieuSuaCalculationService
    {
        Task<decimal> CalculateTongTienAsync(int maPhieuSua);
        decimal CalculateTongTienFromDetails(Models.PhieuSua phieuSua);
        Task UpdatePhieuSuaTongTienAsync(int maPhieuSua);
        Task<int> UpdateAllPhieuSuaTongTienAsync();
    }
} 