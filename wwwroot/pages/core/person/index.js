"use strict";

var localVariables;

function setLocalVariables() {
    localVariables = {
        URLs: {
            services: {
                gridView: 'person/gridView',
                delete: 'person/delete'
            }
        },
        clazzName: "Person"
    };
}
const changeDate = (res) => {
    if (res) {
        return new persianDate(Number(res)).format("YYYY/MM/DD");
    } else {
        return "-";
    }
}
var initAutoCompletes = function() {

};

function localPageReady() {
    setLocalVariables();
    initLocalVariables(localVariables);
    initAutoCompletes();
    onPageReady();
}

$(document).ready(function() {
    localPageReady();
});