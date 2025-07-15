// JavaScript cho trang báo cáo
document.addEventListener("DOMContentLoaded", function () {
  // Khởi tạo dropdown menu
  initDropdowns();

  // Khởi tạo tooltips
  initTooltips();

  // Khởi tạo responsive table
  initResponsiveTable();
});

function initDropdowns() {
  // Bootstrap 5 dropdown tự động xử lý, không cần code thêm
  // Chỉ cần đảm bảo có data-bs-toggle="dropdown"
  const dropdownToggles = document.querySelectorAll(".dropdown-toggle");
  dropdownToggles.forEach((toggle) => {
    if (!toggle.hasAttribute("data-bs-toggle")) {
      toggle.setAttribute("data-bs-toggle", "dropdown");
    }
  });
}

function initTooltips() {
  // Thêm tooltip cho các card thống kê
  const cards = document.querySelectorAll(".card");
  cards.forEach((card) => {
    card.addEventListener("mouseenter", function () {
      this.style.transform = "translateY(-2px)";
      this.style.transition = "transform 0.2s ease";
    });

    card.addEventListener("mouseleave", function () {
      this.style.transform = "translateY(0)";
    });
  });
}

function initResponsiveTable() {
  // Thêm responsive cho bảng
  const tables = document.querySelectorAll(".table-responsive");
  tables.forEach((table) => {
    const tableElement = table.querySelector("table");
    if (tableElement) {
      // Thêm class để styling responsive
      tableElement.classList.add("table-responsive");
    }
  });
}

// Hàm format số tiền
function formatCurrency(amount) {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
  }).format(amount);
}

// Hàm format ngày tháng
function formatDate(dateString) {
  const date = new Date(dateString);
  return date.toLocaleDateString("vi-VN");
}

// Hàm export dữ liệu ra Excel (có thể mở rộng sau)
function exportToExcel() {
  // TODO: Implement export functionality
  alert("Tính năng export sẽ được phát triển sau!");
}

// Hàm in báo cáo
function printReport() {
  window.print();
}

// Hàm test dữ liệu
function testData() {
  fetch("/BaoCao/TestData")
    .then((response) => response.json())
    .then((data) => {
      console.log("=== TEST DATA RESULT ===");
      console.log("Tổng phiếu sửa:", data.TongPhieuSua);
      console.log("Phiếu tháng này:", data.PhieuSuaThangNay);
      console.log("Phiếu gần nhất:", data.PhieuSuaGanNhat);
      console.log("Thống kê trạng thái:", data.ThongKeTrangThai);

      alert(
        `Tổng phiếu: ${data.TongPhieuSua}\nPhiếu tháng này: ${data.PhieuSuaThangNay}\nXem console để chi tiết`
      );
    })
    .catch((error) => {
      console.error("Error testing data:", error);
      alert("Lỗi khi test dữ liệu: " + error.message);
    });
}

// Hàm load dữ liệu test cho biểu đồ
function loadTestData() {
  // Tạo dữ liệu test ngẫu nhiên
  const testMonthlyData = [];
  const testDailyData = [];

  // Tạo dữ liệu 12 tháng
  for (let i = 11; i >= 0; i--) {
    const date = new Date();
    date.setMonth(date.getMonth() - i);
    const monthStr = date.toLocaleDateString("vi-VN", {
      month: "2-digit",
      year: "numeric",
    });
    testMonthlyData.push({
      Thang: monthStr,
      SoPhieuSua: Math.floor(Math.random() * 20) + 5,
      DoanhThu: Math.floor(Math.random() * 2000000) + 500000,
    });
  }

  // Tạo dữ liệu 30 ngày
  for (let i = 29; i >= 0; i--) {
    const date = new Date();
    date.setDate(date.getDate() - i);
    const dayStr = date.toLocaleDateString("vi-VN", {
      day: "2-digit",
      month: "2-digit",
    });
    testDailyData.push({
      Ngay: dayStr,
      SoPhieuSua: Math.floor(Math.random() * 8) + 1,
      DoanhThu: Math.floor(Math.random() * 800000) + 100000,
    });
  }

  // Cập nhật biểu đồ với dữ liệu test
  if (typeof revenueChart !== "undefined") {
    revenueChart.data.labels = testMonthlyData.map((item) => item.Thang);
    revenueChart.data.datasets[0].data = testMonthlyData.map(
      (item) => item.DoanhThu
    );
    revenueChart.update();
  }

  if (typeof phieuChart !== "undefined") {
    phieuChart.data.labels = testMonthlyData.map((item) => item.Thang);
    phieuChart.data.datasets[0].data = testMonthlyData.map(
      (item) => item.SoPhieuSua
    );
    phieuChart.update();
  }

  // Ẩn thông báo không có dữ liệu
  const revenueNoData = document.getElementById("revenueNoData");
  const phieuNoData = document.getElementById("phieuNoData");
  if (revenueNoData) revenueNoData.style.display = "none";
  if (phieuNoData) phieuNoData.style.display = "none";

  alert("Đã load dữ liệu test cho biểu đồ!");
}

// Hàm refresh dữ liệu
function refreshData() {
  location.reload();
}
