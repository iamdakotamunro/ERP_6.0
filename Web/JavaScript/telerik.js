//获取窗口对象
function GetRadWindow() {
    var oWindow = null;
    if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog
    else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow; //IE (and Moz az well)
    return oWindow;
}

//关闭窗口
function CancelWindow() {
    GetRadWindow().Close();
}
//刷新父页面Grid
function Rebind(age) {
    GetRadWindow().BrowserWindow.refreshGrid(age);
}
//关闭窗口并刷新父页面Grid
function CloseAndRebind(age) {
    GetRadWindow().BrowserWindow.refreshGrid(age);
    GetRadWindow().Close();
}

//获取父页面窗口对象
function ParentGetRadWindow() {
    var oWindow = null;
    if (window.parent.radWindow) oWindow = window.parent.radWindow; //Will work in Moz in all cases, including clasic dialog
    else if (window.parent.frameElement.radWindow) oWindow = window.parent.frameElement.radWindow; //IE (and Moz az well)
    return oWindow;
}

//关闭父页面窗口并刷新父页面的父页面Grid
function ParentCloseAndRebind(age) {
    ParentGetRadWindow().BrowserWindow.refreshGrid(age);
    ParentGetRadWindow().Close();
}