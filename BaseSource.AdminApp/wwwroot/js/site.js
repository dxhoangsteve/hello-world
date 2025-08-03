
$('form[data-name="ajaxForm"]').submit(function (e) {
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
            if (res.IsSuccessed == true) {
                localStorage.setItem("IsSuccessed", res.isSuccessed);
                localStorage.setItem("Message", "Successed!");

                window.location.href = res.ResultObj;
            } else if (res.validationErrors != null && res.validationErrors.length) {
                $.each(res.validationErrors, function (i, v) {
                    debugger
                    $form.find("span[data-valmsg-for='" + v.pos + "']").html(v.error);
                });
                $btnSubmit.removeAttr("disabled");
            } else if (res.Message != null) {
                alert(res.Message);
            }
        }
    });
});

$(document).ready(function () {
    //get it if Message key found
    if (localStorage.getItem("IsSuccessed") !== null && localStorage.getItem("Message") !== null) {
        if (localStorage.getItem("IsSuccessed") == "true") {
            //toastr.info(localStorage.getItem("Message"));
            alert(localStorage.getItem("Message"));
        } else {
            //toastr.error(localStorage.getItem("Message"));
            alert(localStorage.getItem("Message"));
        }
        localStorage.clear();
    }
});