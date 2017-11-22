using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.Model.ShopFront
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ShopLensProcessInfo
    {
        public LensProcessInfo LensProcess { get; set; }

        public IList<LensProcessDetailsInfo> LensProcessDetails { get; set; }
    }
}
