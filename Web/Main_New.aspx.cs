using System;
using System.Collections.Generic;
using System.Linq;
using ERP.SAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using MIS.Model.View;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    public partial class Main_New : WindowsPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ModulePageList = null;
            if (!IsPostBack)
            {
                LoadCurrentModules();
            }
        }

        /// <summary>
        /// 加载当前模块下的权限页面
        /// </summary>
        /// <param name="childModules"></param>
        protected void LoadCurrentPages(IEnumerable<MenuInfo> childModules)
        {
            LoadChildMenuList(childModules, null);
        }

        protected IList<MenuInfo> ModulePageList
        {
            get
            {
                if (ViewState["ModulePageList"] == null)
                {
                    ViewState["ModulePageList"] = MISService.GetMenuList(CurrentSession.System.ID);//ERP
                }
                return ViewState["ModulePageList"] as IList<MenuInfo>;
            }
            set { ViewState["ModulePageList"] = value; }
        }

        /// <summary>
        /// 加载当前用户模块
        /// </summary>
        protected void LoadCurrentModules()
        {
            var moduleInfos = ModulePageList;
            if (moduleInfos.Count > 0)
            {
                foreach (var moduleInfo in moduleInfos.Where(ent => ent.ParentID == Guid.Empty))
                {
                    var parent_item = new RadMenuItem { Text = moduleInfo.Name, Value = moduleInfo.ID.ToString() };
                    RadMenu_First.Items.Add(parent_item);
                }
                if (RadMenu_First.Items.Count > 0)
                {
                    RadMenu_First.Items[0].Selected = true;
                }
                else
                {
                    Response.Redirect("default.aspx");
                }
                LoadCurrentPages(moduleInfos.Where(ent => ent.ParentID == RadMenu_First.SelectedValue.ToString().ToGuid()));
            }
            else
            {
                RadAjaxManager1.Alert("数据加载失败，请重新登录!");
            }
        }

        protected void LoadChildMenuList(IEnumerable<MenuInfo> menuInfos, RadPanelItem parentPanelItem)
        {
            foreach (MenuInfo menu in menuInfos)
            {
                var m = menu;
                var item = new RadPanelItem { Text = m.Name, Value = m.ID.ToString(), PostBack = false };
                if (string.IsNullOrEmpty(m.PageUrl))
                {
                    var childModules = ModulePageList.Where(ent => ent.ParentID == m.ID).ToList();
                    if (childModules.Any())
                    {
                        LoadChildMenuList(childModules, item);
                    }
                    else
                    {
                        if (m.PageUrl != null)
                        {
                            item.Attributes.Add("onclick", "addTab('" + m.Name + "','" + m.ID.ToString() + "','" + m.PageUrl + "');");
                        }
                    }
                }
                else
                {
                    if (m.PageUrl != string.Empty)
                    {
                        item.Attributes.Add("onclick", "addTab('" + m.Name + "','" + m.ID.ToString() + "','" + m.PageUrl + "');");
                    }
                }
                if (parentPanelItem == null)
                {
                    RadPanelBar_LeftMenu.Items.Add(item);
                }
                else
                {
                    parentPanelItem.Items.Add(item);
                }
            }
        }

        //点击菜单项
        protected void RadMenu_First_OnItemClick(object sender, RadMenuEventArgs e)
        {
            RadPanelBar_LeftMenu.Items.Clear();
            var moduleInfos = ModulePageList;
            LoadCurrentPages(moduleInfos.Where(ent => ent.ParentID == e.Item.Value.ToString().ToGuid()));
        }
    }
}
