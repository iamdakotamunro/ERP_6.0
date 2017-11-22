using System;

namespace CMS.WCF.ExceptionService.Model
{
    [Serializable]
    public class CommandInfo
    {
        public Guid CommandID { get; set; }
        public string CommandMethod { get; set; }
        public byte[] CommandParameter { get; set; }
        public DateTime CommandDate { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsSendCommand { get; set; }
        public string Exception { get; set; }
        public int? ExceptionCount { get; set; }
        public int? ExceptionTotalCount { get; set; }
        public Guid? IdentifyID { get; set; }
    }
}
