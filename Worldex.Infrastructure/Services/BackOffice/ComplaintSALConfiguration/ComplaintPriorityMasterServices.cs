using Worldex.Core.Entities.Complaint;
using Worldex.Core.Interfaces.BackOffice.ComplaintSALConfiguration;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.BackOffice.ComplaintSALConfiguration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Services.BackOffice.ComplaintSALConfiguration
{
    /// <summary>
    /// Create by pankaj for creating configuration for complaint configuration priority wise
    /// Date  :07-01-2019
    /// </summary>
    public class ComplaintPriorityMasterServices : IComplaintPriorityMaster
    {
        private readonly WorldexContext _dbContext;
        private readonly ICustomRepository<ComplaintPriorityMaster> _CustomRepository;
        public ComplaintPriorityMasterServices(WorldexContext context,
           ICustomRepository<ComplaintPriorityMaster> CustomRepository)
        {
            _dbContext = context;
            _CustomRepository = CustomRepository;
        }
        public long Add(ComplaintPriorityMasterreqViewModel ComplaintPriorityMaster)
        {
            try
            {
                ComplaintPriorityMaster complaintPriority = new ComplaintPriorityMaster()
                {
                    Priority = ComplaintPriorityMaster.Priority,
                    PriorityTime = ComplaintPriorityMaster.PriorityTime,
                    CreatedBy = ComplaintPriorityMaster.UserId,
                    CreatedDate = DateTime.UtcNow,
                    Status =1
                };
                _CustomRepository.Insert(complaintPriority);

                return complaintPriority.Id;


            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
        public long IsComplaintPriorityExist(ComplaintPriorityMasterreqViewModel ComplaintPriorityMaster)
        {
            try
            {
                var IspriorityExist = _CustomRepository.Table.FirstOrDefault(i => i.Status == 1 && (i.Priority == ComplaintPriorityMaster.Priority ||
                  i.PriorityTime == ComplaintPriorityMaster.PriorityTime));
                if (IspriorityExist == null)
                {
                    return 0;

                }
                else
                {
                    return IspriorityExist.Id;
                }


            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
        public long Update(ComplaintPriorityMasterupdatereqViewModel ComplaintPriorityMasterupdate)
        {
            try
            {
                var IspriorityExist = _CustomRepository.Table.FirstOrDefault(i => i.Id == ComplaintPriorityMasterupdate.Id);
                if (IspriorityExist == null)
                {
                    return 0;
                }
                else
                {
                    IspriorityExist.Priority = ComplaintPriorityMasterupdate.Priority;
                    IspriorityExist.PriorityTime = ComplaintPriorityMasterupdate.PriorityTime;
                    IspriorityExist.UpdatedBy = ComplaintPriorityMasterupdate.UserId;
                    IspriorityExist.UpdatedDate = DateTime.UtcNow;
                    _CustomRepository.Update(IspriorityExist);
                    return IspriorityExist.Id;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }

        }

        public long Delete(ComplaintPriorityMasterDeleteReqViewModel ComplaintPriorityMasterDelete)
        {
            try
            {
                var IspriorityExist = _CustomRepository.Table.FirstOrDefault(i => i.Id == ComplaintPriorityMasterDelete.Id && i.Status == 1);
                if (IspriorityExist == null)
                {
                    return 0;
                }
                else
                {
                    IspriorityExist.Status = 9;
                    IspriorityExist.UpdatedBy = ComplaintPriorityMasterDelete.UserId;
                    IspriorityExist.UpdatedDate = DateTime.UtcNow;
                    _CustomRepository.Update(IspriorityExist);
                    return IspriorityExist.Id;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }


        public ComplaintPrioritygetdataResponse GetComplaintPriority(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                IQueryable<ComplaintPrioritygetdataViewModel> Result;

                string Query = "Select   Priority,PriorityTime,id,CreatedDate From ComplaintPriorityMaster where Status=1";
                Result = _dbContext.ComplaintPrioritygetdata.FromSql(Query);
                if (PageIndex == 0)
                {
                    PageIndex = 1;
                }

                if (Page_Size == 0)
                {
                    Page_Size = 10;
                }

                var skip = Page_Size * (PageIndex - 1);

                ComplaintPrioritygetdataResponse response = new ComplaintPrioritygetdataResponse();
                response.TotalCount = Result.Count();
                response.ComplaintPriorityGet = Result.Skip(skip).Take(Page_Size).ToList();
                return response;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
    }
}
