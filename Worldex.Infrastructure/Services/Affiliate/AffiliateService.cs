using Worldex.Core.ApiModels;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Entities.Affiliate;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Affiliate;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.Interfaces.User;
using Worldex.Core.Services;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.AccountViewModels.Affiliate;
using Worldex.Infrastructure.BGTask;
using Google.Apis.Services;
using Google.Apis.Urlshortener.v1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

namespace Worldex.Infrastructure.Services.Affiliate
{
    public class AffiliateService : IAffiliateService
    {
        private readonly ICommonRepository<AffiliateUserMaster> _affiliateUserMasterRepository;
        private readonly ICommonRepository<AffiliatePromotionMaster> _affiliatePromotionMasterRepository;
        private readonly IUserService _userService;
        private readonly ICommonRepository<AffiliatePromotionUserTypeMapping> _affiliatePromotionUserTypeMappingRepository;
        private readonly EncyptedDecrypted _encdecAEC;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly ICommonRepository<AffiliateSchemeMaster> _affiliateSchemeMaster;
        private readonly IAffiliateRepository _affiliateRepository;
        private readonly IMessageService _messageService;
        private readonly IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;
        private readonly ICommonRepository<AffiliatePromotionShare> _affiliatePromotionShareRepository;
        private readonly ICommonRepository<AffiliatePromotionLimitConfiguration> _affiliatePromotionLimitConfigurationRepository;
        private readonly ICommonRepository<AffiliateLinkClick> _affiliateLinkClickRepository;

        public AffiliateService(ICommonRepository<AffiliateUserMaster> affiliateUserMasterRepository, ICommonRepository<AffiliatePromotionMaster> affiliatePromotionMasterRepository, IUserService userService, 
            ICommonRepository<AffiliatePromotionUserTypeMapping> affiliatePromotionUserTypeMappingRepository, EncyptedDecrypted encdecAEC, Microsoft.Extensions.Configuration.IConfiguration configuration,
            ICommonRepository<AffiliateSchemeMaster> affiliateSchemeMaster, IAffiliateRepository affiliateRepository, IMessageService messageService, IPushNotificationsQueue<SendSMSRequest> pushSMSQueue, IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue,
            ICommonRepository<AffiliatePromotionShare> affiliatePromotionShareRepository, ICommonRepository<AffiliatePromotionLimitConfiguration> affiliatePromotionLimitConfigurationRepository, ICommonRepository<AffiliateLinkClick> affiliateLinkClickRepository)
        {
            _affiliateUserMasterRepository = affiliateUserMasterRepository;
            _affiliatePromotionMasterRepository = affiliatePromotionMasterRepository;
            _userService = userService;
            _affiliatePromotionUserTypeMappingRepository = affiliatePromotionUserTypeMappingRepository;
            _encdecAEC = encdecAEC;
            _configuration = configuration;
            _affiliateSchemeMaster = affiliateSchemeMaster;
            _affiliateRepository = affiliateRepository;
            _messageService = messageService;
            _pushSMSQueue = pushSMSQueue;
            _pushNotificationsQueue = pushNotificationsQueue;
            _affiliatePromotionShareRepository = affiliatePromotionShareRepository;
            _affiliatePromotionLimitConfigurationRepository = affiliatePromotionLimitConfigurationRepository;
            _affiliateLinkClickRepository = affiliateLinkClickRepository;
        }

        public long AddAffiliateUser(long ParentId, long PromotionType, short UserBit, long SchemeId, long BizUserId)
        {
            try
            {
                var ReferCode = GenerateReferCode(10);
                AffiliateUserMaster affiliateUser = new AffiliateUserMaster()
                {
                    CreatedBy = 1,
                    CreatedDate = Helpers.UTC_To_IST(),
                    ParentId = ParentId,
                    ReferCode = ReferCode,
                    PromotionTypeId = PromotionType,
                    UserBit = UserBit,
                    UserId = BizUserId,
                    SchemeMstId = SchemeId
                };

                var affiliateUSer = _affiliateUserMasterRepository.Add(affiliateUser);

                if(affiliateUser != null)
                {
                    return affiliateUser.Id;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long GetAdminUser()
        {
            try
            {
                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long GetAffiliateParentUser(string ReferCode)
        {
            try
            {
                var ParentUser = _affiliateUserMasterRepository.FindBy(x => x.ReferCode == ReferCode).FirstOrDefault();

                if(ParentUser != null)
                {
                    return ParentUser.Id;
                }
                else
                {
                    return 0;
                }

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<AffiliatePromotionTypeResponse> GetAffiliatePromotionType()
        {
            try
            {
                List<AffiliatePromotionTypeResponse> Response = new List<AffiliatePromotionTypeResponse>();
                var affiliatePromotionList = _affiliatePromotionMasterRepository.GetAllList().Where(x => x.Status == 1).ToList();

                if(affiliatePromotionList.Count > 0)
                {
                    foreach(var affiliatePromotion in affiliatePromotionList)
                    {
                        AffiliatePromotionTypeResponse affiliatePromotionTypeResponse = new AffiliatePromotionTypeResponse();

                        affiliatePromotionTypeResponse.Id = affiliatePromotion.Id;
                        affiliatePromotionTypeResponse.PromotionType = affiliatePromotion.PromotionType;

                        Response.Add(affiliatePromotionTypeResponse);
                    }
                }

                return Response;
            }
            catch(Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //public long AddAffiliatePromotionType(AddAffiliatePromotionTypeRequest Request, ApplicationUser UserData)
        //{
        //    try
        //    {
        //        AffiliatePromotionLinkViewModel affiliatePromotionLinkViewModel = new AffiliatePromotionLinkViewModel();
        //        AffiliatePromotionUserTypeMapping affiliatePromotionUserTypeMapping = new AffiliatePromotionUserTypeMapping();

        //        if(UserData != null)
        //        {
        //            var AffiliateUser = _affiliateUserMasterRepository.FindBy(x => x.UserId == UserData.Id).FirstOrDefault();

        //            if (AffiliateUser != null)
        //            {
        //                var AllPromotionType = GetAffiliatePromotionType();

        //                if (AllPromotionType.Count > 0)
        //                {
        //                    byte[] passwordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());
        //                    foreach (var PromotionType in AllPromotionType)
        //                    {
        //                        var promotionAvailable = Request.PromotionType.Contains(PromotionType.Id);

        //                        var AlreadyPromotion = _affiliatePromotionUserTypeMappingRepository.FindBy(x => x.PromotionTypeId == PromotionType.Id && x.AffiliateUserId == AffiliateUser.Id).FirstOrDefault();

        //                        if(promotionAvailable)
        //                        {
        //                            if(AlreadyPromotion == null)
        //                            {
        //                                affiliatePromotionLinkViewModel = new AffiliatePromotionLinkViewModel();
        //                                affiliatePromotionLinkViewModel.Id = AffiliateUser.Id;
        //                                affiliatePromotionLinkViewModel.Username = UserData.UserName;
        //                                affiliatePromotionLinkViewModel.PromotionType = PromotionType.Id;
        //                                affiliatePromotionLinkViewModel.ReferCode = AffiliateUser.ReferCode;
        //                                affiliatePromotionLinkViewModel.Mobile = UserData.Mobile;
        //                                affiliatePromotionLinkViewModel.CurrentTime = DateTime.UtcNow;
        //                                affiliatePromotionLinkViewModel.Expirytime = DateTime.UtcNow + TimeSpan.FromDays(30);

        //                                string TokenDetail = JsonConvert.SerializeObject(affiliatePromotionLinkViewModel);
        //                                string SubScriptionKey = EncyptedDecrypted.Encrypt(TokenDetail, passwordBytes);


        //                                string ctokenlink = "";
        //                                string sociallink = "";
        //                                string FacebookUrl = _configuration["AffiliateFacebookShare"].ToString();
        //                                string GoogleUrl = _configuration["AffiliateGoogleShare"].ToString();
        //                                string TwitterUrl = _configuration["AffiliateTwitterShare"].ToString();
        //                                if (PromotionType.Id == 6) // Facebook
        //                                {
        //                                    sociallink = FacebookUrl;
        //                                    sociallink += "join on this amazing affiliate program. refer and earn.";
        //                                    sociallink += "&display=popup";
        //                                    sociallink += "&link=";
        //                                }
        //                                else if (PromotionType.Id == 7) // Google +
        //                                {
        //                                    sociallink = GoogleUrl;
        //                                }
        //                                else if (PromotionType.Id == 8) // Twitter
        //                                {
        //                                    sociallink = TwitterUrl;
        //                                    sociallink += "join on this amazing affiliate program. refer and earn.";
        //                                    sociallink += "&url=";
        //                                }

        //                                byte[] plainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKey);
        //                                ctokenlink += _configuration["AffiliatePromotionShare"].ToString() + AffiliateUser.ReferCode;
        //                                ctokenlink += "&tag=" + Convert.ToBase64String(plainTextBytes);

        //                                var shortLink = "";
        //                                if (PromotionType.Id == 2) //SMS
        //                                {
        //                                    //shortLink = GenerateShortifyLink(ctokenlink);
        //                                    shortLink = GenerateShortifyBitlyLink(ctokenlink);
        //                                }

        //                                if (PromotionType.Id == 8) //Twitter
        //                                {
        //                                    //shortLink = GenerateShortifyLink(ctokenlink);
        //                                    ctokenlink = GenerateShortifyBitlyLink(ctokenlink);
        //                                }


        //                                var PromotionLink = sociallink + ctokenlink;
        //                                affiliatePromotionUserTypeMapping = new AffiliatePromotionUserTypeMapping()
        //                                {
        //                                    AffiliateUserId = AffiliateUser.Id,
        //                                    CreatedBy = UserData.Id,
        //                                    CreatedDate = Helpers.UTC_To_IST(),
        //                                    PromotionLink = PromotionLink,
        //                                    DecryptedCode = Convert.ToBase64String(plainTextBytes),
        //                                    PromotionTypeId = PromotionType.Id,
        //                                    Status = Convert.ToInt16(ServiceStatus.Active),
        //                                    ShortLink = shortLink
        //                                };

        //                                _affiliatePromotionUserTypeMappingRepository.Add(affiliatePromotionUserTypeMapping);
        //                            }
        //                            else
        //                            {
        //                                AlreadyPromotion.Status = Convert.ToInt16(ServiceStatus.Active);
        //                                _affiliatePromotionUserTypeMappingRepository.Update(AlreadyPromotion);
        //                            }

        //                        }
        //                        else
        //                        {
        //                            if (AlreadyPromotion != null)
        //                            {
        //                                AlreadyPromotion.Status = Convert.ToInt16(ServiceStatus.InActive);
        //                                _affiliatePromotionUserTypeMappingRepository.Update(AlreadyPromotion);
        //                            }
        //                            else
        //                            {
        //                                affiliatePromotionLinkViewModel = new AffiliatePromotionLinkViewModel();
        //                                affiliatePromotionLinkViewModel.Id = AffiliateUser.Id;
        //                                affiliatePromotionLinkViewModel.Username = UserData.UserName;
        //                                affiliatePromotionLinkViewModel.PromotionType = PromotionType.Id;
        //                                affiliatePromotionLinkViewModel.ReferCode = AffiliateUser.ReferCode;
        //                                affiliatePromotionLinkViewModel.Mobile = UserData.Mobile;
        //                                affiliatePromotionLinkViewModel.CurrentTime = DateTime.UtcNow;
        //                                affiliatePromotionLinkViewModel.Expirytime = DateTime.UtcNow + TimeSpan.FromDays(30);

        //                                string TokenDetail = JsonConvert.SerializeObject(affiliatePromotionLinkViewModel);
        //                                string SubScriptionKey = EncyptedDecrypted.Encrypt(TokenDetail, passwordBytes);


        //                                string ctokenlink = "";
        //                                string sociallink = "";
        //                                string FacebookUrl = _configuration["AffiliateFacebookShare"].ToString();
        //                                string GoogleUrl = _configuration["AffiliateGoogleShare"].ToString();
        //                                string TwitterUrl = _configuration["AffiliateTwitterShare"].ToString();
        //                                if (PromotionType.Id == 6) // Facebook
        //                                {
        //                                    sociallink = FacebookUrl;
        //                                    sociallink += "join on this amazsing affiliate programe. refer and earn.";
        //                                    sociallink += "&display=popup";
        //                                    sociallink += "&link=";
        //                                }
        //                                else if (PromotionType.Id == 7) // Google +
        //                                {
        //                                    sociallink = GoogleUrl;
        //                                }
        //                                else if (PromotionType.Id == 8) // Twitter
        //                                {
        //                                    sociallink = TwitterUrl;
        //                                    sociallink += "join on this amazsing affiliate programe. refer and earn.";
        //                                    sociallink += "&url=";
        //                                }

        //                                byte[] plainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKey);
        //                                ctokenlink += _configuration["AffiliatePromotionShare"].ToString() + AffiliateUser.ReferCode;
        //                                ctokenlink += "&tag=" + Convert.ToBase64String(plainTextBytes);

        //                                var shortLink = "";
        //                                if (PromotionType.Id == 2) //SMS
        //                                {
        //                                    //shortLink = GenerateShortifyLink(ctokenlink);
        //                                    shortLink = GenerateShortifyBitlyLink(ctokenlink);
        //                                }

        //                                if (PromotionType.Id == 8) //SMS
        //                                {
        //                                    ctokenlink = GenerateShortifyBitlyLink(ctokenlink);
        //                                }


        //                                var PromotionLink = sociallink + ctokenlink;
        //                                affiliatePromotionUserTypeMapping = new AffiliatePromotionUserTypeMapping()
        //                                {
        //                                    AffiliateUserId = AffiliateUser.Id,
        //                                    CreatedBy = UserData.Id,
        //                                    CreatedDate = Helpers.UTC_To_IST(),
        //                                    PromotionLink = PromotionLink,
        //                                    DecryptedCode = Convert.ToBase64String(plainTextBytes),
        //                                    PromotionTypeId = PromotionType.Id,
        //                                    Status = Convert.ToInt16(ServiceStatus.InActive),
        //                                    ShortLink = shortLink
        //                                };

        //                                _affiliatePromotionUserTypeMappingRepository.Add(affiliatePromotionUserTypeMapping);
        //                            }
        //                        }
        //                    }
        //                }

        //                var AlreadyPromotion1 = _affiliatePromotionUserTypeMappingRepository.FindBy(x => x.PromotionTypeId == 9 && x.AffiliateUserId == AffiliateUser.Id).FirstOrDefault();

        //                if (AlreadyPromotion1 == null)
        //                {
        //                    //For Affiliate Link
        //                    affiliatePromotionLinkViewModel = new AffiliatePromotionLinkViewModel();
        //                    affiliatePromotionLinkViewModel.Id = AffiliateUser.Id;
        //                    affiliatePromotionLinkViewModel.Username = UserData.UserName;
        //                    affiliatePromotionLinkViewModel.PromotionType = 9;
        //                    affiliatePromotionLinkViewModel.ReferCode = AffiliateUser.ReferCode;
        //                    affiliatePromotionLinkViewModel.Mobile = UserData.Mobile;
        //                    affiliatePromotionLinkViewModel.CurrentTime = DateTime.UtcNow;
        //                    affiliatePromotionLinkViewModel.Expirytime = DateTime.UtcNow + TimeSpan.FromDays(30);

        //                    byte[] passwordBytes1 = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());

        //                    string TokenDetail1 = JsonConvert.SerializeObject(affiliatePromotionLinkViewModel);
        //                    string SubScriptionKey1 = EncyptedDecrypted.Encrypt(TokenDetail1, passwordBytes1);

        //                    string ctokenlink1 = "";
        //                    byte[] plainTextBytes1 = Encoding.UTF8.GetBytes(SubScriptionKey1);
        //                    ctokenlink1 += _configuration["AffiliatePromotionShare"].ToString() + AffiliateUser.ReferCode;
        //                    ctokenlink1 += "&tag=" + Convert.ToBase64String(plainTextBytes1);


        //                    affiliatePromotionUserTypeMapping = new AffiliatePromotionUserTypeMapping()
        //                    {
        //                        AffiliateUserId = AffiliateUser.Id,
        //                        CreatedBy = UserData.Id,
        //                        CreatedDate = Helpers.UTC_To_IST(),
        //                        PromotionLink = ctokenlink1,
        //                        DecryptedCode = Convert.ToBase64String(plainTextBytes1),
        //                        PromotionTypeId = 9,
        //                        Status = Convert.ToInt16(ServiceStatus.Active),
        //                        ShortLink = ""
        //                    };

        //                    _affiliatePromotionUserTypeMappingRepository.Add(affiliatePromotionUserTypeMapping);
        //                }
        //                return 1;
        //            }
        //            else
        //            {
        //                return 2; // User is not as Affiliate User
        //            }
        //        }
        //        else
        //        {
        //            return 0;
        //        }               
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}

        public long AddAffiliatePromotionType(AddAffiliatePromotionTypeRequest Request, ApplicationUser UserData)
        {
            try
            {
                AffiliatePromotionLinkViewModel affiliatePromotionLinkViewModel = new AffiliatePromotionLinkViewModel();
                AffiliatePromotionUserTypeMapping affiliatePromotionUserTypeMapping = new AffiliatePromotionUserTypeMapping();

                if (UserData != null)
                {
                    var AffiliateUser = _affiliateUserMasterRepository.FindBy(x => x.UserId == UserData.Id).FirstOrDefault();

                    if (AffiliateUser != null)
                    {
                        var AllPromotionType = GetAffiliatePromotionType();

                        if (AllPromotionType.Count > 0)
                        {
                            byte[] passwordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());
                            foreach (var PromotionType in AllPromotionType)
                            {
                                var promotionAvailable = Request.PromotionType.Contains(PromotionType.Id);

                                var AlreadyPromotion = _affiliatePromotionUserTypeMappingRepository.FindBy(x => x.PromotionTypeId == PromotionType.Id && x.AffiliateUserId == AffiliateUser.Id).FirstOrDefault();

                                if (promotionAvailable)
                                {
                                    if (AlreadyPromotion == null)
                                    {
                                        affiliatePromotionLinkViewModel = new AffiliatePromotionLinkViewModel();
                                        affiliatePromotionLinkViewModel.Id = AffiliateUser.Id;
                                        affiliatePromotionLinkViewModel.Username = UserData.UserName;
                                        affiliatePromotionLinkViewModel.PromotionType = PromotionType.Id;
                                        affiliatePromotionLinkViewModel.ReferCode = AffiliateUser.ReferCode;
                                        affiliatePromotionLinkViewModel.Mobile = UserData.Mobile;
                                        affiliatePromotionLinkViewModel.CurrentTime = DateTime.UtcNow;
                                        affiliatePromotionLinkViewModel.Expirytime = DateTime.UtcNow + TimeSpan.FromDays(30);

                                        string TokenDetail = JsonConvert.SerializeObject(affiliatePromotionLinkViewModel);
                                        string SubScriptionKey = EncyptedDecrypted.Encrypt(TokenDetail, passwordBytes);


                                        string ctokenlink = "";
                                        string sociallink = "";
                                        string FBsociallink = "";
                                        string FacebookUrl = _configuration["AffiliateFacebookShare"].ToString();
                                        string GoogleUrl = _configuration["AffiliateGoogleShare"].ToString();
                                        string TwitterUrl = _configuration["AffiliateTwitterShare"].ToString();
                                        if (PromotionType.Id == 6) // Facebook
                                        {
                                            sociallink = FacebookUrl;
                                            sociallink += "join on this amazing affiliate program. refer and earn.";
                                            sociallink += "&display=popup";
                                            sociallink += "&link=";
                                        }
                                        else if (PromotionType.Id == 7) // Google +
                                        {
                                            sociallink = GoogleUrl;
                                        }
                                        else if (PromotionType.Id == 8) // Twitter
                                        {
                                            sociallink = TwitterUrl;
                                            sociallink += "join on this amazing affiliate program. refer and earn.";
                                            sociallink += "&url=";
                                        }

                                        byte[] plainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKey);
                                        ctokenlink += _configuration["AffiliatePromotionShare"].ToString() + AffiliateUser.ReferCode;
                                        ctokenlink += "&tag=" + Convert.ToBase64String(plainTextBytes);

                                        var shortLink = "";
                                        if (PromotionType.Id == 2) //SMS
                                        {
                                            //shortLink = GenerateShortifyLink(ctokenlink);
                                            shortLink = GenerateShortifyBitlyLink(ctokenlink);
                                        }

                                        if (PromotionType.Id == 8 || PromotionType.Id == 6)  //Twitter,Facebook
                                        {
                                            //shortLink = GenerateShortifyLink(ctokenlink);
                                            ctokenlink = GenerateShortifyBitlyLink(ctokenlink);
                                        }


                                        var PromotionLink = sociallink + ctokenlink;
                                        affiliatePromotionUserTypeMapping = new AffiliatePromotionUserTypeMapping()
                                        {
                                            AffiliateUserId = AffiliateUser.Id,
                                            CreatedBy = UserData.Id,
                                            CreatedDate = Helpers.UTC_To_IST(),
                                            PromotionLink = PromotionLink,
                                            DecryptedCode = Convert.ToBase64String(plainTextBytes),
                                            PromotionTypeId = PromotionType.Id,
                                            Status = Convert.ToInt16(ServiceStatus.Active),
                                            ShortLink = shortLink
                                        };

                                        _affiliatePromotionUserTypeMappingRepository.Add(affiliatePromotionUserTypeMapping);
                                    }
                                    else
                                    {
                                        AlreadyPromotion.Status = Convert.ToInt16(ServiceStatus.Active);
                                        _affiliatePromotionUserTypeMappingRepository.Update(AlreadyPromotion);
                                    }

                                }
                                else
                                {
                                    if (AlreadyPromotion != null)
                                    {
                                        AlreadyPromotion.Status = Convert.ToInt16(ServiceStatus.InActive);
                                        _affiliatePromotionUserTypeMappingRepository.Update(AlreadyPromotion);
                                    }
                                    else
                                    {
                                        affiliatePromotionLinkViewModel = new AffiliatePromotionLinkViewModel();
                                        affiliatePromotionLinkViewModel.Id = AffiliateUser.Id;
                                        affiliatePromotionLinkViewModel.Username = UserData.UserName;
                                        affiliatePromotionLinkViewModel.PromotionType = PromotionType.Id;
                                        affiliatePromotionLinkViewModel.ReferCode = AffiliateUser.ReferCode;
                                        affiliatePromotionLinkViewModel.Mobile = UserData.Mobile;
                                        affiliatePromotionLinkViewModel.CurrentTime = DateTime.UtcNow;
                                        affiliatePromotionLinkViewModel.Expirytime = DateTime.UtcNow + TimeSpan.FromDays(30);

                                        string TokenDetail = JsonConvert.SerializeObject(affiliatePromotionLinkViewModel);
                                        string SubScriptionKey = EncyptedDecrypted.Encrypt(TokenDetail, passwordBytes);


                                        string ctokenlink = "";
                                        string sociallink = "";
                                        string FacebookUrl = _configuration["AffiliateFacebookShare"].ToString();
                                        string GoogleUrl = _configuration["AffiliateGoogleShare"].ToString();
                                        string TwitterUrl = _configuration["AffiliateTwitterShare"].ToString();
                                        if (PromotionType.Id == 6) // Facebook
                                        {
                                            sociallink = FacebookUrl;
                                            sociallink += "join on this amazsing affiliate programe. refer and earn.";
                                            sociallink += "&display=popup";
                                            sociallink += "&link=";
                                        }
                                        else if (PromotionType.Id == 7) // Google +
                                        {
                                            sociallink = GoogleUrl;
                                        }
                                        else if (PromotionType.Id == 8) // Twitter
                                        {
                                            sociallink = TwitterUrl;
                                            sociallink += "join on this amazsing affiliate programe. refer and earn.";
                                            sociallink += "&url=";
                                        }

                                        byte[] plainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKey);
                                        ctokenlink += _configuration["AffiliatePromotionShare"].ToString() + AffiliateUser.ReferCode;
                                        ctokenlink += "&tag=" + Convert.ToBase64String(plainTextBytes);

                                        var shortLink = "";
                                        if (PromotionType.Id == 2) //SMS
                                        {
                                            //shortLink = GenerateShortifyLink(ctokenlink);
                                            shortLink = GenerateShortifyBitlyLink(ctokenlink);
                                        }

                                        if (PromotionType.Id == 8 || PromotionType.Id == 6) //twitter ,facebook
                                        {
                                            ctokenlink = GenerateShortifyBitlyLink(ctokenlink);
                                        }


                                        var PromotionLink = sociallink + ctokenlink;
                                        affiliatePromotionUserTypeMapping = new AffiliatePromotionUserTypeMapping()
                                        {
                                            AffiliateUserId = AffiliateUser.Id,
                                            CreatedBy = UserData.Id,
                                            CreatedDate = Helpers.UTC_To_IST(),
                                            PromotionLink = PromotionLink,
                                            DecryptedCode = Convert.ToBase64String(plainTextBytes),
                                            PromotionTypeId = PromotionType.Id,
                                            Status = Convert.ToInt16(ServiceStatus.InActive),
                                            ShortLink = shortLink
                                        };

                                        _affiliatePromotionUserTypeMappingRepository.Add(affiliatePromotionUserTypeMapping);
                                    }
                                }
                            }
                        }

                        var AlreadyPromotion1 = _affiliatePromotionUserTypeMappingRepository.FindBy(x => x.PromotionTypeId == 9 && x.AffiliateUserId == AffiliateUser.Id).FirstOrDefault();

                        if (AlreadyPromotion1 == null)
                        {
                            //For Affiliate Link
                            affiliatePromotionLinkViewModel = new AffiliatePromotionLinkViewModel();
                            affiliatePromotionLinkViewModel.Id = AffiliateUser.Id;
                            affiliatePromotionLinkViewModel.Username = UserData.UserName;
                            affiliatePromotionLinkViewModel.PromotionType = 9;
                            affiliatePromotionLinkViewModel.ReferCode = AffiliateUser.ReferCode;
                            affiliatePromotionLinkViewModel.Mobile = UserData.Mobile;
                            affiliatePromotionLinkViewModel.CurrentTime = DateTime.UtcNow;
                            affiliatePromotionLinkViewModel.Expirytime = DateTime.UtcNow + TimeSpan.FromDays(30);

                            byte[] passwordBytes1 = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());

                            string TokenDetail1 = JsonConvert.SerializeObject(affiliatePromotionLinkViewModel);
                            string SubScriptionKey1 = EncyptedDecrypted.Encrypt(TokenDetail1, passwordBytes1);

                            string ctokenlink1 = "";
                            byte[] plainTextBytes1 = Encoding.UTF8.GetBytes(SubScriptionKey1);
                            ctokenlink1 += _configuration["AffiliatePromotionShare"].ToString() + AffiliateUser.ReferCode;
                            ctokenlink1 += "&tag=" + Convert.ToBase64String(plainTextBytes1);


                            affiliatePromotionUserTypeMapping = new AffiliatePromotionUserTypeMapping()
                            {
                                AffiliateUserId = AffiliateUser.Id,
                                CreatedBy = UserData.Id,
                                CreatedDate = Helpers.UTC_To_IST(),
                                PromotionLink = ctokenlink1,
                                DecryptedCode = Convert.ToBase64String(plainTextBytes1),
                                PromotionTypeId = 9,
                                Status = Convert.ToInt16(ServiceStatus.Active),
                                ShortLink = ""
                            };

                            _affiliatePromotionUserTypeMappingRepository.Add(affiliatePromotionUserTypeMapping);
                        }
                        return 1;
                    }
                    else
                    {
                        return 2; // User is not as Affiliate User
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }


        public void ActiveAffiliateAccount(long UserId)
        {
            try
            {
                var UserData = _userService.GetUserDataById(UserId);

                if (UserData != null)
                {
                    var AffiliateUser = _affiliateUserMasterRepository.FindBy(x => x.UserId == UserData.Id).FirstOrDefault();

                    if (AffiliateUser != null)
                    {
                        AffiliateUser.MakeAffiliateUserActive();
                        _affiliateUserMasterRepository.Update(AffiliateUser);
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }
        }

        public string GenerateReferCode(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public int GeAffiliateSchemeType(long SchemeId)
        {
            try
            {
                var SchemeData = _affiliateSchemeMaster.GetActiveById(SchemeId);

                if (SchemeData != null)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }
        }

        public List<AffiliateSchemeTypeResponseData> GetAffiliateSchemeType()
        {
            try
            {
                List<AffiliateSchemeTypeResponseData> Response = new List<AffiliateSchemeTypeResponseData>();
                var affiliateSchemeList = _affiliateSchemeMaster.GetAllList().Where(x => x.Status == 1).ToList();

                if (affiliateSchemeList.Count > 0)
                {
                    foreach (var affiliateScheme in affiliateSchemeList)
                    {
                        AffiliateSchemeTypeResponseData affiliateSchemeTypeResponse = new AffiliateSchemeTypeResponseData();

                        affiliateSchemeTypeResponse.Id = affiliateScheme.Id;
                        affiliateSchemeTypeResponse.Value = affiliateScheme.SchemeType + " - " + affiliateScheme.SchemeName;

                        Response.Add(affiliateSchemeTypeResponse);
                    }
                }

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<AffiliateSchemeAvailableSchemeData> GetDetailAffiliateSchemeType()
        {
            try
            {
                List<AffiliateSchemeAvailableSchemeData> Response = new List<AffiliateSchemeAvailableSchemeData>();
                var affiliateSchemeList = _affiliateRepository.GetDetailAffiliateSchemeType();

                if(affiliateSchemeList.Count > 0)
                {
                    var PlanData = affiliateSchemeList.ToList().GroupBy(x => x.PlanType);
                    
                    foreach(var Scheme in PlanData)
                    {
                        AffiliateSchemeAvailableSchemeData AvailableSchemeList = new AffiliateSchemeAvailableSchemeData();
                        AvailableSchemeList.AvailableScheme = new List<AffiliateSchemeDetailData>();

                        var AvailableData = Scheme.ToList().GroupBy(x => x.SchemeDetailName);

                        foreach (var SubScheme in AvailableData)
                        {
                            
                            AffiliateSchemeDetailData affiliateSchemeDetailData = new AffiliateSchemeDetailData();

                            int i = 0;
                            string ExtraDetail = "";
                            List<String> SchemeDetail = new List<String>();         
                            foreach (var AvailScheme in SubScheme)
                            {
                                AvailableSchemeList.Id = AvailScheme.Id;
                                AvailableSchemeList.Name = AvailScheme.SchemeName;
                                AvailableSchemeList.Value = AvailScheme.PlanType;
                                affiliateSchemeDetailData.SchemeName = AvailScheme.SchemeDetailName;
                                SchemeDetail.Add(AvailScheme.PlanDetail);
                                ExtraDetail = AvailScheme.ExtraDetail;
                                i++;
                            }
                            affiliateSchemeDetailData.SchemeDetail = SchemeDetail;
                            SchemeDetail.Add(ExtraDetail);
                            AvailableSchemeList.AvailableScheme.Add(affiliateSchemeDetailData);
                        }
                        Response.Add(AvailableSchemeList);
                    }
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public dynamic GetAffiliatePromotionLink(ApplicationUser User)
        {
            try
            {
                var AffiliateUser = _affiliateUserMasterRepository.FindBy(x => x.UserId == User.Id).FirstOrDefault();
                if(AffiliateUser != null)
                {
                    var AvailablePromotion = _affiliateRepository.GetAffiliatePromotionLink(AffiliateUser.Id);

                    if (AvailablePromotion.Count > 0)
                    {
                        
                        var columns = new Dictionary<string, AffiliateAvailablePromotionLinkData>();

                        foreach(var Promotion in AvailablePromotion)
                        {
                            AffiliateAvailablePromotionLinkData PromotionData = new AffiliateAvailablePromotionLinkData();
                            PromotionData.Status = Promotion.PromotionStatus;
                            PromotionData.PromotionLink = Promotion.PromotionLink;

                            columns.Add(Promotion.PromotionType, PromotionData);
                        }

                        return columns;
                    }
                    return null;
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public int SendAffiliateEmail(SendAffiliateEmailRequest Request,ApplicationUser User)
        {
            try
            {
                var AffiliateUser = _affiliateUserMasterRepository.FindBy(x => x.UserId == User.Id).FirstOrDefault();

                if (AffiliateUser != null)
                {
                    var PromotionLink = _affiliatePromotionUserTypeMappingRepository.FindBy(x => x.PromotionTypeId == 1 && x.AffiliateUserId == AffiliateUser.Id).FirstOrDefault();

                    if (PromotionLink != null)
                    {
                        var PromotionLimit = _affiliatePromotionLimitConfigurationRepository.GetSingle(x => x.PromotionType == PromotionLink.PromotionTypeId && x.Status == 1);

                        for (int i = 0; i < Request.EmailList.Count(); i++)
                        {
                            var SendPromotionLimit = _affiliateRepository.GetAffiliatePromotionLimitCount(AffiliateUser.Id, PromotionLink.PromotionTypeId);
                            
                            if(SendPromotionLimit.HourlyCount >= PromotionLimit.HourlyLimit)
                            {
                                return 4;
                            }
                            else if (SendPromotionLimit.DailyCount >= PromotionLimit.DailyLimit)
                            {
                                return 5;
                            }

                            SendEmailRequest EmailRequest = new SendEmailRequest();
                            TemplateMasterData EmailData = new TemplateMasterData();
                            CommunicationParamater communicationParamater = new CommunicationParamater();
                            communicationParamater.Param1 = User.FirstName + " " + User.LastName;
                            communicationParamater.Param2 = PromotionLink.PromotionLink;

                            if (!string.IsNullOrEmpty(User.Email))
                            {
                                //communicationParamater.Param1 = User.UserName.ToLower() + "";

                                EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_AffiliatePromotion, communicationParamater, enCommunicationServiceType.Email).Result;
                                if (EmailData != null)
                                {
                                    EmailRequest.Body = EmailData.Content;
                                    EmailRequest.Subject = EmailData.AdditionalInfo;
                                    EmailRequest.Recepient = Request.EmailList[i].ToString();
                                    EmailRequest.EmailType = 0;
                                    _pushNotificationsQueue.Enqueue(EmailRequest);

                                    //Add Data Into AffiliatePromotion Share
                                    AffiliatePromotionShare EmailPromotion = new AffiliatePromotionShare()
                                    {
                                        AffiliateUserId = AffiliateUser.Id,
                                        CreatedBy = User.Id,
                                        PromotionDetail = Request.EmailList[i].ToString(),
                                        PromotionTypeId = 1,
                                        Status = 1,
                                        CreatedDate = Helpers.UTC_To_IST()
                                    };
                                    _affiliatePromotionShareRepository.Add(EmailPromotion);
                                }
                            }
                        }
                    }
                    else
                    {
                        return 2;
                    }
                }
                else
                {
                    return 3;
                }
                return 1;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public int SendAffiliateSMS(SendAffiliateSMSRequest Request,ApplicationUser User)
        {
            try
            {
                var AffiliateUser = _affiliateUserMasterRepository.FindBy(x => x.UserId == User.Id).FirstOrDefault();

                if (AffiliateUser != null)
                {
                    var PromotionLink = _affiliatePromotionUserTypeMappingRepository.FindBy(x => x.PromotionTypeId == 2 && x.AffiliateUserId == AffiliateUser.Id).FirstOrDefault();

                    if (PromotionLink != null)
                    {
                        var PromotionLimit = _affiliatePromotionLimitConfigurationRepository.GetSingle(x => x.PromotionType == PromotionLink.PromotionTypeId && x.Status == 1);

                        for (int i = 0; i < Request.MobileNumberList.Count(); i++)
                        {
                            var SendPromotionLimit = _affiliateRepository.GetAffiliatePromotionLimitCount(AffiliateUser.Id, PromotionLink.PromotionTypeId);

                            if (SendPromotionLimit.HourlyCount >= PromotionLimit.HourlyLimit)
                            {
                                return 4;
                            }
                            else if (SendPromotionLimit.DailyCount >= PromotionLimit.DailyLimit)
                            {
                                return 5;
                            }

                            SendSMSRequest SendSMSRequestObj = new SendSMSRequest();
                            TemplateMasterData SmsData = new TemplateMasterData();

                            CommunicationParamater communicationParamater = new CommunicationParamater();
                            communicationParamater.Param1 = User.FirstName + " " + User.LastName;
                            communicationParamater.Param2 = PromotionLink.ShortLink;

                            SmsData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SMS_AffiliatePromotion, communicationParamater, enCommunicationServiceType.SMS).Result;

                            if (SmsData != null)
                            {
                                if (SmsData.IsOnOff == 1)
                                {
                                    SendSMSRequestObj.Message = SmsData.Content;
                                    SendSMSRequestObj.MobileNo = Convert.ToInt64(Request.MobileNumberList[i]);
                                    _pushSMSQueue.Enqueue(SendSMSRequestObj);

                                    //Add Data Into AffiliatePromotion Share
                                    AffiliatePromotionShare SMSPromotion = new AffiliatePromotionShare()
                                    {
                                        AffiliateUserId = AffiliateUser.Id,
                                        CreatedBy = User.Id,
                                        PromotionDetail = Request.MobileNumberList[i].ToString(),
                                        PromotionTypeId = 2,
                                        Status = 1,
                                        CreatedDate = Helpers.UTC_To_IST()
                                    };
                                    _affiliatePromotionShareRepository.Add(SMSPromotion);
                                }
                            }
                        }
                    }
                    else
                    {
                        return 3;
                    }
                }
                else
                {
                    return 2;
                }
                return 1;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //public int AddPromotionLinkClick(AddPromotionLinkClickRequest Request, string PassData)
        //{
        //    try
        //    {
        //        byte[] DecpasswordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());
        //        string DecryptToken = "";
        //        AffiliatePromotionLinkViewModel dmodel = new AffiliatePromotionLinkViewModel();
        //        // Set Try Catch Block By Rushali (26-12-2019)
        //        try
        //        {
        //            var bytes = Convert.FromBase64String(PassData);
        //            var encodedString = Encoding.UTF8.GetString(bytes);
        //            DecryptToken = EncyptedDecrypted.Decrypt(encodedString, DecpasswordBytes);
        //            dmodel = JsonConvert.DeserializeObject<AffiliatePromotionLinkViewModel>(DecryptToken);

        //        }
        //        catch (Exception e)
        //        {
        //            return 0;
        //        }
        //        //check refercode condition by Rushali (25-12-2019)
        //        if (dmodel.ReferCode == null)
        //        {
        //            return 0;
        //        }
        //        if (dmodel != null)
        //        {
        //            var AffiliateLink = _affiliateLinkClickRepository.GetSingle(x => x.AffiliateUserId == dmodel.Id && x.IPAddress == Request.IPAddress && x.PromotionTypeId == dmodel.PromotionType);

        //            if(AffiliateLink == null)
        //            {
        //                AffiliateLinkClick _AffiliateLinkObj = new AffiliateLinkClick()
        //                {
        //                    IPAddress = Request.IPAddress,
        //                    AffiliateUserId = dmodel.Id,
        //                    LinkDetail = PassData,
        //                    PromotionTypeId = dmodel.PromotionType,
        //                    Status = 1,
        //                    CreatedBy = dmodel.Id,
        //                    CreatedDate = Helpers.UTC_To_IST()
        //                };

        //               var data=  _affiliateLinkClickRepository.Add(_AffiliateLinkObj);
        //               if (data != null)
        //               {
        //                   return 1;
        //               }

        //               return 0;
        //            }
        //        }
        //        return 0;
        //    }
        //    catch(Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}


        public int AddPromotionLinkClick(AddPromotionLinkClickRequest Request, string PassData)
        {
            try
            {
                byte[] DecpasswordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());
                string DecryptToken = "";
                AffiliatePromotionLinkViewModel dmodel = new AffiliatePromotionLinkViewModel();

                // Set Try Catch Block By Bhavik (26-12-2019)
                try
                {
                    HelperForLog.WriteLogIntoFile("Link click", "affiliate", "Ipaddress: " + Request.IPAddress);
                    var bytes = Convert.FromBase64String(PassData);
                    var encodedString = Encoding.UTF8.GetString(bytes);
                    DecryptToken = EncyptedDecrypted.Decrypt(encodedString, DecpasswordBytes);
                    dmodel = JsonConvert.DeserializeObject<AffiliatePromotionLinkViewModel>(DecryptToken);

                }
                catch (Exception ex)
                {
                    HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);

                    HelperForLog.WriteLogIntoFile("Link click", "affiliate", "Ex first: ## " + ex.ToString());
                    return 0;
                }


                //check refercode condition by bhavik Date 24/12/19
                if (dmodel.ReferCode == null)
                {

                    HelperForLog.WriteLogIntoFile("Link click", "affiliate", "Not get refer cpde ");
                    return 0;
                }
                if (dmodel != null)
                {
                    var AffiliateLink = _affiliateLinkClickRepository.GetSingle(x => x.AffiliateUserId == dmodel.Id && x.IPAddress == Request.IPAddress && x.PromotionTypeId == dmodel.PromotionType);

                    if (AffiliateLink == null)
                    {
                        AffiliateLinkClick _AffiliateLinkObj = new AffiliateLinkClick()
                        {
                            IPAddress = Request.IPAddress,
                            AffiliateUserId = dmodel.Id,
                            LinkDetail = PassData,
                            PromotionTypeId = dmodel.PromotionType,
                            Status = 1,
                            CreatedBy = dmodel.Id,
                            CreatedDate = Helpers.UTC_To_IST()
                        };

                        var data = _affiliateLinkClickRepository.Add(_AffiliateLinkObj);
                        //check data add or not condition by bhavik Date 24/12/19
                        if (data != null)
                        {
                            return 1;
                        }
                        HelperForLog.WriteLogIntoFile("Link click", "affiliate", "Not get refer object ");

                        return 0;
                    }
                }
                HelperForLog.WriteLogIntoFile("Link click", "affiliate", "Not decrypt ");

                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public string GenerateShortifyLink(string OriginalUrl)
        {
            try
            {
                string APIKey = _configuration["AffiliateShortifyAPIKey"].ToString();

                UrlshortenerService service = new UrlshortenerService(new BaseClientService.Initializer()
                {
                    ApiKey = APIKey
                });

                var m = new Google.Apis.Urlshortener.v1.Data.Url();
                m.LongUrl = OriginalUrl;
                var data = service.Url.Insert(m).Execute().Id;

                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return "";
            }
        }

        public string GenerateUnShortifyLink(string url)
        {
            try
            {
                string APIKey = _configuration["AffiliateShortifyAPIKey"].ToString();

                UrlshortenerService service = new UrlshortenerService(new BaseClientService.Initializer()
                {
                    ApiKey = APIKey
                });

                var data = service.Url.Get(url).Execute().LongUrl;

                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return "";
            }
            
        }

        public string GenerateShortifyBitlyLink(string OriginalUrl)
        {
            try
            {
                string APIKey = _configuration["AffiliateShortifyBitlyAPIKey"].ToString();
                string UserName = _configuration["AffiliateShortifyBitlyUser"].ToString();

                StringBuilder uri = new StringBuilder("http://api.bit.ly/shorten?");

                uri.Append("version=2.0.1");

                uri.Append("&format=xml");
                uri.Append("&longUrl=");
                uri.Append(HttpUtility.UrlEncode(OriginalUrl));
                uri.Append("&login=");
                uri.Append(HttpUtility.UrlEncode(UserName));
                uri.Append("&apiKey=");
                uri.Append(HttpUtility.UrlEncode(APIKey));

                HttpWebRequest request = WebRequest.Create(uri.ToString()) as HttpWebRequest;
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ServicePoint.Expect100Continue = false;
                request.ContentLength = 0;
                WebResponse objResponse = request.GetResponse();
                XmlDocument objXML = new XmlDocument();
                objXML.Load(objResponse.GetResponseStream());

                XmlNode nShortUrl = objXML.SelectSingleNode("//shortUrl");

                return nShortUrl.InnerText;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return "";
            }
        }

        //2019-3-15
        #region Count methods

        public AffiliateDashboardCount GetAffiliateDashboardCount(long UserId)
        {
            try
            {
                AffiliateDashboardCount Response = new AffiliateDashboardCount();

                //get the all count
                var Data = _affiliateRepository.GetAffiliateDashboardCount(UserId);

                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public GetAffiateUserRegisteredResponse GetAffiateUserRegistered(long UserId, string FromDate, string ToDate, int Status, int SchemeType, long ParentId, string SCondition, int PageNo, int PageSize)
        {
            try
            {
                long TotalCount = 0;
                int PageSize1 = 0;
                long TotalPages = 0;

                GetAffiateUserRegisteredResponse Response = new GetAffiateUserRegisteredResponse();

                var _Res = _affiliateRepository.GetAffiateUserRegistered(UserId,FromDate, ToDate, Status, SchemeType, ParentId, SCondition, PageSize, PageNo, ref TotalCount, ref PageSize1, ref TotalPages);

                if (_Res.Count > 0)
                {
                    Response.Response = _Res;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Response.Response = new List<GetAffiateUserRegisteredData>();
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoDataFound;
                    Response.ReturnMsg = "No Data Found";
                }

                Response.TotalPages = TotalPages;
                Response.TotalCount = TotalCount;
                Response.PageNo = PageNo;
                Response.PageSize = PageSize1;

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public GetReferralLinkClickResponse GetReferralLinkClick(string FromDate, string ToDate, long UserId, string SCondition, int PageNo, int PageSize)
        {
            try
            {
                long TotalCount = 0;
                int PageSize1 = 0;
                long TotalPages = 0;

                GetReferralLinkClickResponse Response = new GetReferralLinkClickResponse();

                var _Res = _affiliateRepository.GetReferralLinkClick(FromDate, ToDate, UserId, SCondition, PageSize, PageNo, ref TotalCount, ref PageSize1, ref TotalPages);

                if (_Res.Count > 0)
                {
                    Response.Response = _Res;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Response.Response = new List<GetReferralLinkClickData>();
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoDataFound;
                    Response.ReturnMsg = "No Data Found";
                }

                Response.TotalPages = TotalPages;
                Response.TotalCount = TotalCount;
                Response.PageNo = PageNo;
                Response.PageSize = PageSize1;

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public GetFacebookLinkClickResponse GetFacebookLinkClick(string FromDate, string ToDate, long UserId, string SCondition, int PageNo, int PageSize)
        {
            try
            {
                long TotalCount = 0;
                int PageSize1 = 0;
                long TotalPages = 0;

                GetFacebookLinkClickResponse Response = new GetFacebookLinkClickResponse();

                var _Res = _affiliateRepository.GetFacebookLinkClick(FromDate, ToDate, UserId, SCondition, PageSize, PageNo, ref TotalCount, ref PageSize1, ref TotalPages);

                if (_Res.Count > 0)
                {
                    Response.Response = _Res;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Response.Response = new List<GetFacebookLinkClickData>();
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoDataFound;
                    Response.ReturnMsg = "No Data Found";
                }

                Response.TotalPages = TotalPages;
                Response.TotalCount = TotalCount;
                Response.PageNo = PageNo;
                Response.PageSize = PageSize1;

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public GetTwitterLinkClickResponse GetTwitterLinkClick(string FromDate, string ToDate, long UserId, string SCondition, int PageNo, int PageSize)
        {
            try
            {
                long TotalCount = 0;
                int PageSize1 = 0;
                long TotalPages = 0;

                GetTwitterLinkClickResponse Response = new GetTwitterLinkClickResponse();

                var _Res = _affiliateRepository.GetTwitterLinkClick(FromDate, ToDate, UserId, SCondition, PageSize, PageNo, ref TotalCount, ref PageSize1, ref TotalPages);

                if (_Res.Count > 0)
                {
                    Response.Response = _Res;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Response.Response = new List<GetTwitterLinkClickData>();
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoDataFound;
                    Response.ReturnMsg = "No Data Found";
                }

                Response.TotalPages = TotalPages;
                Response.TotalCount = TotalCount;
                Response.PageNo = PageNo;
                Response.PageSize = PageSize1;

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public GetEmailSentResponse GetEmailSent(string FromDate, string ToDate, long UserId, string SCondition, int PageNo, int PageSize)
        {
            try
            {
                long TotalCount = 0;
                int PageSize1 = 0;
                long TotalPages = 0;

                GetEmailSentResponse Response = new GetEmailSentResponse();

                var _Res = _affiliateRepository.GetEmailSent(FromDate, ToDate, UserId, SCondition, PageSize, PageNo, ref TotalCount, ref PageSize1, ref TotalPages);

                if (_Res.Count > 0)
                {
                    Response.Response = _Res;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Response.Response = new List<GetEmailSentData>();
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoDataFound;
                    Response.ReturnMsg = "No Data Found";
                }

                Response.TotalPages = TotalPages;
                Response.TotalCount = TotalCount;
                Response.PageNo = PageNo;
                Response.PageSize = PageSize1;

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public GetSMSSentResponse GetSMSSent(string FromDate, string ToDate, long UserId, string SCondition, int PageNo, int PageSize)
        {
            try
            {
                long TotalCount = 0;
                int PageSize1 = 0;
                long TotalPages = 0;

                GetSMSSentResponse Response = new GetSMSSentResponse();

                var _Res = _affiliateRepository.GetSMSSent(FromDate, ToDate, UserId, SCondition, PageSize, PageNo, ref TotalCount, ref PageSize1, ref TotalPages);

                if (_Res.Count > 0)
                {
                    Response.Response = _Res;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Response.Response = new List<GetSMSSentData>();
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoDataFound;
                    Response.ReturnMsg = "No Data Found";
                }

                Response.TotalPages = TotalPages;
                Response.TotalCount = TotalCount;
                Response.PageNo = PageNo;
                Response.PageSize = PageSize1;

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetAllAffiliateUserData> GetAllAffiliateUser()
        {
            try
            {
                List<GetAllAffiliateUserData> Response = new List<GetAllAffiliateUserData>();
                Response = _affiliateRepository.GetAllAffiliateUser();

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool IsValidDateFormate(string date)
        {
            try
            {
                DateTime dt = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region History Report

        public ListAffiliateCommissionHistoryReport AffiliateCommissionHistoryReport(int PageNo, int PageSize, DateTime? FromDate, DateTime? ToDate, long? TrnUserId, long? AffiliateUserId, long? SchemeMappingId, long? TrnRefNo)
        {
            try
            {
                ListAffiliateCommissionHistoryReport Resp = new ListAffiliateCommissionHistoryReport();
                Resp.PageNo = PageNo;
                Resp.PageSize = PageSize;
                int TotalCount = 0;
                var data = _affiliateRepository.AffiliateCommissionHistoryReport(PageNo + 1, PageSize, FromDate, ToDate, TrnUserId, AffiliateUserId, SchemeMappingId, TrnRefNo, ref TotalCount);
                if (data.Count() == 0)
                {
                    Resp.Data = new List<AffiliateCommissionHistoryReport>();
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    return Resp;
                }
                Resp.Data = data;
                Resp.TotalCount = TotalCount;
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ErrorCode = enErrorCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        public ListInviteFrdClaas GetAffiliateInviteFrieds(long UserId)
        {
            try
            {
                ListInviteFrdClaas Resp = new ListInviteFrdClaas();
                var data = _affiliateRepository.GetAffiliateInviteFrieds(UserId);
                if (data == null)
                {
                    Resp.Data = new InviteFrdClaas();
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    return Resp;
                }
                Resp.Data = data;
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ErrorCode = enErrorCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public ListGetMonthWiseCommissionData GetMonthWiseCommissionChartDetail(int? Year,long UserId)
        {
            try
            {
                ListGetMonthWiseCommissionData Resp = new ListGetMonthWiseCommissionData();
                var data = _affiliateRepository.GetMonthWiseCommissionChartDetail(Year, UserId);
                if (data == null)
                {
                    Resp.Response = new GetMonthWiseCommissionDataV1();
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    return Resp;
                }
                Resp.Response = data;
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ErrorCode = enErrorCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
    }
}
