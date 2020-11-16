'use strict';
var localVariables;
function setLocalVariables() {
    localVariables = {
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
    localPageReady();

});