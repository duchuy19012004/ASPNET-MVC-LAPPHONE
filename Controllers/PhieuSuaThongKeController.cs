using Microsoft.AspNetCore.Mvc;
using phonev2.Services.PhieuSuaThongKe;

namespace phonev2.Controllers
{
    public class PhieuSuaThongKeController : Controller
    {
        private readonly IPhieuSuaThongKeService _thongKeService;
        public PhieuSuaThongKeController(IPhieuSuaThongKeService thongKeService)
        {
            _thongKeService = thongKeService;
        }

        // View báo cáo
        public IActionResult Index()
        {
            return View();
        }

        // API trả về dữ liệu thống kê cho biểu đồ
        [HttpGet]
        public async Task<IActionResult> GetThongKe(string type = "month", int? year = null, int? month = null, int? week = null)
        {
            var data = await _thongKeService.GetThongKeAsync(type, year, month, week);
            return Json(data);
        }

        // API: Top 5 linh kiện sử dụng nhiều nhất
        [HttpGet]
        public async Task<IActionResult> GetTopLinhKien(string type = "month", int? year = null, int? month = null, int? week = null, bool today = false)
        {
            if (today)
            {
                var now = DateTime.Now;
                year = now.Year;
                month = now.Month;
                type = "month";
            }
            var data = await _thongKeService.GetTopLinhKienAsync(5, type, year, month, week);
            return Json(data);
        }

        // API: Thống kê theo loại linh kiện
        [HttpGet]
        public async Task<IActionResult> GetThongKeLoaiLinhKien(string type = "month", int? year = null, int? month = null, int? week = null, bool today = false)
        {
            if (today)
            {
                var now = DateTime.Now;
                year = now.Year;
                month = now.Month;
                type = "month";
            }
            var data = await _thongKeService.GetThongKeLoaiLinhKienAsync(type, year, month, week);
            return Json(data);
        }
    }
} 