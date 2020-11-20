var userInfo = Storage.getUserInfo();
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
    switch (response.errorCode) {
    case 2:
        if (top.showConfirm) {
            top.showConfirm({
                body: "زمان استفاده از حساب کاربری شما به پایان رسیده است. آیا مایلید دوباره وارد شوید؟ ",
                confirmButton: {
                    onClick: function(e) {
                        showLoginPage();
                    }
                }
            });
        } else {
            showLoginPage();
        }
        break;
    default:
        if (top.showConfirm) {
            setTimeout(function() {
                    top.showConfirm({
                        title: "خطا",
                        body: response.errorDesc,
                        confirmButton: {
                            hidden: true
                        },
                        declineButton: {
                            text: GeneralBundle.$close
                        }
                    });
                },
                450);
        } else {
            alert(response.errorDesc);
        }
        break;
    }
}
const changeButtonState = (params) => {
    // state = 0 => Reset
    // state = 1 => Do something
    let { button, state } = params;
    button = button instanceof jQuery ? button : $(button);
    if (state === 1) {
        button.attr("disabled", true).css({ pointerEvents: "none" });
        button.addClass("loading");
    } else {
        if (state === 2) {
            button.removeAttr("disabled").css({ pointerEvents: "" });
            button.removeClass("loading");
            button.addClass("failed");
        } else if (state === 0) {
            button.removeAttr("disabled").css({ pointerEvents: "" });
            button.removeClass("loading");
            button.addClass("finished");
        }
    }
};
console.developers = () => {
    console.info(
        " %c• %cVahid Jafarzadeh           %c---   %cCore Developer       %c---    %cVahidJafarzadeh1373@outlook.com",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F45866; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F49390; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #A2666F; font-weight: bold; font-size: 1.2rem;");
};