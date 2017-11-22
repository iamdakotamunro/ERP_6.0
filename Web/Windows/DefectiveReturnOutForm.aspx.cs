using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.Model.Goods;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.SAL.WMS;
using ERP.UI.Web.Common;
using OperationLog.Core;
using Telerik.Web.UI;
using HostingFilialeAuth = ERP.SAL.WMS.HostingFilialeAuth;
using StorageAuth = ERP.SAL.WMS.StorageAuth;
using WebControl = ERP.UI.Web.Common.WebControl;
using System.Text.RegularExpressions;
using ERP.BLL.Implement.Goods;
using ERP.UI.Web.Base;

namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// 添加售后退货出库
    /// </summary>
    public partial class DefectiveReturnOutForm : WindowsPage
    {
        static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        static readonly GoodsClassManager _goodsClassManager = new GoodsClassManager(_goodsCenterSao);
        static readonly CodeManager _code = new CodeManager();
        static readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Write);
        static readonly StorageManager _storageManager = new StorageManager();
        static readonly IPurchaseSet _purchaseSet = new PurchaseSet(GlobalConfig.DB.FromType.Write);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txt_DateCreated.Text = DateTime.Now.ToString("yyyy/MM/dd");
                txt_Transactor.Text = CurrentSession.Personnel.Get().RealName;
                //绑定仓库
                BindWarehouse();
                BindGoodsClass();
            }
        }

        #region 下拉框选择事件
        /// <summary>
        /// 入库仓储Changed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbInStockOnSelectedIndexChanged(object sender, EventArgs e)
        {
            Guid warehouseId = string.IsNullOrEmpty(RCB_Warehouse.SelectedValue)
                ? Guid.Empty
                : new Guid(RCB_Warehouse.SelectedValue);

            //绑定入库储
            var list = new List<StorageAuth>();
            var warehouseAuth = CurrentSession.Personnel.WarehouseList.FirstOrDefault(p => p.WarehouseId == warehouseId);
            if (warehouseAuth != null && warehouseAuth.Storages != null)
            {
                list.AddRange(warehouseAuth.Storages.Where(storages => storages.StorageType == (int)StorageAuthType.S || storages.StorageType == (int)StorageAuthType.H));
                list.Insert(0, new StorageAuth { StorageType = 0, StorageTypeName = "" });
            }

            RCB_StorageAuth.DataSource = list;
            RCB_StorageAuth.DataBind();

            //清空物流配送公司下拉框
            RCB_HostingFilialeAuth.DataSource = new List<HostingFilialeAuth>();
            RCB_HostingFilialeAuth.DataBind();

            GetRgGoodsData();
            if (InDetailList.Count > 0)
            {
                InDetailList = new List<StorageRecordDetailInfo>();
                RGGoods.Rebind();
            }
        }

        /// <summary>
        /// 入库储Changed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbStorageAuthOnSelectedIndexChanged(object sender, EventArgs e)
        {
            //仓库id
            Guid warehouseId = string.IsNullOrEmpty(RCB_Warehouse.SelectedValue) ? Guid.Empty : new Guid(RCB_Warehouse.SelectedValue);
            //储位id
            byte storageType = string.IsNullOrEmpty(RCB_StorageAuth.SelectedValue) ? default(byte) : byte.Parse(RCB_StorageAuth.SelectedValue);

            if (storageType == (int)StorageAuthType.S)
            {
                lbl_str.Text = "次品";
            }
            else if (storageType == (int)StorageAuthType.H)
            {
                lbl_str.Text = "坏件";
            }
            else
            {
                lbl_str.Text = string.Empty;
            }

            //绑定物流配送公司
            var list = new List<HostingFilialeAuth>();
            var warehouseAuth = CurrentSession.Personnel.WarehouseList.FirstOrDefault(p => p.WarehouseId == warehouseId);
            if (warehouseAuth != null)
            {
                if (warehouseAuth.Storages != null)
                {
                    var storageAuth = warehouseAuth.Storages.FirstOrDefault(p => p.StorageType == storageType);
                    if (storageAuth != null)
                    {
                        list.AddRange(storageAuth.Filiales);
                        list.Insert(0, new HostingFilialeAuth { HostingFilialeId = Guid.Empty, HostingFilialeName = "" });
                    }
                }
            }
            RCB_HostingFilialeAuth.DataSource = list;
            RCB_HostingFilialeAuth.DataBind();
        }


        /// <summary>
        /// 入库储Changed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbFilialeAuthOnSelectedIndexChanged(object sender, EventArgs e)
        {
            //仓库id
            Guid warehouseId = string.IsNullOrEmpty(RCB_Warehouse.SelectedValue) ? Guid.Empty : new Guid(RCB_Warehouse.SelectedValue);
            //储位id
            byte storageType = string.IsNullOrEmpty(RCB_StorageAuth.SelectedValue) ? default(byte) : byte.Parse(RCB_StorageAuth.SelectedValue);

            Guid hostFilialeId = string.IsNullOrEmpty(RCB_HostingFilialeAuth.SelectedValue) ? Guid.Empty : new Guid(RCB_HostingFilialeAuth.SelectedValue);

            BindRgGoodsAll(warehouseId, storageType, hostFilialeId);
        }

        #endregion

        /// <summary> 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            if (ValidateData())
            {
                BindRgGoodsAll(new Guid(RCB_Warehouse.SelectedValue), byte.Parse(RCB_StorageAuth.SelectedValue), new Guid(RCB_HostingFilialeAuth.SelectedValue));
            }
            else
            {
                if (InDetailList.Count > 0)
                {
                    InDetailList = new List<StorageRecordDetailInfo>();
                    RGGoods.Rebind();
                }
            }
        }

        private bool ValidateData()
        {
            if (string.IsNullOrEmpty(RCB_Warehouse.SelectedValue) || RCB_Warehouse.SelectedValue == Guid.Empty.ToString())
            {
                RAM.Alert("请选择仓库！");
                return false;
            }
            if (string.IsNullOrEmpty(RCB_StorageAuth.SelectedValue) || RCB_StorageAuth.SelectedValue == "0")
            {
                RAM.Alert("请选择储位！");
                return false;
            }
            if (string.IsNullOrEmpty(RCB_HostingFilialeAuth.SelectedValue) || RCB_HostingFilialeAuth.SelectedValue == Guid.Empty.ToString())
            {
                RAM.Alert("请选择物流配送公司！");
                return false;
            }
            return true;
        }

        /// <summary>删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RgGoods_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = (GridEditableItem)e.Item;
            var realGoodsId = new Guid(editedItem.GetDataKeyValue("RealGoodsId").ToString());
            var unitPrice = editedItem.GetDataKeyValue("UnitPrice").ToString();
            var goodsStockInfo = InDetailList.FirstOrDefault(w => w.RealGoodsId == realGoodsId && w.UnitPrice == Convert.ToDecimal(unitPrice));
            if (goodsStockInfo != null)
            {
                InDetailList.Remove(goodsStockInfo);
                RGGoods.Rebind();
            }
        }

        protected void RgGoods_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            Lab_TotalNumber.Text = InDetailList.Sum(p => p.Quantity).ToString();
            RGGoods.DataSource = InDetailList.OrderBy(w => w.GoodsName).ThenBy(w => w.Specification).ToList();
        }

        /// <summary>保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_InsterStock(object sender, EventArgs e)
        {
            if (!CanSubmit())
            {
                RAM.Alert("请不要重复提交!");
                return;
            }
            ExecuteSubmit((ctx) =>
            {
                if (string.IsNullOrEmpty(RCB_Warehouse.SelectedValue) ||
                RCB_Warehouse.SelectedValue == Guid.Empty.ToString())
                {
                    RAM.Alert("请选择出库仓储！");
                    return;
                }
                if (string.IsNullOrEmpty(RCB_StorageAuth.SelectedValue) ||
                    RCB_StorageAuth.SelectedValue == "0")
                {
                    RAM.Alert("请选择出库仓储！");
                    return;
                }
                if (string.IsNullOrEmpty(RCB_HostingFilialeAuth.SelectedValue) ||
                    RCB_HostingFilialeAuth.SelectedValue == Guid.Empty.ToString())
                {
                    RAM.Alert("请选择物流配送公司！");
                    return;
                }
                //if (string.IsNullOrEmpty(RCB_CompanyId.SelectedValue) ||
                //    RCB_CompanyId.SelectedValue == Guid.Empty.ToString())
                //{
                //    RAM.Alert("请选择供应商！");
                //    return;
                //}
                //出入库详细
                var goodsStockList = GetRgGoodsData();
                if (goodsStockList == null || goodsStockList.Count == 0)
                {
                    RAM.Alert("商品数量不能为0！");
                    return;
                }
                var infos = new List<String>();
                var unitPriceInfo = new List<String>();
                var quantityInfo = new List<String>();
                foreach (var list in goodsStockList)
                {
                    if (list.UnitPrice < 0)
                    {
                        unitPriceInfo.Add(list.GoodsName + " " + list.Specification);
                    }
                    var quantity = InDetailList.First(ent => ent.RealGoodsId == list.RealGoodsId).Quantity;
                    if (list.Quantity > quantity)
                    {
                        quantityInfo.Add(list.GoodsName + " " + list.Specification);
                    }
                }
                if (unitPriceInfo.Count > 0)
                {
                    infos.Add(" 单价为负数的商品：" + String.Join("、", unitPriceInfo));
                }
                if (quantityInfo.Count > 0)
                {
                    infos.Add(" 退货数大于原有退货数的商品：" + String.Join("、", quantityInfo));
                }
                if (infos.Count > 0)
                {
                    RAM.Alert(String.Join("\r\n", infos));
                    return;
                }

                if (!GetPowerOperationPoint("AfterSaleOutEditUnitPrice"))
                {
                    if (goodsStockList.Any(list => InDetailList.First(ent => ent.GoodsId == list.GoodsId).UnitPrice > list.UnitPrice))
                    {
                        RAM.Alert("当前无权限单价只能修改为大于或等于原单价！");
                        ctx.SetFail();
                        return;
                    }
                }

                //仓库id
                var warehouseId = new Guid(RCB_Warehouse.SelectedValue);

                //把商品详细数据根据供应商分组，一个供应商对应生成一个出库单
                var dic = goodsStockList.GroupBy(ent => ent.ThirdCompanyID).ToDictionary(k => k.Key, v => v.ToList());

                #region 公共属性
                //储位id
                int storageType = string.IsNullOrEmpty(RCB_StorageAuth.SelectedValue) ? default(int) : int.Parse(RCB_StorageAuth.SelectedValue);
                //物流配送公司
                Guid hostingFilialeId = string.IsNullOrEmpty(RCB_HostingFilialeAuth.SelectedValue) ? Guid.Empty : new Guid(RCB_HostingFilialeAuth.SelectedValue);
                //入库单备注
                string storageRecordDescription = !string.IsNullOrWhiteSpace(txt_Description.Text) ? txt_Description.Text.Trim() : "无";
                var personnelInfo = CurrentSession.Personnel.Get();
                string description = string.Format("[售后退货出库;出库人:{0};出库单备注:{1};{2}]", personnelInfo.RealName, storageRecordDescription, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                string transactor = txt_Transactor.Text;
                #endregion

                //已经存在的销售退货单据
                var existsList = _storageRecordDao.GetStorageRecordList(warehouseId, hostingFilialeId, (int)StorageRecordType.AfterSaleOut, (int)StorageRecordState.WaitAudit, storageType);
                foreach (var item in dic)
                {
                    if (item.Value == null) continue;
                    var stockId = Guid.NewGuid();
                    //总金额
                    decimal accountReceivable = 0;
                    //总数量
                    double subtotalQuantity = 0;
                    string tradeCode;
                    var info = existsList.FirstOrDefault(act => act.ThirdCompanyID == item.Key);
                    if (info != null && !info.StockValidation)
                    {
                        stockId = info.StockId;
                        tradeCode = info.TradeCode;
                        accountReceivable = info.AccountReceivable;
                        subtotalQuantity = (double)info.SubtotalQuantity;
                        var details = _storageRecordDao.GetStorageRecordDetailListByStockId(info.StockId);
                        foreach (var storageRecordDetailInfo in item.Value)
                        {
                            accountReceivable += Convert.ToDecimal(storageRecordDetailInfo.Quantity) * storageRecordDetailInfo.UnitPrice;
                            subtotalQuantity += Convert.ToDouble(storageRecordDetailInfo.Quantity);
                            var detailInfo = details.FirstOrDefault(act => act.RealGoodsId == storageRecordDetailInfo.RealGoodsId && act.UnitPrice == storageRecordDetailInfo.UnitPrice && act.GoodsCode == storageRecordDetailInfo.GoodsCode && act.GoodsName == storageRecordDetailInfo.GoodsName && act.Units == storageRecordDetailInfo.Units);
                            if (detailInfo != null)
                            {
                                detailInfo.Quantity += storageRecordDetailInfo.Quantity;
                            }
                            else
                            {
                                details.Add(storageRecordDetailInfo);
                            }
                        }
                        info.AccountReceivable = accountReceivable;
                        info.SubtotalQuantity = (decimal)subtotalQuantity;
                        _storageManager.UpdateStorageRecordAndStorageRecordDetail(info, details);
                    }
                    else
                    {
                        foreach (StorageRecordDetailInfo goodsStockInfo in item.Value)
                        {
                            goodsStockInfo.StockId = stockId;
                            accountReceivable += Convert.ToDecimal(goodsStockInfo.Quantity) * goodsStockInfo.UnitPrice;
                            subtotalQuantity += Convert.ToDouble(goodsStockInfo.Quantity);
                        }
                        tradeCode = _code.GetCode(CodeType.SO);
                        //出入库
                        var stockInfo = new StorageRecordInfo
                        {
                            StockId = stockId,
                            FilialeId = hostingFilialeId,
                            WarehouseId = warehouseId,
                            ThirdCompanyID = item.Key,
                            RelevanceFilialeId = Guid.Empty,
                            RelevanceWarehouseId = Guid.Empty,
                            AccountReceivable = accountReceivable,
                            DateCreated = DateTime.Now,
                            Description = description,
                            LinkTradeCode = string.Empty,
                            LinkTradeID = Guid.Empty,
                            StockState = (int)StorageRecordState.WaitAudit,
                            StockType = (int)StorageRecordType.AfterSaleOut,
                            StockValidation = false,
                            SubtotalQuantity = (decimal)subtotalQuantity,
                            TradeCode = tradeCode,
                            Transactor = transactor,
                            IsOut = true,
                            StorageType = storageType,
                            LinkTradeType = (int)StorageRecordLinkTradeType.Other
                        };
                        _storageManager.NewInsertStockAndGoods(stockInfo, item.Value);
                    }

                    //自动退货添加操作记录添加
                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, stockId, tradeCode,
                        OperationPoint.StorageOutManager.AutoReturnGoods.GetBusinessInfo(), string.Empty);
                }
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            });
        }

        /// <summary>获得入库商品详细
        /// </summary>
        /// <returns></returns>
        private List<StorageRecordDetailInfo> GetRgGoodsData()
        {
            var list = new List<StorageRecordDetailInfo>();
            for (int i = 0; i < RGGoods.Items.Count; i++)
            {
                var info = new StorageRecordDetailInfo();
                //退货数
                var quantitytxt = RGGoods.Items[i]["Quantity"].FindControl("TB_Quantity") as TextBox;
                var unitPricetxt = RGGoods.Items[i]["UnitPrice"].FindControl("lbl_UnitPrice") as TextBox;

                var goodsId = RGGoods.Items[i]["GoodsId"].Text;
                var goodsCode = RGGoods.Items[i]["GoodsCode"].Text;
                var goodsName = RGGoods.Items[i]["GoodsName"].Text;
                var thirdCompanyId = RGGoods.Items[i].GetDataKeyValue("ThirdCompanyID").ToString();
                var specification = RGGoods.Items[i]["Specification"].Text;
                var unitPrice = unitPricetxt == null ? 0 : decimal.Parse(unitPricetxt.Text);

                var quantity = quantitytxt == null ? 0 : int.Parse(quantitytxt.Text);
                var realGoodsId = new Guid(RGGoods.Items[i].GetDataKeyValue("RealGoodsId").ToString());

                info.GoodsId = new Guid(goodsId);
                info.Specification = specification;
                info.Quantity = quantity;
                info.UnitPrice = unitPrice;
                info.GoodsName = goodsName;
                info.GoodsCode = goodsCode;
                info.RealGoodsId = realGoodsId;
                info.Description = string.Empty;
                info.NonceWarehouseGoodsStock = 0;
                info.ThirdCompanyID = new Guid(thirdCompanyId);
                list.Add(info);
            }
            return list;
        }

        /// <summary>绑定数据源
        /// </summary>
        private void BindRgGoods(Guid warehouseId, byte storageType, Guid hostingFilialeId, Guid realGoodsId, String goodsName, String sku)
        {
            var autoReturnGoodsRequest = new AutoReturnGoodsRequestDTO
            {
                WarehouseId = warehouseId,
                StorageType = storageType,
                HostingFilialeId = hostingFilialeId
            };
            var dicRealGoodsIdAndCount = WMSSao.CalculateAutoReturnGoods(autoReturnGoodsRequest);
            if (dicRealGoodsIdAndCount == null || dicRealGoodsIdAndCount.Count <= 0)
            {
                RAM.Alert("该仓储下没有需退货商品！");
                InDetailList = new List<StorageRecordDetailInfo>();
                RGGoods.Rebind();
                return;
            }
            if (InDetailList.Any(ent => ent.RealGoodsId == realGoodsId))
            {
                RAM.Alert(string.Format("该商品:{0},{1} 已经存在该列表中！", goodsName, sku));
                return;
            }
            var goodsStockList = InDetailList;
            //排除未完成的销售退货数据
            var exists = _storageRecordDao.GetReturnRealGoods(warehouseId, (int)StorageRecordType.AfterSaleOut, storageType, new List<int> { (int)StorageRecordState.WaitAudit, (int)StorageRecordState.Refuse, (int)StorageRecordState.Approved }, realGoodsId);
            //var cancel = _storageRecordDao.GetCancelRealGoods(warehouseId, (int)StorageRecordType.AfterSaleOut, storageType,(int)StorageRecordState.Canceled, realGoodsId);
            //需要售后退货的商品
            var dicRealGoodsIdAndQuantity = dicRealGoodsIdAndCount.Where(p => p.Key == realGoodsId && p.Value > (exists.ContainsKey(p.Key) ? Math.Abs(exists[p.Key]) : 0)).ToDictionary(p => p.Key, p => p.Value - (exists.ContainsKey(p.Key) ? Math.Abs(exists[p.Key]) : 0));
            if (dicRealGoodsIdAndQuantity.Count == 0)
            {
                RAM.Alert(string.Format("该商品:{0},{1} 不是需退货商品！", goodsName, sku));
            }
            var realGoodsIdList = dicRealGoodsIdAndQuantity.Select(p => p.Key).ToList();
            //根据子商品Id获取主商品信息
            var dicRealGoodsIdAndGoodsInfo = _goodsCenterSao.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(realGoodsIdList);
            //根据子商品Id获取子商品的信息
            var childGoodsInfoList = _goodsCenterSao.GetStockDeclareGridList(realGoodsIdList).ToList();
            var goodsIdList = childGoodsInfoList.Select(p => p.GoodsId).Distinct().ToList();
            //根据仓库Id获取供应商Id
            var dicGoodsIdAndCompanyId = _purchaseSet.GetCompanyIdByWarehouseId(warehouseId, hostingFilialeId);

            var companyIdList = dicGoodsIdAndCompanyId.Where(p => goodsIdList.Contains(p.Key)).Select(p => p.Value).ToList();
            //获取商品的最后一次进货价信息
            var goodsPurchaseLastPriceInfoList = _storageRecordDao.GetGoodsPurchaseLastPriceInfoByWarehouseId(warehouseId);
            goodsPurchaseLastPriceInfoList = goodsPurchaseLastPriceInfoList.Where(p => goodsIdList.Contains(p.GoodsId) && companyIdList.Contains(p.ThirdCompanyId)).ToList();

            foreach (var item in dicRealGoodsIdAndQuantity)
            {
                //根据子商品Id获取主商品信息
                var goodsInfo = dicRealGoodsIdAndGoodsInfo.ContainsKey(item.Key) ? dicRealGoodsIdAndGoodsInfo[item.Key] : new GoodsInfo();
                if (childGoodsInfoList.All(ent => ent.RealGoodsId != item.Key))
                    continue;
                //根据子商品Id获取子商品的信息
                var childGoodsInfo = childGoodsInfoList.First(p => p.RealGoodsId.Equals(item.Key));
                //根据商品id获取供应商
                var companyId = dicGoodsIdAndCompanyId.ContainsKey(childGoodsInfo.GoodsId) ? dicGoodsIdAndCompanyId[childGoodsInfo.GoodsId] : Guid.Empty;

                decimal unitPrice = 0;
                if (goodsPurchaseLastPriceInfoList.Count > 0)
                {
                    var goodsPurchaseLastPriceInfo = goodsPurchaseLastPriceInfoList.FirstOrDefault(p => p.GoodsId.Equals(childGoodsInfo.GoodsId) && p.ThirdCompanyId.Equals(companyId));
                    unitPrice = goodsPurchaseLastPriceInfo != null ? goodsPurchaseLastPriceInfo.UnitPrice : 0;
                }

                var goodsStockInfo = new StorageRecordDetailInfo
                {
                    StockId = Guid.Empty,
                    ThirdCompanyID = companyId,
                    GoodsId = goodsInfo.GoodsId,
                    RealGoodsId = childGoodsInfo.RealGoodsId,
                    GoodsName = goodsInfo.GoodsName,
                    GoodsCode = goodsInfo.GoodsCode,
                    Specification = childGoodsInfo.Specification,
                    Units = goodsInfo.Units,
                    UnitPrice = unitPrice,
                    Quantity = item.Value,
                    NonceWarehouseGoodsStock = 0,
                    Description = string.Empty
                };
                if (goodsStockList.Count(w => w.RealGoodsId == goodsStockInfo.RealGoodsId && w.UnitPrice == goodsStockInfo.UnitPrice) == 0)
                {
                    goodsStockList.Add(goodsStockInfo);
                }
            }
            InDetailList = goodsStockList;
            RGGoods.Rebind();
        }

        private void BindRgGoodsAll(Guid warehouseId, byte storageType, Guid hostingFilialeId)
        {
            var autoReturnGoodsRequest = new AutoReturnGoodsRequestDTO
            {
                WarehouseId = warehouseId,
                StorageType = storageType,
                HostingFilialeId = hostingFilialeId
            };
            var dicRealGoodsIdAndCount = WMSSao.CalculateAutoReturnGoods(autoReturnGoodsRequest);
            if (dicRealGoodsIdAndCount == null || dicRealGoodsIdAndCount.Count <= 0)
            {
                RAM.Alert("该仓储下没有需退货商品！");
                InDetailList = new List<StorageRecordDetailInfo>();
                RGGoods.Rebind();
                return;
            }

            var goodsStockList = new List<StorageRecordDetailInfo>();
            //排除未完成的销售退货数据
            var exists = _storageRecordDao.GetAllReturnRealGoods(warehouseId, (int)StorageRecordType.AfterSaleOut, storageType, new List<int> { (int)StorageRecordState.WaitAudit, (int)StorageRecordState.Refuse, (int)StorageRecordState.Approved });
            //需要售后退货的商品
            var dicRealGoodsIdAndQuantity = dicRealGoodsIdAndCount.Where(p => p.Value > (exists.ContainsKey(p.Key) ? Math.Abs(exists[p.Key]) : 0)).ToDictionary(p => p.Key, p => p.Value - (exists.ContainsKey(p.Key) ? Math.Abs(exists[p.Key]) : 0));
            var realGoodsIdList = dicRealGoodsIdAndQuantity.Select(p => p.Key).ToList();
            //根据子商品Id获取主商品信息
            var dicRealGoodsIdAndGoodsInfo = _goodsCenterSao.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(realGoodsIdList);
            //根据子商品Id获取子商品的信息
            var childGoodsInfoList = _goodsCenterSao.GetStockDeclareGridList(realGoodsIdList).ToList();
            var goodsIdList = childGoodsInfoList.Select(p => p.GoodsId).Distinct().ToList();
            //根据仓库Id获取供应商Id
            var dicGoodsIdAndCompanyId = _purchaseSet.GetCompanyIdByWarehouseId(warehouseId, hostingFilialeId);
            var companyIdList = dicGoodsIdAndCompanyId.Where(p => goodsIdList.Contains(p.Key)).Select(p => p.Value).ToList();
            //获取商品的最后一次进货价信息
            var goodsPurchaseLastPriceInfoList = _storageRecordDao.GetGoodsPurchaseLastPriceInfoByWarehouseId(warehouseId);
            goodsPurchaseLastPriceInfoList = goodsPurchaseLastPriceInfoList.Where(p => goodsIdList.Contains(p.GoodsId) && companyIdList.Contains(p.ThirdCompanyId)).ToList();

            foreach (var item in dicRealGoodsIdAndQuantity)
            {
                //根据子商品Id获取主商品信息
                var goodsInfo = dicRealGoodsIdAndGoodsInfo.ContainsKey(item.Key) ? dicRealGoodsIdAndGoodsInfo[item.Key] : new GoodsInfo();
                if (childGoodsInfoList.All(ent => ent.RealGoodsId != item.Key))
                    continue;
                //根据子商品Id获取子商品的信息
                var childGoodsInfo = childGoodsInfoList.First(p => p.RealGoodsId.Equals(item.Key));
                //根据商品id获取供应商
                var companyId = dicGoodsIdAndCompanyId.ContainsKey(childGoodsInfo.GoodsId) ? dicGoodsIdAndCompanyId[childGoodsInfo.GoodsId] : Guid.Empty;

                decimal unitPrice = 0;
                if (goodsPurchaseLastPriceInfoList.Count > 0)
                {
                    var goodsPurchaseLastPriceInfo = goodsPurchaseLastPriceInfoList.FirstOrDefault(p => p.GoodsId.Equals(childGoodsInfo.GoodsId) && p.ThirdCompanyId.Equals(companyId));
                    unitPrice = goodsPurchaseLastPriceInfo != null ? goodsPurchaseLastPriceInfo.UnitPrice : 0;
                }

                var goodsStockInfo = new StorageRecordDetailInfo
                {
                    StockId = Guid.Empty,
                    ThirdCompanyID = companyId,
                    GoodsId = goodsInfo.GoodsId,
                    RealGoodsId = childGoodsInfo.RealGoodsId,
                    GoodsName = goodsInfo.GoodsName,
                    GoodsCode = goodsInfo.GoodsCode,
                    Specification = childGoodsInfo.Specification,
                    Units = goodsInfo.Units,
                    UnitPrice = unitPrice,
                    Quantity = item.Value,
                    NonceWarehouseGoodsStock = 0,
                    Description = string.Empty
                };
                goodsStockList.Add(goodsStockInfo);
            }
            InDetailList = goodsStockList;
            InDetailListAll = InDetailList;
            RGGoods.Rebind();
        }

        /// <summary>商品清单数据源
        /// </summary>
        private IList<StorageRecordDetailInfo> InDetailList
        {
            get
            {
                if (ViewState["InDetailList"] == null)
                    return new List<StorageRecordDetailInfo>();
                return (IList<StorageRecordDetailInfo>)ViewState["InDetailList"];
            }
            set { ViewState["InDetailList"] = value; }
        }


        private IList<StorageRecordDetailInfo> InDetailListAll
        {
            get
            {
                if (ViewState["InDetailListAll"] == null)
                    return new List<StorageRecordDetailInfo>();
                return (IList<StorageRecordDetailInfo>)ViewState["InDetailListAll"];
            }
            set { ViewState["InDetailListAll"] = value; }
        }
        //选择分类
        protected void GoodsClass_SelectedIndexChanged(object source, EventArgs e)
        {
            HFSonGoods.Value = "";
            Guid goodsClass = Guid.Empty;
            if (!string.IsNullOrEmpty(RCB_GoodsClass.SelectedValue))
            {
                goodsClass = new Guid(RCB_GoodsClass.SelectedValue);
            }

            //根据分类包含产品生成表行
            IList<GoodsInfo> goodsInfoList = _goodsCenterSao.GetGoodsInfoListSimpleByClassId(goodsClass, string.Empty);
            var tempList = GoodsList;
            foreach (GoodsInfo goodsInfo in goodsInfoList)
            {
                var fieldList = _goodsCenterSao.GetFieldDetailByGoodsId(goodsInfo.GoodsId).ToList();
                var atigmiaDic = new List<FieldInfo>();
                var axialsDic = new List<FieldInfo>();
                var luminosityDic = new List<FieldInfo>();

                foreach (FieldInfo fieldInfo in fieldList)
                {
                    var childFieldList = new List<FieldInfo> { new FieldInfo { FieldId = fieldInfo.FieldId, FieldValue = "全部" } };
                    childFieldList.AddRange(fieldInfo.ChildFields.OrderBy(w => w.FieldValue).ToList());
                    switch (fieldInfo.FieldName)
                    {
                        case "光度":
                            luminosityDic = childFieldList;
                            break;
                        case "散光":
                            atigmiaDic = childFieldList;
                            break;
                        case "轴位":
                            axialsDic = childFieldList;
                            break;
                    }
                }
                if (tempList.All(act => act.GoodsId != goodsInfo.GoodsId))
                {
                    tempList.Add(new SearchGoodsInfo
                    {
                        Astigmias = atigmiaDic,
                        Axialss = axialsDic,
                        GoodsId = goodsInfo.GoodsId,
                        GoodsCode = goodsInfo.GoodsCode,
                        GoodsName = goodsInfo.GoodsName,
                        Luminositys = luminosityDic
                    });
                }
            }
            GoodsList = tempList;
            RGSelectGoods.Rebind();
        }

        //下拉选择商品
        protected void Goods_SelectedIndexChanged(object obj, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(RCB_Goods.SelectedValue))
            {
                HFSonGoods.Value = "";
                var goodsId = new Guid(RCB_Goods.SelectedValue);
                GoodsInfo goodsBaseInfo = _goodsCenterSao.GetGoodsBaseInfoById(goodsId);
                if (goodsBaseInfo != null && !string.IsNullOrEmpty(goodsBaseInfo.GoodsName))
                {
                    //获取产品属性，根据分类属性生成表列
                    IList<FieldInfo> fieldList = _goodsCenterSao.GetFieldDetailByGoodsId(goodsId).ToList();
                    var tempList = GoodsList;
                    var selectGoods = tempList.FirstOrDefault(act => act.GoodsId == goodsId);
                    var atigmiaDic = new List<FieldInfo>();
                    var axialsDic = new List<FieldInfo>();
                    var luminosityDic = new List<FieldInfo>();
                    foreach (FieldInfo fieldInfo in fieldList)
                    {
                        var childFieldList = new List<FieldInfo> { new FieldInfo { FieldId = fieldInfo.FieldId, FieldValue = "全部" } };
                        childFieldList.AddRange(fieldInfo.ChildFields.OrderBy(act => act.OrderIndex).ToList());
                        switch (fieldInfo.FieldName)
                        {
                            case "光度":
                                luminosityDic = childFieldList;
                                break;
                            case "散光":
                                atigmiaDic = childFieldList;
                                break;
                            case "轴位":
                                axialsDic = childFieldList;
                                break;
                        }
                    }
                    if (selectGoods == null)
                    {
                        tempList.Add(new SearchGoodsInfo
                        {
                            Astigmias = atigmiaDic,
                            Axialss = axialsDic,
                            GoodsId = goodsBaseInfo.GoodsId,
                            GoodsCode = goodsBaseInfo.GoodsCode,
                            GoodsName = goodsBaseInfo.GoodsName,
                            Luminositys = luminosityDic
                        });
                        GoodsList = tempList;
                    }
                }
            }
            RGSelectGoods.Rebind();
        }

        /// <summary>搜索商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbGoodsItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            //此处商品搜索有待更新，需要 过滤是否下架商品。
            var combo = (RadComboBox)sender;
            combo.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text) && e.Text.Length >= 2)
            {
                var list = _goodsCenterSao.GetGoodsSelectList(e.Text);
                var totalCount = list.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    foreach (var item in list)
                    {
                        var rcb = new RadComboBoxItem
                        {
                            Text = item.Value,
                            Value = item.Key,
                        };
                        combo.Items.Add(rcb);
                    }
                }
            }
        }

        /// <summary>选择并添加商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SelectGoods_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(RCB_Warehouse.SelectedValue) || RCB_Warehouse.SelectedValue == Guid.Empty.ToString())
            {
                RAM.Alert("请选择仓库！");
                return;
            }
            if (string.IsNullOrEmpty(RCB_StorageAuth.SelectedValue) ||
               RCB_StorageAuth.SelectedValue == "0")
            {
                RAM.Alert("请选择出库储！");
                return;
            }
            if (string.IsNullOrEmpty(RCB_HostingFilialeAuth.SelectedValue) ||
                RCB_HostingFilialeAuth.SelectedValue == Guid.Empty.ToString())
            {
                RAM.Alert("请选择物流配送公司！");
                return;
            }
            if (RGSelectGoods.SelectedItems.Count == 0)
            {
                RAM.Alert("请选择商品");
                return;
            }
            var warehouseId = new Guid(RCB_Warehouse.SelectedValue);
            var hostingFilialeId = new Guid(RCB_HostingFilialeAuth.SelectedValue);
            var storageType = Convert.ToByte(RCB_StorageAuth.SelectedValue);
            var goodsIds = new List<Guid>();
            foreach (GridDataItem dataItem in RGSelectGoods.SelectedItems)
            {
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                if (goodsIds.Count(w => w == goodsId) == 0)
                    goodsIds.Add(goodsId);
            }
            var goodsList = _goodsCenterSao.GetGoodsListByGoodsIds(goodsIds);
            if (goodsList.Count == 0)
            {
                RAM.Alert("商品中心异常，请稍候再试！");
                return;
            }
            Dictionary<string, List<string>> dicFiled = CreateFiledGoods(HFSonGoods.Value);
            foreach (GridDataItem dataItem in RGSelectGoods.SelectedItems)
            {
                string dataItemClientId = dataItem.OriginalClientID;
                var selectedGoodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                GoodsInfo goodsInfo = goodsList.FirstOrDefault(w => w.GoodsId == selectedGoodsId) ?? new GoodsInfo();
                var current = GoodsList.FirstOrDefault(act => act.GoodsId == goodsInfo.GoodsId);
                if (current == null) continue;
                IList<ChildGoodsInfo> childGoodsList = new List<ChildGoodsInfo>();
                if (current.Luminositys.Count == 0 && current.Astigmias.Count == 0 && current.Axialss.Count == 0)
                {
                    var childGoodsInfo = new ChildGoodsInfo
                    {
                        GoodsId = goodsInfo.GoodsId,
                        RealGoodsId = goodsInfo.GoodsId,
                        Specification = string.Empty
                    };
                    childGoodsList.Add(childGoodsInfo);
                }
                else if (dataItem.Cells.Count >= 7)
                {
                    var dicSelectedField = new Dictionary<Guid, List<Guid>>();
                    for (int i = 5; i < dataItem.Cells.Count; i++)
                    {
                        var fieldInfoList = i == 5
                                ? current.Luminositys
                                : i == 6 ? current.Astigmias : current.Axialss;
                        if (i == 5 && current.Luminositys.Count == 0) break;
                        if (i == 6 && current.Astigmias.Count == 0) break;
                        if (i == 7 && current.Axialss.Count == 0) break;
                        var fieldId = fieldInfoList.First().FieldId;
                        string key = dataItemClientId + "_RCB_Field" + i;
                        foreach (var keyValuePair in dicFiled)
                        {
                            if (key == keyValuePair.Key)
                            {
                                var list = keyValuePair.Value.Select(str => new Guid(str)).ToList();
                                if (list.Count(w => w == fieldId) > 0)
                                {
                                    //该属性选择了全部
                                    FieldInfo fieldInfo = fieldInfoList.FirstOrDefault(w => w.FieldId == fieldId);
                                    if (fieldInfo != null)
                                        dicSelectedField.Add(fieldId, fieldInfoList.Where(act => act.FieldId != fieldId).Select(w => w.FieldId).ToList());
                                }
                                else
                                {
                                    dicSelectedField.Add(fieldId, list);
                                }
                                break;
                            }
                        }
                    }
                    if (dicSelectedField.Count > 0)
                    {
                        childGoodsList = _goodsCenterSao.GetRealGoodsListByGoodsIdAndFields(selectedGoodsId, dicSelectedField);
                        if (childGoodsList.Count == 0)
                        {
                            RAM.Alert(string.Format("商品{0}属性选择不完整！", goodsInfo.GoodsCode));
                            return;
                        }
                    }
                    else
                    {
                        RAM.Alert(string.Format("商品{0}请选择子商品！", goodsInfo.GoodsCode));
                        return;
                    }
                }

                foreach (var childGoodsInfo in childGoodsList)
                {
                    BindRgGoods(warehouseId, storageType, hostingFilialeId, childGoodsInfo.RealGoodsId, goodsInfo.GoodsName, childGoodsInfo.Specification);
                }
            }
        }

        /// <summary>
        /// 取得用户操作权限
        /// </summary>
        protected bool GetPowerOperationPoint(string powerName)
        {
            const string PAGE_NAME = "StorageRecordApplyOut.aspx";
            return WebControl.GetPowerOperationPoint(PAGE_NAME, powerName);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DeleteGoods_Click(object sender, EventArgs e)
        {
            foreach (GridDataItem dataItem in RGGoods.SelectedItems)
            {
                var realGoodsId = new Guid(dataItem.GetDataKeyValue("RealGoodsId").ToString());
                var unitPrice = dataItem.GetDataKeyValue("UnitPrice").ToString();
                var goodsStockInfo = InDetailList.FirstOrDefault(w => w.RealGoodsId == realGoodsId && w.UnitPrice == Convert.ToDecimal(unitPrice));
                if (goodsStockInfo != null)
                {
                    InDetailList.Remove(goodsStockInfo);
                }
            }
            RGGoods.Rebind();
        }

        /// <summary>选择商品框的商品绑定
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RGSelectGoods_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            RGSelectGoods.MasterTableView.Columns[3].Visible = GoodsList.Any(act => act.Luminositys.Count > 0);
            RGSelectGoods.MasterTableView.Columns[4].Visible = GoodsList.Any(act => act.Astigmias.Count > 0);
            RGSelectGoods.MasterTableView.Columns[5].Visible = GoodsList.Any(act => act.Axialss.Count > 0);
            RGSelectGoods.DataSource = GoodsList;
        }

        /// <summary>价格表单元格绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RGSelectGoods_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var dataItem = (GridDataItem)e.Item;
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                var current = GoodsList.FirstOrDefault(act => act.GoodsId == goodsId);
                if (current != null)
                {
                    foreach (GridTableCell cell in e.Item.Cells)
                    {
                        int cellIndex = e.Item.Cells.GetCellIndex(cell);
                        if (cellIndex >= 5)
                        {
                            var fieldList = cellIndex == 5
                                ? current.Luminositys
                                : cellIndex == 6 ? current.Astigmias : current.Axialss;
                            if (fieldList.Count > 0)
                            {
                                var rads = new RadComboBox();
                                cell.Controls.Add(rads);
                                rads.ID = "RCB_Field" + cellIndex;
                                rads.Height = Unit.Pixel(300);
                                rads.Width = Unit.Pixel(100);
                                rads.ItemTemplate = LoadTemplate("~/UserControl/ChildFieldIControl.ascx");
                                rads.DataSource = fieldList;
                                rads.DataTextField = "FieldValue";
                                rads.DataValueField = "FieldId";
                                rads.DataBind();
                                rads.HighlightTemplatedItems = true;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>建立 Dictionary 存放每行选择的商品属性
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> CreateFiledGoods(string str)
        {
            var dic = new Dictionary<string, List<string>>();
            if (!string.IsNullOrEmpty(str))
            {
                string[] goodsArr = str.Split('@');
                foreach (string combGoods in goodsArr)
                {
                    string regStr = Regex.Match(combGoods, @"([^_]*_){4}([^_]*)", RegexOptions.IgnoreCase).Value;
                    if (dic.Count > 0)
                    {
                        bool isExist = false;
                        foreach (var keyValuePair in dic)
                        {
                            if (keyValuePair.Key == regStr)
                            {
                                keyValuePair.Value.Add(combGoods.Split('|')[1]);
                                isExist = true;
                            }
                        }
                        if (isExist == false)
                        {
                            var fieldList = new List<string> { combGoods.Split('|')[1] };
                            dic.Add(regStr, fieldList);
                        }
                    }
                    else
                    {
                        var fieldList = new List<string> { combGoods.Split('|')[1] };
                        dic.Add(regStr, fieldList);
                    }
                }
            }
            return dic;
        }

        //商品分类
        private void BindGoodsClass()
        {
            RCB_GoodsClass.DataSource = _goodsClassManager.GetGoodsClassListWithRecursion();
            RCB_GoodsClass.DataBind();
            RCB_GoodsClass.Items.Insert(0, new RadComboBoxItem("", Guid.Empty.ToString()));
        }

        public List<SearchGoodsInfo> GoodsList
        {
            get
            {
                if (ViewState["GoodsList"] == null)
                {
                    return new List<SearchGoodsInfo>();
                }
                return (List<SearchGoodsInfo>)ViewState["GoodsList"];
            }
            set
            {
                ViewState["GoodsList"] = value;
            }
        }

        #region 下拉框绑定
        /// <summary>
        /// 绑定入库仓储
        /// </summary>
        private void BindWarehouse()
        {
            var wList = CurrentSession.Personnel.WarehouseList;
            RCB_Warehouse.DataSource = wList;
            RCB_Warehouse.DataTextField = "WarehouseName";
            RCB_Warehouse.DataValueField = "WarehouseId";
            RCB_Warehouse.DataBind();
            RCB_Warehouse.Items.Insert(0, new RadComboBoxItem("", Guid.Empty.ToString()));
        }
        #endregion
    }
}