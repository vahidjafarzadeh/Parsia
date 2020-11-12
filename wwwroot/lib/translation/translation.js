/**
 * Created by Farshad Kazemi on 2/28/2018.
 */

var Translation = (function() {
    var _container;

    const translate = function(container) {
        _container = container ? container : $("html");
        _container.find("[ksun-bundle-key]").each(function() {
            var ctx = $(this);
            var bundleKey = ctx.attr("ksun-bundle-key");
            if (bundleKey.indexOf("$") === 0) { // general bundle
                try {
                    for (var key in GeneralBundle) {
                        if (ctx.attr("data-toggle") === "tooltip") {
                            ctx.attr("title", GeneralBundle[bundleKey] ? GeneralBundle[bundleKey] : bundleKey);
                        } else {
                            switch (this.tagName) {
                            case "LEGEND":
                            case "SPAN":
                            case "DIV":
                            case "OPTION":
                            case "H1":
                            case "H2":
                            case "H3":
                            case "H4":
                            case "H5":
                            case "H6":
                            case "P":
                            case "A":
                            case "LABEL":
                                this.innerHTML = GeneralBundle[bundleKey] ? GeneralBundle[bundleKey] : bundleKey;
                                break;
                            case "BUTTON":
                                if (ctx.find(".title").length > 0) {
                                    ctx.find(".title")
                                        .text(GeneralBundle[bundleKey] ? GeneralBundle[bundleKey] : bundleKey);
                                } else {
                                    this.innerHTML = GeneralBundle[bundleKey] ? GeneralBundle[bundleKey] : bundleKey;
                                }
                                break;
                            case "INPUT":
                                if (!this.getAttribute("placeholder")) {
                                    this.setAttribute("placeholder",
                                        GeneralBundle[bundleKey] ? GeneralBundle[bundleKey] : bundleKey);
                                }
                                break;
                            case "TD":
                                this.setAttribute("data-label",
                                    GeneralBundle[bundleKey] ? GeneralBundle[bundleKey] : bundleKey);
                                break;
                            }
                        }
                    }
                } catch (e) {

                }
            } else
            {
                try {
                    for (var key in Bundle) {
                        if (ctx.attr("data-toggle") === "tooltip") {
                            ctx.attr("title", Bundle[bundleKey] ? Bundle[bundleKey] : bundleKey);
                        } else {
                            switch (this.tagName) {
                            case "LEGEND":
                            case "SPAN":
                            case "DIV":
                            case "OPTION":
                            case "H1":
                            case "H2":
                            case "H3":
                            case "H4":
                            case "H5":
                            case "H6":
                            case "P":
                            case "A":
                            case "LABEL":
                                this.innerHTML = Bundle[bundleKey] ? Bundle[bundleKey] : bundleKey;
                                break;
                            case "BUTTON":
                                if (ctx.find(".title").length > 0) {
                                    ctx.find(".title").text(Bundle[bundleKey] ? Bundle[bundleKey] : bundleKey);
                                } else {
                                    this.innerHTML = Bundle[bundleKey] ? Bundle[bundleKey] : bundleKey;
                                }
                                break;
                            case "INPUT":
                                if (!this.getAttribute("placeholder")) {
                                    this.setAttribute("placeholder", Bundle[bundleKey] ? Bundle[bundleKey] : bundleKey);
                                }
                                break;
                            case "TD":
                                this.setAttribute("data-label", Bundle[bundleKey] ? Bundle[bundleKey] : bundleKey);
                                break;
                            }
                        }
                    }
                } catch (e) {

                }
            }
        });
    };

    return {
        translate: translate
    };

})();