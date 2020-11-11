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
    ignoreToClear: []
};
var initAutoCompletes = function() {
    const parentAutocomplete = new Autocomplete();
    const defaultHandler = new Handler();
    defaultHandler.complete = () => {};
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
accessHandler.beforeSend = () => {};
accessHandler.complete = () => {};
accessHandler.success = function(result) {
    if (result.result) {
        $("#adminOnlyDiv").removeClass("d-none");
    } else {
        $("#adminOnlyDiv").addClass("d-none");
    }
};


$(document).ready(function() {
    initLocalVariables(localVariables);
    initAutoCompletes();
    onPageReady();
    const dataWhere = new Wheres();
    dataWhere.add("ticket", Storage.getUserInfo().ticket, ENVIRONMENT.Condition.EQUAL);
    Api.post({ url: URLs.services.getAccess, data: dataWhere, handler: accessHandler });

});