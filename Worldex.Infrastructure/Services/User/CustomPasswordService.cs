using Worldex.Core.Entities;
using Worldex.Core.Entities.User;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.Interfaces.User;
using Worldex.Core.ViewModels.AccountViewModels.Login;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.User
{
    public partial class CustomPasswordService : ICustomPassword
    {
        private readonly WorldexContext _dbContext;
        private readonly IUserService _userService;
        private readonly ICustomRepository<CustomPassword> _customRepository;
        private readonly ILogger<CustomPassword> _logger;

        public CustomPasswordService(
            WorldexContext dbContext, IUserService userService,
            ICustomRepository<CustomPassword> customRepository,
            //IMessageRepository<Customtoken> customRepository,
            ILogger<CustomPassword> logger)
        {
            _dbContext = dbContext;
            _userService = userService;
            _customRepository = customRepository;
            _logger = logger;
        }

        public async Task<CustomtokenViewModel> AddPassword(CustomtokenViewModel model)
        {
            try
            {
                var token = new CustomPassword
                {
                    UserId = model.UserId,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = model.UserId,
                    EnableStatus = false,
                    Password = model.Password
                };
                _customRepository.Insert(token);

                CustomtokenViewModel datamodel = new CustomtokenViewModel();
                if (token != null)
                {
                    model.UserId = token.UserId;
                    model.EnableStatus = token.EnableStatus;
                    model.Password = token.Password;
                    return model;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw;
            }
        }

        public async Task<CustomtokenViewModel> GetPassword(long userid)
        {
            try
            {
                var data = _customRepository.Table.Where(i => i.UserId == userid && i.EnableStatus == false).LastOrDefault();
                CustomtokenViewModel model = new CustomtokenViewModel();
                if (data != null)
                {
                    model.Password = data.Password;
                    model.EnableStatus = data.EnableStatus;
                    model.UserId = data.UserId;
                    model.Id = data.Id;
                    return model;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// added by nirav savariya for set password for login with mobile and email
        /// </summary>
        public async Task<CustomtokenViewModel> IsValidPassword(string appkey, string otp)
        {
            try
            {
                if (!string.IsNullOrEmpty(appkey) && !string.IsNullOrEmpty(otp))
                {
                    string password = string.Empty;
                    if (otp.Length > 6)
                    {
                        if (otp.Length > 12)
                        {
                            string _Pass1 = appkey.Substring(0, 20);
                            string _Pass11 = _Pass1 + otp.Substring(1, 2);
                            string _Pass2 = appkey.Substring(20, 10);
                            string _Pass22 = _Pass2 + otp.Substring(6, 3);
                            string _Pass3 = appkey.Substring(30, 28);
                            string _Pass33 = _Pass3 + otp.Substring(10, 1);
                            password = _Pass11 + _Pass22 + _Pass33;
                        }
                        else
                        {
                            string _Pass1 = appkey.Substring(0, 20);   // If the provider key length is less then  then provide key combination is skip 2 and then add 6 to password.
                            string _Pass11 = _Pass1 + otp.Substring(1, 6);
                            string _Pass2 = appkey.Substring(20, 10);
                            string _Pass3 = appkey.Substring(30, 28);
                            password = _Pass11 + _Pass2 + _Pass3;
                        }
                    }
                    else
                    {
                        string _Pass1 = appkey.Substring(0, 20);
                        string _Pass11 = _Pass1 + otp.Substring(0, 3);
                        string _Pass2 = appkey.Substring(20, 10);
                        string _Pass22 = _Pass2 + otp.Substring(3, 3);
                        string _Pass3 = appkey.Substring(30, 28);
                        password = _Pass11 + _Pass22 + _Pass3;
                    }
                    var data = _customRepository.Table.Where(i => i.Password == password && i.EnableStatus == false).FirstOrDefault();
                    if (data != null)
                    {
                        CustomtokenViewModel model = new CustomtokenViewModel();
                        model.Password = data.Password;
                        model.EnableStatus = data.EnableStatus;
                        model.UserId = data.UserId;
                        model.Id = data.Id;
                        return model;
                    }

                    else
                        return null;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return null;
            }
        }

        public void UpdateOtp(long Id)
        {
            try
            {
                var tempdata = _customRepository.GetById(Id);
                tempdata.SetAsPasswordStatus();
                tempdata.SetAsUpdateDate(tempdata.Id);
                _customRepository.Update(tempdata);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// User 2FA Custome Token Create and insert
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public async Task<string> Get2FACustomToken(long UserId)
        {
            // Status Old Custom Password UpdateStatus
            var custompassword = await GetPassword(UserId);
            if (custompassword != null)
            {
                UpdateOtp(custompassword.Id);
            }
            // End Old Custom Password UpdateStatus

            //Start New Create Custome Password
            string OtpValue = string.Empty;
            OtpValue = _userService.GenerateRandomOTPWithPassword().ToString();
            //End New Create Custome Password

            //Start New Insert Custome Password
            CustomtokenViewModel data = new CustomtokenViewModel(); // added by nirav savariya for login with mobile and email on 16-10-2018
            data.Password = OtpValue;
            data.UserId = UserId;
            data.EnableStatus = false;
            var CustomtokenViewModel = await AddPassword(data);
            //End New Insert Custome Password

            if (CustomtokenViewModel != null)
                return OtpValue;
            else
                return null;
        }

        //2019-6-18
        public bool AddActivityTypeLog(TypeLogRequest data)
        {
            try
            {
                ActivityTypeLog newObj = new ActivityTypeLog();
                newObj.ActivityDate = Helpers.UTC_To_IST();
                newObj.CreatedDate = Helpers.UTC_To_IST();
                newObj.CreatedBy = data.UserID;
                newObj.UserID = data.UserID;
                newObj.ActivityType = data.ActivityType;
                newObj.OldValue = data.OldValue;
                newObj.NewValue = data.NewValue;
                newObj.Status = 1;

                var Obj = _dbContext.ActivityTypeLog.Add(newObj);
                _dbContext.SaveChanges();
                if (Obj==null)
                {
                    return false;
                }
                return true;
            }
            catch(Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
    }
}
