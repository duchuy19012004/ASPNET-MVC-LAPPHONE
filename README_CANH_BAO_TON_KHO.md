# Hệ Thống Cảnh Báo Tồn Kho - Linh Kiện

## Tổng Quan

Hệ thống cảnh báo tồn kho được thiết kế để giúp quản lý hiệu quả việc theo dõi và cảnh báo khi linh kiện có tồn kho thấp, đảm bảo không bị gián đoạn kinh doanh.

## Các Tính Năng Chính

### 1. Cảnh Báo Tồn Kho Thấp (`LowStockAlert`)

- **Mục đích**: Hiển thị danh sách linh kiện có tồn kho ≤ ngưỡng cảnh báo
- **Truy cập**: `/LinhKien/LowStockAlert`
- **Tính năng**:
  - Lọc theo ngưỡng cảnh báo (5, 10, 20, 50 cái)
  - Phân loại theo mức độ: Hết hàng, Sắp hết, Ít hàng
  - Nhập hàng nhanh (từng linh kiện hoặc hàng loạt)
  - Xuất danh sách CSV
  - Đề xuất số lượng nhập thêm

### 2. Báo Cáo Tồn Kho (`StockReport`)

- **Mục đích**: Báo cáo tổng quan tồn kho theo loại linh kiện
- **Truy cập**: `/LinhKien/StockReport`
- **Tính năng**:
  - Biểu đồ cột hiển thị tồn kho theo loại
  - Biểu đồ tròn phân bố tồn kho
  - Thống kê tổng quan (tổng mặt hàng, hết hàng, tồn kho thấp)
  - Top 5 loại có nhiều hàng nhất
  - Xuất báo cáo CSV

### 3. Báo Cáo Lợi Nhuận (`ProfitReport`)

- **Mục đích**: Phân tích lợi nhuận theo linh kiện
- **Truy cập**: `/LinhKien/ProfitReport`
- **Tính năng**:
  - Top 10 linh kiện có lợi nhuận cao nhất
  - Phân bố lợi nhuận theo loại
  - Thống kê tổng lợi nhuận, doanh thu, chi phí
  - Tỷ lệ lợi nhuận trung bình

## Cách Sử Dụng

### Truy Cập Cảnh Báo Tồn Kho

1. **Từ trang danh sách linh kiện**:

   - Click nút "Cảnh Báo Tồn Kho" (màu vàng)
   - Hoặc truy cập trực tiếp: `/LinhKien/LowStockAlert`

2. **Từ sidebar**:
   - Vào "Quản Lý Sản Phẩm" → "Linh Kiện"
   - Click "Cảnh Báo Tồn Kho"

### Thiết Lập Ngưỡng Cảnh Báo

1. **Ngưỡng mặc định**: 10 cái
2. **Thay đổi ngưỡng**:
   - Nhập số vào ô "Ngưỡng cảnh báo"
   - Click "Lọc"
   - Hoặc click các nút nhanh: ≤5, ≤10, ≤20, ≤50

### Nhập Hàng Nhanh

#### Nhập Từng Linh Kiện

1. Click nút "+" (màu xanh) để nhập theo đề xuất
2. Click nút "✏️" (màu xanh dương) để nhập số lượng tùy chọn
3. Xác nhận số lượng và thực hiện

#### Nhập Hàng Loạt

1. Chọn các linh kiện cần nhập (checkbox)
2. Click "Nhập Hàng Đã Chọn"
3. Nhập số lượng chung cho tất cả
4. Xác nhận và thực hiện

### Xuất Báo Cáo

1. **Xuất danh sách cảnh báo**:

   - Click "Xuất Danh Sách" trong trang LowStockAlert
   - File CSV sẽ được tải về

2. **Xuất báo cáo tồn kho**:

   - Click "Xuất Báo Cáo" trong trang StockReport
   - File CSV chứa thống kê theo loại

3. **Xuất báo cáo lợi nhuận**:
   - Click "Xuất Báo Cáo" trong trang ProfitReport
   - File CSV chứa chi tiết lợi nhuận

## Màu Sắc Cảnh Báo

### Trong Bảng Dữ Liệu

- **🔴 Đỏ**: Hết hàng (0 cái)
- **🟡 Vàng**: Sắp hết (1-5 cái)
- **🔵 Xanh dương**: Ít hàng (6-20 cái)
- **🟢 Xanh lá**: Còn nhiều (>20 cái)

### Trong Thống Kê

- **Đỏ**: Hết hàng
- **Vàng**: Tồn kho thấp
- **Xanh dương**: Tồn kho bình thường
- **Xanh lá**: Tồn kho tốt

## Cấu Trúc Dữ Liệu

### Model LinhKien

```csharp
public class LinhKien
{
    public int MaLinhKien { get; set; }
    public string TenLinhKien { get; set; }
    public int MaLoaiLinhKien { get; set; }
    public string? HangSanXuat { get; set; }
    public decimal GiaNhap { get; set; }
    public decimal GiaBan { get; set; }
    public int SoLuongTon { get; set; }
    public string? ThongSoKyThuat { get; set; }
    public DateTime NgayTao { get; set; }
    public bool TrangThai { get; set; }

    // Navigation
    public virtual LoaiLinhKien? LoaiLinhKien { get; set; }

    // Computed properties
    public string TrangThaiTonKho { get; }
    public string TonKhoCssClass { get; }
    public decimal LoiNhuan { get; }
    public decimal TyLeLoiNhuan { get; }
}
```

### Services

- **ILinhKienService**: CRUD cơ bản và tìm kiếm
- **ILinhKienStatisticsService**: Báo cáo và thống kê
- **ILinhKienValidationService**: Validation dữ liệu

## API Endpoints

### Cảnh Báo Tồn Kho

```
GET /LinhKien/LowStockAlert?threshold=10
```

### Báo Cáo Tồn Kho

```
GET /LinhKien/StockReport
```

### Báo Cáo Lợi Nhuận

```
GET /LinhKien/ProfitReport
```

### Cập Nhật Tồn Kho (AJAX)

```
POST /LinhKien/UpdateStock
{
    "id": 1,
    "quantity": 10,
    "action": "add" // add, subtract, set
}
```

## Tùy Chỉnh

### Thay Đổi Ngưỡng Mặc Định

Trong `LinhKienController.cs`:

```csharp
public async Task<IActionResult> LowStockAlert(int threshold = 10)
```

### Thay Đổi Màu Sắc

Trong `wwwroot/css/baocao.css`:

```css
.badge.bg-danger {
  /* Hết hàng */
}
.badge.bg-warning {
  /* Sắp hết */
}
.badge.bg-info {
  /* Ít hàng */
}
.badge.bg-success {
  /* Còn nhiều */
}
```

### Thêm Loại Cảnh Báo Mới

Trong `Models/LinhKien.cs`:

```csharp
public string TrangThaiTonKho
{
    get
    {
        if (SoLuongTon == 0) return "Hết hàng";
        if (SoLuongTon <= 5) return "Sắp hết";
        if (SoLuongTon <= 20) return "Ít hàng";
        return "Còn hàng";
    }
}
```

## Lưu Ý Quan Trọng

1. **Backup dữ liệu**: Luôn backup trước khi thực hiện nhập hàng hàng loạt
2. **Kiểm tra giá**: Đảm bảo giá nhập và giá bán hợp lý
3. **Theo dõi định kỳ**: Kiểm tra cảnh báo tồn kho hàng ngày
4. **Cập nhật thông tin**: Cập nhật giá và thông tin linh kiện thường xuyên

## Hỗ Trợ

Nếu gặp vấn đề hoặc cần hỗ trợ:

1. Kiểm tra log lỗi trong console
2. Xác nhận kết nối database
3. Kiểm tra quyền truy cập file
4. Liên hệ admin để được hỗ trợ
