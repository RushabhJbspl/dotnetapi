using Worldex.Core.ApiModels;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Communication;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels;
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
    public class NotificationHandler : IRequestHandler<SendNotificationRequest, CommunicationResponse>
    {
        private readonly IMessageRepository<NotificationQueue> _MessageRepository;
        private readonly IMessageConfiguration _MessageConfiguration;
        private readonly IMessageService _MessageService;
        private WebApiParseResponse _WebApiParseResponse;
        private GetDataForParsingAPI _GetDataForParsingAPI;
        private WebAPIParseResponseCls _GenerateResponse;
        private IMemoryCache _cache;

        public NotificationHandler(IMessageRepository<NotificationQueue> MessageRepository, MessageConfiguration MessageConfiguration, MessageService MessageService, GetDataForParsingAPI GetDataForParsingAPI, WebApiParseResponse WebApiParseResponse, WebAPIParseResponseCls GenerateResponse, IMemoryCache cache)
        {
            _MessageRepository = MessageRepository;
            _MessageConfiguration = MessageConfiguration;
            _MessageService = MessageService;
            _GetDataForParsingAPI = GetDataForParsingAPI;
            _WebApiParseResponse = WebApiParseResponse;
            _GenerateResponse = GenerateResponse;
            _cache = cache;
        }

        public async Task<CommunicationResponse> Handle(SendNotificationRequest Request, CancellationToken cancellationToken)
        {
            try
            {
                //khushali 16-01-2019 NotificationType = 3 condition for Resend Notificaton
                var Notification = new NotificationQueue()
                {
                    Message = Request.Message,
                    Subject = Request.Subject,
                    DeviceID = Request.DeviceID,
                    TickerText = Request.TickerText,
                    ContentTitle = Request.ContentTitle,
                    Status = 0,
                    CreatedBy = 1,
                    CreatedDate = DateTime.UtcNow
                };
                if (Request.NotificationType != 3)
                {
                    await _MessageRepository.Add(Notification);
                }
                else
                {
                    Notification = _MessageRepository.GetById(Request.NotificationID);
                }
                
                Notification.InQueueMessage();
                _MessageRepository.Update(Notification);
                IReadOnlyList<CommunicationProviderList> ConfigurationList = _cache.Get<IReadOnlyList<CommunicationProviderList>>("PushNotificationConfiguration");
                if (ConfigurationList == null)
                {
                    IQueryable Result = await _MessageConfiguration.GetAPIConfigurationAsync(Convert.ToInt32(enWebAPIRouteType.CommunicationAPI), Convert.ToInt32(enCommunicationServiceType.PushNotification));
                    ConfigurationList = Result.Cast<CommunicationProviderList>().ToList().AsReadOnly();
                    _cache.Set<IReadOnlyList<CommunicationProviderList>>("PushNotificationConfiguration", ConfigurationList);
                }
                else if (ConfigurationList.Count == 0)
                {
                    IQueryable Result = await _MessageConfiguration.GetAPIConfigurationAsync(Convert.ToInt32(enWebAPIRouteType.CommunicationAPI), Convert.ToInt32(enCommunicationServiceType.PushNotification));
                    ConfigurationList = Result.Cast<CommunicationProviderList>().ToList().AsReadOnly();
                    _cache.Set<IReadOnlyList<CommunicationProviderList>>("PushNotificationConfiguration", ConfigurationList);
                }
                foreach (CommunicationProviderList Provider in ConfigurationList)
                {
                    string Response = await _MessageService.SendNotificationAsync(Notification.DeviceID,Notification.TickerText, Notification.ContentTitle, Notification.Message, Provider.SendURL,Provider.RequestFormat,Provider.SenderID,Provider.MethodType,Provider.ContentType);
                    CopyClass.CopyObject(Provider, ref _GetDataForParsingAPI);
                    _GenerateResponse = _WebApiParseResponse.ParseResponseViaRegex(Response, _GetDataForParsingAPI);
                    if (_GenerateResponse.Status == enTransactionStatus.Success)
                    {
                        Notification.SentMessage();
                        _MessageRepository.Update(Notification);
                        return await Task.FromResult(new CommunicationResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.NotificationSuccessMessage });
                    }
                    else
                    {
                        continue;
                    }
                }
                Notification.FailMessage();
                _MessageRepository.Update(Notification);
                return await Task.FromResult(new CommunicationResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotificationFailMessage });
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new CommunicationResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = EnResponseMessage.NotificationExceptionMessage});
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
