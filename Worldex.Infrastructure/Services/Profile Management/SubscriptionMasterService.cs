using Worldex.Core.Entities.Complaint;
using Worldex.Core.Entities.Organization;
using Worldex.Core.Entities.Profile_Management;
using Worldex.Core.Interfaces.Organization;
using Worldex.Core.Interfaces.Profile_Management;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.Profile_Management;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.Profile_Management
{
    public class SubscriptionMasterService : ISubscriptionMaster
    {
        private readonly ICustomRepository<SubscriptionMaster> _subscriptionRepository;
        private readonly ICustomRepository<ProfileMaster> _profileRepository;
        private readonly ICustomRepository<Typemaster> _typemasterRepository;
        private readonly WorldexContext _dbContext;
        private readonly ICustomRepository<ProfileLevelMaster> _ProfilelevelRepository;
        //private readonly IMediator _mediator;
        private readonly IActivityLogProcess _activityLogProcess;
        private readonly ICustomExtendedRepository<ActivityType_Master> _ActivityTypeMaster;
        private readonly ICustomExtendedRepository<ActivityRegister> _ActivityRegister;
        private readonly IActivityMasterConfiguration _activityMasterConfiguration;
        public SubscriptionMasterService(ICustomRepository<SubscriptionMaster> subscriptionRepository, ICustomRepository<ProfileMaster> profileRepository, ICustomRepository<Typemaster> typemasterRepository, WorldexContext dbContext,
            //IMediator mediator, 
            IActivityLogProcess activityLogProcess,
            ICustomRepository<ProfileLevelMaster> ProfilelevelRepository,
            ICustomExtendedRepository<ActivityType_Master> ActivityTypeMaster,
            ICustomExtendedRepository<ActivityRegister> ActivityRegister,
            IActivityMasterConfiguration activityMasterConfiguration)
        {
            _subscriptionRepository = subscriptionRepository;
            _profileRepository = profileRepository;
            _typemasterRepository = typemasterRepository;
            _dbContext = dbContext;
            //_mediator = mediator;
            _activityLogProcess = activityLogProcess;
            _ProfilelevelRepository = ProfilelevelRepository;
            _ActivityTypeMaster = ActivityTypeMaster;
            _ActivityRegister = ActivityRegister;
            _activityMasterConfiguration = activityMasterConfiguration;
        }

        public long AddMultiSubscription(int UserId, long ProfileId)
        {
            try
            {
                // added by nirav savariya for current level update 
                var Subscriptiondata = _subscriptionRepository.Table.Where(i => i.UserId == UserId).FirstOrDefault();
                if (Subscriptiondata != null)
                {

                    Subscriptiondata.Status = 1;
                    Subscriptiondata.UpdatedBy = UserId;
                    Subscriptiondata.UpdatedDate = DateTime.UtcNow;
                    Subscriptiondata.ProfileId = ProfileId;
                    _subscriptionRepository.Update(Subscriptiondata);
                    return Subscriptiondata.Id;
                }
                else
                {

                    SubscriptionAccessibleFeatures features = new SubscriptionAccessibleFeatures();
                    features.IsAddCoin = false;
                    features.IsAfterrecursion = 2;
                    features.Isbeforerecursion = 1;
                    features.IsDevicewhitelisting = false;
                    features.IsIPWhitelisting = false;
                    features.IsLedger = false;
                    features.IsMarginTrading = false;
                    features.IsMonthlyrecursion = 1;
                    features.IsPaymentFail = false;
                    features.IsPaymentReceived = false;
                    features.IsProfileActivation = false;
                    features.IsProfileupgradedowngrade = false;
                    features.IsRefund = false;
                    features.IsSocialTrading = false;
                    features.IsStackingBalance = false;
                    features.IsUpcomingpaymentnotification = false;
                    features.IsWhitelistwithdrawaladdress = false;
                    features.IsYearlyrecursion = 2;


                    var Subscription = new SubscriptionMaster();
                    Subscription.UserId = UserId;
                    Subscription.ProfileId = ProfileId;
                    Subscription.CreatedDate = DateTime.UtcNow;
                    Subscription.CreatedBy = UserId;
                    Subscription.Status = 1;
                    Subscription.AccessibleFeatures = JsonConvert.SerializeObject(features);
                    _subscriptionRepository.Insert(Subscription);
                    return Subscription.Id;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public long AddSubscription(SubscriptionViewModel model)
        {
            try
            {
                SubscriptionAccessibleFeatures features = new SubscriptionAccessibleFeatures();
                features.IsAddCoin = false;
                features.IsAfterrecursion = 2;
                features.Isbeforerecursion = 1;
                features.IsDevicewhitelisting = false;
                features.IsIPWhitelisting = false;
                features.IsLedger = false;
                features.IsMarginTrading = false;
                features.IsMonthlyrecursion = 1;
                features.IsPaymentFail = false;
                features.IsPaymentReceived = false;
                features.IsProfileActivation = false;
                features.IsProfileupgradedowngrade = false;
                features.IsRefund = false;
                features.IsSocialTrading = false;
                features.IsStackingBalance = false;
                features.IsUpcomingpaymentnotification = false;
                features.IsWhitelistwithdrawaladdress = false;
                features.IsYearlyrecursion = 2;

                model.AccessibleFeatures = features;

                var Subscription = new SubscriptionMaster();

                Subscription.UserId = model.UserId;
                Subscription.ProfileId = _profileRepository.Table.Where(i => i.Profilelevel == 1).Count() > 0 ? _profileRepository.Table.Where(i => i.Profilelevel == 1).FirstOrDefault().Id : 0;
                Subscription.CreatedDate = DateTime.UtcNow;
                Subscription.CreatedBy = model.UserId;
                Subscription.Status = 1;
                Subscription.AccessibleFeatures = JsonConvert.SerializeObject(model.AccessibleFeatures);
                //StartDate = DateTime.UtcNow,
                //EndDate = DateTime.UtcNow.AddYears(1),
                //ActiveStatus = true

                _subscriptionRepository.Insert(Subscription);


                string requestLog = JsonConvert.SerializeObject(Subscription);
                ActivityReqRes modeldata = new ActivityReqRes();
                modeldata.Id = Guid.NewGuid();
                modeldata.Remark = string.Empty;
                modeldata.Connection = string.Empty;
                modeldata.ApplicationId = Guid.Empty;
                modeldata.Channel = "";
                modeldata.DeviceId = string.Empty;
                modeldata.IPAddress = string.Empty;
                modeldata.ActivityType = "/api/Profile/AddProfile";
                modeldata.Session = string.Empty;
                modeldata.AccessToken = string.Empty;
                modeldata.AliasName = "Add Profile";
                modeldata.ModuleTypeName = "Profile";
                modeldata.HostURLName = "";

                modeldata.ActivityDetId = Guid.NewGuid();
                modeldata.Request = requestLog;
                //Task.Run(() => _mediator.Send(modeldata));
                _activityLogProcess.AddActivityLog(modeldata);


                ActivityRes modelres = new ActivityRes();
                modelres.ErrorCode = 0;
                modelres.ReturnCode = 0;
                modelres.ReturnMsg = "Success full add default profile subscribe.";
                modelres.StatusCode = 200;
                modelres.Response = "Success full add default profile subscribe.";
                modelres.ActivityId = modeldata.Id;
                modelres.Id = modeldata.Id;
                modelres.CreatedBy = model.UserId;
                _activityLogProcess.UpdateActivityLogAsync(modelres);
                //Task.Run(() => _mediator.Send(model));

                return Subscription.Id;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public long UnsubscribeProfile(int UserId, long ProfileId)
        {
            var Subscription = _subscriptionRepository.Table.Where(i => i.ProfileId == ProfileId && i.UserId == UserId && i.Status == 1).FirstOrDefault();
            if (Subscription != null)
            {
                Subscription.UpdatedBy = UserId;
                Subscription.UpdatedDate = DateTime.UtcNow;
                Subscription.Status = 0;
                _subscriptionRepository.Update(Subscription);

                return Subscription.Id;
            }
            else
                return 0;
        }

        public bool GetSubscriptionData(int UserId, long ProfileId)
        {
            try
            {
                bool Status = false;
                var Subscription = _activityMasterConfiguration.GetUserSubscription().Where(i => i.UserId == UserId && i.ProfileId == ProfileId && i.Status == 1).FirstOrDefault();
                //var Subscription = _subscriptionRepository.Table.Where(i => i.ProfileId == ProfileId && i.UserId == UserId && i.Status == 1).FirstOrDefault();
                if (Subscription == null)
                    Status = true;
                else
                    Status = false;
                if (Status)
                {
                    int Level = _activityMasterConfiguration.GetMasterProfileData().Where(j => j.Id == ProfileId).Count() > 0 ? _activityMasterConfiguration.GetMasterProfileData().Where(j => j.Id == ProfileId).FirstOrDefault().KYCLevel : 0;
                    //int Level = _profileRepository.Table.Where(i => i.Id == ProfileId).Count() > 0 ? _profileRepository.Table.Where(i => i.Id == ProfileId).FirstOrDefault().KYCLevel : 0;
                    long currentprofile = _activityMasterConfiguration.GetUserSubscription().Where(i => i.UserId == UserId && i.Status == 1).Count() > 0 ? _activityMasterConfiguration.GetUserSubscription().Where(i => i.UserId == UserId && i.Status == 1).FirstOrDefault().ProfileId : 0;
                    //long currentprofile = _subscriptionRepository.Table.Where(j => j.UserId == UserId && j.Status == 1).Count() > 0 ? _subscriptionRepository.Table.Where(j => j.UserId == UserId && j.Status == 1).FirstOrDefault().ProfileId : 0;
                    long currentprofilelevel = _activityMasterConfiguration.GetMasterProfileData().Where(i => i.Id == ProfileId).Count() > 0 ? _activityMasterConfiguration.GetMasterProfileData().Where(i => i.Id == ProfileId).FirstOrDefault().Profilelevel : 0;
                    //var SubscriptionData = (dynamic)null;
                    //if (ProfileId > currentprofile)
                    var SubscriptionData = (from pf in _profileRepository.Table
                                            join ss in _subscriptionRepository.Table on pf.Id equals ss.ProfileId
                                            where ss.UserId == UserId && ss.Status == 1
                                            select new { ProfileId = pf.Profilelevel, ProlfileKYCLevel = pf.KYCLevel }).ToList();
                    //else
                    //    SubscriptionData = (from pf in _profileRepository.Table
                    //                        join ss in _subscriptionRepository.Table on pf.Id equals ss.ProfileId
                    //                        where ss.UserId == UserId && ss.Status == 0
                    //                        select new { ProfileId = pf.Profilelevel, ProlfileKYCLevel = pf.KYCLevel }).ToList();
                    if (SubscriptionData.Count > 0)
                    {
                        //if (ProfileId > currentprofile)
                        //{
                        foreach (var item in SubscriptionData)
                        {
                            if (item.ProlfileKYCLevel > Level)
                            {
                                Status = false;
                                break;
                            }
                            else if (item.ProfileId > currentprofilelevel)
                            {
                                Status = false;
                                break;
                            }
                            else
                                Status = true;
                        }
                        //}
                        //else
                        //{
                        //    foreach (var item in SubscriptionData)
                        //    {
                        //        if (item.ProlfileKYCLevel > Level)
                        //        {
                        //            Status = false;
                        //            break;
                        //        }

                        //        else if (currentprofilelevelid >= item.ProfileId)
                        //        {
                        //            Status = false;
                        //            break;
                        //        }
                        //        else
                        //            Status = true;
                        //    }
                        //}
                    }
                    else
                        Status = false;
                    return Status;
                }
                else
                {
                    return Status;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public long GetSpcialProfileSubscriptionData(int UserId, int ProfileId)
        {

            try
            {

                string Qry = "";

                IQueryable<SubscriptionProfile> Subscription;
                Qry = @" Select sm.ProfileId from ProfileMaster pm inner join Typemaster tm on pm.TypeId = tm.Id  and  tm.[Type] = 'SocialProfile' and tm.Status = 1 ";
                Qry += " inner join subscriptionmaster sm on pm.Id = sm.ProfileId and sm.Status = 1 where sm.UserId =" + UserId;


                Subscription = _dbContext.SubscriptionProfileId.FromSql(Qry);
                List<SubscriptionProfile> Sprofile = new List<SubscriptionProfile>();
                Sprofile = Subscription.ToList();

                if (Sprofile.Count == 0)
                    return 0;
                else
                {
                    return Sprofile.ToList().FirstOrDefault().ProfileId;
                    //var profile = Sprofile.Where(s => s.ProfileId == ProfileId).ToList().FirstOrDefault();
                    //if (profile != null)
                    //    return profile.ProfileId;
                    //else
                    //    return 0;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }


        }

        public SubscriptionProfileType GetSpcialProfiletype(int UserId)
        {

            try
            {

                string Qry = "";

                IQueryable<SubscriptionProfileType> Subscription;
                //Qry = @" Select sm.ProfileId from ProfileMaster pm inner join Typemaster tm on pm.TypeId = tm.Id  and  tm.[Type] = 'SocialProfile' and tm.Status = 1 ";
                //Qry += " inner join subscriptionmaster sm on pm.Id = sm.ProfileId and sm.Status = 1 where sm.UserId =" + UserId;
                Qry = @" Select sm.ProfileId,tm.Subtype as ProfileType from ProfileMaster pm inner join Typemaster tm on pm.TypeId = tm.Id  and tm.[Type] = 'SocialProfile' and tm.Status = 1 ";
                Qry += " inner join subscriptionmaster sm on pm.Id = sm.ProfileId and sm.Status = 1 where sm.UserId =" + UserId; //+" and tm.SubType = 'Leader'";

                Subscription = _dbContext.SubscriptionProfileType.FromSql(Qry);
                List<SubscriptionProfileType> Sprofile = new List<SubscriptionProfileType>();
                Sprofile = Subscription.ToList();

                if (Sprofile.Count == 0)
                    return null;
                else
                {
                    return Sprofile.ToList().FirstOrDefault();
                    //var profile = Sprofile.Where(s => s.ProfileId == ProfileId).ToList().FirstOrDefault();
                    //if (profile != null)
                    //    return profile.ProfileId;
                    //else
                    //    return 0;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }


        }

        public GetActiveSubscriptionData CurrentUserProfileData(int UserId)
        {
            try
            {
                var UserData = (from ss in _subscriptionRepository.Table
                                join pf in _profileRepository.Table on ss.ProfileId equals pf.Id
                                join plf in _ProfilelevelRepository.Table on pf.Profilelevel equals plf.Id
                                where ss.UserId == UserId && ss.Status == 1
                                select new
                                {
                                    ProfileId = ss.ProfileId,
                                    ProfileName = plf.ProfileName,
                                    Status = ss.Status,
                                    AccessibleFeatures = ss.AccessibleFeatures,
                                    CreatedDate = ss.CreatedDate,
                                    Profilelevel = pf.Profilelevel
                                }).FirstOrDefault();


                GetActiveSubscriptionData data = new GetActiveSubscriptionData();
                if (UserData != null)
                {
                    data.Status = Convert.ToBoolean(UserData.Status);
                    data.ProfileName = UserData.ProfileName;
                    data.Profilelevel = UserData.Profilelevel;
                    data.ProfileId = UserData.ProfileId;
                    data.CreatedDate = UserData.CreatedDate;
                    if (!string.IsNullOrEmpty(UserData.AccessibleFeatures))
                        data.AccessibleFeatures = JsonConvert.DeserializeObject<AccessibleFeaturesViewModel>(UserData.AccessibleFeatures);
                    else
                        data.AccessibleFeatures = null;
                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public UserProfileHistoryDataResponse GetUserProfileHistoryData(int Userid, string AddProfile, int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                IQueryable<ProfileHistoryData> Result;
                List<ProfileHistoryData> UserProfileData = new List<ProfileHistoryData>();
                var AllUserProfileData = new List<UserProfileHistoryData>();
                string Query = string.Empty;
                var IsProfileExist = _ActivityTypeMaster.Table.FirstOrDefault(i => i.TypeMaster == AddProfile);  ///This Method are user to get ActivityTypeId base on particular method wise
                if (IsProfileExist != null)
                {
                    Query = " Select AM.Id,AM.CreatedDate,AM.CreatedBy as [UserId],AD.Request,AD.Response from Activityregister AM";
                    Query += " Inner join ActivityRegisterDetail AD on AM.Id=AD.ActivityId Where ActivityTypeId='" + IsProfileExist.Id + "'";
                    Query += " and Createdby=" + Userid + " and Returncode=0 and Errorcode=0 and statuscode=200";
                    Result = _dbContext.ProfileHistoryData.FromSql(Query);

                    UserProfileData = Result.OrderBy(i => i.CreatedDate).ToList();

                    foreach (var item in UserProfileData)
                    {
                        //var ProfileId = (dynamic)null;
                        UserProfileHistoryData imodel = new UserProfileHistoryData();
                        var  ProfileId = JsonConvert.DeserializeObject<SubscriptionProfile>(item.Request);
                        imodel.ProfileId = ProfileId.ProfileId;
                        imodel.Profilelevel = _profileRepository.Table.Where(i => i.Id == ProfileId.ProfileId).Count() > 0 ? _profileRepository.Table.Where(i => i.Id == ProfileId.ProfileId).FirstOrDefault().Profilelevel : 0;
                        imodel.ProfileName = _ProfilelevelRepository.Table.Where(i => i.Id == imodel.Profilelevel).Count() > 0 ? _ProfilelevelRepository.Table.Where(i => i.Id == imodel.Profilelevel).FirstOrDefault().ProfileName : string.Empty;
                        imodel.CreatedDate = item.CreatedDate;
                        AllUserProfileData.Add(imodel);
                    }

                    if (PageIndex == 0)
                    {
                        PageIndex = 1;
                    }

                    if (Page_Size == 0)
                    {
                        Page_Size = 10;
                    }

                    var skip = Page_Size * (PageIndex - 1);
                    UserProfileHistoryDataResponse UserProfileHistory = new UserProfileHistoryDataResponse()
                    {
                        UserProfileHistory = AllUserProfileData.Skip(skip).Take(Page_Size).OrderByDescending(i => i.CreatedDate).ToList(),
                        TotalCount = AllUserProfileData.Count()
                    };

                    return UserProfileHistory;
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
