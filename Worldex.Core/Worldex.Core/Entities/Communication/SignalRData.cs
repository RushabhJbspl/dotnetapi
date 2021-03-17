using Worldex.Core.Enums;
using Worldex.Core.ViewModels;
using MediatR;
using System;

namespace Worldex.Core.Entities.Communication
{    
    public class SignalRData: IRequest<CommunicationResponse>
    {
        public string DataObj { get; set; }
        public enMethodName Method { get; set; }
        public string Parameter { get; set; }
        public string WalletName { get; set; }
    }
    public class ThirdPartyAPISinalR : IRequest<CommunicationResponse>
    {
        public string DataObj { get; set; }
        public enMethodName Method { get; set; }
        public string UserID { get; set; }
        public string Parameter { get; set; }
        public string WalletName { get; set; }
        public short IsPrivate { get; set; }
    }
    public class TradeHistoryInfo
    {
        public long TrnNo { get; set; }
        public string Type { get; set; }
        public Decimal Price { get; set; }
        public Decimal Amount { get; set; }
        public Decimal Total { get; set; }
        public DateTime DateTime { get; set; }
        public short Status { get; set; }
        public string StatusText { get; set; }
        public String PairName { get; set; }
        public Decimal ChargeRs { get; set; }
        public short IsCancel { get; set; }
    }
}
