using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using System.Web;
using System.Xml;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels.ManageViewModels;
using Microsoft.EntityFrameworkCore;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.Entities.User;
using Microsoft.AspNetCore.Identity;
using Worldex.Core.ApiModels;
using Microsoft.Extensions.Logging;

namespace Worldex.Infrastructure.Services
{
    public class ManageService : IManageService
    {
        private readonly ILogger<ManageService> _logger;
        private readonly WorldexContext _dbContext;
        private readonly ICommonRepository<SubscribeNewsLetter> _SubscribeNewsLetterRepository;

        public ManageService(WorldexContext dbContext, ILogger<ManageService> logger, ICommonRepository<SubscribeNewsLetter> SubscribeNewsLetterRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _SubscribeNewsLetterRepository = SubscribeNewsLetterRepository;
        }

        public UserInfoResponse GetUserInfo(int UserId)
        {
            try
            {
                IndexViewModel obj = new IndexViewModel();
                obj = (from us in _dbContext.Users
                       join st in _dbContext.SubscriptionMaster on us.Id equals st.UserId
                       join PM in _dbContext.ProfileMaster on st.ProfileId equals PM.Id
                       join TM in _dbContext.Typemaster on PM.TypeId equals TM.Id
                       where (us.Id.Equals(UserId)) && (st.Status.Equals(1))
                       select new IndexViewModel
                       {
                           FirstName = us.FirstName,
                           LastName = us.LastName,
                           Username = us.UserName,
                           IsEmailConfirmed = us.EmailConfirmed,
                           Email = us.Email,
                           PhoneNumber = us.PhoneNumber,
                           MobileNo = us.Mobile,
                           TwoFactorEnabled = us.TwoFactorEnabled,
                           SocialProfile = TM.SubType,
                           IsDeviceEnabled = us.IsDeviceAuthEnable
                       }
                            ).FirstOrDefault();

                if (obj == null)
                {
                    obj = (from us in _dbContext.Users
                           where (us.Id.Equals(UserId))
                           select new IndexViewModel
                           {
                               FirstName = us.FirstName,
                               LastName = us.LastName,
                               Username = us.UserName,
                               IsEmailConfirmed = us.EmailConfirmed,
                               Email = us.Email,
                               PhoneNumber = us.PhoneNumber,
                               MobileNo = us.Mobile,
                               TwoFactorEnabled = us.TwoFactorEnabled,
                               IsDeviceEnabled = us.IsDeviceAuthEnable,
                               SocialProfile = "No"
                           }).FirstOrDefault();
                }
                UserInfoResponse objResponse = new UserInfoResponse();
                objResponse.UserData = obj;

                return objResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //komal 13 May 2019 Add newsletter method
        public BizResponseClass AddSubscribeNewsLetter (string Email)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                var IsExist = _SubscribeNewsLetterRepository.FindBy(e => e.NormalizedEmail == Email.ToUpper() && e.Status==1).FirstOrDefault();
                if(IsExist!=null)
                {
                    _Res.ErrorCode = Core.Enums.enErrorCode.EmailAlreadySubscribed;
                    _Res.ReturnCode = Core.Enums.enResponseCode.Fail;
                    _Res.ReturnMsg = "EmailAlreadySubscribed";
                    return _Res;
                }
                var Model = _SubscribeNewsLetterRepository.Add(new SubscribeNewsLetter() {
                    CreatedBy = 999,
                    CreatedDate = DateTime.UtcNow,
                    Email=Email,
                    NormalizedEmail=Email.ToUpper(),
                    Status=1
                });


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

        public BizResponseClass RemoveSubscribeNewsLetter(string Email)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                var IsExist = _SubscribeNewsLetterRepository.FindBy(e => e.NormalizedEmail == Email.ToUpper() && e.Status == 1).FirstOrDefault();
                if(IsExist==null)
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
