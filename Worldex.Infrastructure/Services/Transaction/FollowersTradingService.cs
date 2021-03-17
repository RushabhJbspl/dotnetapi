using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.Configuration;
using Worldex.Core.ViewModels.Configuration;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.Transaction
{
    public class FollowersTradingService : IFollowersTrading
    {
        private readonly ITransactionProcessV1 _transactionProcessV1;
        private readonly IWalletService _WalletService;
        private readonly IProfileConfigurationService _ProfileConfigurationService;
        private readonly ITransactionQueue<NewTransactionRequestCls> _iTransactionQueue;
        string ControllerName = "FollowersTradingService";
        public FollowersTradingService(ITransactionProcessV1 transactionProcessV1, IWalletService WalletService, 
            IProfileConfigurationService ProfileConfigurationService, ITransactionQueue<NewTransactionRequestCls> iTransactionQueue)
        {
            _transactionProcessV1 = transactionProcessV1;
            _WalletService = WalletService;
            _ProfileConfigurationService = ProfileConfigurationService;
            _iTransactionQueue = iTransactionQueue;
        }
        public async Task<BizResponse> ProcessFollowersNewTransactionAsync(FollowersOrderRequestCls request)
        {
            NewTransactionRequestCls LeaderTradeReq = new NewTransactionRequestCls();
            long GUserID = 0;
            try
            {
                //Task.Delay(10000).Wait();
                LeaderTradeReq = CopyClass.DeepCopy(request.Req);

                //order type from back office
                if (request.Req.ISFollowersReq != 0)//Not Main request
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("Not Main Request " + request.Req.ISFollowersReq, ControllerName, "##TrnNo:" + LeaderTradeReq.TrnNo, Helpers.UTC_To_IST()));
                    return new BizResponse { ReturnMsg = "", ReturnCode = enResponseCodeService.Fail };
                }

                //order type from back office
                if (LeaderTradeReq.ordertype != enTransactionMarketType.LIMIT)
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("OrderType Not Allowed " + LeaderTradeReq.ordertype, ControllerName, "##TrnNo:" + LeaderTradeReq.TrnNo, Helpers.UTC_To_IST()));
                    return new BizResponse { ReturnMsg = "", ReturnCode = enResponseCodeService.Fail };                   
                }

                //==========If leader check
                LeaderwiseFollower FollowerListResult=new LeaderwiseFollower();//Rita 18-3-19 as second operation gives error 
                try
                {
                    FollowerListResult = _ProfileConfigurationService.GetLeaderWiseFollowerCongfigList(LeaderTradeReq.MemberID);
                }
                catch(Exception ex)
                {
                    HelperForLog.WriteErrorLog("Error in GetLeaderWiseFollowerCongfigList UserID:" + GUserID + ":##TrnNo " + LeaderTradeReq.TrnNo + " GUID:" + LeaderTradeReq.GUID, ControllerName, ex);
                }
                if (FollowerListResult == null)
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("FollowerListResult NULL User:" + LeaderTradeReq.MemberID, ControllerName, "##TrnNo:" + LeaderTradeReq.TrnNo, Helpers.UTC_To_IST()));                   
                    return new BizResponse { ReturnMsg = "", ReturnCode = enResponseCodeService.Fail };
                }               
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("FollowerListResult FOUND:" + FollowerListResult.ProfileType, ControllerName, "##TrnNo:" + LeaderTradeReq.TrnNo, Helpers.UTC_To_IST()));
                if (FollowerListResult.ProfileType == false)
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("FollowerListResult Not Leader:" + LeaderTradeReq.MemberID, ControllerName, "##TrnNo:" + LeaderTradeReq.TrnNo, Helpers.UTC_To_IST()));                   
                    return new BizResponse { ReturnMsg = "", ReturnCode = enResponseCodeService.Fail };
                }

                //===========get all followers
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("FollowerListResult Loop START:", ControllerName, "##TrnNo:" + LeaderTradeReq.TrnNo, Helpers.UTC_To_IST()));
                foreach (ConfigListFollower Follower in FollowerListResult.FollowerList)
                {
                    GUserID = Follower.Id;
                    //time validation
                    //decimal CopyPer = 50;//assign from list
                    decimal FollowersQty = 0;

                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("Follower Name : " + Follower.UserName + " UserID : " + Follower.Id + " Follower.ConfigKey:" + Follower.ConfigKey + " userconfig:" + Follower.userconfig, ControllerName, "##TrnNo:" + LeaderTradeReq.TrnNo, Helpers.UTC_To_IST()));
                    //Copy-per validation
                    if (Follower.ConfigKey.ToUpper() == "CAN_MIRROR_TRADE" && Follower.userconfig.ToUpper() == "YES")//Mirror , same trade
                    {
                        FollowersQty = LeaderTradeReq.Qty;                        
                    }
                    else if (Follower.ConfigKey.ToUpper() == "CAN_COPY_TRADE" && Follower.userconfig.ToUpper() == "YES")//Copy, Per Trade
                    {
                        if (Follower.TradePercentage<=0 || Follower.TradePercentage > 100)
                        {
                            Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("LOOP continue Wrong Copy Percentage:" + Follower.TradePercentage + " UserID : " + Follower.Id, ControllerName, "##TrnNo:" + LeaderTradeReq.TrnNo, Helpers.UTC_To_IST()));
                            continue;
                        }
                        FollowersQty = Helpers.DoRoundForTrading(LeaderTradeReq.Qty * Follower.TradePercentage / 100, 18);
                    }
                    else
                    {
                        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("LOOP continue - userconfig FAIL:" + Follower.ConfigKey + " UserID : " + Follower.Id, ControllerName, "##TrnNo:" + LeaderTradeReq.TrnNo, Helpers.UTC_To_IST()));
                        continue;
                    }

                    Guid FollowersGUID = Guid.NewGuid();
                    string DebitAccountID = "";
                    string CreditAccountID = "";
                    DebitAccountID = await _WalletService.GetDefaultAccWalletID(request.Order_Currency, Follower.Id);
                    CreditAccountID = await _WalletService.GetDefaultAccWalletID(request.Delivery_Currency, Follower.Id);

                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("Follower DebitAccountID:" + DebitAccountID + " CreditAccountID:" + CreditAccountID + " UserID : " + Follower.Id + " GUID:" + FollowersGUID, ControllerName, "##TrnNo:" + LeaderTradeReq.TrnNo, Helpers.UTC_To_IST()));

                    _iTransactionQueue.Enqueue(new NewTransactionRequestCls()
                    {
                        TrnMode = LeaderTradeReq.TrnMode,
                        TrnType = LeaderTradeReq.TrnType,
                        ordertype = LeaderTradeReq.ordertype,
                        SMSCode = Follower.ConfigKey,
                        FollowTradeType = Follower.ConfigKey,//Rita 15-3-19 as remain to update
                        TransactionAccount = LeaderTradeReq.TransactionAccount.ToString(),
                        Amount = 0,//auto calculation
                        PairID = LeaderTradeReq.PairID,
                        Price = LeaderTradeReq.Price,
                        Qty = FollowersQty,
                        DebitAccountID = DebitAccountID,
                        CreditAccountID = CreditAccountID,
                        StopPrice = LeaderTradeReq.StopPrice,
                        GUID = FollowersGUID,
                        MemberID = Follower.Id,
                        MemberMobile = Follower.Mobile,                       
                        accessToken = "",
                        ISFollowersReq = 1,
                        FollowingTo = LeaderTradeReq.MemberID,
                        LeaderTrnNo = LeaderTradeReq.TrnNo
                    });

                }
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("FollowerListResult Loop END:", ControllerName, "##TrnNo:" + LeaderTradeReq.TrnNo, Helpers.UTC_To_IST()));
                return new BizResponse { ReturnMsg = "", ReturnCode = enResponseCodeService.Success };
            }
            catch(Exception ex)
            {
                HelperForLog.WriteErrorLog("ProcessFollowersNewTransactionAsync Internal Error UserID:"+ GUserID + ":##TrnNo " + LeaderTradeReq.TrnNo + " GUID:" + LeaderTradeReq.GUID, ControllerName, ex);
                return new BizResponse { ReturnMsg = "", ReturnCode = enResponseCodeService.InternalError };
            }
        }
    }
}
