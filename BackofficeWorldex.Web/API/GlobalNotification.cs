using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Communication;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.SignalR;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.Configuration;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.Data;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using Worldex.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BackofficeWorldex.Web.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class GlobalNotification : Controller
    {
        private readonly ILogger<GlobalNotification> _logger;
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;
        private readonly IPushNotificationsQueue<SendEmailRequest> _pushEmailQueue;
        private readonly IPushNotificationsQueue<SendNotificationRequest> _pushNotificationQueue;
        //private readonly IApplicationDataService _applicationDataService;
        private readonly IMessageRepository<EmailQueue> _EmailRepository;
        private readonly IMessageRepository<MessagingQueue> _MessageRepository;
        private readonly IMessageRepository<NotificationQueue> _NotificationRepository;
        private readonly ICommunicationService _communicationService;
        private readonly IMasterConfiguration _MasterConfiguration;

        public GlobalNotification(UserManager<ApplicationUser> userManager, ILogger<GlobalNotification> logger, IMediator mediator,
            IPushNotificationsQueue<SendSMSRequest> pushSMSQueue, IPushNotificationsQueue<SendEmailRequest> pushEmailQueue,
            IPushNotificationsQueue<SendNotificationRequest> pushNotificationQueue,
            IMessageRepository<EmailQueue> EmailRepository, IMessageRepository<MessagingQueue> MessageRepository,
            IMessageRepository<NotificationQueue> NotificationRepository, ICommunicationService communicationService,
            IMasterConfiguration MasterConfiguration)
        {
            _logger = logger;
            _mediator = mediator;
            _userManager = userManager;
            //_applicationDataService = ApplicationDataService;
            _pushSMSQueue = pushSMSQueue;
            _pushEmailQueue = pushEmailQueue;
            _pushNotificationQueue = pushNotificationQueue;
            _EmailRepository = EmailRepository;
            _MessageRepository = MessageRepository;
            _NotificationRepository = NotificationRepository;
            _communicationService = communicationService;
            _MasterConfiguration = MasterConfiguration;
        }

        #region "Global notification with signalR"

        //[HttpPost]
        //[Route("News")]
        //public async Task<IActionResult> News()
        //{
        //    try
        //    {
        //        using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
        //        {
        //            string Content = await reader.ReadToEndAsync();
        //            SignalRComm<string> CommonData = new SignalRComm<string>();
        //            CommonData.Data = Content;
        //            CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.BroadCast);
        //            CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.News);
        //            CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.BroadcastMessage);
        //            CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);

        //            SignalRData SendData = new SignalRData();
        //            SendData.Method = enMethodName.News;
        //            SendData.DataObj = JsonConvert.SerializeObject(CommonData);
        //            await _mediator.Send(SendData);
        //            return Ok();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
        //        return BadRequest();
        //    }
        //}

        //[HttpPost]
        //[Route("Announcement")]
        //public async Task<IActionResult> Announcement()
        //{
        //    try
        //    {
        //        using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
        //        {
        //            string Content = await reader.ReadToEndAsync();
        //            SignalRComm<string> CommonData = new SignalRComm<string>();
        //            CommonData.Data = Content;
        //            CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.BroadCast);
        //            CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.Announcement);
        //            CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.BroadcastMessage);
        //            CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);

        //            SignalRData SendData = new SignalRData();
        //            SendData.Method = enMethodName.Announcement;
        //            SendData.DataObj = JsonConvert.SerializeObject(CommonData);
        //            await _mediator.Send(SendData);
        //            return Ok();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
        //        return BadRequest();
        //    }
        //}

        [HttpPost]
        [Route("EnvironmentMode/{Mode}")]
        public async Task<IActionResult> EnvironmentMode(short Mode)
        {
            try
            {
                ActivityNotificationMessage LogoutActivity = new ActivityNotificationMessage();
                if (Mode == 1)
                {
                    LogoutActivity.MsgCode = Convert.ToInt32(enErrorCode.Undermaintenance);
                }
                else
                {
                    LogoutActivity.MsgCode = Convert.ToInt32(enErrorCode.Production);
                }

                SignalRComm<ActivityNotificationMessage> CommonData = new SignalRComm<ActivityNotificationMessage>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.EnvironmentMode);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.ReceiveEnvironmentMode);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.Base);
                CommonData.Data = LogoutActivity;
                CommonData.Parameter = null;

                //SignalRDataNotify SendData = new SignalRDataNotify();
                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.EnvironmentMode;
                SendData.Parameter = "1";
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                await _mediator.Send(SendData);

                return Ok(new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = "internal server error", ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region "Push SMS , Email , notification and Resend method for backoffice"
        [HttpPost]
        [Route("PushSMS")]
        [Authorize]
        public async Task<ActionResult<BizResponse>> PushSMS([FromBody]PushSMSRequest Request)
        {
            try
            {
                BizResponse Response = new BizResponse();
                SendSMSRequest sMSRequest;
                if (Request.MobileNo.Count > 0)
                {
                    foreach (var obj in Request.MobileNo)
                    {
                        if (obj.ToString().Length != 10)
                        {
                            Response.ErrorCode = enErrorCode.Invalid_MobileNo;
                            Response.ReturnCode = enResponseCodeService.Fail;
                            Response.ReturnMsg = "Fail";
                            return Response;
                        }
                        sMSRequest = new SendSMSRequest();
                        sMSRequest.MobileNo = obj;
                        sMSRequest.Message = Request.Message;
                        _pushSMSQueue.Enqueue(sMSRequest);
                    }
                }
                Response.ErrorCode = enErrorCode.Success;
                Response.ReturnCode = enResponseCodeService.Success;
                Response.ReturnMsg = "Success";
                return Response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("PushEmail")]
        [Authorize]
        public async Task<ActionResult<BizResponse>> PushEmail([FromBody]PushEmailRequest Request)
        {
            try
            {
                BizResponse Response = new BizResponse();
                SendEmailRequest sendEmail;

                if (Request.Recepient.Count != 0)
                {
                    if (Request.Recepient.Count <= 100)
                    {
                        sendEmail = new SendEmailRequest();
                        sendEmail.Recepient = (Request.Recepient != null) ? string.Join(',', Request.Recepient) : null; ;
                        sendEmail.Subject = Request.Subject;
                        sendEmail.Attachment = Request.Attachment;
                        sendEmail.BCC = (Request.BCC != null) ? string.Join(',', Request.BCC) : null;
                        sendEmail.Body = Request.Body;
                        sendEmail.CC = (Request.CC != null) ? string.Join(',', Request.CC) : null;
                        sendEmail.EmailType = 2;//komal 15-01-2019 EmailType=2 from admin 
                        _pushEmailQueue.Enqueue(sendEmail);
                    }
                    else
                    {
                        foreach (var obj in Request.Recepient)
                        {
                            sendEmail = new SendEmailRequest();
                            sendEmail.Recepient = obj;
                            sendEmail.Subject = Request.Subject;
                            sendEmail.Attachment = Request.Attachment;
                            //sendEmail.BCC = (Request.BCC != null) ? string.Join(',', Request.BCC) : null;
                            sendEmail.BCC = "";
                            sendEmail.CC = "";
                            sendEmail.Body = Request.Body;
                            //sendEmail.CC = (Request.CC != null) ? string.Join(',', Request.CC) : null;
                            sendEmail.EmailType = 2;//komal 15-01-2019 EmailType=2 from admin 
                            _pushEmailQueue.Enqueue(sendEmail);
                        }
                    }
                }
                else
                {
                    Response.ErrorCode = enErrorCode.Empty_EmailList;
                    Response.ReturnCode = enResponseCodeService.Fail;
                    Response.ReturnMsg = "Empty Email List";
                    return Response;
                }

                Response.ErrorCode = enErrorCode.Success;
                Response.ReturnCode = enResponseCodeService.Success;
                Response.ReturnMsg = "Success";
                return Response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("ResendEmail/{EmailID}")]
        [Authorize]
        public async Task<ActionResult<BizResponse>> ResendEmail(long EmailID)
        {
            try
            {
                BizResponse Response = new BizResponse();
                if (EmailID != null)
                {
                    var EmailDetail = _EmailRepository.GetById(EmailID);
                    if (EmailDetail.Status == Convert.ToInt16(MessageStatusType.Fail) || EmailDetail.Status == Convert.ToInt16(MessageStatusType.Success) && (EmailDetail.UpdatedDate == null || (EmailDetail.UpdatedDate != null && EmailDetail.UpdatedDate < DateTime.UtcNow.AddMinutes(-10))))
                    {
                        var sendEmail = new SendEmailRequest();
                        sendEmail.Recepient = EmailDetail.Recepient;
                        sendEmail.Subject = EmailDetail.Subject;
                        sendEmail.Attachment = EmailDetail.Attachment;
                        sendEmail.BCC = EmailDetail.BCC;
                        sendEmail.Body = EmailDetail.Body;
                        sendEmail.CC = EmailDetail.CC;
                        sendEmail.EmailType = 3;
                        sendEmail.EmailID = EmailID;
                        _pushEmailQueue.Enqueue(sendEmail);
                    }
                    else
                    {
                        Response.ErrorCode = enErrorCode.MessageInQueue;
                        Response.ReturnCode = enResponseCodeService.Fail;
                        Response.ReturnMsg = "Your request is already in processing.";
                        return Response;
                    }
                }
                else
                {
                    Response.ErrorCode = enErrorCode.InvalidInput;
                    Response.ReturnCode = enResponseCodeService.Fail;
                    Response.ReturnMsg = "Invali Input";
                    return Response;
                }

                Response.ErrorCode = enErrorCode.Success;
                Response.ReturnCode = enResponseCodeService.Success;
                Response.ReturnMsg = "Success";
                return Response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("ResendSMS/{MessageID}")]
        [Authorize]
        public async Task<ActionResult<BizResponse>> ResendMessage(long MessageID)
        {
            try
            {
                BizResponse Response = new BizResponse();
                if (MessageID != null)
                {
                    var MessageDetail = _MessageRepository.GetById(MessageID);
                    if (MessageDetail.Status == Convert.ToInt16(MessageStatusType.Fail) && (MessageDetail.UpdatedDate == null || MessageDetail.UpdatedDate < DateTime.UtcNow.AddMinutes(-10)))
                    {
                        var sMSRequest = new SendSMSRequest();
                        sMSRequest.MobileNo = MessageDetail.MobileNo;
                        sMSRequest.Message = MessageDetail.SMSText;
                        sMSRequest.MessageType = 3;
                        sMSRequest.MessageID = MessageID;
                        _pushSMSQueue.Enqueue(sMSRequest);
                    }
                    else
                    {
                        Response.ErrorCode = enErrorCode.MessageInQueue;
                        Response.ReturnCode = enResponseCodeService.Fail;
                        Response.ReturnMsg = "Your request is already in processing.";
                        return Response;
                    }
                }
                else
                {
                    Response.ErrorCode = enErrorCode.InvalidInput;
                    Response.ReturnCode = enResponseCodeService.Fail;
                    Response.ReturnMsg = "Invali Input";
                    return Response;
                }

                Response.ErrorCode = enErrorCode.Success;
                Response.ReturnCode = enResponseCodeService.Success;
                Response.ReturnMsg = "Success";
                return Response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("ResendNotification/{NotificationID}")]
        [Authorize]
        public async Task<ActionResult<BizResponse>> ResendNotification(long NotificationID)
        {
            try
            {
                BizResponse Response = new BizResponse();
                if (NotificationID != null)
                {
                    var NotificationDetail = _NotificationRepository.GetById(NotificationID);
                    if (NotificationDetail.Status == Convert.ToInt16(MessageStatusType.Fail))
                    {
                        var Notification = new SendNotificationRequest();
                        Notification.Message = NotificationDetail.Message;
                        Notification.DeviceID = NotificationDetail.DeviceID;
                        Notification.Subject = NotificationDetail.Subject;
                        Notification.TickerText = NotificationDetail.TickerText;
                        Notification.ContentTitle = NotificationDetail.ContentTitle;
                        Notification.NotificationType = 3;
                        Notification.NotificationID = NotificationID;
                        _pushNotificationQueue.Enqueue(Notification);
                    }
                    else
                    {
                        Response.ErrorCode = enErrorCode.MessageInQueue;
                        Response.ReturnCode = enResponseCodeService.Fail;
                        Response.ReturnMsg = "Your request is already in processing.";
                        return Response;
                    }
                }
                else
                {
                    Response.ErrorCode = enErrorCode.InvalidInput;
                    Response.ReturnCode = enResponseCodeService.Fail;
                    Response.ReturnMsg = "Invali Input";
                    return Response;
                }

                Response.ErrorCode = enErrorCode.Success;
                Response.ReturnCode = enResponseCodeService.Success;
                Response.ReturnMsg = "Success";
                return Response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("PushNotification")]
        [Authorize]
        public async Task<ActionResult<BizResponse>> PushNotification([FromBody]PushNotificationRequest Request)
        {
            try
            {
                BizResponse Response = new BizResponse();
                SendNotificationRequest Notification;

                if (Request.DeviceID.Length != 0)
                {

                    Notification = new SendNotificationRequest();
                    Notification.Message = Request.Message;
                    Notification.DeviceID = (Request.DeviceID != null) ? string.Join(',', Request.DeviceID) : null;
                    Notification.Subject = Request.Subject;
                    Notification.TickerText = Request.TickerText;
                    Notification.ContentTitle = Request.ContentTitle;
                    Notification.NotificationType = 0;
                    _pushNotificationQueue.Enqueue(Notification);
                }
                else
                {
                    Response.ErrorCode = enErrorCode.Empty_DeviceList;
                    Response.ReturnCode = enResponseCodeService.Fail;
                    Response.ReturnMsg = "Empty Device List";
                    return Response;
                }

                Response.ErrorCode = enErrorCode.Success;
                Response.ReturnCode = enResponseCodeService.Success;
                Response.ReturnMsg = "Success";
                return Response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return BadRequest();
            }
        }

        //[HttpGet]
        //[Route("GetDeviceList")]
        //public async Task<ActionResult<DeviceUserResponseRes>> GetDeviceList()
        //{
        //    try
        //    {
        //        DeviceUserResponseRes Response = new DeviceUserResponseRes();
        //        Response = _communicationService.GetDeviceList();
        //        Response.ErrorCode = enErrorCode.Success;
        //        Response.ReturnCode = enResponseCode.Success;
        //        Response.ReturnMsg = "Success";
        //        return Ok(Response);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
        //        return BadRequest();
        //    }
        //}

        //[HttpPost]
        //[Route("SendToAllEmail")]
        //public async Task<ActionResult<BizResponse>> SendToAllEmail([FromBody] SendAllEmailRequest Request)
        //{
        //    int SendEmailCount = 100;
        //    try
        //    {
        //        BizResponse Response = new BizResponse();
        //        SendEmailRequest sendEmail;

        //        var list = _MasterConfiguration.GetAllEmail();
        //        if (list.Count != 0)
        //        {
        //            int cnt = 1;
        //            int skip = SendEmailCount * (cnt - 1);
        //            var batch = list.Skip(skip).Take(SendEmailCount).Select(e=>e.emailid).ToList();

        //            while (batch.Count != 0) 
        //            {
        //                sendEmail = new SendEmailRequest();
        //                sendEmail.Recepient = (batch != null) ? string.Join(',', batch) : null; 
        //                sendEmail.Subject = Request.Subject;
        //                sendEmail.Body = Request.Body;
        //                sendEmail.EmailType = 2;//komal 15-01-2019 EmailType=2 from admin 
        //                _pushEmailQueue.Enqueue(sendEmail);

        //                cnt += 1;
        //                skip = SendEmailCount * (cnt - 1);
        //                batch = list.Skip(skip).Take(SendEmailCount).Select(e => e.emailid).ToList();
        //            }

        //            Response.ErrorCode = enErrorCode.Success;
        //            Response.ReturnCode = enResponseCodeService.Success;
        //            Response.ReturnMsg = "Success";
        //            return Response;
        //        }
        //        Response.ErrorCode = enErrorCode.NoDataFound;
        //        Response.ReturnCode = enResponseCodeService.Fail;
        //        Response.ReturnMsg = "Fail";
        //        return Response;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
        //        return BadRequest();
        //    }
        //}
        #endregion
    }
}
