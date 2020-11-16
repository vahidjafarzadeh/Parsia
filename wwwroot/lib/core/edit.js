/**
 * Created by Farshad Kazemi on 3/4/2018.
 */

"use strict";
// variable(s)
var localVariables,
    URLs,
    localShowRow,
    componentID,
    hasReferrer,
    pageLoadingModal,
    pageConfirmModal,
    showRowHandler,
    showRowFilter,
    saveHandler,
    saveOrUpdateCallback,
    baseTabButton,
    entityTabButton,
    baseForm,
    entityForm,
    objectsListAddBtn,
    objectsConfig,
    returnButton,
    newButton,
    refreshButton,
    submitButton,
    clearButton,
    dataContainer,
    currentData,
    gridRefreshFlag = false,
    ignoreToClear,
    pageLoadingModal,
    datePickerList = {};


var initViews = function() {
    pageLoadingModal = $("#loading-modal");
    pageConfirmModal = $("#confirm-modal");
    baseTabButton = $("#base-tab");
    entityTabButton = $("#entity-tab");
    baseForm = $(".base-form");
    entityForm = $(".entity-form");
    dataContainer = $("#data-container");
    returnButton = $("#edit-return");
    newButton = $("#edit-new");
    refreshButton = $("#edit-refresh");
    submitButton = $("#edit-submit");
    pageLoadingModal = $('#loading-modal');
    clearButton = $("#edit-clear");
    objectsListAddBtn = $(".objects-list-wrapper").find(".add");
};

let initInstances = function() {
    componentID = Util.urlParameter("id");
    if (localVariables) {
        const {
            URLs: localURLs,
            ignoreToClear: localIgnoreToClear,
            saveHandler: localSaveHandler,
            objectsConfig: localObjectsConfig,
            showRow: lShowRow,
            saveOrUpdateCallback: localSaveOrUpdateCallback
        } = localVariables;
        URLs = localURLs || {};
        ignoreToClear = localIgnoreToClear || [];
        if (localSaveHandler) {
            saveHandler = localSaveHandler;
        } else {
            saveHandler = new Handler();
            saveHandler.beforeSend = () => {
                if (top.showLoading) {
                    top.showLoading();
                }
                changeButtonState({ button: submitButton, state: 1 });
            };
            saveHandler.success = function(resp) {
                if (top.hideLoading) {
                    top.hideLoading();
                } else if (hidePageLoading) {
                    hidePageLoading();
                }
                if (resp.done) {
                    if (localSaveOrUpdateCallback) {
                        const returnValue = localSaveOrUpdateCallback.apply(null, [resp.result]);
                        if (returnValue) {
                            // if return true at the end of saveOrUpdateCallback, rest of code doesn't execute
                            return;
                        }
                    }
                    try {
                        const entityIDElement = $('[ksun-bundle-key="' + "entityId" + '"]');
                        entityIDElement.addClass("floating-mode");
                        if (typeof resp.result === "object") {
                            $(entityIDElement).closest(`.${ENVIRONMENT.CssClass.FORM_GROUP}`).data().entityId =
                                resp.result.entityId;
                            entityIDElement.val(resp.result.entityId).trigger("click");
                        } else {
                            $(entityIDElement).closest(`.${ENVIRONMENT.CssClass.FORM_GROUP}`).data().entityId =
                                resp.result;
                            entityIDElement.val(resp.result).trigger("click");
                        }
                        if (top.showConfirm) {
                            setTimeout(function() {
                                    top.showConfirm({
                                        title: "پیام سیستمی",
                                        body: "ذخیره سازی / بروزرسانی با موفقیت انجام شد",
                                        confirmButton: {
                                            text: GeneralBundle.$close
                                        },
                                        declineButton: {
                                            hidden: true
                                        }
                                    });
                                },
                                450);
                        }
                    } catch (e) {
                        console.log("entityId not found");
                    }


                } else {
                    errorHandler(resp);
                }
            };
            saveHandler.complete = (resp) => {
                if (resp && resp.responseJSON) {
                    if (resp.responseJSON.done) {
                        changeButtonState({ button: submitButton, state: 0 });
                    } else {
                        changeButtonState({ button: submitButton, state: 2 });
                    }
                } else {
                    changeButtonState({ button: submitButton, state: 0 });
                }
            };
        }
        objectsConfig = localObjectsConfig || {};
        localShowRow = lShowRow || undefined;
        saveOrUpdateCallback = localSaveOrUpdateCallback || undefined;
    }

    hasReferrer = Util.urlParameter("ref");
    if (!hasReferrer) {
        refreshButton.remove();
        returnButton.remove();
    }

    showRowHandler = new Handler();
    showRowHandler.success = function(resp) {
        if (top.hideLoading) {
            top.hideLoading();
        } else if (hidePageLoading) {
            hidePageLoading();
        }
        if (resp.done) {
            fillForm(resp.result);
            if (localVariables.showRow && localVariables.showRow.onSuccess) {
                localVariables.showRow.onSuccess.apply(null, [resp.result]);
            }
        } else {
            if (resp.errorCode === ENVIRONMENT.ErrorCode.BUSINESS_MESSAGE) {
                if (top.showConfirm) {
                    setTimeout(function() {
                            top.showConfirm({
                                title: "پیام سیستمی",
                                body: resp.errorDesc,
                                confirmButton: {
                                    text: GeneralBundle.$close
                                },
                                declineButton: {
                                    hidden: true
                                }
                            });
                        },
                        450);
                }
            }
        }
    };
    showRowFilter = new Filter();
    $("[ksun-default-value]").each((index, item) => {
        if ($(item).get(0).tagName === "TEXTAREA") {
            $(item).val($(item).attr("ksun-default-value"));
        }
    });

    $(".date").each(function() {
        var ctx = $(this);
        if (!ctx.attr("readonly")) {
            var key = ctx.attr("ksun-bundle-key");
            key = key.indexOf("$") === 0 ? key.substr(1, key.length - 1) : key;
            var obj = {};
            const dateConfig = {
                timePicker: {
                    enabled: !!ctx.attr("ksun-has-timepicker")
                },
                initialValue: ctx.attr("ksun-initial-value") && ctx.attr("ksun-initial-value") === "true",
                format: ctx.attr("ksun-date-format") ? ctx.attr("ksun-date-format") : "dddd D MMMM YYYY ساعت: HH:mm:ss",
                onSelect: function(unix) {
                    obj[key] = unix;
                    ctx.data("object", obj);
                    ctx.closest(`.${ENVIRONMENT.CssClass.FORM_GROUP}`).removeClass(ENVIRONMENT.CssClass.HAS_ERROR);
                }
            };
            if (!dateConfig.initialValue) {
                dateConfig.initialValue = false;
            }
            datePickerList[key] = ctx.persianDatepicker(dateConfig);
            if (ctx.attr("ksun-default-value")) {
                const currentDate = Number(ctx.attr("ksun-default-value"));
                datePickerList[key].pDatePicker.setDate(currentDate);
            } else if (ctx.attr("ksun-default-time")) {
                const currentDateTime = new persianDate();
                const arr = ctx.attr("ksun-default-time").split(":");
                const hour = arr[0];
                const min = arr.length > 1 ? arr[1] : 0;
                const sec = arr.length > 2 ? arr[2] : 0;
                currentDateTime.hour(Number(hour));
                currentDateTime.minute(Number(min));
                currentDateTime.second(Number(sec));
                datePickerList[key].pDatePicker.setDate(currentDateTime.gDate.getTime());
            }
        }
    });

    $(".clob").each(function() {
        try {
            var bundleKey = $(this).attr("ksun-bundle-key");
            const defaultValue = $(this).attr("ksun-default-value");
            bundleKey = bundleKey.indexOf("$") === 0 ? bundleKey.substr(1, bundleKey.length - 1) : bundleKey;
            if (localVariables.Config && localVariables.Config.CKEditor && localVariables.Config.CKEditor[bundleKey]) {
                CKEDITOR.editorConfig = function(config) {
                    config.language = localVariables.Config.CKEditor[bundleKey].language;
                    config.uiColor = localVariables.Config.CKEditor[bundleKey].uiColor;
                    config.height = localVariables.Config.CKEditor[bundleKey].height;
                };
                CKEDITOR.replace(this);
            } else {
                CKEDITOR.replace(this);
            }
            if (defaultValue) {
                CKEDITOR.instances[$(this).attr("id")].setData(defaultValue);
            }
        } catch (e) {
            console.warn("CKEditor is undefined", e);
        }
    });

    try {
        FloatingForm.init({ counter: true });
    } catch (e) {
        console.error("*** Add floating-form.js script ***");
    }
};

var initLocalVariables = function(obj) {
    localVariables = obj;
};

var initListeners = function() {
    baseTabButton.on("click",
        function() {
            entityForm.removeClass("visible");
            entityTabButton.removeClass("active");
            baseForm.addClass("visible");
            baseTabButton.addClass("active");
        });
    entityTabButton.on("click",
        function() {
            baseForm.removeClass("visible");
            baseTabButton.removeClass("active");
            entityForm.addClass("visible");
            entityTabButton.addClass("active");
        });
    returnButton.on("click",
        function() {
            try {
                parent.editFrameWrapper.removeClass("visible");
                if (gridRefreshFlag) {
                    parent.retrieveData();
                }
            } catch (e) {

            }
        });
    newButton.on("click",
        function() {
            showRow();
        });
    refreshButton.on("click",
        function() {
            showRow(currentData);
        });
    submitButton.on("click",
        function() {
            saveOrUpdate();
        });
    clearButton.on("click",
        function() {
            clearForm();
        });
    if (localVariables && localVariables.lockAble) {
        const lockBtn = $("<span/>",
            {
                class: "locker",
                html: `<i class="fas fa-lock"></i>`,
                click: () => {
                    if ($(`[ksun-bundle-key="entityId"]`).val()) {
                        var formData = new FormData();
                        formData.append("tblName", localVariables.clazzName);
                        formData.append("tblEntityId", $(`[ksun-bundle-key="entityId"]`).val());
                        if (lockBtn.find("svg").hasClass("text-gold")) {
                            const handler = new Handler();
                            handler.success = (data) => {
                                if (data.done) {
                                    if (top.hideLoading) {
                                        top.hideLoading();
                                    } else if (hidePageLoading) {
                                        hidePageLoading();
                                    }
                                    if (data.result) {
                                        $(".locker").find("svg").removeClass("text-gold");
                                        if (top.showConfirm) {
                                            setTimeout(function() {
                                                    top.showConfirm({
                                                        title: "پیام سیستمی",
                                                        body: "موجودیت با موفقیت از حالت قفل خارج گردید",
                                                        confirmButton: {
                                                            hidden: true
                                                        },
                                                        declineButton: {
                                                            text: GeneralBundle.$close
                                                        }
                                                    });
                                                },
                                                450);
                                        }
                                    }
                                } else {
                                    if (top.hideLoading) {
                                        top.hideLoading();
                                    }
                                    errorHandler(data);
                                }
                            };
                            var requestConfig = { url: "entityState/deleteLock", formData: formData, handler: handler };
                            Api.postForm(requestConfig);
                        } else if (lockBtn.find("svg").hasClass("text-danger")) {
                            $(".locker").find("svg").addClass("text-danger");
                            if (top.showConfirm) {
                                setTimeout(function() {
                                        const data = $(".locker").data().text;
                                        top.showConfirm({
                                            title: "پیام سیستمی",
                                            body: `رکورد جاری توسط ${data.name} ${data.family}  از سازمان ${data.orgName
                                                }   با نقش ${data.roleName}  تا پایان عملیات جاری قفل می باشد`,
                                            confirmButton: {
                                                hidden: true
                                            },
                                            declineButton: {
                                                text: GeneralBundle.$close
                                            }
                                        });
                                    },
                                    450);
                            }
                        } else {
                            const handler = new Handler();
                            handler.success = (data) => {
                                if (data.done) {
                                    if (top.hideLoading) {
                                        top.hideLoading();
                                    } else if (hidePageLoading) {
                                        hidePageLoading();
                                    }
                                    if (data.result) {
                                        $(".locker").find("svg").addClass("text-gold");
                                        if (top.showConfirm) {
                                            setTimeout(function() {
                                                    top.showConfirm({
                                                        title: "پیام سیستمی",
                                                        body: "موجودیت با موفقیت قفل گردید",
                                                        confirmButton: {
                                                            hidden: true
                                                        },
                                                        declineButton: {
                                                            text: GeneralBundle.$close
                                                        }
                                                    });
                                                },
                                                450);
                                        }
                                    }
                                } else {
                                    if (top.hideLoading) {
                                        top.hideLoading();
                                    }
                                    errorHandler(data);
                                }
                            };
                            var requestConfig = { url: "entityState/lock", formData: formData, handler: handler };
                            Api.postForm(requestConfig);
                        }
                    } else {
                        if (top.showConfirm) {
                            setTimeout(function() {
                                    top.showConfirm({
                                        title: "پیام سیستمی",
                                        body: "امکان قفل کردن رکورد قبل از ثبت آن وجود ندارد",
                                        confirmButton: {
                                            hidden: true
                                        },
                                        declineButton: {
                                            text: GeneralBundle.$close
                                        }
                                    });
                                },
                                450);
                        }
                    }
                }
            });
        $(".entity-form").prepend(lockBtn);
    }
    $("input[type=text]").each(function() {
        Util.setInputValidationListener($(this));
    });
    $("textarea").each(function() {
        Util.setInputValidationListener($(this));
    });
    $("input[type=password]").each(function() {
        Util.setInputValidationListener($(this));
    });
    $("input[type=checkbox].is-boolean").on("change",
        function() {
            const label = $(this).next(".form-check-label");
            if (label.length > 0 && label.attr("ksun-bundle-key")) {
                const key = label.attr("ksun-bundle-key").split("_")[0];
                if ($(this).prop("checked")) {
                    label.html(GeneralBundle[key] ? GeneralBundle[key] : label.html());
                } else {
                    label.html(GeneralBundle[key + "_reverse"] ? GeneralBundle[key + "_reverse"] : label.html());
                }
            }
        });
    $("select").each(function() {
        Util.setSelect2ValidationListener($(this));
    });
    // file
    $(".file").each(function () {
        const ctx = $(this);
        const acceptFormats = ctx.attr("ksun-accept-format") ? ctx.attr("ksun-accept-format").split(";") : "*";
        const maxCount = ctx.attr("ksun-max-count") ? Number(ctx.attr("ksun-max-count")) : 1;
        const maxSize = ctx.attr("ksun-max-size") ? Number(ctx.attr("ksun-max-size"))*10000 : 20*10000;
        var fileManagerConfig = {
            acceptFormats: acceptFormats,
            maxCount: maxCount,
            maxSize: maxSize, // 2MB
            onConfirm: onFileManagerConfirm,
            elem: ctx // selected button
        };
        ctx.on("click",
            function() {
                top.FileManager.configure(fileManagerConfig);
                top.FileManager.showFileManager();
            });
    });

    // object list
    objectsListAddBtn.on("click",
        function() {
            const pattern = $(this).prev(".form-control-wrapper");
            const newItem = pattern.clone(); // don't use clone(true), because we don't need to use pattern data()
            newItem.val("");
            newItem.insertAfter(pattern);
        });

    // ripple
    $.ripple(".btn.material",
        {
            debug: false,
            on: "mousedown",
            opacity: 0.4,
            color: "auto",
            multi: false,
            duration: 0.7,
            rate: function(pxPerSecond) {
                return pxPerSecond;
            },
            easing: "linear"
        });
};

var initScrollBar = function() {
    if ($(window).width() > 992) {
        const scrollCtn = $(".scroll-container").not(".ignore");
        if (scrollCtn.length) {
            new SimpleBar(scrollCtn[0]);
        }
    }
};

var onFileManagerConfirm = function(listOfFiles, element) {
    const parentElem = element.closest(".form-group");
    parentElem.find(".feedback").remove();

    if (listOfFiles.length === 0 && parentElem.hasClass(ENVIRONMENT.CssClass.MANDATORY)) {
        parentElem.addClass(ENVIRONMENT.CssClass.HAS_ERROR);
        parentElem.append(
            `<div class="col-sm-10 offset-sm-2 invalid-feedback feedback">${GeneralBundle.$feedbackSelectItem}</div>`);
        return;
    }
    parentElem.removeClass(ENVIRONMENT.CssClass.HAS_ERROR);
    var wrapper = element.next(".files-wrapper");
    if (wrapper.length > 0) {
        wrapper.empty();
    } else {
        wrapper = $('<div class="files-wrapper d-flex align-items-center flex-wrap"></div>');
        wrapper.insertAfter(element);
    }
    listOfFiles.forEach(function(file) {
        addFile(file, wrapper);
    });
};

var addFile = function(file, container) {

    const ctn = $("<div/>",
        {
            class: "item",
            data: {
                object: file
            }
        });
    const title = $("<span/>",
        {
            class: "title",
            text: file.name
        });
    var removeCtn = $("<span/>",
        {
            class: "file-remove-svg-holder",
            html: '<i class="fas fa-times"></i>',
            click: function() {
                removeCtn.closest(".item").remove().tooltip("dispose");
            }
        });
    ctn.append(title, removeCtn);
    if (file.thumbnail) {
        ctn.attr("title",
            `<img style="width: 150px; object-fit: cover" src=${file.thumbnail}>${file.description
            ? `<p style="margin-bottom: 0!important;">${file.description}</p>`
            : ``}`);
    } else {
        ctn.attr("title", `<p style="margin-bottom: 0!important;">${file.description}</p>`);
    }
    ctn.tooltip({ html: true, trigger: "hover" });
    container.append(ctn);
};

var clearForm = function() {
    $(".edit-fieldset").find("input").each(function() {
        var ctx = $(this);
        var bundleKey = ctx.attr("ksun-bundle-key");
        bundleKey = bundleKey.indexOf("$") === 0 ? bundleKey.substr(1, bundleKey.length - 1) : bundleKey;
        var mustIgnore = false;
        ignoreToClear.forEach(function(item) {
            if (bundleKey === item) {
                mustIgnore = true;
                const defaultValue = ctx.attr("ksun-default-value");
                if (defaultValue) {
                    if (ctx.hasClass("date")) {
                        const currentDate = Number(defaultValue);
                        ctx.data().datepicker.setDate(currentDate);
                    } else {
                        ctx.val(defaultValue);
                    }
                }
                return -1;
            }
        });
        if (!mustIgnore) {
            const itemParent = ctx.closest(`.${ENVIRONMENT.CssClass.FORM_GROUP}`);
            ctx.removeClass(ENVIRONMENT.CssClass.FLOATING_MODE);
            if (itemParent.data("secure")) {
                itemParent.removeData(bundleKey);
            }
            if (ctx.attr("type") === "radio") {
                if (ctx.attr("checked")) {
                    ctx.prop("checked", true);
                }
            } else if (ctx.attr("type") === "checkbox") {
                ctx.prop("checked", false);
            } else {
                ctx.val(undefined);
            }
        }
    });
    $(".edit-fieldset").find("select").each(function() {
        const ctx = $(this);
        var bundleKey = ctx.attr("ksun-bundle-key");
        bundleKey = bundleKey.indexOf("$") === 0 ? bundleKey.substr(1, bundleKey.length - 1) : bundleKey;
        var mustIgnore = false;
        ignoreToClear.forEach(function(item) {
            if (bundleKey === item) {
                mustIgnore = true;
                return -1;
            }
        });
        if (!mustIgnore) {
            const itemParent = ctx.closest(`.${ENVIRONMENT.CssClass.FORM_GROUP}`);
            ctx.removeClass(ENVIRONMENT.CssClass.FLOATING_MODE);
            if (itemParent.data("secure")) {
                itemParent.removeData(bundleKey);
            }
            if (ctx.select2) {
                if (ctx.find("option").length) {
                    $(this).select2("val", -1);
                }
            }
        }
    });
    $(".has-error").removeClass("has-error");

    $(".edit-fieldset").find("TEXTAREA").each(function() {
        const ctx = $(this);
        var bundleKey = ctx.attr("ksun-bundle-key");
        bundleKey = bundleKey.indexOf("$") === 0 ? bundleKey.substr(1, bundleKey.length - 1) : bundleKey;
        var mustIgnore = false;
        ignoreToClear.forEach(function(item) {
            if (bundleKey === item) {
                mustIgnore = true;
                return -1;
            }
        });
        if (!mustIgnore) {
            const itemParent = ctx.closest(`.${ENVIRONMENT.CssClass.FORM_GROUP}`);
            if (itemParent.data("secure")) {
                itemParent.removeData(bundleKey);
            }
            if (window.CKEDITOR && CKEDITOR.instances[this.id]) {
                CKEDITOR.instances[this.id].setData("");
            } else {
                ctx.val(undefined);
            }

        }
    });

    // Clear multiple Select Containers
    $(".multiple-select-container").empty();

    // objects list (clear inputs)
    $(".objects-list-wrapper").find(".form-control-wrapper").each(function(index) {
        const elem = $(this);
        if (index > 0) {
            elem.remove();
        } else {
            elem.find("input").removeData("object").val("");
        }
    });

    //remove file
    $(".files-wrapper").remove();
    $("#edit-submit").removeClass("failed").removeClass("finished");
};

var fillForm = function(rowData, callback) {
    $("[ksun-bundle-key]").each(function() {
        var ctx = $(this);
        var bundleKey = ctx.attr("ksun-bundle-key");
        bundleKey = bundleKey.indexOf("$") === 0 ? bundleKey.substr(1, bundleKey.length - 1) : bundleKey;
        for (var key in rowData) {
            if (rowData.hasOwnProperty(key) && key === bundleKey) {
                if (this.tagName === "INPUT") {
                    if (key === "created" || key === "updated") {
                        ctx.val(new persianDate(Number(rowData[key])).format("dddd D MMMM YYYY") +
                            "ساعت " +
                            new persianDate(Number(rowData[key])).format("HH:mm:ss"));
                        ctx.data("object", { date: rowData[key] });
                    } else {
                        if (ctx.hasClass("date")) {
                            if (!ctx.attr("readonly")) {
                                if (datePickerList[key]) {
                                    if (rowData[key]) {
                                        datePickerList[key].pDatePicker.setDate(Number(rowData[key]));
                                    }
                                }
                            } else {
                                if (rowData[key]) {
                                    ctx.val(new persianDate(Number(rowData[key])).format("dddd D MMMM YYYY") +
                                        "ساعت " +
                                        new persianDate(Number(rowData[key])).format("HH:mm:ss"));
                                }
                            }
                        } else {
                            var itemParent = ctx.closest(`.${ENVIRONMENT.CssClass.FORM_GROUP}`);
                            if (itemParent.length && itemParent.attr("data-secure")) {
                                itemParent.data(bundleKey, rowData[key]);
                            }
                            var format = this.getAttribute("ksun-text-format");
                            if (Array.isArray(rowData[key])) {
                                if (rowData[key].length > 0) {
                                    if (typeof rowData[key][0] === "object") {
                                        if (format) {
                                            var temp = "";
                                            for (var k in rowData[key]) {
                                                if (rowData[key].hasOwnProperty(k)) {
                                                    temp += (Util.getTitleOfObjectFromFormat(rowData[key][k], format) +
                                                        ",");
                                                }
                                            }
                                            ctx.val(temp);
                                        } else {
                                            ctx.val(rowData[key].fullTitle);
                                        }
                                    } else {
                                        ctx.val(rowData[key].join(","));
                                    }
                                }
                            } else if (typeof rowData[key] === "object") {
                                if (format) {
                                    ctx.val(Util.getTitleOfObjectFromFormat(rowData[key], format));
                                } else {
                                    ctx.val(rowData[key].fullTitle);
                                }
                                ctx.data("object", rowData[key]);
                            } else {
                                if (ctx.attr("type") === "checkbox") {
                                    ctx.prop("checked", rowData[key]);
                                } else if (ctx.attr("type") === "radio") {
                                    if (ctx.data("value") === rowData[key]) {
                                        ctx.prop("checked", true);
                                    }
                                } else {
                                    ctx.val(rowData[key]);
                                }
                            }
                        }
                    }
                } else if (this.tagName === "SELECT") {
                    if (ctx.hasClass("autocomplete")) {
                        var config = ctx.data().select2Config;
                        if (config && config.multipleSelect) {
                            var multipleSelectWrapper = ctx.closest(".form-group").find(".multiple-select-wrapper");
                            var multipleSelectCtn = multipleSelectWrapper.find(".multiple-select-container");
                            if (Array.isArray(rowData[bundleKey])) {
                                rowData[bundleKey].forEach(function(itemData) {
                                    var item = $("<span/>",
                                        {
                                            class: "badge badge-light",
                                            html: $("<span/>",
                                                {
                                                    class: "title",
                                                    text: itemData[config.text]
                                                }),
                                            data: {
                                                id: itemData[config.value]
                                            }
                                        });
                                    const removeIcon = $("<span/>",
                                        {
                                            class: "remove-icon",
                                            html: '<i class="fas fa-times"></i>',
                                            click: function(e) {
                                                item.remove();
                                            }
                                        });
                                    item.append(removeIcon);
                                    multipleSelectCtn.prepend(item);
                                });
                            }
                        } else {
                            var tempKey = (ctx.data().select2Config && ctx.data().select2Config.value)
                                ? ctx.data().select2Config.value
                                : "entityId";
                            if (ctx.hasClass("static-autocomplete")) {
                                if ($.isArray(rowData[key])) {
                                    ctx.data({ multi: rowData[key] }).trigger("change.select2");
                                } else {
                                    ctx.val(rowData[key][tempKey]).trigger("change.select2");
                                }
                            } else {
                                ctx.empty();
                                var option = $("<option></option>");
                                option.val(rowData[key][tempKey]);
                                if (rowData[key][config.text]) {
                                    option.text(rowData[key][config.text]);
                                } else {
                                    option.text(rowData[key]["fullTitle"]);
                                }
                                option.data("object", rowData[key]);
                                ctx.append(option).trigger("change.select2");
                            }
                        }
                    }
                } else if (this.tagName === "TEXTAREA") {
                    if (window.CKEDITOR && CKEDITOR.instances[ctx.attr("id")]) {
                        CKEDITOR.instances[ctx.attr("id")].setData(rowData[key]);
                    } else {
                        ctx.val(rowData[key]);
                    }
                } else if (this.tagName === "BUTTON") {
                    if (ctx.hasClass("file")) {
                        var wrapper = ctx.next(".files-wrapper");
                        if (wrapper.length > 0) {
                            wrapper.empty();
                        } else {
                            wrapper = $('<div class="files-wrapper d-flex align-items-center flex-wrap"></div>');
                            wrapper.insertAfter(ctx);
                        }
                        if (Array.isArray(rowData[key])) {
                            rowData[key].forEach(function(file) {
                                addFile(file, wrapper);
                            });
                        } else {
                            addFile(rowData[key], wrapper);
                        }
                    }
                }
            }
            ctx.addClass(ENVIRONMENT.CssClass.FLOATING_MODE);
        }
    });
    // objects list
    $(".objects-list-wrapper").each(function() {
        var ctx = $(this);
        const key = ctx.attr("ksun-object-key");
        var conf = objectsConfig[key];
        // create input(s)
        var initialInputWrapper = ctx.find(".form-control-wrapper").first();
        rowData[key].forEach(function(obj) {
            const inputWrapper = initialInputWrapper.clone();
            const input = inputWrapper.find("input");
            input.data("object", obj);
            input.val(obj[conf.property]);
            inputWrapper.insertBefore(ctx.find(".add"));
        });
        initialInputWrapper.remove();
    });

    if (callback && typeof callback === "function") {
        callback.apply(null, []);
    }
    if (localVariables.lockAble) {
        var formData = new FormData();
        formData.append("tblName", localVariables.clazzName);
        formData.append("tblEntityId", rowData.entityId);
        const handler = new Handler();
        handler.beforeSend = () => {
        };
        handler.complete = () => {
        };
        handler.success = (data) => {
            if (data.done) {
                $(".locker").find("svg").removeClass("text-gold text-danger");
                if (top.hideLoading) {
                    top.hideLoading();
                } else if (hidePageLoading) {
                    hidePageLoading();
                }
                const mainData = data.result;
                if (mainData) {
                    if (mainData.active === "1") {
                        $(".locker").find("svg").addClass("text-gold");
                    } else {
                        $(".locker").find("svg").addClass("text-danger");
                        $("#edit-submit").remove();
                        $("#edit-clear").remove();
                        $(".locker").data({ text: data.result });
                        if (top.showConfirm) {
                            setTimeout(function () {
                                top.showConfirm({
                                    title: "پیام سیستمی",
                                    body: "موجودیت جاری قفل می باشد",
                                    confirmButton: {
                                        hidden: true
                                    },
                                    declineButton: {
                                        text: GeneralBundle.$close
                                    }
                                });
                            },
                                450);
                        }
                    }
                }
            } else {
                if (top.hideLoading) {
                    top.hideLoading();
                }
                if (data.errorCode !== 8) {
                   errorHandler(data); 
                }
                
            }
        };
        var requestConfig = { url: "entityState/getState", formData: formData, handler: handler };
        Api.postForm(requestConfig);
    }
};

var showRow = function(rowData) {
    if (localShowRow) {
        if (localShowRow.replace) {
            localShowRow.method.apply(null, [rowData]);
        } else {
            clearForm();
            if (rowData) {
                gridRefreshFlag = false;
                refreshButton.removeAttr("disabled");
                currentData = rowData;
                if (submitButton.find('[ksun-bundle-key="$submit"]').length) {
                    submitButton.find('[ksun-bundle-key="$submit"]').text(GeneralBundle.$update);
                } else {
                    submitButton.text(GeneralBundle.$update);
                }
                var showRowWheres = new Wheres();
                showRowWheres.add("entityId", rowData.entityId, ENVIRONMENT.Condition.EQUAL);
                showRowFilter.setWheres(showRowWheres.getList());
                Api.showRow({ url: URLs.services.showRow, filter: showRowFilter.get(), handler: showRowHandler });
            } else {
                currentData = undefined;
                if (submitButton.find('[ksun-bundle-key="$submit"]').length) {
                    submitButton.find('[ksun-bundle-key="$submit"]').text(GeneralBundle.$submit);
                } else {
                    submitButton.text(GeneralBundle.$submit);
                }
                refreshButton.attr("disabled", true);
            }
            localShowRow.method.apply(null, [rowData]);
        }
    } else {
        clearForm();
        if (rowData) {
            gridRefreshFlag = false;
            refreshButton.removeAttr("disabled");
            currentData = rowData;
            if (submitButton.find('[ksun-bundle-key="$submit"]').length) {
                submitButton.find('[ksun-bundle-key="$submit"]').text(GeneralBundle.$update);
            } else {
                submitButton.text(GeneralBundle.$update);
            }
            var showRowWheres = new Wheres();
            showRowWheres.add("entityId", rowData.entityId, ENVIRONMENT.Condition.EQUAL);
            showRowFilter.setWheres(showRowWheres.getList());
            Api.showRow({ url: URLs.services.showRow, filter: showRowFilter.get(), handler: showRowHandler });
        } else {
            currentData = undefined;
            if (submitButton.find('[ksun-bundle-key="$submit"]').length) {
                submitButton.find('[ksun-bundle-key="$submit"]').text(GeneralBundle.$submit);
            } else {
                submitButton.text(GeneralBundle.$submit);
            }
            refreshButton.attr("disabled", true);
        }
    }
};

var saveOrUpdate = function() {
    $("#edit-submit").removeClass("failed").removeClass("finished");
    if (Util.validateForm(entityForm) && Util.validateForm(baseForm)) {
        if (entityForm.attr("enctype")) {
            var formData = new FormData();

            // Input
            dataContainer.find("input").not(".list-item,.read-only").each(function() {
                const ctx = $(this);
                var key = ctx.attr("ksun-bundle-key");
                if (key) {
                    const itemParent = ctx.closest(`.${ENVIRONMENT.CssClass.FORM_GROUP}`);
                    key = key.indexOf("$") === 0 ? key.substr(1, key.length - 1) : key;
                    if (ctx.attr("type") === "checkbox") {
                        formData.append(key, ctx.prop("checked"));
                    } else if (ctx.attr("type") === "radio") {
                        if (ctx.prop("checked")) {
                            formData.append(key, ctx.data("value"));
                        }
                    } else {
                        if (ctx.data("object")) {
                            if (key === "created" || key === "updated") {
                                formData.append(key, ctx.data("object").date);
                            } else if (ctx.hasClass(ENVIRONMENT.CssClass.DATE)) {
                                if (ctx.val()) {
                                    formData.append(key, ctx.data("object")[key]);
                                }

                            } else {
                                formData.append(key, ctx.data("object").entityId);
                            }
                        } else if (itemParent.length &&
                            itemParent.attr("data-secure") &&
                            itemParent.data() &&
                            itemParent.data()[key]) {
                            formData.append(key, itemParent.data()[key]);
                        } else if (ctx.val()) {
                            if (ctx.hasClass(ENVIRONMENT.CssClass.DATE)) {
                                if (ctx.val()) {
                                    formData.append(key, ctx.data("object")[key]);
                                }
                            } else {
                                formData.append(key, ctx.val());
                            }
                        }
                    }
                }
            });

            //TextArea
            dataContainer.find("textArea").not(".clob").each(function() {
                const ctx = $(this);
                var key = ctx.attr("ksun-bundle-key");
                if (key) {
                    const itemParent = ctx.closest(`.${ENVIRONMENT.CssClass.FORM_GROUP}`);
                    key = key.indexOf("$") === 0 ? key.substr(1, key.length - 1) : key;
                    formData.append(key, ctx.val());
                }
            });
            // Autocomplete
            dataContainer.find("select.autocomplete").each(function() {
                var ctx = $(this);
                var key = ctx.attr("ksun-bundle-key");
                var itemParent = ctx.closest(`.${ENVIRONMENT.CssClass.FORM_GROUP}`);
                key = key.indexOf("$") === 0 ? key.substr(1, key.length - 1) : key;
                var config = ctx.data().select2Config;
                if (config) {
                    if (config.multipleSelect) { // درصورتیکه قابلیت انتخاب چند مورد وجود داشته باشد
                        var multipleSelectWrapper = itemParent.find(".multiple-select-wrapper");
                        if (multipleSelectWrapper.length) {
                            multipleSelectWrapper.find(".badge").each(function() {
                                if ($(this).data().id) {
                                    formData.append(key, $(this).data().id);
                                }
                            });
                        }
                    } else {
                        var valueProperty = (config && config.value) ? config.value : "entityId";
                        if (itemParent.length && itemParent.attr("data-secure")) {
                            if (currentData &&
                                ctx.find("option:selected").length > 0 &&
                                ctx.find("option:selected").data("object")) {
                                formData.append(key, ctx.find("option:selected").data("object")[valueProperty]);
                            } else if (ctx.val()) {
                                formData.append(key, ctx.val());
                            }
                        } else if (ctx.val()) {
                            formData.append(key, ctx.val());
                        }
                    }
                } else {
                    var valueProperty = "entityId";
                    if (itemParent.length && itemParent.attr("data-secure")) {
                        if (currentData &&
                            ctx.find("option:selected").length > 0 &&
                            ctx.find("option:selected").data("object")) {
                            formData.append(key, ctx.find("option:selected").data("object")[valueProperty]);
                        } else if (ctx.val()) {
                            formData.append(key, ctx.val());
                        }
                    } else if (ctx.val()) {
                        if (ctx.siblings(".multiple-select-wrapper").length > 0
                        ) { // درصورتیکه قابلیت انتخاب چند مورد وجود داشته باشد
                            var multipleSelectWrapper = ctx.siblings(".multiple-select-wrapper");
                            if (multipleSelectWrapper.length) {
                                multipleSelectWrapper.find(".badge").each(function() {
                                    if ($(this).data().id) {
                                        formData.append(key, Number($(this).data().id));
                                    }
                                });
                            }
                        } else {
                            formData.append(key, ctx.val());
                        }

                    } else {
                        if (ctx.siblings(".multiple-select-wrapper").length > 0
                        ) { // درصورتیکه قابلیت انتخاب چند مورد وجود داشته باشد
                            var multipleSelectWrapper = ctx.siblings(".multiple-select-wrapper");
                            if (multipleSelectWrapper.length) {
                                multipleSelectWrapper.find(".badge").each(function() {
                                    if ($(this).data().id) {
                                        formData.append(key, Number($(this).data().id));
                                    }
                                });
                            }
                        }
                    }
                }
            });

            // File
            dataContainer.find(".file").each(function() {
                const ctx = $(this);
                var key = ctx.attr("ksun-bundle-key");
                key = key.indexOf("$") === 0 ? key.substr(1, key.length - 1) : key;
                const maxCount = ctx.attr("ksun-max-count") ? Number(ctx.attr("ksun-max-count")) : 1;
                if (ctx.next(".files-wrapper").length === 1) {
                    if (maxCount > 1) {
                        ctx.next(".files-wrapper").find(".item").each(function(ind, item) {
                            const object = $(item).data("object");
                            formData.append(key, object.entityId);
                        });
                    } else {
                        ctx.next(".files-wrapper").find(".item").each(function(ind, item) {
                            if (ind === 0) {
                                const object = $(item).data("object");
                                formData.append(key, object.entityId);
                            }
                        });
                    }

                }
            });

            // CKEditor
            try {
                dataContainer.find(".clob").each(function() {
                    var key = $(this).attr("ksun-bundle-key");
                    key = key.indexOf("$") === 0 ? key.substr(1, key.length - 1) : key;
                    if (CKEDITOR.instances[this.id] && CKEDITOR.instances[this.id].getData()) {
                        formData.append(key, CKEDITOR.instances[this.id].getData());
                    }
                });
            } catch (e) {
                console.error("CKEditor is undefined!", e);
            }

            var requestConfig = { url: URLs.services.save, formData: formData, handler: saveHandler };
            Api.postForm(requestConfig);
        } else {
            var data = {};
            dataContainer.find("input").not(".list-item,.read-only").each(function() {
                const ctx = $(this);
                var key = ctx.attr("ksun-bundle-key");
                if (key) {
                    const itemParent = ctx.closest(`.${ENVIRONMENT.CssClass.FORM_GROUP}`);
                    key = key.indexOf("$") === 0 ? key.substr(1, key.length - 1) : key;
                    if (ctx.attr("type") === "checkbox") {
                        data[key] = ctx.prop("checked");
                    } else if (ctx.attr("type") === "radio") {
                        if (ctx.prop("checked")) {
                            data[key] = ctx.data("value");
                        }
                    } else {
                        if (ctx.data("object")) {
                            if (key === "created" || key === "updated") {
                                data[key] = ctx.data("object").date;
                            } else if (ctx.hasClass(ENVIRONMENT.CssClass.DATE)) {
                                if (ctx.val()) {
                                    data[key] = ctx.data("object")[key];
                                }

                            } else {
                                data[key] = { entityId: ctx.data("object").entityId };
                            }
                        } else if (itemParent.length && itemParent.attr("data-secure")) {
                            data[key] = itemParent.data() ? itemParent.data()[key] : null;
                        } else if (ctx.val()) {

                            if (ctx.hasClass(ENVIRONMENT.CssClass.DATE)) {
                                if (ctx.val()) {
                                    data[key] = ctx.data("object")[key];
                                }
                            } else {

                                data[key] = ctx.val();
                            }
                        }
                    }
                }
            });

            // Autocomplete
            dataContainer.find("select.autocomplete").each(function() {
                var ctx = $(this);
                var key = ctx.attr("ksun-bundle-key");
                var itemParent = ctx.closest(`.${ENVIRONMENT.CssClass.FORM_GROUP}`);
                key = key.indexOf("$") === 0 ? key.substr(1, key.length - 1) : key;
                var config = ctx.data().select2Config;
                if (config) {
                    if (config.multipleSelect) { // درصورتیکه قابلیت انتخاب چند مورد وجود داشته باشد
                        data[key] = [];
                        var multipleSelectWrapper = itemParent.find(".multiple-select-wrapper");
                        if (multipleSelectWrapper.length) {
                            multipleSelectWrapper.find(".badge").each(function(i, v) {
                                if ($(v).data().id) {
                                    data[key].push({ entityId: $(v).data().id });
                                }
                            });
                        }
                    } else {
                        var valueProperty = (config && config.value) ? config.value : "entityId";
                        if (itemParent.length && itemParent.attr("data-secure")) {
                            if (currentData &&
                                ctx.find("option:selected").length > 0 &&
                                ctx.find("option:selected").data("object")) {
                                data[key] = {};
                                data[key][valueProperty] = ctx.find("option:selected").data("object")[valueProperty];
                            } else if (ctx.val()) {
                                data[key] = {};
                                data[key][valueProperty] = ctx.val();
                            }
                        } else if (ctx.val()) {
                            data[key] = {};
                            data[key][valueProperty] = ctx.val();
                        }
                    }
                } else {
                    var valueProperty = "entityId";
                    if (itemParent.length && itemParent.attr("data-secure")) {
                        if (currentData &&
                            ctx.find("option:selected").length > 0 &&
                            ctx.find("option:selected").data("object")) {
                            data[key] = {};
                            data[key][valueProperty] = ctx.find("option:selected").data("object")[valueProperty];
                        } else if (ctx.val()) {
                            data[key] = {};
                            data[key][valueProperty] = ctx.val();
                        }
                    } else if (ctx.val()) {
                        data[key] = {};
                        data[key][valueProperty] = ctx.val();
                    }
                }
            });

            // File
            dataContainer.find(".file").each(function() {
                const ctx = $(this);
                var key = ctx.attr("ksun-bundle-key");
                key = key.indexOf("$") === 0 ? key.substr(1, key.length - 1) : key;
                const maxCount = ctx.attr("ksun-max-count") ? Number(ctx.attr("ksun-max-count")) : 1;
                if (ctx.next(".files-wrapper").length === 1) {
                    if (maxCount > 1) {
                        data[key] = [];
                        ctx.next(".files-wrapper").find(".item").each(function(ind, item) {
                            const object = $(item).data("object");
                            data[key].push({
                                entityId: object.entityId,
                                parentID: object.parentID ? object.parentID : -1
                            });
                        });
                    } else {
                        ctx.next(".files-wrapper").find(".item").each(function(ind, item) {
                            if (ind === 0) {
                                const object = $(item).data("object");
                                data[key] = {
                                    entityId: object.entityId,
                                    parentID: object.parentID ? object.parentID : -1
                                };
                            }
                        });
                    }

                }
            });

            // Textarea
            dataContainer.find("textarea").each(function() {
                const ctx = $(this);
                const key = ctx.attr("ksun-bundle-key");
                if (ctx.val() && ctx.val() !== null) {
                    data[key] = ctx.val();
                }
            });

            // CKEditor
            try {
                dataContainer.find(".clob").each(function() {
                    var key = $(this).attr("ksun-bundle-key");
                    key = key.indexOf("$") === 0 ? key.substr(1, key.length - 1) : key;
                    if (CKEDITOR.instances[this.id] && CKEDITOR.instances[this.id].getData()) {
                        data[key] = CKEDITOR.instances[this.id].getData();
                    }
                });
            } catch (e) {
                console.error("CKEditor is undefined!", e);
            }

            // Object List
            dataContainer.find(".objects-list-wrapper").each(function() {
                const ctx = $(this);
                const key = ctx.attr("ksun-object-key");
                data[key] = [];
            });
            dataContainer.find(".objects-list-wrapper").each(function() {
                const ctx = $(this);
                var key = ctx.attr("ksun-object-key");
                var conf = objectsConfig[key];
                ctx.find(".list-item").each(function() {
                    const elem = $(this);
                    var obj = {};
                    if (elem.data("object")) { // show row mode
                        obj = elem.data("object");
                    }
                    obj[conf.property] = elem.val();
                    data[key].push(obj);
                });
            });

            var requestConfig = { url: URLs.services.save, data: data, handler: saveHandler };
            Api.save(requestConfig);
        }
        gridRefreshFlag = true;
    } else {
        changeButtonState({ button: submitButton, state: 0 });
    }
};

var showPageLoading = function() {
    pageLoadingModal.modal("show");
};
var hidePageLoading = function() {
    setTimeout(function() {
            pageLoadingModal.modal("hide");
        },
        500);
};
var showConfirmation = function(config) {
    pageConfirmModal.modal("show");
};

var onComponentLoad = function() {
    var data = { id: componentID, contentHeight: document.body.scrollHeight + 25 + "px" };
    if (top && top.onComponentLoad) {
        setTimeout(function() {
                top.onComponentLoad(data);
            },
            150);
    }
};

var panelRelatedActions = function() {
    if (top.isPanel) {
        $("#process-title").removeAttr("ksun-bundle-key");
    }
};

var onPageReady = function() {
    // change page language according to user lang
    $("html").attr("lang", Storage.getUserInfo().lang === 1 ? "FA" : "EN");
    initViews();
    initInstances();
    initListeners();
    initScrollBar();
    panelRelatedActions();
    Translation.translate();

    if (Util.urlParameter("eID")) {
        showRow({ entityId: Util.urlParameter("eID") });
    }

    Mousetrap.bind('ctrl+s', function (e) {
        e.stopPropagation();
        e.preventDefault();
        saveOrUpdate();
    });
    Mousetrap.bind('f6', function (e) {
        e.stopPropagation();
        e.preventDefault();
        clearForm();
    });
    Mousetrap.bind('esc', function (e) {
        try {
            e.stopPropagation();
            e.preventDefault();
            parent.editFrameWrapper.removeClass("visible");
            if (gridRefreshFlag) {
                parent.retrieveData();
            }
        } catch (e) {

        }
    });
    Mousetrap.bind('f5', function (e) {
        e.stopPropagation();
        e.preventDefault();
        showRow(currentData);
    });
    Mousetrap.bind('ctrl+m', function (e) {
        showRow();
    });
};