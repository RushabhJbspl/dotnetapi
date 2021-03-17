using Worldex.Core.Entities.KYC;
using Worldex.Core.Interfaces.KYC;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.KYC;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Services.KYC
{
    public class KYCLevelMasterService : IKYCLevelMaster
    {
        private readonly ICustomRepository<KYCLevelMaster> _kyclevelRepository;
        private readonly WorldexContext _dbContext;
        public KYCLevelMasterService(ICustomRepository<KYCLevelMaster> kyclevelRepository, WorldexContext dbContext)
        {
            _kyclevelRepository = kyclevelRepository;
            _dbContext = dbContext;
        }

        public long ADDKYCLevel(KYCLevelInsertReqViewModel kYCLevelInsertReqViewModel)
        {
            try
            {
                KYCLevelMaster kYCLevelMaster = new KYCLevelMaster()
                {
                    KYCName = kYCLevelInsertReqViewModel.KYCName,
                    Level = kYCLevelInsertReqViewModel.Level,
                    CreatedBy = kYCLevelInsertReqViewModel.Userid,
                    CreatedDate = DateTime.UtcNow,
                    Status = 1

                };
                _kyclevelRepository.Insert(kYCLevelMaster);
                return kYCLevelMaster.Id;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public long UpdateKYCLevel(KYCLevelUpdateReqViewModel kYCLevelUpdateReqViewModel)
        {
            try
            {
                var IsKYCKYCLevelExist = _kyclevelRepository.Table.FirstOrDefault(i => i.Id == kYCLevelUpdateReqViewModel.Id);

                if (IsKYCKYCLevelExist != null)
                {
                    IsKYCKYCLevelExist.KYCName = kYCLevelUpdateReqViewModel.KYCName;
                    IsKYCKYCLevelExist.Level = kYCLevelUpdateReqViewModel.Level;
                    IsKYCKYCLevelExist.UpdatedBy = kYCLevelUpdateReqViewModel.UserId;
                    IsKYCKYCLevelExist.UpdatedDate = DateTime.UtcNow;
                    IsKYCKYCLevelExist.Status = kYCLevelUpdateReqViewModel.status;
                    _kyclevelRepository.Update(IsKYCKYCLevelExist);
                    return IsKYCKYCLevelExist.Id;
                }
                else
                {
                    return 0;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public long IsKYCKYCLevelExist(string Kyclevelname)
        {
            try
            {
                var IsKYCKYCLevelExist = _kyclevelRepository.Table.FirstOrDefault(i => i.KYCName == Kyclevelname);
                if (IsKYCKYCLevelExist == null)
                {
                    return 0;
                }
                else
                {
                    return IsKYCKYCLevelExist.Id;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public int KYCUserWiseLevelCount(int Level)
        {
            try
            {
                IQueryable<KYCLevelWiseCount> Result;
                string Qry = "";
                int Count = 0;
                Qry = @"Select Count(KLM.Id) as Count From BizUser BU ";
                Qry += " Inner join  KYCIdentityConfigurationMapping KICM On BU.Id=KICM.Userid ";
                Qry += " Inner join kYCIdentityMaster KIM On KICM.KYCConfigurationMasterId=KIM.Id ";
                Qry += "Inner join kyclevelmaster KLM	 on KLM.Id = KICM.LevelId";
                Qry += " where  KLM.level= case when " + Level + ">0 then " + Level + " else KLM.Level end";
                Result = _dbContext.KYCLevelWiseCount.FromSql(Qry);
                var LevelCount = Result.ToList();
                foreach (var item in LevelCount)
                {
                    Count = item.Count;
                }
                return Count;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<KYCLevelViewModel> GetKYCLevelData()
        {
            try
            {
                var KYCLevelDataList = _kyclevelRepository.Table.OrderBy(i => i.Level).ToList();
                List<KYCLevelViewModel> listmodel = new List<KYCLevelViewModel>();
                if (KYCLevelDataList != null)
                {
                    foreach (var item in KYCLevelDataList)
                    {
                        KYCLevelViewModel model = new KYCLevelViewModel();
                        model.KYCName = item.KYCName;
                        model.Level = item.Level;
                        model.EnableStatus = item.EnableStatus;
                        model.IsDelete = item.IsDelete;
                        listmodel.Add(model);
                    }
                }
                return listmodel;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }
        public KYCLevelListResponse GetKYCLevelList(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                IQueryable<KYCLevelList> Result;
                string Qry = "";
                Qry = @"Select Id,createddate,kycname,Level From kYCLevelMaster";
                Result = _dbContext.KYCLevelList.FromSql(Qry);


                if (PageIndex == 0)
                {
                    PageIndex = 1;
                }

                if (Page_Size == 0)
                {
                    Page_Size = 10;
                }

                var skip = Page_Size * (PageIndex - 1);

                KYCLevelListResponse response = new KYCLevelListResponse();
                response.TotalCount = Result.Count();
                response.KYCLevelList = Result.Skip(skip).Take(Page_Size).ToList();
                return response;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }


        public List<KYCLevelList> GetKYCLevelList()
        {
            try
            {
                IQueryable<KYCLevelList> Result;
                string Qry = "";
                Qry = @"Select Id,createddate,kycname,Level From kYCLevelMaster";
                Result = _dbContext.KYCLevelList.FromSql(Qry);
                
                return Result.ToList();
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }

    }
}
