/**
 * Created by F.Kazemi on 12/25/2018.
 */

var Modal = (function() {
    "use strict";
    var confirmModalConfig = {
        modalWrapper: $("body"),
        element: null,
        classList: ["modal", "fade"],
        modalDialog: {
            element: null,
            classList: ["modal-dialog", "modal-dialog-centered", "modal-dialog-top-md-down"]
        },
        modalContent: {
            element: null,
            classList: ["modal-content"]
        },
        modalHeader: {
            element: null,
            classList: ["modal-header"],
            modalTitle: {
                element: null,
                title: "پیام سیستمی",
                classList: ["modal-title"]
            },
            closeButton: {
                element: null,
                icon: '<i class="fas fa-times"></i>',
                classList: ["close"]
            }
        },
        modalBody: {
            element: null,
            classList: ["modal-body"]
        },
        modalFooter: {
            element: null,
            classList: ["modal-footer"],
            buttonWrapper: {
                element: null,
                classList: ["button-wrapper"],
                declineButton: {
                    element: null,
                    classList: ["btn", "btn-danger", "btn-sm"]
                },
                confirmButton: {
                    element: null,
                    classList: ["btn", "btn-success", "btn-sm"]
                }
            }
        }

    };
    const init = function(config) {

        if (config) {
            if (config.modalWrapper) {
                confirmModalConfig.modalWrapper =
                    (config.modalWrapper instanceof jQuery ? config.modalWrapper : $(config.modalWrapper));
            }
        }

        confirmModalConfig.element = $("<div/>",
            {
                class: confirmModalConfig.classList.join(" "),
                id: "confirm-modal",
                tabindex: -1,
                role: "dialog"
            });

        confirmModalConfig.modalDialog.element = $("<div/>",
            {
                class: confirmModalConfig.modalDialog.classList.join(" "),
                role: "document"
            });

        confirmModalConfig.modalContent.element = $("<div/>",
            {
                class: confirmModalConfig.modalContent.classList.join(" ")
            });

        confirmModalConfig.modalHeader.element = $("<div/>",
            {
                class: confirmModalConfig.modalHeader.classList.join(" ")
            });
        confirmModalConfig.modalHeader.modalTitle.element = $("<h5/>",
            {
                class: confirmModalConfig.modalHeader.modalTitle.classList.join(" ")
            });
        confirmModalConfig.modalHeader.closeButton.element = $("<button/>",
            {
                class: confirmModalConfig.modalHeader.closeButton.classList.join(" "),
                "data-dismiss": "modal",
                type: "button",
                html: confirmModalConfig.modalHeader.closeButton.icon
            });
        confirmModalConfig.modalHeader.element
            .append(confirmModalConfig.modalHeader.modalTitle.element,
                confirmModalConfig.modalHeader.closeButton.element);

        confirmModalConfig.modalBody.element = $("<div/>",
            {
                class: confirmModalConfig.modalBody.classList.join(" ")
            });

        confirmModalConfig.modalFooter.element = $("<div/>",
            {
                class: confirmModalConfig.modalFooter.classList.join(" ")
            });
        confirmModalConfig.modalFooter.buttonWrapper.element = $("<div/>",
            {
                class: confirmModalConfig.modalFooter.buttonWrapper.classList.join(" ")
            });
        confirmModalConfig.modalFooter.buttonWrapper.declineButton.element = $("<button/>",
            {
                class: confirmModalConfig.modalFooter.buttonWrapper.declineButton.classList.join(" "),
                id: "decline-btn"
            });
        confirmModalConfig.modalFooter.buttonWrapper.confirmButton.element = $("<button/>",
            {
                class: confirmModalConfig.modalFooter.buttonWrapper.confirmButton.classList.join(" "),
                id: "confirm-btn"
            });
        confirmModalConfig.modalFooter.buttonWrapper.element
            .append(confirmModalConfig.modalFooter.buttonWrapper.declineButton.element,
                confirmModalConfig.modalFooter.buttonWrapper.confirmButton.element);
        confirmModalConfig.modalFooter.element.append(confirmModalConfig.modalFooter.buttonWrapper.element);

        confirmModalConfig.modalContent.element
            .append(confirmModalConfig.modalHeader.element,
                confirmModalConfig.modalBody.element,
                confirmModalConfig.modalFooter.element);

        confirmModalConfig.modalDialog.element
            .append(confirmModalConfig.modalContent.element);

        confirmModalConfig.element.append(confirmModalConfig.modalDialog.element);

        confirmModalConfig.modalWrapper.append(confirmModalConfig.element);

        return {
            showConfirm: showConfirm,
            hideConfirm: hideConfirm
        };
    };

    /**
     * @param config: object { title, body, confirmButton: { text, onClick, hidden }, declineButton: { text, onClick, hidden }, script: function }
     */
    var showConfirm = function(config) {
        confirmModalConfig.modalFooter.buttonWrapper.confirmButton.element.off("click");
        confirmModalConfig.modalFooter.buttonWrapper.declineButton.element.off("click");
        if (config) {
            if (config.wrapperClassList && Array.isArray(config.wrapperClassList)) {
                config.wrapperClassList.forEach(function(cl) {
                    confirmModalConfig.element.addClass(cl);
                });
            } else {
                confirmModalConfig.element.removeClass();
                confirmModalConfig.classList.forEach(function(cl) {
                    confirmModalConfig.element.addClass(cl);
                });
            }
            confirmModalConfig.modalHeader.modalTitle.element.html(config.title ? config.title : "پیام سیستمی");
            confirmModalConfig.modalBody.element.empty();
            confirmModalConfig.modalBody.element.append(config.body ? config.body : "آیا مطمئن هستید؟");
            if (config.confirmButton) {
                confirmModalConfig.modalFooter.buttonWrapper.confirmButton.element.text(config.confirmButton.text
                    ? config.confirmButton.text
                    : GeneralBundle.$confirm);
                confirmModalConfig.modalFooter.buttonWrapper.confirmButton.element.css({
                    display: config.confirmButton.hidden ? "none" : "inline-block"
                });
                if (config.confirmButton.onClick) {
                    confirmModalConfig.modalFooter.buttonWrapper.confirmButton.element.on("click",
                        function(e) {
                            if (config.confirmButton.onClick.apply(null, e) === 1
                            ) { // If you need to close modal, just return 1 at the end of onClick function
                                hideConfirm();
                            }
                        });
                } else {
                    confirmModalConfig.modalFooter.buttonWrapper.confirmButton.element.on("click",
                        function() {
                            hideConfirm();
                        });
                }
            } else {
                confirmModalConfig.modalFooter.buttonWrapper.confirmButton.element.text(GeneralBundle.$confirm)
                    .css({ display: "inline-block" }).on("click",
                        function() {
                            hideConfirm();
                        });
            }
            if (config.declineButton) {
                confirmModalConfig.modalFooter.buttonWrapper.declineButton.element.text(config.declineButton.text
                    ? config.declineButton.text
                    : GeneralBundle.$decline);
                confirmModalConfig.modalFooter.buttonWrapper.declineButton.element.css({
                    display: config.declineButton.hidden ? "none" : "inline-block"
                });
                if (config.declineButton.onClick) {
                    confirmModalConfig.modalFooter.buttonWrapper.declineButton.element.on("click",
                        function() {
                            if (config.declineButton.onClick.apply(null, [event]) === 1
                            ) { // If you need to close modal, just return 1 at the end of onClick function
                                hideConfirm();
                            }
                        });
                } else {
                    confirmModalConfig.modalFooter.buttonWrapper.declineButton.element.on("click",
                        function() {
                            hideConfirm();
                        });
                }
            } else {
                confirmModalConfig.modalFooter.buttonWrapper.declineButton.element.text(GeneralBundle.$decline)
                    .css({ display: "inline-block" }).on("click",
                        function() {
                            hideConfirm();
                        });
            }
            if (config.script) {
                config.script.apply(null, []);
            }
        }
        else {
            confirmModalConfig.modalHeader.modalTitle.element.html("پیام سیستمی");
            confirmModalConfig.modalBody.element.html("آیا مطمئن هستید؟");
            confirmModalConfig.modalFooter.buttonWrapper.confirmButton.element.text(GeneralBundle.$confirm)
                .css({ display: "inline-block" }).on("click",
                    function() {
                        hideConfirm();
                    });
            confirmModalConfig.modalFooter.buttonWrapper.declineButton.element.text(GeneralBundle.$decline)
                .css({ display: "inline-block" }).on("click",
                    function() {
                        hideConfirm();
                    });
        }
        confirmModalConfig.element.modal("show");
    };

    var hideConfirm = function() {
        confirmModalConfig.element.modal("hide");
    };
    return {
        init: init,
        showConfirm: showConfirm,
        hideConfirm: hideConfirm
    };
})();