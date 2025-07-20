// LinhKien JavaScript Functions
$(document).ready(function () {
  // Toggle Status AJAX
  $(".toggle-status").on("click", function () {
    const btn = $(this);
    const id = btn.data("id");

    btn.prop("disabled", true);

    $.post("/LinhKien/ToggleStatus", { id: id })
      .done(function (response) {
        if (response.success) {
          const newStatus = response.status;
          btn.data("status", newStatus.toString());

          if (newStatus) {
            btn.removeClass("btn-secondary").addClass("btn-success");
            btn.html('<i class="fas fa-check me-1"></i>Đang bán');
          } else {
            btn.removeClass("btn-success").addClass("btn-secondary");
            btn.html('<i class="fas fa-times me-1"></i>Ngừng bán');
          }

          showToast("Đã cập nhật trạng thái thành công!", "success");
        } else {
          showToast("Có lỗi xảy ra: " + response.message, "error");
        }
      })
      .fail(function () {
        showToast("Không thể kết nối đến server!", "error");
      })
      .always(function () {
        btn.prop("disabled", false);
      });
  });

  // Stock Update AJAX - Updated for modal support
  $(".stock-btn").on("click", function () {
    const btn = $(this);
    const id = btn.data("id");
    const action = btn.data("action");

    // Use modal instead of prompt
    if (typeof updateStockModal === "function") {
      updateStockModal(id);
      // Set the action and quantity based on button clicked
      setTimeout(() => {
        $("#stockAction").val(action);
        $("#stockQuantity").val(1);
        updateStockPreview();
      }, 100);
    } else {
      // Fallback to prompt for backward compatibility
      let actionText = "";
      switch (action) {
        case "add":
          actionText = "cộng";
          break;
        case "subtract":
          actionText = "trừ";
          break;
        case "set":
          actionText = "đặt";
          break;
      }

      const quantity = prompt(`Nhập số lượng muốn ${actionText}:`, "1");
      if (!quantity || isNaN(quantity) || parseInt(quantity) <= 0) return;

      btn.prop("disabled", true);

      $.post("/LinhKien/UpdateStock", {
        id: id,
        quantity: parseInt(quantity),
        action: action,
      })
        .done(function (response) {
          if (response.success) {
            // Update stock display
            const row = btn.closest("tr");
            const stockBadge = row.find(".badge");
            stockBadge.text(response.newStock);
            stockBadge.attr("class", response.stockClass);

            // Update row color based on stock
            row.removeClass("table-danger table-warning");
            if (response.newStock === 0) {
              row.addClass("table-danger");
            } else if (response.newStock <= 5) {
              row.addClass("table-warning");
            }

            showToast(
              `Đã cập nhật tồn kho thành công! Số lượng mới: ${response.newStock}`,
              "success"
            );
          } else {
            showToast("Có lỗi xảy ra: " + response.message, "error");
          }
        })
        .fail(function () {
          showToast("Không thể kết nối đến server!", "error");
        })
        .always(function () {
          btn.prop("disabled", false);
        });
    }
  });

  // Price calculation for Create/Edit forms
  function calculateProfit() {
    const giaNhap = parseFloat($("#GiaNhap").val()) || 0;
    const giaBan = parseFloat($("#GiaBan").val()) || 0;
    const soLuong = parseInt($("#SoLuongTon").val()) || 0;

    const loiNhuan = giaBan - giaNhap;
    const tyLeLoiNhuan = giaNhap > 0 ? (loiNhuan / giaNhap) * 100 : 0;
    const tongLoiNhuan = loiNhuan * soLuong;

    // Update profit display
    $("#profitAmount").text(formatCurrency(loiNhuan));
    $("#profitPercent").text(tyLeLoiNhuan.toFixed(1) + "%");
    $("#totalProfit").text(formatCurrency(tongLoiNhuan));

    // Update suggested prices
    const suggest20 = giaNhap * 1.2;
    const suggest30 = giaNhap * 1.3;
    const suggest50 = giaNhap * 1.5;

    $("#suggest20").text(formatCurrency(suggest20));
    $("#suggest30").text(formatCurrency(suggest30));
    $("#suggest50").text(formatCurrency(suggest50));

    // Update price validation
    if (giaBan < giaNhap) {
      $("#GiaBan").addClass("is-invalid");
      $("#priceValidation").text("Giá bán không được nhỏ hơn giá nhập").show();
    } else {
      $("#GiaBan").removeClass("is-invalid");
      $("#priceValidation").hide();
    }
  }

  // Format currency
  function formatCurrency(amount) {
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
    }).format(amount);
  }

  // Bind price calculation events
  $("#GiaNhap, #GiaBan, #SoLuongTon").on("input", calculateProfit);

  // Initialize calculation on page load
  calculateProfit();

  // Auto-complete for brand suggestions
  $("#HangSanXuat").on("input", function () {
    const value = $(this).val().toLowerCase();
    const suggestions = [
      "Kingston",
      "Samsung",
      "Intel",
      "AMD",
      "ASUS",
      "MSI",
      "Gigabyte",
      "Corsair",
      "G.Skill",
      "Western Digital",
      "Seagate",
      "NVIDIA",
      "Crucial",
      "HyperX",
      "Team Group",
    ];

    const filtered = suggestions.filter((brand) =>
      brand.toLowerCase().includes(value)
    );

    // Update datalist
    const datalist = $("#brandSuggestions");
    datalist.empty();
    filtered.forEach((brand) => {
      datalist.append(`<option value="${brand}">`);
    });
  });

  // Search AJAX for dropdown
  $("#searchLinhKien").on("input", function () {
    const term = $(this).val();
    const categoryId = $("#categoryFilter").val();

    if (term.length >= 2) {
      $.get("/LinhKien/SearchAjax", {
        term: term,
        categoryId: categoryId,
      }).done(function (data) {
        const results = $("#searchResults");
        results.empty();

        data.forEach(function (item) {
          results.append(`
                            <div class="search-item" data-id="${
                              item.id
                            }" data-price="${item.price}">
                                <strong>${item.text}</strong><br>
                                <small class="text-muted">
                                    ${
                                      item.category
                                    } | ${formatCurrency(item.price)} | Còn ${item.stock}
                                </small>
                            </div>
                        `);
        });

        if (data.length > 0) {
          results.show();
        } else {
          results.hide();
        }
      });
    } else {
      $("#searchResults").hide();
    }
  });

  // Handle search result selection
  $(document).on("click", ".search-item", function () {
    const id = $(this).data("id");
    const price = $(this).data("price");

    $("#selectedLinhKienId").val(id);
    $("#selectedLinhKienPrice").val(price);
    $("#searchLinhKien").val($(this).find("strong").text());
    $("#searchResults").hide();
  });

  // Hide search results when clicking outside
  $(document).on("click", function (e) {
    if (!$(e.target).closest(".search-container").length) {
      $("#searchResults").hide();
    }
  });
});

// Toast notification function
function showToast(message, type) {
  const toastId = "toast-" + Date.now();
  const toastClass = type === "success" ? "bg-success" : "bg-danger";

  const toast = `
        <div id="${toastId}" class="toast ${toastClass} text-white" role="alert" style="position: fixed; top: 20px; right: 20px; z-index: 9999;">
            <div class="toast-body">
                ${message}
            </div>
        </div>
    `;

  $("body").append(toast);
  $("#" + toastId)
    .toast({ delay: 3000 })
    .toast("show");

  setTimeout(() => $("#" + toastId).remove(), 4000);
}

// Export functions for global use
window.linhKienUtils = {
  showToast: showToast,
  formatCurrency: function (amount) {
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
    }).format(amount);
  },
};
