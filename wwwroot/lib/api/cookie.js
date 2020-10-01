/**
 * Created by F.Kazemi on 7/21/2018.
 */

var Cookie = (function () {
    var set = function (name, value, expireMinutes) {
        var d = new Date();
        d.setTime(d.getTime() + (expireMinutes*60*1000));
        var expires = "expires="+ d.toUTCString();
        document.cookie = name + "=" + value + ";" + expires + ";path=/";
    };
    var get = function (name) {
        name += "=";
        var decodedCookie = decodeURIComponent(document.cookie);
        var ca = decodedCookie.split(';');
        for(var i = 0; i <ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) === ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) === 0) {
                return c.substring(name.length, c.length);
            }
        }
        return null;
    };
    var remove = function (name) {
        if(get(name)) {
            document.cookie = name + '=;expires=Thu, 01 Jan 1970 00:00:01 GMT;path=/';
        }
    };
    return {
        set: set,
        get: get,
        remove: remove
    }
})();
