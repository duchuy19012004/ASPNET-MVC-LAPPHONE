// JS cho trang Create phiếu sửa
$(document).ready(function () {
  // 1. Modal thêm khách hàng mới
  $("#btnAddKhachHang").on("click", function () {
    $("#addKhachHangModal .modal-content").html(
      '<div class="text-center p-5"><div class="spinner-border"></div></div>'
    );
    $("#addKhachHangModal").modal("show");
    $.get("/KhachHang/CreateQuick", function (data) {
      $("#addKhachHangModal .modal-content").html(data);
    });
  });

  // 2. Submit form thêm khách hàng nhanh qua AJAX
  $(document).on("submit", "#addKhachHangModal form", function (e) {
    e.preventDefault();
    var $form = $(this);
    $.post($form.attr("action"), $form.serialize(), function (res) {
      if (res.success && res.khachHang) {
        $("#maKhachHangHidden").val(res.khachHang.maKhachHang);
        $("#khachHangInfo").html(
          `<span class='text-success'><i class='fas fa-user'></i> ${res.khachHang.hoTen} (${res.khachHang.soDienThoai})</span>`
        );
        $("#addKhachHangModal").modal("hide");
        $("#btnEditKhachHang, #btnViewKhachHang").show();
      } else {
        // Hiển thị lỗi nếu có
        $("#addKhachHangModal .modal-content").html(
          res.html || "Có lỗi xảy ra!"
        );
      }
    });
  });

  // 3. Modal sửa khách hàng nhanh
  $("#btnEditKhachHang").on("click", function () {
    var id = $("#maKhachHangHidden").val();
    if (!id) return;
    $("#editKhachHangModal .modal-content").html(
      '<div class="text-center p-5"><div class="spinner-border"></div></div>'
    );
    $("#editKhachHangModal").modal("show");
    $.get("/KhachHang/EditQuick", { id: id }, function (data) {
      $("#editKhachHangModal .modal-content").html(data);
    });
  });

  // 4. Submit form sửa khách hàng nhanh qua AJAX
  $(document).on("submit", "#editKhachHangModal form", function (e) {
    e.preventDefault();
    var $form = $(this);
    $.post($form.attr("action"), $form.serialize(), function (res) {
      if (res.success && res.khachHang) {
        $("#maKhachHangHidden").val(res.khachHang.maKhachHang);
        $("#khachHangInfo").html(
          `<span class='text-success'><i class='fas fa-user'></i> ${res.khachHang.hoTen} (${res.khachHang.soDienThoai})</span>`
        );
        $("#editKhachHangModal").modal("hide");
      } else {
        $("#editKhachHangModal .modal-content").html(
          res.html || "Có lỗi xảy ra!"
        );
      }
    });
  });

  // 5. Kiểm tra mã khách hàng trước khi submit phiếu sửa
  $(document).on("submit", 'form[asp-action="Create"]', function (e) {
    var maKh = $("#maKhachHangHidden").val();
    if (!maKh) {
      $("#khachHangError").text("Vui lòng chọn hoặc thêm khách hàng!");
      $("#searchKhachHangPhone").focus();
      e.preventDefault();
      return false;
    }
    $("#khachHangError").text("");
  });

  // 6. (Khung) Xử lý hiển thị linh kiện khi chọn dịch vụ
  // Tính tổng tiền động khi chọn dịch vụ hoặc tick linh kiện
  function updateTongTienPhieuSua() {
    // Tổng giá dịch vụ
    var tongGiaDichVu = 0;
    $(".dichvu-select").each(function () {
      var selected = $(this).val();
      if (selected) {
        var dv = window.dichVuList.find((x) => x.value == selected);
        if (dv) tongGiaDichVu += parseFloat(dv.gia);
      }
    });
    // Tổng giá linh kiện
    var tongGiaLinhKien = 0;
    $("#linhkien-list-by-dichvu input[type=checkbox]:checked").each(
      function () {
        var gia = $(this).data("gia");
        if (gia) tongGiaLinhKien += parseFloat(gia);
      }
    );
    // Hiển thị
    $("#giaDichVu").text(tongGiaDichVu.toLocaleString());
    $("#giaLinhKien").text(tongGiaLinhKien.toLocaleString());
    $("#tongTienPhieuSua").text(
      (tongGiaDichVu + tongGiaLinhKien).toLocaleString()
    );
  }

  // Khi chọn dịch vụ, load linh kiện và tính lại tổng tiền
  $(document).on("change", ".dichvu-select", function () {
    var selectedDichVu = $(this).val();
    if (!selectedDichVu) {
      $("#linhkien-list-by-dichvu").html("");
      updateTongTienPhieuSua();
      return;
    }
    $.get(
      "/LinhKien/GetByDichVu",
      { maDichVu: selectedDichVu },
      function (data) {
        if (data && data.length > 0) {
          var html = '<div class="form-group"><label>Chọn linh kiện:</label>';
          data.forEach(function (lk, idx) {
            var giaBan = lk.giaBan && lk.giaBan > 0 ? lk.giaBan : 0;
            html += `<div class="row align-items-center mb-2">
                        <div class="col-auto">
                          <input class="form-check-input" type="checkbox" name="LinhKiens[${idx}].MaLinhKien" value="${
              lk.maLinhKien
            }" id="lk_${lk.maLinhKien}" data-gia="${giaBan}" data-idx="${idx}">
                        </div>
                        <div class="col">
                          <label class="form-check-label" for="lk_${
                            lk.maLinhKien
                          }">${lk.tenLinhKien} <span class='text-primary'>(${
              giaBan > 0 ? giaBan.toLocaleString() + " VNĐ" : "Chưa có giá"
            })</span></label>
                        </div>
                        <div class="col-auto">
                          <div class="input-group input-group-sm" style="width:110px;display:none;" data-idx="${idx}">
                            <button type="button" class="btn btn-outline-secondary btn-minus" data-idx="${idx}">-</button>
                            <input type="number" class="form-control soLuongInput" min="1" value="1" data-idx="${idx}" name="LinhKiens[${idx}].SoLuong">
                            <button type="button" class="btn btn-outline-secondary btn-plus" data-idx="${idx}">+</button>
                          </div>
                        </div>
                      </div>`;
          });
          html += "</div>";
          $("#linhkien-list-by-dichvu").html(html);
        } else {
          $("#linhkien-list-by-dichvu").html(
            '<div class="text-muted">Không có linh kiện nào cho dịch vụ này.</div>'
          );
        }
        updateTongTienPhieuSua();
      }
    );
  });

  // Khi tick vào checkbox linh kiện thì show/hide input số lượng
  $(document).on(
    "change",
    "#linhkien-list-by-dichvu input[type=checkbox]",
    function () {
      var idx = $(this).data("idx");
      var group = $(
        "#linhkien-list-by-dichvu .input-group[data-idx='" + idx + "']"
      );
      if ($(this).is(":checked")) {
        group.show();
      } else {
        group.hide();
      }
      updateTongTienPhieuSua();
    }
  );

  // Nút tăng số lượng
  $(document).on("click", ".btn-plus", function () {
    var idx = $(this).data("idx");
    var input = $(
      "#linhkien-list-by-dichvu input.soLuongInput[data-idx='" + idx + "']"
    );
    var val = parseInt(input.val()) || 1;
    input.val(val + 1).trigger("input");
  });
  // Nút giảm số lượng
  $(document).on("click", ".btn-minus", function () {
    var idx = $(this).data("idx");
    var input = $(
      "#linhkien-list-by-dichvu input.soLuongInput[data-idx='" + idx + "']"
    );
    var val = parseInt(input.val()) || 1;
    if (val > 1) input.val(val - 1).trigger("input");
  });

  // Khi thay đổi số lượng thì tính lại tổng tiền
  $(document).on(
    "input",
    "#linhkien-list-by-dichvu input.soLuongInput",
    function () {
      updateTongTienPhieuSua();
    }
  );

  // Cập nhật tổng tiền linh kiện theo số lượng
  function updateTongTienPhieuSua() {
    // Tổng giá dịch vụ
    var tongGiaDichVu = 0;
    $(".dichvu-select").each(function () {
      var selected = $(this).val();
      if (selected) {
        var dv = window.dichVuList.find((x) => x.value == selected);
        if (dv) tongGiaDichVu += parseFloat(dv.gia);
      }
    });
    // Tổng giá linh kiện
    var tongGiaLinhKien = 0;
    $("#linhkien-list-by-dichvu input[type=checkbox]:checked").each(
      function () {
        var gia = $(this).data("gia");
        var idx = $(this).data("idx");
        var soLuong =
          parseInt(
            $(
              "#linhkien-list-by-dichvu input.soLuongInput[data-idx='" +
                idx +
                "']"
            ).val()
          ) || 1;
        if (gia) tongGiaLinhKien += parseFloat(gia) * soLuong;
      }
    );
    // Hiển thị
    $("#giaDichVu").text(tongGiaDichVu.toLocaleString());
    $("#giaLinhKien").text(tongGiaLinhKien.toLocaleString());
    $("#tongTienPhieuSua").text(
      (tongGiaDichVu + tongGiaLinhKien).toLocaleString()
    );
  }

  // Khi submit form, serialize danh sách linh kiện (chỉ các linh kiện được chọn và số lượng) vào input hidden
  $(document).on("submit", 'form[asp-action="Create"]', function (e) {
    // Xóa các input hidden cũ nếu có
    $("#linhkien-list-by-dichvu input[type=hidden]").remove();
    // Duyệt các checkbox được chọn
    $("#linhkien-list-by-dichvu input[type=checkbox]:checked").each(function (
      i
    ) {
      var idx = $(this).data("idx");
      var maLinhKien = $(this).val();
      var soLuong =
        parseInt(
          $(
            "#linhkien-list-by-dichvu input.soLuongInput[data-idx='" +
              idx +
              "']"
          ).val()
        ) || 1;
      // Tạo input hidden cho MaLinhKien và SoLuong
      var inputMa = `<input type='hidden' name='LinhKiens[${i}].MaLinhKien' value='${maLinhKien}' />`;
      var inputSoLuong = `<input type='hidden' name='LinhKiens[${i}].SoLuong' value='${soLuong}' />`;
      $(this)
        .closest("form")
        .append(inputMa + inputSoLuong);
    });
  });

  // Khi load trang, tính tổng tiền ban đầu
  $(function () {
    updateTongTienPhieuSua();
  });

  // 7. Hiển thị/ẩn nút Sửa/Xem khách hàng
  function updateEditViewButton() {
    if ($("#maKhachHangHidden").val()) {
      $("#btnEditKhachHang").show();
      $("#btnViewKhachHang").show();
    } else {
      $("#btnEditKhachHang").hide();
      $("#btnViewKhachHang").hide();
    }
  }
  $("#searchKhachHangPhone").on("input", updateEditViewButton);
  $(document).on("change", "#maKhachHangHidden", updateEditViewButton);
  updateEditViewButton();

  // --- Autocomplete tìm kiếm khách hàng và tự động điền thông tin ---
  $(function () {
    $("#searchKhachHangPhone").autocomplete({
      source: function (request, response) {
        $.getJSON("/KhachHang/Search", { term: request.term }, function (data) {
          response(
            $.map(data, function (item) {
              return {
                label: `${item.hoTen} (${item.soDienThoai})`,
                value: item.soDienThoai,
                id: item.maKhachHang,
                hoTen: item.hoTen,
                diaChi: item.diaChi,
              };
            })
          );
        });
      },
      select: function (event, ui) {
        $("#maKhachHangHidden").val(ui.item.id);
        $("#tenKhachHangInput").val(ui.item.hoTen);
        $("#diaChiKhachHangInput").val(ui.item.diaChi);
        return false;
      },
      change: function (event, ui) {
        if (!ui.item) {
          // Nếu nhập số điện thoại mới, clear các trường khác
          $("#maKhachHangHidden").val("");
          $("#tenKhachHangInput").val("");
          $("#diaChiKhachHangInput").val("");
        }
      },
    });
  });
});
