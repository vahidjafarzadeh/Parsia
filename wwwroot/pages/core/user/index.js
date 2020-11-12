"use strict";

var localVariables;

function setLocalVariables() {
    localVariables = {
        URLs: {
            services: {
                gridView: "comboval/gridView",
                delete: "comboval/delete"
            }
        },
        clazzName: "ComboVal"
    };
}

var initAutoCompletes = function() {

    const parentAutocomplete = new Autocomplete();
    parentAutocomplete.init({
        element: $("#parent"),
        placeholder: "parentPlaceholder",
        type: ENVIRONMENT.Autocomplete.TYPE.DYNAMIC,
        dropdownCssClass: "form-control-sm floating",
        url: "comboval/autocompleteView",
        dynamicConfig: ComboValAutocomplete.dynamicConfig
    });
};

function localPageReady() {
    setLocalVariables();
    initLocalVariables(localVariables);
    initAutoCompletes();
    onPageReady();
}

$(document).ready(function() {
    localPageReady();
});