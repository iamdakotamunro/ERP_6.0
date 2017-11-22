var ToolTipMsg = function () { this.initialize() };
ToolTipMsg.defaultWidth = "";    //全局默认宽度
ToolTipMsg.defaultHeight = "";   //全局默认高度(此为auto)       
ToolTipMsg.defaultCss = null;    //全局默认样式
ToolTipMsg.defaultBox = null;    //全局默认显示msg的容器
ToolTipMsg.mouseEnter = null;    //鼠标进入时发生的事件，利用此事件可以更新自定义显示msg的div中的内容，例如更改图片url
ToolTipMsg.winWidth = 0;
ToolTipMsg.winHeight = 0;
ToolTipMsg.prototype = {
    initialize: function () {
        if (!ToolTipMsg.defaultCss) {
            ToolTipMsg.defaultCss = "display: none; position: absolute; _position: absolute;z-index: 100;border:solid 1px #efa204;padding:5px;background-color:#ffffce;font-size:13px;word-wrap:break-word;";
        }
        if (!ToolTipMsg.defaultBox) { ToolTipMsg.defaultBox = $('<div style="' + ToolTipMsg.defaultCss + '"></div>'); }
        ToolTipMsg.defaultBox.appendTo(document.body);
        ToolTipMsg.winWidth = $(window).width();
        ToolTipMsg.winHeight = $(window).height();
    },
    bindToolTip: function (selector) {
        var mouseMove = this.mouseMove, mouseOut = this.mouseOut;
        $(selector).each(function () { $(this).mousemove(mouseMove).mouseout(mouseOut); });
    },
    mouseMove: function (e) {
        var $this = $(this);
        var tipMsg = $this.attr("ToolTipMsg"), tipBox = $this.attr("ToolTipBox");
        //当既没有ToolTipMsg属性也没有ToolTipBox属性时直接返回                
        if ((!tipMsg || $.trim(tipMsg).length == 0) && (!tipBox || tipBox.length == 0)) { return; }
        var top = e.pageY, left = e.pageX;
        if (!ToolTipMsg.tempToolBox) {
            var tipWidth = $this.attr("tipWidth"), tipHeight = $this.attr("tipHeight"), tipMaxWidth = $this.attr("tipMaxWidth");
            if (tipBox) {
                //倘若有自定义的显示内容的容器
                ToolTipMsg.tempToolBox = $("#" + tipBox).css({ "position": "absolute", "_position": "absolute", "z-index": 100 });
            }
            else {
                //采用默认的
                ToolTipMsg.tempToolBox = ToolTipMsg.defaultBox.html(tipMsg);
            }
            if (!tipWidth) { tipWidth = ToolTipMsg.defaultWidth; }
            if (!tipHeight) { tipHeight = ToolTipMsg.defaultHeight; }
            ToolTipMsg.tempToolBox.css({ "width": tipWidth, "height": tipHeight, "max-width": tipMaxWidth });
            if (ToolTipMsg.mouseEnter) { ToolTipMsg.mouseEnter.apply(this); }
        }
        if (ToolTipMsg.winWidth + $(document).scrollLeft() - left - ToolTipMsg.tempToolBox.width() < 33) {
            left = e.pageX - ToolTipMsg.tempToolBox.width() - 30;
        }
        if (ToolTipMsg.winHeight + $(document).scrollTop() - top - ToolTipMsg.tempToolBox.height() < 33) {
            top = e.pageY - ToolTipMsg.tempToolBox.height() - 30;
        }

        ToolTipMsg.tempToolBox.css({ "top": (top < 0 ? 10 : top) + "px", "left": (left - 5) + "px" }).show();
    },
    mouseOut: function (e) {
        if (ToolTipMsg.tempToolBox) {
            ToolTipMsg.tempToolBox.hide();
            ToolTipMsg.tempToolBox = null;
        }
    }
}