﻿'use strict';
var localVariables = {
    lockAble: false,
    clazzName: "Location",
    URLs: {
        services: {
            showRow: 'location/showRow',
            save: 'location/save'
        }
    }
};
var initAutoCompletes = function () {
    var newsGroupAutocomplete = new Autocomplete();
    newsGroupAutocomplete.init({
        element: $('#parent'),
        placeholder: "name",
        type: ENVIRONMENT.Autocomplete.TYPE.DYNAMIC,
        url: 'location/autocompleteView',
        dynamicConfig: LocationAutocomplete.dynamicConfig,
        dropdownCssClass: 'form-control-sm'
    });
    let typeComplete = new Autocomplete();
    let where = new Wheres();
    where.add('parentCode', 'LOCATION', ENVIRONMENT.Condition.EQUAL);
    typeComplete.init({
        element: $('#type'),
        staticKey: 'locationTypeName',
        placeholder: 'locationTypeNamePlaceholder',
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        dropdownCssClass: 'form-control-sm floating',
        where: where,
        searchColumn: 'name'
    });
};
$(document).ready(function () {
    Storage.setPageNeedLogin(true);
    if (Storage.isPageNeedLogin() && Storage.getUserInfo() === null) {
        SSO.init();
    } else {
        initLocalVariables(localVariables);
        initAutoCompletes();
        onPageReady();
    }
});
