const replaceBundle = () => {
    $.each($("[parsia-bundle-key]"),
        (i, v) => {
            $(v).text(Bundle[$(v).attr("parsia-bundle-key")]);
        });
};
let gridView = () => {};
const localPageReady = () => {
    gridView();
};
$(function() {
    $.ripple(".material",
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
    replaceBundle();


});