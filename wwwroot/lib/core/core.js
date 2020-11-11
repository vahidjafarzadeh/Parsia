var userInfo = Storage.getUserInfo();

/** retURL : آدرسی که بعداز لاگین در سرور اعتبار سنجی باید نمایش داده شود*/
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
    case ENVIRONMENT.ErrorCode.BUSINESS_MESSAGE:
        switch (response.errorDesc) {
        case "NO_DATA":
            break;
        default:
            if (top.showConfirm) {
                setTimeout(function() {
                        top.showConfirm({
                            title: "خطا در ذخیره سازی / بروزرسانی",
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
            }
            break;
        }
        break;
    case ENVIRONMENT.ErrorCode.USER_EXPIRED:
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
    console.info("%c•-• %cDevelopment Team %c•-•",
        "color: #FF9F1C; font-size: 2.7rem; font-weight: bold; font-family: 'Sans-Serif'",
        "color: #FFBC42; font-size: 2.7rem; font-weight: bold; font-family: 'Sans-Serif'",
        "color: #FF9F1C; font-size: 2.7rem; font-weight: bold; font-family: 'Sans-Serif'");

    console.info(" %c• %cMahdi Golnari              %c---   %cProject Manager      %c---    %cGolnari@gmail.com",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F45866; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F49390; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #A2666F; font-weight: bold; font-size: 1.2rem;");

    console.info(
        " %c• %cFarshad Kazemi             %c---   %cSoftware Architect   %c---    %cFarshad.Kazemi70@gmail.com",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F45866; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F49390; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #A2666F; font-weight: bold; font-size: 1.2rem;");

    console.info(
        " %c• %cVahid Jafarzadeh           %c---   %cCore Developer       %c---    %cVahidJafarzadeh1373@outlook.com",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F45866; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F49390; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #A2666F; font-weight: bold; font-size: 1.2rem;");

    console.info(" %c• %cAli Salimi                 %c---   %cCore Developer       %c---    %cAli.Salimi.ir@live.com",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F45866; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F49390; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #A2666F; font-weight: bold; font-size: 1.2rem;");

    console.info(
        " %c• %cMahdi Khosravan            %c---   %cCore Developer       %c---    %cMehdi.Khosravan@gmail.com",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F45866; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F49390; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #A2666F; font-weight: bold; font-size: 1.2rem;");

    console.info(
        " %c• %cMojtaba Abdolian           %c---   %cDeveloper            %c---    %cMojtaba_Abdolian@yahoo.com",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F45866; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F49390; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #A2666F; font-weight: bold; font-size: 1.2rem;");

    console.info(" %c• %cMahdi Taghizadeh           %c---   %cDeveloper            %c---    %cDelphi.MT@gmail.com",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F45866; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F49390; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #A2666F; font-weight: bold; font-size: 1.2rem;");

    console.info(
        " %c• %cMohammad Farahani          %c---   %cDeveloper            %c---    %cMohammad.Farahani62@outlook.com",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F45866; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F49390; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #A2666F; font-weight: bold; font-size: 1.2rem;");

    console.info(" %c• %cMohammad Javad Fani        %c---   %cDeveloper            %c---    %cMfs0business@gmail.com",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F45866; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F49390; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #A2666F; font-weight: bold; font-size: 1.2rem;");

    console.info(
        " %c• %cMohammad Amin Mahmoudian   %c---   %cDeveloper            %c---    %cm.a.m.mahmoodian@gmail.com",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F45866; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F49390; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #A2666F; font-weight: bold; font-size: 1.2rem;");
    /*



    console.log(" %c• %cHamid Bayat                %c---   %cDeveloper            %c---    %cFarshad.Kazemi70@gmail.com",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F45866; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F49390; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #A2666F; font-weight: bold; font-size: 1.2rem;");*/

    console.info(" %c• %cMohaddeseh Abbasi          %c---   %cDeveloper            %c---    %cMohaddese136@gmail.com",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F45866; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F49390; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #A2666F; font-weight: bold; font-size: 1.2rem;");

    console.info(" %c• %cFatemeh Faraji             %c---   %cDeveloper            %c---    %cEnffaraji@gmail.com",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F45866; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #F49390; font-weight: bold; font-size: 1.2rem;",
        "color: #FF9F1C; font-weight: bold; font-size: 1.2rem;",
        "color: #A2666F; font-weight: bold; font-size: 1.2rem;");
};
//delete window.console;