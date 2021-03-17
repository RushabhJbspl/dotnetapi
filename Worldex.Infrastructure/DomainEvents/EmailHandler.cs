using Worldex.Core.ApiModels;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Communication;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.DTOClasses;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services
{
    public class EmailHandler : IRequestHandler<SendEmailRequest, CommunicationResponse>
    {
        private readonly IMessageRepository<EmailQueue> _MessageRepository;
        private readonly IMessageConfiguration _MessageConfiguration;
        private readonly IMessageService _MessageService;
        private WebApiParseResponse _WebApiParseResponse;
        private GetDataForParsingAPI _GetDataForParsingAPI;
        private WebAPIParseResponseCls _GenerateResponse;
        private IMemoryCache _cache;

        public EmailHandler(IMessageRepository<EmailQueue> MessageRepository, MessageConfiguration MessageConfiguration, MessageService MessageService, GetDataForParsingAPI GetDataForParsingAPI, WebApiParseResponse WebApiParseResponse, WebAPIParseResponseCls GenerateResponse, IMemoryCache cache) //IPushNotificationsQueue PushNotificationsQueue
        {
            _MessageRepository = MessageRepository;
            _MessageConfiguration = MessageConfiguration;
            _MessageService = MessageService;
            _GetDataForParsingAPI = GetDataForParsingAPI;
            _WebApiParseResponse = WebApiParseResponse;
            _GenerateResponse = GenerateResponse;
            _cache = cache;
        }

        public async Task<CommunicationResponse> Handle(SendEmailRequest Request, CancellationToken cancellationToken)
        {
            try
            {
                //komal 15-01-2019 EmailType=2 from admin
                //khushali 16-01-2019 EmailType = 3 condition for Resend Mail
                var Email = new EmailQueue()
                {
                    EmailType = Request.EmailType,
                    Recepient = Request.Recepient,
                    Attachment = Request.Attachment,
                    BCC = Request.BCC,
                    CC = Request.CC,
                    Body = Request.Body,
                    Status = Convert.ToInt16(MessageStatusType.Initialize),
                    Subject = Request.Subject,
                    CreatedDate = DateTime.UtcNow
                };
                if (Request.EmailType != 3)
                {                   
                    await _MessageRepository.Add(Email);                    
                }
                else
                {
                    Email =  _MessageRepository.GetById(Request.EmailID);
                    Email.UpdatedBy = 1;
                    Email.UpdatedDate = DateTime.UtcNow;
                }

                Email.InQueueMessage();
                _MessageRepository.Update(Email);

                //Testing by khushali
                //IReadOnlyList<CommunicationProviderList> ConfigurationList = _cache.Get<IReadOnlyList<CommunicationProviderList>>("EmailConfiguration");
                //if (ConfigurationList == null)
                //{
                HelperForLog.WriteLogIntoFile("EmailHandler", "1 SendMessage", " -Data- ");
                IEnumerable<CommunicationProviderList> ConfigurationList = await _MessageConfiguration.GetAPIConfigurationAsyncV1(Convert.ToInt32(enWebAPIRouteType.CommunicationAPI), Convert.ToInt32(enCommunicationServiceType.Email));
                //HelperForLog.WriteLogForSocket("EmailHandler", "0 SendMessage", " -Data- ");
                //IQueryable Result = await _MessageConfiguration.GetAPIConfigurationAsync(Convert.ToInt32(enWebAPIRouteType.CommunicationAPI), Convert.ToInt32(enCommunicationServiceType.Email));
                //HelperForLog.WriteLogForSocket("EmailHandler", "0 SendMessage", " -Data- ");
                

                //    ConfigurationList = Result.Cast<CommunicationProviderList>().ToList().AsReadOnly();
                //    _cache.Set<IReadOnlyList<CommunicationProviderList>>("EmailConfiguration", ConfigurationList);
                //}
                //else if (ConfigurationList.Count == 0)
                //{
                //    IQueryable Result = await _MessageConfiguration.GetAPIConfigurationAsync(Convert.ToInt32(enWebAPIRouteType.CommunicationAPI), Convert.ToInt32(enCommunicationServiceType.Email));
                //    ConfigurationList = Result.Cast<CommunicationProviderList>().ToList().AsReadOnly();
                //    _cache.Set<IReadOnlyList<CommunicationProviderList>>("EmailConfiguration", ConfigurationList);
                //}
                foreach (CommunicationProviderList Provider in ConfigurationList)
                {
                    string Response = await _MessageService.SendEmailAsync(Email.Recepient, Email.Subject, Email.BCC, Email.CC, Email.Body, Provider.SendURL, Provider.UserID, Provider.Password, Provider.SenderID);
                    CopyClass.CopyObject(Provider, ref _GetDataForParsingAPI);
                    _GenerateResponse = _WebApiParseResponse.ParseResponseViaRegex(Response, _GetDataForParsingAPI);
                    if (_GenerateResponse.Status == enTransactionStatus.Success)
                    {
                        Email.SentMessage();
                        _MessageRepository.Update(Email);
                        return await Task.FromResult(new CommunicationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.EMailSuccessMessage });
                    }
                    else
                    {
                        Email.FailMessage();
                        _MessageRepository.Update(Email);
                        continue;
                    }
                }
                return await Task.FromResult(new CommunicationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.EmailFailMessage });
            }
            catch (Exception ex)
            {
               return await Task.FromResult(new CommunicationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = EnResponseMessage.EmailExceptionMessage});
            }
        }
    }
}
