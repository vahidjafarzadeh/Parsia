'use strict';
var localVariables;
function setLocalVariables() {
    localVariables = {
        URLs: {
            services: {
                gridView: 'location/gridView',
                delete: 'location/delete'
            }
        },
        clazzName: "Location"
    };
}
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
function localPageReady() {
    setLocalVariables();
    initLocalVariables(localVariables);
    onPageReady();
}
$(document).ready(function () {
    initAutoCompletes();
    localPageReady();
});