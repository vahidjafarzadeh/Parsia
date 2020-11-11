/**
 * Created by MFS on 3/02/2019.
 */

var AutocompleteStatic = function() {

    var config = {
        element: undefined,
        placeholder: "Enter Value",
        url: "",
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        data: [{ id: 1, title: "", object: {} }],
        handler: new Handler(),
        onSelect: null
    };
    const init = function(configObject) {
        if (configObject) {
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
            if (configObject.onSelect) {
                config.onSelect = configObject.onSelect;
            }
            if (configObject.staticKey) {
                config.staticKey = btoa(encodeURIComponent(configObject.staticKey).replace(/%([0-9A-F]{2})/g,
                    function toSolidBytes(match, p1) {
                        return String.fromCharCode(`0x${p1}`);
                    }));
            }

            if (configObject.dynamicConfig) {
                config.minCharacter =
                    configObject.dynamicConfig.minCharacter ? configObject.dynamicConfig.minCharacter : 1;
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
                    config.text = configObject.text ? configObject.text : "title";
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
        const staticFilter = new Filter(1, 100, staticWhere.getList());
        var staticItem = {};

        staticItem = Storage.get(config.staticKey);
        if (staticItem) { // agar baraye key morde-e nazar data vojood dashte bashad
            if (Util.differentTime(new Date().getTime(), staticItem.createdTime, Config.AUTOCOMPLETE_CACHE_TIMEOUT)
            ) { // agar bazeye zamani b payan nareside bashad, data az storage khande shavad
                fillStaticAutocomplete(staticItem.data);
            } else {
                staticHandler.success = function(data) {
                    if (data.done) {
                        // map result
                        var temp = [];
                        data.result.forEach(function(item) {
                            const obj = { value: item.entityId, text: item.name, object: item };
                            temp.push(obj);
                        });
                        fillStaticAutocomplete(temp);
                        staticItem = { createdTime: new Date().getTime(), data: temp };
                        Storage.set(config.staticKey, staticItem);
                    }
                };
                Api.gridView({
                    url: "EntityCustom/autocompleteView",
                    filter: staticFilter.get(),
                    handler: staticHandler
                });
            }
        } else {
            staticHandler.success = function(data) {
                if (data.done) {
                    // map result
                    var temp = [];
                    data.result.forEach(function(item) {
                        const obj = { value: item.entityId, text: item.title, object: item };
                        temp.push(obj);
                    });
                    fillStaticAutocomplete(temp);
                    staticItem = { createdTime: new Date().getTime(), data: temp };
                    Storage.set(config.staticKey, staticItem);
                }
            };
            Api.gridView({ url: "EntityCustom/autocompleteView", filter: staticFilter.get(), handler: staticHandler });
        }

    };
    var fillStaticAutocomplete = function(data) {
        config.element.empty();
        if (data) {
            data.forEach(function(item) {
                const option = $("<option></option>");
                option.attr("value", item.value);
                option.text(item.text);
                option.data("object", item.object);
                config.element.append(option);
            });
        }
        config.element.select2({
            placeholder: config.placeholder ? Bundle[config.placeholder] : null,
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
        }).on("select2:select",
            function(e) {
                const optionElement = $(e.params.data.element);
                const object = optionElement.data("object");
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
            });
    };

    return {
        init: init
    };

};