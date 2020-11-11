/**
 * Created by Farshad Kazemi on 2/14/2018.
 */

/**
 * @param key: string
 * @param value: string|number|array
 * @param condition: string
 * @param operatorWithNext: string
 * @param operatorWithList: string
 * @param wheres: Array<Filter>
 * @constructor
 */
var Filter = function(where, pageSize, pageNo, orderBy, language, ticket) {

    this.wheres = where;
    this.pageSize = pageSize;
    this.pageNo = pageNo;
    this.orderBy = orderBy;
    this.language = language;
    this.ticket = ticket;
    this.getWheres = function() {
        return this.wheres;
    };
    this.setWheres = function(wheres) {
        this.wheres = wheres;
    };
    this.setPageSize = function(pageSize) {
        this.pageSize = pageSize;
    };
    this.getPageSize = function() {
        return this.pageSize;
    };
    this.setPageNumber = function(pageNumber) {
        this.pageNo = pageNumber;
    };
    this.getPageNumber = function() {
        return this.pageNo;
    };
    this.setOrdersBy = function(ordersBy) {
        this.orderBy = ordersBy;
    };
    this.getOrdersBy = function() {
        return this.orderBy;
    };
    this.setLanguage = function(lang) {
        this.language = lang;
    };
    this.getLanguage = function() {
        return this.language;
    };
    this.setTicket = function(ticket) {
        this.ticket = ticket;
    };
    this.getTicket = function() {
        return this.ticket;
    };
    this.get = function() {
        return {
            wheres: this.wheres,
            pageSize: this.pageSize,
            pageNo: this.pageNo,
            orderBy: this.orderBy,
            language: this.language,
            ticket: this.ticket
        };
    };
};