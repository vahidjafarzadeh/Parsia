'use strict';
var localVariables;
function setLocalVariables(){
    localVariables = {
        clazzName: "Role",
        URLs: {
            services: {
                gridView: 'role/gridView',
                delete: 'role/delete'
            }
        }
    };
}
function localPageReady(){
    setLocalVariables();
    initLocalVariables(localVariables);
    onPageReady();
}
const addTimeText =(res)=>{
    return res + " دقیقه";
}

$(document).ready(function () {
    Storage.setPageNeedLogin(true);
    if (Storage.isPageNeedLogin() && Storage.getUserInfo() === null) {
        SSO.init();
    } else {
        localPageReady();
    }
});