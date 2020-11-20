var protocol = window.location.protocol;
var host = window.location.host;
var _URL = protocol + "//" + host;
var PATH = window.location.pathname.toString().split("/");
var temp = PATH.splice(1, PATH.length - 2);
PATH = temp.join("/");
PATH += "/";
var AUTHENTICATION_SERVER = _URL + "/";

const Config = {
    FULL_URL: window.location.href,
    APPLICATION_URL: _URL + "/",
    SERVICE_URL: _URL + "/service/",
    WEBSOCKET_URL: (protocol === "https:" ? "wss" : "ws") + "://" + host + "/wsendpoint/?" + host,
    PATH: PATH,
    USER_TIMEOUT: 500 * 60 * 1000, // 500 minutes in millisecond(s)
    AUTOCOMPLETE_CACHE_TIMEOUT: 24 * 60 * 60 * 1000, // 1 Day
    INPUT_DEFAULT_MAX_LENGTH: 1000000,
    INPUT_DEFAULT_MIN_LENGTH: 1,
    INPUT_DEFAULT_FRACTIONAL_LENGTH: 2
};

const AppConfig = {
    USER_DATA_TIMEOUT: 15
};