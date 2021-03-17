using Worldex.Core.ApiModels;
using Worldex.Core.Entities.Affiliate;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Affiliate;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.AccountViewModels.Affiliate;
using Worldex.Core.ViewModels.BackOfficeAffiliate;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.Affiliate
{
    public class AffiliateBackOfficeService : IAffiliateBackOfficeService
    {
        private readonly IAffiliateBackOfficeRepository _affiliateBackOfficeRepository;
        private readonly WorldexContext _dbContext;
        private readonly ICommonRepository<AffiliateSchemeTypeMapping> _affiliateSchemeTypeMappingRepository;
        private readonly ICommonRepository<AffiliateSchemeMaster> _affiliateSchemeMaster;
        private readonly ICommonRepository<AffiliateSchemeTypeMaster> _affiliateSchemeTypeMaster;
        private readonly ICommonRepository<AffiliateSchemeDetail> _AffiliateSchemeDetail;
        private readonly ICommonRepository<AffiliatePromotionMaster> _AffiliatePromotionMaster;
        public AffiliateBackOfficeService(IAffiliateBackOfficeRepository affiliateBackOfficeRepository, WorldexContext dbContext, ICommonRepository<AffiliateSchemeTypeMapping> affiliateSchemeTypeMappingRepository, ICommonRepository<AffiliateSchemeMaster> affiliateSchemeMaster, ICommonRepository<AffiliateSchemeDetail> AffiliateSchemeDetail, ICommonRepository<AffiliatePromotionMaster> AffiliatePromotionMaster, ICommonRepository<AffiliateSchemeTypeMaster> affiliateSchemeTypeMaster)
        {
            _AffiliateSchemeDetail = AffiliateSchemeDetail;
            _affiliateBackOfficeRepository = affiliateBackOfficeRepository;
            _affiliateSchemeMaster = affiliateSchemeMaster;
            _affiliateSchemeTypeMaster = affiliateSchemeTypeMaster;
            _dbContext = dbContext;
            _AffiliatePromotionMaster = AffiliatePromotionMaster;
            _affiliateSchemeTypeMappingRepository = affiliateSchemeTypeMappingRepository;
        }

        public AffiliateDashboardCount GetAffiliateDashboardCount()
        {
            try
            {
                AffiliateDashboardCount Response = new AffiliateDashboardCount();

                //get the all count
                var Data = _affiliateBackOfficeRepository.GetAffiliateDashboardCount();

                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public GetAffiateUserRegisteredResponse GetAffiateUserRegistered(string FromDate, string ToDate, int Status, int SchemeType, long ParentId, string SCondition, int PageNo, int PageSize)
        {
            try
            {
                long TotalCount = 0;
                int PageSize1 = 0;
                long TotalPages = 0;

                GetAffiateUserRegisteredResponse Response = new GetAffiateUserRegisteredResponse();

                var _Res = _affiliateBackOfficeRepository.GetAffiateUserRegistered(FromDate, ToDate, Status, SchemeType, ParentId, SCondition, PageSize, PageNo, ref TotalCount, ref PageSize1, ref TotalPages);

                if (_Res.Count > 0)
                {
                    Response.Response = _Res;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Response.Response = new List<GetAffiateUserRegisteredData>();
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoDataFound;
                    Response.ReturnMsg = "No Data Found";
                }

                Response.TotalPages = TotalPages;
                Response.TotalCount = TotalCount;
                Response.PageNo = PageNo;
                Response.PageSize = PageSize1;

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public GetReferralLinkClickResponse GetReferralLinkClick(string FromDate, string ToDate, long UserId, string SCondition, int PageNo, int PageSize)
        {
            try
            {
                long TotalCount = 0;
                int PageSize1 = 0;
                long TotalPages = 0;

                GetReferralLinkClickResponse Response = new GetReferralLinkClickResponse();

                var _Res = _affiliateBackOfficeRepository.GetReferralLinkClick(FromDate, ToDate, UserId, SCondition, PageSize, PageNo, ref TotalCount, ref PageSize1, ref TotalPages);

                if (_Res.Count > 0)
                {
                    Response.Response = _Res;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Response.Response = new List<GetReferralLinkClickData>();
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoDataFound;
                    Response.ReturnMsg = "No Data Found";
                }

                Response.TotalPages = TotalPages;
                Response.TotalCount = TotalCount;
                Response.PageNo = PageNo;
                Response.PageSize = PageSize1;

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public GetFacebookLinkClickResponse GetFacebookLinkClick(string FromDate, string ToDate, long UserId, string SCondition, int PageNo, int PageSize)
        {
            try
            {
                long TotalCount = 0;
                int PageSize1 = 0;
                long TotalPages = 0;

                GetFacebookLinkClickResponse Response = new GetFacebookLinkClickResponse();

                var _Res = _affiliateBackOfficeRepository.GetFacebookLinkClick(FromDate, ToDate, UserId, SCondition, PageSize, PageNo, ref TotalCount, ref PageSize1, ref TotalPages);

                if (_Res.Count > 0)
                {
                    Response.Response = _Res;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Response.Response = new List<GetFacebookLinkClickData>();
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoDataFound;
                    Response.ReturnMsg = "No Data Found";
                }

                Response.TotalPages = TotalPages;
                Response.TotalCount = TotalCount;
                Response.PageNo = PageNo;
                Response.PageSize = PageSize1;

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public GetTwitterLinkClickResponse GetTwitterLinkClick(string FromDate, string ToDate, long UserId, string SCondition, int PageNo, int PageSize)
        {
            try
            {
                long TotalCount = 0;
                int PageSize1 = 0;
                long TotalPages = 0;

                GetTwitterLinkClickResponse Response = new GetTwitterLinkClickResponse();

                var _Res = _affiliateBackOfficeRepository.GetTwitterLinkClick(FromDate, ToDate, UserId, SCondition, PageSize, PageNo, ref TotalCount, ref PageSize1, ref TotalPages);

                if (_Res.Count > 0)
                {
                    Response.Response = _Res;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Response.Response = new List<GetTwitterLinkClickData>();
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoDataFound;
                    Response.ReturnMsg = "No Data Found";
                }

                Response.TotalPages = TotalPages;
                Response.TotalCount = TotalCount;
                Response.PageNo = PageNo;
                Response.PageSize = PageSize1;

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public GetEmailSentResponse GetEmailSent(string FromDate, string ToDate, long UserId, string SCondition, int PageNo, int PageSize)
        {
            try
            {
                long TotalCount = 0;
                int PageSize1 = 0;
                long TotalPages = 0;

                GetEmailSentResponse Response = new GetEmailSentResponse();

                var _Res = _affiliateBackOfficeRepository.GetEmailSent(FromDate, ToDate, UserId, SCondition, PageSize, PageNo, ref TotalCount, ref PageSize1, ref TotalPages);

                if (_Res.Count > 0)
                {
                    Response.Response = _Res;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Response.Response = new List<GetEmailSentData>();
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoDataFound;
                    Response.ReturnMsg = "No Data Found";
                }

                Response.TotalPages = TotalPages;
                Response.TotalCount = TotalCount;
                Response.PageNo = PageNo;
                Response.PageSize = PageSize1;

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public GetSMSSentResponse GetSMSSent(string FromDate, string ToDate, long UserId, string SCondition, int PageNo, int PageSize)
        {
            try
            {
                long TotalCount = 0;
                int PageSize1 = 0;
                long TotalPages = 0;

                GetSMSSentResponse Response = new GetSMSSentResponse();

                var _Res = _affiliateBackOfficeRepository.GetSMSSent(FromDate, ToDate, UserId, SCondition, PageSize, PageNo, ref TotalCount, ref PageSize1, ref TotalPages);

                if (_Res.Count > 0)
                {
                    Response.Response = _Res;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Response.Response = new List<GetSMSSentData>();
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoDataFound;
                    Response.ReturnMsg = "No Data Found";
                }

                Response.TotalPages = TotalPages;
                Response.TotalCount = TotalCount;
                Response.PageNo = PageNo;
                Response.PageSize = PageSize1;

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetAllAffiliateUserData> GetAllAffiliateUser()
        {
            try
            {
                List<GetAllAffiliateUserData> Response = new List<GetAllAffiliateUserData>();
                Response = _affiliateBackOfficeRepository.GetAllAffiliateUser();

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool IsValidDateFormate(string date)
        {
            try
            {
                DateTime dt = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        // AffiliateSchemeTypeMapping CRUD develop by Pratik :: 15-3-2019

        #region Affiliate Scheme Type Mapping

        public BizResponseClass AddAffiliateSchemeTypeMapping(AffiliateSchemeTypeMappingViewModel Req, long UserID)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = _affiliateSchemeTypeMappingRepository.GetSingle(i => i.SchemeMstId == Req.SchemeMasterId && i.SchemeTypeMstId == Req.SchemeTypeMasterId);
                if (IsExist != null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.DataAlreadyExist;
                    Resp.ErrorCode = enErrorCode.AfRecordAlreadyExist;
                    return Resp;
                }
                AffiliateSchemeTypeMapping ObjAffiliateSchemeTypeMapping = new AffiliateSchemeTypeMapping()
                {
                    SchemeMstId = Req.SchemeMasterId,
                    SchemeTypeMstId = Req.SchemeTypeMasterId,
                    MinimumDepositionRequired = Req.MinimumDepositionRequired,
                    DepositWalletTypeId = Req.DepositWalletTypeId,
                    CommissionTypeInterval = Convert.ToInt16(Req.CommissionTypeInterval),
                    Description = Req.Description,
                    CommissionHour = Req.CommissionHour,
                    CreatedBy = UserID,
                    CreatedDate = Helpers.UTC_To_IST(),
                    Status = Convert.ToInt16(Req.Status)
                };
                _affiliateSchemeTypeMappingRepository.Add(ObjAffiliateSchemeTypeMapping);
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                Resp.ErrorCode = enErrorCode.AfRecordAdded;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BizResponseClass UpdateAffiliateSchemeTypeMapping(AffiliateSchemeTypeMappingUpdateViewModel model, long UserId)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = _affiliateSchemeTypeMappingRepository.GetSingle(i => i.Id == model.MappingId);
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.AfNoRecordFound;
                    return Resp;
                }
                else
                {
                    IsExist.MinimumDepositionRequired = model.MinimumDepositionRequired;
                    IsExist.DepositWalletTypeId = model.DepositWalletTypeId;
                    IsExist.CommissionTypeInterval = Convert.ToInt16(model.CommissionTypeInterval);
                    IsExist.Description = model.Description;
                    IsExist.CommissionHour = model.CommissionHour;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();
                    IsExist.UpdatedBy = UserId;
                    IsExist.Status = Convert.ToInt16(model.Status);
                    _affiliateSchemeTypeMappingRepository.Update(IsExist);

                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                    Resp.ErrorCode = enErrorCode.AfRecordUpdated;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<BizResponseClass> ChangeAffiliateSchemeTypeMappingStatus(SchemeTypeMappingChangeStatusViewModel request, long id)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = await _affiliateSchemeTypeMappingRepository.GetSingleAsync(i => i.Id == request.MappingId);
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.AfNoRecordFound;
                    return Resp;
                }
                IsExist.Status = Convert.ToInt16(request.Status);
                IsExist.UpdatedBy = id;
                IsExist.UpdatedDate = Helpers.UTC_To_IST();
                await _affiliateSchemeTypeMappingRepository.UpdateAsync(IsExist);

                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                Resp.ErrorCode = enErrorCode.AfRecordUpdated;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public GetByIdSchemeTypeMapping GetAffiliateSchemeTypeMapping(long Id)
        {
            try
            {
                var IsExist = _affiliateBackOfficeRepository.GetSchemeTypeMappingData(Id);
                GetByIdSchemeTypeMapping modeldata = new GetByIdSchemeTypeMapping();
                if (IsExist == null)
                {
                    modeldata.ReturnCode = enResponseCode.Fail;
                    modeldata.ReturnMsg = EnResponseMessage.NotFound;
                    modeldata.ErrorCode = enErrorCode.AfNoRecordFound;
                    return modeldata;
                }
                else
                {
                    modeldata.Data = new AffiliateSchemeTypeMappingListViewModel()
                    {
                        MappingId = IsExist.MappingId,
                        SchemeMasterId = IsExist.SchemeMasterId,
                        SchemeTypeMasterId = IsExist.SchemeTypeMasterId,
                        MinimumDepositionRequired = IsExist.MinimumDepositionRequired,
                        DepositWalletTypeId = IsExist.DepositWalletTypeId,
                        CommissionTypeInterval = IsExist.CommissionTypeInterval,
                        Description = IsExist.Description,
                        CommissionHour = IsExist.CommissionHour,
                        Status = IsExist.Status,
                        SchemeName = IsExist.SchemeName,
                        SchemeTypeName = IsExist.SchemeTypeName,
                        DepositWalletTypeName = IsExist.DepositWalletTypeName,
                        UserId = IsExist.UserId,
                        UserName = IsExist.UserName
                    };
                    modeldata.ReturnCode = enResponseCode.Success;
                    modeldata.ReturnMsg = EnResponseMessage.FindRecored;
                    modeldata.ErrorCode = enErrorCode.AfRecordFound;
                }
                return modeldata;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public AffiliateSchemeTypeMappingListResponse ListAffiliateSchemeTypeMapping(long? SchemeId, long? SchemeTypeId, int PageNo, int? PageSize)
        {
            try
            {
                var items = _affiliateBackOfficeRepository.ListSchemeTypeMappingData();
                if (SchemeId != null)
                {
                    items = items.Where(x => x.SchemeMasterId == SchemeId).ToList();
                }
                if (SchemeTypeId != null)
                {
                    items = items.Where(x => x.SchemeTypeMasterId == SchemeTypeId).ToList();
                }
                PageSize = Convert.ToInt32(PageSize == null ? Helpers.PageSize : PageSize);
                AffiliateSchemeTypeMappingListResponse obj = new AffiliateSchemeTypeMappingListResponse();
                obj.AffiliateSchemeTypeMappingList = items.OrderByDescending(x => x.MappingId).ToList();
                obj.TotalCount = items.Count;
                obj.PageNo = PageNo;
                obj.PageSize = Convert.ToInt32(PageSize);
                if (items.Count > 0)
                {
                    PageNo = PageNo + 1;
                    if (PageNo > 0)
                    {
                        int skip = Convert.ToInt32(PageSize) * (PageNo - 1);
                        obj.AffiliateSchemeTypeMappingList = obj.AffiliateSchemeTypeMappingList.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                    }
                    obj.ReturnCode = enResponseCode.Success;
                    obj.ReturnMsg = EnResponseMessage.FindRecored;
                    obj.ErrorCode = enErrorCode.AfRecordFound;
                }
                else
                {
                    //Add List for Data By Rushali (19-12-2019)
                    obj.AffiliateSchemeTypeMappingList = new List<AffiliateSchemeTypeMappingListViewModel>();
                    obj.ReturnCode = enResponseCode.Fail;
                    obj.ReturnMsg = EnResponseMessage.NotFound;
                    obj.ErrorCode = enErrorCode.AfNoRecordFound;
                }
                return obj;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Scheme Master CRUD

        public async Task<BizResponseClass> AddAffiliateScheme(AffiliateSchemeMasterReqRes request, long UserId)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = await _affiliateSchemeMaster.GetSingleAsync(i => i.SchemeName == request.SchemeName);
                if (IsExist != null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.DataAlreadyExist;
                    Resp.ErrorCode = enErrorCode.AfRecordAlreadyExist;
                    return Resp;
                }
                AffiliateSchemeMaster NewObj = new AffiliateSchemeMaster()
                {
                    SchemeName = request.SchemeName,
                    SchemeType = request.SchemeType,
                    Status = Convert.ToInt16(request.Status == null ? ServiceStatus.Active : request.Status),
                    CreatedBy = UserId,
                    CreatedDate = Helpers.UTC_To_IST()
                };
                await _affiliateSchemeMaster.AddAsync(NewObj);
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                Resp.ErrorCode = enErrorCode.AfRecordAdded;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<BizResponseClass> UpdateAffiliateScheme(AffiliateSchemeMasterReqRes request, long Userid)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = await _affiliateSchemeMaster.GetSingleAsync(i => i.Id == request.SchemeMasterId);
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.AfNoRecordFound;
                    return Resp;
                }
                var IsDuplicate = await _affiliateSchemeMaster.GetSingleAsync(i => i.SchemeName == request.SchemeName && i.Id != request.SchemeMasterId);
                if (IsDuplicate != null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.DuplicateRecord;
                    Resp.ErrorCode = enErrorCode.AfDuplicateRecord;
                    return Resp;
                }
                IsExist.SchemeName = request.SchemeName;
                IsExist.SchemeType = request.SchemeType;
                IsExist.Status = Convert.ToInt16(request.Status == null ? (ServiceStatus)IsExist.Status : request.Status);
                await _affiliateSchemeMaster.UpdateAsync(IsExist);

                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                Resp.ErrorCode = enErrorCode.AfRecordUpdated;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<BizResponseClass> ChangeAffiliateSchemeStatus(ChangeAffiliateSchemeStatus request, long id)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = await _affiliateSchemeMaster.GetSingleAsync(i => i.Id == request.SchemeMasterId);
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.AfNoRecordFound;
                    return Resp;
                }
                IsExist.Status = Convert.ToInt16(request.Status);
                IsExist.UpdatedBy = id;
                IsExist.UpdatedDate = Helpers.UTC_To_IST();
                await _affiliateSchemeMaster.UpdateAsync(IsExist);

                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                Resp.ErrorCode = enErrorCode.AfRecordUpdated;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<GetAffiliateSchemeMasterRes> GetAffiliateSchemeById(long affiliateSchemeId, long id)
        {
            GetAffiliateSchemeMasterRes Resp = new GetAffiliateSchemeMasterRes();
            try
            {
                var IsExist = await _affiliateSchemeMaster.GetSingleAsync(i => i.Id == affiliateSchemeId);
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.AfNoRecordFound;
                    return Resp;
                }
                Resp.Data = new GetAllAffiliateSchemeMasterRes
                {
                    SchemeMasterId = IsExist.Id,
                    SchemeName = IsExist.SchemeName,
                    SchemeType = IsExist.SchemeType,
                    Status = IsExist.Status
                };

                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                Resp.ErrorCode = enErrorCode.AfRecordFound;

                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<ListAffiliateSchemeMasterRes> ListAffiliateSchemeById(int PageNo, int? PageSize)
        {
            ListAffiliateSchemeMasterRes Resp = new ListAffiliateSchemeMasterRes();
            try
            {
                PageSize = Convert.ToInt32(PageSize == null ? Helpers.PageSize : PageSize);
                var data = _affiliateBackOfficeRepository.GetAffiliateSchemeMasterdata();
                if (data == null || data.Count <= 0)
                {
                    // Add List for Data Rushali (19-12-2019)
                    Resp.Data = new List<GetAllAffiliateSchemeMasterRes>();
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.AfNoRecordFound;
                    return Resp;
                }
                Resp.Data = data;
                Resp.PageNo = PageNo;
                Resp.PageSize = Convert.ToInt32(PageSize);
                Resp.TotalCount = Resp.Data.Count;
                PageNo = PageNo + 1;
                if (PageNo > 0)
                {
                    int skip = Convert.ToInt32(PageSize) * (PageNo - 1);
                    Resp.Data = Resp.Data.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                }
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                Resp.ErrorCode = enErrorCode.AfRecordFound;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Scheme Type Master CRUD

        public async Task<BizResponseClass> AddAffiliateSchemeType(AffiliateSchemeTypeMasterReq request, long id)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = await _affiliateSchemeTypeMaster.GetSingleAsync(i => i.SchemeTypeName == request.SchemeTypeName);
                if (IsExist != null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.DataAlreadyExist;
                    Resp.ErrorCode = enErrorCode.AfRecordAlreadyExist;
                    return Resp;
                }
                AffiliateSchemeTypeMaster NewObj = new AffiliateSchemeTypeMaster()
                {
                    SchemeTypeName = request.SchemeTypeName,
                    Description = request.Description,
                    Status = Convert.ToInt16(request.Status == null ? ServiceStatus.Active : request.Status),
                    CreatedBy = id,
                    CreatedDate = Helpers.UTC_To_IST()
                };
                await _affiliateSchemeTypeMaster.AddAsync(NewObj);
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                Resp.ErrorCode = enErrorCode.AfRecordAdded;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<BizResponseClass> UpdateAffiliateSchemeType(AffiliateSchemeTypeMasterReq request, long id)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = await _affiliateSchemeTypeMaster.GetSingleAsync(i => i.Id == request.SchemeTypeId);
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.AfNoRecordFound;
                    return Resp;
                }
                var IsDuplicate = await _affiliateSchemeTypeMaster.GetSingleAsync(i => i.SchemeTypeName == request.SchemeTypeName && i.Id != request.SchemeTypeId);
                if (IsDuplicate != null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.DuplicateRecord;
                    Resp.ErrorCode = enErrorCode.AfDuplicateRecord;
                    return Resp;
                }
                IsExist.SchemeTypeName = request.SchemeTypeName;
                IsExist.Description = request.Description;
                IsExist.Status = Convert.ToInt16(request.Status == null ? (ServiceStatus)IsExist.Status : request.Status);
                await _affiliateSchemeTypeMaster.UpdateAsync(IsExist);

                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                Resp.ErrorCode = enErrorCode.AfRecordUpdated;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<BizResponseClass> ChangeAffiliateSchemeTypeStatus(ChangeAffiliateSchemeTypeStatus request, long id)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = await _affiliateSchemeTypeMaster.GetSingleAsync(i => i.Id == request.SchemeTypeId);
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.AfNoRecordFound;
                    return Resp;
                }
                IsExist.Status = Convert.ToInt16(request.Status);
                IsExist.UpdatedBy = id;
                IsExist.UpdatedDate = Helpers.UTC_To_IST();
                await _affiliateSchemeTypeMaster.UpdateAsync(IsExist);

                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                Resp.ErrorCode = enErrorCode.AfRecordUpdated;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<GetAffiliateSchemeTypeMasterRes> GetAffiliateSchemeTypeById(long schemeTypeId, long id)
        {
            GetAffiliateSchemeTypeMasterRes Resp = new GetAffiliateSchemeTypeMasterRes();
            try
            {
                var IsExist = await _affiliateSchemeTypeMaster.GetSingleAsync(i => i.Id == schemeTypeId);
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.AfNoRecordFound;
                    return Resp;
                }
                Resp.Data = new GetAllAffiliateSchemeTypeMasterRes
                {
                    SchemeTypeId = IsExist.Id,
                    SchemeTypeName = IsExist.SchemeTypeName,
                    Description = IsExist.Description,
                    Status = IsExist.Status
                };
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                Resp.ErrorCode = enErrorCode.AfRecordFound;

                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<ListAffiliateSchemeTypeMasterRes> ListAffiliateSchemeTypeById(int PageNo, int? PageSize)
        {
            ListAffiliateSchemeTypeMasterRes Resp = new ListAffiliateSchemeTypeMasterRes();
            try
            {
                PageSize = Convert.ToInt32(PageSize == null ? Helpers.PageSize : PageSize);
                var data = _affiliateBackOfficeRepository.GetAffiliateSchemeTypeMasterdata();
                if (data == null || data.Count <= 0)
                {
                    //Add List for Data Rushali(19-12-2019)
                    Resp.Data = new List<GetAllAffiliateSchemeTypeMasterRes>();
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.AfNoRecordFound;
                    return Resp;
                }
                Resp.Data = data;
                Resp.PageNo = PageNo;
                Resp.PageSize = Convert.ToInt32(PageSize);
                Resp.TotalCount = Resp.Data.Count;
                PageNo = PageNo + 1;
                if (PageNo > 0)
                {
                    int skip = Convert.ToInt32(PageSize) * (PageNo - 1);
                    Resp.Data = Resp.Data.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                }
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                Resp.ErrorCode = enErrorCode.AfRecordFound;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region   Promotion master CRUD

        public async Task<BizResponseClass> AddAffiliatePromotion(AffiliatePromotionMasterReq request, long id)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = await _AffiliatePromotionMaster.GetSingleAsync(i => i.PromotionType == request.PromotionType);
                if (IsExist != null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.DataAlreadyExist;
                    Resp.ErrorCode = enErrorCode.AfRecordAlreadyExist;
                    return Resp;
                }
                AffiliatePromotionMaster NewObj = new AffiliatePromotionMaster()
                {
                    PromotionType = request.PromotionType,
                    Status = Convert.ToInt16(request.Status == null ? ServiceStatus.Active : request.Status),
                    CreatedBy = id,
                    CreatedDate = Helpers.UTC_To_IST()
                };
                await _AffiliatePromotionMaster.AddAsync(NewObj);
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                Resp.ErrorCode = enErrorCode.AfRecordAdded;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<BizResponseClass> UpdateAffiliatePromotion(AffiliatePromotionMasterReq request, long id)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = await _AffiliatePromotionMaster.GetSingleAsync(i => i.Id == request.PromotionId);
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.AfNoRecordFound;
                    return Resp;
                }
                var IsDuplicate = await _AffiliatePromotionMaster.GetSingleAsync(i => i.PromotionType == request.PromotionType && i.Id != request.PromotionId);//&& i.Status != 9
                if (IsDuplicate != null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.DuplicateRecord;
                    Resp.ErrorCode = enErrorCode.DuplicateRecord;
                    return Resp;
                }
                IsExist.PromotionType = request.PromotionType;
                IsExist.Status = Convert.ToInt16(request.Status == null ? (ServiceStatus)IsExist.Status : request.Status);
                await _AffiliatePromotionMaster.UpdateAsync(IsExist);

                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                Resp.ErrorCode = enErrorCode.AfRecordUpdated;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<BizResponseClass> ChangeAffiliatePromotionStatus(ChangeAffiliatePromotionStatus request, long id)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = await _AffiliatePromotionMaster.GetSingleAsync(i => i.Id == request.PromotionId);
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.AfNoRecordFound;
                    return Resp;
                }
                IsExist.Status = Convert.ToInt16(request.Status);
                IsExist.UpdatedBy = id;
                IsExist.UpdatedDate = Helpers.UTC_To_IST();
                await _AffiliatePromotionMaster.UpdateAsync(IsExist);

                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                Resp.ErrorCode = enErrorCode.AfRecordUpdated;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<GetAffiliatePromotionMasterRes> GetAffiliatePromotionById(long PromotionId, long id)
        {
            GetAffiliatePromotionMasterRes Resp = new GetAffiliatePromotionMasterRes();
            try
            {
                var IsExist = await _AffiliatePromotionMaster.GetSingleAsync(i => i.Id == PromotionId);
                if (IsExist == null)
                {
                    Resp.Data = new GetAllAffiliatePromotionMasterRes();
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.AfNoRecordFound;
                    return Resp;
                }
                Resp.Data = new GetAllAffiliatePromotionMasterRes
                {
                    PromotionId = IsExist.Id,
                    PromotionType = IsExist.PromotionType,
                    Status = IsExist.Status
                };
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                Resp.ErrorCode = enErrorCode.AfRecordFound;

                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<ListAllAffiliatePromotionMasterRes> ListAffiliatePromotion(int PageNo, int? PageSize)
        {
            ListAllAffiliatePromotionMasterRes Resp = new ListAllAffiliatePromotionMasterRes();
            try
            {
                PageSize = Convert.ToInt32(PageSize == null ? Helpers.PageSize : PageSize);
                var data = _affiliateBackOfficeRepository.ListAffiliatePromotion();
                if (data == null || data.Count <= 0)
                {
                    Resp.Data = new List<GetAllAffiliatePromotionMasterRes>();
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.AfNoRecordFound;
                    return Resp;
                }
                Resp.Data = data;
                Resp.PageNo = PageNo;
                Resp.PageSize = Convert.ToInt32(PageSize);
                Resp.TotalCount = Resp.Data.Count;
                PageNo = PageNo + 1;
                if (PageNo > 0)
                {
                    int skip = Convert.ToInt32(PageSize) * (PageNo - 1);
                    Resp.Data = Resp.Data.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                }
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                Resp.ErrorCode = enErrorCode.AfRecordFound;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Scheme detail 

        public async Task<ListAffiliateShemeDetailRes> ListAffiliateSchemeDetail(int PageNo, int? PageSize)
        {
            ListAffiliateShemeDetailRes Resp = new ListAffiliateShemeDetailRes();
            try
            {
                PageSize = Convert.ToInt32(PageSize == null ? Helpers.PageSize : PageSize);
                var data = _affiliateBackOfficeRepository.ListAffiliateSchemeDetail();
                if (data == null || data.Count <= 0)
                {
                    Resp.Data = new List<GetAffiliateShemeDetailRes>();
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.AfNoRecordFound;
                    return Resp;
                }
                Resp.Data = data;
                Resp.PageNo = PageNo;
                Resp.PageSize = Convert.ToInt32(PageSize);
                Resp.TotalCount = Resp.Data.Count;
                PageNo = PageNo + 1;
                if (PageNo > 0)
                {
                    int skip = Convert.ToInt32(PageSize) * (PageNo - 1);
                    Resp.Data = Resp.Data.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                }

                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                Resp.ErrorCode = enErrorCode.AfRecordFound;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<GetAffiliateShemeDetailResById> GetAffiliateSchemeDetail(long Id)
        {
            GetAffiliateShemeDetailResById Resp = new GetAffiliateShemeDetailResById();
            try
            {
                var data = _affiliateBackOfficeRepository.GetAffiliateSchemeDetail(Id);
                if (data == null)
                {
                    Resp.Data = new GetAffiliateShemeDetailRes();
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.AfNoRecordFound;
                    return Resp;
                }
                Resp.Data = data;
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                Resp.ErrorCode = enErrorCode.AfRecordFound;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<BizResponseClass> ChangeAffiliateShemeDetailStatus(ChangeAffiliateSchemeDetailStatus request, long id)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = await _AffiliateSchemeDetail.GetSingleAsync(i => i.Id == request.DetailId);
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.AfNoRecordFound;
                    return Resp;
                }
                IsExist.Status = Convert.ToInt16(request.Status);
                IsExist.UpdatedBy = id;
                IsExist.UpdatedDate = Helpers.UTC_To_IST();
                await _AffiliateSchemeDetail.UpdateAsync(IsExist);

                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                Resp.ErrorCode = enErrorCode.AfRecordUpdated;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<BizResponseClass> AddAffiliateSchemeDetail(AffiliateShemeDetailReq request, long id)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                if (request.SchemeMappingId <= 0)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.AfMappingIdRequired;
                    Resp.ErrorCode = enErrorCode.AfSchemeTypeMappingIdRequired;
                    return Resp;
                }
                var MappingData = await _affiliateSchemeTypeMappingRepository.GetSingleAsync(i => i.Id == request.SchemeMappingId && i.Status == 1);
                if (MappingData == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.AfMappingDataNotFound;
                    Resp.ErrorCode = enErrorCode.AfNoRecordFound;
                    return Resp;
                }
                if (MappingData.SchemeMstId == 1)
                {
                    if (request.Level == 0)
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.AfInvalidLevelForMLM;
                        Resp.ErrorCode = enErrorCode.AfInvalidLevel;
                        return Resp;
                    }
                }
                if (MappingData.SchemeMstId != 1)
                {
                    if (request.Level != 0)
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.AfInvalidLevel;
                        Resp.ErrorCode = enErrorCode.AfInvalidLevel;
                        return Resp;
                    }
                }
                var IsExist = await _AffiliateSchemeDetail.GetSingleAsync(i => i.SchemeMappingId == request.SchemeMappingId && i.Level == request.Level && i.Status == 1);
                if (IsExist != null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.DataAlreadyExist;
                    Resp.ErrorCode = enErrorCode.AfRecordAlreadyExist;
                    return Resp;
                }
                int data = _affiliateBackOfficeRepository.GetRange(request.MaximumValue, request.MinimumValue, request.SchemeMappingId);
                if (data == 0)//for insert into db=1 and 0=give error
                {
                    Resp.ErrorCode = enErrorCode.AfInvalidMinMaxValue;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.InvalidValue;
                    return Resp;
                }
                AffiliateSchemeDetail NewObj = new AffiliateSchemeDetail
                {
                    SchemeMappingId = request.SchemeMappingId,
                    CommissionType = request.CommissionType,
                    CommissionValue = request.CommissionValue,
                    CreatedBy = id,
                    CreatedDate = Helpers.UTC_To_IST(),
                    CreditWalletTypeId = request.CreditWalletTypeId,
                    DistributionType = request.DistributionType,
                    Level = request.Level,
                    MaximumValue = request.MaximumValue,
                    MinimumValue = request.MinimumValue,
                    Status = Convert.ToInt16(request.Status),
                    TrnWalletTypeId = request.TrnWalletTypeId
                };
                await _AffiliateSchemeDetail.AddAsync(NewObj);
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                Resp.ErrorCode = enErrorCode.AfRecordAdded;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<BizResponseClass> UpdateAffiliateSchemeDetail(AffiliateShemeDetailReq request, long id)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = await _AffiliateSchemeDetail.GetSingleAsync(i => i.Id == request.DetailId);
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.AfNoRecordFound;
                    return Resp;
                }
                var IsDuplicate = await _AffiliateSchemeDetail.GetSingleAsync(i => i.SchemeMappingId == request.SchemeMappingId && i.Level == request.Level && i.Status == 1 && i.Id != request.DetailId);
                if (IsDuplicate != null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.DataAlreadyExist;
                    Resp.ErrorCode = enErrorCode.AfRecordAlreadyExist;
                    return Resp;
                }
                if (request.SchemeMappingId <= 0)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.AfMappingIdRequired;
                    Resp.ErrorCode = enErrorCode.AfSchemeTypeMappingIdRequired;
                    return Resp;
                }
                var MappingData = await _affiliateSchemeTypeMappingRepository.GetSingleAsync(i => i.Id == request.SchemeMappingId);
                if (MappingData == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.AfMappingDataNotFound;
                    Resp.ErrorCode = enErrorCode.AfNoRecordFound;
                    return Resp;
                }
                if (MappingData.SchemeMstId == 1)
                {
                    if (request.Level == 0)
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.AfInvalidLevel;
                        Resp.ErrorCode = enErrorCode.AfInvalidLevel;
                        return Resp;
                    }
                }
                if (MappingData.SchemeMstId != 1)
                {
                    if (request.Level != 0)
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.AfInvalidLevel;
                        Resp.ErrorCode = enErrorCode.AfInvalidLevel;
                        return Resp;
                    }
                }
                //2019-7-18 added range validation as per nupoora ben instruction
                //int rangedata = _affiliateBackOfficeRepository.GetRange(request.MaximumValue, request.MinimumValue, request.SchemeMappingId);
                //if(rangedata==0)
                //{
                //    Resp.ErrorCode = enErrorCode.AfInvalidMinMaxValue;
                //    Resp.ReturnCode = enResponseCode.Fail;
                //    Resp.ReturnMsg = EnResponseMessage.InvalidValue;
                //    return Resp;
                //}//Commite by mansi-21-10-2019
                int data = _affiliateBackOfficeRepository.GetDetailId(Convert.ToInt64(request.DetailId), request.SchemeMappingId,request.MaximumValue, request.MinimumValue);
                if (data == 0)//for insert into db=1 and 0=give error
                {
                    Resp.ErrorCode = enErrorCode.AfInvalidMinMaxValue;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.InvalidValue;
                    return Resp;
                }
                IsExist.SchemeMappingId = request.SchemeMappingId;
                IsExist.CommissionType = request.CommissionType;
                IsExist.CommissionValue = request.CommissionValue;
                //IsExist.CreatedBy = id;
                //IsExist.CreatedDate = Helpers.UTC_To_IST();
                IsExist.UpdatedBy= id;
                IsExist.UpdatedDate= Helpers.UTC_To_IST();
                IsExist.CreditWalletTypeId = request.CreditWalletTypeId;
                IsExist.DistributionType = request.DistributionType;
                IsExist.Level = request.Level;
                IsExist.MaximumValue = request.MaximumValue;
                IsExist.MinimumValue = request.MinimumValue;
                IsExist.Status = Convert.ToInt16(request.Status);
                IsExist.TrnWalletTypeId = request.TrnWalletTypeId;

                await _AffiliateSchemeDetail.UpdateAsync(IsExist);
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                Resp.ErrorCode = enErrorCode.AfRecordUpdated;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region History Report

        public ListAffiliateCommissionHistoryReport AffiliateCommissionHistoryReport(int PageNo, int PageSize, DateTime? FromDate, DateTime? ToDate, long? TrnUserId, long? AffiliateUserId, long? SchemeMappingId, long? TrnRefNo)
        {
            try
            {
                ListAffiliateCommissionHistoryReport Resp = new ListAffiliateCommissionHistoryReport();
                Resp.PageNo = PageNo;
                Resp.PageSize = PageSize;
                int TotalCount = 0;
                var data = _affiliateBackOfficeRepository.AffiliateCommissionHistoryReport(PageNo + 1, PageSize, FromDate, ToDate, TrnUserId, AffiliateUserId, SchemeMappingId, TrnRefNo, ref TotalCount);
                if (data.Count() == 0)
                {
                    Resp.Data = new List<AffiliateCommissionHistoryReport>();
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    return Resp;
                }
                Resp.Data = data;
                Resp.TotalCount = TotalCount;
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ErrorCode = enErrorCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region  Graph API

        public ListInviteFrdClaas GetAffiliateInviteFrieds()
        {
            try
            {
                ListInviteFrdClaas Resp = new ListInviteFrdClaas();
                var data = _affiliateBackOfficeRepository.GetAffiliateInviteFrieds();
                if (data == null)
                {
                    Resp.Data = new InviteFrdClaas();
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    return Resp;
                }
                Resp.Data = data;
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ErrorCode = enErrorCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public ListGetMonthWiseCommissionData GetMonthWiseCommissionChartDetail(int? Year)
        {
            try
            {
                ListGetMonthWiseCommissionData Resp = new ListGetMonthWiseCommissionData();
                var data = _affiliateBackOfficeRepository.GetMonthWiseCommissionChartDetail(Year);
                if (data==null)
                {
                    Resp.Response = new GetMonthWiseCommissionDataV1();
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    return Resp;
                }
                Resp.Response = data;
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ErrorCode = enErrorCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion
    }
}
