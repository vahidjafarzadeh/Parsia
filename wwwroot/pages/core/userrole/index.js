'use strict';
var localVariables;
function setLocalVariables() {
    localVariables = {
        clazzName: "UserRole",
        URLs: {
            services: {
                gridView: 'userRole/gridView',
                delete: 'userRole/delete'
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