/**
 * Created by F.Kazemi on 3/10/2018.
 */

var Autocomplete = function() {
    const defaultHandler = new Handler();
    defaultHandler.complete = () => {};
    var config = {
        element: undefined,
        placeholder: "$autocompletePlaceholder",
        url: "",
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        data: [{ id: 1, title: "", object: {} }],
        handler: defaultHandler,
        onSelect: null,
        onChange: null,
        autocompleteCacheTimeout: null
    };
    const init = function(configObject) {
        if (configObject) {
            if (configObject.autocompleteCacheTimeout) {
                config.autocompleteCacheTimeout = configObject.autocompleteCacheTimeout;
            }
            if (configObject.element) {
                config.element = configObject.element;
            }
            if (configObject.placeholder) {
                config.placeholder = configObject.placeholder;
            }
            if (configObject.url) {
                config.url = configObject.url;
            }
            if (configObject.type) {
                config.type = configObject.type;
            }
            if (configObject.dropdownCssClass) {
                config.dropdownCssClass = configObject.dropdownCssClass;
            }
            if (configObject.containerCssClass) {
                config.containerCssClass = configObject.containerCssClass;
            }
            if (configObject.data) {
                config.data = configObject.data;
            }
            if (configObject.handler) {
                config.handler = configObject.handler;
            }
            if (configObject.where) {
                config.where = configObject.where;
            }
            if (configObject.orderBy) {
                config.orderBy = configObject.orderBy;
            }
            if (configObject.onSelect) {
                config.onSelect = configObject.onSelect;
            }
            if (configObject.onChange) {
                config.onChange = configObject.onChange;
            }
            if (configObject.onClear) {
                config.onClear = configObject.onClear;
            }
            if (configObject.staticKey) {
                config.staticKey = btoa(encodeURIComponent(configObject.staticKey).replace(/%([0-9A-F]{2})/g,
                    function toSolidBytes(match, p1) {
                        return String.fromCharCode(`0x${p1}`);
                    }));
            }
            if (configObject.multipleSelect) {
                // multipleSelect: { count: 5, singleLine: true }
                config.multipleSelect = configObject.multipleSelect;
            }
            if (configObject.onNoResult) {
                config.onNoResult = configObject.onNoResult;
            }
            if (configObject.callback) {
                config.callback = configObject.callback;
            }
            if (configObject.dynamicConfig) {
                config.minCharacter =
                    configObject.dynamicConfig.minCharacter !== undefined &&
                    configObject.dynamicConfig.minCharacter !== null
                    ? configObject.dynamicConfig.minCharacter
                    : 1;
                config.searchColumn = configObject.dynamicConfig.searchColumn;
                config.text = configObject.dynamicConfig.text;
                config.value = configObject.dynamicConfig.value;
            } else {
                if (configObject.minCharacter) {
                    config.minCharacter = configObject.minCharacter ? configObject.minCharacter : 1;
                }
                if (configObject.searchColumn) {
                    config.searchColumn = configObject.searchColumn;
                }
                if (configObject.text) {
                    config.text = configObject.text ? configObject.text : "fullTitle";
                }
                if (configObject.value) {
                    config.value = configObject.value ? configObject.value : "entityId";
                }
            }
        }
        switch (config.type) {
        case ENVIRONMENT.Autocomplete.TYPE.STATIC:
            initStaticAutocomplete();
            break;
        case ENVIRONMENT.Autocomplete.TYPE.DYNAMIC:
            initDynamicAutocomplete();
            break;
        }
    };

    var initStaticAutocomplete = function() {
        var staticWhere = new Wheres();
        if (config.where) {
            staticWhere = config.where;
        }
        const staticHandler = new Handler();
        staticHandler.beforeSend = function() {
        };
        staticHandler.complete = function() {
        };
        staticHandler.error = function() {
        };
        const staticFilter = new Filter(staticWhere.getList(),
            100,
            1,
            (config.orderBy ? config.orderBy.get() : null),
            1,
            "");
        var staticItem = {};

        staticItem = Storage.get(config.staticKey);
        if (staticItem) { // agar baraye key morde-e nazar data vojood dashte bashad
            if (Util.differentTime(new Date().getTime(),
                staticItem.createdTime,
                config.autocompleteCacheTimeout !== null
                ? config.autocompleteCacheTimeout
                : Config.AUTOCOMPLETE_CACHE_TIMEOUT)
            ) { // agar bazeye zamani b payan nareside bashad, data az storage khande shavad
                fillStaticAutocomplete(staticItem.data);
            } else {
                staticHandler.success = function(data) {
                    if (data.done) {
                        // map result
                        var temp = [];
                        data.result.forEach(function(item) {
                            var obj;
                            if (item.name) {
                                obj = { value: item.entityId, text: item.name, object: item };
                            } else {
                                obj = { value: item.entityId, text: item.fullTitle, object: item };
                            }

                            temp.push(obj);
                        });
                        fillStaticAutocomplete(temp);
                        staticItem = { createdTime: new Date().getTime(), data: temp };
                        Storage.set(config.staticKey, staticItem);
                        if (config.callback) {
                            config.callback.apply(null, [{ staticKey: config.staticKey, data: temp }]);
                        }
                    }
                };
                Api.gridView(
                    {
                        url: config.url ? config.url : "comboval/autocompleteView/parent",
                        filter: staticFilter.get(),
                        handler: staticHandler
                    }
                );
            }
        } else {
            staticHandler.success = function(data) {
                if (data.done) {
                    // map result
                    var temp = [];
                    data.result.forEach(function(item) {
                        var obj;
                        if (item.name) {
                            obj = { value: item.entityId, text: item.name, object: item };
                        } else {
                            obj = { value: item.entityId, text: item.fullTitle, object: item };
                        }

                        temp.push(obj);
                    });
                    fillStaticAutocomplete(temp);
                    staticItem = { createdTime: new Date().getTime(), data: temp };
                    Storage.set(config.staticKey, staticItem);
                    if (config.callback) {
                        config.callback.apply(null, [{ staticKey: config.staticKey, data: temp }]);
                    }
                }
            };
            Api.gridView(
                {
                    url: config.url ? config.url : "comboval/autocompleteView/parent",
                    filter: staticFilter.get(),
                    handler: staticHandler
                }
            );
        }

    };
    var fillStaticAutocomplete = function(data) {
        config.element.empty();
        if (data) {
            if (config.placeholder) {
                const temp = $('<option class="placeholder"></option>');
                config.element.append(temp);
            }
            data.forEach(function(item) {
                const option = $("<option></option>");
                option.attr("value", item.value);
                option.text(item.text);
                option.data("object", item.object);
                config.element.append(option);
            });
        }
        const placeholder = config.placeholder.indexOf("$") === 0
            ? GeneralBundle[config.placeholder]
            : Bundle[config.placeholder];
        config.element.select2({
                placeholder: placeholder,
                allowClear: true,
                dropdownCssClass: config.dropdownCssClass ? config.dropdownCssClass : null,
                containerCssClass: config.containerCssClass ? config.containerCssClass : null,
                language: {
                    noResults: function() {
                        return GeneralBundle["$autocompleteNoData"];
                    }
                },
                escapeMarkup: function(markup) {
                    return markup;
                }
            })
            .on("select2:select",
                function(e) {
                    const optionElement = $(e.params.data.element);
                    const itemData = $(e.params.data)[0];
                    const object = optionElement.data("object");
                    // multipleSelect
                    if (config.multipleSelect) {
                        const wrapper = $(e.target).closest(".form-group").find(".multiple-select-wrapper");
                        const ctn = wrapper.find(".multiple-select-container");
                        var isExist = false;
                        ctn.find(".badge").each(function() {
                            const ctx = $(this);
                            if (ctx.data().id === itemData.id) {
                                isExist = true;
                            }
                        });
                        if (config.multipleSelect.count) {

                            if (ctn.find(".badge").length < config.multipleSelect.count) {
                                if (!isExist) {
                                    var item = $("<span/>",
                                        {
                                            class: "badge badge-light",
                                            html: $("<span/>",
                                                {
                                                    class: "title",
                                                    text: itemData.text
                                                }),
                                            data: {
                                                id: itemData.id
                                            }
                                        });
                                    if (!config.multipleSelect.singleLine) {
                                        item.addClass("full-width");
                                    }
                                    var removeIcon = $("<span/>",
                                        {
                                            class: "remove-icon",
                                            html: '<i class="fas fa-times"></i>',
                                            click: function(e) {
                                                item.remove();
                                            }
                                        });
                                    item.append(removeIcon);
                                    ctn.prepend(item);
                                }
                            }
                        } else {
                            if (!isExist.length) {
                                var item = $("<span/>",
                                    {
                                        class: "badge badge-light",
                                        html: $("<span/>",
                                            {
                                                class: "title",
                                                text: itemData.text
                                            }),
                                        data: {
                                            id: itemData.id
                                        }
                                    });
                                var removeIcon = $("<span/>",
                                    {
                                        class: "remove-icon",
                                        html: '<i class="fas fa-times"></i>',
                                        click: function(e) {
                                            item.remove();
                                        }
                                    });
                                item.append(removeIcon);
                                ctn.prepend(item);
                            }
                        }
                    }
                    if (config.onSelect) {
                        config.onSelect.apply(null,
                            [
                                {
                                    object: object,
                                    value: optionElement.attr("value"),
                                    title: optionElement.text()
                                }, e
                            ]);
                    }
                })
            .on("change.select2",
                function(e) {
                    const target = $(e.currentTarget);
                    const selectedOption = target.find(`option[value="${target.val()}"]`);
                    //console.log(selectedOption.text(), )
                    const object = selectedOption.data("object");
                    if (config.multipleSelect && target.data() && target.data().multi) {
                        const wrapper = $(e.target).closest(".form-group");
                        var ctn = wrapper.find(".multiple-select-container");
                        const lstData = target.data().multi;
                        $.each(lstData,
                            (i, itemData) => {
                                var isExist = false;
                                ctn.find(".badge").each(function() {
                                    const ctx = $(this);
                                    if (ctx.data().id === itemData.entityId) {
                                        isExist = true;
                                    }
                                });
                                if (config.multipleSelect.count) {
                                    if (ctn.find(".badge").length < config.multipleSelect.count) {
                                        if (!isExist) {
                                            var item = $("<span/>",
                                                {
                                                    class: "badge badge-light",
                                                    html: $("<span/>",
                                                        {
                                                            class: "title",
                                                            text: itemData.name || itemData.fullTitle
                                                        }),
                                                    data: {
                                                        id: itemData.entityId
                                                    }
                                                });
                                            if (!config.multipleSelect.singleLine) {
                                                item.addClass("full-width");
                                            }
                                            var removeIcon = $("<span/>",
                                                {
                                                    class: "remove-icon",
                                                    html: '<i class="fas fa-times"></i>',
                                                    click: function(e) {
                                                        item.remove();
                                                    }
                                                });
                                            item.append(removeIcon);
                                            ctn.prepend(item);
                                        }
                                    }
                                } else {
                                    if (!isExist.length) {
                                        var item = $("<span/>",
                                            {
                                                class: "badge badge-light",
                                                html: $("<span/>",
                                                    {
                                                        class: "title",
                                                        text: itemData.text
                                                    }),
                                                data: {
                                                    id: itemData.id
                                                }
                                            });
                                        if (!config.multipleSelect.singleLine) {
                                            item.addClass("full-width");
                                        }
                                        var removeIcon = $("<span/>",
                                            {
                                                class: "remove-icon",
                                                html: '<i class="fas fa-times"></i>',
                                                click: function(e) {
                                                    item.remove();
                                                }
                                            });
                                        item.append(removeIcon);
                                        ctn.prepend(item);
                                    }
                                }
                            });
                    } else {
                        if (config.onChange) {
                            config.onChange.apply(null,
                                [
                                    {
                                        object: object,
                                        value: selectedOption.attr("value"),
                                        title: selectedOption.text()
                                    }, e
                                ]);
                        }
                    }

                })
            .on("select2:unselecting",
                (event) => {
                    if (config.onClear) {
                        config.onClear.apply(null, [event]);
                    }
                });
        if (config.callback) {
            config.callback.apply(null, [{ staticKey: config.staticKey, data: data }]);
        }
    };
    var initDynamicAutocomplete = function() {
        var dynamicWhere = new Wheres();
        if (config.where) {
            dynamicWhere = config.where;
        }
        var dynamicFilter = new Filter(dynamicWhere.getList(),
            10,
            1,
            (config.orderBy ? config.orderBy.get() : null),
            1,
            "");
        config.element.data("select2Config", config);
        const placeholder = config.placeholder.indexOf("$") === 0
            ? GeneralBundle[config.placeholder]
            : Bundle[config.placeholder];
        config.element.select2({
                placeholder: placeholder,
                allowClear: true,
                dropdownCssClass: config.dropdownCssClass ? config.dropdownCssClass : null,
                containerCssClass: config.containerCssClass ? config.containerCssClass : null,
                language: {
                    noResults: function() {
                        if (config.onNoResult) {
                            config.onNoResult.apply(null, []);
                        }
                        return GeneralBundle["$autocompleteNoData"];
                    },
                    inputTooShort: function(args) {
                        return GeneralBundle["$autocompleteEnterPartOne"] +
                            args.minimum +
                            GeneralBundle["$autocompleteEnterPartTwo"];
                    },
                    searching: function(args) {
                        return GeneralBundle["$autocompleteIsSearching"];
                    }
                },
                escapeMarkup: function(markup) {
                    return markup;
                },
                minimumInputLength: config.minCharacter,
                ajax: {
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: Config.SERVICE_URL + config.url,
                    dataType: "json",
                    data: function(term, page) {
                        dynamicWhere.remove(config.searchColumn);
                        if (term.term) {
                            dynamicWhere.add(config.searchColumn, term.term, ENVIRONMENT.Condition.CONTAINS);
                        } else {
                            dynamicWhere.add(config.searchColumn, "$#$", ENVIRONMENT.Condition.NOT_EQUAL);
                        }
                        dynamicFilter = new Filter(dynamicWhere.getList(),
                            20,
                            1,
                            (config.orderBy ? config.orderBy.get() : null),
                            1,
                            "");
                        dynamicFilter = dynamicFilter.get();
                        dynamicFilter.ticket = Storage.getUserInfo().ticket;
                        return JSON.stringify(dynamicFilter);
                    },
                    processResults: function(data) {
                        var items = [];
                        if (data.done && data.result) {
                            config.element.empty();
                            data.result.forEach(function(item) {
                                items.push({ id: item[config.value], text: item[config.text], object: item });
                            });
                        } else {
                            if (data.errorDesc) {
                                if (data.errorDesc !== "NO_DATA") {
                                    errorHandler(data);
                                }
                            } else {
                                errorHandler(data);
                            }
                        }
                        return {
                            results: items
                            /*pagination: { "more": true}*/
                        };
                    }
                }
            })
            .on("select2:select",
                function(e) {
                    var itemData = $(e.params.data)[0];
                    // multipleSelect
                    if (config.multipleSelect) {
                        const wrapper = $(e.target).closest(".form-group").find(".multiple-select-wrapper");
                        const ctn = wrapper.find(".multiple-select-container");
                        var isExist = false;
                        ctn.find(".badge").each(function() {
                            const ctx = $(this);
                            if (ctx.data().id === itemData.id) {
                                isExist = true;
                            }
                        });
                        if (config.multipleSelect.count) {
                            if (ctn.find(".badge").length < config.multipleSelect.count) {
                                if (!isExist.length) {
                                    var item = $("<span/>",
                                        {
                                            class: "badge badge-light",
                                            html: $("<span/>",
                                                {
                                                    class: "title",
                                                    text: itemData.text
                                                }),
                                            data: {
                                                id: itemData.id
                                            }
                                        });
                                    if (!config.multipleSelect.singleLine) {
                                        item.addClass("full-width");
                                    }
                                    var removeIcon = $("<span/>",
                                        {
                                            class: "remove-icon",
                                            html: '<i class="fas fa-times"></i>',
                                            click: function(e) {
                                                item.remove();
                                            }
                                        });
                                    item.append(removeIcon);
                                    ctn.prepend(item);
                                }
                            }
                        } else {
                            if (!isExist.length) {
                                var item = $("<span/>",
                                    {
                                        class: "badge badge-light",
                                        html: $("<span/>",
                                            {
                                                class: "title",
                                                text: itemData.text
                                            }),
                                        data: {
                                            id: itemData.id
                                        }
                                    });
                                if (!config.multipleSelect.singleLine) {
                                    item.addClass("full-width");
                                }
                                var removeIcon = $("<span/>",
                                    {
                                        class: "remove-icon",
                                        html: '<i class="fas fa-times"></i>',
                                        click: function(e) {
                                            item.remove();
                                        }
                                    });
                                item.append(removeIcon);
                                ctn.prepend(item);
                            }
                        }
                    }
                    if (config.onSelect) {
                        config.onSelect.apply(null,
                            [
                                {
                                    object: itemData.object,
                                    value: itemData.id,
                                    title: itemData.text
                                }, e
                            ]);
                    }
                })
            .on("change.select2",
                function(e) {
                    const target = $(e.currentTarget);
                    const selectedOption = target.find(`option[value="${target.val()}"]`);
                    if (selectedOption.data()) {
                        const object = selectedOption.data("object");
                        /*// multipleSelect
                        if (config.multipleSelect) {
                            var itemData = {
                                text: selectedOption.text(),
                                id: selectedOption.val()
                            };
                            var wrapper = $(e.target).closest('.form-group').next('.multiple-select-wrapper');
                            var ctn = wrapper.find('.multiple-select-container');
                            var isExist = false;
                            ctn.find('.badge').each(function () {
                                var ctx = $(this);
                                if(ctx.data().id === itemData.id) {
                                    isExist = true;
                                }
                            });
                            if (config.multipleSelect.count) {
                                if (ctn.find('.badge').length < config.multipleSelect.count) {
                                    if (!isExist.length) {
                                        var item = $('<span/>', {
                                            class: 'badge badge-light',
                                            html: $('<span/>', {
                                                class: 'title',
                                                text: itemData.text
                                            }),
                                            data: {
                                                id: itemData.id
                                            }
                                        });
                                        var removeIcon = $('<span/>', {
                                            class: 'remove-icon',
                                            html: '<i class="fas fa-times"></i>',
                                            click: function (e) {
                                                item.remove();
                                            }
                                        });
                                        item.append(removeIcon);
                                        ctn.prepend(item);
                                    }
                                }
                            } else {
                                if (!isExist.length) {
                                    var item = $('<span/>', {
                                        class: 'badge badge-light',
                                        html: $('<span/>', {
                                            class: 'title',
                                            text: itemData.text
                                        }),
                                        data: {
                                            id: itemData.id
                                        }
                                    });
                                    var removeIcon = $('<span/>', {
                                        class: 'remove-icon',
                                        html: '<i class="fas fa-times"></i>',
                                        click: function (e) {
                                            item.remove();
                                        }
                                    });
                                    item.append(removeIcon);
                                    ctn.prepend(item);
                                }
                            }
                        }*/
                        if (config.onChange) {
                            config.onChange.apply(null,
                                [
                                    {
                                        object: object,
                                        value: selectedOption.attr("value"),
                                        title: selectedOption.text()
                                    }, e
                                ]);
                        }
                    }
                })
            .on("select2:unselecting",
                (event) => {
                    if (config.onClear) {
                        config.onClear.apply(null, [event]);
                    }
                });
    };

    const initAutocompleteCustomData = function(conf) {
        // placeholder
        const temp = $('<option class="placeholder"></option>');
        conf.element.append(temp);
        const placeholder = conf.placeholder.indexOf("$") === 0
            ? GeneralBundle[conf.placeholder]
            : Bundle[conf.placeholder];
        if (conf.dynamicConfig) {
            conf.element.data("select2Config", conf.dynamicConfig);
        }
        conf.data.forEach(function(item) {
            const option = $("<option></option>");
            option.attr("value", item.value);
            option.text(item.text);
            option.data("object", item.object);
            conf.element.append(option);
        });
        conf.element.select2({
            placeholder: placeholder,
            allowClear: true,
            dropdownCssClass: conf.dropdownCssClass ? conf.dropdownCssClass : null,
            containerCssClass: conf.containerCssClass ? conf.containerCssClass : null,
            language: {
                noResults: function() {
                    return GeneralBundle["$autocompleteNoData"];
                }
            },
            escapeMarkup: function(markup) {
                return markup;
            }
        }).on("select2:select",
            function(e) {

                const optionElement = $(e.params.data.element);
                const object = optionElement.data("object");
                if (conf.onSelect) {
                    conf.onSelect.apply(null,
                        [
                            {
                                object: object,
                                value: optionElement.attr("value"),
                                title: optionElement.text()
                            }, e
                        ]);
                }
            }).on("change.select2",
            function(e) {

                const target = $(e.currentTarget);
                const selectedOption = target.find(`option[value="${target.val()}"]`);
                if (selectedOption.data()) {
                    const object = selectedOption.data("object");
                    if (conf.onChange) {
                        conf.onChange.apply(null,
                            [
                                {
                                    object: object,
                                    value: selectedOption.attr("value"),
                                    title: selectedOption.text()
                                }, e
                            ]);
                    }
                }
            }).on("select2:unselecting",
            (event) => {

                if (conf.onClear) {
                    conf.onClear.apply(null, [event]);
                }
            });
    };

    return {
        init: init,
        initAutocompleteCustomData: initAutocompleteCustomData
    };

};