/**
 * Created by Farshad Kazemi on 2/17/2018.
 */
var Handler = (function () {
    var
        beforeSend = function () {
            if(top.showLoading) {
                top.showLoading();
            } else if(this.showPageLoading) {
                showPageLoading();
            }
        },
        success = function (data, textStatus, jqXHR) {
            if (data.done) {
                if(top.hideLoading) {
                    top.hideLoading();
                } else if(hidePageLoading) {
                    hidePageLoading();
                }
                if (data.result) {
                    //fillGrid(data.result);
                }
            } else {
                if(top.hideLoading) {
                    top.hideLoading();
                }
                errorHandler(data);
            }
        },
        complete = function (jqXHR) {},
        error = function (jqXHR) {};

    return {
        beforeSend: beforeSend,
        success: success,
        complete: complete,
        error: error
    }
});