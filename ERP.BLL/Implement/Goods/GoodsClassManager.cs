using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model.Goods;
using ERP.SAL.Interface;

namespace ERP.BLL.Implement.Goods
{
    public class GoodsClassManager
    {
        private readonly IGoodsCenterSao _goodsClassSao;
        public GoodsClassManager(IGoodsCenterSao goodsClassSao)
        {
            _goodsClassSao = goodsClassSao;
        }

        #region --> 递归返回商品树分类 （通用）

        /// <summary>递归返回商品树分类 （通用）
        /// </summary>
        /// <returns></returns>
        public IList<GoodsClassInfo> GetGoodsClassListWithRecursion()
        {
            return GetChildClassList(Guid.Empty, 0, _goodsClassSao.GetAllClassList());
        }

        public IList<GoodsClassInfo> GetChildClassList(Guid parentClassId, int depth, IEnumerable<GoodsClassInfo> allClassList)
        {
            depth++;
            IList<GoodsClassInfo> classTree = new List<GoodsClassInfo>();
            var goodsClassInfos = allClassList as List<GoodsClassInfo> ?? allClassList.ToList();
            var goodsClassList = goodsClassInfos.Where(w => w.ParentClassId == parentClassId).OrderBy(act => act.OrderIndex).ToList();

            string tag = depth == 1 ? "+" : "|" + new String('-', depth * 2);
            foreach (var info in goodsClassList)
            {
                info.ClassName = tag + info.ClassName;
                classTree.Add(info);
                foreach (GoodsClassInfo childGoodsClassInfo in GetChildClassList(info.ClassId, depth, goodsClassInfos))
                {
                    classTree.Add(childGoodsClassInfo);
                }
            }
            return classTree;
        }
        #endregion
        
    }
}
