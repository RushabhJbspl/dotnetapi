using Worldex.Core.ApiModels;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Wallet
{
    public class ListAddWalletRequest : BizResponseClass
    {
        public List<AddWalletRequestRes> Data { get; set; }
    }

    public class AddWalletRequestRes
    {
        public long RequestId { get; set; }
        public string WalletName { get; set; }
        public string WalletType { get; set; }
        public string StrStatus { get; set; }
        public short Status { get; set; }
        public string RoleName { get; set; }
        public string ToEmail { get; set; }
        public string FromEmail { get; set; }
        public string Message { get; set; }
        public short OwnerApprovalStatus { get; set; }
        public string StrOwnerApprovalStatus { get; set; }
        public string RequestType { get; set; }
    }

    public class UserWalletWise
    {
        public string RoleName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string WalletName { get; set; }
        public string WalletType { get; set; }
        public long RoleID { get; set; }
    }

    public class ListUserWalletWise :BizResponseClass
    {
        public List<UserWalletWise> Data { get; set; }
    }
}
