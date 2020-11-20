/**
 * Created by Farshad Kazemi on 2/18/2018.
 */


// variable(s)
var pageNumberInput, pageSizeCombo;
var prevPageButton, nextPageButton, paginationWrapper;
var table, tableBody, tableHeader, patternRow, headerCheckbox, selectAll, actions, selectedRows = [];
var columns = [];
var sortedColumnCombo, orderTypeCombo, sortedBadgesWrapper;
var pageNumber = 1, pageSize = 10, lastPageNumber = 1, hasNextPage = false;
var localVariables;
var URLs;
var gridHandler, gridWheres, gridFilter, gridOrderByList;
var deleteHandler, deleteFilter;
var pageLoadingModal, pageConfirmModal;
var componentID, businessAccess;
var simpleSearchInput, simpleSearchButton, newButton, searchBar;
var editFrameWrapper, editFrame, datePickerList = {};
var filters, filterSearchBtn, dataContainer;
var initViews = function() {
    newButton = $("#new");
    filters = $(".btn-filter");
    filterSearchBtn = $(".filter-search-btn");
    dataContainer = $(".data-container");
    businessAccess = $("#business-access");
    simpleSearchInput = $("#simple-search");
    simpleSearchButton = $("#simple-search-btn");

    searchBar = $(".search-bar");
    table = $("table.main-table");
    tableHeader = table.find("thead");
    tableBody = table.find("tbody");
    patternRow = tableBody.find(".pattern-row").clone();
    headerCheckbox = $("#header-checkbox");
    selectAll = $("#select-all"); // in grid card view
    sortedColumnCombo = $("#sorted-column");
    orderTypeCombo = $("#order-type");
    sortedBadgesWrapper = $(".badges-wrapper");
    pageNumberInput = $("#page-number");
    pageSizeCombo = $("#page-size");
    prevPageButton = $(`[data-pagination-key=${ENVIRONMENT.PaginationKey.PREV}]`).find("a.page-link");
    nextPageButton = $(`[data-pagination-key=${ENVIRONMENT.PaginationKey.NEXT}]`).find("a.page-link");
    paginationWrapper = $(".ksun-pagination-wrapper");
    pageLoadingModal = $("#loading-modal");
    pageConfirmModal = $("#confirm-modal");
    editFrameWrapper = $(".edit-frame-wrapper");
    editFrame = $(".edit-frame");
};
var initInstances = function() {
    componentID = Util.urlParameter("id");
    if (localVariables) {
        if (localVariables.actions) {
            actions = localVariables.actions;
        } else {
            actions = [
                { key: "EDIT", title: GeneralBundle["$edit"], icon: "fas fa-pencil-alt" },
                { key: "DELETE", title: GeneralBundle["$delete"], icon: "fas fa-times" }
            ];
        }
        if (!localVariables.clazzName) {
            localVariables.clazzName = "";
        }
        if (localVariables.URLs) {
            if (!localVariables.URLs.edit) {
                localVariables.URLs.edit = Config.PATH + "edit.html";
            }
            URLs = localVariables.URLs;
            URLs.businessAccess = "pages/core/businessaccess/edit.html";
        } else {
            URLs = { businessAccess: "pages/core/businessaccess/edit.html" };
        }
        if (localVariables.where) {
            localVariables.defultWhere = localVariables.where.getList();
            gridWheres = localVariables.where;
        } else {
            gridWheres = new Wheres();
        }
        if (localVariables.gridHandler) {
            gridHandler = localVariables.gridHandler;
        } else {
            gridHandler = new Handler();
            gridHandler.success = function(resp) {
                if (top.hideLoading) {
                    top.hideLoading();
                } else if (hidePageLoading) {
                    hidePageLoading();
                }
                if (resp.done) {
                    if (resp.result) {
                        hasNextPage = resp.result.length >= pageSize + 1;
                        lastPageNumber = pageNumber;
                        updatePaginationView();
                        fillGrid(resp.result);
                        Translation.translate();
                    }
                } else {
                    errorHandler(resp);
                }
            };
        }
        if (localVariables.deleteHandler) {
            deleteHandler = localVariables.deleteHandler;
        } else {
            deleteHandler = new Handler();
            deleteHandler.success = function(resp) {
                if (resp.done) {
                    if (resp.result) {
                        hidePageLoading();
                        if (top.showConfirm) {
                            setTimeout(function() {
                                    top.showConfirm({
                                        title: "حذف",
                                        body: "حذف با موفقیت انجام شد",
                                        confirmButton: {
                                            hidden: true
                                        },
                                        declineButton: {
                                            text: GeneralBundle.$close
                                        }
                                    });
                                },
                                450);
                            retrieveData();
                        }
                    } else {
                        switch (resp.errorCode) {
                        case ENVIRONMENT.ErrorCode.BUSINESS_MESSAGE:
                            switch (resp.errorDesc) {
                            case ENVIRONMENT.ErrorDesc.NO_DATA:
                                $(`[data-pagination-key=${ENVIRONMENT.PaginationKey.NEXT}]`).addClass("disabled");
                                break;
                            }
                            break;
                        }
                        hidePageLoading();
                    }
                }
            };
        }
    }
    if (editFrame.length) {
        editFrame.attr("src", Config.APPLICATION_URL + URLs.edit + "?id=" + componentID + "&ref=1");
    }

    gridOrderByList = new OrderBy();
    gridFilter = new Filter(pageNumber, pageSize, gridWheres.getList(), gridOrderByList.get());
    deleteFilter = new Filter();
    componentID = Util.urlParameter("id");
    if (localVariables) {
        const {
            URLs: localURLs,
            ignoreToClear: localIgnoreToClear,
            saveHandler: localSaveHandler,
            objectsConfig: localObjectsConfig,
            showRow: lShowRow,
            saveOrUpdateCallback: localSaveOrUpdateCallback
        } = localVariables;
        URLs = localURLs || {};
        ignoreToClear = localIgnoreToClear || [];
        if (localSaveHandler) {
            saveHandler = localSaveHandler;
        } else {
            saveHandler = new Handler();
            saveHandler.beforeSend = () => {
                if (top.showLoading) {
                    top.showLoading();
                }
                changeButtonState({ button: submitButton, state: 1 });
            };
            saveHandler.success = function(resp) {
                if (top.hideLoading) {
                    top.hideLoading();
                } else if (hidePageLoading) {
                    hidePageLoading();
                }
                if (resp.done) {
                    if (localSaveOrUpdateCallback) {
                        const returnValue = localSaveOrUpdateCallback.apply(null, [resp.result]);
                        if (returnValue) {
                            // if return true at the end of saveOrUpdateCallback, rest of code doesn't execute
                            return;
                        }
                    }
                    try {
                        const entityIDElement = $('[ksun-bundle-key="' + "entityId" + '"]');
                        entityIDElement.addClass("floating-mode");
                        if (typeof resp.result === "object") {
                            $(entityIDElement).closest(`.${ENVIRONMENT.CssClass.FORM_GROUP}`).data().entityId =
                                resp.result.entityId;
                            entityIDElement.val(resp.result.entityId).trigger("click");
                        } else {
                            $(entityIDElement).closest(`.${ENVIRONMENT.CssClass.FORM_GROUP}`).data().entityId =
                                resp.result;
                            entityIDElement.val(resp.result).trigger("click");
                        }
                        if (top.showConfirm) {
                            setTimeout(function() {
                                    top.showConfirm({
                                        title: "پیام سیستمی",
                                        body: "ذخیره سازی / بروزرسانی با موفقیت انجام شد",
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
                    } catch (e) {
                        console.log("entityId not found");
                    }


                } else {
                    errorHandler(resp);
                }
            };
            saveHandler.complete = (resp) => {
                if (resp && resp.responseJSON) {
                    if (resp.responseJSON.done) {
                        changeButtonState({ button: submitButton, state: 0 });
                    } else {
                        changeButtonState({ button: submitButton, state: 2 });
                    }
                } else {
                    changeButtonState({ button: submitButton, state: 0 });
                }
            };
        }
        objectsConfig = localObjectsConfig || {};
        localShowRow = lShowRow || undefined;
        saveOrUpdateCallback = localSaveOrUpdateCallback || undefined;
    }

    $("[ksun-default-value]").each((index, item) => {
        if ($(item).get(0).tagName === "TEXTAREA") {
            $(item).val($(item).attr("ksun-default-value"));
        }
    });

    $(".date").each(function() {
        var ctx = $(this);
        if (!ctx.attr("readonly")) {
            var key = ctx.attr("ksun-bundle-key");
            key = key.indexOf("$") === 0 ? key.substr(1, key.length - 1) : key;
            var obj = {};
            const dateConfig = {
                timePicker: {
                    enabled: !!ctx.attr("ksun-has-timepicker")
                },
                initialValue: ctx.attr("ksun-initial-value") && ctx.attr("ksun-initial-value") === "true",
                format: ctx.attr("ksun-date-format") ? ctx.attr("ksun-date-format") : "dddd D MMMM YYYY ساعت: HH:mm:ss",
                onSelect: function(unix) {
                    obj[key] = unix;
                    ctx.data("object", obj);
                    ctx.closest(`.${ENVIRONMENT.CssClass.FORM_GROUP}`).removeClass(ENVIRONMENT.CssClass.HAS_ERROR);
                }
            };
            if (!dateConfig.initialValue) {
                dateConfig.initialValue = false;
            }
            datePickerList[key] = ctx.persianDatepicker(dateConfig);
            if (ctx.attr("ksun-default-value")) {
                const currentDate = Number(ctx.attr("ksun-default-value"));
                datePickerList[key].pDatePicker.setDate(currentDate);
            } else if (ctx.attr("ksun-default-time")) {
                const currentDateTime = new persianDate();
                const arr = ctx.attr("ksun-default-time").split(":");
                const hour = arr[0];
                const min = arr.length > 1 ? arr[1] : 0;
                const sec = arr.length > 2 ? arr[2] : 0;
                currentDateTime.hour(Number(hour));
                currentDateTime.minute(Number(min));
                currentDateTime.second(Number(sec));
                datePickerList[key].pDatePicker.setDate(currentDateTime.gDate.getTime());
            }
        }
    });

    $(".clob").each(function() {
        try {
            var bundleKey = $(this).attr("ksun-bundle-key");
            const defaultValue = $(this).attr("ksun-default-value");
            bundleKey = bundleKey.indexOf("$") === 0 ? bundleKey.substr(1, bundleKey.length - 1) : bundleKey;
            if (localVariables.Config && localVariables.Config.CKEditor && localVariables.Config.CKEditor[bundleKey]) {
                CKEDITOR.editorConfig = function(config) {
                    config.language = localVariables.Config.CKEditor[bundleKey].language;
                    config.uiColor = localVariables.Config.CKEditor[bundleKey].uiColor;
                    config.height = localVariables.Config.CKEditor[bundleKey].height;
                };
                CKEDITOR.replace(this);
            } else {
                CKEDITOR.replace(this);
            }
            if (defaultValue) {
                CKEDITOR.instances[$(this).attr("id")].setData(defaultValue);
            }
        } catch (e) {
            console.warn("CKEditor is undefined", e);
        }
    });

    try {
        FloatingForm.init({ counter: true });
    } catch (e) {
        console.error("*** Add floating-form.js script ***");
    }
};


var initListeners = function() {

    // search bar button(s)
    newButton.on("click",
        function() {
            showEdit();
        });
    //business access
    businessAccess.on("click",
        function() {
            if (selectedRows.length === 0) {
                if (top.showConfirm) {
                    setTimeout(function() {
                            top.showConfirm({
                                title: "پیام سیستمی",
                                body: "لطفا حداقل یک ردیف را انتخاب نمایید",
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
            } else {
                if ($("#modal-business-aAccess").length === 0) {
                    const modal = $("<div/>",
                        {
                            class: "modal bd-example-modal-lg",
                            tabindex: "-1",
                            role: "dialog",
                            id: "modal-business-aAccess"
                        });
                    const modalDialog = $("<div/>", { class: "modal-dialog modal-lg", role: "document" });
                    const modalContent = $("<div/>", { class: "modal-content" });
                    const header = $("<div/>", { class: "modal-header" });
                    const h5 = $("<h5/>", { class: "modal-title", text: GeneralBundle["$businessAccess"] });
                    const button = $("<button/>",
                        {
                            type: "button",
                            class: "close ml-0 text-danger",
                            "data-dismiss": "modal",
                            "aria-label": "Close",
                            html: ` <span aria-hidden="true">&times;</span>`
                        });
                    header.append(h5, button);
                    const body = $("<div/>", { class: "modal-body" });
                    const iframe = $("<iframe/>",
                        {
                            class: "access-frame",
                            style: "width: 100%;height:375px;border: 0;margin: 0;",
                            src: localVariables.URLs.businessAccess + "?key=" + new Date().getTime()
                        });
                    iframe.on("load",
                        () => {
                            iframe.contents().find("header").remove();
                            iframe.contents().find("#edit-clear").remove();
                            iframe.contents().find(".edit-form-container")
                                .attr("style", "max-width: 100%;width: 100%;");
                            iframe.contents().find("form").attr("style", "border:0 !important;");
                            let ids = "";
                            $.each(selectedRows,
                                (i, v) => {
                                    ids += v.entityId + ",";
                                });
                            iframe.contents().find("#1020427247163600").val(ids.substring(0, ids.length - 1));
                            iframe.contents().find("#1020427247150500").val(localVariables.clazzName);
                            iframe.contents().find("#entityId").closest(".form-group").addClass("d-none");
                            iframe.contents().find("#1020427247150500").closest(".form-group").addClass("d-none");
                            iframe.contents().find("#1020427247163600").closest(".form-group").addClass("d-none");
                            iframe.contents().find("#created-by").closest(".form-group").addClass("d-none");
                            iframe.contents().find("#created").closest(".form-group").addClass("d-none");
                            iframe.contents().find("#updated-by").closest(".form-group").addClass("d-none");
                            iframe.contents().find("#updated").closest(".form-group").addClass("d-none");
                            iframe.contents().find("#wrapper-active").addClass("d-none");
                        });
                    body.append(iframe);
                    modalContent.append(header, body);
                    modalDialog.append(modalContent);
                    modal.append(modalDialog);
                    $("body").append(modal);
                }
                const modals = $("#modal-business-aAccess");
                modals.find("iframe").attr("src", localVariables.URLs.businessAccess + "?key=" + new Date().getTime());
                modals.modal("show");
            }

        });
    // search
    simpleSearchButton.on("click",
        function() {
            if (simpleSearchInput.val()) {
                gridWheres.clearList();
                gridWheres.add(ENVIRONMENT.FULL_TEXT_SEARCH_KEY,
                    simpleSearchInput.val(),
                    ENVIRONMENT.Condition.CONTAINS,
                    ENVIRONMENT.Operator.AND);

            } else {
                gridWheres.clearList();
                if (localVariables.defultWhere) {
                    $.each(localVariables.defultWhere,
                        (i, v) => {
                            gridWheres.add(v.key, v.value, v.condition, v.operatorWithNext);
                        });
                }
            }
            if (gridWheres.getList().length > 0) {
                const removeConstarint = gridWheres.getList()[gridWheres.getList().length - 1];
                removeConstarint.operatorWithNext = null;
                gridWheres.getList()[gridWheres.getList().length - 1] = removeConstarint;
            }
            retrieveData();
        });
    //search enter
    simpleSearchInput.on("keypress",
        (e) => {
            const code = e.keyCode || e.which;
            if (code === 13) {
                simpleSearchButton.click();
            }
        });
    //filter
    filters.on("click",
        () => {
            const table = $(".main-table");
            const pagination = $(".ksun-pagination-wrapper");
            const search = $(".search-bar");
            if (table.hasClass("fade_grid")) {
                table.removeClass("fade_grid");
            } else {
                table.addClass("fade_grid");
            }
            if (pagination.hasClass("fade_grid")) {
                pagination.removeClass("fade_grid");
            } else {
                pagination.addClass("fade_grid");
            }
            if (search.find(".input-group:eq(0)").hasClass("fade_grid")) {
                search.find(".input-group:eq(0)").removeClass("fade_grid");
            } else {
                search.find(".input-group:eq(0)").addClass("fade_grid");
            }
            $.each(search.find("button"),
                (i, v) => {
                    if (!$(v).hasClass("btn-filter")) {
                        if ($(v).hasClass("fade_grid")) {
                            $(v).removeClass("fade_grid");
                        } else {
                            $(v).addClass("fade_grid");
                        }
                    }
                });
        });
    filterSearchBtn.on("click",
        () => {
            gridWheres.clearList();
            dataContainer.find("input").not(".list-item,.read-only").each(function() {
                var ctx = $(this);
                var key = ctx.attr("ksun-bundle-key");
                let condition = Number(ctx.attr("ksun-condition-key"));
                let operation = Number(ctx.attr("ksun-operation-key"));
                if (key) {
                    var itemParent = ctx.closest(`.${ENVIRONMENT.CssClass.FORM_GROUP}`);
                    key = key.indexOf("$") === 0 ? key.substr(1, key.length - 1) : key;
                    if (ctx.attr("type") === "checkbox") {
                        gridWheres.add(ctx.attr("ksun-column-key"),
                            ctx.prop("checked") === true ? 1 : 0,
                            condition,
                            operation);
                    } else if (ctx.attr("type") === "radio") {
                        if (ctx.prop("checked")) {
                            gridWheres.add(ctx.attr("ksun-column-key"),
                                ctx.data("value") === true ? 1 : 0,
                                condition,
                                operation);
                        }
                    } else {
                        if (ctx.data("object")) {
                            if (key === "created" || key === "updated") {
                                gridWheres.add(ctx.attr("ksun-column-key"),
                                    ctx.data("object").date,
                                    condition,
                                    operation);
                            } else if (ctx.hasClass(ENVIRONMENT.CssClass.DATE)) {
                                if (ctx.val()) {
                                    gridWheres.add(ctx.attr("ksun-column-key"),
                                        ctx.data("object")[key],
                                        condition,
                                        operation,
                                        null,
                                        null,
                                        ENVIRONMENT.FieldType.DATE);
                                }
                            } else {
                                gridWheres.add(ctx.attr("ksun-column-key"),
                                    { entityId: ctx.data("object").entityId },
                                    condition,
                                    operation);
                            }
                        } else if (itemParent.length && itemParent.attr("data-secure")) {
                            gridWheres.add(ctx.attr("ksun-column-key"),
                                itemParent.data() ? itemParent.data()[key] : null,
                                condition,
                                operation);
                        } else if (ctx.val()) {
                            if (ctx.hasClass(ENVIRONMENT.CssClass.DATE)) {
                                if (ctx.val()) {
                                    gridWheres.add(ctx.attr("ksun-column-key"),
                                        ctx.data("object")[key],
                                        condition,
                                        operation,
                                        null,
                                        null,
                                        ENVIRONMENT.FieldType.DATE);
                                }
                            } else {
                                gridWheres.add(ctx.attr("ksun-column-key"), ctx.val(), condition, operation);
                            }
                        }
                    }
                }
            });

            // Autocomplete
            dataContainer.find("select.autocomplete").each(function() {
                var ctx = $(this);
                var key = ctx.attr("ksun-bundle-key");
                let condition = Number(ctx.attr("ksun-condition-key"));
                let operation = Number(ctx.attr("ksun-operation-key"));
                var itemParent = ctx.closest(`.${ENVIRONMENT.CssClass.FORM_GROUP}`);
                key = key.indexOf("$") === 0 ? key.substr(1, key.length - 1) : key;
                var config = ctx.data().select2Config;
                if (config) {
                    if (config.multipleSelect) { // درصورتیکه قابلیت انتخاب چند مورد وجود داشته باشد
                        data[key] = [];
                        var multipleSelectWrapper = itemParent.find(".multiple-select-wrapper");
                        if (multipleSelectWrapper.length) {
                            multipleSelectWrapper.find(".badge").each(function(i, v) {
                                if ($(v).data().id) {
                                    data[key].push({ entityId: $(v).data().id });
                                }
                            });
                            gridWheres.add(ctx.attr("ksun-column-key"), data[key], condition, operation);
                        }
                    } else {
                        var valueProperty = (config && config.value) ? config.value : "entityId";
                        if (itemParent.length && itemParent.attr("data-secure")) {
                            if (currentData &&
                                ctx.find("option:selected").length > 0 &&
                                ctx.find("option:selected").data("object")) {
                                gridWheres.add(ctx.attr("ksun-column-key"),
                                    ctx.find("option:selected").data("object")[valueProperty],
                                    condition,
                                    operation);
                            } else if (ctx.val()) {
                                gridWheres.add(ctx.attr("ksun-column-key"), ctx.val(), condition, operation);
                            }
                        } else if (ctx.val()) {
                            gridWheres.add(ctx.attr("ksun-column-key"), ctx.val(), condition, operation);
                        }
                    }
                } else {
                    var valueProperty = "entityId";
                    if (itemParent.length && itemParent.attr("data-secure")) {
                        if (currentData &&
                            ctx.find("option:selected").length > 0 &&
                            ctx.find("option:selected").data("object")) {
                            gridWheres.add(ctx.attr("ksun-column-key"),
                                ctx.find("option:selected").data("object")[valueProperty],
                                condition,
                                operation);
                        } else if (ctx.val()) {
                            gridWheres.add(ctx.attr("ksun-column-key"), ctx.val(), condition, operation);
                        }
                    } else if (ctx.val()) {
                        gridWheres.add(ctx.attr("ksun-column-key"), ctx.val(), condition, operation);
                    }
                }
            });

            // File
            dataContainer.find(".file").each(function() {
                var ctx = $(this);
                var key = ctx.attr("ksun-bundle-key");
                let condition = Number(ctx.attr("ksun-condition-key"));
                let operation = Number(ctx.attr("ksun-operation-key"));
                key = key.indexOf("$") === 0 ? key.substr(1, key.length - 1) : key;
                const maxCount = ctx.attr("ksun-max-count") ? Number(ctx.attr("ksun-max-count")) : 1;
                if (ctx.next(".files-wrapper").length === 1) {
                    if (maxCount > 1) {
                        data[key] = [];
                        ctx.next(".files-wrapper").find(".item").each(function(ind, item) {
                            const object = $(item).data("object");
                            data[key].push({
                                entityId: object.entityId,
                                parentID: object.parentID ? object.parentID : -1
                            });
                        });
                        gridWheres.add(ctx.attr("ksun-column-key"), data[key], condition, operation);
                    } else {
                        ctx.next(".files-wrapper").find(".item").each(function(ind, item) {
                            if (ind === 0) {
                                const object = $(item).data("object");
                                data[key] = {
                                    entityId: object.entityId,
                                    parentID: object.parentID ? object.parentID : -1
                                };
                                gridWheres.add(ctx.attr("ksun-column-key"), data[key], condition, operation);
                            }
                        });
                    }
                }
            });

            // Textarea
            dataContainer.find("textarea").each(function() {
                const ctx = $(this);
                const key = ctx.attr("ksun-bundle-key");
                let condition = Number(ctx.attr("ksun-condition-key"));
                let operation = Number(ctx.attr("ksun-operation-key"));
                if (ctx.val() && ctx.val() !== null) {
                    gridWheres.add(ctx.attr("ksun-column-key"), ctx.val(), condition, operation);
                }
            });

            // CKEditor
            try {
                dataContainer.find(".clob").each(function() {
                    var key = $(this).attr("ksun-bundle-key");
                    key = key.indexOf("$") === 0 ? key.substr(1, key.length - 1) : key;
                    let condition = Number(ctx.attr("ksun-condition-key"));
                    let operation = Number(ctx.attr("ksun-operation-key"));
                    if (CKEDITOR.instances[this.id] && CKEDITOR.instances[this.id].getData()) {
                        gridWheres.add(ctx.attr("ksun-column-key"),
                            CKEDITOR.instances[this.id].getData(),
                            condition,
                            operation);
                    }
                });
            } catch (e) {
                console.error("CKEditor is undefined!", e);
            }

            // Object List
            dataContainer.find(".objects-list-wrapper").each(function() {
                const ctx = $(this);
                const key = ctx.attr("ksun-object-key");
                let condition = Number(ctx.attr("ksun-condition-key"));
                let operation = Number(ctx.attr("ksun-operation-key"));
                gridWheres.add(ctx.attr("ksun-column-key"), [], condition, operation);
            });
            dataContainer.find(".objects-list-wrapper").each(function() {
                const ctx = $(this);
                var key = ctx.attr("ksun-object-key");
                let condition = Number(ctx.attr("ksun-condition-key"));
                let operation = Number(ctx.attr("ksun-operation-key"));
                var conf = objectsConfig[key];
                ctx.find(".list-item").each(function() {
                    const elem = $(this);
                    var obj = {};
                    if (elem.data("object")) { // show row mode
                        obj = elem.data("object");
                    }
                    obj[conf.property] = elem.val();
                    data[key].push(obj);
                });
                gridWheres.add(ctx.attr("ksun-column-key"), data[key], condition, operation);
            });
            retrieveData();
            filters.click();
        });
    // sort
    sortedColumnCombo.append($(`<option value=-1 selected disabled>${GeneralBundle["$sortAccordingTo"]}</option>`));
    tableHeader.find(".value").not(".event-off").each(function(index) {
        const columnKey = $(this).find("[ksun-bundle-key]").attr("ksun-bundle-key");
        columns.push(columnKey);
        $(this).data("column", { key: columnKey, orderType: null });
        // card grid mode
        const option = $(`<option value=${columnKey} ksun-bundle-key=${columnKey}></option>`);
        sortedColumnCombo.append(option);
    });
    sortListener();

    sortedColumnCombo.on("change",
        function() {
            var sortedColumn = { key: this.value, orderType: orderTypeCombo.val() };
            var badge = undefined;
            sortedBadgesWrapper.find(".badge").each(function() {
                if ($(this).data("object").key === sortedColumn.key) {
                    badge = $(this);
                }
            });
            if (badge) {
                if (badge.data("object").orderType !== sortedColumn.orderType) {
                    badge.data("object").orderType = sortedColumn.orderType;
                    badge.find("span.text").text($(this).find("option:selected").text() +
                        " | " +
                        orderTypeCombo.find("option:selected").text());
                    gridOrderByList.add(sortedColumn.key, sortedColumn.orderType);
                    doSort();
                }
            } else {
                const badgeItem = $('<span class="badge badge-dark"></span>');
                const text =
                    $(`<span class="text">${$(this).find("option:selected").text()} | ${orderTypeCombo
                        .find("option:selected").text()}</span>`);
                badgeItem.append(text);
                badgeItem.data("object", sortedColumn);
                const remove = $('<span><i class="icon fas fa-times"></i></span>');
                remove.on("click",
                    function() {
                        const parentBadge = $(this).closest(".badge");
                        const data = parentBadge.data("object");
                        gridOrderByList.remove(data.key);
                        parentBadge.remove();
                        doSort();
                    });
                badgeItem.append(remove);
                sortedBadgesWrapper.append(badgeItem);
                gridOrderByList.add(sortedColumn.key, sortedColumn.orderType);
                doSort();
            }
        });
    orderTypeCombo.on("change",
        function() {
            var sortedColumn = { key: sortedColumnCombo.val(), orderType: this.value };
            var badge = undefined;
            sortedBadgesWrapper.find(".badge").each(function() {
                if ($(this).data("object").key == sortedColumn.key) {
                    badge = $(this);
                }
            });
            if (badge) {
                if (badge.data("object").orderType != sortedColumn.orderType) {
                    badge.data("object").orderType = sortedColumn.orderType;
                    badge.find("span.text").text(sortedColumnCombo.find("option:selected").text() +
                        " | " +
                        $(this).find("option:selected").text());
                    gridOrderByList.add(sortedColumn.key, sortedColumn.orderType);
                    doSort();
                }
            } else {
                const badgeItem = $('<span class="badge badge-dark"></span>');
                const text =
                    $(`<span class="text">${sortedColumnCombo.find("option:selected").text()} | ${orderTypeCombo
                        .find("option:selected").text()}</span>`);
                badgeItem.append(text);
                badgeItem.data("object", sortedColumn);
                const remove = $('<span><i class="icon fas fa-times"></i></span>');
                remove.on("click",
                    function() {
                        const parentBadge = $(this).closest(".badge");
                        const data = parentBadge.data("object");
                        gridOrderByList.remove(data.key);
                        parentBadge.remove();
                        doSort();

                    });
                badgeItem.append(remove);
                sortedBadgesWrapper.append(badgeItem);
                gridOrderByList.add(sortedColumn.key, sortedColumn.orderType);
                doSort();
            }
        });
    // tooltip
    tooltipListener();
    // pagination
    paginationListener();

    // page number
    pageNumberInput.val(pageNumber);
    pageNumberInput.closest(".input-group").find(".input-group-text").on("click",
        function() {
            const value = pageNumberInput.val();
            if (value !== pageNumber) {
                if (!hasNextPage && value > pageNumber) {
                    pageNumberInput.val(lastPageNumber);
                    if (top.showConfirm) {
                        setTimeout(function() {
                                top.showConfirm({
                                    title: "پیام سیستمی",
                                    body: "اطلاعاتی برای نمایش وجود ندارد",
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
                } else {
                    updatePageNumber(pageNumber, value);
                    retrieveData();
                }
            }
        });

    // page size
    pageSizeCombo.val(pageSize);
    pageSizeCombo.on("change",
        function() {
            pageSize = Number(this.value);
            updatePageNumber(1, 1);
            retrieveData();
        });

    // header checkbox
    headerCheckbox.on("change",
        function() {
            selectedRows = [];
            if ($(this).prop("checked")) {
                tableBody.find("tr").each(function() {
                    selectedRows.push($(this).data("rowData"));
                    $(this).find("input[type=checkbox]").prop("checked", true);
                });
            } else {
                tableBody.find("tr").each(function() {
                    $(this).find("input[type=checkbox]").prop("checked", false);
                });
            }
        });

    try {
        const hammerInstance = new Hammer(selectAll.get(0));
        hammerInstance.on("tap",
            function(ev) {
                selectedRows = [];
                if (selectAll.data("checkedStatus") === undefined || selectAll.data("checkedStatus") === false) {
                    tableBody.find("tr").each(function() {
                        selectedRows.push($(this).data("rowData"));
                        $(this).addClass("selected");
                    });
                    selectAll.data("checkedStatus", true);
                } else {
                    selectAll.data("checkedStatus", false);
                    tableBody.find("tr").each(function() {
                        $(this).removeClass("selected");
                    });
                }
            });
    } catch (e) {
        console.warn("index.js => selectAll is undefined");
    }


    // ripple
    $.ripple(".btn.material",
        {
            debug: false,
            on: "mousedown",
            opacity: 0.4,
            color: "auto",
            multi: false,
            duration: 0.7,
            rate: function(pxPerSecond) {
                return pxPerSecond;
            },
            easing: "linear"
        });
};
var initLocalVariables = function(obj) {
    localVariables = obj;
};
var initScrollBar = function() {
    if ($(window).width() > 992) {
        new SimpleBar($(".scroll-container")[0]);
    }
};
var retrieveData = function() {
    gridFilter = new Filter(gridWheres.getList(), pageSize, pageNumber, gridOrderByList.get(), 1, "");
    Api.gridView({ url: URLs.services.gridView, filter: gridFilter.get(), handler: gridHandler });
};

var fillGrid = function(data) {
    tableBody.empty();
    if (data) {
        var finalData = [];
        var pattern = ["$rowNum"];
        if (data[0]) {
            data[0].forEach(function (key) {
                pattern.push(key);
            });
        }
        data.forEach(function(row, index) {
            if (index > 0 && index <= pageSize) {
                var obj = {};
                row.forEach(function(value, i) {
                    obj[pattern[i]] = value;
                });
                finalData.push(obj);
            }
        });
        finalData.forEach(function(value) {
            makeRow(value, finalData.length);
        });
        tableBody.find(".pattern-row").remove();
        tooltipListener();
    }
};
var makeRow = function(rowData, dataLength) {
    var row = patternRow.clone();
    row.removeAttr("class");
    let actionList = [];
    for (var key in rowData) {
        var td = row.find(`[ksun-bundle-key="${key}"]`);
        var formatter = td.attr("ksun-formatter");
        var rd = rowData[key];
        if (formatter) {
            if (rd && isNaN(rd)) {
                rd = rd.replace(/\n/g, " ");
            }
            td.html(eval(formatter + "('" + rd + "')"));
        } else {
            td.html(rd ? rd : "---");
        }
    }
    row.data("rowData", rowData);
    row.on("dblclick",
        function() {
            showEdit(row.data("rowData"));
        });

    var hammerInstance = new Hammer(row.get(0));
    hammerInstance.on("tap",
        function(ev) {
            if (ev.pointerType === "touch" && top && top.showActionDialog) {
                top.showActionDialog({
                    headerTitle: "انتخاب عملیات",
                    actions: actions,
                    componentID: componentID,
                    rowData: row.data("rowData")
                });
            }
        });

    if (table.hasClass("has-checkbox")) {
        let genId = Util.uuid();
        var checkboxTD = row.find('[ksun-bundle-key="$checkbox"]');
        let divFormGroup = $("<div/>",
            {
                class: "form-group floating-form-group",
                style: "width: 30px;padding: 0;margin: 0 auto;"
            });
        let divFloating = $("<div/>",
            {
                class: "floating-form-checkbox-wrapper"
            });
        let innerContainer = $("<div/>", { class: "col-sm-12 form-control-wrapper col-value" });
        let checkBox = $("<input/>",
            {
                type: "checkbox",
                class: "form-control form-control-sm floating-form-control",
                id: genId,
                "ksun-bundle-key": "$action"
            });
        let flg = false;
        $.each(selectedRows,
            (i, v) => {
                if (v.entityId === rowData.entityId) {
                    flg = true;
                }
            });
        if (flg) {
            checkBox.prop("checked", true);
        }
        let label = $("<label/>", { class: "floating-form-checkbox", for: genId });
        checkBox.on("dblclick",
            function(e) {
                e.stopPropagation();
            });
        checkBox.on("change",
            function(e) {
                var ctx = $(this);
                if (ctx.prop("checked")) {
                    selectedRows.push(ctx.closest("tr").data("rowData"));
                    if (tableBody.find("tr").length === selectedRows.length) {
                        headerCheckbox.prop("checked", true);
                    }
                } else {
                    headerCheckbox.prop("checked", false);
                    selectedRows = selectedRows.filter(function(r) {
                        if (r.rowNumber !== ctx.closest("tr").data("rowData").rowNumber) {
                            return r;
                        }
                    });
                }
            });
        innerContainer.append(checkBox, label);
        divFloating.append(innerContainer);
        divFormGroup.append(divFloating);
        checkboxTD.append(divFormGroup);
        // long press
        hammerInstance.on("press",
            function(ev) {
                if (ev.pointerType === "touch") {
                    if (row.hasClass("selected")) { // deselect
                        selectAll.data("checkedStatus", false);
                        row.removeClass("selected");
                        selectedRows = selectedRows.filter(function(r) {
                            if (r.rowNumber !== row.data("rowData").rowNumber) {
                                return r;
                            }
                        });
                    } else { // select
                        row.addClass("selected");
                        selectedRows.push(row.data("rowData"));
                        if (tableBody.find("tr").length === tableBody.find("tr.selected").length) {
                            selectAll.data("checkedStatus", true);
                        }
                    }
                }
            });
    }
    var actionTD = row.find('[ksun-bundle-key="$actions"]');
    var colValue = $('<div class="col-value"></div>');
    actions.forEach(function(action) {
        let flag = true;
        if (localVariables.rowConstraint &&
            localVariables.rowConstraint[action.key] &&
            localVariables.rowConstraint[action.key]["validation"]) {
            flag = eval(localVariables.rowConstraint[action.key]["validation"] + "(`" + JSON.stringify(rowData) + "`)");
        }
        if (flag) {
            const id = Util.uuid();
            const actionItem =
                $(`<a href="javascript: void(0)" id="${id
                    }" class="action" data-toggle="tooltip" data-placement="top" title=${action.title}></a>`);
            actionItem.addClass(action.key.toLowerCase());
            const icon = $("<i></i>");
            icon.addClass(action.icon);
            actionItem.append(icon);
            actionItem.on("click",
                function() {
                    actionHandler($(this).closest("tr").data("rowData"), action.key);
                });
            colValue.append(actionItem);
            if (localVariables.rowConstraint &&
                localVariables.rowConstraint[action.key] &&
                localVariables.rowConstraint[action.key]["configuration"]) {
                actionList.push({ key: action.key, id: `#${id}` });
            }
        }

    });
    actionTD.append(colValue);

    tableBody.append(row);

    if (tableBody.find("tr").length === dataLength) {
        $.event.trigger({
            type: "tableCreated",
            message: "table-created",
            time: new Date()
        });
    }
    $.each(actionList,
        (i, v) => {
            eval(localVariables.rowConstraint[v.key]["configuration"] + "(`" + v.id + "`)");
        });
};

var sortListener = function() {
    tableHeader.find(".value").not(".event-off").on("click",
        function() {
            const element = $(this).find("span");
            let columnName = element.attr("ksun-bundle-key");
            columnName = columnName.indexOf("$") === 0 ? columnName.substr(1, columnName.length - 1) : columnName;
            const fieldType = element.attr("ksun-field-type")
                ? element.attr("ksun-field-type")
                : ENVIRONMENT.FieldType.STRING;
            const orderType = $(this).data("orderType");
            if (orderType) {
                if (orderType === ENVIRONMENT.OrderType.ASC) {
                    $(this).data("orderType", ENVIRONMENT.OrderType.DESC);
                    $(this).find(".icon").attr("data-icon", "long-arrow-alt-down");
                    gridOrderByList.add(columnName, ENVIRONMENT.OrderType.DESC, fieldType);
                } else if (orderType === ENVIRONMENT.OrderType.DESC) {
                    $(this).data("orderType", null);
                    $(this).find(".icon").attr("data-icon", "sort");
                    gridOrderByList.remove(columnName);
                }
            } else {
                $(this).data("orderType", ENVIRONMENT.OrderType.ASC);
                $(this).find(".icon").attr("data-icon", "long-arrow-alt-up");
                gridOrderByList.add(columnName, ENVIRONMENT.OrderType.ASC, fieldType);
            }
            doSort();
        });
};
var tooltipListener = function() {
    $('[data-toggle="tooltip"]').each(function() {
        const placement = $(this).attr("data-placement") ? $(this).attr("data-placement") : "top";
        $(this).tooltip({
            placement: placement
        });
    });
    /*$('[data-toggle="tooltip"]').on('shown.bs.tooltip', function () {
     $('.tooltip').addClass('animated swing');
     });*/
};
var paginationListener = function() {
    nextPageButton.on("click",
        function() {
            if (!nextPageButton.attr(ENVIRONMENT.CssClass.DISABLED)) {
                updatePageNumber(pageNumber, ++pageNumber);
                retrieveData();
            }
        });
    prevPageButton.on("click",
        function() {
            if (!prevPageButton.attr(ENVIRONMENT.CssClass.DISABLED) || pageNumber > 1) {
                updatePageNumber(pageNumber, --pageNumber);
                retrieveData();
            }
        });
};

var doSort = function() {
    gridFilter.setOrdersBy(gridOrderByList.get());
    retrieveData();
};

var updatePageNumber = function(oldPageNumber, newPageNumber) {
    lastPageNumber = oldPageNumber;
    pageNumber = newPageNumber;
};
var updatePaginationView = function() {
    if (pageNumber === 1) {
        prevPageButton.attr(ENVIRONMENT.CssClass.DISABLED, true);
    } else {
        prevPageButton.removeAttr(ENVIRONMENT.CssClass.DISABLED);
    }
    if (hasNextPage) {
        nextPageButton.removeAttr(ENVIRONMENT.CssClass.DISABLED);
    } else {
        nextPageButton.attr(ENVIRONMENT.CssClass.DISABLED, true);
    }
    pageNumberInput.val(lastPageNumber);
};
var actionHandler = function(rowData, actionType) {
    switch (actionType) {
    case ENVIRONMENT.Action.DELETE:
        deleteRow(rowData);
        break;
    case ENVIRONMENT.Action.EDIT:
        showEdit(rowData);
        break;
    }
};

var deleteRow = function(rowData) {
    if (top.showConfirm) {
        setTimeout(function() {
                top.showConfirm({
                    title: "عملیات حذف",
                    body: `آیا از حذف رکورد با شناسه ی ${rowData.entityId} مطمئن هستید؟`,
                    confirmButton: {
                        onClick: function() {
                            const deleteWheres = new Wheres();
                            deleteWheres.add("entityId", rowData.entityId, ENVIRONMENT.Condition.EQUAL);
                            deleteFilter.setWheres(deleteWheres.getList());
                            Api.delete(
                                { url: URLs.services.delete, filter: deleteFilter.get(), handler: deleteHandler });
                            return 1;
                        }
                    },
                    declineButton: {
                        text: GeneralBundle.$close
                    }
                });
            },
            450);
    }
};

var showEdit = function(rowData) {
    /*var url = Config.APPLICATION_URL + URLs.edit + '?id=' + componentID;
     editFrame.attr('src',url);*/
    editFrameWrapper.addClass("visible");
    setTimeout(function() {
            const editWindow = editFrame.get(0).contentWindow;
            editWindow.showRow(rowData);
        },
        350); // transition time
};

var showConfirmation = function(config) {
    pageConfirmModal.modal("show");
};
var showPageLoading = function () {
    pageLoadingModal.modal('show');
};
var hidePageLoading = function () {
    setTimeout(function () {
        pageLoadingModal.modal('hide');
    }, 500);
};
var onPageReady = function(params) {
    // change page language according to user lang
    $("html").attr("lang", Storage.getUserInfo().lang === 1 ? "FA" : "FA");
    $(document).on("tableCreated",
        function() {
        });
    initViews();
    initInstances();
    initListeners();
    initScrollBar();
    Translation.translate();
    if (params) {
        if (!params.disableAdvanceSearch) {
            AdvanceSearch.init();
        }
        if (!params.ignoreRetrieveData) {
            retrieveData();
        }
    } else {
        retrieveData();
    }
};