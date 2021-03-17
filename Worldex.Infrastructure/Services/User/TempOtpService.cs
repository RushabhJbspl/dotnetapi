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

namespace Worldex.Infrastructure.Services.User
{
    public class TempOtpService : ITempOtpService
    {
        private readonly WorldexContext _dbContext;
        private readonly IUserService _userService;
        private readonly ICustomRepository<TempOtpMaster> _tempRepository;
        public TempOtpService(WorldexContext dbContext, IUserService userService, ICustomRepository<TempOtpMaster> tempRepository)
        {
            _dbContext = dbContext;
            _userService = userService;
            _tempRepository = tempRepository;
        }

        public async Task<TempOtpViewModel> AddTempOtp(int UserId, int RegTypeId)
        {
            var currentTempotp = new TempOtpMaster
            {
                UserId = UserId,
                RegTypeId = RegTypeId,
                OTP = _userService.GenerateRandomOTP().ToString(),
                CreatedTime = DateTime.UtcNow,
                ExpirTime = DateTime.UtcNow.AddHours(2),
                Status = 0,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = UserId,
                EnableStatus = false
            };
            _dbContext.Add(currentTempotp);
            _dbContext.SaveChanges();

            TempOtpViewModel model = new TempOtpViewModel();            
            model.OTP = currentTempotp.OTP;
            model.Id = currentTempotp.Id;
            model.UserId = currentTempotp.UserId;
            model.EnableStatus = currentTempotp.EnableStatus;
            model.ExpirTime = currentTempotp.ExpirTime;
            model.CreatedTime = currentTempotp.CreatedTime;
            model.RegTypeId = currentTempotp.RegTypeId;
            return model;
        }

        public async Task<TempOtpViewModel> GetTempData(int Id)
        {
            var tempotp = _dbContext.TempOtpMaster.Where(i => i.UserId == Id).LastOrDefault();
            TempOtpViewModel model = new TempOtpViewModel();
            if (tempotp != null)
            {
                model.UserId = tempotp.UserId;
                model.RegTypeId = tempotp.RegTypeId;
                model.OTP = tempotp.OTP;
                model.CreatedTime = tempotp.CreatedTime;
                model.ExpirTime = tempotp.ExpirTime;
                model.EnableStatus = tempotp.EnableStatus;
                model.Id = tempotp.Id;
                return model;
            }
            else
                return null;
        }

        public void Update(long Id)
        {
            var tempdata = _tempRepository.GetById(Id);
            tempdata.SetAsOTPStatus();
            tempdata.SetAsUpdateDate(tempdata.UserId);
            _tempRepository.Update(tempdata);
        }
    }
}
