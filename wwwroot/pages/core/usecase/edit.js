'use strict';
let checkboxContainer;
var localVariables = {
    lockAble: false,
    clazzName: "UseCase",
    URLs: {
        services: {
            showRow: 'usecase/showRow',
            save: 'usecase/save',
            autocompleteView: "usecase/autocompleteView",
            actionList: "action/getAllData"
        }
    }
};
let localInitListeners = () => {

}
let localInitViews = function () {
    checkboxContainer = $("#checkbox-container");
};
var initAutoCompletes = function () {
    //والد
    let provinceAutocomplete = new Autocomplete();
    provinceAutocomplete.init({
        element: $('#parent'),
        placeholder: "parent",
        type: ENVIRONMENT.Autocomplete.TYPE.DYNAMIC,
        url: 'usecase/autocompleteView',
        dynamicConfig: UsecaseAutocomplete.dynamicConfig,
        dropdownCssClass: 'form-control-sm floating'
    });

};
const getAllAction = () => {
    const handler = new Handler();
    handler.beforeSend = () => {
    };
    handler.success = (data, textStatus, jqXHR) => {
        if (data.done) {
            if (top.hideLoading) {
                top.hideLoading();
            } else if (hidePageLoading) {
                hidePageLoading();
            }
            if (data.result) {
                createCheckBox(data.result);
            }
        } else {
            if (top.hideLoading) {
                top.hideLoading();
            }
            errorHandler(data);
        }
    }
    Api.post({
        url: localVariables.URLs.services.actionList,
        handler: handler,
        data: {}
    });
}
const createCheckBox = (data, selected) => {
    if (data.length !== 0) {
        checkboxContainer.html("");
    }
    $.each(data, (index, value) => {
        let formGroup = $("<div/>", {
            class: "form-group floating-form-group col-12 col-lg-2"
        });
        let floating = $("<div/>", {
            class: "form-group floating-form-group col-12 floating-form-checkbox-wrapper d-rtl",
        });
        let input = $("<input/>", {
            class: "form-control form-control-sm floating-form-control floating-mode input",
            type: "checkbox",
            tabindex: "3",
            id: "entity_" + value.entityId,
            value: value.entityId,
            "ksun-bundle-key": "useCaseActions"
        });
        let label = $("<label/>", { class: "floating-form-checkbox lbl lbl-checkbox", for: "entity_" + value.entityId });
        let secondLabel = $("<label/>", {
            class: "floating-form-checkbox-label lbl lbl-text",
            for: "entity_" + value.entityId,
            html: value.actionName,
        });
        input.data().value = value;
        floating.append(input, label, secondLabel);
        formGroup.append(floating);
        checkboxContainer.append(formGroup);
    });
    if (selected && selected.length > 0) {
        $.each(selected, (i, v) => {
            $("#entity_" + v.action.entityId).prop("checked", true);
        });
    }
}
fillForm = function (rowData, callback) {
    $('[ksun-bundle-key]').each(function () {
        var ctx = $(this);
        var bundleKey = ctx.attr('ksun-bundle-key');
        bundleKey = bundleKey.indexOf("$") === 0 ? bundleKey.substr(1, bundleKey.length - 1) : bundleKey;
        for (var key in rowData) {
            if (rowData.hasOwnProperty(key) && key === bundleKey) {
                if (this.tagName === 'INPUT') {
                    if (key === 'created' || key === 'updated') {
                        ctx.val(new persianDate(Number(rowData[key])).format('dddd D MMMM YYYY') + "ساعت " + new persianDate(Number(rowData[key])).format('HH:mm:ss'));
                        ctx.data('object', { date: rowData[key] });
                    } else {
                        if (ctx.hasClass('date')) {
                            if (!ctx.attr('readonly')) {
                                if (datePickerList[key]) {
                                    datePickerList[key].pDatePicker.setDate(Number(rowData[key]));
                                }
                            } else {
                                ctx.val(new persianDate(Number(rowData[key])).format('dddd D MMMM YYYY') + "ساعت " + new persianDate(Number(rowData[key])).format('HH:mm:ss'));
                            }
                        } else {
                            var itemParent = ctx.closest('.' + ENVIRONMENT.CssClass.FORM_GROUP);
                            if (itemParent.length && itemParent.attr('data-secure')) {
                                itemParent.data(bundleKey, rowData[key]);
                            }
                            var format = this.getAttribute('ksun-text-format');
                            if (Array.isArray(rowData[key])) {
                                if (rowData[key].length > 0) {
                                    if (typeof rowData[key][0] === 'object') {
                                        if (format) {
                                            var temp = "";
                                            for (var k in rowData[key]) {
                                                if (rowData[key].hasOwnProperty(k)) {
                                                    temp += (Util.getTitleOfObjectFromFormat(rowData[key][k], format) + ",");
                                                }
                                            }
                                            ctx.val(temp);
                                        } else {
                                            ctx.val(rowData[key].fullTitle);
                                        }
                                    } else {
                                        ctx.val(rowData[key].join(','));
                                    }
                                }
                            } else if (typeof rowData[key] === 'object') {
                                if (format) {
                                    ctx.val(Util.getTitleOfObjectFromFormat(rowData[key], format));
                                } else {
                                    ctx.val(rowData[key].fullTitle);
                                }
                                ctx.data('object', rowData[key]);
                            } else {
                                if (ctx.attr('type') === 'checkbox') {
                                    ctx.prop('checked', rowData[key]);
                                } else if (ctx.attr('type') === 'radio') {
                                    if (ctx.data('value') === rowData[key]) {
                                        ctx.prop('checked', true);
                                    }
                                } else {
                                    ctx.val(rowData[key]);
                                }
                            }
                        }
                    }
                } else if (this.tagName === 'SELECT') {
                    if (ctx.hasClass('autocomplete')) {
                        var config = ctx.data().select2Config;
                        if (config && config.multipleSelect) {
                            var multipleSelectWrapper = ctx.closest('.form-group').next('.multiple-select-wrapper');
                            var multipleSelectCtn = multipleSelectWrapper.find('.multiple-select-container');
                            if (Array.isArray(rowData[bundleKey])) {
                                rowData[bundleKey].forEach(function (itemData) {
                                    var item = $('<span/>', {
                                        class: 'badge badge-light',
                                        html: $('<span/>', {
                                            class: 'title',
                                            text: itemData[config.text]
                                        }),
                                        data: {
                                            id: itemData[config.value]
                                        }
                                    });
                                    var removeIcon = $('<span/>', {
                                        class: 'remove-icon',
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
                            var tempKey = (ctx.data().select2Config && ctx.data().select2Config.value) ? ctx.data().select2Config.value : 'entityId';
                            if (ctx.hasClass('static-autocomplete')) {
                                ctx.val(rowData[key][tempKey]).trigger('change.select2');
                            } else {
                                ctx.empty();
                                var option = $('<option></option>');
                                option.val(rowData[key][tempKey]);
                                option.text(rowData[key][config.text]);
                                option.data('object', rowData[key]);
                                ctx.append(option).trigger('change.select2');
                            }
                        }
                    }
                } else if (this.tagName === 'TEXTAREA') {
                    if (window.CKEDITOR && CKEDITOR.instances[ctx.attr('id')]) {
                        CKEDITOR.instances[ctx.attr('id')].setData(rowData[key]);
                    } else {
                        ctx.val(rowData[key]);
                    }
                } else if (this.tagName === 'BUTTON') {
                    if (ctx.hasClass('file')) {
                        var wrapper = ctx.next('.files-wrapper');
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
    $('.objects-list-wrapper').each(function () {
        var ctx = $(this);
        var key = ctx.attr('ksun-object-key');
        var conf = objectsConfig[key];
        // create input(s)
        var initialInputWrapper = ctx.find('.form-control-wrapper').first();
        rowData[key].forEach(function (obj) {
            var inputWrapper = initialInputWrapper.clone();
            var input = inputWrapper.find('input');
            input.data('object', obj);
            input.val(obj[conf.property]);
            inputWrapper.insertBefore(ctx.find('.add'));
        });
        initialInputWrapper.remove();
    });
    if (callback && typeof callback === 'function') {
        callback.apply(null, []);
    }
    createCheckBox([], rowData.useCaseActions);
};
saveOrUpdate = function () {
    const formData = new FormData();
    formData.append("entityId",$("#entityId").val());
    formData.append("clazz",$("#clazz").val());
    formData.append("tableName",$("#tableName").val());
    formData.append("code",$("#code").val());
    formData.append("useCaseName", $("#useCaseName").val());
    if ($("#parent").val()) {
        formData.append("parent", $("#parent").val());
    }
    formData.append("Active", $("#active").prop("checked"));
    formData.append("virtualNode",$("#virtualNode").prop("checked"));
    let child = [];
    $.each(checkboxContainer.find("input"),(i, v) => {
            if ($(v).prop("checked") === true) {
                child.push($(v).data().value.entityId);
            }
        });
    formData.append("useCaseActions",child);
    let requestConfig = { url: URLs.services.save, formData: formData, handler: saveHandler };
    Api.postForm(requestConfig);
    gridRefreshFlag = true;

};
$(document).ready(function () {
    Storage.setPageNeedLogin(true);
    if (Storage.isPageNeedLogin() && Storage.getUserInfo() === null) {
        SSO.init();
    } else {
        initLocalVariables(localVariables);
        localInitViews();
        initAutoCompletes();
        localInitListeners();
        onPageReady();
        getAllAction();
    }
});
