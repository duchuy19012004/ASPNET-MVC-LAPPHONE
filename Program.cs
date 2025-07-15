using Microsoft.EntityFrameworkCore;
using phonev2.Repository;
using phonev2.Services.KhachHang;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Đăng ký các service cho DI
builder.Services.AddScoped<phonev2.Services.PhieuSua.IPhieuSuaService, phonev2.Services.PhieuSua.PhieuSuaService>();
builder.Services.AddScoped<phonev2.Services.PhieuSua.IPhieuSuaStatisticsService, phonev2.Services.PhieuSua.PhieuSuaStatisticsService>();
builder.Services.AddScoped<phonev2.Services.PhieuSua.IPhieuSuaValidationService, phonev2.Services.PhieuSua.PhieuSuaValidationService>();
builder.Services.AddScoped<phonev2.Services.PhieuSua.IPhieuSuaCalculationService, phonev2.Services.PhieuSua.PhieuSuaCalculationService>();

// Đăng ký service cho PhieuSuaThongKe
builder.Services.AddScoped<phonev2.Services.PhieuSuaThongKe.IPhieuSuaThongKeService, phonev2.Services.PhieuSuaThongKe.PhieuSuaThongKeService>();

// Đăng ký services cho LoaiLinhKien
builder.Services.AddScoped<phonev2.Services.LoaiLinhKien.ILoaiLinhKienService, phonev2.Services.LoaiLinhKien.LoaiLinhKienService>();
builder.Services.AddScoped<phonev2.Services.LoaiLinhKien.ILoaiLinhKienValidationService, phonev2.Services.LoaiLinhKien.LoaiLinhKienValidationService>();
builder.Services.AddScoped<phonev2.Services.LoaiLinhKien.ILoaiLinhKienStatisticsService, phonev2.Services.LoaiLinhKien.LoaiLinhKienStatisticsService>();

// Đăng ký services cho LinhKien
builder.Services.AddScoped<phonev2.Services.LinhKien.ILinhKienService, phonev2.Services.LinhKien.LinhKienService>();
builder.Services.AddScoped<phonev2.Services.LinhKien.ILinhKienValidationService, phonev2.Services.LinhKien.LinhKienValidationService>();
builder.Services.AddScoped<phonev2.Services.LinhKien.ILinhKienStatisticsService, phonev2.Services.LinhKien.LinhKienStatisticsService>();

// Đăng ký service cho KhachHang
builder.Services.AddScoped<IKhachHangService, KhachHangService>();

// Cấu hình Entity Framework với SQL Server
builder.Services.AddDbContext<PhoneLapDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BikeConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Dashboard}/{id?}");

app.Run();