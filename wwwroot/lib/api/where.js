/**
 * Created by Farshad Kazemi on 2/17/2018.
 */

var Wheres = function() {

    var list = [];

    const add = function(key, value, condition, operatorWithNext, operatorWithList, wheres, fieldType) {
        const obj = {
            key: key,
            value: `${value}`,
            condition: condition,
            operatorWithNext: 1,
            operatorWithList: 1,
            wheres: [],
            fieldType: 1
        };
        if (key === ENVIRONMENT.FULL_TEXT_SEARCH_KEY) {
            list = [];
            list.push(obj);
            return;
        }
        if (operatorWithNext) {
            obj.operatorWithNext = operatorWithNext;
        }
        if (operatorWithList) {
            obj.operatorWithList = operatorWithList;
        }
        if (wheres) {
            obj.wheres = wheres;
        }
        if (fieldType) {
            obj.fieldType = fieldType;
        }
        list.push(obj);
    };

    const remove = function(key, wheres) {
        var index = -1;
        if (wheres) {
            wheres.forEach(function(item, i) {
                if (item.key == key) {
                    index = i;
                    return -1;
                }
            });
            if (index > -1) {
                wheres.splice(index, 1);
            }
        } else {
            list.forEach(function(item, i) {
                if (item.key == key) {
                    index = i;
                    return -1;
                }
            });
            if (index > -1) {
                list.splice(index, 1);
            }
        }
    };

    const removeByValue = function(value) {
        var index = -1;
        list.forEach(function(item, i) {
            if (item.value === value) {
                index = i;
                return -1;
            }
        });
        if (index > -1) {
            list.splice(index, 1);
        }
    };

    const get = function(key, wheres) {
        var result = null;
        if (wheres) {
            wheres.forEach(function(item) {
                if (item.key == key) {
                    result = item;
                    return -1;
                }
            });
        } else {
            list.forEach(function(item) {
                if (item.key == key) {
                    result = item;
                    return -1;
                }
            });
        }
        return result;
    };

    const getList = function() {
        return list;
    };

    const clearList = function() {
        list = [];
    };

    return {
        add: add,
        remove: remove,
        removeByValue: removeByValue,
        get: get,
        getList: getList,
        clearList: clearList
    };
};