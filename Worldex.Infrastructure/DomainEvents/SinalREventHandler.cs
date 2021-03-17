using Worldex.Core.ApiModels;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Communication;
using Worldex.Core.Entities.SignalR;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.FeedConfiguration;
using Worldex.Core.SignalR;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Worldex.Infrastructure.DomainEvents
{
    public class SinalREventHandler : IRequestHandler<SignalRData, CommunicationResponse>
    {
        private SocketHub _chat;
        public SinalREventHandler(SocketHub chat)
        {
            _chat = chat;
        }
        public async Task<CommunicationResponse> Handle(SignalRData request, CancellationToken cancellationToken)
        {
            CommunicationResponse response = new CommunicationResponse();
            try
            {
                switch (request.Method)
                {
                    case enMethodName.BuyerBook:
                        await _chat.BuyerBook(request.Parameter, request.DataObj);   break;
                    case  enMethodName.SellerBook:
                            await _chat.SellerBook(request.Parameter, request.DataObj);   break;
                        case  enMethodName.StopLimitBuyerBook:
                            await _chat.StopLimitBuyerBook(request.Parameter, request.DataObj);   break;
                        case  enMethodName.StopLimitSellerBook:
                            await _chat.StopLimitSellerBook(request.Parameter, request.DataObj);   break;
                        case  enMethodName.OrderHistory:
                            await _chat.OrderHistory(request.Parameter, request.DataObj);   break;
                        case  enMethodName.ChartData:
                            await _chat.ChartData(request.Parameter, request.DataObj);   break;
                        case  enMethodName.MarketData:
                            await _chat.MarketData(request.Parameter, request.DataObj);   break;
                        case  enMethodName.ActiveOrder:
                            await _chat.ActiveOrder(request.Parameter, request.DataObj);   break;
                        case  enMethodName.OpenOrder:
                            await _chat.OpenOrder(request.Parameter, request.DataObj);   break;
                        case  enMethodName.TradeHistory:
                            await _chat.TradeHistory(request.Parameter, request.DataObj);   break;
                        case  enMethodName.RecentOrder:
                            await _chat.RecentOrder(request.Parameter, request.DataObj);   break;
                        case  enMethodName.BuyerSideWallet:
                            await _chat.WalletBalUpdate(request.Parameter, request.WalletName, request.DataObj);   break;
                        case  enMethodName.SellerSideWallet:
                            await _chat.WalletBalUpdate(request.Parameter, request.WalletName, request.DataObj);   break;
                        case  enMethodName.Price:
                            await _chat.LastPrice(request.Parameter, request.DataObj);   break;
                        case  enMethodName.PairData:
                            await _chat.PairData(request.Parameter, request.DataObj);   break;
                        case  enMethodName.MarketTicker:
                            await _chat.MarketTicker(request.Parameter, request.DataObj);   break;
                        case  enMethodName.ActivityNotification:
                            await _chat.ActivityNotification(request.Parameter, request.DataObj);   break;
                        case  enMethodName.News:
                            await _chat.BroadCastNews(request.DataObj);   break;
                        case  enMethodName.Announcement:
                            await _chat.BroadCastAnnouncement(request.DataObj);   break;
                        //case  enMethodName.SendGroupMessage:
                        //    await _chat.SendGroupMessage(request.Parameter, request.DataObj);   break;
                        case  enMethodName.Time:
                            await _chat.GetTime(request.DataObj);   break;
                        case  enMethodName.WalletActivity:
                            await _chat.WalletActivity(request.Parameter, request.DataObj);   break;
                        case  enMethodName.SessionExpired:
                            await _chat.OnSessionExpired(request.Parameter, request.DataObj);   break;
                        case enMethodName.EnvironmentMode:
                        await _chat.EnvironmentMode(request.DataObj); break;
                }

                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(response);
            }
        }
    }
}
