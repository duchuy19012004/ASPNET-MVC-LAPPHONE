using Microsoft.EntityFrameworkCore;
using phonev2.Models;

namespace phonev2.Repository
{
    public class PhoneLapDbContext : DbContext
    {
        public PhoneLapDbContext(DbContextOptions<PhoneLapDbContext> options) : base(options)
        {
        }

        // Định nghĩa các DbSet cho các bảng của bạn
        // Ví dụ:
        // public DbSet<Customer> Customers { get; set; }
        // public DbSet<Phone> Phones { get; set; }
        // public DbSet<Laptop> Laptops { get; set; }
        // public DbSet<RepairOrder> RepairOrders { get; set; }
        // public DbSet<Service> Services { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình các mối quan hệ và ràng buộc của database tại đây
            // Ví dụ:
            // modelBuilder.Entity<RepairOrder>()
            //     .HasOne(r => r.Customer)
            //     .WithMany(c => c.RepairOrders)
            //     .HasForeignKey(r => r.CustomerId);
        }
    }
}