"use strict";

var localVariables;

function setLocalVariables() {
    localVariables = {
        URLs: {
            services: {
                gridView: 'user/gridView',
                delete: 'user/delete'
            }
        },
        clazzName: "Users"
    };
}
function localPageReady() {
    setLocalVariables();
    initLocalVariables(localVariables);
    onPageReady();
}
const changeDate = (res) => {
    if (res) {
        return new persianDate(Number(res)).format("dddd YYYY/MM/DD HH:mm:ss");
    } else {
        return "-";
    }
}
$(document).ready(function () {
    Storage.setPageNeedLogin(true);
    if (Storage.isPageNeedLogin() && Storage.getUserInfo() === null) {
        SSO.init();
    } else {
        localPageReady();
    }
});
