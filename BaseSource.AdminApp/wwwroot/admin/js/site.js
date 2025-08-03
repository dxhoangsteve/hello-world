$(".asyncPartial").each(function (i, item) {
    var url = $(item).data("url");
    if (url && url.length > 0) {
        $(item).load(url);
    }
});

$(function () {
    // toastr ======================================================//
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
    // end toastr ==================================================//

    //get it if Message key found
    if (localStorage.getItem("IsSuccessed") !== null && localStorage.getItem("Message") !== null) {
        if (localStorage.getItem("IsSuccessed") == "true") {
            toastr.info(localStorage.getItem("Message"));
        } else {
            toastr.error(localStorage.getItem("Message"));
        }
        localStorage.clear();
    }
});

$("#page-top").on("click", 'button[data-toggle="btn-confirm"]', function (e) {
    e.preventDefault();

    if (confirm($(this).attr("data-title"))) {
        var $btnSubmit = $(this);
        $btnSubmit.attr("disabled", "true");

        var isRedirect = "false";
        if (typeof $btnSubmit.attr("data-redirect") !== 'undefined') {
            isRedirect = $btnSubmit.attr("data-redirect");
        }
        var isReloadPartial = "false";
        if (typeof $btnSubmit.attr("data-reloadpartial") !== 'undefined') {
            isReloadPartial = $btnSubmit.attr("data-reloadpartial");
        }
        $.ajax({
            url: $btnSubmit.attr("data-href"),
            method: 'POST',
            beforeSend: function () {
                $btnSubmit.append(`<i class="fas fa-sync-alt fa-fw fa-spin ms-2"></i>`);
            },
            complete: function () {
                $btnSubmit.find("i").remove();
            },
            success: function (res) {

                if (res.isSuccessed == true) {
                    if (isReloadPartial == "true") {
                        var $partial = $btnSubmit.closest('.asyncPartial');
                        $partial.load($partial.find('input[name="UrlReloadPartial"]').val());

                        toastr.info("Successed!");
                    } else {
                        localStorage.setItem("IsSuccessed", res.isSuccessed);
                        localStorage.setItem("Message", "Successed!");

                        if (isRedirect == "true") {
                            window.location.href = res.resultObj;
                        } else {
                            window.location.reload();
                        }
                    }
                } else {
                    toastr.error(res.message);
                }

                $btnSubmit.removeAttr("disabled");
            }
        });
    }
});


$(document).on('submit', 'form[data-name="ajaxForm"]', function (e) {
    e.preventDefault();

    var $form = $(this);
    var $btnSubmit = $form.find("button[type='submit']");

    $btnSubmit.attr("disabled", "true");
    $.ajax({
        method: $form.attr("method"),
        url: $form.attr("action"),
        data: $form.serializeArray(),
        beforeSend: function () {
            $btnSubmit.append(`<i class="fas fa-sync-alt fa-fw fa-spin"></i>`);
        },
        complete: function () {
            $btnSubmit.find("i").remove();
        },
        success: function (res) {
            $form.find(".field-validation-valid").empty();
            if (res.isSuccessed == true) {
                localStorage.setItem("IsSuccessed", res.isSuccessed);
                if (res.message != null) {
                    localStorage.setItem("Message", res.message);
                } else {
                    localStorage.setItem("Message", "Successed!");
                }

                if (res.resultObj) {
                    window.location.href = res.resultObj;
                } else {
                    window.location.reload();
                }
            } else if (res.validationErrors != null && res.validationErrors.length) {
                $.each(res.validationErrors, function (i, v) {
                    $form.find("span[data-valmsg-for='" + v.pos + "']").html(v.error);
                });
            } else if (res.message != null) {
                toastr.error(res.message);
            }
            $btnSubmit.removeAttr("disabled");
        }
    });
});



$(document).on('submit', 'form[data-name="ajaxFormUpload"]', function (e) {
    e.preventDefault();

    var formData = new FormData(this);

    var $form = $(this);
    var $btnSubmit = $form.find("button[type='submit']");

    $btnSubmit.attr("disabled", "true");
    $.ajax({
        method: $form.attr("method"),
        url: $form.attr("action"),
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,
        beforeSend: function () {
            $btnSubmit.append(`<i class="fas fa-sync-alt fa-fw fa-spin"></i>`);
        },
        complete: function () {
            $btnSubmit.find("i").remove();
        },
        success: function (res) {
            $btnSubmit.attr("disabled", "true");
            $form.find(".field-validation-valid").empty();
            if (res.isSuccessed == true) {
                localStorage.setItem("IsSuccessed", res.isSuccessed);
                localStorage.setItem("Message", "Successed!");

                window.location.href = res.resultObj;
            } else if (res.validationErrors != null && res.validationErrors.length) {
                $.each(res.validationErrors, function (i, v) {
                    $form.find("span[data-valmsg-for='" + v.pos + "']").html(v.error);
                });
            } else if (res.message != null) {
                toastr.error(res.message);
            }
            $btnSubmit.removeAttr("disabled");
        }
    });
});

document.getElementById('details-modal').addEventListener('hidden.bs.modal', event => {
    $('#details-modal .modal-dialog').removeClass().addClass('modal-dialog');
    $('#details-modal .modal-body').html('');
});

$(document).on("click", 'a[data-toggle="show-details-modal"]', function (e) {
    e.preventDefault();

    $("#details-modal .modal-title").html($(this).attr('data-title'));
    $("#details-modal .modal-body").html($($(this).attr('data-content-id')).html());
    $("#details-modal").modal('show');
});
//////////////////

$("input[type='checkbox'][data-toggle='checkbox-ajax']").change(function () {
    var $btnSubmit = $(this);
    $.ajax({
        url: $btnSubmit.attr("data-href"),
        method: 'POST',
        success: function (res) {
            if (res.isSuccessed == true) {
                toastr.success("Successed!");
            } else {
                toastr.error(res.message);
            }
        }
    });
});

$(document).on("click", '[data-toggle="btn-load-details-modal"]', function (e) {
    e.preventDefault();

    $('#details-modal .modal-dialog').addClass($(this).attr('data-modalsize'));
    $("#details-modal .modal-title").html($(this).attr('data-title'));
    $("#details-modal .modal-body").html('<i class="fas fa-sync-alt fa-fw fa-spin"></i>');
    $("#details-modal").modal('show');

    $.ajax({
        method: "GET",
        url: $(this).attr("data-href"),
        success: function (res) {
            $("#details-modal .modal-body").html(res);

            initMagnific();
        }
    });
});
//////////////////

$(function () {
    $('select.form-control.select-picker').selectpicker({
        liveSearch: true
    });
});

// form export ajax
$(document).on('click', '[data-toggle="export-external-form"]', function (e) {
    e.preventDefault();

    var url = $(this).attr('data-url');
    var $form = $($(this).attr('data-form'));

    window.location.href = url + "?" + $form.serialize();
});

$(document).on('focus', '.input-format', function () {
    if (!$(this).hasClass('loaded')) { // Kiểm tra nếu chưa được khởi tạo
        $(this).on('input', function () {
            let value = $(this).val().replace(/\,/g, ''); // Xóa dấu chấm cũ
            if (!isNaN(value) && value !== '') {
                // Cập nhật giá trị định dạng trong input hiển thị
                $(this).val(Number(value).toLocaleString('en-US'));
                // Đồng bộ giá trị thực vào input ẩn tương ứng
                $(this)
                    .siblings(`input[name="${$(this).data('raw-name')}"]`)
                    .val(value);
            } else {
                // Nếu giá trị không hợp lệ, xóa giá trị input ẩn
                $(this).siblings(`input[name="${$(this).data('raw-name')}"]`).val('');
            }
        });
    }
});

// datetime picker
$(document).on('focus', '.datetime-picker', function () {
    if (!$(this).data('daterangepicker')) { // Kiểm tra nếu chưa được khởi tạo
        $(this).daterangepicker({
            locale: {
                format: 'YYYY-MM-DD HH:mm',
                applyLabel: "Chọn",
                cancelLabel: "Xóa"
            },
            singleDatePicker: true,
            timePicker: true,
            timePicker24Hour: true,
            autoUpdateInput: false
        });
        $(this).on('apply.daterangepicker', function (ev, picker) {
            $(this).val(picker.startDate.format('YYYY-MM-DD HH:mm'));
        });
        $(this).on('cancel.daterangepicker', function (ev, picker) {
            $(this).val('');
        });
    }
});

// ẩn hiện form search
$(function () {
    const searchFormContainer = $('#frmSearchHide');
    const toggleSwitch = $('#toggleSearchSwitch');

    // 1. Khôi phục trạng thái ẩn/hiện từ localStorage
    const isFormVisible = localStorage.getItem('formVisible') === 'true';
    if (isFormVisible) {
        searchFormContainer.show();
        toggleSwitch.prop('checked', true);
    } else {
        searchFormContainer.hide();
        toggleSwitch.prop('checked', false);
    }

    toggleSwitch.change(function () {
        const isChecked = $(this).is(':checked');

        // Ẩn/hiện form
        if (isChecked) {
            searchFormContainer.slideDown(300);
        } else {
            searchFormContainer.slideUp(300);
        }

        // Lưu trạng thái vào localStorage
        localStorage.setItem('formVisible', isChecked);
    });
});

$(function () {
    $('input').attr('autocomplete', 'off');
});

$(function () {
    if ($('#top-scroll').length) {
        var $topScroll = $('#top-scroll');
        var $bottomScroll = $('#table-fit');
        var $scrollContent = $('#scroll-content');

        function updateTopScrollWidth() {
            var tableWidth = $bottomScroll.find('table').outerWidth();
            var containerWidth = $bottomScroll.outerWidth();

            var width = tableWidth > containerWidth ? tableWidth : containerWidth;
            $scrollContent.css('width', (width + 100) + 'px');

            var containerOffset = $bottomScroll.offset();
            $topScroll.css({
                width: containerWidth + 'px',
                left: containerOffset.left + 'px'
            });

            toggleTopScrollVisibility();
        }

        function toggleTopScrollVisibility() {
            var rect = $bottomScroll[0].getBoundingClientRect();
            var isVisible = rect.top >= 0 && rect.bottom <= window.innerHeight;

            var bottomScrollWidth = $bottomScroll[0].scrollWidth;
            var bottomClientWidth = $bottomScroll[0].clientWidth;

            if (isVisible && bottomScrollWidth > bottomClientWidth) {
                $topScroll.hide(); // Ẩn nếu bảng hiển thị thanh cuộn ngang
            } else {
                $topScroll.show(); // Hiển thị nếu bảng không có thanh cuộn ngang
            }
        }

        // Gọi hàm cập nhật khi tải trang
        updateTopScrollWidth();

        // Gọi lại khi thay đổi kích thước trình duyệt
        $(window).on('resize', function () {
            updateTopScrollWidth();
        });

        // Lắng nghe sự kiện cuộn trang
        $(window).on('scroll', function () {
            toggleTopScrollVisibility();
        });

        $topScroll.on('scroll', function () {
            $bottomScroll.scrollLeft($topScroll.scrollLeft());
        });

        $bottomScroll.on('scroll', function () {
            $topScroll.scrollLeft($bottomScroll.scrollLeft());
        });
    }
});

function convertToUnSignString(s) {
    return s.normalize("NFD") // Chuẩn hóa thành dạng decomposed (NFD)
        .replace(/[\u0300-\u036f]/g, '') // Loại bỏ dấu thanh
        .replace(/đ/g, 'd') // Chuyển đổi "đ" -> "d"
        .replace(/Đ/g, 'D'); // Chuyển đổi "Đ" -> "D"
}

$(document).on("change", '#LanguageFormSelect', function (e) {
    $('#form-vi').toggle();
    $('#form-en').toggle();
});

// dropzone
Dropzone.autoDiscover = false;
document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".dropzone").forEach(function (dropzoneElement) {
        var dropzoneId = dropzoneElement.id;
        var maxFiles = parseInt(dropzoneElement.getAttribute("data-max-files") || 1);

        new Dropzone("#" + dropzoneId, {
            url: "/admin/editor/UploadImage", // API upload file
            maxFiles: maxFiles,
            acceptedFiles: "image/*",
            addRemoveLinks: true,
            dictRemoveFile: "Xóa",
            init: function () {
                var dz = this;

                $("input[name='" + $(this.element).attr('data-id') + "']").each(function () {
                    var url = $(this).val();
                    var mockFile = { name: url, size: 1, uploadedUrl: url }; // Tạo mock file
                    dz.displayExistingFile(mockFile, url, null, null, false);
                    dz.files.push(mockFile);
                });

                this.on("addedfile", function (file) {
                    debugger
                    if (this.files.length > maxFiles || $("input[name='" + $(this.element).attr('data-id') + "']").length >= maxFiles) {
                        this.removeFile(file); // Xóa file mới ngay lập tức
                    }
                });

                this.on("sending", function (file, xhr, formData) {
                    var idtemp = $(this.element).closest('form').find('[name="EditorUploadId"]').val();
                    formData.append("id", idtemp);
                });
                this.on("removedfile", function (file) {
                    // debugger
                    var targetId = $(this.element).attr('data-id');
                    $(this.element).closest('form').find('input[name="' + targetId + '"][value="' + file.uploadedUrl + '"]').remove();
                });
            },
            success: function (file, response) {
                // debugger
                file.uploadedUrl = response.location;

                var targetId = $(this.element).attr('data-id');
                $(this.element).closest('form').append('<input type="hidden" name="' + targetId + '" value="' + response.location + '">');
            }
        });
    });
});

function convertToUnSignString(s) {
    // Normalize to FormD (decompose accents)
    let temp = s.normalize('NFD');

    // Remove diacritical marks using regex
    temp = temp.replace(/[\u0300-\u036f]/g, '');

    // Replace specific Vietnamese characters
    temp = temp.replace(/\u0111/g, 'd').replace(/\u0110/g, 'D');

    return temp;
}

function initMagnific() {
    $('.popup-gallery:not(.inited)').each(function () {
        $(this).magnificPopup({
            delegate: 'a.gallery-item',
            type: 'image',
            gallery: {
                enabled: true
            }
        }).addClass('inited');
    });
}

$(function () {
    initMagnific();
});

///////////
$(document).on("change", 'select[name="ProvinceId"]', function (e) {
    if ($('select[name="WardId"]').length) {
        var selected = $(this).find(":selected").val();
        $.ajax({
            method: "GET",
            url: '/admin/location/GetAllWard',
            data: {
                provinceId: selected
            },
            success: function (res) {
                $('select[name="WardId"]').html('<option value="">--- Select ---</option>');
                for (const item of res) {
                    $('select[name="WardId"]').append(`<option value="${item.id}" data-tokens="${convertToUnSignString(item.name)}">${item.name}</option>`);
                }
                $('select[name="WardId"]').selectpicker('destroy').selectpicker({
                    liveSearch: true
                });
            }
        });
    }
});