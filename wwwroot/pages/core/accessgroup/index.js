"use strict";
var localVariables;
function setLocalVariables() {
    localVariables = {
        URLs: {
            services: {
                gridView: "accessGroup/gridView",
                delete: "accessGroup/delete"
            }
        },
        clazzName: "AccessGroup"
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