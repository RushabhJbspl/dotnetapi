using Worldex.Core.ViewModels.Fiat_Bank_Integration;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.Entities.User;
using Worldex.Core.ViewModels.FiatBankIntegration;
using Worldex.Core.ViewModels.Wallet;

namespace Worldex.Infrastructure.Interfaces
{
    public interface IFiatIntegration
    {
        Task<BizResponseClass> AddUserBankDetail(AddBankDetailReq Req,long UserId);
        Task<ListUserBankReq> ListUserBankDetail(short? Status,short? RequestType, long UserId);
        Task<BizResponseClass> AcceptRejectUserBankRequest(AdminApprovalReq req, long UserId);
        Task<BizResponseClass> AddUpdateFiatTradeConfiguration(FiatTradeConfigurationReq req, int id);
        Task<ListFiatTradeConfigurationRes> GetFiatTradeConfiguration(short? status);

        GetBankDetail GetUserbankDetails(long UserId);
      ///  BizResponseClass InsertUpdateFiatConfiguration(FiatCoinConfigurationReq Req, long UserId);
        BizResponseClass InsertUpdateFiatCurrency(FiatCurrencyConfigurationReq Req, long UserId);
        InsertUpdateCoinRes InsertUpdateFiatConfiguration(ListFiatCoinConfigurationReq Req, long UserId);
        ListFiatCoinConfigurationRes ListFiatConfiguration(long? FromCurrencyId, long? ToCurrencyId, short? Status, short? TransactionType);
      ///  SellResponse FiatSellRequestConfirmation(FiatSellConfirmReq Req, ApplicationUser user);
    }
}
