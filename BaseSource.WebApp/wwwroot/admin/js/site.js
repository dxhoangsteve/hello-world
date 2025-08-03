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

function convertToUnSignString(s) {
    return s.normalize("NFD") // Chuẩn hóa thành dạng decomposed (NFD)
        .replace(/[\u0300-\u036f]/g, '') // Loại bỏ dấu thanh
        .replace(/đ/g, 'd') // Chuyển đổi "đ" -> "d"
        .replace(/Đ/g, 'D'); // Chuyển đổi "Đ" -> "D"
}

$(document).on('click', '[data-toggle="input-copy"]', function (e) {
    e.preventDefault();
    let copyText = $(this).get(0);
    copyText.select();
    copyText.setSelectionRange(0, 99999)
    document.execCommand("copy");

    toastr.success('Copied');
});
