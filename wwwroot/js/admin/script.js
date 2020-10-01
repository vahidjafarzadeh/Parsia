"use strict";
//initialise all variable in admin page
let loading, containerMenu, navMenu, tabContainer, mainSection, next, before;
const initVariable = () => {
    loading = $('.loading');
    tabContainer = $('.tab-container-main');
    navMenu = $('.main-nav');
    containerMenu = $('.container-menu');
    mainSection = $('.main-section');
    next = $('.next');
    before = $('.before');
    changeAllBundle();
}
const initAllEvent = () => {
    next.on("click",() => {
        let item = tabContainer.find("li:eq(0)");
        tabContainer.append(item);
        item.click();
    });
    before.on("click", () => {
        let item = tabContainer.find("li").last();
        tabContainer.prepend(item);
        item.click();
    });
}
const createScrollBar = () => {

}
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
    let lstRoot = [];
    $.each(data, (i, v) => {
        if (v.parentId === -1) {
            lstRoot.push(v);
        }
    });
    $.each(lstRoot, (i, v) => {
        let li = $("<li/>", {
            click: () => {
                containerMenu.find(".active").removeClass("active");
                li.addClass("active");
                navMenu.find(".container-tootal-menu").html("");
                let ul1 = $("<ul/>", { class: "base-menu" });
                $.each(data, (index, item) => {
                    if (item.parentId === v.entityId) {
                        let li1 = $("<li/>", {
                            class: "base-child", click: (e) => {
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
                        let p = $("<p/>", { class: "title-text", text: item.name });
                        li1.append(p);
                        let ul2 = $("<ul/>", { class: "child-menu" });
                        let counter = 1;
                        $.each(data, (ind, value) => {
                            if (value.parentId === item.entityId) {
                                let li3 = $("<li/>",
                                    {
                                        html: value.icon,
                                        click: () => {
                                            createTab(value);
                                        }
                                    });
                                let span = $("<span/>", { text: value.name });
                                li3.find("i").addClass("color-" + (counter));
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
        let span = $("<span/>", {
            class: "container-menu",
            html: v.icon,
            "data-toggle": "tooltip",
            "data-placement": "left",
            "title": v.name
        });
        li.append(span);
        containerMenu.append(li);
    });
    containerMenu.find("li:eq(1)").click();
    $('[data-toggle="tooltip"]').tooltip();
    let simpleBar = new SimpleBar(navMenu[0]);
}
const changeAllBundle = () => {
    $.each($("body").find("[parsi-title-key]"), (i, v) => {
        $(v).attr("title", adminBundle[$(v).attr("parsi-title-key")]);
    });
}
const createTab = (data) => {
    let flag = true;
    $.each(tabContainer.find("li"), (i, v) => {
        if ($(v).data()) {
            if ($(v).data().value.entityId === data.entityId) {
                $(v).click();
                flag = false;
            }

        }
    });
    if (flag) {
        tabContainer.find(".active").removeClass("active");
        let uuid = new Date().getTime();
        let li = $("<li/>", {
            class: "active", click: () => {
                tabContainer.find(".active").removeClass("active");
                $(li).addClass("active");
                if (mainSection.find("#" + uuid).length > 0) {
                    mainSection.find("iframe").fadeOut();
                    setTimeout(() => {
                        mainSection.find("#" + uuid).fadeIn();
                    }, 500);
                } else {
                    var iframe = $("<iframe/>", {
                        src: data.path,
                        id: uuid,
                        class: "custom-frame"
                    });
                    mainSection.find("iframe").fadeOut();
                    setTimeout(() => {
                        mainSection.append(iframe);
                    }, 500);
                }

            }
        });
        let spanText = $("<span/>", { class: "text", text: data.name });
        let spanIconRemove = $("<span/>", {
            class: "icon remove", html: `<i class="fas fa-times"></i>`, click: () => {
                li.remove();
                tabContainer.find(".active").removeClass("active");
                tabContainer.find("li:eq(0)").addClass("active").trigger("click");
                mainSection.find("#" + uuid).remove();
            }
        });
        let spanIconRefresh = $("<span/>", {
            class: "icon refresh", html: `<i class="fas fa-sync-alt"></i>`, click: () => {
                mainSection.find("#" + uuid).attr("src", data.path);
            }
        });
        li.append(spanText, spanIconRemove, spanIconRefresh);
        li.data({ value: data });
        tabContainer.append(li);
        li.trigger("click");
    }
}

$(function () {
    initVariable();
    initAllEvent();
    createScrollBar();
    loading.fadeOut("slow");
    createMenu([
        { name: "تنظیمات", title: "setting", path: "/", icon: "<i class='fas fa-cogs'></i>", orderNode: 1, parentId: -1, entityId: 1 },
        { name: "مدیریت کاربران", title: "user management", path: "/", icon: "<i class='fas fa-users'></i>", orderNode: 2, parentId: -1, entityId: 2 },
        { name: "منو ها", title: "menu", path: "/", icon: "<i class='fas fa-bars'></i>", orderNode: 3, parentId: -1, entityId: 3 },
        { name: "تنظیمات سامانه", title: "base setting", path: "/", icon: "<i class='fas fa-users'></i>", orderNode: 1, parentId: 1, entityId: 4 },
        { name: "تنظیمات سیستم", title: "system setting", path: "/", icon: "<i class='fas fa-users'></i>", orderNode: 2, parentId: 1, entityId: 5 },
        { name: "مدیریت فایل ها", title: "file manager", path: "/", icon: "<i class='fas fa-file'></i>", orderNode: 1, parentId: 4, entityId: 6 },
        { name: "مقادیر چند گزینه ای", title: "combo val", path: "https://www.google.com", icon: "<i class='fas fa-list-ol'></i>", orderNode: 2, parentId: 4, entityId: 7 },
        { name: "آیکون ها", title: "icons", path: "/", icon: "<i class='fas fa-atom'></i>", orderNode: 3, parentId: 4, entityId: 8 },
        { name: "تنظیمات سایت", title: "site setting", path: "/", icon: "<i class='fab fa-weebly'></i>", orderNode: 1, parentId: 5, entityId: 9 },
        { name: "تنظیمات پویا", title: "dynamic setting", path: "/", icon: "<i class='fas fa-chart-line'></i>", orderNode: 2, parentId: 5, entityId: 10 },
        { name: "لیست کاربران", title: "user list", path: "/", icon: "<i class='fas fa-users'></i>", orderNode: 1, parentId: 2, entityId: 11 },
        { name: "دسترسی ها", title: "access group", path: "/", icon: "<i class='fas fa-users'></i>", orderNode: 2, parentId: 2, entityId: 12 },
        { name: "افراد", title: "persons", path: "/", icon: "<i class='fas fa-user-friends'></i>", orderNode: 1, parentId: 11, entityId: 13 },
        { name: "کاربران سامانه", title: "users", path: "/", icon: "<i class='fas fa-users'></i>", orderNode: 2, parentId: 11, entityId: 14 },
        { name: "نقش کاربران", title: "user role", path: "/", icon: "<i class='fas fa-user-cog'></i>", orderNode: 3, parentId: 11, entityId: 15 },
        { name: "گروه دسترسی", title: "access", path: "/", icon: "<i class='fas fa-users'></i>", orderNode: 3, parentId: 12, entityId: 16 },
        { name: "عملیات", title: "actions", path: "/", icon: "<i class='fas fa-people-carry'></i>", orderNode: 1, parentId: 12, entityId: 17 },
        { name: "نقش ها", title: "roles", path: "/", icon: "<i class='fas fa-user-circle'></i>", orderNode: 4, parentId: 12, entityId: 18 },
        { name: "سازمان ها", title: "organization", path: "/", icon: "<i class='fas fa-building'></i>", orderNode: 5, parentId: 12, entityId: 19 },
        { name: "منوها مدیریت", title: "menu admin", path: "/", icon: "<i class='fas fa-ellipsis-v'></i>", orderNode: 1, parentId: 20, entityId: 23 },
        { name: "مدیریت منوها", title: "menu management", path: "/", icon: "<i class='fas fa-users'></i>", orderNode: 1, parentId: 3, entityId: 20 },
        { name: "منوهای سایت", title: "menu site", path: "/", icon: "<i class='fas fa-th-list'></i>", orderNode: 2, parentId: 20, entityId: 21 },
        { name: "فرآیندها", title: "use case", path: "/", icon: "<i class='fas fa-random'></i>", orderNode: 2, parentId: 12, entityId: 22 }
    ]);
})