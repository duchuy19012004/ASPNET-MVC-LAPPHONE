using Microsoft.EntityFrameworkCore;
using phonev2.Models;
using phonev2.Repository;

namespace phonev2.Services.LinhKien
{
    /// <summary>
    /// Service cho việc validation linh kiện
    /// </summary>
    public class LinhKienValidationService : ILinhKienValidationService
    {
        private readonly PhoneLapDbContext _context;

        public LinhKienValidationService(PhoneLapDbContext context)
        {
            _context = context;
        }

        // === VALIDATION CƠ BẢN ===

        public async Task<(bool IsValid, List<string> Errors)> ValidateAsync(phonev2.Models.LinhKien linhKien)
        {
            var errors = new List<string>();

            // Validate tên linh kiện
            var (isValidTen, errorTen) = await ValidateTenLinhKienAsync(linhKien.TenLinhKien, linhKien.MaLoaiLinhKien, linhKien.HangSanXuat, linhKien.MaLinhKien);
            if (!isValidTen) errors.Add(errorTen);

            // Validate giá
            var (isValidGia, errorGia) = ValidateGia(linhKien.GiaNhap, linhKien.GiaBan);
            if (!isValidGia) errors.Add(errorGia);

            // Validate số lượng tồn kho
            var (isValidSoLuong, errorSoLuong) = ValidateSoLuongTon(linhKien.SoLuongTon);
            if (!isValidSoLuong) errors.Add(errorSoLuong);

            // Validate loại linh kiện
            var (isValidLoai, errorLoai) = await ValidateLoaiLinhKienAsync(linhKien.MaLoaiLinhKien);
            if (!isValidLoai) errors.Add(errorLoai);

            return (errors.Count == 0, errors);
        }

        public async Task<(bool IsValid, List<string> Errors)> ValidateForCreateAsync(phonev2.Models.LinhKien linhKien)
        {
            var (isValid, errors) = await ValidateAsync(linhKien);
            if (!isValid) return (false, errors);

            // Kiểm tra trùng lặp khi tạo mới
            var exists = await _context.LinhKien
                .AnyAsync(l => l.TenLinhKien.ToLower() == linhKien.TenLinhKien.ToLower() 
                            && l.MaLoaiLinhKien == linhKien.MaLoaiLinhKien
                            && (!string.IsNullOrEmpty(l.HangSanXuat) && !string.IsNullOrEmpty(linhKien.HangSanXuat) 
                                ? l.HangSanXuat.ToLower() == linhKien.HangSanXuat.ToLower() : true));

            if (exists)
            {
                errors.Add("Linh kiện này đã tồn tại trong hệ thống.");
            }

            return (errors.Count == 0, errors);
        }

        public async Task<(bool IsValid, List<string> Errors)> ValidateForUpdateAsync(phonev2.Models.LinhKien linhKien)
        {
            var (isValid, errors) = await ValidateAsync(linhKien);
            if (!isValid) return (false, errors);

            // Kiểm tra trùng lặp khi cập nhật (trừ chính nó)
            var exists = await _context.LinhKien
                .AnyAsync(l => l.TenLinhKien.ToLower() == linhKien.TenLinhKien.ToLower() 
                            && l.MaLoaiLinhKien == linhKien.MaLoaiLinhKien
                            && l.MaLinhKien != linhKien.MaLinhKien
                            && (!string.IsNullOrEmpty(l.HangSanXuat) && !string.IsNullOrEmpty(linhKien.HangSanXuat) 
                                ? l.HangSanXuat.ToLower() == linhKien.HangSanXuat.ToLower() : true));

            if (exists)
            {
                errors.Add("Linh kiện này đã tồn tại trong hệ thống.");
            }

            return (errors.Count == 0, errors);
        }

        // === VALIDATION CHI TIẾT ===

        public async Task<(bool IsValid, string Error)> ValidateTenLinhKienAsync(string tenLinhKien, int maLoaiLinhKien, string? hangSanXuat, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(tenLinhKien))
            {
                return (false, "Tên linh kiện không được để trống.");
            }

            if (tenLinhKien.Length > 200)
            {
                return (false, "Tên linh kiện không được vượt quá 200 ký tự.");
            }

            // Kiểm tra trùng lặp
            var query = _context.LinhKien.Where(l => l.TenLinhKien.ToLower() == tenLinhKien.ToLower() 
                                                 && l.MaLoaiLinhKien == maLoaiLinhKien);

            if (!string.IsNullOrEmpty(hangSanXuat))
            {
                query = query.Where(l => l.HangSanXuat != null && l.HangSanXuat.ToLower() == hangSanXuat.ToLower());
            }

            if (excludeId.HasValue)
            {
                query = query.Where(l => l.MaLinhKien != excludeId.Value);
            }

            if (await query.AnyAsync())
            {
                return (false, "Linh kiện này đã tồn tại trong hệ thống.");
            }

            return (true, "");
        }

        public (bool IsValid, string Error) ValidateGia(decimal giaNhap, decimal giaBan)
        {
            if (giaNhap < 0)
            {
                return (false, "Giá nhập không được âm.");
            }

            if (giaBan < 0)
            {
                return (false, "Giá bán không được âm.");
            }

            if (giaBan < giaNhap)
            {
                return (false, "Giá bán không được nhỏ hơn giá nhập.");
            }

            return (true, "");
        }

        public (bool IsValid, string Error) ValidateSoLuongTon(int soLuongTon)
        {
            if (soLuongTon < 0)
            {
                return (false, "Số lượng tồn kho không được âm.");
            }

            return (true, "");
        }

        public async Task<(bool IsValid, string Error)> ValidateLoaiLinhKienAsync(int maLoaiLinhKien)
        {
            var loaiLinhKien = await _context.LoaiLinhKien.FindAsync(maLoaiLinhKien);
            if (loaiLinhKien == null)
            {
                return (false, "Loại linh kiện không tồn tại.");
            }

            if (!loaiLinhKien.TrangThai)
            {
                return (false, "Loại linh kiện này đã bị vô hiệu hóa.");
            }

            return (true, "");
        }

        // === VALIDATION BUSINESS RULES ===

        public async Task<(bool IsValid, List<string> Errors)> ValidateBusinessRulesAsync(phonev2.Models.LinhKien linhKien)
        {
            var errors = new List<string>();

            // Kiểm tra quy tắc kinh doanh
            if (linhKien.GiaBan < linhKien.GiaNhap * 1.1m)
            {
                errors.Add("Giá bán nên cao hơn giá nhập ít nhất 10% để đảm bảo lợi nhuận.");
            }

            if (linhKien.SoLuongTon > 1000)
            {
                errors.Add("Số lượng tồn kho quá lớn, cần kiểm tra lại.");
            }

            return (errors.Count == 0, errors);
        }

        public async Task<(bool CanDelete, string Error)> CanDeleteAsync(int id)
        {
            // Kiểm tra xem linh kiện có đang được sử dụng trong các giao dịch khác không
            var hasPhieuNhap = await _context.ChiTietPhieuNhap.AnyAsync(ct => ct.MaLinhKien == id);
            if (hasPhieuNhap)
            {
                return (false, "Không thể xóa linh kiện này vì đang được sử dụng trong phiếu nhập.");
            }

            var hasPhieuSua = await _context.ChiTietLinhKien.AnyAsync(ct => ct.MaLinhKien == id);
            if (hasPhieuSua)
            {
                return (false, "Không thể xóa linh kiện này vì đang được sử dụng trong phiếu sửa.");
            }

            return (true, "");
        }
    }
} 