'use strict';
var advanceSearchItems;
function setAdvanceSearchItems() {
    advanceSearchItems = [
    ];
}
var localVariables;
function setLocalVariables() {
    localVariables = {
        URLs: {
            services: {
                gridView: 'action/gridView',
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
    localPageReady();

});