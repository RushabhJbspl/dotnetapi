using Worldex.Core.Entities.Complaint;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces.Complaint;
using Worldex.Core.Interfaces.Organization;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.BackOfficeComplain;
using Worldex.Core.ViewModels.Complaint;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Services.Complaint
{
    public class ComplainmasterServices : IComplainmaster
    {
        private readonly WorldexContext _dbContext;
        private readonly ICustomRepository<Complainmaster> _ComplainmasterRepository;
        private readonly ICustomRepository<ComplainStatusTypeMaster> _ComplainStatusRepository;
        private readonly IActivityMasterConfiguration _IactivityMasterConfiguration;

        public ComplainmasterServices(ICustomRepository<Complainmaster> customRepository, WorldexContext context, ICustomRepository<ComplainStatusTypeMaster> ComplainStatusRepository,
            IActivityMasterConfiguration IactivityMasterConfiguration)
        {
            _ComplainmasterRepository = customRepository;
            _dbContext = context;
            _ComplainStatusRepository = ComplainStatusRepository;
            _IactivityMasterConfiguration = IactivityMasterConfiguration;
        }

        public long AddComplainmaster(ComplainmasterReqViewModel model)
        {
            try
            {
                if (model != null)
                {
                    var Compaintmaster = new Complainmaster()
                    {
                        Description = model.Description,
                        Subject = model.Subject,
                        TypeId = model.TypeId,
                        UserID = model.UserID,
                        ComplaintPriorityId = model.ComplaintPriorityId,
                        //Status=_ComplainStatusRepository.Table.Where(i =>i.CompainStatusType == enComplainStatusType.Open.ToString()).FirstOrDefault().Id,
                        Status = Convert.ToInt16(enComplainStatusType.Open),
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = model.UserID
                    };
                    _ComplainmasterRepository.Insert(Compaintmaster);
                    return Compaintmaster.Id;
                }
                return 0;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }

        }

        public GetBackOffComRptResponse GetBackofficeAllData(int UserId, int pageIndex, int pageSize, long ComplainId = 0, string EmailId = null, string MobileNo = null, long Status = 0, long TypeId = 0, int PriorityId = 0,
            DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                var ComplainData = new List<GetBackOffComRpt>();
                int GetUserId = 0;
                //var ComplainDataList = _ComplainmasterRepository.Table.ToList();
                //var ComplainDataList = _dbContext.Complainmaster.ToList();
                var ComplainDataList = (from cm in _ComplainmasterRepository.Table
                                        join cp in _dbContext.ComplaintPriorityMaster on cm.ComplaintPriorityId equals cp.Id
                                        select new
                                        {
                                            Id = cm.Id,
                                            CreatedDate = cm.CreatedDate,
                                            CreatedBy = cm.CreatedBy,
                                            UpdatedBy = cm.UpdatedBy,
                                            UpdatedDate = cm.UpdatedBy,
                                            Status = cm.Status,
                                            UserID = cm.UserID,
                                            TypeId = cm.TypeId,
                                            Subject = cm.Subject,
                                            Description = cm.Description,
                                            Priority = cp.Priority,
                                            ComplaintPriorityId = cm.ComplaintPriorityId
                                        }).OrderByDescending(cm => cm.CreatedDate).ToList();

                //var ComplainDataList = _dbContext.Complainmaster.ToList();
                if (ComplainId > 0)
                {
                    ComplainDataList = ComplainDataList.Where(i => i.Id == ComplainId).ToList();
                }
                if (!string.IsNullOrEmpty(EmailId))
                {
                    GetUserId = _dbContext.Users.Where(i => i.Email == EmailId).Count() > 0 ? _dbContext.Users.Where(i => i.Email == EmailId).FirstOrDefault().Id : 0;
                    if (GetUserId > 0)
                    {
                        //if (ComplainId > 0)
                        //  ComplainDataList = ComplainDataList.Where(i => i.UserID == GetUserId && i.Id == ComplainId).ToList();
                        //else
                        ComplainDataList = ComplainDataList.Where(i => i.UserID == GetUserId).ToList();
                    }
                    else
                        return null;
                }
                if (!string.IsNullOrEmpty(MobileNo))
                {
                    GetUserId = _dbContext.Users.Where(i => i.Mobile == MobileNo).Count() > 0 ? _dbContext.Users.Where(i => i.Mobile == MobileNo).FirstOrDefault().Id : 0;
                    if (GetUserId > 0)
                    {
                        //if (!string.IsNullOrEmpty(EmailId) || GetUserId > 0)
                        // ComplainDataList = ComplainDataList.Where(i => i.UserID == GetUserId).ToList();
                        //else
                        ComplainDataList = ComplainDataList.Where(i => i.UserID == GetUserId).ToList();
                    }
                    else
                        return null;
                }
                if (Status > 0)
                {
                    //if (ComplainId > 0 || !string.IsNullOrEmpty(EmailId) || !string.IsNullOrEmpty(MobileNo) || GetUserId > 0)
                    // ComplainDataList = ComplainDataList.Where(i => i.Status == Status && i.UserID == GetUserId).ToList();
                    //else
                    ComplainDataList = ComplainDataList.Where(i => i.Status == Status).ToList();
                }
                if (TypeId > 0)
                {
                    //if (ComplainId > 0 || Status > 0)
                    //   ComplainDataList = ComplainDataList.Where(i => i.TypeId == TypeId && i.Status == Status).ToList();
                    //else
                    ComplainDataList = ComplainDataList.Where(i => i.TypeId == TypeId).ToList();
                }
                if (PriorityId > 0)
                {
                    ComplainDataList = ComplainDataList.Where(i => i.ComplaintPriorityId == PriorityId).ToList();
                }


                if (FromDate != null && ToDate != null)
                {
                    string FromDatedate = String.Format("{0:yyyy/MM/dd}", FromDate);
                    string ToDatestr = String.Format("{0:yyyy/MM/dd}", ToDate);
                    FromDatedate += " 00:00:00.0000000";
                    ToDatestr = " 23:59:59.999";
                    FromDate = DateTime.Parse(FromDatedate);
                    ToDate = DateTime.Parse(ToDatestr);
                    ComplainDataList = ComplainDataList.Where(i => i.CreatedDate >= FromDate && i.CreatedDate <= ToDate).ToList();
                }
                else
                {
                    string Fromdatestr = DateTime.Now.ToString("yyyy-MM-dd");
                    Fromdatestr += " 00:00:00.0000000";
                    FromDate = DateTime.Parse(Fromdatestr);
                    string todatestr = DateTime.Now.ToString("yyyy-MM-dd");
                    todatestr += " 23:59:59.999";
                    ToDate = DateTime.Parse(todatestr);
                    ComplainDataList = ComplainDataList.Where(i => i.CreatedDate >= FromDate && i.CreatedDate <= ToDate).ToList();
                }


                foreach (var item in ComplainDataList)
                {
                    GetBackOffComRpt ComplainDataViewModel = new GetBackOffComRpt()
                    {
                        UserId = item.UserID,
                        //UserName = _dbContext.Users.Where(i => i.Id == item.UserID).Count() > 0 ? _dbContext.Users.Where(i => i.Id == item.UserID).FirstOrDefault().UserName : string.Empty,
                        UserName = _IactivityMasterConfiguration.GetAlluserData().Where(i => i.Id == item.UserID).Count() > 0 ? _IactivityMasterConfiguration.GetAlluserData().Where(i => i.Id == item.UserID).FirstOrDefault().UserName : string.Empty,
                        Subject = item.Subject,
                        Description = item.Description,
                        //Type = _dbContext.Typemaster.Where(i => i.Id == item.TypeId).Count() > 0 ? _dbContext.Typemaster.Where(i => i.Id == item.TypeId).FirstOrDefault().SubType : string.Empty,
                        Type = _IactivityMasterConfiguration.GetTypeMasterData().Where(i => i.Id == item.TypeId).Count() > 0 ? _IactivityMasterConfiguration.GetTypeMasterData().Where(i => i.Id == item.TypeId).FirstOrDefault().SubType : string.Empty,
                        ComplainId = item.Id,
                        CreatedDate = item.CreatedDate,
                        //Status = _ComplainStatusRepository.Table.Where(i => i.Id == item.Status).Count() > 0 ? _ComplainStatusRepository.Table.Where(i => i.Id == item.Status).FirstOrDefault().CompainStatusType : string.Empty
                        Status = _IactivityMasterConfiguration.GetComplainStatus().Where(i => i.Id == item.Status).Count() > 0 ? _IactivityMasterConfiguration.GetComplainStatus().Where(i => i.Id == item.Status).FirstOrDefault().CompainStatusType : string.Empty,
                        Priority = item.Priority
                    };
                    ComplainData.Add(ComplainDataViewModel);
                }

                var total = ComplainData.Count();
                if (pageIndex == 0)
                    pageIndex = 1;
                if (pageSize == 0)
                    pageSize = 10;

                var skip = pageSize * (pageIndex - 1);
                GetBackOffComRptResponse response = new GetBackOffComRptResponse();
                response.TotalCount = ComplainDataList.Count;
                response.GetTotalCompList = ComplainData.Skip(skip).Take(pageSize).ToList();
                return response;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public GetComplainAllData GetBackOfficeComplain(long ComplainId)
        {
            try
            {
                string Qry = "";
                IQueryable<BackoffComplainMasterViewModel> Result;
                Qry = @" Select cm.id as ComplainId ,cm.Subject as Subject ,Tm.Type as Type,CSTM.CompainStatusType as Status  From Complainmaster cm ";
                Qry += " Inner join Typemaster Tm  on cm.TypeId=tm.Id Inner Join ComplainStatusTypeMaster CSTM on CSTM.Id=cm.Status  where cm.Id=" + ComplainId;
                Qry += " order by cm.CreatedDate desc";

                Result = _dbContext.BackOffComplainData.FromSql(Qry);

                GetComplainAllData BackOfficeComData = new GetComplainAllData();
                BackOfficeComData.ComplainMasterData = Result.ToList();

                Qry = string.Empty;
                IQueryable<BackOffComplainTrailViewModel> ResultComplainDetail;
                Qry = "Select CT.id as TrailID ,CT.Description as Description ,BU.UserName as Username,CT.Remark,CT.CreatedDate as CreatedDate";
                Qry += " from CompainTrail CT Inner join BizUser BU On bu.Id=CT.CreatedBy where CT.ComplainId=" + ComplainId;
                ResultComplainDetail = _dbContext.BackOffCompainTrailData.FromSql(Qry);
                BackOfficeComData.CompainTrailData = ResultComplainDetail.ToList();
                return BackOfficeComData;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public ComplainChildParentViewmodel GetComplain(int ComplainId)
        {
            try
            {
                string Qry = "";
                IQueryable<ComplainMasterDataViewModel> Result;
                Qry = @" Select cm.id as ComplainId ,cm.Subject as Subject ,Tm.Type as Type,cpm.Priority  as Priority   From Complainmaster cm ";
                Qry += " Inner join Typemaster Tm  on cm.TypeId=tm.Id  ";
                Qry += "Inner join ComplaintPriorityMaster CPM On CPM.id=cm.ComplaintPriorityId ";
                Qry += "where cm.Id=" + ComplainId;
                Qry += " order by cm.CreatedDate desc";

                Result = _dbContext.ComplainMasterDataViewModel.FromSql(Qry);

                ComplainChildParentViewmodel complainChildParentViewmodel = new ComplainChildParentViewmodel();
                complainChildParentViewmodel.ComplainMasterDataViewModel = Result.ToList();

                Qry = string.Empty;
                IQueryable<CompainTrailViewModel> ResultComplainDetail;
                Qry = "Select CT.id as TrailID ,CT.Description as Description ,CST.CompainStatusType as Complainstatus,CT.CreatedDate as CreatedDate,BU.UserName as Username";
                Qry += " from CompainTrail CT Inner join BizUser BU On bu.Id=CT.CreatedBy Inner Join ComplainStatusTypeMaster CST on CT.Complainstatus=CST.Id  where CT.ComplainId=" + ComplainId;
                ResultComplainDetail = _dbContext.CompainTrailViewModel.FromSql(Qry);
                complainChildParentViewmodel.CompainTrailViewModel = ResultComplainDetail.ToList();


                return complainChildParentViewmodel;


            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public List<UserWiseCompaintDetailResponce> GetComplainByUserWise(int UserId,string Subject=null, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                string Qry = "";
                IQueryable<UserWiseCompaintDetailResponce> Result;
                //              Qry = @" Select Cm.Subject,Cm.Description,Tm.SubType,Cm.Id as CompainNumber
                //From Complainmaster Cm Inner join Typemaster Tm On Cm.TypeId=tm.Id  Where cm.UserID=" + UserId + " order by Cm.CreatedBy Desc";
                Qry = @" select IsNull((select top 1 CST.CompainStatusType from CompainTrail CT Inner Join  ComplainStatusTypeMaster CST on CT.Complainstatus=CST.Id where CT.ComplainId=CM.Id order by CT.CreatedDate desc),'') as Status ,";
                Qry += " IsNull((select top 1 CT.Remark  from CompainTrail CT where CT.ComplainId=CM.Id order by CT.CreatedDate desc),'') as Remark ,";
                Qry += " IsNull(CM.Subject,'') as Subject,Cm.Id as CompainNumber,IsNull(cm.Description,'') as Description,IsNull(TM.SubType,'')as Type,CM.CreatedDate  as CreatedDate,Bz.Id as UserId,BZ.UserName as UserName,CPM.Priority  ";
                Qry += " from Complainmaster CM ";
                Qry += "Inner join Typemaster TM on TM.Id =cm.TypeId ";
                Qry += "Inner join BizUser BZ On bz.Id = Cm.UserID";
                Qry += " Inner join ComplaintPriorityMaster CPM  On  cm.ComplaintPriorityId=cpm.id";
                //Qry += " Where CM.UserID=case when(" + UserId + "=0) then cm.UserID else " + UserId + " End order by CM.id desc";
                Qry += " Where CM.UserID=case when(" + UserId + "=0) then cm.UserID else " + UserId + " End";

                //Add by Pratik for front panel filter :: 1-04-2019
                if (!string.IsNullOrEmpty(Subject))
                {
                    Qry += " and CM.Subject = '" + Subject + "'";
                }
                if (FromDate != null && ToDate != null)
                {
                    string FromDatedate = String.Format("{0:yyyy/MM/dd}", FromDate);                   
                    FromDatedate += " 00:00:00.0000000";                    
                    FromDate = DateTime.Parse(FromDatedate);

                    string ToDatestr = String.Format("{0:yyyy-MM-dd }", ToDate);                   
                    ToDatestr += " 23:59:59.999";
                    ToDate = DateTime.Parse(ToDatestr);
                    Qry += " and CM.CreatedDate Between '" + FromDatedate + "'  and '" + ToDatestr + "'";                  

                    //Qry += " and CM.CreatedDate  >= '" + FromDate + "'  and CM.CreatedDate <= '" + ToDate + "'";
                }
                Qry += " order by  CM.id desc";

                

                Result = _dbContext.userWiseCompaintDetailResponce.FromSql(Qry);
                
                return Result.ToList();
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public GetTotalCountCom GetTotalComplainCount(long Type = 0, short ComplainStatus = 0, int UserId = 0)
        {
            try
            {
                GetTotalCountCom model = new GetTotalCountCom();
                var GetTotalCount = (dynamic)null;
                int j = 0, k = 0, l = 0;
                if (Type > 0 && ComplainStatus > 0)
                {
                    if (UserId > 0)
                        GetTotalCount = _ComplainmasterRepository.Table.Where(i => i.Status == ComplainStatus && i.TypeId == Type && i.UserID == UserId).ToList();
                    else
                        GetTotalCount = _ComplainmasterRepository.Table.Where(i => i.Status == ComplainStatus && i.TypeId == Type).ToList();
                    if (GetTotalCount != null)
                    {
                        foreach (var item in GetTotalCount)
                        {
                            if (item.Status == Convert.ToInt16(enComplainStatusType.Open))
                                j = j + 1;
                            else if (item.Status == Convert.ToInt16(enComplainStatusType.Close))
                                k = k + 1;
                            else if (item.Status == Convert.ToInt16(enComplainStatusType.Pending))
                                l = l + 1;
                        }
                    }
                    else
                        return null;
                }
                else if (Type > 0)
                {
                    if (UserId > 0)
                        GetTotalCount = _ComplainmasterRepository.Table.Where(i => i.TypeId == Type && i.UserID == UserId).ToList();
                    else
                        GetTotalCount = _ComplainmasterRepository.Table.Where(i => i.TypeId == Type).ToList();
                    if (GetTotalCount != null)
                    {
                        foreach (var item in GetTotalCount)
                        {
                            if (item.Status == Convert.ToInt16(enComplainStatusType.Open))
                                j = j + 1;
                            else if (item.Status == Convert.ToInt16(enComplainStatusType.Close))
                                k = k + 1;
                            else if (item.Status == Convert.ToInt16(enComplainStatusType.Pending))
                                l = l + 1;
                        }
                    }
                    else
                        return null;
                }
                else if (ComplainStatus > 0)
                {
                    if (UserId > 0)
                        GetTotalCount = _ComplainmasterRepository.Table.Where(i => i.Status == ComplainStatus && i.UserID == UserId).ToList();
                    else
                        GetTotalCount = _ComplainmasterRepository.Table.Where(i => i.Status == ComplainStatus).ToList();
                    if (GetTotalCount != null)
                    {
                        foreach (var item in GetTotalCount)
                        {
                            if (item.Status == Convert.ToInt16(enComplainStatusType.Open))
                                j = j + 1;
                            else if (item.Status == Convert.ToInt16(enComplainStatusType.Close))
                                k = k + 1;
                            else if (item.Status == Convert.ToInt16(enComplainStatusType.Pending))
                                l = l + 1;
                        }
                    }
                    else
                        return null;
                }
                else
                {
                    if (UserId > 0)
                        GetTotalCount = _ComplainmasterRepository.Table.Where(i => i.UserID == UserId).ToList();
                    else
                        GetTotalCount = _ComplainmasterRepository.Table.ToList();
                    foreach (var item in GetTotalCount)
                    {
                        if (item.Status == Convert.ToInt16(enComplainStatusType.Open))
                            j = j + 1;
                        else if (item.Status == Convert.ToInt16(enComplainStatusType.Close))
                            k = k + 1;
                        else if (item.Status == Convert.ToInt16(enComplainStatusType.Pending))
                            l = l + 1;
                    }
                }

                if (GetTotalCount.Count > 0)
                {
                    model.TotalCount = GetTotalCount.Count;
                    model.TotalOpenCount = j;
                    model.TotalCloseCount = k;
                    model.TotalPendidngCount = l;
                    return model;
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
    }
}

