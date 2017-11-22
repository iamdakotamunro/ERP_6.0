using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SalePlatformInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ExternalName { get; set; }

        public int AccountCheckingType { get; set; }
        
    }
}
