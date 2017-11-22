using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Enum;
using ERP.Model;

namespace ERP.DAL.Interface.IInventory
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/8/11 13:51:01 
     * 描述    : 单据红冲接口
     * =====================================================================
     * 修改时间：2016/8/11 13:51:01 
     * 修改人  ：  
     * 描述    ：
     */
    public interface IDocumentRedDao
    {
        /// <summary>
        /// 红冲单添加
        /// </summary>
        /// <param name="documentRedInfo">红冲单据模型</param>
        bool InsertDocumentRed(DocumentRedInfo documentRedInfo);

        /// <summary>
        /// 批量添加红冲单据明细
        /// </summary>
        /// <param name="documentRedDetailList">红冲单据明细List</param>
        bool BatchInsertDocumentRedDetail(IList<DocumentRedDetailInfo> documentRedDetailList);

        /// <summary>
        /// 更新红冲单数据
        /// </summary>
        /// <param name="redId">红冲单据ID</param>
        /// <param name="accountReceivable">总金额</param>
        /// <param name="description">描述</param>
        /// <param name="memo">备注</param>
        /// <param name="state"></param>
        /// zal 2016-09-29
        bool UpdateDocumentRedByRedId(Guid redId, decimal accountReceivable, string description, string memo,int state);

        /// <summary>
        /// 更新红冲单据状态和描述
        /// </summary>
        /// <param name="redId">出入库单据ID</param>
        /// <param name="state">单据状态</param>
        /// <param name="description">描述</param>
        /// <param name="memo">备注</param>
        bool UpdateStateDocumentRed(Guid redId, DocumentRedState state, string description, string memo);

        /// <summary>
        /// 根据红冲Id删除数据
        /// </summary>
        /// <param name="redId"></param>
        /// <returns></returns>
        /// zal 2016-09-29
        bool DelDocumentRedByRedId(Guid redId);

        /// <summary>
        /// 根据红冲Id删除红冲明细
        /// </summary>
        /// <param name="redId"></param>
        /// <returns></returns>
        /// zal 2016-09-29
        bool DelDocumentRedDetailByRedId(Guid redId);

        /// <summary>
        /// 红冲单据分页查询
        /// </summary>
        /// <param name="warehouseId">仓库</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="redType">红冲类型</param>
        /// <param name="documentType">单据类型</param>
        /// <param name="state">状态</param>
        /// <param name="no">单号</param>
        /// <param name="startPage">当前页</param>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns></returns>
        IList<DocumentRedInfo> GetDocumentRedListToPage(Guid warehouseId, DateTime startTime, DateTime endTime,
            int redType, int documentType, int state, string no, int startPage, int pageSize,
            out long recordCount);

        /// <summary>
        /// 根据单据红冲ID获取记录信息
        /// </summary>
        /// <param name="redId">记录Id</param>
        /// <returns></returns>
        DocumentRedInfo GetDocumentRed(Guid redId);

        DocumentRedInfo GetDocumentRedByNewRedId(Guid redId);

        IList<DocumentRedInfo> GetDocumentRedInfoByLinkTradeCode(string linkeTradeCode);

        IList<DocumentRedInfo> GetDocumentRedInfoByLinkTradeId(Guid linkTradeId);

        /// <summary>
        ///  根据单据红冲记录ID获取明细
        /// </summary>
        /// <param name="redId">红冲记录ID</param>
        /// <returns></returns>
        IList<DocumentRedDetailInfo> GetDocumentRedDetailListByRedId(Guid redId);

        /// <summary>
        ///  根据出入库ID获取出入库明细
        /// </summary>
        /// <param name="stockId">出入库ID</param>
        /// <returns></returns>
        IList<DocumentRedDetailInfo> GetDocumentRedDetailListByStockId(Guid stockId);
    }
}
