using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Configuration
{
    public class ServiceProviderDetail : BizBase
    {
        [Required]
        public long ServiceProID { get; set; }

        [Required]
        public long ProTypeID { get; set; }

        [Required]
        public long AppTypeID { get; set; }

        [Required]
        public long TrnTypeID { get; set; }

        [Required]
        public long LimitID { get; set; }
        
        public long DemonConfigID { get; set; }

        [Required]
        public long ServiceProConfigID { get; set; }

        public long ThirPartyAPIID { get; set; }

        public string SerProDetailName { get; set; }

        public int IsStopConvertAmount { get; set; }//added IsStopConvertAmount 2019-5-13

        public void DisableProvider()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceProviderEvent<ServiceProviderDetail>(this));
        }
        public void EnableProvider()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceProviderEvent<ServiceProviderDetail>(this));
        }

        public void ChangeProviderType(enWebAPIRouteType proType)
        {
            switch (proType)
            {
                    case enWebAPIRouteType.CommunicationAPI :
                    ProTypeID = Convert.ToInt64(enWebAPIRouteType.CommunicationAPI);
                break;
                    case enWebAPIRouteType.LiquidityProvider :
                        ProTypeID = Convert.ToInt64(enWebAPIRouteType.LiquidityProvider);
                break;
                    case enWebAPIRouteType.MarketData :
                        ProTypeID = Convert.ToInt64(enWebAPIRouteType.MarketData);
                break;
                    case enWebAPIRouteType.PaymentGateway:
                        ProTypeID = Convert.ToInt64(enWebAPIRouteType.PaymentGateway);
                break;
                    case enWebAPIRouteType.TransactionAPI :
                        ProTypeID = Convert.ToInt64(enWebAPIRouteType.TransactionAPI);
                break;
            }
            Events.Add(new ServiceProviderEvent<ServiceProviderDetail>(this));
        }
        public void ChangeAppType(enAppType proType)
        {
            switch (proType)
            {
                case enAppType.BitcoinDeamon:
                    AppTypeID = Convert.ToInt64(enAppType.BitcoinDeamon);
                    break;
                case enAppType.HttpApi:
                    AppTypeID = Convert.ToInt64(enAppType.HttpApi);
                    break;
                case enAppType.JsonRPC:
                    AppTypeID = Convert.ToInt64(enAppType.JsonRPC);
                    break;
                case enAppType.RestAPI:
                    AppTypeID = Convert.ToInt64(enAppType.RestAPI);
                    break;
                case enAppType.SocketApi:
                    AppTypeID = Convert.ToInt64(enAppType.SocketApi);
                    break;
                case enAppType.TCPSocket :
                    AppTypeID = Convert.ToInt64(enAppType.TCPSocket);
                    break;
                case enAppType.WebSocket :
                    AppTypeID = Convert.ToInt64(enAppType.WebSocket);
                    break;
            }
            Events.Add(new ServiceProviderEvent<ServiceProviderDetail>(this));
        }
        public void ChangeTrnType(enTrnType trnType )
        {
            switch (trnType )
            {
                case enTrnType.Buy_Trade:
                    TrnTypeID = Convert.ToInt64(enTrnType.Buy_Trade);
                    break;
                case enTrnType.Charge :
                    TrnTypeID = Convert.ToInt64(enTrnType.Charge);
                    break;
                case enTrnType.Commission :
                    TrnTypeID = Convert.ToInt64(enTrnType.Commission);
                    break;
                case enTrnType.Deposit :
                    TrnTypeID = Convert.ToInt64(enTrnType.Deposit);
                    break;
                case enTrnType.Generate_Address :
                    TrnTypeID = Convert.ToInt64(enTrnType.Generate_Address);
                    break;
                case enTrnType.Sell_Trade:
                    TrnTypeID = Convert.ToInt64(enTrnType.Sell_Trade);
                    break;
                case enTrnType.Shoping_Cart :
                    TrnTypeID = Convert.ToInt64(enTrnType.Shoping_Cart);
                    break;
                case enTrnType.Topup:
                    TrnTypeID = Convert.ToInt64(enTrnType.Topup);
                    break;
                case enTrnType.Transaction :
                    TrnTypeID = Convert.ToInt64(enTrnType.Transaction);
                    break;
                case enTrnType.Withdraw :
                    TrnTypeID = Convert.ToInt64(enTrnType.Withdraw);
                    break;
            }
            Events.Add(new ServiceProviderEvent<ServiceProviderDetail>(this));
        }
    }

    //khushali 04-06-2019 for Arbitrage trading 
    public class ServiceProviderDetailArbitrage : BizBase
    {
        [Required]
        public long ServiceProID { get; set; }

        [Required]
        public long ProTypeID { get; set; }

        [Required]
        public long AppTypeID { get; set; }

        [Required]
        public long TrnTypeID { get; set; }

        [Required]
        public long LimitID { get; set; }

        public long DemonConfigID { get; set; }

        [Required]
        public long ServiceProConfigID { get; set; }

        public long ThirPartyAPIID { get; set; }

        public string SerProDetailName { get; set; }

        public int IsStopConvertAmount { get; set; }//added IsStopConvertAmount 2019-5-13

        public void DisableProvider()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceProviderEvent<ServiceProviderDetailArbitrage>(this));
        }
        public void EnableProvider()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceProviderEvent<ServiceProviderDetailArbitrage>(this));
        }

        public void ChangeProviderType(enWebAPIRouteType proType)
        {
            switch (proType)
            {
                case enWebAPIRouteType.CommunicationAPI:
                    ProTypeID = Convert.ToInt64(enWebAPIRouteType.CommunicationAPI);
                    break;
                case enWebAPIRouteType.LiquidityProvider:
                    ProTypeID = Convert.ToInt64(enWebAPIRouteType.LiquidityProvider);
                    break;
                case enWebAPIRouteType.MarketData:
                    ProTypeID = Convert.ToInt64(enWebAPIRouteType.MarketData);
                    break;
                case enWebAPIRouteType.PaymentGateway:
                    ProTypeID = Convert.ToInt64(enWebAPIRouteType.PaymentGateway);
                    break;
                case enWebAPIRouteType.TransactionAPI:
                    ProTypeID = Convert.ToInt64(enWebAPIRouteType.TransactionAPI);
                    break;
            }
            Events.Add(new ServiceProviderEvent<ServiceProviderDetailArbitrage>(this));
        }
        public void ChangeAppType(enAppType proType)
        {
            switch (proType)
            {
                case enAppType.BitcoinDeamon:
                    AppTypeID = Convert.ToInt64(enAppType.BitcoinDeamon);
                    break;
                case enAppType.HttpApi:
                    AppTypeID = Convert.ToInt64(enAppType.HttpApi);
                    break;
                case enAppType.JsonRPC:
                    AppTypeID = Convert.ToInt64(enAppType.JsonRPC);
                    break;
                case enAppType.RestAPI:
                    AppTypeID = Convert.ToInt64(enAppType.RestAPI);
                    break;
                case enAppType.SocketApi:
                    AppTypeID = Convert.ToInt64(enAppType.SocketApi);
                    break;
                case enAppType.TCPSocket:
                    AppTypeID = Convert.ToInt64(enAppType.TCPSocket);
                    break;
                case enAppType.WebSocket:
                    AppTypeID = Convert.ToInt64(enAppType.WebSocket);
                    break;
            }
            Events.Add(new ServiceProviderEvent<ServiceProviderDetailArbitrage>(this));
        }
        public void ChangeTrnType(enTrnType trnType)
        {
            switch (trnType)
            {
                case enTrnType.Buy_Trade:
                    TrnTypeID = Convert.ToInt64(enTrnType.Buy_Trade);
                    break;
                case enTrnType.Charge:
                    TrnTypeID = Convert.ToInt64(enTrnType.Charge);
                    break;
                case enTrnType.Commission:
                    TrnTypeID = Convert.ToInt64(enTrnType.Commission);
                    break;
                case enTrnType.Deposit:
                    TrnTypeID = Convert.ToInt64(enTrnType.Deposit);
                    break;
                case enTrnType.Generate_Address:
                    TrnTypeID = Convert.ToInt64(enTrnType.Generate_Address);
                    break;
                case enTrnType.Sell_Trade:
                    TrnTypeID = Convert.ToInt64(enTrnType.Sell_Trade);
                    break;
                case enTrnType.Shoping_Cart:
                    TrnTypeID = Convert.ToInt64(enTrnType.Shoping_Cart);
                    break;
                case enTrnType.Topup:
                    TrnTypeID = Convert.ToInt64(enTrnType.Topup);
                    break;
                case enTrnType.Transaction:
                    TrnTypeID = Convert.ToInt64(enTrnType.Transaction);
                    break;
                case enTrnType.Withdraw:
                    TrnTypeID = Convert.ToInt64(enTrnType.Withdraw);
                    break;
            }
            Events.Add(new ServiceProviderEvent<ServiceProviderDetailArbitrage>(this));
        }
    }

    //Rita 5-6-19 ,as no more reoute entry then make this separate and inner join with ServicePRoviderDetailArbitrage
    public class AllowesOrderTypeArbitrage : BizBase
    {
        [Required]
        public long SerProDetailID { get; set; }

        [Required]
        public long OrderType { get; set; }
    }
}
