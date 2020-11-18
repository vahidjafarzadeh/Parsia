var loading, localVariable, modalInstance, loadingModal;
function showConfirm(config) {
    modalInstance.showConfirm(config);
}
function hideConfirm() {
    modalInstance.hideConfirm();
};
function showLoginPage(retURL) {
    if (top.login) {
        const externalID = Util.urlParameter("externalID");
        top.login(externalID);
    } else {
        top.location.href =
            AUTHENTICATION_SERVER +
            "login/?" +
            new Date().getTime() +
            "&r=" +
            window.btoa(retURL !== null ? retURL : top.location.href); //encode
    }
}
function errorHandler(response) {
    $(".captcha-container").find("span").click();
    showConfirm({
        title: "خطا",
        body: response.errorDesc,
        confirmButton: {
            hidden: true
        },
        declineButton: {
            text: "بستن"
        }
    });
}
function showLoading() {
    try {
        loadingModal.modal('show');
    } catch (e) {
        console.log('ShowLoading failed. loadingModal in descktop-mockup/js/scritp.js is Undefined!');
    }
}
function hideLoading() {
    loadingModal.modal('hide');

}
var initialVariable = () => {
    loading = $(".page-loader-wrapper");
    loadingModal = $('#loading-modal');
    modalInstance = Modal.init();
    localVariable = {
        urls: {
            login: "account/Login",
            selectUserRole:"account/selectUserRole"
        }
    }
}
const GetSecurityManager = (userInfo) => {
    var handler = new Handler();
    handler.beforeSend = () => {
    }
    handler.complete = () => {
    }
    handler.success = (data) => {
        if (data.done) {
            Storage.setUserInfo(data.result);
            window.location.href = "/";
        } else {
            errorHandler(data);
        }
    }
    Api.post({ url: localVariable.urls.selectUserRole, data: userInfo, handler: handler, withoutTicket:true });
}
$(document).ready(function () {
    initialVariable();
    $(".float").on("focus", (e) => {
        if (!$(e.currentTarget).hasClass("floating-mode")) {
            $(e.currentTarget).addClass("floating-mode");
        }
    });
    $(".float").on("blur", (e) => {
        if ($(e.currentTarget).hasClass("floating-mode") && !$(e.currentTarget).val()) {
            $(e.currentTarget).removeClass("floating-mode");
        }
    });
    $.ripple(".material", {
        debug: false,
        on: "mousedown",
        opacity: 0.4,
        color: "auto",
        multi: false,
        duration: 0.7,
        rate: function (pxPerSecond) {
            return pxPerSecond;
        },
        easing: "linear"
    });
    $("#captcha").on("keyup", (e) => {
        if ($("#captcha").val().length >= 4) {
            $("#captcha").val($("#captcha").val().substring(0, 4));
        }
    });
    $("#btn-login").on("click", (e) => {
        const formData = new FormData();
        let username = $("#username").val();
        let password = $("#password").val();
        let remember = $("#remember").prop("checked");
        let captcha = $("#captcha").val();
        if (!username) {
            errorHandler({ errorDesc: "لطفا نام کاربری را وارد نمایید" });
        } else if (!password) {
            errorHandler({ errorDesc: "لطفا کلمه عبور را وارد نمایید" });
        } else if (!captcha) {
            errorHandler({ errorDesc: "لطفا کد امنیتی را وارد نمایید" });
        } else {
            formData.append("username", username);
            formData.append("password", password);
            formData.append("captcha", captcha);
            formData.append("remember", remember);
            var handler = new Handler();
            handler.beforeSend = () => {
                showLoading();
            }
            handler.complete = () => {
                setTimeout(() => {
                    loadingModal.modal('hide');
                }, 1000);

            }
            handler.success = (data) => {
                hideLoading();
                if (data.done) {
                    if (data.result.length > 1) {
                        $(".container-role").html("");
                        $.each(data.result, (i,v) => {
                            let div = $("<div/>", { class: "col-12 col-md-12" });
                            let p = $("<p/>",
                                {
                                    class: "btn btn-sm f-w o-h material indigo mt-2 mb-2 role-manager" ,
                                    click: () => {
                                        var userInfo = v;
                                        userInfo.username = username;
                                        userInfo.password = password;
                                        userInfo.remember = remember;
                                        GetSecurityManager(userInfo);
                                    }
                                });
                            let icon = $("<i/>", { class: "fas fa-scroll" });
                            let org = $("<span/>", { class: "org", text: `سازمان ${v.organizationName}` });
                            let role = $("<span/>", { class: "role", text: `نقش ${v.roleName}` });
                            p.append(icon,org,role);
                            div.append(p);
                            $(".container-role").append(div);
                        });
                        $(".logged-box").fadeOut("slow");
                        $(".role-box").fadeIn("slow");
                        
                    } else {
                        var userInfo = data.result[0];
                        userInfo.username = username;
                        userInfo.password = password;
                        userInfo.remember = remember;
                        GetSecurityManager(userInfo);
                    }
                } else {
                    errorHandler(data);
                }
            };
            Api.postForm({ url: localVariable.urls.login, formData: formData, handler: handler });
        }

    });
    Captcha("container-captcha");
    loading.fadeOut("slow");
});