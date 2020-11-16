'use strict';
var useCaseAutoComplete, targetAutoComplete, menuAutoComplete;
var localVariables = {
    URLs: {
        services: {
            showRow: 'menu/showRow',
            save: 'menu/save'
        }
    }
};
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
    autoTargetWhere.add('parentCode', 'OPEN_WINDOW', ENVIRONMENT.Condition.EQUAL);
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
$(document).ready(function () {
    initLocalVariables(localVariables);
    initAutoCompletes();
    onPageReady();
    $("#icon").on('keyup', function () {
        let words = $(this).val();
        $(this).val(words.replace(/"/g, "'"));
    });
});
