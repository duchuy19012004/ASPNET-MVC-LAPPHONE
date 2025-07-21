# TEST CASES - MODULE LINH KIỆN

## 1. CHỨC NĂNG THÊM LINH KIỆN MỚI

### 1.1 Test Case Cơ Bản

- **TC001**: Thêm linh kiện với đầy đủ thông tin hợp lệ

  - Input: Tên linh kiện, loại linh kiện, giá nhập, giá bán, số lượng tồn
  - Expected: Lưu thành công, hiển thị thông báo thành công
  - Status: ⏳

- **TC002**: Thêm linh kiện với thông tin tối thiểu
  - Input: Chỉ nhập các trường bắt buộc (tên, loại, giá nhập, giá bán, số lượng)
  - Expected: Lưu thành công
  - Status: ⏳

### 1.2 Test Case Validation

- **TC003**: Thêm linh kiện không nhập tên

  - Input: Để trống tên linh kiện
  - Expected: Hiển thị lỗi "Tên linh kiện là bắt buộc"
  - Status: ⏳

- **TC004**: Thêm linh kiện không chọn loại

  - Input: Không chọn loại linh kiện
  - Expected: Hiển thị lỗi "Loại linh kiện là bắt buộc"
  - Status: ⏳

- **TC005**: Thêm linh kiện với giá nhập âm

  - Input: Giá nhập = -1000
  - Expected: Hiển thị lỗi "Giá nhập phải lớn hơn hoặc bằng 0"
  - Status: ⏳

- **TC006**: Thêm linh kiện với giá bán âm

  - Input: Giá bán = -500
  - Expected: Hiển thị lỗi "Giá bán phải lớn hơn hoặc bằng 0"
  - Status: ⏳

- **TC007**: Thêm linh kiện với số lượng âm

  - Input: Số lượng = -10
  - Expected: Hiển thị lỗi "Số lượng tồn phải lớn hơn hoặc bằng 0"
  - Status: ⏳

- **TC008**: Thêm linh kiện với tên quá dài (>200 ký tự)

  - Input: Tên linh kiện 250 ký tự
  - Expected: Hiển thị lỗi "Tên linh kiện không được vượt quá 200 ký tự"
  - Status: ⏳

- **TC009**: Thêm linh kiện với hãng sản xuất quá dài (>100 ký tự)
  - Input: Hãng sản xuất 150 ký tự
  - Expected: Hiển thị lỗi "Hãng sản xuất không được vượt quá 100 ký tự"
  - Status: ⏳

### 1.3 Test Case Edge Cases

- **TC010**: Thêm linh kiện với giá nhập = 0

  - Input: Giá nhập = 0
  - Expected: Lưu thành công (cho phép)
  - Status: ⏳

- **TC011**: Thêm linh kiện với giá bán = 0

  - Input: Giá bán = 0
  - Expected: Lưu thành công (cho phép)
  - Status: ⏳

- **TC012**: Thêm linh kiện với số lượng = 0
  - Input: Số lượng = 0
  - Expected: Lưu thành công (hết hàng)
  - Status: ⏳

## 2. CHỨC NĂNG CHỈNH SỬA LINH KIỆN

### 2.1 Test Case Modal Edit

- **TC013**: Mở modal chỉnh sửa linh kiện

  - Action: Click nút "Chỉnh sửa"
  - Expected: Modal mở với dữ liệu đúng
  - Status: ⏳

- **TC014**: Chỉnh sửa tên linh kiện

  - Input: Thay đổi tên linh kiện
  - Expected: Lưu thành công với tên mới
  - Status: ⏳

- **TC015**: Chỉnh sửa loại linh kiện

  - Input: Thay đổi loại linh kiện
  - Expected: Lưu thành công với loại mới
  - Status: ⏳

- **TC016**: Chỉnh sửa giá nhập

  - Input: Thay đổi giá nhập
  - Expected: Lưu thành công, cập nhật lợi nhuận
  - Status: ⏳

- **TC017**: Chỉnh sửa giá bán

  - Input: Thay đổi giá bán
  - Expected: Lưu thành công, cập nhật lợi nhuận
  - Status: ⏳

- **TC018**: Chỉnh sửa số lượng tồn
  - Input: Thay đổi số lượng
  - Expected: Lưu thành công
  - Status: ⏳

### 2.2 Test Case Trạng Thái

- **TC019**: Bật trạng thái "Đang bán"

  - Input: Check checkbox "Linh kiện đang được bán"
  - Expected: Lưu thành công với TrangThai = true
  - Status: ⏳

- **TC020**: Tắt trạng thái "Ngừng bán"
  - Input: Uncheck checkbox "Linh kiện đang được bán"
  - Expected: Lưu thành công với TrangThai = false
  - Status: ⏳

### 2.3 Test Case Gợi Ý Giá

- **TC021**: Click gợi ý lợi nhuận 20%

  - Action: Click button "LN 20%"
  - Expected: Giá bán được cập nhật = Giá nhập \* 1.2
  - Status: ⏳

- **TC022**: Click gợi ý lợi nhuận 30%

  - Action: Click button "LN 30%"
  - Expected: Giá bán được cập nhật = Giá nhập \* 1.3
  - Status: ⏳

- **TC023**: Click gợi ý lợi nhuận 50%
  - Action: Click button "LN 50%"
  - Expected: Giá bán được cập nhật = Giá nhập \* 1.5
  - Status: ⏳

### 2.4 Test Case Validation Edit

- **TC024**: Chỉnh sửa với tên rỗng

  - Input: Xóa tên linh kiện
  - Expected: Hiển thị lỗi validation
  - Status: ⏳

- **TC025**: Chỉnh sửa với giá nhập âm
  - Input: Giá nhập = -100
  - Expected: Hiển thị lỗi validation
  - Status: ⏳

## 3. CHỨC NĂNG XÓA LINH KIỆN

### 3.1 Test Case Soft Delete

- **TC026**: Xóa linh kiện (soft delete)

  - Action: Click nút "Xóa" và xác nhận
  - Expected: Linh kiện bị đánh dấu đã xóa, không hiển thị trong danh sách chính
  - Status: ⏳

- **TC027**: Xóa linh kiện đang được sử dụng trong phiếu sửa
  - Action: Xóa linh kiện có trong chi tiết phiếu sửa
  - Expected: Hiển thị thông báo không thể xóa
  - Status: ⏳

### 3.2 Test Case Khôi Phục

- **TC028**: Khôi phục linh kiện đã xóa
  - Action: Vào trang "Linh Kiện Đã Xóa" và click "Khôi phục"
  - Expected: Linh kiện được khôi phục, hiển thị lại trong danh sách chính
  - Status: ⏳

### 3.3 Test Case Xóa Vĩnh Viễn

- **TC029**: Xóa vĩnh viễn linh kiện
  - Action: Xóa vĩnh viễn linh kiện đã soft delete
  - Expected: Linh kiện bị xóa hoàn toàn khỏi database
  - Status: ⏳

## 4. CHỨC NĂNG TÌM KIẾM VÀ LỌC

### 4.1 Test Case Tìm Kiếm

- **TC030**: Tìm kiếm theo tên linh kiện

  - Input: Nhập tên linh kiện
  - Expected: Hiển thị kết quả chứa tên đó
  - Status: ⏳

- **TC031**: Tìm kiếm theo hãng sản xuất

  - Input: Nhập tên hãng
  - Expected: Hiển thị linh kiện của hãng đó
  - Status: ⏳

- **TC032**: Tìm kiếm theo thông số kỹ thuật

  - Input: Nhập thông số
  - Expected: Hiển thị linh kiện có thông số đó
  - Status: ⏳

- **TC033**: Tìm kiếm với từ khóa không tồn tại
  - Input: Nhập từ khóa không có trong database
  - Expected: Hiển thị thông báo "Không tìm thấy linh kiện nào"
  - Status: ⏳

### 4.2 Test Case Lọc

- **TC034**: Lọc theo loại linh kiện

  - Action: Chọn loại linh kiện từ dropdown
  - Expected: Hiển thị chỉ linh kiện thuộc loại đó
  - Status: ⏳

- **TC035**: Lọc theo trạng thái tồn kho

  - Action: Chọn "Hết hàng"
  - Expected: Hiển thị linh kiện có số lượng = 0
  - Status: ⏳

- **TC036**: Lọc theo trạng thái tồn kho "Sắp hết"
  - Action: Chọn "Sắp hết (≤5)"
  - Expected: Hiển thị linh kiện có số lượng ≤ 5
  - Status: ⏳

### 4.3 Test Case Sắp Xếp

- **TC037**: Sắp xếp theo tên A-Z

  - Action: Chọn "Tên A-Z"
  - Expected: Danh sách sắp xếp theo tên tăng dần
  - Status: ⏳

- **TC038**: Sắp xếp theo tên Z-A

  - Action: Chọn "Tên Z-A"
  - Expected: Danh sách sắp xếp theo tên giảm dần
  - Status: ⏳

- **TC039**: Sắp xếp theo giá thấp → cao

  - Action: Chọn "Giá thấp → cao"
  - Expected: Danh sách sắp xếp theo giá bán tăng dần
  - Status: ⏳

- **TC040**: Sắp xếp theo giá cao → thấp
  - Action: Chọn "Giá cao → thấp"
  - Expected: Danh sách sắp xếp theo giá bán giảm dần
  - Status: ⏳

## 5. CHỨC NĂNG CẬP NHẬT TỒN KHO

### 5.1 Test Case Cộng Thêm

- **TC041**: Cộng thêm tồn kho
  - Action: Click nút "+" và nhập số lượng
  - Expected: Số lượng tồn tăng lên
  - Status: ⏳

### 5.2 Test Case Trừ Bớt

- **TC042**: Trừ bớt tồn kho
  - Action: Click nút "-" và nhập số lượng
  - Expected: Số lượng tồn giảm đi
  - Status: ⏳

### 5.3 Test Case Đặt Lại

- **TC043**: Đặt lại tồn kho
  - Action: Click nút "=" và nhập số lượng mới
  - Expected: Số lượng tồn được đặt lại
  - Status: ⏳

### 5.4 Test Case Validation Tồn Kho

- **TC044**: Trừ bớt quá số lượng hiện có

  - Action: Trừ bớt nhiều hơn số lượng tồn
  - Expected: Hiển thị lỗi "Số lượng không đủ"
  - Status: ⏳

- **TC045**: Nhập số lượng âm
  - Action: Nhập số lượng âm
  - Expected: Hiển thị lỗi "Số lượng không được âm"
  - Status: ⏳

## 6. CHỨC NĂNG BÁO CÁO

### 6.1 Test Case Báo Cáo Tồn Kho

- **TC046**: Xem báo cáo tồn kho
  - Action: Click "Báo Cáo Tồn Kho"
  - Expected: Hiển thị báo cáo theo loại linh kiện
  - Status: ⏳

### 6.2 Test Case Báo Cáo Lợi Nhuận

- **TC047**: Xem báo cáo lợi nhuận
  - Action: Click "Báo Cáo Lợi Nhuận"
  - Expected: Hiển thị báo cáo lợi nhuận
  - Status: ⏳

### 6.3 Test Case Cảnh Báo Tồn Kho Thấp

- **TC048**: Xem cảnh báo tồn kho thấp
  - Action: Click "Cảnh Báo Tồn Kho Thấp"
  - Expected: Hiển thị linh kiện có tồn kho ≤ threshold
  - Status: ⏳

## 7. TEST CASE UI/UX

### 7.1 Test Case Responsive

- **TC049**: Hiển thị trên desktop

  - Action: Mở trang trên desktop
  - Expected: Layout hiển thị đúng, không bị vỡ
  - Status: ⏳

- **TC050**: Hiển thị trên tablet

  - Action: Mở trang trên tablet
  - Expected: Layout responsive, dễ sử dụng
  - Status: ⏳

- **TC051**: Hiển thị trên mobile
  - Action: Mở trang trên mobile
  - Expected: Layout mobile-friendly
  - Status: ⏳

### 7.2 Test Case Modal

- **TC052**: Đóng modal bằng nút X

  - Action: Click nút X trên modal
  - Expected: Modal đóng lại
  - Status: ⏳

- **TC053**: Đóng modal bằng click ngoài

  - Action: Click ngoài modal
  - Expected: Modal đóng lại
  - Status: ⏳

- **TC054**: Đóng modal bằng ESC
  - Action: Nhấn phím ESC
  - Expected: Modal đóng lại
  - Status: ⏳

### 7.3 Test Case Toast Messages

- **TC055**: Hiển thị toast thành công

  - Action: Thực hiện thao tác thành công
  - Expected: Hiển thị toast màu xanh
  - Status: ⏳

- **TC056**: Hiển thị toast lỗi
  - Action: Thực hiện thao tác lỗi
  - Expected: Hiển thị toast màu đỏ
  - Status: ⏳

## 8. TEST CASE PERFORMANCE

### 8.1 Test Case Load Time

- **TC057**: Load trang danh sách nhanh

  - Action: Mở trang danh sách
  - Expected: Load trong < 2 giây
  - Status: ⏳

- **TC058**: Load modal nhanh
  - Action: Mở modal edit
  - Expected: Load trong < 1 giây
  - Status: ⏳

### 8.2 Test Case Pagination

- **TC059**: Chuyển trang

  - Action: Click phân trang
  - Expected: Load trang mới nhanh
  - Status: ⏳

- **TC060**: Thay đổi số lượng/trang
  - Action: Thay đổi pageSize
  - Expected: Reload với số lượng mới
  - Status: ⏳

## 9. TEST CASE SECURITY

### 9.1 Test Case Anti-Forgery

- **TC061**: Submit form không có token
  - Action: Gửi request không có anti-forgery token
  - Expected: Bị từ chối với lỗi 400
  - Status: ⏳

### 9.2 Test Case Input Validation

- **TC062**: Nhập script injection
  - Input: `<script>alert('xss')</script>`
  - Expected: Được encode và không thực thi
  - Status: ⏳

## 10. TEST CASE EDGE CASES

### 10.1 Test Case Dữ Liệu Lớn

- **TC063**: Nhập số lượng rất lớn

  - Input: Số lượng = 999999999
  - Expected: Xử lý được hoặc hiển thị lỗi phù hợp
  - Status: ⏳

- **TC064**: Nhập giá rất lớn
  - Input: Giá = 999999999999
  - Expected: Xử lý được hoặc hiển thị lỗi phù hợp
  - Status: ⏳

### 10.2 Test Case Ký Tự Đặc Biệt

- **TC065**: Nhập tên có ký tự đặc biệt
  - Input: Tên = "RAM DDR4 @#$%^&\*()"
  - Expected: Lưu thành công
  - Status: ⏳

### 10.3 Test Case Unicode

- **TC066**: Nhập tên có tiếng Việt
  - Input: Tên = "Bộ nhớ RAM DDR4"
  - Expected: Lưu và hiển thị đúng
  - Status: ⏳

---

## HƯỚNG DẪN TESTING

### Cách sử dụng:

1. Copy từng test case và thực hiện theo hướng dẫn
2. Đánh dấu ✅ nếu PASS, ❌ nếu FAIL
3. Ghi chú lỗi nếu có
4. Báo cáo kết quả cho team

### Priority:

- **High**: TC001-TC020 (CRUD cơ bản)
- **Medium**: TC021-TC040 (Validation, Search, Sort)
- **Low**: TC041-TC066 (Advanced features, UI/UX)

### Environment:

- Browser: Chrome, Firefox, Edge
- Device: Desktop, Tablet, Mobile
- Data: Test data đa dạng
