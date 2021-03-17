using System;

namespace Worldex.Core.ViewModels.Complaint
{
    public class CompainTrailViewModel
    {
        public long TrailID { get; set; }
        public string Description { get; set; }
        public string Complainstatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Username { get; set; }
    }
}
