var FileManager = (function () {
    let config = {
        countSelectFile: 0,
        maxSelectFileLength: 0,
        allowExtensionSelect: [],
        element: "",
        onConfirm: {}
    };
    var configure = function (configObject) {
        if (configObject.acceptFormats) {
            config.allowExtensionSelect = configObject.acceptFormats;
        }
        if (configObject.maxCount) {
            config.countSelectFile = configObject.maxCount;
        }
        if (configObject.maxSize) {
            config.maxSelectFileLength = configObject.maxSize;
        }
        if (configObject.elem) {
            config.element = configObject.elem;
        }
        if (configObject.onConfirm) {
            config.onConfirm = configObject.onConfirm;
        }
    };
    var showFileManager = function () {
        $("#iframe-file-manager").prop('contentWindow').gridView();
        $("#iframe-file-manager").prop('contentWindow').initialData(config);
        $("#file-manager").modal('show');
        $("#selected-files").off("click");
        $("#selected-files").on("click", () => {
            var lstData = $("#iframe-file-manager").prop('contentWindow').getAllFileSelected();
            config.onConfirm.apply(null, [lstData, config.element]);
            $("#file-manager").modal('hide');
        });
    };
    var init = function () {
        initViews();
        initListeners();
        initInstances();
        Util.setInputValidationListener(descriptionInput);
        Translation.translate(modal);
    };
    return {
        configure: configure,
        init: init,
        showFileManager: showFileManager
    }
})();