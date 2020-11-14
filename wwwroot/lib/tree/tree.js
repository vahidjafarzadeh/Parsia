let conf = {
    //پیش نیاز ها
    dependencies: {
        link: [`/lib/sweetalert/sweetalert.css`, `/sass/page/edit.css`, `sass/core/core.css`, `/lib/tree/style.css`],
        script: [`/lib/sweetalert/sweetalert.min.js`]
    },
    //شناسه المان روی صفحه که تبدیل به tree می شود
    element: "#tree",
    //سرویس دریافت اطلاعات از سرور
    serviceUrl: 'combo/getTreeMenu',
    //آدرس سرویس حذف کردن هر رکورد
    removeService: 'combo/delete',
    //آدرس صفحه ویرایش رکورد ها
    urlEditPage: '/core/page/comboval/new/edit.html',
    //نام پارامتر ارسالی برای ویرایش و افزودن و حذف رکورد
    clauseField: "entityId",
    //شناسه اتوکامپلیت والد در صفحه ویرایش
    elementIdInEditPageForFillParent: "#parent",
    //در صورت نیاز به دریافت کل اطلاعات به صورت یکجا true در غیر این صورت false
    getTotal: true,
    //عملیات های پیش فرض کراد برای ساختن مجدد هر نود
    action: {
        delete: "delete",
        insert: "insert",
        update: "update"
    },
    //کلید اصلی والد
    rootKey: "entityId",
    //مقدار کلید اضلی والد
    rootValue: [-1],
    //فیلد مناسب برای چک کردن این که یگ نود دارای فرزند هست یانه
    childKeyForShowSquare: "code",
    //در صورتی که این مقدار را داشته باشد علامت مربع کنار درخت نمایش داده خواهد شد
    childKeyValueForShowSquare: "1",
    //تعداد لود نود در هر ریشه
    countShow: -1,
    //دارای صفحه بندی می باشد
    doPagination:false,
    //متن نمایش بیشتر
    titleShow: "بیشتر ...",
    //نام فیلد فرزند برای قرار گیری در والد در صورتی که آبجکت باشد با نقطه نوشته شود
    childKey: "parent.entityId",
    //در صورت نیاز به کراد true و در صورت نیاز به انتخاب false
    showCrud: false,
    //در صورتی که true باشد بعد از هر افزودن و یا ویرایش کردن اطلاعات مجددا از سرور دریافت خواهد شد
    autoRefreshDataAfterSaveOrUpdate: true,
    //در صورتی که اطلاعات به صورت اتوماتیک رفرش نشوند این پیغام به کتربر نمایش داده می شود
    messageForUpdate: "کاربر گرامی جهت مشاهده تغییرات لطفا از دکمه تازه سازی اطلاعات استفاده نمایید",
    //نام فیلد نمایشی هر رکورد
    showField: "name",
    //قابلیت انتخاب فیلد ها در سطح
    selectState: 0,
    //تابع اعمال شونده بر روی نام نمایش
    functionData: ``,
    //تنظیم برای مهندس عبدلیان افزوده شده
    customInsert: false,
    //انتخاب تمام فرزندان در صورت انتخاب شدن پدر
    selectAllChildWhenSelectParent: true,
    //باز بودن کل درخت در صورتی که اطلاعات به صورت یکجا دریافت می شوند
    openWholeTreeInGetTotal: true,
    //رنگ زمینه ریشه ها
    colors: [["#f5f5f5", "#676767"]],
    //عنوان درخت در ریشه ها
    titleTree: [""],
    //عنوان درخت
    title: "درخت مقادیر چند گزینه ای",
    //آیکون درخت
    titleIcon: `<svg id="Capa_1" enable-background="new 0 0 511.819 511.819" height="512" viewBox="0 0 511.819 511.819" width="512" xmlns="http://www.w3.org/2000/svg"><g><path d="m464.387 96.44v-35.023c13.577-3.365 23.673-15.648 23.673-30.25 0-17.186-13.981-31.167-31.167-31.167s-31.167 13.981-31.167 31.167c0 14.603 10.096 26.886 23.673 30.25v35.023c-13.577 3.365-23.673 15.648-23.673 30.251 0 4.757 1.075 9.265 2.989 13.302l-30.541 23.964c-4.878-3.168-10.688-5.017-16.926-5.017-5.059 0-9.836 1.219-14.065 3.368l-71.063-40.189c.231-1.528.353-3.092.353-4.684 0-17.186-13.981-31.167-31.167-31.167s-31.167 13.981-31.167 31.167c0 1.592.121 3.156.353 4.684l-23.168 13.102c-3.603 2.037-4.871 6.61-2.834 10.212 1.378 2.438 3.917 3.807 6.53 3.807 1.25 0 2.517-.313 3.683-.972l21.616-12.224c5.688 7.618 14.771 12.559 24.988 12.559s19.3-4.941 24.988-12.559l65.035 36.78c-3.312 4.95-5.247 10.895-5.247 17.284 0 14.603 10.096 26.886 23.673 30.25v63.817c-13.577 3.365-23.673 15.648-23.673 30.25 0 5.776 1.585 11.187 4.335 15.83l-64.123 36.264c-5.688-7.618-14.771-12.559-24.988-12.559s-19.3 4.941-24.988 12.559l-63.309-35.803c2.92-4.744 4.609-10.323 4.609-16.291 0-14.603-10.096-26.886-23.673-30.25v-63.817c13.577-3.365 23.673-15.648 23.673-30.25 0-6.582-2.056-12.69-5.553-17.727l12.502-7.071c3.603-2.038 4.871-6.61 2.834-10.212-2.038-3.604-6.608-4.872-10.213-2.834l-17.243 9.752c-4.085-1.97-8.663-3.075-13.494-3.075-17.186 0-31.167 13.981-31.167 31.167 0 14.603 10.096 26.886 23.673 30.25v63.817c-11.175 2.769-19.987 11.581-22.756 22.756h-35.026c-3.365-13.577-15.648-23.673-30.25-23.673-17.186 0-31.167 13.981-31.167 31.167s13.981 31.167 31.167 31.167c14.602 0 26.886-10.096 30.25-23.673h35.024c3.365 13.577 15.648 23.673 30.25 23.673 5.556 0 10.773-1.468 15.295-4.027l68.745 38.878c-.231 1.528-.353 3.092-.353 4.684 0 14.602 10.096 26.886 23.673 30.25v35.023c-13.577 3.365-23.673 15.648-23.673 30.251 0 17.186 13.981 31.167 31.167 31.167s31.167-13.981 31.167-31.167c0-14.603-10.096-26.886-23.673-30.251v-35.023c13.577-3.365 23.673-15.648 23.673-30.25 0-1.592-.121-3.156-.353-4.684l69.294-39.188c4.644 2.751 10.056 4.337 15.834 4.337 4.94 0 9.613-1.16 13.767-3.215l25.082 25.082c-5.943 9.86-5.944 22.322 0 32.182l-25.721 25.721c-12.073-7.835-28.421-6.469-38.999 4.109-12.152 12.152-12.152 31.924 0 44.077 6.076 6.076 14.057 9.114 22.039 9.114s15.963-3.038 22.039-9.114c10.07-10.071 11.786-25.371 5.167-37.234l26.066-26.066c4.809 2.912 10.334 4.469 16.1 4.469 8.324 0 16.151-3.242 22.038-9.129 2.392-2.392 4.365-5.13 5.864-8.137 1.846-3.704.34-8.204-3.365-10.05-3.709-1.848-8.205-.34-10.051 3.365-.775 1.556-1.8 2.977-3.047 4.224-3.055 3.056-7.118 4.739-11.439 4.739-4.322 0-8.385-1.683-11.44-4.739-6.307-6.306-6.308-16.567-.005-22.876.002-.002.004-.003.005-.005s.003-.004.005-.005c5.29-5.282 13.437-6.27 19.815-2.4 3.537 2.147 8.147 1.019 10.295-2.52 2.147-3.538 1.019-8.147-2.52-10.294-10.104-6.13-22.496-5.931-32.246-.029l-24.197-24.197c3.717-5.132 5.917-11.431 5.917-18.238 0-14.603-10.096-26.886-23.673-30.25v-63.817c13.577-3.365 23.673-15.648 23.673-30.25 0-5.505-1.44-10.677-3.955-15.17l29.733-23.33c5.213 3.922 11.689 6.25 18.7 6.25 17.186 0 31.167-13.981 31.167-31.167 0-14.603-10.096-26.887-23.673-30.252zm-199.083 37.175c-8.921 0-16.179-7.258-16.179-16.179s7.258-16.179 16.179-16.179 16.179 7.258 16.179 16.179-7.258 16.179-16.179 16.179zm-131.033 56.493c0-8.921 7.258-16.179 16.179-16.179s16.179 7.258 16.179 16.179-7.258 16.179-16.179 16.179-16.179-7.258-16.179-16.179zm-79.345 140.496c-8.921 0-16.179-7.258-16.179-16.179s7.258-16.179 16.179-16.179 16.179 7.258 16.179 16.179-7.258 16.179-16.179 16.179zm79.345-16.178c0-8.921 7.258-16.179 16.179-16.179s16.179 7.258 16.179 16.179c0 8.765-7.409 16.179-16.179 16.179-8.921-.001-16.179-7.259-16.179-16.179zm147.212 166.226c0 8.921-7.258 16.179-16.179 16.179s-16.179-7.258-16.179-16.179 7.258-16.179 16.179-16.179 16.179 7.257 16.179 16.179zm-16.179-79.346c-8.921 0-16.179-7.258-16.179-16.179s7.258-16.179 16.179-16.179 16.179 7.258 16.179 16.179-7.258 16.179-16.179 16.179zm123.551 61.645c-6.306 6.307-16.57 6.31-22.88 0-6.309-6.309-6.309-16.573 0-22.881 3.154-3.154 7.297-4.731 11.439-4.731 4.143 0 8.286 1.577 11.44 4.731 6.31 6.309 6.31 16.573.001 22.881zm51.859-431.784c0-8.921 7.258-16.179 16.179-16.179s16.179 7.258 16.179 16.179-7.258 16.179-16.179 16.179-16.179-7.258-16.179-16.179zm-43.288 283.259c0 8.694-7.456 16.179-16.179 16.179-8.921 0-16.178-7.258-16.178-16.179s7.258-16.179 16.179-16.179c8.92 0 16.178 7.258 16.178 16.179zm-16.179-108.139c-8.921 0-16.179-7.258-16.179-16.179s7.258-16.179 16.179-16.179 16.179 7.258 16.179 16.179-7.258 16.179-16.179 16.179zm75.646-63.417c-8.921 0-16.179-7.258-16.179-16.179s7.258-16.179 16.179-16.179 16.179 7.258 16.179 16.179-7.258 16.179-16.179 16.179z"/></g></svg>`,
    //عنوان موارد انتخابی
    selectBoxTitle: "موارد انتخابی",
    //آیکون موارد انتخابی
    selectBoxIcon: `<svg version="1.1" id="Capa_1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px"\t viewBox="0 0 438.891 438.891" style="enable-background:new 0 0 438.891 438.891;" xml:space="preserve"><g>\t<g>\t\t<g>\t\t\t<path d="M347.968,57.503h-39.706V39.74c0-5.747-6.269-8.359-12.016-8.359h-30.824c-7.314-20.898-25.6-31.347-46.498-31.347\t\t\t\tc-20.668-0.777-39.467,11.896-46.498,31.347h-30.302c-5.747,0-11.494,2.612-11.494,8.359v17.763H90.923\t\t\t\tc-23.53,0.251-42.78,18.813-43.886,42.318v299.363c0,22.988,20.898,39.706,43.886,39.706h257.045\t\t\t\tc22.988,0,43.886-16.718,43.886-39.706V99.822C390.748,76.316,371.498,57.754,347.968,57.503z M151.527,52.279h28.735\t\t\t\tc5.016-0.612,9.045-4.428,9.927-9.404c3.094-13.474,14.915-23.146,28.735-23.51c13.692,0.415,25.335,10.117,28.212,23.51\t\t\t\tc0.937,5.148,5.232,9.013,10.449,9.404h29.78v41.796H151.527V52.279z M370.956,399.185c0,11.494-11.494,18.808-22.988,18.808\t\t\t\tH90.923c-11.494,0-22.988-7.314-22.988-18.808V99.822c1.066-11.964,10.978-21.201,22.988-21.42h39.706v26.645\t\t\t\tc0.552,5.854,5.622,10.233,11.494,9.927h154.122c5.98,0.327,11.209-3.992,12.016-9.927V78.401h39.706\t\t\t\tc12.009,0.22,21.922,9.456,22.988,21.42V399.185z"/>\t\t\t<path d="M179.217,233.569c-3.919-4.131-10.425-4.364-14.629-0.522l-33.437,31.869l-14.106-14.629\t\t\t\tc-3.919-4.131-10.425-4.363-14.629-0.522c-4.047,4.24-4.047,10.911,0,15.151l21.42,21.943c1.854,2.076,4.532,3.224,7.314,3.135\t\t\t\tc2.756-0.039,5.385-1.166,7.314-3.135l40.751-38.661c4.04-3.706,4.31-9.986,0.603-14.025\t\t\t\tC179.628,233.962,179.427,233.761,179.217,233.569z"/>\t\t\t<path d="M329.16,256.034H208.997c-5.771,0-10.449,4.678-10.449,10.449s4.678,10.449,10.449,10.449H329.16\t\t\t\tc5.771,0,10.449-4.678,10.449-10.449S334.931,256.034,329.16,256.034z"/>\t\t\t<path d="M179.217,149.977c-3.919-4.131-10.425-4.364-14.629-0.522l-33.437,31.869l-14.106-14.629\t\t\t\tc-3.919-4.131-10.425-4.364-14.629-0.522c-4.047,4.24-4.047,10.911,0,15.151l21.42,21.943c1.854,2.076,4.532,3.224,7.314,3.135\t\t\t\tc2.756-0.039,5.385-1.166,7.314-3.135l40.751-38.661c4.04-3.706,4.31-9.986,0.603-14.025\t\t\t\tC179.628,150.37,179.427,150.169,179.217,149.977z"/>\t\t\t<path d="M329.16,172.442H208.997c-5.771,0-10.449,4.678-10.449,10.449s4.678,10.449,10.449,10.449H329.16\t\t\t\tc5.771,0,10.449-4.678,10.449-10.449S334.931,172.442,329.16,172.442z"/>\t\t\t<path d="M179.217,317.16c-3.919-4.131-10.425-4.363-14.629-0.522l-33.437,31.869l-14.106-14.629\t\t\t\tc-3.919-4.131-10.425-4.363-14.629-0.522c-4.047,4.24-4.047,10.911,0,15.151l21.42,21.943c1.854,2.076,4.532,3.224,7.314,3.135\t\t\t\tc2.756-0.039,5.385-1.166,7.314-3.135l40.751-38.661c4.04-3.706,4.31-9.986,0.603-14.025\t\t\t\tC179.628,317.554,179.427,317.353,179.217,317.16z"/>\t\t\t<path d="M329.16,339.626H208.997c-5.771,0-10.449,4.678-10.449,10.449s4.678,10.449,10.449,10.449H329.16\t\t\t\tc5.771,0,10.449-4.678,10.449-10.449S334.931,339.626,329.16,339.626z"/>\t\t</g>\t</g></g></svg>`
};
let btnRefresh, iconSearch, mainTarget, mainModal, containerFrame, searchInput, containerCheckList, $tree, currentData;
const init = (data) => {
    currentData = [];
    prepareComponent(data);
    if (conf.showCrud) {
        let divTree = $("<div/>", {class: "col-xs-12 col-sm-12 col-md-12 tree"});
        let p = $("<p/>", {class: "text-title", html: `${conf.titleIcon} <span>${conf.title}<span/>`});
        let search = $("<input/>", {
            class: "search-tree",
            placeHolder: "جستجو ...",
            id: "search-box",
            "ksun-bundle-key":"",
            keyup: (e) => {
                if ((e.keyCode || e.which) === 13) {
                    if (!search.val()) {
                        mainTarget.html("");
                    }
                    let hand = new Handler();
                    hand.beforeSend = () => {
                    };
                    hand.success = () => {
                    };
                    initService(mainTarget, {}, hand, search.val(),0,null,false);
                }
            }
        });
        searchInput = search;
        iconSearch = $("<span/>", {
            class: "icon-search",
            html: `<i class="fas fa-search"></i>`,
            click: (e) => {
                if (!search.val()) {
                    mainTarget.html("");
                }
                let hand = new Handler();
                hand.beforeSend = () => {
                };
                hand.success = () => {
                };
                initService(mainTarget, {}, hand, search.val(),0,null,false);
            }
        });
        btnRefresh = $("<button/>", {
            class: "btn-refresh",
            html: `<i class="fas fa-sync-alt icon-refresh"></i>`,
            click: () => {
                searchInput.val("");
                iconSearch.click();
            }
        });
        p.append(search, iconSearch, btnRefresh);
        let treeContainer = $("<div/>", {class: "content-tree"});
        divTree.append(p);
        divTree.append(treeContainer);
        $tree.append(divTree);
        mainTarget = treeContainer;
        let modal = $("<div/>", {class: "modal bd-example-modal-lg", role: "dialog"});
        let modalDialog = $("<div/>", {class: "modal-dialog modal-lg", role: "document"});
        let modalContent = $("<div/>", {class: "modal-content"});
        let modalHeader = $("<div/>", {
            class: "modal-header", style: "background: #fafafa !important;",
            html: `<button type="button" class="close close-modal" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>`
        });
        let modalBody = $("<div/>", {class: "modal-body", style: "background: #fafafa !important;padding: 0;"});
        modalContent.append(modalHeader);
        modalContent.append(modalBody);
        modalDialog.append(modalContent);
        modal.append(modalDialog);
        $tree.append(modal);
        mainModal = modal;
        containerFrame = modalBody;

    }
    else {
        let divTree = $("<div/>", {class: "col-xs-12 col-sm-12 col-md-10 tree"});
        let p = $("<p/>", {class: "text-title", html: `${conf.titleIcon} <span>${conf.title}<span/>`});
        let search = $("<input/>", {
            class: "search-tree",
            placeHolder: "جستجو ...",
            id: "search-box",
            "ksun-bundle-key":"",
            keyup: (e) => {
                if ((e.keyCode || e.which) === 13) {
                    if (!search.val()) {
                        mainTarget.html("")
                    }
                    let hand = new Handler();
                    hand.beforeSend = () => {
                    };
                    hand.success = () => {
                    };
                    initService(mainTarget, {}, hand, search.val(),0,null,false);
                }
            }
        });
        searchInput = search;
        iconSearch = $("<span/>", {
            class: "icon-search",
            html: `<i class="fas fa-search"></i>`,
            click: (e) => {
                if (!search.val()) {
                    mainTarget.html("")
                }
                let hand = new Handler();
                hand.beforeSend = () => {
                };
                hand.success = () => {
                };
                initService(mainTarget, {}, hand, search.val(), 0,null,false);
            }
        });
        p.append(search, iconSearch);
        let treeContainer = $("<div/>", {class: "content-tree"});
        divTree.append(p);
        let divContainerData = $("<div/>", {
            class: "col-xs-12 col-sm-12 col-md-2 container-data",
            id: "container-checkbox-object"
        });
        let p2 = $("<p/>", {
            class: "text-title",
            html: `${conf.selectBoxIcon} <span>${conf.selectBoxTitle}<span/>`
        });
        $tree.append(divTree);
        divContainerData.append(p2);
        divTree.append(treeContainer);
        $tree.append(divContainerData);
        mainTarget = treeContainer;
        containerCheckList = divContainerData;
    }
    let hand = new Handler();
    hand.beforeSend = () => {
    };
    hand.success = () => {
    };
    initService($tree.find(".tree"), {}, hand, null,null,null,false);
}
const getCheckedList = () => {
    let data = [];
    containerCheckList.find(".content-check").each((index, item) => {
        data.push($(item).data().value);
    });
    return data;
}
const setCheckedList = (lst) => {
    containerCheckList.find("span").remove();
    $.each(lst, (i, item) => {
        let id = Util.uuid();
        let span = $("<span/>", {
            class: "content-check " + id,
            html: getTitleName(item[conf.showField])
        });
        let close = $("<span/>", {
            class: "close", html: `<i class="fas fa-times"></i>`, click: () => {
                $.each($tree.find("input[type=checkbox]"), (ind, val) => {
                    if ($(val).data().value[conf.rootKey] === item[conf.rootKey]) {
                        $(val).prop("checked", false);
                    }
                });
                span.remove();
            }
        });
        span.append(close);
        span.data({value: item});
        containerCheckList.append(span);
    });
    searchInput.val("");
    iconSearch.click();
}
const createTreeMenu = (target, data, level,addChild) => {
    if (conf.getTotal) {
        if ($(target).hasClass("item")) {
            createFullTree(target, data);
        } else {
            createFullTree(mainTarget, data, level);
        }
    } else {
        if (!mainTarget.html()) {
            mainTarget.append(createSimpleMenu(data, true, level));
        } else {
            if(addChild){
                let ul = createSimpleMenu(data, false, level);
                ul.find(".lazy").remove();
                let mainLi = $(target).closest("ul").find(".lazy");
                $.each(ul.find("li"),(i,v)=>{
                    mainLi.before(v);
                });
                mainLi.find(".loading-item").fadeOut();
                if(data.length < conf.countShow){
                    $(target).closest("ul").find(".lazy").remove();
                }
            }else{
                $(target).append(createSimpleMenu(data, false, level));
            }
        }
    }
}
const createSimpleMenu = (data, root, level) => {
    if (level === undefined) {
        level = 0;
    }
    let titleText = conf.titleTree[level || 0] || "";
    let ul = $("<ul/>");
    if (root) {
        ul.addClass("root-tree");
    } else {
        ul.addClass("child-tree");
        ul.attr("style", "display:none");
    }
    $.each(data, (index, item) => {
        let li = $("<li/>", {
            class: "item"
        });
        let loading = $("<div/>", {
            class: "loading-item"
        })
        li.data().value = item;
        let div = $("<div/>", {class: "container-item"});
        let span = $("<span/>", {
            class: "tree-open",
            click: (e) => {
                e.stopPropagation();
                e.preventDefault();
                if (li.find("ul:eq(0)").length === 0) {
                    let hand = new Handler();
                    loading.fadeIn();
                    hand.beforeSend = () => {
                    };
                    hand.success = () => {
                        if (!span.hasClass("down")) {
                            span.addClass("down");
                        } else {
                            span.removeClass("down");
                        }
                        loading.fadeOut();
                        toggleTarget(li);
                    }
                    initService(li, item, hand, "", level + 1,null,false);
                } else {
                    toggleTarget(li);
                    if (span.hasClass("down")) {
                        span.removeClass("down");
                    } else {
                        span.addClass("down");
                    }
                }
            }
        });
        let flagAddSpan = true;
        if (item[conf.childKeyForShowSquare]) {
            if (item[conf.childKeyForShowSquare] !== conf.childKeyValueForShowSquare) {
                flagAddSpan = false;
            }
        }
        if (flagAddSpan) {
            div.append(span);
        }
        if (!conf.showCrud) {
            let id = Util.uuid();
            let formGroup = $("<div/>", {class: "form-group floating-form-group"});
            let wrapper = $("<div/>", {
                class: "floating-form-checkbox-wrapper",
                style: `background-color:${conf.colors[level] ? conf.colors[level][0] : conf.colors[0][0]};`
            });
            let input = $("<input/>", {
                type: "checkbox",
                class: "form-control form-control-sm floating-form-control",
                id: id,
                "ksun-bundle-key":"",
                click: (e) => {
                    if (input.prop("checked") === true) {
                        let span = $("<span/>", {
                            class: "content-check " + id,
                            html: getTitleName(item[conf.showField])
                        });
                        let close = $("<span/>", {
                            class: "close", html: `<i class="fas fa-times"></i>`, click: () => {
                                $.each($tree.find("input[type=checkbox]"), (ind, val) => {
                                    if ($(val).data().value[conf.rootKey] === item[conf.rootKey]) {
                                        $(val).prop("checked", false);
                                    }
                                });
                                span.remove();
                            }
                        });
                        span.append(close);
                        span.data({value: item});
                        containerCheckList.append(span);
                    } else {
                        $.each(containerCheckList.find(".content-check"), (ind, val) => {
                            if ($(val).data().value[conf.rootKey] === item[conf.rootKey]) {
                                $(val).fadeOut(300);
                                setTimeout(() => {
                                    $(val).remove();
                                }, 1000);
                            }
                        });
                    }
                    if (conf.selectAllChildWhenSelectParent) {
                        li.find("input").each((i, v) => {
                            $(v).prop("checked", input.prop("checked"));
                            if ($(v).prop("checked") === true) {
                                let span = $("<span/>", {
                                    class: "content-check " + $(v).attr("id"),
                                    html: getTitleName($(v).closest("li").data().value[conf.showField])
                                });
                                let close = $("<span/>", {
                                    class: "close", html: `<i class="fas fa-times"></i>`, click: () => {
                                        $.each($tree.find("input[type=checkbox]"), (ind, val) => {
                                            if ($(val).data().value[conf.rootKey] === $(v).data().value[conf.rootKey]) {
                                                $(val).prop("checked", false);
                                            }
                                        });
                                        span.remove();
                                    }
                                });
                                span.append(close);
                                span.data({value: $(v).closest("li").data().value});
                                let flag = true;
                                $.each(containerCheckList.find(".content-check"), (i, val) => {
                                    if ($(val).data().value === $(v).closest("li").data().value) {
                                        flag = false;
                                    }
                                });
                                if (flag) {
                                    containerCheckList.append(span);
                                }
                            } else {
                                $.each(containerCheckList.find(".content-check"), (ind, val) => {
                                    if ($(val).data().value[conf.rootKey] === $(v).data().value[conf.rootKey]) {
                                        $(val).fadeOut(300);
                                        setTimeout(() => {
                                            $(val).remove();
                                        }, 1000);
                                    }
                                });

                            }
                        });
                    }
                }
            });
            input.data().value = item;
            $.each(containerCheckList.find(".content-check"), (ind, val) => {
                if ($(val).data().value[conf.rootKey] === item[conf.rootKey]) {
                    input.prop("checked", true);
                }
            });
            let label = $("<label/>", {class: "floating-form-checkbox", for: id});
            let lbl = $("<label/>", {
                class: "floating-form-checkbox-label",
                for: id,
                style: `color:${conf.colors[level] ? conf.colors[level][1] : conf.colors[0][1]};`,
                html: titleText + getTitleName(item[conf.showField])
            });
            if (level >= conf.selectState) {
                wrapper.append(input);
                wrapper.append(label);
            }
            wrapper.append(lbl);
            formGroup.append(wrapper);
            formGroup.append(loading);
            div.append(formGroup);
        } else {
            let formGroup = $("<div/>", {class: "form-group floating-form-group"});
            let wrapper = $("<div/>", {
                class: "floating-form-checkbox-wrapper",
                style: `background-color:${conf.colors[level] ? conf.colors[level][0] : conf.colors[0][0]};`
            });
            let lbl = $("<label/>", {
                class: "floating-form-checkbox-label",
                style: `color:${conf.colors[level] ? conf.colors[level][1] : conf.colors[0][1]};`,
                html: titleText + getTitleName(item[conf.showField])
            });
            if (getTitleName(item[conf.showField]).indexOf(searchInput.val().trim()) !== -1 && searchInput.val().trim() !== "") {
                lbl.addClass("text-success");
            }
            lbl.data().value = item;
            let spanRemove = $("<span/>", {
                class: "btn-icon btn-icon-remove",
                html: `<i class="fas fa-trash" style="color:${conf.colors[level] ? conf.colors[level][1] : conf.colors[0][1]}"></i>`,
                click: () => {
                    swal({
                        title: "حذف",
                        text: "آیا از حذف این رکورد اطمینان دارید",
                        showCancelButton: true,
                        confirmButtonText: "بله! حذف کن",
                        cancelButtonText: "انصراف",
                        showLoaderOnConfirm: true
                    }, function (res) {
                        if (res) {
                            removeItem(item);
                        }
                    });
                }
            });
            let spanEdit = $("<span/>", {
                class: "btn-icon btn-icon-edit",
                html: `<i class="fas fa-edit" style="color:${conf.colors[level] ? conf.colors[level][1] : conf.colors[0][1]}"></i>`,
                style: `color:${conf.colors[level] ? conf.colors[level][1] : conf.colors[0][1]};`,
                click: () => {
                    showEditPage(item);
                }
            });
            let spanAdd = $("<span/>", {
                class: "btn-icon btn-icon-add",
                html: `<i class="fas fa-plus" style="color:${conf.colors[level] ? conf.colors[level][1] : conf.colors[0][1]}"></i>`,
                style: `color:${conf.colors[level] ? conf.colors[level][1] : conf.colors[0][1]};`,
                click: () => {
                    showAddPage(item);
                }
            });
            wrapper.append(lbl);
            wrapper.append(spanRemove);
            wrapper.append(spanEdit);
            wrapper.append(spanAdd);
            formGroup.append(wrapper);
            div.append(formGroup);
        }
        li.append(div);
        ul.append(li);
    });
    if (conf.countShow !== -1 && !root && data && data.length >= conf.countShow) {
        let li = $("<li/>", {
            class: "item lazy"
        });
        let loading = $("<div/>", {
            class: "loading-item"
        })
        let div = $("<div/>", {class: "container-item"});
        let span = $("<span/>", {
            class: "tree-open",
            click: (e) => {
                e.stopPropagation();
                e.preventDefault();
                let hand = new Handler();
                loading.fadeIn();
                hand.beforeSend = () => {
                };
                hand.success = () => {
                }
                let data = li.closest("ul").closest("li").data();
                if (data) {
                    let count = li.closest("ul").find("li").length || 1;
                    initService(li.closest("ul"), data.value, hand, "", level,Number(count),true);
                }
            }
        });
        div.append(span);
        let formGroup = $("<div/>", {class: "form-group floating-form-group"});
        let wrapper = $("<div/>", {
            class: "floating-form-checkbox-wrapper",
        });
        let lbl = $("<label/>", {
            class: "floating-form-checkbox-label text-danger",
            html: conf.titleShow
        });
        wrapper.append(lbl);
        formGroup.append(wrapper);
        formGroup.append(loading);
        div.append(formGroup);
        li.append(div);
        ul.append(li);
    }
    return ul;
}
const createFullTree = (target, data, level) => {
    let root = [];
    $.each(data, (i, v) => {
        $.each(conf.rootValue, (ind, val) => {
            if (v[conf.rootKey] === val) {
                root.push(v);
                return false;
            }
        });
    });
    $(target).html(createSearchItem(root, true, data, level));
};
const createSearchItem = (data, isRoot, totalData, level) => {
    if (level === undefined) {
        level = 0;
    }
    let titleText = conf.titleTree[level || 0] || "";
    let ul = $("<ul/>");
    if (isRoot) {
        ul.addClass("root-tree");
    } else {
        ul.addClass("child-tree");
        if (conf.openWholeTreeInGetTotal === false && searchInput.val() === "" && conf.getTotal === true) {
            ul.attr("style", "display:none");
        }
    }
    $.each(data, (index, item) => {
        let li = $("<li/>", {
            class: "item"
        });
        let loading = $("<div/>",
            {
                class: "loading-item"
            });
        li.data().value = item;
        let div = $("<div/>", {class: "container-item"});
        let span = $("<span/>", {
            class: "tree-open down",
            click: (e) => {
                e.stopPropagation();
                e.preventDefault();
                toggleTarget(li);
                if (span.hasClass("down")) {
                    span.removeClass("down");
                } else {
                    span.addClass("down");
                }
            }
        });
        if (conf.openWholeTreeInGetTotal === false && searchInput.val() === "" && conf.getTotal === true) {
            span.removeClass("down");
        }
        let flagAddSpan = true;
        if (item[conf.childKeyForShowSquare]) {
            if (item[conf.childKeyForShowSquare] !== conf.childKeyValueForShowSquare) {
                flagAddSpan = false;
            }
        }
        if (flagAddSpan) {
            div.append(span);
        }
        if (!conf.showCrud) {
            let id = Util.uuid();
            let formGroup = $("<div/>", {class: "form-group floating-form-group"});
            let wrapper = $("<div/>", {
                class: "floating-form-checkbox-wrapper",
                style: `background-color:${conf.colors[level] ? conf.colors[level][0] : conf.colors[0][0]};`
            });
            let input = $("<input/>", {
                type: "checkbox",
                class: "form-control form-control-sm floating-form-control",
                id: id,
                "ksun-bundle-key":"",
                click: (e) => {
                    if (input.prop("checked") === true) {
                        let span = $("<span/>", {
                            class: "content-check " + id,
                            html: getTitleName(item[conf.showField])
                        });
                        let close = $("<span/>", {
                            class: "close", html: `<i class="fas fa-times"></i>`, click: () => {
                                $.each($tree.find("input[type=checkbox]"), (ind, val) => {
                                    if ($(val).data().value[conf.rootKey] === item[conf.rootKey]) {
                                        $(val).prop("checked", false);
                                    }
                                });
                                span.remove();
                            }
                        });
                        span.append(close);
                        span.data({value: item});
                        containerCheckList.append(span);
                    } else {
                        $.each(containerCheckList.find(".content-check"), (ind, val) => {
                            if ($(val).data().value[conf.rootKey] === item[conf.rootKey]) {
                                $(val).fadeOut(300);
                                setTimeout(() => {
                                    $(val).remove();
                                }, 1000);
                            }
                        });
                    }
                    if (conf.selectAllChildWhenSelectParent) {
                        li.find("input").each((i, v) => {
                            $(v).prop("checked", input.prop("checked"));
                            if ($(v).prop("checked") === true) {
                                let span = $("<span/>", {
                                    class: "content-check " + $(v).attr("id"),
                                    html: getTitleName($(v).closest("li").data().value[conf.showField])
                                });
                                let close = $("<span/>", {
                                    class: "close", html: `<i class="fas fa-times"></i>`, click: () => {
                                        $.each($tree.find("input[type=checkbox]"), (ind, val) => {
                                            if ($(val).data().value[conf.rootKey] === $(v).data().value[conf.rootKey]) {
                                                $(val).prop("checked", false);
                                            }
                                        });
                                        span.remove();
                                    }
                                });
                                span.append(close);
                                span.data({value: $(v).closest("li").data().value});
                                let flag = true;
                                $.each(containerCheckList.find(".content-check"), (i, val) => {
                                    if ($(val).data().value === $(v).closest("li").data().value) {
                                        flag = false;
                                    }
                                });
                                if (flag) {
                                    containerCheckList.append(span);
                                }
                            } else {
                                $.each(containerCheckList.find(".content-check"), (ind, val) => {
                                    if ($(val).data().value[conf.rootKey] === $(v).data().value[conf.rootKey]) {
                                        $(val).fadeOut(300);
                                        setTimeout(() => {
                                            $(val).remove();
                                        }, 1000);
                                    }
                                });

                            }
                        });
                    }
                }
            });
            input.data().value = item;
            $.each(containerCheckList.find(".content-check"), (ind, val) => {
                if ($(val).data().value[conf.rootKey] === item[conf.rootKey]) {
                    input.prop("checked", true);
                }
            });
            let label = $("<label/>", {class: "floating-form-checkbox", for: id});
            let lbl = $("<label/>", {
                class: "floating-form-checkbox-label",
                for: id,
                style: `color:${conf.colors[level] ? conf.colors[level][1] : conf.colors[0][1]};`,
                html: titleText + getTitleName(item[conf.showField])
            });
            if (getTitleName(item[conf.showField]).indexOf(searchInput.val().trim()) !== -1 && searchInput.val().trim() !== "") {
                lbl.addClass("text-success");
            }
            if (searchInput.val()) {
                wrapper.append(input);
                wrapper.append(label);
            } else {
                if (level >= conf.selectState) {
                    wrapper.append(input);
                    wrapper.append(label);
                }
            }
            wrapper.append(lbl);
            formGroup.append(wrapper);
            formGroup.append(loading);
            div.append(formGroup);
        }
        else {
            let formGroup = $("<div/>", {class: "form-group floating-form-group"});
            let wrapper = $("<div/>", {
                class: "floating-form-checkbox-wrapper",
                style: `background-color:${conf.colors[level] ? conf.colors[level][0] : conf.colors[0][0]};`
            });
            let lbl = $("<label/>", {
                class: "floating-form-checkbox-label",
                style: `color:${conf.colors[level] ? conf.colors[level][1] : conf.colors[0][1]};`,
                html: titleText + getTitleName(item[conf.showField])
            });
            if (getTitleName(item[conf.showField]).indexOf(searchInput.val().trim()) !== -1 && searchInput.val().trim() !== "") {
                lbl.addClass("text-success");
            }
            lbl.data().value = item;
            let spanRemove = $("<span/>", {
                class: "btn-icon btn-icon-remove",
                html: `<i class="fas fa-trash"  style="color:${conf.colors[level] ? conf.colors[level][1] : conf.colors[0][1]}"></i>`,
                click: () => {
                    swal({
                        title: "حذف",
                        text: "آیا از حذف این رکورد اطمینان دارید",
                        showCancelButton: true,
                        confirmButtonText: "بله! حذف کن",
                        cancelButtonText: "انصراف",
                        showLoaderOnConfirm: true
                    }, function (res) {
                        if (res) {
                            removeItem(item);
                        }
                    });
                }
            });
            let spanEdit = $("<span/>", {
                class: "btn-icon btn-icon-edit",
                html: `<i class="fas fa-edit"  style="color:${conf.colors[level] ? conf.colors[level][1] : conf.colors[0][1]}"></i>`,
                click: () => {
                    showEditPage(item);
                }
            });
            let spanAdd = $("<span/>", {
                class: "btn-icon btn-icon-add",
                html: `<i class="fas fa-plus"  style="color:${conf.colors[level] ? conf.colors[level][1] : conf.colors[0][1]}"></i>`,
                click: () => {
                    showAddPage(item);
                }
            });
            wrapper.append(lbl);
            wrapper.append(spanRemove);
            wrapper.append(spanEdit);
            wrapper.append(spanAdd);
            formGroup.append(wrapper);
            div.append(formGroup);
        }
        li.append(div);
        li.append(createSearchItem(getChild(item, totalData), false, totalData, level + 1));
        ul.append(li);
    });
    return ul;
}
const getChild = (root, data) => {
    let lstData = [];
    let child = conf.childKey.split(".");
    let val = "";
    let lstCheckField = [];
    $.each(child, (i, v) => {
        val += `["${v}"]`;
        lstCheckField.push(val);
    });
    $.each(data, (index, item) => {
        let flag = true;
        $.each(lstCheckField, (i, v) => {
            try {
                if (!eval("item" + v)) {
                    flag = false;
                    return false;
                }
            } catch (e) {
                debugger;
            }
        });
        if (flag) {
            if (eval("item" + val) === root[conf.rootKey]) {
                lstData.push(item);
            }
        }
    });
    return lstData;
}
const initService = (target, obj, hand, term, level,pageNumber,addChild) => {
    let handler = new Handler();
    if(!pageNumber && conf.doPagination === true){
        pageNumber = 0;
    }else if (!pageNumber && conf.doPagination === false){
        pageNumber = -1;
    }
    if (term) {
        handler.success = (data) => {
            if (data.done) {
                if (top.hideLoading) {
                    top.hideLoading();
                }
                if (data.resultCountAll !== 0) {
                    let root = [];
                    $.each(data.result, (i, v) => {
                        $.each(conf.rootValue, (ind, val) => {
                            if (v[conf.rootKey] === val) {
                                root.push(v);
                                return false;
                            }
                        });
                    });
                    mainTarget.html(createSearchItem(root, true, data.result, level));
                } else {
                    swal("پیغام سیستمی", "اطلاعاتی یافت نشد", "info");
                }
            } else {
                if (data.done === false) {
                    if (top.hideLoading) {
                        top.hideLoading();
                    }
                    errorHandler(data);
                } else {
                    swal("پیغام سیستمی", data, "info");
                }
            }
            hand.success();
        }
        Api.post({
            url: conf.serviceUrl + "/" + conf.getTotal + "/" + term + "/" + pageNumber,
            handler: handler,
            data: obj,
            async: false
        });
    } else {
        handler.success = (data) => {
            if (data.done) {
                if (top.hideLoading) {
                    top.hideLoading();
                }
                createTreeMenu(target, data.result, level,addChild);
            } else {
                if (data.done === false) {
                    if (top.hideLoading) {
                        top.hideLoading();
                    }
                    errorHandler(data);
                } else {
                    swal("پیغام سیستمی", data, "info");
                }
            }
            hand.success();
        }
        Api.post({
            url: conf.serviceUrl + "/" + conf.getTotal + "/-/" + pageNumber,
            handler: handler,
            data: obj,
            async: false
        });
    }
}
const reCreateNode = (node, action) => {
    if (action === conf.action.delete) {
        $.each($tree.find(".floating-form-checkbox-label"), (ind, val) => {
            if ($(val).data().value[conf.rootKey] === node[conf.rootKey]) {
                $(val).closest("li").remove();
                return false;
            }
        });
        swal("پیغام سیستمی", "رکورد با موفقیت حذف گردید", "info");
    } else {
        if (conf.autoRefreshDataAfterSaveOrUpdate === true) {
            btnRefresh.click();
        } else {
            swal("پیغام سیستمی", conf.messageForUpdate, "info");
        }
    }
};
const removeItem = (obj) => {
    let handler = new Handler();
    handler.success = (data) => {
        if (data.done) {
            if (top.hideLoading) {
                top.hideLoading();
            }
            reCreateNode(obj, conf.action.delete);
        } else {
            if (top.hideLoading) {
                top.hideLoading();
            }
            errorHandler(data);
        }
    }
    let deleteWheres = new Wheres();
    let deleteFilter = new Filter();
    deleteWheres.add(conf.clauseField, obj[conf.clauseField], ENVIRONMENT.Condition.EQUAL);
    deleteFilter.setWheres(deleteWheres.getList());
    Api.delete({url: conf.removeService, handler: handler, filter: deleteFilter.get(), async: false});
}
const showEditPage = (obj) => {
    let clickEdit = false;
    containerFrame.html("");
    let iframe = $("<iframe/>", {
        class: "main-frame",
        id: "main-frame",
        src: conf.urlEditPage + "?eID=" + obj[conf.clauseField]
    });
    containerFrame.html(iframe);
    iframe.on("load", function () {
        if (conf.customInsert === true) {
            let split = obj[conf.showField].split("|");
            let indexFinder = -1;
            for (let i = split.length - 1; i >= 0; i--) {
                if (split[i] !== "*") {
                    indexFinder = i;
                    break;
                }
            }
            setTimeout(() => {
                mainModal.modal("show");
                switch (Number(indexFinder)) {
                    case 1 :
                        iframe.contents().find("#vendor").closest(".floating-form-group").removeClass("d-none");
                        iframe.contents().find("#vendor").addClass("floating-mode");
                        iframe.contents().find("#vendor").focus();
                        break;
                    case 2 :
                        iframe.contents().find("#product").closest(".floating-form-group").removeClass("d-none");
                        iframe.contents().find("#product").addClass("floating-mode");
                        iframe.contents().find("#product").focus();
                        break;
                    case 3 :
                        iframe.contents().find("#version").closest(".floating-form-group").removeClass("d-none");
                        iframe.contents().find("#version").addClass("floating-mode");
                        iframe.contents().find("#version").focus();
                        break;
                    case 4 :
                        iframe.contents().find("#updatedVersion").closest(".floating-form-group").removeClass("d-none");
                        iframe.contents().find("#updatedVersion").addClass("floating-mode");
                        iframe.contents().find("#updatedVersion").focus();
                        break;
                    case 5 :
                        iframe.contents().find("#edition").closest(".floating-form-group").removeClass("d-none");
                        iframe.contents().find("#edition").addClass("floating-mode");
                        iframe.contents().find("#edition").focus();
                        break;
                    case 6 :
                        iframe.contents().find("#otherFeatures").closest(".floating-form-group").removeClass("d-none");
                        iframe.contents().find("#otherFeatures").addClass("floating-mode");
                        iframe.contents().find("#otherFeatures").focus();
                        break;
                    case 0:
                        iframe.contents().find("#edit-submit").text("امکان ویرایش این سطح وجود ندارد").removeClass("deep-purple").addClass("btn-danger").attr("disabled", "disabled");
                        break;
                }
            }, 1000);
            mainModal.off("hidden.bs.modal");
            mainModal.on("hidden.bs.modal", function () {
                if (clickEdit === true) {
                    reCreateNode(obj, conf.action.insert);
                }
            });
            iframe.contents().find("header").remove();
            iframe.contents().find("#edit-clear").remove();
            iframe.contents().find("#edit-submit").click(() => {
                clickEdit = true;
            });

        } else {
            mainModal.modal("show");
            iframe.contents().find("header").remove();
            iframe.contents().find("#edit-clear").remove();
            iframe.contents().find("#edit-submit").click(() => {
                clickEdit = true;
            });
            mainModal.off("hidden.bs.modal");
            mainModal.on("hidden.bs.modal", function () {
                if (clickEdit === true) {
                    reCreateNode(obj, conf.action.update);
                }
            });
        }

    });
}
const showAddPage = (obj) => {
    let clickAdd = false;
    containerFrame.html("");
    let iframe = $("<iframe/>", {
        class: "main-frame",
        id: "main-frame",
        src: conf.urlEditPage
    });
    if (conf.customInsert === true) {
        iframe.attr("src", iframe.attr("src") + "?eID=" + obj[conf.clauseField]);
        containerFrame.html(iframe);
        iframe.on("load", function () {
            let split = obj[conf.showField].split("|");
            let indexFinder = -1;
            for (let i = split.length - 1; i >= 0; i--) {
                if (split[i] !== "*") {
                    indexFinder = i;
                    break;
                }
            }
            setTimeout(() => {
                iframe.contents().find("#91970944605360").val("");
                iframe.contents().find("#edit-submit").text("ثبت");
                iframe.contents().find("#91970944605360").data().entityId = null;
                iframe.contents().find("#91970944605360").removeClass("floating-mode");
                mainModal.modal("show");
                switch (Number(indexFinder)) {
                    case 0 :
                        iframe.contents().find("#vendor").closest(".floating-form-group").removeClass("d-none");
                        iframe.contents().find("#vendor").val("");
                        iframe.contents().find("#vendor").addClass("floating-mode");
                        iframe.contents().find("#vendor").focus();
                        break;
                    case 1 :
                        iframe.contents().find("#product").closest(".floating-form-group").removeClass("d-none");
                        iframe.contents().find("#product").val("");
                        iframe.contents().find("#product").addClass("floating-mode");
                        iframe.contents().find("#product").focus();
                        break;
                    case 2 :
                        iframe.contents().find("#version").closest(".floating-form-group").removeClass("d-none");
                        iframe.contents().find("#version").val("");
                        iframe.contents().find("#version").addClass("floating-mode");
                        iframe.contents().find("#version").focus();
                        break;
                    case 3 :
                        iframe.contents().find("#updatedVersion").closest(".floating-form-group").removeClass("d-none");
                        iframe.contents().find("#updatedVersion").val("");
                        iframe.contents().find("#updatedVersion").addClass("floating-mode");
                        iframe.contents().find("#updatedVersion").focus();
                        break;
                    case 4 :
                        iframe.contents().find("#edition").closest(".floating-form-group").removeClass("d-none");
                        iframe.contents().find("#edition").val("");
                        iframe.contents().find("#edition").addClass("floating-mode");
                        iframe.contents().find("#edition").focus();
                        break;
                    case 5 :
                        iframe.contents().find("#otherFeatures").closest(".floating-form-group").removeClass("d-none");
                        iframe.contents().find("#otherFeatures").val("");
                        iframe.contents().find("#otherFeatures").addClass("floating-mode");
                        iframe.contents().find("#otherFeatures").focus();
                        break;
                    case 6:
                        iframe.contents().find("#edit-submit").text("امکان افزودن مقدار در این سطح وجود ندارد").removeClass("deep-purple").addClass("btn-danger").attr("disabled", "disabled");
                        break;
                }
                iframe.contents().find(conf.elementIdInEditPageForFillParent).html("");
                iframe.contents().find(conf.elementIdInEditPageForFillParent).append($('<option/>', {
                    value: obj[conf.clauseField],
                    text: getTitleName(obj[conf.showField]),
                    data: {
                        object: obj
                    }
                })).trigger('change.select2');
            }, 1000);
            mainModal.off("hidden.bs.modal");
            mainModal.on("hidden.bs.modal", function () {
                if (clickAdd === true) {
                    reCreateNode(obj, conf.action.insert);
                }
            });
            iframe.contents().find("header").remove();
            iframe.contents().find("#edit-clear").remove();
            iframe.contents().find("#edit-submit").click(() => {
                clickAdd = true;
            });
        });
    } else {
        containerFrame.html(iframe);
        iframe.on("load", function () {
            iframe.contents().find(conf.elementIdInEditPageForFillParent).append($('<option/>', {
                value: obj[conf.clauseField],
                text: getTitleName(obj[conf.showField]),
                data: {
                    object: obj
                }
            })).trigger('change.select2');
            iframe.contents().find("header").remove();
            iframe.contents().find("#edit-clear").remove();
            iframe.contents().find("#edit-submit").click(() => {
                clickAdd = true;
            });
            mainModal.modal("show");
            mainModal.off("hidden.bs.modal");
            mainModal.on("hidden.bs.modal", function () {
                if (clickAdd === true) {
                    reCreateNode(obj, conf.action.insert);
                }
            });
        });
    }
}
const toggleTarget = (elm) => {
    $(elm).find(".child-tree:eq(0)").slideToggle(300);
}
const prepareComponent = (data) => {
    if (data) {
        if (data.dependencies && data.dependencies.link) {
            conf.dependencies.link = data.dependencies.link;
        }
        if (data.dependencies && data.dependencies.script) {
            conf.dependencies.script = data.dependencies.script;
        }
        if (data.element) {
            conf.element = data.element;
        }
        if (data.colors) {
            conf.colors = data.colors;
        }
        if (data.doPagination === true || data.doPagination === false) {
            conf.doPagination = data.doPagination;
        }
        if (data.serviceUrl) {
            conf.serviceUrl = data.serviceUrl;
        }
        if (data.removeService) {
            conf.removeService = data.removeService;
        }
        if (data.urlEditPage) {
            conf.urlEditPage = data.urlEditPage;
        }
        if (data.childKeyForShowSquare) {
            conf.childKeyForShowSquare = data.childKeyForShowSquare;
        }
        if (data.childKeyValueForShowSquare) {
            conf.childKeyValueForShowSquare = data.childKeyValueForShowSquare;
        }
        if (data.clauseField) {
            conf.clauseField = data.clauseField;
        }
        if (data.elementIdInEditPageForFillParent) {
            conf.elementIdInEditPageForFillParent = data.elementIdInEditPageForFillParent;
        }
        if (data.getTotal === true || data.getTotal === false) {
            conf.getTotal = data.getTotal;
        }
        if (data.customInsert === true || data.customInsert === false) {
            conf.customInsert = data.customInsert;
        }
        if (data.rootKey) {
            conf.rootKey = data.rootKey;
        }
        if (data.rootValue) {
            conf.rootValue = data.rootValue;
        }
        if (data.childKey) {
            conf.childKey = data.childKey;
        }
        if (data.selectState) {
            conf.selectState = data.selectState;
        }
        if (data.showCrud === true || data.showCrud === false) {
            conf.showCrud = data.showCrud;
        }
        if (data.autoRefreshDataAfterSaveOrUpdate === true || data.autoRefreshDataAfterSaveOrUpdate === false) {
            conf.autoRefreshDataAfterSaveOrUpdate = data.autoRefreshDataAfterSaveOrUpdate;
        }
        if (data.messageForUpdate) {
            conf.messageForUpdate = data.messageForUpdate;
        }
        if (data.showField) {
            conf.showField = data.showField;
        }
        if (data.functionData) {
            conf.functionData = data.functionData;
        }
        if (data.selectAllChildWhenSelectParent === true || data.selectAllChildWhenSelectParent === false) {
            conf.selectAllChildWhenSelectParent = data.selectAllChildWhenSelectParent;
        }
        if (data.openWholeTreeInGetTotal === true || data.openWholeTreeInGetTotal === false) {
            conf.openWholeTreeInGetTotal = data.openWholeTreeInGetTotal;
        }
        if (data.title) {
            conf.title = data.title;
        }
        if (data.titleIcon) {
            conf.titleIcon = data.titleIcon;
        }
        if (data.selectBoxTitle) {
            conf.selectBoxTitle = data.selectBoxTitle;
        }
        if (data.selectBoxIcon) {
            conf.selectBoxIcon = data.selectBoxIcon;
        }
        if (data.titleTree) {
            conf.titleTree = data.titleTree;
        }
        if (data.countShow !== undefined) {
            conf.countShow = data.countShow;
        }
        if (data.titleShow) {
            conf.titleShow = data.titleShow;
        }
    }
    $tree = $(conf.element);
    $tree.addClass("v_tree");
    $.each(conf.dependencies.link, (i, v) => {
        $('head').append(`<link rel="stylesheet" type="text/css" href="${v}">`);
    });
    $.each(conf.dependencies.script, (i, v) => {
        Util.scriptImporter.url(v);
    });
}
const getTitleName = (name) => {
    let treeName = name;
    if (conf.functionData) {
        var x = (data) => {
            return data
        };
        eval("x =" + conf.functionData);
        treeName = x(treeName);
    }
    return treeName;
}
export {init, getCheckedList, setCheckedList};