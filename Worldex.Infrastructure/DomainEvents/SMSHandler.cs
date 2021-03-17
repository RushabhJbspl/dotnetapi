using Worldex.Core.ApiModels;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Communication;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.DTOClasses;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services
{
    public class SMSHandler : IRequestHandler<SendSMSRequest, CommunicationResponse>
    {
        private readonly IMessageRepository<MessagingQueue> _MessageRepository;
        private readonly IMessageConfiguration _MessageConfiguration;
        private readonly IMessageService _MessageService;
        private WebApiParseResponse _WebApiParseResponse;
        private GetDataForParsingAPI _GetDataForParsingAPI;
        private WebAPIParseResponseCls _GenerateResponse;
        private IMemoryCache _cache;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICommonRepository<CountryMaster> _countryMaster;

        public SMSHandler(IMessageRepository<MessagingQueue> MessageRepository, MessageConfiguration MessageConfiguration, 
            MessageService MessageService, GetDataForParsingAPI GetDataForParsingAPI, WebApiParseResponse WebApiParseResponse, 
            WebAPIParseResponseCls GenerateResponse, IMemoryCache cache, UserManager<ApplicationUser> userManager, ICommonRepository<CountryMaster> CountryMaster)
        {
            _MessageRepository = MessageRepository;
            _MessageConfiguration = MessageConfiguration;
            _MessageService = MessageService;
            _GetDataForParsingAPI = GetDataForParsingAPI;
            _WebApiParseResponse = WebApiParseResponse;
            _GenerateResponse = GenerateResponse;
            _cache = cache;
            _userManager = userManager;
            _countryMaster = CountryMaster;
        }

        public async Task<CommunicationResponse> Handle(SendSMSRequest Request, CancellationToken cancellationToken)
        {
            try
            {
                long CountryCode = 0;
                ApplicationUser User = await _userManager.FindByNameAsync(Request.MobileNo.ToString());
                if(User != null && !string.IsNullOrEmpty(User.CountryCode))
                {
                    CountryCode = _countryMaster.FindBy(e => e.CountryCode == User.CountryCode).FirstOrDefault().CountryDialingCode;
                }
                
                //khushali 16-01-2019 MessageType = 3 condition for Resend SMS
                var Message = new MessagingQueue()
                {
                    MobileNo = CountryCode == 0 ? Request.MobileNo : Convert.ToInt64(CountryCode.ToString() + Request.MobileNo.ToString()),
                    SMSText = Request.Message,
                    SMSSendBy = 0,
                    Status = Convert.ToInt16(enMessageService.Init),
                    CreatedBy = 1,
                    CreatedDate = DateTime.UtcNow
                };
                if (Request.MessageType != 3)
                {
                    await _MessageRepository.Add(Message);
                }
                else
                {
                    Message = _MessageRepository.GetById(Request.MessageID);
                    Message.UpdatedBy = 1;
                    Message.UpdatedDate = DateTime.UtcNow;
                }

                Message.InQueueMessage();
                _MessageRepository.Update(Message);
                if (User == null && string.IsNullOrEmpty(User?.CountryCode) && Request.MessageType != 9)
                {
                    Message.FailMessage();
                    Message.RespText = "Invali Mobile No.";
                    _MessageRepository.Update(Message);
                    return await Task.FromResult(new CommunicationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SMSFailMessage });
                }
                IReadOnlyList<CommunicationProviderList> ConfigurationList = _cache.Get<IReadOnlyList<CommunicationProviderList>>("SMSConfiguration");
                if (ConfigurationList == null)
                {
                    IQueryable Result = await _MessageConfiguration.GetAPIConfigurationAsync(Convert.ToInt32(enWebAPIRouteType.CommunicationAPI), Convert.ToInt32(enCommunicationServiceType.SMS));
                    ConfigurationList = Result.Cast<CommunicationProviderList>().ToList().AsReadOnly();
                    _cache.Set<IReadOnlyList<CommunicationProviderList>>("SMSConfiguration", ConfigurationList);
                }
                else if (ConfigurationList.Count == 0)
                {
                    IQueryable Result = await _MessageConfiguration.GetAPIConfigurationAsync(Convert.ToInt32(enWebAPIRouteType.CommunicationAPI), Convert.ToInt32(enCommunicationServiceType.SMS));
                    ConfigurationList = Result.Cast<CommunicationProviderList>().ToList().AsReadOnly();
                    _cache.Set<IReadOnlyList<CommunicationProviderList>>("SMSConfiguration", ConfigurationList);
                }
                foreach (CommunicationProviderList Provider in ConfigurationList)
                {
                    string Response = await _MessageService.SendSMSAsync(Message.MobileNo, Message.SMSText, Provider.SendURL, Provider.SenderID, Provider.UserID, Provider.Password,  Provider.Apptype == null ? Convert.ToInt64(enAppType.HttpApi) : Provider.Apptype) ;
                    //string Response = " GUID:2387354506261631296 , responseTime:2018-10-05 16:00:46";
                    CopyClass.CopyObject(Provider, ref _GetDataForParsingAPI);
                    _GenerateResponse = _WebApiParseResponse.ParseResponseViaRegex(Response, _GetDataForParsingAPI);
                    if (_GenerateResponse.Status == enTransactionStatus.Success)
                    {
                        Message.SentMessage();
                        Message.RespText = Response;
                        _MessageRepository.Update(Message);
                        return await Task.FromResult(new CommunicationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SMSSuccessMessage });
                    }
                    else
                    {
                        Message.FailMessage();
                        Message.RespText = Response;
                        _MessageRepository.Update(Message);
                        continue;
                    }
                }
                return await Task.FromResult(new CommunicationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SMSFailMessage });
            }
            catch(Exception ex)
            {
                return await Task.FromResult(new CommunicationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = EnResponseMessage.SMSExceptionMessage });
            }
        }

        //public Task<ToDoItemResponse> Handle(ToDoItemRequest request, CancellationToken cancellationToken)
        //{
        //    var toDoItem = _todoRepository.GetById(request.Id);
        //    toDoItem.MarkComplete();
        //    _todoRepository.Update(toDoItem);
        //    return Task.FromResult(new ToDoItemResponse { IsDone = toDoItem.IsDone });
        //}
    }
}
