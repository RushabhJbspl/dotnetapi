using Worldex.Core.ApiModels;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.BackOfficeReport;
using Worldex.Core.ViewModels.BackOfficeReports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Services.BackofficeReport
{
    public class BackOfficeReportServices : IBackOfficeReport
    {
        private readonly WorldexContext _dbContext;
        private readonly ICommonRepository<SubscribeNewsLetter> _SubscribeNewsLetterRepository;
        private readonly ILogger<BackOfficeReportServices> _logger;

        public BackOfficeReportServices(WorldexContext context, ILogger<BackOfficeReportServices> logger, 
            ICommonRepository<SubscribeNewsLetter> SubscribeNewsLetterRepository)
        {
            _dbContext = context;
            _logger = logger;
            _SubscribeNewsLetterRepository = SubscribeNewsLetterRepository;
        }

        public SignReportModel GetSignUpReport(int PageIndex = 0, int Page_Size = 0,
            string EmailAddress = null, string Username = null, string Mobile = null,
            string filtration = null, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                //string DateTime=
                IQueryable<SignReportViewmodel> SignupReport;
                // Comment by Pratik Change TempUserRegister table to BizUser :: 26-03-2019
                //string Query = @"Select TUR.Id,TUR.UserName,TUR.Email,TUR.Mobile, TUR.FirstName,TUR.LastName,RT.Type As RegType,TUR.RegisterStatus,TUR.CreatedDate   ";
                //Query += " From TempUserRegister  TUR";
                //Query += " Inner Join RegisterType RT  On TUR.RegTypeId=RT.Id where ";



                string Query = @"Select TUR.Id,TUR.UserName,TUR.Email,TUR.Mobile, TUR.FirstName,TUR.LastName,RT.Type As RegType,TUR.Status,TUR.CreatedDate   ";
                Query += " From BizUser  TUR";
                Query += " Inner Join RegisterType RT  On TUR.RegTypeId=RT.Id where ";
                if (FromDate != null && ToDate != null)
                {
                    string FromDatestr = String.Format("{0:yyyy/MM/dd}", FromDate);
                    string ToDatestr = String.Format("{0:yyyy/MM/dd}", ToDate);
                    Query += " TUR.CreatedDate between '" + FromDatestr + " 00:00:00.0000000' And '" + ToDatestr + " 23:59:59.999' and ";
                }
                else
                {
                    //var date = DateTime.Now.ToString("yyyy-MM-dd");
                    //Query += " TUR.CreatedDate between '" + date + " 00:00:00.0000000' and '" + date + " 23:59:59.999' and ";

                }

                if (!string.IsNullOrEmpty(filtration))
                {
                    switch ((SignupReportfiltration)Enum.Parse(typeof(SignupReportfiltration), filtration.ToString()))
                    {
                        case SignupReportfiltration.Today:
                            //Query += " TUR.CreatedDate between DATEADD(DAY," + Convert.ToInt32(SignupReportfiltration.Today) + ", GETDATE()) and DATEADD(DAY, 0, GETDATE()) and ";
                            Query += " TUR.CreatedDate between cast(getdate() as date) and DATEADD(DAY, 0, GETDATE()) and "; //changes -mansi 2019-07-29
                            break;
                        case SignupReportfiltration.Weekly:
                            Query += " TUR.CreatedDate between DATEADD(DAY," + Convert.ToInt32(SignupReportfiltration.Weekly) + " , GETDATE()) and DATEADD(DAY, 0, GETDATE()) and ";
                            break;
                        case SignupReportfiltration.Monthly:
                            Query += " TUR.CreatedDate between DATEADD(DAY," + Convert.ToInt32(SignupReportfiltration.Monthly) + " , GETDATE()) and DATEADD(DAY, 0, GETDATE()) and ";
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(EmailAddress))
                {
                    Query += "TUR.Email= '" + EmailAddress + "'";
                }
                if (!string.IsNullOrEmpty(Mobile))
                {
                    if (!string.IsNullOrEmpty(EmailAddress))
                        Query += " Or TUR.Mobile= '" + Mobile + "'";
                    else
                        Query += "TUR.Mobile= '" + Mobile + "'";
                }
                if (!string.IsNullOrEmpty(Username))
                {
                    if (!string.IsNullOrEmpty(EmailAddress) || !string.IsNullOrEmpty(Mobile))
                        Query += " and TUR.UserName = '" + Username + "'";
                    else
                        Query += "TUR.UserName='" + Username + "'";
                }
                if (string.IsNullOrEmpty(Username) && string.IsNullOrEmpty(Mobile)
                    && string.IsNullOrEmpty(EmailAddress))
                {
                    Query += "RT.Id=RT.Id order by TUR.CreatedDate Desc ";
                }
                else
                {
                    Query += "order by TUR.CreatedDate Desc ";
                }
                SignupReport = _dbContext.SignReportViewmodel.FromSql(Query);

                var SignupReportData = new List<SignReportViewmodel>();
                SignupReportData = SignupReport.ToList();
                //var SignupReport1.ToList();
                var total = SignupReportData.Count();
                //var pageSize = 10; // set your page size, which is number of records per page

                //var page = 1; // set current page number, must be >= 1
                if (PageIndex == 0)
                {
                    PageIndex = 1;
                }

                if (Page_Size == 0)
                {
                    Page_Size = 10;
                }

                var skip = Page_Size * (PageIndex - 1);

                SignReportModel signReportModel = new SignReportModel();
                signReportModel.SignReportList = SignupReportData.Skip(skip).Take(Page_Size).ToList();
                signReportModel.Total = total;
                return signReportModel;

            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }

        }

        public List<SignReportCountViewmodel> GetUserReportCount()
        {
            try
            {
                //string timestamp = "'" + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) + "'";
                DateTime dateTime =  DateTime.UtcNow;
                string FromDatestr = "'" +  String.Format("{0:yyyy/MM/dd}", dateTime) + " 00:00:00.000" + "'";
                string ToDatestr = "'" +  String.Format("{0:yyyy/MM/dd}", dateTime) + " 23:59:59.999" + "'";
                IQueryable<SignReportCountViewmodel> SignupReport;

                //string Query = @"WITH SignUP_CTE AS  ( select ( SELECT COUNT(*)  FROM TempUserRegister   ";
                //Query += " WHERE CreatedDate between DATEADD(DAY, 0, " + timestamp + ") and  DATEADD(DAY, 0, " + timestamp + ")) as Today,";
                //Query += " (SELECT COUNT(*)  FROM TempUserRegister WHERE CreatedDate between DATEADD(DAY,- 7, " + timestamp + ") and  DATEADD(DAY, 0," + timestamp + ") ) as Weekly,";
                //Query += " (SELECT COUNT(*)  FROM TempUserRegister  WHERE CreatedDate between DATEADD(DAY, -30," + timestamp + ") and  DATEADD(DAY, 0, " + timestamp + ") ) as Monthly,";
                //Query += " (SELECT COUNT(*)  FROM TempUserRegister  ) as Total";
                //Query += ")  select * from SignUP_CTE";

                // Commeted by khushali -03-05-2019 Query change
                //string Query = @"WITH SignUP_CTE AS  ( select ( SELECT COUNT(*)  FROM BizUser   ";
                //Query += " WHERE CreatedDate between DATEADD(DAY, 0, " + timestamp + ") and  DATEADD(DAY, 0, " + timestamp + ")) as Today,";
                //Query += " (SELECT COUNT(*)  FROM BizUser WHERE CreatedDate between DATEADD(DAY,- 7, " + timestamp + ") and  DATEADD(DAY, 0," + timestamp + ") ) as Weekly,";
                //Query += " (SELECT COUNT(*)  FROM BizUser  WHERE CreatedDate between DATEADD(DAY, -30," + timestamp + ") and  DATEADD(DAY, 0, " + timestamp + ") ) as Monthly,";
                //Query += " (SELECT COUNT(*)  FROM BizUser  ) as Total";
                //Query += ")  select * from SignUP_CTE";

                string Query = @"WITH SignUP_CTE AS  ( select ( SELECT COUNT(*)  FROM BizUser   ";
                Query += " WHERE CreatedDate between DATEADD(DAY, 0, " + FromDatestr + ") and  DATEADD(DAY, 0, " + ToDatestr + ")) as Today,";
                Query += " (SELECT COUNT(*)  FROM BizUser WHERE CreatedDate between DATEADD(DAY,- 7, " + FromDatestr + ") and  DATEADD(DAY, 0," + ToDatestr + ") ) as Weekly,";
                Query += " (SELECT COUNT(*)  FROM BizUser  WHERE CreatedDate between DATEADD(DAY, -30," + FromDatestr + ") and  DATEADD(DAY, 0, " + ToDatestr + ") ) as Monthly,";
                Query += " (SELECT COUNT(*)  FROM BizUser  ) as Total";
                Query += ")  select * from SignUP_CTE";
                var CustomerSignupreport = _dbContext.SignReportCountViewmodels.FromSql(Query);
                return CustomerSignupreport.ToList();
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }

        }

        public GetSubscribeNewLetterResponse GetSubscribeNewLetter(long PageSize,long PageNo)
        {
            GetSubscribeNewLetterResponse _Res = new GetSubscribeNewLetterResponse();
            List<GetSubscribeNewLetter> subscribeNews = new List<GetSubscribeNewLetter>();
            try
            {
                var list = _SubscribeNewsLetterRepository.FindBy(e => e.Status == 1).ToList();
                _Res.Count = list.Count;
                if(list.Count==0)
                {
                    _Res.ErrorCode = enErrorCode.ReportDataNotFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "ReportDataNotFound";
                    return _Res;
                }
                foreach(var obj in list.Skip(Convert.ToInt16(PageSize * PageNo)).Take(Convert.ToInt16(PageSize)))
                {
                    subscribeNews.Add(new Core.ViewModels.BackOfficeReports.GetSubscribeNewLetter() {
                        CreatedDate=obj.CreatedDate,
                         Email=obj.Email,
                         ID=obj.Id,
                         Status=obj.Status
                    });
                }

                _Res.PageCount= _Res.Count / PageSize;
                _Res.Response = subscribeNews;
                _Res.ErrorCode = Core.Enums.enErrorCode.Success;
                _Res.ReturnCode = Core.Enums.enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public GetSubscribeNewLetterCountResponse GetSubscribeNewLetterCount()
        {
            GetSubscribeNewLetterCountResponse _Res = new GetSubscribeNewLetterCountResponse();
            try
            {
                var list = _SubscribeNewsLetterRepository.FindBy(e => e.Status == 1).ToList();
                _Res.Count = list.Count;
                _Res.ErrorCode = Core.Enums.enErrorCode.Success;
                _Res.ReturnCode = Core.Enums.enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        public BizResponseClass RemoveSubscribeNewsLetter(long ID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                var IsExist = _SubscribeNewsLetterRepository.FindBy(e => e.Id==ID && e.Status == 1).FirstOrDefault();
                if (IsExist == null)
                {
                    _Res.ErrorCode = Core.Enums.enErrorCode.EmailNotFoundEnterValidEmail;
                    _Res.ReturnCode = Core.Enums.enResponseCode.Fail;
                    _Res.ReturnMsg = "EmailNotFoundEnterValidEmail";
                    return _Res;
                }
                IsExist.Status = 9;
                IsExist.UpdatedBy = 999;
                IsExist.UpdatedDate = DateTime.UtcNow;
                _SubscribeNewsLetterRepository.Update(IsExist);

                _Res.ErrorCode = Core.Enums.enErrorCode.Success;
                _Res.ReturnCode = Core.Enums.enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
    }
}
