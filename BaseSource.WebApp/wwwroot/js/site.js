$(".asyncPartial").each(function (i, item) {
    var url = $(item).data("url");
    if (url && url.length > 0) {
        $(item).load(url, function () {
            //feather.replace();
        });
    }
});

$(function () {
    // toastr ======================================================//
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-bottom-left",
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
            $btnSubmit.append(`<i class="fas fa-sync-alt fa-fw fa-spin ms-2"></i>`);
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


$(function () {
    $('select.form-control.select-picker').selectpicker({
        liveSearch: true
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
                localStorage.setItem("Message", res.message || "Successed!");

                window.location.href = res.resultObj;
            } else if (res.validationErrors != null && res.validationErrors.length) {
                $.each(res.validationErrors, function (i, v) {
                    $form.find("span[data-valmsg-for='" + v.pos + "']").html(v.error);
                });
                toastr.error(res.message);
            } else if (res.message != null) {
                toastr.error(res.message);
            }
            $btnSubmit.removeAttr("disabled");
        }
    });
});

/// ------------------ pagination ajax
$(document).on('change', 'select[data-name="select-pagesize-ajax"]', function () {
    $(this).closest('.pagerAjaxPartial').load($(this).val());
});

$(document).on('click', 'a[data-name="pagelink-ajax"]', function (e) {
    e.preventDefault();

    $(this).closest('.pagerAjaxPartial').load($(this).attr('data-href'));
});

// form search ajax
$('form[data-toggle="form-search-ajax"]').submit(function (e) {
    e.preventDefault();

    var $form = $(this);
    var target = $form.attr('data-target');

    $('div[data-name="' + target + '"]').each(function (i, item) {
        //$(item).html('<i class="fas fa-sync-alt fa-fw fa-spin"></i>');
        $(item).load($(item).attr('data-url'), $form.serialize());
    });
});


$(document).on("click", 'button[data-toggle="btn-confirm"]', function (e) {
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
                        if (res.message) {
                            localStorage.setItem("Message", res.message);
                        } else {
                            localStorage.setItem("Message", "Successed!");
                        }

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

if ($('.details-modal').length) {
    document.getElementById('details-modal').addEventListener('hidden.bs.modal', event => {
        $('#details-modal .modal-dialog').removeClass().addClass('modal-dialog');
        $('#details-modal .modal-body').html('');
    });
}

$(document).on("click", 'a[data-toggle="show-details-modal"]', function (e) {
    e.preventDefault();

    $("#details-modal .modal-title").html($(this).attr('data-title'));
    $("#details-modal .modal-body").html($($(this).attr('data-content-id')).html());
    $("#details-modal").modal('show');
});

function formatDate(strDate) {
    var date = new Date(Date.parse(strDate));
    var dd = String(date.getDate()).padStart(2, '0');
    var mm = String(date.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = date.getFullYear();

    return dd + '/' + mm + '/' + yyyy;
}

function formatDateTime(strDate) {
    var date = new Date(Date.parse(strDate));
    var dd = String(date.getDate()).padStart(2, '0');
    var mm = String(date.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = date.getFullYear();

    var h = String(date.getHours()).padStart(2, '0');
    var min = String(date.getMinutes()).padStart(2, '0');
    var s = String(date.getSeconds()).padStart(2, '0');

    return dd + '/' + mm + '/' + yyyy + ' ' + h + ':' + min + ':' + s;
}

function formatTime(seconds) {
    var h = Math.floor(seconds / 3600),
        m = Math.floor(seconds / 60) % 60,
        s = seconds % 60;
    if (h < 10) h = "0" + h;
    if (m < 10) m = "0" + m;
    if (s < 10) s = "0" + s;
    return h + ":" + m + ":" + s;
}

function formatNumber(data) {
    data = Math.round(parseFloat(data));
    return data.toLocaleString('en');
}

// extension method:
$.fn.classChange = function (cb) {
    return $(this).each((_, el) => {
        new MutationObserver(mutations => {
            mutations.forEach(mutation => cb && cb(mutation.target, $(mutation.target).prop(mutation.attributeName)));
        }).observe(el, {
            attributes: true,
            attributeFilter: ['class'] // only listen for class attribute changes 
        });
    });
}

function isAuthenticated() {
    if ($('[data-toggle="common-userId"]').length) {
        return true;
    }
    return false;
}

$(document).on('click', '[data-toggle="input-copy"]', function (e) {
    e.preventDefault();
    let copyText = $(this).get(0);
    copyText.select();
    copyText.setSelectionRange(0, 99999)
    document.execCommand("copy");

    toastr.success('Copied');
});
