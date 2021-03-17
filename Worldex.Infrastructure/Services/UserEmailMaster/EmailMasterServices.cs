using Worldex.Core.Entities.EmailMaster;
using Worldex.Core.Interfaces.EmailMaster;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.EmailMaster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Services.UserEmailMaster
{
    public class EmailMasterServices : IEmailMaster
    {
        private readonly WorldexContext _dbContext;
        private readonly ICustomExtendedRepository<EmailMaster> _CustomEmailRepository;
        public EmailMasterServices(ICustomExtendedRepository<EmailMaster> CustomEmailRepository, WorldexContext dbContext)
        {
            _CustomEmailRepository = CustomEmailRepository;
            _dbContext = dbContext;
        }
        public Guid Add(EmailMasterReqViewModel emailMasterViewModel)
        {
            try
            {
                var IpAddress = _CustomEmailRepository.Table.FirstOrDefault(i => i.Email == emailMasterViewModel.Email);
                if (IpAddress == null)
                {
                    EmailMaster emailMaster = new EmailMaster();
                    emailMaster.IsPrimary = emailMasterViewModel.IsPrimary;
                    emailMaster.Email = emailMasterViewModel.Email;
                    emailMaster.CreatedDate = DateTime.UtcNow;
                    emailMaster.CreatedBy = emailMasterViewModel.Userid;
                    emailMaster.UserId = emailMasterViewModel.Userid;
                    _CustomEmailRepository.Insert(emailMaster);
                    return emailMaster.Id;
                }
                else
                {
                    return IpAddress.Id;
                }
            }

            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Guid IsEmailExist(string EmailAddress)
        {
            try
            {
                var Emaildata = _CustomEmailRepository.Table.FirstOrDefault(i => i.Email == EmailAddress && i.Status == true);
                if (Emaildata == null)
                {
                    return Guid.Empty;
                }
                else
                {
                    return Emaildata.Id;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public Guid Delete(EmailMasterDeleteViewModel emailMasterViewModel)
        {
            try
            {
                var EmailAddress = _CustomEmailRepository.Table.FirstOrDefault(i => i.Id == emailMasterViewModel.Id && i.Status==true);
                if (EmailAddress != null)
                {
                    EmailMaster emailMaster = new EmailMaster();
                    EmailAddress.IsDeleted = true;
                    _CustomEmailRepository.Update(EmailAddress);
                    return EmailAddress.Id;
                }
                else
                    return Guid.Empty;


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Guid Update(EmailMasterUpdateReqViewModel emailMasterViewModel)
        {
            try
            {
                var EmailAddress = _CustomEmailRepository.Table.FirstOrDefault(i => i.Id == emailMasterViewModel.Id  && i.Status==true);
                if (EmailAddress != null)
                {
                    EmailAddress.Id = EmailAddress.Id;
                    EmailAddress.IsPrimary = emailMasterViewModel.IsPrimary;
                    EmailAddress.Email = emailMasterViewModel.Email;
                    EmailAddress.UpdatedDate = DateTime.UtcNow;
                    EmailAddress.UpdatedBy = emailMasterViewModel.Userid;
                    _CustomEmailRepository.Update(EmailAddress);
                    return EmailAddress.Id;
                }
                else
                    return Guid.Empty;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<EmailListViewModel> GetuserWiseEmailList(int UserId)
        {
            try
            {
                string Qry = "";
                IQueryable<EmailListViewModel> Result;
                Qry = @"Select * From EmailMaster where IsDeleted=0 and  userid =" + UserId;
                Result = _dbContext.emailListViewModels.FromSql(Qry);
                return Result.ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
    }
}
