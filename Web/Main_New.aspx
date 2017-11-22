<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Main_New.aspx.cs" Inherits="ERP.UI.Web.Main_New" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="Stylesheet" type="text/css" href="/App_themes/main_new.css" />
    <script type="text/javascript" src="JavaScript/jquery.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RadScriptManager1" runat="server">
    </rad:RadScriptManager>
    <rad:RadSkinManager ID="rsmSkin" runat="server" Skin="WebBlue">
    </rad:RadSkinManager>
    <div id="header">
        <div style="float: right; width: 320px;">
            <asp:Label ID="PersonnelInfoLabel" runat="server" Text="登陆者：员工姓名"></asp:Label>
            [<a href="ChangePassWord.aspx" style="color: White" target="_parent">修改密码</a>] [<a
                href="QuitLogin.aspx" style="color: White" target="_parent">退出</a>]
        </div>
        <div style="float: left; width: 120px; font-size: 16px; font-weight: bold">
            后台管理系统</div>
        <div style="margin: 0 auto;">
            <rad:RadMenu ID="RadMenu_First" OnItemClick="RadMenu_First_OnItemClick" runat="server"
                Skin="Default" EnableRoundedCorners="true" EnableShadows="true">
            </rad:RadMenu>
        </div>
    </div>
    <div id="main">
        <div id="menu">
            <rad:RadPanelBar Width="140" ID="RadPanelBar_LeftMenu" runat="server" ExpandMode="SingleExpandedItem">
            </rad:RadPanelBar>
        </div>
        <div id="content">
            <rad:RadTabStrip SelectedIndex="1" ID="RadTabStrip_TabList" OnClientTabSelected="OnClientPageTabSelected" MultiPageID="RadMultiPage_ContentPages" ShowBaseLine="True" runat="server" ScrollChildren="true"
                ScrollButtonsPosition="Middle" PerTabScrolling="true" dir="ltr">
                <Tabs>
                    <rad:RadTab Text="管理主页" Selected="true" Value="master" />
                </Tabs>
            </rad:RadTabStrip>
            <rad:RadMultiPage CssClass="RadMultiPage" ID="RadMultiPage_ContentPages" runat="server"
                SelectedIndex="1">
                <rad:RadPageView Selected="true" ID="RadPageView_DefaultPage" runat="server">
                    <iframe style="z-index: 2; visibility: visible; width: 100%; height: 800px;" 
                        frameborder="0" height="100%" id="Stage" name="Stage"></iframe>
                </rad:RadPageView>
            </rad:RadMultiPage>
        </div>
    </div>
    <div id="footer" style="display: none;">
        Power by 上海可得光学有限公司 Version 1.0
    </div>
    <!--AJAX-->
    <rad:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RadMenu_First" EventName="RadMenu_First_OnItemClick">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadPanelBar_LeftMenu" LoadingPanelID="RadAjaxLoadingPanel1" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server">
    </rad:RadAjaxLoadingPanel>
    </form>
    <script type="text/javascript" language="javascript">
    var pageTabTripWidth;
    var childTabWidthTotal;
    $(function () {
        pageTabTripWidth = 0;
        childTabWidthTotal = 0;
        layoutInit();
        screen.autoResize();
        $(window).resize(function () { screen.autoResize(); });
        $("iframe").css("border", "0");
    });

    function layoutInit() {
        var headerHt = $("#header").outerHeight() + 5;
        var footHt = 0; 
        var tabHt = $("#<%= RadTabStrip_TabList.ClientID %>").outerHeight();
        screen.TOPHEIGHT = headerHt;
        screen.BOTTOMHEIGHT = footHt;
        screen.TABHEIGHT = tabHt;
        pageTabTripWidth = $("#RadTabStrip_TabList").width();
    }

    var screen = {
        autoResize: function() {
            var viewHeight = screen.WindowHeight();
            if (viewHeight < screen.MINHEIGHT) {
                viewHeight = screen.MINHEIGHT;
            }
            var contentHeight = viewHeight - (screen.TOPHEIGHT + screen.BOTTOMHEIGHT);
            $('#content').height(contentHeight);
            var radMultiPage = $('.RadMultiPage');
            radMultiPage.height(contentHeight - (screen.TABHEIGHT + 10));
            radMultiPage.find('iframe').height(radMultiPage.height() - 26);
        },
        MINHEIGHT: 0,
        TOPHEIGHT: 0,
        BOTTOMHEIGHT: 0,
        TABHEIGHT: 0,
        WindowHeight: function() {
            //var bw = screen.Browser();
            if (document.documentElement.clientHeight) {
                //alert("document.documentElement.clientHeight");
                var ht = document.documentElement.clientHeight;
                return (ht - 6);
            }
            if (window.innerHeight) {
                //alert("window.innerHeight");
                //if (bw == "ma")
                return (window.innerHeight - 20);
            }
            if (document.body.clientHeight) {
                //alert("document.body.clientHeight");
                //if (bw == "ma")
                return (document.body.clientHeight - 20);
            }
        },
        Browser: function() {
            if ($.browser.msie) {
                return "ie";
            }
            if ($.browser.opera) {
                return "op";
            }
            if ($.browser.mozilla) {
                return "ma";
            }
            if ($.browser.chrome) {
                return "cm";
            }
            return "null";
        }
    };

    //添加TAB
    function addTab(text, value, url) {
        var tabStrip = $find("<%= RadTabStrip_TabList.ClientID %>");
        var tab = tabStrip.findTabByValue(value);
        if (!tab) {
            if ((pageTabTripWidth - childTabWidthTotal) > 90) {
                tab = new Telerik.Web.UI.RadTab();
                tabStrip.get_tabs().add(tab);
                tab.set_value(value);
                tab.set_text(text);
                childTabWidthTotal += $(tab.get_element()).width();
            }
            else {
                alert("打开的管理窗口过多！");
                return false;
            }
        }
        tab.set_selected(true);
        addPageView(tab, url);
        return false;
    }

    //添加pageView
    function addPageView(tab, url) {
        var multiPages = $find("<%= RadMultiPage_ContentPages.ClientID %>");
        var id = tab.get_value();
        var pageView = multiPages.findPageViewByID(id);
        if (!pageView) {
            //debugger;
            pageView = new Telerik.Web.UI.RadPageView();
            multiPages.get_pageViews().add(pageView);
            pageView.set_id(id);
            tab.pageViewId = id;
            pageView.get_element().appendChild(addChildTopMenu(id));
            pageView.get_element().appendChild(addIframe(url));
        }
        pageView.set_selected(true);
        return false;
    }

    function addChildTopMenu(tabValue) {
        var childTopMenu = document.createElement("div");
    	$(childTopMenu).attr("class", "childTopMenu");
    	$(childTopMenu).html("<span class='rightTabTooltipMenu'><a href='javascript:void(0);' title='刷新当前管理页面' onclick=\"loadIframe('" + tabValue + "'); return false;\"><img src='/App_Themes/refresh.png' align='absmiddle' border=0 /></a>&nbsp;<a href='javascript:void(0);' title='关闭当前管理页面' onclick=\"removeTab('" + tabValue + "');return false;\"><img src='/App_Themes/close_over.png' align='absmiddle' border=0 /></a></span>");
        return childTopMenu;
    }

    //添加iframe
    function addIframe(url) {
        var iframe = document.createElement("iframe");
        $(iframe).attr("scrolling", "auto");
        $(iframe).attr("frameborder", "0");
        $(iframe).attr("width", "100%");
        $(iframe).attr("height", $('.RadMultiPage').height() - 22);
        $(iframe).attr("src", url);
        return iframe;
    }

    //选择TAB
    function OnClientPageTabSelected(sender, eventArgs) {
        var tab = eventArgs.get_tab();
        if (tab) {
            tab.select();
            var id = tab.pageViewId;
            var multiPages = $find("<%= RadMultiPage_ContentPages.ClientID %>");
            var pageView = multiPages.findPageViewByID(id);
            if (pageView) {
                pageView.select();
            }
        }
    }

    //删除TAB
    function removeTab(tabValue) {
        var tabStrip = $find("<%=RadTabStrip_TabList.ClientID %>");
        //debugger;
        var tab = tabStrip.findTabByValue(tabValue);
        if (tab) {

            childTabWidthTotal = (childTabWidthTotal - $(tab.get_element()).width());

            var prevIndex = tab.get_index() - 1;
            if (prevIndex < 0) { prevIndex = 0; }
            tabStrip.set_selectedIndex(prevIndex);

            //删除pageView和删除tab
            removePageView(tab.pageViewId);
            tabStrip.get_tabs().remove(tab);
        }
    }

    //删除pageView
    function removePageView(pageViewId) {
        var multiPages = $find("<%=RadMultiPage_ContentPages.ClientID %>");
        var pageView = multiPages.findPageViewByID(pageViewId);
        if (pageView) {
            multiPages.get_pageViews().remove(pageView);
        }
    }

    function loadIframe(tabValue) {
        var multiPages = $find("<%=RadMultiPage_ContentPages.ClientID %>");
        var pageView = multiPages.findPageViewByID(tabValue);
        if (pageView) {
            var pageViewElement = pageView.get_element();
            var iframe = $(pageViewElement).find("iframe");
            var url = iframe.attr("src");
            iframe.attr("src", url);
        }
    }
    </script>

</body>
</html>
