var vGridContent;   //DataGrid的内容
var vHeaderInfo;   //打印的表头
var vTailerInfo;   //打印的表尾
/*
目的：在页面写入隐藏帧并打印
参数：
vDataGrid   所要打印的DataGrid句柄

备注：
代码中调用如下
btPrint.Attributes.Add("onclick","return PrintDataGrid(document.all('SheetList'))");

SheetList为待打印的DataGrid的ID
*/


function PrintDataGrid(vDataGrid)
{
   PickupHeaderInfo();
   
   document.body.insertAdjacentHTML("beforeEnd","<iframe name=printHiddenFrame width=0 height=0></iframe>");
   
   var doc = printHiddenFrame.document;
   //var preDlg = window.open("PrintList.htm");
   doc.open();
   doc.write("<body onload=\"setTimeout('parent.onprintHiddenFrame()', 0)\">");
   doc.write("<iframe name=printMe width=0 height=0 ></iframe>");
   doc.write("</body>");
    alert("dddd");
   doc.close();
    //var objTable = document.getElementById("RGoods");  
    //objTable.setAttribute("height","100%");
    //objTable.setAttribute("scroll","none"); 
   
   //CreateHtmlReport(preDlg, true);

   CreateHtmlReport(printHiddenFrame.printMe,vDataGrid);
   return false;
    
}
     

/*
目的：在隐藏帧中写入DataGrid的内容，并重写DataGrid的格式
参数：
vHideFrame 隐藏帧的句柄
vDataGrid   所要打印的DataGrid句柄

备注：
*/
function CreateHtmlReport(vHideFrame,vDataGrid)
{
vGridContent = vDataGrid.innerHTML;


// 输出报表头信息及抽取过来的表格
var doc = vHideFrame.document;
doc.open();
doc.write("<html><head>");

doc.write("<style type=\"text/css\" media=\"print\">");
doc.write("input{border-width:0px;}");
doc.write("font{font-size:9px;}");
doc.write("</style>");
doc.write("</head><body>");
doc.write(vHeaderInfo);
doc.write(vGridContent);
doc.write(vTailerInfo);
doc.write("</body></html>");
doc.close();


// 重新设置表格样式
vDataGrid.borderColor = "#000000";
vDataGrid.width = "100%";
vDataGrid.height = "100%";
vDataGrid.style.fontFamily = "Verdana";
vDataGrid.style.fontSize = "9px";
vDataGrid.style.borderRight = "2px solid #000000";
vDataGrid.style.borderTop = "2px solid #000000";
vDataGrid.style.borderLeft = "2px solid #000000";
vDataGrid.style.borderBottom = "2px solid #000000";
vDataGrid.style.borderCollapse = "collapse";


// 重新设置表格头样式
var TBody = vDataGrid.children(0);
TBody.children(0).style.fontWeight = "bold";
TBody.children(0).style.fontSize="9px";
TBody.children(0).bgColor = "#E7E7E7";
// 替换原表格底部的页码信息
var pageInfo = "<td>第 " + ((4 - 3) / 1 + 1) + " 页 / 共 " + "1" + " 页&nbsp;&nbsp;&nbsp;&nbsp;</td>";
}
//创建表头 表尾
function PickupHeaderInfo()
{
try
{
   // 提取报表标题字体大小
   var ReportTitleWithSizeInfo = "<font size='" + "+2" + "'>" + "入库单购买明细" + "</font>"
   var reportDate = "2001-12-21";
   var reportWriter = "杨海飞";
   var nowdate=new Date();
   reportDate = "<b>单位名称</b>：" +nowdate.toLocaleString() + "<br>";
   reportDate +="<b>单据编号</b>：测试而已<br>";  

   // 生成报表头信息
   vHeaderInfo = "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=gb2312\">";
   vHeaderInfo += "<title>详细报表</title></head>" +"<body bgcolor='#FFFFFF' style='color: #000000; font-family: Verdana; font-size:12px; cursor: default'>";
   vHeaderInfo += "<br><p align='center'><b>" + ReportTitleWithSizeInfo + "</b></p>";
   vHeaderInfo += "<p>" + reportDate;
   vHeaderInfo += reportWriter + "</p>";
}
catch (e)
{
   alert("提取报表公共信息失败，打印操作被取消！");
   self.close();
}
}

//下面的脚本来自msdn
// The code by Captain <cerebrum@iname.com>
     // Mead & Company, http://www.meadroid.com/wpm/

// fake print() for IE4.x
if ( !printIsNativeSupport() )
   window.print = printFrame;

// main stuff
function printFrame(frame, onfinish) {
   if ( !frame ) frame = window;

   if ( frame.document.readyState !== "complete" &&
        !confirm("The document to print is not downloaded yet! Continue with printing?") )
   {
     if ( onfinish ) onfinish();
     return;
   }

   if ( printIsNativeSupport() ) {
     /* focus handling for this scope is IE5Beta workaround,
        should be gone with IE5 RTM.
     */
     var focused = document.activeElement; 
     frame.focus();
     frame.self.print();
     if ( onfinish ) onfinish();
     if ( focused && !focused.disabled ) focused.focus();
     return;
   }

   var eventScope = printGetEventScope(frame);
   var focused = document.activeElement;

   window.printHelper = function() {
     execScript("on error resume next: printWB.ExecWB 6, 1", "VBScript");
     printFireEvent(frame, eventScope, "onafterprint");
     printWB.outerHTML = "";
     if ( onfinish ) onfinish();
     if ( focused && !focused.disabled ) focused.focus();
     window.printHelper = null;
   }

   document.body.insertAdjacentHTML("beforeEnd","<object id=\"printWB\" width=0 height=0 \classid=\"clsid:8856F961-340A-11D0-A96B-00C04FD705A2\"></object>");

   printFireEvent(frame, eventScope, "onbeforeprint");
   frame.focus();
   window.printHelper = printHelper;
   setTimeout("window.printHelper()", 0);
}

// helpers
function printIsNativeSupport() {
   var agent = window.navigator.userAgent;
   var i = agent.indexOf("MSIE ")+5;
   return parseInt(agent.substr(i)) >= 5 && agent.indexOf("5.0b1") < 0;
}

function printFireEvent(frame, obj, name) {
   var handler = obj[name];
   switch ( typeof(handler) ) {
     case "string": frame.execScript(handler); break;
     case "function": handler();
   }
}

function printGetEventScope(frame) {
   var frameset = frame.document.all.tags("FRAMESET");
   if ( frameset.length ) return frameset[0];
   return frame.document.body;
}

function onprintHiddenFrame() {
   function onfinish() {
     printHiddenFrame.outerHTML = "";
     if ( window.onprintcomplete ) window.onprintcomplete();
   }
   printFrame(printHiddenFrame.printMe, onfinish);
}

