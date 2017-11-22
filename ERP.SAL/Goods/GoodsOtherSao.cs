using System;
using System.Collections.Generic;
using DescriptionExtendTitleInfo = ERP.Model.DescriptionExtendTitleInfo;
using GoodsGroupInfo = ERP.Model.Goods.GoodsGroupInfo;
using ServiceDescriptionExtendTitleInfo = KeedeGroup.GoodsManageSystem.Public.Model.Table.DescriptionExtendTitleInfo;
using ServiceGroupInfo = KeedeGroup.GoodsManageSystem.Public.Model.Table.GroupInfo;

namespace ERP.SAL.Goods
{
    public partial class GoodsCenterSao
    {

        #region --> (GMS)GroupInfo > (ERP)GroupInfo

        /// <summary>
        /// 转换成本地GroupInfo
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static GoodsGroupInfo ConvertToMyGroupInfo(ServiceGroupInfo info)
        {
            return new GoodsGroupInfo
                       {
                           GroupId = info.GroupID,
                           GroupName = info.Name
                       };
        }

        /// <summary>
        /// 转换成本地GroupInfo
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<GoodsGroupInfo> ConvertToMyGroupList(IEnumerable<ServiceGroupInfo> list)
        {
            foreach (var item in list)
            {
                yield return ConvertToMyGroupInfo(item);
            }
        }
        #endregion

        #region --> (ERP)DescriptionExtendTitleInfo > (GMS)DescriptionExtendTitleInfo
        /// <summary>
        /// 转换成服务
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        static ServiceDescriptionExtendTitleInfo ConvertToServiceDescriptionExtendTitleInfo(DescriptionExtendTitleInfo info)
        {
            return new ServiceDescriptionExtendTitleInfo
                       {
                           Id = info.Id,
                           Title = info.Title,
                           Position = info.Position
                       };
        }
        #endregion

        #region --> (GSM)DescriptionExtendTitleInfo > (ERP)DescriptionExtendTitleInfo
        static DescriptionExtendTitleInfo ConvertToDescriptionExtendTitleInfo(ServiceDescriptionExtendTitleInfo info)
        {
            return new DescriptionExtendTitleInfo
            {
                Id = info.Id,
                Title = info.Title,
                Position = info.Position
            };
        }
        #endregion

        /// <summary>
        /// 获取所有平台
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GoodsGroupInfo> GetGroupList()
        {
            var result = GoodsServerClient.GetGroupList();
            if (result != null && result.IsSuccess)
            {
                var items = result.Data ?? new List<ServiceGroupInfo>();
                foreach (var item in items)
                {
                    yield return ConvertToMyGroupInfo(item);
                }
            }
        }

        /// <summary>
        /// 根据销售销ID获平台
        /// </summary>
        /// <param name="salePlatformId"></param>
        /// <returns></returns>
        public GoodsGroupInfo GetGroupInfoBySalePlatformId(Guid salePlatformId)
        {
            var result = GoodsServerClient.GetGroupInfoBySalePlatformId(salePlatformId);
            if (result != null && result.IsSuccess)
            {
                return result.Data == null ? null : ConvertToMyGroupInfo(result.Data);
            }
            return null;
        }

        public DescriptionExtendTitleInfo GetDescriptionExtendTitleInfo(Guid id)
        {
            var result = GoodsServerClient.GetDescriptionExtendTitleInfoById(id);
            if (result != null && result.IsSuccess)
            {
                return result.Data == null ? null : ConvertToDescriptionExtendTitleInfo(result.Data);
            }
            return null;
        }

        public IEnumerable<DescriptionExtendTitleInfo> GetDescriptionExtendTitleInfoList()
        {
            var result = GoodsServerClient.GetDescriptionExtendTitleList();
            if (result != null && result.IsSuccess)
            {
                var items = result.Data ?? new List<ServiceDescriptionExtendTitleInfo>();
                foreach (var item in items)
                {
                    yield return ConvertToDescriptionExtendTitleInfo(item);
                }
            }
        }

        /// <summary>
        /// 新增扩展标题
        /// </summary>
        /// <param name="info"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool AddDescriptionExtendTitle(DescriptionExtendTitleInfo info, out string errorMessage)
        {
            var result = GoodsServerClient.AddDescriptionExtendTitle(ConvertToServiceDescriptionExtendTitleInfo(info));
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }

        /// <summary>
        /// 修改扩展标题
        /// </summary>
        /// <param name="info"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool UpdateDescriptionExtendTitle(DescriptionExtendTitleInfo info, out string errorMessage)
        {
            var result = GoodsServerClient.UpdateDescriptionExtendTitle(ConvertToServiceDescriptionExtendTitleInfo(info));
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }

        /// <summary>
        /// 删除扩展标题
        /// </summary>
        /// <param name="id"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool DeleteDescriptionExtendTitle(Guid id, out string errorMessage)
        {
            var result = GoodsServerClient.DeleteDescriptionExtendTitle(id);
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }
    }
}
