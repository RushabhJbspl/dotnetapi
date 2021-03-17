using Worldex.Core.Entities.SocialProfile;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.Interfaces.SocialProfile;
using Worldex.Core.ViewModels.SocialProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Services.SocialProfile
{
    public class GroupMasterService : IGroupMasterService
    {
        private readonly ICustomRepository<GroupMaster> _groupMasterRepository;

        public GroupMasterService(ICustomRepository<GroupMaster> groupMasterRepository)
        {
            this._groupMasterRepository = groupMasterRepository;
        }


        public bool AddGroup(GroupMasterViewModel model, int UserId = 0)
        {
            try
            {
                var getGroup = _groupMasterRepository.Table.FirstOrDefault(g => g.GroupName == model.GroupName && g.UserId == UserId && g.Status == Convert.ToInt16(ServiceStatus.Active));
                if (getGroup == null)
                {
                    //getIp.IsEnable = true;
                    //_ipMasterRepository.Update(getIp);

                    //    return getGroup.Id;
                    //}

                    var group = new GroupMaster
                    {
                        UserId = UserId,
                        GroupName = model.GroupName,
                        Status = Convert.ToInt16(ServiceStatus.Active),
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = UserId,
                    };
                    _groupMasterRepository.Insert(group);
                    //_dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }

        public GroupMasterModel GetGroupbyUserId(long UserId = 0)
        {
            try
            {
                var getGroup = _groupMasterRepository.Table.FirstOrDefault(g => g.UserId == UserId && g.Status == Convert.ToInt16(ServiceStatus.Active));
                if (getGroup != null)
                {
                    //getIp.IsEnable = true;
                    //_ipMasterRepository.Update(getIp);

                    //    return getGroup.Id;
                    //}

                    var group = new GroupMasterModel
                    {
                        Id = getGroup.Id,
                        //UserId = getGroup.UserId,
                        GroupName = getGroup.GroupName,
                        //Status = getGroup.Status,
                        //CreatedDate = getGroup.CreatedDate,
                        //CreatedBy = getGroup.UserId,
                    };

                    //_dbContext.SaveChanges();

                    return group;
                }
                return null;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }

        public GroupMasterModel GetGroupbyName(string GroupName = null)
        {
            try
            {
                var getGroup = _groupMasterRepository.Table.FirstOrDefault(g => g.GroupName.ToLower() == GroupName.Trim().ToLower() && g.Status == Convert.ToInt16(ServiceStatus.Active));
                if (getGroup != null)
                {
                    var group = new GroupMasterModel
                    {
                        Id = getGroup.Id,
                        //UserId = getGroup.UserId,
                        GroupName = getGroup.GroupName,
                        //Status = getGroup.Status,
                        //CreatedDate = getGroup.CreatedDate,
                        //CreatedBy = getGroup.UserId,
                    };
                    return group;
                }
                return null;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }

        public List<GroupMasterModel> GetGroupListByUserId(long UserId = 0)
        {
            try
            {
                var getGroup = _groupMasterRepository.Table.Where(g => g.UserId == UserId && g.Status == Convert.ToInt16(ServiceStatus.Active));

                //getIp.IsEnable = true;
                //_ipMasterRepository.Update(getIp);

                //    return getGroup.Id;
                //}
                List<GroupMasterModel> gList = new List<GroupMasterModel>();
                foreach (var item in getGroup)
                {
                    GroupMasterModel gmodel = new GroupMasterModel();
                    gmodel.Id = item.Id;
                    gmodel.GroupName = item.GroupName;
                    gList.Add(gmodel);
                }
                //var group = new List<GroupMasterModel>(g=>
                //{
                //    Id = g.,
                //    //UserId = getGroup.UserId,
                //    GroupName = getGroup.GroupName,
                //    //Status = getGroup.Status,
                //    //CreatedDate = getGroup.CreatedDate,
                //    //CreatedBy = getGroup.UserId,
                //});

                //_dbContext.SaveChanges();

                return gList;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
    }
}
