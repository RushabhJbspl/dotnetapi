using CleanArchitecture.Core.Entities.Transaction;
using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Core.Interfaces.Repository;
using CleanArchitecture.Infrastructure.DTOClasses;
using CleanArchitecture.Infrastructure.Interfaces;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Web.BackgroundTask
{
    internal class TimedHostedPairStatisticsCalService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        //private readonly ICommonRepository<TradePairStastics>  _tradePairStastics;
        //private readonly IFrontTrnRepository _frontTrnRepository;
        //private readonly IBasePage _basePage;
        private readonly IMediator _mediator;

        public TimedHostedPairStatisticsCalService(ILogger<TimedHostedService> logger, IMediator mediator)//ICommonRepository<TradePairStastics> tradePairStastics, IFrontTrnRepository frontTrnRepository, IBasePage basePage
        {
            _logger = logger;
            //_tradePairStastics = tradePairStastics;
            //_frontTrnRepository = frontTrnRepository;
            //_basePage = basePage;
            _mediator = mediator;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }
        //Rita 23-1-19 LTP change issue solved, as this service only take starting data,then update old one
        private void DoWork(object state)
        {
            try
            {
                _mediator.Send(new PairDataCalculationCls { TempVar = "" });
                //_logger.LogInformation("Timed Background Service for ChartData is working.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }

        //private void PairDataCalculation(object state)
        //{
        //    List<TradePairStastics> PairDataUpdated = new List<TradePairStastics>();

        //    var PairData = _tradePairStastics.GetAll().ToList();
        //    var PairCalcData = _frontTrnRepository.GetPairStatisticsCalculation();
        //    if (PairCalcData != null)
        //    {
        //        if (PairData.Count > 0 && PairCalcData.Count > 0)
        //        {
        //            foreach (var PairStatisticsObj in PairData)
        //            {

        //                var PairCalcObj = PairCalcData.Where(x => x.PairId == PairStatisticsObj.PairId).FirstOrDefault();

        //                if (PairCalcObj != null)
        //                {
        //                    var HighLowDataDay = _frontTrnRepository.GetHighLowValue(PairStatisticsObj.PairId, -1);
        //                    var HighLowDataWeek = _frontTrnRepository.GetHighLowValue(PairStatisticsObj.PairId, -7);
        //                    var HighLowDataYear = _frontTrnRepository.GetHighLowValue(PairStatisticsObj.PairId, -365);


        //                    PairStatisticsObj.ChangeVol24 = PairCalcObj.Volume;
        //                    PairStatisticsObj.ChangePer24 = PairCalcObj.ChangePer;
        //                    PairStatisticsObj.ChangeValue = PairCalcObj.ChangeValue;

        //                    if (HighLowDataDay != null)
        //                    {
        //                        PairStatisticsObj.Low24Hr = HighLowDataDay.LowPrice;
        //                        PairStatisticsObj.High24Hr = HighLowDataDay.HighPrice;
        //                    }
        //                    if (HighLowDataWeek != null)
        //                    {
        //                        PairStatisticsObj.LowWeek = HighLowDataWeek.LowPrice;
        //                        PairStatisticsObj.LowWeek = HighLowDataWeek.HighPrice;
        //                    }
        //                    if (HighLowDataYear != null)
        //                    {
        //                        PairStatisticsObj.Low52Week = HighLowDataYear.LowPrice;
        //                        PairStatisticsObj.High52Week = HighLowDataYear.HighPrice;
        //                    }

        //                    //Calculate Up DownBit
        //                    if (PairStatisticsObj.ChangePer24 < PairCalcObj.ChangePer)
        //                    {
        //                        PairStatisticsObj.UpDownBit = 1;
        //                    }
        //                    else if (PairStatisticsObj.ChangePer24 > PairCalcObj.ChangePer)
        //                    {
        //                        PairStatisticsObj.UpDownBit = 0;
        //                    }

        //                    PairStatisticsObj.CronDate = _basePage.UTC_To_IST();

        //                    PairDataUpdated.Add(PairStatisticsObj);
        //                }
        //            }
        //            if (PairDataUpdated.Count > 0)
        //            {
        //                _frontTrnRepository.UpdatePairStatisticsCalculation(PairDataUpdated);
        //            }
        //        }
        //    }
        //}

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service For PaiCalculation is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
