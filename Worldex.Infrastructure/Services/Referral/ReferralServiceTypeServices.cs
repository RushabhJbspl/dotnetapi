using System;
using System.Collections.Generic;
using System.Linq;
using Worldex.Core.Entities.Referral;
using Worldex.Core.Interfaces.Referral;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.Referral;

namespace Worldex.Infrastructure.Services.Referral
{
    public class ReferralServiceTypeServices : IReferralServiceType
    {
        private readonly WorldexContext _dbContext;
        private readonly ICustomRepository<ReferralServiceType> _ReferralServiceTypeRepository;

        public ReferralServiceTypeServices(ICustomRepository<ReferralServiceType> ReferralServiceTypeRepository, WorldexContext dbContext)
        {
            _dbContext = dbContext;
            _ReferralServiceTypeRepository = ReferralServiceTypeRepository;
        }
        
        public long AddReferralServiceType(ReferralServiceTypeViewModel ReferralServiceTypeInsert, long UserID)
        {
            try
            {
                ReferralServiceType ObjReferralServiceType = new ReferralServiceType()
                {
                    ServiceTypeName = ReferralServiceTypeInsert.ServiceTypeName,
                    CreatedBy = UserID,
                    CreatedDate = DateTime.UtcNow,
                    Status = 1
                };
                _ReferralServiceTypeRepository.Insert(ObjReferralServiceType);
                return ObjReferralServiceType.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsReferralServiceTypeExist(string ServiceTypeName)
        {
            var referraldata = _dbContext.ReferralServiceType.Any(i => i.ServiceTypeName == ServiceTypeName);
            return referraldata;
        }

        public long UpdateReferralServiceType(ReferralServiceTypeUpdateViewModel model, long UserId)
        {
            var GetUpdate = _ReferralServiceTypeRepository.Table.Where(i => i.Id == model.Id).FirstOrDefault();
            if (GetUpdate != null)
            {
                GetUpdate.ServiceTypeName = model.ServiceTypeName;
                GetUpdate.UpdatedDate = DateTime.UtcNow;
                GetUpdate.UpdatedBy = UserId;
                _ReferralServiceTypeRepository.Update(GetUpdate);
                return GetUpdate.Id;
            }
            return 0;
        }

        public ReferralServiceTypeUpdateViewModel GetReferralServiceType(long Id)
        {
            try
            {
                var GetData = _ReferralServiceTypeRepository.Table.Where(i => i.Id == Id).FirstOrDefault();
                ReferralServiceTypeUpdateViewModel modeldata = new ReferralServiceTypeUpdateViewModel();
                if (GetData != null)
                {
                    modeldata.Id = GetData.Id;
                    modeldata.ServiceTypeName = GetData.ServiceTypeName;
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

        public List<ReferralServiceTypeListViewModel> ListReferralServiceType()
        {
            try
            {
                var ListData = _ReferralServiceTypeRepository.Table.OrderByDescending(i => i.CreatedDate).ToList();
                if (ListData == null)
                {
                    return null;
                }
                List<ReferralServiceTypeListViewModel> myViewModel = new List<ReferralServiceTypeListViewModel>();
                string myString = Newtonsoft.Json.JsonConvert.SerializeObject(ListData);
                myViewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ReferralServiceTypeListViewModel>>(myString);
                return myViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ReferralServiceTypeDropDownViewModel> DropDownReferralServiceType()
        {
            try
            {
                var ListData = _ReferralServiceTypeRepository.Table.Where(x => x.Status == 1).ToList();
                if (ListData == null)
                {
                    return null;
                }
                List<ReferralServiceTypeDropDownViewModel> myViewModel = new List<ReferralServiceTypeDropDownViewModel>();
                string myString = Newtonsoft.Json.JsonConvert.SerializeObject(ListData);
                myViewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ReferralServiceTypeDropDownViewModel>>(myString);
                return myViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DisableReferralServiceType(ReferralServiceTypeStatusViewModel model, long UserId)
        {
            try
            {
                var Disable = _ReferralServiceTypeRepository.Table.Where(i => i.Id == model.Id).FirstOrDefault();
                if (Disable != null)
                {
                    Disable.UpdatedDate = DateTime.UtcNow;
                    Disable.UpdatedBy = UserId;
                    Disable.Status = 0;
                    _ReferralServiceTypeRepository.Update(Disable);
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

        public bool EnableReferralServiceType(ReferralServiceTypeStatusViewModel model, long UserId)
        {
            try
            {
                var Disable = _ReferralServiceTypeRepository.Table.Where(i => i.Id == model.Id).FirstOrDefault();
                if (Disable != null)
                {
                    Disable.UpdatedDate = DateTime.UtcNow;
                    Disable.UpdatedBy = UserId;
                    Disable.Status = 1;
                    _ReferralServiceTypeRepository.Update(Disable);
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
