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

        // Các DbSet khác sẽ được thêm sau
        // public DbSet<NhaCungCap> NhaCungCap { get; set; }
        // public DbSet<LinhKien> LinhKien { get; set; }
        // public DbSet<KhachHang> KhachHang { get; set; }

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
        }
    }
}