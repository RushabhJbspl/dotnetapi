using Worldex.Core.Entities.KYC;
using Worldex.Core.Entities.KYCConfiguration;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces.KYCConfiguration;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.KYC;
using Worldex.Core.ViewModels.KYCConfiguration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Internal.Networking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Services.KYCConfiguration
{
    /// <summary>
    /// This Controller are use for configuration kyc opration
    /// </summary>
    public class KYCConfigurationServices : IKYCConfiguration
    {
        private readonly WorldexContext _dbContext;
        private readonly ICustomExtendedRepository<KYCIdentityMaster> _KYCIdentityRepository;
        private readonly ICustomExtendedRepository<KYCIdentityConfigurationMapping> _KYCIdentityConfigurationRepository;
        private readonly ICustomRepository<PersonalVerification> _personalVerificationRepository;
        public KYCConfigurationServices(WorldexContext dbContext, ICustomExtendedRepository<KYCIdentityMaster> KYCIdentityRepository,
            ICustomExtendedRepository<KYCIdentityConfigurationMapping> KYCIdentityConfigurationMapping,
            ICustomRepository<PersonalVerification> customRepository)
        {
            _dbContext = dbContext;
            _KYCIdentityRepository = KYCIdentityRepository;
            _KYCIdentityConfigurationRepository = KYCIdentityConfigurationMapping;
            _personalVerificationRepository = customRepository;
        }

        /// <summary>
        /// this method are use for add KYC Identiry 
        /// </summary>
        /// <param name="kYCIdentityMasterInsertReqViewModel"></param>
        /// <returns></returns>
        public Guid Add(KYCIdentityMasterInsertReqViewModel kYCIdentityMasterInsertReqViewModel)
        {
            try
            {
                var KYCConfiguration = _KYCIdentityRepository.Table.FirstOrDefault(i => i.Name == kYCIdentityMasterInsertReqViewModel.Name);
                if (KYCConfiguration == null)
                {
                    KYCIdentityMaster kYCIdentityMaster = new KYCIdentityMaster()
                    {
                        Name = kYCIdentityMasterInsertReqViewModel.Name,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = kYCIdentityMasterInsertReqViewModel.UserId,
                        Status = true,
                        DocumentMasterId = kYCIdentityMasterInsertReqViewModel.DocumentMasterId


                    };
                    _KYCIdentityRepository.Insert(kYCIdentityMaster);
                }

                return Guid.Empty;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Guid IsKYCConfigurationExist(string Name)
        {
            try
            {
                var ISConfigurationExist = _KYCIdentityRepository.Table.FirstOrDefault(i => i.Name == Name);
                if (ISConfigurationExist == null)
                {
                    return Guid.Empty;
                }
                else
                {
                    return ISConfigurationExist.Id;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public Guid Update(KYCIdentityMasterUpdateReqViewModel kYCIdentityMasterUpdateReqViewModel)
        {
            try
            {
                var KYCConfiguration = _KYCIdentityRepository.Table.FirstOrDefault(i => i.Id == kYCIdentityMasterUpdateReqViewModel.Id);
                if (KYCConfiguration != null)
                {
                    KYCConfiguration.Id = kYCIdentityMasterUpdateReqViewModel.Id;
                    KYCConfiguration.UpdatedDate = DateTime.UtcNow;
                    KYCConfiguration.UpdatedBy = kYCIdentityMasterUpdateReqViewModel.UserId;
                    KYCConfiguration.Name = kYCIdentityMasterUpdateReqViewModel.Name;
                    KYCConfiguration.Status = kYCIdentityMasterUpdateReqViewModel.Stutas;
                    KYCConfiguration.DocumentMasterId = kYCIdentityMasterUpdateReqViewModel.DocumentMasterId;
                    _KYCIdentityRepository.Update(KYCConfiguration);
                    return KYCConfiguration.Id;
                }
                else
                    return Guid.Empty;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Guid AddUserKYCMappingConfiguration(UserKYCConfigurationMappingReqViewModel userKYCConfigurationMapping)
        {
            try
            {
                var KYCConfiguration = _KYCIdentityConfigurationRepository.Table.FirstOrDefault(i => i.Userid == userKYCConfigurationMapping.UserId &&
                i.KYCConfigurationMasterId == userKYCConfigurationMapping.KYCConfigurationMasterId);
                if (KYCConfiguration == null)
                {
                    KYCIdentityConfigurationMapping kYCIdentityConfigurationMapping = new KYCIdentityConfigurationMapping()
                    {
                        Userid = userKYCConfigurationMapping.UserId,
                        KYCConfigurationMasterId = userKYCConfigurationMapping.KYCConfigurationMasterId,
                        Status = true,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = userKYCConfigurationMapping.UserId,
                        LevelId = userKYCConfigurationMapping.LevelId

                    };
                    _KYCIdentityConfigurationRepository.Insert(kYCIdentityConfigurationMapping);
                    return kYCIdentityConfigurationMapping.Id;
                }
                else
                    return Guid.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Guid UpdateKYCMappig(UserKYCConfigurationMappingUpdateReqViewModel userKYCConfigurationMappingUpdateReqViewModel)
        {
            try
            {
                var KYCConfigurationMapping = _KYCIdentityConfigurationRepository.Table.FirstOrDefault(i => i.Userid == userKYCConfigurationMappingUpdateReqViewModel.UserId &&
                i.Id == userKYCConfigurationMappingUpdateReqViewModel.Id);
                if (KYCConfigurationMapping != null)
                {
                    KYCConfigurationMapping.Userid = userKYCConfigurationMappingUpdateReqViewModel.UserId;
                    KYCConfigurationMapping.Status = userKYCConfigurationMappingUpdateReqViewModel.Status;
                    KYCConfigurationMapping.UpdatedDate = DateTime.UtcNow;
                    KYCConfigurationMapping.UpdatedBy = userKYCConfigurationMappingUpdateReqViewModel.UserId;
                    KYCConfigurationMapping.LevelId = userKYCConfigurationMappingUpdateReqViewModel.LevelId;
                    _KYCIdentityConfigurationRepository.Update(KYCConfigurationMapping);
                    return KYCConfigurationMapping.Id;
                }
                else
                {
                    return Guid.Empty;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Guid IsKYCConfigurationmappingExist(UserKYCConfigurationMappingReqViewModel userKYCConfigurationMapping)
        {
            try
            {
                var KYCMappingConfiguration = _KYCIdentityConfigurationRepository.Table.FirstOrDefault(i => i.Userid == userKYCConfigurationMapping.UserId &&
               i.KYCConfigurationMasterId == userKYCConfigurationMapping.KYCConfigurationMasterId);
                if (KYCMappingConfiguration == null)
                {
                    return Guid.Empty;
                }
                else
                {
                    return KYCMappingConfiguration.Id;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public List<KYCIndentityMappinglistViewModel> KYCIndentityConfigurationlist(int UserId)
        {
            try
            {
                string Qry = "";
                IQueryable<KYCIndentityMappinglistViewModel> Result;
                Qry = @"Select KICM.Id,KIM.Name From BizUser BU ";
                Qry += " Inner join  KYCIdentityConfigurationMapping KICM On BU.Id=KICM.Userid ";
                Qry += " Inner join kYCIdentityMaster KIM On KICM.KYCConfigurationMasterId=KIM.Id";
                Qry += " where KICM.Status=1 and BU.Id=" + UserId;
                Result = _dbContext.kYCIndentitylistViewModels.FromSql(Qry);
                return Result.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public KYCListFilterationDataListResponseViewModel GetKYCList(DateTime? fromdate, DateTime? todate, int PageIndex = 0, int Page_Size = 0, int Status = 0, string Mobile = null, string EmailAddress = null, string HostURL = null)
        {

            try
            {
                string fromdatetime = string.Empty, todatetime = string.Empty;
                KYCListFilterationDataListResponseViewModel kYCListFilterationData = new KYCListFilterationDataListResponseViewModel();
                List<KYCListFilterationDataViewModel> KYC = new List<KYCListFilterationDataViewModel>();
                List<KYCListFilterationDataViewModel> kYCList = new List<KYCListFilterationDataViewModel>();
                if (fromdate != null && todate != null)
                {
                    if (todate < fromdate)
                    {
                        kYCListFilterationData.ErrorCode = enErrorCode.InvalidFromDate_ToDate;
                        kYCListFilterationData.ReturnCode = enResponseCode.Fail;
                        kYCListFilterationData.ReturnMsg = EnResponseMessage.InvalidFromDate_ToDate;
                        kYCListFilterationData.kYCListFilterationDataViewModels = kYCList;
                        return kYCListFilterationData;
                    }
                }
                if (fromdate != null && todate != null)
                {
                    fromdatetime = fromdate.Value.Add(new TimeSpan(0, 00, 00, 00)).ToString("yyyy-MM-dd HH:mm:ss");
                    todatetime = todate.Value.Add(new TimeSpan(0, 23, 59, 59)).ToString("yyyy-MM-dd HH:mm:ss");
                }

                IQueryable<KYCListFilterationDataViewModel> Result;
                string Qry = "";
                Qry += "Select pv.Id,Bu.FirstName as FirstName,bu.LastName as LastName,KIM.Name AS 'ValidIdentityCard',BU.UserName ,bu.Mobile,pv.FrontImage,pv.BackImage,pv.SelfieImage,pv.VerifyStatus,'' as Statusname,Pv.IdentityDocNumber AS 'ValidIDCardNumber',PV.CreatedDate as Createddate ";
                Qry += " from PersonalVerification Pv";
                Qry += " inner join kYCIdentityMaster KIM on pv.ValidIdentityCard=Kim.Id ";
                Qry += " Inner join kYCLevelMaster KLM On KLM.Id=pv.KYCLevelId ";
                Qry += " Inner join BizUser BU On Bu.Id=pv.UserID Where  ";
                if (!string.IsNullOrEmpty(EmailAddress))
                {
                    Qry += "BU.Email= '" + EmailAddress + "'";
                }
                if (!string.IsNullOrEmpty(Mobile))
                {
                    if (!string.IsNullOrEmpty(EmailAddress))
                        Qry += " And BU.Mobile= '" + Mobile + "'";
                    else
                        Qry += "BU.Mobile= '" + Mobile + "'";
                }

                
                if (!string.IsNullOrEmpty(EmailAddress) || !string.IsNullOrEmpty(Mobile))
                {
                    if (Status > 0)
                        Qry += " and  PV.VerifyStatus = " + Status;
                }
                else
                {
                    Qry += "PV.VerifyStatus= case when " + Status + " >0 then " + Status + " else  PV.VerifyStatus end ";
                }

                if (fromdate != null && todate != null)
                {
                    if (!string.IsNullOrEmpty(EmailAddress) || !string.IsNullOrEmpty(Mobile) || Status > 0)
                        Qry += " and PV.CreatedDate Between '" + fromdatetime + "' and '" + todatetime + "'";
                    else
                        Qry += " and PV.CreatedDate Between '" + fromdatetime + "'  and '" + todatetime + "'";
                }


                //string fromdatetime = string.Empty, todatetime = string.Empty;
                //if (fromdate != null && todate != null)
                //{
                //    fromdatetime = fromdate.Value.Add(new TimeSpan(0, 00, 00, 00)).ToString("yyyy-MM-dd HH:mm:ss");
                //    todatetime = todate.Value.Add(new TimeSpan(0, 23, 59, 59)).ToString("yyyy-MM-dd HH:mm:ss");
                //}


                if (fromdate == null && todate == null)
                {
                    DateTime date = DateTime.Now;
                    string fromdatetm = date.Date.Add(new TimeSpan(0, 00, 00, 00)).ToString("yyyy-MM-dd HH:mm:ss");
                    string todatetm = date.Date.Add(new TimeSpan(0, 23, 59, 59)).ToString("yyyy-MM-dd HH:mm:ss");

                    if (!string.IsNullOrEmpty(EmailAddress) || !string.IsNullOrEmpty(Mobile) || Status > 0)
                        Qry += " and PV.CreatedDate between '" + fromdatetm + "' and '" + todatetm + "'";
                    else
                        Qry += " and PV.CreatedDate between '" + fromdatetm + "' and '" + todatetm + "'";
                }

                Qry += " order by  pv.CreatedDate desc";
                Result = _dbContext.kYCListFilterationResponseViewModels.FromSql(Qry);


                //List<KYCListFilterationDataViewModel> KYC = new List<KYCListFilterationDataViewModel>();
                //List<KYCListFilterationDataViewModel> kYCList = new List<KYCListFilterationDataViewModel>();
                KYC = Result.ToList();

                foreach (var item in KYC)
                {
                    KYCListFilterationDataViewModel Model = new KYCListFilterationDataViewModel();
                    Model.Id = item.Id;
                    Model.FirstName = item.FirstName;
                    Model.LastName = item.LastName;
                    Model.UserName = item.UserName;
                    Model.Mobile = item.Mobile;
                    Model.ValidIdentityCard = item.ValidIdentityCard;
                    if (!string.IsNullOrEmpty(HostURL))
                    {
                        Model.FrontImage = HostURL + item.FrontImage;
                        Model.BackImage = HostURL + item.BackImage;
                        Model.SelfieImage = HostURL + item.SelfieImage;
                    }
                    else
                    {
                        Model.FrontImage = item.FrontImage;
                        Model.BackImage = item.BackImage;
                        Model.SelfieImage = item.SelfieImage;
                    }
                    Model.ValidIDCardNumber = item.ValidIDCardNumber;
                    Model.VerifyStatus = item.VerifyStatus;

                    switch ((KYCStatus)Enum.Parse(typeof(KYCStatus), item.VerifyStatus.ToString()))
                    {
                        case KYCStatus.Approval:
                            Model.Statusname = KYCStatus.Approval.ToString();
                            break;
                        case KYCStatus.Pending:
                            Model.Statusname = KYCStatus.Pending.ToString();
                            break;
                        case KYCStatus.Reject:
                            Model.Statusname = KYCStatus.Reject.ToString();
                            break;
                    }
                    Model.Createddate = item.Createddate;

                    kYCList.Add(Model);
                }


                var total = kYCList.Count();
                if (total == 0)
                {
                    kYCListFilterationData.ErrorCode = enErrorCode.NotFound;
                    kYCListFilterationData.ReturnCode = enResponseCode.Fail;
                    kYCListFilterationData.ReturnMsg = EnResponseMessage.NotFound;
                    kYCListFilterationData.kYCListFilterationDataViewModels = kYCList;
                    return kYCListFilterationData;
                }
                if (PageIndex == 0)
                {
                    PageIndex = 1;
                }

                if (Page_Size == 0)
                {
                    Page_Size = 10;
                }

                var skip = Page_Size * (PageIndex - 1);
                kYCListFilterationData.kYCListFilterationDataViewModels = kYCList.Skip(skip).Take(Page_Size).ToList();
                kYCListFilterationData.TotalCount = total;
                kYCListFilterationData.ErrorCode = enErrorCode.Success;
                kYCListFilterationData.ReturnCode = enResponseCode.Success;
                kYCListFilterationData.ReturnMsg = EnResponseMessage.FindRecored;
                return kYCListFilterationData;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<KYCIndentitylistViewModel> KYCIdentityGetList()
        {
            try
            {
                IQueryable<KYCIndentitylistViewModel> Result;

                string Query = @"Select Id,Name,DocumentMasterId,Status,CreatedDate From kYCIdentityMaster";
                Result = _dbContext.KYCIndentitylistViewModels.FromSql(Query);
                return Result.ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public KYCInsertDocumentId CheckDocumentFormat(string Name)
        {
            try
            {
                var IsDocumentmasterExist = _KYCIdentityRepository.Table.FirstOrDefault(i => i.Name == Name);
                if (IsDocumentmasterExist == null)
                {
                    return null;
                }
                else
                {
                    KYCInsertDocumentId kYCInsertDocumentId = new KYCInsertDocumentId();
                    kYCInsertDocumentId.ID = IsDocumentmasterExist.Id;
                    kYCInsertDocumentId.DocumentMasterId = IsDocumentmasterExist.DocumentMasterId;
                    return kYCInsertDocumentId;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }

        }


        public long KYCVerification(KYCUpdateViewModel kYCUpdateViewModel)
        {
            try
            {

                var GetVerify = _personalVerificationRepository.Table.Where(i => i.Id == kYCUpdateViewModel.id).ToList().FirstOrDefault();
                if (GetVerify != null)
                {
                    GetVerify.Id = kYCUpdateViewModel.id;
                    GetVerify.VerifyStatus = kYCUpdateViewModel.VerifyStatus;
                    GetVerify.Remark = kYCUpdateViewModel.Remark;
                    _personalVerificationRepository.Update(GetVerify);
                    return GetVerify.Id;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {

                ex.ToString();
                throw ex;
            }
        }
    }
}
