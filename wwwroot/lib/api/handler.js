var showConfirmModalComplete = true;
var errorHandler = function (data) {
    showConfirmModalComplete = false;
    hideLoading();
    if (data && data.done !== undefined && data.done === false) {
        if (data.errorCode === 2 || data.errorCode === 3 || data.errorCode === 4 || data.errorCode === 6 || data.errorCode === 7) {
            localStorage.removeItem("ticket");
            setTimeout(() => { window.location.href = Config.applicationUrl + "/login"; }, 2000);
        }
        showError(data.errorDesc);
    } else {
        showError("کاربر گرامی در حین انجام عملیات خطایی اتفاق افتاد لطفا مجددا تلاش نمایید");
    }

};
var createModalLoading = function (color) {
    var modal = $("<div/>",
        {
            "class": "modal fade",
            "id": "loadingModal",
            "tabindex": "-1",
            "role": "dialog",
            "style": "display: none"
        });

    var modalDialog = $("<div/>",
        {
            "role": "document",
            "class": "modal-dialog  modal-sm"
        });
    var modalContent = $("<div/>",
        {
            "class": "modal-content"
        });
    var modalBody = $("<div/>",
        {
            "class": "modal-body"
        });
    var text = $("<p/>",
        {
            "class": "text-loading col-" + color, "text": "لطفا شکیبا باشید"
        });
    var preLoad = $("<div/>",
        {
            "class": "preloader"
        });
    var spinner = $("<div/>", { "class": "spinner-layer pl-" + color });
    var circleClipper = $("<div/>", { "class": "circle-clipper left" });
    var circleLeft = $("<div/>", { "class": "circle" });
    var circleClipper2 = $("<div/>", { "class": "circle-clipper right" });
    var circleLeft2 = $("<div/>", { "class": "circle" });
    circleClipper2.append(circleLeft2);
    circleClipper.append(circleLeft);
    spinner.append(circleClipper);
    spinner.append(circleClipper2);
    preLoad.append(spinner);
    modalBody.append(preLoad,text);
    modalContent.append(modalBody);
    modalDialog.append(modalContent);
    modal.append(modalDialog);
    $("body").append(modal);
};
var showLoading = function () {
    if ($("body").find("#loadingModal").length === 0) {
        createModalLoading(Config.theme);
    }
    setTimeout(() => {
        $('#loadingModal').modal('show');
    }, 10);
};
var hideLoading = function () {
    setTimeout(() => {
        $('#loadingModal').modal('hide');
    }, 1000);
};
var createConfirmModal = function () {
    var modal = $("<div/>",
        {
            "class": "modal fade",
            "id": "confirmModal",
            "tabindex": "-1",
            "role": "dialog",
            "style": "display: none"
        });
    var modalDialog = $("<div/>",
        {
            "role": "document",
            "class": "modal-dialog  modal-sm"
        });

    var modalContent = $("<div/>",
        {
            "class": "modal-content modal-content"
        });

    var modalHeader = $("<div/>",
        {
            "class": "modal-header"
        });
    var contentModalHeader = $("<h4/>",
        {
            "class": "modal-title",
            "text": "پیغام سيستمی"
        });
    var modalFooter = $("<div/>",
        {
            "class": "modal-footer"
        });
    var contentModalFooter = $("<button/>",
        {
            "class": "btn btn-light material ",
            "text": "بستن",
            "type": "button",
            "data-dismiss": "modal"
        });
    var modalBody = $("<div/>",
        {
            "class": "modal-body"
        });
    modalFooter.append(contentModalFooter);
    modalHeader.append(contentModalHeader);
    modalContent.append(modalHeader);
    modalContent.append(modalBody);
    modalContent.append(modalFooter);
    modalDialog.append(modalContent);
    modal.append(modalDialog);
    $("body").append(modal);
};
var showConfirm = function (message) {
    debugger;
    if ($("body").find("#confirmModal").length === 0) {
        createConfirmModal();
    }
    setTimeout(() => {
        if (typeof message === "object") {
            $('#confirmModal .modal-content').removeAttr('class').addClass('modal-content modal-col-teal');
            $('#confirmModal .modal-content .modal-body').text(message.body);
        } else {
            $('#confirmModal .modal-content').removeAttr('class').addClass('modal-content modal-col-teal');
            $('#confirmModal .modal-content .modal-body').text(message);
        }
       
        $('#confirmModal').modal('show');
    }, 10);
};
var hideConfirm = function () {
    setTimeout(() => {
        $('#confirmModal').modal('hide');
    }, 1000);
};
var showError = function (message) {
    if ($("body").find("#confirmModal").length === 0) {
        createConfirmModal();
    }
    setTimeout(() => {
        $('#confirmModal .modal-content').removeAttr('class').addClass('modal-content modal-col-red');
        $('#confirmModal .modal-content .modal-body').text(message);
        $('#confirmModal').modal('show');
    }, 10);
};
var Handler = (function () {
    var beforeSend = function () {
        showConfirmModalComplete = true;
        showLoading();
    },
        success = function (data, textStatus, jqXhr) { },
        complete = function (jqXhr) {
            if (jqXhr && jqXhr.responseJSON && jqXhr.responseJSON.done && jqXhr.responseJSON.result.ticket) {
                localStorage.setItem("ticket", jqXhr.responseJSON.result.ticket);
            }
            hideLoading();
            if (showConfirmModalComplete) {
                setTimeout(() => {
                    showConfirm("عملیات با موفقیت انجام شد");
                }, 1000);
            }

        },
        error = function (jqXhr) { errorHandler(jqXhr.responseJSON); },
        configError = function (data) {
            errorHandler(data);
        };
    return {
        beforeSend: beforeSend,
        success: success,
        complete: complete,
        error: error,
        configError: configError
    };
});
