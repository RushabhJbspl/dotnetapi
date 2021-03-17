using Worldex.Core.ApiModels;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Plivo;
using Plivo.Resource.Message;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using static Twilio.Rest.Api.V2010.Account.Call.FeedbackSummaryResource;

namespace Worldex.Infrastructure.Services
{
    public class MessageService : IMessageService
    {
        //public EmailSettings _emailSettings { get; }
        //public SMSSetting _smsSettings { get; set; }
        public IWebApiSendRequest _WebAPISendRequest { get; set; }//2018-12-10 add inferface ,to Solve error of console app DI
        private IMessageConfiguration _messageConfiguration { get; set; }
        private IMemoryCache _cache { get; set; }

        public MessageService(IWebApiSendRequest WebAPISendRequest, IMessageConfiguration MessageConfiguration, IMemoryCache Cache)
        {
            //_emailSettings = emailSettings.Value;
            //_smsSettings = SmsSetting.Value;
            _WebAPISendRequest = WebAPISendRequest;
            _messageConfiguration = MessageConfiguration;
            _cache = Cache;
        }

        public Task<string> SendEmailAsync(string Recepient, string Subject, string BCC, string CC, string Body, string Url, string UserID, string Password, string Port)
        {
            try
            {
                if(Url.ToLower() == "smtp.gmail.com")
                {
                    //Thread.Sleep(5000);
                    List<string> CCList = new List<string>();
                    List<string> BccList = new List<string>();
                    List<string> RecepientList = new List<string>(); //komal 21-01-2019 Make list Recepient
                    MailMessage mail = new MailMessage()
                    {
                        From = new MailAddress(UserID)
                    };
                    //mail.To.Add(new MailAddress(toEmail));
                    if (!string.IsNullOrEmpty(Recepient))
                    {
                        RecepientList = Recepient.Split(',').ToList();
                    }
                    if (!string.IsNullOrEmpty(CC))
                    {
                        CCList = CC.Split(',').ToList();
                    }
                    if (!string.IsNullOrEmpty(BCC))
                    {
                        BccList = BCC.Split(',').ToList();
                    }
                    mail.Subject = Subject;
                    foreach (string Rc in RecepientList)
                    {
                        mail.To.Add(new MailAddress(Rc));
                    }
                    foreach (string cc in CCList)
                    {
                        mail.CC.Add(new MailAddress(cc));
                    }
                    foreach (string Bcc in BccList)
                    {
                        mail.Bcc.Add(new MailAddress(Bcc));
                    }
                    mail.Body = Body;
                    mail.IsBodyHtml = true;
                    mail.Priority = MailPriority.High;
                    //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
                    int val = 0;
                    if (Int32.TryParse(Port.Trim(), out val))
                    {
                        using (SmtpClient smtp = new SmtpClient(Url, val))
                        {
                            smtp.Credentials = new NetworkCredential(UserID, Password);
                            smtp.EnableSsl = true;
                            smtp.SendMailAsync(mail).Wait();
                        }
                        return Task.FromResult("Success");
                    }
                    return Task.FromResult("Fail");
                }
                else if (Url.ToLower() == "smtp.sendgrid.net")
                {
                    SendGrid.Response response;
                    var client = new SendGridClient(Password);
                    var from = new EmailAddress(UserID,Port); // ' Login Email and Display Its name in mail,on Inbox
                    var sto = new EmailAddress(Recepient, "");
                    var plainTextContent = Body;
                    var htmlContent = Body;
                    var msg = MailHelper.CreateSingleEmail(from, sto, Subject, plainTextContent, htmlContent);
                    response = client.SendEmailAsync(msg).GetAwaiter().GetResult();

                    if (response.StatusCode == HttpStatusCode.Accepted)
                    {
                        return Task.FromResult("Success");
                    }
                    else
                    {
                        Task.Run(() => HelperForLog.WriteLogIntoFile("SendEmailAsync", "Messageservice", "API Response: " + JsonConvert.SerializeObject(response)));
                        return Task.FromResult("Fail");
                    }
                }
                return Task.FromResult("Fail");
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("SendEmailAsync ", "Messageservice", ex));
                return Task.FromResult("Fail");
            }            
        }

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            else
                return true;
        }

        public Task<string> SendSMSAsync(long Mobile, string Message, string Url, string SerderID, string UserID, string Password,long Apptype)
        {
            string Response="";
            try
            {
                //Thread.Sleep(0);
                Url = Url.Replace("[USERID]", UserID);
                Url = Url.Replace("[AUTHKEY]", Password);
                Url = Url.Replace("[MOBILENO]", Mobile.ToString());
                Url = Url.Replace("[SENDERID]", SerderID);
                Url = Url.Replace("[MSGTEXT]", Message);
                switch (Apptype)
                {
                    case (long)enAppType.HttpApi:
                        Response = _WebAPISendRequest.SendRequestAsync(Url, "", "GET", "application/json", null, 15000).GetAwaiter().GetResult();
                        break;
                    case (long)enAppType.Twillio:
                        string accountSid = UserID ;
                        string authToken = Password;

                        TwilioClient.Init(accountSid, authToken);

                        try
                        {
                            MessageResource message = MessageResource.Create(
                            from: new Twilio.Types.PhoneNumber(SerderID),
                            body: Message,
                            to: new Twilio.Types.PhoneNumber("+" + Mobile.ToString())
                            );
                            if (message?.Status != null && message.Status != StatusEnum.Failed)
                            {
                                Response = "SUCCESS";
                            }
                            else
                            {
                                Task.Run(() => HelperForLog.WriteLogIntoFile("SendSMSAsync", "Messageservice", "API Response: " + JsonConvert.SerializeObject(message)));
                                Response = "Fail";
                            }
                        }
                        catch (Exception ex)
                        {
                            Task.Run(() => HelperForLog.WriteErrorLog("SendSMSAsync", "Messageservice", ex));
                            Response = "Fail";
                        }
                        break;
                    case (long)enAppType.Plivo: // khushali - 08-05-2019 Plivo SMS API integration for worldex
                        var PlivoAPI = new PlivoApi(UserID, Password);
                        MessageCreateResponse PlivoResponse = PlivoAPI.Message.Create(
                            src: SerderID,
                            dst: new List<String> { "+" + Mobile.ToString() },
                            text: Message
                        );
                        Task.Run(() => HelperForLog.WriteLogIntoFile("SendSMSAsync", "Messageservice", "API Response: " + JsonConvert.SerializeObject(PlivoResponse)));

                        if (PlivoResponse != null && PlivoResponse?.MessageUuid.Count > 0)
                        {
                            var MessageResp = PlivoAPI.Message.Get(PlivoResponse.MessageUuid.First());
                            if(MessageResp.MessageState.ToLower() != "failed")
                            {
                                Response = "SUCCESS";
                            }
                            else
                            {
                                Response = "Fail";
                            }                            
                        }
                        else
                        {
                            Response = "Fail";
                        }                            
                        break;

                    default:
                        Task.Run(() => HelperForLog.WriteLogIntoFile("SendSMSAsync", "Messageservice", "API Response: No Provider Found:"));
                        Response = "Fail";
                        break;
                }
                return Task.FromResult(Response);
            }
            catch(Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("SendSMSAsync" , "Messageservice", ex));
                throw ex;
            }            
        }

        public Task<string> SendNotificationAsync(string DeviceID,string tickerText,string contentTitle, string Message, string Url, string Request,string APIKey,string MethodType, string ContentType)
        {
            string Response = "";
            try
            {
                //{ "registration_ids": [#DeviceID#], "data": {"tickerText":"#tickerText#", "contentTitle":"#contentTitle#","message": "#Message#"}}
                string[] DeviceList = DeviceID.Split(",");
                string strdevice = JsonConvert.SerializeObject(DeviceList);
                Request = Request.Replace("#DeviceID#", strdevice);
                Request = Request.Replace("#tickerText#", tickerText);
                Request = Request.Replace("#contentTitle#", contentTitle);
                Request = Request.Replace("#Message#", Message);
                WebHeaderCollection HeaderCollection = new WebHeaderCollection();
                HeaderCollection.Add(string.Format("Authorization: key={0}", APIKey));
                Response = _WebAPISendRequest.SendRequestAsync(Url, Request,MethodType,ContentType, HeaderCollection).GetAwaiter().GetResult();
                return Task.FromResult(Response);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public async Task SendEmail(string email, string subject, string message)
        //{
        //    try
        //    {
        //        string toEmail = string.IsNullOrEmpty(email) ? _emailSettings.ToEmail : email;
        //        MailMessage mail = new MailMessage()
        //        {
        //            From = new MailAddress(_emailSettings.UsernameEmail)
        //        };
        //        mail.To.Add(new MailAddress(toEmail));
        //        //mail.CC.Add(new MailAddress(_emailSettings.CcEmail));

        //        mail.Subject = "JOSHI BIZTECH SOLUTIONS LIMITED  - " + subject;
        //        mail.Body = message;
        //        mail.IsBodyHtml = true;
        //        mail.Priority = MailPriority.High;

        //        using (SmtpClient smtp = new SmtpClient(_emailSettings.PrimaryDomain, _emailSettings.PrimaryPort))
        //        {
        //            smtp.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
        //            smtp.EnableSsl = true;
        //            await smtp.SendMailAsync(mail);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //    }
        //}     

        // khushali 08-12-2018 For SMS and EMail not working parrallay - error Dbcontext Second operation start
        //public Task<TemplateMasterData> SendMessageAsync(EnTemplateType TemplateType, CommunicationParamater MessageParameter,enCommunicationServiceType CommType)
        //{
        //    try
        //    {
        //        IQueryable Result = null;

        //        if (CommType == enCommunicationServiceType.Email)
        //        {
        //            Result =  _messageConfiguration.GetTemplateConfigurationAsyncV1(Convert.ToInt16(CommType), Convert.ToInt16(TemplateType),CommType);
        //        }
        //        else if(CommType == enCommunicationServiceType.SMS)
        //        {
        //            Result = _messageConfiguration.GetTemplateConfigurationAsyncV1(Convert.ToInt16(CommType), Convert.ToInt16(TemplateType), CommType);
        //        }

        //        foreach (TemplateMasterData Provider in Result)
        //        {
        //            Provider.Content = Provider.Content.Replace("#Param1#", MessageParameter.Param1);
        //            Provider.Content = Provider.Content.Replace("#Param2#", MessageParameter.Param2);
        //            Provider.Content = Provider.Content.Replace("#Param3#", MessageParameter.Param3);
        //            Provider.Content = Provider.Content.Replace("#Param4#", MessageParameter.Param4);
        //            Provider.Content = Provider.Content.Replace("#Param5#", MessageParameter.Param5);
        //            Provider.Content = Provider.Content.Replace("#Param6#", MessageParameter.Param6);
        //            Provider.Content = Provider.Content.Replace("#Param7#", MessageParameter.Param7);
        //            Provider.Content = Provider.Content.Replace("#Param8#", MessageParameter.Param8);
        //            Provider.Content = Provider.Content.Replace("#Param9#", MessageParameter.Param9);
        //            Provider.Content = Provider.Content.Replace("#Param10#", MessageParameter.Param10);

        //            return Task.FromResult(Provider);
        //        }
        //        return null;
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        public async Task<TemplateMasterData> ReplaceTemplateMasterData(EnTemplateType TemplateType, CommunicationParamater MessageParameter, enCommunicationServiceType CommType)
        {
            try
            {
                IList<TemplateMasterData> Result = null;
                IReadOnlyList<TemplateMasterData> ConfigurationList = _cache.Get<IReadOnlyList<TemplateMasterData>>("TemplateConfiguration");
                if (ConfigurationList == null)
                {
                    Result = _messageConfiguration.GetTemplateConfigurationAsyncV1();
                    ConfigurationList = Result.ToList().AsReadOnly();
                    _cache.Set<IReadOnlyList<TemplateMasterData>>("TemplateConfiguration", ConfigurationList);
                }
                else if (ConfigurationList.Count == 0)
                {
                    Result = _messageConfiguration.GetTemplateConfigurationAsyncV1();
                    ConfigurationList = Result.ToList().AsReadOnly();
                    _cache.Set<IReadOnlyList<TemplateMasterData>>("TemplateConfiguration", ConfigurationList);
                }

                //if (CommType == enCommunicationServiceType.Email)
                //{
                //Result =  _messageConfiguration.GetTemplateConfigurationAsyncV1(Convert.ToInt16(CommType), Convert.ToInt16(TemplateType),CommType);
                //}
                //else if(CommType == enCommunicationServiceType.SMS)
                //{
                //    Result = _messageConfiguration.GetTemplateConfigurationAsyncV1(Convert.ToInt16(CommType), Convert.ToInt16(TemplateType), CommType);
                //}

                //ConfigurationList = ConfigurationList.Select(s => s.TemplateID == Convert.ToInt32(TemplateType) && s.CServiceTypeID == Convert.ToInt32(CommType)).ToList().AsReadOnly();
                List<TemplateMasterData> TempConfigurationList = new List<TemplateMasterData>();
                
                //TempConfigurationList = ConfigurationList.AsEnumerable().Select((template, index) => new TemplateMasterData() {
                //    Content = template.Content,
                //   AdditionalInfo = template.AdditionalInfo,
                //    IsOnOff  = template.IsOnOff,
                //     TemplateID  = template.TemplateID,
                //    ServiceTypeID = template.ServiceTypeID
                //    }).Where(s => s.TemplateID == Convert.ToInt32(TemplateType) && s.ServiceTypeID == Convert.ToInt32(CommType)).ToList();
                
                TempConfigurationList = ConfigurationList.Where(s => s.TemplateID == Convert.ToInt32(TemplateType) && s.ServiceTypeID == Convert.ToInt32(CommType) && s.IsOnOff == 1).ToList();
                foreach (TemplateMasterData Provider in TempConfigurationList.ToList())
                {
                    var CopyTemplateMasterData = new TemplateMasterData();
                    CopyTemplateMasterData = (TemplateMasterData)Provider.Clone();
                    CopyTemplateMasterData.Content = CopyTemplateMasterData.Content.Replace("###Param1###".ToUpper(), MessageParameter.Param1);
                    CopyTemplateMasterData.Content = CopyTemplateMasterData.Content.Replace("###Param2###".ToUpper(), MessageParameter.Param2);
                    CopyTemplateMasterData.Content = CopyTemplateMasterData.Content.Replace("###Param3###".ToUpper(), MessageParameter.Param3);
                    CopyTemplateMasterData.Content = CopyTemplateMasterData.Content.Replace("###Param4###".ToUpper(), MessageParameter.Param4);
                    CopyTemplateMasterData.Content = CopyTemplateMasterData.Content.Replace("###Param5###".ToUpper(), MessageParameter.Param5);
                    CopyTemplateMasterData.Content = CopyTemplateMasterData.Content.Replace("###Param6###".ToUpper(), MessageParameter.Param6);
                    CopyTemplateMasterData.Content = CopyTemplateMasterData.Content.Replace("###Param7###".ToUpper(), MessageParameter.Param7);
                    CopyTemplateMasterData.Content = CopyTemplateMasterData.Content.Replace("###Param8###".ToUpper(), MessageParameter.Param8);
                    CopyTemplateMasterData.Content = CopyTemplateMasterData.Content.Replace("###Param9###".ToUpper(), MessageParameter.Param9);
                    CopyTemplateMasterData.Content = CopyTemplateMasterData.Content.Replace("###Param10###".ToUpper(), MessageParameter.Param10);
                    CopyTemplateMasterData.Content = CopyTemplateMasterData.Content.Replace("###Param11###".ToUpper(), MessageParameter.Param11);
                    CopyTemplateMasterData.Content = CopyTemplateMasterData.Content.Replace("###Param12###".ToUpper(), MessageParameter.Param12);
                    CopyTemplateMasterData.Content = CopyTemplateMasterData.Content.Replace("###Param13###".ToUpper(), MessageParameter.Param13);
                    CopyTemplateMasterData.Content = CopyTemplateMasterData.Content.Replace("###Param14###".ToUpper(), MessageParameter.Param14);
                    CopyTemplateMasterData.Content = CopyTemplateMasterData.Content.Replace("###Param15###".ToUpper(), MessageParameter.Param15);
                    CopyTemplateMasterData.Content = CopyTemplateMasterData.Content.Replace("###year###".ToUpper(), DateTime.Now.Year.ToString());

                    return await Task.FromResult(CopyTemplateMasterData);
                }
                return null;
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("template configuration:#TemplateType " + TemplateType, "Messageservice", ex));
                return null;
            }
        }
    }
}
