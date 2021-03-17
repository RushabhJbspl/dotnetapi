using Worldex.Core.ApiModels;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Charges;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.MarginEntitiesWallet;
using Worldex.Core.Entities.NewWallet;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

//Chirag 11-06-2019 Added
namespace Worldex.Core.ViewModels.Wallet
{
    class InsertUpdateArbitrageChargeConf
    {
    }

    public class InsertUpdateArbitrageChargeConfigurationMasterReq
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "1,Please enter Required Param,17086")]
        public long WalletTypeId { get; set; }

        [Required(ErrorMessage = "1,Please enter Required Param,4601")]
        public long PairID { get; set; }

        [Required(ErrorMessage = "1,Please enter Required Param,17087")]
        public long SerProID { get; set; }

        [Required(ErrorMessage = "1,Please enter Required Param,4710")]
        public long TrnType { get; set; }

        [Required(ErrorMessage = "1,Please enter Required Param,4458")]
        public short KYCComplaint { get; set; }

        [Required(ErrorMessage = "1,Please enter Required Param,31026")]
        public short Status { get; set; }

        public string Remarks { get; set; }
    }

    public class InsertUpdateArbitrageChargeConfigurationDetailReq
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "1,Please enter Required Param,31026")]
        public short Status { get; set; }

        [Required(ErrorMessage = "1,Please enter Required Param,4459")]
        public long ChargeConfigurationMasterID { get; set; }

        [Required(ErrorMessage = "1,Please enter Required Param,4460")]
        public Int16 ChargeDistributionBasedOn { get; set; }

        [Required(ErrorMessage = "1,Please enter Required Param,4462")]
        public long ChargeType { get; set; }

        [Required(ErrorMessage = "1,Please enter Required Param,4726")]
        public decimal ChargeValue { get; set; }

        [Required(ErrorMessage = "1,Please enter Required Param,4463")]
        public Int16 ChargeValueType { get; set; }

        [Required(ErrorMessage = "1,Please enter Required Param,14068")]
        public decimal MakerCharge { get; set; }

        [Required(ErrorMessage = "1,Please enter Required Param,14069")]
        public decimal TakerCharge { get; set; }

        [Required(ErrorMessage = "1,Please enter Required Param,17071")]
        public string Remarks { get; set; }

        public Int16 IsCurrencyConverted { get; set; }

        [Required(ErrorMessage = "1,Please enter Required Param,4461")]
        public long DeductionWalletTypeId { get; set; }
    }

    public class InsertUpdateChargeConfigurationMasterArbitrageReq
    {
        public long Id { get; set; }

        public long WalletTypeId { get; set; }

        public long TrnType { get; set; }

        public short KYCComplaint { get; set; }

        [Required(ErrorMessage = "1,Please enter Required Param,31026")]
        public short Status { get; set; }

        [Required(ErrorMessage = "1,Please enter Required Param,4436")]
        public short SlabType { get; set; }

        //[Required(ErrorMessage = "1,Please enter Required Param,14067")]
        public long SpecialChargeConfigurationID { get; set; }

        public string Remarks { get; set; }
    }

    public class ListArbitrageChargeConfigurationMasterRes : BizResponseClass
    {
        public List<ArbitrageChargeConfigurationMasterRes> Data { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public int TotalCount { get; set; }
    }

    public class ListChargeConfigurationMasterArbitrageRes : BizResponseClass
    {
        public List<ChargeConfigurationMasterArbitrageRes> Data { get; set; }
        public int? PageNo { get; set; }
        public int? PageSize { get; set; }
        public int TotalCount { get; set; }
    }

    public class ChargeConfigurationMasterArbitrageRes
    {
        public Int64 Id { get; set; }
        public Int16 Status { get; set; }
        public string StrStatus { get; set; }
        public Int64 WalletTypeID { get; set; }
        public string WalletTypeName { get; set; }
        public Int64 TrnType { get; set; }
        public string TrnTypeName { get; set; }
        public Int16 KYCComplaint { get; set; }
        public Int16 SlabType { get; set; }
        public string SlabTypeName { get; set; }
        public Int64 SpecialChargeConfigurationID { get; set; }
        public string Remarks { get; set; }
    }

    public class ListLPArbitrageWalletMismatchRes : BizResponseClass
    {
        public List<LPArbitrageWalletMismatchRes> Data { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
        public long PageNo { get; set; }
    }

    public class LPArbitrageWalletMismatchRes
    {
        public string Id { get; set; }
        public Int16 Status { get; set; }
        public string StrStatus { get; set; }
        public Int64 WalletTypeID { get; set; }
        public string WalletTypeName { get; set; }
        public long SerProID { get; set; }
        public string SerProIDName { get; set; }
        public decimal MisMatchingAmount { get; set; }
        public decimal SettledAmount { get; set; }
        public long ResolvedBy { get; set; }
        public string ResolvedByName { get; set; }
        public DateTime ResolvedDate { get; set; }
        public string Remarks { get; set; }
    }

    public class ArbitrageChargeConfigurationMasterRes
    {
        public Int64 Id { get; set; }
        public Int16 Status { get; set; }
        public string StrStatus { get; set; }
        public Int64 WalletTypeID { get; set; }
        public string WalletTypeName { get; set; }
        public Int64 PairID { get; set; }
        public string PairName { get; set; }
        public Int64 SerProID { get; set; }
        public string ProviderName { get; set; }
        public Int64 TrnType { get; set; }
        public string TrnTypeName { get; set; }
        public Int16 KYCComplaint { get; set; }
        public string Remarks { get; set; }
    }

    public class ListProviderWalletLedgerResv1 : BizResponseClass
    {
        public List<ProviderWalletLedgerRes> ProviderWalletLedgers { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
        public long PageNo { get; set; }
    }

    public class ProviderWalletLedgerRes
    {
        public long LedgerId { get; set; }

        public decimal PreBal { get; set; }

        public decimal PostBal { get; set; }

        public decimal CrAmount { get; set; }

        public decimal DrAmount { get; set; }

        public string Remarks { get; set; }

        public decimal Amount { get; set; }

        public DateTime TrnDate { get; set; }
    }

    public class ListProviderWalletRes : BizResponseClass
    {
        public List<ProviderWalletRes> Data { get; set; }
        public int TotalPage { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class ProviderWalletRes
    {
        public Int64 WalletTypeID { get; set; }
        public string WalletTypeIDName { get; set; }
        public string WalletName { get; set; }
        public Guid AccWalletID { get; set; }
        public decimal Balance { get; set; }
        public decimal OutBoundBalance { get; set; }
        public decimal InBoundBalance { get; set; }
        public Int64 SerProId { get; set; }
        public string SerProIdName { get; set; }
        public Int16 Status { get; set; }
        public string StrStatus { get; set; }
    }

    public class ListArbitrageWalletTypeMasterRes : BizResponseClass
    {
        public IEnumerable<ArbitrageWalletTypeMaster> ArbitrageWalletTypeMasters { get; set; }
    }

    public class InsertUpdateArbitrageWalletTypeMasterReq
    {
        public long Id { get; set; }

        public string WalletTypeName { get; set; }

        [Required(ErrorMessage = "1,Please enter Required Param,14067")]
        public string Description { get; set; }

        [Required(ErrorMessage = "1,Please enter Required Param,4204")]
        public short IsDepositionAllow { get; set; }

        [Required(ErrorMessage = "1,Please enter Required Param,4205")]
        public short IsWithdrawalAllow { get; set; }

        [Required(ErrorMessage = "1,Please enter Required Param,4206")]
        public short IsTransactionWallet { get; set; }

        public short IsDefaultWallet { get; set; }

        public short ConfirmationCount { get; set; }

        public short IsLocal { get; set; }

        public Int64 CurrencyTypeID { get; set; }

        public short IsLeaverageAllow { get; set; }

        [Required(ErrorMessage = "1,Please enter Required Param,31026")]
        public short Status { get; set; }
    }

    public class SpecialChargeConfigurationM : IRequest<SpecialChargeConfiguration>
    {
        public long Id { get; set; }
        public Int16 Status { get; set; }
    }

    public class SpecialChargeConfigurationHandler : IRequestHandler<SpecialChargeConfigurationM, SpecialChargeConfiguration>
    {
        private readonly ICommonRepository<SpecialChargeConfiguration> _specialChargeConfiguration;

        public SpecialChargeConfigurationHandler(ICommonRepository<SpecialChargeConfiguration> specialChargeConfiguration)
        {
            _specialChargeConfiguration = specialChargeConfiguration;
        }

        public Task<SpecialChargeConfiguration> Handle(SpecialChargeConfigurationM request, CancellationToken cancellationToken)
        {
            SpecialChargeConfiguration SpecialChargeConfiguration = _specialChargeConfiguration.GetSingle(i => i.Id == request.Id && i.Status == request.Status);
            return Task.FromResult(SpecialChargeConfiguration);
        }
    }

    public class ArbitrageChargeConfigurationMasterM : IRequest<ArbitrageChargeConfigurationMaster>
    {
        public ArbitrageChargeConfigurationMaster ArbitrageChargeConfigurationMaster { get; set; }
        public Int16 ActionType { get; set; }
    }

    public class ArbitrageChargeConfigurationMasterHandler : IRequestHandler<ArbitrageChargeConfigurationMasterM, ArbitrageChargeConfigurationMaster>
    {
        private readonly ICommonRepository<ArbitrageChargeConfigurationMaster> _ArbitrageChargeConfigurationMaster;

        public ArbitrageChargeConfigurationMasterHandler(ICommonRepository<ArbitrageChargeConfigurationMaster> ArbitrageChargeConfigurationMaster)
        {
            _ArbitrageChargeConfigurationMaster = ArbitrageChargeConfigurationMaster;
        }
        public Task<ArbitrageChargeConfigurationMaster> Handle(ArbitrageChargeConfigurationMasterM request, CancellationToken cancellationToken)
        {
            if (request.ActionType == 1)
                _ArbitrageChargeConfigurationMaster.Add(request.ArbitrageChargeConfigurationMaster);
            else if (request.ActionType == 2)
                _ArbitrageChargeConfigurationMaster.UpdateWithAuditLog(request.ArbitrageChargeConfigurationMaster);
            else if (request.ActionType == 3)
                return Task.FromResult(_ArbitrageChargeConfigurationMaster.GetSingle(i => i.WalletTypeID == request.ArbitrageChargeConfigurationMaster.WalletTypeID && i.TrnType == request.ArbitrageChargeConfigurationMaster.TrnType && i.SerProId == request.ArbitrageChargeConfigurationMaster.SerProId));
            else if (request.ActionType == 4)
                return Task.FromResult(_ArbitrageChargeConfigurationMaster.GetSingle(i => i.SerProId == request.ArbitrageChargeConfigurationMaster.SerProId && i.PairId == request.ArbitrageChargeConfigurationMaster.PairId && i.KYCComplaint == request.ArbitrageChargeConfigurationMaster.KYCComplaint));
            else if (request.ActionType == 5)
                return Task.FromResult(_ArbitrageChargeConfigurationMaster.GetSingle(i => i.Id == request.ArbitrageChargeConfigurationMaster.Id));
            return Task.FromResult(new ArbitrageChargeConfigurationMaster());
        }
    }

    public class ArbitrageChargeConfigurationDetailM : IRequest<ArbitrageChargeConfigurationDetail>
    {
        public ArbitrageChargeConfigurationDetail ArbitrageChargeConfigurationDetail { get; set; }
        public Int16 ActionType { get; set; }
    }

    public class ArbitrageChargeConfigurationDetailHandler : IRequestHandler<ArbitrageChargeConfigurationDetailM, ArbitrageChargeConfigurationDetail>
    {
        private readonly ICommonRepository<ArbitrageChargeConfigurationDetail> _ArbitrageChargeConfigurationDetail;

        public ArbitrageChargeConfigurationDetailHandler(ICommonRepository<ArbitrageChargeConfigurationDetail> ArbitrageChargeConfigurationDetail)
        {
            _ArbitrageChargeConfigurationDetail = ArbitrageChargeConfigurationDetail;
        }
        public Task<ArbitrageChargeConfigurationDetail> Handle(ArbitrageChargeConfigurationDetailM request, CancellationToken cancellationToken)
        {
            if (request.ActionType == 1)
                _ArbitrageChargeConfigurationDetail.Add(request.ArbitrageChargeConfigurationDetail);
            else if (request.ActionType == 2)
                _ArbitrageChargeConfigurationDetail.UpdateWithAuditLog(request.ArbitrageChargeConfigurationDetail);
            else if (request.ActionType == 3)
                return Task.FromResult(_ArbitrageChargeConfigurationDetail.GetSingle(i => i.ChargeConfigurationMasterID == request.ArbitrageChargeConfigurationDetail.ChargeConfigurationMasterID && i.ChargeDistributionBasedOn == request.ArbitrageChargeConfigurationDetail.ChargeDistributionBasedOn && i.ChargeType == request.ArbitrageChargeConfigurationDetail.ChargeType && i.ChargeValue == request.ArbitrageChargeConfigurationDetail.ChargeValue && i.ChargeValueType == request.ArbitrageChargeConfigurationDetail.ChargeValueType ));
            else if (request.ActionType == 4)
                return Task.FromResult(_ArbitrageChargeConfigurationDetail.GetSingle(i => i.Id != request.ArbitrageChargeConfigurationDetail.Id && i.ChargeConfigurationMasterID == request.ArbitrageChargeConfigurationDetail.ChargeConfigurationMasterID && i.ChargeDistributionBasedOn == request.ArbitrageChargeConfigurationDetail.ChargeDistributionBasedOn && i.ChargeType == request.ArbitrageChargeConfigurationDetail.ChargeType && i.ChargeValue == request.ArbitrageChargeConfigurationDetail.ChargeValue && i.ChargeValueType == request.ArbitrageChargeConfigurationDetail.ChargeValueType));
            else if (request.ActionType == 5)
                return Task.FromResult(_ArbitrageChargeConfigurationDetail.GetSingle(i => i.Id == request.ArbitrageChargeConfigurationDetail.Id));
            return Task.FromResult(new ArbitrageChargeConfigurationDetail());
        }
    }

    public class ChargeConfigurationMasterArbitrageM : IRequest<ChargeConfigurationMasterArbitrage>
    {
        public ChargeConfigurationMasterArbitrage ChargeConfigurationMasterArbitrage { get; set; }
        public Int16 ActionType { get; set; }
    }

    public class ChargeConfigurationMasterArbitrageMhandler : IRequestHandler<ChargeConfigurationMasterArbitrageM, ChargeConfigurationMasterArbitrage>
    {
        private readonly ICommonRepository<ChargeConfigurationMasterArbitrage> _chargeConfigurationMasterArbitrage;

        public ChargeConfigurationMasterArbitrageMhandler(ICommonRepository<ChargeConfigurationMasterArbitrage> chargeConfigurationMasterArbitrage)
        {
            _chargeConfigurationMasterArbitrage = chargeConfigurationMasterArbitrage;
        }
        public Task<ChargeConfigurationMasterArbitrage> Handle(ChargeConfigurationMasterArbitrageM request, CancellationToken cancellationToken)
        {
            if (request.ActionType == 1)
                _chargeConfigurationMasterArbitrage.Add(request.ChargeConfigurationMasterArbitrage);
            else if (request.ActionType == 2)
                _chargeConfigurationMasterArbitrage.UpdateWithAuditLog(request.ChargeConfigurationMasterArbitrage);
            else if (request.ActionType == 3)
                return Task.FromResult(_chargeConfigurationMasterArbitrage.GetSingle(i => i.WalletTypeID == request.ChargeConfigurationMasterArbitrage.WalletTypeID && i.TrnType == request.ChargeConfigurationMasterArbitrage.TrnType && i.SpecialChargeConfigurationID == request.ChargeConfigurationMasterArbitrage.SpecialChargeConfigurationID));
            else if (request.ActionType == 4)
                return Task.FromResult(_chargeConfigurationMasterArbitrage.GetSingle(i => i.WalletTypeID == request.ChargeConfigurationMasterArbitrage.WalletTypeID && i.TrnType == request.ChargeConfigurationMasterArbitrage.TrnType));
            else if (request.ActionType == 5)
                return Task.FromResult(_chargeConfigurationMasterArbitrage.GetSingle(i => i.Id == request.ChargeConfigurationMasterArbitrage.Id));
            return Task.FromResult(new ChargeConfigurationMasterArbitrage());
        }
    }

    public class ArbitrageWalletTypeMasterM : IRequest<List<ArbitrageWalletTypeMaster>>
    {
        public ArbitrageWalletTypeMaster ArbitrageWalletTypeMaster { get; set; }
        public Int16 ActionType { get; set; }
    }

    public class LPArbitrageWalletMasterM : IRequest<List<LPArbitrageWalletMaster>>
    {
        public string AccWalletID { get; set; }
    }

    public class LPArbitrageWalletMasterMHandler : IRequestHandler<LPArbitrageWalletMasterM, List<LPArbitrageWalletMaster>>
    {
        private readonly ICommonRepository<LPArbitrageWalletMaster> _LPArbitrageWalletMaster;

        public LPArbitrageWalletMasterMHandler(ICommonRepository<LPArbitrageWalletMaster> LPArbitrageWalletMaster)
        {
            _LPArbitrageWalletMaster = LPArbitrageWalletMaster;
        }

        public Task<List<LPArbitrageWalletMaster>> Handle(LPArbitrageWalletMasterM request, CancellationToken cancellationToken)
        {
            List<LPArbitrageWalletMaster> LPArbitrageWalletMasterList = new List<LPArbitrageWalletMaster>();
            LPArbitrageWalletMaster LPArbitrageWalletMaster = _LPArbitrageWalletMaster.GetSingle(item => item.AccWalletID == Guid.Parse(request.AccWalletID));
            LPArbitrageWalletMasterList.Add(LPArbitrageWalletMaster);
            return Task.FromResult(LPArbitrageWalletMasterList);
        }
    }

    public class ArbitrageWalletTypeMasterMHandler : IRequestHandler<ArbitrageWalletTypeMasterM, List<ArbitrageWalletTypeMaster>>
    {
        private readonly ICommonRepository<ArbitrageWalletTypeMaster> _arbitrageWalletTypeMaster;

        public ArbitrageWalletTypeMasterMHandler(ICommonRepository<ArbitrageWalletTypeMaster> arbitrageWalletTypeMaster)
        {
            _arbitrageWalletTypeMaster = arbitrageWalletTypeMaster;
        }
        public Task<List<ArbitrageWalletTypeMaster>> Handle(ArbitrageWalletTypeMasterM request, CancellationToken cancellationToken)
        {
            List<ArbitrageWalletTypeMaster> ArbitrageWalletTypeMasterList = new List<ArbitrageWalletTypeMaster>();
            if (request.ActionType == 1)
                _arbitrageWalletTypeMaster.Add(request.ArbitrageWalletTypeMaster);
            else if (request.ActionType == 2)
                _arbitrageWalletTypeMaster.UpdateWithAuditLog(request.ArbitrageWalletTypeMaster);
            else if (request.ActionType == 3)
            {
                ArbitrageWalletTypeMaster ArbitrageWalletTypeMaster = _arbitrageWalletTypeMaster.GetSingle(i => i.Id == request.ArbitrageWalletTypeMaster.Id && i.Status == request.ArbitrageWalletTypeMaster.Status);
                ArbitrageWalletTypeMasterList.Add(ArbitrageWalletTypeMaster);
                return Task.FromResult(ArbitrageWalletTypeMasterList);
            }
            else if (request.ActionType == 4)
            {
                ArbitrageWalletTypeMaster ArbitrageWalletTypeMaster = _arbitrageWalletTypeMaster.GetSingle(item => item.Id == request.ArbitrageWalletTypeMaster.Id);
                ArbitrageWalletTypeMasterList.Add(ArbitrageWalletTypeMaster);
                return Task.FromResult(ArbitrageWalletTypeMasterList);
            }
            else if (request.ActionType == 5)
            {
                //List<ArbitrageWalletTypeMaster> ArbitrageWalletTypeMasterList1 = (List<ArbitrageWalletTypeMaster>)_arbitrageWalletTypeMaster.FindBy(item => item.Status != Convert.ToInt16(request.ArbitrageWalletTypeMaster.Status));
                List<ArbitrageWalletTypeMaster> ArbitrageWalletTypeMasterList1 = _arbitrageWalletTypeMaster.List();//2019-7-19 change query due to error --return all list of wallettype
                return Task.FromResult(ArbitrageWalletTypeMasterList1);
            }
            else if(request.ActionType == 6)
            {
                ArbitrageWalletTypeMaster ArbitrageWalletTypeMaster = _arbitrageWalletTypeMaster.GetSingle(i => i.WalletTypeName == request.ArbitrageWalletTypeMaster.WalletTypeName);
                ArbitrageWalletTypeMasterList.Add(ArbitrageWalletTypeMaster);
                return Task.FromResult(ArbitrageWalletTypeMasterList);
            }
            return Task.FromResult(ArbitrageWalletTypeMasterList);
        }
    }

    public class TradePairMasterArbitrageM : IRequest<TradePairMasterArbitrage>
    {
        public long PairID { get; set; }
        public short Status { get; set; }
    }

    public class TradePairMasterArbitrageMhandler : IRequestHandler<TradePairMasterArbitrageM, TradePairMasterArbitrage>
    {
        private readonly ICommonRepository<TradePairMasterArbitrage> _tradePairMasterArbitrage;

        public TradePairMasterArbitrageMhandler(ICommonRepository<TradePairMasterArbitrage> tradePairMasterArbitrage)
        {
            _tradePairMasterArbitrage = tradePairMasterArbitrage;
        }
        public Task<TradePairMasterArbitrage> Handle(TradePairMasterArbitrageM request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_tradePairMasterArbitrage.GetSingle(i => i.Id == request.PairID && i.Status == request.Status));
        }
    }

    public class ServiceProviderMasterArbitrageM : IRequest<ServiceProviderMasterArbitrage>
    {
        public long SerProID { get; set; }
        public short Status { get; set; }
    }

    public class ServiceProviderMasterArbitrageMhandler : IRequestHandler<ServiceProviderMasterArbitrageM, ServiceProviderMasterArbitrage>
    {
        private readonly ICommonRepository<ServiceProviderMasterArbitrage> _serviceProviderMasterArbitrage;

        public ServiceProviderMasterArbitrageMhandler(ICommonRepository<ServiceProviderMasterArbitrage> serviceProviderMasterArbitrage)
        {
            _serviceProviderMasterArbitrage = serviceProviderMasterArbitrage;
        }
        public Task<ServiceProviderMasterArbitrage> Handle(ServiceProviderMasterArbitrageM request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_serviceProviderMasterArbitrage.GetSingle(i => i.Id == request.SerProID && i.Status == request.Status));
        }
    }

    public class CurrencyTypeMasterM : IRequest<CurrencyTypeMaster>
    {
        public long CurrencyTypeID { get; set; }
        public short Status { get; set; }
    }

    public class CurrencyTypeMasterMHandler : IRequestHandler<CurrencyTypeMasterM, CurrencyTypeMaster>
    {
        private readonly ICommonRepository<CurrencyTypeMaster> _currencyTypeMaster;

        public CurrencyTypeMasterMHandler(ICommonRepository<CurrencyTypeMaster> CurrencyTypeMaster)
        {
            _currencyTypeMaster = CurrencyTypeMaster;
        }
        public Task<CurrencyTypeMaster> Handle(CurrencyTypeMasterM request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_currencyTypeMaster.GetSingle(i => i.Id == request.CurrencyTypeID && i.Status == request.Status));
        }
    }

    public class _ChargeConfigurationDetailArbitrageM : IRequest<ChargeConfigurationDetailArbitrage>
    {
        public ChargeConfigurationDetailArbitrage ChargeConfigurationDetailArbitrage { get; set; }
        public Int16 ActionType { get; set; }
    }

    public class _ChargeConfigurationDetailArbitrageMHandler : IRequestHandler<_ChargeConfigurationDetailArbitrageM, ChargeConfigurationDetailArbitrage>
    {
        private readonly ICommonRepository<ChargeConfigurationDetailArbitrage> _ChargeConfigurationDetailArbitrage;

        public _ChargeConfigurationDetailArbitrageMHandler(ICommonRepository<ChargeConfigurationDetailArbitrage> ChargeConfigurationDetailArbitrage)
        {
            _ChargeConfigurationDetailArbitrage = ChargeConfigurationDetailArbitrage;
        }
        public Task<ChargeConfigurationDetailArbitrage> Handle(_ChargeConfigurationDetailArbitrageM request, CancellationToken cancellationToken)
        {
            if (request.ActionType == 1)
                _ChargeConfigurationDetailArbitrage.Add(request.ChargeConfigurationDetailArbitrage);
            else if (request.ActionType == 2)
                _ChargeConfigurationDetailArbitrage.UpdateWithAuditLog(request.ChargeConfigurationDetailArbitrage);
            else if (request.ActionType == 3)
                return Task.FromResult(_ChargeConfigurationDetailArbitrage.GetSingle(i => i.ChargeConfigurationMasterID == request.ChargeConfigurationDetailArbitrage.ChargeConfigurationMasterID && i.ChargeDistributionBasedOn == Convert.ToInt16(request.ChargeConfigurationDetailArbitrage.ChargeDistributionBasedOn) && i.ChargeType == Convert.ToInt64(request.ChargeConfigurationDetailArbitrage.ChargeType) && i.ChargeValue == request.ChargeConfigurationDetailArbitrage.ChargeValue && i.ChargeValueType == Convert.ToInt16(request.ChargeConfigurationDetailArbitrage.ChargeValueType)));
            else if (request.ActionType == 4)
                return Task.FromResult(_ChargeConfigurationDetailArbitrage.GetSingle(i => i.Id != request.ChargeConfigurationDetailArbitrage.Id && i.ChargeConfigurationMasterID == request.ChargeConfigurationDetailArbitrage.ChargeConfigurationMasterID && i.ChargeDistributionBasedOn == Convert.ToInt16(request.ChargeConfigurationDetailArbitrage.ChargeDistributionBasedOn) && i.ChargeType == Convert.ToInt64(request.ChargeConfigurationDetailArbitrage.ChargeType) && i.ChargeValue == request.ChargeConfigurationDetailArbitrage.ChargeValue && i.ChargeValueType == Convert.ToInt16(request.ChargeConfigurationDetailArbitrage.ChargeValueType)));
            else if (request.ActionType == 5)
                return Task.FromResult(_ChargeConfigurationDetailArbitrage.GetSingle(i => i.Id == request.ChargeConfigurationDetailArbitrage.Id));
            return Task.FromResult(new ChargeConfigurationDetailArbitrage());
        }
    }

    public class LPArbitrageTrnChargeLogRes
    {
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public string BatchNo { get; set; }
        public long TrnNo { get; set; }
        public long TrnTypeID { get; set; }
        public string TrnTypeName { get; set; }
        public string Amount { get; set; }
        public string StrAmount { get; set; }
        public string Charge { get; set; }
        public long StakingChargeMasterID { get; set; }
        public long ChargeConfigurationDetailID { get; set; }
        public string ChargeConfigurationDetailRemarks { get; set; }
        public string TimeStamp { get; set; }
        public long DWalletID { get; set; }
        public string DAccWalletId { get; set; }
        public string DWalletName { get; set; }
        public long OWalletID { get; set; }
        public Guid OAccWalletID { get; set; }
        public string OWalletName { get; set; }
        public long DUserID { get; set; }
        public string DUserName { get; set; }
        public string DEmail { get; set; }
        public long OUserID { get; set; }
        public string OUserName { get; set; }
        public string OEmail { get; set; }
        public long WalletTypeID { get; set; }
        public string WalletTypeName { get; set; }
        public short SlabType { get; set; }
        public string SlabTypeName { get; set; }
        public string TrnChargeLogRemarks { get; set; }
        public long ChargeConfigurationMasterID { get; set; }
        public long SpecialChargeConfigurationID { get; set; }
        public string SpecialChargeConfigurationRemarks { get; set; }
        public short Ismaker { get; set; }
        public string StrIsmaker { get; set; }
        public DateTime Date { get; set; }
    }
    public class ListLPWalletMismatchRes : BizResponseClass
    {
        public List<LPWalletMismatchRes> Data { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
        public long PageNo { get; set; }
    }

    public class LPWalletMismatchRes
    {
        public string Id { get; set; }
        public Int16 Status { get; set; }
        public string StrStatus { get; set; }
        public Int64 WalletTypeID { get; set; }
        public string WalletTypeName { get; set; }
        public long SerProID { get; set; }
        public string SerProIDName { get; set; }
        public decimal MisMatchingAmount { get; set; }
        public decimal SettledAmount { get; set; }
        public long? ResolvedBy { get; set; }
        public string ResolvedByName { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string Remarks { get; set; }
        public string currencyName { get; set; }
    }
    public class ListLPArbitrageTrnChargeLogRes : BizResponseClass
    {
        public List<LPArbitrageTrnChargeLogRes> Data { get; set; }
        public long TotalCount { get; set; }
        public int PageNo { get; set; }
        public long PageSize { get; set; }
    }

    public class ListArbitrageWalletTQRes : BizResponseClass
    {
        public List<ArbitrageWalletTQRes> Data { get; set; }
        public long TotalCount { get; set; }
        public int PageNo { get; set; }
        public long PageSize { get; set; }
    }

    public class ArbitrageWalletTQRes
    {
        public Int32 Status { get; set; }
        public string StrStatus { get; set; }
        public long ProviderID { get; set; }
        public string ProviderName { get; set; }
        public DateTime TrnDate { get; set; }
        public string Amount { get; set; }
        public long TrnRefNo { get; set; }
        public string Currency { get; set; }
    }
}
