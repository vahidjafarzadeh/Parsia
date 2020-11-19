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
            login: "account/login",
            register: "account/register",
            activeCode: "account/activeCode",
            changePassword: "account/changePassword",
            recovery: "account/recovery",
            selectUserRole: "account/selectUserRole"
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
    Api.post({ url: localVariable.urls.selectUserRole, data: userInfo, handler: handler, withoutTicket: true });
}
const prepare = () => {
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
    $("#code").on("keyup", (e) => {
        if ($("#code").val().length >= 6) {
            $("#code").val($("#code").val().substring(0, 6));
        }
    });
    $("#btn-sms-verify").off("click");
    $("#btn-sms-verify").on("click", (e) => {
        const formData = new FormData();
        let phoneNumber = $("#phoneNumber").val();
        let code = $("#code").val();
        let captcha = $("#captcha").val();
        if (!phoneNumber) {
            errorHandler({ errorDesc: "لطفا شماره تلفن همراه را وارد نمایید" });
        } else if (!code) {
            errorHandler({ errorDesc: "لطفا کد فعال سازی را وارد نمایید" });
        } else if (!captcha) {
            errorHandler({ errorDesc: "لطفا کد امنیتی را وارد نمایید" });
        } else {
            formData.append("phoneNumber", phoneNumber);
            formData.append("code", code);
            formData.append("captcha", captcha);
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
                    showConfirm({
                        title: "پیغام سیستم",
                        body: "فعال سازی اکانت با موفقیت انجام شد",
                        confirmButton: {
                            hidden: true
                        },
                        declineButton: {
                            text: "بستن"
                        }
                    });
                    setTimeout(() => {
                        window.location.href = "/login";
                    }, 3000);
                } else {
                    errorHandler(data);
                }
            };
            Api.postForm({ url: localVariable.urls.activeCode, formData: formData, handler: handler });
        }

    });
    $("#btn-phone-recovery").on("click", (e) => {
        window.location.href = "/recovery-password?code=" + $("#codes").val();
    });
    Captcha("container-captcha");
    loading.fadeOut("slow");
}
$(document).ready(function () {
    initialVariable();
    prepare();
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
                        $.each(data.result, (i, v) => {
                            let div = $("<div/>", { class: "col-12 col-md-12" });
                            let p = $("<p/>",
                                {
                                    class: "btn btn-sm f-w o-h material indigo mt-2 mb-2 role-manager",
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
                            p.append(icon, org, role);
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
    $("#btn-register").on("click", (e) => {
        const formData = new FormData();
        let username = $("#username").val();
        let password = $("#password").val();
        let confirmPassword = $("#confirmPassword").val();
        let law = $("#law").prop("checked");
        let captcha = $("#captcha").val();
        if (!username) {
            errorHandler({ errorDesc: "لطفا نام کاربری را وارد نمایید" });
        } else if (!password) {
            errorHandler({ errorDesc: "لطفا کلمه عبور را وارد نمایید" });
        } else if (!confirmPassword) {
            errorHandler({ errorDesc: "لطفا تکرار کلمه عبور را وارد نمایید" });
        } else if (password !== confirmPassword) {
            errorHandler({ errorDesc: "کلمه عبور با تکرار کلمه عبور مغایرت دارد" });
        } else if (!law) {
            errorHandler({ errorDesc: "لطفا موافقت خود مبنی بر پذیرش قوانین سایت اعلام نمایید" });
        } else if (!captcha) {
            errorHandler({ errorDesc: "لطفا کد امنیتی را وارد نمایید" });
        } else {
            formData.append("username", username);
            formData.append("password", password);
            formData.append("confirmPassword", confirmPassword);
            formData.append("law", law);
            formData.append("captcha", captcha);
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
                    if (data.result === "phone") {
                        showConfirm({
                            title: "پیغام سیستم",
                            body: "عضویت با موفقیت انجام شد . برای فعال سازی اکانت خود کد ارسالی به شماره همراه خود را وارد نمایید",
                            confirmButton: {
                                hidden: true
                            },
                            declineButton: {
                                text: "بستن"
                            }
                        });
                        setTimeout(() => {
                            $.get("/active-phone", (data) => {
                                var phone = $("#username").val();
                                $(".login-box").remove();
                                $("body").append(data);
                                setTimeout(() => {
                                    $("#phoneNumber").val(phone);
                                    prepare();
                                }, 100);
                            });
                        }, 3000);
                    }
                    else {
                        showConfirm({
                            title: "پیغام سیستم",
                            body:"عضویت با موفقیت انجام شد .لطفا برای فعال سازی اکانت خود روی لینک ارسالی به ایمیل خود کلیک نمایید",
                            confirmButton: {
                                hidden: true
                            },
                            declineButton: {
                                text: "بستن"
                            }
                        });
                        setTimeout(() => {
                            window.location.href = "/login";
                        },3000);
                    }
                } else {
                    errorHandler(data);
                }
            };
            Api.postForm({ url: localVariable.urls.register, formData: formData, handler: handler });
        }

    });
    $("#btn-active-code").on("click", (e) => {
        $.get("/active-phone", (data) => {
            var phone = $("#username").val();
            $(".login-box").remove();
            $("body").append(data);
            setTimeout(() => {
                $("#phoneNumber").val(phone);
                prepare();
            }, 100);
        });
    });
    $("#btn-recover-tow").on("click", (e) => {
        const formData = new FormData();
        let username = $("#username").val();
        let captcha = $("#captcha").val();
        if (!username) {
            errorHandler({ errorDesc: "لطفا نام کاربری را وارد نمایید" });
        } else if (!captcha) {
            errorHandler({ errorDesc: "لطفا کد امنیتی را وارد نمایید" });
        } else {
            formData.append("username", username);
            formData.append("captcha", captcha);
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
                    if (data.result === "phone") {
                        showConfirm({
                            body: "کد بازیابی با موفقیت ارسال شد . کد فعال سازی ارسالی به تلفن همراه خود را وارد نمایید ",
                            title: "پیغام سیستم",
                            confirmButton: {
                                hidden: true
                            },
                            declineButton: {
                                text: "بستن"
                            }
                        });
                        setTimeout(() => {
                            $.get("/recover-phone", (data) => {
                                $(".login-box").remove();
                                $("body").append(data);
                                prepare();
                            });
                        }, 3000);
                    }
                    else {
                        showConfirm({
                            body:"کد فعال سازی با موفقیت ارسال شد. با کلیک بر روی لینک ارسالی به ایمیل خود کلمه عبور خود را بازیابی کنید",
                            title: "پیغام سیستم",
                            confirmButton: {
                                hidden: true
                            },
                            declineButton: {
                                text: "بستن"
                            }
                        });
                    }
                } else {
                    errorHandler(data);
                }
            };
            Api.postForm({ url: localVariable.urls.recovery, formData: formData, handler: handler });
        }

    });
    $("#btn-reload-password").on("click", (e) => {
        const formData = new FormData();
        let newPassword = $("#new-password").val();
        let confirmPassword = $("#confirm-password").val();
        let code = $("#code").val();
        let captcha = $("#captcha").val();
        if (!newPassword) {
            errorHandler({ errorDesc: "لطفا کلمه عبور جدید را وارد نمایید" });
        }
        else if (!confirmPassword) {
            errorHandler({ errorDesc: "لطفا تکرار کلمه عبور جدید را وارد نمایید" });
        } else if (!code) {
            errorHandler({ errorDesc: "لطفا کد فعال سازی را وارد نمایید" });
        }
        else if (newPassword !== confirmPassword) {
            errorHandler({ errorDesc: "کلمه عبور جدید با تکرار کلمه عبور جدید مغایرت دارد" });
        }
        else if (!captcha) {
            errorHandler({ errorDesc: "لطفا کد امنیتی را وارد نمایید" });
        } else {
            formData.append("new-password", newPassword);
            formData.append("confirm-password", confirmPassword);
            formData.append("captcha", captcha);
            formData.append("code", code);
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
                    showConfirm({
                        body: "کلمه عبور با موفقیت تغییر یافت . هم اکنون می توانید وارد سایت شوید",
                        title: "پیغام سیستم",
                        confirmButton: {
                            hidden: true
                        },
                        declineButton: {
                            text: "بستن"
                        }
                    });
                    setTimeout(() => {
                        window.location.href = "/login";
                    }, 3000);
                } else {
                    errorHandler(data);
                }
            };
            Api.postForm({ url: localVariable.urls.changePassword, formData: formData, handler: handler });
        }

    });
   
});