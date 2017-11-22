(function ($) {
    $.extend({
        isNull: function(str) {
            if (str != null && str.toString().trim().length > 0) {
                return false;
            }
            return true;
        },
        //验证汉字个数(str:字符串;minLength:汉字最小长度;maxLength:汉字最大长度;)
        chineseLengthValid: function(str, minLength, maxLength) {
            var len = $.getStringByteLength(str.toString().trim());
            if (len > maxLength * 2 || len < minLength * 2) {
                return false;
            }
            return true;
        },
        //获取字符串字节数(val:字符串;)
        getStringByteLength: function(val) {
            var zhlength = 0; // 全角
            var enlength = 0; // 半角
            for (var i = 0; i < val.toString().trim().length; i++) {
                if (val.substring(i, i + 1).match(/[^\x00-\xff]/ig) != null)
                    zhlength += 1;
                else
                    enlength += 1;
            }
            // 返回当前字符串字节长度
            return (zhlength * 2) + enlength;
        },
        //正则验证
        checkType: function (type) {
            switch (type) {
                case "Mail"://电子信箱
                    return /\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*/;
                    // /^([a-z0-9_\.-]+)@@([\da-z\.-]+)\.([a-z\.]{2,6})$/
                case "Int"://整数
                    return /^\d+$/;
                case "Decimal"://浮点数
                    return /^\d+(\.\d+)?$/;
                case "DateTime"://日期
                    return /^[1,2]\d{3}([/,-])(0?[1-9]|1[0,1,2])\1(3[0,1]|[1,2]\d|0?[1-9])$/;
                case "PhoneNumber"://手机号码
                    return /^1[3|4|5|8][0-9]\d{4,8}$/;
                case "ZipCode"://邮政编码
                    return /^\d{6}$/;
                case "Money"://货币（正数或负数）
                    return /^[+-]?\d*\.?\d{1,3}$/;
                case "IDCard"://身份证
                    return /(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/;
                case "QQ"://QQ
                    return /^\d{5,10}$/;
                case "Passport"://护照
                    return /(P\d{7})|(G\d{8})/;
            }
    }
    });
}(jQuery));

//错误提示信息
function castErrorMessage(type) {
    switch (type) {
        case "Mail"://电子信箱
            return "邮箱格式不正确！";
        case "Int"://整数
            return "请填写整数！";
        case "Decimal"://浮点数
            return "请填写整数或小数！";
        case "DateTime"://日期
            return "日期格式不正确！";
        case "PhoneNumber"://电话号码
            return "电话格式不正确！";
        case "ZipCode"://邮政编码
            return "邮政编码格式不正确！";
        case "Money"://货币（正数或负数）
            return "货币格式不正确！";
        case "IDCard"://身份证
            return "身份证格式不正确！";
    }
}
