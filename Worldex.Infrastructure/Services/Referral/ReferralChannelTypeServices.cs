using System;
using System.Collections.Generic;
using Worldex.Core.Entities.Referral;
using Worldex.Core.Interfaces.Referral;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.Referral;
using System.Linq;

namespace Worldex.Infrastructure.Services.Referral
{
    public class ReferralChannelTypeServices : IReferralChannelType
    {
        private readonly ICustomRepository<ReferralChannelType> _ReferralChannelTypeRepository;
        private readonly WorldexContext _dbContext;


        public ReferralChannelTypeServices(ICustomRepository<ReferralChannelType> ReferralChannelTypeRepository,
            WorldexContext dbContext)
        {
            _ReferralChannelTypeRepository = ReferralChannelTypeRepository;
            _dbContext = dbContext;
        }

        public long AddReferralChannelType(ReferralChannelTypeViewModel ReferralChannelTypeInsert, long UserID)
        {
            try
            {
                ReferralChannelType ObjReferralChannelType = new ReferralChannelType()
                {
                    ChannelTypeName = ReferralChannelTypeInsert.ChannelTypeName,
                    HourlyLimit = ReferralChannelTypeInsert.HourlyLimit,
                    DailyLimit = ReferralChannelTypeInsert.DailyLimit,
                    WeeklyLimit = ReferralChannelTypeInsert.WeeklyLimit,
                    MonthlyLimit = ReferralChannelTypeInsert.MonthlyLimit,
                    CreatedBy = UserID,
                    CreatedDate = DateTime.UtcNow,
                    Status = 1
                };
                _ReferralChannelTypeRepository.Insert(ObjReferralChannelType);
                return ObjReferralChannelType.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsReferralChannelTypeExist(string ChannelTypeName)
        {
            var referraldata = _dbContext.ReferralChannelType.Any(i => i.ChannelTypeName == ChannelTypeName);
            return referraldata;
        }

        public bool IsReferralChannelTypeExistById(int ChannelTypeId)
        {
            var referraldata = _dbContext.ReferralChannelType.Any(i => i.Id == ChannelTypeId);
            return referraldata;
        }
        
        public long UpdateReferralChannelType(ReferralChannelTypeUpdateViewModel model, long UserId)
        {
            var GetUpdate = _ReferralChannelTypeRepository.Table.Where(i => i.Id == model.Id).FirstOrDefault();
            if (GetUpdate != null)
            {
                GetUpdate.ChannelTypeName = model.ChannelTypeName;
                GetUpdate.HourlyLimit = model.HourlyLimit;
                GetUpdate.DailyLimit = model.DailyLimit;
                GetUpdate.WeeklyLimit = model.WeeklyLimit;
                GetUpdate.MonthlyLimit = model.MonthlyLimit;
                GetUpdate.UpdatedDate = DateTime.UtcNow;
                GetUpdate.UpdatedBy = UserId;
                _ReferralChannelTypeRepository.Update(GetUpdate);
                return GetUpdate.Id;
            }
            return 0;
        }

        public ReferralChannelTypeUpdateViewModel GetReferralChannelType(long Id)
        {
            try
            {
                var GetData = _ReferralChannelTypeRepository.Table.Where(i => i.Id == Id).FirstOrDefault();
                ReferralChannelTypeUpdateViewModel modeldata = new ReferralChannelTypeUpdateViewModel();
                if (GetData != null)
                {
                    modeldata.Id = GetData.Id;
                    modeldata.ChannelTypeName = GetData.ChannelTypeName;
                    modeldata.HourlyLimit = GetData.HourlyLimit;
                    modeldata.DailyLimit = GetData.DailyLimit;
                    modeldata.WeeklyLimit = GetData.WeeklyLimit;
                    modeldata.MonthlyLimit = GetData.MonthlyLimit;
                    return modeldata;
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
        
        public List<ReferralChannelTypeListViewModel> ListReferralChannelType()
        {
            try
            {

                var ListData = _ReferralChannelTypeRepository.Table.OrderByDescending(i => i.CreatedDate).ToList();
                if (ListData == null)
                {
                    return null;
                }

                List<ReferralChannelTypeListViewModel> myViewModel = new List<ReferralChannelTypeListViewModel>();
                string myString = Newtonsoft.Json.JsonConvert.SerializeObject(ListData);
                myViewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ReferralChannelTypeListViewModel>>(myString);
                return myViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ReferralChannelTypeDropDownViewModel> DropDownReferralChannelType()
        {
            try
            {
                var ListData = _ReferralChannelTypeRepository.Table.Where(x => x.Status == 1).ToList();
                if (ListData == null)
                {
                    return null;
                }
                List<ReferralChannelTypeDropDownViewModel> myViewModel = new List<ReferralChannelTypeDropDownViewModel>();
                string myString = Newtonsoft.Json.JsonConvert.SerializeObject(ListData);
                myViewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ReferralChannelTypeDropDownViewModel>>(myString);
                return myViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DisableReferralChannelType(ReferralChannelTypeStatusViewModel model, long UserId)
        {
            try
            {
                var Disable = _ReferralChannelTypeRepository.Table.Where(i => i.Id == model.Id).FirstOrDefault();
                if (Disable != null)
                {
                    Disable.UpdatedDate = DateTime.UtcNow;
                    Disable.UpdatedBy = UserId;
                    Disable.Status = 0;
                    _ReferralChannelTypeRepository.Update(Disable);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public bool EnableReferralChannelType(ReferralChannelTypeStatusViewModel model, long UserId)
        {
            try
            {
                var Disable = _ReferralChannelTypeRepository.Table.Where(i => i.Id == model.Id).FirstOrDefault();
                if (Disable != null)
                {
                    Disable.UpdatedDate = DateTime.UtcNow;
                    Disable.UpdatedBy = UserId;
                    Disable.Status = 1;
                    _ReferralChannelTypeRepository.Update(Disable);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        
    }
}
