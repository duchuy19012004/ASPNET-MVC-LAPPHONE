// LinhKien Soft Delete Management
const LinhKienSoftDelete = {
  // Khôi phục linh kiện đã xóa
  restore: function (id, tenLinhKien) {
    if (confirm(`Bạn có chắc muốn khôi phục linh kiện "${tenLinhKien}"?`)) {
      $.post("/LinhKien/Restore/" + id, function (response) {
        if (response.success) {
          LinhKienSoftDelete.showAlert("success", response.message);
          setTimeout(() => {
            location.reload();
          }, 1500);
        } else {
          LinhKienSoftDelete.showAlert("error", response.message);
        }
      }).fail(function () {
        LinhKienSoftDelete.showAlert(
          "error",
          "Có lỗi xảy ra khi khôi phục linh kiện!"
        );
      });
    }
  },

  // Xóa vĩnh viễn linh kiện
  hardDelete: function (id, tenLinhKien) {
    const message =
      `Bạn có chắc muốn xóa vĩnh viễn linh kiện "${tenLinhKien}"?\n\n` +
      `CẢNH BÁO: Hành động này sẽ xóa hoàn toàn linh kiện khỏi hệ thống và có thể ảnh hưởng đến các báo cáo thống kê!`;

    if (confirm(message)) {
      $.post("/LinhKien/HardDelete/" + id, function (response) {
        if (response.success) {
          LinhKienSoftDelete.showAlert("success", response.message);
          setTimeout(() => {
            location.reload();
          }, 1500);
        } else {
          LinhKienSoftDelete.showAlert("error", response.message);
        }
      }).fail(function () {
        LinhKienSoftDelete.showAlert(
          "error",
          "Có lỗi xảy ra khi xóa vĩnh viễn linh kiện!"
        );
      });
    }
  },

  // Hiển thị thông báo
  showAlert: function (type, message) {
    const alertClass = type === "success" ? "alert-success" : "alert-danger";
    const icon =
      type === "success" ? "fa-check-circle" : "fa-exclamation-triangle";

    const alertHtml = `
            <div class="alert ${alertClass} alert-dismissible fade show" role="alert">
                <i class="fas ${icon} me-2"></i>
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `;

    // Thêm alert vào đầu card-body
    $(".card-body").prepend(alertHtml);

    // Tự động ẩn sau 5 giây
    setTimeout(() => {
      $(".alert").fadeOut();
    }, 5000);
  },

  // Xác nhận xóa với lý do
  confirmDelete: function (id) {
    const lyDoXoa = $("#lyDoXoa").val();
    let message = "Bạn có chắc chắn muốn xóa linh kiện này?";

    if (lyDoXoa.trim() !== "") {
      message += "\n\nLý do: " + lyDoXoa;
    }

    if (confirm(message)) {
      $.ajax({
        url: "/LinhKien/Delete",
        type: "POST",
        data: {
          id: id,
          lyDoXoa: lyDoXoa,
          __RequestVerificationToken: $(
            'input[name="__RequestVerificationToken"]'
          ).val(),
        },
        success: function (response) {
          if (response.success) {
            // Sử dụng toast notification nếu có
            if (typeof showToast === "function") {
              showToast(response.message, "success");
            } else {
              LinhKienSoftDelete.showAlert("success", response.message);
            }
            $("#deleteModal").modal("hide");
            setTimeout(() => location.reload(), 1000);
          } else {
            if (typeof showToast === "function") {
              showToast(response.message, "error");
            } else {
              LinhKienSoftDelete.showAlert("error", response.message);
            }
          }
        },
        error: function () {
          const errorMsg = "Có lỗi xảy ra khi xóa!";
          if (typeof showToast === "function") {
            showToast(errorMsg, "error");
          } else {
            LinhKienSoftDelete.showAlert("error", errorMsg);
          }
        },
      });
    }
  },

  // Hiển thị thông tin linh kiện đã xóa trong modal
  showDeletedInfo: function (linhKien) {
    const modalHtml = `
            <div class="modal fade" id="deletedInfoModal" tabindex="-1">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header bg-secondary text-white">
                            <h5 class="modal-title">
                                <i class="fas fa-info-circle me-2"></i>
                                Thông Tin Linh Kiện Đã Xóa
                            </h5>
                            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <div class="alert alert-info">
                                <i class="fas fa-info-circle me-2"></i>
                                <strong>Linh kiện này đã bị xóa vào ${
                                  linhKien.NgayXoaText
                                }</strong>
                            </div>
                            
                            <div class="row">
                                <div class="col-md-6">
                                    <h6>Thông tin cơ bản</h6>
                                    <p><strong>Tên:</strong> ${
                                      linhKien.TenLinhKien
                                    }</p>
                                    <p><strong>Mã:</strong> #${
                                      linhKien.MaLinhKien
                                    }</p>
                                    <p><strong>Loại:</strong> ${
                                      linhKien.LoaiLinhKien?.TenLoaiLinhKien ||
                                      "N/A"
                                    }</p>
                                    <p><strong>Hãng:</strong> ${
                                      linhKien.HangSanXuat || "N/A"
                                    }</p>
                                </div>
                                <div class="col-md-6">
                                    <h6>Thông tin xóa</h6>
                                    <p><strong>Ngày xóa:</strong> ${
                                      linhKien.NgayXoaText
                                    }</p>
                                    <p><strong>Lý do:</strong> ${
                                      linhKien.LyDoXoa || "Không có"
                                    }</p>
                                    <p><strong>Trạng thái:</strong> <span class="badge bg-secondary">Đã xóa</span></p>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
                            <button type="button" class="btn btn-success" onclick="LinhKienSoftDelete.restore(${
                              linhKien.MaLinhKien
                            }, '${linhKien.TenLinhKien}')">
                                <i class="fas fa-undo me-1"></i>Khôi Phục
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `;

    // Xóa modal cũ nếu có
    $("#deletedInfoModal").remove();

    // Thêm modal mới
    $("body").append(modalHtml);

    // Hiển thị modal
    $("#deletedInfoModal").modal("show");
  },
};

// Export cho sử dụng global
window.LinhKienSoftDelete = LinhKienSoftDelete;
