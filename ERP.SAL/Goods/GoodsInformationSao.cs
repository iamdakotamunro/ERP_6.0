using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model;
using ERP.Model.Goods;
using KeedeGroup.GoodsManageSystem.Public.Model.ERP;
using KeedeGroup.GoodsManageSystem.Public.Model.Table;

namespace ERP.SAL.Goods
{
    public partial class GoodsCenterSao
    {
        /// <summary>
        /// 转换成GoodsInformationInfo
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        static GoodsInformationInfo ConvertToGoodsInformationInfo(KeedeGroup.GoodsManageSystem.Public.Model.Table.GoodsQualificationDetailInfo info)
        {
            return new GoodsInformationInfo
            {
                ID = Guid.NewGuid(),
                IdentifyId = info.GoodsID,
                Path = info.Path,
                UploadDate = info.LastOperationTime,
                OverdueDate = info.OverdueDate,
                QualificationType = info.GoodsQualificationType,
                ExtensionName = info.ExtensionName,
                Number = info.NameOrNo
            };
        }

        static SupplierGoodsInfo ConvertToQualificationShowInfo(GoodsQualificationModel model)
        {
            return new SupplierGoodsInfo
            {
                ID = model.GoodsID,
                Name = model.GoodsName,
                Complete = model.GoodsQualificationState,
                Type = "商品资质"
            };
        }

        /// <summary>
        /// ERP -->  GMS
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static KeedeGroup.GoodsManageSystem.Public.Model.Table.GoodsQualificationDetailInfo ConvertToInformationInfo(GoodsInformationInfo info)
        {
            return new KeedeGroup.GoodsManageSystem.Public.Model.Table.GoodsQualificationDetailInfo
            {
                GoodsID = info.IdentifyId,
                NameOrNo = info.Number,
                Path = info.Path,
                LastOperationTime = info.UploadDate == null ? DateTime.MinValue : Convert.ToDateTime(info.UploadDate),
                OverdueDate = info.OverdueDate == null ? DateTime.MinValue : Convert.ToDateTime(info.OverdueDate),
                GoodsQualificationType = info.QualificationType,
                ExtensionName = info.ExtensionName
            };
        }

        /// <summary>
        /// 查询商品资质(分页)
        /// </summary>
        /// <param name="goodsName">商品名</param>
        /// <param name="nameOrNo">资质名</param>
        /// <param name="state">是否完整</param>
        /// <param name="dateState">是否过期</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="totalCount">总数</param>
        /// <returns></returns>
        public IList<SupplierGoodsInfo> SelectGoodsInformationInfosByPage(string goodsName, string nameOrNo,
            int? state, int? dateState, int pageIndex, int pageSize, out long totalCount)
        {
            var result = GoodsServerClient.GetGoodsQualificationListByPage(goodsName, nameOrNo, state, dateState, pageIndex, pageSize);
            if (result != null && result.IsSuccess)
            {
                var items = result.Data ?? new List<GoodsQualificationModel>();
                totalCount = result.Total;
                return items.Select(ConvertToQualificationShowInfo).ToList();
            }
            totalCount = 0;
            return new List<SupplierGoodsInfo>();
        }

        /// <summary>
        /// 获取资料信息
        /// </summary>
        /// <param name="identifyId"></param>
        /// <returns></returns>
        public IList<GoodsInformationInfo> GetInformation(Guid identifyId)
        {
            var result = GoodsServerClient.GetGoodsQualificationDetailList(identifyId);
            if (result != null && result.IsSuccess)
            {
                var items = result.Data ?? new List<KeedeGroup.GoodsManageSystem.Public.Model.Table.GoodsQualificationDetailInfo>();
                return items.Select(ConvertToGoodsInformationInfo).ToList();
            }
            return new List<GoodsInformationInfo>();
        }

        /// <summary>
        /// 添加商品资料
        /// </summary>
        /// <param name="identifyId"></param>
        /// <param name="informationList"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool SetInformationList(Guid identifyId, IEnumerable<GoodsInformationInfo> informationList, out string errorMessage)
        {
            var list = informationList.Select(ConvertToInformationInfo).ToList();
            var result = GoodsServerClient.SaveGoodsQualificationDetailList(identifyId, list);
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }

        /// <summary>
        /// 根据GoodsId获取商品资料信息
        /// </summary>
        /// <param name="goodsIds">商品id列表</param>
        /// <returns></returns>
        /// zal 2015-11-23
        public Dictionary<Guid, List<GoodsInformationInfo>> GetGoodsInformationList(List<Guid> goodsIds)
        {
            var dics = new Dictionary<Guid, List<GoodsInformationInfo>>();
            if (goodsIds != null && goodsIds.Count > 0)
            {
                var result = GoodsServerClient.GetGoodsQualificationDetailDict(goodsIds);
                if (result != null && result.IsSuccess)
                {
                    var list = result.Data ?? new Dictionary<Guid, List<KeedeGroup.GoodsManageSystem.Public.Model.Table.GoodsQualificationDetailInfo>>();
                    foreach (var info in list)
                    {
                        var values = info.Value.Select(ConvertToGoodsInformationInfo).ToList();
                        dics.Add(info.Key, values);
                    }
                }
            }
            return dics;
        }
    }
}
