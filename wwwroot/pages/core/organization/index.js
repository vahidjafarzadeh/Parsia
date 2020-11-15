'use strict';
var localVariables;
function setLocalVariables() {
    localVariables = {
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
    localPageReady();
});