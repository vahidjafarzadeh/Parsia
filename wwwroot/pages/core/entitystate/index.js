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
        , clazzName: 'CoEntityState'
    };
}


function localPageReady() {
    setLocalVariables();
    initLocalVariables(localVariables);
    onPageReady();
}
$(document).ready(function () {
    localPageReady();
});