using Worldex.Core.Entities.Complaint;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces.Complaint;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.BackOfficeComplain;
using Worldex.Core.ViewModels.Complaint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Services.Complaint
{
    public class CompainTrailServices : ICompainTrail
    {
        private readonly WorldexContext _dbContext;
        private readonly ICustomRepository<Complainmaster> _ComplainmasterRepository;

        public CompainTrailServices(WorldexContext dbContext, ICustomRepository<Complainmaster> ComplainmasterRepository)
        {
            _dbContext = dbContext;
            _ComplainmasterRepository = ComplainmasterRepository;
        }

        public long AddBackOffComMaster(BackOffAddCom model, int UserId)
        {
            try
            {
                if (model != null)
                {
                    var complainTrailData = new CompainTrail();
                    {
                        complainTrailData.ComplainId = model.ComplainId;
                        complainTrailData.Complainstatus = model.ComplainstatusId;
                        complainTrailData.Description = model.Description;
                        complainTrailData.CreatedDate = DateTime.UtcNow;
                        complainTrailData.CreatedBy = UserId;
                        complainTrailData.Remark = model.Remark;
                    }
                    _dbContext.Add(complainTrailData);
                    _dbContext.SaveChanges();

                    long complainTrailId = complainTrailData.Id;
                    //var complainmaster = new Complainmaster();
                    var complainmaster = _ComplainmasterRepository.Table.Where(i => i.Id == model.ComplainId).FirstOrDefault();

                    if (model.ComplainstatusId == Convert.ToInt16(enComplainStatusType.Open))
                        complainmaster.SetDoneComplainStatus(UserId);
                    else if (model.ComplainstatusId == Convert.ToInt16(enComplainStatusType.Close))
                        complainmaster.SetCloseComplainStatus(UserId);
                    else
                        complainmaster.SetPendingComplainStatus(UserId);
                    _ComplainmasterRepository.Update(complainmaster);
                    return complainTrailId;
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public long AddCompainTrail(CompainTrailReqVirewModel compainTrail)
        {
            try
            {
                if (compainTrail == null)
                {
                    return 0;
                }
                else
                {
                    var compainTrailData = new CompainTrail();
                    {
                        compainTrailData.ComplainId = compainTrail.ComplainId;
                        if (string.IsNullOrEmpty(compainTrail.Complainstatus))
                            compainTrailData.Complainstatus = Convert.ToInt64(enComplainStatusType.Open);
                        else
                            compainTrailData.Complainstatus = 1;
                        compainTrailData.CreatedDate = DateTime.UtcNow;
                        compainTrailData.CreatedBy = compainTrail.UserID;
                        compainTrailData.Description = compainTrail.Description;
                        if (string.IsNullOrEmpty(compainTrail.Remark))
                            compainTrailData.Remark = " ";
                        else
                            compainTrailData.Remark = compainTrail.Remark;
                    }
                    _dbContext.Add(compainTrailData);
                    _dbContext.SaveChanges();
                    return compainTrailData.Id;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }
        public void GetData(int userid)
        {
            try
            {

                var Client = _dbContext.Complainmaster.Where(o => o.UserID.Equals(userid)).Select(m => new CompainTrail
                {
                    Description = m.Description,
                    ComplainId = m.Id


                }).ToList();



            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }
    }
}
