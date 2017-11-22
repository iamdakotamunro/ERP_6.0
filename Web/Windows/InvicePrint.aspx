<%@ Page Language="C#" AutoEventWireup="true" Inherits="LonmeShop.AdminWeb.InvicePrint"
    CodeBehind="InvicePrint.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <script type="text/javascript" language="javascript">
        //function doPrint() { 
        //bdhtml=window.document.body.innerHTML; 
        //sprnstr="<!--startprint-->"; 
        //eprnstr="<!--endprint-->"; 
        ///prnhtml=bdhtml.substr(bdhtml.indexOf(sprnstr)+17); 
        //prnhtml=prnhtml.substring(0,prnhtml.indexOf(eprnstr)); 
        //window.document.body.innerHTML=prnhtml; 
        //window.print(); 
        //GetRadWindow().Close();
        //window.location.reload();
        //} 
    </script>

</head>
<body onload="doPrint()">
    <form id="form1" runat="server">
    <div>
        <!--startprint-->
        <div id="div_print">
            <div style="font-size: 20px; letter-spacing: 26px; margin-left: 45px; margin-top: 15px;">
                <asp:Label ID="LabelZip" runat="server" Text="Label"></asp:Label></div>
            <div style="margin-left: 150px; font-size: 22px; margin-top: 62px; margin-right: 10px;
                font-family: 楷体_GB2312">
                <asp:Label ID="LabelAddress" runat="server" Text="Label"></asp:Label></div>
            <div style="margin-left: 190px; font-size: 25px; margin-top: 60px; margin-right: 10px;
                font-family: 楷体_GB2312; font-weight: 600;">
                <asp:Label ID="LabelName" runat="server" Text="Label"></asp:Label>
                （收）</div>
        </div>
        <!--endprint-->
    </div>
    </form>
</body>
</html>
