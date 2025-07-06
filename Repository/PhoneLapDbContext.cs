using Microsoft.EntityFrameworkCore;
using phonev2.Models;

namespace phonev2.Repository
{
    public class PhoneLapDbContext : DbContext
    {
        public PhoneLapDbContext(DbContextOptions<PhoneLapDbContext> options) : base(options)
        {
        }

        // DbSet cho Dịch Vụ
        public DbSet<DichVu> DichVu { get; set; }

        // Các DbSet khác sẽ được thêm sau
        // public DbSet<KhachHang> KhachHangs { get; set; }
        // public DbSet<NhanVien> NhanViens { get; set; }
        // public DbSet<LinhKien> LinhKiens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình bảng DichVu
            modelBuilder.Entity<DichVu>(entity =>
            {
                entity.HasKey(e => e.MaDichVu);
                
                entity.Property(e => e.TenDichVu)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.MoTa)
                    .HasMaxLength(500);
                
                entity.Property(e => e.GiaDichVu)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
                
                entity.Property(e => e.NgayTao)
                    .HasDefaultValueSql("GETDATE()");
                
                entity.Property(e => e.TrangThai)
                    .HasDefaultValue(true);

                // Index cho tìm kiếm nhanh
                entity.HasIndex(e => e.TenDichVu);
                entity.HasIndex(e => e.TrangThai);
            });
        }
    }
}