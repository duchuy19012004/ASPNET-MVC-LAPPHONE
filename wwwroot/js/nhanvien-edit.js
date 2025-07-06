// NhanVien Edit JavaScript - Complete Rewrite
$(document).ready(function () {
  console.log("NhanVien Edit Script Loaded");

  // Cache DOM elements
  const $form = $("#employeeEditForm");
  const $hoTenInput = $('input[name="HoTen"]');
  const $ngaySinhInput = $('input[name="NgaySinh"]');
  const $emailInput = $('input[name="Email"]');
  const $soDienThoaiInput = $('input[name="SoDienThoai"]');
  const $diaChiInput = $('textarea[name="DiaChi"]');
  const $chucVuSelect = $("#chucVuSelect");
  const $chucVuCustom = $("#chucVuCustom");
  const $ngayVaoLamInput = $('input[name="NgayVaoLam"]');
  const $luongInput = $('input[name="Luong"]');
  const $trangThaiSwitch = $("#trangThaiSwitch");
  const $trangThaiLabel = $("#trangThaiLabel");
  const $submitBtn = $("#submitBtn");

  // Preview elements
  const $previewName = $("#previewName");
  const $previewAge = $("#previewAge");
  const $previewPhone = $("#previewPhone");
  const $previewEmail = $("#previewEmail");
  const $previewChucVu = $("#previewChucVu");
  const $previewSalary = $("#previewSalary");
  const $previewStatus = $("#previewStatus");
  const $ageDisplay = $("#ageDisplay");
  const $workExpDisplay = $("#workExpDisplay");
  const $salaryDisplay = $("#salaryDisplay");

  // Initialize form on page load
  initializeForm();

  // ===========================================
  // EVENT HANDLERS
  // ===========================================

  // Basic input events
  $hoTenInput.on("input", updatePreviewName);
  $ngaySinhInput.on("change", updateAge);
  $emailInput.on("input", updatePreviewEmail);
  $soDienThoaiInput.on("input", updatePreviewPhone);
  $ngayVaoLamInput.on("change", updateWorkExperience);
  $luongInput.on("input", updatePreviewSalary);

  // Special events
  $chucVuSelect.on("change", handleChucVuChange);
  $chucVuCustom.on("input", updateChucVuFromCustom);
  $trangThaiSwitch.on("change", updateTrangThaiLabel);

  // Form submission
  $form.on("submit", handleFormSubmit);

  // Auto-format events
  $soDienThoaiInput.on("input", autoFormatPhone);
  $luongInput.on("input", autoFormatSalary);

  // Validation events
  $hoTenInput.on("blur", validateName);
  $emailInput.on("blur", validateEmail);
  $soDienThoaiInput.on("blur", validatePhone);
  $ngaySinhInput.on("change", validateBirthDate);
  $ngayVaoLamInput.on("change", validateWorkDate);

  // ===========================================
  // INITIALIZATION FUNCTIONS
  // ===========================================

  function initializeForm() {
    console.log("Initializing form...");

    // Update all previews with current values
    updatePreviewName();
    updateAge();
    updatePreviewEmail();
    updatePreviewPhone();
    updatePreviewSalary();
    updateWorkExperience();
    updateTrangThaiLabel();

    // Handle custom chức vụ display
    const currentChucVu = $chucVuSelect.val();
    if (currentChucVu === "Khác") {
      $chucVuCustom.show();
    }

    console.log("Form initialized successfully");
  }

  // ===========================================
  // PREVIEW UPDATE FUNCTIONS
  // ===========================================

  function updatePreviewName() {
    const hoTen = $hoTenInput.val().trim();
    if (hoTen) {
      $previewName.text(hoTen);
      // Update avatar
      const firstLetter = hoTen.charAt(0).toUpperCase();
      $(".avatar-circle-large").text(firstLetter);
    }
  }

  function updateAge() {
    const ngaySinh = $ngaySinhInput.val();
    if (ngaySinh) {
      const birthDate = new Date(ngaySinh);
      const today = new Date();
      let age = today.getFullYear() - birthDate.getFullYear();
      const monthDiff = today.getMonth() - birthDate.getMonth();

      if (
        monthDiff < 0 ||
        (monthDiff === 0 && today.getDate() < birthDate.getDate())
      ) {
        age--;
      }

      if (age >= 0 && age <= 120) {
        $ageDisplay.text(`Tuổi: ${age} tuổi`);
        $previewAge.text(`${age} tuổi (${getAgeGroup(age)})`);
      } else {
        $ageDisplay.text("Ngày sinh không hợp lệ");
        $previewAge.text("--");
      }
    }
  }

  function updatePreviewEmail() {
    const email = $emailInput.val().trim();
    $previewEmail.text(email || "Chưa nhập");
  }

  function updatePreviewPhone() {
    const phone = $soDienThoaiInput.val().trim();
    $previewPhone.text(phone || "Chưa nhập");
  }

  function updatePreviewSalary() {
    const luong = parseFloat($luongInput.val()) || 0;
    const formattedSalary = formatCurrency(luong);

    // Update preview panel
    if ($previewSalary.length > 0) {
      $previewSalary.text(formattedSalary);
    }

    // Update salary display text under input
    updateSalaryDisplayText(luong);
  }

  function updateSalaryDisplayText(luong) {
    let displayText = "Lương cơ bản hàng tháng";

    if (luong > 0) {
      // Hiển thị số tiền formatted + mô tả
      const formattedAmount = new Intl.NumberFormat("vi-VN").format(luong);

      let levelText = "";
      if (luong >= 20000000) {
        levelText = " - Mức lương rất cao";
      } else if (luong >= 15000000) {
        levelText = " - Mức lương cao";
      } else if (luong >= 10000000) {
        levelText = " - Mức lương khá";
      } else if (luong >= 7000000) {
        levelText = " - Mức lương trung bình";
      } else if (luong >= 5000000) {
        levelText = " - Mức lương cơ bản";
      } else {
        levelText = " - Mức lương thấp";
      }

      displayText = `${formattedAmount} VNĐ${levelText}`;
    }

    if ($salaryDisplay.length > 0) {
      $salaryDisplay.text(displayText);
    }
  }

  function updateWorkExperience() {
    const ngayVaoLam = $ngayVaoLamInput.val();
    if (ngayVaoLam) {
      const startDate = new Date(ngayVaoLam);
      const today = new Date();
      const diffTime = Math.abs(today - startDate);
      const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

      const years = Math.floor(diffDays / 365);
      const months = Math.floor((diffDays % 365) / 30);

      let experience = "";
      if (years > 0) {
        experience += years + " năm ";
      }
      if (months > 0) {
        experience += months + " tháng";
      }
      if (experience === "") {
        experience = "Dưới 1 tháng";
      }

      $workExpDisplay.text(`Kinh nghiệm làm việc: ${experience.trim()}`);
    }
  }

  function updateTrangThaiLabel() {
    const isActive = $trangThaiSwitch.is(":checked");
    const labelText = isActive ? "Đang làm việc" : "Tạm nghỉ";
    const badgeClass = isActive ? "bg-success" : "bg-danger";

    $trangThaiLabel.text(labelText);

    if ($previewStatus.length > 0) {
      $previewStatus
        .removeClass("bg-success bg-danger")
        .addClass(badgeClass)
        .text(labelText);
    }
  }

  // ===========================================
  // SPECIAL HANDLERS
  // ===========================================

  function handleChucVuChange() {
    const selectedValue = $chucVuSelect.val();

    if (selectedValue === "Khác") {
      $chucVuCustom.show().focus();
      $previewChucVu.text("Chức vụ khác");
    } else {
      $chucVuCustom.hide().val("");
      $previewChucVu.text(selectedValue || "Chưa chọn chức vụ");
    }
  }

  function updateChucVuFromCustom() {
    const customValue = $chucVuCustom.val().trim();
    if (customValue) {
      $previewChucVu.text(customValue);
      $chucVuSelect.val("Khác");
    } else {
      $previewChucVu.text("Chức vụ khác");
    }
  }

  // ===========================================
  // AUTO FORMAT FUNCTIONS
  // ===========================================

  function autoFormatPhone() {
    let value = $(this).val().replace(/\D/g, ""); // Remove non-digits
    if (value.length > 11) {
      value = value.substring(0, 11);
    }
    $(this).val(value);
    updatePreviewPhone();
  }

  function autoFormatSalary() {
    let value = $(this).val().replace(/\D/g, ""); // Remove non-digits
    if (value) {
      // Just set the numeric value, no rounding
      $(this).val(value);
    }
    updatePreviewSalary();
  }

  // ===========================================
  // VALIDATION FUNCTIONS
  // ===========================================

  function validateName() {
    const value = $(this).val().trim();
    if (value.length < 2) {
      $(this).addClass("is-invalid").removeClass("is-valid");
    } else {
      $(this).removeClass("is-invalid").addClass("is-valid");
    }
  }

  function validateEmail() {
    const value = $(this).val().trim();
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (value && emailPattern.test(value)) {
      $(this).removeClass("is-invalid").addClass("is-valid");
    } else if (value) {
      $(this).addClass("is-invalid").removeClass("is-valid");
    } else {
      $(this).removeClass("is-invalid is-valid");
    }
  }

  function validatePhone() {
    const value = $(this).val().trim();
    const phonePattern = /^(0[3|5|7|8|9])+([0-9]{8})$/;

    if (value && phonePattern.test(value)) {
      $(this).removeClass("is-invalid").addClass("is-valid");
    } else if (value) {
      $(this).addClass("is-invalid").removeClass("is-valid");
    } else {
      $(this).removeClass("is-invalid is-valid");
    }
  }

  function validateBirthDate() {
    const selectedDate = new Date($(this).val());
    const today = new Date();

    if (selectedDate > today) {
      showAlert("Ngày sinh không được là ngày trong tương lai");
      $(this).val("");
      $ageDisplay.text("Tuổi sẽ được tính tự động");
      $previewAge.text("--");
    } else {
      updateAge();
    }
  }

  function validateWorkDate() {
    const workDate = new Date($(this).val());
    const birthDate = new Date($ngaySinhInput.val());

    if ($ngaySinhInput.val() && workDate < birthDate) {
      showAlert("Ngày vào làm không được trước ngày sinh");
      $(this).val("");
      $workExpDisplay.text("Kinh nghiệm sẽ được tính từ ngày này");
    } else {
      updateWorkExperience();
    }
  }

  // ===========================================
  // FORM SUBMISSION
  // ===========================================

  function handleFormSubmit(e) {
    console.log("Form submission started");

    // Handle custom chức vụ
    if ($chucVuSelect.val() === "Khác") {
      const customChucVu = $chucVuCustom.val().trim();
      if (customChucVu) {
        // Create hidden input with custom value
        $("<input>")
          .attr({
            type: "hidden",
            name: "ChucVu",
            value: customChucVu,
          })
          .appendTo($form);

        // Remove select name to avoid conflict
        $chucVuSelect.removeAttr("name");
      } else {
        e.preventDefault();
        showAlert("Vui lòng nhập chức vụ hoặc chọn từ danh sách có sẵn.");
        $chucVuCustom.focus();
        return false;
      }
    }

    // Show loading state
    $submitBtn
      .prop("disabled", true)
      .html('<i class="fas fa-spinner fa-spin me-1"></i> Đang cập nhật...');

    // Validate form
    if (!validateFormSubmission()) {
      e.preventDefault();
      $submitBtn
        .prop("disabled", false)
        .html('<i class="fas fa-save me-1"></i> Cập Nhật');
      return false;
    }

    console.log("Form validation passed, submitting...");
  }

  function validateFormSubmission() {
    let isValid = true;
    const errors = [];

    // Validate required fields
    if (!$hoTenInput.val().trim()) {
      errors.push("Họ và tên là bắt buộc");
      isValid = false;
    }

    if (!$emailInput.val().trim()) {
      errors.push("Email là bắt buộc");
      isValid = false;
    }

    if (!$soDienThoaiInput.val().trim()) {
      errors.push("Số điện thoại là bắt buộc");
      isValid = false;
    }

    if (!$diaChiInput.val().trim()) {
      errors.push("Địa chỉ là bắt buộc");
      isValid = false;
    }

    if (!$ngaySinhInput.val()) {
      errors.push("Ngày sinh là bắt buộc");
      isValid = false;
    }

    if (!$ngayVaoLamInput.val()) {
      errors.push("Ngày vào làm là bắt buộc");
      isValid = false;
    }

    const chucVu = $chucVuSelect.val();
    if (!chucVu || (chucVu === "Khác" && !$chucVuCustom.val().trim())) {
      errors.push("Chức vụ là bắt buộc");
      isValid = false;
    }

    const luong = parseFloat($luongInput.val());
    if (!luong || luong <= 0) {
      errors.push("Lương phải lớn hơn 0");
      isValid = false;
    }

    // Validate formats
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (
      $emailInput.val().trim() &&
      !emailPattern.test($emailInput.val().trim())
    ) {
      errors.push("Email không đúng định dạng");
      isValid = false;
    }

    const phonePattern = /^(0[3|5|7|8|9])+([0-9]{8})$/;
    if (
      $soDienThoaiInput.val().trim() &&
      !phonePattern.test($soDienThoaiInput.val().trim())
    ) {
      errors.push("Số điện thoại không đúng định dạng Việt Nam");
      isValid = false;
    }

    // Validate age
    if ($ngaySinhInput.val()) {
      const birthDate = new Date($ngaySinhInput.val());
      const today = new Date();
      const age = today.getFullYear() - birthDate.getFullYear();

      if (age < 16 || age > 70) {
        errors.push("Tuổi phải từ 16 đến 70");
        isValid = false;
      }
    }

    // Validate work start date
    if ($ngayVaoLamInput.val() && $ngaySinhInput.val()) {
      const startDate = new Date($ngayVaoLamInput.val());
      const birthDate = new Date($ngaySinhInput.val());
      const minWorkAge = new Date(birthDate);
      minWorkAge.setFullYear(birthDate.getFullYear() + 16);

      if (startDate < minWorkAge) {
        errors.push("Ngày vào làm phải sau khi đủ 16 tuổi");
        isValid = false;
      }
    }

    if (!isValid) {
      console.log("Validation errors:", errors);
      showAlert("Vui lòng kiểm tra lại thông tin:\n" + errors.join("\n"));
    }

    return isValid;
  }

  // ===========================================
  // UTILITY FUNCTIONS
  // ===========================================

  function getAgeGroup(age) {
    if (age < 25) return "Trẻ";
    if (age < 35) return "Trung bình";
    if (age < 50) return "Trưởng thành";
    return "Cao tuổi";
  }

  function formatCurrency(amount) {
    if (amount === 0) return "0 VNĐ";
    return new Intl.NumberFormat("vi-VN").format(amount) + " VNĐ";
  }

  function showAlert(message) {
    alert(message);
  }

  // ===========================================
  // DEBUG INFO
  // ===========================================

  console.log("All event handlers attached successfully");
  console.log("Form elements found:", {
    form: $form.length,
    inputs: $("input", $form).length,
    selects: $("select", $form).length,
    textareas: $("textarea", $form).length,
  });
});
