
        $(document).ready(function() {
            
            // Initialize date inputs with default values
            const today = new Date().toISOString().split('T')[0];
            if (!$('#NgayVaoLam').val()) {
                $('#NgayVaoLam').val(today);
            }
            
            // Custom job title handling
            $('#chucVuSelect').on('change', function() {
                const selectedValue = $(this).val();
                if (selectedValue === 'Khác') {
                    $('#chucVuCustom').show().focus();
                    // Don't clear the value immediately, let user type
                } else {
                    $('#chucVuCustom').hide();
                    // Value is already set by asp-for binding
                }
                updatePreview();
                updateValidation();
            });
            
            $('#chucVuCustom').on('input', function() {
                // When typing custom job title, update the main ChucVu field
                const customValue = $(this).val();
                $('#ChucVu').val(customValue);
                updatePreview();
                updateValidation();
            });
            
            // Age calculation
            $('#NgaySinh').on('change', function() {
                const birthDate = new Date($(this).val());
                const today = new Date();
                let age = today.getFullYear() - birthDate.getFullYear();
                const monthDiff = today.getMonth() - birthDate.getMonth();
                
                if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
                    age--;
                }
                
                if ($(this).val()) {
                    $('#ageDisplay').text(`Tuổi: ${age}`);
                    $('#previewAge').text(`${age} tuổi`);
                    
                    if (age < 18) {
                        $('#ageDisplay').addClass('text-danger').text(`Tuổi: ${age} (Phải từ 18 tuổi trở lên)`);
                    } else {
                        $('#ageDisplay').removeClass('text-danger');
                    }
                } else {
                    $('#ageDisplay').text('Tuổi sẽ được tính tự động');
                    $('#previewAge').text('--');
                }
                updateValidation();
            });
            
            // Work experience calculation
            $('#NgayVaoLam').on('change', function() {
                if ($(this).val()) {
                    const startDate = new Date($(this).val());
                    const today = new Date();
                    const years = today.getFullYear() - startDate.getFullYear();
                    
                    $('#workExpDisplay').text(`Kinh nghiệm: ${years} năm (tính từ ${$(this).val()})`);
                    $('#previewStartDate').text(new Date($(this).val()).toLocaleDateString('vi-VN'));
                } else {
                    $('#workExpDisplay').text('Kinh nghiệm sẽ được tính từ ngày này');
                    $('#previewStartDate').text('--');
                }
                updateValidation();
            });
            
            // Salary formatting
            $('#Luong').on('input', function() {
                const value = parseInt($(this).val()) || 0;
                $('#salaryDisplay').text(`${value.toLocaleString('vi-VN')} VNĐ/tháng`);
                $('#previewSalary').text(`${value.toLocaleString('vi-VN')} VNĐ`);
                updateValidation();
            });
            
            // Auto format phone number
            $('#SoDienThoai').on('input', function() {
                let value = $(this).val().replace(/\D/g, '');
                if (value.length > 11) {
                    value = value.substring(0, 11);
                }
                $(this).val(value);
                updatePreview();
            });
            
            // Real-time preview updates
            $('#HoTen').on('input', updatePreview);
            $('#SoDienThoai').on('input', updatePreview);
            $('#Email').on('input', updatePreview);
            $('#DiaChi').on('input', updatePreview);
            $('#TrangThai').on('change', updatePreview);
            
            // Real-time validation
            $('#HoTen, #NgaySinh, #SoDienThoai, #Email, #DiaChi, #ChucVu, #Luong').on('input change', updateValidation);
            
            function updatePreview() {
                const name = $('#HoTen').val() || 'Họ và tên sẽ hiển thị ở đây';
                const phone = $('#SoDienThoai').val() || 'Chưa nhập';
                const email = $('#Email').val() || 'Chưa nhập';
                const chucVu = $('#ChucVu').val() || 'Chưa chọn chức vụ';
                const trangThai = $('#TrangThai').val() === 'true';
                
                $('#previewName').text(name);
                $('#previewPhone').text(phone);
                $('#previewEmail').text(email);
                $('#previewChucVu').text(chucVu);
                
                // Update avatar
                if (name && name !== 'Họ và tên sẽ hiển thị ở đây') {
                    $('#previewAvatar').text(name.charAt(0).toUpperCase());
                } else {
                    $('#previewAvatar').text('?');
                }
                
                // Update status
                const statusBadge = $('#previewStatus');
                if (trangThai) {
                    statusBadge.removeClass('bg-secondary').addClass('bg-success').text('Đang làm việc');
                } else {
                    statusBadge.removeClass('bg-success').addClass('bg-secondary').text('Tạm nghỉ');
                }
            }
            
            function updateValidation() {
                const validations = {
                    name: { value: $('#HoTen').val().trim(), check: 'nameCheck', icon: 'nameIcon' },
                    age: { 
                        value: $('#NgaySinh').val(), 
                        check: 'ageCheck', 
                        icon: 'ageIcon',
                        validator: function(val) {
                            if (!val) return false;
                            const age = new Date().getFullYear() - new Date(val).getFullYear();
                            return age >= 18;
                        }
                    },
                    phone: { 
                        value: $('#SoDienThoai').val(), 
                        check: 'phoneCheck', 
                        icon: 'phoneIcon',
                        validator: function(val) {
                            return /^(0[3|5|7|8|9])+([0-9]{8})$/.test(val);
                        }
                    },
                    email: { 
                        value: $('#Email').val(), 
                        check: 'emailCheck', 
                        icon: 'emailIcon',
                        validator: function(val) {
                            return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(val);
                        }
                    },
                    address: { value: $('#DiaChi').val().trim(), check: 'addressCheck', icon: 'addressIcon' },
                    job: { value: $('#ChucVu').val(), check: 'jobCheck', icon: 'jobIcon' },
                    salary: { 
                        value: $('#Luong').val(), 
                        check: 'salaryCheck', 
                        icon: 'salaryIcon',
                        validator: function(val) {
                            return val && parseInt(val) > 0;
                        }
                    }
                };
                
                let allValid = true;
                
                Object.keys(validations).forEach(key => {
                    const item = validations[key];
                    const isValid = item.validator ? item.validator(item.value) : !!item.value;
                    
                    const checkElement = $(`#${item.check}`);
                    const iconElement = $(`#${item.icon}`);
                    
                    if (isValid) {
                        checkElement.removeClass('text-muted text-danger').addClass('text-success').text('Hợp lệ');
                        iconElement.removeClass('fa-circle text-secondary fa-times text-danger').addClass('fa-check text-success');
                    } else if (item.value) {
                        checkElement.removeClass('text-muted text-success').addClass('text-danger').text('Không hợp lệ');
                        iconElement.removeClass('fa-circle text-secondary fa-check text-success').addClass('fa-times text-danger');
                        allValid = false;
                    } else {
                        checkElement.removeClass('text-success text-danger').addClass('text-muted').text('Chưa nhập');
                        iconElement.removeClass('fa-check text-success fa-times text-danger').addClass('fa-circle text-secondary');
                        allValid = false;
                    }
                });
                
                // Update overall status
                const overallStatus = $('#overallStatus');
                if (allValid) {
                    overallStatus.removeClass('bg-secondary bg-warning').addClass('bg-success')
                               .html('<i class="fas fa-check me-1"></i>Sẵn sàng lưu');
                } else {
                    overallStatus.removeClass('bg-success bg-warning').addClass('bg-secondary')
                               .html('<i class="fas fa-clock me-1"></i>Đang nhập thông tin...');
                }
            }
            
            // Form submission validation
            $('#employeeForm').on('submit', function(e) {
                const name = $('#HoTen').val().trim();
                const phone = $('#SoDienThoai').val().trim();
                const email = $('#Email').val().trim();
                const address = $('#DiaChi').val().trim();
                const chucVu = $('#ChucVu').val().trim();
                const salary = $('#Luong').val();
                
                console.log('Form validation:', {
                    name: name,
                    phone: phone,
                    email: email,
                    address: address,
                    chucVu: chucVu,
                    salary: salary
                });
                
                if (!name || !phone || !email || !address || !chucVu || !salary) {
                    e.preventDefault();
                    toastr.error('Vui lòng điền đầy đủ thông tin bắt buộc!');
                    
                    // Highlight missing fields
                    if (!name) $('#HoTen').addClass('is-invalid');
                    if (!phone) $('#SoDienThoai').addClass('is-invalid');
                    if (!email) $('#Email').addClass('is-invalid');
                    if (!address) $('#DiaChi').addClass('is-invalid');
                    if (!chucVu) {
                        $('#ChucVu').addClass('is-invalid');
                        $('#chucVuSelect').addClass('is-invalid');
                    }
                    if (!salary) $('#Luong').addClass('is-invalid');
                    
                    return false;
                }
                
                // Validate phone format
                if (!/^(0[3|5|7|8|9])+([0-9]{8})$/.test(phone)) {
                    e.preventDefault();
                    toastr.error('Số điện thoại không đúng định dạng!');
                    $('#SoDienThoai').focus().addClass('is-invalid');
                    return false;
                }
                
                // Validate email format
                if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
                    e.preventDefault();
                    toastr.error('Email không đúng định dạng!');
                    $('#Email').focus().addClass('is-invalid');
                    return false;
                }
                
                // Show loading
                $('#submitBtn').prop('disabled', true)
                              .html('<i class="fas fa-spinner fa-spin me-1"></i>Đang lưu...');
            });
            
            // Remove invalid class when user starts typing
            $('#HoTen, #SoDienThoai, #Email, #DiaChi, #Luong').on('input', function() {
                $(this).removeClass('is-invalid');
            });
            
            $('#chucVuSelect').on('change', function() {
                $(this).removeClass('is-invalid');
                $('#ChucVu').removeClass('is-invalid');
            });
            
            // Initialize
            updatePreview();
            updateValidation();
        });
 