using System;
using System.Collections.Generic;
using System.Linq;
using Worldex.Core.Entities.Referral;
using Worldex.Core.Interfaces.Referral;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.Referral;

namespace Worldex.Infrastructure.Services.Referral
{
    public class ReferralPayTypeServices : IReferralPayType
    {
        private readonly WorldexContext _dbContext;
        private readonly ICustomRepository<ReferralPayType> _ReferralPayTypeRepository;

        public ReferralPayTypeServices(ICustomRepository<ReferralPayType> ReferralPayTypeRepository, WorldexContext dbContext)
        {
            _dbContext = dbContext;           
            _ReferralPayTypeRepository = ReferralPayTypeRepository;
        }

        public long AddReferralPayType(ReferralPayTypeViewModel ReferralPayTypeInsert,long UserID)
        {
            try
            {
                ReferralPayType ObjReferralPayType = new ReferralPayType()
                {
                    PayTypeName = ReferralPayTypeInsert.PayTypeName,                  
                    CreatedBy = UserID,
                    CreatedDate = DateTime.UtcNow,
                    Status = 1
                };
                _ReferralPayTypeRepository.Insert(ObjReferralPayType);
                return ObjReferralPayType.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    
        public bool IsReferralPayTypeExist(string PayTypeName)
        {
            var referraldata = _dbContext.ReferralPayType.Any(i => i.PayTypeName == PayTypeName);
            return referraldata;
        }

        public long UpdateReferralPayType(ReferralPayTypeUpdateViewModel model,long UserId)
        {
            var GetUpdate =_ReferralPayTypeRepository.Table.Where(i => i.Id == model.Id).FirstOrDefault();
            if (GetUpdate != null)
            {
                GetUpdate.PayTypeName = model.PayTypeName;
                GetUpdate.UpdatedDate = DateTime.UtcNow;
                GetUpdate.UpdatedBy = UserId;
                _ReferralPayTypeRepository.Update(GetUpdate);
                return GetUpdate.Id;
            }
            return 0;
        }
        
        public ReferralPayTypeUpdateViewModel GetReferralPayType(long Id)
        {
            try
            {
                var GetData = _ReferralPayTypeRepository.Table.Where(i => i.Id == Id).FirstOrDefault();
                ReferralPayTypeUpdateViewModel modeldata = new ReferralPayTypeUpdateViewModel();
                if (GetData != null)
                {
                    modeldata.Id = GetData.Id;
                    modeldata.PayTypeName = GetData.PayTypeName;                  
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

        public List<ReferralPayTypeListViewModel> ListReferralPayType()
        {
            try
            {
                var ListData = _ReferralPayTypeRepository.Table.OrderByDescending(i => i.CreatedDate).ToList();
                if (ListData == null)
                {
                    return null;
                }             
                List<ReferralPayTypeListViewModel> myViewModel = new List<ReferralPayTypeListViewModel>();
                string myString = Newtonsoft.Json.JsonConvert.SerializeObject(ListData);
                myViewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ReferralPayTypeListViewModel>>(myString);               
                return myViewModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<ReferralPayTypeDropDownViewModel> DropDownReferralPayType()
        {
            try
            {
                var ListData = _ReferralPayTypeRepository.Table.Where(x => x.Status == 1).ToList();
                if (ListData == null)
                {
                    return null;
                }
                List<ReferralPayTypeDropDownViewModel> myViewModel = new List<ReferralPayTypeDropDownViewModel>();
                string myString = Newtonsoft.Json.JsonConvert.SerializeObject(ListData);
                myViewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ReferralPayTypeDropDownViewModel>>(myString);
                return myViewModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool DisableReferralPayType(ReferralPayTypeStatusViewModel model, long UserId)
        {
            try
            {
                var Disable = _ReferralPayTypeRepository.Table.Where(i => i.Id == model.Id).FirstOrDefault();
                if (Disable != null)
                {                    
                    Disable.UpdatedDate = DateTime.UtcNow;
                    Disable.UpdatedBy = UserId;
                    Disable.Status = 0;
                    _ReferralPayTypeRepository.Update(Disable);
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

        public bool EnableReferralPayType(ReferralPayTypeStatusViewModel model, long UserId)
        {
            try
            {
                var Disable = _ReferralPayTypeRepository.Table.Where(i => i.Id == model.Id).FirstOrDefault();
                if (Disable != null)
                {
                    Disable.UpdatedDate = DateTime.UtcNow;
                    Disable.UpdatedBy = UserId;
                    Disable.Status = 1;
                    _ReferralPayTypeRepository.Update(Disable);
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
