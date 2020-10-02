/**
 * Created by Farshad Kazemi on 2/14/2018.
 */

var Storage = (function() {

    var pageNeedLogin = true;

    const setPageNeedLogin = function(needLogin) {
        pageNeedLogin = needLogin;
    };

    var isPageNeedLogin = function() {
        return pageNeedLogin;
    };

    var get = function(key, isUpdatable) {
        var obj = localStorage.getItem(key);
        if (obj == null) {
            return null;
        }
        try {
            obj = JSON.parse(obj);
            if (!isUpdatable) {
                return obj;
            }
            obj.timestamp = new Date().getTime();
        } catch (e) {

        }
        set(key, obj);
        return obj;
    };

    var set = function(key, value) {
        localStorage.setItem(key, JSON.stringify(value));
    };

    var remove = function(key) {
        localStorage.removeItem(key);
    };

    var setUserInfo = function(value) {
        value.timestamp = new Date().getTime();
        set(ENVIRONMENT.StorageKey.USER, value);
    };

    const getUserInfo = function(showLoginType) {
        const currentDate = new Date().getTime();
        var obj = get(ENVIRONMENT.StorageKey.USER);
        if (isPageNeedLogin()) {
            if (!obj || !obj.ticket || obj.ticket.indexOf("tguest|") === 0) {
                return null;
            }
            if (!Util.differentTime(currentDate, obj.timestamp, Config.USER_TIMEOUT)) {
                remove(ENVIRONMENT.StorageKey.USER);
                return null;
            }
        }

        if (obj === null) {
            obj = { ticket: `tguest|${new Date().getTime()}`, lang: "FA" };
        }

        obj.timestamp = currentDate;
        setUserInfo(obj);

        obj.getTicket = function() {
            if (!this.ticket) {
                showLoginPage();
                return;
            }
            return this.ticket;
        };

        return obj;
    };

    const removeUserInfo = function() {
        remove(ENVIRONMENT.StorageKey.USER);
    };

    return {
        get: get,
        set: set,
        remove: remove,
        setUserInfo: setUserInfo,
        getUserInfo: getUserInfo,
        removeUserInfo: removeUserInfo,
        setPageNeedLogin: setPageNeedLogin,
        isPageNeedLogin: isPageNeedLogin
    };
})();