using phonev2.Models;
using phonev2.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace phonev2.Services.PhieuSua
{
    public class PhieuSuaService : IPhieuSuaService
    {
        private readonly phonev2.Repository.PhoneLapDbContext _context;

        public PhieuSuaService(phonev2.Repository.PhoneLapDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Models.PhieuSua> phieuSuas, int totalCount, int totalPages)> GetPhieuSuasAsync(string search, int page, int pageSize, string sort)
        {
            var query = _context.PhieuSua
                .Include(p => p.ChiTietPhieuSuas).ThenInclude(ct => ct.DichVu)
                .Include(p => p.ChiTietLinhKiens).ThenInclude(ct => ct.LinhKien)
                .Include(p => p.KhachHang)
                .Include(p => p.NhanVien)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.ToLower();
                query = query.Where(p =>
                    _context.KhachHang.Any(kh => kh.MaKhachHang == p.MaKhachHang && (kh.HoTen.ToLower().Contains(s) || kh.SoDienThoai.Contains(s)))
                    || _context.NhanVien.Any(nv => nv.MaNhanVien == p.MaNhanVien && nv.HoTen.ToLower().Contains(s))
                    || p.ChiTietPhieuSuas.Any(ct => ct.DichVu != null && ct.DichVu.TenDichVu.ToLower().Contains(s))
                );
            }

            // Sắp xếp
            switch (sort)
            {
                case "name_asc":
                    query = query.OrderBy(p => p.MaPhieuSua);
                    break;
                case "name_desc":
                    query = query.OrderByDescending(p => p.MaPhieuSua);
                    break;
                case "oldest":
                    query = query.OrderBy(p => p.NgaySua);
                    break;
                case "newest":
                    query = query.OrderByDescending(p => p.NgaySua);
                    break;
                default:
                    query = query.OrderByDescending(p => p.MaPhieuSua);
                    break;
            }

            int totalCount = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var list = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Sau khi lấy danh sách phiếu sửa, log ra để kiểm tra property KhachHang và NhanVien
            foreach (var p in list)
            {
                System.Diagnostics.Debug.WriteLine($"PhieuSua: {p.MaPhieuSua}, MaKhachHang: {p.MaKhachHang}, TenKhachHang: {p.KhachHang?.HoTen}, MaNhanVien: {p.MaNhanVien}, TenNhanVien: {p.NhanVien?.HoTen}");
            }

            return (list, totalCount, totalPages);
        }
        public Task<Models.PhieuSua> GetPhieuSuaByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> CreatePhieuSuaAsync(PhieuSuaCreateViewModel vm)
        {
            try
            {
                // Đảm bảo ngày sửa được set
                if (vm.PhieuSua.NgaySua == default)
                {
                    vm.PhieuSua.NgaySua = DateTime.Now;
                }

                // Đảm bảo trạng thái được set
                vm.PhieuSua.TrangThai = TrangThaiPhieuSua.TiepNhan;

                _context.PhieuSua.Add(vm.PhieuSua);
                await _context.SaveChangesAsync();

                // Lấy MaPhieuSua sau khi đã lưu
                var maPhieuSua = vm.PhieuSua.MaPhieuSua;

                // Thêm dịch vụ
                if (vm.DichVus != null)
                {
                    foreach (var dv in vm.DichVus)
                    {
                        if (dv.MaDichVu > 0)
                        {
                            _context.ChiTietPhieuSua.Add(new ChiTietPhieuSua
                            {
                                MaPhieuSua = maPhieuSua,
                                MaDichVu = dv.MaDichVu,
                                SoLuong = 1 // Mặc định 1, vì không có trường SoLuong
                            });
                        }
                    }
                }

                // Thêm linh kiện
                if (vm.LinhKiens != null)
                {
                    foreach (var lk in vm.LinhKiens)
                    {
                        if (lk.MaLinhKien > 0 && lk.SoLuong > 0)
                        {
                            _context.ChiTietLinhKien.Add(new ChiTietLinhKien
                            {
                                MaPhieuSua = maPhieuSua,
                                MaLinhKien = lk.MaLinhKien,
                                SoLuong = lk.SoLuong
                            });
                        }
                    }
                }

                await _context.SaveChangesAsync();

                // Tính toán và cập nhật tổng tiền
                var phieuSua = await _context.PhieuSua
                    .Include(p => p.ChiTietPhieuSuas).ThenInclude(ct => ct.DichVu)
                    .Include(p => p.ChiTietLinhKiens).ThenInclude(ct => ct.LinhKien)
                    .FirstOrDefaultAsync(p => p.MaPhieuSua == maPhieuSua);

                if (phieuSua != null)
                {
                    decimal tongTienDichVu = phieuSua.ChiTietPhieuSuas?.Sum(ct => ct.DichVu?.GiaDichVu ?? 0) ?? 0;
                    decimal tongTienLinhKien = phieuSua.ChiTietLinhKiens?.Sum(lk => (lk.LinhKien?.GiaBan ?? 0) * lk.SoLuong) ?? 0;
                    phieuSua.TongTien = tongTienDichVu + tongTienLinhKien;
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return false;
            }
        }
        public async Task<bool> UpdatePhieuSuaAsync(Models.PhieuSua phieuSua)
        {
            try
            {
                // Lấy phiếu sửa hiện tại từ DB để so sánh trạng thái
                var currentPhieuSua = await _context.PhieuSua.AsNoTracking().FirstOrDefaultAsync(p => p.MaPhieuSua == phieuSua.MaPhieuSua);
                if (currentPhieuSua == null)
                {
                    return false;
                }

                // Tự động set NgayTraThucTe khi chuyển sang trạng thái Hoàn thành
                if (phieuSua.TrangThai == TrangThaiPhieuSua.HoanThanh && currentPhieuSua.TrangThai != TrangThaiPhieuSua.HoanThanh)
                {
                    phieuSua.NgayTraThucTe = DateTime.Now;
                }
                // Clear NgayTraThucTe nếu chuyển về trạng thái khác
                else if (phieuSua.TrangThai != TrangThaiPhieuSua.HoanThanh && currentPhieuSua.TrangThai == TrangThaiPhieuSua.HoanThanh)
                {
                    phieuSua.NgayTraThucTe = null;
                }
                // Giữ nguyên NgayTraThucTe nếu không thay đổi trạng thái hoặc vẫn là Hoàn thành
                else if (phieuSua.TrangThai == TrangThaiPhieuSua.HoanThanh && currentPhieuSua.TrangThai == TrangThaiPhieuSua.HoanThanh)
                {
                    phieuSua.NgayTraThucTe = currentPhieuSua.NgayTraThucTe;
                }

                // Tính toán và cập nhật tổng tiền
                var phieuSuaWithDetails = await _context.PhieuSua
                    .Include(p => p.ChiTietPhieuSuas).ThenInclude(ct => ct.DichVu)
                    .Include(p => p.ChiTietLinhKiens).ThenInclude(ct => ct.LinhKien)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.MaPhieuSua == phieuSua.MaPhieuSua);

                if (phieuSuaWithDetails != null)
                {
                    decimal tongTienDichVu = phieuSuaWithDetails.ChiTietPhieuSuas?.Sum(ct => ct.DichVu?.GiaDichVu ?? 0) ?? 0;
                    decimal tongTienLinhKien = phieuSuaWithDetails.ChiTietLinhKiens?.Sum(lk => (lk.LinhKien?.GiaBan ?? 0) * lk.SoLuong) ?? 0;
                    phieuSua.TongTien = tongTienDichVu + tongTienLinhKien;
                }

                // Attach the entity and mark it as modified
                _context.Attach(phieuSua);
                _context.Entry(phieuSua).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return false;
            }
        }
        public async Task<bool> DeletePhieuSuaAsync(int id)
        {
            try
            {
                var phieuSua = await _context.PhieuSua.FindAsync(id);
                if (phieuSua != null)
                {
                    _context.PhieuSua.Remove(phieuSua);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return false;
            }
        }
        public async Task<bool> UpdateTrangThaiAsync(int id, int trangThai)
        {
            var phieu = await _context.PhieuSua.FindAsync(id);
            if (phieu == null) return false;
            phieu.TrangThai = (phonev2.Models.TrangThaiPhieuSua)trangThai;
            await _context.SaveChangesAsync();
            return true;
        }
        public Task<bool> AddDichVuToPhieuSuaAsync(int maPhieuSua, int maDichVu, int soLuong)
        {
            throw new NotImplementedException();
        }
        public Task<bool> AddLinhKienToPhieuSuaAsync(int maPhieuSua, int maLinhKien, int soLuong)
        {
            throw new NotImplementedException();
        }
        public async Task<int> UpdateAllTongTienAsync()
        {
            try
            {
                var allPhieuSua = await _context.PhieuSua
                    .Include(p => p.ChiTietPhieuSuas).ThenInclude(ct => ct.DichVu)
                    .Include(p => p.ChiTietLinhKiens).ThenInclude(ct => ct.LinhKien)
                    .ToListAsync();

                int updatedCount = 0;
                foreach (var phieuSua in allPhieuSua)
                {
                    decimal tongTienDichVu = phieuSua.ChiTietPhieuSuas?.Sum(ct => ct.DichVu?.GiaDichVu ?? 0) ?? 0;
                    decimal tongTienLinhKien = phieuSua.ChiTietLinhKiens?.Sum(lk => (lk.LinhKien?.GiaBan ?? 0) * lk.SoLuong) ?? 0;
                    decimal newTongTien = tongTienDichVu + tongTienLinhKien;
                    
                    if (phieuSua.TongTien != newTongTien)
                    {
                        phieuSua.TongTien = newTongTien;
                        updatedCount++;
                    }
                }

                if (updatedCount > 0)
                {
                    await _context.SaveChangesAsync();
                }

                return updatedCount;
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return 0;
            }
        }
        public async Task<PhieuSuaCreateViewModel> GetCreateViewModelAsync()
        {
            var vm = new PhieuSuaCreateViewModel
            {
                PhieuSua = new Models.PhieuSua
                {
                    NgaySua = DateTime.Now,
                    TrangThai = TrangThaiPhieuSua.TiepNhan
                },
                DichVus = new List<Models.ViewModels.DichVuChonVM>(),
                LinhKiens = new List<Models.ViewModels.LinhKienChonVM>(),
                KhachHangList = GetKhachHangList(),
                NhanVienList = GetNhanVienList(),
                DichVuList = GetDichVuOptionList(),
                LinhKienList = GetLinhKienList()
            };
            return vm;
        }
        public async Task<Models.ViewModels.PhieuSuaEditViewModel> GetPhieuSuaEditViewModelAsync(int id)
        {
            // Lấy phiếu sửa và chi tiết
            var phieuSua = await _context.PhieuSua
                .Include(p => p.ChiTietPhieuSuas).ThenInclude(ct => ct.DichVu)
                .Include(p => p.ChiTietLinhKiens).ThenInclude(ct => ct.LinhKien)
                .Include(p => p.KhachHang) // Bổ sung Include KhachHang
                .Include(p => p.NhanVien) // Bổ sung Include NhanVien
                .FirstOrDefaultAsync(p => p.MaPhieuSua == id);
            if (phieuSua == null) return null;

            // Debug: Kiểm tra dữ liệu khách hàng
            System.Diagnostics.Debug.WriteLine($"PhieuSua {id}: MaKhachHang = {phieuSua.MaKhachHang}");
            System.Diagnostics.Debug.WriteLine($"PhieuSua {id}: KhachHang = {(phieuSua.KhachHang != null ? $"{phieuSua.KhachHang.HoTen} ({phieuSua.KhachHang.SoDienThoai})" : "NULL")}");
            System.Diagnostics.Debug.WriteLine($"PhieuSua {id}: MaNhanVien = {phieuSua.MaNhanVien}");
            System.Diagnostics.Debug.WriteLine($"PhieuSua {id}: NhanVien = {(phieuSua.NhanVien != null ? phieuSua.NhanVien.HoTen : "NULL")}");

            // Nếu KhachHang vẫn null, load riêng biệt
            if (phieuSua.KhachHang == null && phieuSua.MaKhachHang.HasValue)
            {
                System.Diagnostics.Debug.WriteLine($"Loading KhachHang separately for MaKhachHang = {phieuSua.MaKhachHang}");
                phieuSua.KhachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.MaKhachHang == phieuSua.MaKhachHang.Value);
                System.Diagnostics.Debug.WriteLine($"Loaded KhachHang = {(phieuSua.KhachHang != null ? $"{phieuSua.KhachHang.HoTen} ({phieuSua.KhachHang.SoDienThoai})" : "NULL")}");
            }

            // Nếu NhanVien vẫn null, load riêng biệt
            if (phieuSua.NhanVien == null && phieuSua.MaNhanVien.HasValue)
            {
                System.Diagnostics.Debug.WriteLine($"Loading NhanVien separately for MaNhanVien = {phieuSua.MaNhanVien}");
                phieuSua.NhanVien = await _context.NhanVien.FirstOrDefaultAsync(nv => nv.MaNhanVien == phieuSua.MaNhanVien.Value);
                System.Diagnostics.Debug.WriteLine($"Loaded NhanVien = {(phieuSua.NhanVien != null ? phieuSua.NhanVien.HoTen : "NULL")}");
            }

            var vm = new Models.ViewModels.PhieuSuaEditViewModel
            {
                PhieuSua = phieuSua,
                DichVus = phieuSua.ChiTietPhieuSuas?.Select(ct => new Models.ViewModels.DichVuChonVM
                {
                    MaDichVu = ct.MaDichVu
                }).ToList() ?? new List<Models.ViewModels.DichVuChonVM>(),
                LinhKiens = phieuSua.ChiTietLinhKiens?.Select(lk => new Models.ViewModels.LinhKienChonVM
                {
                    MaLinhKien = lk.MaLinhKien,
                    SoLuong = lk.SoLuong,
                    MaDichVu = 0 // Sẽ được set sau khi load từ DichVuLinhKien
                }).ToList() ?? new List<Models.ViewModels.LinhKienChonVM>(),
                KhachHangList = GetKhachHangList(),
                NhanVienList = GetNhanVienList(),
                DichVuList = GetDichVuOptionList(),
                LinhKienList = GetLinhKienList()
            };

            // Cập nhật MaDichVu cho các linh kiện dựa trên DichVuLinhKien
            if (vm.LinhKiens.Any())
            {
                var linhKienIds = vm.LinhKiens.Select(lk => lk.MaLinhKien).ToList();
                var dichVuLinhKienMapping = await _context.DichVuLinhKien
                    .Where(dvlk => linhKienIds.Contains(dvlk.MaLinhKien))
                    .Select(dvlk => new { dvlk.MaLinhKien, dvlk.MaDichVu })
                    .ToListAsync();

                foreach (var linhKien in vm.LinhKiens)
                {
                    var mapping = dichVuLinhKienMapping.FirstOrDefault(m => m.MaLinhKien == linhKien.MaLinhKien);
                    if (mapping != null)
                    {
                        linhKien.MaDichVu = mapping.MaDichVu;
                    }
                }
            }

            return vm;
        }
        public async Task<Models.ViewModels.PhieuSuaEditViewModel> GetPhieuSuaDetailsViewModelAsync(int id)
        {
            // Có thể dùng chung logic với Edit
            return await GetPhieuSuaEditViewModelAsync(id);
        }
        public List<SelectListItem> GetKhachHangList()
        {
            return _context.KhachHang.Where(kh => kh.TrangThai)
                .Select(kh => new SelectListItem { Value = kh.MaKhachHang.ToString(), Text = kh.HoTen }).ToList();
        }
        public List<SelectListItem> GetNhanVienList()
        {
            return _context.NhanVien.Where(nv => nv.TrangThai)
                .Select(nv => new SelectListItem { Value = nv.MaNhanVien.ToString(), Text = nv.HoTen }).ToList();
        }
        public List<SelectListItem> GetDichVuList()
        {
            return _context.DichVu.Where(dv => dv.TrangThai)
                .Select(dv => new SelectListItem { Value = dv.MaDichVu.ToString(), Text = dv.TenDichVu }).ToList();
        }
        public List<SelectListItem> GetLinhKienList()
        {
            return _context.LinhKien.Where(lk => lk.TrangThai)
                .Select(lk => new SelectListItem { Value = lk.MaLinhKien.ToString(), Text = lk.TenLinhKien }).ToList();
        }
        public List<phonev2.Models.ViewModels.DichVuOptionVM> GetDichVuOptionList()
        {
            return _context.DichVu.Where(dv => dv.TrangThai)
                .Select(dv => new phonev2.Models.ViewModels.DichVuOptionVM
                {
                    Value = dv.MaDichVu.ToString(),
                    Text = dv.TenDichVu,
                    GiaDichVu = dv.GiaDichVu
                }).ToList();
        }
    }
} 