using System;
using System.Collections.Generic;
using Worldex.Core.Interfaces.Referral;
using Worldex.Core.Entities.Referral;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.Referral;
using System.Linq;
using Worldex.Core.ApiModels;
using System.Threading.Tasks;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;

namespace Worldex.Infrastructure.Services.Referral
{
    public class ReferralServices : IReferralService
    {
        private readonly WorldexContext _dbContext;
        private readonly ICustomRepository<ReferralService> _ReferralServiceRepository;
        private readonly ICommonRepository<ReferralSchemeTypeMapping> _ReferralSchemeTypeMappingRepo;
        private readonly ICommonRepository<ReferralServiceDetail> _ReferralServiceDetailRepo;
        private readonly IReferralCommonRepo _ReferralCommonRepository;

        public ReferralServices(ICustomRepository<ReferralService> ReferralServiceRepository, ICommonRepository<ReferralServiceDetail> ReferralServiceDetailRepo, IReferralCommonRepo ReferralCommonRepository, WorldexContext dbContext, ICommonRepository<ReferralSchemeTypeMapping> ReferralSchemeTypeMappingRepo)
        {
            _dbContext = dbContext;
            _ReferralServiceDetailRepo = ReferralServiceDetailRepo;
            _ReferralCommonRepository = ReferralCommonRepository;
            _ReferralServiceRepository = ReferralServiceRepository;
            _ReferralSchemeTypeMappingRepo = ReferralSchemeTypeMappingRepo;
        }

        public long AddReferralService(ReferralServiceViewModel ReferralServiceInsert, long UserID)
        {
            try
            {
                ReferralService ObjReferralService = new ReferralService()
                {
                    ReferralServiceTypeId = ReferralServiceInsert.ReferralServiceTypeId,
                    ReferralPayTypeId = ReferralServiceInsert.ReferralPayTypeId,
                    CurrencyId = ReferralServiceInsert.CurrencyId,
                    Description = ReferralServiceInsert.Description,
                    ReferMinCount = ReferralServiceInsert.ReferMinCount,
                    ReferMaxCount = ReferralServiceInsert.ReferMaxCount,
                    RewardsPay = ReferralServiceInsert.RewardsPay,
                    ActiveDate = ReferralServiceInsert.ActiveDate,
                    ExpireDate = ReferralServiceInsert.ExpireDate,
                    CreatedBy = UserID,
                    CreatedDate = DateTime.UtcNow,
                    Status = 1
                };
                _ReferralServiceRepository.Insert(ObjReferralService);
                return ObjReferralService.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public long UpdateReferralService(ReferralServiceUpdateViewModel model, long UserId)
        {
            var GetUpdate = _ReferralServiceRepository.Table.Where(i => i.Id == model.Id).FirstOrDefault();
            if (GetUpdate != null)
            {
                GetUpdate.ReferralServiceTypeId = model.ReferralServiceTypeId;
                GetUpdate.ReferralPayTypeId = model.ReferralPayTypeId;
                GetUpdate.CurrencyId = model.CurrencyId;
                GetUpdate.Description = model.Description;
                GetUpdate.ReferMinCount = model.ReferMinCount;
                GetUpdate.ReferMaxCount = model.ReferMaxCount;
                GetUpdate.RewardsPay = model.RewardsPay;
                GetUpdate.ActiveDate = model.ActiveDate;
                GetUpdate.ExpireDate = model.ExpireDate;
                GetUpdate.UpdatedDate = DateTime.UtcNow;
                GetUpdate.UpdatedBy = UserId;
                _ReferralServiceRepository.Update(GetUpdate);
                return GetUpdate.Id;
            }
            return 0;
        }

        public ReferralServiceUpdateViewModel GetReferralServiceById(long Id)
        {
            try
            {
                var GetData = _ReferralServiceRepository.Table.Where(i => i.Id == Id).FirstOrDefault();
                ReferralServiceUpdateViewModel modeldata = new ReferralServiceUpdateViewModel();
                if (GetData != null)
                {
                    modeldata.Id = GetData.Id;
                    modeldata.ReferralServiceTypeId = GetData.ReferralServiceTypeId;
                    modeldata.ReferralPayTypeId = GetData.ReferralPayTypeId;
                    modeldata.CurrencyId = GetData.CurrencyId;
                    modeldata.Description = GetData.Description;
                    modeldata.ReferMinCount = GetData.ReferMinCount;
                    modeldata.ReferMaxCount = GetData.ReferMaxCount;
                    modeldata.RewardsPay = GetData.RewardsPay;
                    modeldata.ActiveDate = GetData.ActiveDate;
                    modeldata.ExpireDate = GetData.ExpireDate;
                    return modeldata;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public ReferralServiceUpdateViewModel GetReferralService()
        {
            try
            {
                var GetData = _ReferralServiceRepository.Table.Where(i => i.Status == 1 && (i.ActiveDate.Date <= DateTime.UtcNow.Date && i.ExpireDate.Date >= DateTime.UtcNow.Date)).FirstOrDefault();
                ReferralServiceUpdateViewModel modeldata = new ReferralServiceUpdateViewModel();
                if (GetData != null)
                {
                    modeldata.Id = GetData.Id;
                    modeldata.ReferralServiceTypeId = GetData.ReferralServiceTypeId;
                    modeldata.ReferralPayTypeId = GetData.ReferralPayTypeId;
                    modeldata.CurrencyId = GetData.CurrencyId;
                    modeldata.Description = GetData.Description;
                    modeldata.ReferMinCount = GetData.ReferMinCount;
                    modeldata.ReferMaxCount = GetData.ReferMaxCount;
                    modeldata.RewardsPay = GetData.RewardsPay;
                    modeldata.ActiveDate = GetData.ActiveDate;
                    modeldata.ExpireDate = GetData.ExpireDate;
                    return modeldata;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public ReferralServiceListResponse ListReferralService(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                if (PageIndex == 0)
                {
                    PageIndex = 1;
                }

                if (Page_Size == 0)
                {
                    Page_Size = 10;
                }

                var skip = Page_Size * (PageIndex - 1);

                var items = (from rs in _dbContext.ReferralService
                             join st in _dbContext.ReferralServiceType on rs.ReferralServiceTypeId equals st.Id
                             join pt in _dbContext.ReferralPayType on rs.ReferralPayTypeId equals pt.Id
                             join cu in _dbContext.WalletTypeMasters on rs.CurrencyId equals cu.Id
                             select new ReferralServiceListViewModel
                             {
                                 Id = rs.Id,
                                 ReferralServiceTypeId = rs.ReferralServiceTypeId,
                                 ReferralServiceTypeName = st.ServiceTypeName,
                                 ReferralPayTypeId = rs.ReferralPayTypeId,
                                 ReferralPayTypeName = pt.PayTypeName,
                                 CurrencyId = rs.CurrencyId,
                                 CurrencyName = cu.WalletTypeName,
                                 Description = rs.Description,
                                 ReferMinCount = rs.ReferMinCount,
                                 ReferMaxCount = rs.ReferMaxCount,
                                 RewardsPay = rs.RewardsPay,
                                 ActiveDate = rs.ActiveDate,
                                 ExpireDate = rs.ExpireDate,
                                 CreatedDate = rs.CreatedDate,
                                 Status = rs.Status
                             }
                            ).ToList();

                int TotalCount = items.Count;

                ReferralServiceListResponse obj = new ReferralServiceListResponse();
                obj.TotalCount = TotalCount;
                obj.ReferralServiceList = items.OrderBy(x => x.Id).Skip(skip).Take(Page_Size).ToList();

                return obj;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ReferralServiceDropDownViewModel> DropDownReferralService(int PayTypeId = 0)
        {
            try
            {
                List<ReferralServiceDropDownViewModel> ListData = new List<ReferralServiceDropDownViewModel>();
                if (PayTypeId > 0)
                {
                    ListData = _ReferralServiceRepository.Table.Where(x => x.ReferralPayTypeId == PayTypeId).Select(y => new ReferralServiceDropDownViewModel { Id = y.Id, ServiceSlab = Convert.ToString((y.ReferMinCount + " - " + y.ReferMaxCount)) }).ToList();
                }
                else
                {
                    ListData = _ReferralServiceRepository.Table.Select(y => new ReferralServiceDropDownViewModel { Id = y.Id, ServiceSlab = Convert.ToString((y.ReferMinCount + " - " + y.ReferMaxCount)) }).ToList();
                }
                if (ListData == null)
                {
                    return null;
                }
                List<ReferralServiceDropDownViewModel> myViewModel = new List<ReferralServiceDropDownViewModel>();
                string myString = Newtonsoft.Json.JsonConvert.SerializeObject(ListData);
                myViewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ReferralServiceDropDownViewModel>>(myString);
                return myViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DisableReferralService(ReferralServiceStatusViewModel model, long UserId)
        {
            try
            {
                var Disable = _ReferralServiceRepository.Table.Where(i => i.Id == model.Id).FirstOrDefault();
                if (Disable != null)
                {
                    Disable.UpdatedDate = DateTime.UtcNow;
                    Disable.UpdatedBy = UserId;
                    Disable.Status = 0;
                    _ReferralServiceRepository.Update(Disable);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public bool EnableReferralService(ReferralServiceStatusViewModel model, long UserId)
        {
            try
            {
                var Disable = _ReferralServiceRepository.Table.Where(i => i.Id == model.Id).FirstOrDefault();
                if (Disable != null)
                {
                    Disable.UpdatedDate = DateTime.UtcNow;
                    Disable.UpdatedBy = UserId;
                    Disable.Status = 1;
                    _ReferralServiceRepository.Update(Disable);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public bool ReferralServiceExist(int ServiceId)
        {
            var referraldata = _dbContext.ReferralService.Any(i => i.Id == ServiceId);
            return referraldata;
        }

        public long ReferralServiceId()
        {
            try
            {
                var GetData = _ReferralServiceRepository.Table.Where(x => x.Status == 1).FirstOrDefault();
                ReferralServiceStatusViewModel modeldata = new ReferralServiceStatusViewModel();
                if (GetData != null)
                {
                    modeldata.Id = GetData.Id;
                    return modeldata.Id;
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        #region ReferralSchemeTypeMapping CRUD

        public async Task<BizResponseClass> AddUpdateReferralSchemeTypeMapping(AddReferralSchemeTypeMappingReq request, long userid)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                //Insert
                if (request.Id == null || request.Id == 0)
                {
                    var IsExist = await _ReferralSchemeTypeMappingRepo.GetSingleAsync(x => x.PayTypeId == request.PayTypeId && x.ServiceTypeMstId == request.ServiceTypeMstId && x.Status != 9);
                    if (IsExist != null)
                    {
                        Resp.ErrorCode = enErrorCode.ReferralRecordAlreadyExist;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.DataAlreadyExist;
                        return Resp;
                    }
                    else
                    {
                        ReferralSchemeTypeMapping NewObj = new ReferralSchemeTypeMapping
                        {
                            MinimumDepositionRequired = request.MinimumDepositionRequired,
                            ServiceTypeMstId = request.ServiceTypeMstId,
                            PayTypeId = request.PayTypeId,
                            Description = request.Description,
                            CreatedBy = userid,
                            CreatedDate = Helpers.UTC_To_IST(),
                            Status = Convert.ToInt16(request.Status),
                            FromDate = request.FromDate,
                            ToDate = request.ToDate
                        };
                        _ReferralSchemeTypeMappingRepo.Add(NewObj);
                        Resp.ErrorCode = enErrorCode.Success;
                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                    }
                }
                //Update
                else
                {
                    if (request.Id != null && request.Id > 0)
                    {
                        var IsDuplicate = await _ReferralSchemeTypeMappingRepo.GetSingleAsync(x => x.Id != request.Id && x.PayTypeId == request.PayTypeId && x.ServiceTypeMstId == request.ServiceTypeMstId && x.Status != 9);
                        if (IsDuplicate != null)
                        {
                            Resp.ErrorCode = enErrorCode.ReferralRecordAlreadyExist;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.DataAlreadyExist;
                            return Resp;
                        }
                        var IsExist = await _ReferralSchemeTypeMappingRepo.GetSingleAsync(x => x.Id == request.Id && x.Status == 1);
                        if (IsExist == null)
                        {
                            Resp.ErrorCode = enErrorCode.ReferralRecordNotFound;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.ReferralNoDataFound;
                            return Resp;
                        }
                        else
                        {
                            IsExist.MinimumDepositionRequired = request.MinimumDepositionRequired;
                            IsExist.ServiceTypeMstId = request.ServiceTypeMstId;
                            IsExist.PayTypeId = request.PayTypeId;
                            IsExist.Description = request.Description;
                            IsExist.UpdatedBy = userid;
                            IsExist.UpdatedDate = Helpers.UTC_To_IST();
                            IsExist.Status = Convert.ToInt16(request.Status);
                            IsExist.FromDate = request.FromDate;
                            IsExist.ToDate = request.ToDate;

                            _ReferralSchemeTypeMappingRepo.UpdateWithAuditLog(IsExist);
                            Resp.ErrorCode = enErrorCode.Success;
                            Resp.ReturnCode = enResponseCode.Success;
                            Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                        }
                    }
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AddUpdateReferralSchemeTypeMapping", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<BizResponseClass> ChangeReferralSchemeTypeMappingStatus(long id, ServiceStatus status, long UserId)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = await _ReferralSchemeTypeMappingRepo.GetSingleAsync(x => x.Id == id);
                if (IsExist == null)
                {
                    Resp.ErrorCode = enErrorCode.ReferralRecordNotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.ReferralNoDataFound;
                    return Resp;
                }
                else
                {
                    IsExist.Status = Convert.ToInt16(status);
                    IsExist.UpdatedBy = UserId;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();

                    _ReferralSchemeTypeMappingRepo.UpdateWithAuditLog(IsExist);
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                    return Resp;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ChangeReferralSchemeTypeMappingStatus", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<GetReferralSchemeTypeMappingRes> GetReferralSchemeTypeMappingById(long id)
        {
            GetReferralSchemeTypeMappingRes Resp = new GetReferralSchemeTypeMappingRes();
            try
            {
                var data = _ReferralCommonRepository.GetByIdMappingData(id);
                if (data != null)
                {
                    Resp.Data = data;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.ReferralRecordNotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.ReferralNoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetReferralSchemeTypeMappingById", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<ListReferralSchemeTypeMappingRes> ListReferralSchemeTypeMapping(long? payTypeId, long? serviceTypeMstId, short? status)
        {
            ListReferralSchemeTypeMappingRes Resp = new ListReferralSchemeTypeMappingRes();
            try
            {
                var data = _ReferralCommonRepository.ListMappingData(payTypeId, serviceTypeMstId, status);
                if (data != null && data.Count > 0)
                {
                    Resp.Data = data;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.ReferralRecordNotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.ReferralNoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListReferralSchemeTypeMapping", this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region ReferralServiceDetail CRUD

        public async Task<BizResponseClass> AddUpdateReferralServiceDetail(AddServiceDetail req, long id)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                //Insert
                if (req.Id == null || req.Id == 0)
                {
                    var IsExist = await _ReferralServiceDetailRepo.GetSingleAsync(x => x.CreditWalletTypeId == req.CreditWalletTypeId && x.CommissionType == req.CommissionType && x.SchemeTypeMappingId == req.SchemeTypeMappingId && x.Status != 9);
                    if (IsExist != null)
                    {
                        Resp.ErrorCode = enErrorCode.ReferralRecordAlreadyExist;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.DataAlreadyExist;
                        return Resp;
                    }
                    else
                    {
                        ReferralServiceDetail NewObj = new ReferralServiceDetail
                        {
                            SchemeTypeMappingId = req.SchemeTypeMappingId,
                            CommissionType = req.CommissionType,
                            CommissionValue = req.CommissionValue,
                            CreditWalletTypeId = req.CreditWalletTypeId,
                            MaximumCoin = req.MaximumCoin,
                            MaximumLevel = req.MaximumLevel,
                            MaximumValue = req.MaximumValue,
                            MinimumValue = req.MinimumValue,
                            CreatedBy = id,
                            CreatedDate = Helpers.UTC_To_IST(),
                            Status = Convert.ToInt16(req.Status)
                        };
                        _ReferralServiceDetailRepo.Add(NewObj);
                        Resp.ErrorCode = enErrorCode.Success;
                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                    }
                }
                //Update
                else
                {
                    if (req.Id != null && req.Id > 0)
                    {
                        var IsDuplicate = await _ReferralServiceDetailRepo.GetSingleAsync(x => x.Id != req.Id && x.CreditWalletTypeId == req.CreditWalletTypeId && x.CommissionType == req.CommissionType && x.SchemeTypeMappingId == req.SchemeTypeMappingId && x.Status != 9);
                        if (IsDuplicate != null)
                        {
                            Resp.ErrorCode = enErrorCode.ReferralRecordAlreadyExist;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.DataAlreadyExist;
                            return Resp;
                        }
                        var IsExist = await _ReferralServiceDetailRepo.GetSingleAsync(x => x.Id == req.Id && x.Status == 1);
                        if (IsExist == null)
                        {
                            Resp.ErrorCode = enErrorCode.ReferralRecordNotFound;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.ReferralNoDataFound;
                            return Resp;
                        }
                        else
                        {
                            IsExist.SchemeTypeMappingId = req.SchemeTypeMappingId;
                            IsExist.CommissionType = req.CommissionType;
                            IsExist.CommissionValue = req.CommissionValue;
                            IsExist.CreditWalletTypeId = req.CreditWalletTypeId;
                            IsExist.UpdatedBy = id;
                            IsExist.UpdatedDate = Helpers.UTC_To_IST();
                            IsExist.Status = Convert.ToInt16(req.Status);
                            IsExist.MaximumCoin = req.MaximumCoin;
                            IsExist.MaximumLevel = req.MaximumLevel;
                            IsExist.MaximumValue = req.MaximumValue;
                            IsExist.MinimumValue = req.MinimumValue;

                            _ReferralServiceDetailRepo.UpdateWithAuditLog(IsExist);
                            Resp.ErrorCode = enErrorCode.Success;
                            Resp.ReturnCode = enResponseCode.Success;
                            Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                        }
                    }
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AddUpdateReferralServiceDetail", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<BizResponseClass> ChangeReferralServiceDetailStatus(long id, ServiceStatus status, int userid)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = await _ReferralServiceDetailRepo.GetSingleAsync(x => x.Id == id);
                if (IsExist == null)
                {
                    Resp.ErrorCode = enErrorCode.ReferralRecordNotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.ReferralNoDataFound;
                    return Resp;
                }
                else
                {
                    IsExist.Status = Convert.ToInt16(status);
                    IsExist.UpdatedBy = userid;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();

                    _ReferralServiceDetailRepo.UpdateWithAuditLog(IsExist);
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                    return Resp;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ChangeReferralServiceDetailStatus", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<GetReferralServiceDetailRes> GetReferralServiceDetailByid(long id)
        {
            GetReferralServiceDetailRes Resp = new GetReferralServiceDetailRes();
            try
            {
                var data = _ReferralCommonRepository.GetByIdReferralServiceDetail(id);
                if (data != null)
                {
                    Resp.Data = data;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.ReferralRecordNotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.ReferralNoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetReferralServiceDetailByid", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<ListReferralServiceDetailRes> ReferralServiceDetail(long? schemeTypeMappingId, long? creditWalletTypeId, short? status)
        {
            ListReferralServiceDetailRes Resp = new ListReferralServiceDetailRes();
            try
            {
                var data = _ReferralCommonRepository.ListReferralServiceDetail(schemeTypeMappingId, creditWalletTypeId, status);
                if (data != null && data.Count > 0)
                {
                    Resp.Data = data;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.ReferralRecordNotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.ReferralNoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListReferralSchemeTypeMapping", this.GetType().Name, ex);
                throw;
            }
        }




        #endregion

    }
}
