let containerFilter, localVariable, fileWrapper, newFolder, wrapperCreateFolder, backFolder, createNewFolder, prevFolder,
    folderName, fileUpload, fileDialog, fileName, alt, description, createNewFile, backNewFile, removeFile, filter;
const initializeVariable = () => {
    localVariable = {
        urls: {
            getAllExtension: "file/getAllExtension",
            gridView: "file/gridView",
            createFolder: "file/createFolder",
            createFile: "file/createFile",
            getDetails: "file/getDetails",
            delete: "file/delete",
        },
        currentPath: "",
        parentToSave: null,
        lastParentId: [],
        countSelectFile: 1,//تعداد مجاز انتخاب فایل
        lstSelectFile: [],//فایل های انتخاب شده
        maxSelectFileLength: 0,//حداکثر طول اندازه فایل برای انتخاب
        allowExtensionSelect: []//پسوندهای مورد پذیرش
    }
    containerFilter = $("#file-manager-filter-main-list");
    fileWrapper = $("#file-manager-file-list-wrapper");
    newFolder = $("#file-manager-new-folder-button");
    backFolder = $("#back-new-folder");
    createNewFolder = $("#create-new-folder");
    folderName = $("#folder-name");
    prevFolder = $("#file-manager-previous-folder");
    fileUpload = $("#file-manager-new-file-button");
    fileDialog = $("#fileDialog");
    fileName = $("#file-name");
    alt = $("#alt");
    description = $("#description");
    createNewFile = $("#create-new-file");
    backNewFile = $("#back-new-file");
    removeFile = $("#context-menu-delete-item");
    filter = $("#file-manager-filter-btn");
    wrapperCreateFolder = $(".create-folder");
}
const getExtensionForView = () => {
    const configData = {
        Wheres: [],
        PageSize: 0,
        PageNo: 0,
        Ticket: "",
        OrderBy: [],
        Language: 1
    };
    const handler = new Handler();
    handler.beforeSend = () => { };
    handler.complete = () => { };
    handler.success = (data) => {
        if (data.done) {
            $.each(data.result, (index, value) => {
                if (value) {
                    const li = $("<li/>");
                    const label = $("<label/>", { class: "control control--checkbox", text: value });
                    const input = $("<input/>", { type: "checkbox", "extension": value });
                    const indicator = $("<div/>", { class: "control__indicator" });
                    label.append(input, indicator);
                    li.append(label);
                    containerFilter.append(li);
                }
            });
        } else {
            handler.configError(data);
        }
    }
    Api.post({ url: localVariable.urls.getAllExtension, data: configData, handler: handler });
}
const createFileAndFolder = data => {
    fileWrapper.html("");
    $.each(data, (index, value) => {
        if (value.extension) {
            createFile(value.name, fileWrapper, value);
        } else {
            createFolder(value.name, fileWrapper, value);
        }
    });
}
const createFolder = (name, holder, data) => {
    const container = $("<div/>", {
        class: "wrapper-file",
        "entity-id": data.entityId
    });
    container.contextmenu((e) => {
        e.stopPropagation();
        e.preventDefault();
        getDetailsFile(data);
    });
    container.dblclick(function () {
        localVariable.currentPath = data.path ? data.path : "";
        localVariable.parentToSave = data.entityId;
        localVariable.lastParentId.push(data.parentId ? data.parentId : null);
        const where = new Wheres();
        prevFolder.removeAttr("disabled");
        where.add("parentId", `${data.entityId}`, ENVIRONMENT.Condition.EQUAL, ENVIRONMENT.Operator.AND, 1, [], ENVIRONMENT.FieldType.LONG);
        where.add("displayInFileManager", "true", ENVIRONMENT.Condition.EQUAL, ENVIRONMENT.Operator.AND, 1, [], ENVIRONMENT.FieldType.BOOLEAN);
        const configData = {
            wheres: where.getList(),
            pageSize: 0,
            pageNo: 0,
            ticket: "",
            orderBy: [],
            language: 1
        };
        const handler = new Handler();
        handler.complete = () => {
            hideLoading();
        };
        handler.success = (data) => {
            if (data.done) {
                createFileAndFolder(data.result);
            } else {
                handler.configError(data);
            }
        }
        Api.post({ url: localVariable.urls.gridView, data: configData, handler: handler });
    });
    const span = $("<span/>", {
        class: "svg-holder"
    });
    span.append(getIcon(null));
    container.append(span, name);
    holder.append(container);
}
const createFile = (name, holder, data) => {
    let flg = false;
    $.each(localVariable.lstSelectFile, (i, v) => {
        if (v.entityId === data.entityId) {
            flg = true;
        }
    });
    const container = $("<div/>", {
        class: "wrapper-file",
        title: "",
        "entity-id": data.entityId,
        "data-html": "true",
        "data-original-title": `نام: ${data.title}<br>سایز: ${(data.dataSize / 100000).toFixed(2)} MB<br>`
    });
    container.popover({
        trigger: 'hover',
        html: true,
        container: 'body',
        placement: 'right',
        fixTitle: function () {
            var $e = this.$element;
            if ($e.attr('title') || typeof ($e.attr('data-original-title')) !== 'string') {
                $e.attr('data-original-title', $e.attr('title') || '').attr('title', '');
            }
        }
    });
    container.contextmenu((e) => {
        e.stopPropagation();
        e.preventDefault();
        getDetailsFile(data);
    });
    if (flg) {
        container.addClass("active");
    }
    container.on("click", () => {
        if (container.hasClass("active")) {
            container.removeClass("active");
            const arrayTemp = [];
            $.each(localVariable.lstSelectFile, (i, v) => {
                if (v.entityId !== data.entityId) {
                    arrayTemp.push(v);
                }
            });
            localVariable.lstSelectFile = arrayTemp;
        } else {
            if (localVariable.lstSelectFile.length < localVariable.countSelectFile &&
                localVariable.maxSelectFileLength >= Number(data.dataSize)) {
                if (localVariable.allowExtensionSelect.length === 0) {
                    localVariable.lstSelectFile.push(data);
                    container.addClass("active");
                } else {
                    let flag = false;

                    $.each(localVariable.allowExtensionSelect, (i, v) => {
                        if (v === data.extension) {
                            flag = true;
                        }
                    });
                    if (flag) {
                        localVariable.lstSelectFile.push(data);
                        container.addClass("active");
                    }
                }
            }
        }
        $("#file-manager-selected-count").text(localVariable.lstSelectFile.length);
    });
    const span = $("<span/>", {
        class: "svg-holder"
    });
    const ext = data.extension.toLowerCase();
    if (ext === "png" || ext === "jpeg" || ext === "jpg") {
        span.append(`<img src="${data.thumbnail}" alt="${data.name}"/>`);
    }
    else {
        span.append(getIcon(ext));
    }
    container.append(span, name);
    holder.append(container);
}
const getDetailsFile = (data) => {
    if (!data.extension) {
        removeFile.attr("remove-id", data.entityId);
        $('#context-menu-get-link-item').addClass("d-none");
        $('#context-menu-download-item').addClass("d-none");
        $('#details').modal("show");
    } else {
        $('#context-menu-get-link-item').removeClass("d-none");
        $('#context-menu-download-item').removeClass("d-none");
        var where = new Wheres();
        where.add("entityId", `${data.entityId}`, ENVIRONMENT.Condition.EQUAL, ENVIRONMENT.Operator.AND, 1, [], ENVIRONMENT.FieldType.LONG);
        const configData = {
            wheres: where.getList(),
            pageSize: 0,
            pageNo: 0,
            ticket: "",
            orderBy: [],
            language: 1
        };
        const handler = new Handler();
        handler.beforeSend = () => { };
        handler.complete = () => {
            hideLoading();
        };
        handler.success = (resp) => {
            if (resp.done) {
                removeFile.attr("remove-id", resp.result.entityId);
                $('#details').modal("show");
                $("#container-link").val(resp.result.path);
                $("#download-link").attr("href", resp.result.path);
            } else {
                handler.configError(resp);
            }
        }
        Api.post({ url: localVariable.urls.getDetails, data: configData, handler: handler });
    }



}
const getIcon = extension => {
    switch (extension) {
        case null:
            return `<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 48 48"><defs><linearGradient id="qON1JWtKOKa29~WEXmr8wa" x1="23" x2="23" y1="40" y2="8" gradientUnits="userSpaceOnUse"><stop offset=".295" stop-color="#cf6c00"/><stop offset=".425" stop-color="#d3780a"/><stop offset=".656" stop-color="#df9624"/><stop offset=".96" stop-color="#f2c84f"/><stop offset="1" stop-color="#f5cf55"/></linearGradient><linearGradient id="qON1JWtKOKa29~WEXmr8wb" x1="27.711" x2="22.683" y1="42.374" y2="13.862" gradientTransform="matrix(1 0 -.176 1 4.761 0)" gradientUnits="userSpaceOnUse"><stop offset="0" stop-color="#e3aa00"/><stop offset="1" stop-color="#ffea82"/></linearGradient></defs><g data-name="open-file folder"><path fill="url(#qON1JWtKOKa29~WEXmr8wa)" d="M7,40a2,2,0,0,1-2-2V11.908a3,3,0,0,1,.5-1.664l.9-1.353A2,2,0,0,1,8.07,8h7.87a2,2,0,0,1,1.668.9l.8,1.208a2,2,0,0,0,1.668.9H39a2,2,0,0,1,2,2v3.917Z"/><path fill="url(#qON1JWtKOKa29~WEXmr8wb)" d="M38.708,40h-32A1.633,1.633,0,0,1,5.06,38L8.411,19a2.488,2.488,0,0,1,2.352-2H19.7a2.444,2.444,0,0,0,1.826-.9L22.541,14.9a2.444,2.444,0,0,1,1.826-.9H43.292a1.633,1.633,0,0,1,1.648,2L41.06,38A2.488,2.488,0,0,1,38.708,40Z"/></g></svg>`;
        case "mp3":
            return `<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 64 64"><linearGradient id="yJBNuRAsX4J12z5mVT7kaa" x1="32" x2="32" y1="901" y2="845.988" gradientTransform="matrix(1 0 0 -1 0 906)" gradientUnits="userSpaceOnUse" spreadMethod="reflect"><stop offset="0" stop-color="#1a6dff"/><stop offset="1" stop-color="#c822ff"/></linearGradient><path fill="url(#yJBNuRAsX4J12z5mVT7kaa)" d="M51.121,15.707l-8.828-8.828C41.727,6.313,40.973,6,40.171,6H15c-1.654,0-3,1.346-3,3v40v2v4 c0,1.654,1.346,3,3,3h34c1.654,0,3-1.346,3-3v-4v-2V17.828C52,17.027,51.688,16.273,51.121,15.707z M48.586,16H43 c-0.551,0-1-0.448-1-1V9.414L48.586,16z M49,56H15c-0.551,0-1-0.448-1-1v-1.185C14.314,53.928,14.648,54,15,54h34 c0.352,0,0.686-0.072,1-0.184V55C50,55.552,49.551,56,49,56z M50,51c0,0.552-0.449,1-1,1H15c-0.551,0-1-0.448-1-1v-2V9 c0-0.552,0.449-1,1-1h25v7c0,1.654,1.346,3,3,3h7v31V51z"/><linearGradient id="yJBNuRAsX4J12z5mVT7kab" x1="33" x2="33" y1="29" y2="39" gradientUnits="userSpaceOnUse" spreadMethod="reflect"><stop offset="0" stop-color="#6dc7ff"/><stop offset="1" stop-color="#e6abff"/></linearGradient><path fill="url(#yJBNuRAsX4J12z5mVT7kab)" d="M31.5,39h-1c-0.276,0-0.5-0.224-0.5-0.5V30c0-0.552,0.448-1,1-1h3c1.105,0,2,0.895,2,2v3 c0,1.105-0.895,2-2,2h-2v2.5C32,38.776,31.776,39,31.5,39z M32,34h1.5c0.276,0,0.5-0.224,0.5-0.5v-2c0-0.276-0.224-0.5-0.5-0.5H32 V34z"/><linearGradient id="yJBNuRAsX4J12z5mVT7kac" x1="41" x2="41" y1="29" y2="39" gradientUnits="userSpaceOnUse" spreadMethod="reflect"><stop offset="0" stop-color="#6dc7ff"/><stop offset="1" stop-color="#e6abff"/></linearGradient><path fill="url(#yJBNuRAsX4J12z5mVT7kac)" d="M38,29.5v1c0,0.276,0.224,0.5,0.5,0.5h3c0.276,0,0.5,0.224,0.5,0.5v1 c0,0.276-0.224,0.5-0.5,0.5h-2c-0.276,0-0.5,0.224-0.5,0.5v1c0,0.276,0.224,0.5,0.5,0.5h2c0.276,0,0.5,0.224,0.5,0.5v1 c0,0.276-0.224,0.5-0.5,0.5h-3c-0.276,0-0.5,0.224-0.5,0.5v1c0,0.276,0.224,0.5,0.5,0.5H42c1.105,0,2-0.895,2-2v-2.086 c0-0.265-0.105-0.52-0.293-0.707L43.5,34l0.207-0.207C43.895,33.605,44,33.351,44,33.086V31c0-1.105-0.895-2-2-2h-3.5 C38.224,29,38,29.224,38,29.5z"/><linearGradient id="yJBNuRAsX4J12z5mVT7kad" x1="24" x2="24" y1="29" y2="39" gradientUnits="userSpaceOnUse" spreadMethod="reflect"><stop offset="0" stop-color="#6dc7ff"/><stop offset="1" stop-color="#e6abff"/></linearGradient><path fill="url(#yJBNuRAsX4J12z5mVT7kad)" d="M27.524,29.475l0.45,9C27.988,38.761,27.76,39,27.474,39H26.48 c-0.268,0-0.488-0.211-0.5-0.479L25.75,33l-1.173,2.155c-0.249,0.457-0.905,0.457-1.154,0L22.25,33l-0.23,5.521 C22.009,38.789,21.788,39,21.52,39h-0.995c-0.286,0-0.514-0.239-0.499-0.525l0.45-9C20.49,29.209,20.709,29,20.976,29h1.214 c0.19,0,0.363,0.108,0.448,0.278L24,32.024l1.362-2.746C25.447,29.108,25.62,29,25.81,29h1.214 C27.291,29,27.51,29.209,27.524,29.475z"/></svg>`;
        case "mp4":
            return `<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 64 64" fill="#FFFFFF">  <path d="M23.65625 4C22.320313 4 21.0625 4.519531 20.121094 5.464844L11.464844 14.121094C10.519531 15.066406 10 16.320313 10 17.65625L10 57C10 58.652344 11.347656 60 13 60L53 60C54.652344 60 56 58.652344 56 57L56 7C56 5.347656 54.652344 4 53 4 Z M 24 6L53 6C53.550781 6 54 6.449219 54 7L54 57C54 57.550781 53.550781 58 53 58L13 58C12.449219 58 12 57.550781 12 57L12 18L21 18C22.652344 18 24 16.652344 24 15 Z M 22 6.5L22 15C22 15.550781 21.550781 16 21 16L12.5 16C12.613281 15.835938 12.738281 15.675781 12.878906 15.535156L21.535156 6.878906C21.679688 6.734375 21.835938 6.609375 22 6.5 Z M 28.023438 21.816406C27.671875 21.808594 27.316406 21.890625 26.996094 22.0625C26.355469 22.417969 25.964844 23.085938 25.964844 23.816406L25.964844 42.183594C25.964844 42.910156 26.355469 43.582031 26.996094 43.933594C27.296875 44.097656 27.632813 44.183594 27.964844 44.183594C28.335938 44.183594 28.707031 44.078125 29.03125 43.871094L43.53125 34.6875C44.113281 34.320313 44.464844 33.6875 44.464844 33C44.464844 32.308594 44.113281 31.679688 43.53125 31.3125L29.03125 22.125C28.722656 21.933594 28.375 21.828125 28.023438 21.816406 Z M 27.964844 23.816406L42.464844 33L27.964844 42.1875 Z M 15 52C14.449219 52 14 52.449219 14 53L14 55C14 55.550781 14.449219 56 15 56C15.550781 56 16 55.550781 16 55L16 53C16 52.449219 15.550781 52 15 52 Z M 20 52C19.449219 52 19 52.449219 19 53L19 55C19 55.550781 19.449219 56 20 56C20.550781 56 21 55.550781 21 55L21 53C21 52.449219 20.550781 52 20 52 Z M 25 52C24.449219 52 24 52.449219 24 53L24 55C24 55.550781 24.449219 56 25 56C25.550781 56 26 55.550781 26 55L26 53C26 52.449219 25.550781 52 25 52 Z M 30 52C29.449219 52 29 52.449219 29 53L29 55C29 55.550781 29.449219 56 30 56C30.550781 56 31 55.550781 31 55L31 53C31 52.449219 30.550781 52 30 52 Z M 35 52C34.449219 52 34 52.449219 34 53L34 55C34 55.550781 34.449219 56 35 56C35.550781 56 36 55.550781 36 55L36 53C36 52.449219 35.550781 52 35 52 Z M 40 52C39.449219 52 39 52.449219 39 53L39 55C39 55.550781 39.449219 56 40 56C40.550781 56 41 55.550781 41 55L41 53C41 52.449219 40.550781 52 40 52 Z M 45 52C44.449219 52 44 52.449219 44 53L44 55C44 55.550781 44.449219 56 45 56C45.550781 56 46 55.550781 46 55L46 53C46 52.449219 45.550781 52 45 52 Z M 50 52C49.449219 52 49 52.449219 49 53L49 55C49 55.550781 49.449219 56 50 56C50.550781 56 51 55.550781 51 55L51 53C51 52.449219 50.550781 52 50 52Z" fill="#FFFFFF" /></svg>`;
        case "pdf":
            return `<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="48" height="48" viewBox="0 0 48 48"><path fill="#FF5722" d="M40 45L8 45 8 3 30 3 40 13z"/><path fill="#FBE9E7" d="M38.5 14L29 14 29 4.5z"/><path fill="#FFEBEE" d="M15.81 29.5V33H13.8v-9.953h3.391c.984 0 1.77.306 2.355.916s.878 1.403.878 2.379-.29 1.745-.868 2.311S18.175 29.5 17.149 29.5H15.81zM15.81 27.825h1.381c.383 0 .679-.125.889-.376s.314-.615.314-1.094c0-.497-.107-.892-.321-1.187-.214-.293-.501-.442-.861-.447H15.81V27.825zM21.764 33v-9.953h2.632c1.162 0 2.089.369 2.778 1.107.691.738 1.043 1.75 1.057 3.035v1.613c0 1.308-.346 2.335-1.035 3.079C26.504 32.628 25.553 33 24.341 33H21.764zM23.773 24.722v6.61h.602c.67 0 1.142-.177 1.415-.53.273-.353.417-.962.431-1.828v-1.729c0-.93-.13-1.578-.39-1.944-.26-.367-.702-.56-1.326-.578H23.773zM34.807 28.939h-3.124V33h-2.01v-9.953h5.51v1.675h-3.5v2.55h3.124V28.939z"/></svg>`;
        case "docx":
            return `<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="48" height="48" viewBox="0 0 48 48"><path fill="#1976D2" d="M40 45L8 45 8 3 30 3 40 13z"/><path fill="#E3F2FD" d="M38.5 14L29 14 29 4.5z"/><path fill="#FAFAFA" d="M26.696,30.228l1.468-8.024h3.059L28.507,35h-3.199l-1.714-7.295L21.915,35h-3.19L16,22.203h3.067l1.468,8.024l1.758-8.024h2.619L26.696,30.228z"/></svg>`;
        case "zip":
            return `<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 48 48"><path fill="#ff9100" d="M40 45L8 45 8 3 30 3 40 13z"/><path fill="#ffe0b2" d="M38.5 14L29 14 29 4.5zM17.393 31.331h4.102V33h-6.453v-1.211l4.06-7.066H15v-1.676h6.419v1.184L17.393 31.331zM24.83 33h-2.01v-9.953h2.01V33zM28.528 29.5V33h-2.01v-9.953h3.391c.984 0 1.769.305 2.354.916.586.611.879 1.404.879 2.379s-.29 1.744-.868 2.31c-.579.566-1.381.848-2.406.848H28.528zM28.528 27.824h1.381c.383 0 .679-.125.889-.375.209-.25.315-.615.315-1.094 0-.496-.107-.891-.321-1.188-.215-.293-.502-.441-.861-.445h-1.401V27.824z"/></svg>`;
        case "rar":
            return `<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="48" height="48" viewBox="0 0 48 48"><path fill="#CFD8DC" d="M17 24L7 17.467c0 0 1-1.865 1-3.732S7 10 7 10l10 6.533V24zM17 40L7 32.533c0 0 1-.934 1-2.8S7 26 7 26l10 6.533V40zM17 32L7 24.533c0 0 1-.934 1-2.8S7 18 7 18l10 6.533V32z"/><path fill="#3F51B5" d="M17.624,34c-0.219,0-0.44-0.071-0.624-0.219l-10.625-8c-0.432-0.346-0.501-0.975-0.156-1.406c0.344-0.431,0.975-0.501,1.405-0.156l10.625,8c0.432,0.346,0.501,0.975,0.156,1.406C18.208,33.871,17.917,34,17.624,34z"/><path fill="#3F51B5" d="M42,25H16c0,0,1,1.742,1,4.246S16,33,16,33h26c0,0,1-1,1-4S42,25,42,25z"/><path fill="#9C27B0" d="M17.624,26c-0.219,0-0.44-0.071-0.624-0.219l-10.625-8c-0.432-0.346-0.501-0.975-0.156-1.406c0.344-0.431,0.975-0.502,1.405-0.156l10.625,8c0.432,0.346,0.501,0.975,0.156,1.406C18.208,25.871,17.917,26,17.624,26z"/><path fill="#9C27B0" d="M42,17H16c0,0,1,1.742,1,4.246S16,25,16,25h26c0,0,1-1,1-4S42,17,42,17z"/><path fill="#8BC34A" d="M18.609,41c0-0.293-0.113-0.584-0.36-0.781l-10.625-8c-0.43-0.345-1.061-0.274-1.405,0.156c-0.345,0.432-0.275,1.061,0.156,1.406L15.962,41H18.609z"/><path fill="#8BC34A" d="M42,33H16c0,0,1,1.742,1,4.246S16,41,16,41h26c0,0,1-1,1-4S42,33,42,33z"/><path fill="#689F38" d="M42,33H16c0,0,0.441,0.756,0.733,2h26.035C42.473,33.559,42,33,42,33z"/><path fill="#303F9F" d="M42,25H16c0,0,0.441,0.756,0.733,2h26.035C42.473,25.559,42,25,42,25z"/><path fill="#FDD835" d="M21.034 32c.11-.45.299-1.379.299-2.5s-.189-2.05-.299-2.5H19c.003.009.333 1.267.333 2.5S19.002 31.993 19 32H21.034zM21.034 24c.11-.45.299-1.379.299-2.5s-.189-2.05-.299-2.5H19c.003.009.333 1.267.333 2.5S19.002 23.993 19 24H21.034zM21.034 40c.11-.45.299-1.379.299-2.5s-.189-2.05-.299-2.5H19c.003.009.333 1.267.333 2.5S19.002 39.993 19 40H21.034z"/><path fill="#7B1FA2" d="M42.768,19C42.473,17.559,42,17,42,17L30.844,8.591C30.844,8.591,30.063,8,29,8C27.771,8,7,8,7,8l0.018,0.018C6.703,8.012,6.39,8.137,6.191,8.412c-0.325,0.446-0.226,1.072,0.22,1.396l9.737,7.47c0.161,0.325,0.418,0.92,0.607,1.722H42.768z"/><path fill="#AF7000" d="M32,16L21,8h-5l11,8c1.75,1.25,2,1.625,2,3s0,19,0,19s0,3-2,3c1.056,0,3.678,0,5,0c2,0,2-3,2-3s0-18,0-19C34,17,32.438,16.438,32,16z"/><g><path fill="#FFC107" d="M34,27v4h-5v-4H34 M34.25,26h-5.5C28.336,26,28,26.336,28,26.75v4.5c0,0.414,0.336,0.75,0.75,0.75h5.5c0.414,0,0.75-0.336,0.75-0.75v-4.5C35,26.336,34.664,26,34.25,26L34.25,26z"/></g><path fill="#5B3B07" d="M31.5 28.5A0.5 0.5 0 1 0 31.5 29.5A0.5 0.5 0 1 0 31.5 28.5Z"/><path fill="#FFEB3B" d="M31.5,29c-0.276,0-0.5-0.224-0.5-0.5v-3c0-0.276,0.224-0.5,0.5-0.5s0.5,0.224,0.5,0.5v3C32,28.776,31.776,29,31.5,29z"/></svg>`;
        case "ico":
            return `<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 40 40"><path fill="#ffc49c" d="M2.5 2.5H17.5V17.5H2.5z"/><path fill="#a16a4a" d="M17,3v14H3V3H17 M18,2H2v16h16V2L18,2z"/><path fill="#bae0bd" d="M22.5 22.5H37.5V37.5H22.5z"/><path fill="#5e9c76" d="M37,23v14H23V23H37 M38,22H22v16h16V22L38,22z"/><path fill="#8bb7f0" d="M2.5 22.5H17.5V37.5H2.5z"/><path fill="#4e7ab5" d="M17,23v14H3V23H17 M18,22H2v16h16V22L18,22z"/><path fill="#ffeea3" d="M6.229 26.229H13.771V33.771H6.229z" transform="rotate(-45.001 10 30)"/><path fill="#ffeea3" d="M6 26H14V34H6z"/><path fill="#a16a4a" d="M12.897 9.894L10 12.584 13.985 17 17 17 17 13.712z"/><path fill="#fff" d="M14 4.75A1.25 1.25 0 1 0 14 7.25A1.25 1.25 0 1 0 14 4.75Z"/><path fill="#e09367" d="M14.849 17L3 17 3 13.5 7.667 8.833z"/><path fill="#fff" d="M30 30l-1.457-2.428C27.864 26.44 28.68 25 30 25h0c1.32 0 2.136 1.44 1.457 2.572L30 30zM30 30l1.457 2.428C32.136 33.56 31.32 35 30 35h0c-1.32 0-2.136-1.44-1.457-2.572L30 30z"/><path fill="#fff" d="M30 30l-2.831.048c-1.32.022-2.16-1.404-1.5-2.548l0 0c.66-1.143 2.315-1.129 2.956.025L30 30zM30 30l2.831-.048c1.32-.022 2.16 1.404 1.5 2.548l0 0c-.66 1.143-2.315 1.129-2.956-.025L30 30z"/><path fill="#fff" d="M30 30l-1.374 2.475c-.641 1.154-2.296 1.168-2.956.025l0 0c-.66-1.143.179-2.57 1.5-2.548L30 30zM30 30l1.374-2.475c.641-1.154 2.296-1.168 2.956-.025l0 0c.66 1.143-.179 2.57-1.5 2.548L30 30z"/><path fill="#ffeea3" d="M30 28A2 2 0 1 0 30 32A2 2 0 1 0 30 28Z"/><path fill="#fff" d="M10 28A2 2 0 1 0 10 32A2 2 0 1 0 10 28Z"/><g><path fill="#c2e8ff" d="M22.5 2.5H37.5V17.5H22.5z"/><path fill="#7496c4" d="M37,3v14H23V3H37 M38,2H22v16h16V2L38,2z"/></g><path fill="#fff" d="M33 11c-.047 0-.086.023-.132.027-.248-.579-.822-.986-1.493-.986-.204 0-.396.047-.576.116C30.48 9.478 29.8 9 29 9c-1.105 0-2 .895-2 2-1.105 0-2 .895-2 2 0 1.104.895 2 2 2s4.974 0 6 0c1.105 0 2-.896 2-2C35 11.895 34.105 11 33 11zM34 4.75A1.25 1.25 0 1 0 34 7.25 1.25 1.25 0 1 0 34 4.75z"/></svg>`;
        case "svg":
            return `<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 64 64"><linearGradient id="4mU0_HDsJp2MQbnKQxoyna" x1="32" x2="32" y1="397" y2="341.988" gradientTransform="matrix(1 0 0 -1 0 402)" gradientUnits="userSpaceOnUse" spreadMethod="reflect"><stop offset="0" stop-color="#1a6dff"/><stop offset="1" stop-color="#c822ff"/></linearGradient><path fill="url(#4mU0_HDsJp2MQbnKQxoyna)" d="M51.121,15.707l-8.828-8.828C41.727,6.313,40.973,6,40.171,6H15c-1.654,0-3,1.346-3,3v40v2v4 c0,1.654,1.346,3,3,3h34c1.654,0,3-1.346,3-3v-4v-2V17.828C52,17.027,51.688,16.273,51.121,15.707z M48.586,16H43 c-0.551,0-1-0.448-1-1V9.414L48.586,16z M49,56H15c-0.551,0-1-0.448-1-1v-1.185C14.314,53.928,14.648,54,15,54h34 c0.352,0,0.686-0.072,1-0.184V55C50,55.552,49.551,56,49,56z M50,51c0,0.552-0.449,1-1,1H15c-0.551,0-1-0.448-1-1v-2V9 c0-0.552,0.449-1,1-1h25v7c0,1.654,1.346,3,3,3h7v31V51z"/><linearGradient id="4mU0_HDsJp2MQbnKQxoynb" x1="22.357" x2="22.357" y1="25.3" y2="40.714" gradientUnits="userSpaceOnUse" spreadMethod="reflect"><stop offset="0" stop-color="#6dc7ff"/><stop offset="1" stop-color="#e6abff"/></linearGradient><path fill="url(#4mU0_HDsJp2MQbnKQxoynb)" d="M25.3,40.714l-7-6.865c-0.189-0.186-0.297-0.439-0.3-0.704 c-0.003-0.266,0.101-0.521,0.286-0.71l7-7.135l1.428,1.4l-6.3,6.421l6.286,6.165L25.3,40.714z"/><linearGradient id="4mU0_HDsJp2MQbnKQxoync" x1="41.643" x2="41.643" y1="25.3" y2="40.714" gradientUnits="userSpaceOnUse" spreadMethod="reflect"><stop offset="0" stop-color="#6dc7ff"/><stop offset="1" stop-color="#e6abff"/></linearGradient><path fill="url(#4mU0_HDsJp2MQbnKQxoync)" d="M38.7,40.714l-1.4-1.428l6.286-6.165l-6.3-6.421l1.428-1.4l7,7.135 c0.186,0.189,0.289,0.444,0.286,0.71c-0.003,0.265-0.11,0.519-0.3,0.704L38.7,40.714z"/><linearGradient id="4mU0_HDsJp2MQbnKQxoynd" x1="32" x2="32" y1="22.713" y2="43.287" gradientUnits="userSpaceOnUse" spreadMethod="reflect"><stop offset="0" stop-color="#6dc7ff"/><stop offset="1" stop-color="#e6abff"/></linearGradient><path fill="url(#4mU0_HDsJp2MQbnKQxoynd)" d="M29.958 43.287L28.042 42.713 34.042 22.713 35.958 23.287z"/></svg>`;
        case "txt":
            return `<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 100 100" fill="#FFFFFF">  <path d="M34.179688 10.164062C32.103687 10.164062 30.615234 11.426266 30.615234 13.697266L30.648438 25.806641C30.648438 28.846641 32.770234 30.712891 35.615234 30.712891C38.460234 30.712891 40.615234 29.002891 40.615234 25.962891L40.615234 14.306641L39.613281 14.306641L39.523438 25.869141C39.523438 28.079141 37.599438 29.808594 35.648438 29.808594C33.697437 29.808594 31.615234 28.049844 31.615234 25.839844L31.644531 13.611328C31.644531 12.304328 33.077484 11.126953 34.271484 11.126953C35.465484 11.126953 36.615234 12.275031 36.615234 13.582031L36.615234 24.056641C36.615234 24.056641 36.139813 25.056641 35.632812 25.056641C35.125812 25.056641 34.486234 24.700391 34.615234 24.150391L34.615234 14.308594L33.615234 14.308594L33.582031 23.964844C33.582031 25.344844 34.32225 25.964844 35.65625 25.964844C36.99025 25.964844 37.615234 25.249141 37.615234 23.869141L37.615234 13.664062C37.615234 11.393062 36.255687 10.164062 34.179688 10.164062 z M 41.613281 11.306641C41.061281 11.306641 40.613281 11.754641 40.613281 12.306641C40.613281 12.858641 41.061281 13.306641 41.613281 13.306641L59.613281 13.306641L59.613281 23.792969C59.613281 29.039969 63.881906 33.306641 69.128906 33.306641L79.613281 33.306641L79.613281 82.574219C79.613281 86.837219 76.146812 90.306641 71.882812 90.306641L28.347656 90.306641C24.084656 90.306641 20.615234 86.838219 20.615234 82.574219L20.615234 21.039062C20.615234 16.776063 24.083656 13.308594 28.347656 13.308594L29.615234 13.308594C30.167234 13.308594 30.615234 12.860594 30.615234 12.308594C30.615234 11.756594 30.167234 11.308594 29.615234 11.308594L28.347656 11.308594C22.981656 11.308594 18.615234 15.672062 18.615234 21.039062L18.615234 82.574219C18.615234 87.940219 22.980656 92.306641 28.347656 92.306641L71.882812 92.306641C77.248812 92.306641 81.613281 87.940219 81.613281 82.574219L81.613281 32.554688C81.613281 32.289688 81.508313 32.035656 81.320312 31.847656L61.072266 11.599609C60.885266 11.411609 60.630234 11.306641 60.365234 11.306641L41.613281 11.306641 z M 60.613281 13.966797L78.951172 32.306641L69.128906 32.306641C64.433906 32.306641 60.613281 28.487969 60.613281 23.792969L60.613281 13.966797 z M 28.371094 19.505859C28.245219 19.481734 28.110094 19.505984 27.996094 19.583984C25.878094 21.023984 24.613281 23.406078 24.613281 25.955078L24.613281 78.617188C24.613281 82.856187 28.048484 86.306641 32.271484 86.306641L67.955078 86.306641C72.178078 86.306641 75.613234 82.856188 75.615234 78.617188L75.615234 60.806641C75.615234 60.530641 75.391234 60.306641 75.115234 60.306641C74.839234 60.306641 74.615234 60.530641 74.615234 60.806641L74.615234 78.617188C74.615234 82.305187 71.628031 85.306641 67.957031 85.306641L32.273438 85.306641C28.602438 85.306641 25.615234 82.305188 25.615234 78.617188L25.615234 25.955078C25.615234 23.737078 26.714594 21.664156 28.558594 20.410156C28.786594 20.255156 28.846406 19.945797 28.691406 19.716797C28.613906 19.603297 28.496969 19.529984 28.371094 19.505859 z M 75.113281 47.306641C74.837281 47.306641 74.613281 47.530641 74.613281 47.806641L74.613281 50.806641C74.613281 51.082641 74.837281 51.306641 75.113281 51.306641C75.390281 51.306641 75.613281 51.082641 75.613281 50.806641L75.613281 47.806641C75.613281 47.530641 75.389281 47.306641 75.113281 47.306641 z M 30.113281 51.306641C29.837281 51.306641 29.613281 51.530641 29.613281 51.806641C29.613281 52.082641 29.837281 52.306641 30.113281 52.306641L53.113281 52.306641C53.390281 52.306641 53.613281 52.082641 53.613281 51.806641C53.613281 51.530641 53.389281 51.306641 53.113281 51.306641L30.113281 51.306641 z M 75.113281 52.306641C74.837281 52.306641 74.613281 52.530641 74.613281 52.806641L74.613281 58.806641C74.613281 59.082641 74.837281 59.306641 75.113281 59.306641C75.390281 59.306641 75.613281 59.082641 75.613281 58.806641L75.613281 52.806641C75.613281 52.530641 75.389281 52.306641 75.113281 52.306641 z M 30.113281 58.306641C29.837281 58.306641 29.613281 58.530641 29.613281 58.806641C29.613281 59.082641 29.837281 59.306641 30.113281 59.306641L44.113281 59.306641C44.390281 59.306641 44.613281 59.082641 44.613281 58.806641C44.613281 58.530641 44.389281 58.306641 44.113281 58.306641L30.113281 58.306641 z M 46.113281 58.306641C45.837281 58.306641 45.613281 58.530641 45.613281 58.806641C45.613281 59.082641 45.837281 59.306641 46.113281 59.306641L70.113281 59.306641C70.390281 59.306641 70.613281 59.082641 70.613281 58.806641C70.613281 58.530641 70.389281 58.306641 70.113281 58.306641L46.113281 58.306641 z M 30.113281 65.306641C29.837281 65.306641 29.613281 65.530641 29.613281 65.806641C29.613281 66.082641 29.837281 66.306641 30.113281 66.306641L48.113281 66.306641C48.390281 66.306641 48.613281 66.082641 48.613281 65.806641C48.613281 65.530641 48.389281 65.306641 48.113281 65.306641L30.113281 65.306641 z M 50.113281 65.306641C49.837281 65.306641 49.613281 65.530641 49.613281 65.806641C49.613281 66.082641 49.837281 66.306641 50.113281 66.306641L61.113281 66.306641C61.390281 66.306641 61.613281 66.082641 61.613281 65.806641C61.613281 65.530641 61.389281 65.306641 61.113281 65.306641L50.113281 65.306641 z M 63.113281 65.306641C62.837281 65.306641 62.613281 65.530641 62.613281 65.806641C62.613281 66.082641 62.837281 66.306641 63.113281 66.306641L71.113281 66.306641C71.390281 66.306641 71.613281 66.082641 71.613281 65.806641C71.613281 65.530641 71.389281 65.306641 71.113281 65.306641L63.113281 65.306641 z M 30.113281 72.306641C29.837281 72.306641 29.613281 72.530641 29.613281 72.806641C29.613281 73.082641 29.837281 73.306641 30.113281 73.306641L54.113281 73.306641C54.390281 73.306641 54.613281 73.082641 54.613281 72.806641C54.613281 72.530641 54.389281 72.306641 54.113281 72.306641L30.113281 72.306641 z M 57.113281 72.306641C56.837281 72.306641 56.613281 72.530641 56.613281 72.806641C56.613281 73.082641 56.837281 73.306641 57.113281 73.306641L70.113281 73.306641C70.390281 73.306641 70.613281 73.082641 70.613281 72.806641C70.613281 72.530641 70.389281 72.306641 70.113281 72.306641L57.113281 72.306641 z" fill="#FFFFFF" /></svg>`;
        case "css":
            return `<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 64 64"><linearGradient id="0QsINg2dXUJ2sgMWOelW1a" x1="32" x2="32" y1="565" y2="509.988" gradientTransform="matrix(1 0 0 -1 0 570)" gradientUnits="userSpaceOnUse" spreadMethod="reflect"><stop offset="0" stop-color="#1a6dff"/><stop offset="1" stop-color="#c822ff"/></linearGradient><path fill="url(#0QsINg2dXUJ2sgMWOelW1a)" d="M51.121,15.707l-8.828-8.828C41.727,6.313,40.973,6,40.171,6H15c-1.654,0-3,1.346-3,3v40v2v4 c0,1.654,1.346,3,3,3h34c1.654,0,3-1.346,3-3v-4v-2V17.828C52,17.027,51.688,16.273,51.121,15.707z M48.586,16H43 c-0.551,0-1-0.448-1-1V9.414L48.586,16z M49,56H15c-0.551,0-1-0.448-1-1v-1.185C14.314,53.928,14.648,54,15,54h34 c0.352,0,0.686-0.072,1-0.184V55C50,55.552,49.551,56,49,56z M50,51c0,0.552-0.449,1-1,1H15c-0.551,0-1-0.448-1-1v-2V9 c0-0.552,0.449-1,1-1h25v7c0,1.654,1.346,3,3,3h7v31V51z"/><linearGradient id="0QsINg2dXUJ2sgMWOelW1b" x1="23.75" x2="23.75" y1="29" y2="39" gradientUnits="userSpaceOnUse" spreadMethod="reflect"><stop offset="0" stop-color="#6dc7ff"/><stop offset="1" stop-color="#e6abff"/></linearGradient><path fill="url(#0QsINg2dXUJ2sgMWOelW1b)" d="M26,39h-3c-1.105,0-2-0.895-2-2v-6c0-1.105,0.895-2,2-2h3c0.276,0,0.5,0.224,0.5,0.5v1 c0,0.276-0.224,0.5-0.5,0.5h-2.5c-0.276,0-0.5,0.224-0.5,0.5v5c0,0.276,0.224,0.5,0.5,0.5H26c0.276,0,0.5,0.224,0.5,0.5v1 C26.5,38.776,26.276,39,26,39z"/><linearGradient id="0QsINg2dXUJ2sgMWOelW1c" x1="32" x2="32" y1="29" y2="39" gradientUnits="userSpaceOnUse" spreadMethod="reflect"><stop offset="0" stop-color="#6dc7ff"/><stop offset="1" stop-color="#e6abff"/></linearGradient><path fill="url(#0QsINg2dXUJ2sgMWOelW1c)" d="M33,39h-3.5c-0.276,0-0.5-0.224-0.5-0.5v-1c0-0.276,0.224-0.5,0.5-0.5H33v-2h-2 c-1.105,0-2-0.895-2-2v-2c0-1.105,0.895-2,2-2h2.5c0.276,0,0.5,0.224,0.5,0.5v1c0,0.276-0.224,0.5-0.5,0.5H31v2h2 c1.105,0,2,0.895,2,2v2C35,38.105,34.105,39,33,39z"/><linearGradient id="0QsINg2dXUJ2sgMWOelW1d" x1="40" x2="40" y1="29" y2="39" gradientUnits="userSpaceOnUse" spreadMethod="reflect"><stop offset="0" stop-color="#6dc7ff"/><stop offset="1" stop-color="#e6abff"/></linearGradient><path fill="url(#0QsINg2dXUJ2sgMWOelW1d)" d="M41,39h-3.5c-0.276,0-0.5-0.224-0.5-0.5v-1c0-0.276,0.224-0.5,0.5-0.5H41v-2h-2 c-1.105,0-2-0.895-2-2v-2c0-1.105,0.895-2,2-2h2.5c0.276,0,0.5,0.224,0.5,0.5v1c0,0.276-0.224,0.5-0.5,0.5H39v2h2 c1.105,0,2,0.895,2,2v2C43,38.105,42.105,39,41,39z"/></svg>`;
        case "js":
            return `<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 48 48"><path fill="#ffd600" d="M6,42V6h36v36H6z"/><path fill="#000001" d="M29.538 32.947c.692 1.124 1.444 2.201 3.037 2.201 1.338 0 2.04-.665 2.04-1.585 0-1.101-.726-1.492-2.198-2.133l-.807-.344c-2.329-.988-3.878-2.226-3.878-4.841 0-2.41 1.845-4.244 4.728-4.244 2.053 0 3.528.711 4.592 2.573l-2.514 1.607c-.553-.988-1.151-1.377-2.078-1.377-.946 0-1.545.597-1.545 1.377 0 .964.6 1.354 1.985 1.951l.807.344C36.452 29.645 38 30.839 38 33.523 38 36.415 35.716 38 32.65 38c-2.999 0-4.702-1.505-5.65-3.368L29.538 32.947zM17.952 33.029c.506.906 1.275 1.603 2.381 1.603 1.058 0 1.667-.418 1.667-2.043V22h3.333v11.101c0 3.367-1.953 4.899-4.805 4.899-2.577 0-4.437-1.746-5.195-3.368L17.952 33.029z"/></svg>`;
        case "mkv":
            return `<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 32 32" fill="#FFFFFF">  <path d="M6 3L6 10L8 10L8 5L24 5L24 10L26 10L26 3L6 3 z M 7 12L7 20L9 20L9 15.332031L10 16.666016L11 15.332031L11 20L13 20L13 12L11 12L10 13.332031L9 12L7 12 z M 14 12L14 20L16 20L16 18L16.111328 17.777344L17 20L19 20L17.222656 15.554688L19 12L17 12L16 14L16 12L14 12 z M 20 12L20 13.150391L22.107422 20L23.720703 20L26 13.162109L26 12L24 12L24 12.837891L22.957031 15.964844L22 12.849609L22 12L20 12 z M 6 22L6 29L26 29L26 22L24 22L24 27L8 27L8 22L6 22 z" fill="#FFFFFF" /></svg>`;
        case "ogg":
            return `<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 48 48"><path fill="#ffc107" d="M40 45L8 45 8 3 30 3 40 13z"/><path fill="#fff3e0" d="M38.5 14L29 14 29 4.5z"/><path fill="#c47205" d="M19.307 28.893c0 1.334-.315 2.373-.947 3.111-.631.738-1.507 1.106-2.628 1.106-1.117 0-1.994-.365-2.632-1.096-.639-.732-.962-1.756-.971-3.072v-1.703c0-1.367.316-2.434.95-3.203.633-.768 1.513-1.152 2.639-1.152 1.107 0 1.979.379 2.618 1.133.638.754.961 1.813.971 3.176V28.893zM17.29 27.225c0-.896-.128-1.564-.383-2.002-.256-.438-.652-.656-1.189-.656-.533 0-.928.211-1.183.631-.256.422-.388 1.064-.397 1.924v1.771c0 .869.13 1.512.39 1.924.26.414.66.619 1.203.619.523 0 .913-.201 1.169-.605.255-.402.385-1.029.39-1.877V27.225zM27.517 31.846c-.388.42-.862.734-1.426.947-.563.211-1.179.316-1.849.316-1.145 0-2.033-.354-2.666-1.063-.634-.707-.96-1.738-.978-3.094v-1.789c0-1.373.3-2.43.899-3.17.6-.74 1.474-1.111 2.622-1.111 1.08 0 1.895.268 2.443.801.55.533.867 1.369.954 2.508h-1.955c-.055-.633-.187-1.064-.396-1.295-.21-.229-.538-.346-.984-.346-.543 0-.937.199-1.183.596-.246.396-.374 1.027-.383 1.893v1.805c0 .908.136 1.566.406 1.98.272.412.717.617 1.337.617.396 0 .718-.078.964-.238L25.5 31.08v-1.826h-1.408v-1.516h3.425V31.846zM35.706 31.846c-.388.42-.862.734-1.426.947-.563.211-1.179.316-1.849.316-1.144 0-2.033-.354-2.666-1.063-.634-.707-.96-1.738-.978-3.094v-1.789c0-1.373.3-2.43.898-3.17.6-.74 1.474-1.111 2.622-1.111 1.08 0 1.895.268 2.443.801.55.533.867 1.369.954 2.508h-1.955c-.055-.633-.188-1.064-.396-1.295-.21-.229-.538-.346-.984-.346-.543 0-.936.199-1.183.596s-.374 1.027-.383 1.893v1.805c0 .908.136 1.566.406 1.98.272.412.717.617 1.337.617.396 0 .718-.078.964-.238l.178-.123v-1.826h-1.408v-1.516h3.425V31.846z"/></svg>`;
        case "gif":
            return `<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 24 24" fill="#FFFFFF">  <path d="M6 2C4.9057453 2 4 2.9057453 4 4L4 9C2.895 9 2 9.895 2 11L2 16C2 17.105 2.895 18 4 18L4 20C4 21.094255 4.9057453 22 6 22L18 22C19.094255 22 20 21.094255 20 20L20 7L15 2L6 2 z M 6 4L14 4L14 8L18 8L18 20L6 20L6 18L14 18C15.105 18 16 17.105 16 16L16 11C16 9.895 15.105 9 14 9L6 9L6 4 z M 6.0351562 11C7.2071562 11.001 7.6732813 11.537 7.8632812 12L6 12C5.118 11.987 5 12.856937 5 13.085938L5 13.914062C5 14.145062 5.2067813 15.01 6.1757812 15C6.4587813 14.997 6.976 14.836266 7 14.822266L6.9785156 14L6 14L6.0214844 13L8 13L8 15.380859C7.94 15.436859 7.2914219 15.961047 6.1074219 15.998047C5.7854219 16.008047 4.132 16.038062 4 13.914062L4 13.091797C4 12.736797 3.9581563 10.998 6.0351562 11 z M 9 11L10 11L10 16L9 16L9 11 z M 11 11L14 11L14 12L12 12L12 13L13.599609 13L13.599609 14L12 14L12 16L11 16L11 11 z" fill="#FFFFFF" /></svg>`;
        case "webm":
            return `<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 26 26" fill="#FFFFFF">  <path d="M25,2V1c0-0.551-0.449-1-1-1H2C1.449,0,1,0.449,1,1v1h2v3H1v16h2v3H1v1c0,0.551,0.449,1,1,1h22c0.551,0,1-0.449,1-1v-1h-1v-3h1V5h-1V2H25z M12,2h3v3h-3V2z M9,24H6v-3h3V24z M9,5H6V2h3V5z M15,24h-3v-3h3V24z M17.626,13.597l-6.593,3.806C10.574,17.668,10,17.337,10,16.806V9.194c0-0.531,0.574-0.862,1.034-0.597l6.593,3.806C18.086,12.668,18.086,13.332,17.626,13.597z M21,24h-3v-3h3V24z M21,5h-3V2h3V5z" fill="#FFFFFF" /></svg>`;
        case "avi":
            return `<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 80 80" fill="#FFFFFF">  <path d="M8 12L8 68L72 68L72 12L8 12 z M 11 14L15 14C15.551 14 16 14.449 16 15L16 17C16 17.551 15.551 18 15 18L11 18C10.449 18 10 17.551 10 17L10 15C10 14.449 10.449 14 11 14 z M 18 14L62 14L62 66L18 66L18 14 z M 65 14L69 14C69.551 14 70 14.449 70 15L70 17C70 17.551 69.551 18 69 18L65 18C64.449 18 64 17.551 64 17L64 15C64 14.449 64.449 14 65 14 z M 11 20L15 20C15.551 20 16 20.449 16 21L16 23C16 23.551 15.551 24 15 24L11 24C10.449 24 10 23.551 10 23L10 21C10 20.449 10.449 20 11 20 z M 65 20L69 20C69.551 20 70 20.449 70 21L70 23C70 23.551 69.551 24 69 24L65 24C64.449 24 64 23.551 64 23L64 21C64 20.449 64.449 20 65 20 z M 11 26L15 26C15.551 26 16 26.449 16 27L16 29C16 29.551 15.551 30 15 30L11 30C10.449 30 10 29.551 10 29L10 27C10 26.449 10.449 26 11 26 z M 65 26L69 26C69.551 26 70 26.449 70 27L70 29C70 29.551 69.551 30 69 30L65 30C64.449 30 64 29.551 64 29L64 27C64 26.449 64.449 26 65 26 z M 11 32L15 32C15.551 32 16 32.449 16 33L16 35C16 35.551 15.551 36 15 36L11 36C10.449 36 10 35.551 10 35L10 33C10 32.449 10.449 32 11 32 z M 65 32L69 32C69.551 32 70 32.449 70 33L70 35C70 35.551 69.551 36 69 36L65 36C64.449 36 64 35.551 64 35L64 33C64 32.449 64.449 32 65 32 z M 34.318359 32.011719C33.630703 32.015375 33 32.566141 33 33.337891L33 46.662109C33 47.691109 34.121906 48.327828 35.003906 47.798828L46.462891 40.923828C47.159891 40.505828 47.159891 39.495125 46.462891 39.078125L35.003906 32.203125C34.783406 32.070875 34.547578 32.0105 34.318359 32.011719 z M 11 38L15 38C15.551 38 16 38.449 16 39L16 41C16 41.551 15.551 42 15 42L11 42C10.449 42 10 41.551 10 41L10 39C10 38.449 10.449 38 11 38 z M 65 38L69 38C69.551 38 70 38.449 70 39L70 41C70 41.551 69.551 42 69 42L65 42C64.449 42 64 41.551 64 41L64 39C64 38.449 64.449 38 65 38 z M 11 44L15 44C15.551 44 16 44.449 16 45L16 47C16 47.551 15.551 48 15 48L11 48C10.449 48 10 47.551 10 47L10 45C10 44.449 10.449 44 11 44 z M 65 44L69 44C69.551 44 70 44.449 70 45L70 47C70 47.551 69.551 48 69 48L65 48C64.449 48 64 47.551 64 47L64 45C64 44.449 64.449 44 65 44 z M 11 50L15 50C15.551 50 16 50.449 16 51L16 53C16 53.551 15.551 54 15 54L11 54C10.449 54 10 53.551 10 53L10 51C10 50.449 10.449 50 11 50 z M 65 50L69 50C69.551 50 70 50.449 70 51L70 53C70 53.551 69.551 54 69 54L65 54C64.449 54 64 53.551 64 53L64 51C64 50.449 64.449 50 65 50 z M 11 56L15 56C15.551 56 16 56.449 16 57L16 59C16 59.551 15.551 60 15 60L11 60C10.449 60 10 59.551 10 59L10 57C10 56.449 10.449 56 11 56 z M 65 56L69 56C69.551 56 70 56.449 70 57L70 59C70 59.551 69.551 60 69 60L65 60C64.449 60 64 59.551 64 59L64 57C64 56.449 64.449 56 65 56 z M 24 60 A 1 1 0 0 0 23 61 A 1 1 0 0 0 24 62 A 1 1 0 0 0 25 61 A 1 1 0 0 0 24 60 z M 28 60 A 1 1 0 0 0 27 61 A 1 1 0 0 0 28 62 A 1 1 0 0 0 29 61 A 1 1 0 0 0 28 60 z M 32 60 A 1 1 0 0 0 31 61 A 1 1 0 0 0 32 62 A 1 1 0 0 0 33 61 A 1 1 0 0 0 32 60 z M 36 60 A 1 1 0 0 0 35 61 A 1 1 0 0 0 36 62 A 1 1 0 0 0 37 61 A 1 1 0 0 0 36 60 z M 40 60 A 1 1 0 0 0 39 61 A 1 1 0 0 0 40 62 A 1 1 0 0 0 41 61 A 1 1 0 0 0 40 60 z M 44 60 A 1 1 0 0 0 43 61 A 1 1 0 0 0 44 62 A 1 1 0 0 0 45 61 A 1 1 0 0 0 44 60 z M 48 60 A 1 1 0 0 0 47 61 A 1 1 0 0 0 48 62 A 1 1 0 0 0 49 61 A 1 1 0 0 0 48 60 z M 52 60 A 1 1 0 0 0 51 61 A 1 1 0 0 0 52 62 A 1 1 0 0 0 53 61 A 1 1 0 0 0 52 60 z M 56 60 A 1 1 0 0 0 55 61 A 1 1 0 0 0 56 62 A 1 1 0 0 0 57 61 A 1 1 0 0 0 56 60 z M 11 62L15 62C15.551 62 16 62.449 16 63L16 65C16 65.551 15.551 66 15 66L11 66C10.449 66 10 65.551 10 65L10 63C10 62.449 10.449 62 11 62 z M 65 62L69 62C69.551 62 70 62.449 70 63L70 65C70 65.551 69.551 66 69 66L65 66C64.449 66 64 65.551 64 65L64 63C64 62.449 64.449 62 65 62 z" fill="#FFFFFF" /></svg>`;
        case "html":
            return `<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 48 48"><path fill="#E65100" d="M41,5H7l3,34l14,4l14-4L41,5L41,5z"/><path fill="#FF6D00" d="M24 8L24 39.9 35.2 36.7 37.7 8z"/><path fill="#FFF" d="M24,25v-4h8.6l-0.7,11.5L24,35.1v-4.2l4.1-1.4l0.3-4.5H24z M32.9,17l0.3-4H24v4H32.9z"/><path fill="#EEE" d="M24,30.9v4.2l-7.9-2.6L15.7,27h4l0.2,2.5L24,30.9z M19.1,17H24v-4h-9.1l0.7,12H24v-4h-4.6L19.1,17z"/></svg>`;
        default:
            return `<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 48 48"><defs><linearGradient id="Gq7ZuXOiiL83JSLL__tY6g" x1="23" x2="23" y1="13.221" y2="10.491" gradientUnits="userSpaceOnUse"><stop offset="0" stop-color="#3965e3"/><stop offset=".692" stop-color="#4f9bf6"/><stop offset="1" stop-color="#59b4ff"/></linearGradient><linearGradient id="Gq7ZuXOiiL83JSLL__tY6b" x1="24" x2="24" y1="12" y2="41" gradientUnits="userSpaceOnUse"><stop offset=".011" stop-color="#b3b3b3"/><stop offset=".013" stop-color="#b5b5b5" stop-opacity=".868"/><stop offset=".015" stop-color="#b9b9b9" stop-opacity=".665"/><stop offset=".019" stop-color="#bcbcbc" stop-opacity=".484"/><stop offset=".022" stop-color="#bebebe" stop-opacity=".332"/><stop offset=".026" stop-color="silver" stop-opacity=".209"/><stop offset=".031" stop-color="#c2c2c2" stop-opacity=".114"/><stop offset=".036" stop-color="#c3c3c3" stop-opacity=".048"/><stop offset=".044" stop-color="#c4c4c4" stop-opacity=".01"/><stop offset=".066" stop-color="#c4c4c4" stop-opacity="0"/><stop offset="1" stop-color="#c4c4c4" stop-opacity="0"/></linearGradient><linearGradient id="Gq7ZuXOiiL83JSLL__tY6c" x1="42.777" x2="19.49" y1="18.254" y2="34.559" gradientTransform="matrix(-1 0 0 1 48 0)" xlink:href="#Gq7ZuXOiiL83JSLL__tY6a"/><linearGradient id="Gq7ZuXOiiL83JSLL__tY6d" x1="386" x2="386" y1="33" y2="10" gradientTransform="matrix(-1 0 0 1 410 0)" gradientUnits="userSpaceOnUse"><stop offset=".839" stop-color="#cf6c00"/><stop offset=".858" stop-color="#d67f10"/><stop offset=".898" stop-color="#e3a12e"/><stop offset=".937" stop-color="#edbb43"/><stop offset=".972" stop-color="#f3ca50"/><stop offset="1" stop-color="#f5cf55"/></linearGradient><linearGradient id="Gq7ZuXOiiL83JSLL__tY6e" x1="24" x2="24" y1="35" y2="13" gradientUnits="userSpaceOnUse"><stop offset=".787" stop-color="#cf6c00"/><stop offset=".812" stop-color="#d67f10"/><stop offset=".866" stop-color="#e3a12e"/><stop offset=".916" stop-color="#edbb43"/><stop offset=".963" stop-color="#f3ca50"/><stop offset="1" stop-color="#f5cf55"/></linearGradient><linearGradient id="Gq7ZuXOiiL83JSLL__tY6f" x1="24" x2="24" y1="37" y2="16" gradientUnits="userSpaceOnUse"><stop offset="0" stop-color="#ecbe29"/><stop offset=".142" stop-color="#f1c940"/><stop offset=".361" stop-color="#f7d85d"/><stop offset=".578" stop-color="#fbe271"/><stop offset=".793" stop-color="#fee87e"/><stop offset="1" stop-color="#ffea82"/></linearGradient><linearGradient id="Gq7ZuXOiiL83JSLL__tY6a" x1="42.777" x2="19.49" y1="18.254" y2="34.559" gradientUnits="userSpaceOnUse"><stop offset=".022" stop-color="#b3b3b3"/><stop offset=".023" stop-color="#b5b5b5" stop-opacity=".868"/><stop offset=".026" stop-color="#b9b9b9" stop-opacity=".665"/><stop offset=".029" stop-color="#bcbcbc" stop-opacity=".484"/><stop offset=".032" stop-color="#bebebe" stop-opacity=".332"/><stop offset=".035" stop-color="silver" stop-opacity=".209"/><stop offset=".039" stop-color="#c2c2c2" stop-opacity=".114"/><stop offset=".044" stop-color="#c3c3c3" stop-opacity=".048"/><stop offset=".051" stop-color="#c4c4c4" stop-opacity=".01"/><stop offset=".07" stop-color="#c4c4c4" stop-opacity="0"/><stop offset="1" stop-color="#c4c4c4" stop-opacity="0"/></linearGradient><linearGradient id="Gq7ZuXOiiL83JSLL__tY6h" x1="17" x2="17" y1="18.162" y2="12.464" gradientUnits="userSpaceOnUse"><stop offset=".181" stop-color="#a33100"/><stop offset=".573" stop-color="#d9451a"/><stop offset=".816" stop-color="#fc522b"/></linearGradient><linearGradient id="Gq7ZuXOiiL83JSLL__tY6i" x1="20" x2="20" y1="12.162" y2="6.464" gradientTransform="matrix(-1 0 0 1 48 0)" gradientUnits="userSpaceOnUse"><stop offset="0" stop-color="#6da60a"/><stop offset=".663" stop-color="#78d22a"/></linearGradient><linearGradient id="Gq7ZuXOiiL83JSLL__tY6l" x1="24" x2="24" y1="35" y2="27" gradientUnits="userSpaceOnUse"><stop offset="0" stop-color="#666"/><stop offset="1" stop-color="#c4c4c4"/></linearGradient><linearGradient id="Gq7ZuXOiiL83JSLL__tY6k" x1="24" x2="24" y1="20.414" y2="21.823" gradientUnits="userSpaceOnUse"><stop offset=".092" stop-color="#b3b3b3"/><stop offset=".118" stop-color="#b5b5b5" stop-opacity=".868"/><stop offset=".164" stop-color="#b9b9b9" stop-opacity=".665"/><stop offset=".216" stop-color="#bcbcbc" stop-opacity=".484"/><stop offset=".273" stop-color="#bebebe" stop-opacity=".332"/><stop offset=".337" stop-color="silver" stop-opacity=".209"/><stop offset=".413" stop-color="#c2c2c2" stop-opacity=".114"/><stop offset=".506" stop-color="#c3c3c3" stop-opacity=".048"/><stop offset=".637" stop-color="#c4c4c4" stop-opacity=".01"/><stop offset="1" stop-color="#c4c4c4" stop-opacity="0"/></linearGradient><linearGradient id="Gq7ZuXOiiL83JSLL__tY6m" x1="24" x2="24" y1="34" y2="28" gradientUnits="userSpaceOnUse"><stop offset="0" stop-color="#a6a6a6"/><stop offset="1" stop-color="#f2f2f2"/></linearGradient><radialGradient id="Gq7ZuXOiiL83JSLL__tY6j" cx="23.237" cy="13.25" r="48.98" gradientTransform="matrix(1 0 0 .937 0 .832)" gradientUnits="userSpaceOnUse"><stop offset="0" stop-color="#d1d1d1"/><stop offset=".266" stop-color="#8b8b8b"/><stop offset=".52" stop-color="#4f4f4f"/><stop offset=".734" stop-color="#242424"/><stop offset=".901" stop-color="#0a0a0a"/><stop offset="1"/></radialGradient></defs><g data-name="card file box"><polygon fill="#242424" points="4 20 10 12 38 12 44 20 24 41 4 20"/><polygon fill="url(#Gq7ZuXOiiL83JSLL__tY6b)" points="10 12 38 12 24 41 10 12"/><polygon fill="url(#Gq7ZuXOiiL83JSLL__tY6a)" points="38 12 44 20 24 41 38 12"/><polygon fill="url(#Gq7ZuXOiiL83JSLL__tY6c)" points="10 12 4 20 24 41 10 12"/><rect width="24" height="23" x="12" y="10" fill="url(#Gq7ZuXOiiL83JSLL__tY6d)" rx="2"/><rect width="28" height="22" x="10" y="13" fill="url(#Gq7ZuXOiiL83JSLL__tY6e)" rx="2"/><rect width="32" height="21" x="8" y="16" fill="url(#Gq7ZuXOiiL83JSLL__tY6f)" rx="2"/><path fill="url(#Gq7ZuXOiiL83JSLL__tY6g)" d="M18,13l1.41-2.11A1.981,1.981,0,0,1,21.07,10h3.87a2.007,2.007,0,0,1,1.67.9L28,13Z"/><path fill="url(#Gq7ZuXOiiL83JSLL__tY6h)" d="M12,16l1.41-2.11A1.981,1.981,0,0,1,15.07,13h3.87a2.007,2.007,0,0,1,1.67.9L22,16Z"/><path fill="url(#Gq7ZuXOiiL83JSLL__tY6i)" d="M33,10,31.59,7.89A1.981,1.981,0,0,0,29.93,7H26.06a2.007,2.007,0,0,0-1.67.9L23,10Z"/><path fill="url(#Gq7ZuXOiiL83JSLL__tY6j)" d="M42,41H6a2,2,0,0,1-2-2V20H44V39A2,2,0,0,1,42,41Z"/><path fill="url(#Gq7ZuXOiiL83JSLL__tY6k)" d="M42,41H6a2,2,0,0,1-2-2V20H44V39A2,2,0,0,1,42,41Z"/><rect width="16" height="10" x="16" y="26" opacity=".05" rx="2"/><rect width="15" height="9" x="16.5" y="26.5" opacity=".07" rx="1.5"/><rect width="14" height="8" x="17" y="27" fill="url(#Gq7ZuXOiiL83JSLL__tY6l)" rx="1"/><rect width="12" height="6" x="18" y="28" fill="url(#Gq7ZuXOiiL83JSLL__tY6m)" rx=".5"/></g></svg>`;
    }
}
const eventListener = () => {
    newFolder.on("click", () => {
        wrapperCreateFolder.find(".file").addClass("d-none");
        wrapperCreateFolder.find(".folder").removeClass("d-none");
        wrapperCreateFolder.fadeIn("slow");
    });
    backFolder.on("click", () => {
        wrapperCreateFolder.fadeOut("slow");
    });
    backNewFile.on("click", () => {
        wrapperCreateFolder.fadeOut("slow");
    });
    createNewFolder.on("click", () => {
        const handler = new Handler();
        if (folderName.val()) {
            const configData = {
                folderName: folderName.val(),
                path: localVariable.currentPath,
                parentId: localVariable.parentToSave,
                ticket: ""
            };
            handler.beforeSend = () => { };
            handler.complete = () => { };
            handler.success = (data) => {
                if (data.done) {
                    folderName.val("");
                    createFolder(data.result.name, fileWrapper, data.result);
                    backFolder.click();
                } else {
                    handler.configError(data);
                }
            }
            Api.post({ url: localVariable.urls.createFolder, data: configData, handler: handler });
        } else {
            handler.configError({ done: false, errorDesc: "لطفا نام پوشه را وارد نمایید" });
        }
    });
    prevFolder.on("click", () => {
        var where = new Wheres();
        where.add("displayInFileManager", "true", ENVIRONMENT.Condition.EQUAL, ENVIRONMENT.Operator.AND, 1, [], ENVIRONMENT.FieldType.BOOLEAN);
        var parentToSearch = localVariable.lastParentId.pop();
        if (!parentToSearch) {
            prevFolder.attr("disabled", "disabled");
            where.add("parentId", "null", ENVIRONMENT.Condition.IS_NULL, ENVIRONMENT.Operator.AND);
        } else {
            prevFolder.removeAttr("disabled");
            where.add("parentId", `${parentToSearch}`, ENVIRONMENT.Condition.EQUAL, ENVIRONMENT.Operator.AND, 1, [], ENVIRONMENT.FieldType.LONG);
        }
        const configData = {
            wheres: where.getList(),
            pageSize: 0,
            pageNo: 0,
            ticket: "",
            orderBy: [],
            language: 1
        };
        const handler = new Handler();
        handler.complete = () => {
            hideLoading();
        };
        handler.success = (data) => {
            if (data.done) {
                createFileAndFolder(data.result);
            } else {
                handler.configError(data);
            }
        }
        Api.post({ url: localVariable.urls.gridView, data: configData, handler: handler });
    });
    fileUpload.on("click", () => {
        fileDialog.trigger('click');
    });
    fileDialog.on("change", () => {
        wrapperCreateFolder.find(".folder").addClass("d-none");
        wrapperCreateFolder.find(".file").removeClass("d-none");
        wrapperCreateFolder.fadeIn("slow");
    });
    createNewFile.on("click", () => {
        const handler = new Handler();
        if (fileName.val()) {
            const formData = new FormData();
            formData.append('ticket', '');
            formData.append('name', fileName.val());
            formData.append('alt', alt.val());
            formData.append('description', description.val());
            formData.append('file', fileDialog[0].files[0]);
            formData.append('parentId', localVariable.parentToSave);
            formData.append('path', localVariable.currentPath);
            handler.success = (data) => {
                if (data.done) {
                    fileName.val("");
                    alt.val("");
                    description.val("");
                    createFile(data.result.name, fileWrapper, data.result);
                    backNewFile.click();
                } else {
                    handler.configError(data);
                }
            }
            Api.postForm({ url: localVariable.urls.createFile, formData: formData, isAsync: false, handler: handler });
        } else {
            handler.configError({ done: false, errorDesc: "لطفا نام فایل را وارد نمایید" });
        }
    });
    removeFile.on("click", () => {
        const handler = new Handler();
        handler.success = (resp) => {
            if (resp.done) {
                $('#details').modal("hide");
                $(`[entity-id="${resp.result.entityId}"]`).fadeOut("slow");
            } else {
                handler.configError(resp);
            }
        }
        var where = new Wheres();
        var entityId = removeFile.attr("remove-id");
        where.add("entityId", `${entityId}`, ENVIRONMENT.Condition.EQUAL, ENVIRONMENT.Operator.AND, 1, [], ENVIRONMENT.FieldType.LONG);
        const configData = {
            wheres: where.getList(),
            pageSize: 0,
            pageNo: 0,
            ticket: "",
            orderBy: [],
            language: 1
        };
        Api.post({ url: localVariable.urls.delete, data: configData, handler: handler });
    });
    filter.on("click", () => {
        var where = new Wheres();
        let flag = true;
        where.add("displayInFileManager", "true", ENVIRONMENT.Condition.EQUAL, ENVIRONMENT.Operator.AND, 1, [], ENVIRONMENT.FieldType.BOOLEAN);
        $.each($("#file-manager-filter-main-list input"), (index, item) => {
            if ($(item).prop("checked")) {
                flag = false;
                let ext = $(item).attr("extension");
                where.add("extension", `${ext}`, ENVIRONMENT.Condition.EQUAL, ENVIRONMENT.Operator.OR, 1, [], ENVIRONMENT.FieldType.STRING);
            }
        });
        if (flag) {
            where.add("parentId", "null", ENVIRONMENT.Condition.IS_NULL, ENVIRONMENT.Operator.AND);
        }
        const configData = {
            wheres: where.getList(),
            pageSize: 0,
            pageNo: 0,
            ticket: "",
            orderBy: [],
            language: 1
        };
        const handler = new Handler();
        handler.complete = () => {
            hideLoading();
        };
        handler.success = (data) => {
            if (data.done) {
                createFileAndFolder(data.result);
            } else {
                handler.configError(data);
            }
        }
        Api.post({ url: localVariable.urls.gridView, data: configData, handler: handler });
    });
}
gridView = () => {
    var where = new Wheres();
    where.add("parentId", "null", ENVIRONMENT.Condition.IS_NULL, ENVIRONMENT.Operator.AND);
    where.add("displayInFileManager", "true", ENVIRONMENT.Condition.EQUAL, ENVIRONMENT.Operator.AND, 1, [], ENVIRONMENT.FieldType.BOOLEAN);
    const configData = {
        wheres: where.getList(),
        pageSize: 0,
        pageNo: 0,
        ticket: "",
        orderBy: [],
        language: 1
    };
    const handler = new Handler();
    handler.complete = () => {
        hideLoading();
    };
    handler.success = (data) => {
        if (data.done) {
            createFileAndFolder(data.result);
        } else {
            handler.configError(data);
        }
    }
    Api.post({ url: localVariable.urls.gridView, data: configData, handler: handler });
}
$(function () {
    initializeVariable();
    eventListener();
    getExtensionForView();
    gridView();

})