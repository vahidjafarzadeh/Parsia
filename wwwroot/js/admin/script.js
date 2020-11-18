"use strict";
//initialise all variable in admin page
let loading, loadingModal, containerMenu, navMenu, tabContainer, mainSection, next, before, logo, menu, modalInstance;
const initVariable = () => {
    loading = $(".loading");
    tabContainer = $(".tab-container-main");
    navMenu = $(".main-nav");
    containerMenu = $(".container-menu");
    mainSection = $(".main-section");
    next = $(".next");
    before = $(".before");
    menu = $(".menu");
    logo = $("#logo");
    loadingModal = $('#loading-modal');
    modalInstance = Modal.init();
    changeAllBundle();
};
const initAllEvent = () => {
    next.on("click",
        () => {
            const item = tabContainer.find("li:eq(0)");
            tabContainer.append(item);
            item.click();
        });
    before.on("click",
        () => {
            const item = tabContainer.find("li").last();
            tabContainer.prepend(item);
            item.click();
        });
    logo.on("click",
        () => {
            if ($(window).width() <= 768) {
                if (menu.hasClass("auto")) {
                    menu.removeClass("auto");
                } else {
                    menu.addClass("auto");
                }
            }
        });
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
const createScrollBar = () => {

};
const createMenu = (data) => {
    data.sort(function (a, b) {
        if (a.orderNode !== b.orderNode) {
            return a.orderNode - b.orderNode;
        }
        if (a.orderNode === b.orderNode) {
            return 0;
        }
        return a.orderNode > b.orderNode ? 1 : -1;
    });
    const lstRoot = [];
    $.each(data,
        (i, v) => {
            if (v.parent && v.parent.entityId === 1) {
                lstRoot.push(v);
            }
        });
    $.each(lstRoot,
        (i, v) => {
            const li = $("<li/>",
                {
                    click: () => {
                        if ($(window).width() <= 768) {
                            navMenu.slideToggle("slow");
                            menu.removeClass("auto");
                        }
                        containerMenu.find(".active").removeClass("active");
                        li.addClass("active");
                        navMenu.find(".container-tootal-menu").html("");
                        const ul1 = $("<ul/>", { class: "base-menu" });
                        $.each(data,
                            (index, item) => {
                                if (item.parent && item.parent.entityId === v.entityId) {
                                    const li1 = $("<li/>",
                                        {
                                            class: "base-child",
                                            click: (e) => {
                                                if ($(e.target).hasClass("title-text")) {
                                                    if (li1.hasClass("rotate")) {
                                                        li1.removeClass("rotate");
                                                    } else {
                                                        li1.addClass("rotate");
                                                    }
                                                    li1.find("ul").slideToggle("slow");
                                                }
                                            }
                                        });
                                    const p = $("<p/>", { class: "title-text", text: item.title });
                                    li1.append(p);
                                    const ul2 = $("<ul/>", { class: "child-menu" });
                                    let counter = 1;
                                    $.each(data,
                                        (ind, value) => {
                                            if (value.parent && value.parent.entityId === item.entityId) {
                                                const li3 = $("<li/>",
                                                    {
                                                        html: value.icon,
                                                        click: () => {
                                                            if ($(window).width() <= 768) {
                                                                navMenu.slideToggle("slow");
                                                            }
                                                            createTab(value);
                                                        }
                                                    });
                                                const span = $("<span/>", { text: value.title });
                                                li3.find("i").addClass(`color-${counter}`);
                                                li3.append(span);
                                                ul2.append(li3);
                                                counter++;
                                            }
                                        });
                                    li1.append(ul2);
                                    ul1.append(li1);
                                }
                            });
                        navMenu.find(".container-tootal-menu").append(ul1);
                    }
                });
            const span = $("<span/>",
                {
                    class: "container-menu",
                    html: v.icon,
                    "data-toggle": "tooltip",
                    "data-placement": "left",
                    "title": v.title
                });
            li.append(span);
            containerMenu.append(li);
        });
    containerMenu.find("li:eq(1)").click();
    $('[data-toggle="tooltip"]').tooltip();
    const simpleBar = new SimpleBar(navMenu[0]);
};
const changeAllBundle = () => {
    $.each($("body").find("[parsi-title-key]"),
        (i, v) => {
            $(v).attr("title", adminBundle[$(v).attr("parsi-title-key")]);
        });
};
const createTab = (data) => {
    tabContainer.find(".active").removeClass("active");
    const uuid = new Date().getTime();
    const li = $("<li/>",
        {
            class: "active",
            click: () => {
                tabContainer.find(".active").removeClass("active");
                $(li).addClass("active");
                if (mainSection.find(`#${uuid}`).length > 0) {
                    mainSection.find("iframe").fadeOut();
                    setTimeout(() => {
                        mainSection.find(`#${uuid}`).fadeIn();
                    },
                        500);
                } else {
                    var iframe = $("<iframe/>",
                        {
                            src: data.path,
                            id: uuid,
                            class: "custom-frame"
                        });
                    mainSection.find("iframe").fadeOut();
                    setTimeout(() => {
                        mainSection.append(iframe);
                    },
                        500);
                }

            },
            mousedown: (e) => {
                var keyCode = e.which || e.keyCode;
                if (keyCode === ENVIRONMENT.KeyCode.MIDDLE_MOUSE) {
                    e.preventDefault();
                    li.remove();
                    tabContainer.find(".active").removeClass("active");
                    tabContainer.find("li:eq(0)").addClass("active").trigger("click");
                    mainSection.find(`#${uuid}`).remove();
                }

            }
        });
    const spanText = $("<span/>", { class: "text", text: data.title });
    const spanIconRemove = $("<span/>",
        {
            class: "icon remove",
            html: `<i class="fas fa-times"></i>`,
            click: () => {
                li.remove();
                tabContainer.find(".active").removeClass("active");
                tabContainer.find("li:eq(0)").addClass("active").trigger("click");
                mainSection.find(`#${uuid}`).remove();
            }
        });
    const spanIconRefresh = $("<span/>",
        {
            class: "icon refresh",
            html: `<i class="fas fa-sync-alt"></i>`,
            click: () => {
                mainSection.find(`#${uuid}`).attr("src", data.path);
            }
        });
    li.append(spanText, spanIconRemove, spanIconRefresh);
    li.data({ value: data });
    tabContainer.append(li);
    li.trigger("click");
};
function showConfirm(config) {
    modalInstance.showConfirm(config);
}
function hideConfirm() {
    modalInstance.hideConfirm();
};
const getMenu = () => {
    var handler = new Handler();
    handler.beforeSend = () => {
    }
    handler.complete = () => {
    }
    handler.success = (data) => {
        if (data.done) {
            createMenu(data.result);
        } else {
            errorHandler(data);
        }
    }
    Api.post({ url: "menu/getAllMenu", data: {}, handler: handler});
}
$(document).ready(function () {
    Storage.setPageNeedLogin(true);
    if (Storage.isPageNeedLogin() && Storage.getUserInfo() === null) {
        SSO.init();
    } else {
        initVariable();
        initAllEvent();
        createScrollBar();
        loading.fadeOut("slow");
        getMenu();
    }
});