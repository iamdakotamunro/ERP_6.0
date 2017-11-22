<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Expresslisting.ascx.cs" Inherits="ERP.UI.Web.UserControl.Expresslisting" %>


<script src="../JsLib/jquery.js" type="text/javascript"></script>

<div class="exwindows" id="exserch">
    <div class="exbor">
        <div class="exborcont">
            <div class="extitle">
                物流信息<span class="exclose" onclick="cse()" title='关闭'></span>
                <div class="exarrow exarrownotbg" id="exarrow">
                </div>
            </div>
            <%--  <div>
                    <span class="ewleft">物流编号：</span><span class="ewright">sdfsdfdsf</span>
                </div>--%>
            <div>
                <span class="ewleft">物流公司：</span><span class="ewright" id="EXCompany"></span>
            </div>
            <div>
                <span class="ewleft">运单号码：</span><span class="ewright" id="ExpressNo"></span>
            </div>
            <div>
                <span class="ewleft">物流跟踪：</span> <span class="ewright"><span>以下信息由物流公司提供，如有疑问请查询<a
                    id="exgos" style="color: blue; font-size: 14px;" target="_blank"></a>官方网站 </span>
                </span>
            </div>
            <div id="LatelyInfo">
            </div>
        </div>
    </div>
</div>

<script type="text/javascript">


    function SetExpressData(trackingInfo) {
        var str = '';
        if (trackingInfo.status == "1") {
            for (var i = 0; i < trackingInfo.data.length; i++) {
                str = str + "<div " + (i + 1) + trackingInfo.data.length + " > " + trackingInfo.data[i].time + " " + trackingInfo.data[i].context + "</div>"
            }
        } else {
            str = "<div class=\"ewlast\" >没有查询到符合条件的运单</div>";
        }        
        $('#LatelyInfo').html(str);
    }

    //<![CDATA[
    String.prototype.getQueryString = function (name) {
        var reg = new RegExp("(^|&|\\?)" + name + "=([^&]*)(&|$)"), r;
        if (r = this.match(reg)) return unescape(r[2]);
        return null;
    };
    var address = location.search.getQueryString("address"); 
    function loadXML(xmlFile) {
        var xmlDoc;
        if (window.ActiveXObject) {
            xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
            xmlDoc.async = false;
            xmlDoc.load(xmlFile);
        }
        else if (document.implementation && document.implementation.createDocument) {
            var oXmlHttp = new XMLHttpRequest();
            oXmlHttp.open("GET", xmlFile, false);
            oXmlHttp.send(null);
            return oXmlHttp.responseXML;
        } else {
            alert('您的浏览器不支持该系统脚本！');
        }
        return xmlDoc;
    }

    function getRootPath() {
        var strFullPath = window.document.location.href;
        var strPath = window.document.location.pathname;
        var pos = strFullPath.indexOf(strPath);
        var prePath = strFullPath.substring(0, pos);
        //var postPath = strPath.substring(0, strPath.substr(1).indexOf('/') + 1);
        return prePath;
    }


    function SetWTPExpressData() {
        var str = '';
        var xmlDoc = loadXML("../express/XMLFile1.xml");
        var step = xmlDoc.getElementsByTagName("step");
        var steplen = step.length;
        for (var i = 0; i < steplen; i++) {                       //XML中记录了多个坐标点，要每个点都标记一下
            var acceptTime = step[i].getElementsByTagName("acceptTime");
            var acceptAddress = step[i].getElementsByTagName("acceptAddress");
            str = str + "<div> " + acceptTime[0].firstChild.nodeValue
                + " " + acceptAddress[0].firstChild.nodeValue + "</div>";
        }
        $('#LatelyInfo').html(str);
    }

    $(document).ready(function() {
        $(".checking").live("click", function() {
            if (!$(this).data("oldsrc")) {
                $(this).data("oldsrc", $(this).attr("src"));
            }
            $(this).attr("src", $(this).data("oldsrc") + "&t=" + Date.parse(new Date()));
            $(this).prev("input").val("");
        })
    })
    cse();
    function subclick(excode, exno) {
        var authCode = $("#authcode").val();
        if (authCode == '') {
            alert('请输入验证码！'); return;
        }
        $.ajax({
        url: '/UserControl/TrackingExpressInfo.aspx',
            type: 'post',
            dataType: 'json',
            data: $.param({ ExpressCode: excode, ExpressNo: exno, AuthCode: authCode }),
            beforeSend: function() {
                $('#latelyInfo').html("数据正在加载...");
            },
            success: function(result) {
                SetExpressData(result);
            },
            error: function() {
                alert("System.SearchExpressInfo.Error");
            }
        })
    }
    function cse() { //CloseSerarchExpress
        $(".checking").remove();
        $("#authcode").remove();
        $("#subbutton").remove();
        $("#LatelyInfo div").remove();
        $('#exserch').hide();
    }
   
</script>

<style type="text/css">
    /*物流查询窗口*/
    .exwindows
    {
        position: absolute;
        width: 510px;
        display: none;
        left: 90px;
        z-index: 100;
    }
    .exbor
    {
        width: 460px; /*border: 1px solid #f0d18e;*/
        border: 1px solid #3B4E5F;
        padding: 14px; /*background-color: #fffdee;*/
        background-color: #DAE1E7;
    }
    .exborcont
    {
        /*border: 1px solid #fde6bc;
        background-color: #fffffb;*/
        border: 1px solid #cccccc;
        background-color: #F7F7F7;
        padding: 10px;
    }
    .exborcont div
    {
        margin-top: 4px;
        margin-bottom: 4px;
    }
    .remicon div
    {
        margin-bottom: 4px;
        margin-left: 20px;
        margin-right: 20px;
    }
    .remicon
    {
        margin-left: 10px;
    }
    .remicon .extitle
    {
        font-weight: normal;
    }
    .remicon .reitems
    {
        margin-bottom: 10px;
        padding-left: 60px;
        color: #7C7C7C;
    }
    /*   .remicon #reDate,.remicon #reBalance{
     width:110px;
    
    }*/
    .rebottome
    {
        margin-top: 20px;
        text-align: center;
    }
    .extitle
    {
        text-align:center;
        color: #010101;
        font-size: 14px;
        font-weight: 600;
        border-bottom: 1px solid #d6d6d6;
        height: 28px;
        line-height: 28px;
    }
    .ewleft
    {
        width: 60px;
    }
    .ewright
    {
        width: 340px;
    }
    .ewlast
    {
        color: #c20300;
    }
    .exclose, .exarrow
    {
        display: block;
        position: relative; /*background-image: url("site/account/drawbg.jpg");*/
    }
    .exclose
    {
        /*height: 20px;
        width: 22px;*/
        height: 16px;
        width: 16px;
        cursor: pointer; 
        /*background-position: 0px -138px;*/
        top: -57px;
        *top: -54px;
        /*left: 436px;*/
        left: 448px;
        *left: 237px;
        background-image: url( "../App_Themes/Default/images/Cancel.gif" );
    }
    .exarrow
    {
        background-position: -21px -139px;
        height: 22px;
        width: 14px;
        left: 461px;
        top: -35px;
    }
    .exarrownotbg
    {
        background-position: -34px -139px;
    }
    /*End*/
    .Button
    {
        border: 1px solid #D5D5D5;
        border-bottom: 1px solid #C2C2C2;
        border-right: 1px solid #C2C2C2;
        font-size: 11px;
        color: #666666;
        background-image: url(./images/buttonBg.gif);
        background-position: top;
        background-color: white;
        height: 20px;
        vertical-align: middle;
    }
    .StandardInput
    {
        width: 150px;
        height: 18px;
        font-size: 12px;
        border: solid 1px #eaeaea;
    }
</style>
