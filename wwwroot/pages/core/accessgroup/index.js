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
    localPageReady();
});