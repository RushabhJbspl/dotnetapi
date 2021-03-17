using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worldex.Core.Entities.User;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.Interfaces.User;
using Worldex.Core.ViewModels.AccountViewModels.SignUp;
using Worldex.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Worldex.Infrastructure.Services.User
{
    public class TempUserRegisterService : ITempUserRegisterService
    {
        private readonly WorldexContext _dbContext;
        private readonly ILogger<TempUserRegisterService> _log;
        private readonly IUserService _userService;
        private readonly ITempOtpService _tempOtpService;
        private readonly ICustomRepository<TempUserRegister> _tempRepository;

        public TempUserRegisterService(WorldexContext dbContext, ILogger<TempUserRegisterService> log, IUserService userService, ITempOtpService tempOtpService, ICustomRepository<TempUserRegister> tempRepository)
        {
            _dbContext = dbContext;
            _log = log;
            _userService = userService;
            _tempOtpService = tempOtpService;
            _tempRepository = tempRepository;
        }

        public bool GetMobileNumberCheck(string MobileNumber)
        {
            var userdata = _dbContext.TempUserRegister.Where(i => i.Mobile == MobileNumber).FirstOrDefault();
            if (userdata?.Mobile == MobileNumber)
                return false;
            else
                return true;
        }

        public bool GetMobileNumber(string MobileNumber)
        {
            var userdata = _dbContext.TempUserRegister.Where(i => i.Mobile == MobileNumber && i.RegisterStatus == false).FirstOrDefault();
            if (userdata?.Mobile == MobileNumber)
                return false;
            else
                return true;
        }

        public async Task<TempUserRegisterViewModel> AddTempRegister(TempUserRegisterViewModel model)
        {
            var currentTempReguser = new TempUserRegister
            {
                RegTypeId = model.RegTypeId,
                Mobile = model.Mobile,
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PasswordHash = model.PasswordHash,
                Email = model.Email,
                CountryCode=model.CountryCode,
                CreatedDate = DateTime.UtcNow,
            };
            _dbContext.Add(currentTempReguser);
            _dbContext.SaveChanges();
            if (!(currentTempReguser.RegTypeId == Convert.ToInt16(Core.Enums.enRegisterType.Standerd)))
            {
                var obj = await _tempOtpService.AddTempOtp((int)currentTempReguser.Id, currentTempReguser.RegTypeId);
                TempUserRegisterViewModel temp = new TempUserRegisterViewModel();
                temp.Id = currentTempReguser.Id;
                temp.RegTypeId = currentTempReguser.RegTypeId;
                temp.UserName = currentTempReguser.UserName;
                temp.Email = currentTempReguser.Email;
                temp.RegisterStatus = currentTempReguser.RegisterStatus;
                temp.CountryCode = currentTempReguser.CountryCode;
                return temp;
            }
            else if (currentTempReguser != null)
            {
                TempUserRegisterViewModel temp = new TempUserRegisterViewModel();
                temp.Id = currentTempReguser.Id;
                temp.RegTypeId = currentTempReguser.RegTypeId;
                temp.UserName = currentTempReguser.UserName;
                temp.Email = currentTempReguser.Email;
                temp.RegisterStatus = currentTempReguser.RegisterStatus;
                temp.CountryCode = currentTempReguser.CountryCode;
                return temp;
            }
            else
            {
                return null;
            }
        }

        public async Task<TempUserRegisterViewModel> FindById(long Id)
        {
            var userdata = _dbContext.TempUserRegister.Find(Id);
            TempUserRegisterViewModel model = new TempUserRegisterViewModel();
            if (userdata != null)
            {
                model.UserName = userdata.UserName;
                model.FirstName = userdata.FirstName;
                model.LastName = userdata.LastName;
                model.Mobile = userdata.Mobile;
                model.PasswordHash = userdata.PasswordHash;
                model.RegisterStatus = userdata.RegisterStatus;
                model.Email = userdata.Email;
                model.Id = userdata.Id;
                model.CountryCode = userdata.CountryCode;
                return model;
            }
            else
                return null;
        }

        public void Update(long Id)
        {
            try
            {
                var tempdata = _tempRepository.GetById(Id);
                tempdata.SetAsStatus();
                tempdata.SetAsUpdateDate(tempdata.Id);
                _tempRepository.Update(tempdata);
            }
            catch (Exception ex)
            {
                ex.ToString();
                //throw;
            }
        }

        public bool GetEmailCheckExist(string Email)
        {
            var userdata = _dbContext.TempUserRegister.Where(i => i.Email == Email).FirstOrDefault();
            if (userdata?.Email == Email)
                return false;
            else
                return true;
        }

        public bool GetEmail(string Email)
        {
            var userdata = _dbContext.TempUserRegister.Where(i => i.Email == Email && i.RegisterStatus == false).FirstOrDefault();
            if (userdata?.Email == Email)
                return false;
            else
                return true;
        }


        public bool GetUserNameCheckExist(string UserName)
        {
            var userdata = _dbContext.TempUserRegister.Where(i => i.UserName == UserName).FirstOrDefault();
            if (userdata?.UserName == UserName)
                return false;
            else
                return true;
        }

        public bool GetUserName(string UserName)
        {
            var userdata = _dbContext.TempUserRegister.Where(i => i.UserName == UserName && i.RegisterStatus == false).FirstOrDefault();
            if (userdata?.UserName == UserName)
                return false;
            else
                return true;
        }


        public async Task<TempUserRegisterViewModel> GetMobileNo(string MobileNo)
        {
            var userdata = _dbContext.TempUserRegister.Where(i => i.Mobile == MobileNo).FirstOrDefault();
            TempUserRegisterViewModel model = new TempUserRegisterViewModel();
            if (userdata != null)
            {
                model.UserName = userdata.UserName;
                model.FirstName = userdata.FirstName;
                model.LastName = userdata.LastName;
                model.Mobile = userdata.Mobile;
                model.PasswordHash = userdata.PasswordHash;
                model.RegisterStatus = userdata.RegisterStatus;
                model.Email = userdata.Email;
                model.Id = userdata.Id;
                model.CountryCode = userdata.CountryCode;
                return model;
            }
            else
            {
                return null;
            }
        }

        public async Task<TempUserRegisterViewModel> GetEmailDet(string Email)
        {
            var userdata = _dbContext.TempUserRegister.Where(i => i.Email == Email).FirstOrDefault();
            TempUserRegisterViewModel model = new TempUserRegisterViewModel();
            if (userdata != null)
            {
                model.UserName = userdata.UserName;
                model.FirstName = userdata.FirstName;
                model.LastName = userdata.LastName;
                model.Mobile = userdata.Mobile;
                model.PasswordHash = userdata.PasswordHash;
                model.RegisterStatus = userdata.RegisterStatus;
                model.Email = userdata.Email;
                model.Id = userdata.Id;
                model.CountryCode = userdata.CountryCode;
                return model;
            }
            else
            {
                return null;
            }
        }
    }
}
