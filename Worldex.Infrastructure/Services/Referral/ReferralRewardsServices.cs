using System;
using System.Collections.Generic;
using Worldex.Core.ViewModels.Referral;
using Worldex.Core.Entities.Referral;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.Interfaces.Referral;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Worldex.Infrastructure.Services.Referral
{
    public class ReferralRewardsServices : IReferralRewards
    {
        private readonly WorldexContext _dbContext;

        private readonly ICustomRepository<ReferralRewards> _ReferralRewardsRepository;

        public ReferralRewardsServices(ICustomRepository<ReferralRewards> ReferralRewardsRepository, WorldexContext dbContext)
        {
            _dbContext = dbContext;
            _ReferralRewardsRepository = ReferralRewardsRepository;
        }
        
        public long AddReferralRewards(ReferralRewardsViewModel ReferralRewardsInsert,long UserId)
        {
            try
            {
                ReferralRewards ObjReferralRewards = new ReferralRewards()
                {
                    UserId = ReferralRewardsInsert.UserId,
                    ReferralServiceId = ReferralRewardsInsert.ReferralServiceId,
                    ReferralPayRewards = ReferralRewardsInsert.ReferralPayRewards,
                    LifeTimeUserCount= ReferralRewardsInsert.LifeTimeUserCount,
                    NewUserCount = ReferralRewardsInsert.NewUserCount,
                    CommissionCurrecyId = ReferralRewardsInsert.CommissionCurrecyId,
                    CommissionCroneID = ReferralRewardsInsert.CommissionCroneID,
                    ReferralPayTypeId = ReferralRewardsInsert.ReferralPayTypeId,
                    SumChargeAmount = ReferralRewardsInsert.SumChargeAmount,
                    TransactionCurrencyId = ReferralRewardsInsert.TransactionCurrencyId,
                    SumOfTransaction = ReferralRewardsInsert.SumOfTransaction,
                    TrnUserId = ReferralRewardsInsert.TrnUserId,
                    FromWalletId = ReferralRewardsInsert.FromWalletId,
                    ToWalletId = ReferralRewardsInsert.ToWalletId,
                    TrnRefNo = ReferralRewardsInsert.TrnRefNo,
                    CommissionAmount = ReferralRewardsInsert.CommissionAmount,
                    TransactionAmount = ReferralRewardsInsert.TransactionAmount,
                    TrnDate = ReferralRewardsInsert.TrnDate,
                    CreatedBy = UserId,
                    CreatedDate = DateTime.UtcNow,
                    Status = 1
                };
                _ReferralRewardsRepository.Insert(ObjReferralRewards);
                return ObjReferralRewards.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ReferralRewardsListResponse ListAdminReferralRewards(int PageIndex = 0, int Page_Size = 0, long ReferralServiceId = 0, int UserId = 0,int TrnUserId=0, DateTime? FromDate = null, DateTime? ToDate = null)
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

                var items = (from rs in _dbContext.ReferralRewards
                             join st in _dbContext.ReferralService on rs.ReferralServiceId equals st.Id
                             join us in _dbContext.Users on rs.UserId equals us.Id
                             join pt in _dbContext.ReferralPayType on st.ReferralPayTypeId equals pt.Id
                             join cu in _dbContext.WalletTypeMasters on st.CurrencyId equals cu.Id
                             join ComCurr in _dbContext.WalletTypeMasters on rs.CommissionCurrecyId equals ComCurr.Id
                             join TrnCurr in _dbContext.WalletTypeMasters on rs.TransactionCurrencyId equals TrnCurr.Id
                             join Trnus in _dbContext.Users on rs.TrnUserId equals Trnus.Id
                             join FromWa in _dbContext.WalletMasters on rs.FromWalletId equals FromWa.Id
                             join ToWa in _dbContext.WalletMasters on rs.ToWalletId equals ToWa.Id
                             select new ReferralRewardsListViewModel
                             {
                                 Id = rs.Id,
                                 UserId = rs.UserId,
                                 UserName = us.UserName,                                                            
                                 ReferralServiceId = rs.ReferralServiceId,
                                 ReferralServiceDescription = st.Description,
                                 ReferralPayRewards=rs.ReferralPayRewards,
                                 ReferralPayTypeId=pt.Id,
                                 ReferralPayTypeName=pt.PayTypeName,
                                 CurrencyId=cu.Id,
                                 CurrencyName=cu.WalletTypeName,
                                 CreatedDate = rs.CreatedDate,
                                 LifeTimeUserCount= rs.LifeTimeUserCount,
                                 NewUserCount = rs.LifeTimeUserCount,
                                 CommissionCurrecyName = ComCurr.WalletTypeName,
                                 SumChargeAmount =rs.SumChargeAmount,
                                 TransactionCurrecyName= TrnCurr.WalletTypeName,
                                 SumOfTransaction =rs.SumOfTransaction,
                                 TrnUserId=rs.TrnUserId,
                                 TrnUserName= Trnus.UserName,
                                 FromWalletName = FromWa.Walletname,
                                 ToWalletName = ToWa.Walletname,
                                 TrnRefNo = rs.TrnRefNo,
                                 CommissionAmount = rs.CommissionAmount,
                                 TransactionAmount =rs.TransactionAmount
                             }
                            ).ToList();

                if (ReferralServiceId != 0)
                {
                    items = items.Where(x => x.ReferralServiceId == ReferralServiceId).ToList();
                }                
                if (UserId != 0)
                {
                    items = items.Where(x => x.UserId == UserId).ToList();
                }
                if (TrnUserId != 0)
                {
                    items = items.Where(x => x.TrnUserId == TrnUserId).ToList();
                }
                if (FromDate != null && ToDate != null)
                {
                    items = items.Where(x => x.CreatedDate.Date >= FromDate && x.CreatedDate.Date <= ToDate).ToList();
                }

                int TotalCount = items.Count();

                ReferralRewardsListResponse obj = new ReferralRewardsListResponse();
                obj.TotalCount = TotalCount;
                obj.ReferralRewardsList = items.OrderByDescending(x => x.Id).Skip(skip).Take(Page_Size).ToList();
                return obj;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }

        public ReferralRewardsListResponse ListUserReferralRewards(int UserId, int PageIndex = 0, int Page_Size = 0, long ReferralServiceId = 0,int TrnUserId=0, DateTime? FromDate = null, DateTime? ToDate = null)
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
                //UserId = 95618;
                // var items = _dbContext.ReferralRewardsListViewModel.FromSql("select rs.Id as Id, rs.UserId as UserId, us.UserName as UserName, rs.ReferralServiceId as ReferralServiceId, ms.Description as ReferralServiceDescription, rs.ReferralPayRewards as ReferralPayRewards, pt.Id as ReferralPayTypeId, pt.PayTypeName as ReferralPayTypeName, cu.Id as CurrencyId, cu.WalletTypeName as CurrencyName, rs.CreatedDate as CreatedDate, rs.LifeTimeUserCount as LifeTimeUserCount, rs.LifeTimeUserCount as NewUserCount, ComCurr.WalletTypeName as CommissionCurrecyName, rs.CommissionCurrecyId as CommissionCurrecyId, rs.SumChargeAmount as SumChargeAmount, rs.TransactionCurrencyId as TransactionCurrencyId, ISNULL(TrnCurr.WalletTypeName,'') as TransactionCurrecyName, rs.SumOfTransaction as SumOfTransaction, rs.TrnUserId as TrnUserId, Trnus.UserName as TrnUserName, FromWa.Walletname as FromWalletName, ToWa.Walletname as ToWalletName, rs.TrnRefNo as TrnRefNo, rs.CommissionAmount as CommissionAmount, rs.TransactionAmount as TransactionAmount, cast(0 as bigint) as CommissionCroneID, cast('' as varchar(50)) as CommissionCroneRemarks, rs.FromWalletId as FromWalletId, rs.ToWalletId as ToWalletId, rs.TrnDate as TrnDate  From ReferralRewards rs inner join ReferralServiceDetail st on rs.ReferralServiceId = st.Id inner join ReferralSchemeTypeMapping m on m.id = st.SchemeTypeMappingId inner join Referralservice ms on ms.id = m.ServiceTypeMstId inner join BizUser us on rs.UserID = us.ID inner join ReferralPayType pt on rs.ReferralPayTypeId = pt.ID inner join WalletTypeMasters cu on st.CreditWalletTypeId = cu.Id inner join WalletTypeMasters ComCurr on rs.CommissionCurrecyId = ComCurr.Id left join WalletTypeMasters TrnCurr on rs.TransactionCurrencyId = TrnCurr.Id inner join Bizuser Trnus on rs.TrnUserId = Trnus.Id inner join WalletMasters FromWa on rs.FromWalletId = FromWa.Id inner join WalletMasters ToWa on rs.ToWalletId = ToWa.Id where rs.UserID = {0}", 95618).ToList();
                var items = _dbContext.ReferralRewardsListViewModel.FromSql("select rs.Id as Id, rs.UserId as UserId, us.UserName as UserName, rs.ReferralServiceId as ReferralServiceId, ms.Description as ReferralServiceDescription, rs.ReferralPayRewards as ReferralPayRewards, pt.Id as ReferralPayTypeId, pt.PayTypeName as ReferralPayTypeName, cu.Id as CurrencyId, cu.WalletTypeName as CurrencyName, rs.CreatedDate as CreatedDate, rs.LifeTimeUserCount as LifeTimeUserCount, rs.LifeTimeUserCount as NewUserCount, ComCurr.WalletTypeName as CommissionCurrecyName, rs.CommissionCurrecyId as CommissionCurrecyId, rs.SumChargeAmount as SumChargeAmount, rs.TransactionCurrencyId as TransactionCurrencyId, ISNULL(TrnCurr.WalletTypeName,'') as TransactionCurrecyName, rs.SumOfTransaction as SumOfTransaction, rs.TrnUserId as TrnUserId, Trnus.UserName as TrnUserName, FromWa.Walletname as FromWalletName, ToWa.Walletname as ToWalletName, rs.TrnRefNo as TrnRefNo, rs.CommissionAmount as CommissionAmount, rs.TransactionAmount as TransactionAmount, cast(0 as bigint) as CommissionCroneID, cast('' as varchar(50)) as CommissionCroneRemarks, rs.FromWalletId as FromWalletId, rs.ToWalletId as ToWalletId, rs.TrnDate as TrnDate  From ReferralRewards rs inner join ReferralServiceDetail st on rs.ReferralServiceId = st.Id  inner join ReferralSchemeTypeMapping m on m.id=st.SchemeTypeMappingId inner join Referralservice ms on ms.id=m.ServiceTypeMstId inner join BizUser us on rs.UserID = us.ID inner join ReferralPayType pt on rs.ReferralPayTypeId = pt.ID inner join WalletTypeMasters cu on st.CreditWalletTypeId = cu.Id inner join WalletTypeMasters ComCurr on rs.CommissionCurrecyId = ComCurr.Id  left join WalletTypeMasters TrnCurr on rs.TransactionCurrencyId = TrnCurr.Id  inner join Bizuser Trnus on rs.TrnUserId = Trnus.Id inner join WalletMasters FromWa on rs.FromWalletId = FromWa.Id inner join WalletMasters ToWa on rs.ToWalletId = ToWa.Id  where rs.UserID ={0}", UserId).ToList();

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
                //                 ReferralPayRewards=rs.ReferralPayRewards,
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


                if (ReferralServiceId != 0)
                {
                    items = items.Where(x => x.ReferralServiceId == ReferralServiceId).ToList();
                }
                if (TrnUserId != 0)
                {
                    items = items.Where(x => x.TrnUserId == TrnUserId).ToList();
                }
                if (FromDate != null && ToDate != null)
                {
                    items = items.Where(x => x.CreatedDate.Date >= FromDate && x.CreatedDate.Date <= ToDate).ToList();
                }

                int TotalCount = items.Count();
                ReferralRewardsListResponse obj = new ReferralRewardsListResponse();
                obj.TotalCount = TotalCount;
                obj.ReferralRewardsList = items.OrderByDescending(x => x.Id).Skip(skip).Take(Page_Size).ToList();
                return obj;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }

    }
}
