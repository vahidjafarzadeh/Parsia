"use strict";

var localVariables = {
    URLs: {
        services: {
            showRow: "comboval/showRow",
            save: "comboval/save",
            autocomplete: "comboval/autocompleteView",
            getAccess: "comboval/getAccess"
        }
    },
    lockAble: false,
    clazzName: "ComboVal",
    ignoreToClear: []
};
var initAutoCompletes = function () {
    const parentAutocomplete = new Autocomplete();
    const defaultHandler = new Handler();
    defaultHandler.complete = () => { };
    parentAutocomplete.init({
        element: $("#parent"),
        placeholder: "parentPlaceholder",
        type: ENVIRONMENT.Autocomplete.TYPE.DYNAMIC,
        dropdownCssClass: "form-control-sm floating",
        handler: defaultHandler,
        url: localVariables.URLs.services.autocomplete,
        dynamicConfig: ComboValAutocomplete.dynamicConfig
    });
};
var accessHandler = new Handler();
accessHandler.beforeSend = () => { };
accessHandler.complete = () => { };
accessHandler.success = function (result) {
    if (result.result) {
        $("#adminOnlyDiv").removeClass("d-none");
    } else {
        $("#adminOnlyDiv").addClass("d-none");
    }
};
$(document).ready(function () {
    Storage.setPageNeedLogin(true);
    if (Storage.isPageNeedLogin() && Storage.getUserInfo() === null) {
        SSO.init();
    } else {
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
        const dataWhere = new Wheres();
        dataWhere.add("ticket", Storage.getUserInfo().ticket, ENVIRONMENT.Condition.EQUAL);
        Api.post({ url: URLs.services.getAccess, data: dataWhere, handler: accessHandler });
    }
});