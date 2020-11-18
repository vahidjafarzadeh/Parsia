'use strict';

var localVariables;
function setLocalVariables() {
    localVariables = {
        URLs: {
            services: {
                gridView: 'entitystate/gridView',
                delete: 'entitystate/delete'
            }

        }
        , clazzName: 'EntityState'
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