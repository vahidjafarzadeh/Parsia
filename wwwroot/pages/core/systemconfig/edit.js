'use strict';
let checkboxContainer;
var localVariables = {
    URLs: {
        services: {
            showRow: 'systemConfig/showRow',
            save: 'systemConfig/save'
        }
    }
};
$(document).ready(function () {
    Storage.setPageNeedLogin(true);
    if (Storage.isPageNeedLogin() && Storage.getUserInfo() === null) {
        SSO.init();
    } else {
        onPageReady();
    }
});
