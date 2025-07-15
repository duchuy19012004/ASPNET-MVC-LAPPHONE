using phonev2.Models;
using phonev2.Models.ViewModels;

namespace phonev2.Services.PhieuSua
{
    public class PhieuSuaValidationService : IPhieuSuaValidationService
    {
        public (bool isValid, List<string> errors) ValidatePhieuSua(Models.PhieuSua phieuSua)
        {
            throw new NotImplementedException();
        }
        public (bool isValid, List<string> errors) ValidatePhieuSuaCreate(PhieuSuaCreateViewModel vm)
        {
            throw new NotImplementedException();
        }
        public (bool isValid, List<string> errors) ValidateNgayHenTra(DateTime ngaySua, DateTime? ngayHenTra)
        {
            throw new NotImplementedException();
        }
    }
} 