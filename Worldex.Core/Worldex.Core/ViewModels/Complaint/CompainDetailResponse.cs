using System;

namespace Worldex.Core.ViewModels.Complaint
{
    public class CompainDetailResponse  
    {
        public long CompainNumber { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Complainstatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Userid { get; set; }
        public string Username { get; set; }
        public long CompainTrailID { get; set; }
    }
}
