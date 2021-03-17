using Worldex.Core.Entities.Backoffice.PasswordPolicy;
using Worldex.Core.Interfaces.PasswordPolicy;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.BackOffice.PasswordPolicy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Services.BackOffice.PasswordPolicy
{
    public class UserLinkMasterServices : IUserLinkMaster
    {

        private readonly WorldexContext _dbContext;
        private readonly ICustomExtendedRepository<UserLinkMaster> _UserLinkMaster;
        public UserLinkMasterServices(WorldexContext dbContext, ICustomExtendedRepository<UserLinkMaster> UserLinkMaster)
        {
            _dbContext = dbContext;
            _UserLinkMaster = UserLinkMaster;
        }
        public Guid Add(UserLinkMasterViewModel userLinkMastes)
        {
            try
            {
                UserLinkMaster userLinkMaster = new UserLinkMaster()
                {
                    LinkvalidTime = userLinkMastes.LinkvalidTime,
                    UserLinkData = userLinkMastes.UserLinkData,
                    Status = true,
                    UserId = userLinkMastes.UserId,
                    CreatedBy = userLinkMastes.UserId,
                    CreatedDate = DateTime.UtcNow
                };
                _UserLinkMaster.Insert(userLinkMaster);
                return userLinkMaster.Id;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }

        public Guid Update(UserLinkMasterUpdateViewModel userLinkMastes)
        {
            try
            {
                var UserLinkdata = _UserLinkMaster.Table.FirstOrDefault(i => i.Id == userLinkMastes.Id && i.Status == true);
                if (UserLinkdata != null)
                {
                    UserLinkdata.Status = false;
                    UserLinkdata.LinkvalidTime = userLinkMastes.LinkvalidTime;
                    UserLinkdata.UpdatedBy = userLinkMastes.UserId;
                    UserLinkdata.UpdatedDate = DateTime.UtcNow;
                    _UserLinkMaster.Update(UserLinkdata);
                    return UserLinkdata.Id;
                }
                else
                    return Guid.Empty;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
        public UserLinkMaster VerifyUserLink(Guid id)
        {
            try
            {
                var UserLinkdata = _UserLinkMaster.Table.FirstOrDefault(i => i.Id == id && i.Status == true);
                if (UserLinkdata != null)
                {
                    UserLinkdata.Status = false;
                    UserLinkdata.UpdatedBy = UserLinkdata.CreatedBy;
                    UserLinkdata.UpdatedDate = DateTime.UtcNow;
                    _UserLinkMaster.Update(UserLinkdata);
                    return UserLinkdata;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
        public UserLinkMaster GetUserLinkData(Guid id)
        {
            try
            {
                var UserLinkdata = _UserLinkMaster.Table.FirstOrDefault(i => i.Id == id);
                if (UserLinkdata != null)
                {
                    UserLinkdata.Status = false;
                    UserLinkdata.UpdatedBy = UserLinkdata.CreatedBy;
                    UserLinkdata.UpdatedDate = DateTime.UtcNow;
                    _UserLinkMaster.Update(UserLinkdata);
                    return UserLinkdata;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
    }
}
