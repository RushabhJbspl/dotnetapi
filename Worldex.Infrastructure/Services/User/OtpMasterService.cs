using System;
using System.Linq;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.User;
using Worldex.Core.StaticClasses;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.AccountViewModels.SignUp;
using Worldex.Infrastructure.BGTask;
using Microsoft.EntityFrameworkCore;

namespace Worldex.Infrastructure.Services.User
{
    public partial class OtpMasterService : IOtpMasterService
    {
        private readonly WorldexContext _dbContext;
        private readonly IUserService _userService;
        //private readonly ICustomRepository<OtpMaster> _customRepository;
        private readonly IMessageRepository<OtpMaster> _customRepository;
        private readonly IRegisterTypeService _registerTypeService;
        //private readonly IMediator _mediator;
        private readonly IMessageService _messageService;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue; //24-11-2018 komal make Email Enqueue
        private IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;//24-11-2018 komal make SMS Enqueue

        public OtpMasterService(
            WorldexContext dbContext, IUserService userService,
            //ICustomRepository<OtpMaster> customRepository,
            IMessageRepository<OtpMaster> customRepository,


        //IRegisterTypeService registerTypeService, IMediator mediator, IMessageService messageService)
        IRegisterTypeService registerTypeService, IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue,
        IMessageService MessageService, IPushNotificationsQueue<SendSMSRequest> PushSMSQueue)
        {
            _dbContext = dbContext;
            _userService = userService;
            _customRepository = customRepository;
            //_customRepository = customRepository;
            _registerTypeService = registerTypeService;
            //_mediator = mediator;
            _messageService = MessageService;
            _pushNotificationsQueue = pushNotificationsQueue; //24-11-2018 komal make Email Enqueue
            _pushSMSQueue = PushSMSQueue; //24-11-2018 komal make SMS Enqueue
        }

        public async Task<OtpMasterViewModel> AddOtp(int UserId, string Email = null, string Mobile = null)
        {
            try//Rita 11-3-19 added try cach as error in LoginWithEmail Object reference not set to an instance of an object.
            {                
                //long currentOTP = 0;
                //int count = 0;
                //while (currentOTP.ToString().Length != 6)
                //{
                //    if (count >= 5)
                //        break;
                //    currentOTP = SecureRandomNumberGenerator.HOTP(SecureRandomNumberGenerator.CounterNow());
                //    count++;
                //}

                string OtpValue = string.Empty;                
                OtpValue = _userService.GenerateRandomOTPWithPassword().ToString();
                string alpha = string.Empty; string numeric = string.Empty;
                foreach (char str in OtpValue)
                {
                    if (char.IsDigit(str))
                    {
                        if (numeric.Length < 6)
                            numeric += str.ToString();
                        else
                            alpha += str.ToString();
                    }
                    else
                        alpha += str.ToString();
                }

                int Regtypeid = 0;
                if (!String.IsNullOrEmpty(Email))
                {
                    Regtypeid = await _registerTypeService.GetRegisterId(Core.Enums.enRegisterType.Email);
                }
                else if (!String.IsNullOrEmpty(Mobile))
                {
                    Regtypeid = await _registerTypeService.GetRegisterId(Core.Enums.enRegisterType.Mobile);
                }

                var currentotp = new OtpMaster
                {
                    UserId = UserId,
                    RegTypeId = Regtypeid,
                    OTP = numeric,//currentOTP.ToString(),
                    //ExpirTime = DateTime.UtcNow.AddHours(2),
                    ExpirTime = DateTime.UtcNow.AddMinutes(5), // khushali 15-02-2019 OTP expire in 5 min As per discussion with Kartik bhai
                    Status = 0,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = UserId
                };
                _customRepository.Add(currentotp);

                if (!String.IsNullOrEmpty(Email))
                {
                    //+++++++++++++++++++++++++++++++++++++++++++++++++++++//
                    // khushali 30-01-2019 for Common Template Method call 
                    TemplateMasterData TemplateData = new TemplateMasterData();
                    CommunicationParamater communicationParamater = new CommunicationParamater();
                    SendEmailRequest request = new SendEmailRequest();
                    communicationParamater.Param1 = Email;
                    communicationParamater.Param2 = numeric.ToString();
                    TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.LoginWithOTP, communicationParamater, enCommunicationServiceType.Email).Result;
                    if (TemplateData != null)
                    {
                        if (TemplateData.IsOnOff == 1)
                        {
                            request.Recepient = Email;
                            request.Body = TemplateData.Content;
                            request.Subject = TemplateData.AdditionalInfo;
                            _pushNotificationsQueue.Enqueue(request);
                        }
                    }
                }
                if (!String.IsNullOrEmpty(Mobile))
                {
                    //+++++++++++++++++++++++++++++++++++++++++++++++++++++//
                    // khushali 30-01-2019 for Common Template Method call 
                    TemplateMasterData TemplateData = new TemplateMasterData();
                    CommunicationParamater communicationParamater = new CommunicationParamater();
                    SendSMSRequest request = new SendSMSRequest();
                    communicationParamater.Param1 = numeric.ToString();
                    TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SMS_VerificationCode, communicationParamater, enCommunicationServiceType.SMS).Result;
                    if (TemplateData != null)
                    {
                        if (TemplateData.IsOnOff == 1)
                        {
                            request.MobileNo = Convert.ToInt64(Mobile);
                            request.Message = TemplateData.Content;
                            _pushSMSQueue.Enqueue(request);
                        }
                    }
                }

                string _Pass1 = alpha.Substring(0, 20);
                string _Pass11 = _Pass1 + numeric.Substring(0, 3);
                string _Pass2 = alpha.Substring(20, 10);
                string _Pass22 = _Pass2 + numeric.Substring(3, 3);
                string _Pass3 = alpha.Substring(30, 28);
                string password = _Pass11 + _Pass22 + _Pass3;

                OtpMasterViewModel model = new OtpMasterViewModel();
                if (currentotp != null)
                {
                    model.UserId = currentotp.UserId;
                    model.RegTypeId = currentotp.RegTypeId;
                    model.OTP = currentotp.OTP;
                    model.ExpirTime = currentotp.ExpirTime;
                    model.Status = currentotp.Status;
                    model.Id = currentotp.Id;
                    model.Password = password;
                    model.appkey = alpha;
                    return model;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AddOtp", "OtpMasterService", ex);
                return null;
            }
        }
        
        public async Task<OtpMasterViewModel> GetOtpData(int Id, int OTPType)
        {
            // khushali 12-04-2019  login with OTP it shows invalid OTP every time even if user entering correct otp (resolve Ticket )
            var otpmaster = _dbContext.OtpMaster.Where(i => i.UserId == Id && i.Status == 0 && i.RegTypeId == OTPType).OrderByDescending(e => e.Id).FirstOrDefault();
            if (otpmaster != null)
            {
                OtpMasterViewModel model = new OtpMasterViewModel();

                model.UserId = otpmaster.UserId;
                model.RegTypeId = otpmaster.RegTypeId;
                model.OTP = otpmaster.OTP;
                model.ExpirTime = otpmaster.ExpirTime;
                model.Id = otpmaster.Id;
                return model;
            }
            else
                return null;
        }

        public void UpdateOtp(long Id, short Status, string Message)
        {
            //var tempdata = _customRepository.GetById(Convert.ToInt16(Id));
            var tempdata = _customRepository.GetById(Id);//ntrivedi 29-03-2019 conversion error in paro live             
            tempdata.SetAsUpdateDate(tempdata.UserId, Message, Status);
            //tempdata.Status = true;
            _customRepository.Update(tempdata);
        }
        public async Task<OtpMasterViewModel> AddOtpForSignupuser(int UserId, string Email = null, string Mobile = null)
        {            
            string OtpValue = string.Empty;
            
            OtpValue = _userService.GenerateRandomOTPWithPassword().ToString();
            string alpha = string.Empty; string numeric = string.Empty;
            foreach (char str in OtpValue)
            {
                if (char.IsDigit(str))
                {
                    if (numeric.Length < 6)
                        numeric += str.ToString();
                    else
                        alpha += str.ToString();
                }
                else
                    alpha += str.ToString();
            }

            int Regtypeid = 0;
            if (!String.IsNullOrEmpty(Email))
            {
                Regtypeid = await _registerTypeService.GetRegisterId(Core.Enums.enRegisterType.Email);
            }
            else if (!String.IsNullOrEmpty(Mobile))
            {
                Regtypeid = await _registerTypeService.GetRegisterId(Core.Enums.enRegisterType.Mobile);
            }

            var currentotp = new OtpMaster
            {
                UserId = UserId,
                RegTypeId = Regtypeid,
                OTP = numeric,
                //ExpirTime = DateTime.UtcNow.AddHours(2),
                ExpirTime = DateTime.UtcNow.AddMinutes(5), // khushali 15-02-2019 OTP expire in 5 min As per discussion with Kartik bhai
                Status = 0,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = UserId
            };
            _customRepository.Add(currentotp);

            OtpMasterViewModel model = new OtpMasterViewModel();
            if (currentotp != null)
            {
                model.UserId = currentotp.UserId;
                model.RegTypeId = currentotp.RegTypeId;
                model.OTP = currentotp.OTP;
                model.ExpirTime = currentotp.ExpirTime;
                model.Status = currentotp.Status;
                model.Id = currentotp.Id;
                // model.Password = password;
                model.appkey = alpha;
                return model;
            }
            else
                return null;
        }
        public void UpdateEmailAndMobileOTP(long id)
        {
            try
            {
                var otpmaster = _dbContext.OtpMaster.Where(i => i.Id == id).LastOrDefault();
                if (otpmaster != null)
                {
                    otpmaster.UpdatedBy = otpmaster.UserId;
                    otpmaster.UpdatedDate = DateTime.UtcNow;
                    _customRepository.Update(otpmaster);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public int GetOTPCountByType(int UserId, int OTPType)
        {
            return _dbContext.OtpMaster.Count(c => c.UserId == UserId && c.RegTypeId == OTPType
                        && c.CreatedDate >= Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-dd 00:00:00.0000000"))
                        && c.CreatedDate >= Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-dd 23:59:59.9999999")));
        }

        public int GetTotalOTPCount(int UserId)
        {
            return _dbContext.OtpMaster.Count(c => c.UserId == UserId
                        && c.CreatedDate >= Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-dd 00:00:00.0000000"))
                        && c.CreatedDate >= Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-dd 23:59:59.9999999")));
        }
    }
}
