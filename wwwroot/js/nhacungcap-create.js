$(document).ready(function () {
  // Real-time preview and validation
  function updatePreview() {
    const name =
      $("#TenNhaCungCap").val() || "Tên nhà cung cấp sẽ hiển thị ở đây";
    const address = $("#DiaChi").val() || "Địa chỉ sẽ hiển thị ở đây";
    const phone = $("#SoDienThoai").val() || "Chưa có SĐT";
    const email = $("#Email").val() || "Chưa có email";
    const status = $("#trangThaiSwitch").is(":checked");

    // Update preview
    $("#previewName span").text(name);
    $("#previewAddress span").text(address);
    $("#previewPhone span").text(phone);
    $("#previewEmail span").text(email);

    // Update status badge
    if (status) {
      $("#previewStatus").removeClass("bg-secondary").addClass("bg-success");
      $("#previewStatus span").text("Đang hợp tác");
      $("#previewStatus i").removeClass("fa-pause").addClass("fa-handshake");
    } else {
      $("#previewStatus").removeClass("bg-success").addClass("bg-secondary");
      $("#previewStatus span").text("Ngừng hợp tác");
      $("#previewStatus i").removeClass("fa-handshake").addClass("fa-pause");
    }

    // Update contact info
    if (phone !== "Chưa có SĐT" && email !== "Chưa có email") {
      $("#previewContact").text("Thông tin liên hệ đầy đủ");
    } else {
      $("#previewContact").text("Thiếu thông tin liên hệ");
    }

    // Update validation
    updateValidation();
  }

  // Real-time validation
  function updateValidation() {
    const name = $("#TenNhaCungCap").val();
    const phone = $("#SoDienThoai").val();
    const email = $("#Email").val();
    const address = $("#DiaChi").val();
    const status = $("#trangThaiSwitch").is(":checked");

    // Validate name
    updateValidationItem(
      "nameCheck",
      name,
      name.length >= 3,
      "Hợp lệ",
      "Tối thiểu 3 ký tự"
    );

    // Validate phone
    const phonePattern = /^(0[3|5|7|8|9])+([0-9]{8})$/;
    updateValidationItem(
      "phoneCheck",
      phone,
      phonePattern.test(phone),
      "Định dạng đúng",
      "Định dạng không đúng"
    );

    // Validate email
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    updateValidationItem(
      "emailCheck",
      email,
      emailPattern.test(email),
      "Định dạng đúng",
      "Định dạng không đúng"
    );

    // Validate address
    updateValidationItem(
      "addressCheck",
      address,
      address.length >= 10,
      "Hợp lệ",
      "Tối thiểu 10 ký tự"
    );

    // Validate status
    $("#statusCheck").text(status ? "Đang hợp tác" : "Ngừng hợp tác");

    // Overall validation
    const allValid =
      name.length >= 3 &&
      phonePattern.test(phone) &&
      emailPattern.test(email) &&
      address.length >= 10;

    if (allValid) {
      $("#overallStatus")
        .removeClass("bg-secondary bg-warning")
        .addClass("bg-success");
      $("#overallStatus").html('<i class="fas fa-check me-1"></i>Sẵn sàng lưu');
      $("#submitBtn").prop("disabled", false);
    } else {
      $("#overallStatus")
        .removeClass("bg-success bg-secondary")
        .addClass("bg-warning");
      $("#overallStatus").html(
        '<i class="fas fa-exclamation-triangle me-1"></i>Cần kiểm tra lại'
      );
      $("#submitBtn").prop("disabled", true);
    }
  }

  function updateValidationItem(
    elementId,
    value,
    isValid,
    validText,
    invalidText
  ) {
    const element = $(`#${elementId}`);
    const parentLi = element.closest("li");
    const icon = parentLi.find("i");

    if (!value) {
      element.text("Chưa nhập");
      icon
        .removeClass("fa-check-circle fa-times-circle text-success text-danger")
        .addClass("fa-circle text-secondary");
      parentLi.removeClass("text-success text-danger").addClass("text-muted");
    } else if (isValid) {
      element.text(validText);
      icon
        .removeClass("fa-circle fa-times-circle text-secondary text-danger")
        .addClass("fa-check-circle text-success");
      parentLi.removeClass("text-muted text-danger").addClass("text-success");
    } else {
      element.text(invalidText);
      icon
        .removeClass("fa-circle fa-check-circle text-secondary text-success")
        .addClass("fa-times-circle text-danger");
      parentLi.removeClass("text-muted text-success").addClass("text-danger");
    }
  }

  // Update preview on input change
  $("#TenNhaCungCap, #DiaChi, #SoDienThoai, #Email, #trangThaiSwitch").on(
    "input change",
    updatePreview
  );

  // Switch label change
  $("#trangThaiSwitch").on("change", function () {
    const label = $(this).is(":checked")
      ? "Nhà cung cấp đang hợp tác"
      : "Nhà cung cấp ngừng hợp tác";
    $(".switch-label").text(label);
  });

  // Phone number formatting
  $("#SoDienThoai").on("input", function () {
    let value = $(this).val().replace(/\D/g, ""); // Remove non-digits
    if (value.length > 10) {
      value = value.substring(0, 10);
    }
    $(this).val(value);
    updatePreview();
  });

  // Email validation styling
  $("#Email").on("blur", function () {
    const email = $(this).val();
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (email && !emailPattern.test(email)) {
      $(this).addClass("is-invalid");
    } else {
      $(this).removeClass("is-invalid");
    }
  });

  // Auto-focus first field
  $("#TenNhaCungCap").focus();

  // Initial preview update
  updatePreview();

  // Form submission validation
  $("form").on("submit", function (e) {
    const name = $("#TenNhaCungCap").val();
    const phone = $("#SoDienThoai").val();
    const email = $("#Email").val();
    const address = $("#DiaChi").val();

    const phonePattern = /^(0[3|5|7|8|9])+([0-9]{8})$/;
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (!name || name.length < 3) {
      e.preventDefault();
      alert("Tên nhà cung cấp phải có ít nhất 3 ký tự!");
      $("#TenNhaCungCap").focus();
      return false;
    }

    if (!phonePattern.test(phone)) {
      e.preventDefault();
      alert("Số điện thoại không đúng định dạng Việt Nam!");
      $("#SoDienThoai").focus();
      return false;
    }

    if (!emailPattern.test(email)) {
      e.preventDefault();
      alert("Email không đúng định dạng!");
      $("#Email").focus();
      return false;
    }

    if (!address || address.length < 10) {
      e.preventDefault();
      alert("Địa chỉ phải có ít nhất 10 ký tự!");
      $("#DiaChi").focus();
      return false;
    }
  });
});
