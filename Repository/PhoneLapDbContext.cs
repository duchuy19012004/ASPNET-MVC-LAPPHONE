using Microsoft.EntityFrameworkCore;
using phonev2.Models;

namespace phonev2.Repository
{
    public class PhoneLapDbContext : DbContext
    {
        public PhoneLapDbContext(DbContextOptions<PhoneLapDbContext> options) : base(options)
        {
        }

        // DbSet cho các bảng
        public DbSet<DichVu> DichVu { get; set; }
        public DbSet<LoaiLinhKien> LoaiLinhKien { get; set; }
        public DbSet<LinhKien> LinhKien { get; set; }
        public DbSet<NhaCungCap> NhaCungCap { get; set; }
        public DbSet<NhanVien> NhanVien { get; set; }
        public DbSet<KhachHang> KhachHang { get; set; }
        public DbSet<PhieuNhap> PhieuNhap { get; set; }
        public DbSet<ChiTietPhieuNhap> ChiTietPhieuNhap { get; set; }
        public DbSet<ThietBi> ThietBi { get; set; }
        public DbSet<PhieuSua> PhieuSua { get; set; }
        public DbSet<ChiTietPhieuSua> ChiTietPhieuSua { get; set; }
        public DbSet<ChiTietLinhKien> ChiTietLinhKien { get; set; }
        public DbSet<DichVuLinhKien> DichVuLinhKien { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình bảng DichVu
            modelBuilder.Entity<DichVu>(entity =>
            {
                entity.HasKey(e => e.MaDichVu);
                entity.Property(e => e.TenDichVu).IsRequired().HasMaxLength(100);
                entity.Property(e => e.MoTa).HasMaxLength(500);
                entity.Property(e => e.GiaDichVu).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.TrangThai).HasDefaultValue(true);
                entity.HasIndex(e => e.TenDichVu);
                entity.HasIndex(e => e.TrangThai);
            });

            // Cấu hình bảng LoaiLinhKien
            modelBuilder.Entity<LoaiLinhKien>(entity =>
            {
                entity.HasKey(e => e.MaLoaiLinhKien);
                entity.Property(e => e.TenLoaiLinhKien).IsRequired().HasMaxLength(100);
                entity.Property(e => e.MoTa).HasMaxLength(500);
                entity.Property(e => e.ThoiGianBaoHanh).IsRequired(false);
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.TrangThai).HasDefaultValue(true);
                entity.HasIndex(e => e.TenLoaiLinhKien);
                entity.HasIndex(e => e.TrangThai);
                entity.HasIndex(e => e.ThoiGianBaoHanh);
            });

            // Cấu hình bảng LinhKien
            modelBuilder.Entity<LinhKien>(entity =>
            {
                entity.HasKey(e => e.MaLinhKien);
                entity.Property(e => e.TenLinhKien).IsRequired().HasMaxLength(200);
                entity.Property(e => e.HangSanXuat).HasMaxLength(100);
                entity.Property(e => e.GiaNhap).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.GiaBan).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.SoLuongTon).IsRequired();
                entity.Property(e => e.ThongSoKyThuat).HasMaxLength(1000);
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.TrangThai).HasDefaultValue(true);
                entity.HasIndex(e => e.TenLinhKien);
                entity.HasIndex(e => e.MaLoaiLinhKien);
                entity.HasIndex(e => e.HangSanXuat);
                entity.HasIndex(e => e.SoLuongTon);
                entity.HasIndex(e => e.TrangThai);
                entity.HasIndex(e => e.GiaBan);
                entity.HasIndex(e => e.NgayTao);

                // Foreign Key
                entity.HasOne(e => e.LoaiLinhKien)
                      .WithMany()
                      .HasForeignKey(e => e.MaLoaiLinhKien)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Cấu hình bảng NhaCungCap
            modelBuilder.Entity<NhaCungCap>(entity =>
            {
                entity.HasKey(e => e.MaNhaCungCap);
                entity.Property(e => e.TenNhaCungCap).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DiaChi).IsRequired().HasMaxLength(255);
                entity.Property(e => e.SoDienThoai).IsRequired().HasMaxLength(15);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.TrangThai).HasDefaultValue(true);
                entity.HasIndex(e => e.TenNhaCungCap);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.SoDienThoai);
                entity.HasIndex(e => e.TrangThai);
            });

            // Cấu hình bảng NhanVien
            modelBuilder.Entity<NhanVien>(entity =>
            {
                entity.HasKey(e => e.MaNhanVien);
                entity.Property(e => e.HoTen).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SoDienThoai).IsRequired().HasMaxLength(15);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DiaChi).IsRequired().HasMaxLength(255);
                entity.Property(e => e.ChucVu).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Luong).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.NgaySinh).IsRequired();
                entity.Property(e => e.NgayVaoLam).IsRequired();
                entity.Property(e => e.NgayNghiViec).IsRequired(false);
                entity.Property(e => e.TrangThai).HasDefaultValue(true);
                entity.HasIndex(e => e.HoTen);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.SoDienThoai);
                entity.HasIndex(e => e.ChucVu);
                entity.HasIndex(e => e.TrangThai);
                entity.HasIndex(e => e.NgayVaoLam);
                entity.HasIndex(e => e.NgayNghiViec);
                entity.HasCheckConstraint("CK_NhanVien_Luong", "Luong >= 0");
                entity.HasCheckConstraint("CK_NhanVien_NgaySinh", "NgaySinh <= GETDATE()");
                entity.HasCheckConstraint("CK_NhanVien_NgayVaoLam", "NgayVaoLam >= NgaySinh");
                entity.HasCheckConstraint("CK_NhanVien_NgayNghiViec", "NgayNghiViec IS NULL OR NgayNghiViec >= NgayVaoLam");
            });

            // Cấu hình bảng KhachHang
            modelBuilder.Entity<KhachHang>(entity =>
            {
                entity.HasKey(e => e.MaKhachHang);
                entity.Property(e => e.HoTen).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SoDienThoai).IsRequired().HasMaxLength(15);
                entity.Property(e => e.DiaChi).IsRequired().HasMaxLength(255);
                entity.Property(e => e.TongChiTieu).HasColumnType("decimal(18,2)").HasDefaultValue(0);
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.TrangThai).HasDefaultValue(true);
                entity.HasIndex(e => e.HoTen);
                entity.HasIndex(e => e.SoDienThoai).IsUnique();
                entity.HasIndex(e => e.TongChiTieu);
                entity.HasIndex(e => e.TrangThai);
                entity.HasIndex(e => e.NgayTao);
                entity.HasCheckConstraint("CK_KhachHang_TongChiTieu", "TongChiTieu >= 0");
                entity.HasCheckConstraint("CK_KhachHang_NgayTao", "NgayTao <= GETDATE()");
            });

            // Cấu hình bảng PhieuNhap
            modelBuilder.Entity<PhieuNhap>(entity =>
            {
                entity.HasKey(e => e.MaPhieuNhap);
                entity.Property(e => e.NgayNhap).IsRequired();
                entity.Property(e => e.TongTien).HasColumnType("decimal(18,2)").HasDefaultValue(0);
                entity.Property(e => e.GhiChu).HasMaxLength(500);
                entity.Property(e => e.TrangThai).IsRequired().HasMaxLength(20).HasDefaultValue("Nháp");
                entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");
                entity.HasOne(e => e.NhaCungCap)
                      .WithMany()
                      .HasForeignKey(e => e.MaNhaCungCap)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.NhanVien)
                      .WithMany()
                      .HasForeignKey(e => e.MaNhanVien)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.NgayNhap);
                entity.HasIndex(e => e.TrangThai);
                entity.HasIndex(e => e.MaNhaCungCap);
                entity.HasIndex(e => e.MaNhanVien);
                entity.HasIndex(e => e.NgayTao);
                entity.HasCheckConstraint("CK_PhieuNhap_TongTien", "TongTien >= 0");
                entity.HasCheckConstraint("CK_PhieuNhap_NgayNhap", "NgayNhap <= GETDATE()");
                entity.HasCheckConstraint("CK_PhieuNhap_TrangThai", "TrangThai IN ('Nháp', 'Đã xác nhận')");
            });

            // Cấu hình bảng ChiTietPhieuNhap
            modelBuilder.Entity<ChiTietPhieuNhap>(entity =>
            {
                entity.HasKey(e => new { e.MaPhieuNhap, e.MaLinhKien });
                entity.Property(e => e.SoLuong).IsRequired();
                entity.Property(e => e.GiaNhap).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.ThanhTien).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.GhiChu).HasMaxLength(200);
                entity.HasOne(e => e.PhieuNhap)
                      .WithMany(p => p.ChiTietPhieuNhaps)
                      .HasForeignKey(e => e.MaPhieuNhap)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.LinhKien)
                      .WithMany()
                      .HasForeignKey(e => e.MaLinhKien)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.MaPhieuNhap);
                entity.HasIndex(e => e.MaLinhKien);
                entity.HasCheckConstraint("CK_ChiTietPhieuNhap_SoLuong", "SoLuong > 0");
                entity.HasCheckConstraint("CK_ChiTietPhieuNhap_GiaNhap", "GiaNhap >= 0");
                entity.HasCheckConstraint("CK_ChiTietPhieuNhap_ThanhTien", "ThanhTien >= 0");
            });

            // Cấu hình bảng ThietBi
            modelBuilder.Entity<ThietBi>(entity =>
            {
                entity.HasKey(e => e.MaThietBi);
                entity.Property(e => e.TenThietBi).IsRequired().HasMaxLength(200);
                entity.Property(e => e.LoaiThietBi).IsRequired().HasMaxLength(50);
                entity.Property(e => e.HangSanXuat).HasMaxLength(100);
                entity.Property(e => e.Model).HasMaxLength(100);
                entity.HasOne(e => e.KhachHang)
                      .WithMany()
                      .HasForeignKey(e => e.MaKhachHang)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.TenThietBi);
                entity.HasIndex(e => e.LoaiThietBi);
                entity.HasIndex(e => e.MaKhachHang);
                entity.HasIndex(e => e.HangSanXuat);
                entity.HasIndex(e => e.Model);
            });

            // Cấu hình bảng PhieuSua
            modelBuilder.Entity<PhieuSua>(entity =>
            {
                entity.HasKey(e => e.MaPhieuSua);
                entity.Property(e => e.NgaySua).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.TongTien).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
                entity.Property(e => e.GhiChu).HasMaxLength(500);
                entity.Property(e => e.NgayHenTra).IsRequired(false);
                entity.Property(e => e.NgayTraThucTe).IsRequired(false);
                entity.Property(e => e.TrangThai).HasDefaultValue(TrangThaiPhieuSua.TiepNhan);
                entity.HasIndex(e => e.NgaySua);
                entity.HasIndex(e => e.TrangThai);
                entity.HasIndex(e => e.NgayHenTra);
                entity.HasIndex(e => e.NgayTraThucTe);
                // FK
                entity.HasOne<KhachHang>()
                      .WithMany()
                      .HasForeignKey(e => e.MaKhachHang)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<NhanVien>()
                      .WithMany()
                      .HasForeignKey(e => e.MaNhanVien)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Cấu hình bảng ChiTietPhieuSua
            modelBuilder.Entity<ChiTietPhieuSua>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ThanhTien).HasColumnType("decimal(18,2)");
                entity.Property(e => e.SoLuong).HasDefaultValue(1);
                entity.HasOne(e => e.PhieuSua)
                      .WithMany(p => p.ChiTietPhieuSuas)
                      .HasForeignKey(e => e.MaPhieuSua)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.DichVu)
                      .WithMany()
                      .HasForeignKey(e => e.MaDichVu)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.MaPhieuSua);
                entity.HasIndex(e => e.MaDichVu);
            });

            // Cấu hình bảng ChiTietLinhKien
            modelBuilder.Entity<ChiTietLinhKien>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ThanhTien).HasColumnType("decimal(18,2)");
                entity.Property(e => e.SoLuong).HasDefaultValue(1);
                entity.HasOne(e => e.PhieuSua)
                      .WithMany(p => p.ChiTietLinhKiens)
                      .HasForeignKey(e => e.MaPhieuSua)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.LinhKien)
                      .WithMany()
                      .HasForeignKey(e => e.MaLinhKien)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => e.MaPhieuSua);
                entity.HasIndex(e => e.MaLinhKien);
            });

            // Cấu hình bảng liên kết DịchVuLinhKien
            modelBuilder.Entity<DichVuLinhKien>(entity =>
            {
                entity.HasKey(e => new { e.MaDichVu, e.MaLinhKien });
                entity.HasOne(e => e.DichVu)
                      .WithMany(dv => dv.DichVuLinhKiens)
                      .HasForeignKey(e => e.MaDichVu)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.LinhKien)
                      .WithMany(lk => lk.DichVuLinhKiens)
                      .HasForeignKey(e => e.MaLinhKien)
                      .OnDelete(DeleteBehavior.Restrict);
            });

        }
    }
}