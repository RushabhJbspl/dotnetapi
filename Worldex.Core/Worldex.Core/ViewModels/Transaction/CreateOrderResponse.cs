using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction
{
    public class CreateTransactionResponse : BizResponseClass
    { 
        public CreateOrderInfo response { get; set; }
    }
    public class CreateOrderInfo
    {
        //public long TrnNo { get; set; }      
        public Guid TrnID { get; set; }
        //[DataMember]
        //public RateInfo RateInfo { get; set; }
    }
    public class CloseOpenPostionResponseMargin : BizResponseClass
    {
        
    }
}
