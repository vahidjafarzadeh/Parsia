'use strict';
var useCaseAutoComplete, targetAutoComplete, menuAutoComplete ;
var initAutoCompletes = function () {
    useCaseAutoComplete = $('#use-case');
    targetAutoComplete = $('#target');
    menuAutoComplete = $('#parent-menu');



    var autoUsecase = new Autocomplete();
    autoUsecase.init({
        element: useCaseAutoComplete,
        placeholder: "useCase",
        type: ENVIRONMENT.Autocomplete.TYPE.DYNAMIC,
        dropdownCssClass: 'form-control-sm',
        url: 'usecase/autocompleteView',
        dynamicConfig: UsecaseAutocomplete.dynamicConfig
    });

    var autoTarget = new Autocomplete();
    var autoTargetWhere = new Wheres();
    autoTargetWhere.add('val', 'OPEN_WINDOW', ENVIRONMENT.Condition.CONTAINS);
    autoTarget.init({
        element: targetAutoComplete,
        placeholder: "target",
        staticKey: 'combovalTarget',
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        where: autoTargetWhere,
        dropdownCssClass: 'form-control-sm'
    });

    var autoParent = new Autocomplete();
    autoParent.init({
        element: menuAutoComplete,
        placeholder: "parent",
        type: ENVIRONMENT.Autocomplete.TYPE.DYNAMIC,
        dropdownCssClass: 'form-control-sm',
        url: 'menu/autocompleteView',
        dynamicConfig: MenuAutocomplete.dynamicConfig
    });
};
var localVariables;
function setLocalVariables() {
    localVariables = {
        URLs: {
            services: {
                gridView: 'menu/gridView',
                delete: 'menu/delete'
            }
        }
    };
}
function localPageReady() {
    setLocalVariables();
    initLocalVariables(localVariables);
    onPageReady();
}
$(document).ready(function () {
    initAutoCompletes();
    localPageReady();
});