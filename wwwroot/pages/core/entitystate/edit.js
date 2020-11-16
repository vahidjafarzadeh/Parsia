'use strict';

var localVariables = {
    URLs: {
        services: {
            showRow: 'entitystate/showRow',
            save: 'entitystate/save'
        }
    },
    Config: {
        CKEditor: {}
    },

    ignoreToClear: []
};

var initAutoCompletes = function () {

    const organizationAutocompleteConf = {
        element: $('#501883031743700'),
        placeholder: "organizationPlaceholder",
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        dropdownCssClass: 'form-control-sm floating'
    };

    organizationAutocompleteConf.staticKey = 'coEntityStateOrganization'; // Process Name + Field Name
    organizationAutocompleteConf.autocompleteCacheTimeout = 120000;
    organizationAutocompleteConf.url = 'organization/autocompleteView';
    const organizationAutocomplete = new Autocomplete();
    organizationAutocomplete.init(organizationAutocompleteConf);


    const userRoleAutocompleteConf = {
        element: $('#501883033824700'),
        placeholder: "userRolePlaceholder",
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        dropdownCssClass: 'form-control-sm floating'
    };

    userRoleAutocompleteConf.staticKey = 'coEntityStateUserRole'; // Process Name + Field Name
    userRoleAutocompleteConf.autocompleteCacheTimeout = 120000;
    userRoleAutocompleteConf.url = 'role/autocompleteView';
    const userRoleAutocomplete = new Autocomplete();
    userRoleAutocomplete.init(userRoleAutocompleteConf);


    const userLockerAutocompleteConf = {
        element: $('#501883033880700'),
        placeholder: "userLockerPlaceholder",
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        dropdownCssClass: 'form-control-sm floating'
    };

    userLockerAutocompleteConf.staticKey = 'coEntityStateUserLocker'; // Process Name + Field Name
    userLockerAutocompleteConf.autocompleteCacheTimeout = 120000;
    userLockerAutocompleteConf.url = 'user/autocompleteView';
    const userLockerAutocomplete = new Autocomplete();
    userLockerAutocomplete.init(userLockerAutocompleteConf);

};

$(document).ready(function () {

    initLocalVariables(localVariables);
    initAutoCompletes();
    onPageReady();

});