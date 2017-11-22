using System;
using System.Web;

namespace ERP.UI.Web.Common
{
    /// <summary>
    /// function:make sure the submit operation happen in a time slice
    /// coder:dyy
    /// date:2009 Dec 31
    /// </summary>
    [Serializable]
    public class SubmitController
    {
        private readonly Int32 _waitsSecond;

        /// <summary> call this method when you have a sucessful submit
        /// </summary>
        public void Submit()
        {
            HttpRuntime.Cache.Insert(SubmitID.ToString(), DateTime.Now);
        }

        protected Guid SubmitID;

        /// <summary>is the submit allowed or not 
        /// </summary>
        public bool Enabled
        {
            get
            {
                var obj = HttpRuntime.Cache.Get(SubmitID.ToString());
                if (obj == null)
                {
                    return true;
                }
                DateTime dtSumbit = (DateTime)obj;
                TimeSpan ts = DateTime.Now - dtSumbit;
                if (ts.TotalSeconds >= _waitsSecond)
                {
                    return true;
                }
                return false;
            }
        }

        public SubmitController()
        {
            _waitsSecond = 60;
            if (SubmitID == Guid.Empty)
                SubmitID = Guid.NewGuid();
        }

        public SubmitController(Guid guPass)
        {
            _waitsSecond = 60;
            SubmitID = guPass;
        }

        public SubmitController(Guid guPass, Int32 second)
        {
            _waitsSecond = second;
            SubmitID = guPass;
        }
    }
}
