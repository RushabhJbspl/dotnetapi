using Worldex.Core.ApiModels;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Configuration
{
    public class OtherConfiguration
    {
    }
    public class OrderTypeResponse : BizResponseClass
    {
        public List<OrderTypeInfo> Response { get; set; }
    }
    public class OrderTypeInfo
    {
        public long ID { get; set; }
        public string OrderType { get; set; }
    }
    public class TransactionTypeResponse : BizResponseClass
    {
        public List<TransactionTypeInfo> Response { get; set; }
    }
    public class TransactionTypeInfo
    {
        public long Id { get; set; }
        public string TrnTypeName { get; set; }
    }

    public class ServiceTypeMasterResponse : BizResponseClass
    {
        public List<ServiceTypeMasterInfo> Response { get; set; }
    }
    public class ServiceTypeMasterInfo
    {
        public long Id { get; set; }
        public string SerTypeName { get; set; }
    }
}
