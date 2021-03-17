using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Worldex.Core.Entities;
using Worldex.Core.Entities.User;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces.BackOffice.ProfileConfiguration;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.Profile_Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BackofficeWorldex.Web.API.BackOffice
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileConfigurationController : ControllerBase
    {

        /// <summary>
        ///  Created by pankaj for configuration userprofile configuration
        ///  date : 17-01-2019
        /// </summary>
        IProfileConfigurationData _IProfileConfiguration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICustomRepository<WalletTypeMaster> _WalletTypeMaster;
        public ProfileConfigurationController(IProfileConfigurationData profileConfiguration, UserManager<ApplicationUser> UserManager, ICustomRepository<WalletTypeMaster> WalletTypeMaste)
        {
            _IProfileConfiguration = profileConfiguration;
            _WalletTypeMaster = WalletTypeMaste;
            _userManager = UserManager;
        }
        [HttpPost("AddProfileConfiguration")]
        public async Task<IActionResult> AddProfileConfiguration(ProfileConfigurationAddViewModel profileMaster)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                long id = _IProfileConfiguration.IsProfilelevelExistConfiguration(profileMaster.Profilelevel);
                //if (id > 0)
                //{
                //    return BadRequest(new ProfileConfigurationResponseViewmodel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserProfileLevelExist, ErrorCode = enErrorCode.Status9076UserProfileLevelExist });
                //}
                string TransactionLimit = string.Empty;
                int tempCount = 0;
                // TransactionLimit = "{" + "\"TransactionLimit\"" + ":[";
                if (profileMaster.TransactionLimit != null)
                {
                    int Transactionlimit = profileMaster.TransactionLimit.Count;


                    foreach (var item in profileMaster.TransactionLimit)
                    {
                        tempCount += 1;
                        
                        var data = _WalletTypeMaster.Table.FirstOrDefault(i => i.Id == item.CurrencyId);
                       
                        TransactionLimitViewModel transactionLimit = new TransactionLimitViewModel()
                        {
                            
                            CurrencyId = item.CurrencyId,
                            CurrancyName = data.WalletTypeName.ToString(),
                            Daily = item.Daily,
                            Hourly = item.Hourly,
                            Monthly = item.Monthly,
                            Qauterly = item.Qauterly,
                            Weekly = item.Weekly,
                            Yearly = item.Yearly
                        };
                        
                        TransactionLimit += JsonConvert.SerializeObject(transactionLimit);
                        if (Transactionlimit > tempCount)
                        {
                            TransactionLimit += ",";
                        }
                    }
                    if (!string.IsNullOrEmpty(TransactionLimit))
                        TransactionLimit = _IProfileConfiguration.CreateJson("TransactionLimit", TransactionLimit);
                    // TransactionLimit += "]}";

                }
                string WithdrawalLimit = string.Empty;
                if (profileMaster.WithdrawalLimit != null)
                {
                    tempCount = 0;
                    int Withdraw = profileMaster.WithdrawalLimit.Count;
                    foreach (var item in profileMaster.WithdrawalLimit)
                    {
                        
                        var data = _WalletTypeMaster.Table.FirstOrDefault(i => i.Id == item.CurrencyId);
                        tempCount += 1;
                        TransactionLimitViewModel transactionLimit = new TransactionLimitViewModel()
                        {
                            CurrencyId = item.CurrencyId,
                            CurrancyName = data.WalletTypeName,
                            Daily = item.Daily,
                            Hourly = item.Hourly,
                            Monthly = item.Monthly,
                            Qauterly = item.Qauterly,
                            Weekly = item.Weekly,
                            Yearly = item.Yearly
                        };
                        WithdrawalLimit += JsonConvert.SerializeObject(transactionLimit);
                        if (Withdraw > tempCount)
                        {
                            WithdrawalLimit += ",";
                        }
                    }
                }
                if (!string.IsNullOrEmpty(WithdrawalLimit))
                {
                    WithdrawalLimit = _IProfileConfiguration.CreateJson("WithdrawalLimit", WithdrawalLimit);
                }
                string TradeLimit = string.Empty;
                if (profileMaster.TradeLimit != null)
                {
                    tempCount = 0;
                    int TradeCount = profileMaster.TradeLimit.Count;
                    foreach (var item in profileMaster.TradeLimit)
                    {
                        
                        var data = _WalletTypeMaster.Table.FirstOrDefault(i => i.Id ==item.CurrencyId);
                        tempCount += 1;
                        TransactionLimitViewModel transactionLimit = new TransactionLimitViewModel()
                        {
                            CurrencyId = item.CurrencyId,
                            CurrancyName = data.WalletTypeName,
                            Daily = item.Daily,
                            Hourly = item.Hourly,
                            Monthly = item.Monthly,
                            Qauterly = item.Qauterly,
                            Weekly = item.Weekly,
                            Yearly = item.Yearly
                        };
                        TradeLimit += JsonConvert.SerializeObject(transactionLimit);
                        if (TradeCount > tempCount)
                        {
                            TradeLimit += ",";
                        }
                    }
                    if (!string.IsNullOrEmpty(TradeLimit))
                        TradeLimit = _IProfileConfiguration.CreateJson("TradeLimit", TradeLimit);
                }
                string DepositLimit = string.Empty;
                if (profileMaster.DepositLimit != null)
                {
                    tempCount = 0;
                    int DepositCount = profileMaster.DepositLimit.Count;
                    foreach (var item in profileMaster.DepositLimit)

                    {
                
                        var data = _WalletTypeMaster.Table.FirstOrDefault(i => i.Id ==item.CurrencyId);
                        TransactionLimitViewModel transactionLimit = new TransactionLimitViewModel()
                        {
                            CurrencyId = item.CurrencyId,
                            CurrancyName = data.WalletTypeName,
                            Daily = item.Daily,
                            Hourly = item.Hourly,
                            Monthly = item.Monthly,
                            Qauterly = item.Qauterly,
                            Weekly = item.Weekly,
                            Yearly = item.Yearly
                        };
                        DepositLimit += JsonConvert.SerializeObject(transactionLimit);
                        if (DepositCount > tempCount)
                        {
                            DepositLimit += ",";
                        }
                    }
                    if (!string.IsNullOrEmpty(DepositLimit))
                        DepositLimit = _IProfileConfiguration.CreateJson("DepositLimit", DepositLimit);
                }

                ProfileConfigurationAddReqViewModel profileConfiguration = new ProfileConfigurationAddReqViewModel();

                profileConfiguration.DepositFee = profileMaster.DepositFee;
                profileConfiguration.DepositLimit = DepositLimit;
                profileConfiguration.Description = profileMaster.Description;
                profileConfiguration.IsProfileExpiry = profileMaster.IsProfileExpiry;
                profileConfiguration.IsRecursive = profileMaster.IsRecursive;
                profileConfiguration.KYCLevel = profileMaster.KYCLevel;
                profileConfiguration.LevelName = profileMaster.LevelName;
                profileConfiguration.ProfileFree = profileMaster.ProfileFree;
                profileConfiguration.Profilelevel = profileMaster.Profilelevel;
                profileConfiguration.SubscriptionAmount = profileMaster.SubscriptionAmount;
                profileConfiguration.TradeLimit = TradeLimit;
                profileConfiguration.Tradingfee = profileMaster.Tradingfee;
                profileConfiguration.TransactionLimit = TransactionLimit;
                profileConfiguration.TypeId = profileMaster.TypeId;
                profileConfiguration.UserId = user.Id;
                profileConfiguration.Withdrawalfee = profileMaster.Withdrawalfee;
                profileConfiguration.WithdrawalLimit = WithdrawalLimit;
               long Id = _IProfileConfiguration.AddProfileConfiguration(profileConfiguration);
                if (Id > 0)
                {
                    return Ok(new ProfileConfigurationResponseViewmodel { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.AddUserProfileConfiguration });
                }
                else
                {
                    return BadRequest(new ProfileConfigurationResponseViewmodel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserProfileConfigurationNotInsert, ErrorCode = enErrorCode.Status9071UserProfileConfigurationNotInsert });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ProfileConfigurationResponseViewmodel { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [HttpPost("UpdatProfileConfiguration")]
        public async Task<IActionResult> UpdatProfileConfiguration(ProfileConfigurationUpdateViewModel profileMaster)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                string TransactionLimit = string.Empty;
                int tempCount = 0;
                // TransactionLimit = "{" + "\"TransactionLimit\"" + ":[";
                
                if (profileMaster.TransactionLimit != null)
                {
                    int Transactionlimit = profileMaster.TransactionLimit.Count;


                    foreach (var item in profileMaster.TransactionLimit)
                    {
                        tempCount += 1;
                        
                        var data = _WalletTypeMaster.Table.FirstOrDefault(i => i.Id ==item.CurrencyId);
                        TransactionLimitViewModel transactionLimit = new TransactionLimitViewModel()
                        {
                            CurrencyId = item.CurrencyId,
                           
                            CurrancyName = data.WalletTypeName,
                            Daily = item.Daily,
                            Hourly = item.Hourly,
                            Monthly = item.Monthly,
                            Qauterly = item.Qauterly,
                            Weekly = item.Weekly,
                            Yearly = item.Yearly
                        };
                       
                        TransactionLimit += JsonConvert.SerializeObject(transactionLimit);
                        if (Transactionlimit > tempCount)
                        {
                            TransactionLimit += ",";
                        }
                    }
                    if (!string.IsNullOrEmpty(TransactionLimit))
                        TransactionLimit = _IProfileConfiguration.CreateJson("TransactionLimit", TransactionLimit);
                    // TransactionLimit += "]}";

                }
                string WithdrawalLimit = string.Empty;
                if (profileMaster.WithdrawalLimit != null)
                {
                    tempCount = 0;
                    int Withdraw = profileMaster.WithdrawalLimit.Count;
                    foreach (var item in profileMaster.WithdrawalLimit)
                    {
                        tempCount += 1;
                        var data = _WalletTypeMaster.Table.FirstOrDefault(i => i.Id == item.CurrencyId);
                        TransactionLimitViewModel transactionLimit = new TransactionLimitViewModel()
                        {
                            CurrencyId = item.CurrencyId,
                            CurrancyName = data.WalletTypeName,
                            Daily = item.Daily,
                            Hourly = item.Hourly,
                            Monthly = item.Monthly,
                            Qauterly = item.Qauterly,
                            Weekly = item.Weekly,
                            Yearly = item.Yearly
                        };
                        WithdrawalLimit += JsonConvert.SerializeObject(transactionLimit);
                        if (Withdraw > tempCount)
                        {
                            WithdrawalLimit += ",";
                        }
                    }
                }
                if (!string.IsNullOrEmpty(WithdrawalLimit))
                {
                    WithdrawalLimit = _IProfileConfiguration.CreateJson("WithdrawalLimit", WithdrawalLimit);
                }
                string TradeLimit = string.Empty;
                if (profileMaster.TradeLimit != null)
                {
                    tempCount = 0;
                    int TradeCount = profileMaster.TradeLimit.Count;
                    foreach (var item in profileMaster.TradeLimit)
                    {
                        tempCount += 1;
                        
                        var data = _WalletTypeMaster.Table.FirstOrDefault(i => i.Id == item.CurrencyId);
                        TransactionLimitViewModel transactionLimit = new TransactionLimitViewModel()
                        {
                            CurrencyId = item.CurrencyId,
                            CurrancyName = data.WalletTypeName,
                            Daily = item.Daily,
                            Hourly = item.Hourly,
                            Monthly = item.Monthly,
                            Qauterly = item.Qauterly,
                            Weekly = item.Weekly,
                            Yearly = item.Yearly
                        };
                        
                        TradeLimit += JsonConvert.SerializeObject(transactionLimit);
                        if (TradeCount > tempCount)
                        {
                            TradeLimit += ",";
                        }
                    }
                    if (!string.IsNullOrEmpty(TradeLimit))
                        TradeLimit = _IProfileConfiguration.CreateJson("TradeLimit", TradeLimit);
                }
                string DepositLimit = string.Empty;

                if (profileMaster.DepositLimit != null)
                {
                    tempCount = 0;
                    int DepositCount = profileMaster.DepositLimit.Count;
                    foreach (var item in profileMaster.DepositLimit)
                    {
                        
                        var data = _WalletTypeMaster.Table.FirstOrDefault(i => i.Id == item.CurrencyId);
                        TransactionLimitViewModel transactionLimit = new TransactionLimitViewModel()
                        {
                            CurrencyId = item.CurrencyId,
                            CurrancyName = data.WalletTypeName,
                            Daily = item.Daily,
                            Hourly = item.Hourly,
                            Monthly = item.Monthly,
                            Qauterly = item.Qauterly,
                            Weekly = item.Weekly,
                            Yearly = item.Yearly
                        };
                        DepositLimit += JsonConvert.SerializeObject(transactionLimit);
                        if (DepositCount > tempCount)
                        {
                            DepositLimit += ",";
                        }
                    }
                    if (!string.IsNullOrEmpty(DepositLimit))
                        DepositLimit = _IProfileConfiguration.CreateJson("DepositLimit", DepositLimit);
                }

                ProfileConfigurationUpdateReqViewModel profileConfiguration = new ProfileConfigurationUpdateReqViewModel();
                profileConfiguration.Id = profileMaster.Id;
                profileConfiguration.DepositFee = profileMaster.DepositFee;
                if (!string.IsNullOrEmpty(DepositLimit))
                    profileConfiguration.DepositLimit = DepositLimit;
                else
                    profileConfiguration.DepositLimit = " ";
                profileConfiguration.Description = profileMaster.Description;
                profileConfiguration.IsProfileExpiry = profileMaster.IsProfileExpiry;
                profileConfiguration.IsRecursive = profileMaster.IsRecursive;
                profileConfiguration.KYCLevel = profileMaster.KYCLevel;
                profileConfiguration.LevelName = profileMaster.LevelName;
                profileConfiguration.ProfileFree = profileMaster.ProfileFree;
                profileConfiguration.Profilelevel = profileMaster.Profilelevel;
                profileConfiguration.SubscriptionAmount = profileMaster.SubscriptionAmount;
                profileConfiguration.Withdrawalfee = profileMaster.Withdrawalfee;
                if (!string.IsNullOrEmpty(TradeLimit))
                    profileConfiguration.TradeLimit = TradeLimit;
                else
                    profileConfiguration.TradeLimit = " ";
                profileConfiguration.Tradingfee = profileMaster.Tradingfee;
                if (!string.IsNullOrEmpty(TransactionLimit))
                    profileConfiguration.TransactionLimit = TransactionLimit;
                else
                    profileConfiguration.TransactionLimit = " ";
                profileConfiguration.TypeId = profileMaster.TypeId;
                profileConfiguration.UserId = user.Id;
                profileConfiguration.Withdrawalfee = profileMaster.Withdrawalfee; //changes -mansi 25-07-2019 
                if (!string.IsNullOrEmpty(WithdrawalLimit))
                    profileConfiguration.WithdrawalLimit = WithdrawalLimit;
                else
                    profileConfiguration.WithdrawalLimit = " ";
              
                long Id = _IProfileConfiguration.UpdateProfileConfiguration(profileConfiguration);
                if (Id > 0)
                {
                    return Ok(new ProfileConfigurationResponseViewmodel { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.updateUserProfileConfiguration });
                }
                else
                {
                    return BadRequest(new ProfileConfigurationResponseViewmodel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserProfileConfigurationNotupdate, ErrorCode = enErrorCode.Status9072UserProfileConfigurationNotupdate });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ProfileConfigurationResponseViewmodel { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [HttpPost("DeleteProfileConfiguration")]
        public async Task<IActionResult> DeleteProfileConfiguration(ProfileConfigurationDeleteViewModel profileConfiguration)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                ProfileConfigurationDeleteReqViewModel profileConfigurationDelete = new ProfileConfigurationDeleteReqViewModel()
                {
                    Id = profileConfiguration.Id,
                    UserId = user.Id
                };
                long Id = _IProfileConfiguration.DeleteProfileConfiguration(profileConfigurationDelete);
                if (Id > 0)
                {
                    return Ok(new ProfileConfigurationResponseViewmodel { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.DeleteUserProfileConfiguration });
                }
                else
                {
                    return BadRequest(new ProfileConfigurationResponseViewmodel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserProfileConfigurationNotDelete, ErrorCode = enErrorCode.Status9075UserProfileConfigurationNotDelete });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new ProfileConfigurationResponseViewmodel { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [HttpGet("GetProfileConfiguration")]
        public async Task<IActionResult> GetProfileConfiguration(int PageIndex = 0, int Page_Size = 0, long Typeid = 0, bool IsRecursive = true, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                var ProfileConfigurationData = _IProfileConfiguration.GetProfileConfiguration(PageIndex, Page_Size, Typeid, IsRecursive, FromDate, ToDate);
                return Ok(new ProfileConfigurationGetResponseViewmodel { ReturnCode = enResponseCode.Success, getProfileConfiguration = ProfileConfigurationData.getProfileConfiguration, TotalCount = ProfileConfigurationData.TotalCount, ReturnMsg = EnResponseMessage.GetUserProfileConfiguration });
            }
            catch (Exception ex)
            {
                return BadRequest(new ProfileConfigurationResponseViewmodel { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }

        [HttpGet("GetProfileConfigurationById")]
        public async Task<IActionResult> GetProfileConfigurationById(long Id)
        {
            try
            {
                var ProfileConfigurationData = _IProfileConfiguration.GetProfileConfigurationById(Id);
                return Ok(new ProfileConfigurationGetResponseByIdViewmodel { ReturnCode = enResponseCode.Success, getProfileConfiguration = ProfileConfigurationData.getProfileConfiguration, ReturnMsg = EnResponseMessage.GetUserProfileConfiguration });
            }
            catch (Exception ex)
            {
                return BadRequest(new ProfileConfigurationResponseViewmodel { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }
        [HttpGet("ProfileCustomerCount")]
       
        public async Task<IActionResult> ProfileCustomerCount()
        {
            try
            {
                var Data = _IProfileConfiguration.GetProfilelevelmaster();
                return Ok(new ProfilelevelCountresponseViewmodel { ReturnCode = enResponseCode.Success, ProfilelevelCount = Data, ReturnMsg = EnResponseMessage.GetCustomerProfileConfiguration });
            }
            catch (Exception ex)
            {
                return BadRequest(new ProfileConfigurationResponseViewmodel { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
                //return BadRequest(new ProfileConfigurationResponseViewmodel { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [HttpGet("Profilewiseuserlist")]
     
        public async Task<IActionResult> Profilewiseuserlist(long ProfileId, int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                var Data = _IProfileConfiguration.Profilewiseuserlist(ProfileId, PageIndex, Page_Size);
                return Ok(new ProfilewiseuserlistResponseVoewmodel { ReturnCode = enResponseCode.Success, profilewiseuserlist = Data.profilewiseuserlist, TotalCount = Data.TotalCount, ReturnMsg = EnResponseMessage.Profilewiseuserlist });
            }
            catch (Exception ex)
            {
                return BadRequest(new ProfileConfigurationResponseViewmodel { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [HttpGet("GetProfilelevelmaster")]
        public async Task<IActionResult> GetProfilelevelmaster( int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                var Data = _IProfileConfiguration.GetProfilelevelmaster( PageIndex, Page_Size);
                return Ok(new GetProfilelevelmasterResponse { ReturnCode = enResponseCode.Success, GetProfilelevelmasters = Data.GetProfilelevelmasters, TotalCount = Data.TotalCount, ReturnMsg = EnResponseMessage.Profilelevelmaster });
            }
            catch (Exception ex)
            {
                return BadRequest(new ProfileConfigurationResponseViewmodel { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [HttpGet("GetProfilelevelmasterDropDownList")]
        public async Task<IActionResult> GetProfilelevelmasterDropDownList()
        {
            try
            {
                var Data = _IProfileConfiguration.GetProfilelevelmasterDropDownList();
                return Ok(new GetProfilelevelDropDownListResponse { ReturnCode = enResponseCode.Success, GetProfilelevelmasters = Data, ReturnMsg = EnResponseMessage.Profilelevelmaster });
            }
            catch (Exception ex)
            {
                return BadRequest(new ProfileConfigurationResponseViewmodel { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

    }
}