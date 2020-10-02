/**
 * Created by F.Kazemi on 7/21/2018.
 */

var Cookie = (function() {
    const set = function(name, value, expireMinutes) {
        const d = new Date();
        d.setTime(d.getTime() + (expireMinutes * 60 * 1000));
        const expires = `expires=${d.toUTCString()}`;
        document.cookie = name + "=" + value + ";" + expires + ";path=/";
    };
    var get = function(name) {
        name += "=";
        const decodedCookie = decodeURIComponent(document.cookie);
        const ca = decodedCookie.split(";");
        for (let i = 0; i < ca.length; i++) {
            let c = ca[i];
            while (c.charAt(0) === " ") {
                c = c.substring(1);
            }
            if (c.indexOf(name) === 0) {
                return c.substring(name.length, c.length);
            }
        }
        return null;
    };
    const remove = function(name) {
        if (get(name)) {
            document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:01 GMT;path=/";
        }
    };
    return {
        set: set,
        get: get,
        remove: remove
    };
})();