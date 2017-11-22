using System;
using System.Collections.Generic;

namespace ERP.Model
{
    [Serializable]
    public class SearchGoodsInfo
    {
        public Guid GoodsId { get; set; }

        public string GoodsCode { get; set; }

        public string GoodsName { get; set; }

        public List<FieldInfo> Luminositys { get; set; }

        public List<FieldInfo> Astigmias { get; set; }

        public List<FieldInfo> Axialss { get; set; }
    }
}
