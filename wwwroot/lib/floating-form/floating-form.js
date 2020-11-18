var FloatingForm = (function() {
    "use strict";
    const init = function(config) {
        var elements;
        if (config && config.element) {
            const element = config.element instanceof jQuery ? config.element[0] : config.element;
            elements = [element];
        } else {
            elements = [].slice.call(document.querySelectorAll(".floating-form-control"));
        }
        elements.forEach(function(element) {
            if (element.value) {
                element.classList.add("floating-mode");
            }
            element.addEventListener("focus",
                function(e) {
                    element.classList.add("floating-mode");
                });
            element.addEventListener("blur",
                function(e) {
                    if (element.classList.contains("date")) {
                        setTimeout(function() {
                                if (!element.value) {
                                    element.classList.remove("floating-mode");
                                }
                            },
                            300);
                    } else {
                        if (!element.value) {
                            element.classList.remove("floating-mode");
                        }
                    }
                });
            if (config && config.counter) {
                const parentElem = element.parentNode;
                var counters = [].slice.call(parentElem.querySelectorAll(".character-counter"));
                element.addEventListener("keyup",
                    function(e) {
                        counters.forEach(function(c) {
                            const maxCount = c.getAttribute("ksun-max-count");
                            c.innerHTML = element.value.length + "/" + maxCount;
                        });
                    });
            }
        });
    };

    return { init: init };
})();