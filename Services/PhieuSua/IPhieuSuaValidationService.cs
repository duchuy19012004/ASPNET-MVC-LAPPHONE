using phonev2.Models;
using phonev2.Models.ViewModels;

namespace phonev2.Services.PhieuSua
{
    public interface IPhieuSuaValidationService
    {
        (bool isValid, List<string> errors) ValidatePhieuSua(Models.PhieuSua phieuSua);
        (bool isValid, List<string> errors) ValidatePhieuSuaCreate(PhieuSuaCreateViewModel vm);
        (bool isValid, List<string> errors) ValidateNgayHenTra(DateTime ngaySua, DateTime? ngayHenTra);
    }
} 