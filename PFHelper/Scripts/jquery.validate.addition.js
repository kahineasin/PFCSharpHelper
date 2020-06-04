/*
附加验证--benjamin
*/
if (jQuery !== null && jQuery !== undefined && jQuery.validator !== null && jQuery.validator !== undefined) {
    jQuery.extend(jQuery.validator.messages, {
        required: "必填",
        remote: "请修正该字段",
        email: "请输入正确格式的电子邮件",
        url: "请输入合法的网址",
        date: "请输入合法的日期",
        dateISO: "请输入合法的日期 (ISO).",
        number: "请输入合法的数字",
        digits: "只能输入整数",
        creditcard: "请输入合法的信用卡号",
        equalTo: "请再次输入相同的值",
        accept: "请输入拥有合法后缀名的字符串",
        maxlength: jQuery.validator.format("请输入一个 长度最多是 {0} 的字符串"),
        minlength: jQuery.validator.format("请输入一个 长度最少是 {0} 的字符串"),
        rangelength: jQuery.validator.format("请输入 一个长度介于 {0} 和 {1} 之间的字符串"),
        range: jQuery.validator.format("请输入一个介于 {0} 和 {1} 之间的值"),
        max: jQuery.validator.format("请输入一个最大为{0} 的值"),
        min: jQuery.validator.format("请输入一个最小为{0} 的值"),
        isYear: "不是年",
        isMonth: "不是月",
        isLargerThen: "必需大于最小值",
        //minNum: "不能小于最小值",//注意不要使用minNum,可能是关键字,把规则写在dom上时不生效--benjamin20200116
        noBlankSpace: "不能有空格"
    });

    jQuery.validator.methods.minDate = function (value, element, param) {//截止时间不能小于开始时间(任何一个为空时不验证)

        var hasFormat = param instanceof Array;
        var domSelector = hasFormat ? param[0] : param;
        var dateFormat = hasFormat ? param[1] : '';
        var startDate = jQuery(domSelector).val();

        if (value === null || value === '' || startDate === null || startDate === '') { return true; }

        ////var date1 = new Date($pf.formatTime(startDate, "yyyy-MM-dd hh:mm:ss"));
        ////var date2 = new Date($pf.formatTime(value, "yyyy-MM-dd hh:mm:ss"));
        //var date1 = new Date(Date.parse(startDate.replace("-", "/")));
        //var date2 = new Date(Date.parse(value.replace("-", "/")));

        var date1 = hasFormat ? $pf.stringToDate(startDate, dateFormat) : new Date(Date.parse(startDate.replace("-", "/")));
        var date2 = hasFormat ? $pf.stringToDate(value, dateFormat) : new Date(Date.parse(value.replace("-", "/")));
        return date1 <= date2;
    };
    //是年份如:2019
    jQuery.validator.methods.isYear = function (value, element, param) {//截止时间不能小于开始时间(任何一个为空时不验证)
        var reg = /^\d{4}$/;
        return this.optional(element) || (reg.test(value));
    };
    //是月份1~12
    jQuery.validator.methods.isMonth = function (value, element, param) {//截止时间不能小于开始时间(任何一个为空时不验证)
        var reg = /^((1[0-2])|([1-9]))$/;
        return this.optional(element) || (reg.test(value));
    };
    jQuery.validator.methods.isLargerThen = function (value, element, param) {//截止时间不能小于开始时间(任何一个为空时不验证)
        var startNum = jQuery(param).val();

        if (value === null || value === '' || startNum === null || startNum === '') { return true; }

        var num1 = parseInt(startNum);
        var num2 = parseInt(value);
        return num1 <= num2;
    };
    jQuery.validator.methods.noBlankSpace = function (value, element, param) {//截止时间不能小于开始时间(任何一个为空时不验证)
        if (value === null || value === '') { return true; }
        return !(/^\s.*$/.test(value) || /^.*\s$/.test(value));

    };
    //jQuery.validator.methods.compareTime = function (value, element, param) {
    //    //var startDate = jQuery(param).val() + ":00";补全yyyy-MM-dd HH:mm:ss格式 
    //    //value = value + ":00"; 
    //    var startDate = jQuery(param).val();
    //    var date1 = new Date(Date.parse(("2000-01-01 " + startDate).replace("-", "/")));
    //    var date2 = new Date(Date.parse(("2000-01-01 " + value).replace("-", "/")));
    //    return date1 < date2;
    //};

}