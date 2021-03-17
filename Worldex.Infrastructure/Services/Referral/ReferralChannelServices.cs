using Worldex.Core.Entities.Referral;
using Worldex.Core.Interfaces.Referral;
using System;
using System.Collections.Generic;
using System.Text;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.Referral;
using System.Linq;
using Worldex.Core.Entities.User;
using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Interfaces;
using Worldex.Core.Enums;
using Worldex.Infrastructure.BGTask;
using Worldex.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Google.Apis.Services;
using Google.Apis.Urlshortener.v1;
using Newtonsoft.Json;
using System.Net;
using System.Web;
using System.Xml;
using Worldex.Core.Services;

namespace Worldex.Infrastructure.Services.Referral
{
    public class ReferralChannelServices : IReferralChannel
    {
        private readonly WorldexContext _dbContext;
        private readonly ICustomRepository<ReferralChannel> _ReferralChannelRepository;
        private readonly ICustomRepository<ReferralChannelType> _ReferralChannelTypeRepository;
        private readonly ICustomRepository<ReferralUser> _ReferralUserRepository;
        private readonly IMessageService _messageService;
        private readonly IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly EncyptedDecrypted _encdecAEC;
        private readonly ICustomRepository<ReferralUserClick> _ReferralUserClickRepository;
        private readonly ICustomRepository<ReferralRewards> _ReferralRewardsRepository;

        public ReferralChannelServices(ICustomRepository<ReferralChannel> ReferralChannelRepository, WorldexContext dbContext, ICustomRepository<ReferralUser> ReferralUserRepository,
            IMessageService messageService, IPushNotificationsQueue<SendSMSRequest> pushSMSQueue, ICustomRepository<ReferralChannelType> ReferralChannelTypeRepository,
            IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue, Microsoft.Extensions.Configuration.IConfiguration configuration, EncyptedDecrypted encdecAEC,
            ICustomRepository<ReferralUserClick> ReferralUserClickRepository, ICustomRepository<ReferralRewards> ReferralRewardsRepository)
        {
            _dbContext = dbContext;
            _ReferralChannelRepository = ReferralChannelRepository;
            _ReferralUserRepository = ReferralUserRepository;
            _messageService = messageService;
            _pushSMSQueue = pushSMSQueue;
            _ReferralChannelTypeRepository = ReferralChannelTypeRepository;
            _pushNotificationsQueue = pushNotificationsQueue;
            _configuration = configuration;
            _encdecAEC = encdecAEC;
            _ReferralUserClickRepository = ReferralUserClickRepository;
            _ReferralRewardsRepository = ReferralRewardsRepository;
        }

        public long AddReferralChannel(ReferralChannelViewModel ReferralChannelInsert, long UserID)
        {
            try
            {
                ReferralChannel ObjReferralChannel = new ReferralChannel()
                {
                    UserId = UserID,
                    ReferralChannelTypeId = ReferralChannelInsert.ReferralChannelTypeId,
                    ReferralChannelServiceId = ReferralChannelInsert.ReferralChannelServiceId,
                    ReferralReceiverAddress = ReferralChannelInsert.ReferralReceiverAddress,
                    CreatedBy = UserID,
                    CreatedDate = DateTime.UtcNow,
                    Status = 1
                };
                _ReferralChannelRepository.Insert(ObjReferralChannel);
                return ObjReferralChannel.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ReferralChannelListResponse ListAdminReferralChannelInvite(int PageIndex = 0, int Page_Size = 0, long ReferralChannelTypeId = 0, long ReferralPayTypeId = 0, long ReferralServiceId = 0, string UserName = null, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                if (PageIndex == 0)
                {
                    PageIndex = 1;
                }
                if (Page_Size == 0)
                {
                    Page_Size = 10;
                }
                var skip = Page_Size * (PageIndex - 1);

                var items = (from rs in _dbContext.ReferralChannel
                             join st in _dbContext.ReferralService on rs.ReferralChannelServiceId equals st.Id
                             join cht in _dbContext.ReferralChannelType on rs.ReferralChannelTypeId equals cht.Id
                             join us in _dbContext.Users on rs.UserId equals us.Id
                             join pt in _dbContext.ReferralPayType on st.ReferralPayTypeId equals pt.Id
                             select new ReferralChannelListViewModel
                             {
                                 Id = rs.Id,
                                 UserId = us.Id,
                                 UserName = us.UserName,
                                 ReferralPayTypeId = pt.Id,
                                 PayTypeName = pt.PayTypeName,
                                 ReferralChannelTypeId = rs.ReferralChannelTypeId,
                                 ChannelTypeName = cht.ChannelTypeName,
                                 ReferralChannelServiceId = rs.ReferralChannelServiceId,
                                 Description = st.Description,
                                 ReferralReceiverAddress = rs.ReferralReceiverAddress,
                                 CreatedDate = rs.CreatedDate,
                                 Status = rs.Status
                             }
                            ).ToList();

                if (ReferralChannelTypeId != 0)
                {
                    items = items.Where(x => x.ReferralChannelTypeId == ReferralChannelTypeId).ToList();
                }
                if (ReferralPayTypeId != 0)
                {
                    items = items.Where(x => x.ReferralPayTypeId == ReferralPayTypeId).ToList();
                }
                if (ReferralServiceId != 0)
                {
                    items = items.Where(x => x.ReferralChannelServiceId == ReferralServiceId).ToList();
                }
                if (!String.IsNullOrEmpty(UserName))
                {
                    items = items.Where(x => x.UserName == UserName).ToList();
                }
                if (FromDate != null && ToDate != null)
                {
                    items = items.Where(x => x.CreatedDate.Date >= FromDate && x.CreatedDate.Date <= ToDate).ToList();
                }

                int TotalCount = items.Count();
                ReferralChannelListResponse obj = new ReferralChannelListResponse();
                obj.TotalCount = TotalCount;
                obj.ReferralChannelList = items.OrderByDescending(x => x.Id).Skip(skip).Take(Page_Size).ToList();
                return obj;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ReferralChannelListResponse ListAdminReferralChannelWithChannelType(long ReferralChannelTypeId, int PageIndex = 0, int Page_Size = 0, long ReferralPayTypeId = 0, long ReferralServiceId = 0, string UserName = null, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                if (PageIndex == 0)
                {
                    PageIndex = 1;
                }
                if (Page_Size == 0)
                {
                    Page_Size = 10;
                }

                var skip = Page_Size * (PageIndex - 1);

                var items = (from rs in _dbContext.ReferralChannel
                             join st in _dbContext.ReferralService on rs.ReferralChannelServiceId equals st.Id
                             join cht in _dbContext.ReferralChannelType on rs.ReferralChannelTypeId equals cht.Id
                             join us in _dbContext.Users on rs.UserId equals us.Id
                             join pt in _dbContext.ReferralPayType on st.ReferralPayTypeId equals pt.Id
                             where cht.Id.Equals(ReferralChannelTypeId)
                             select new ReferralChannelListViewModel
                             {
                                 Id = rs.Id,
                                 UserId = us.Id,
                                 UserName = us.UserName,
                                 ReferralPayTypeId = pt.Id,
                                 PayTypeName = pt.PayTypeName,
                                 ReferralChannelTypeId = rs.ReferralChannelTypeId,
                                 ChannelTypeName = cht.ChannelTypeName,
                                 ReferralChannelServiceId = rs.ReferralChannelServiceId,
                                 Description = st.Description,
                                 ReferralReceiverAddress = rs.ReferralReceiverAddress,
                                 CreatedDate = rs.CreatedDate,
                                 Status = rs.Status
                             }
                            ).ToList();

                if (ReferralPayTypeId != 0)
                {
                    items = items.Where(x => x.ReferralPayTypeId == ReferralPayTypeId).ToList();
                }
                if (ReferralServiceId != 0)
                {
                    items = items.Where(x => x.ReferralChannelServiceId == ReferralServiceId).ToList();
                }
                if (!String.IsNullOrEmpty(UserName))
                {
                    items = items.Where(x => x.UserName == UserName).ToList();
                }
                if (FromDate != null && ToDate != null)
                {
                    items = items.Where(x => x.CreatedDate.Date >= FromDate && x.CreatedDate.Date <= ToDate).ToList();
                }


                int TotalCount = items.Count();

                ReferralChannelListResponse obj = new ReferralChannelListResponse();
                obj.TotalCount = TotalCount;
                obj.ReferralChannelList = items.OrderByDescending(x => x.Id).Skip(skip).Take(Page_Size).ToList();
                return obj;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ReferralChannelListResponse ListUserReferralChannelInvite(long UserId, int PageIndex = 0, int Page_Size = 0, long ReferralChannelTypeId = 0, long ReferralPayTypeId = 0, long ReferralServiceId = 0, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                if (PageIndex == 0)
                {
                    PageIndex = 1;
                }

                if (Page_Size == 0)
                {
                    Page_Size = 10;
                }

                var skip = Page_Size * (PageIndex - 1);

                var items = (from rs in _dbContext.ReferralChannel
                             join st in _dbContext.ReferralService on rs.ReferralChannelServiceId equals st.Id
                             join cht in _dbContext.ReferralChannelType on rs.ReferralChannelTypeId equals cht.Id
                             join us in _dbContext.Users on rs.UserId equals us.Id
                             join pt in _dbContext.ReferralPayType on st.ReferralPayTypeId equals pt.Id
                             where rs.UserId.Equals(UserId)
                             select new ReferralChannelListViewModel
                             {
                                 Id = rs.Id,
                                 UserId = us.Id,
                                 UserName = us.UserName,
                                 ReferralPayTypeId = pt.Id,
                                 PayTypeName = pt.PayTypeName,
                                 ReferralChannelTypeId = rs.ReferralChannelTypeId,
                                 ChannelTypeName = cht.ChannelTypeName,
                                 ReferralChannelServiceId = rs.ReferralChannelServiceId,
                                 Description = st.Description,
                                 ReferralReceiverAddress = rs.ReferralReceiverAddress,
                                 CreatedDate = rs.CreatedDate,
                                 Status = rs.Status
                             }
                            ).ToList();

                if (ReferralChannelTypeId != 0)
                {
                    items = items.Where(x => x.ReferralChannelTypeId == ReferralChannelTypeId).ToList();
                }
                if (ReferralPayTypeId != 0)
                {
                    items = items.Where(x => x.ReferralPayTypeId == ReferralPayTypeId).ToList();
                }
                if (ReferralServiceId != 0)
                {
                    items = items.Where(x => x.ReferralChannelServiceId == ReferralServiceId).ToList();
                }
                if (FromDate != null && ToDate != null)
                {
                    items = items.Where(x => x.CreatedDate.Date >= FromDate && x.CreatedDate.Date <= ToDate).ToList();
                }
                int TotalCount = items.Count();

                ReferralChannelListResponse obj = new ReferralChannelListResponse();
                obj.TotalCount = TotalCount;
                obj.ReferralChannelList = items.OrderByDescending(x => x.Id).Skip(skip).Take(Page_Size).ToList();
                return obj;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ReferralChannelListResponse ListUserReferralChannelWithChannelType(long ReferralChannelTypeId, long UserId, int PageIndex = 0, int Page_Size = 0, long ReferralPayTypeId = 0, long ReferralServiceId = 0, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                if (PageIndex == 0)
                {
                    PageIndex = 1;
                }
                if (Page_Size == 0)
                {
                    Page_Size = 10;
                }

                var skip = Page_Size * (PageIndex - 1);

                var items = (from rs in _dbContext.ReferralChannel
                             join st in _dbContext.ReferralService on rs.ReferralChannelServiceId equals st.Id
                             join cht in _dbContext.ReferralChannelType on rs.ReferralChannelTypeId equals cht.Id
                             join us in _dbContext.Users on rs.UserId equals us.Id
                             join pt in _dbContext.ReferralPayType on st.ReferralPayTypeId equals pt.Id
                             where (cht.Id.Equals(ReferralChannelTypeId)) && (rs.UserId.Equals(UserId))
                             select new ReferralChannelListViewModel
                             {
                                 Id = rs.Id,
                                 UserId = us.Id,
                                 UserName = us.UserName,
                                 ReferralPayTypeId = pt.Id,
                                 PayTypeName = pt.PayTypeName,
                                 ReferralChannelTypeId = rs.ReferralChannelTypeId,
                                 ChannelTypeName = cht.ChannelTypeName,
                                 ReferralChannelServiceId = rs.ReferralChannelServiceId,
                                 Description = st.Description,
                                 ReferralReceiverAddress = rs.ReferralReceiverAddress,
                                 CreatedDate = rs.CreatedDate,
                                 Status = rs.Status
                             }
                            ).ToList();


                if (ReferralPayTypeId != 0)
                {
                    items = items.Where(x => x.ReferralPayTypeId == ReferralPayTypeId).ToList();
                }
                if (ReferralServiceId != 0)
                {
                    items = items.Where(x => x.ReferralChannelServiceId == ReferralServiceId).ToList();
                }
                if (FromDate != null && ToDate != null)
                {
                    items = items.Where(x => x.CreatedDate.Date >= FromDate && x.CreatedDate.Date <= ToDate).ToList();
                }

                int TotalCount = items.Count();

                ReferralChannelListResponse obj = new ReferralChannelListResponse();
                obj.TotalCount = TotalCount;
                obj.ReferralChannelList = items.OrderByDescending(x => x.Id).Skip(skip).Take(Page_Size).ToList();
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ReferralChannelAllCountViewModel AllCountForAdminReferralChannel()
        {
            try
            {
                //Rushabh 21-08-2019 As Count Mismatch In ListAdminParticipateReferralUser & AllCountForAdminReferralChannel Method
                #region OLD CODE

                //int Participant = _ReferralUserRepository.Table.Count();                
                //int Invite = _ReferralChannelRepository.Table.Count();
                //int Clicks = _ReferralUserClickRepository.Table.Count();
                //int Converts = _ReferralRewardsRepository.Table.Count();
                //int EmailInvite = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 1).Count();
                //int SMSInvite = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 2).Count();
                //int FacebookShare = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 3).Count();
                //int TwitterShare = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 4).Count();
                //int LinkedIn = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 5).Count();
                //int GoogleShare = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 6).Count();
                //int InstaShare = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 7).Count();
                //int Pinterest = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 8).Count();
                //int Telegram = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 9).Count();
                //int WhatsApp = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 10).Count();
                //int Messenger = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 11).Count();
                //int QRCode = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 12).Count();

                //ReferralChannelAllCountViewModel obj = new ReferralChannelAllCountViewModel
                //{
                //    TotalParticipants = Participant,
                //    Invite = Invite,
                //    Clicks = Clicks,
                //    Converts = Converts,
                //    EmailInvite = EmailInvite,
                //    SMSInvite = SMSInvite,
                //    FacebookShare = FacebookShare,
                //    TwitterShare = TwitterShare,
                //    LinkedIn = LinkedIn,
                //    GoogleShare = GoogleShare,
                //    InstaShare = InstaShare,
                //    Pinterest = Pinterest,
                //    Telegram = Telegram,
                //    WhatsApp = WhatsApp,
                //    Messenger = Messenger,
                //    QRCode = QRCode
                //};

                #endregion

                var Participant = _dbContext.ReturnCount.FromSql(@"SELECT COUNT(rs.Id) AS 'TotalCount' FROM ReferralUser rs INNER JOIN ReferralService st ON rs.ReferralServiceId = st.Id INNER JOIN BizUser us ON rs.UserId = us.Id INNER JOIN BizUser rus ON rs.ReferUserId = rus.Id INNER JOIN ReferralChannelType cht ON rs.ReferralChannelTypeId = cht.Id").FirstOrDefault();
                var Clicks = _dbContext.ReturnCount.FromSql(@"SELECT COUNT(ruc.id) AS 'TotalCount' FROM ReferralUserClick ruc INNER JOIN ReferralService st ON ruc.ReferralServiceId = st.Id INNER JOIN ReferralChannelType cht ON ruc.ReferralChannelTypeId = cht.Id").FirstOrDefault();
                var Converts = _dbContext.ReturnCount.FromSql(@"select rs.Id as Id, rs.UserId as UserId, us.UserName as UserName, rs.ReferralServiceId as ReferralServiceId, ms.Description as ReferralServiceDescription, rs.ReferralPayRewards as ReferralPayRewards, pt.Id as ReferralPayTypeId, pt.PayTypeName as ReferralPayTypeName, cu.Id as CurrencyId, cu.WalletTypeName as CurrencyName, rs.CreatedDate as CreatedDate, rs.LifeTimeUserCount as LifeTimeUserCount, rs.LifeTimeUserCount as NewUserCount, ComCurr.WalletTypeName as CommissionCurrecyName, rs.CommissionCurrecyId as CommissionCurrecyId, rs.SumChargeAmount as SumChargeAmount, rs.TransactionCurrencyId as TransactionCurrencyId, ISNULL(TrnCurr.WalletTypeName,'') as TransactionCurrecyName, rs.SumOfTransaction as SumOfTransaction, rs.TrnUserId as TrnUserId, Trnus.UserName as TrnUserName, FromWa.Walletname as FromWalletName, ToWa.Walletname as ToWalletName, rs.TrnRefNo as TrnRefNo, rs.CommissionAmount as CommissionAmount, rs.TransactionAmount as TransactionAmount, cast(0 as bigint) as CommissionCroneID, cast('' as varchar(50)) as CommissionCroneRemarks, rs.FromWalletId as FromWalletId, rs.ToWalletId as ToWalletId, rs.TrnDate as TrnDate From ReferralRewards rs inner join ReferralServiceDetail st on rs.ReferralServiceId = st.Id inner join ReferralSchemeTypeMapping m on m.id=st.SchemeTypeMappingId inner join Referralservice ms on ms.id=m.ServiceTypeMstId inner join BizUser us on rs.UserID = us.ID inner join ReferralPayType pt on rs.ReferralPayTypeId = pt.ID inner join WalletTypeMasters cu on st.CreditWalletTypeId = cu.Id inner join WalletTypeMasters ComCurr on rs.CommissionCurrecyId = ComCurr.Id left join WalletTypeMasters TrnCurr on rs.TransactionCurrencyId = TrnCurr.Id inner join Bizuser Trnus on rs.TrnUserId = Trnus.Id inner join WalletMasters FromWa on rs.FromWalletId = FromWa.Id inner join WalletMasters ToWa on rs.ToWalletId = ToWa.Id ").FirstOrDefault();
                var Invite = _dbContext.ReferralChannelCount.FromSql(@"SELECT rch.id, rch.ReferralChannelTypeId FROM ReferralChannel rch INNER JOIN ReferralChannelType cht ON rch.ReferralChannelTypeId = cht.Id").ToList();


                ReferralChannelAllCountViewModel obj = new ReferralChannelAllCountViewModel
                {
                    TotalParticipants = Participant.TotalCount,
                    Invite = Invite.Count,
                    Clicks = Clicks.TotalCount,
                    Converts = Converts.TotalCount,
                    EmailInvite = Invite.Where(x => x.ReferralChannelTypeId == 1).Count(),
                    SMSInvite = Invite.Where(x => x.ReferralChannelTypeId == 2).Count(),
                    FacebookShare = Invite.Where(x => x.ReferralChannelTypeId == 3).Count(),
                    TwitterShare = Invite.Where(x => x.ReferralChannelTypeId == 4).Count(),
                    LinkedIn = Invite.Where(x => x.ReferralChannelTypeId == 5).Count(),
                    GoogleShare = Invite.Where(x => x.ReferralChannelTypeId == 6).Count(),
                    InstaShare = Invite.Where(x => x.ReferralChannelTypeId == 7).Count(),
                    Pinterest = Invite.Where(x => x.ReferralChannelTypeId == 8).Count(),
                    Telegram = Invite.Where(x => x.ReferralChannelTypeId == 9).Count(),
                    WhatsApp = Invite.Where(x => x.ReferralChannelTypeId == 10).Count(),
                    Messenger = Invite.Where(x => x.ReferralChannelTypeId == 11).Count(),
                    QRCode = Invite.Where(x => x.ReferralChannelTypeId == 12).Count()
                };
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ReferralChannelAllCountViewModel AllCountForUserReferralChannel(long UserId)
        {
            try
            {
                // 31-07-2019 commented by rushabh , Added optimize query
                //int Participant = _ReferralUserRepository.Table.Where(x => x.UserId == UserId).Count();
                //int Invite = _ReferralChannelRepository.Table.Where(x => x.UserId == UserId).Count();
                //int Clicks = _ReferralUserClickRepository.Table.Where(x => x.UserId == UserId).Count();
                //int Converts = _ReferralRewardsRepository.Table.Where(x => x.UserId == UserId).Count();
                //int EmailInvite = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 1 && x.UserId == UserId).Count();
                //int SMSInvite = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 2 && x.UserId == UserId).Count();
                //int FacebookShare = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 3 && x.UserId == UserId).Count();
                //int TwitterShare = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 4 && x.UserId == UserId).Count();
                //int LinkedIn = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 5 && x.UserId == UserId).Count();
                //int GoogleShare = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 6 && x.UserId == UserId).Count();
                //int InstaShare = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 7 && x.UserId == UserId).Count();
                //int Pinterest = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 8 && x.UserId == UserId).Count();
                //int Telegram = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 9 && x.UserId == UserId).Count();
                //int WhatsApp = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 10 && x.UserId == UserId).Count();
                //int Messenger = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 11 && x.UserId == UserId).Count();
                //int QRCode = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 12 && x.UserId == UserId).Count();

                //ReferralChannelAllCountViewModel obj = new ReferralChannelAllCountViewModel();
                //obj.TotalParticipants = Participant;
                //obj.Invite = Invite;
                //obj.Clicks = Clicks;
                //obj.Converts = Converts;
                //obj.EmailInvite = EmailInvite;
                //obj.SMSInvite = SMSInvite;
                //obj.FacebookShare = FacebookShare;
                //obj.TwitterShare = TwitterShare;
                //obj.LinkedIn = LinkedIn;
                //obj.GoogleShare = GoogleShare;
                //obj.InstaShare = InstaShare;
                //obj.Pinterest = Pinterest;
                //obj.Telegram = Telegram;
                //obj.WhatsApp = WhatsApp;
                //obj.Messenger = Messenger;
                //obj.QRCode = QRCode;
                //return obj;

                int Participant = _ReferralUserRepository.Table.Where(x => x.UserId == UserId).Count();
                int Invite = _ReferralChannelRepository.Table.Where(x => x.UserId == UserId).Count();
                int Clicks = _ReferralUserClickRepository.Table.Where(x => x.UserId == UserId).Count();
                //int Converts = _ReferralRewardsRepository.Table.Where(x => x.UserId == UserId).Count();
                //var items = (from rs in _dbContext.ReferralRewards
                //             join st in _dbContext.ReferralService on rs.ReferralServiceId equals st.Id
                //             join us in _dbContext.Users on rs.UserId equals us.Id
                //             join pt in _dbContext.ReferralPayType on st.ReferralPayTypeId equals pt.Id
                //             join cu in _dbContext.WalletTypeMasters on st.CurrencyId equals cu.Id
                //             join ComCurr in _dbContext.WalletTypeMasters on rs.CommissionCurrecyId equals ComCurr.Id
                //             join TrnCurr in _dbContext.WalletTypeMasters on rs.TransactionCurrencyId equals TrnCurr.Id
                //             join Trnus in _dbContext.Users on rs.TrnUserId equals Trnus.Id
                //             join FromWa in _dbContext.WalletMasters on rs.FromWalletId equals FromWa.Id
                //             join ToWa in _dbContext.WalletMasters on rs.ToWalletId equals ToWa.Id
                //             where rs.UserId.Equals(UserId)
                //             select new ReferralRewardsListViewModel
                //             {
                //                 Id = rs.Id,
                //                 UserId = rs.UserId,
                //                 UserName = us.UserName,
                //                 ReferralServiceId = rs.ReferralServiceId,
                //                 ReferralServiceDescription = st.Description,
                //                 ReferralPayRewards = rs.ReferralPayRewards,
                //                 ReferralPayTypeId = pt.Id,
                //                 ReferralPayTypeName = pt.PayTypeName,
                //                 CurrencyId = cu.Id,
                //                 CurrencyName = cu.WalletTypeName,
                //                 CreatedDate = rs.CreatedDate,
                //                 LifeTimeUserCount = rs.LifeTimeUserCount,
                //                 NewUserCount = rs.LifeTimeUserCount,
                //                 CommissionCurrecyName = ComCurr.WalletTypeName,
                //                 SumChargeAmount = rs.SumChargeAmount,
                //                 TransactionCurrecyName = TrnCurr.WalletTypeName,
                //                 SumOfTransaction = rs.SumOfTransaction,
                //                 TrnUserId = rs.TrnUserId,
                //                 TrnUserName = Trnus.UserName,
                //                 FromWalletName = FromWa.Walletname,
                //                 ToWalletName = ToWa.Walletname,
                //                 TrnRefNo = rs.TrnRefNo,
                //                 CommissionAmount = rs.CommissionAmount,
                //                 TransactionAmount = rs.TransactionAmount
                //             }
                //            ).ToList();
                var items = _dbContext.ReferralRewardsListViewModel.FromSql("select rs.Id as Id, rs.UserId as UserId, us.UserName as UserName, rs.ReferralServiceId as ReferralServiceId, ms.Description as ReferralServiceDescription, rs.ReferralPayRewards as ReferralPayRewards, pt.Id as ReferralPayTypeId, pt.PayTypeName as ReferralPayTypeName, cu.Id as CurrencyId, cu.WalletTypeName as CurrencyName, rs.CreatedDate as CreatedDate, rs.LifeTimeUserCount as LifeTimeUserCount, rs.LifeTimeUserCount as NewUserCount, ComCurr.WalletTypeName as CommissionCurrecyName, rs.CommissionCurrecyId as CommissionCurrecyId, rs.SumChargeAmount as SumChargeAmount, rs.TransactionCurrencyId as TransactionCurrencyId, ISNULL(TrnCurr.WalletTypeName,'') as TransactionCurrecyName, rs.SumOfTransaction as SumOfTransaction, rs.TrnUserId as TrnUserId, Trnus.UserName as TrnUserName, FromWa.Walletname as FromWalletName, ToWa.Walletname as ToWalletName, rs.TrnRefNo as TrnRefNo, rs.CommissionAmount as CommissionAmount, rs.TransactionAmount as TransactionAmount, cast(0 as bigint) as CommissionCroneID, cast('' as varchar(50)) as CommissionCroneRemarks, rs.FromWalletId as FromWalletId, rs.ToWalletId as ToWalletId, rs.TrnDate as TrnDate  From ReferralRewards rs inner join ReferralServiceDetail st on rs.ReferralServiceId = st.Id  inner join ReferralSchemeTypeMapping m on m.id=st.SchemeTypeMappingId inner join Referralservice ms on ms.id=m.ServiceTypeMstId inner join BizUser us on rs.UserID = us.ID inner join ReferralPayType pt on rs.ReferralPayTypeId = pt.ID inner join WalletTypeMasters cu on st.CreditWalletTypeId = cu.Id inner join WalletTypeMasters ComCurr on rs.CommissionCurrecyId = ComCurr.Id  left join WalletTypeMasters TrnCurr on rs.TransactionCurrencyId = TrnCurr.Id  inner join Bizuser Trnus on rs.TrnUserId = Trnus.Id inner join WalletMasters FromWa on rs.FromWalletId = FromWa.Id inner join WalletMasters ToWa on rs.ToWalletId = ToWa.Id  where rs.UserID ={0}", UserId).ToList();

                int Converts = items.Count();
                int EmailInvite = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 1 && x.UserId == UserId).Count();
                int SMSInvite = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 2 && x.UserId == UserId).Count();
                int FacebookShare = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 3 && x.UserId == UserId).Count();
                int TwitterShare = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 4 && x.UserId == UserId).Count();
                int LinkedIn = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 5 && x.UserId == UserId).Count();
                int GoogleShare = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 6 && x.UserId == UserId).Count();
                int InstaShare = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 7 && x.UserId == UserId).Count();
                int Pinterest = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 8 && x.UserId == UserId).Count();
                int Telegram = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 9 && x.UserId == UserId).Count();
                int WhatsApp = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 10 && x.UserId == UserId).Count();
                int Messenger = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 11 && x.UserId == UserId).Count();
                int QRCode = _ReferralChannelRepository.Table.Where(x => x.ReferralChannelTypeId == 12 && x.UserId == UserId).Count();

                ReferralChannelAllCountViewModel obj = new ReferralChannelAllCountViewModel
                {
                    TotalParticipants = Participant,
                    Invite = Invite,
                    Clicks = Clicks,
                    Converts = Converts,
                    EmailInvite = EmailInvite,
                    SMSInvite = SMSInvite,
                    FacebookShare = FacebookShare,
                    TwitterShare = TwitterShare,
                    LinkedIn = LinkedIn,
                    GoogleShare = GoogleShare,
                    InstaShare = InstaShare,
                    Pinterest = Pinterest,
                    Telegram = Telegram,
                    WhatsApp = WhatsApp,
                    Messenger = Messenger,
                    QRCode = QRCode
                };
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ReferralLimitCount GetUserReferralChannelLimit(int channelTypeId, long UserId)
        {
            try
            {
                ReferralLimitCount response = new ReferralLimitCount();
                response = _dbContext.ReferralLimitCount.FromSql(@"Select (SELECT Count(Id) From ReferralChannel
                            where CreatedDate >=  DATEADD(hh, DATEPART(hh, GETUTCDATE()), DATEADD(dd, 0, DATEDIFF(dd, 0, GETUTCDATE())))
                            And ReferralChannelTypeId = {0} And UserId = {1}) As HourlyCount,
                            (SELECT Count(Id) From ReferralChannel where CreatedDate >= cast(GETUTCDATE() as Date)
                            And ReferralChannelTypeId = {0} And UserId = {1}) As DailyCount,
                            (select Count(Id) from ReferralChannel where DATEDIFF(Day, CreatedDate, GETUTCDATE()) <= 7 
                            And ReferralChannelTypeId = {0} And UserId = {1}) As WeeklyCount, 
                            (select Count(Id) from ReferralChannel where CreatedDate between DATEADD(mm, DATEDIFF(mm, 0, GETUTCDATE()), 0)  And DATEADD(dd, -1, DATEADD(mm, DATEDIFF(mm, 0, GETUTCDATE()) + 1, 0))
                            And ReferralChannelTypeId = {0} And UserId = {1}) As MonthlyCount", channelTypeId, UserId).FirstOrDefault();
                //Select(SELECT Count(Id) From ReferralChannel RCS
                //where CreatedDate >= DATEADD(hh, DATEPART(hh, GETUTCDATE()), DATEADD(dd, 0, DATEDIFF(dd, 0, GETUTCDATE())))
                //And ReferralChannelTypeId = 2 And UserId = 402) As HourlyCount,
                //(SELECT Count(Id) From ReferralChannel where CreatedDate >= cast(GETUTCDATE() as Date)
                // And ReferralChannelTypeId = 2 And UserId = 402) As DailyCount,
                // (select Count(Id) from ReferralChannel where DATEDIFF(Day, CreatedDate, GETUTCDATE()) <= 7
                //And ReferralChannelTypeId = 1 And UserId = 402) As WeeklyCount,
                //(select Count(Id) from ReferralChannel where CreatedDate between DATEADD(mm, DATEDIFF(mm, 0, GETUTCDATE()), 0)  And DATEADD(dd, -1, DATEADD(mm, DATEDIFF(mm, 0, GETUTCDATE()) + 1, 0))
                //And ReferralChannelTypeId = 1 And UserId = 402) As MonthlyCount,
                //(select Count(Id) from ReferralChannel where CreatedDate >= DATEADD(dd, DATEPART(dd, dbo.GetISTDate()), DATEADD(dd, 0, DATEDIFF(dd, -Day(DATEADD(DD, -1, DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 1, 0))), dbo.GetISTDate())))
                //And ReferralChannelTypeId = 1 And UserId = 402) As MonthlyCount3,
                //(select Count(Id) from ReferralChannel where DATEDIFF(Day, CreatedDate, getdate()) <= DAY(DATEADD(DD, -1, DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 1, 0)))
                //And ReferralChannelTypeId = 1 And UserId = 402) As MonthlyCount2,
                //(select Count(Id) from ReferralChannel where CreatedDate >= DATEADD(dd, DATEPART(dd, dbo.GetISTDate()), DATEADD(dd, 0, DATEDIFF(dd, -7, dbo.GetISTDate())))
                //And ReferralChannelTypeId = 1 And UserId = 402) As WeeklyCount2
                return response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public int SendReferralEmail(SendReferralEmailRequest Request, ApplicationUser User)
        {
            try
            {
                var UserData = _dbContext.Users.Where(x => x.Id == User.Id).FirstOrDefault();

                if (UserData != null)
                {
                    //var PromotionLink = _affiliatePromotionUserTypeMappingRepository.FindBy(x => x.PromotionTypeId == 1 && x.AffiliateUserId == AffiliateUser.Id).FirstOrDefault();
                    //if (PromotionLink != null)
                    //{
                    //ReferralChannelType ReferralLimit = (ReferralChannelType)_ReferralChannelTypeRepository.Table.Where(x => x.Id == Request.ReferralChannelTypeId);
                    var ReferralLimit = _ReferralChannelTypeRepository.Table.Where(x => x.Id == Request.ReferralChannelTypeId).FirstOrDefault();
                    //for (int i = 0; i < Request.EmailList.Count(); i++)
                    //{
                    var SendLimit = GetUserReferralChannelLimit(Request.ReferralChannelTypeId, User.Id);

                    if (SendLimit.HourlyCount >= ReferralLimit.HourlyLimit)
                    {
                        return 4;
                    }
                    else if (SendLimit.DailyCount >= ReferralLimit.DailyLimit)
                    {
                        return 5;
                    }
                    else if (SendLimit.WeeklyCount >= ReferralLimit.WeeklyLimit)
                    {
                        return 6;
                    }
                    else if (SendLimit.MonthlyCount >= ReferralLimit.MonthlyLimit)
                    {
                        return 7;
                    }

                    SendEmailRequest EmailRequest = new SendEmailRequest();
                    TemplateMasterData EmailData = new TemplateMasterData();
                    CommunicationParamater communicationParamater = new CommunicationParamater();
                    communicationParamater.Param1 = User.FirstName + " " + User.LastName;
                    communicationParamater.Param2 = Request.EmailShareURL.ToString();// Add here short link for send email

                    if (!string.IsNullOrEmpty(User.Email))
                    {
                        //communicationParamater.Param1 = User.UserName.ToLower() + "";

                        EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.Email_Referral, communicationParamater, enCommunicationServiceType.Email).Result;
                        if (EmailData != null)
                        {
                            EmailRequest.Body = EmailData.Content;
                            EmailRequest.Subject = EmailData.AdditionalInfo;
                            EmailRequest.Recepient = Request.EmailAddress.ToString();
                            EmailRequest.EmailType = 0;
                            _pushNotificationsQueue.Enqueue(EmailRequest);

                            ReferralChannel ObjReferralChannel = new ReferralChannel()
                            {
                                UserId = User.Id,
                                ReferralChannelTypeId = Request.ReferralChannelTypeId,
                                ReferralChannelServiceId = 1,
                                ReferralReceiverAddress = Request.EmailAddress.ToString(),
                                CreatedBy = User.Id,
                                CreatedDate = DateTime.UtcNow,
                                Status = 1
                            };
                            _ReferralChannelRepository.Insert(ObjReferralChannel);
                        }
                    }
                    // }
                    //}
                    //else
                    //{
                    //    return 2;
                    //}
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

        public int SendReferralSMS(SendReferralSMSRequest Request, ApplicationUser User)
        {
            try
            {
                var UserData = _dbContext.Users.Where(x => x.Id == User.Id).FirstOrDefault();

                if (UserData != null)
                {

                    // ReferralChannelType ReferralLimit = (ReferralChannelType)_ReferralChannelTypeRepository.Table.Where(x => x.Id == Request.ReferralChannelTypeId);
                    var ReferralLimit = _ReferralChannelTypeRepository.Table.Where(x => x.Id == Request.ReferralChannelTypeId).FirstOrDefault();
                    //for (int i = 0; i < Request.MobileNumberList.Count(); i++)
                    //{
                    var SendLimit = GetUserReferralChannelLimit(Request.ReferralChannelTypeId, User.Id);

                    if (SendLimit.HourlyCount >= ReferralLimit.HourlyLimit)
                    {
                        return 4;
                    }
                    else if (SendLimit.DailyCount >= ReferralLimit.DailyLimit)
                    {
                        return 5;
                    }
                    else if (SendLimit.WeeklyCount >= ReferralLimit.WeeklyLimit)
                    {
                        return 6;
                    }
                    else if (SendLimit.MonthlyCount >= ReferralLimit.MonthlyLimit)
                    {
                        return 7;
                    }

                    SendSMSRequest SendSMSRequestObj = new SendSMSRequest();
                    TemplateMasterData SmsData = new TemplateMasterData();

                    CommunicationParamater communicationParamater = new CommunicationParamater();
                    communicationParamater.Param1 = User.FirstName + " " + User.LastName;
                    communicationParamater.Param2 = GenerateShortifyBitlyLink(Request.SMSShareURL.ToString()); // Add here short link for send sms

                    SmsData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SMS_Referral, communicationParamater, enCommunicationServiceType.SMS).Result;

                    if (SmsData != null)
                    {
                        if (SmsData.IsOnOff == 1)
                        {
                            SendSMSRequestObj.Message = SmsData.Content;
                            SendSMSRequestObj.MobileNo = Convert.ToInt64(Request.MobileNumber);
                            _pushSMSQueue.Enqueue(SendSMSRequestObj);

                            ReferralChannel ObjReferralChannel = new ReferralChannel()
                            {
                                UserId = User.Id,
                                ReferralChannelTypeId = Request.ReferralChannelTypeId,
                                ReferralChannelServiceId = 2,
                                ReferralReceiverAddress = Request.MobileNumber.ToString(),
                                CreatedBy = User.Id,
                                CreatedDate = DateTime.UtcNow,
                                Status = 1
                            };
                            _ReferralChannelRepository.Insert(ObjReferralChannel);
                        }
                    }
                    //}
                    //}
                    //else
                    //{
                    //    return 3;
                    //}
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

        //public string AddReferralUserClick(AddReferralClickRequest Request)
        //{
        //    try
        //    {
        //        byte[] DecpasswordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());
        //        var bytes = Convert.FromBase64String(Request.PassURL);
        //        var encodedString = Encoding.UTF8.GetString(bytes);
        //        string DecryptToken = EncyptedDecrypted.Decrypt(encodedString, DecpasswordBytes);

        //        ReferralChannelShareViewModel dmodel = JsonConvert.DeserializeObject<ReferralChannelShareViewModel>(DecryptToken);

        //        if (dmodel != null)
        //        {
        //            var ReferralData = _dbContext.Users.Where(i => i.ReferralCode == dmodel.ReferralCode).FirstOrDefault();
        //            int UserId = ReferralData == null ? 1 : ReferralData.Id;
        //            var ReferralLink = _ReferralUserClickRepository.Table.Where(x => x.UserId == UserId && x.IPAddress == Request.IPAddress && x.ReferralServiceId == dmodel.ServiceId && x.ReferralChannelTypeId == dmodel.ChannelTypeId).FirstOrDefault();

        //            if (ReferralLink == null)
        //            {
        //                ReferralUserClick _AffiliateLinkObj = new ReferralUserClick()
        //                {
        //                    IPAddress = Request.IPAddress,
        //                    UserId = UserId,
        //                    ReferralChannelTypeId = dmodel.ChannelTypeId,
        //                    ReferralServiceId = dmodel.ServiceId,
        //                    Status = 1,
        //                    CreatedBy = UserId,
        //                    CreatedDate = DateTime.UtcNow
        //                };
        //                _ReferralUserClickRepository.Insert(_AffiliateLinkObj);
        //                return dmodel.ReferralCode;
        //            }
        //            else
        //            {
        //                return dmodel.ReferralCode;
        //            }
        //        }
        //        return string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}

        public ReferralUserClickViewModel AddReferralUserClick(AddReferralClickRequest Request)
        {
            try
            {
                byte[] DecpasswordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());
                string DecryptToken = "";
                ReferralChannelShareViewModel dmodel = new ReferralChannelShareViewModel();
                //Set Try and Catch Block By Rushali (26-12-2019)
                try
                {
                    var bytes = Convert.FromBase64String(Request.PassURL);
                    var encodedString = Encoding.UTF8.GetString(bytes);
                    DecryptToken = EncyptedDecrypted.Decrypt(encodedString, DecpasswordBytes);
                }
                catch (Exception e)
                {
                    return null;
                }

                ReferralUserClickViewModel obj = new ReferralUserClickViewModel();
                if (dmodel != null)
                {
                    var ReferralData = _dbContext.Users.Where(i => i.ReferralCode == dmodel.ReferralCode).FirstOrDefault();
                    int UserId = ReferralData == null ? 1 : ReferralData.Id;
                    var ReferralLink = _ReferralUserClickRepository.Table.Where(x => x.UserId == UserId && x.IPAddress == Request.IPAddress && x.ReferralServiceId == dmodel.ServiceId && x.ReferralChannelTypeId == dmodel.ChannelTypeId).FirstOrDefault();
                   // Add Referral Code Null Condition By Rushali (24-12-2019)
                    if (ReferralData.ReferralCode == null)
                    {
                        return null;
                    }
                    if (ReferralLink == null)
                    {
                        ReferralUserClick _AffiliateLinkObj = new ReferralUserClick()
                        {
                            IPAddress = Request.IPAddress,
                            UserId = UserId,
                            ReferralChannelTypeId = dmodel.ChannelTypeId,
                            ReferralServiceId = dmodel.ServiceId,
                            Status = 1,
                            CreatedBy = UserId,
                            CreatedDate = DateTime.UtcNow
                        };
                        _ReferralUserClickRepository.Insert(_AffiliateLinkObj);
                        obj.ReferralCode = dmodel.ReferralCode;
                        obj.ReferralServiceId = dmodel.ServiceId;
                        obj.ReferralChannelTypeId = dmodel.ChannelTypeId;
                        return obj;
                    }
                    else
                    {
                        //obj.ReferralCode = dmodel.ReferralCode;
                        //obj.ReferralServiceId = dmodel.ServiceId;
                        //obj.ReferralChannelTypeId = dmodel.ChannelTypeId;
                        return null;
                    }
                }
                return null;
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

        public ReferralChannelShareURLViewModel GetReferralURL(ReferralChannelShareViewModel model, long UserId)
        {
            try
            {
                byte[] passwordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());

                ReferralChannelShareViewModel objShare = new ReferralChannelShareViewModel();
                objShare.ReferralCode = model.ReferralCode;
                objShare.ServiceId = model.ServiceId;

                //For Email
                objShare.ChannelTypeId = 1;
                string TokenDetailEmail = JsonConvert.SerializeObject(objShare);
                string SubScriptionKeyEmail = EncyptedDecrypted.Encrypt(TokenDetailEmail, passwordBytes);
                byte[] EmailplainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKeyEmail);

                //For SMS
                objShare.ChannelTypeId = 2;
                string TokenDetailSMS = JsonConvert.SerializeObject(objShare);
                string SubScriptionKeySMS = EncyptedDecrypted.Encrypt(TokenDetailSMS, passwordBytes);
                byte[] SMSplainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKeySMS);


                //For Facebook
                objShare.ChannelTypeId = 3;
                string TokenDetailFacebook = JsonConvert.SerializeObject(objShare);
                string SubScriptionKeyFacebook = EncyptedDecrypted.Encrypt(TokenDetailFacebook, passwordBytes);
                byte[] FacebookplainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKeyFacebook);

                //For Twitter
                objShare.ChannelTypeId = 4;
                string TokenDetailTwitter = JsonConvert.SerializeObject(objShare);
                string SubScriptionKeyTwitter = EncyptedDecrypted.Encrypt(TokenDetailTwitter, passwordBytes);
                byte[] TwitterplainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKeyTwitter);

                //For LinkedIn
                objShare.ChannelTypeId = 5;
                string TokenDetailLinkedIn = JsonConvert.SerializeObject(objShare);
                string SubScriptionKeyLinkedIn = EncyptedDecrypted.Encrypt(TokenDetailLinkedIn, passwordBytes);
                byte[] LinkedInplainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKeyLinkedIn);

                //For Google
                objShare.ChannelTypeId = 6;
                string TokenDetailGoogle = JsonConvert.SerializeObject(objShare);
                string SubScriptionKeyGoogle = EncyptedDecrypted.Encrypt(TokenDetailGoogle, passwordBytes);
                byte[] GoogleInplainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKeyGoogle);

                //For Insta
                objShare.ChannelTypeId = 7;
                string TokenDetailInsta = JsonConvert.SerializeObject(objShare);
                string SubScriptionKeyInsta = EncyptedDecrypted.Encrypt(TokenDetailInsta, passwordBytes);
                byte[] InstaInplainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKeyInsta);

                //For Pintrest
                objShare.ChannelTypeId = 8;
                string TokenDetailPintrest = JsonConvert.SerializeObject(objShare);
                string SubScriptionKeyPintrest = EncyptedDecrypted.Encrypt(TokenDetailPintrest, passwordBytes);
                byte[] PintrestInplainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKeyPintrest);

                //For Telegram
                objShare.ChannelTypeId = 9;
                string TokenDetailTelegram = JsonConvert.SerializeObject(objShare);
                string SubScriptionKeyTelegram = EncyptedDecrypted.Encrypt(TokenDetailTelegram, passwordBytes);
                byte[] TelegramInplainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKeyTelegram);

                // For WhatsApp
                objShare.ChannelTypeId = 10;
                string TokenDetailWhatsApp = JsonConvert.SerializeObject(objShare);
                string SubScriptionKeyWhatsApp = EncyptedDecrypted.Encrypt(TokenDetailWhatsApp, passwordBytes);
                byte[] WhatsAppInplainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKeyWhatsApp);

                //For Messenger
                objShare.ChannelTypeId = 11;
                string TokenDetailMessenger = JsonConvert.SerializeObject(objShare);
                string SubScriptionKeyMessenger = EncyptedDecrypted.Encrypt(TokenDetailMessenger, passwordBytes);
                byte[] MessengerInplainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKeyMessenger);

                //For QRCode
                objShare.ChannelTypeId = 12;
                string TokenDetailQRCode = JsonConvert.SerializeObject(objShare);
                string SubScriptionKeyQRCode = EncyptedDecrypted.Encrypt(TokenDetailQRCode, passwordBytes);
                byte[] QRCodeInplainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKeyQRCode);

                ReferralChannelShareURLViewModel objShareURL = new ReferralChannelShareURLViewModel();
                objShareURL.EmailURL = Convert.ToBase64String(EmailplainTextBytes);
                objShareURL.SMSURL = Convert.ToBase64String(SMSplainTextBytes);
                objShareURL.FacebookURL = Convert.ToBase64String(FacebookplainTextBytes);
                objShareURL.TwitterURL = Convert.ToBase64String(TwitterplainTextBytes);
                objShareURL.LinkedInURL = Convert.ToBase64String(LinkedInplainTextBytes);
                objShareURL.GoogleURL = Convert.ToBase64String(GoogleInplainTextBytes);
                objShareURL.InstaURL = Convert.ToBase64String(InstaInplainTextBytes);
                objShareURL.PintrestURL = Convert.ToBase64String(PintrestInplainTextBytes);
                objShareURL.TelegramURL = Convert.ToBase64String(TelegramInplainTextBytes);
                objShareURL.WhatsAppURL = Convert.ToBase64String(WhatsAppInplainTextBytes);
                objShareURL.MessengerURL = Convert.ToBase64String(MessengerInplainTextBytes);
                objShareURL.QRCodeURL = Convert.ToBase64String(QRCodeInplainTextBytes);

                return objShareURL;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

    }
}
