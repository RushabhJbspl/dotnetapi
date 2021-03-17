using Worldex.Core.Entities.User;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.Interfaces.User;
using Worldex.Core.ViewModels.AccountViewModels.UserKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.User
{
   public class UserKeyMasterService : IUserKeyMasterService
    {
        private readonly ICustomRepository<UserKeyMaster> _customRepository;
        private readonly IUserService _userService;

        public UserKeyMasterService(ICustomRepository<UserKeyMaster> customRepository, IUserService userService)
        {
            _customRepository = customRepository;
            _userService = userService;
        }

        /// <summary>
        /// User 2FA Custome Token Create and insert
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public string Get2FACustomToken(long UserId)
        {
            // Status Old Custom Password UpdateStatus
            var custompassword = GetUserUniqueKeyList(UserId);
            foreach (var item in custompassword)
            {
                UpdateOtp(item.Id);
            }            
            // End Old Custom Password UpdateStatus

            //Start New Create Custome Password
            string UserKeyVelue = string.Empty;
            UserKeyVelue = _userService.GenerateRandomOTPWithPassword().ToString();
            //End New Create Custome Password

            //Start New Insert Custome Password
            UserKeyViewModel data = new UserKeyViewModel(); 
            data.UniqueKey = UserKeyVelue;
            data.UserId = UserId;
            data.EnableStatus = false;
            var CustomtokenViewModel = AddUniqueKey(data);
            //End New Insert Custome Password

            if (CustomtokenViewModel != null)
                return UserKeyVelue;
            else
                return null;
        }


        public void UpdateOtp(long Id)
        {
            try
            {
                var tempdata = _customRepository.GetById(Id);
                tempdata.SetAsUniqueKeyStatus();
                tempdata.SetAsUpdateDate(tempdata.Id);
                _customRepository.Update(tempdata);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw;
            }
        }


        public List<UserKeyViewModel> GetUserUniqueKeyList(long userid)
        {
            try
            {
                //var data = _customRepository.Table.Where(i => i.UserId == userid && i.EnableStatus == false).LastOrDefault();
                var data = _customRepository.Table.Where(i => i.UserId == userid && i.EnableStatus == false).ToList();
               List<UserKeyViewModel> Listmodel = new List<UserKeyViewModel>();
                foreach(var item in data)
                {
                    UserKeyViewModel model = new UserKeyViewModel();
                    model.UniqueKey = item.uniqueKey;
                    model.EnableStatus = item.EnableStatus;
                    model.UserId = item.UserId;
                    model.Id = item.Id;
                    Listmodel.Add(model);
                }
                return Listmodel;
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw;
            }
        }

        public UserKeyViewModel GetUserUniqueKey(string useruniqueKey)
        {
            try
            {
                var data = _customRepository.Table.Where(i => i.uniqueKey == useruniqueKey && i.EnableStatus == false).LastOrDefault();
                UserKeyViewModel model = new UserKeyViewModel();
                if (data != null)
                {
                    model.UniqueKey = data.uniqueKey;
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
               // _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw;
            }
        }

        public UserKeyViewModel AddUniqueKey(UserKeyViewModel model)
        {
            try
            {
                var token = new UserKeyMaster
                {
                    UserId = model.UserId,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = model.UserId,
                    EnableStatus = false,
                    uniqueKey = model.UniqueKey
                };
                _customRepository.Insert(token);

                UserKeyViewModel datamodel = new UserKeyViewModel();
                if (token != null)
                {
                    model.UserId = token.UserId;
                    model.EnableStatus = token.EnableStatus;
                    model.UniqueKey = token.uniqueKey;
                    return model;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw;
            }
        }
    }
}
