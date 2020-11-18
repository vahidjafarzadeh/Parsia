"use strict";

var localVariables;

function setLocalVariables() {
    localVariables = {
        URLs: {
            services: {
                gridView: 'person/gridView',
                delete: 'person/delete'
            }
        },
        clazzName: "Person"
    };
}
const changeDate = (res) => {
    if (res) {
        return new persianDate(Number(res)).format("YYYY/MM/DD");
    } else {
        return "-";
    }
}
var initAutoCompletes = function () {
    //جنسیت
    let sex = new Autocomplete();
    let whereSex = new Wheres();
    whereSex.add('parentCode', 'GENDER', ENVIRONMENT.Condition.EQUAL);
    sex.init({
        element: $('#sex'),
        staticKey: 'sexComboPerson',
        placeholder: 'sex',
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        dropdownCssClass: 'form-control-sm floating',
        where: whereSex,
        searchColumn: 'name'
    });
    //ملیت
    let nationality = new Autocomplete();
    let whereNationality = new Wheres();
    whereNationality.add('parentCode', 'NATIONALITY', ENVIRONMENT.Condition.EQUAL);
    nationality.init({
        element: $('#nationality'),
        staticKey: 'nationalityComboPerson',
        placeholder: 'nationality',
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        dropdownCssClass: 'form-control-sm floating',
        where: whereNationality,
        searchColumn: 'name'
    });
    //گروه خونی
    let bloodType = new Autocomplete();
    let whereBloodType = new Wheres();
    whereBloodType.add('parentCode', 'BLOOD_TYPE', ENVIRONMENT.Condition.EQUAL);
    bloodType.init({
        element: $('#bloodType'),
        staticKey: 'bloodTypeComponentPerson',
        placeholder: 'bloodType',
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        dropdownCssClass: 'form-control-sm floating',
        where: whereBloodType,
        searchColumn: 'name'
    });
    //وضعیت حیات
    let lifeStatus = new Autocomplete();
    let whereLifeStatus = new Wheres();
    whereLifeStatus.add('parentCode', 'LIFE_STATUS', ENVIRONMENT.Condition.EQUAL);
    lifeStatus.init({
        element: $('#lifeStatus'),
        staticKey: 'lifeStatusComboPersons',
        placeholder: 'lifeStatus',
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        dropdownCssClass: 'form-control-sm floating',
        where: whereLifeStatus,
        searchColumn: 'name'
    });
    //تابعیت
    let citizenship = new Autocomplete();
    let whereCitizenship = new Wheres();
    whereCitizenship.add('parentCode', 'CITIZEN_SHIP', ENVIRONMENT.Condition.EQUAL);
    citizenship.init({
        element: $('#citizenship'),
        staticKey: 'citizenshipComboPerson',
        placeholder: 'citizenship',
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        dropdownCssClass: 'form-control-sm floating',
        where: whereCitizenship,
        searchColumn: 'name'
    });
    //دین
    let religion = new Autocomplete();
    let whereReligion = new Wheres();
    whereReligion.add('parentCode', 'RELIGION', ENVIRONMENT.Condition.EQUAL);
    religion.init({
        element: $('#religion'),
        staticKey: 'religionComboPerson',
        placeholder: 'religion',
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        dropdownCssClass: 'form-control-sm floating',
        where: whereReligion,
        searchColumn: 'name'
    });
    //مذهب
    let subReligion = new Autocomplete();
    let whereSubReligion = new Wheres();
    whereSubReligion.add('parentCode', 'SUB_RELIGION', ENVIRONMENT.Condition.EQUAL);
    subReligion.init({
        element: $('#subReligion'),
        staticKey: 'subReligionComboPerson',
        placeholder: 'subReligion',
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        dropdownCssClass: 'form-control-sm floating',
        where: whereSubReligion,
        searchColumn: 'name'
    });
    //وضعیت تاهل
    let maritalStatus = new Autocomplete();
    let whereMaritalStatus = new Wheres();
    whereMaritalStatus.add('parentCode', 'MARITAL_STATUS', ENVIRONMENT.Condition.EQUAL);
    maritalStatus.init({
        element: $('#maritalStatus'),
        staticKey: 'maritalStatusComboPerson',
        placeholder: 'maritalStatus',
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        dropdownCssClass: 'form-control-sm floating',
        where: whereMaritalStatus,
        searchColumn: 'name'
    });
    //وضعیت نظام وظیفه
    let militaryServiceStatus = new Autocomplete();
    let whereMilitaryServiceStatus = new Wheres();
    whereMilitaryServiceStatus.add('parentCode', 'MILITARY_SERVICE_STATUS', ENVIRONMENT.Condition.EQUAL);
    militaryServiceStatus.init({
        element: $('#militaryServiceStatus'),
        staticKey: 'militaryServiceStatusStatusComboPerson',
        placeholder: 'militaryServiceStatus',
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        dropdownCssClass: 'form-control-sm floating',
        where: whereMilitaryServiceStatus,
        searchColumn: 'name'
    });
    //وضعیت مسکن
    let housingSituation = new Autocomplete();
    let whereHousingSituation = new Wheres();
    whereHousingSituation.add('parentCode', 'HOUSING_SITUATION', ENVIRONMENT.Condition.EQUAL);
    housingSituation.init({
        element: $('#housingSituation'),
        staticKey: 'housingSituationStatusComboPersons',
        placeholder: 'housingSituation',
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        dropdownCssClass: 'form-control-sm floating',
        where: whereHousingSituation,
        searchColumn: 'name'
    });
    //وضعیت جسمانی
    let healthStatus = new Autocomplete();
    let whereHealthStatus = new Wheres();
    whereHealthStatus.add('parentCode', 'HEALTH_STATUS', ENVIRONMENT.Condition.EQUAL);
    healthStatus.init({
        element: $('#healthStatus'),
        staticKey: 'healthStatusComboPersons',
        placeholder: 'healthStatus',
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        dropdownCssClass: 'form-control-sm floating',
        where: whereHealthStatus,
        searchColumn: 'name'
    });
    //نوع معلولیت
    let disabilityType = new Autocomplete();
    let whereDisabilityType = new Wheres();
    whereDisabilityType.add('parentCode', 'DISABILITY_TYPE', ENVIRONMENT.Condition.EQUAL);
    disabilityType.init({
        element: $('#disabilityType'),
        staticKey: 'disabilityTypeComboPersons',
        placeholder: 'disabilityType',
        type: ENVIRONMENT.Autocomplete.TYPE.STATIC,
        dropdownCssClass: 'form-control-sm floating',
        where: whereDisabilityType,
        searchColumn: 'name'
    });
};

function localPageReady() {
    setLocalVariables();
    initLocalVariables(localVariables);
    initAutoCompletes();
    onPageReady();
}
$(document).ready(function () {
    Storage.setPageNeedLogin(true);
    if (Storage.isPageNeedLogin() && Storage.getUserInfo() === null) {
        SSO.init();
    } else {
        localPageReady();
    }
});
