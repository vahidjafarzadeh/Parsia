'use strict';
var localVariables;
function setLocalVariables() {
    localVariables = {
        clazzName: "Organization",
        URLs: {
            services: {
                gridView: 'organization/gridView',
                delete: 'organization/delete'
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
    Storage.setPageNeedLogin(true);
    if (Storage.isPageNeedLogin() && Storage.getUserInfo() === null) {
        SSO.init();
    } else {
        localPageReady();
    }
});