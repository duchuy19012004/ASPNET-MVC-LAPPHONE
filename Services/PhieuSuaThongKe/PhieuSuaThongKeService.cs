using System.Globalization;
using Microsoft.EntityFrameworkCore;
using phonev2.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace phonev2.Services.PhieuSuaThongKe
{
    public class PhieuSuaThongKeService : IPhieuSuaThongKeService
    {
        private readonly PhoneLapDbContext _context;
        public PhieuSuaThongKeService(PhoneLapDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ThongKePhieuSuaDto>> GetThongKeAsync(string type, int? year, int? month, int? week)
        {
            var query = _context.PhieuSua.AsQueryable();
            var now = DateTime.Now;
            year ??= now.Year;

            // Lọc theo năm
            query = query.Where(p => p.NgaySua.Year == year);

            if (type == "month")
            {
                // Nếu có month, lọc theo tháng
                if (month.HasValue)
                {
                    query = query.Where(p => p.NgaySua.Month == month.Value);
                    // Trả về theo ngày trong tháng
                    var daysInMonth = DateTime.DaysInMonth(year.Value, month.Value);
                    var result = new List<ThongKePhieuSuaDto>();
                    for (int day = 1; day <= daysInMonth; day++)
                    {
                        var items = await query.Where(p => p.NgaySua.Day == day).ToListAsync();
                        result.Add(new ThongKePhieuSuaDto
                        {
                            Label = day.ToString("00"),
                            SoPhieu = items.Count,
                            TongTien = items.Sum(p => p.TongTien ?? 0)
                        });
                    }
                    return result;
                }
                else
                {
                    // Trả về theo tháng trong năm
                    var result = new List<ThongKePhieuSuaDto>();
                    for (int m = 1; m <= 12; m++)
                    {
                        var items = await query.Where(p => p.NgaySua.Month == m).ToListAsync();
                        result.Add(new ThongKePhieuSuaDto
                        {
                            Label = m.ToString("00"),
                            SoPhieu = items.Count,
                            TongTien = items.Sum(p => p.TongTien ?? 0)
                        });
                    }
                    return result;
                }
            }
            else if (type == "week")
            {
                // Lọc theo tuần trong năm
                var calendar = CultureInfo.CurrentCulture.Calendar;
                var weekRule = CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
                var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
                var weeks = Enumerable.Range(1, 53);
                var result = new List<ThongKePhieuSuaDto>();
                foreach (var w in weeks)
                {
                    var items = await query.Where(p => calendar.GetWeekOfYear(p.NgaySua, weekRule, firstDayOfWeek) == w).ToListAsync();
                    if (items.Count == 0 && w > now.GetIso8601WeekOfYear()) break;
                    result.Add(new ThongKePhieuSuaDto
                    {
                        Label = $"Tuần {w}",
                        SoPhieu = items.Count,
                        TongTien = items.Sum(p => p.TongTien ?? 0)
                    });
                }
                return result;
            }
            else // type == "year"
            {
                // Trả về theo năm (mỗi năm 1 cột)
                var years = await _context.PhieuSua.Select(p => p.NgaySua.Year).Distinct().OrderBy(y => y).ToListAsync();
                var result = new List<ThongKePhieuSuaDto>();
                foreach (var y in years)
                {
                    var items = await _context.PhieuSua.Where(p => p.NgaySua.Year == y).ToListAsync();
                    result.Add(new ThongKePhieuSuaDto
                    {
                        Label = y.ToString(),
                        SoPhieu = items.Count,
                        TongTien = items.Sum(p => p.TongTien ?? 0)
                    });
                }
                return result;
            }
        }

        public async Task<IEnumerable<TopLinhKienDto>> GetTopLinhKienAsync(int top, string type, int? year, int? month, int? week)
        {
            var query = _context.ChiTietLinhKien
                .Include(ct => ct.LinhKien)
                .Include(ct => ct.PhieuSua)
                .AsQueryable();
            var now = DateTime.Now;
            year ??= now.Year;
            query = query.Where(ct => ct.PhieuSua.NgaySua.Year == year);
            if (type == "month" && month.HasValue)
                query = query.Where(ct => ct.PhieuSua.NgaySua.Month == month.Value);
            if (type == "week" && week.HasValue)
            {
                var calendar = CultureInfo.CurrentCulture.Calendar;
                var weekRule = CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
                var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
                query = query.Where(ct => calendar.GetWeekOfYear(ct.PhieuSua.NgaySua, weekRule, firstDayOfWeek) == week.Value);
            }
            var result = await query
                .GroupBy(ct => ct.LinhKien.TenLinhKien)
                .Select(g => new TopLinhKienDto
                {
                    TenLinhKien = g.Key,
                    SoLan = g.Sum(x => x.SoLuong)
                })
                .OrderByDescending(x => x.SoLan)
                .Take(top)
                .ToListAsync();
            return result;
        }

        public async Task<IEnumerable<TopLoaiLinhKienDto>> GetThongKeLoaiLinhKienAsync(string type, int? year, int? month, int? week)
        {
            var query = _context.ChiTietLinhKien
                .Include(ct => ct.LinhKien).ThenInclude(lk => lk.LoaiLinhKien)
                .Include(ct => ct.PhieuSua)
                .AsQueryable();
            var now = DateTime.Now;
            year ??= now.Year;
            query = query.Where(ct => ct.PhieuSua.NgaySua.Year == year);
            if (type == "month" && month.HasValue)
                query = query.Where(ct => ct.PhieuSua.NgaySua.Month == month.Value);
            if (type == "week" && week.HasValue)
            {
                var calendar = CultureInfo.CurrentCulture.Calendar;
                var weekRule = CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
                var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
                query = query.Where(ct => calendar.GetWeekOfYear(ct.PhieuSua.NgaySua, weekRule, firstDayOfWeek) == week.Value);
            }
            var result = await query
                .GroupBy(ct => ct.LinhKien.LoaiLinhKien.TenLoaiLinhKien)
                .Select(g => new TopLoaiLinhKienDto
                {
                    TenLoaiLinhKien = g.Key,
                    SoLan = g.Sum(x => x.SoLuong)
                })
                .OrderByDescending(x => x.SoLan)
                .ToListAsync();
            return result;
        }
    }

    public static class DateTimeExtensions
    {
        // Chuẩn hóa lấy số tuần ISO 8601
        public static int GetIso8601WeekOfYear(this DateTime time)
        {
            var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
} 