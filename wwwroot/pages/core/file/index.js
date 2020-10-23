let containerFilter, localVariable;
const initializeVariable = () => {
    localVariable = {
        urls: {
            getAllExtension: "file/getAllExtension",
            gridView: "file/gridView"
        }
    }
    containerFilter = $("#file-manager-filter-main-list");
}
const getExtensionForView = () => {
    const configData = {
        Wheres: [],
        PageSize: 0,
        PageNo: 0,
        Ticket: "",
        OrderBy: [],
        Language: 1
    };
    const handler = new Handler();
    handler.beforeSend = () => { };
    handler.complete = () => { };
    handler.success = (data) => {
        if (data.done) {
            $.each(data.result, (index, value) => {
                const li = $("<li/>");
                const label = $("<label/>", { class: "control control--checkbox", text: value });
                const input = $("<input/>", { type: "checkbox" });
                const indicator = $("<div/>", { class: "control__indicator" });
                label.append(input, indicator);
                li.append(label);
                containerFilter.append(li);
            });
        } else {
            handler.configError(data);
        }
    }
    Api.post({ url: localVariable.urls.getAllExtension, data: configData, handler: handler });
}
gridView = () => {
    var where = new Wheres();
    where.add("parentId", "null", ENVIRONMENT.Condition.IS_NULL, ENVIRONMENT.Operator.AND);
    where.add("displayInFileManager", "true", ENVIRONMENT.Condition.EQUAL, ENVIRONMENT.Operator.AND,1,[], ENVIRONMENT.FieldType.BOOLEAN);
    const configData = {
        wheres: where.getList(),
        pageSize: 0,
        pageNo: 0,
        ticket: "",
        orderBy: [],
        language: 1
    };
    const handler = new Handler();
    handler.complete = () => {
        hideLoading();
    };
    handler.success = (data) => {
        if (data.done) {
            $.each(data.result, (index, value) => {
                console.log(value);
            });
        } else {
            handler.configError(data);
        }
    }
    Api.post({ url: localVariable.urls.gridView, data: configData, handler: handler });
}

$(function () {
    initializeVariable();
    getExtensionForView();
    gridView();

})