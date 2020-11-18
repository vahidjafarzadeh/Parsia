'use strict';
var localVariables = {
    lockAble: false,
    clazzName: "Organization",
    URLs: {
        services: {
            showRow: 'organization/showRow',
            save: 'organization/save',
            autocompleteView: "organization/autocompleteView"
        }
    },
    Config: {
        CKEditor: {
            aboutUs: {
                language: 'fa',
                uiColor: '#f5f5f5',
                height: 500
            }
        }
    }
};
let localInitListeners = () => {

}
let localInitViews = function () {

};
var initAutoCompletes = function () {
    //وضعیت سازمان
    let orgStatus = new Autocomplete();
    let whereOrgStatus = new Wheres();
    whereOrgStatus.add('parentCode', 'ORGANIZATION_STATUS', ENVIRONMENT.Condition.EQUAL);
    orgStatus.init({
        element: $('#organizationStatus'),
        staticKey: 'organizationOrgStatuses',
        placeholder: 'orgStatus',
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        dropdownCssClass: 'form-control-sm floating',
        where: whereOrgStatus,
        searchColumn: 'name'
    });
    //سازمان والد
    let parentOrganization = new Autocomplete();
    parentOrganization.init({
        element: $('#parent'),
        placeholder: "parentOrganizationPlaceholder",
        type: ENVIRONMENT.Autocomplete.TYPE.DYNAMIC,
        url: localVariables.URLs.services.autocompleteView,
        dynamicConfig: OrganizationAutocomplete.dynamicConfig,
        dropdownCssClass: 'form-control-sm floating'
    });
    //نوع سازمان
    let orgType = new Autocomplete();
    let whereOrgType = new Wheres();
    whereOrgType.add('parentCode', 'ORGANIZATION_TYPE', ENVIRONMENT.Condition.EQUAL);
    orgType.init({
        element: $('#organizationType'),
        staticKey: 'organizationType',
        placeholder: 'type',
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        dropdownCssClass: 'form-control-sm floating',
        where: whereOrgType,
        searchColumn: 'name'
    });
    //درجه سازمان
    let orgGrade = new Autocomplete();
    let whereOrgGrade = new Wheres();
    whereOrgGrade.add('parentCode', 'ORGANIZATION_GRADE', ENVIRONMENT.Condition.EQUAL);
    orgGrade.init({
        element: $('#organizationGrade'),
        staticKey: 'organizationOrgGrade',
        placeholder: 'orgGrade',
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        dropdownCssClass: 'form-control-sm floating',
        where: whereOrgGrade,
        searchColumn: 'name'
    });
    //نوع مالکیت
    let ownershipType = new Autocomplete();
    let whereOwnershipType = new Wheres();
    whereOwnershipType.add('parentCode', 'ORGANIZATION_OWNERSHIP_TYPE', ENVIRONMENT.Condition.EQUAL);
    ownershipType.init({
        element: $('#organizationOwnershipType'),
        staticKey: 'organizationOwnershipTypes',
        placeholder: 'ownershipType',
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        dropdownCssClass: 'form-control-sm floating',
        where: whereOwnershipType,
        searchColumn: 'name'
    });
    //نوع مسیر
    let roadType = new Autocomplete();
    let whereRoadType = new Wheres();
    whereRoadType.add('parentCode', 'ORGANIZATION_ROAD_TYPE', ENVIRONMENT.Condition.EQUAL);
    roadType.init({
        element: $('#organizationRoadType'),
        staticKey: 'organizationRoadType',
        placeholder: 'roadType',
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        dropdownCssClass: 'form-control-sm floating',
        where: whereRoadType,
        searchColumn: 'name'
    });
    //استان
    let provinceAutocomplete = new Autocomplete();
    provinceAutocomplete.init({
        element: $('#province'),
        placeholder: "province",
        type: ENVIRONMENT.Autocomplete.TYPE.DYNAMIC,
        url: 'location/autocompleteView',
        dynamicConfig: LocationAutocomplete.dynamicConfig,
        dropdownCssClass: 'form-control-sm floating'
    });
    //شهر
    let cityAutocomplete = new Autocomplete();
    cityAutocomplete.init({
        element: $('#city'),
        placeholder: "city",
        type: ENVIRONMENT.Autocomplete.TYPE.DYNAMIC,
        url: 'location/autocompleteView',
        dynamicConfig: LocationAutocomplete.dynamicConfig,
        dropdownCssClass: 'form-control-sm floating'
    });

};
$(document).ready(function () {
    Storage.setPageNeedLogin(true);
    if (Storage.isPageNeedLogin() && Storage.getUserInfo() === null) {
        SSO.init();
    } else {
        initLocalVariables(localVariables);
        localInitViews();
        initAutoCompletes();
        localInitListeners();
        onPageReady();
    }
});
