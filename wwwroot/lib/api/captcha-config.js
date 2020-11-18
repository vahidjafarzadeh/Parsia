var getCaptcha = function (hand) {
    $.get("/service/captcha/get", function (res) {
        hand.success(res);
    });
};
var Captcha = function (target) {
    var handler = new Handler();
    handler.success = function (data) {
        if (data.done) {
            var captchaContainer = $("<div/>", { "class": "captcha-container", id: "captchaContainer" });
            var img = $("<img/>", { id:"captcha-image", src: "data:image/png;base64," + data.result });
            var captchaRefresh = $("<span/>", {
                "html": `<i class="fas fa-redo-alt"></i>`, click: function () {
                    handler.success = function (data) {
                        if (data.done) {
                            $("#captcha-image").remove();
                            var img = $("<img/>", { id: "captcha-image", src: "data:image/png;base64," + data.result });
                            $("#captchaContainer").append(img);
                        } else {
                            handler.configError(data);
                        }
                    };
                    getCaptcha(handler);
                }
            });
            captchaContainer.append(captchaRefresh);
            captchaContainer.append(img);
            $("#" + target).append(captchaContainer);
        }
        else {
            alert(data.erroCode);
        }

    };
    handler.beforeSend = function () { };
    handler.complete = function () { };
    getCaptcha(handler);
};