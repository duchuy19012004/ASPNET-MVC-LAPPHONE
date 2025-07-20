// JavaScript cho các trang báo cáo

// Toast notification function
function showToast(message, type = "success") {
  const toast = document.createElement("div");
  toast.className = `toast-custom ${type}`;
  toast.innerHTML = `
        <div class="p-3">
            <div class="d-flex justify-content-between align-items-center">
                <span>${message}</span>
                <button type="button" class="btn-close" onclick="this.parentElement.parentElement.parentElement.remove()"></button>
            </div>
        </div>
    `;

  document.body.appendChild(toast);

  // Auto remove after 5 seconds
  setTimeout(() => {
    if (toast.parentElement) {
      toast.remove();
    }
  }, 5000);
}

// Format currency function
function formatCurrency(amount) {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
  }).format(amount);
}

// Format number function
function formatNumber(num) {
  return new Intl.NumberFormat("vi-VN").format(num);
}

// Export to CSV function
function exportToCSV(data, filename) {
  let csvContent = "data:text/csv;charset=utf-8,";

  // Add headers
  if (data.length > 0) {
    csvContent += Object.keys(data[0]).join(",") + "\n";
  }

  // Add data rows
  data.forEach((row) => {
    const values = Object.values(row).map((value) => {
      // Handle special characters and quotes
      if (typeof value === "string" && value.includes(",")) {
        return `"${value}"`;
      }
      return value;
    });
    csvContent += values.join(",") + "\n";
  });

  const encodedUri = encodeURI(csvContent);
  const link = document.createElement("a");
  link.setAttribute("href", encodedUri);
  link.setAttribute("download", filename);
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
}

// Print function
function printReport() {
  window.print();
}

// Chart configuration
const chartColors = {
  primary: "rgba(54, 162, 235, 0.8)",
  success: "rgba(75, 192, 192, 0.8)",
  warning: "rgba(255, 205, 86, 0.8)",
  danger: "rgba(255, 99, 132, 0.8)",
  info: "rgba(153, 102, 255, 0.8)",
  secondary: "rgba(201, 203, 207, 0.8)",
};

const chartBorderColors = {
  primary: "rgba(54, 162, 235, 1)",
  success: "rgba(75, 192, 192, 1)",
  warning: "rgba(255, 205, 86, 1)",
  danger: "rgba(255, 99, 132, 1)",
  info: "rgba(153, 102, 255, 1)",
  secondary: "rgba(201, 203, 207, 1)",
};

// Common chart options
const commonChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: "top",
      labels: {
        usePointStyle: true,
        padding: 20,
      },
    },
    tooltip: {
      backgroundColor: "rgba(0, 0, 0, 0.8)",
      titleColor: "#fff",
      bodyColor: "#fff",
      borderColor: "#fff",
      borderWidth: 1,
    },
  },
};

// Create bar chart
function createBarChart(canvasId, data, options = {}) {
  const ctx = document.getElementById(canvasId);
  if (!ctx) return null;

  const defaultOptions = {
    ...commonChartOptions,
    scales: {
      y: {
        beginAtZero: true,
        ticks: {
          callback: function (value) {
            return formatNumber(value);
          },
        },
      },
    },
  };

  return new Chart(ctx, {
    type: "bar",
    data: data,
    options: { ...defaultOptions, ...options },
  });
}

// Create pie chart
function createPieChart(canvasId, data, options = {}) {
  const ctx = document.getElementById(canvasId);
  if (!ctx) return null;

  const defaultOptions = {
    ...commonChartOptions,
    plugins: {
      ...commonChartOptions.plugins,
      tooltip: {
        ...commonChartOptions.plugins.tooltip,
        callbacks: {
          label: function (context) {
            const total = context.dataset.data.reduce((a, b) => a + b, 0);
            const percentage = ((context.parsed / total) * 100).toFixed(1);
            return (
              context.label +
              ": " +
              formatNumber(context.parsed) +
              " (" +
              percentage +
              "%)"
            );
          },
        },
      },
    },
  };

  return new Chart(ctx, {
    type: "pie",
    data: data,
    options: { ...defaultOptions, ...options },
  });
}

// Create line chart
function createLineChart(canvasId, data, options = {}) {
  const ctx = document.getElementById(canvasId);
  if (!ctx) return null;

  const defaultOptions = {
    ...commonChartOptions,
    scales: {
      y: {
        beginAtZero: true,
        ticks: {
          callback: function (value) {
            return formatNumber(value);
          },
        },
      },
    },
    elements: {
      line: {
        tension: 0.4,
      },
    },
  };

  return new Chart(ctx, {
    type: "line",
    data: data,
    options: { ...defaultOptions, ...options },
  });
}

// Loading spinner
function showLoading(element) {
  element.innerHTML = '<div class="loading-spinner"></div>';
}

function hideLoading(element, originalContent) {
  element.innerHTML = originalContent;
}

// AJAX helper functions
function ajaxGet(url, successCallback, errorCallback) {
  $.ajax({
    url: url,
    type: "GET",
    success: function (response) {
      if (successCallback) successCallback(response);
    },
    error: function (xhr, status, error) {
      if (errorCallback) errorCallback(xhr, status, error);
      else showToast("Có lỗi xảy ra khi tải dữ liệu", "error");
    },
  });
}

function ajaxPost(url, data, successCallback, errorCallback) {
  $.ajax({
    url: url,
    type: "POST",
    data: data,
    success: function (response) {
      if (successCallback) successCallback(response);
    },
    error: function (xhr, status, error) {
      if (errorCallback) errorCallback(xhr, status, error);
      else showToast("Có lỗi xảy ra khi gửi dữ liệu", "error");
    },
  });
}

// Date range picker for reports
function initDateRangePicker(elementId, callback) {
  $(elementId).daterangepicker(
    {
      locale: {
        format: "DD/MM/YYYY",
        separator: " - ",
        applyLabel: "Áp dụng",
        cancelLabel: "Hủy",
        fromLabel: "Từ",
        toLabel: "Đến",
        customRangeLabel: "Tùy chọn",
        weekLabel: "T",
        daysOfWeek: ["CN", "T2", "T3", "T4", "T5", "T6", "T7"],
        monthNames: [
          "Tháng 1",
          "Tháng 2",
          "Tháng 3",
          "Tháng 4",
          "Tháng 5",
          "Tháng 6",
          "Tháng 7",
          "Tháng 8",
          "Tháng 9",
          "Tháng 10",
          "Tháng 11",
          "Tháng 12",
        ],
        firstDay: 1,
      },
      ranges: {
        "Hôm nay": [moment(), moment()],
        "Hôm qua": [moment().subtract(1, "days"), moment().subtract(1, "days")],
        "7 ngày trước": [moment().subtract(6, "days"), moment()],
        "30 ngày trước": [moment().subtract(29, "days"), moment()],
        "Tháng này": [moment().startOf("month"), moment().endOf("month")],
        "Tháng trước": [
          moment().subtract(1, "month").startOf("month"),
          moment().subtract(1, "month").endOf("month"),
        ],
      },
    },
    callback
  );
}

// Auto-refresh functionality
function autoRefresh(interval = 30000) {
  // Default 30 seconds
  setInterval(() => {
    location.reload();
  }, interval);
}

// Initialize tooltips
function initTooltips() {
  $('[data-toggle="tooltip"]').tooltip();
}

// Initialize popovers
function initPopovers() {
  $('[data-toggle="popover"]').popover();
}

// Document ready function
$(document).ready(function () {
  // Initialize tooltips and popovers
  initTooltips();
  initPopovers();

  // Add loading states to buttons
  $(".btn-loading").on("click", function () {
    const btn = $(this);
    const originalText = btn.html();
    btn
      .prop("disabled", true)
      .html('<span class="loading-spinner"></span> Đang xử lý...');

    // Re-enable after 3 seconds if no response
    setTimeout(() => {
      btn.prop("disabled", false).html(originalText);
    }, 3000);
  });

  // Auto-hide alerts after 5 seconds
  $(".alert-auto-hide").delay(5000).fadeOut();

  // Smooth scrolling for anchor links
  $('a[href^="#"]').on("click", function (event) {
    const target = $(this.getAttribute("href"));
    if (target.length) {
      event.preventDefault();
      $("html, body")
        .stop()
        .animate(
          {
            scrollTop: target.offset().top - 100,
          },
          1000
        );
    }
  });
});

// Export functions for different report types
window.exportStockReport = function () {
  const data = [];
  $(".table tbody tr").each(function () {
    const row = $(this);
    data.push({
      "Loại Linh Kiện": row.find("td:nth-child(1) strong").text(),
      "Tổng Mặt Hàng": row.find("td:nth-child(2) .badge").text(),
      "Hết Hàng": row.find("td:nth-child(3) .badge").text(),
      "Tồn Kho Thấp": row.find("td:nth-child(4) .badge").text(),
      "Tồn Kho Bình Thường": row.find("td:nth-child(5) .badge").text(),
      "Tồn Kho Tốt": row.find("td:nth-child(6) .badge").text(),
      "Tổng Giá Trị": row.find("td:nth-child(7) strong").text(),
    });
  });

  exportToCSV(
    data,
    `bao-cao-ton-kho-${new Date().toISOString().split("T")[0]}.csv`
  );
};

window.exportProfitReport = function () {
  const data = [];
  $(".table tbody tr").each(function () {
    const row = $(this);
    data.push({
      "Linh Kiện": row.find("td:nth-child(1) strong").text(),
      Loại: row.find("td:nth-child(2) .badge").text(),
      "Giá Nhập": row.find("td:nth-child(3) strong").text(),
      "Giá Bán": row.find("td:nth-child(4) strong").text(),
      "Lợi Nhuận/Đơn Vị": row.find("td:nth-child(5) strong").text(),
      "Tỷ Lệ LN": row.find("td:nth-child(6) .badge").text(),
      "Tồn Kho": row.find("td:nth-child(7) .badge").text(),
      "Tổng Lợi Nhuận": row.find("td:nth-child(8) strong").text(),
      "Trạng Thái": row.find("td:nth-child(9) .badge").text(),
    });
  });

  exportToCSV(
    data,
    `bao-cao-loi-nhuan-${new Date().toISOString().split("T")[0]}.csv`
  );
};

window.printReport = function () {
  window.print();
};
