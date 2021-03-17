using Worldex.Core.Entities.Log;
using Worldex.Core.Entities.User;
using Worldex.Core.Interfaces.Log;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.AccountViewModels.Log;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.Log
{
    public class SignUpLogService : ISignupLogService
    {
        private readonly ICustomRepository<SignUpLog> _CustomLoginRepository;
        private readonly ICustomRepository<RegisterType> _registertypeRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public SignUpLogService(ICustomRepository<SignUpLog> customRepository, ICustomRepository<RegisterType> registertypeRepository, UserManager<ApplicationUser> userManager)
        {
            _CustomLoginRepository = customRepository;
            _registertypeRepository = registertypeRepository;
            _userManager = userManager;
        }


        public async Task<SignUpLogResponse> GetSignUpLogHistoryByUserId(long UserId, int pageIndex, int pageSize)
        {
            try
            {
                string username = string.Empty;
                var SignUpHistoryList = _CustomLoginRepository.Table.Where(i => i.UserId == UserId && i.RegisterStatus == true).ToList();
                if (SignUpHistoryList == null)
                {
                    return null;
                }

                var SignUpHistory = new List<SignUpLogDataViewModel>();
                foreach (var item in SignUpHistoryList)
                {
                    var user = await _userManager.FindByIdAsync(item.UserId.ToString());
                    if (user != null)
                        username = user.UserName;
                    else
                        username = string.Empty;
                    SignUpLogDataViewModel signuploghistoryViewModel = new SignUpLogDataViewModel()
                    {
                        UserName = username,
                        RegisterType = _registertypeRepository.Table.Where(i => i.Id == item.RegisterType).FirstOrDefault().Type,
                        Device = item.DeviceID,
                        Mode = item.Mode,
                        IpAddress = item.IPAddress,
                        Location = item.Location,
                        HostName = item.HostName,
                        Date = item.CreatedDate
                    };
                    SignUpHistory.Add(signuploghistoryViewModel);
                }

                var total = SignUpHistory.Count();
                if (pageIndex == 0)
                {
                    pageIndex = 1;
                }
                if (pageSize == 0)
                {
                    pageSize = 10;
                }
                var skip = pageSize * (pageIndex - 1);
                SignUpLogResponse signUpLog = new SignUpLogResponse()
                {
                    SignUpLogHistoryList = SignUpHistory.Skip(skip).Take(pageSize).ToList(),
                    TotalCount = total
                };
                return signUpLog;


            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public long AddSignUpLog(SignUpLogViewModel model)
        {
            try
            {
                var SignUpLogHistory = new SignUpLog()
                {
                    TempUserId = model.TempUserId,
                    RegisterType = model.RegisterType,
                    DeviceID = model.Device,
                    Mode = model.Mode,
                    IPAddress = model.IpAddress,
                    Location = model.Location,
                    HostName = model.HostName,
                    Status = 0,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = model.TempUserId,
                    RegisterStatus = false
                };
                _CustomLoginRepository.Insert(SignUpLogHistory);
                return SignUpLogHistory.Id;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }

        }

        public void UpdateVerifiedUser(int TempUserId,int UserId)
        {
            try
            {
                var SignUpLog = _CustomLoginRepository.Table.Where(i => i.TempUserId == TempUserId).FirstOrDefault();
                SignUpLog.SetAsUpdateDate(UserId);
                _CustomLoginRepository.Update(SignUpLog);
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }
    }
}
