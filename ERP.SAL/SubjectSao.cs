using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model;
using ERP.Model.Goods;
using ERP.SAL.Interface;

namespace ERP.SAL
{
    /// <summary>
    /// 票据处理——会计科目
    /// </summary>
    public class SubjectSao : ISubjectSao
    {
        /// <summary>
        /// 获取所有会计科目分类
        /// </summary>
        /// <returns></returns>
        public List<SubjectClassInfo> GetAllSubjectClassList()
        {
            //TODO:待定
            var g1 = Guid.Parse("2b8300cc-122d-460d-9f87-75115a073620");
            var g2 = Guid.Parse("2b8300cc-122d-460d-9f87-75115a073622");
            var g2_1 = Guid.Parse("2b8300cc-122d-460d-9f87-75115a073621");
            var g3 = Guid.Parse("2b8300cc-122d-460d-9f87-75115a073623");
            var list = new List<SubjectClassInfo>();
            list.Add(new SubjectClassInfo()
            {
                ClassId = g1,
                ClassName = "1级",
                OrderIndex = 1,
                ParentClassId = Guid.Empty,
            });

            list.Add(new SubjectClassInfo()
            {
                ClassId = g2,
                ClassName = "1.1级",
                OrderIndex = 20,
                ParentClassId = g1,
            });

            list.Add(new SubjectClassInfo()
            {
                ClassId = g2_1,
                ClassName = "1.1.1级",
                OrderIndex = 30,
                ParentClassId = g2,
            });

            list.Add(new SubjectClassInfo()
            {
                ClassId = g3,
                ClassName = "1.2级",
                OrderIndex = 40,
                ParentClassId = g1,
            });

            return list;
        }

        /// <summary>
        /// 添加会计科目
        /// </summary>
        /// <param name="subjectClassInfo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool Add(SubjectInfo subjectClassInfo, out string errorMessage)
        {
            //TODO:待定
            throw new NotImplementedException();
        }

        /// <summary>
        /// 删除会计科目
        /// </summary>
        /// <param name="subjectID"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool Delete(int subjectID, out string errorMessage)
        {
            //TODO:待定
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更新会计科目
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool Update(SubjectInfo model, out string errorMessage)
        {
            //TODO:待定
            throw new NotImplementedException();
        }
    }
}