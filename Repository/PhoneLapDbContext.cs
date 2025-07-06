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

        // Các DbSet khác sẽ được thêm sau
        // public DbSet<NhaCungCap> NhaCungCap { get; set; }
        // public DbSet<KhachHang> KhachHang { get; set; }
        // public DbSet<NhanVien> NhanVien { get; set; }

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
        }
    }
}