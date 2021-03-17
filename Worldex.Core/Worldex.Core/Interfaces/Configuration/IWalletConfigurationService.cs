using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.WalletConfiguration;
using Worldex.Core.ViewModels.WalletOperations;

namespace Worldex.Core.Interfaces.Configuration
{
    public interface IWalletConfigurationService
    {
        //vsolanki 11-10-2018
        #region wallettypemaster
        ListWalletTypeMasterResponse ListAllWalletTypeMaster();

        WalletTypeMasterResponse AddWalletTypeMaster(WalletTypeMasterRequest addWalletTypeMasterRequest, long Userid);

        WalletTypeMasterResponse UpdateWalletTypeMaster(WalletTypeMasterUpdateRequest updateWalletTypeMasterRequest, long Userid, long WalletTypeId);

        BizResponseClass DisableWalletTypeMaster(long WalletTypeId);

        WalletTypeMasterResponse GetWalletTypeMasterById(long WalletTypeId);
        #endregion

        #region Other Method

        TransferInOutRes GetTransferIn(string Coin, int Page, int PageSize, long? UserId, string Address, string TrnID, long? OrgId);

        TransferInOutRes GetTransferOutHistory(string coinName,int Page, int PageSize, long? UserId, string Address, string TrnID, long? OrgId);

        #endregion
    }
}
