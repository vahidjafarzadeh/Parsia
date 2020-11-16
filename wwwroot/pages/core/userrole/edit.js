'use strict';
var localVariables = {
    URLs: {
        services: {
            showRow: 'userRole/showRow',
            save: 'userRole/save',
            autocompleteView: "userRole/autocompleteView",
            autocompleteViewRole: "role/autocompleteView",
            autocompleteViewUser: "user/autocompleteView",
            autocompleteViewOrganization: "organization/autocompleteView",
        }
    }
};
var initAutoCompletes = function () {
    //organization
    let organization = new Autocomplete();
    organization.init({
        element: $('#organization'),
        placeholder: "organization",
        type: ENVIRONMENT.Autocomplete.TYPE.DYNAMIC,
        url: localVariables.URLs.services.autocompleteViewOrganization,
        dynamicConfig: OrganizationAutocomplete.dynamicConfig,
        dropdownCssClass: 'form-control-sm floating'
    });
    //user
    let user = new Autocomplete();
    user.init({
        element: $('#user'),
        placeholder: "user",
        type: ENVIRONMENT.Autocomplete.TYPE.DYNAMIC,
        url: localVariables.URLs.services.autocompleteViewUser,
        dynamicConfig: UserAutocomplete.dynamicConfig,
        dropdownCssClass: 'form-control-sm floating'
    });
    //role
    let role = new Autocomplete();
    role.init({
        element: $('#role'),
        placeholder: "role",
        type: ENVIRONMENT.Autocomplete.TYPE.DYNAMIC,
        url: localVariables.URLs.services.autocompleteViewRole,
        dynamicConfig: RoleAutocomplete.dynamicConfig,
        dropdownCssClass: 'form-control-sm floating'
    });
};
$(document).ready(function () {
    initLocalVariables(localVariables);
    initAutoCompletes();
    onPageReady();
});