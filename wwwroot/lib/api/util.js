/**
 * Created by Farshad Kazemi on 2/14/2018.
 */

"use strict";
var Util = (function() {
    /**
     * @param start: number (millisecond)
     * @param end: number (millisecond)
     * @param period: number (millisecond)
     */
    const diffTime = function(start, end, period) {
        return start - end <= period;
    };

    const loadScript = function(src, currentDomain, dataIndex, callback) {
        const tag = document.createElement("script");
        tag.type = "text/javascript";
        tag.src = currentDomain ? Config.APPLICATION_URL + src : src;
        if (callback) {
            tag.onload = callback.apply(null, []);
        }
        if (dataIndex) {
            $(`script[data-index= ${dataIndex}]`).after(tag);
        } else {
            $("body").append(tag);
        }

    };

    const urlParameter = function(name) {
        const results = new RegExp(`[\?&]${name}=([^&#]*)`).exec(window.location.toString());
        if (results === null) {
            return null;
        } else {
            return results[1] || 0;
        }
    };

    const validateForm = function(formObject, selectorListForIgnore) { // TODO CheckBox & Radio & Select Validation
        selectorListForIgnore = selectorListForIgnore ? selectorListForIgnore : [];
        var returnValue = true;
        formObject.find(`.${ENVIRONMENT.CssClass.MANDATORY}`).not(".read-only").each(function() {
            var ctx = $(this);
            if (ctx.data("secure")) {
                return true;
            }
            if (ctx.hasClass(ENVIRONMENT.CssClass.HAS_ERROR)) {
                returnValue = false;
                return false;
            } else {
                var formControl = ctx.find(".form-control");
                var maxLength = formControl.attr("ksun-max-length")
                    ? formControl.attr("ksun-max-length")
                    : Config.INPUT_DEFAULT_MAX_LENGTH;
                var minLength = formControl.attr("ksun-min-length")
                    ? formControl.attr("ksun-min-length")
                    : Config.INPUT_DEFAULT_MIN_LENGTH;
                var type = formControl.attr("ksun-input-type")
                    ? formControl.attr("ksun-input-type")
                    : ENVIRONMENT.InputType.TEXT;
                switch (formControl.get(0).tagName) {
                case "INPUT":
                    if (formControl.get(0).type === "text") {
                        if (formControl.val().length < minLength) {
                            ctx.find(`.${ENVIRONMENT.CssClass.FEED_BACK}`).text(
                                GeneralBundle.$feedbackMinimumLengthPartOne +
                                +minLength +
                                GeneralBundle.$feedbackMinimumLengthPartTwo);
                            ctx.addClass("has-error");
                            returnValue = false;
                            return false;
                        }
                        if (formControl.val().length > maxLength) {
                            ctx.find(`.${ENVIRONMENT.CssClass.FEED_BACK}`).text(
                                GeneralBundle.$feedbackMaximumLengthPartOne +
                                +maxLength +
                                GeneralBundle.$feedbackMaximumLengthPartTwo);
                            ctx.addClass("has-error");
                            returnValue = false;
                            return false;
                        }
                        if (type === ENVIRONMENT.InputType.NUMBER) {
                            if (ctx.hasClass("double")) {
                                var fractionalLength = ctx.attr("ksun-fractional-length");
                            } else {
                                if (formControl.val().length > maxLength) {
                                    ctx.find(`.${ENVIRONMENT.CssClass.FEED_BACK}`).text(
                                        GeneralBundle.$feedbackMaximumLengthPartOne +
                                        +maxLength +
                                        GeneralBundle.$feedbackMaximumLengthPartTwo);
                                    ctx.addClass("has-error");
                                    returnValue = false;
                                    return false;
                                }
                                var regex = /^\d+$/;
                                if (!(regex.test(formControl.val()))) {
                                    ctx.find(`.${ENVIRONMENT.CssClass.FEED_BACK}`)
                                        .text(GeneralBundle.$feedbackJustNumber);
                                    ctx.addClass("has-error");
                                    returnValue = false;
                                    return false;
                                }
                            }
                        }
                    }
                    break;
                case "TEXTAREA":
                    if (formControl.is(":visible")) {
                        if (formControl.val().length < minLength) {
                            ctx.find(`.${ENVIRONMENT.CssClass.FEED_BACK}`).text(
                                GeneralBundle.$feedbackMinimumLengthPartOne +
                                +minLength +
                                GeneralBundle.$feedbackMinimumLengthPartTwo);
                            ctx.addClass("has-error");
                            returnValue = false;
                            return false;
                        }
                        if (formControl.val().length > maxLength) {
                            ctx.find(`.${ENVIRONMENT.CssClass.FEED_BACK}`).text(
                                GeneralBundle.$feedbackMaximumLengthPartOne +
                                +maxLength +
                                GeneralBundle.$feedbackMaximumLengthPartTwo);
                            ctx.addClass("has-error");
                            returnValue = false;
                            return false;
                        }
                    }
                    break;
                case "SELECT":
                    if (formControl.find("option:selected").length === 0 ||
                        !formControl.val() ||
                        formControl.val() < 0) {
                        ctx.find(`.${ENVIRONMENT.CssClass.FEED_BACK}`).text(GeneralBundle.$feedbackSelectItem);
                        ctx.addClass("has-error");
                        returnValue = false;
                        return false;
                    }
                    break;
                }
            }
        });
        if (!returnValue && top.showConfirm) {
            setTimeout(function() {
                    top.showConfirm({
                        title: "پیام سیستمی",
                        body: "موارد اجباری را تکمیل نمایید",
                        confirmButton: {
                            text: GeneralBundle.$close
                        },
                        declineButton: {
                            hidden: true
                        }
                    });
                },
                450);
        }
        return returnValue;
    };

    const setInputValidationListener = function(inputObject) {
        if (inputObject.hasClass("read-only") || inputObject.attr("readonly")) {
            return;
        }
        var maxLength = inputObject.attr("ksun-max-length")
            ? Number(inputObject.attr("ksun-max-length"))
            : Config.INPUT_DEFAULT_MAX_LENGTH;
        var minLength = inputObject.attr("ksun-min-length")
            ? Number(inputObject.attr("ksun-min-length"))
            : Config.INPUT_DEFAULT_MIN_LENGTH;
        var inputType = inputObject.attr("ksun-input-type")
            ? inputObject.attr("ksun-input-type")
            : ENVIRONMENT.InputType.TEXT;
        var itemParent = inputObject.closest(`.${ENVIRONMENT.CssClass.FORM_GROUP}`);
        inputObject.on("keydown",
            function(e) {
                var inputLength = 0;
                var keyCode = e.keyCode | e.which;
                // Allow: backspace, delete, tab, escape
                if ($.inArray(keyCode,
                        [
                            ENVIRONMENT.KeyCode.DELETE, ENVIRONMENT.KeyCode.BACK_SPACE, ENVIRONMENT.KeyCode.TAB,
                            ENVIRONMENT.KeyCode.ESC, ENVIRONMENT.KeyCode.ENTER, ENVIRONMENT.KeyCode.CTRL
                        ]) !==
                    -1 ||
                    // Allow: Ctrl/cmd+A
                    (keyCode === ENVIRONMENT.KeyCode.A && (e.ctrlKey === true || e.metaKey === true)) ||
                    // Allow: Ctrl/cmd+Z
                    (keyCode === ENVIRONMENT.KeyCode.Z && (e.ctrlKey === true || e.metaKey === true)) ||
                    // Allow: Ctrl/cmd+C
                    (keyCode === ENVIRONMENT.KeyCode.C && (e.ctrlKey === true || e.metaKey === true)) ||
                    // Allow: Ctrl/cmd+X
                    (keyCode === ENVIRONMENT.KeyCode.X && (e.ctrlKey === true || e.metaKey === true)) ||
                    // Allow: Ctrl/cmd+F5
                    (keyCode === ENVIRONMENT.KeyCode.F5 && (e.ctrlKey === true || e.metaKey === true)) ||
                    // Allow: F5
                    (keyCode === ENVIRONMENT.KeyCode.F5) ||
                    // Allow: home, end, left, right
                    (keyCode >= 35 && keyCode <= 39)) {
                    // let it happen, don't do anything
                    return;
                }
                // Check Type
                switch (inputType) {
                case ENVIRONMENT.InputType.NUMBER:
                    if (inputObject.hasClass("double")) {
                        if ((keyCode >= ENVIRONMENT.KeyCode.NUM_0 && keyCode <= ENVIRONMENT.KeyCode.NUM_9) ||
                            (keyCode >= ENVIRONMENT.KeyCode.NUM_PAD_0 && keyCode <= ENVIRONMENT.KeyCode.NUM_PAD_9) ||
                            keyCode === ENVIRONMENT.KeyCode.DECIMAL_POINT) {

                            if (keyCode === ENVIRONMENT.KeyCode.DECIMAL_POINT) {
                                // prevent inserting dot(.) at the start position
                                if (!inputObject.val()) {
                                    e.preventDefault();
                                }

                                // prevent inserting dot(.) more than once
                                if (inputObject.val().indexOf(".") > -1) {
                                    e.preventDefault();
                                }
                            }

                            if (inputObject.data("isSelected")) {
                                inputObject.data("isSelected", false);
                                inputObject.val(null);
                            }

                            if (inputObject.val()) {
                                var splitArray = inputObject.val().split(".");
                                var integerPartLength = splitArray[0].length;
                                var fractionalPartLength = splitArray.length === 2 ? splitArray[1].length : 0;
                                var validFractionalLength = inputObject.attr("ksun-fractional-length")
                                    ? Number(inputObject.attr("ksun-fractional-length"))
                                    : Config.INPUT_DEFAULT_FRACTIONAL_LENGTH;

                                if (integerPartLength === maxLength) {
                                    if (splitArray.length === 2 || keyCode === ENVIRONMENT.KeyCode.DECIMAL_POINT) {
                                        if (fractionalPartLength === validFractionalLength) {
                                            e.preventDefault();
                                        }
                                    } else {
                                        e.preventDefault();
                                    }
                                } else if (fractionalPartLength === validFractionalLength) {
                                    e.preventDefault();
                                }

                                /*if (fractionalPartLength === validFractionalLength) {
                                        e.preventDefault();
                                    } else {
    
                                    }*/


                                /*if(integerPartLength === maxLength) {
    
                                        e.preventDefault();
                                    } else if(fractionalPartLength === validFractionalLength) {
                                        e.preventDefault();
                                    } else {
                                        return;
                                    }*/
                            }


                        } else {
                            e.preventDefault();
                        }
                    } else {
                        if ((keyCode >= ENVIRONMENT.KeyCode.NUM_0 && keyCode <= ENVIRONMENT.KeyCode.NUM_9) ||
                            (keyCode >= ENVIRONMENT.KeyCode.NUM_PAD_0 && keyCode <= ENVIRONMENT.KeyCode.NUM_PAD_9)) {
                            if (inputObject.data("isSelected")) {
                                inputObject.data("isSelected", false);
                                inputObject.val(null);
                            }
                            inputLength = e.target.value.length;
                            if (inputLength < maxLength) {
                                return;
                            } else {
                                e.preventDefault();
                            }
                        } else {
                            e.preventDefault();
                        }
                    }
                    break;
                case ENVIRONMENT.InputType.TEXT:
                    if (inputObject.data("isSelected")) {
                        inputObject.data("isSelected", false);
                        inputObject.val(null);
                    }
                    inputLength = e.target.value.length;
                    if (inputLength < maxLength) {
                        return;
                    } else {
                        e.preventDefault();
                    }
                    break;
                }
            });
        inputObject.on("focusout",
            function(e) {
                inputObject.data("isSelected", false);
                if (e.target.value.length < minLength) {
                    if (itemParent.length && itemParent.hasClass(ENVIRONMENT.CssClass.MANDATORY)) {
                        itemParent.addClass(ENVIRONMENT.CssClass.HAS_ERROR);
                        itemParent.removeClass(ENVIRONMENT.CssClass.VALID);
                        itemParent.find(`.${ENVIRONMENT.CssClass.FEED_BACK}`).text(
                            GeneralBundle.$feedbackMinimumLengthPartOne +
                            +minLength +
                            GeneralBundle.$feedbackMinimumLengthPartTwo);
                    }
                } else {
                    if (itemParent.length && itemParent.hasClass(ENVIRONMENT.CssClass.MANDATORY)) {
                        itemParent.addClass(ENVIRONMENT.CssClass.VALID);
                    }
                }
            });
        inputObject.on("focusin",
            function(e) {
                inputObject.data("isSelected", false);
                if (itemParent.length) {
                    itemParent.removeClass(ENVIRONMENT.CssClass.HAS_ERROR);
                    itemParent.removeClass(ENVIRONMENT.CssClass.VALID);
                }
            });
        inputObject.on("select",
            function() {
                inputObject.data("isSelected", true);
            });
    };

    const setSelect2ValidationListener = function(selectObject) {
        if (selectObject.hasClass("read-only") || selectObject.attr("readonly")) {
            return;
        }
        var itemParent = selectObject.closest(`.${ENVIRONMENT.CssClass.FORM_GROUP}`);
        selectObject.on("select2:open",
            function() {
                if (itemParent.length && itemParent.hasClass(ENVIRONMENT.CssClass.MANDATORY)) {
                    itemParent.removeClass(ENVIRONMENT.CssClass.HAS_ERROR);
                }
            });
        selectObject.on("select2:close",
            function() {
                if (itemParent.length && itemParent.hasClass(ENVIRONMENT.CssClass.MANDATORY) && !selectObject.val()) {
                    itemParent.addClass(ENVIRONMENT.CssClass.HAS_ERROR);
                    itemParent.find(`.${ENVIRONMENT.CssClass.FEED_BACK}`).text(GeneralBundle.$feedbackSelectItem);
                }

            });
    };

    const isValidPassword = function(password) {
        const regex = new RegExp("((?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[~!@#$%^&*-_?]).{8,20})");
        return regex.test(password);
    };
    const isValidUsername = function(username) {
        const regex = new RegExp("^[a-zA-Z0-9_-]{5,20}$");
        return regex.test(username);
    };
    const isValidName = function(name) {
        const regex =
            new RegExp(
                "^[\u0620\u0621\u0622\u0624\u0626\u0627\u0628\u067E\u062A-\u062C\u0686\u062D-\u0632\u0698\u0633-\u063A\u0641\u0642\u06A9\u06AF\u0644-\u0648\u06CC\\s]+$");
        return regex.test(name);
    };
    const isValidMobile = function(mobile) {
        const regex = new RegExp("(09)?[0-9]{9}");
        return regex.test(mobile);
    };
    const isValidPhone = function(phone) {
        const regex = new RegExp("(0)?[0-9]{10}");
        return regex.test(phone);
    };
    const isValidEmail = function(email) {
        const regex =
            new RegExp(
                "^[a-zA-Z0-9_+&*-]+(?:\\.[a-zA-Z0-9_+&*-]+)*@(?:[a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,7}$"); //[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,3}$
        return regex.test(email);
    };
    const isValidNationalCode = function(nationalCode) {
        if (!/^\d{10}$/.test(nationalCode))
            return false;
        const check = parseInt(nationalCode[9]);
        var sum = 0;
        var i;
        for (i = 0; i < 9; ++i) {
            sum += parseInt(nationalCode[i]) * (10 - i);
        }
        sum %= 11;
        return (sum < 2 && check === sum) || (sum >= 2 && check + sum === 11);
    };
    const isValidDomain = function(domain) {
        const regex =
            new RegExp(
                "^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:([1-9]|[1-5]?[0-9]{2,4}|6[1-4][0-9]{3}|65[1-4][0-9]{2}|655[1-2][0-9]|6553[1-5]))?(\/.*)?$");
        return regex.test(domain);
    };
    const isNumberCardBank = function(number) {
        const regex =
            new RegExp(
                "^((?:4\d{3})|(?:5[1-5]\d{2})|(?:6011)|(?:3[68]\d{2})|(?:30[012345]\d))[ -]?(\d{4})[ -]?(\d{4})[ -]?(\d{4}|3[4,7]\d{13})$");
        return regex.test(number);
    };

    const isValidIpOfServer = function(ip) {
        const regex =
            new RegExp(
                "^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
        return regex.test(ip);
    };

    const getTitleOfObjectFromFormat = function(obj, format) {
        const regex = /{([^}]+)}/gm;
        var match;
        var temp = format;
        while ((match = regex.exec(format)) !== null) {
            if (match.index === regex.lastIndex) {
                regex.lastIndex++;
            }
            temp = temp.replace(`{${match[1]}}`, obj[match[1]]);
        }
        return temp;
    };

    const convertFileToBase64 = function(file, onLoad, onError) {
        var reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = function() {
            if (onLoad) {
                onLoad.apply(null, [reader.result]);
            }
        };
        reader.onerror = function(error) {
            if (onError) {
                onError.apply(null, [error]);
            }
        };
    };

    const getOperatingSystemFromUserAgent = function(userAgent = navigator.userAgent) {
        var result = { type: "Unknown", architecture: "32bit" };
        if (userAgent.indexOf("x64") > -1 || userAgent.indexOf("X64") > -1 || userAgent.indexOf("Win64") > -1) {
            result.architecture = "64bit";
        }
        if (userAgent.indexOf("Win16") > -1) {
            result.type = "Windows 3.11";
            return result;
        }
        if (userAgent.indexOf("Windows 95") > -1 ||
            userAgent.indexOf("Win95") > -1 ||
            userAgent.indexOf("Windows_95") > -1) {
            result.type = "Windows 95";
            return result;
        }
        if (userAgent.indexOf("Windows 98") > -1 || userAgent.indexOf("Win98") > -1) {
            result.type = "Windows 98";
            return result;
        }
        if (userAgent.indexOf("Windows NT 5.0") > -1 || userAgent.indexOf("Windows 2000") > -1) {
            result.type = "Windows 2000";
            return result;
        }
        if (userAgent.indexOf("Windows NT 5.1") > -1 || userAgent.indexOf("Windows XP") > -1) {
            result.type = "Windows XP";
            return result;
        }
        if (userAgent.indexOf("Windows NT 5.2") > -1) {
            result.type = "Windows Server 2003";
            return result;
        }
        if (userAgent.indexOf("Windows NT 6.0") > -1) {
            result.type = "Windows Vista";
            return result;
        }
        if (userAgent.indexOf("Windows NT 6.1") > -1) {
            result.type = "Windows 7";
            return result;
        }
        if (userAgent.indexOf("Windows NT 6.2") > -1) {
            result.type = "Windows 8";
            return result;
        }
        if (userAgent.indexOf("Windows NT 6.3") > -1) {
            result.type = "Windows 8.1";
            return result;
        }
        if (userAgent.indexOf("Windows NT 10.0") > -1) {
            result.type = "Windows 10";
            return result;
        }
        if (userAgent.indexOf("Windows NT 4.0") > -1 ||
            userAgent.indexOf("WinNT4.0") > -1 ||
            userAgent.indexOf("Windows NT") > -1 ||
            userAgent.indexOf("WinNT") > -1) {
            result.type = "Windows NT 4.0";
            return result;
        }
        if (userAgent.indexOf("Windows ME") > -1) {
            result.type = "Windows ME";
            return result;
        }
        if (userAgent.indexOf("OpenBSD") > -1) {
            result.type = "Open BSD";
            return result;
        }
        if (userAgent.indexOf("SunOS") > -1) {
            result.type = "Sun OS";
            return result;
        }
        if (userAgent.indexOf("Linux") > -1 || userAgent.indexOf("X11") > -1) {
            result.type = "Linux";
            return result;
        }
        if (userAgent.indexOf("Mac_PowerPC") > -1 || userAgent.indexOf("Macintosh") > -1) {
            result.type = "Mac OS";
            return result;
        }
        if (userAgent.indexOf("QNX") > -1) {
            result.type = "QNX";
            return result;
        }
        if (userAgent.indexOf("BeOS") > -1) {
            result.type = "BeOS";
            return result;
        }
        if (userAgent.indexOf("OS/2") > -1) {
            result.type = "OS/2";
            return result;
        }
        if (userAgent.indexOf("Android") > -1) {
            userAgent = userAgent.substr(userAgent.indexOf("Android"), userAgent.length - 1);
            result.type = userAgent.split(";")[0];
            return result;
        }
        if (userAgent.indexOf("iPad") > -1 ||
            userAgent.indexOf("iPod") > -1 ||
            userAgent.indexOf("iphone") > -1 ||
            userAgent.indexOf("iPhone") > -1) {
            result.type = "iOS";
            let version;
            try {
                version = userAgent.match(/(OS |os |OS_)(\d+((_|.)\d)?((_|.)\d)?)/)[0].replace("OS ", "")
                    .replace("_", ".");
                result.type += (` ${version}`);
            } catch (e) {
            }
            return result;
        }
        if (userAgent.indexOf("nuhk") > -1 ||
            userAgent.indexOf("Googlebot") > -1 ||
            userAgent.indexOf("Yammybot") > -1 ||
            userAgent.indexOf("Openbot") > -1 ||
            userAgent.indexOf("Slurp") > -1 ||
            userAgent.indexOf("MSNBot") > -1 ||
            userAgent.indexOf("Ask Jeeves/Teoma") > -1 ||
            userAgent.indexOf("ia_archiver") > -1) {
            result.type = "Search Bot";
            return result;
        }

    };
    const getBrowserFromUserAgent = function(userAgent = navigator.userAgent) {
        var result = { type: "Unknown", version: "Unknown" };

        var firefoxIndex = userAgent.indexOf("Firefox");
        if (firefoxIndex > -1) {
            var titleWithVersion = userAgent.substr(firefoxIndex, userAgent.length - 1).split(" ")[0];
            result.version = titleWithVersion.split("/")[1] ? titleWithVersion.split("/")[1] : "-";
            result.type = "Firefox";
            return result;
        }

        var chromeIndex = userAgent.indexOf("Chrome");
        if (chromeIndex > -1) {
            var titleWithVersion = userAgent.substr(chromeIndex, userAgent.length - 1).split(" ")[0];
            result.version = titleWithVersion.split("/")[1] ? titleWithVersion.split("/")[1] : "-";
            result.type = "Chrome";
            return result;
        }

        var operaIndex = userAgent.indexOf("Opera") > -1 ? userAgent.indexOf("Opera") : userAgent.indexOf("OPR");
        if (operaIndex > -1) {
            var titleWithVersion = userAgent.substr(operaIndex, userAgent.length - 1).split(" ")[0];
            result.version = titleWithVersion.split("/")[1] ? titleWithVersion.split("/")[1] : "-";
            result.type = "Opera";
            return result;
        }

        var safariIndex = userAgent.indexOf("Safari");
        if (safariIndex > -1) {
            var titleWithVersion = userAgent.substr(safariIndex, userAgent.length - 1).split(" ")[0];
            result.version = titleWithVersion.split("/")[1] ? titleWithVersion.split("/")[1] : "-";
            result.type = "Safari";
            return result;
        }

        var ieIndex = userAgent.indexOf("MSIE");
        if (ieIndex > -1) {
            var titleWithVersion = userAgent.substr(ieIndex, userAgent.length - 1).split(" ")[0];
            result.version = titleWithVersion.split("/")[1] ? titleWithVersion.split("/")[1] : "-";
            result.type = "Internet Explorer";
            return result;
        }
        return result;
    };
    const isMobileOrTablet = () => {
        let check = false;
        (function(a) {
            if (
                /(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino|android|ipad|playbook|silk/i
                    .test(a) ||
                    /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i
                    .test(a.substr(0, 4)))
                check = true;
        })(navigator.userAgent || navigator.vendor || window.opera);
        return check;
    };

    const createFile = function(content, name, type, obj) {
        obj.attr("href", `data:${type};charset=utf-8,${encodeURIComponent(content ? content : "")}`);
        obj.attr("download", name);
    };

    const numberWithCommas = function(number) {
        return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    };

    /**
     * @param extensions: array -> ['pdf','zip']
     * @param file: file
     */
    const checkFileExtension = function(extensions, file) {
        if (!Array.isArray(extensions)) {
            throw new Error("checkFileExtension method accepts array of extensions as first parameter");
        }
        var res = false;
        var extension = file.name.split("\.");
        extension = extension[extension.length - 1].toLowerCase();
        extensions.forEach(function(ext) {
            if (ext.toLowerCase() === extension) {
                res = true;
                return -1; // terminate forEach()
            }
        });
        return res;
    };

    const isScrolledIntoView = function(elem) {
        const docViewTop = $(window).scrollTop();
        const docViewBottom = docViewTop + $(window).height();

        const elemTop = $(elem).offset().top;
        const elemBottom = elemTop + $(elem).height();

        return ((elemBottom <= docViewBottom) && (elemTop >= docViewTop));
    };

    const uuid = function() {
        var seed = Date.now();
        if (window.performance && typeof window.performance.now === "function") {
            seed += performance.now();
        }

        const res = "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g,
            function(c) {
                const r = (seed + Math.random() * 16) % 16 | 0;
                seed = Math.floor(seed / 16);

                return (c === "x" ? r : r & (0x3 | 0x8)).toString(16);
            });

        return res;
    };

    const getRandomInt = function(min, max) {
        min = Math.ceil(min);
        max = Math.floor(max);
        return Math.floor(Math.random() * (max - min + 1)) + min;
    };

    const shortenString = function(string, length) {
        return string.length > length ? string.substr(0, length).concat(" ...") : string;
    };

    const getFileExtension = function(file) {
        return file.name.split(".").pop();
    };

    const isRtlLanguage = function() {
        return document.querySelector("html").getAttribute("lang") &&
            document.querySelector("html").getAttribute("lang").toUpperCase() === "FA";
    };

    const findByCustomID = function(parentContainer, value) {
        return $(parentContainer.get(0).shadowRoot.querySelector(`[data-custom-id="${value}"]`));
    };

    function convertToEnglishDigit(input) {
        // persian
        input = input.replace(/۰/g, "0");
        input = input.replace(/۱/g, "1");
        input = input.replace(/۲/g, "2");
        input = input.replace(/۳/g, "3");
        input = input.replace(/۴/g, "4");
        input = input.replace(/۵/g, "5");
        input = input.replace(/۶/g, "6");
        input = input.replace(/۷/g, "7");
        input = input.replace(/۸/g, "8");
        input = input.replace(/۹/g, "9");
        // arabic
        input = input.replace(/٠/g, "0");
        input = input.replace(/١/g, "1");
        input = input.replace(/٢/g, "2");
        input = input.replace(/٣/g, "3");
        input = input.replace(/٤/g, "4");
        input = input.replace(/٥/g, "5");
        input = input.replace(/٦/g, "6");
        input = input.replace(/٧/g, "7");
        input = input.replace(/٨/g, "8");
        input = input.replace(/٩/g, "9");

        return input;
    }

    const scriptImporter = {
        url: (url) => {
            return new Promise((resolve, reject) => {
                const script = document.createElement("script");
                script.type = "text/javascript";
                script.src = url;
                script.onreadystatechange = () => {
                    if (script.readyState === "loaded" || script.readyState === "complete") {
                        script.onreadystatechange = null;
                        resolve(script);
                    }
                };
                // others
                script.onload = () => {
                    resolve(script);
                };
                //script.addEventListener('load', () => {console.log("Success ==> " , url); resolve(script)}, false);
                script.addEventListener("error",
                    () => {
                        console.log("Error ==> ", url);
                        reject(script)
                    },
                    false);
                document.body.appendChild(script);
            });
        },
        urls: (urls) => {
            return Promise.all(urls.map(scriptImporter.url));
        }
    };

    return {
        differentTime: diffTime,
        loadScript: loadScript,
        urlParameter: urlParameter,
        setInputValidationListener: setInputValidationListener,
        setSelect2ValidationListener: setSelect2ValidationListener,
        validateForm: validateForm,
        isValidPassword: isValidPassword,
        isValidUsername: isValidUsername,
        isValidName: isValidName,
        isValidMobile: isValidMobile,
        isValidPhone: isValidPhone,
        isValidEmail: isValidEmail,
        isValidNationalCode: isValidNationalCode,
        isValidDomain: isValidDomain,
        isNumberCardBank: isNumberCardBank,
        isValidIpOfServer: isValidIpOfServer,
        getTitleOfObjectFromFormat: getTitleOfObjectFromFormat,
        convertFileToBase64: convertFileToBase64,
        getOperatingSystemFromUserAgent: getOperatingSystemFromUserAgent,
        getBrowserFromUserAgent: getBrowserFromUserAgent,
        isMobileOrTablet: isMobileOrTablet,
        createFile: createFile,
        numberWithCommas: numberWithCommas,
        checkFileExtension: checkFileExtension,
        isScrolledIntoView: isScrolledIntoView,
        uuid: uuid,
        getRandomInt: getRandomInt,
        shortenString: shortenString,
        getFileExtension: getFileExtension,
        isRtlLanguage: isRtlLanguage,
        findByCustomID: findByCustomID,
        convertToEnglishDigit: convertToEnglishDigit,
        scriptImporter: scriptImporter
    };
})();