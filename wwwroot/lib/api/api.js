/**
 * Created by Farshad Kazemi on 2/14/2018.
 */

var Api = (function() {

    /**
     * @param requestConfig: object => url, filter, handler, isAsync, withoutTicket
     */
    const gridView = function(requestConfig) {
        /*requestConfig.filter.language = 'FA';
        requestConfig.filter.ticket = 'tadmin|61506607758403';*/
        requestConfig.filter.ticket = Storage.getUserInfo().ticket;
        requestConfig.filter.language = Storage.getUserInfo().lang;
        try {
            const conf = {
                method: "POST",
                dataType: "json",
                contentType: "application/json",
                data: JSON.stringify(requestConfig.filter),
                url: Config.SERVICE_URL + requestConfig.url,
                cache: false,
                headers: {
                    "X-Authentication-Ticket": Storage.getUserInfo().ticket
                },
                beforeSend: function() {
                    requestConfig.handler.beforeSend();
                },
                success: function(data, textStatus, jqXHR) {
                    if (data.errorCode === 2) {
                        Storage.removeUserInfo();
                        localStorage.removeItem("user");
                        showLoginPage(top.location.href);
                        return;
                    }
                    requestConfig.handler.success(data, textStatus, jqXHR);
                },
                complete: function(jqXHR) {
                    requestConfig.handler.complete(jqXHR);
                },
                error: function(jqXHR) {
                    requestConfig.handler.error(jqXHR);
                }
            };
            if (requestConfig.isAsync !== undefined && requestConfig.isAsync === false) {
                conf.async = false;
            }
            $.ajax(conf);
        } catch (e) {
        }
    };

    const deleteRow = function(requestConfig) {
        requestConfig.filter.ticket = Storage.getUserInfo().ticket;
        requestConfig.filter.language = Storage.getUserInfo().lang;
        try {
            const conf = {
                method: "POST",
                dataType: "json",
                contentType: "application/json",
                data: JSON.stringify(requestConfig.filter),
                url: Config.SERVICE_URL + requestConfig.url,
                cache: false,
                headers: {
                    "X-Authentication-Ticket": Storage.getUserInfo().ticket
                },
                beforeSend: function() {
                    requestConfig.handler.beforeSend();
                },
                success: function(data, textStatus, jqXHR) {
                    requestConfig.handler.success(data, textStatus, jqXHR);
                },
                complete: function(jqXHR) {
                    requestConfig.handler.complete(jqXHR);
                },
                error: function(jqXHR) {
                    requestConfig.handler.error(jqXHR);
                }
            };
            if (requestConfig.isAsync !== undefined && requestConfig.isAsync === false) {
                conf.async = false;
            }
            $.ajax(conf);
        } catch (e) {
        }
    };

    const showRow = function(requestConfig) {
        requestConfig.filter.ticket = Storage.getUserInfo().ticket;
        requestConfig.filter.language = Storage.getUserInfo().lang;
        try {
            const conf = {
                method: "POST",
                dataType: "json",
                contentType: "application/json",
                data: JSON.stringify(requestConfig.filter),
                url: Config.SERVICE_URL + requestConfig.url,
                cache: false,
                headers: {
                    "X-Authentication-Ticket": Storage.getUserInfo().ticket
                },
                beforeSend: function() {
                    requestConfig.handler.beforeSend();
                },
                success: function(data, textStatus, jqXHR) {
                    requestConfig.handler.success(data, textStatus, jqXHR);
                },
                complete: function(jqXHR) {
                    requestConfig.handler.complete(jqXHR);
                },
                error: function(jqXHR) {
                    requestConfig.handler.error(jqXHR);
                }
            };
            if (requestConfig.isAsync !== undefined && requestConfig.isAsync === false) {
                conf.async = false;
            }
            $.ajax(conf);
        } catch (e) {
        }
    };

    const save = function(requestConfig) {
        post(requestConfig);
    };

    var post = function(requestConfig) {
        if (!requestConfig.withoutTicket && Storage.getUserInfo() !== null && Storage.getUserInfo().ticket !== null) {
            requestConfig.data.ticket = Storage.getUserInfo().ticket;
        }
        try {
            const conf = {
                method: "POST",
                dataType: "json",
                contentType: "application/json",
                data: JSON.stringify(requestConfig.data),
                url: Config.SERVICE_URL + requestConfig.url,
                cache: false,
                beforeSend: function() {
                    requestConfig.handler.beforeSend();
                },
                success: function(data, textStatus, jqXHR) {
                    requestConfig.handler.success(data, textStatus, jqXHR);
                },
                complete: function(jqXHR) {
                    requestConfig.handler.complete(jqXHR);
                },
                error: function(jqXHR) {
                    requestConfig.handler.error(jqXHR);
                }
            };
            if (!requestConfig.withoutTicket &&
                Storage.getUserInfo() !== null &&
                Storage.getUserInfo().ticket !== null) {
                conf.headers = { "X-Authentication-Ticket": Storage.getUserInfo().ticket };
            }
            if (requestConfig.isAsync !== undefined && requestConfig.isAsync === false) {
                conf.async = false;
            }
            $.ajax(conf);
        } catch (e) {
            debugger;
        }
    };

    /**
     * @param requestConfig: Object -> { url, queryParams, handler, isAsync, isDirectory }
     */
    const get = function(requestConfig) {
        try {
            const urlPrefix = requestConfig.isDirectory ? Config.APPLICATION_URL : Config.SERVICE_URL;
            const conf = {
                method: "GET",
                dataType: "json",
                contentType: "application/json",
                url: requestConfig.queryParams
                    ? urlPrefix + requestConfig.url + "?" + requestConfig.queryParams
                    : urlPrefix + requestConfig.url,
                cache: false,
                beforeSend: function() {
                    requestConfig.handler.beforeSend();
                },
                success: function(data, textStatus, jqXHR) {
                    requestConfig.handler.success(data, textStatus, jqXHR);
                },
                complete: function(jqXHR) {
                    requestConfig.handler.complete(jqXHR);
                },
                error: function(jqXHR) {
                    requestConfig.handler.error(jqXHR);
                }
            };
            if (requestConfig.isAsync !== undefined && requestConfig.isAsync === false) {
                conf.async = false;
            }
            $.ajax(conf);
        } catch (e) {
            console.log(e);
        }
    };

    /**
     * @param requestConfig: object -> {url, onProgressFunction, onSuccessFunction, onErrorFunction, file, entityId, description}
     */
    const upload = function(requestConfig) {
        const formData = new FormData();
        if (requestConfig.file) {
            formData.append("file", requestConfig.file);
            formData.append("entityId", requestConfig.entityId ? requestConfig.entityId : "");
            formData.append(`file-${requestConfig.file.name}-size`, requestConfig.file.size);
            formData.append("upload_file", true);
            formData.append("description", requestConfig.description);
        }
        if (requestConfig.folderName) {
            formData.append("folderName", requestConfig.folderName);
        }
        formData.append("parentID", requestConfig.parentID);
        formData.append("ticket", Storage.getUserInfo().ticket);

        $.ajax({
            type: "POST",
            url: Config.SERVICE_URL + requestConfig.url,
            headers: {
                "X-Authentication-Ticket": Storage.getUserInfo().ticket
            },
            xhr: function() {
                const myXhr = $.ajaxSettings.xhr();
                if (myXhr.upload) {
                    myXhr.upload.addEventListener("progress",
                        function(evt) {
                            if (evt.lengthComputable) {
                                const percentComplete = Math.ceil((evt.loaded / evt.total) * 100);
                                if (requestConfig.onProgress) {
                                    requestConfig.onProgress.apply(null, [evt, percentComplete]);
                                }
                            }
                        },
                        false);
                }
                return myXhr;
            },
            success: function(data) {
                // your callback here
                if (requestConfig.onSuccess) {
                    requestConfig.onSuccess.apply(null, [data, requestConfig]);
                }
            },
            error: function(error) {
                // handle error
                if (requestConfig.onError) {
                    requestConfig.onError.apply(null, [error]);
                }
            },
            async: true,
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            timeout: 60000
        });
    };

    /**
     * @param requestConfig: object -> {url, formData, isAsync, handler, withoutTicket}
     */
    const postForm = function(requestConfig) {
        const formData = requestConfig.formData;
        if (!requestConfig.withoutTicket) {
            formData.append("ticket", Storage.getUserInfo() ? Storage.getUserInfo().ticket : null);
        }
        try {
            const conf = {
                method: "POST",
                contentType: false,
                processData: false,
                data: formData,
                url: Config.SERVICE_URL + requestConfig.url,
                cache: false,
                beforeSend: function() {
                    requestConfig.handler.beforeSend();
                },
                success: function(data, textStatus, jqXHR) {
                    requestConfig.handler.success(data, textStatus, jqXHR);
                },
                complete: function(jqXHR) {
                    requestConfig.handler.complete(jqXHR);
                },
                error: function(jqXHR) {
                    requestConfig.handler.error(jqXHR);
                }
            };
            if (!requestConfig.withoutTicket) {
                conf.headers = {
                    "X-Authentication-Ticket": Storage.getUserInfo() ? Storage.getUserInfo().ticket : null
                };
            }
            if (requestConfig.isAsync !== undefined && requestConfig.isAsync === false) {
                conf.async = false;
            }
            $.ajax(conf);
        } catch (e) {
            console.log(e);
        }
    };

    return {
        gridView: gridView,
        delete: deleteRow,
        showRow: showRow,
        save: save,
        post: post,
        get: get,
        upload: upload,
        postForm: postForm
    };

})();