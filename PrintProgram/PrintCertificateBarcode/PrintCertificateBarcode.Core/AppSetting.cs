using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Common;

namespace PrintCertificateBarcode.Core
{
    public static class AppSetting
    {
        public static class Label
        {
            public static int With
            {
                get
                {
                    return Configuration.AppSettings["LabelWidth"].ToInt();
                }
            }

            public static int Height
            {
                get
                {
                    return Configuration.AppSettings["LabelHeight"].ToInt();
                }
            }

            public static float DPI
            {
                get
                {
                    return float.Parse(Configuration.AppSettings["LabelDPI"]);
                }
            }
        }

        public static  Guid JingTuoSaleFilialeID
        {
            get
            {
                return Configuration.AppSettings["JingTuoSaleFilialeID"].ToGuid();
            }
        }
    }
}
