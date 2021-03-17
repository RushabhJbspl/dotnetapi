using System;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities
{
    public class TrnAcBatch : BizBase
    {
        public TrnAcBatch()
        {
            CreatedBy = 900;
            CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            UpdatedBy = 900;
            Status = 1;
        }
    }
}
