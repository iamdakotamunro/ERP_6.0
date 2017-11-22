//================================================
// Դ��������˾:�Ϻ���ý��Ϣ�������޹�˾
// �ļ�����ʱ��:2008��5��29��
// �ļ�������:����
// ����޸�ʱ��:2008��10��9��
// ���һ���޸���:����
//================================================

using System;
using System.Collections.Generic;
using ERP.Model;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// �������нӿ���
    /// </summary>
    public interface ISalesGoodsRanking
    {
        /// <summary>
        /// ��ȡ��������
        /// <para>
        /// Code by Ruanjianfeng 2012-2-28
        /// </para>
        /// </summary>
        /// <param name="top"></param>
        /// <param name="goodsClassList"></param>
        /// <param name="brandId">Ʒ���µ���ƷID</param>
        /// <param name="goodsName"></param>
        /// <param name="goodsCode"></param>
        /// <param name="warehouseId"></param>
        /// <param name="salePlatformId"> </param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="salefilialeId"> </param>
        /// <param name="isContainDisableSalePlatform"> </param>
        /// <returns></returns>
        IList<SalesGoodsRankingInfo> GetGoodsSalesRanking(int top, string goodsClassList, Guid brandId, string goodsName, string goodsCode,
                                                           Guid warehouseId, Guid salefilialeId, Guid salePlatformId, DateTime startTime,
                                                          DateTime endTime, bool isContainDisableSalePlatform);

        IList<SalesGoodsRankingInfo> GetGoodsSalesRankingByGoodsIds(List<Guid> goodsIds,
            Guid warehouseId, Guid salefilialeId, Guid salePlatformId, DateTime startTime, DateTime endTime);

        bool UpdateGoodsSalesRankingGoodsName(Guid goodsId, string goodsName, string goodsCode, Guid seriesId, Guid brandId);

        /// <summary>
        /// ������������ϵ��ID
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="seriesId"></param>
        /// <returns></returns>
        bool UpdateGoodsSaleSeriesId(Guid goodsId, Guid seriesId);

        /// <summary>
        /// ��ȡ������Ʒһ��ʱ��������
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="salefilialeId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        IList<SalesGoodsRankingInfo> GetSalesRankingChart(Guid goodsId, Guid warehouseId,Guid hostingFilialeId, Guid salefilialeId, Guid salePlatformId, DateTime startTime, DateTime endTime);

        /// <summary>
        /// ��ȡϵ����Ʒһ��ʱ��������
        /// </summary>
        /// <param name="seriesId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="salefilialeId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        IList<SalesGoodsRankingInfo> GetSalesRankingChartBySeriesId(
            Guid seriesId, Guid warehouseId,Guid hostingFilialeId, Guid salefilialeId, Guid salePlatformId, DateTime startTime, DateTime endTime);

        #region  ������ѯ�Ľ�  

        /// <summary>
        /// ����Ʒ�������в�ѯ ����ϵ��
        /// </summary>
        /// <param name="top"></param>
        /// <param name="goodsClassList"></param>
        /// <param name="brandId"></param>
        /// <param name="goodsName"></param>
        /// <param name="goodsId"></param>
        /// <param name="salefilialeId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="isContainDisableSalePlatform"></param>
        /// <returns></returns>
        IList<SaleRaningShowInfo> GetGoodsSaleRankingBySeriesId(int top, string goodsClassList,
            Guid brandId, string goodsName,
            Guid goodsId, Guid salefilialeId, Guid salePlatformId,
            DateTime startTime, DateTime endTime, bool isContainDisableSalePlatform);

        /// <summary>
        /// ����Ʒ�������в�ѯ
        /// </summary>
        /// <param name="top"></param>
        /// <param name="goodsClassList"></param>
        /// <param name="brandId"></param>
        /// <param name="goodsName"></param>
        /// <param name="goodsId"></param>
        /// <param name="salefilialeId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="isContainDisableSalePlatform"></param>
        /// <returns></returns>
        IList<SaleRaningShowInfo> GetGoodsSaleRankingBySale(int top, string goodsClassList, Guid brandId,
            string goodsName,
            Guid goodsId, Guid salefilialeId, Guid salePlatformId,
            DateTime startTime, DateTime endTime, bool isContainDisableSalePlatform);

        /// <summary>
        /// ��ƽ̨������в�ѯ
        /// </summary>
        /// <param name="top"></param>
        /// <param name="goodsClassList"></param>
        /// <param name="brandId"></param>
        /// <param name="goodsName"></param>
        /// <param name="goodsId"></param>
        /// <param name="salefilialeId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="isContainDisableSalePlatform"></param>
        /// <returns></returns>
        IList<SaleRaningShowInfo> GetGoodsSaleRankingBySalePlate(int top, string goodsClassList, Guid brandId,
            string goodsName,
            Guid goodsId, Guid salefilialeId, Guid salePlatformId,
            DateTime startTime, DateTime endTime, bool isContainDisableSalePlatform);

        /// <summary>
        /// ��Ʒ�Ʒ�����в�ѯ
        /// </summary>
        /// <param name="top"></param>
        /// <param name="goodsClassList"></param>
        /// <param name="brandId"></param>
        /// <param name="goodsName"></param>
        /// <param name="goodsId"></param>
        /// <param name="salefilialeId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="isContainDisableSalePlatform"></param>
        /// <returns></returns>
        IList<SaleRaningShowInfo> GetGoodsSaleRankingByBrand(int top, string goodsClassList, Guid brandId, string goodsName, Guid goodsId, Guid salefilialeId, Guid salePlatformId, DateTime startTime, DateTime endTime, bool isContainDisableSalePlatform);

        #endregion


        /// <summary>
        /// ��Ӧ��������ѯ
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        IList<CompanySaleStatisticsInfo> SelectGoodsSaleStatisticsInfos(DateTime startTime, DateTime endTime);

        /// <summary>
        /// ��ϸ��ѯ
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="filialeId"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        IList<CompanySaleStatisticsInfo> SelectCompanySaleStatisticsInfosByFilialeId(DateTime startTime,
            DateTime endTime, Guid filialeId, string companyName);


        /// <summary>
        /// ���ݿ�ʼʱ��,��ֹʱ���ȡָ��ʱ����ڵ���Ʒ��������
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-28
        IList<GoodsSalesInfo> GetAllRealGoodsSaleNumber(DateTime startTime, DateTime endTime);

        /// <summary>
        /// ���ݿ�ʼʱ��,��ֹʱ���ȡָ��ʱ����ڵ���Ʒ�վ���������
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        IList<GoodsAvgDaySalesInfo> GetAvgRealGoodsSaleNumber(DateTime startTime, DateTime endTime);


        /// <summary>
        /// ���ݿ�ʼʱ��,��ֹʱ�䣬����ƷID������ƽ̨ ��ȡʱ����ھ�������ƽ̨ĳ������Ʒ��������
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="realGoodsId">����ƷID</param>
        /// <param name="salePlatformId">����ƽ̨</param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-28
        int GetRealGoodsSaleNumber(DateTime startTime, DateTime endTime, Guid realGoodsId, Guid salePlatformId);

        /// <summary>�������۹�˾��ȡ��������Ʒ������
        /// </summary>
        /// <param name="fromTime">��ʼʱ��</param>
        /// <param name="toTime">��ֹʱ��</param>
        /// <param name="saleFilialeId">���۹�˾ID</param>
        /// <returns>Key����ƷGoodsId, Value: ����</returns>
        Dictionary<Guid, int> GetGoodsSaleBySaleFilialeId(DateTime fromTime, DateTime toTime, Guid saleFilialeId);

        /// <summary>��������ƽ̨��ȡ��������Ʒ������
        /// </summary>
        /// <param name="fromTime">��ʼʱ��</param>
        /// <param name="toTime">��ֹʱ��</param>
        /// <param name="salePlatformIdList">����ƽ̨ID����</param>
        /// <returns>Key����ƷGoodsId, Value: ����</returns>
        /// zal 2017-07-27
        Dictionary<Guid, int> GetGoodsSaleBySalePlatformIdList(DateTime fromTime, DateTime toTime,List<Guid> salePlatformIdList);
    }
}
