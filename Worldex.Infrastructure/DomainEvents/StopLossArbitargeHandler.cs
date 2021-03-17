using Worldex.Core.ApiModels;
using Worldex.Core.Entities;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Repository;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.DTOClasses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Worldex.Core.Entities.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Worldex.Core.Entities.Transaction;

namespace Worldex.Infrastructure.DomainEvents
{
    public class StopLossArbitargeHandler : IRequestHandler<StopLossClsArbitarge>
    {
        private readonly ICommonRepository<TradeTransactionQueueArbitrage> _tradeTrnRepositiory;
        private readonly ICommonRepository<TransactionQueueArbitrage> _trnRepositiory;
        private readonly IFrontTrnRepository _frontTrnRepository;
        private readonly IMediator _mediator;
        private readonly ILPStatusCheckArbitrage<LPStatusCheckDataArbitrage> _lPStatusCheckQueue;
        TransactionQueueArbitrage TransactionQueuecls;
        string ControllerName = "StopLossArbitargeHandler";
        private readonly ICommonRepository<CronMaster> _cronMaster;
        private IMemoryCache _cache;
        private readonly ITransactionQueue<NewCancelOrderArbitrageRequestCls> _TransactionQueueCancelOrderArbitrage;

        public StopLossArbitargeHandler(IFrontTrnRepository FrontTrnRepository, ICommonRepository<TradeTransactionQueueArbitrage> TradeTrnRepositiory,
            ICommonRepository<TransactionQueueArbitrage> TrnRepositiory, IMediator mediator, ILPStatusCheckArbitrage<LPStatusCheckDataArbitrage> LPStatusCheckQueue,
            ICommonRepository<CronMaster> CronMaster, IMemoryCache cache, ITransactionQueue<NewCancelOrderArbitrageRequestCls> TransactionQueueCancelOrderArbitrage)
        {
            _tradeTrnRepositiory = TradeTrnRepositiory;
            _trnRepositiory = TrnRepositiory;
            _frontTrnRepository = FrontTrnRepository;
            _mediator = mediator;
            _lPStatusCheckQueue = LPStatusCheckQueue;
            _cronMaster = CronMaster;
            _cache = cache;
            _TransactionQueueCancelOrderArbitrage = TransactionQueueCancelOrderArbitrage;
        }

        public async Task<Unit> Handle(StopLossClsArbitarge request, CancellationToken cancellationToken)
        {
            List<StopLossArbitargeResponse> Data = new List<StopLossArbitargeResponse>();
            CronMaster cronMaster = new CronMaster();
            try
            {
                List<CronMaster> cronMasterList = _cache.Get<List<CronMaster>>("CronMaster");
                if (cronMasterList == null)
                {
                    cronMasterList = _cronMaster.List();
                    _cache.Set("CronMaster", cronMasterList);
                }
                else if (cronMasterList.Count() == 0)
                {
                    cronMasterList = _cronMaster.List();
                    _cache.Set("CronMaster", cronMasterList);
                }
                cronMaster = cronMasterList.Where(e => e.Id == (short)enCronMaster.StopLossArbitrage).FirstOrDefault();
                if (cronMaster != null && cronMaster.Status == (short)ServiceStatus.Active)
                {
                    Data = _frontTrnRepository.StopLossArbitargeCron();
                    foreach (var item in Data)
                    {
                        List<TradeStopLossArbitarge> TradeStopLossArbitargeList = _cache.Get<List<TradeStopLossArbitarge>>("TradeStopLossArbitargeList");
                        if (TradeStopLossArbitargeList == null)
                        {
                            TradeStopLossArbitargeList = new List<TradeStopLossArbitarge>();
                            
                        }
                        TradeStopLossArbitarge FindRecord = TradeStopLossArbitargeList.Where(e => e.TrnNo == item.TrnNo).FirstOrDefault();
                        if (FindRecord != null)
                        {
                            FindRecord.MaxTry++;
                        }
                        else
                        {
                            FindRecord = new TradeStopLossArbitarge { TrnNo = item.TrnNo, MaxTry = 1 };
                            TradeStopLossArbitargeList.Add(FindRecord);
                        }
                        _cache.Set<List<TradeStopLossArbitarge>>("TradeStopLossArbitargeList", TradeStopLossArbitargeList);
                        if (FindRecord.MaxTry <= 3)
                        {
                            _cache.Set("CronMaster", cronMasterList);
                            _TransactionQueueCancelOrderArbitrage.Enqueue(new NewCancelOrderArbitrageRequestCls()
                            {
                                MemberID = item.MemberID,
                                TranNo = item.TrnNo,
                                accessToken = "",
                                CancelAll = 0,
                                OrderType = 0
                            });
                        }                        
                    }
                }
                return await Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("LPStatusCheckHandlerArbitrage Error:##GUID " + request.uuid, ControllerName, ex);
                return await Task.FromResult(new Unit());
            }
        }
    }
}
