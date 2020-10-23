/**
 * Created by Farshad Kazemi on 2/13/2018.
 */
const ENVIRONMENT = {
    FULL_TEXT_SEARCH_KEY: 'fullTitle',
    OrderType: {
        ASC: 1,
        DESC: 2
    },
    Condition: {
        EQUAL: 1, // =
        NOT_EQUAL: 2, // <>
        GREATER: 3, // >
        LESS: 4, // <
        GREATER_OR_EQUAL: 5, // >=
        LESS_OR_EQUAL: 6, // <=
        CONTAINS: 7, // LIKE
        START_WITH: 8, // %LIKE
        END_WITH: 9, // LIKE%
        NOT_LIKE: 10, // NOT %LIKE%
        IN:11, // IN
        NOT_IN: 12, // NOT IN
        IS_NULL:13, // IS NULL
        NOT_NULL: 14 // IS NOT NULL
    },
    Operator: {
        AND:1,
        OR: 2
    },
    FieldType: {
//        TEXT: 'TEXT',
//        NUMBER: 'NUMBER',
//        AUTOCOMPLETE: 'AUTOCOMPLETE',
        DATE: 2,
        BOOLEAN: 8,
        STRING:0,
        INTEGER: 3,
        LONG: 4,
        DOUBLE: 5,
        FLOAT: 7
    },
    FieldCondition: {
        NUMBER: ['EQUAL', 'NOT_EQUAL', 'GREATER', 'LESS', 'GREATER_OR_EQUAL', 'LESS_OR_EQUAL', 'IN', 'NOT_IN', 'BETWEEN', 'IS_NULL', 'NOT_NULL'],
        TEXT: ['EQUAL', 'NOT_EQUAL', 'CONTAINS', 'START_WITH', 'END_WITH', 'IN', 'NOT_IN', 'BETWEEN', 'IS_NULL', 'NOT_NULL'],
        AUTOCOMPLETE: ['EQUAL', 'NOT_EQUAL', 'IN', 'NOT_IN', 'IS_NULL', 'NOT_NULL'],
        DATE: ['EQUAL', 'NOT_EQUAL', 'BETWEEN', 'IN', 'NOT_IN', 'IS_NULL', 'NOT_NULL'],
        BOOLEAN: ['EQUAL', 'NOT_EQUAL']
    },
    StorageKey: {
        USER: 'user'
    },
    ErrorCode: {
        USER_EXPIRED: 'USER_EXPIRED',
        BUSINESS_MESSAGE: 'BUSINESS_MESSAGE',
        IS_NULL: 'IS_NULL'
    },
    ErrorDesc: {
        NO_DATA: 'NO_DATA'
    },
    Action: {
        DELETE: 'DELETE',
        EDIT: 'EDIT',
        DUPLICATE: 'DUPLICATE',
        EXPORT:'EXPORT'
    },
    KeyCode: {
        MIDDLE_MOUSE: 2,
        BACK_SPACE: 8,
        TAB: 9,
        ENTER: 13,
        SHIFT: 16,
        CTRL: 17,
        ALT: 18,
        PAUSE_BREAK: 19,
        CAPS_LOCK: 20,
        ESC: 27,
        PAGE_UP: 33,
        PAGE_DOWN: 34,
        END: 35,
        HOME: 36,
        ARROW_LEFT: 37,
        ARROW_UP: 38,
        ARROW_RIGHT: 39,
        ARROW_DOWN: 40,
        INSERT: 45,
        DELETE: 46,
        DIGIT_0: 48,
        DIGIT_1: 49,
        DIGIT_2: 50,
        DIGIT_3: 51,
        DIGIT_4: 52,
        DIGIT_5: 53,
        DIGIT_6: 54,
        DIGIT_7: 55,
        DIGIT_8: 56,
        DIGIT_9: 57,
        A: 65,
        B: 66,
        C: 67,
        D: 68,
        E: 69,
        F: 70,
        G: 71,
        H: 72,
        I: 73,
        J: 74,
        K: 75,
        L: 76,
        M: 77,
        N: 78,
        O: 79,
        P: 80,
        Q: 81,
        R: 82,
        S: 83,
        T: 84,
        U: 85,
        V: 86,
        W: 87,
        X: 88,
        Y: 89,
        Z: 90,
        WINDOWS_LEFT: 91,
        WINDOWS_RIGHT: 92,
        SELECT: 93,
        NUM_PAD_0: 96,
        NUM_PAD_1: 97,
        NUM_PAD_2: 98,
        NUM_PAD_3: 99,
        NUM_PAD_4: 100,
        NUM_PAD_5: 101,
        NUM_PAD_6: 102,
        NUM_PAD_7: 103,
        NUM_PAD_8: 104,
        NUM_PAD_9: 105,
        MULTIPLY: 106,
        ADD: 107,
        SUBTRACT: 109,
        DECIMAL_POINT: 110,
        DIVIDE: 111,
        F1: 112,
        F2: 113,
        F3: 114,
        F4: 115,
        F5: 116,
        F6: 117,
        F7: 118,
        F8: 119,
        F9: 120,
        F10: 121,
        F11: 122,
        F12: 123,
        NUM_LOCK: 144,
        SCROLL_LOCK: 145,
        SEMI_COLON: 186,
        EQUAL: 187,
        COMMA: 188,
        DASH: 189,
        PERIOD: 190,
        FORWARD_SLASH: 191,
        GRAVE_ACCENT: 192,
        BRACKET_OPEN: 219,
        BACK_SLASH: 220,
        BRACKET_CLOSE: 221,
        SINGLE_QUOTE: 222,
        NUM_0: 48,
        NUM_1: 49,
        NUM_2: 50,
        NUM_3: 51,
        NUM_4: 52,
        NUM_5: 53,
        NUM_6: 54,
        NUM_7: 55,
        NUM_8: 56,
        NUM_9: 57
    },
    PaginationKey: {
        NEXT: 'next',
        PREV: 'prev'
    },
    ResponseCode: {
        200: "موفقیت آمیز",
        201: "Created",
        202: "Accepted",
        204: "No Content",
        205: "Reset Content",
        206: "Partial Content",
        301: "Moved Permanently",
        302: "Found",
        303: "See Other",
        304: "Not Modified",
        305: "Use Proxy",
        307: "Temporary Redirect",
        400: "اطلاعات ارسالی اشتباه می باشد",
        401: "Unauthorized",
        402: "Payment Required",
        403: "Forbidden",
        404: "صفحه مورد نظر یافت نشد",
        405: "Method Not Allowed",
        406: "Not Acceptable",
        407: "Proxy Authentication Required",
        408: "Request Timeout",
        409: "Conflict",
        410: "Gone",
        411: "Length Required",
        412: "Precondition Failed",
        413: "Request Entity Too Large",
        414: "Request-URI Too Long",
        415: "Unsupported Media Type",
        416: "Requested Range Not Satisfiable",
        417: "Expectation Failed",
        500: "Internal Server Error",
        501: "Not Implemented",
        502: "Bad Gateway",
        503: "Service Unavailable",
        504: "Gateway Timeout",
        505: "HTTP Version Not Supported"
    },
    Autocomplete: {
        TYPE: {
            STATIC: 'STATIC',
            DYNAMIC: 'DYNAMIC'
        }
    },
    ShowLoginPageType: {
        SHOW_LOGIN_PAGE: "SHOW_LOGIN_PAGE",
        WRITE_MESSAGE: "WRITE_MESSAGE",
        NOTHINGS:"NOTHINGS"
    },
    CssClass: {
        ACTIVE: 'active',
        VISIBLE: 'visible',
        HAS_ERROR: "has-error",
        VALID: "valid",
        MANDATORY: 'mandatory',
        FORM_GROUP: 'form-group',
        FORM_CONTROL: 'form-control',
        FEED_BACK: 'feedback',
        FLOATING_MODE: 'floating-mode',
        INVALID_FEED_BACK: 'invalid-feedback',
        DATE: 'date',
        DISABLE: 'disable',
        DISABLED: 'disabled',
        SUCCESS: 'success',
        DANGER: 'danger',
        WARNING: 'warning',
        ANIMATED: 'animated',
        BOUNCE_IN: 'bounceIn',
        FLASH: 'flash',
        FADE_IN_UP: 'fadeInUp',
        EXPANDED: 'expanded',
        INVISIBLE: 'invisible'
    },
    InputType: {
        TEXT: 'text',
        NUMBER: 'number'
    },
    PersianDateFormat: {
        FullDateDigit: '',
        FullDateText: '',
        FullDateDigitWithMonthText: 'D MMMM YYYY'
    },
    EditorType: {
        DEFAULT: 'default',
        HTML: 'html',
        CSS: 'css'
    }
};


