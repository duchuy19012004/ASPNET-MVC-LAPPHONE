$(document).ready(function () {
  // Auto format phone number
  $("#SoDienThoai").on("input", function () {
    let value = $(this).val().replace(/\D/g, "");
    if (value.length > 11) {
      value = value.substring(0, 11);
    }
    $(this).val(value);
  });

  // Validate email format (chỉ khi chưa có lỗi server-side)
  $("#Email").on("blur", function () {
    const email = $(this).val();
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    // Chỉ validate client-side nếu không có lỗi server-side
    if (!$(this).siblings(".text-danger").text().trim()) {
      if (email && !emailRegex.test(email)) {
        $(this).addClass("is-invalid");
        if (!$(this).next(".invalid-feedback").length) {
          $(this).after(
            '<div class="invalid-feedback">Email không đúng định dạng</div>'
          );
        }
      } else {
        $(this).removeClass("is-invalid");
        $(this).next(".invalid-feedback").remove();
      }
    }
  });

  // Validate phone number format (chỉ khi chưa có lỗi server-side)
  $("#SoDienThoai").on("blur", function () {
    const phone = $(this).val();
    const phoneRegex = /^(0[3|5|7|8|9])+([0-9]{8})$/;

    // Chỉ validate client-side nếu không có lỗi server-side
    if (!$(this).siblings(".text-danger").text().trim()) {
      if (phone && !phoneRegex.test(phone)) {
        $(this).addClass("is-invalid");
        if (!$(this).next(".invalid-feedback").length) {
          $(this).after(
            '<div class="invalid-feedback">Số điện thoại không đúng định dạng</div>'
          );
        }
      } else {
        $(this).removeClass("is-invalid");
        $(this).next(".invalid-feedback").remove();
      }
    }
  });

  // Confirm before submit
  $("form").on("submit", function (e) {
    if (
      !confirm("Bạn có chắc chắn muốn cập nhật thông tin nhà cung cấp này?")
    ) {
      e.preventDefault();
    }
  });
});
