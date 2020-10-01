/**
 * Created by Farshad Kazemi on 2/17/2018.
 */

const OrderBy = function () {

    let list = [];

    const add = (columnName, type, fieldType = "STRING") => {
        let index = -1;
        list.forEach(function (item, i) {
            if (item.columnName === columnName) {
                index = i;
                return -1;
            }
        });
        if (index > -1) {
            list[index].orderByType = type;
        } else {
            list.push({columnName: columnName, orderByType: type, fieldType: fieldType});
        }
    };

    const remove = (columnName) => {
        list = list.filter(item => item.columnName !== columnName);
    };

    const get = () => {
        return list;
    };

    return {
        add: add,
        remove: remove,
        get: get
    }

};
