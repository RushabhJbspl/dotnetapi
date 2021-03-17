using Worldex.Core.Entities.User;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.SecurityQuestion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Services.SecurityQuestion
{
    public class SecurityQuestionServices : ISecurityQuestion
    {
        private readonly WorldexContext _dbContext;
        private readonly ICustomExtendedRepository<SecurityQuestionMaster> _securityQuestionRepository;
        public SecurityQuestionServices(WorldexContext dbContext, ICustomExtendedRepository<SecurityQuestionMaster> securityQuestionRepository)
        {
            _dbContext = dbContext;
            _securityQuestionRepository = securityQuestionRepository;
        }
        public Guid Add(SecurityQuestionMasterReqViewModel securityQuestionMasterViewModel)
        {
            try
            {
                var _securityQuestion = _securityQuestionRepository.Table.FirstOrDefault(i => i.UserId == securityQuestionMasterViewModel.Userid && i.Status == true);
                if (_securityQuestion == null)
                {
                    SecurityQuestionMaster securityQuestionMaster = new SecurityQuestionMaster();
                    securityQuestionMaster.SecurityQuestion = securityQuestionMasterViewModel.SecurityQuestion;
                    securityQuestionMaster.Answer = securityQuestionMasterViewModel.Answer;
                    securityQuestionMaster.UserId = securityQuestionMasterViewModel.Userid;
                    securityQuestionMaster.CreatedDate = DateTime.UtcNow;
                    securityQuestionMaster.CreatedBy = securityQuestionMasterViewModel.Userid;
                    _securityQuestionRepository.Insert(securityQuestionMaster);
                    return securityQuestionMaster.Id;
                }
                else
                {
                    _securityQuestion.SecurityQuestion = securityQuestionMasterViewModel.SecurityQuestion;
                    _securityQuestion.Answer = securityQuestionMasterViewModel.Answer;
                    _securityQuestion.UserId = securityQuestionMasterViewModel.Userid;
                    _securityQuestion.UpdatedDate = DateTime.UtcNow;
                    _securityQuestion.UpdatedBy = securityQuestionMasterViewModel.Userid;
                    _securityQuestionRepository.Update(_securityQuestion);
                    return _securityQuestion.Id;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }

        public Guid Update(SecurityQuestionMasterReqViewModel securityQuestionMasterViewModel)
        {
            try
            {
                var _securityQuestion = _securityQuestionRepository.Table.FirstOrDefault(i => i.UserId == securityQuestionMasterViewModel.Userid);
                if (_securityQuestion != null)
                {
                    _securityQuestion.SecurityQuestion = securityQuestionMasterViewModel.SecurityQuestion;
                    _securityQuestion.Answer = securityQuestionMasterViewModel.Answer;
                    _securityQuestion.UserId = securityQuestionMasterViewModel.Userid;
                    _securityQuestion.UpdatedDate = DateTime.UtcNow;
                    _securityQuestion.UpdatedBy = securityQuestionMasterViewModel.Userid;
                    _securityQuestionRepository.Update(_securityQuestion);
                    return _securityQuestion.Id;
                }
                else
                {
                    return Guid.Empty;
                }

            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
    }
}
