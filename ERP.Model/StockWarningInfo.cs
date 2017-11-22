//================================================
// Դ��������˾:�Ϻ���ý��Ϣ�������޹�˾
// �ļ�����ʱ��:2008��6��3��
// �ļ�������:����
// ����޸�ʱ��:2008��6��3��
// ���һ���޸���:����
//================================================
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// ���Ԥ����Ϣ��
    /// </summary>
    [Serializable]
    public class StockWarningInfo
    {
        /// <summary>
        /// ����ƷID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// ��Ʒ��
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// ��Ʒ����
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// ���
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// ��ǰ�ֹ�˾��Ʒ���
        /// </summary>
        public double NonceFilialeGoodsStock { get; set; }

        /// <summary>
        /// ��ǰ������
        /// </summary>
        public double NonceRequest { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        public Int32 RequireQuantity { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public Int32 SubtotalQuantity { get; set; }

        /// <summary>
        /// ��ǰ��Ʒ�ܿ��
        /// </summary>
        public double NonceGoodsStock { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public double SalesNumber { get; set; }

        /// <summary>
        /// ����Ʒ�Ƿ�ȱ��
        /// </summary>
        public bool IsScarcity { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsOnShelf { set; get; }

        /// <summary>
        /// ��ǰ�ֿ���Ʒ���
        /// </summary>
        public double NonceWarehouseGoodsStock
        {
            get { return NonceFilialeGoodsStock; }
            set { NonceFilialeGoodsStock = value; }
        }

        /// <summary>
        /// ������λ�б�
        /// </summary>
        public IList<CompanyCussentInfo> ccInfo { get; set; }

        /// <summary>
        /// ������λID
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// ǰ��һ���������ڵ����ۼ�¼
        /// </summary>
        public double FirstNumberOneStockUpSale { get; set; }

        /// <summary>
        /// ǰ�ڶ����������ڵ����ۼ�¼
        /// </summary>
        public double FirstNumberTwoStockUpSale { get; set; }

        /// <summary>
        /// ǰ�������������ڵ����ۼ�¼
        /// </summary>
        public double FirstNumberThreeStockUpSale { get; set; }


        /// <summary>
        /// Ҫ��ȥ�Ĳɹ�����������������δ��ɵĲɹ�����(���к��в���δ�ɹ��ɹ�����Ʒ����)
        /// </summary>
        public double SubtractPurchasingQuantity { get; set; }

        /// <summary>
        /// Ҫ��ȥ�Ĳɹ�����������������δ��ɵĲɹ�����(���к��в���δ�ɹ��ɹ�����Ʒ����)
        /// </summary>
        public double PurchasingQuantity { get; set; }

        public double UppingQuantity { get; set; }

        public Dictionary<int, double> PerStepSales { get; set; }

        public Guid HostingFilialeId { get; set; }

        /// <summary>
        /// ʵ����Ҫ�Ĳɹ�����
        /// </summary>
        public double RealityNeedPurchasingQuantity
        {
            get
            {
                double needPurchasingQuantity = SubtractPurchasingQuantity < 0 ? Math.Ceiling(PlanPurchasingquantity + SubtractPurchasingQuantity - (NonceWarehouseGoodsStock + UppingQuantity - RequireQuantity) + SubtotalQuantity) :
                    Math.Ceiling(PlanPurchasingquantity - SubtractPurchasingQuantity - (NonceWarehouseGoodsStock + UppingQuantity - RequireQuantity) + SubtotalQuantity);
                return needPurchasingQuantity;
            }
        }

        /// <summary>
        /// ��Ȩ��ƽ������
        /// </summary>
        public double WeightedAverageSaleQuantity
        {
            /*
                            9 * N1 + 8 * N2 + 7 * N3 + 6 * N4 + 5 * N5 + 4 * N6+3 * N7 + 2 * N8 + 1*N9
            ��Ȩ��ƽ������ = -----------------------------------------------------------------------------
                                           10 * (9 + 8 + 7 + 6 + 5 + 4 + 3 + 2 + 1)
            */
            get
            {
                if (PerStepSales == null || PerStepSales.Count == 0) return 0;
                if (PerStepSales.Count == 1)
                {
                    return PerStepSales.First().Value;
                }
                var perStepSales = PerStepSales.OrderBy(act => act.Key);

                var one = perStepSales.Take(1).Sum(act => act.Value);
                var two = perStepSales.Skip(1).Take(1).Sum(act => act.Value);
                var three = perStepSales.Skip(2).Take(1).Sum(act => act.Value);
                var four = perStepSales.Skip(3).Take(1).Sum(act => act.Value);
                var five = perStepSales.Skip(4).Take(1).Sum(act => act.Value);
                var six = perStepSales.Skip(5).Take(1).Sum(act => act.Value);
                var seven = perStepSales.Skip(6).Take(1).Sum(act => act.Value);
                var eight = perStepSales.Skip(7).Take(1).Sum(act => act.Value);
                var nine = perStepSales.Skip(8).Take(1).Sum(act => act.Value);

                return (9 * one + 8 * two + 7 * three + 6 * four + 5 * five + 4 * six + 3 * seven + 2 * eight + 1 * nine) /(10 * (9 + 8 + 7 + 6 + 5 + 4 + 3 + 2 + 1));


                //var first = perStepSales.Take(3).Sum(act => act.Value);
                //var two = perStepSales.Skip(3).Take(3).Sum(act => act.Value);
                //var third = perStepSales.Skip(6).Take(3).Sum(act => act.Value);
                //return (3 * first + 2 * two + third) / (10 * 9 * 6);

                //var sumSaleNumber = (FirstNumberOneStockUpSale * 3 + FirstNumberTwoStockUpSale * 2 + FirstNumberThreeStockUpSale);
                //var sumWeightedNumber = ((FirstNumberOneStockUpSale == 0 ? 0 : 3) + (FirstNumberTwoStockUpSale == 0 ? 0 : 2) + (FirstNumberThreeStockUpSale == 0 ? 0 : 1));
                //return sumWeightedNumber == 0 ? 0 : (sumSaleNumber / sumWeightedNumber) / 10;
            }
        }

        /// <summary>
        /// �������ڵ�����������
        /// </summary>
        public double SaleInCrease
        {
            get
            {
                var num1 = (FirstNumberTwoStockUpSale == 0 ? 1 : FirstNumberTwoStockUpSale);
                var num2 = (FirstNumberThreeStockUpSale == 0 ? 1 : FirstNumberThreeStockUpSale);
                var num = (FirstNumberOneStockUpSale / num1 + FirstNumberTwoStockUpSale / num2) / 2;
                if (num >= 1.3)
                {
                    return 1.1;
                }
                return num;
            }
        }
        /// <summary>
        /// �������ڵ�����ƽ��������
        /// </summary>
        public double SaleAvgCrease
        {
            get
            {
                var num1 = FirstNumberThreeStockUpSale == 0 ? (FirstNumberTwoStockUpSale > 1 ? 1 : 0) : (FirstNumberTwoStockUpSale - FirstNumberThreeStockUpSale) / FirstNumberThreeStockUpSale;
                var num2 = FirstNumberTwoStockUpSale == 0 ? (FirstNumberOneStockUpSale > 1 ? 1 : 0) : (FirstNumberOneStockUpSale - FirstNumberTwoStockUpSale) / FirstNumberTwoStockUpSale;
                var num = (num1 + num2) / 2;
                if (num >= 1.3)
                {
                    return 1.1;
                }
                return Convert.ToDouble(num.ToString("f2"));
            }
        }
        /// <summary>
        /// ��������
        /// </summary>
        public int StockDay { get; set; }
        /// <summary>
        /// �ƻ��ɹ���
        /// </summary>
        public double PlanPurchasingquantity
        {
            get
            {
                //�ִ���ȷ��ȥ�������� SaleInCrease
                return WeightedAverageSaleQuantity * StockDay;
            }
        }

        /// <summary>
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public StockWarningInfo() { }

        /// <summary>
        /// ��ʼ�����캯��
        /// </summary>
        /// <param name="goodsId">��Ʒ���</param>
        /// <param name="specification">���</param>
        /// <param name="nonceFilialeGoodsStock">ָ����˾�Ŀ����</param>
        /// <param name="nonceRequest">��ǰ������</param>
        /// <param name="nonceGoodsStock">�ܿ����</param>
        /// <param name="salesNumber">���۶�</param>
        public StockWarningInfo(Guid goodsId, string specification, double nonceFilialeGoodsStock, double nonceRequest, double nonceGoodsStock, double salesNumber)
        {
            GoodsId = goodsId;
            Specification = specification;
            NonceFilialeGoodsStock = nonceFilialeGoodsStock;
            NonceRequest = nonceRequest;
            NonceGoodsStock = nonceGoodsStock;
            SalesNumber = salesNumber;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="specification"></param>
        /// <param name="nonceFilialeGoodsStock"></param>
        public StockWarningInfo(Guid goodsId, string specification, double nonceFilialeGoodsStock)
        {
            GoodsId = goodsId;
            Specification = specification;
            NonceFilialeGoodsStock = nonceFilialeGoodsStock;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsId">��Ʒ���</param>
        /// <param name="specification">���</param>
        /// <param name="nonceFilialeGoodsStock">ָ����˾�Ŀ����</param>
        /// <param name="nonceGoodsStock">�ܿ����</param>
        /// <param name="salesNumber">���۶�</param>
        public StockWarningInfo(Guid goodsId, string specification, double nonceFilialeGoodsStock, double nonceGoodsStock, double salesNumber)
        {
            GoodsId = goodsId;
            Specification = specification;
            NonceFilialeGoodsStock = nonceFilialeGoodsStock;
            NonceGoodsStock = nonceGoodsStock;
            SalesNumber = salesNumber;

        }

        /// <summary>
        /// ��ʼ�����캯��
        /// </summary>
        /// <param name="goodsId">��Ʒ���</param>
        /// <param name="specification">���</param>
        /// <param name="nonceFilialeGoodsStock">ָ����˾�Ŀ����</param>
        /// <param name="nonceRequest">��ǰ������</param>
        /// <param name="nonceGoodsStock">�ܿ����</param>
        /// <param name="salesNumber">���۶�</param>
        public StockWarningInfo(Guid goodsId, string specification, double nonceFilialeGoodsStock, int nonceRequest, double nonceGoodsStock, double salesNumber)
        {
            GoodsId = goodsId;
            Specification = specification;
            NonceFilialeGoodsStock = nonceFilialeGoodsStock;
            NonceRequest = nonceRequest;
            NonceGoodsStock = nonceGoodsStock;
            SalesNumber = salesNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsId">��ƷID</param>
        /// <param name="goodsCode">��Ʒ���</param>
        /// <param name="specification">���</param>
        /// <param name="nonceFilialeGoodsStock">ָ����˾�Ŀ����</param>
        /// <param name="nonceRequest">��ǰ������</param>
        /// <param name="nonceGoodsStock">�ܿ����</param>
        /// <param name="salesNumber">���۶�</param>
        /// <param name="goodsName">��Ʒ��</param>
        public StockWarningInfo(Guid goodsId, string goodsName, string goodsCode, string specification, double nonceFilialeGoodsStock, double nonceRequest, double nonceGoodsStock, double salesNumber)
        {
            GoodsId = goodsId;
            GoodsName = goodsName;
            GoodsCode = goodsCode;
            Specification = specification;
            NonceFilialeGoodsStock = nonceFilialeGoodsStock;
            NonceRequest = nonceRequest;
            NonceGoodsStock = nonceGoodsStock;
            SalesNumber = salesNumber;
        }

        public void SetStepSales(Dictionary<int, double> perDoubles)
        {
            PerStepSales = perDoubles;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public override bool Equals(object compareObj)
        {
            if (compareObj is StockWarningInfo)
                return (compareObj as StockWarningInfo).GoodsId == GoodsId;
            return base.Equals(compareObj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (GoodsId == Guid.Empty) return base.GetHashCode();
            string stringRepresentation = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "#" + GoodsId.ToString();
            return stringRepresentation.GetHashCode();
        }
    }
}
