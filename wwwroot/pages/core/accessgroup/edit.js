import * as tree from '/lib/tree/tree.js';
"use strict";

var localVariables = {
    URLs: {
        services: {
            showRow: "accessGroup/showRow",
            save: "accessGroup/save",
            autocomplete: "accessGroup/autocompleteView"
        }
    },
    lockAble: false,
    clazzName: "AccessGroup",
    ignoreToClear: []
};
var initAutoCompletes = function () {

};
var accessHandler = new Handler();
accessHandler.beforeSend = () => { };
accessHandler.complete = () => { };
accessHandler.success = function (result) {

};
$(document).ready(function () {
    initLocalVariables(localVariables);
    initAutoCompletes();
    onPageReady();
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
            console.error(e);
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
    tree.init({
        serviceUrl: 'usecase/getTotalUseCase',
        removeService: '',
        dependencies: {
            link: [`/lib/sweetalert/sweetalert.css`, `/lib/tree/style.css`],
            script: [`/lib/sweetalert/sweetalert.min.js`]
        },
        urlEditPage: '',
        title: "فرآیندها",
        getTotal: true,
        showCrud: false,
        showField: "fullTitle",
        functionData: ``,
        rootValue: [1],
        childKey: "parent",
        selectAllChildWhenSelectParent: true,
        openWholeTreeInGetTotal: true,
        selectState: 1,
        elementIdInEditPageForFillParent: "#tree"
    });
    new SimpleBar($(".scroll-container")[1]);
});
saveOrUpdate = function () {
    const formData = new FormData();
    formData.append("entityId", $("#entityId").val());
    formData.append("name", $("#name").val());
    formData.append("code", $("#code").val());
    formData.append("Active", $("#active").prop("checked"));
    let child = [];
    $.each(tree.getCheckedList(), (i, v) => {
        if (`${v.entityId}`.indexOf(",") !== -1) {
            child.push(v.entityId);
        } else {
            child.push(`${v.entityId}`);
        }

    });
    formData.append("actions", child);
    let requestConfig = { url: URLs.services.save, formData: formData, handler: saveHandler };
    Api.postForm(requestConfig);
    gridRefreshFlag = true;
};
fillForm = function (rowData, callback) {
    $("[ksun-bundle-key]").each(function () {
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
                                rowData[bundleKey].forEach(function (itemData) {
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
                                            click: function (e) {
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
                            rowData[key].forEach(function (file) {
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
    $(".objects-list-wrapper").each(function () {
        var ctx = $(this);
        const key = ctx.attr("ksun-object-key");
        var conf = objectsConfig[key];
        // create input(s)
        var initialInputWrapper = ctx.find(".form-control-wrapper").first();
        rowData[key].forEach(function (obj) {
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
    var lstTreeFiller = [];
    $.each(rowData.useCaseActionList, (i, v) => {
        let obj = {
            entityId: `${v.useCase.entityId}|${v.action.entityId}`,
            parent: v.useCase.entityId,
            fullTitle: v.action.actionName
        }
        lstTreeFiller.push(obj);
    });
    tree.setCheckedList(lstTreeFiller);

};