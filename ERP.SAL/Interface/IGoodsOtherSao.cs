using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Model;
using ERP.Model.Goods;

namespace ERP.SAL.Interface
{
    public partial interface IGoodsCenterSao
    {
        /// <summary>
        /// 获取所有平台
        /// </summary>
        /// <returns></returns>
        IEnumerable<GoodsGroupInfo> GetGroupList();

        /// <summary>
        /// 根据销售销ID获平台
        /// </summary>
        /// <param name="salePlatformId"></param>
        /// <returns></returns>
        GoodsGroupInfo GetGroupInfoBySalePlatformId(Guid salePlatformId);

        DescriptionExtendTitleInfo GetDescriptionExtendTitleInfo(Guid id);

        IEnumerable<DescriptionExtendTitleInfo> GetDescriptionExtendTitleInfoList();

        /// <summary>
        /// 新增扩展标题
        /// </summary>
        /// <param name="info"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool AddDescriptionExtendTitle(DescriptionExtendTitleInfo info, out string errorMessage);

        /// <summary>
        /// 修改扩展标题
        /// </summary>
        /// <param name="info"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool UpdateDescriptionExtendTitle(DescriptionExtendTitleInfo info, out string errorMessage);

        /// <summary>
        /// 删除扩展标题
        /// </summary>
        /// <param name="id"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool DeleteDescriptionExtendTitle(Guid id, out string errorMessage);
    }
}
