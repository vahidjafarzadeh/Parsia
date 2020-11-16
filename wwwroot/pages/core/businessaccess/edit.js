'use strict';

var localVariables = {
    URLs: {
        services: {
            showRow: 'businessAccess/showRow',
            save: 'businessAccess/save'
        }
    },
    Config: {
        CKEditor: {
            entityIds: {
                language: 'fa',
                uiColor: '#f5f5f5',
                height: 500
            },
        }
    },
    ignoreToClear: ['addCurrentList']
};

var initAutoCompletes = function () {

    const organizationAutocompleteConf = {
        element: $('#1020427237314400'),
        placeholder: "organizationPlaceholder",
        type: ENVIRONMENT.Autocomplete.TYPE.DYNAMIC,
        dropdownCssClass: 'form-control-sm floating'
    };

    organizationAutocompleteConf.dynamicConfig = OrganizationAutocomplete.dynamicConfig;
    organizationAutocompleteConf.url = 'organization/autocompleteView';
    const organizationAutocomplete = new Autocomplete();
    organizationAutocomplete.init(organizationAutocompleteConf);


    const roleAutocompleteConf = {
        element: $('#1020427247106900'),
        placeholder: "rolePlaceholder",
        type: ENVIRONMENT.Autocomplete.TYPE.DYNAMIC,
        dropdownCssClass: 'form-control-sm floating'
    };

    roleAutocompleteConf.dynamicConfig = RoleAutocomplete.dynamicConfig;
    roleAutocompleteConf.url = 'role/autocompleteView';
    const roleAutocomplete = new Autocomplete();
    roleAutocomplete.init(roleAutocompleteConf);

};

$(document).ready(function () {
    initLocalVariables(localVariables);
    initAutoCompletes();
    onPageReady();

});