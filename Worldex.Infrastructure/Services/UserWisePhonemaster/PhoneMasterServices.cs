using Worldex.Core.Entities.PhoneMaster;
using Worldex.Core.Interfaces.PhoneMaster;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.MobileMaster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Services.UserWisePhonemaster
{
    public class PhoneMasterServices : Iphonemaster
    {
        private readonly WorldexContext _dbContext;
        private readonly ICustomExtendedRepository<PhoneMaster> _PhoneMasterRepository;
        public PhoneMasterServices(ICustomExtendedRepository<PhoneMaster> PhoneMasterRepository, WorldexContext dbContext)
        {
            _dbContext = dbContext;
            _PhoneMasterRepository = PhoneMasterRepository;
        }
        public Guid Add(PhoneMasterReqViewModel phoneMasterViewModel)
        {
            try
            {
                var MobileNumber = _PhoneMasterRepository.Table.FirstOrDefault(i => i.Mobilenumber == phoneMasterViewModel.MobileNumber);
                if (MobileNumber == null)
                {
                    PhoneMaster phoneMaster = new PhoneMaster();
                    phoneMaster.IsPrimary = phoneMasterViewModel.IsPrimary;
                    phoneMaster.Mobilenumber = phoneMasterViewModel.MobileNumber;
                    phoneMaster.CreatedDate = DateTime.UtcNow;
                    phoneMaster.CreatedBy = phoneMasterViewModel.Userid;
                    phoneMaster.UserId = phoneMasterViewModel.Userid;
                    _PhoneMasterRepository.Insert(phoneMaster);
                    return phoneMaster.Id;
                }
                else
                {
                  return  MobileNumber.Id;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Guid IsPhoneNumberExist(string MobileNumber)
        {
            try
            {
                var MobileNumberExist = _PhoneMasterRepository.Table.FirstOrDefault(i => i.Mobilenumber == MobileNumber && i.Status == true);
                if (MobileNumberExist == null)
                {
                    return Guid.Empty;
                }
                else
                {
                    return MobileNumberExist.Id;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Guid Update(PhoneMasterUpdateReqViewModel phoneMasterUpdateViewModel)
        {
            try
            {
                var MobileNumber  = _PhoneMasterRepository.Table.FirstOrDefault(i => i.Id == phoneMasterUpdateViewModel.Id && i.Status == true);
                if (MobileNumber != null)
                {
                    MobileNumber.Id = MobileNumber.Id;
                    MobileNumber.IsPrimary = phoneMasterUpdateViewModel.IsPrimary;
                    MobileNumber.Mobilenumber = phoneMasterUpdateViewModel.MobileNumber;
                    MobileNumber.UpdatedDate = DateTime.UtcNow;
                    MobileNumber.UpdatedBy = phoneMasterUpdateViewModel.Userid;
                    _PhoneMasterRepository.Update(MobileNumber);
                    return MobileNumber.Id;
                }
                else
                    return Guid.Empty;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<MobileNumebrListViewModel> GetuserWiseMolibenumberList(int UserId)
        {
            try
            {
                string Qry = "";
                IQueryable<MobileNumebrListViewModel> Result;
                Qry = @"Select * From PhoneMaster where IsDeleted=0 and userid =" + UserId;
                Result = _dbContext.mobileNumebrListViewModels.FromSql(Qry);
                return Result.ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public Guid Delete(PhoneMasterDeleteViewModel phoneMasterDeleteViewModel)
        {
            try
            {
                var MobileNumber = _PhoneMasterRepository.Table.FirstOrDefault(i => i.Id == phoneMasterDeleteViewModel.Id  && i.Status == true);
                if (MobileNumber != null)
                {
                    MobileNumber.Id = MobileNumber.Id;
                    MobileNumber.IsDeleted = true;
                    _PhoneMasterRepository.Update(MobileNumber);
                    return MobileNumber.Id;
                }
                else
                    return Guid.Empty;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
