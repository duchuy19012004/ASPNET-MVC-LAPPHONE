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
        public DbSet<ThietBi> ThietBi { get; set; }

        // Các DbSet khác sẽ được thêm sau
        // public DbSet<KhachHang> KhachHang { get; set; }
        // public DbSet<ThietBi> ThietBi { get; set; }

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

                // Index cho tìm kiếm nhanh
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

                // Index cho tìm kiếm và báo cáo
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

                // Index cho tìm kiếm nhanh
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

                // Index cho tìm kiếm nhanh
                entity.HasIndex(e => e.HoTen);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.SoDienThoai);
                entity.HasIndex(e => e.ChucVu);
                entity.HasIndex(e => e.TrangThai);
                entity.HasIndex(e => e.NgayVaoLam);
                entity.HasIndex(e => e.NgayNghiViec);

                // Check constraints
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

                // Index cho tìm kiếm nhanh
                entity.HasIndex(e => e.HoTen);
                entity.HasIndex(e => e.SoDienThoai).IsUnique();
                entity.HasIndex(e => e.TrangThai);
                entity.HasIndex(e => e.NgayTao);
                entity.HasIndex(e => e.TongChiTieu);

                // Check constraints
                entity.HasCheckConstraint("CK_KhachHang_TongChiTieu", "TongChiTieu >= 0");
                entity.HasCheckConstraint("CK_KhachHang_NgayTao", "NgayTao <= GETDATE()");
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

                // Index cho tìm kiếm nhanh
                entity.HasIndex(e => e.HoTen);
                entity.HasIndex(e => e.SoDienThoai).IsUnique();
                entity.HasIndex(e => e.TrangThai);
                entity.HasIndex(e => e.NgayTao);
                entity.HasIndex(e => e.TongChiTieu);

                // Check constraints
                entity.HasCheckConstraint("CK_KhachHang_TongChiTieu", "TongChiTieu >= 0");
                entity.HasCheckConstraint("CK_KhachHang_NgayTao", "NgayTao <= GETDATE()");
            });
            // Cấu hình bảng ThietBi
            modelBuilder.Entity<ThietBi>(entity =>
        {
            entity.HasKey(e => e.MaThietBi);
            entity.Property(e => e.TenThietBi).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LoaiThietBi).IsRequired().HasMaxLength(50);
            entity.Property(e => e.HangSanXuat).HasMaxLength(50);
            entity.Property(e => e.Model).HasMaxLength(50);
            entity.Property(e => e.SoSerial).HasMaxLength(100);
            
            // Index cho tìm kiếm nhanh
            entity.HasIndex(e => e.TenThietBi);
            entity.HasIndex(e => e.LoaiThietBi);
            entity.HasIndex(e => e.HangSanXuat);
            entity.HasIndex(e => e.SoSerial);
            entity.HasIndex(e => e.MaKhachHang);
            
            // Foreign Key
            entity.HasOne(e => e.KhachHang)
                .WithMany()
                .HasForeignKey(e => e.MaKhachHang)
                .OnDelete(DeleteBehavior.Restrict);
        });
        }
    }
}