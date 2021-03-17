using System;
using System.Collections.Generic;
using System.Text;
using Worldex.Core.Enums;

namespace Worldex.Infrastructure.DTOClasses
{
    public class BizResponse
    {
        public enResponseCodeService ReturnCode { get; set; }
        public enErrorCode ErrorCode { get; set; }
        public string ReturnMsg { get; set; }
    }

    public class FiatBizResponse : BizResponse
    {
        public long TrnNo { get; set; }
    }
}
