'use strict';
var localVariables;
function setLocalVariables(){
    localVariables = {
        clazzName: "UseCase",
        URLs: {
            services: {
                gridView: 'usecase/gridView',
                delete: 'usecase/delete'
            }
        }
    };
}
var initAutoCompletes = function () {
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
function localPageReady() {
    initAutoCompletes();
    setLocalVariables();
    initLocalVariables(localVariables);
    onPageReady();
}
$(document).ready(function () {
    Storage.setPageNeedLogin(true);
    if (Storage.isPageNeedLogin() && Storage.getUserInfo() === null) {
        SSO.init();
    } else {
        localPageReady();
    }
});