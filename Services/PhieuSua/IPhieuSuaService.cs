using phonev2.Models;
using phonev2.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace phonev2.Services.PhieuSua
{
    public interface IPhieuSuaService
    {
        Task<(List<Models.PhieuSua> phieuSuas, int totalCount, int totalPages)> GetPhieuSuasAsync(string search, int page, int pageSize, string sort);
        Task<Models.PhieuSua> GetPhieuSuaByIdAsync(int id);
        Task<bool> CreatePhieuSuaAsync(PhieuSuaCreateViewModel vm);
        Task<bool> UpdatePhieuSuaAsync(Models.PhieuSua phieuSua);
        Task<bool> DeletePhieuSuaAsync(int id);
        Task<bool> AddDichVuToPhieuSuaAsync(int maPhieuSua, int maDichVu, int soLuong);
        Task<bool> AddLinhKienToPhieuSuaAsync(int maPhieuSua, int maLinhKien, int soLuong);
        Task<int> UpdateAllTongTienAsync();
        Task<PhieuSuaCreateViewModel> GetCreateViewModelAsync();
        List<SelectListItem> GetKhachHangList();
        List<SelectListItem> GetNhanVienList();
        List<SelectListItem> GetDichVuList();
        List<SelectListItem> GetLinhKienList();
        Task<Models.ViewModels.PhieuSuaEditViewModel> GetPhieuSuaEditViewModelAsync(int id);
        Task<Models.ViewModels.PhieuSuaEditViewModel> GetPhieuSuaDetailsViewModelAsync(int id);
        Task<bool> UpdateTrangThaiAsync(int id, int trangThai);
        List<phonev2.Models.ViewModels.DichVuOptionVM> GetDichVuOptionList();
    }
} 