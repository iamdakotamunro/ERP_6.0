//clear the string blank space
String.prototype.trim = function() {
    return this.replace(/(\s*$)|(^\s*)/g, '');
}
String.prototype.rtrim = function() {
    return this.replace(/(\s*$)/g, '');
}
String.prototype.ltrim = function() {
    return this.replace(/(^\s*)/g, '');
}

//the browser type object
var browserName = navigator.userAgent.toLowerCase();
mybrowser = {
    version: (browserName.match(/.+(?:rv|it|ra|ie)[\/: ]([\d.]+)/) || [0, '0'])[1],
    safari: /webkit/i.test(browserName) && !this.chrome,
    opera: /opera/i.test(browserName),
    firefox: /firefox/i.test(browserName),
    ie: /msie/i.test(browserName) && !/opera/.test(browserName),
    mozilla: /mozilla/i.test(browserName) && !/(compatible|webkit)/.test(browserName) && !this.chrome,
    chrome: /chrome/i.test(browserName) && /webkit/i.test(browserName) && /mozilla/i.test(browserName)
}

//display or hidden the element
function ShowObject(idName) {
    //    var obj = document.getElementById(idName);
    //    if (obj) {
    //        obj.style.zIndex = 100;
    //        obj.style.visibility = "visible";
    //    }
    //    return false;
    return VisibleObject(idName);
}
function HiddenObject(idName) {
    //    var obj = document.getElementById(idName);
    //    if (obj) {
    //        obj.style.zIndex = -100;
    //        obj.style.visibility = "hidden";
    //    }
    //    return false;
    return VisibleObject(idName);
}
//if object is hidden ,display it
//if object is display,hidden it
function VisibleObject(idName) {
    var obj = document.getElementById(idName);
    if (obj) {
        if (obj.style.visibility == "hidden") {
            obj.style.zIndex = 100;
            obj.style.visibility = "visible";
        }
        else {
            obj.style.zIndex = -100;
            obj.style.visibility = "hidden";
        }
    }
    return false;
}

//a enum for filiale type
var LonemeJS = new Object();
LonemeJS.FilialeType = {
    MainFiliale: 0,
    SubFiliale: 1,
    DoorShop: 2,
    EntityShop: 3
};


function showmask() {
    var mask = $("#DialogMask");
    var loadimg = $("#LoadingImg");
    if (mask.length == 0) {
        mask = $("<div/>").attr("id", "DialogMask");
        mask.appendTo($("body"));
        loadimg = $("<div/>").attr("id", "LoadingImg").css({width:50,height:50}).append($("<img>").attr("src", "../UserDir/PublicImages/loading.gif"));
        loadimg.appendTo(mask);
    }
    mask.show();
    mask.css({
        width: $(document).width(),
        height:$(document).height(),
        zIndex:9998,
        backgroundColor:"white",
        position:"absolute",
        top:0,
        left:0
    });
    loadimg.css({
        zIndex: 9999,
        backgroundColor: "white",
        position: "absolute",
        left: Math.floor(($(document).width()-loadimg.width())/2),
        top: Math.floor(($(document).height() - loadimg.height()) / 2)
    })
}

function hidemask() {
    $("#DialogMask").hide();
}

