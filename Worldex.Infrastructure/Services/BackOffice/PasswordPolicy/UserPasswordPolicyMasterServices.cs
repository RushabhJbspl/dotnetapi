using Worldex.Core.Entities.Backoffice.PasswordPolicy;
using Worldex.Core.Entities.Organization;
using Worldex.Core.Interfaces.PasswordPolicy;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.PasswordPolicy;
using Worldex.Core.ViewModels.BackOffice.ComplaintSALConfiguration;
using Worldex.Core.ViewModels.BackOffice.PasswordPolicy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Services.BackOffice.PasswordPolicy
{
    /// <summary>
    ///Created by pankaj for perform password related opration 09-01-2019
    /// </summary>
    public class UserPasswordPolicyMasterServices : IUserPasswordPolicyMaster
    {
        private readonly WorldexContext _dbContext;
        private readonly ICustomRepository<UserPasswordPolicyMaster> _PasswordcustomRepository;
        private readonly ICustomExtendedRepository<ActivityType_Master> _ActivityTypeMaster;
        private readonly ICustomExtendedRepository<ActivityRegister> _ActivityRegister;

        public UserPasswordPolicyMasterServices(WorldexContext context, ICustomRepository<UserPasswordPolicyMaster> customRepository,
            ICustomExtendedRepository<ActivityType_Master> ActivityTypeMaster, ICustomExtendedRepository<ActivityRegister> ActivityRegister)
        {
            _dbContext = context;
            _PasswordcustomRepository = customRepository;
            _ActivityTypeMaster = ActivityTypeMaster;
            _ActivityRegister = ActivityRegister;
        }
        public long Add(UserPasswordPolicyMasterReqViewModel UserPasswordPolicy)
        {
            try
            {
                UserPasswordPolicyMaster userPasswordPolicyMaster = new UserPasswordPolicyMaster()
                {
                    UserId = UserPasswordPolicy.UserId,
                    LinkExpiryTime = UserPasswordPolicy.LinkExpiryTime,
                    MaxfppwdDay = UserPasswordPolicy.MaxfppwdDay,
                    MaxfppwdMonth = UserPasswordPolicy.MaxfppwdMonth,
                    OTPExpiryTime = UserPasswordPolicy.OTPExpiryTime,
                    PwdExpiretime = UserPasswordPolicy.PwdExpiretime,
                    Status = 1,
                    CreatedBy = UserPasswordPolicy.UserId,
                    CreatedDate = DateTime.UtcNow

                };
                _PasswordcustomRepository.Insert(userPasswordPolicyMaster);
                return userPasswordPolicyMaster.Id;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
        public long IsPasswordPolicyExist(UserPasswordPolicyMasterReqViewModel UserPasswordPolicy)
        {
            try
            {
                var IsPasswordPolicyExist = _PasswordcustomRepository.Table.FirstOrDefault(i => i.Status == 1 && i.UserId == UserPasswordPolicy.UserId);
                if (IsPasswordPolicyExist == null)
                {
                    return 0;

                }
                else
                {
                    return IsPasswordPolicyExist.Id;
                }


            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
        public long Delete(UserPasswordPolicyMasterDeletereqViewModel UserPasswordPolicyMasterDelete)
        {
            try
            {
                var IsPasswordPolicyExist = _PasswordcustomRepository.Table.FirstOrDefault(i => i.Status == 1 && i.Id == UserPasswordPolicyMasterDelete.Id);
                if (IsPasswordPolicyExist != null)
                {
                    IsPasswordPolicyExist.Status = 9;
                    IsPasswordPolicyExist.UpdatedBy = UserPasswordPolicyMasterDelete.UserId;
                    IsPasswordPolicyExist.UpdatedDate = DateTime.UtcNow;
                    _PasswordcustomRepository.Update(IsPasswordPolicyExist);
                    return IsPasswordPolicyExist.Id;
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }

        public long Update(UserPasswordPolicyMasterupdatereqViewModel UserPasswordPolicyMasterUpdate)
        {
            try
            {
                var IsPasswordPolicyExist = _PasswordcustomRepository.Table.FirstOrDefault(i => i.Status == 1 && i.Id == UserPasswordPolicyMasterUpdate.Id);
                if (IsPasswordPolicyExist != null)
                {
                    if (UserPasswordPolicyMasterUpdate.LinkExpiryTime > 0)
                        IsPasswordPolicyExist.LinkExpiryTime = UserPasswordPolicyMasterUpdate.LinkExpiryTime;
                    if (UserPasswordPolicyMasterUpdate.MaxfppwdDay > 0)
                        IsPasswordPolicyExist.MaxfppwdDay = UserPasswordPolicyMasterUpdate.MaxfppwdDay;
                    if (UserPasswordPolicyMasterUpdate.MaxfppwdMonth > 0)
                        IsPasswordPolicyExist.MaxfppwdMonth = UserPasswordPolicyMasterUpdate.MaxfppwdMonth;
                    if (UserPasswordPolicyMasterUpdate.PwdExpiretime > 0)
                        IsPasswordPolicyExist.PwdExpiretime = UserPasswordPolicyMasterUpdate.PwdExpiretime;
                    if (UserPasswordPolicyMasterUpdate.OTPExpiryTime > 0)
                        IsPasswordPolicyExist.OTPExpiryTime = UserPasswordPolicyMasterUpdate.OTPExpiryTime;
                    IsPasswordPolicyExist.UpdatedBy = UserPasswordPolicyMasterUpdate.UserId;
                    IsPasswordPolicyExist.UpdatedDate = DateTime.UtcNow;
                    _PasswordcustomRepository.Update(IsPasswordPolicyExist);
                    return IsPasswordPolicyExist.Id;
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }


        public UserPasswordPolicyMasterresponseListViewModel GetPasswordPolicy(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                IQueryable<UserPasswordPolicyMasterGetdataViewModel> Result;

                string Query = "Select UserId,Id,PwdExpiretime,MaxfppwdDay,MaxfppwdMonth,LinkExpiryTime,OTPExpiryTime,CreatedDate From UserPasswordPolicyMaster where  status=1";
                Result = _dbContext.UserPasswordPolicy.FromSql(Query);
                if (PageIndex == 0)
                {
                    PageIndex = 1;
                }

                if (Page_Size == 0)
                {
                    Page_Size = 10;
                }

                var skip = Page_Size * (PageIndex - 1);
                UserPasswordPolicyMasterresponseListViewModel userPasswordPolicy = new UserPasswordPolicyMasterresponseListViewModel()
                {
                    UserPasswordPolicyMaster = Result.Skip(skip).Take(Page_Size).ToList(),
                    TotalCount = Result.Count()
                };
                return userPasswordPolicy;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }


        public UserPasswordPolicyMaster GetUserPasswordPolicyConfiguration(int UserId)
        {
            try
            {
                var UserPasswordPolicy = _PasswordcustomRepository.Table.FirstOrDefault(i => i.Status == 1 && i.UserId == UserId);
                if (UserPasswordPolicy == null)
                {

                    return UserPasswordPolicy = _PasswordcustomRepository.Table.FirstOrDefault(i => i.Status == 1 && i.UserId == 0);

                }
                else
                {
                    return UserPasswordPolicy;
                }


            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
        public bool CheckForgotPasswordInMonth(int Userid, int ForgotPasswordInMonth, string VerificationType)  ///This method work base on Days calculation
        {
            try
            {

                IQueryable<PasswordPolicyCheckViewModel> Result;

                string Query = string.Empty;

                var IsPasswordPolicyExist = _ActivityTypeMaster.Table.LastOrDefault(i => i.TypeMaster == VerificationType);  ///This Method are user to get ActivityTypeId base on particular method wise
                if (IsPasswordPolicyExist != null)
                {
                    var Year = DateTime.Now.ToString("yyyy");
                    var Month = DateTime.Now.ToString("MM");
                    Query = "select CreatedDate from ActivityRegister  Where activitytypeid='" + IsPasswordPolicyExist.Id;
                    Query += "' and CreatedBy=" + Userid + " And YEAR(CreatedDate)=" + Year + " and MONTH(CreatedDate) =" + Month;
                    Query += "  order by CreatedDate Desc";
                    Result = _dbContext.PasswordPolicyCheck.FromSql(Query);
                    var CheckPassPolicy = Result.ToList();
                    //var ActivityTypeMasterData = _ActivityRegister.Table.LastOrDefault(i => i.ActivityTypeId == IsPasswordPolicyExist.Id && i.CreatedBy == Userid);
                    if (Result != null && CheckPassPolicy.Count < ForgotPasswordInMonth)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
        public bool CheckForgotPasswordInday(int Userid, int ForgotPasswordInday, string VerificationType)  ///This method work base on Days calculation
        {
            try
            {

                IQueryable<PasswordPolicyCheckViewModel> Result;

                string Query = string.Empty;

                var IsPasswordPolicyExist = _ActivityTypeMaster.Table.LastOrDefault(i => i.TypeMaster == VerificationType);  ///This Method are user to get ActivityTypeId base on particular method wise
                if (IsPasswordPolicyExist != null)
                {
                    var date = DateTime.Now.ToString("yyyy-MM-dd");
                    Query = "select CreatedDate from ActivityRegister  Where activitytypeid='" + IsPasswordPolicyExist.Id;
                    Query += "' and CreatedBy=" + Userid + " And CreatedDate between '" + date + " 00:00:00.0000000' and '" + date;
                    Query += " 23:59:59.999' order by CreatedDate Desc";
                    Result = _dbContext.PasswordPolicyCheck.FromSql(Query);
                    var CheckPassPolicy = Result.ToList();
                    //var ActivityTypeMasterData = _ActivityRegister.Table.LastOrDefault(i => i.ActivityTypeId == IsPasswordPolicyExist.Id && i.CreatedBy == Userid);
                    if (Result != null && CheckPassPolicy.Count < ForgotPasswordInday)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
        public bool CheckPasswordPolicyExpiration(int Userid, int PwdExpiretime, string VerificationType)  ///This method work base on Days calculation
        {
            try
            {

                IQueryable<PasswordPolicyCheckViewModel> Result;

                string Query = string.Empty;

                var IsPasswordPolicyExist = _ActivityTypeMaster.Table.LastOrDefault(i => i.TypeMaster == VerificationType);  ///This Method are user to get ActivityTypeId base on particular method wise
                if (IsPasswordPolicyExist != null)
                {
                    Query = "select top(1) CreatedDate from ActivityRegister  Where activitytypeid='" + IsPasswordPolicyExist.Id;
                    Query += "' and CreatedBy=" + Userid + "  order by CreatedDate Desc";
                    Result = _dbContext.PasswordPolicyCheck.FromSql(Query);
                    if (Result.ToList().Count > 0)
                    {
                        var CheckPassPolicy = Result.FirstOrDefault();
                        //var ActivityTypeMasterData = _ActivityRegister.Table.LastOrDefault(i => i.ActivityTypeId == IsPasswordPolicyExist.Id && i.CreatedBy == Userid);
                        if (Result != null && CheckPassPolicy.CreatedDate.AddDays(PwdExpiretime) > DateTime.UtcNow)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return false;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
        public bool CheckOTPAndLinkExpiration(int Userid, int OTPExpirationtime, string VerificationType)  ///This method work base on minutes calculation
        {
            try
            {
                IQueryable<PasswordPolicyCheckViewModel> Result;
                string Query = string.Empty;
                var IsPasswordPolicyExist = _ActivityTypeMaster.Table.LastOrDefault(i => i.TypeMaster == VerificationType);  ///This Method are user to get ActivityTypeId base on particular method wise
                if (IsPasswordPolicyExist != null)
                {
                    Query = "select top(1) CreatedDate from ActivityRegister  Where activitytypeid='" + IsPasswordPolicyExist.Id;
                    Query += "' and CreatedBy=" + Userid + "  order by CreatedDate Desc";
                    Result = _dbContext.PasswordPolicyCheck.FromSql(Query);
                    var CheckPassPolicy = Result.FirstOrDefault();
                    // var ActivityTypeMasterData = _ActivityRegister.Table.LastOrDefault(i => i.ActivityTypeId == IsPasswordPolicyExist.Id && i.CreatedBy == Userid);
                    if (CheckPassPolicy != null && CheckPassPolicy.CreatedDate.AddMinutes(OTPExpirationtime) > DateTime.UtcNow)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }

        public bool CheckPasswordDefaultPolicyExpiration(int Userid, int PwdExpiretime, string VerificationType)
        {
            try
            {

                IQueryable<PasswordPolicyCheckViewModel> Result;

                string Query = string.Empty;

                var IsPasswordPolicyExist = _ActivityTypeMaster.Table.LastOrDefault(i => i.TypeMaster == VerificationType);  ///This Method are user to get ActivityTypeId base on particular method wise
                if (IsPasswordPolicyExist != null)
                {
                    Query = "select top(1) CreatedDate from ActivityRegister  Where activitytypeid='" + IsPasswordPolicyExist.Id;
                    Query += "' and CreatedBy=" + Userid + "  order by CreatedDate Desc";
                    Result = _dbContext.PasswordPolicyCheck.FromSql(Query);
                    if (Result.ToList().Count > 0)
                    {
                        var CheckPassPolicy = Result.FirstOrDefault();
                        //var ActivityTypeMasterData = _ActivityRegister.Table.LastOrDefault(i => i.ActivityTypeId == IsPasswordPolicyExist.Id && i.CreatedBy == Userid);
                        if (Result != null && CheckPassPolicy.CreatedDate.AddDays(PwdExpiretime) > DateTime.UtcNow)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return false;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
    }
}
