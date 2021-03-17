using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.BackOfficeReports
{
    public class GetSubscribeNewLetter
    {
        public long ID { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public short Status { get; set; }
    }
    public class GetSubscribeNewLetterResponse :BizResponseClass
    {
        public long Count { get; set; }
        public long PageCount { get; set; }
        public List<GetSubscribeNewLetter> Response { get; set; }
    }
    public class GetSubscribeNewLetterCountResponse : BizResponseClass
    {
        public long Count { get; set; }
    }
}
