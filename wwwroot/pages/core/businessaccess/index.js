'use strict';

var localVariables;

function setLocalVariables() {
    localVariables = {
        URLs: {
            services: {
                gridView: 'businessaccess/gridView',
                delete: 'cobusinessaccess/delete'
            }

        }
        , clazzName: 'CoBusinessAccess'
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